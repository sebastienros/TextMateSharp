using System.Diagnostics;

namespace TextMateSharp.Fuzz;

public sealed class Options
{
    public string StateDirectory { get; set; } = "fuzz-state";
    public string? Repository { get; set; }
    public ulong? Seed { get; set; }
    public int Workers { get; set; } = Math.Max(1, Environment.ProcessorCount - 1);
    public TimeSpan? Duration { get; set; }
    public string[]? Grammars { get; set; }
    public int HangSeconds { get; set; } = 60;
    public int LineTimeLimitMs { get; set; } = 5_000;
    public int StatusSeconds { get; set; } = 5;
    public int TableSeconds { get; set; } = 60;
    public int MinimizeBudget { get; set; } = 400;
    public int MinimizeSeconds { get; set; } = 60;
    public bool InvalidUtf16 { get; set; } = true;
    public long? Iterations { get; set; }
}

// A differential fuzzing campaign: each worker takes a grammar, generates its next inputs, tokenizes them with the
// branch and the official TextMateSharp, and records how they differ.
public sealed class Campaign
{
    public const int HangExitCode = 3;
    private const int MaxCorpus = 300;

    private readonly Options _options;
    private readonly string _state;
    private readonly Engine _branch;
    private readonly Engine _official;
    private readonly List<Target> _targets;
    private readonly CampaignState _campaign;
    private readonly Dictionary<string, Slot> _slots = new(StringComparer.Ordinal);
    private readonly Dictionary<string, Finding> _findings = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _rawSignatures = new(StringComparer.Ordinal);
    private readonly HashSet<string> _dirtyFindings = new(StringComparer.Ordinal);
    private readonly object _gate = new();
    private readonly object _persistGate = new();
    private readonly List<Worker> _workers = new();
    private readonly CancellationTokenSource _stop = new();
    private readonly Stopwatch _clock = Stopwatch.StartNew();
    private readonly double _previousSeconds;
    private long _execs;

    // A grammar's state, corpus and whether a worker holds it.
    private sealed class Slot
    {
        public required Target Target { get; init; }
        public required GrammarState State { get; init; }
        public required CorpusState Corpus { get; init; }
        public HashSet<string> Scopes { get; } = new(StringComparer.Ordinal);
        public HashSet<long> Skip { get; } = new();
        public bool Busy { get; set; }
        public bool Dirty { get; set; }
    }

    public Campaign(Options options, Engine branch, Engine official, List<Target> targets)
    {
        _options = options;
        _state = Path.GetFullPath(options.StateDirectory);
        _branch = branch;
        _official = official;
        CampaignState? existing = Store.Read<CampaignState>(Path.Combine(_state, "campaign.json"));
        if (existing != null && options.Seed != null && options.Seed != existing.Seed)
        {
            throw new InvalidOperationException($"{_state} holds a campaign with seed {existing.Seed}; pass that seed, no seed, or another --state.");
        }

        _campaign = existing ?? new CampaignState { Seed = options.Seed ?? (ulong)Random.Shared.NextInt64(), Created = DateTime.UtcNow };
        _campaign.BranchVersion = branch.Version;
        _campaign.OfficialVersion = official.Version;
        _previousSeconds = _campaign.Seconds;
        Resumed = existing != null;

        _targets = options.Grammars == null ? targets : targets.Where(t => options.Grammars.Contains(t.Scope) || options.Grammars.Contains(t.Language)).ToList();
        foreach (Target target in _targets)
        {
            if (!_campaign.Grammars.TryGetValue(target.Scope, out GrammarState? state))
            {
                state = new GrammarState();
                _campaign.Grammars[target.Scope] = state;
            }

            var slot = new Slot
            {
                Target = target,
                State = state,
                Corpus = Store.Read<CorpusState>(CorpusPath(target.Scope)) ?? new CorpusState(),
            };
            slot.Scopes.UnionWith(slot.Corpus.Scopes);
            slot.Skip.UnionWith(state.Skip);
            _slots[target.Scope] = slot;
        }

        foreach (string file in Directory.Exists(FindingsDirectory) ? Directory.EnumerateFiles(FindingsDirectory, "*.json") : [])
        {
            Finding finding = Store.Read<Finding>(file)!;
            _findings[finding.Signature] = finding;
            foreach (string raw in finding.RawSignatures)
            {
                _rawSignatures[raw] = finding.Signature;
            }
        }

        RecoverCrashes();
    }

