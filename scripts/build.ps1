$ErrorActionPreference = 'Stop'

$RepoRoot = Split-Path -Parent $PSScriptRoot
$SrcDir = Join-Path $RepoRoot 'src'
$AssetsDir = Join-Path $RepoRoot 'assets'
$DistDir = Join-Path $RepoRoot 'dist\release'
New-Item -ItemType Directory -Force -Path $AssetsDir, $DistDir | Out-Null

$csc = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if (-not (Test-Path $csc)) {
    $csc = Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe'
}
if (-not (Test-Path $csc)) {
    throw 'Could not find .NET Framework csc.exe.'
}

$iconBuilder = Join-Path $DistDir 'MakeGeneratorIcon.exe'
$iconPath = Join-Path $AssetsDir 'RustDeskConfigGenerator.ico'
$generatorExe = Join-Path $DistDir 'RustDeskConfigGenerator.exe'

& $csc /nologo /target:exe /platform:anycpu /reference:System.Drawing.dll /out:$iconBuilder (Join-Path $SrcDir 'MakeGeneratorIcon.cs')
Push-Location $AssetsDir
try {
    & $iconBuilder
} finally {
    Pop-Location
}
Remove-Item $iconBuilder -ErrorAction SilentlyContinue

& $csc /nologo /codepage:65001 /target:winexe /platform:anycpu /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /win32icon:$iconPath /out:$generatorExe (Join-Path $SrcDir 'RustDeskConfigGenerator.cs')

Write-Host "Built $generatorExe"
