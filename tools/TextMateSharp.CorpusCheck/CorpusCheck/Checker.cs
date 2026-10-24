using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TextMateSharp.CorpusCheck;

public sealed class Checker(Options options, List<CorpusDefinition> corpora, Func<IEngine> createOfficial, Func<IEngine> createBranch)
{
    private const int MaxReproSuffix = 512;

    private readonly ConcurrentDictionary<int, InFlight> _inFlight = new();
    private int _nextWorkerId;

    public Report Run()
    {
        IEngine officialProbe = createOfficial(), branchProbe = createBranch();
        var report = new Report
        {
            OfficialVersion = officialProbe.Version,
            BranchVersion = branchProbe.Version,
            Machine = $"{Environment.MachineName}, {RuntimeInformation.OSDescription}, {RuntimeInformation.ProcessArchitecture}, {Environment.ProcessorCount} cores",
            Runtime = RuntimeInformation.FrameworkDescription,
            Threads = options.Threads,
            StartedAt = DateTimeOffset.Now,
            Corpora = corpora.Select(c => new CorpusReport { Name = c.Name, Scope = c.Scope, Repo = c.Repo, Commit = c.Commit }).ToList(),
        };

        var work = new List<WorkItem>();
        for (int i = 0; i < corpora.Count; i++)
        {
            CorpusDefinition corpus = corpora[i];
            CorpusReport corpusReport = report.Corpora[i];
            string root = Path.Combine(options.Corpus, corpus.Name);
            if (!Directory.Exists(root))
            {
                corpusReport.Error = "not fetched";
                continue;
            }
            if (officialProbe.LoadGrammar(corpus.Scope) == null || branchProbe.LoadGrammar(corpus.Scope) == null)
            {
                corpusReport.Error = $"no grammar for {corpus.Scope}";
                continue;
            }

            var files = Directory.EnumerateFiles(root, "*", new EnumerationOptions { RecurseSubdirectories = true, AttributesToSkip = 0 })
                .Select(path => Path.GetRelativePath(root, path).Replace('\\', '/'))
                .Where(relative => !relative.StartsWith(".git/", StringComparison.Ordinal) && corpus.Matches(Path.GetFileName(relative)))
                .Order(StringComparer.Ordinal)
                .Take(options.MaxFiles);
            foreach (string relative in files)
            {
                var info = new FileInfo(Path.Combine(root, relative));
                work.Add(new WorkItem(corpus, corpusReport, relative, info.FullName, info.Length));
                corpusReport.FilesTotal++;
            }
        }

        // Largest files first, so that the last files to finish are small ones.
        work.Sort((a, b) => b.Size.CompareTo(a.Size));
        var progress = new Progress(report, work.Sum(w => w.Size), _inFlight);
        var workers = new ThreadLocal<Worker>(() => new Worker(Interlocked.Increment(ref _nextWorkerId), createOfficial(), createBranch()));
        var stopwatch = Stopwatch.StartNew();
        progress.Start();
        Parallel.ForEach(
            System.Collections.Concurrent.Partitioner.Create(work, EnumerablePartitionerOptions.NoBuffering),
            new ParallelOptions { MaxDegreeOfParallelism = options.Threads },
            item =>
            {
                Worker worker = workers.Value!;
                _inFlight[worker.Id] = new InFlight($"{item.Corpus.Name}/{item.Relative}", Stopwatch.GetTimestamp());
                try
                {
                    CheckFile(item, worker);
                }
                catch (Exception e)
                {
                    lock (item.Report)
                    {
                        item.Report.Errors.Add(new EngineError("checker", item.Relative, 0, "", e.ToString()));
                    }
                }
                finally
                {
                    _inFlight.TryRemove(worker.Id, out _);
                    Interlocked.Increment(ref item.Report.FilesDone);
                    progress.AddBytes(item.Size);
                }
            });
        report.DurationSeconds = stopwatch.Elapsed.TotalSeconds;
        progress.Stop();

        foreach (CorpusReport corpus in report.Corpora)
        {
            corpus.Discrepancies.Sort((a, b) => (string.CompareOrdinal(a.File, b.File), a.Line.CompareTo(b.Line)) switch
            {
                (0, var line) => line,
                (var file, _) => file,
            });
            corpus.Errors.Sort((a, b) => string.CompareOrdinal(a.File, b.File) is var c && c != 0 ? c : a.Line.CompareTo(b.Line));
            corpus.Skipped.Sort((a, b) => string.CompareOrdinal(a.File, b.File));
        }
        return report;
    }