    public bool Resumed { get; }

    public ulong Seed => _campaign.Seed;

    public int Grammars => _targets.Count;

    public string FindingsDirectory => Path.Combine(_state, "findings");

    private string InflightDirectory => Path.Combine(_state, "inflight");

    private string CorpusPath(string scope) => Path.Combine(_state, "corpus", Store.FileName(scope) + ".json");

    public void Stop() => _stop.Cancel();

    public bool Stopping => _stop.IsCancellationRequested;

    public void Run()
    {
        if (_options.Duration is TimeSpan duration)
        {
            _stop.CancelAfter(duration);
        }

        var threads = new List<Thread>();
        for (int i = 0; i < _options.Workers; i++)
        {
            var worker = new Worker(i, _branch.CreateSession(), _official.CreateSession(), Path.Combine(InflightDirectory, i + ".json"), _options.LineTimeLimitMs);
            _workers.Add(worker);
            var thread = new Thread(() => Work(worker), 16 * 1024 * 1024) { Name = "fuzz-" + i, IsBackground = true };
            threads.Add(thread);
            thread.Start();
        }

        var watchdog = new Thread(Watch) { IsBackground = true, Name = "watchdog" };
        watchdog.Start();

        DateTime nextStatus = DateTime.UtcNow.AddSeconds(_options.StatusSeconds);
        DateTime nextTable = DateTime.UtcNow.AddSeconds(_options.TableSeconds);
        DateTime nextPersist = DateTime.UtcNow.AddSeconds(60);
        long lastExecs = 0;
        DateTime lastTime = DateTime.UtcNow;
        while (threads.Any(t => t.IsAlive))
        {
            threads.First(t => t.IsAlive).Join(250);
            DateTime now = DateTime.UtcNow;
            if (now >= nextStatus)
            {
                long execs = Interlocked.Read(ref _execs);
                Console.WriteLine(StatusLine((execs - lastExecs) / (now - lastTime).TotalSeconds) + (Stopping ? " (stopping)" : ""));
                (lastExecs, lastTime) = (execs, now);
                nextStatus = now.AddSeconds(_options.StatusSeconds);
            }
            if (now >= nextTable)
            {
                Console.WriteLine(Table());
                nextTable = now.AddSeconds(_options.TableSeconds);
            }
            // A file named "stop" in the state directory stops the campaign like Ctrl+C, for remote or detached runs.
            if (!Stopping && File.Exists(Path.Combine(_state, "stop")))
            {
                File.Delete(Path.Combine(_state, "stop"));
                Console.WriteLine("stopping: found the stop file");
                Stop();
            }
            if (now >= nextPersist)
            {
                Persist();
                nextPersist = now.AddSeconds(60);
            }
        }

        foreach (Worker worker in _workers)
        {
            worker.Clear();
        }

        Persist();
        Console.WriteLine(Table());
        Console.WriteLine(StatusLine(Interlocked.Read(ref _execs) / Math.Max(_clock.Elapsed.TotalSeconds, 0.001)) + " (stopped; state saved, rerun the same command to resume)");
    }

