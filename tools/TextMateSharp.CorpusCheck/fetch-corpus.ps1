#!/usr/bin/env pwsh
# Fetches the corpora listed in corpora.json at their pinned commits (shallow, blobless, sparse).
#   ./fetch-corpus.ps1 [-CorpusDir ./corpus] [-Only csharp,go]
param(
    [string]$CorpusDir = (Join-Path $PSScriptRoot 'corpus'),
    [string[]]$Only = @(),
    [int]$Parallel = 6
)
$ErrorActionPreference = 'Stop'
$corpora = (Get-Content (Join-Path $PSScriptRoot 'corpora.json') -Raw | ConvertFrom-Json).corpora
$Only = @($Only | ForEach-Object { $_ -split "," } | Where-Object { $_ })
if ($Only.Count -gt 0) { $corpora = $corpora | Where-Object { $Only -contains $_.name } }
New-Item -ItemType Directory -Force $CorpusDir | Out-Null

$corpora | ForEach-Object -ThrottleLimit $Parallel -Parallel {
    $c = $_
    $dir = Join-Path $using:CorpusDir $c.name
    $git = { param([string[]]$a) & git -C $dir @a 2>&1 | Out-Null; if ($LASTEXITCODE -ne 0) { throw "git $a failed in $dir" } }
    if ((Test-Path (Join-Path $dir '.git')) -and ((& git -C $dir rev-parse HEAD 2>$null) -eq $c.commit)) {
        Write-Host "$($c.name): up to date"
        return
    }
    New-Item -ItemType Directory -Force $dir | Out-Null
    if (-not (Test-Path (Join-Path $dir '.git'))) {
        & $git @('init', '-q')
        & $git @('remote', 'add', 'origin', $c.repo)
    }
    & $git @('config', 'core.longpaths', 'true')
    & $git (@('sparse-checkout', 'set', '--no-cone') + @($c.sparse))
    & $git @('fetch', '-q', '--depth', '1', '--filter=blob:none', 'origin', $c.commit)
    & $git @('checkout', '-q', '--force', $c.commit)
    $count = (git -C $dir ls-files -t | Where-Object { $_ -like 'H *' } | Measure-Object).Count
    Write-Host "$($c.name): $($c.repo)@$($c.commit.Substring(0, 12)) ($count files)"
}
