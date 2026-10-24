using System.Diagnostics;
using System.IO.Enumeration;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TextMateSharp.CorpusCheck;

// Tokenizes real-world corpora with the official TextMateSharp and this branch, side by side in one process,
// and reports every line whose tokens (TokenizeLine) or metadata (TokenizeLine2) differ.
public static class Program
{
    public static int Main(string[] args)
    {
        Options options;
        try
        {
            options = Options.Parse(args);
        }
        catch (ArgumentException e)
        {
            Console.Error.WriteLine(e.Message);
            Console.Error.WriteLine(Options.Usage);
            return 2;
        }

        if (options.Help)
        {
            Console.WriteLine(Options.Usage);
            return 0;
        }

        Manifest manifest = Manifest.Load(options.Manifest);
        var corpora = manifest.Corpora.Where(c => options.Only.Count == 0 || options.Only.Contains(c.Name)).ToList();
        if (corpora.Count == 0)
        {
            Console.Error.WriteLine("No corpus selected.");
            return 2;
        }

        string adapters = Path.Combine(AppContext.BaseDirectory, "adapters");
        var official = EngineLoader.Load(Path.Combine(adapters, "official", "CorpusCheck.Adapter.Official.dll"));
        var branch = EngineLoader.Load(Path.Combine(adapters, "branch", "CorpusCheck.Adapter.Branch.dll"));

        var report = new Checker(options, corpora, official, branch).Run();

        Directory.CreateDirectory(options.Output);
        string json = Path.Combine(options.Output, "report.json");
        string markdown = Path.Combine(options.Output, "report.md");
        File.WriteAllText(json, JsonSerializer.Serialize(report, Json.Options));
        File.WriteAllText(markdown, MarkdownReport.Render(report));
        Console.WriteLine($"Wrote {json} and {markdown}");
        return report.Corpora.All(c => c.LinesMismatching == 0 && c.Errors.Count == 0) ? 0 : 1;
    }
}

public sealed class Options
{
    public const string Usage = """
        Usage: TextMateSharp.CorpusCheck [options]
          --corpus <dir>          corpora fetched by fetch-corpus.ps1 (default: ./corpus)
          --manifest <file>       corpora.json (default: next to the executable)
          --out <dir>             report.json and report.md (default: ./out)
          --only <a,b,...>        corpus names to check (default: all)
          --max-files <n>         first files per corpus, by path (default: all)
          --max-lines <n>         lines per file (default: all)
          --max-line-length <n>   skip files with a longer line, e.g. minified (default: 10000)
          --threads <n>           worker threads (default: processor count)
          --time-limit-ms <n>     TokenizeLine time limit per line (default: 10000)
          --max-repro <n>         discrepancies recorded per file (default: 3); 100x that per corpus
        """;

    public string Corpus { get; private set; } = Path.GetFullPath("corpus");
    public string Manifest { get; private set; } = Path.Combine(AppContext.BaseDirectory, "corpora.json");
    public string Output { get; private set; } = Path.GetFullPath("out");
    public HashSet<string> Only { get; } = new(StringComparer.OrdinalIgnoreCase);
    public int MaxFiles { get; private set; } = int.MaxValue;
    public int MaxLines { get; private set; } = int.MaxValue;
    public int MaxLineLength { get; private set; } = 10_000;
    public int Threads { get; private set; } = Environment.ProcessorCount;
    public TimeSpan TimeLimit { get; private set; } = TimeSpan.FromSeconds(10);
    public int MaxReproPerFile { get; private set; } = 3;
    public bool Help { get; private set; }

    public static Options Parse(string[] args)
    {
        var options = new Options();
        for (int i = 0; i < args.Length; i++)
        {
            string Value() => ++i < args.Length ? args[i] : throw new ArgumentException($"{args[i - 1]} needs a value");
            int Int() => int.TryParse(Value(), out int n) && n > 0 ? n : throw new ArgumentException($"{args[i - 1]} needs a positive integer");
            switch (args[i])
            {
                case "--corpus": options.Corpus = Path.GetFullPath(Value()); break;
                case "--manifest": options.Manifest = Path.GetFullPath(Value()); break;
                case "--out": options.Output = Path.GetFullPath(Value()); break;
                case "--only": options.Only.UnionWith(Value().Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)); break;
                case "--max-files": options.MaxFiles = Int(); break;
                case "--max-lines": options.MaxLines = Int(); break;
                case "--max-line-length": options.MaxLineLength = Int(); break;
                case "--threads": options.Threads = Int(); break;
                case "--time-limit-ms": options.TimeLimit = TimeSpan.FromMilliseconds(Int()); break;
                case "--max-repro": options.MaxReproPerFile = Int(); break;
                case "-h" or "--help": options.Help = true; break;
                default: throw new ArgumentException($"Unknown option {args[i]}");
            }
        }
        return options;
    }
}

public sealed record Manifest(List<CorpusDefinition> Corpora)
{
    public static Manifest Load(string path)
        => JsonSerializer.Deserialize<Manifest>(File.ReadAllText(path), Json.Options) ?? throw new InvalidDataException(path);
}

public sealed record CorpusDefinition(string Name, string Scope, string Repo, string Commit, List<string> Sparse, List<string> Files)
{
    public bool Matches(string fileName)
        => Files.Any(p => FileSystemName.MatchesSimpleExpression(p, fileName, ignoreCase: false));
}

public static class Json
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        IncludeFields = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };
}