    private void Work(Worker worker)
    {
        while (!Stopping)
        {
            Slot? slot;
            lock (_gate)
            {
                slot = _slots.Values.Where(s => !s.Busy && (_options.Iterations == null || s.State.Next < _options.Iterations)).MinBy(s => s.State.Next);
                if (slot == null)
                {
                    break;
                }
                slot.Busy = true;
            }

            try
            {
                var batch = Stopwatch.StartNew();
                for (int i = 0; i < 50 && batch.ElapsedMilliseconds < 1000 && !Stopping && (_options.Iterations == null || slot.State.Next < _options.Iterations); i++)
                {
                    try
                    {
                        Iterate(worker, slot);
                    }
                    catch (Exception e)
                    {
                        // A fuzzer bug, not a tokenizer one: report it and move past the iteration.
                        Console.WriteLine($"  fuzzer error in {slot.Target.Scope} at iteration {slot.State.Next}: {e}");
                        lock (slot)
                        {
                            slot.State.Next++;
                        }
                    }
                }
            }
            finally
            {
                lock (_gate)
                {
                    slot.Busy = false;
                }
            }
        }
    }

    private void Iterate(Worker worker, Slot slot)
    {
        long iteration;
        string[] lines;
        lock (slot)
        {
            iteration = slot.State.Next;
            if (slot.Skip.Contains(iteration))
            {
                slot.State.Next++;
                return;
            }
            lines = Generator.Generate(_campaign.Seed, slot.Target, iteration, slot.Corpus.Entries.Select(e => e.ToArray()).ToList(), _options.InvalidUtf16);
        }

        long start = Stopwatch.GetTimestamp();
        (string branch, string official) = worker.Exec(slot.Target.Scope, iteration, lines);
        Difference? difference = Comparison.Compare(slot.Target.Scope, lines, branch, official);
        Interlocked.Increment(ref _execs);
        if (difference != null)
        {
            Record(worker, slot, iteration, lines, difference, branch, official);
        }

        lock (slot)
        {
            foreach (string scope in Scopes(branch))
            {
                if (slot.Scopes.Add(scope))
                {
                    slot.Corpus.Scopes.Add(scope);
                    if (slot.Corpus.Entries.Count < MaxCorpus)
                    {
                        slot.Corpus.Entries.Add(Lines.From(lines));
                    }
                }
            }
            slot.State.Next = iteration + 1;
            slot.State.Execs++;
            slot.State.BusySeconds += Stopwatch.GetElapsedTime(start).TotalSeconds;
            slot.Dirty = true;
        }
    }

    // The scope names of every token the branch produced: new ones mean the input reached new grammar rules.
    private static IEnumerable<string> Scopes(string output)
    {
        foreach (string line in output.Split('\n'))
        {
            if (line.Length > 0 && char.IsDigit(line[0]))
            {
                foreach (string scope in line.Split(' ').Skip(1))
                {
                    yield return scope;
                }
            }
        }
    }

    private void Record(Worker worker, Slot slot, long iteration, string[] lines, Difference difference, string branch, string official)
    {
        bool crash = Comparison.IsCrash(difference.Kind);
        lock (slot)
        {
            slot.State.Discrepancies++;
            if (crash)
            {
                slot.State.Crashes++;
            }
        }

        lock (_gate)
        {
            if (_rawSignatures.TryGetValue(difference.Signature, out string? known))
            {
                Hit(_findings[known]);
                return;
            }
        }

        // A new kind of discrepancy: shrink the input while it shows the same kind, then group it by signature.
        string[] small = lines;
        bool minimized = false;
        string sb = branch;
        string so = official;
        Difference final = difference;
        if (!Comparison.IsTiming(difference.Kind))
        {
            small = Minimize(worker, slot.Target.Scope, iteration, lines, difference.Kind, out minimized);
            (sb, so) = worker.Exec(slot.Target.Scope, iteration, small);
            if (Comparison.Compare(slot.Target.Scope, small, sb, so) is Difference shrunk)
            {
                final = shrunk;
            }
            else
            {
                (small, sb, so) = (lines, branch, official);
            }
        }

        lock (_gate)
        {
            _rawSignatures[difference.Signature] = final.Signature;
            if (_findings.TryGetValue(final.Signature, out Finding? existing))
            {
                existing.RawSignatures.Add(difference.Signature);
                Hit(existing);
                return;
            }

            var finding = new Finding
            {
                Id = final.Kind + "-" + Store.FileName(slot.Target.Scope) + "-" + Comparison.Hash(final.Signature),
                Kind = final.Kind,
                Scope = slot.Target.Scope,
                Language = slot.Target.Language,
                Signature = final.Signature,
                RawSignatures = [difference.Signature],
                Hits = 1,
                Seed = _campaign.Seed,
                Iteration = iteration,
                FirstSeen = DateTime.UtcNow,
                Minimized = minimized,
                Input = Lines.From(small),
                Original = small.Length == lines.Length && small.SequenceEqual(lines) ? null : Lines.From(lines),
                Key = final.Key,
                Branch = Clip(final.Branch),
                Official = Clip(final.Official),
                BranchOutput = Clip(sb),
                OfficialOutput = Clip(so),
            };
            _findings[final.Signature] = finding;
            Store.Write(Path.Combine(FindingsDirectory, finding.Id + ".json"), finding);
            Console.WriteLine($"  new {finding.Kind} in {finding.Scope} ({small.Length} line(s), {small.Sum(l => l.Length)} chars): {finding.Id}");
        }
    }

