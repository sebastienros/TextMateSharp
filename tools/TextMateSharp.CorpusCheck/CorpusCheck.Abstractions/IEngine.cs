namespace TextMateSharp.CorpusCheck;

// One TextMateSharp build (the official NuGet release or this branch), loaded in its own AssemblyLoadContext.
// An engine owns a registry; its grammars must be used by one thread at a time.
public interface IEngine
{
    string Name { get; }

    // The TextMateSharp assembly's informational version.
    string Version { get; }

    IEngineGrammar? LoadGrammar(string scopeName);
}

public interface IEngineGrammar
{
    // IGrammar.TokenizeLine; state is the previous line's RuleStack (null for the first line).
    LineTokens TokenizeLine(string line, object? state, TimeSpan timeLimit);

    // IGrammar.TokenizeLine2: pairs of (start offset, metadata).
    LineTokens2 TokenizeLine2(string line, object? state, TimeSpan timeLimit);
}

public readonly record struct Token(int Start, int End, string Scopes);

public sealed record LineTokens(Token[] Tokens, object State, bool StoppedEarly);

public sealed record LineTokens2(int[] Tokens, object State, bool StoppedEarly);