    private void CheckFile(WorkItem item, Worker worker)
    {
        CorpusReport report = item.Report;
        string[] lines = ReadLines(item.Path, options.MaxLines);
        int longest = lines.Length == 0 ? 0 : lines.Max(l => l.Length);
        if (longest > options.MaxLineLength)
        {
            lock (report)
            {
                report.Skipped.Add(new SkippedFile(item.Relative, $"a line has {longest} characters (--max-line-length {options.MaxLineLength})"));
            }
            Interlocked.Increment(ref report.FilesSkipped);
            return;
        }

        (IEngineGrammar official, IEngineGrammar branch) = worker.Grammars(item.Corpus.Scope);
        TimeSpan limit = options.TimeLimit;
        object? officialState = null, branchState = null, officialState2 = null, branchState2 = null;
        long officialTokens = 0, officialTicks = 0, officialTokens2 = 0, officialTicks2 = 0, officialTimeouts = 0;
        long branchTokens = 0, branchTicks = 0, branchTokens2 = 0, branchTicks2 = 0, branchTimeouts = 0;
        long matching = 0, mismatching = 0, tokensMismatching = 0, metadataMismatching = 0;
        int recorded = 0;

        int lineCount = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            string engine = "official", method = "TokenizeLine";
            LineTokens officialResult, branchResult;
            LineTokens2 officialResult2, branchResult2;
            try
            {
                long start = Stopwatch.GetTimestamp();
                officialResult = official.TokenizeLine(line, officialState, limit);
                long end = Stopwatch.GetTimestamp();
                officialTicks += end - start;

                engine = "branch";
                branchResult = branch.TokenizeLine(line, branchState, limit);
                start = Stopwatch.GetTimestamp();
                branchTicks += start - end;

                engine = "official"; method = "TokenizeLine2";
                officialResult2 = official.TokenizeLine2(line, officialState2, limit);
                end = Stopwatch.GetTimestamp();
                officialTicks2 += end - start;

                engine = "branch";
                branchResult2 = branch.TokenizeLine2(line, branchState2, limit);
                branchTicks2 += Stopwatch.GetTimestamp() - end;
            }
            catch (Exception e)
            {
                lock (report)
                {
                    report.Errors.Add(new EngineError(engine, item.Relative, i + 1, method, $"{e.GetType().FullName}: {e.Message}"));
                }
                Interlocked.Increment(ref (engine == "official" ? report.Official : report.Branch).ErrorCount);
                break;
            }

            lineCount++;
            officialState = officialResult.State;
            branchState = branchResult.State;
            officialState2 = officialResult2.State;
            branchState2 = branchResult2.State;
            officialTokens += officialResult.Tokens.Length;
            branchTokens += branchResult.Tokens.Length;
            officialTokens2 += officialResult2.Tokens.Length / 2;
            branchTokens2 += branchResult2.Tokens.Length / 2;
            bool officialStopped = officialResult.StoppedEarly || officialResult2.StoppedEarly;
            bool branchStopped = branchResult.StoppedEarly || branchResult2.StoppedEarly;
            if (officialStopped) officialTimeouts++;
            if (branchStopped) branchTimeouts++;

            bool tokensEqual = officialResult.Tokens.AsSpan().SequenceEqual(branchResult.Tokens);
            bool metadataEqual = officialResult2.Tokens.AsSpan().SequenceEqual(branchResult2.Tokens);
            if (tokensEqual && metadataEqual)
            {
                matching++;
                continue;
            }

            mismatching++;
            if (!tokensEqual) tokensMismatching++;
            if (!metadataEqual) metadataMismatching++;
            if (recorded >= options.MaxReproPerFile)
            {
                Interlocked.Increment(ref report.DiscrepanciesNotRecorded);
                continue;
            }

            recorded++;
            int contextStart = Reproduce(official, branch, lines, i, limit);
            var discrepancy = new Discrepancy
            {
                File = item.Relative,
                Line = i + 1,
                Text = line,
                Kind = (tokensEqual, metadataEqual) switch { (false, false) => "both", (false, true) => "TokenizeLine", _ => "TokenizeLine2" },
                StoppedEarly = officialStopped || branchStopped,
                ReproducesFromContext = contextStart >= 0,
                ContextStartLine = (contextStart >= 0 ? contextStart : Math.Max(0, i - MaxReproSuffix)) + 1,
                ContextLines = lines[(contextStart >= 0 ? contextStart : Math.Max(0, i - MaxReproSuffix))..(i + 1)],
                OfficialTokens = officialResult.Tokens,
                BranchTokens = branchResult.Tokens,
                OfficialTokens2 = officialResult2.Tokens,
                BranchTokens2 = branchResult2.Tokens,
            };
            lock (report)
            {
                if (report.Discrepancies.Count < options.MaxReproPerFile * 100)
                {
                    report.Discrepancies.Add(discrepancy);
                }
                else
                {
                    report.DiscrepanciesNotRecorded++;
                }
            }
        }