    // The full outputs are kept for small inputs; repro regenerates them for big ones.
    private static string Clip(string output) => output.Length <= 256 * 1024 ? output : output.Substring(0, 256 * 1024) + "\n... clipped, run repro for the rest\n";

    private void Hit(Finding finding)
    {
        finding.Hits++;
        _dirtyFindings.Add(finding.Signature);
    }

    // Removes lines, then characters, while the inputs still differ the same way; bounded by a run budget.
    private string[] Minimize(Worker worker, string scope, long iteration, string[] lines, string kind, out bool complete)
    {
        int budget = _options.MinimizeBudget;
        var clock = Stopwatch.StartNew();
        bool Fails(List<string> candidate)
        {
            // Also bounded in time: each try of a long line may take seconds.
            if (--budget <= 0 || clock.Elapsed.TotalSeconds > _options.MinimizeSeconds)
            {
                budget = 0;
                return false;
            }
            (string b, string o) = worker.Exec(scope, iteration, candidate.ToArray());
            return Comparison.Compare(scope, candidate.ToArray(), b, o)?.Kind == kind;
        }

        var current = lines.ToList();
        for (int chunk = Math.Max(1, current.Count / 2); chunk >= 1 && budget > 0 && !Stopping; chunk /= 2)
        {
            for (int i = 0; i + chunk <= current.Count && current.Count > 1 && budget > 0 && !Stopping;)
            {
                var candidate = new List<string>(current);
                candidate.RemoveRange(i, chunk);
                if (candidate.Count > 0 && Fails(candidate))
                {
                    current = candidate;
                }
                else
                {
                    i += chunk;
                }
            }
        }

        for (int l = 0; l < current.Count && budget > 0 && !Stopping; l++)
        {
            for (int chunk = Math.Max(1, current[l].Length / 2); chunk >= 1 && budget > 0 && !Stopping; chunk /= 2)
            {
                for (int i = 0; i + chunk <= current[l].Length && budget > 0 && !Stopping;)
                {
                    var candidate = new List<string>(current);
                    candidate[l] = current[l].Remove(i, chunk);
                    if (Fails(candidate))
                    {
                        current = candidate;
                    }
                    else
                    {
                        i += chunk;
                    }
                }
            }
        }

        complete = budget > 0 && !Stopping;
        return current.ToArray();
    }

