using System.Text;

namespace TextMateSharp.Fuzz;

// Builds the input of one iteration: a function of the seed, the grammar, the iteration number and the grammar's
// corpus, so a campaign replays (and resumes) identically.
public static class Generator
{
    private static readonly string[] Generic =
    [
        " ", "  ", "\t", "{", "}", "(", ")", "[", "]", "<", ">", "\"", "'", "`", "/*", "*/", "//", "#", "--", "<!--", "-->",
        "\\", "\\\\", "\\\"", "$", "${", "@", "%", "=", "==", "=>", "->", ";", ",", ".", "...", ":", "::", "|", "&", "&&",
        "*", "+", "-", "!", "?", "~", "^", "\r", "0x1F", "1e-9", "_", "\"\"\"", "'''", "```", "<?", "?>", "</", "/>",
        "\u00E9", "\u00DF", "\u4E2D\u6587", "\U0001F600", "\u200B", "\u202E", "\uFEFF", "\U0001D4B3", "\u03A9", "e\u0301", "\u0130", "\uFB03", "\u00A0", "\u2028", "\u0000",
    ];

    // Lone surrogates: invalid UTF-16 that each engine may treat differently when it converts the line.
    private static readonly string[] Broken = ["\uD800", "\uDFFF", "a\uD83D", "\uDE00b"];

    public static ulong Mix(ulong seed, string scope, long iteration)
    {
        ulong h = 14695981039346656037UL ^ seed;
        foreach (char c in scope)
        {
            h = (h ^ c) * 1099511628211UL;
        }
        h ^= (ulong)iteration * 0x9E3779B97F4A7C15UL;
        h ^= h >> 33;
        h *= 0xFF51AFD7ED558CCDUL;
        h ^= h >> 33;
        return h;
    }

    public static string[] Generate(ulong seed, Target target, long iteration, IReadOnlyList<string[]> corpus, bool invalidUtf16)
    {
        var random = new Random((int)(Mix(seed, target.Scope, iteration) & 0x7FFFFFFF));
        var g = new Gen(random, target, corpus, invalidUtf16);
        List<string> lines = random.Next(100) switch
        {
            < 40 => g.Mutate(g.Slice(g.Source())),
            < 55 => g.Mutate(g.Splice()),
            < 70 => g.Fragments(),
            < 82 => g.Nesting(),
            < 90 => g.LongLine(),
            _ => g.Mutate(g.Fragments()),
        };
        if (lines.Count == 0)
        {
            lines.Add("");
        }
        // A line never holds a line break: the tokenizer is fed line by line.
        return lines.Select(l => l.Replace('\n', ' ')).ToArray();
    }

    private sealed class Gen(Random random, Target target, IReadOnlyList<string[]> corpus, bool invalidUtf16)
    {
        private T Pick<T>(IReadOnlyList<T> items) => items[random.Next(items.Count)];

        public string[] Source()
        {
            if (corpus.Count > 0 && (target.Sample.Length == 0 || random.Next(3) == 0))
            {
                return Pick(corpus);
            }
            return target.Sample.Length > 0 ? target.Sample : [Atom()];
        }

        public List<string> Slice(string[] source)
        {
            int count = Math.Min(source.Length, random.Next(4) == 0 ? random.Next(1, 120) : random.Next(1, 16));
            int start = random.Next(source.Length - count + 1);
            return source.Skip(start).Take(count).ToList();
        }

        public List<string> Splice()
        {
            List<string> a = Slice(Source());
            List<string> b = Slice(Source());
            a.InsertRange(random.Next(a.Count + 1), b);
            return a;
        }

        // A keyword, a delimiter or an opener/closer of the grammar, or a generic delimiter or unicode oddity.
        public string Atom()
        {
            int r = random.Next(100);
            if (r < 35 && target.Literals.Length > 0)
            {
                return Pick(target.Literals);
            }
            if (r < 50 && target.Openers.Length > 0)
            {
                return Pick(target.Openers);
            }
            if (r < 60 && target.Closers.Length > 0)
            {
                return Pick(target.Closers);
            }
            if (r < 61 && invalidUtf16)
            {
                return Pick(Broken);
            }
            if (r < 70)
            {
                return Word();
            }
            return Pick(Generic);
        }

        private string Word()
        {
            var text = new StringBuilder();
            int length = random.Next(1, 10);
            for (int i = 0; i < length; i++)
            {
                text.Append((char)(random.Next(4) == 0 ? '0' + random.Next(10) : 'a' + random.Next(26)));
            }
            return text.ToString();
        }

