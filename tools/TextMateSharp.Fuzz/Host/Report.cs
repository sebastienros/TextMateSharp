using System.Text;

namespace TextMateSharp.Fuzz;

// findings.md: the campaign's totals and every unique discrepancy with its minimized input and both outputs.
public static class Report
{
    public static void Write(string state, CampaignState campaign, IEnumerable<Finding> findings)
    {
        List<Finding> list = findings.OrderBy(f => Array.IndexOf(Comparison.Kinds, f.Kind) is int r && r < 0 ? -1 : r).ThenBy(f => f.Scope, StringComparer.Ordinal).ThenByDescending(f => f.Hits).ToList();
        var text = new StringBuilder();
        text.AppendLine("# TextMateSharp differential fuzzing findings");
        text.AppendLine();
        text.AppendLine($"- Seed: `{campaign.Seed}`, started {campaign.Created:u}, fuzzed {TimeSpan.FromSeconds(campaign.Seconds):d\\.hh\\:mm\\:ss}");
        text.AppendLine($"- Branch: `{campaign.BranchVersion}`");
        text.AppendLine($"- Official: `{campaign.OfficialVersion}`");
        text.AppendLine($"- Grammars: {campaign.Grammars.Count}, executions: {campaign.Grammars.Values.Sum(g => g.Execs):N0}, discrepancies: {campaign.Grammars.Values.Sum(g => g.Discrepancies):N0}, crashes: {campaign.Grammars.Values.Sum(g => g.Crashes):N0}");
        text.AppendLine($"- Unique findings: {list.Count}");
        text.AppendLine();
        if (list.Count > 0)
        {
            text.AppendLine("| Kind | Grammar | Hits | Lines | Chars | Finding |");
            text.AppendLine("|---|---|---:|---:|---:|---|");
            foreach (Finding f in list)
            {
                string[] lines = f.Input.ToArray();
                text.AppendLine($"| {f.Kind} | `{f.Scope}` | {f.Hits:N0} | {lines.Length} | {lines.Sum(l => l.Length)} | [{f.Id}](findings/{f.Id}.json) |");
            }
            text.AppendLine();
        }

        foreach (Finding f in list)
        {
            text.AppendLine($"## {f.Id}");
            text.AppendLine();
            text.AppendLine($"{f.Kind} in `{f.Scope}`{(f.Language != null ? $" (language `{f.Language}`)" : "")}, {f.Hits:N0} hit(s), first at iteration {f.Iteration}{(f.Minimized ? ", minimized" : ", not fully minimized")}. First difference: `{f.Key}`.");
            text.AppendLine();
            text.AppendLine("Input (one JSON string per line):");
            text.AppendLine();
            text.AppendLine("```json");
            foreach (string line in f.Input.Text.Take(40))
            {
                text.AppendLine(Quote(line));
            }
            if (f.Input.Text.Length > 40)
            {
                text.AppendLine($"... {f.Input.Text.Length - 40} more line(s)");
            }
            text.AppendLine("```");
            if (f.Branch.Length > 0 || f.Official.Length > 0)
            {
                text.AppendLine();
                text.AppendLine("Branch:");
                text.AppendLine("```");
                text.AppendLine(Clip(Comparison.Strip(f.Branch)));
                text.AppendLine("```");
                text.AppendLine("Official:");
                text.AppendLine("```");
                text.AppendLine(Clip(Comparison.Strip(f.Official)));
                text.AppendLine("```");
            }
            text.AppendLine();
            text.AppendLine($"Reproduce from the state directory: `dotnet <fuzzer>/TextMateSharp.Fuzz.dll repro findings/{f.Id}.json`");
            text.AppendLine();
        }

        // Clipping may split a surrogate pair.
        File.WriteAllText(Path.Combine(state, "findings.md"), Lines.Readable(text.ToString()));
    }

    private static string Quote(string line)
    {
        string json = System.Text.Json.JsonSerializer.Serialize(line, Store.Options);
        return json.Length > 400 ? json.Substring(0, 400) + $"... ({line.Length} chars)" : json;
    }

    private static string Clip(string block)
    {
        block = block.TrimEnd('\n');
        string[] lines = block.Split('\n');
        IEnumerable<string> kept = lines.Take(30).Select(l => l.Length > 300 ? l.Substring(0, 300) + "..." : l);
        return string.Join('\n', kept) + (lines.Length > 30 ? $"\n... {lines.Length - 30} more line(s)" : "");
    }
}
