using System.Security.Cryptography;
using System.Text;

namespace TextMateSharp.Fuzz;

// How the branch's output differs from the official one, if it does.
public sealed record Difference(string Kind, string Key, string Branch, string Official, string Signature);

public static class Comparison
{
    private const char Detail = '\u0001';

    // Kinds, most severe first; the first one present names the discrepancy.
    public static readonly string[] Kinds = ["load", "exception-branch", "exception-official", "exception-type", "stopped-branch", "tokens", "metadata", "invalid-utf16", "stopped-official"];

    public static bool IsCrash(string kind) => kind.StartsWith("exception", StringComparison.Ordinal) || kind is "load" or "hang" or "process-crash";

    public static string Strip(string output)
    {
        var text = new StringBuilder(output.Length);
        foreach (string line in output.Split('\n'))
        {
            int detail = line.IndexOf(Detail);
            text.Append(detail < 0 ? line : line.Substring(0, detail)).Append('\n');
        }
        return text.ToString();
    }

    // Lone surrogates are converted differently by each engine (Onigwrap goes through UTF-8, so its offsets shift):
    // such discrepancies are one known class per grammar, kept apart from the others.
    public static Difference? Compare(string scope, string[] lines, string branch, string official)
    {
        Difference? difference = Compare(scope, branch, official);
        if (difference != null && !IsCrash(difference.Kind) && !IsTiming(difference.Kind) && lines.Any(HasLoneSurrogate))
        {
            return difference with { Kind = "invalid-utf16", Signature = "invalid-utf16|" + scope };
        }
        return difference;
    }

    public static bool HasLoneSurrogate(string line)
    {
        for (int i = 0; i < line.Length; i++)
        {
            if (char.IsHighSurrogate(line[i]) && i + 1 < line.Length && char.IsLowSurrogate(line[i + 1]))
            {
                i++;
            }
            else if (char.IsSurrogate(line[i]))
            {
                return true;
            }
        }
        return false;
    }

    // Timing-dependent kinds are not minimized: each try may take the whole time limit.
    public static bool IsTiming(string kind) => kind.StartsWith("stopped", StringComparison.Ordinal) || kind == "hang";

    public static Difference? Compare(string scope, string branch, string official)
    {
        string b = Strip(branch);
        string o = Strip(official);
        if (b == o)
        {
            return null;
        }

        Dictionary<string, string> bb = Blocks(b);
        Dictionary<string, string> ob = Blocks(o);
        // The first line that differs names the discrepancy: the lines after it start from different states, so their
        // differences are consequences (after a line that stopped early, say).
        string? kind = null;
        string key = "";
        int first = int.MaxValue;
        int rank = int.MaxValue;
        foreach (string k in bb.Keys.Union(ob.Keys))
        {
            string x = bb.GetValueOrDefault(k, "");
            string y = ob.GetValueOrDefault(k, "");
            if (x == y)
            {
                continue;
            }

            string candidate = Classify(k, x, y);
            int line = k == "load" ? -1 : int.Parse(k.AsSpan(1, k.IndexOf(' ') - 1), System.Globalization.CultureInfo.InvariantCulture);
            int r = Array.IndexOf(Kinds, candidate);
            if (line < first || (line == first && r < rank))
            {
                (first, rank, kind, key) = (line, r, candidate, k);
            }
        }

        kind ??= "tokens";
        string bx = bb.GetValueOrDefault(key, "");
        string ox = ob.GetValueOrDefault(key, "");
        return new Difference(kind, key, bx, ox, Signature(kind, scope, bx, ox));
    }

    private static string Classify(string key, string branch, string official)
    {
        string bh = Header(branch);
        string oh = Header(official);
        if (key == "load")
        {
            return "load";
        }
        bool be = bh.Contains(" EXC ");
        bool oe = oh.Contains(" EXC ");
        if (be && oe)
        {
            return "exception-type";
        }
        if (be)
        {
            return "exception-branch";
        }
        if (oe)
        {
            return "exception-official";
        }
        if (bh.Contains(" stopped") != oh.Contains(" stopped"))
        {
            // The side that ran out of --line-time-limit-ms: the branch doing so is a regression, the official one doing so
            // only means the rest of the line could not be compared.
            return bh.Contains(" stopped") ? "stopped-branch" : "stopped-official";
        }
        return key.EndsWith(" m", StringComparison.Ordinal) ? "metadata" : "tokens";
    }

    private static string Header(string block)
    {
        int end = block.IndexOf('\n');
        return end < 0 ? block : block.Substring(0, end);
    }

    // Splits a runner output into blocks keyed by "#line t" (TokenizeLine) or "#line m" (TokenizeLine2).
    private static Dictionary<string, string> Blocks(string output)
    {
        var blocks = new Dictionary<string, string>(StringComparer.Ordinal);
        string key = "load";
        var block = new StringBuilder();
        foreach (string line in output.Split('\n'))
        {
            if (line.Length == 0)
            {
                continue;
            }
            if (line[0] == '#')
            {
                if (block.Length > 0)
                {
                    blocks[key] = block.ToString();
                }
                block.Clear();
                string[] parts = line.Split(' ', 3);
                key = parts[0] + " " + parts[1];
            }
            block.Append(line).Append('\n');
        }
        if (block.Length > 0)
        {
            blocks[key] = block.ToString();
        }
        return blocks;
    }

    // Groups discrepancies that are likely the same bug: the kind, the grammar and the scopes that differ, without
    // offsets (exceptions: the exception types; metadata: which side has more tokens).
    private static string Signature(string kind, string scope, string branch, string official)
    {
        string what;
        if (kind == "tokens")
        {
            var b = new HashSet<string>(branch.Split('\n').Skip(1).Select(DropOffsets));
            var o = new HashSet<string>(official.Split('\n').Skip(1).Select(DropOffsets));
            what = "+" + string.Join(",", b.Except(o).Order(StringComparer.Ordinal)) + " -" + string.Join(",", o.Except(b).Order(StringComparer.Ordinal));
        }
        else if (kind == "metadata")
        {
            what = Math.Sign(branch.Length - official.Length).ToString();
        }
        else
        {
            what = Header(branch).Split(' ', 3).ElementAtOrDefault(2) + "|" + Header(official).Split(' ', 3).ElementAtOrDefault(2);
        }

        return kind + "|" + scope + "|" + what;
    }

    // "3-7 source.cs keyword" -> the innermost scope only, so the same bug at other places groups together.
    private static string DropOffsets(string tokenLine)
    {
        int space = tokenLine.LastIndexOf(' ');
        return space < 0 ? tokenLine : tokenLine.Substring(space + 1);
    }

    public static string Hash(string text) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text))).Substring(0, 10).ToLowerInvariant();
}
