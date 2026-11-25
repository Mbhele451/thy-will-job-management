param([int]$JobID=1020,[int]$TechnicianID=3)
$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
[xml]$cfg=Get-Content "$root/MajorProject.Web/Web.config"
$cs=($cfg.configuration.connectionStrings.add|Where-Object name -eq 'GroupWst34ConnectionString').connectionString
$null=[Reflection.Assembly]::LoadFrom("$root/MajorProject.Web/bin/MajorProject.Data.dll")
$connection=[System.Data.SqlClient.SqlConnection]::new($cs);$connection.Open()
$results=[System.Collections.Generic.List[object]]::new()
function Read($sql){$q=$connection.CreateCommand();$q.CommandText=$sql;$null=$q.Parameters.AddWithValue('@id',$JobID);$table=[System.Data.DataTable]::new();$a=[System.Data.SqlClient.SqlDataAdapter]::new($q);$null=$a.Fill($table);return ,$table}
function Check($name,$passed){$results.Add([pscustomobject]@{test=$name;passed=[bool]$passed});Write-Host "$name : $passed"}
try{
    $job=Read 'SELECT JobStatus FROM Jobs WHERE JobID=@id'
    Check 'Cancellation persisted' ($job.Rows.Count -eq 1 -and $job.Rows[0].JobStatus -eq 'Cancelled')
    $materials=Read 'SELECT ItemID,SUM(QuantityUsed) AS Used FROM JobMaterials WHERE JobID=@id GROUP BY ItemID'
    $restored=Read "SELECT ItemID,SUM(Quantity) AS Restored FROM InventoryTransactions WHERE JobID=@id AND TransactionType='IN' GROUP BY ItemID"
    $stock=Read 'SELECT ItemID,QuantityInStock FROM InventoryItems WHERE ItemID IN(SELECT ItemID FROM JobMaterials WHERE JobID=@id)'
    $before=Get-Content "$PSScriptRoot/cancellation-stock-before.json"|ConvertFrom-Json
    foreach($material in $materials.Rows){
        $record=@($restored.Select('ItemID='+$material.ItemID))
        $old=@($before|Where-Object ItemID -eq $material.ItemID)[0]
        $current=$stock.Select('ItemID='+$material.ItemID)[0]
        Check "Item $($material.ItemID) restored exactly in ledger" ($record.Count -eq 1 -and $record[0].Restored -eq $material.Used)
        Check "Item $($material.ItemID) stock matches original plus restoration" ($current.QuantityInStock -eq $old.QuantityInStock+$material.Used)
    }
    $data=[MajorProject.Data.Data.JobDataAccess]::new($cs)
    $late=$false;$rejected=$false
    try{$null=$data.CancelTechnicianJob($JobID,$TechnicianID,[ref]$late)}catch{$rejected=$_.Exception.ToString() -match 'Only Pending or In Progress'}
    Check 'Second cancellation rejected by lifecycle rule' $rejected
    $again=Read 'SELECT ItemID,QuantityInStock FROM InventoryItems WHERE ItemID IN(SELECT ItemID FROM JobMaterials WHERE JobID=@id)'
    $unchanged=$true
    foreach($item in $stock.Rows){if($again.Select('ItemID='+$item.ItemID)[0].QuantityInStock -ne $item.QuantityInStock){$unchanged=$false}}
    Check 'Second cancellation cannot restore inventory twice' $unchanged
    $replay=Read "SELECT COUNT(*) AS Jobs FROM Jobs WHERE Notes='CODEX-QA-20261116 MULTISERVICE REPLAY'"
    Check 'Duplicate booking POST created only one parent job' ($replay.Rows[0].Jobs -eq 1)
    $lines=Read "SELECT COUNT(*) AS Lines FROM JobDetails WHERE JobID IN(SELECT JobID FROM Jobs WHERE Notes='CODEX-QA-20261116 MULTISERVICE REPLAY')"
    Check 'Multi-service booking has two detail rows' ($lines.Rows[0].Lines -eq 2)
}finally{$connection.Dispose();$results|ConvertTo-Json|Set-Content "$PSScriptRoot/cancellation-regression-results.json" -Encoding utf8}
if(@($results|Where-Object {!$_.passed}).Count){exit 1}
