using System.Globalization;
using System.Runtime.InteropServices;
using TextMateSharp.Fuzz;

// Differential fuzzer: this branch's TextMateSharp against the official release, over every bundled grammar.
//   fuzz   [--state dir] [--seed n] [--workers n] [--duration 10m] [--grammars a,b] [--iterations n] [--hang-seconds 60]
//          [--line-time-limit-ms 5000] [--no-invalid-utf16] [--minimize-budget 400] [--minimize-seconds 60] [--status-seconds 5] [--table-seconds 60] [--repo dir]
//   report [--state dir]
//   repro  <finding.json> [--engine both|branch|official]
string command = args.Length > 0 && !args[0].StartsWith("--", StringComparison.Ordinal) ? args[0] : "fuzz";
string[] rest = command == "fuzz" && (args.Length == 0 || args[0] != "fuzz") ? args : args.Skip(1).ToArray();
var options = new Options();
string? findingPath = null;
string engine = "both";
for (int i = 0; i < rest.Length; i++)
{
    string Value() => i + 1 < rest.Length ? rest[++i] : throw new ArgumentException($"{rest[i]} needs a value");
    switch (rest[i])
    {
        case "--state": options.StateDirectory = Value(); break;
        case "--repo": options.Repository = Value(); break;
        case "--seed": options.Seed = ulong.Parse(Value(), CultureInfo.InvariantCulture); break;
        case "--workers": options.Workers = int.Parse(Value(), CultureInfo.InvariantCulture); break;
        case "--duration": options.Duration = ParseDuration(Value()); break;
        case "--grammars": options.Grammars = Value().Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries); break;
        case "--hang-seconds": options.HangSeconds = int.Parse(Value(), CultureInfo.InvariantCulture); break;
        case "--line-time-limit-ms": options.LineTimeLimitMs = int.Parse(Value(), CultureInfo.InvariantCulture); break;
        case "--minimize-budget": options.MinimizeBudget = int.Parse(Value(), CultureInfo.InvariantCulture); break;
        case "--minimize-seconds": options.MinimizeSeconds = int.Parse(Value(), CultureInfo.InvariantCulture); break;
        case "--status-seconds": options.StatusSeconds = int.Parse(Value(), CultureInfo.InvariantCulture); break;
        case "--table-seconds": options.TableSeconds = int.Parse(Value(), CultureInfo.InvariantCulture); break;
        case "--iterations": options.Iterations = long.Parse(Value(), CultureInfo.InvariantCulture); break;
        case "--no-invalid-utf16": options.InvalidUtf16 = false; break;
        case "--engine": engine = Value(); break;
        default:
            if (command == "repro" && findingPath == null && !rest[i].StartsWith("--", StringComparison.Ordinal))
            {
                findingPath = rest[i];
                break;
            }
            Console.Error.WriteLine($"unknown argument {rest[i]}");
            return 2;
    }
}

string runners = Path.Combine(AppContext.BaseDirectory, "runners");
var branch = new Engine("branch", Path.Combine(runners, "branch"));
var official = new Engine("official", Path.Combine(runners, "official"));

switch (command)
{
    case "fuzz":
    {
        string repository = Catalog.FindRepository(options.Repository);
        List<Target> targets = Catalog.Load(repository);
        var campaign = new Campaign(options, branch, official, targets);
        Console.WriteLine($"branch:   {branch.Version}");
        Console.WriteLine($"official: {official.Version}");
        Console.WriteLine($"{(campaign.Resumed ? "resuming" : "starting")} campaign in {Path.GetFullPath(options.StateDirectory)}: seed {campaign.Seed}, {campaign.Grammars} grammars, {options.Workers} workers; Ctrl+C stops and saves");
        Console.CancelKeyPress += (_, e) =>
        {
            if (!campaign.Stopping)
            {
                // The first Ctrl+C finishes the running iterations and saves; a second one kills the process.
                e.Cancel = true;
                Console.WriteLine("stopping: finishing the running iterations and saving the state (Ctrl+C again to kill)");
                campaign.Stop();
            }
        };
        // SIGTERM (a service stop, `kill`) stops cleanly too.
        using PosixSignalRegistration terminate = PosixSignalRegistration.Create(PosixSignal.SIGTERM, context =>
        {
            context.Cancel = true;
            campaign.Stop();
        });
        campaign.Run();
        Console.WriteLine($"report: {Path.Combine(Path.GetFullPath(options.StateDirectory), "findings.md")}");
        return 0;
    }
    case "report":
    {
        string state = Path.GetFullPath(options.StateDirectory);
        CampaignState campaign = Store.Read<CampaignState>(Path.Combine(state, "campaign.json")) ?? throw new FileNotFoundException("no campaign.json", state);
        string dir = Path.Combine(state, "findings");
        IEnumerable<Finding> findings = Directory.Exists(dir) ? Directory.EnumerateFiles(dir, "*.json").Select(f => Store.Read<Finding>(f)!) : [];
        Report.Write(state, campaign, findings);
        Console.WriteLine(File.ReadAllText(Path.Combine(state, "findings.md")));
        return 0;
    }
    case "repro":
    {
        Finding finding = Store.Read<Finding>(findingPath ?? throw new ArgumentException("repro needs a finding file")) ?? throw new FileNotFoundException(findingPath);
        string[] lines = finding.Input.ToArray();
        Console.WriteLine($"{finding.Kind} in {finding.Scope}, {lines.Length} line(s)");
        string? b = engine is "both" or "branch" ? branch.CreateSession()(finding.Scope, lines, options.LineTimeLimitMs) : null;
        string? o = engine is "both" or "official" ? official.CreateSession()(finding.Scope, lines, options.LineTimeLimitMs) : null;
        if (b == null || o == null)
        {
            Console.WriteLine(b ?? o);
            return 0;
        }

        Difference? difference = Comparison.Compare(finding.Scope, lines, b, o);
        if (difference == null)
        {
            Console.WriteLine("no longer reproduces: both outputs are identical");
            return 0;
        }

        Console.WriteLine($"reproduces: {difference.Kind} at {difference.Key}");
        Console.WriteLine("--- branch");
        Console.WriteLine(Comparison.Strip(difference.Branch).TrimEnd('\n'));
        Console.WriteLine("--- official");
        Console.WriteLine(Comparison.Strip(difference.Official).TrimEnd('\n'));
        return 1;
    }
    default:
        Console.Error.WriteLine($"unknown command {command}; use fuzz, report or repro");
        return 2;
}

static TimeSpan ParseDuration(string text)
{
    char unit = text[^1];
    double value = double.Parse(char.IsDigit(unit) ? text : text[..^1], CultureInfo.InvariantCulture);
    return unit switch
    {
        'h' => TimeSpan.FromHours(value),
        'm' => TimeSpan.FromMinutes(value),
        _ => TimeSpan.FromSeconds(value),
    };
}
