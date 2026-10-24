using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace TextMateSharp.CorpusCheck;

// Redraws a progress block a few times per second, or prints a line every 10 seconds when the output is redirected.
public sealed class Progress(Report report, long totalBytes, ConcurrentDictionary<int, InFlight> inFlight)
{
    private const int MaxCorpusRows = 12;

    private readonly Stopwatch _stopwatch = new();
    private readonly bool _interactive = !Console.IsOutputRedirected;
    private long _bytesDone;
    private int _previousHeight;
    private Thread? _thread;
    private volatile bool _stopping;

    public void AddBytes(long bytes) => Interlocked.Add(ref _bytesDone, bytes);

    public void Start()
    {
        _stopwatch.Start();
        _thread = new Thread(Loop) { IsBackground = true, Name = "progress" };
        _thread.Start();
    }

    public void Stop()
    {
        _stopping = true;
        _thread?.Join();
        Draw();
        if (!_interactive)
        {
            return;
        }
        Console.WriteLine();
    }

    private void Loop()
    {
        while (!_stopping)
        {
            Thread.Sleep(_interactive ? 250 : 10_000);
            if (!_stopping)
            {
                Draw();
            }
        }
    }

    private void Draw()
    {
        double fraction = totalBytes == 0 ? 1 : Math.Min(1, (double)Interlocked.Read(ref _bytesDone) / totalBytes);
        TimeSpan elapsed = _stopwatch.Elapsed;
        string eta = fraction is > 0.01 and < 1 ? $" ETA {Format(elapsed / fraction - elapsed)}" : "";
        int files = report.Corpora.Sum(c => Volatile.Read(ref c.FilesDone)), filesTotal = report.Corpora.Sum(c => c.FilesTotal);
        long lines = report.Corpora.Sum(c => Interlocked.Read(ref c.Lines));
        long mismatching = report.Corpora.Sum(c => Interlocked.Read(ref c.LinesMismatching));
        long errors = report.Corpora.Sum(c => Interlocked.Read(ref c.Official.ErrorCount) + Interlocked.Read(ref c.Branch.ErrorCount));
        long timeouts = report.Corpora.Sum(c => Interlocked.Read(ref c.Official.Timeouts) + Interlocked.Read(ref c.Branch.Timeouts));
        string summary = $"{Bar(fraction, 30)} {fraction,4:P0}  files {files:N0}/{filesTotal:N0}  lines {lines:N0}  mismatched {mismatching:N0}  errors {errors:N0}  timeouts {timeouts:N0}  {Format(elapsed)}{eta}";

        if (!_interactive)
        {
            Console.WriteLine(summary);
            return;
        }

        var text = new StringBuilder();
        text.AppendLine($"TextMateSharp corpus check: official {report.OfficialVersion} vs branch {report.BranchVersion}, {report.Threads} threads");
        text.AppendLine(summary);
        text.AppendLine($"  {"corpus",-14} {"files",13} {"lines",11} {"mismatch",9} {"official tok/s",15} {"branch tok/s",13} {"speedup",8}");

        // Corpora in progress, then those with mismatches, then the rest.
        var rows = report.Corpora
            .Where(c => c.FilesTotal > 0)
            .OrderBy(c => c.FilesDone > 0 && c.FilesDone < c.FilesTotal ? 0 : c.LinesMismatching > 0 ? 1 : c.FilesDone == 0 ? 3 : 2)
            .Take(MaxCorpusRows);
        foreach (CorpusReport c in rows)
        {
            double official = c.Official.TokensPerSecond, branch = c.Branch.TokensPerSecond;
            string speedup = official > 0 && branch > 0 ? $"{branch / official:F2}x" : "";
            string mismatch = c.LinesMismatching > 0 ? $"\u001b[31m{c.LinesMismatching,9:N0}\u001b[0m" : $"{c.LinesMismatching,9:N0}";
            text.AppendLine($"  {Truncate(c.Name, 14),-14} {$"{c.FilesDone:N0}/{c.FilesTotal:N0}",13} {c.Lines,11:N0} {mismatch} {official,15:N0} {branch,13:N0} {speedup,8}");
        }

        // The longest-running files, to spot a slow or stuck line.
        long now = Stopwatch.GetTimestamp();
        foreach (InFlight file in inFlight.Values.OrderBy(f => f.StartedAt).Take(3))
        {
            text.AppendLine($"  \u001b[2m{Format(Stopwatch.GetElapsedTime(file.StartedAt, now)),8} {Truncate(file.File, 90)}\u001b[0m");
        }

        string block = text.ToString();
        int height = block.Count(ch => ch == '\n');
        var output = new StringBuilder();
        if (_previousHeight > 0)
        {
            output.Append($"\u001b[{_previousHeight}F");
        }
        output.Append("\u001b[J").Append(block);
        _previousHeight = height;
        Console.Write(output.ToString());
    }

    private static string Bar(double fraction, int width)
    {
        int full = (int)Math.Round(fraction * width);
        return "[" + new string('█', full) + new string('░', width - full) + "]";
    }

    private static string Truncate(string text, int length) => text.Length <= length ? text : "…" + text[^(length - 1)..];

    private static string Format(TimeSpan time) => time.TotalHours >= 1 ? time.ToString(@"h\:mm\:ss") : time.ToString(@"m\:ss");
}
