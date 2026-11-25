param([switch]$ReadOnlyDatabase)
$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$buildTools = 'C:/Program Files/Microsoft Visual Studio/2022/Community/MSBuild/Current/Bin'
$auditOutput = Join-Path $root 'bin\DesktopStateAudit\'
Push-Location $root
try {
    & "$buildTools/MSBuild.exe" Major_Project.sln /t:Build /p:Configuration=Debug "/p:OutDir=$auditOutput" /v:quiet /nologo /clp:ErrorsOnly
    if ($LASTEXITCODE -ne 0) { throw 'Solution build failed.' }
    & "$buildTools/Roslyn/csc.exe" /nologo /out:bin/DesktopStateAudit/DesktopStateAudit.exe /r:System.Windows.Forms.dll /r:System.Drawing.dll /r:System.Data.dll /r:System.Xml.dll Verification/DesktopStateAudit.cs
    if ($LASTEXITCODE -ne 0) { throw 'State audit compilation failed.' }
    Copy-Item -LiteralPath "$auditOutput/Major_Project.exe.config" -Destination "$auditOutput/DesktopStateAudit.exe.config"
    & ./bin/DesktopStateAudit/DesktopStateAudit.exe
    if ($LASTEXITCODE -ne 0) { throw 'Offline state audit failed. See bin/DesktopStateAudit/desktop-state-results.txt.' }
    if ($ReadOnlyDatabase) {
        & ./bin/DesktopStateAudit/DesktopStateAudit.exe --read-only
        if ($LASTEXITCODE -ne 0) { throw 'Read-only database state audit failed.' }
    }
} finally { Pop-Location }
