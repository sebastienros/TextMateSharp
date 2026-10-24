# TextMateSharp corpus check

- Official: TextMateSharp 2.0.4+622d1b240a5be474939857c0b2780249022f14a3 (Onigwrap)
- Branch: TextMateSharp 1.0.0+d08c54d14911c78682826921092ec50bea1dcbe5 (GregsEx)
- Machine: CPC-sebro-SGN1O, Microsoft Windows 10.0.26200, X64, 16 cores; .NET 10.0.12; 16 threads
- Started 2026-10-24 17:57:36Z, took 00:04:43
- **7,352,306 lines, 0 mismatched, 0 errors** in 21,364 files

Tokens per second count TokenizeLine tokens over the time spent in TokenizeLine on each engine (both run on the same thread, interleaved line by line); the first lines of each grammar on each thread include compiling its rules.

| Corpus | Scope | Source | Files | Lines | Matching | Mismatched (tokens / metadata) | Files w/ mismatch | Errors (O/B) | Timeouts (O/B) | Official tok/s | Branch tok/s | Speedup |
|---|---|---|--:|--:|--:|--:|--:|--:|--:|--:|--:|--:|
| csharp | `source.cs` | [dotnet/roslyn@e3c31f15b7](https://github.com/dotnet/roslyn/tree/e3c31f15b79e226656e3b082a921d4d60127b1fa) | 1,167 | 548,795 | 100.000% | 0 | 0 | 0/0 | 0/0 | 16,454 | 67,775 | 4.12x |
| vb | `source.asp.vb.net` | [dotnet/roslyn@e3c31f15b7](https://github.com/dotnet/roslyn/tree/e3c31f15b79e226656e3b082a921d4d60127b1fa) | 919 | 475,565 | 100.000% | 0 | 0 | 0/0 | 0/0 | 35,494 | 202,473 | 5.70x |
| fsharp | `source.fsharp` | [dotnet/fsharp@fe180deb55](https://github.com/dotnet/fsharp/tree/fe180deb551cc47a57de788c936e2fd4d778a909) | 415 | 288,140 | 100.000% | 0 | 0 | 0/0 | 0/0 | 55,626 | 199,271 | 3.58x |
| razor | `text.html.cshtml` | [dotnet/aspnetcore@ceb0eec49d](https://github.com/dotnet/aspnetcore/tree/ceb0eec49d8d2c77f4f7c26b85ddb6e96940dfed) | 1,602 (+3 skipped) | 48,530 | 100.000% | 0 | 0 | 0/0 | 0/0 | 5,483 | 36,672 | 6.69x |
| typescript | `source.ts` | [microsoft/TypeScript@c63de15a99](https://github.com/microsoft/TypeScript/tree/c63de15a992d37f0d6cec03ac7631872838602cb) | 700 (+1 skipped) | 439,269 | 100.000% | 0 | 0 | 0/0 | 0/0 | 15,670 | 68,898 | 4.40x |
| tsx | `source.tsx` | [microsoft/vscode@1ce5ffeb06](https://github.com/microsoft/vscode/tree/1ce5ffeb06bc84eaa81f5a2baba2e36780dad785) | 267 (+1 skipped) | 64,912 | 100.000% | 0 | 0 | 0/0 | 0/0 | 12,347 | 43,743 | 3.54x |
| javascript | `source.js` | [nodejs/node@91c4436158](https://github.com/nodejs/node/tree/91c4436158c615b0e3bb1b6a02c2c85eea0d1adf) | 435 | 196,922 | 100.000% | 0 | 0 | 0/0 | 0/0 | 20,481 | 75,242 | 3.67x |
| json | `source.json` | [microsoft/vscode@1ce5ffeb06](https://github.com/microsoft/vscode/tree/1ce5ffeb06bc84eaa81f5a2baba2e36780dad785) | 1,142 (+13 skipped) | 673,498 | 100.000% | 0 | 0 | 0/0 | 0/0 | 268,528 | 595,665 | 2.22x |
| go | `source.go` | [golang/go@1fce9bafb0](https://github.com/golang/go/tree/1fce9bafb0581a5d894b80d91674659854104012) | 837 | 272,162 | 100.000% | 0 | 0 | 0/0 | 0/0 | 76,264 | 399,530 | 5.24x |
| python | `source.python` | [python/cpython@11598362a3](https://github.com/python/cpython/tree/11598362a3ebbc8bfd1c4c37ec63ee565136868c) | 2,082 | 1,108,254 | 100.000% | 0 | 0 | 0/0 | 0/0 | 26,769 | 159,204 | 5.95x |
| rust | `source.rust` | [tokio-rs/tokio@8a2408edcf](https://github.com/tokio-rs/tokio/tree/8a2408edcfa2b7f19c00428a9d3a8ea6652a083f) | 381 | 108,954 | 100.000% | 0 | 0 | 0/0 | 0/0 | 38,476 | 267,099 | 6.94x |
| java | `source.java` | [google/guava@1b1d120f9c](https://github.com/google/guava/tree/1b1d120f9cd79ed5dcb1eb1e355e8f48b2b1bc6f) | 610 (+1 skipped) | 181,311 | 100.000% | 0 | 0 | 0/0 | 0/0 | 31,184 | 153,796 | 4.93x |
| c | `source.c` | [git/git@7e34d8174d](https://github.com/git/git/tree/7e34d8174dcda3435d1404c9a30bef46afe6c13e) | 603 | 350,025 | 100.000% | 0 | 0 | 0/0 | 0/0 | 28,960 | 96,887 | 3.35x |
| cpp | `source.cpp` | [fmtlib/fmt@0fc31633e7](https://github.com/fmtlib/fmt/tree/0fc31633e7d53ac895ae0a688b83b7d44f0b0c1f) | 73 | 70,496 | 100.000% | 0 | 0 | 0/0 | 0/0 | 6,746 | 9,818 | 1.46x |
| shellscript | `source.shell` | [git/git@7e34d8174d](https://github.com/git/git/tree/7e34d8174dcda3435d1404c9a30bef46afe6c13e) | 1,112 | 322,800 | 100.000% | 0 | 0 | 0/0 | 0/0 | 79,333 | 251,011 | 3.16x |
| makefile | `source.makefile` | [git/git@7e34d8174d](https://github.com/git/git/tree/7e34d8174dcda3435d1404c9a30bef46afe6c13e) | 21 | 6,101 | 100.000% | 0 | 0 | 0/0 | 0/0 | 20,445 | 400,556 | 19.59x |
| ruby | `source.ruby` | [rails/rails@51811570ff](https://github.com/rails/rails/tree/51811570ff56fcd8f4e535fa0f6f2aefb45bdefd) | 419 | 78,676 | 100.000% | 0 | 0 | 0/0 | 0/0 | 31,836 | 125,679 | 3.95x |
| php | `source.php` | [laravel/framework@5d77975287](https://github.com/laravel/framework/tree/5d7797528723107cd4f495326838a7d7d67fbb20) | 1,744 | 276,205 | 100.000% | 0 | 0 | 0/0 | 0/0 | 16,342 | 56,152 | 3.44x |
| ini | `source.ini` | [php/php-src@97374e0bd8](https://github.com/php/php-src/tree/97374e0bd8e54907dc80b0cd73d543f67bdffbd2) | 10 | 31,727 | 100.000% | 0 | 0 | 0/0 | 0/0 | 303,568 | 1,083,396 | 3.57x |
| perl | `source.perl` | [mojolicious/mojo@4dc4eb2e9c](https://github.com/mojolicious/mojo/tree/4dc4eb2e9ca2037e7fc4e8f65155d2203e46e08c) | 112 | 35,345 | 100.000% | 0 | 0 | 0/0 | 0/0 | 47,190 | 299,602 | 6.35x |
| lua | `source.lua` | [neovim/neovim@7b87ce822e](https://github.com/neovim/neovim/tree/7b87ce822e6fd45a4d9993599c2854afdb0dc4dd) | 166 | 102,627 | 100.000% | 0 | 0 | 0/0 | 0/0 | 68,964 | 376,942 | 5.47x |
| powershell | `source.powershell` | [PowerShell/PowerShell@ac15e9a5fa](https://github.com/PowerShell/PowerShell/tree/ac15e9a5fa45aa97964321ce08702c97b2414616) | 555 | 144,953 | 100.000% | 0 | 0 | 0/0 | 0/0 | 23,679 | 149,748 | 6.32x |
| bat | `source.batchfile` | [dotnet/runtime@1df7015e61](https://github.com/dotnet/runtime/tree/1df7015e61349dafdc12ceb1b14840c79d60ea48) | 64 | 3,538 | 100.000% | 0 | 0 | 0/0 | 0/0 | 7,946 | 49,813 | 6.27x |
| xml | `text.xml` | [dotnet/runtime@1df7015e61](https://github.com/dotnet/runtime/tree/1df7015e61349dafdc12ceb1b14840c79d60ea48) | 41 | 9,279 | 100.000% | 0 | 0 | 0/0 | 0/0 | 185,860 | 1,017,121 | 5.47x |
| sql | `source.sql` | [postgres/postgres@126976a8ff](https://github.com/postgres/postgres/tree/126976a8ffcfa47f40d3df09243ebf4606001912) | 241 | 129,850 | 100.000% | 0 | 0 | 0/0 | 0/0 | 6,567 | 99,278 | 15.12x |
| html | `text.html.derivative` | [mdn/learning-area@21973263ba](https://github.com/mdn/learning-area/tree/21973263ba859ff3477919087b488157f9d618a6) | 154 | 8,693 | 100.000% | 0 | 0 | 0/0 | 0/0 | 4,036 | 17,115 | 4.24x |
| css | `source.css` | [mdn/learning-area@21973263ba](https://github.com/mdn/learning-area/tree/21973263ba859ff3477919087b488157f9d618a6) | 44 | 4,050 | 100.000% | 0 | 0 | 0/0 | 0/0 | 4,524 | 13,666 | 3.02x |
| scss | `source.css.scss` | [twbs/bootstrap@6ae3f8690e](https://github.com/twbs/bootstrap/tree/6ae3f8690e9bf3c9a63c75eef3b995c7ce118bc9) | 117 | 14,819 | 100.000% | 0 | 0 | 0/0 | 0/0 | 3,029 | 5,708 | 1.88x |
| less | `source.css.less` | [less/less.js@713331655e](https://github.com/less/less.js/tree/713331655e437401dfea6cd233893a69762734e1) | 315 | 9,469 | 100.000% | 0 | 0 | 0/0 | 0/0 | 2,666 | 11,561 | 4.34x |
| yaml | `source.yaml` | [kubernetes/examples@d6b8cd27ea](https://github.com/kubernetes/examples/tree/d6b8cd27eacb51e651a1aa6f7c190a28713eff6e) | 250 | 7,196 | 100.000% | 0 | 0 | 0/0 | 0/0 | 15,996 | 296,373 | 18.53x |
| markdown | `text.html.markdown` | [microsoft/vscode-docs@a030905338](https://github.com/microsoft/vscode-docs/tree/a03090533810eb02e5b9a2a650e2a3df32192b42) | 365 | 77,479 | 100.000% | 0 | 0 | 0/0 | 0/0 | 12,863 | 30,069 | 2.34x |
| dart | `source.dart` | [dart-lang/sdk@f675d40d11](https://github.com/dart-lang/sdk/tree/f675d40d11576889472dbbbf3cc2cab278c8f706) | 145 | 161,633 | 100.000% | 0 | 0 | 0/0 | 0/0 | 57,619 | 261,334 | 4.54x |
| swift | `source.swift` | [apple/swift-nio@108daca35d](https://github.com/apple/swift-nio/tree/108daca35da6a1b4ed8da29c1dfbd19668b9b8fc) | 309 | 114,967 | 100.000% | 0 | 0 | 0/0 | 0/0 | 86,487 | 216,710 | 2.51x |
| objectivec | `source.objc` | [AFNetworking/AFNetworking@d9f589cc2c](https://github.com/AFNetworking/AFNetworking/tree/d9f589cc2c1fe9d55eb5eea00558010afea7a41e) | 80 | 15,470 | 100.000% | 0 | 0 | 0/0 | 0/0 | 7,548 | 29,256 | 3.88x |
| groovy | `source.groovy` | [apache/groovy@98c2f9f106](https://github.com/apache/groovy/tree/98c2f9f106d2ee368278ca33d105e540bf23baf0) | 812 | 177,891 | 100.000% | 0 | 0 | 0/0 | 0/0 | 35,628 | 153,972 | 4.32x |
| julia | `source.julia` | [JuliaLang/julia@bbfa0ce415](https://github.com/JuliaLang/julia/tree/bbfa0ce4150ea33f2532198cabcf319a49399360) | 202 | 140,683 | 100.000% | 0 | 0 | 0/0 | 0/0 | 28,100 | 124,629 | 4.44x |
| r | `source.r` | [tidyverse/ggplot2@b638d654c8](https://github.com/tidyverse/ggplot2/tree/b638d654c8189db82a62307fefa84b1da1819da5) | 202 | 54,585 | 100.000% | 0 | 0 | 0/0 | 0/0 | 21,930 | 86,707 | 3.95x |
| clojure | `source.clojure` | [clojure/clojure@903eee79d3](https://github.com/clojure/clojure/tree/903eee79d393cc2937933a2a05044ca2e9abca4c) | 49 | 22,987 | 100.000% | 0 | 0 | 0/0 | 0/0 | 44,968 | 282,914 | 6.29x |
| coffeescript | `source.coffee` | [jashkenas/coffeescript@4aaa755df3](https://github.com/jashkenas/coffeescript/tree/4aaa755df32ae7a55886226b153d1157ba01bb4a) | 75 | 44,701 | 100.000% | 0 | 0 | 0/0 | 0/0 | 23,395 | 107,872 | 4.61x |
| dockerfile | `source.dockerfile` | [docker-library/python@d01180b5d8](https://github.com/docker-library/python/tree/d01180b5d8b1664a9b87862e3cafeafbe87383c6) | 44 | 5,916 | 100.000% | 0 | 0 | 0/0 | 0/0 | 108,692 | 572,145 | 5.26x |
| diff | `source.diff` | [openwrt/openwrt@b5bc4becb4](https://github.com/openwrt/openwrt/tree/b5bc4becb424b4d89f48e372675811821aa155bd) | 974 | 192,320 | 100.000% | 0 | 0 | 0/0 | 0/0 | 146,425 | 539,689 | 3.69x |
| handlebars | `text.html.handlebars` | [TryGhost/Casper@cbdb5c6fca](https://github.com/TryGhost/Casper/tree/cbdb5c6fcaffcc8973af643c7c8878fc0d076e1b) | 25 | 798 | 100.000% | 0 | 0 | 0/0 | 0/0 | 3,959 | 27,413 | 6.92x |
| hlsl | `source.hlsl` | [microsoft/DirectX-Graphics-Samples@e5975f9b07](https://github.com/microsoft/DirectX-Graphics-Samples/tree/e5975f9b0744fc5096dd593ed72bf8f5c004164f) | 446 | 31,357 | 100.000% | 0 | 0 | 0/0 | 0/0 | 29,303 | 157,427 | 5.37x |
| shaderlab | `source.shaderlab` | [TwoTailsGames/Unity-Built-in-Shaders@6a63f93bc1](https://github.com/TwoTailsGames/Unity-Built-in-Shaders/tree/6a63f93bc1f20ce6cd47f981c7494e8328915621) | 192 | 18,912 | 100.000% | 0 | 0 | 0/0 | 0/0 | 28,145 | 151,700 | 5.39x |
| latex | `text.tex.latex` | [latex3/latex2e@3858fc13f7](https://github.com/latex3/latex2e/tree/3858fc13f7d800aaa0d394d56660ff9e72878d1d) | 124 | 100,742 | 100.000% | 0 | 0 | 0/0 | 0/0 | 30,663 | 110,720 | 3.61x |
| asciidoc | `document.adoc` | [asciidoctor/asciidoctor@3fbf3dee5f](https://github.com/asciidoctor/asciidoctor/tree/3fbf3dee5f7fe6e222ba003d1c77bc3afb4b1ff9) | 99 | 9,371 | 100.000% | 0 | 0 | 0/0 | 0/0 | 25,733 | 120,000 | 4.66x |
| pascal | `source.pascal` | [castle-engine/castle-engine@113d8bb8a2](https://github.com/castle-engine/castle-engine/tree/113d8bb8a2af17342049d99c464cbcadd195b366) | 135 | 69,339 | 100.000% | 0 | 0 | 0/0 | 0/0 | 65,306 | 304,587 | 4.66x |
| pug | `text.pug` | [pugjs/pug@c323ed3e63](https://github.com/pugjs/pug/tree/c323ed3e630b931bc04790efb509d34ac5927040) | 137 | 1,195 | 100.000% | 0 | 0 | 0/0 | 0/0 | 178 | 2,221 | 12.47x |
| typst | `source.typst` | [typst/typst@64c5e8c02f](https://github.com/typst/typst/tree/64c5e8c02fe0394d913ead36e42ef8c1334d2107) | 224 | 35,797 | 100.000% | 0 | 0 | 0/0 | 0/0 | 8,228 | 42,339 | 5.15x |
| x86asm | `source.x86asm` | [FFmpeg/FFmpeg@a881f0a9ad](https://github.com/FFmpeg/FFmpeg/tree/a881f0a9addd3d104b87d4f6b58b111b1d0cfd28) | 127 | 64,002 | 100.000% | 0 | 0 | 0/0 | 0/0 | 20,127 | 383,003 | 19.03x |
