# TextMateSharp corpus check

- Official: TextMateSharp 2.0.4+622d1b240a5be474939857c0b2780249022f14a3 (Onigwrap)
- Branch: TextMateSharp 1.0.0+85417c6d5df55dd82c7d8c8fc97c1c8d1a39a4ad (GregsEx)
- Machine: CPC-sebro-SGN1O, Microsoft Windows 10.0.26200, X64, 16 cores; .NET 10.0.12; 16 threads
- Started 2026-10-24 20:53:20Z, took 00:04:39
- **7,352,306 lines, 8 mismatched, 0 errors** in 21,364 files

Tokens per second count TokenizeLine tokens over the time spent in TokenizeLine on each engine (both run on the same thread, interleaved line by line); the first lines of each grammar on each thread include compiling its rules.

| Corpus | Scope | Source | Files | Lines | Matching | Mismatched (tokens / metadata) | Files w/ mismatch | Errors (O/B) | Timeouts (O/B) | Official tok/s | Branch tok/s | Speedup |
|---|---|---|--:|--:|--:|--:|--:|--:|--:|--:|--:|--:|
| csharp | `source.cs` | [dotnet/roslyn@e3c31f15b7](https://github.com/dotnet/roslyn/tree/e3c31f15b79e226656e3b082a921d4d60127b1fa) | 1,167 | 548,795 | 100.000% | 0 | 0 | 0/0 | 0/0 | 16,748 | 66,462 | 3.97x |
| vb | `source.asp.vb.net` | [dotnet/roslyn@e3c31f15b7](https://github.com/dotnet/roslyn/tree/e3c31f15b79e226656e3b082a921d4d60127b1fa) | 919 | 475,565 | 100.000% | 0 | 0 | 0/0 | 0/0 | 36,246 | 191,444 | 5.28x |
| fsharp | `source.fsharp` | [dotnet/fsharp@fe180deb55](https://github.com/dotnet/fsharp/tree/fe180deb551cc47a57de788c936e2fd4d778a909) | 415 | 288,140 | 100.000% | 0 | 0 | 0/0 | 0/0 | 57,042 | 195,841 | 3.43x |
| razor | `text.html.cshtml` | [dotnet/aspnetcore@ceb0eec49d](https://github.com/dotnet/aspnetcore/tree/ceb0eec49d8d2c77f4f7c26b85ddb6e96940dfed) | 1,602 (+3 skipped) | 48,530 | 100.000% | 0 | 0 | 0/0 | 0/0 | 5,650 | 34,283 | 6.07x |
| typescript | `source.ts` | [microsoft/TypeScript@c63de15a99](https://github.com/microsoft/TypeScript/tree/c63de15a992d37f0d6cec03ac7631872838602cb) | 700 (+1 skipped) | 439,269 | 100.000% | 0 | 0 | 0/0 | 0/0 | 16,028 | 64,429 | 4.02x |
| tsx | `source.tsx` | [microsoft/vscode@1ce5ffeb06](https://github.com/microsoft/vscode/tree/1ce5ffeb06bc84eaa81f5a2baba2e36780dad785) | 267 (+1 skipped) | 64,912 | 100.000% | 0 | 0 | 0/0 | 0/0 | 12,545 | 40,707 | 3.24x |
| javascript | `source.js` | [nodejs/node@91c4436158](https://github.com/nodejs/node/tree/91c4436158c615b0e3bb1b6a02c2c85eea0d1adf) | 435 | 196,922 | 100.000% | 0 | 0 | 0/0 | 0/0 | 21,478 | 75,145 | 3.50x |
| json | `source.json` | [microsoft/vscode@1ce5ffeb06](https://github.com/microsoft/vscode/tree/1ce5ffeb06bc84eaa81f5a2baba2e36780dad785) | 1,142 (+13 skipped) | 673,498 | 100.000% | 0 | 0 | 0/0 | 0/0 | 275,850 | 592,201 | 2.15x |
| go | `source.go` | [golang/go@1fce9bafb0](https://github.com/golang/go/tree/1fce9bafb0581a5d894b80d91674659854104012) | 837 | 272,162 | 100.000% | 0 | 0 | 0/0 | 0/0 | 75,344 | 397,490 | 5.28x |
| python | `source.python` | [python/cpython@11598362a3](https://github.com/python/cpython/tree/11598362a3ebbc8bfd1c4c37ec63ee565136868c) | 2,082 | 1,108,254 | 100.000% | 0 | 0 | 0/0 | 0/0 | 27,310 | 150,060 | 5.49x |
| rust | `source.rust` | [tokio-rs/tokio@8a2408edcf](https://github.com/tokio-rs/tokio/tree/8a2408edcfa2b7f19c00428a9d3a8ea6652a083f) | 381 | 108,954 | 100.000% | 0 | 0 | 0/0 | 0/0 | 38,039 | 262,136 | 6.89x |
| java | `source.java` | [google/guava@1b1d120f9c](https://github.com/google/guava/tree/1b1d120f9cd79ed5dcb1eb1e355e8f48b2b1bc6f) | 610 (+1 skipped) | 181,311 | 100.000% | 0 | 0 | 0/0 | 0/0 | 32,785 | 148,823 | 4.54x |
| c | `source.c` | [git/git@7e34d8174d](https://github.com/git/git/tree/7e34d8174dcda3435d1404c9a30bef46afe6c13e) | 603 | 350,025 | 100.000% | 0 | 0 | 0/0 | 0/0 | 29,921 | 90,790 | 3.03x |
| cpp | `source.cpp` | [fmtlib/fmt@0fc31633e7](https://github.com/fmtlib/fmt/tree/0fc31633e7d53ac895ae0a688b83b7d44f0b0c1f) | 73 | 70,496 | 100.000% | 0 | 0 | 0/0 | 0/0 | 6,275 | 9,060 | 1.44x |
| shellscript | `source.shell` | [git/git@7e34d8174d](https://github.com/git/git/tree/7e34d8174dcda3435d1404c9a30bef46afe6c13e) | 1,112 | 322,800 | 100.000% | 0 | 0 | 0/0 | 0/0 | 86,117 | 224,175 | 2.60x |
| makefile | `source.makefile` | [git/git@7e34d8174d](https://github.com/git/git/tree/7e34d8174dcda3435d1404c9a30bef46afe6c13e) | 21 | 6,101 | 100.000% | 0 | 0 | 0/0 | 0/0 | 24,967 | 422,084 | 16.91x |
| ruby | `source.ruby` | [rails/rails@51811570ff](https://github.com/rails/rails/tree/51811570ff56fcd8f4e535fa0f6f2aefb45bdefd) | 419 | 78,676 | 99.990% | **8** (8 / 8) | 4 | 0/0 | 0/0 | 31,032 | 130,051 | 4.19x |
| php | `source.php` | [laravel/framework@5d77975287](https://github.com/laravel/framework/tree/5d7797528723107cd4f495326838a7d7d67fbb20) | 1,744 | 276,205 | 100.000% | 0 | 0 | 0/0 | 0/0 | 16,335 | 55,349 | 3.39x |
| ini | `source.ini` | [php/php-src@97374e0bd8](https://github.com/php/php-src/tree/97374e0bd8e54907dc80b0cd73d543f67bdffbd2) | 10 | 31,727 | 100.000% | 0 | 0 | 0/0 | 0/0 | 378,786 | 992,729 | 2.62x |
| perl | `source.perl` | [mojolicious/mojo@4dc4eb2e9c](https://github.com/mojolicious/mojo/tree/4dc4eb2e9ca2037e7fc4e8f65155d2203e46e08c) | 112 | 35,345 | 100.000% | 0 | 0 | 0/0 | 0/0 | 45,792 | 256,807 | 5.61x |
| lua | `source.lua` | [neovim/neovim@7b87ce822e](https://github.com/neovim/neovim/tree/7b87ce822e6fd45a4d9993599c2854afdb0dc4dd) | 166 | 102,627 | 100.000% | 0 | 0 | 0/0 | 0/0 | 73,890 | 393,627 | 5.33x |
| powershell | `source.powershell` | [PowerShell/PowerShell@ac15e9a5fa](https://github.com/PowerShell/PowerShell/tree/ac15e9a5fa45aa97964321ce08702c97b2414616) | 555 | 144,953 | 100.000% | 0 | 0 | 0/0 | 0/0 | 23,947 | 149,266 | 6.23x |
| bat | `source.batchfile` | [dotnet/runtime@1df7015e61](https://github.com/dotnet/runtime/tree/1df7015e61349dafdc12ceb1b14840c79d60ea48) | 64 | 3,538 | 100.000% | 0 | 0 | 0/0 | 0/0 | 8,539 | 53,193 | 6.23x |
| xml | `text.xml` | [dotnet/runtime@1df7015e61](https://github.com/dotnet/runtime/tree/1df7015e61349dafdc12ceb1b14840c79d60ea48) | 41 | 9,279 | 100.000% | 0 | 0 | 0/0 | 0/0 | 158,950 | 806,722 | 5.08x |
| sql | `source.sql` | [postgres/postgres@126976a8ff](https://github.com/postgres/postgres/tree/126976a8ffcfa47f40d3df09243ebf4606001912) | 241 | 129,850 | 100.000% | 0 | 0 | 0/0 | 0/0 | 6,692 | 94,023 | 14.05x |
| html | `text.html.derivative` | [mdn/learning-area@21973263ba](https://github.com/mdn/learning-area/tree/21973263ba859ff3477919087b488157f9d618a6) | 154 | 8,693 | 100.000% | 0 | 0 | 0/0 | 0/0 | 4,580 | 17,723 | 3.87x |
| css | `source.css` | [mdn/learning-area@21973263ba](https://github.com/mdn/learning-area/tree/21973263ba859ff3477919087b488157f9d618a6) | 44 | 4,050 | 100.000% | 0 | 0 | 0/0 | 0/0 | 5,011 | 13,237 | 2.64x |
| scss | `source.css.scss` | [twbs/bootstrap@6ae3f8690e](https://github.com/twbs/bootstrap/tree/6ae3f8690e9bf3c9a63c75eef3b995c7ce118bc9) | 117 | 14,819 | 100.000% | 0 | 0 | 0/0 | 0/0 | 2,908 | 5,373 | 1.85x |
| less | `source.css.less` | [less/less.js@713331655e](https://github.com/less/less.js/tree/713331655e437401dfea6cd233893a69762734e1) | 315 | 9,469 | 100.000% | 0 | 0 | 0/0 | 0/0 | 2,840 | 11,769 | 4.14x |
| yaml | `source.yaml` | [kubernetes/examples@d6b8cd27ea](https://github.com/kubernetes/examples/tree/d6b8cd27eacb51e651a1aa6f7c190a28713eff6e) | 250 | 7,196 | 100.000% | 0 | 0 | 0/0 | 0/0 | 13,741 | 298,510 | 21.72x |
| markdown | `text.html.markdown` | [microsoft/vscode-docs@a030905338](https://github.com/microsoft/vscode-docs/tree/a03090533810eb02e5b9a2a650e2a3df32192b42) | 365 | 77,479 | 100.000% | 0 | 0 | 0/0 | 0/0 | 12,332 | 29,246 | 2.37x |
| dart | `source.dart` | [dart-lang/sdk@f675d40d11](https://github.com/dart-lang/sdk/tree/f675d40d11576889472dbbbf3cc2cab278c8f706) | 145 | 161,633 | 100.000% | 0 | 0 | 0/0 | 0/0 | 59,256 | 243,914 | 4.12x |
| swift | `source.swift` | [apple/swift-nio@108daca35d](https://github.com/apple/swift-nio/tree/108daca35da6a1b4ed8da29c1dfbd19668b9b8fc) | 309 | 114,967 | 100.000% | 0 | 0 | 0/0 | 0/0 | 87,076 | 218,146 | 2.51x |
| objectivec | `source.objc` | [AFNetworking/AFNetworking@d9f589cc2c](https://github.com/AFNetworking/AFNetworking/tree/d9f589cc2c1fe9d55eb5eea00558010afea7a41e) | 80 | 15,470 | 100.000% | 0 | 0 | 0/0 | 0/0 | 10,160 | 30,521 | 3.00x |
| groovy | `source.groovy` | [apache/groovy@98c2f9f106](https://github.com/apache/groovy/tree/98c2f9f106d2ee368278ca33d105e540bf23baf0) | 812 | 177,891 | 100.000% | 0 | 0 | 0/0 | 0/0 | 35,852 | 149,754 | 4.18x |
| julia | `source.julia` | [JuliaLang/julia@bbfa0ce415](https://github.com/JuliaLang/julia/tree/bbfa0ce4150ea33f2532198cabcf319a49399360) | 202 | 140,683 | 100.000% | 0 | 0 | 0/0 | 0/0 | 28,471 | 121,077 | 4.25x |
| r | `source.r` | [tidyverse/ggplot2@b638d654c8](https://github.com/tidyverse/ggplot2/tree/b638d654c8189db82a62307fefa84b1da1819da5) | 202 | 54,585 | 100.000% | 0 | 0 | 0/0 | 0/0 | 20,889 | 84,933 | 4.07x |
| clojure | `source.clojure` | [clojure/clojure@903eee79d3](https://github.com/clojure/clojure/tree/903eee79d393cc2937933a2a05044ca2e9abca4c) | 49 | 22,987 | 100.000% | 0 | 0 | 0/0 | 0/0 | 45,318 | 240,554 | 5.31x |
| coffeescript | `source.coffee` | [jashkenas/coffeescript@4aaa755df3](https://github.com/jashkenas/coffeescript/tree/4aaa755df32ae7a55886226b153d1157ba01bb4a) | 75 | 44,701 | 100.000% | 0 | 0 | 0/0 | 0/0 | 24,528 | 99,203 | 4.04x |
| dockerfile | `source.dockerfile` | [docker-library/python@d01180b5d8](https://github.com/docker-library/python/tree/d01180b5d8b1664a9b87862e3cafeafbe87383c6) | 44 | 5,916 | 100.000% | 0 | 0 | 0/0 | 0/0 | 79,839 | 568,187 | 7.12x |
| diff | `source.diff` | [openwrt/openwrt@b5bc4becb4](https://github.com/openwrt/openwrt/tree/b5bc4becb424b4d89f48e372675811821aa155bd) | 974 | 192,320 | 100.000% | 0 | 0 | 0/0 | 0/0 | 171,481 | 527,877 | 3.08x |
| handlebars | `text.html.handlebars` | [TryGhost/Casper@cbdb5c6fca](https://github.com/TryGhost/Casper/tree/cbdb5c6fcaffcc8973af643c7c8878fc0d076e1b) | 25 | 798 | 100.000% | 0 | 0 | 0/0 | 0/0 | 5,224 | 30,313 | 5.80x |
| hlsl | `source.hlsl` | [microsoft/DirectX-Graphics-Samples@e5975f9b07](https://github.com/microsoft/DirectX-Graphics-Samples/tree/e5975f9b0744fc5096dd593ed72bf8f5c004164f) | 446 | 31,357 | 100.000% | 0 | 0 | 0/0 | 0/0 | 29,616 | 135,141 | 4.56x |
| shaderlab | `source.shaderlab` | [TwoTailsGames/Unity-Built-in-Shaders@6a63f93bc1](https://github.com/TwoTailsGames/Unity-Built-in-Shaders/tree/6a63f93bc1f20ce6cd47f981c7494e8328915621) | 192 | 18,912 | 100.000% | 0 | 0 | 0/0 | 0/0 | 29,603 | 148,677 | 5.02x |
| latex | `text.tex.latex` | [latex3/latex2e@3858fc13f7](https://github.com/latex3/latex2e/tree/3858fc13f7d800aaa0d394d56660ff9e72878d1d) | 124 | 100,742 | 100.000% | 0 | 0 | 0/0 | 0/0 | 30,040 | 102,478 | 3.41x |
| asciidoc | `document.adoc` | [asciidoctor/asciidoctor@3fbf3dee5f](https://github.com/asciidoctor/asciidoctor/tree/3fbf3dee5f7fe6e222ba003d1c77bc3afb4b1ff9) | 99 | 9,371 | 100.000% | 0 | 0 | 0/0 | 0/0 | 22,786 | 159,691 | 7.01x |
| pascal | `source.pascal` | [castle-engine/castle-engine@113d8bb8a2](https://github.com/castle-engine/castle-engine/tree/113d8bb8a2af17342049d99c464cbcadd195b366) | 135 | 69,339 | 100.000% | 0 | 0 | 0/0 | 0/0 | 67,944 | 303,551 | 4.47x |
| pug | `text.pug` | [pugjs/pug@c323ed3e63](https://github.com/pugjs/pug/tree/c323ed3e630b931bc04790efb509d34ac5927040) | 137 | 1,195 | 100.000% | 0 | 0 | 0/0 | 0/0 | 201 | 2,324 | 11.58x |
| typst | `source.typst` | [typst/typst@64c5e8c02f](https://github.com/typst/typst/tree/64c5e8c02fe0394d913ead36e42ef8c1334d2107) | 224 | 35,797 | 100.000% | 0 | 0 | 0/0 | 0/0 | 7,371 | 38,954 | 5.28x |
| x86asm | `source.x86asm` | [FFmpeg/FFmpeg@a881f0a9ad](https://github.com/FFmpeg/FFmpeg/tree/a881f0a9addd3d104b87d4f6b58b111b1d0cfd28) | 127 | 64,002 | 100.000% | 0 | 0 | 0/0 | 0/0 | 20,553 | 340,073 | 16.55x |

## ruby

### `activerecord/lib/active_record/connection_adapters/abstract/quoting.rb:26` (both; reproduces from line 22 with the initial state)

```
          /
            \A
            (
              (?:
                # table_name.column_name | function(one or no argument)
```

| # | Official | Branch |
|--:|---|---|
| **0** | 0-16 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby` | 0-51 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby` |
| **1** | 16-17 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby comment.line.number-sign.ruby punctuation.definition.comment.ruby` | 51-52 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby meta.group.regexp.ruby punctuation.definition.group.ruby` |
| **2** | 17-18 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby comment.line.number-sign.ruby` | 52-70 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby meta.group.regexp.ruby` |
| **3** | 18-71 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby comment.line.number-sign.ruby` | 70-71 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby meta.group.regexp.ruby punctuation.definition.group.ruby` |

TokenizeLine2 official `[0, 33686016, 16, 33980672]`, branch `[0, 33686016]`

### `activerecord/lib/active_record/connection_adapters/abstract/quoting.rb:54` (both; reproduces from line 50 with the initial state)

```
          /
            \A
            (
              (?:
                # table_name.column_name | function(one or no argument)
```

| # | Official | Branch |
|--:|---|---|
| **0** | 0-16 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby` | 0-51 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby` |
| **1** | 16-17 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby comment.line.number-sign.ruby punctuation.definition.comment.ruby` | 51-52 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby meta.group.regexp.ruby punctuation.definition.group.ruby` |
| **2** | 17-18 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby comment.line.number-sign.ruby` | 52-70 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby meta.group.regexp.ruby` |
| **3** | 18-71 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby comment.line.number-sign.ruby` | 70-71 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby meta.group.regexp.ruby punctuation.definition.group.ruby` |

TokenizeLine2 official `[0, 33686016, 16, 33980672]`, branch `[0, 33686016]`

### `activerecord/lib/active_record/connection_adapters/mysql/quoting.rb:21` (both; reproduces from line 17 with the initial state)

```
            /
              \A
              (
                (?:
                  # `table_name`.`column_name` | function(one or no argument)
```

| # | Official | Branch |
|--:|---|---|
| **0** | 0-18 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby` | 0-57 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby` |
| **1** | 18-19 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby comment.line.number-sign.ruby punctuation.definition.comment.ruby` | 57-58 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby meta.group.regexp.ruby punctuation.definition.group.ruby` |
| **2** | 19-20 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby comment.line.number-sign.ruby` | 58-76 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby meta.group.regexp.ruby` |
| **3** | 20-77 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby comment.line.number-sign.ruby` | 76-77 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby meta.group.regexp.ruby punctuation.definition.group.ruby` |

TokenizeLine2 official `[0, 33686016, 18, 33980672]`, branch `[0, 33686016]`

### `activerecord/lib/active_record/connection_adapters/mysql/quoting.rb:36` (both; reproduces from line 32 with the initial state)

```
            /
              \A
              (
                (?:
                  # `table_name`.`column_name` | function(one or no argument)
```

| # | Official | Branch |
|--:|---|---|
| **0** | 0-18 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby` | 0-57 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby` |
| **1** | 18-19 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby comment.line.number-sign.ruby punctuation.definition.comment.ruby` | 57-58 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby meta.group.regexp.ruby punctuation.definition.group.ruby` |
| **2** | 19-20 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby comment.line.number-sign.ruby` | 58-76 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby meta.group.regexp.ruby` |
| **3** | 20-77 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby comment.line.number-sign.ruby` | 76-77 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby meta.group.regexp.ruby punctuation.definition.group.ruby` |

TokenizeLine2 official `[0, 33686016, 18, 33980672]`, branch `[0, 33686016]`

### `activerecord/lib/active_record/connection_adapters/postgresql/quoting.rb:19` (both; reproduces from line 15 with the initial state)

```
            /
              \A
              (
                (?:
                  # "schema_name"."table_name"."column_name"::type_name | function(one or no argument)::type_name
```

| # | Official | Branch |
|--:|---|---|
| **0** | 0-18 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby` | 0-82 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby` |
| **1** | 18-19 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby comment.line.number-sign.ruby punctuation.definition.comment.ruby` | 82-83 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby meta.group.regexp.ruby punctuation.definition.group.ruby` |
| **2** | 19-20 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby comment.line.number-sign.ruby` | 83-101 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby meta.group.regexp.ruby` |
| **3** | 20-113 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby comment.line.number-sign.ruby` | 101-102 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby meta.group.regexp.ruby punctuation.definition.group.ruby` |
| **4** |  | 102-114 `source.ruby string.regexp.classic.ruby meta.group.regexp.ruby meta.group.regexp.ruby` |

TokenizeLine2 official `[0, 33686016, 18, 33980672]`, branch `[0, 33686016]`

… 3 more discrepancies in report.json, 0 more lines not recorded.
