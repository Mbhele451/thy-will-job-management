param([switch]$ReadOnlyDatabase)
$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$buildTools = 'C:/Program Files/Microsoft Visual Studio/2022/Community/MSBuild/Current/Bin'
$output = Join-Path $root 'bin\DesktopCorrections\'
Push-Location $root
try {
    & "$buildTools/MSBuild.exe" Major_Project.sln /t:Build /p:Configuration=Debug "/p:OutDir=$output" /v:quiet /nologo /clp:ErrorsOnly
    if ($LASTEXITCODE -ne 0) { throw 'Solution build failed.' }
    & "$buildTools/Roslyn/csc.exe" /nologo /out:bin/DesktopCorrections/DesktopCorrectionChecks.exe /r:System.Windows.Forms.dll /r:System.Drawing.dll /r:System.Data.dll Verification/DesktopCorrectionChecks.cs
    if ($LASTEXITCODE -ne 0) { throw 'Correction check compilation failed.' }
    Copy-Item -LiteralPath "$output/Major_Project.exe.config" -Destination "$output/DesktopCorrectionChecks.exe.config"
    & ./bin/DesktopCorrections/DesktopCorrectionChecks.exe
    if ($LASTEXITCODE -ne 0) { throw 'Desktop correction checks failed.' }
    if ($ReadOnlyDatabase) {
        & ./bin/DesktopCorrections/DesktopCorrectionChecks.exe --read-only
        if ($LASTEXITCODE -ne 0) { throw 'Read-only correction checks failed.' }
    }
} finally { Pop-Location }
