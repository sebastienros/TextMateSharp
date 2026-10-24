using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace TextMateSharp.Fuzz;

// A grammar bundled in TextMateSharp.Grammars, with the sample and the literals the generator draws from.
public sealed class Target
{
    public required string Scope { get; init; }
    public string? Language { get; init; }
    public required string GrammarFile { get; init; }
    public required string[] Sample { get; init; }
    public required string[] Literals { get; init; }
    public required string[] Openers { get; init; }
    public required string[] Closers { get; init; }
}

public static class Catalog
{
    public static string FindRepository(string? repository)
    {
        if (repository != null)
        {
            return Path.GetFullPath(repository);
        }

        foreach (string start in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
        {
            for (string? dir = start; dir != null; dir = Path.GetDirectoryName(dir))
            {
                if (Directory.Exists(Path.Combine(dir, "src", "TextMateSharp.Grammars", "Resources", "Grammars")))
                {
                    return dir;
                }
            }
        }

        throw new DirectoryNotFoundException("Could not find the TextMateSharp repository; run from it or pass --repo.");
    }

    // Every grammar (scope name) declared by a package.json under src/TextMateSharp.Grammars/Resources/Grammars.
    public static List<Target> Load(string repository)
    {
        string grammars = Path.Combine(repository, "src", "TextMateSharp.Grammars", "Resources", "Grammars");
        string samples = Path.Combine(repository, "src", "TextMateSharp.WasmDemo", "wwwroot", "samples");
        var targets = new Dictionary<string, Target>(StringComparer.Ordinal);
        foreach (string package in Directory.EnumerateFiles(grammars, "package.json", SearchOption.AllDirectories).Order(StringComparer.Ordinal))
        {
            string dir = Path.GetDirectoryName(package)!;
            using JsonDocument json = JsonDocument.Parse(File.ReadAllText(package));
            if (!json.RootElement.TryGetProperty("contributes", out JsonElement contributes) || !contributes.TryGetProperty("grammars", out JsonElement entries))
            {
                continue;
            }

            // A grammar without a sample of its own (an embedded or injected one) borrows its package's samples.
            var packageSamples = new List<string>();
            if (contributes.TryGetProperty("languages", out JsonElement languages))
            {
                foreach (JsonElement language in languages.EnumerateArray())
                {
                    string file = Path.Combine(samples, language.GetProperty("id").GetString() + ".txt");
                    if (File.Exists(file))
                    {
                        packageSamples.AddRange(ReadLines(file));
                    }
                }
            }

            foreach (JsonElement entry in entries.EnumerateArray())
            {
                string scope = entry.GetProperty("scopeName").GetString()!;
                string file = Path.GetFullPath(Path.Combine(dir, entry.GetProperty("path").GetString()!));
                string? language = entry.TryGetProperty("language", out JsonElement id) ? id.GetString() : null;
                if (targets.ContainsKey(scope) || !File.Exists(file))
                {
                    continue;
                }

                string sampleFile = Path.Combine(samples, language + ".txt");
                string[] sample = language != null && File.Exists(sampleFile) ? ReadLines(sampleFile) : packageSamples.ToArray();
                var literals = new HashSet<string>(StringComparer.Ordinal);
                var openers = new HashSet<string>(StringComparer.Ordinal);
                var closers = new HashSet<string>(StringComparer.Ordinal);
                string content = File.ReadAllText(file);
                if (content.TrimStart().StartsWith('<'))
                {
                    WalkPlist(content, literals, openers, closers);
                }
                else
                {
                    using JsonDocument grammar = JsonDocument.Parse(content, new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true });
                    Walk(grammar.RootElement, literals, openers, closers);
                }

                targets[scope] = new Target
                {
                    Scope = scope,
                    Language = language,
                    GrammarFile = Path.GetRelativePath(repository, file),
                    Sample = sample,
                    Literals = Sorted(literals),
                    Openers = Sorted(openers),
                    Closers = Sorted(closers),
                };
            }
        }

        return targets.Values.OrderBy(t => t.Scope, StringComparer.Ordinal).ToList();
    }

    private static string[] ReadLines(string file) => File.ReadAllText(file).Split('\n');

