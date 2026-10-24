namespace TextMateSharp.CorpusCheck;

public sealed class Report
{
    public required string OfficialVersion { get; init; }
    public required string BranchVersion { get; init; }
    public required string Machine { get; init; }
    public required string Runtime { get; init; }
    public required int Threads { get; init; }
    public required DateTimeOffset StartedAt { get; init; }
    public double DurationSeconds { get; set; }
    public required List<CorpusReport> Corpora { get; init; }
}

public sealed class CorpusReport
{
    public required string Name { get; init; }
    public required string Scope { get; init; }
    public required string Repo { get; init; }
    public required string Commit { get; init; }
    public string? Error { get; set; }
    public int Files;
    public int FilesSkipped;
    public int FilesWithMismatch;
    public long Bytes;
    public long Lines;
    public long LinesMatching;
    public long LinesMismatching;
    public long LinesTokensMismatching;
    public long LinesMetadataMismatching;
    public EngineStats Official { get; } = new();
    public EngineStats Branch { get; } = new();
    public List<SkippedFile> Skipped { get; } = [];
    public List<EngineError> Errors { get; } = [];
    public List<Discrepancy> Discrepancies { get; } = [];
    public int DiscrepanciesNotRecorded;

    // For the progress display only.
    [System.Text.Json.Serialization.JsonIgnore] public int FilesTotal;
    [System.Text.Json.Serialization.JsonIgnore] public int FilesDone;
}

public sealed class EngineStats
{
    public long Tokens;
    public long Ticks;
    public long Tokens2;
    public long Ticks2;
    public long Timeouts;
    public long ErrorCount;
    public double Seconds => (double)Ticks / System.Diagnostics.Stopwatch.Frequency;
    public double Seconds2 => (double)Ticks2 / System.Diagnostics.Stopwatch.Frequency;
    public double TokensPerSecond => Seconds > 0 ? Tokens / Seconds : 0;
    public double Tokens2PerSecond => Seconds2 > 0 ? Tokens2 / Seconds2 : 0;
}

public sealed record SkippedFile(string File, string Reason);

public sealed record EngineError(string Engine, string File, int Line, string Method, string Message);

public sealed class Discrepancy
{
    public required string File { get; init; }

    // 1-based.
    public required int Line { get; init; }
    public required string Text { get; init; }

    // TokenizeLine, TokenizeLine2 or both.
    public required string Kind { get; init; }

    // Either engine stopped early (time limit) on this line.
    public bool StoppedEarly { get; init; }

    // Tokenizing ContextLines from the initial state (null) reproduces the discrepancy on their last line, which is Text.
    // When false, no suffix of up to 512 lines reproduces it: tokenize the file from its first line.
    public required bool ReproducesFromContext { get; init; }
    public required int ContextStartLine { get; init; }
    public required string[] ContextLines { get; init; }
    public required Token[] OfficialTokens { get; init; }
    public required Token[] BranchTokens { get; init; }
    public required int[] OfficialTokens2 { get; init; }
    public required int[] BranchTokens2 { get; init; }
}
