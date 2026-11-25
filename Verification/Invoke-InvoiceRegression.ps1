param([Parameter(Mandatory=$true)][string]$Username,
      [Parameter(Mandatory=$true)][string]$Password,
      [int]$CompletedJobID=1021,[int]$PendingJobID=1020,[int]$OtherCustomerJobID=1,
      [string]$BaseUrl='http://localhost:57260')
$ErrorActionPreference='Stop'
Import-Module Microsoft.PowerShell.Utility
$session=[Microsoft.PowerShell.Commands.WebRequestSession]::new()
$login=Invoke-WebRequest "$BaseUrl/Account/Login.aspx" -WebSession $session
$body=@{}
foreach($field in $login.InputFields){if($field.type -eq 'hidden'){$body[$field.name]=$field.value}}
$body['ctl00$MainContent$txtUsername']=$Username
$body['ctl00$MainContent$txtPassword']=$Password
$body['ctl00$MainContent$btnLogin']='Sign In'
$login=Invoke-WebRequest "$BaseUrl/Account/Login.aspx" -Method Post -Body $body -WebSession $session
if($login.BaseResponse.RequestMessage.RequestUri.AbsolutePath -ne '/Secure/Customer/Dashboard.aspx'){throw 'Customer sign-in failed'}
$results=[System.Collections.Generic.List[object]]::new()
function Check($name,$passed){$results.Add([pscustomobject]@{test=$name;passed=[bool]$passed});Write-Host "$name : $passed"}
try {
    foreach($endpoint in @('InvoicePdf','PaymentReceipt')) {
        $response=Invoke-WebRequest "$BaseUrl/Secure/Customer/$endpoint.aspx?jobID=$CompletedJobID" -WebSession $session -SkipHttpErrorCheck
        $bytes=if($response.Content -is [byte[]]){$response.Content}else{[Text.Encoding]::ASCII.GetBytes($response.Content)}
        $pdf=$response.StatusCode -eq 200 -and $response.Headers['Content-Type'] -like '*application/pdf*' -and $bytes.Length -gt 5 -and [Text.Encoding]::ASCII.GetString($bytes,0,5) -eq '%PDF-'
        Check "Own $endpoint returns a PDF" $pdf
        foreach($id in @($OtherCustomerJobID,0,-1,2147483647)){
            $response=Invoke-WebRequest "$BaseUrl/Secure/Customer/$endpoint.aspx?jobID=$id" -WebSession $session -SkipHttpErrorCheck
            Check "$endpoint rejects inaccessible job $id" ($response.StatusCode -eq 404)
        }
    }
    $paid=Invoke-WebRequest "$BaseUrl/Secure/Customer/PayInvoice.aspx?jobID=$CompletedJobID" -WebSession $session
    Check 'Paid invoice shows receipt and hides new payment options' ($paid.Content -match 'SIM-' -and $paid.Content -match 'Download Receipt' -and $paid.Content -notmatch 'aria-label="Choose card payment"')
    $pending=Invoke-WebRequest "$BaseUrl/Secure/Customer/PayInvoice.aspx?jobID=$PendingJobID" -WebSession $session -SkipHttpErrorCheck
    Check 'Pending job cannot enter checkout' ($pending.BaseResponse.RequestMessage.RequestUri.AbsolutePath -eq '/Secure/Customer/Invoices.aspx')
    $other=Invoke-WebRequest "$BaseUrl/Secure/Customer/PayInvoice.aspx?jobID=$OtherCustomerJobID" -WebSession $session -SkipHttpErrorCheck
    Check 'Other customer cannot enter checkout' ($other.BaseResponse.RequestMessage.RequestUri.AbsolutePath -eq '/Secure/Customer/Invoices.aspx')
} finally {
    $null=Invoke-WebRequest "$BaseUrl/Account/Logout.aspx" -WebSession $session
    $results | ConvertTo-Json | Set-Content "$PSScriptRoot/invoice-regression-results.json" -Encoding utf8
}
if(@($results|Where-Object {!$_.passed}).Count){exit 1}