    private static string[] Sorted(HashSet<string> set) => set.Order(StringComparer.Ordinal).Take(4000).ToArray();

    // A .tmLanguage plist: the patterns follow <key>match</key>, <key>begin</key>...
    private static void WalkPlist(string content, HashSet<string> literals, HashSet<string> openers, HashSet<string> closers)
    {
        foreach (Match match in Regex.Matches(content, @"<key>(match|begin|end|while)</key>\s*<string>(.*?)</string>", RegexOptions.Singleline))
        {
            HashSet<string> set = match.Groups[1].Value switch { "match" => literals, "begin" => openers, _ => closers };
            foreach (string literal in Literals(WebUtility.HtmlDecode(match.Groups[2].Value)))
            {
                set.Add(literal);
                literals.Add(literal);
            }
        }
    }

    private static void Walk(JsonElement element, HashSet<string> literals, HashSet<string> openers, HashSet<string> closers)
    {
        if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement item in element.EnumerateArray())
            {
                Walk(item, literals, openers, closers);
            }
        }
        else if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (JsonProperty property in element.EnumerateObject())
            {
                if (property.Value.ValueKind == JsonValueKind.String)
                {
                    HashSet<string>? set = property.Name switch
                    {
                        "match" => literals,
                        "begin" => openers,
                        "end" or "while" => closers,
                        _ => null,
                    };
                    if (set != null)
                    {
                        foreach (string literal in Literals(property.Value.GetString()!))
                        {
                            set.Add(literal);
                            literals.Add(literal);
                        }
                    }
                }
                else
                {
                    Walk(property.Value, literals, openers, closers);
                }
            }
        }
    }

    // The literal runs of an Oniguruma pattern: keywords, operators and delimiters the grammar looks for. A heuristic:
    // character classes, groups' syntax, anchors and quantifiers end a run, escaped punctuation is kept.
    public static IEnumerable<string> Literals(string pattern)
    {
        var run = new StringBuilder();
        var found = new List<string>();
        void Flush()
        {
            if (run.Length > 0 && run.Length <= 40 && run.ToString().Trim().Length > 0)
            {
                found.Add(run.ToString());
            }
            run.Clear();
        }

        for (int i = 0; i < pattern.Length; i++)
        {
            char c = pattern[i];
            switch (c)
            {
                case '\\' when i + 1 < pattern.Length:
                    char next = pattern[++i];
                    if (char.IsLetterOrDigit(next))
                    {
                        Flush();
                        if (next is 'x' or 'u' or 'p' or 'k' or 'g')
                        {
                            // \x{..}, \p{..}, \k<..>, \g<..>
                            while (i + 1 < pattern.Length && (char.IsLetterOrDigit(pattern[i + 1]) || pattern[i + 1] is '{' or '}' or '<' or '>' or '^' or '_'))
                            {
                                i++;
                            }
                        }
                    }
                    else
                    {
                        run.Append(next);
                    }
                    break;
                case '[':
                    Flush();
                    for (i++; i < pattern.Length && pattern[i] != ']'; i++)
                    {
                        if (pattern[i] == '\\')
                        {
                            i++;
                        }
                    }
                    break;
                case '(':
                    Flush();
                    if (i + 1 < pattern.Length && pattern[i + 1] == '?')
                    {
                        i++;
                        while (i + 1 < pattern.Length && pattern[i + 1] is '<' or '=' or '!' or ':' or '>' or '#' or '-' or 'i' or 'm' or 'x' or '~')
                        {
                            i++;
                        }
                    }
                    break;
                case '{' when i + 1 < pattern.Length && char.IsDigit(pattern[i + 1]):
                    Flush();
                    while (i < pattern.Length && pattern[i] != '}')
                    {
                        i++;
                    }
                    break;
                case '*' or '+' or '?':
                    // The quantified character is not part of the literal.
                    if (run.Length > 0)
                    {
                        run.Length--;
                    }
                    Flush();
                    break;
                case ')' or '|' or '.' or '^' or '$':
                    Flush();
                    break;
                default:
                    run.Append(c);
                    break;
            }
        }

        Flush();
        return found;
    }
}