    // A tokenization that runs past --hang-seconds cannot be interrupted: record it, save the state with that
    // iteration skipped, and exit so a rerun resumes after it.
    private void Watch()
    {
        while (true)
        {
            Thread.Sleep(1000);
            foreach (Worker worker in _workers)
            {
                if (worker.Current is Inflight inflight && worker.Elapsed.TotalSeconds > _options.HangSeconds)
                {
                    Console.WriteLine($"  hang in {inflight.Scope} at iteration {inflight.Iteration} ({worker.Elapsed.TotalSeconds:F0}s); saving and exiting with {HangExitCode}");
                    lock (_gate)
                    {
                        Crash("hang", inflight);
                    }
                    // Halt the other workers first: one that starts its next input after its inflight file is
                    // deleted would leave a file the next run takes for a process crash.
                    foreach (Worker w in _workers)
                    {
                        w.Halt();
                    }
                    foreach (Worker w in _workers)
                    {
                        w.Clear();
                    }
                    Persist();
                    Console.Out.Flush();
                    Environment.Exit(HangExitCode);
                }
            }
        }
    }

    // A previous run that died without saving (a stack overflow, an access violation): the inflight inputs are the cause.
    private void RecoverCrashes()
    {
        if (!Directory.Exists(InflightDirectory))
        {
            return;
        }

        foreach (string file in Directory.EnumerateFiles(InflightDirectory, "*.json"))
        {
            if (Store.Read<Inflight>(file) is Inflight inflight && _slots.ContainsKey(inflight.Scope))
            {
                Console.WriteLine($"  the previous run died in {inflight.Scope} at iteration {inflight.Iteration}; recorded as a process-crash");
                Crash("process-crash", inflight);
            }
            File.Delete(file);
        }

        Persist();
    }

    private void Crash(string kind, Inflight inflight)
    {
        Slot slot = _slots[inflight.Scope];
        lock (slot)
        {
            slot.Skip.Add(inflight.Iteration);
            slot.State.Skip = slot.Skip.Order().ToList();
            slot.State.Discrepancies++;
            slot.State.Crashes++;
        }

        string signature = kind + "|" + inflight.Scope;
        if (_findings.TryGetValue(signature, out Finding? existing))
        {
            Hit(existing);
            return;
        }

        var finding = new Finding
        {
            Id = kind + "-" + Store.FileName(inflight.Scope) + "-" + Comparison.Hash(signature),
            Kind = kind,
            Scope = inflight.Scope,
            Language = slot.Target.Language,
            Signature = signature,
            RawSignatures = [signature],
            Hits = 1,
            Seed = _campaign.Seed,
            Iteration = inflight.Iteration,
            FirstSeen = DateTime.UtcNow,
            Input = inflight.Input,
            Key = "unknown: run each engine alone with `repro --engine branch|official`",
        };
        _findings[signature] = finding;
        _rawSignatures[signature] = signature;
        Store.Write(Path.Combine(FindingsDirectory, finding.Id + ".json"), finding);
    }

