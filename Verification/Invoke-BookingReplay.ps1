param([Parameter(Mandatory=$true)][string]$Username,[Parameter(Mandatory=$true)][string]$Password,[string]$BaseUrl='http://localhost:57260')
# Creates one clearly labelled, real test booking in the configured application database.
$ErrorActionPreference='Stop'
Import-Module Microsoft.PowerShell.Utility
$session=[Microsoft.PowerShell.Commands.WebRequestSession]::new()
function Fields($page){$body=@{};foreach($field in $page.InputFields){if($field.name -and $field.type -notin @('submit','checkbox','radio','button')){$body[$field.name]=$field.value}};return $body}
$login=Invoke-WebRequest "$BaseUrl/Account/Login.aspx" -WebSession $session
$body=Fields $login
$body['ctl00$MainContent$txtUsername']=$Username;$body['ctl00$MainContent$txtPassword']=$Password;$body['ctl00$MainContent$btnLogin']='Sign In'
$login=Invoke-WebRequest "$BaseUrl/Account/Login.aspx" -WebSession $session -Method Post -Body $body
if($login.BaseResponse.RequestMessage.RequestUri.AbsolutePath -ne '/Secure/Customer/Dashboard.aspx'){throw 'Customer sign-in failed'}
$booking=Invoke-WebRequest "$BaseUrl/Secure/Customer/BookJob.aspx" -WebSession $session
$body=Fields $booking
$body['ctl00$MainContent$svc_3']='3';$body['ctl00$MainContent$svc_5']='5'
$body['ctl00$MainContent$btnAddSelected']='Add selected services'
$cart=Invoke-WebRequest "$BaseUrl/Secure/Customer/BookJob.aspx" -WebSession $session -Method Post -Body $body
if($cart.Content -notmatch 'Selected services added'){throw 'The two-service selection was not added'}
$body=Fields $cart
$body['ctl00$MainContent$txtDescription']='CODEX-QA-20261116 MULTISERVICE REPLAY'
$body['ctl00$MainContent$txtPromiseDate']=[DateTime]::Today.ToString('yyyy-MM-dd')
$body['ctl00$MainContent$btnSubmit']='Submit booking'
$saved=Invoke-WebRequest "$BaseUrl/Secure/Customer/BookJob.aspx" -WebSession $session -Method Post -Body $body -TimeoutSec 60
$match=[regex]::Match($saved.Content,'Booking submitted\. Your reference is Job #(\d+)')
if(!$match.Success){throw 'The multi-service booking did not report success'}
$jobID=[int]$match.Groups[1].Value
# Replay the exact completed form, including its original signed ViewState.
$replay=Invoke-WebRequest "$BaseUrl/Secure/Customer/BookJob.aspx" -WebSession $session -Method Post -Body $body -TimeoutSec 60
$ok=$replay.Content -match "This booking was already submitted\. Your reference is Job #$jobID\."
$results=@([pscustomobject]@{test='Two-service booking submitted';passed=$true;jobID=$jobID},[pscustomobject]@{test='Exact duplicate POST returns original booking';passed=$ok;jobID=$jobID})
$results | ConvertTo-Json | Set-Content "$PSScriptRoot/booking-replay-results.json" -Encoding utf8
$results | Format-Table
$null=Invoke-WebRequest "$BaseUrl/Account/Logout.aspx" -WebSession $session
if(!$ok){exit 1}
