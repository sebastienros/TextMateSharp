using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TextMateSharp.Fuzz;

public sealed class CampaignState
{
    public ulong Seed { get; set; }
    public DateTime Created { get; set; }
    public double Seconds { get; set; }
    public string BranchVersion { get; set; } = "";
    public string OfficialVersion { get; set; } = "";
    public SortedDictionary<string, GrammarState> Grammars { get; set; } = new(StringComparer.Ordinal);
}

public sealed class GrammarState
{
    // The next iteration to generate; iterations below it are done.
    public long Next { get; set; }
    public long Execs { get; set; }
    public double BusySeconds { get; set; }
    public long Discrepancies { get; set; }
    public long Crashes { get; set; }
    // Iterations that hung or killed the process, not replayed on resume.
    public List<long> Skip { get; set; } = [];
}

// The inputs that reached scopes nobody reached before, mutated by later iterations.
public sealed class CorpusState
{
    public List<string> Scopes { get; set; } = [];
    public List<Lines> Entries { get; set; } = [];
}

public sealed class Finding
{
    public string Id { get; set; } = "";
    public string Kind { get; set; } = "";
    public string Scope { get; set; } = "";
    public string? Language { get; set; }
    public string Signature { get; set; } = "";
    public List<string> RawSignatures { get; set; } = [];
    public long Hits { get; set; }
    public ulong Seed { get; set; }
    public long Iteration { get; set; }
    public DateTime FirstSeen { get; set; }
    public bool Minimized { get; set; }
    public Lines Input { get; set; } = new();
    public Lines? Original { get; set; }
    public string Key { get; set; } = "";
    public string Branch { get; set; } = "";
    public string Official { get; set; } = "";
    public string BranchOutput { get; set; } = "";
    public string OfficialOutput { get; set; } = "";
}

// The lines of an input. JSON cannot hold lone surrogates, so a line that has one is also kept as UTF-16 hex.
public sealed class Lines
{
    public string[] Text { get; set; } = [];

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string?[]? Utf16 { get; set; }

    public static Lines From(string[] lines)
    {
        var result = new Lines { Text = lines.Select(Readable).ToArray() };
        if (lines.Any(l => l != Readable(l)))
        {
            result.Utf16 = lines.Select(l => l == Readable(l) ? null : Convert.ToHexString(Utf16Bytes(l))).ToArray();
        }
        return result;
    }

    public string[] ToArray()
    {
        if (Utf16 == null)
        {
            return Text;
        }
        return Text.Select((t, i) => Utf16[i] is string hex ? FromUtf16Bytes(Convert.FromHexString(hex)) : t).ToArray();
    }

    // The line with each lone surrogate replaced by U+FFFD, so it can be written as UTF-8.
    public static string Readable(string line)
    {
        var text = new StringBuilder(line.Length);
        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (char.IsHighSurrogate(c) && i + 1 < line.Length && char.IsLowSurrogate(line[i + 1]))
            {
                text.Append(c).Append(line[++i]);
            }
            else
            {
                text.Append(char.IsSurrogate(c) ? '\uFFFD' : c);
            }
        }
        return text.ToString();
    }

    // Encoding.Unicode would replace the lone surrogates this exists to keep.
    private static string FromUtf16Bytes(byte[] bytes)
    {
        var chars = new char[bytes.Length / 2];
        for (int i = 0; i < chars.Length; i++)
        {
            chars[i] = (char)(bytes[2 * i] | (bytes[2 * i + 1] << 8));
        }
        return new string(chars);
    }

    private static byte[] Utf16Bytes(string line)
    {
        var bytes = new byte[line.Length * 2];
        for (int i = 0; i < line.Length; i++)
        {
            bytes[2 * i] = (byte)line[i];
            bytes[2 * i + 1] = (byte)(line[i] >> 8);
        }
        return bytes;
    }
}

public static class Store
{
    public static readonly JsonSerializerOptions Options = new() { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

    public static T? Read<T>(string path) => File.Exists(path) ? JsonSerializer.Deserialize<T>(File.ReadAllText(path), Options) : default;

    // Writes through a temporary file so a kill never leaves a half-written state.
    public static void Write<T>(string path, T value)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        string temp = path + ".tmp";
        File.WriteAllText(temp, JsonSerializer.Serialize(value, Options));
        File.Move(temp, path, overwrite: true);
    }

    public static string FileName(string scope) => string.Concat(scope.Select(c => char.IsLetterOrDigit(c) || c is '.' or '-' or '_' ? c : '_'));
}