        Interlocked.Increment(ref report.Files);
        Interlocked.Add(ref report.Bytes, item.Size);
        Interlocked.Add(ref report.Lines, lineCount);
        Interlocked.Add(ref report.LinesMatching, matching);
        Interlocked.Add(ref report.LinesMismatching, mismatching);
        Interlocked.Add(ref report.LinesTokensMismatching, tokensMismatching);
        Interlocked.Add(ref report.LinesMetadataMismatching, metadataMismatching);
        if (mismatching > 0) Interlocked.Increment(ref report.FilesWithMismatch);
        Add(report.Official, officialTokens, officialTicks, officialTokens2, officialTicks2, officialTimeouts);
        Add(report.Branch, branchTokens, branchTicks, branchTokens2, branchTicks2, branchTimeouts);
    }

    private static void Add(EngineStats stats, long tokens, long ticks, long tokens2, long ticks2, long timeouts)
    {
        Interlocked.Add(ref stats.Tokens, tokens);
        Interlocked.Add(ref stats.Ticks, ticks);
        Interlocked.Add(ref stats.Tokens2, tokens2);
        Interlocked.Add(ref stats.Ticks2, ticks2);
        Interlocked.Add(ref stats.Timeouts, timeouts);
    }

    // The first line of the shortest suffix (0, 1, 2, 4, ... lines before) ending at lines[index] that, tokenized from
    // the initial state, still differs on its last line; -1 when none up to MaxReproSuffix lines does.
    private static int Reproduce(IEngineGrammar official, IEngineGrammar branch, string[] lines, int index, TimeSpan limit)
    {
        int longest = Math.Min(index, MaxReproSuffix);
        for (int back = 0; ; back = back == 0 ? 1 : Math.Min(back * 2, longest))
        {
            try
            {
                object? officialState = null, branchState = null, officialState2 = null, branchState2 = null;
                bool differs = false;
                for (int i = index - back; i <= index; i++)
                {
                    LineTokens o = official.TokenizeLine(lines[i], officialState, limit), b = branch.TokenizeLine(lines[i], branchState, limit);
                    LineTokens2 o2 = official.TokenizeLine2(lines[i], officialState2, limit), b2 = branch.TokenizeLine2(lines[i], branchState2, limit);
                    (officialState, branchState, officialState2, branchState2) = (o.State, b.State, o2.State, b2.State);
                    differs = !o.Tokens.AsSpan().SequenceEqual(b.Tokens) || !o2.Tokens.AsSpan().SequenceEqual(b2.Tokens);
                }
                if (differs)
                {
                    return index - back;
                }
            }
            catch (Exception)
            {
                // The error itself is reported by the main pass.
            }
            if (back == longest)
            {
                break;
            }
        }
        return -1;
    }

    // Lines without their terminator, as an editor feeds them.
    private static string[] ReadLines(string path, int maxLines)
    {
        string text = File.ReadAllText(path);
        var lines = text.Split('\n');
        int count = lines.Length > 0 && lines[^1].Length == 0 ? lines.Length - 1 : lines.Length;
        count = Math.Min(count, maxLines);
        var result = new string[count];
        for (int i = 0; i < count; i++)
        {
            string line = lines[i];
            result[i] = line.EndsWith('\r') ? line[..^1] : line;
        }
        return result;
    }

    private sealed record WorkItem(CorpusDefinition Corpus, CorpusReport Report, string Relative, string Path, long Size);

    // One registry per engine per thread: a grammar tokenizes one line at a time.
    private sealed class Worker(int id, IEngine official, IEngine branch)
    {
        private readonly Dictionary<string, (IEngineGrammar, IEngineGrammar)> _grammars = [];

        public int Id => id;

        public (IEngineGrammar Official, IEngineGrammar Branch) Grammars(string scope)
        {
            if (!_grammars.TryGetValue(scope, out var grammars))
            {
                grammars = (official.LoadGrammar(scope)!, branch.LoadGrammar(scope)!);
                _grammars[scope] = grammars;
            }
            return grammars;
        }
    }
}

public sealed record InFlight(string File, long StartedAt);
