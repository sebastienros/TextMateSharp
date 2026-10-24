# TextMateSharp corpus check

- Official: TextMateSharp 2.0.4+622d1b240a5be474939857c0b2780249022f14a3 (Onigwrap)
- Branch: TextMateSharp 1.0.0+ce309e0f33ec51365961900a160aba622e1c543d (GregsEx)
- Machine: CPC-sebro-SGN1O, Microsoft Windows 10.0.26200, X64, 16 cores; .NET 10.0.12; 16 threads
- Started 2026-10-24 16:42:40Z, took 00:04:44
- **7,352,306 lines, 3 mismatched, 1 errors** in 21,364 files

Tokens per second count TokenizeLine tokens over the time spent in TokenizeLine on each engine (both run on the same thread, interleaved line by line); the first lines of each grammar on each thread include compiling its rules.

| Corpus | Scope | Source | Files | Lines | Matching | Mismatched (tokens / metadata) | Files w/ mismatch | Errors (O/B) | Timeouts (O/B) | Official tok/s | Branch tok/s | Speedup |
|---|---|---|--:|--:|--:|--:|--:|--:|--:|--:|--:|--:|
| csharp | `source.cs` | [dotnet/roslyn@e3c31f15b7](https://github.com/dotnet/roslyn/tree/e3c31f15b79e226656e3b082a921d4d60127b1fa) | 1,167 | 548,795 | 100.000% | 0 | 0 | 0/0 | 0/0 | 16,162 | 69,794 | 4.32x |
| vb | `source.asp.vb.net` | [dotnet/roslyn@e3c31f15b7](https://github.com/dotnet/roslyn/tree/e3c31f15b79e226656e3b082a921d4d60127b1fa) | 919 | 475,565 | 100.000% | 0 | 0 | 0/0 | 0/0 | 35,661 | 202,411 | 5.68x |
| fsharp | `source.fsharp` | [dotnet/fsharp@fe180deb55](https://github.com/dotnet/fsharp/tree/fe180deb551cc47a57de788c936e2fd4d778a909) | 415 | 288,140 | 100.000% | 0 | 0 | 0/0 | 0/0 | 54,474 | 205,391 | 3.77x |
| razor | `text.html.cshtml` | [dotnet/aspnetcore@ceb0eec49d](https://github.com/dotnet/aspnetcore/tree/ceb0eec49d8d2c77f4f7c26b85ddb6e96940dfed) | 1,602 (+3 skipped) | 48,530 | 100.000% | 0 | 0 | 0/0 | 0/0 | 5,353 | 38,461 | 7.19x |
| typescript | `source.ts` | [microsoft/TypeScript@c63de15a99](https://github.com/microsoft/TypeScript/tree/c63de15a992d37f0d6cec03ac7631872838602cb) | 700 (+1 skipped) | 439,269 | 100.000% | 0 | 0 | 0/0 | 0/0 | 15,589 | 66,993 | 4.30x |
| tsx | `source.tsx` | [microsoft/vscode@1ce5ffeb06](https://github.com/microsoft/vscode/tree/1ce5ffeb06bc84eaa81f5a2baba2e36780dad785) | 267 (+1 skipped) | 64,912 | 100.000% | 0 | 0 | 0/0 | 0/0 | 12,134 | 43,376 | 3.57x |
| javascript | `source.js` | [nodejs/node@91c4436158](https://github.com/nodejs/node/tree/91c4436158c615b0e3bb1b6a02c2c85eea0d1adf) | 435 | 196,922 | 100.000% | 0 | 0 | 0/0 | 0/0 | 20,741 | 76,627 | 3.69x |
| json | `source.json` | [microsoft/vscode@1ce5ffeb06](https://github.com/microsoft/vscode/tree/1ce5ffeb06bc84eaa81f5a2baba2e36780dad785) | 1,142 (+13 skipped) | 673,498 | 100.000% | 0 | 0 | 0/0 | 0/0 | 270,064 | 604,385 | 2.24x |
| go | `source.go` | [golang/go@1fce9bafb0](https://github.com/golang/go/tree/1fce9bafb0581a5d894b80d91674659854104012) | 837 | 272,162 | 100.000% | 0 | 0 | 0/0 | 0/0 | 75,749 | 381,515 | 5.04x |
| python | `source.python` | [python/cpython@11598362a3](https://github.com/python/cpython/tree/11598362a3ebbc8bfd1c4c37ec63ee565136868c) | 2,082 | 1,108,254 | 100.000% | **1** (1 / 1) | 1 | 0/0 | 0/0 | 26,818 | 157,747 | 5.88x |
| rust | `source.rust` | [tokio-rs/tokio@8a2408edcf](https://github.com/tokio-rs/tokio/tree/8a2408edcfa2b7f19c00428a9d3a8ea6652a083f) | 381 | 108,954 | 100.000% | 0 | 0 | 0/0 | 0/0 | 38,176 | 285,083 | 7.47x |
| java | `source.java` | [google/guava@1b1d120f9c](https://github.com/google/guava/tree/1b1d120f9cd79ed5dcb1eb1e355e8f48b2b1bc6f) | 610 (+1 skipped) | 181,311 | 100.000% | 0 | 0 | 0/0 | 0/0 | 31,465 | 146,202 | 4.65x |
| c | `source.c` | [git/git@7e34d8174d](https://github.com/git/git/tree/7e34d8174dcda3435d1404c9a30bef46afe6c13e) | 603 | 350,025 | 100.000% | 0 | 0 | 0/0 | 0/0 | 28,521 | 91,534 | 3.21x |
| cpp | `source.cpp` | [fmtlib/fmt@0fc31633e7](https://github.com/fmtlib/fmt/tree/0fc31633e7d53ac895ae0a688b83b7d44f0b0c1f) | 73 | 70,496 | 100.000% | 0 | 0 | 0/0 | 0/0 | 6,290 | 9,756 | 1.55x |
| shellscript | `source.shell` | [git/git@7e34d8174d](https://github.com/git/git/tree/7e34d8174dcda3435d1404c9a30bef46afe6c13e) | 1,112 | 322,800 | 100.000% | 0 | 0 | 0/0 | 0/0 | 78,738 | 241,612 | 3.07x |
| makefile | `source.makefile` | [git/git@7e34d8174d](https://github.com/git/git/tree/7e34d8174dcda3435d1404c9a30bef46afe6c13e) | 21 | 6,101 | 100.000% | 0 | 0 | 0/0 | 0/0 | 23,129 | 402,120 | 17.39x |
| ruby | `source.ruby` | [rails/rails@51811570ff](https://github.com/rails/rails/tree/51811570ff56fcd8f4e535fa0f6f2aefb45bdefd) | 419 | 78,676 | 100.000% | 0 | 0 | 0/0 | 0/0 | 30,357 | 124,722 | 4.11x |
| php | `source.php` | [laravel/framework@5d77975287](https://github.com/laravel/framework/tree/5d7797528723107cd4f495326838a7d7d67fbb20) | 1,744 | 276,205 | 100.000% | 0 | 0 | 0/0 | 0/0 | 16,211 | 54,260 | 3.35x |
| ini | `source.ini` | [php/php-src@97374e0bd8](https://github.com/php/php-src/tree/97374e0bd8e54907dc80b0cd73d543f67bdffbd2) | 10 | 31,727 | 100.000% | 0 | 0 | 0/0 | 0/0 | 443,684 | 839,998 | 1.89x |
| perl | `source.perl` | [mojolicious/mojo@4dc4eb2e9c](https://github.com/mojolicious/mojo/tree/4dc4eb2e9ca2037e7fc4e8f65155d2203e46e08c) | 112 | 35,345 | 100.000% | 0 | 0 | 0/0 | 0/0 | 43,399 | 260,426 | 6.00x |
| lua | `source.lua` | [neovim/neovim@7b87ce822e](https://github.com/neovim/neovim/tree/7b87ce822e6fd45a4d9993599c2854afdb0dc4dd) | 166 | 102,627 | 100.000% | 0 | 0 | 0/0 | 0/0 | 69,724 | 364,700 | 5.23x |
| powershell | `source.powershell` | [PowerShell/PowerShell@ac15e9a5fa](https://github.com/PowerShell/PowerShell/tree/ac15e9a5fa45aa97964321ce08702c97b2414616) | 555 | 144,953 | 100.000% | 0 | 0 | 0/0 | 0/0 | 23,564 | 142,972 | 6.07x |
| bat | `source.batchfile` | [dotnet/runtime@1df7015e61](https://github.com/dotnet/runtime/tree/1df7015e61349dafdc12ceb1b14840c79d60ea48) | 64 | 3,538 | 100.000% | 0 | 0 | 0/0 | 0/0 | 8,629 | 44,548 | 5.16x |
| xml | `text.xml` | [dotnet/runtime@1df7015e61](https://github.com/dotnet/runtime/tree/1df7015e61349dafdc12ceb1b14840c79d60ea48) | 41 | 9,279 | 100.000% | 0 | 0 | 0/0 | 0/0 | 194,551 | 1,067,691 | 5.49x |
| sql | `source.sql` | [postgres/postgres@126976a8ff](https://github.com/postgres/postgres/tree/126976a8ffcfa47f40d3df09243ebf4606001912) | 241 | 129,850 | 100.000% | 0 | 0 | 0/0 | 0/0 | 6,359 | 96,525 | 15.18x |
| html | `text.html.derivative` | [mdn/learning-area@21973263ba](https://github.com/mdn/learning-area/tree/21973263ba859ff3477919087b488157f9d618a6) | 154 | 8,693 | 100.000% | 0 | 0 | 0/0 | 0/0 | 4,936 | 18,234 | 3.69x |
| css | `source.css` | [mdn/learning-area@21973263ba](https://github.com/mdn/learning-area/tree/21973263ba859ff3477919087b488157f9d618a6) | 44 | 4,050 | 100.000% | 0 | 0 | 0/0 | 0/0 | 5,079 | 14,465 | 2.85x |
| scss | `source.css.scss` | [twbs/bootstrap@6ae3f8690e](https://github.com/twbs/bootstrap/tree/6ae3f8690e9bf3c9a63c75eef3b995c7ce118bc9) | 117 | 14,819 | 100.000% | 0 | 0 | 0/0 | 0/0 | 2,904 | 5,350 | 1.84x |
| less | `source.css.less` | [less/less.js@713331655e](https://github.com/less/less.js/tree/713331655e437401dfea6cd233893a69762734e1) | 315 | 9,469 | 100.000% | 0 | 0 | 0/0 | 0/0 | 2,739 | 12,895 | 4.71x |
| yaml | `source.yaml` | [kubernetes/examples@d6b8cd27ea](https://github.com/kubernetes/examples/tree/d6b8cd27eacb51e651a1aa6f7c190a28713eff6e) | 250 | 7,196 | 100.000% | 0 | 0 | 0/0 | 0/0 | 19,520 | 287,774 | 14.74x |
| markdown | `text.html.markdown` | [microsoft/vscode-docs@a030905338](https://github.com/microsoft/vscode-docs/tree/a03090533810eb02e5b9a2a650e2a3df32192b42) | 365 | 77,479 | 100.000% | 0 | 0 | 0/0 | 0/0 | 13,160 | 30,352 | 2.31x |
| dart | `source.dart` | [dart-lang/sdk@f675d40d11](https://github.com/dart-lang/sdk/tree/f675d40d11576889472dbbbf3cc2cab278c8f706) | 145 | 161,633 | 100.000% | 0 | 0 | 0/0 | 0/0 | 56,799 | 263,512 | 4.64x |
| swift | `source.swift` | [apple/swift-nio@108daca35d](https://github.com/apple/swift-nio/tree/108daca35da6a1b4ed8da29c1dfbd19668b9b8fc) | 309 | 114,967 | 100.000% | 0 | 0 | 0/0 | 0/0 | 84,779 | 229,536 | 2.71x |
| objectivec | `source.objc` | [AFNetworking/AFNetworking@d9f589cc2c](https://github.com/AFNetworking/AFNetworking/tree/d9f589cc2c1fe9d55eb5eea00558010afea7a41e) | 80 | 15,470 | 100.000% | 0 | 0 | 0/0 | 0/0 | 9,310 | 25,907 | 2.78x |
| groovy | `source.groovy` | [apache/groovy@98c2f9f106](https://github.com/apache/groovy/tree/98c2f9f106d2ee368278ca33d105e540bf23baf0) | 812 | 177,891 | 100.000% | 0 | 0 | 0/0 | 0/0 | 35,624 | 146,766 | 4.12x |
| julia | `source.julia` | [JuliaLang/julia@bbfa0ce415](https://github.com/JuliaLang/julia/tree/bbfa0ce4150ea33f2532198cabcf319a49399360) | 202 | 140,683 | 99.999% | **2** (2 / 2) | 1 | 0/0 | 0/0 | 28,041 | 123,402 | 4.40x |
| r | `source.r` | [tidyverse/ggplot2@b638d654c8](https://github.com/tidyverse/ggplot2/tree/b638d654c8189db82a62307fefa84b1da1819da5) | 202 | 54,585 | 100.000% | 0 | 0 | 0/0 | 0/0 | 21,137 | 89,634 | 4.24x |
| clojure | `source.clojure` | [clojure/clojure@903eee79d3](https://github.com/clojure/clojure/tree/903eee79d393cc2937933a2a05044ca2e9abca4c) | 49 | 22,987 | 100.000% | 0 | 0 | 0/0 | 0/0 | 46,505 | 264,402 | 5.69x |
| coffeescript | `source.coffee` | [jashkenas/coffeescript@4aaa755df3](https://github.com/jashkenas/coffeescript/tree/4aaa755df32ae7a55886226b153d1157ba01bb4a) | 75 | 44,701 | 100.000% | 0 | 0 | 0/0 | 0/0 | 24,841 | 102,703 | 4.13x |
| dockerfile | `source.dockerfile` | [docker-library/python@d01180b5d8](https://github.com/docker-library/python/tree/d01180b5d8b1664a9b87862e3cafeafbe87383c6) | 44 | 5,916 | 100.000% | 0 | 0 | 0/0 | 0/0 | 171,766 | 595,230 | 3.47x |
| diff | `source.diff` | [openwrt/openwrt@b5bc4becb4](https://github.com/openwrt/openwrt/tree/b5bc4becb424b4d89f48e372675811821aa155bd) | 974 | 192,320 | 100.000% | 0 | 0 | 0/0 | 0/0 | 154,066 | 525,862 | 3.41x |
| handlebars | `text.html.handlebars` | [TryGhost/Casper@cbdb5c6fca](https://github.com/TryGhost/Casper/tree/cbdb5c6fcaffcc8973af643c7c8878fc0d076e1b) | 25 | 798 | 100.000% | 0 | 0 | 0/0 | 0/0 | 6,167 | 41,856 | 6.79x |
| hlsl | `source.hlsl` | [microsoft/DirectX-Graphics-Samples@e5975f9b07](https://github.com/microsoft/DirectX-Graphics-Samples/tree/e5975f9b0744fc5096dd593ed72bf8f5c004164f) | 446 | 31,357 | 100.000% | 0 | 0 | 0/0 | 0/0 | 28,115 | 167,997 | 5.98x |
| shaderlab | `source.shaderlab` | [TwoTailsGames/Unity-Built-in-Shaders@6a63f93bc1](https://github.com/TwoTailsGames/Unity-Built-in-Shaders/tree/6a63f93bc1f20ce6cd47f981c7494e8328915621) | 192 | 18,912 | 100.000% | 0 | 0 | 0/0 | 0/0 | 24,856 | 132,823 | 5.34x |
| latex | `text.tex.latex` | [latex3/latex2e@3858fc13f7](https://github.com/latex3/latex2e/tree/3858fc13f7d800aaa0d394d56660ff9e72878d1d) | 124 | 100,742 | 100.000% | 0 | 0 | 0/0 | 0/0 | 30,479 | 98,548 | 3.23x |
| asciidoc | `document.adoc` | [asciidoctor/asciidoctor@3fbf3dee5f](https://github.com/asciidoctor/asciidoctor/tree/3fbf3dee5f7fe6e222ba003d1c77bc3afb4b1ff9) | 99 | 9,371 | 100.000% | 0 | 0 | 0/0 | 0/0 | 25,023 | 166,847 | 6.67x |
| pascal | `source.pascal` | [castle-engine/castle-engine@113d8bb8a2](https://github.com/castle-engine/castle-engine/tree/113d8bb8a2af17342049d99c464cbcadd195b366) | 135 | 69,339 | 100.000% | 0 | 0 | 0/0 | 0/0 | 61,407 | 340,735 | 5.55x |
| pug | `text.pug` | [pugjs/pug@c323ed3e63](https://github.com/pugjs/pug/tree/c323ed3e630b931bc04790efb509d34ac5927040) | 137 | 1,195 | 100.000% | 0 | 0 | 0/0 | 0/0 | 222 | 2,314 | 10.43x |
| typst | `source.typst` | [typst/typst@64c5e8c02f](https://github.com/typst/typst/tree/64c5e8c02fe0394d913ead36e42ef8c1334d2107) | 224 | 35,797 | 100.000% | 0 | 0 | 0/0 | 0/0 | 8,027 | 41,809 | 5.21x |
| x86asm | `source.x86asm` | [FFmpeg/FFmpeg@a881f0a9ad](https://github.com/FFmpeg/FFmpeg/tree/a881f0a9addd3d104b87d4f6b58b111b1d0cfd28) | 127 | 64,002 | 100.000% | 0 | 0 | 0/0 | 0/0 | 20,425 | 326,208 | 15.97x |

## python

### `Lib/test/test_re.py:690` (both; reproduces from line 690 with the initial state)

```
        self.checkPatternError(r'()(?(¹)a|b)',
```

| # | Official | Branch |
|--:|---|---|
| 0 | 0-8 `source.python` | = |
| 1 | 8-12 `source.python variable.language.special.self.python` | = |
| 2 | 12-13 `source.python meta.member.access.python punctuation.separator.period.python` | = |
| 3 | 13-30 `source.python meta.member.access.python meta.function-call.python meta.function-call.generic.python` | = |
| 4 | 30-31 `source.python meta.member.access.python meta.function-call.python punctuation.definition.arguments.begin.python` | = |
| 5 | 31-32 `source.python meta.member.access.python meta.function-call.python meta.function-call.arguments.python string.regexp.quoted.single.python storage.type.string.python` | = |
| 6 | 32-33 `source.python meta.member.access.python meta.function-call.python meta.function-call.arguments.python string.regexp.quoted.single.python punctuation.definition.string.begin.python` | = |
| 7 | 33-34 `source.python meta.member.access.python meta.function-call.python meta.function-call.arguments.python string.regexp.quoted.single.python support.other.parenthesis.regexp punctuation.parenthesis.begin.regexp` | = |
| 8 | 34-35 `source.python meta.member.access.python meta.function-call.python meta.function-call.arguments.python string.regexp.quoted.single.python support.other.parenthesis.regexp punctuation.parenthesis.end.regexp` | = |
| **9** | 35-36 `source.python meta.member.access.python meta.function-call.python meta.function-call.arguments.python string.regexp.quoted.single.python keyword.operator.conditional.regexp punctuation.parenthesis.conditional.begin.regexp` | 35-36 `source.python meta.member.access.python meta.function-call.python meta.function-call.arguments.python string.regexp.quoted.single.python support.other.parenthesis.regexp punctuation.parenthesis.begin.regexp` |
| **10** | 36-40 `source.python meta.member.access.python meta.function-call.python meta.function-call.arguments.python string.regexp.quoted.single.python keyword.operator.conditional.regexp` | 36-37 `source.python meta.member.access.python meta.function-call.python meta.function-call.arguments.python string.regexp.quoted.single.python keyword.operator.quantifier.regexp` |
| **11** | 40-41 `source.python meta.member.access.python meta.function-call.python meta.function-call.arguments.python string.regexp.quoted.single.python` | 37-38 `source.python meta.member.access.python meta.function-call.python meta.function-call.arguments.python string.regexp.quoted.single.python support.other.parenthesis.regexp punctuation.parenthesis.begin.regexp` |
| **12** | 41-42 `source.python meta.member.access.python meta.function-call.python meta.function-call.arguments.python string.regexp.quoted.single.python keyword.operator.disjunction.regexp` | 38-39 `source.python meta.member.access.python meta.function-call.python meta.function-call.arguments.python string.regexp.quoted.single.python` |
| **13** | 42-43 `source.python meta.member.access.python meta.function-call.python meta.function-call.arguments.python string.regexp.quoted.single.python` | 39-40 `source.python meta.member.access.python meta.function-call.python meta.function-call.arguments.python string.regexp.quoted.single.python support.other.parenthesis.regexp punctuation.parenthesis.end.regexp` |
| **14** | 43-44 `source.python meta.member.access.python meta.function-call.python meta.function-call.arguments.python string.regexp.quoted.single.python keyword.operator.conditional.negative.regexp punctuation.parenthesis.conditional.end.regexp` | 40-41 `source.python meta.member.access.python meta.function-call.python meta.function-call.arguments.python string.regexp.quoted.single.python` |
| **15** | 44-45 `source.python meta.member.access.python meta.function-call.python meta.function-call.arguments.python string.regexp.quoted.single.python punctuation.definition.string.end.python` | 41-42 `source.python meta.member.access.python meta.function-call.python meta.function-call.arguments.python string.regexp.quoted.single.python keyword.operator.disjunction.regexp` |
| **16** | 45-46 `source.python meta.member.access.python meta.function-call.python meta.function-call.arguments.python punctuation.separator.arguments.python` | 42-43 `source.python meta.member.access.python meta.function-call.python meta.function-call.arguments.python string.regexp.quoted.single.python` |
| **17** |  | 43-44 `source.python meta.member.access.python meta.function-call.python meta.function-call.arguments.python string.regexp.quoted.single.python support.other.parenthesis.regexp punctuation.parenthesis.end.regexp` |
| **18** |  | 44-45 `source.python meta.member.access.python meta.function-call.python meta.function-call.arguments.python string.regexp.quoted.single.python punctuation.definition.string.end.python` |
| **19** |  | 45-46 `source.python meta.member.access.python meta.function-call.python meta.function-call.arguments.python punctuation.separator.arguments.python` |

TokenizeLine2 official `[0, 33587200, 8, 33783808, 12, 33587200, 31, 33653248, 32, 33686016, 33, 33751552, 35, 34144768, 40, 33686016, 41, 34144768, 42, 33686016, 43, 34144768, 44, 33686016, 45, 33587200]`, branch `[0, 33587200, 8, 33783808, 12, 33587200, 31, 33653248, 32, 33686016, 33, 33751552, 36, 33718784, 37, 33751552, 38, 33686016, 39, 33751552, 40, 33686016, 41, 34144768, 42, 33686016, 43, 33751552, 44, 33686016, 45, 33587200]`

## julia

### `base/special/trig.jl:1268` (both; reproduces from line 1268 with the initial state)

```
        π²x² = term^2
```

| # | Official | Branch |
|--:|---|---|
| **0** | 0-13 `source.julia` | 0-8 `source.julia` |
| **1** | 13-14 `source.julia keyword.operator.update.julia` | 8-9 `source.julia constant.numeric.julia` |
| **2** | 14-19 `source.julia` | 9-13 `source.julia` |
| **3** | 19-20 `source.julia keyword.operator.arithmetic.julia` | 13-14 `source.julia keyword.operator.update.julia` |
| **4** | 20-21 `source.julia constant.numeric.julia` | 14-19 `source.julia` |
| **5** |  | 19-20 `source.julia keyword.operator.arithmetic.julia` |
| **6** |  | 20-21 `source.julia constant.numeric.julia` |

TokenizeLine2 official `[0, 33587200, 13, 34144256, 14, 33587200, 19, 34144256, 20, 34013184]`, branch `[0, 33587200, 8, 34013184, 9, 33587200, 13, 34144256, 14, 33587200, 19, 34144256, 20, 34013184]`

### `base/special/trig.jl:1273` (both; reproduces from line 1273 with the initial state)

```
            term *= π²x²/((1-2n)*(2n-2))
```

| # | Official | Branch |
|--:|---|---|
| 0 | 0-17 `source.julia` | = |
| 1 | 17-19 `source.julia keyword.operator.update.julia` | = |
| **2** | 19-24 `source.julia` | 19-20 `source.julia` |
| **3** | 24-25 `source.julia keyword.operator.arithmetic.julia` | 20-21 `source.julia constant.numeric.julia` |
| **4** | 25-26 `source.julia meta.bracket.julia` | 21-24 `source.julia` |
| **5** | 26-27 `source.julia meta.bracket.julia` | 24-25 `source.julia keyword.operator.arithmetic.julia` |
| **6** | 27-28 `source.julia constant.numeric.julia` | 25-26 `source.julia meta.bracket.julia` |
| **7** | 28-29 `source.julia keyword.operator.arithmetic.julia` | 26-27 `source.julia meta.bracket.julia` |
| **8** | 29-30 `source.julia constant.numeric.julia` | 27-28 `source.julia constant.numeric.julia` |
| **9** | 30-31 `source.julia` | 28-29 `source.julia keyword.operator.arithmetic.julia` |
| **10** | 31-32 `source.julia meta.bracket.julia` | 29-30 `source.julia constant.numeric.julia` |
| **11** | 32-33 `source.julia keyword.operator.arithmetic.julia` | 30-31 `source.julia` |
| **12** | 33-34 `source.julia meta.bracket.julia` | 31-32 `source.julia meta.bracket.julia` |
| **13** | 34-35 `source.julia constant.numeric.julia` | 32-33 `source.julia keyword.operator.arithmetic.julia` |
| **14** | 35-36 `source.julia` | 33-34 `source.julia meta.bracket.julia` |
| **15** | 36-37 `source.julia keyword.operator.arithmetic.julia` | 34-35 `source.julia constant.numeric.julia` |
| **16** | 37-38 `source.julia constant.numeric.julia` | 35-36 `source.julia` |
| **17** | 38-39 `source.julia meta.bracket.julia` | 36-37 `source.julia keyword.operator.arithmetic.julia` |
| **18** | 39-40 `source.julia meta.bracket.julia` | 37-38 `source.julia constant.numeric.julia` |
| **19** |  | 38-39 `source.julia meta.bracket.julia` |
| **20** |  | 39-40 `source.julia meta.bracket.julia` |

TokenizeLine2 official `[0, 33587200, 17, 34144256, 19, 33587200, 24, 34144256, 25, 33587200, 27, 34013184, 28, 34144256, 29, 34013184, 30, 33587200, 32, 34144256, 33, 33587200, 34, 34013184, 35, 33587200, 36, 34144256, 37, 34013184, 38, 33587200]`, branch `[0, 33587200, 17, 34144256, 19, 33587200, 20, 34013184, 21, 33587200, 24, 34144256, 25, 33587200, 27, 34013184, 28, 34144256, 29, 34013184, 30, 33587200, 32, 34144256, 33, 33587200, 34, 34013184, 35, 33587200, 36, 34144256, 37, 34013184, 38, 33587200]`

## asciidoc
- Error (checker, ) at `docs/modules/cli/partials/man-asciidoctor.adoc:0`: System.IO.IOException: The path cannot be traversed because it contains an untrusted mount point. : 'C:\Users\sebros\tmp\corpuscheck\tools\TextMateSharp.CorpusCheck\corpus\asciidoc\docs\modules\cli\partials\man-asciidoctor.adoc'.
