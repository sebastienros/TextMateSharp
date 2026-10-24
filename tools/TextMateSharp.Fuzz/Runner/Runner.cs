using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TextMateSharp.Grammars;
using TextMateSharp.Registry;

namespace TextMateSharp.Fuzz.Runner
{
    // Compiled twice: against this branch's TextMateSharp and against the official NuGet release. The host loads each
    // build in its own AssemblyLoadContext and talks to it only through CoreLib types (string, string[], int).
    public sealed class Session
    {
        // Text after this character on a line is a detail (an exception message) that the host does not compare.
        public const char Detail = '\u0001';

        // Deeply nested scopes on long lines can make the text huge: past this length the rest is dropped, on both
        // sides at the same point, so what is kept still compares.
        public const int MaxOutput = 16 * 1024 * 1024;

        private readonly RegistryOptions _options = new RegistryOptions(ThemeName.DarkPlus);
        private readonly Registry.Registry _registry;
        private readonly Dictionary<string, IGrammar?> _grammars = new Dictionary<string, IGrammar?>();
        private readonly Dictionary<Type, PropertyInfo?> _stoppedEarly = new Dictionary<Type, PropertyInfo?>();

        public Session()
        {
            _registry = new Registry.Registry(_options);
        }

        // TextMateSharp's version and the regex engine it references (GregsEx or Onigwrap).
        public static string Version
        {
            get
            {
                Assembly textMate = typeof(IGrammar).Assembly;
                var names = new List<string> { "TextMateSharp " + textMate.GetName().Version };
                foreach (AssemblyName reference in textMate.GetReferencedAssemblies())
                {
                    if (reference.Name != "netstandard" && !reference.Name!.StartsWith("System", StringComparison.Ordinal))
                    {
                        names.Add(reference.Name + " " + reference.Version);
                    }
                }
                return string.Join(", ", names);
            }
        }

        // Tokenizes the lines with TokenizeLine and TokenizeLine2 (two independent state chains) and returns a canonical
        // text of every token, its scopes, its metadata and whether the line stopped early.
        public string Run(string scopeName, string[] lines, int timeLimitMs)
        {
            var text = new StringBuilder();
            IGrammar? grammar;
            try
            {
                if (!_grammars.TryGetValue(scopeName, out grammar))
                {
                    grammar = _registry.LoadGrammar(scopeName);
                    _grammars[scopeName] = grammar;
                }
            }
            catch (Exception e)
            {
                _grammars[scopeName] = null;
                return "LOAD " + e.GetType().FullName + Detail + Flatten(e) + "\n";
            }

            if (grammar == null)
            {
                return "LOAD null\n";
            }

            TimeSpan limit = timeLimitMs <= 0 ? TimeSpan.MaxValue : TimeSpan.FromMilliseconds(timeLimitMs);
            IStateStack? state1 = null;
            IStateStack? state2 = null;
            bool dead1 = false;
            bool dead2 = false;
            for (int i = 0; i < lines.Length; i++)
            {
                if (text.Length > MaxOutput)
                {
                    text.Append("#").Append(i).Append(" truncated\n");
                    break;
                }

                if (!dead1)
                {
                    try
                    {
                        ITokenizeLineResult result = grammar.TokenizeLine(lines[i], state1!, limit);
                        state1 = result.RuleStack;
                        text.Append('#').Append(i).Append(" t").Append(StoppedEarly(result) ? " stopped" : "").Append('\n');
                        foreach (IToken token in result.Tokens)
                        {
                            if (text.Length > MaxOutput)
                            {
                                text.Append("truncated\n");
                                break;
                            }
                            text.Append(token.StartIndex).Append('-').Append(token.EndIndex).Append(' ').AppendJoin(' ', token.Scopes).Append('\n');
                        }
                    }
                    catch (Exception e)
                    {
                        dead1 = true;
                        text.Append('#').Append(i).Append(" t EXC ").Append(e.GetType().FullName).Append(Detail).Append(Flatten(e)).Append('\n');
                    }
                }

                if (!dead2)
                {
                    try
                    {
                        ITokenizeLineResult2 result = grammar.TokenizeLine2(lines[i], state2!, limit);
                        state2 = result.RuleStack;
                        text.Append('#').Append(i).Append(" m").Append(StoppedEarly(result) ? " stopped" : "").Append(' ').Append(string.Join(",", result.Tokens)).Append('\n');
                    }
                    catch (Exception e)
                    {
                        dead2 = true;
                        text.Append('#').Append(i).Append(" m EXC ").Append(e.GetType().FullName).Append(Detail).Append(Flatten(e)).Append('\n');
                    }
                }
            }

            return text.ToString();
        }

        // StoppedEarly is public on one result type and internal on the other, so it is read by reflection.
        private bool StoppedEarly(object result)
        {
            Type type = result.GetType();
            if (!_stoppedEarly.TryGetValue(type, out PropertyInfo? property))
            {
                property = type.GetProperty("StoppedEarly", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                _stoppedEarly[type] = property;
            }

            return property != null && (bool)property.GetValue(result)!;
        }

        private static string Flatten(Exception e) => (e.Message + " @ " + e.StackTrace).Replace('\n', ' ').Replace('\r', ' ');
    }
}
