using System.Collections.Concurrent;
using System.Reflection;
using TextMateSharp.Grammars;

namespace TextMateSharp.CorpusCheck;

// Compiled twice: against the official NuGet packages (OFFICIAL) and against this branch's projects.
public sealed class Engine : IEngine
{
#if OFFICIAL
    public string Name => "official";
#else
    public string Name => "branch";
#endif

    private readonly RegistryOptions _options = new(ThemeName.DarkPlus);
    private readonly TextMateSharp.Registry.Registry _registry;

    public Engine()
    {
        _registry = new TextMateSharp.Registry.Registry(_options);
    }

    public string Version
        => typeof(IGrammar).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
           ?? typeof(IGrammar).Assembly.GetName().Version?.ToString() ?? "?";

    public IEngineGrammar? LoadGrammar(string scopeName)
    {
        IGrammar? grammar = _registry.LoadGrammar(scopeName);
        return grammar == null ? null : new EngineGrammar(grammar);
    }

    private sealed class EngineGrammar(IGrammar grammar) : IEngineGrammar
    {
        // StoppedEarly is public on the internal result classes only.
        private static readonly ConcurrentDictionary<Type, PropertyInfo?> StoppedEarlyProperties = new();

        public LineTokens TokenizeLine(string line, object? state, TimeSpan timeLimit)
        {
            ITokenizeLineResult result = grammar.TokenizeLine(line, (IStateStack?)state, timeLimit);
            var tokens = new Token[result.Tokens.Length];
            for (int i = 0; i < tokens.Length; i++)
            {
                IToken token = result.Tokens[i];
                tokens[i] = new Token(token.StartIndex, token.EndIndex, string.Join(' ', token.Scopes));
            }
            return new LineTokens(tokens, result.RuleStack, StoppedEarly(result));
        }

        public LineTokens2 TokenizeLine2(string line, object? state, TimeSpan timeLimit)
        {
            ITokenizeLineResult2 result = grammar.TokenizeLine2(line, (IStateStack?)state, timeLimit);
            return new LineTokens2(result.Tokens, result.RuleStack, StoppedEarly(result));
        }

        private static bool StoppedEarly(object result)
            => StoppedEarlyProperties.GetOrAdd(result.GetType(), t => t.GetProperty("StoppedEarly"))?.GetValue(result) is true;
    }
}