        public List<string> Fragments()
        {
            var lines = new List<string>();
            int count = random.Next(1, 12);
            for (int l = 0; l < count; l++)
            {
                var line = new StringBuilder();
                int atoms = random.Next(0, 16);
                for (int a = 0; a < atoms; a++)
                {
                    line.Append(Atom());
                    if (random.Next(3) == 0)
                    {
                        line.Append(' ');
                    }
                }
                lines.Add(line.ToString());
            }
            return lines;
        }

        // Openers repeated deeply, across one line or many (multi-line state carry), closed or not.
        public List<string> Nesting()
        {
            string opener = target.Openers.Length > 0 && random.Next(3) != 0 ? Pick(target.Openers) : Pick(Generic);
            string closer = target.Closers.Length > 0 && random.Next(3) != 0 ? Pick(target.Closers) : Pick(Generic);
            int depth = random.Next(10) == 0 ? random.Next(50, 200) : random.Next(2, 50);
            bool multiLine = random.Next(2) == 0;
            var lines = new List<string>();
            var line = new StringBuilder();
            for (int i = 0; i < depth; i++)
            {
                line.Append(opener);
                if (random.Next(4) == 0)
                {
                    line.Append(Atom());
                }
                if (multiLine)
                {
                    lines.Add(line.ToString());
                    line.Clear();
                }
            }
            int closing = random.Next(3) == 0 ? random.Next(depth + 1) : depth;
            for (int i = 0; i < closing; i++)
            {
                line.Append(closer);
                if (multiLine && random.Next(2) == 0)
                {
                    lines.Add(line.ToString());
                    line.Clear();
                }
            }
            lines.Add(line.ToString());
            return lines;
        }

        public List<string> LongLine()
        {
            List<string> slice = Slice(Source());
            string unit = random.Next(2) == 0 && slice.Count > 0 ? Pick(slice) + Atom() : Atom() + " ";
            var line = new StringBuilder();
            // Mostly under a thousand characters: the official tokenizer is quadratic on a line's length, and its time
            // limit runs out on longer ones, which then compare nothing past that point.
            int target = random.Next(50) == 0 ? random.Next(2000, 5000) : random.Next(100, 1000);
            while (line.Length < target)
            {
                line.Append(unit.Length == 0 ? " " : unit);
            }
            slice.Insert(random.Next(slice.Count + 1), line.ToString());
            return slice;
        }

        public List<string> Mutate(List<string> lines)
        {
            int count = random.Next(1, 9);
            for (int m = 0; m < count && lines.Count > 0; m++)
            {
                int index = random.Next(lines.Count);
                string line = lines[index];
                int at = random.Next(line.Length + 1);
                switch (random.Next(12))
                {
                    case 0 or 1 or 2:
                        lines[index] = line.Insert(at, Atom());
                        break;
                    case 3 when line.Length > 0:
                        at = Math.Min(at, line.Length - 1);
                        lines[index] = line.Remove(at, random.Next(1, Math.Min(line.Length - at, 32) + 1));
                        break;
                    case 4 when line.Length > 0:
                        int from = random.Next(line.Length);
                        int span = random.Next(1, Math.Min(line.Length - from, 64) + 1);
                        lines[index] = line.Insert(at, line.Substring(from, span));
                        break;
                    case 5:
                        lines.Insert(index, line);
                        break;
                    case 6 when lines.Count > 1:
                        lines.RemoveAt(index);
                        break;
                    case 7 when lines.Count > 1:
                        int other = random.Next(lines.Count);
                        (lines[index], lines[other]) = (lines[other], lines[index]);
                        break;
                    case 8:
                        lines[index] = line.Substring(0, at);
                        lines.Insert(index + 1, line.Substring(at));
                        break;
                    case 9 when index + 1 < lines.Count:
                        lines[index] = line + lines[index + 1];
                        lines.RemoveAt(index + 1);
                        break;
                    case 10 when line.Length > 0:
                        lines[index] = line.Substring(0, at) + (char)random.Next(0x20, 0x7F) + line.Substring(Math.Min(at + 1, line.Length));
                        break;
                    default:
                        lines.Insert(index, string.Concat(Enumerable.Range(0, random.Next(1, 6)).Select(_ => Atom())));
                        break;
                }
            }
            return lines;
        }
    }
}
