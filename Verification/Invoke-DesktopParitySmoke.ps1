param([switch]$CheckDatabaseSchema)

$ErrorActionPreference = 'Stop'
$repository = Split-Path $PSScriptRoot -Parent
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
$installation = & $vswhere -latest -version '[17.0,18.0)' -requires Microsoft.Component.MSBuild -property installationPath
if (!$installation) { throw 'Visual Studio 2022 with MSBuild is required.' }
$msbuild = Join-Path $installation 'MSBuild/Current/Bin/MSBuild.exe'
$csc = Join-Path $installation 'MSBuild/Current/Bin/Roslyn/csc.exe'

Push-Location $repository
try {
    $output = Join-Path $repository "bin/DesktopParitySmoke-$([guid]::NewGuid().ToString('N'))"
    $log = "$output-build.log"
    & $msbuild Major_Project.csproj /t:Build /p:Configuration=Debug "/p:OutDir=$output/" /v:minimal /nologo *> $log
    if ($LASTEXITCODE -ne 0) {
        Get-Content -LiteralPath $log -Tail 30
        throw 'Desktop build failed.'
    }
    Write-Host 'Desktop and shared projects built successfully.'

    & $csc /nologo "/out:$output/DesktopParitySmoke.exe" /r:System.Windows.Forms.dll /r:System.Drawing.dll /r:System.Data.dll /r:System.Configuration.dll "/r:$output/MajorProject.Business.dll" Verification/DesktopParitySmoke.cs
    if ($LASTEXITCODE -ne 0) { throw 'Desktop test fixture failed to compile.' }
    Copy-Item -LiteralPath "$output/Major_Project.exe.config" -Destination "$output/DesktopParitySmoke.exe.config"

    Push-Location $output
    try {
        .\DesktopParitySmoke.exe
        if ($LASTEXITCODE -ne 0) { throw 'Desktop UI smoke checks failed.' }

        if ($CheckDatabaseSchema) {
            .\DesktopParitySmoke.exe schema
            if ($LASTEXITCODE -ne 0) { throw 'The read-only database schema check failed.' }
        }
    } finally { Pop-Location }
} finally { Pop-Location }
