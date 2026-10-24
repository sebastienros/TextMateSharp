using NUnit.Framework;
using System;
using TextMateSharp.Internal.Oniguruma;
using TextMateSharp.Internal.Rules;

namespace TextMateSharp.Tests.Internal.Rules
{
    [TestFixture]
    public class RegExpSourceTests
    {
        [Test]
        public void ResolveBackReferences_EscapesTheCapture()
        {
            string resolved = Resolve(@"\1;", "a.b", new OnigCaptureIndex(1, 0, 3));

            Assert.AreEqual(@"a\.b;", resolved);
        }

        [Test]
        public void ResolveBackReferences_KeepsASurrogatePair()
        {
            string resolved = Resolve(@"\1", "x😀", new OnigCaptureIndex(1, 1, 3));

            Assert.AreEqual("😀", resolved);
        }

        [Test]
        public void ResolveBackReferences_ReplacesALoneSurrogateWithAPatternThatCompiles()
        {
            string line = "\uD800 \uDE00";
            string resolved = Resolve(@"\1-\2", line, new OnigCaptureIndex(1, 0, 1), new OnigCaptureIndex(2, 2, 3));

            Assert.AreEqual(@"(?:\p{Cs}|\x{FFFD})-(?:\p{Cs}|\x{FFFD})", resolved);

            // the lone low surrogate in the line and U+FFFD both match; the pattern no longer throws
            var scanner = new OnigScanner(new[] { Resolve(@"\1", line, new OnigCaptureIndex(1, 2, 3)) });
            Assert.AreEqual(2, scanner.FindNextMatchSync(line, 0).GetCaptureIndices()[0].Start);
            Assert.IsNotNull(scanner.FindNextMatchSync("�", 0));
        }

        static string Resolve(string source, string line, params OnigCaptureIndex[] captures)
        {
            var all = new OnigCaptureIndex[captures.Length + 1];
            all[0] = new OnigCaptureIndex(0, 0, line.Length);
            captures.CopyTo(all, 1);
            return new RegExpSource(source, RuleId.Of(1)).ResolveBackReferences(line.AsMemory(), all);
        }
    }
}
