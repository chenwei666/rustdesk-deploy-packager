$ErrorActionPreference = 'Stop'

$csc = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if (-not (Test-Path $csc)) {
    $csc = Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe'
}
if (-not (Test-Path $csc)) {
    throw 'Could not find .NET Framework csc.exe.'
}

& $csc /nologo /target:exe /platform:anycpu /reference:System.Drawing.dll /out:MakeGeneratorIcon.exe MakeGeneratorIcon.cs
& .\MakeGeneratorIcon.exe
Remove-Item .\MakeGeneratorIcon.exe -ErrorAction SilentlyContinue

& $csc /nologo /codepage:65001 /target:winexe /platform:anycpu /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /win32icon:RustDeskConfigGenerator.ico /out:RustDeskConfigGenerator.exe RustDeskConfigGenerator.cs

Write-Host 'Built RustDeskConfigGenerator.exe'
