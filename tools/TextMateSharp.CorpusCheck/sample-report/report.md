# TextMateSharp corpus check

- Official: TextMateSharp 2.0.4+622d1b240a5be474939857c0b2780249022f14a3 (Onigwrap)
- Branch: TextMateSharp 1.0.0+ad9d3a713e65eae24a46f0c63bcd159d35cb07e7 (GregsEx)
- Machine: Mac, macOS 15.8.1, Arm64, 14 cores; .NET 10.0.11; 14 threads
- Started 2026-10-24 16:28:27Z, took 00:00:34
- **606,013 lines, 0 mismatched, 0 errors** in 1,466 files

Tokens per second count TokenizeLine tokens over the time spent in TokenizeLine on each engine (both run on the same thread, interleaved line by line); the first lines of each grammar on each thread include compiling its rules.

| Corpus | Scope | Source | Files | Lines | Matching | Mismatched (tokens / metadata) | Files w/ mismatch | Errors (O/B) | Timeouts (O/B) | Official tok/s | Branch tok/s | Speedup |
|---|---|---|--:|--:|--:|--:|--:|--:|--:|--:|--:|--:|
| csharp | `source.cs` | [dotnet/roslyn@e3c31f15b7](https://github.com/dotnet/roslyn/tree/e3c31f15b79e226656e3b082a921d4d60127b1fa) | 30 | 36,552 | 100.000% | 0 | 0 | 0/0 | 0/0 | 18,790 | 73,425 | 3.91x |
| vb | `source.asp.vb.net` | [dotnet/roslyn@e3c31f15b7](https://github.com/dotnet/roslyn/tree/e3c31f15b79e226656e3b082a921d4d60127b1fa) | 30 | 7,958 | 100.000% | 0 | 0 | 0/0 | 0/0 | 67,748 | 361,685 | 5.34x |
| fsharp | `source.fsharp` | [dotnet/fsharp@fe180deb55](https://github.com/dotnet/fsharp/tree/fe180deb551cc47a57de788c936e2fd4d778a909) | 30 | 27,575 | 100.000% | 0 | 0 | 0/0 | 0/0 | 34,093 | 127,960 | 3.75x |
| razor | `text.html.cshtml` | [dotnet/aspnetcore@ceb0eec49d](https://github.com/dotnet/aspnetcore/tree/ceb0eec49d8d2c77f4f7c26b85ddb6e96940dfed) | 30 | 2,365 | 100.000% | 0 | 0 | 0/0 | 0/0 | 2,425 | 12,978 | 5.35x |
| typescript | `source.ts` | [microsoft/TypeScript@c63de15a99](https://github.com/microsoft/TypeScript/tree/c63de15a992d37f0d6cec03ac7631872838602cb) | 30 | 108,683 | 100.000% | 0 | 0 | 0/0 | 0/0 | 28,662 | 92,559 | 3.23x |
| tsx | `source.tsx` | [microsoft/vscode@1ce5ffeb06](https://github.com/microsoft/vscode/tree/1ce5ffeb06bc84eaa81f5a2baba2e36780dad785) | 30 | 8,763 | 100.000% | 0 | 0 | 0/0 | 0/0 | 10,807 | 51,747 | 4.79x |
| javascript | `source.js` | [nodejs/node@91c4436158](https://github.com/nodejs/node/tree/91c4436158c615b0e3bb1b6a02c2c85eea0d1adf) | 30 | 14,547 | 100.000% | 0 | 0 | 0/0 | 0/0 | 19,009 | 74,973 | 3.94x |
| json | `source.json` | [microsoft/vscode@1ce5ffeb06](https://github.com/microsoft/vscode/tree/1ce5ffeb06bc84eaa81f5a2baba2e36780dad785) | 30 | 4,007 | 100.000% | 0 | 0 | 0/0 | 0/0 | 64,806 | 2,435,463 | 37.58x |
| go | `source.go` | [golang/go@1fce9bafb0](https://github.com/golang/go/tree/1fce9bafb0581a5d894b80d91674659854104012) | 30 | 11,899 | 100.000% | 0 | 0 | 0/0 | 0/0 | 60,968 | 463,679 | 7.61x |
| python | `source.python` | [python/cpython@11598362a3](https://github.com/python/cpython/tree/11598362a3ebbc8bfd1c4c37ec63ee565136868c) | 30 | 20,573 | 100.000% | 0 | 0 | 0/0 | 0/0 | 22,393 | 147,799 | 6.60x |
| rust | `source.rust` | [tokio-rs/tokio@8a2408edcf](https://github.com/tokio-rs/tokio/tree/8a2408edcfa2b7f19c00428a9d3a8ea6652a083f) | 30 | 5,284 | 100.000% | 0 | 0 | 0/0 | 0/0 | 20,425 | 546,054 | 26.73x |
| java | `source.java` | [google/guava@1b1d120f9c](https://github.com/google/guava/tree/1b1d120f9cd79ed5dcb1eb1e355e8f48b2b1bc6f) | 30 | 5,901 | 100.000% | 0 | 0 | 0/0 | 0/0 | 11,760 | 111,637 | 9.49x |
| c | `source.c` | [git/git@7e34d8174d](https://github.com/git/git/tree/7e34d8174dcda3435d1404c9a30bef46afe6c13e) | 30 | 19,114 | 100.000% | 0 | 0 | 0/0 | 0/0 | 14,620 | 90,526 | 6.19x |
| cpp | `source.cpp` | [fmtlib/fmt@0fc31633e7](https://github.com/fmtlib/fmt/tree/0fc31633e7d53ac895ae0a688b83b7d44f0b0c1f) | 30 | 21,094 | 100.000% | 0 | 0 | 0/0 | 0/0 | 6,194 | 15,499 | 2.50x |
| shellscript | `source.shell` | [git/git@7e34d8174d](https://github.com/git/git/tree/7e34d8174dcda3435d1404c9a30bef46afe6c13e) | 30 | 6,482 | 100.000% | 0 | 0 | 0/0 | 0/0 | 10,855 | 16,545 | 1.52x |
| makefile | `source.makefile` | [git/git@7e34d8174d](https://github.com/git/git/tree/7e34d8174dcda3435d1404c9a30bef46afe6c13e) | 21 | 6,101 | 100.000% | 0 | 0 | 0/0 | 0/0 | 24,562 | 782,948 | 31.88x |
| ruby | `source.ruby` | [rails/rails@51811570ff](https://github.com/rails/rails/tree/51811570ff56fcd8f4e535fa0f6f2aefb45bdefd) | 30 | 8,618 | 100.000% | 0 | 0 | 0/0 | 0/0 | 17,020 | 155,402 | 9.13x |
| php | `source.php` | [laravel/framework@5d77975287](https://github.com/laravel/framework/tree/5d7797528723107cd4f495326838a7d7d67fbb20) | 30 | 3,318 | 100.000% | 0 | 0 | 0/0 | 0/0 | 4,374 | 12,777 | 2.92x |
| ini | `source.ini` | [php/php-src@97374e0bd8](https://github.com/php/php-src/tree/97374e0bd8e54907dc80b0cd73d543f67bdffbd2) | 10 | 31,727 | 100.000% | 0 | 0 | 0/0 | 0/0 | 172,901 | 383,141 | 2.22x |
| perl | `source.perl` | [mojolicious/mojo@4dc4eb2e9c](https://github.com/mojolicious/mojo/tree/4dc4eb2e9ca2037e7fc4e8f65155d2203e46e08c) | 30 | 9,546 | 100.000% | 0 | 0 | 0/0 | 0/0 | 21,463 | 675,302 | 31.46x |
| lua | `source.lua` | [neovim/neovim@7b87ce822e](https://github.com/neovim/neovim/tree/7b87ce822e6fd45a4d9993599c2854afdb0dc4dd) | 30 | 15,851 | 100.000% | 0 | 0 | 0/0 | 0/0 | 69,945 | 980,208 | 14.01x |
| powershell | `source.powershell` | [PowerShell/PowerShell@ac15e9a5fa](https://github.com/PowerShell/PowerShell/tree/ac15e9a5fa45aa97964321ce08702c97b2414616) | 30 | 6,183 | 100.000% | 0 | 0 | 0/0 | 0/0 | 20,781 | 41,531 | 2.00x |
| bat | `source.batchfile` | [dotnet/runtime@1df7015e61](https://github.com/dotnet/runtime/tree/1df7015e61349dafdc12ceb1b14840c79d60ea48) | 30 | 1,699 | 100.000% | 0 | 0 | 0/0 | 0/0 | 9,631 | 90,797 | 9.43x |
| xml | `text.xml` | [dotnet/runtime@1df7015e61](https://github.com/dotnet/runtime/tree/1df7015e61349dafdc12ceb1b14840c79d60ea48) | 30 | 3,679 | 100.000% | 0 | 0 | 0/0 | 0/0 | 118,464 | 2,236,460 | 18.88x |
| sql | `source.sql` | [postgres/postgres@126976a8ff](https://github.com/postgres/postgres/tree/126976a8ffcfa47f40d3df09243ebf4606001912) | 30 | 14,235 | 100.000% | 0 | 0 | 0/0 | 0/0 | 19,267 | 66,976 | 3.48x |
| html | `text.html.derivative` | [mdn/learning-area@21973263ba](https://github.com/mdn/learning-area/tree/21973263ba859ff3477919087b488157f9d618a6) | 30 | 2,138 | 100.000% | 0 | 0 | 0/0 | 0/0 | 7,737 | 7,324 | 0.95x |
| css | `source.css` | [mdn/learning-area@21973263ba](https://github.com/mdn/learning-area/tree/21973263ba859ff3477919087b488157f9d618a6) | 30 | 3,411 | 100.000% | 0 | 0 | 0/0 | 0/0 | 11,800 | 42,179 | 3.57x |
| scss | `source.css.scss` | [twbs/bootstrap@6ae3f8690e](https://github.com/twbs/bootstrap/tree/6ae3f8690e9bf3c9a63c75eef3b995c7ce118bc9) | 30 | 5,918 | 100.000% | 0 | 0 | 0/0 | 0/0 | 5,109 | 11,276 | 2.21x |
| less | `source.css.less` | [less/less.js@713331655e](https://github.com/less/less.js/tree/713331655e437401dfea6cd233893a69762734e1) | 30 | 1,129 | 100.000% | 0 | 0 | 0/0 | 0/0 | 3,473 | 4,875 | 1.40x |
| yaml | `source.yaml` | [kubernetes/examples@d6b8cd27ea](https://github.com/kubernetes/examples/tree/d6b8cd27eacb51e651a1aa6f7c190a28713eff6e) | 30 | 1,072 | 100.000% | 0 | 0 | 0/0 | 0/0 | 16,812 | 498,471 | 29.65x |
| markdown | `text.html.markdown` | [microsoft/vscode-docs@a030905338](https://github.com/microsoft/vscode-docs/tree/a03090533810eb02e5b9a2a650e2a3df32192b42) | 30 | 6,784 | 100.000% | 0 | 0 | 0/0 | 0/0 | 6,979 | 27,812 | 3.98x |
| dart | `source.dart` | [dart-lang/sdk@f675d40d11](https://github.com/dart-lang/sdk/tree/f675d40d11576889472dbbbf3cc2cab278c8f706) | 30 | 16,260 | 100.000% | 0 | 0 | 0/0 | 0/0 | 86,702 | 1,169,870 | 13.49x |
| swift | `source.swift` | [apple/swift-nio@108daca35d](https://github.com/apple/swift-nio/tree/108daca35da6a1b4ed8da29c1dfbd19668b9b8fc) | 30 | 12,568 | 100.000% | 0 | 0 | 0/0 | 0/0 | 39,960 | 453,751 | 11.36x |
| objectivec | `source.objc` | [AFNetworking/AFNetworking@d9f589cc2c](https://github.com/AFNetworking/AFNetworking/tree/d9f589cc2c1fe9d55eb5eea00558010afea7a41e) | 30 | 7,461 | 100.000% | 0 | 0 | 0/0 | 0/0 | 9,852 | 14,120 | 1.43x |
| groovy | `source.groovy` | [apache/groovy@98c2f9f106](https://github.com/apache/groovy/tree/98c2f9f106d2ee368278ca33d105e540bf23baf0) | 30 | 5,565 | 100.000% | 0 | 0 | 0/0 | 0/0 | 9,611 | 248,888 | 25.90x |
| julia | `source.julia` | [JuliaLang/julia@bbfa0ce415](https://github.com/JuliaLang/julia/tree/bbfa0ce4150ea33f2532198cabcf319a49399360) | 30 | 24,622 | 100.000% | 0 | 0 | 0/0 | 0/0 | 39,274 | 226,868 | 5.78x |
| r | `source.r` | [tidyverse/ggplot2@b638d654c8](https://github.com/tidyverse/ggplot2/tree/b638d654c8189db82a62307fefa84b1da1819da5) | 30 | 8,324 | 100.000% | 0 | 0 | 0/0 | 0/0 | 18,299 | 102,362 | 5.59x |
| clojure | `source.clojure` | [clojure/clojure@903eee79d3](https://github.com/clojure/clojure/tree/903eee79d393cc2937933a2a05044ca2e9abca4c) | 30 | 18,979 | 100.000% | 0 | 0 | 0/0 | 0/0 | 60,583 | 288,698 | 4.77x |
| coffeescript | `source.coffee` | [jashkenas/coffeescript@4aaa755df3](https://github.com/jashkenas/coffeescript/tree/4aaa755df32ae7a55886226b153d1157ba01bb4a) | 30 | 34,508 | 100.000% | 0 | 0 | 0/0 | 0/0 | 45,360 | 157,347 | 3.47x |
| dockerfile | `source.dockerfile` | [docker-library/python@d01180b5d8](https://github.com/docker-library/python/tree/d01180b5d8b1664a9b87862e3cafeafbe87383c6) | 30 | 4,028 | 100.000% | 0 | 0 | 0/0 | 0/0 | 48,677 | 1,473,380 | 30.27x |
| diff | `source.diff` | [openwrt/openwrt@b5bc4becb4](https://github.com/openwrt/openwrt/tree/b5bc4becb424b4d89f48e372675811821aa155bd) | 30 | 2,268 | 100.000% | 0 | 0 | 0/0 | 0/0 | 48,022 | 1,164,372 | 24.25x |
| handlebars | `text.html.handlebars` | [TryGhost/Casper@cbdb5c6fca](https://github.com/TryGhost/Casper/tree/cbdb5c6fcaffcc8973af643c7c8878fc0d076e1b) | 25 | 798 | 100.000% | 0 | 0 | 0/0 | 0/0 | 5,553 | 141,192 | 25.42x |
| hlsl | `source.hlsl` | [microsoft/DirectX-Graphics-Samples@e5975f9b07](https://github.com/microsoft/DirectX-Graphics-Samples/tree/e5975f9b0744fc5096dd593ed72bf8f5c004164f) | 30 | 1,529 | 100.000% | 0 | 0 | 0/0 | 0/0 | 30,482 | 158,797 | 5.21x |
| shaderlab | `source.shaderlab` | [TwoTailsGames/Unity-Built-in-Shaders@6a63f93bc1](https://github.com/TwoTailsGames/Unity-Built-in-Shaders/tree/6a63f93bc1f20ce6cd47f981c7494e8328915621) | 30 | 2,787 | 100.000% | 0 | 0 | 0/0 | 0/0 | 33,594 | 170,929 | 5.09x |
| latex | `text.tex.latex` | [latex3/latex2e@3858fc13f7](https://github.com/latex3/latex2e/tree/3858fc13f7d800aaa0d394d56660ff9e72878d1d) | 30 | 11,640 | 100.000% | 0 | 0 | 0/0 | 0/0 | 13,324 | 131,807 | 9.89x |
| asciidoc | `document.adoc` | [asciidoctor/asciidoctor@3fbf3dee5f](https://github.com/asciidoctor/asciidoctor/tree/3fbf3dee5f7fe6e222ba003d1c77bc3afb4b1ff9) | 30 | 3,022 | 100.000% | 0 | 0 | 0/0 | 0/0 | 17,357 | 416,029 | 23.97x |
| pascal | `source.pascal` | [castle-engine/castle-engine@113d8bb8a2](https://github.com/castle-engine/castle-engine/tree/113d8bb8a2af17342049d99c464cbcadd195b366) | 30 | 16,825 | 100.000% | 0 | 0 | 0/0 | 0/0 | 72,393 | 560,809 | 7.75x |
| pug | `text.pug` | [pugjs/pug@c323ed3e63](https://github.com/pugjs/pug/tree/c323ed3e630b931bc04790efb509d34ac5927040) | 30 | 210 | 100.000% | 0 | 0 | 0/0 | 0/0 | 518 | 3,816 | 7.37x |
| typst | `source.typst` | [typst/typst@64c5e8c02f](https://github.com/typst/typst/tree/64c5e8c02fe0394d913ead36e42ef8c1334d2107) | 30 | 4,141 | 100.000% | 0 | 0 | 0/0 | 0/0 | 2,793 | 56,267 | 20.14x |
| x86asm | `source.x86asm` | [FFmpeg/FFmpeg@a881f0a9ad](https://github.com/FFmpeg/FFmpeg/tree/a881f0a9addd3d104b87d4f6b58b111b1d0cfd28) | 30 | 8,272 | 100.000% | 0 | 0 | 0/0 | 0/0 | 74,679 | 931,444 | 12.47x |
