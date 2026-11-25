param([switch]$RenderPdf, [switch]$Database)
$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$buildTools = 'C:/Program Files/Microsoft Visual Studio/2022/Community/MSBuild/Current/Bin'
$workflowOutput = Join-Path $root 'bin\DesktopWorkflowVerification\'
Push-Location $root
try {
    & "$buildTools/MSBuild.exe" Major_Project.sln /t:Build /p:Configuration=Debug "/p:OutDir=$workflowOutput" /v:quiet /nologo /clp:ErrorsOnly
    if ($LASTEXITCODE -ne 0) { throw 'Solution build failed.' }
    & "$buildTools/Roslyn/csc.exe" /nologo /out:bin/DesktopWorkflowVerification/DesktopWorkflowChecks.exe /r:System.Windows.Forms.dll /r:System.Drawing.dll /r:System.Data.dll /r:System.Xml.dll /r:bin/DesktopWorkflowVerification/Major_Project.exe /r:bin/DesktopWorkflowVerification/MajorProject.Business.dll /r:bin/DesktopWorkflowVerification/MajorProject.Data.dll Verification/DesktopWorkflowChecks.cs
    if ($LASTEXITCODE -ne 0) { throw 'Workflow check compilation failed.' }
    $checkArgs = @()
    if ($RenderPdf) { $checkArgs += '--render-pdf' }
    if ($Database) { $checkArgs += '--database' }
    & ./bin/DesktopWorkflowVerification/DesktopWorkflowChecks.exe @checkArgs
    if ($LASTEXITCODE -ne 0) { throw 'Workflow checks failed.' }
} finally { Pop-Location }
