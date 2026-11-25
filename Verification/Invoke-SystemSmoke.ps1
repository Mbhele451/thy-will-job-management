param(
    [string]$BaseUrl = 'http://localhost:57260',
    [Parameter(Mandatory=$true)][hashtable]$Accounts,
    [string]$OutputFile = "$PSScriptRoot/system-http-results.json"
)
$ErrorActionPreference = 'Stop'
Import-Module Microsoft.PowerShell.Utility
$results = [System.Collections.Generic.List[object]]::new()
function Record($role, $path, $passed, $detail) {
    $results.Add([pscustomobject]@{role=$role; path=$path; passed=$passed; detail=$detail})
    Write-Host "$role $path : $passed $detail"
}
function Request($path, $session) {
    Invoke-WebRequest "$BaseUrl/$path" -WebSession $session -UseBasicParsing -SkipHttpErrorCheck -TimeoutSec 40
}
$pages = Get-ChildItem "$PSScriptRoot/../MajorProject.Web/Secure" -Recurse -Filter *.aspx |
    ForEach-Object { 'Secure/' + ($_.FullName -split '\\Secure\\', 2)[1].Replace('\','/') }
foreach ($role in @('Anonymous') + @($Accounts.Keys)) {
    $effectiveRole = if ($role -ne 'Anonymous' -and $Accounts[$role].Role) { $Accounts[$role].Role } else { $role }
    $session = [Microsoft.PowerShell.Commands.WebRequestSession]::new()
    if ($role -ne 'Anonymous') {
        $login = Request 'Account/Login.aspx' $session
        $body = @{}
        foreach ($field in $login.InputFields) {
            if ($field.type -eq 'hidden') { $body[$field.name] = $field.value }
        }
        $body['ctl00$MainContent$txtUsername'] = $Accounts[$role].Username
        $body['ctl00$MainContent$txtPassword'] = $Accounts[$role].Password
        $body['ctl00$MainContent$btnLogin'] = 'Sign In'
        $signedIn = Invoke-WebRequest "$BaseUrl/Account/Login.aspx" -Method Post -Body $body -WebSession $session -UseBasicParsing -SkipHttpErrorCheck -TimeoutSec 40
        $url = $signedIn.BaseResponse.RequestMessage.RequestUri.AbsolutePath
        $ok = $signedIn.StatusCode -eq 200 -and $url -like '/Secure/*'
        Record $role 'Account/Login.aspx' $ok $url
        if (!$ok) { continue }
    }
    foreach ($path in $pages) {
        try {
            $response = Request $path $session
            $final = $response.BaseResponse.RequestMessage.RequestUri.AbsolutePath
            $owner = ($path -split '/')[1]
            $isShared = $owner -in @('Profile.aspx','Dashboard.aspx')
            $allowed = $role -ne 'Anonymous' -and ($owner -eq $effectiveRole -or $isShared -or ($owner -eq 'Staff' -and $effectiveRole -in @('Admin','Manager')))
            # Download endpoints require a job selection, so verify graceful handling separately.
            $download = $path -match '(InvoicePdf|PaymentReceipt|PayInvoice|CashPayment)\.aspx$'
            if (!$allowed) {
                $ok = $response.StatusCode -eq 200 -and $final -match '/Account/(Login|AccessDenied)\.aspx$'
            } elseif ($download) {
                $ok = $response.StatusCode -in @(200,400,404) -and ($response.StatusCode -ne 200 -or $response.Content -notmatch "Server Error in '/' Application")
            } else {
                $ok = $response.StatusCode -eq 200 -and $final -notmatch '/Account/(Login|AccessDenied)\.aspx$' -and $response.Content -notmatch "Server Error in '/' Application"
            }
            $detail = "HTTP $($response.StatusCode) $final"
            if ($response.StatusCode -ge 500) {
                $plain = [System.Net.WebUtility]::HtmlDecode(($response.Content -replace '<[^>]+>', ' ' -replace '\s+', ' '))
                $match = [regex]::Match($plain, '(?:Parser Error Message|Compiler Error Message|Exception Details):.{0,1000}')
                if ($match.Success) { $detail += ' ' + $match.Value }
            }
            Record $role $path $ok $detail
        } catch { Record $role $path $false $_.Exception.Message }
    }
    if ($role -ne 'Anonymous') { $null = Request 'Account/Logout.aspx' $session }
}
$results | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath $OutputFile -Encoding utf8
Write-Host "Passed $(@($results | Where-Object passed).Count) / $($results.Count)"
if (@($results | Where-Object { !$_.passed }).Count) { exit 1 }
