param([Parameter(Mandatory=$true)][string]$Username,[Parameter(Mandatory=$true)][string]$Password,[string]$BaseUrl='http://localhost:57260')
$ErrorActionPreference='Stop'
Import-Module Microsoft.PowerShell.Utility
$session=[Microsoft.PowerShell.Commands.WebRequestSession]::new()
$results=[System.Collections.Generic.List[object]]::new()
function Fields($page){
    $body=@{}
    foreach($field in $page.InputFields){
        if($field.type -notin @('submit','button','checkbox','radio') -and $field.name){$body[$field.name]=$field.value}
    }
    return $body
}
function Check($name,$passed){$results.Add([pscustomobject]@{test=$name;passed=[bool]$passed});Write-Host "$name : $passed"}
function InvalidPost($name,$path,$button,$changes,$expected){
    $page=Invoke-WebRequest "$BaseUrl/$path" -WebSession $session
    $body=Fields $page
    $body['ctl00$MainContent$'+$button]='Submit'
    foreach($key in $changes.Keys){$body['ctl00$MainContent$'+$key]=$changes[$key]}
    $response=Invoke-WebRequest "$BaseUrl/$path" -WebSession $session -Method Post -Body $body -SkipHttpErrorCheck
    Check $name ($response.StatusCode -eq 200 -and $response.Content -match $expected -and $response.Content -notmatch "Server Error in '/' Application")
}
$login=Invoke-WebRequest "$BaseUrl/Account/Login.aspx" -WebSession $session
$body=Fields $login
$body['ctl00$MainContent$txtUsername']=$Username;$body['ctl00$MainContent$txtPassword']=$Password;$body['ctl00$MainContent$btnLogin']='Sign In'
$login=Invoke-WebRequest "$BaseUrl/Account/Login.aspx" -WebSession $session -Method Post -Body $body
if($login.BaseResponse.RequestMessage.RequestUri.AbsolutePath -ne '/Secure/Manager/Dashboard.aspx'){throw 'Manager sign-in failed'}
try {
    InvalidPost 'Inventory negative initial quantity rejected by server' 'Secure/Manager/Inventory.aspx' 'btnAddItem' @{txtNewQuantity='-1'} 'Initial quantity must be zero or greater'
    InvalidPost 'Inventory zero unit price rejected by server' 'Secure/Manager/Inventory.aspx' 'btnAddItem' @{txtNewQuantity='0';txtNewUnitPrice='0'} 'Unit price must be greater than zero'
    InvalidPost 'Inventory negative reorder level rejected by server' 'Secure/Manager/Inventory.aspx' 'btnAddItem' @{txtNewQuantity='0';txtNewUnitPrice='1';txtNewReorderLevel='-1'} 'Reorder level must be zero or greater'
    InvalidPost 'Inventory adjustment requires selected item' 'Secure/Manager/Inventory.aspx' 'btnUpdateQuantity' @{txtStockQuantity='1'} 'Select an inventory item first'
    InvalidPost 'Staff creation requires a role and name' 'Secure/Manager/StaffManagement.aspx' 'btnAddStaff' @{} 'Select a valid staff role|Full Name is required'
    $staff=Invoke-WebRequest "$BaseUrl/Secure/Manager/StaffManagement.aspx" -WebSession $session
    Check 'Manager role pickers cannot grant SysAdmin' ($staff.Content -notmatch '<option[^>]*>SysAdmin</option>')
    $null=Invoke-WebRequest "$BaseUrl/Account/Logout.aspx" -WebSession $session
    InvalidPost 'Empty public registration rejected by server' 'Account/Register.aspx' 'btnRegister' @{} 'Please enter your full name\.|Public registration is currently closed'
    foreach($path in @('Default.aspx','Public/About.aspx','Public/Services.aspx','Public/Contact.aspx','Public/Terms.aspx','Public/PrivacyPolicy.aspx','Account/Login.aspx','Account/Register.aspx','Account/forgotPassword.aspx')){
        $response=Invoke-WebRequest "$BaseUrl/$path" -WebSession $session -SkipHttpErrorCheck
        Check "Public page $path loads" ($response.StatusCode -eq 200 -and $response.Content -notmatch "Server Error in '/' Application")
    }
} finally {
    $results | ConvertTo-Json | Set-Content "$PSScriptRoot/form-validation-results.json" -Encoding utf8
}
if(@($results|Where-Object {!$_.passed}).Count){exit 1}
