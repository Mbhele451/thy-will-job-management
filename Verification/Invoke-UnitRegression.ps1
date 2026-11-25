$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
$compiler='C:/Program Files/Microsoft Visual Studio/2022/Community/MSBuild/Current/Bin/Roslyn/csc.exe'
if(!(Test-Path $compiler)){throw 'Set the compiler path to the Roslyn csc.exe in your Visual Studio installation.'}
Push-Location $root
try {
    $output = Join-Path $root 'bin/UnitRegression'
    [void][IO.Directory]::CreateDirectory($output)
    $sources=@('Verification/SystemRegressionTests.cs','MajorProject.Business/Models/BookingCart.cs','MajorProject.Business/Models/CustomerIdentity.cs','MajorProject.Business/Helpers/ValidationHelper.cs','MajorProject.Business/Helpers/AdminPolicy.cs','MajorProject.Business/Helpers/CryptographyHelper.cs','MajorProject.Business/Services/Payments/PaymentInputValidation.cs','MajorProject.Web/Helpers/BookingSubmissionLedger.cs','MajorProject.Web/Helpers/CsvReportWriter.cs')
    & $compiler /nologo "/out:$output/SystemRegressionTests.exe" /r:System.Data.dll /r:System.Web.dll @sources
    if($LASTEXITCODE -ne 0){throw 'Test compilation failed'}
    & "$output/SystemRegressionTests.exe" | Tee-Object "$output/system-regression-results.txt"
    if($LASTEXITCODE -ne 0){throw 'Regression assertions failed'}
} finally {Pop-Location}
