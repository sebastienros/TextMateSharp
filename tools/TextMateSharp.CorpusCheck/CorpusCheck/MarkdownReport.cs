using System.Globalization;
using System.Text;

namespace TextMateSharp.CorpusCheck;

public static class MarkdownReport
{
    private const int DiscrepanciesPerCorpus = 5;
    private const int ContextLinesShown = 12;

    public static string Render(Report report)
    {
        var md = new StringBuilder();
        var inv = CultureInfo.InvariantCulture;
        long lines = report.Corpora.Sum(c => c.Lines), mismatching = report.Corpora.Sum(c => c.LinesMismatching);
        int errors = report.Corpora.Sum(c => c.Errors.Count);

        md.AppendLine("# TextMateSharp corpus check");
        md.AppendLine();
        md.AppendLine($"- Official: TextMateSharp {report.OfficialVersion} (Onigwrap)");
        md.AppendLine($"- Branch: TextMateSharp {report.BranchVersion} (GregsEx)");
        md.AppendLine($"- Machine: {report.Machine}; {report.Runtime}; {report.Threads} threads");
        md.AppendLine($"- Started {report.StartedAt:u}, took {TimeSpan.FromSeconds(report.DurationSeconds):hh\\:mm\\:ss}");
        md.AppendLine(string.Create(inv, $"- **{lines:N0} lines, {mismatching:N0} mismatched, {errors:N0} errors** in {report.Corpora.Sum(c => c.Files):N0} files"));
        md.AppendLine();
        md.AppendLine("Tokens per second count TokenizeLine tokens over the time spent in TokenizeLine on each engine (both run on the same thread, interleaved line by line); the first lines of each grammar on each thread include compiling its rules.");
        md.AppendLine();
        md.AppendLine("| Corpus | Scope | Source | Files | Lines | Matching | Mismatched (tokens / metadata) | Files w/ mismatch | Errors (O/B) | Timeouts (O/B) | Official tok/s | Branch tok/s | Speedup |");
        md.AppendLine("|---|---|---|--:|--:|--:|--:|--:|--:|--:|--:|--:|--:|");
        foreach (CorpusReport c in report.Corpora.Where(c => c.Error != "not fetched"))
        {
            string source = $"[{c.Repo.Replace("https://github.com/", "")}@{c.Commit[..10]}]({c.Repo}/tree/{c.Commit})";
            if (c.Error != null)
            {
                md.AppendLine($"| {c.Name} | `{c.Scope}` | {source} | | | | {c.Error} | | | | | | |");
                continue;
            }
            double official = c.Official.TokensPerSecond, branch = c.Branch.TokensPerSecond;
            string matching = c.Lines == 0 ? "" : string.Create(inv, $"{100.0 * c.LinesMatching / c.Lines:F3}%");
            string skipped = c.FilesSkipped > 0 ? $" (+{c.FilesSkipped} skipped)" : "";
            string mismatch = c.LinesMismatching == 0 ? "0" : string.Create(inv, $"**{c.LinesMismatching:N0}** ({c.LinesTokensMismatching:N0} / {c.LinesMetadataMismatching:N0})");
            md.AppendLine(string.Create(inv,
                $"| {c.Name} | `{c.Scope}` | {source} | {c.Files:N0}{skipped} | {c.Lines:N0} | {matching} | {mismatch} | {c.FilesWithMismatch:N0} | {c.Official.ErrorCount}/{c.Branch.ErrorCount} | {c.Official.Timeouts}/{c.Branch.Timeouts} | {official:N0} | {branch:N0} | {(official > 0 ? $"{branch / official:F2}x" : "")} |"));
        }

        var notFetched = report.Corpora.Where(c => c.Error == "not fetched").Select(c => c.Name).ToList();
        if (notFetched.Count > 0)
        {
            md.AppendLine();
            md.AppendLine($"Not fetched (run fetch-corpus.ps1): {string.Join(", ", notFetched)}.");
        }

        foreach (CorpusReport c in report.Corpora.Where(c => c.Discrepancies.Count > 0 || c.Errors.Count > 0))
        {
            md.AppendLine();
            md.AppendLine($"## {c.Name}");
            foreach (EngineError error in c.Errors.Take(20))
            {
                md.AppendLine($"- Error ({error.Engine}, {error.Method}) at `{error.File}:{error.Line}`: {error.Message.Split('\n')[0]}");
            }
            if (c.Errors.Count > 20)
            {
                md.AppendLine($"- … {c.Errors.Count - 20} more errors in report.json");
            }

            foreach (Discrepancy d in c.Discrepancies.Take(DiscrepanciesPerCorpus))
            {
                md.AppendLine();
                string repro = d.ReproducesFromContext
                    ? $"reproduces from line {d.ContextStartLine} with the initial state"
                    : "needs the file from its first line to reproduce";
                md.AppendLine($"### `{d.File}:{d.Line}` ({d.Kind}{(d.StoppedEarly ? ", stopped early" : "")}; {repro})");
                md.AppendLine();
                md.AppendLine("```");
                foreach (string line in d.ContextLines.TakeLast(ContextLinesShown))
                {
                    md.AppendLine(line);
                }
                md.AppendLine("```");
                md.AppendLine();
                md.AppendLine("| # | Official | Branch |");
                md.AppendLine("|--:|---|---|");
                int count = Math.Max(d.OfficialTokens.Length, d.BranchTokens.Length);
                for (int i = 0; i < count; i++)
                {
                    string o = i < d.OfficialTokens.Length ? Describe(d.OfficialTokens[i]) : "";
                    string b = i < d.BranchTokens.Length ? Describe(d.BranchTokens[i]) : "";
                    md.AppendLine(o == b ? $"| {i} | {o} | = |" : $"| **{i}** | {o} | {b} |");
                }
                if (d.Kind != "TokenizeLine")
                {
                    md.AppendLine();
                    md.AppendLine($"TokenizeLine2 official `[{string.Join(", ", d.OfficialTokens2)}]`, branch `[{string.Join(", ", d.BranchTokens2)}]`");
                }
            }
            if (c.Discrepancies.Count > DiscrepanciesPerCorpus || c.DiscrepanciesNotRecorded > 0)
            {
                md.AppendLine();
                md.AppendLine($"… {c.Discrepancies.Count - Math.Min(c.Discrepancies.Count, DiscrepanciesPerCorpus)} more discrepancies in report.json, {c.DiscrepanciesNotRecorded} more lines not recorded.");
            }
        }

        return md.ToString();
    }

    private static string Describe(Token token) => $"{token.Start}-{token.End} `{token.Scopes}`";
}
