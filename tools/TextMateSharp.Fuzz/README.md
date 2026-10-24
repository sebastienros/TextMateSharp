# TextMateSharp.Fuzz

Differential fuzzer for every grammar bundled in `TextMateSharp.Grammars`: it tokenizes generated inputs with this
branch's TextMateSharp (GregsEx) and with the official release from nuget.org (TextMateSharp 2.0.4, Onigwrap), in the
same process, and records every difference.

## How it works

- `Runner/Runner.cs` is compiled twice: `Runner/Branch` references `src/TextMateSharp*`, `Runner/Official` references
  the NuGet packages (`-p:OfficialTextMateSharpVersion=x.y.z` picks another release). The host copies both builds to
  `runners/branch` and `runners/official` and loads each in its own `AssemblyLoadContext` (with native Onigwrap
  resolved from the runner's `deps.json`). The grammar files are the same in both: `src/TextMateSharp.Grammars` has not
  changed since v2.0.4.
- Targets: every `scopeName` declared by a `package.json` under `src/TextMateSharp.Grammars/Resources/Grammars`
  (76 grammars, embedded ones included).
- Inputs, a function of the seed, the grammar, the iteration number and the grammar's corpus:
  slices of the bundled sample (`src/TextMateSharp.WasmDemo/wwwroot/samples`) or of the corpus with mutations (insert,
  delete, duplicate, swap, split and join lines), splices, fragments built from the grammar's own literals (keywords,
  operators, `begin`/`end` delimiters extracted from its regexes), deep nesting of openers across one or many lines,
  long lines, unicode (astral, combining, bidi, BOM, U+2028, NUL) and lone surrogates.
- Each input is tokenized line by line, carrying the state, with `TokenizeLine` and with `TokenizeLine2` (two state
  chains), with a per-line time limit (`--line-time-limit-ms`, 5 s). The outputs compared are token offsets, scopes,
  metadata, `StoppedEarly` and exception types.
- Corpus: an input whose branch tokens reach a scope name never seen for that grammar is kept and mutated later.
- A new discrepancy is minimized (lines, then characters, while it shows the same kind; at most 400 runs or 60 s)
  and saved once per signature (kind, grammar, the scopes that differ); later hits only increment its count.

## Kinds of findings

Most severe first. The first differing line names the kind; later lines start from different states.

| Kind | Meaning |
|---|---|
| `load` | A grammar loads in one version only. |
| `exception-branch`, `exception-official`, `exception-type` | One version throws (or both, with different types). `exception-* runner` means the runner itself failed. |
| `stopped-branch` | The branch ran out of the per-line time limit and the official did not: a performance regression. |
| `tokens` | Different offsets or scopes from `TokenizeLine`. |
| `metadata` | Same tokens, different `TokenizeLine2` metadata. |
| `invalid-utf16` | A difference on an input with a lone surrogate: Onigwrap converts lines to UTF-8, so the official offsets go past the line. One finding per grammar. Disable with `--no-invalid-utf16`. |
| `stopped-official` | The official version ran out of the time limit (it is quadratic on long lines) and the branch did not. Not minimized. |
| `hang` | A single tokenization ran past `--hang-seconds` (60 s): the fuzzer saves the state with that iteration skipped and exits with code 3. |
| `process-crash` | The previous run died while running this input (stack overflow, access violation, or a `kill -9`). |

## Run

```bash
dotnet build -c Release tools/TextMateSharp.Fuzz/Host
dotnet tools/TextMateSharp.Fuzz/Host/bin/Release/net10.0/TextMateSharp.Fuzz.dll --state fuzz-state --seed 20261024
```

Run it from the repository (it finds the grammars and samples there, or pass `--repo`). Every 5 s it prints the
executions, exec/s and findings; every 60 s a per-grammar table (iterations, exec/s, discrepancies, crashes).

Ctrl+C (or SIGTERM) finishes the running iterations, saves and exits with 0; a second Ctrl+C kills it. Running the same
command again resumes where it stopped; a resumed campaign generates the same inputs an uninterrupted one would.
The state is also saved every minute, so a killed campaign loses at most a minute.

Options: `--workers n` (processor count - 1), `--duration 8h`, `--grammars source.cs,json` (scope names or language
ids), `--iterations n` (per grammar), `--hang-seconds 60`, `--line-time-limit-ms 5000`, `--minimize-budget 400`,
`--minimize-seconds 60`, `--no-invalid-utf16`, `--status-seconds 5`, `--table-seconds 60`.

The state directory holds:

- `campaign.json`: the seed, the versions, and per grammar the next iteration, executions, discrepancies, crashes and
  skipped iterations.
- `corpus/<scope>.json`: the corpus and the scopes reached.
- `findings/<kind>-<scope>-<hash>.json`: the minimized input (`Input.Text`, with `Input.Utf16` hex for lines holding
  lone surrogates), the original one, the first differing block of each version and their full outputs.
- `findings.md`: the report, rewritten on every save; `report --state dir` rewrites it.
- `inflight/<worker>.json`: the input each worker is running, turned into a `process-crash` finding if the process dies.

Reproduce a finding (exit code 1 if it still differs), or run one version alone (to tell which one hangs):

```bash
dotnet tools/TextMateSharp.Fuzz/Host/bin/Release/net10.0/TextMateSharp.Fuzz.dll repro fuzz-state/findings/<id>.json
dotnet tools/TextMateSharp.Fuzz/Host/bin/Release/net10.0/TextMateSharp.Fuzz.dll repro fuzz-state/findings/<id>.json --engine official --line-time-limit-ms 0
```

## Long campaign on devbox1

From the Mac (`ssh devbox1` lands in pwsh 7):

```pwsh
git clone https://github.com/sebastienros/TextMateSharp.git C:\src\TextMateSharp-fuzz
cd C:\src\TextMateSharp-fuzz
git checkout sebros/grammar-fuzzing-53d483
dotnet build -c Release tools/TextMateSharp.Fuzz/Host
```

Then run it in a loop that restarts after a hang (exit 3) or a process crash, and stops on Ctrl+C (exit 0) or on a
usage error (exit 2):

```pwsh
cd C:\src\TextMateSharp-fuzz
do {
  dotnet tools/TextMateSharp.Fuzz/Host/bin/Release/net10.0/TextMateSharp.Fuzz.dll --state C:\fuzz\tms --seed 20261024 --duration 12h
} while ($LASTEXITCODE -ne 0 -and $LASTEXITCODE -ne 2)
```

Start it in a session that survives the SSH connection (for example an RDP console or a scheduled task); to resume
later, run the same loop again: the state in `C:\fuzz\tms` carries the seed and the position of every grammar.
Read the results with `Get-Content C:\fuzz\tms\findings.md`, or copy them back with
`scp -r devbox1:C:/fuzz/tms ./fuzz-state`.
