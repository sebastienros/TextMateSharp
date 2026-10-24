# TextMateSharp.CorpusCheck

Checks that this branch's TextMateSharp (GregsEx) produces the same tokens as the official release
(TextMateSharp + TextMateSharp.Grammars 2.0.4 from nuget.org, Onigwrap) on real-world code, one corpus per bundled grammar.

## How it works

- `CorpusCheck.Adapter.Official` and `CorpusCheck.Adapter.Branch` compile the same `Adapter/Engine.cs` against the NuGet
  packages and against `src/`. Building `CorpusCheck` publishes both under `bin/<config>/net10.0/adapters/`, and the
  checker loads each in its own `AssemblyLoadContext` (they share the `TextMateSharp` assembly identity), with only
  `CorpusCheck.Abstractions` shared. The official adapter's native `onigwrap` comes from its `runtimes/<rid>/native`.
- Files run in parallel, largest first; each thread has its own registry and grammars per engine, since a grammar
  tokenizes one line at a time. Each file is tokenized line by line (without the line terminator, carrying the state)
  with `TokenizeLine` and `TokenizeLine2` on both engines, and every line's offsets, scopes and metadata are compared.
- For a differing line, the report keeps the file, line number, text, both outputs, and the shortest suffix of
  preceding lines (0, 1, 2, 4, … up to 512) that reproduces it from the initial state, ready to become a test case.
- Errors (exceptions) stop the file and are reported per engine; lines where an engine hits the time limit count as timeouts.
- Tokens per second: `TokenizeLine` tokens over the time spent in `TokenizeLine`, per engine. Both engines run interleaved
  on the same threads, so it compares them under the same load; it is not a substitute for the BenchmarkDotNet suite.

## Corpora

[`corpora.json`](corpora.json) lists, per grammar scope, a repository, a pinned commit, the sparse-checkout patterns and
the file-name patterns tokenized (Roslyn for C# and VB, TypeScript 5.9.3 for TypeScript, Node's `lib` for JavaScript,
Go's standard library, CPython's `Lib`, tokio, Guava, git, fmt, Rails, Laravel, PostgreSQL's regression SQL, …).
Not covered, for lack of a natural corpus: log, git-commit/git-rebase, markdown-math, bibtex, CUDA, TOML.

`fetch-corpus.ps1` (PowerShell 7, Windows/macOS/Linux) fetches them shallow, blobless and sparse into `./corpus`
(git-ignored); all 50 took under a minute on a fast connection. It is idempotent: a corpus already at its
commit is skipped.

## Run

```
pwsh ./fetch-corpus.ps1                    # or -Only csharp,go
dotnet build CorpusCheck/CorpusCheck.csproj -c Release
dotnet CorpusCheck/bin/Release/net10.0/TextMateSharp.CorpusCheck.dll --corpus corpus --out out
```

Options: `--only csharp,go`, `--max-files N` (first N files per corpus, by path), `--max-lines N`, `--threads N`,
`--time-limit-ms N` (default 10000), `--max-line-length N` (skip files with longer lines, e.g. minified; default 10000),
`--max-repro N` (discrepancies kept per file). The exit code is 0 when everything matches, 1 otherwise.

Output: `out/report.json` (everything, including every recorded discrepancy) and `out/report.md` (the summary table
and the first discrepancies per corpus). [`sample-report/`](sample-report) is a local run with `--max-files 30`
(macOS arm64, 14 threads): 1,466 files, 606,013 lines, no mismatch, no error. [`devbox1-report/`](devbox1-report) is the full run on devbox1 (16 cores, 4 min 44 s): 21,364 files, 7,352,306 lines, 3 mismatched lines (Unicode superscript digits in Python and Julia), and 1 error from a symbolic link in the asciidoc corpus, which is now skipped.

## On devbox1

```
ssh devbox1
cd <TextMateSharp checkout>; git fetch; git checkout <branch>
cd tools/TextMateSharp.CorpusCheck
pwsh ./fetch-corpus.ps1 -Parallel 8
dotnet build CorpusCheck/CorpusCheck.csproj -c Release
dotnet CorpusCheck/bin/Release/net10.0/TextMateSharp.CorpusCheck.dll --corpus corpus --out out *> run.log
```

Long paths are enabled per corpus (`core.longpaths`). Over SSH the output is not a console, so progress prints one line
every 10 seconds; run it from a Windows Terminal (RDP) for the live display. Copy `out/report.json` and `out/report.md`
back with `scp devbox1:<path>/out/report.* .`.
