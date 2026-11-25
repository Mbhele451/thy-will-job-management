$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$buildTools = 'C:/Program Files/Microsoft Visual Studio/2022/Community/MSBuild/Current/Bin'
Push-Location $root
try {
    & "$buildTools/MSBuild.exe" Major_Project.csproj /t:Build /p:Configuration=Debug /v:quiet /nologo /clp:ErrorsOnly
    if ($LASTEXITCODE -ne 0) { throw 'Desktop build failed.' }
    & "$buildTools/Roslyn/csc.exe" /nologo /out:bin/Debug/DesktopDesignerChecks.exe /r:System.Windows.Forms.dll /r:System.Drawing.dll /r:System.Data.dll Verification/DesktopDesignerChecks.cs UI/Forms/Common/GridPresentation.cs
    if ($LASTEXITCODE -ne 0) { throw 'Designer check compilation failed.' }
    & ./bin/Debug/DesktopDesignerChecks.exe "$root/bin/Debug/DesignerPreviews"
    if ($LASTEXITCODE -ne 0) { throw 'Designer checks failed.' }
} finally { Pop-Location }