    // Saves each grammar's corpus with a snapshot of its state taken under the same lock, so a resume generates the
    // same inputs an uninterrupted run would have.
    private void Persist()
    {
        lock (_persistGate)
        {
            var snapshot = new CampaignState
            {
                Seed = _campaign.Seed,
                Created = _campaign.Created,
                Seconds = _previousSeconds + _clock.Elapsed.TotalSeconds,
                BranchVersion = _campaign.BranchVersion,
                OfficialVersion = _campaign.OfficialVersion,
            };
            foreach ((string scope, GrammarState state) in _campaign.Grammars)
            {
                Slot? slot = _slots.GetValueOrDefault(scope);
                lock ((object?)slot ?? state)
                {
                    if (slot != null && slot.Dirty)
                    {
                        Store.Write(CorpusPath(scope), slot.Corpus);
                        slot.Dirty = false;
                    }
                    snapshot.Grammars[scope] = new GrammarState
                    {
                        Next = state.Next,
                        Execs = state.Execs,
                        BusySeconds = state.BusySeconds,
                        Discrepancies = state.Discrepancies,
                        Crashes = state.Crashes,
                        Skip = state.Skip.ToList(),
                    };
                }
            }
            Store.Write(Path.Combine(_state, "campaign.json"), snapshot);

            lock (_gate)
            {
                foreach (string signature in _dirtyFindings)
                {
                    Finding finding = _findings[signature];
                    Store.Write(Path.Combine(FindingsDirectory, finding.Id + ".json"), finding);
                }
                _dirtyFindings.Clear();
                try
                {
                    Report.Write(_state, snapshot, _findings.Values);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"  could not write findings.md: {e.Message}");
                }
            }
        }
    }

    private string StatusLine(double rate)
    {
        long execs = _slots.Values.Sum(s => s.State.Execs);
        int unique;
        long hits;
        int crashes;
        lock (_gate)
        {
            unique = _findings.Count;
            hits = _findings.Values.Sum(f => f.Hits);
            crashes = _findings.Values.Count(f => Comparison.IsCrash(f.Kind));
        }
        TimeSpan elapsed = TimeSpan.FromSeconds(_previousSeconds + _clock.Elapsed.TotalSeconds);
        return $"[{elapsed:d\\.hh\\:mm\\:ss}] execs {execs:N0} ({rate:N0}/s) | grammars {_slots.Count} | findings {unique} unique / {hits:N0} hits | crash kinds {crashes}";
    }

    private string Table()
    {
        var text = new System.Text.StringBuilder();
        text.AppendLine($"  {"grammar",-34} {"iters",9} {"exec/s",8} {"discrep",8} {"crashes",8}");
        foreach (Slot slot in _slots.Values)
        {
            GrammarState s = slot.State;
            double rate = s.BusySeconds > 0 ? s.Execs / s.BusySeconds : 0;
            text.AppendLine($"  {slot.Target.Scope,-34} {s.Next,9:N0} {rate,8:N1} {s.Discrepancies,8:N0} {s.Crashes,8:N0}");
        }
        return text.ToString();
    }
}

public sealed class Inflight
{
    public string Scope { get; set; } = "";
    public long Iteration { get; set; }
    public Lines Input { get; set; } = new();
}

// A fuzzing thread with its own tokenizer sessions. Before each run it writes the input to its inflight file, so a
// run that kills the process is known to the next one.
public sealed class Worker(int id, Func<string, string[], int, string> branch, Func<string, string[], int, string> official, string inflightPath, int lineTimeLimitMs)
{
    private long _started;
    private volatile bool _halted;

    public int Id => id;

    public Inflight? Current { get; private set; }

    public TimeSpan Elapsed => Volatile.Read(ref _started) is long started && started != 0 ? Stopwatch.GetElapsedTime(started) : TimeSpan.Zero;

    // Stops the worker before its next run, for the process to exit.
    public void Halt() => _halted = true;

    public (string Branch, string Official) Exec(string scope, long iteration, string[] lines)
    {
        while (_halted)
        {
            Thread.Sleep(Timeout.Infinite);
        }

        var inflight = new Inflight { Scope = scope, Iteration = iteration, Input = Lines.From(lines) };
        Store.Write(inflightPath, inflight);
        if (_halted)
        {
            // Halted while writing: the file must not outlive the process.
            Clear();
            Thread.Sleep(Timeout.Infinite);
        }
        Current = inflight;
        Volatile.Write(ref _started, Stopwatch.GetTimestamp());
        string b = Guard(branch, scope, lines);
        string o = Guard(official, scope, lines);
        Volatile.Write(ref _started, 0);
        Current = null;
        return (b, o);
    }

    public void Clear()
    {
        if (File.Exists(inflightPath))
        {
            File.Delete(inflightPath);
        }
    }

    private string Guard(Func<string, string[], int, string> run, string scope, string[] lines)
    {
        try
        {
            return run(scope, lines, lineTimeLimitMs);
        }
        catch (Exception e)
        {
            // The runner itself failed (not the tokenizer, whose exceptions the runner catches).
            return "#0 t EXC runner " + e.GetType().FullName + "\u0001" + e.ToString().Replace('\n', ' ') + "\n";
        }
    }
}
