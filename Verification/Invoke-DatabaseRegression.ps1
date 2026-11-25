$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
[xml]$config = Get-Content "$root/MajorProject.Web/Web.config"
$cs = ($config.configuration.connectionStrings.add | Where-Object name -eq 'GroupWst34ConnectionString').connectionString
$null = [Reflection.Assembly]::LoadFrom("$root/MajorProject.Web/bin/MajorProject.Data.dll")
$null = [Reflection.Assembly]::LoadFrom("$root/MajorProject.Web/bin/MajorProject.Business.dll")
$connection = [System.Data.SqlClient.SqlConnection]::new($cs)
$connection.Open()
$results = [System.Collections.Generic.List[object]]::new()
function Check($name, $condition) {
    $results.Add([pscustomobject]@{test=$name; passed=[bool]$condition})
    Write-Host "$name : $condition"
}
function Read($sql) {
    $command = $connection.CreateCommand(); $command.CommandText = $sql
    $table = [System.Data.DataTable]::new()
    $adapter = [System.Data.SqlClient.SqlDataAdapter]::new($command)
    $null = $adapter.Fill($table)
    return ,$table
}
function Reject($name, [scriptblock]$action, [string]$expected) {
    try { & $action; Check $name $false }
    catch {
        $message = $_.Exception.ToString()
        $passed = $message -match $expected
        Check $name $passed
        if (!$passed) { Write-Host "Unexpected rejection: $message" }
    }
}
try {
    $sysadmin = Read "SELECT s.StaffID FROM Staff s JOIN Roles r ON r.RoleID=s.RoleID WHERE s.Username='Mbhele45' AND r.RoleName='SysAdmin'"
    if ($sysadmin.Rows.Count -eq 1) {
        $security = [MajorProject.Data.Data.SystemAdminDataAccess]::new($cs)
        $accounts = $security.Accounts([int]$sysadmin.Rows[0].StaffID, '')
        Check 'SysAdmin accounts return Boolean block flags' ($accounts.Columns['IsBlocked'].DataType -eq [bool])
        Check 'SysAdmin account list contains staff and customers' ($accounts.Select("AccountType = 'Staff'").Count -gt 0 -and $accounts.Select("AccountType = 'Customer'").Count -gt 0)
    }
    $jobService = [MajorProject.Business.Services.JobService]::new($cs)
    $paymentService = [MajorProject.Business.Services.Payments.PaymentService]::new($cs)
    $customer = Read "SELECT CustomerID FROM Customers WHERE EmailAddress=''"
    $customerID = [int]$customer.Rows[0].CustomerID
    Check 'Customer cannot read another account job' ($jobService.GetJobByCustomerID($customerID, 1).Rows.Count -eq 0)
    Check 'Technician cannot read another technician job' ($jobService.GetTechnicianJobByID(1022, 3).Rows.Count -eq 0)
    Reject 'Technician cannot start another technician job' { $jobService.StartTechnicianJob(1022, 3) } 'does not belong'
    Reject 'Completed job cannot be restarted' { $jobService.StartTechnicianJob(1021, 4) } 'Only Pending'
    Reject 'Completed job cannot be finished twice' { $jobService.CompleteTechnicianJob(1021, 4) } 'In Progress'
    Reject 'Pending invoice cannot be paid' { $null = $paymentService.ProcessSimulatedInvoicePayment(1022, $customerID, 'EFT') } 'Only completed'
    Reject 'Cancelled invoice cannot be paid' { $null = $paymentService.ProcessSimulatedInvoicePayment(1020, $customerID, 'EFT') } 'Only completed'
    Reject 'Cancelled job cannot restart' { $jobService.StartTechnicianJob(1020, 3) } 'Only Pending'
    Reject 'Other customer invoice cannot be paid' { $null = $paymentService.ProcessSimulatedInvoicePayment(1, $customerID, 'EFT') } 'not found'
    Reject 'Unknown payment method rejected' { $null = $paymentService.ProcessSimulatedInvoicePayment(1021, $customerID, 'Unknown') } 'Invalid payment method'
    Reject 'Negative cash amount rejected' { $null = $paymentService.ProcessStaffCashPayment(1021, -1, 1) } 'greater than zero'
    Reject 'Fractional cent cash amount rejected' { $null = $paymentService.ProcessStaffCashPayment(1021, 1.001, 1) } 'two decimal places'
    Reject 'Paid invoice cannot be charged again' { $null = $paymentService.ProcessSimulatedInvoicePayment(1021, $customerID, 'EFT') } 'already been paid'
    $before = Read 'SELECT (SELECT COUNT(*) FROM Jobs) AS Jobs, (SELECT COUNT(*) FROM JobDetails) AS Details, (SELECT COUNT(*) FROM InventoryTransactions) AS Transactions, (SELECT SUM(CAST(QuantityInStock AS bigint)) FROM InventoryItems) AS Stock'
    $cart = [System.Data.DataTable]::new()
    $null = $cart.Columns.Add('JobTypeID', [int]); $null = $cart.Columns.Add('Quantity', [int])
    $null = $cart.Rows.Add(3, 1000)
    Reject 'Insufficient inventory rejects entire booking' { $null = $jobService.CreateJobWithDetailsAndMaterials($customerID, [DateTime]::Today, 'CODEX-QA rollback test', $cart) } 'stock|inventory'
    $after = Read 'SELECT (SELECT COUNT(*) FROM Jobs) AS Jobs, (SELECT COUNT(*) FROM JobDetails) AS Details, (SELECT COUNT(*) FROM InventoryTransactions) AS Transactions, (SELECT SUM(CAST(QuantityInStock AS bigint)) FROM InventoryItems) AS Stock'
    foreach ($column in @('Jobs','Details','Transactions','Stock')) {
        Check "Failed booking preserves $column" ($before.Rows[0][$column] -eq $after.Rows[0][$column])
    }
    $totals = Read "SELECT COUNT(*) AS Mismatches FROM Jobs WHERE Notes LIKE '%CODEX-QA-20261116%' AND (GrandTotal <> TotalMaterialCost+LabourCost+TransportCost+TaxAmount OR ABS(TaxAmount-ROUND((TotalMaterialCost+LabourCost+TransportCost)*0.15,2))>0.01)"
    Check 'QA job totals and tax agree with saved costs' ($totals.Rows[0].Mismatches -eq 0)
    $stock = Read 'SELECT COUNT(*) AS NegativeStock FROM InventoryItems WHERE QuantityInStock < 0'
    Check 'Inventory remains nonnegative' ($stock.Rows[0].NegativeStock -eq 0)
} finally {
    $connection.Dispose()
    $results | ConvertTo-Json | Set-Content "$PSScriptRoot/database-regression-results.json" -Encoding utf8
}
if (@($results | Where-Object { !$_.passed }).Count) { exit 1 }
