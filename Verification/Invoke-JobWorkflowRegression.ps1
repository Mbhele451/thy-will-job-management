param([string]$BuildDirectory=(Join-Path (Split-Path $PSScriptRoot -Parent) 'bin/Debug'), [ValidatePattern('^[A-Za-z0-9_]+$')][string]$LocalDBInstance='MSSQLLocalDB')
$ErrorActionPreference='Stop'
# Runs against a disposable LocalDB database. No production connection string or credentials are read.
$db='ThyWillWorkflowTest_'+[Guid]::NewGuid().ToString('N')
$master="Data Source=(localdb)\$LocalDBInstance;Initial Catalog=master;Integrated Security=True;Connect Timeout=15"
function Sql($cs,$sql){$c=New-Object System.Data.SqlClient.SqlConnection $cs;try{$c.Open();$q=$c.CreateCommand();$q.CommandText=$sql;$q.CommandTimeout=60;$d=New-Object System.Data.DataTable;$a=New-Object System.Data.SqlClient.SqlDataAdapter $q;[void]$a.Fill($d);return ,$d}finally{$c.Dispose()}}
$script:checks=0
function Check($name,$condition){if(!$condition){throw "FAILED: $name"};$script:checks++;Write-Output "PASS: $name"}
function Reject($name,[scriptblock]$action){$failed=$false;try{& $action | Out-Null}catch{$failed=$true};Check $name $failed}
function Used($amount){$d=New-Object System.Data.DataTable;[void]$d.Columns.Add('ItemID',[int]);[void]$d.Columns.Add('UsedQuantity',[int]);[void]$d.Rows.Add(1,$amount);return ,$d}
$cs=$master.Replace('Initial Catalog=master','Initial Catalog='+$db)
try {
 [void](Sql $master "CREATE DATABASE [$db]")
 [void](Sql $cs @'
 CREATE TABLE Roles(RoleID int PRIMARY KEY,RoleName varchar(30));
 CREATE TABLE Staff(StaffID int PRIMARY KEY,RoleID int REFERENCES Roles(RoleID),FullName nvarchar(100),EmploymentStatus varchar(20),IsAvailable bit);
 CREATE TABLE Customers(CustomerID int PRIMARY KEY,AccountStatus varchar(20),FullName nvarchar(100),EmailAddress nvarchar(320));
 CREATE TABLE AppAccountSecurity(SecurityID int IDENTITY PRIMARY KEY,StaffID int NULL,CustomerID int NULL,IsBlocked bit);
 CREATE TABLE Jobs(JobID int PRIMARY KEY,StaffID int NULL REFERENCES Staff(StaffID),CustomerID int REFERENCES Customers(CustomerID),PromiseDate datetime,JobStatus varchar(20),GrandTotal decimal(18,2),UpdatedAt datetime);
 CREATE TABLE InventoryItems(ItemID int PRIMARY KEY,ItemName nvarchar(200),QuantityInStock int,UpdatedAt datetime);
 CREATE TABLE Payments(JobID int NOT NULL, PaymentStatus varchar(20) NOT NULL);
 CREATE TABLE JobMaterials(JobMaterialID int IDENTITY PRIMARY KEY,JobID int REFERENCES Jobs(JobID),ItemID int REFERENCES InventoryItems(ItemID),QuantityUsed int, TotalCost decimal(18,2) NOT NULL DEFAULT 0);
 CREATE TABLE InventoryTransactions(TransactionID int IDENTITY PRIMARY KEY,ItemID int,JobID int,TransactionType varchar(10),Quantity int,TransactionDate datetime);
 CREATE TABLE AppSecurityAudit(AuditID bigint IDENTITY PRIMARY KEY,ActorStaffID int NULL,TargetStaffID int NULL,TargetCustomerID int NULL,ActionName varchar(50) NOT NULL,Detail nvarchar(500) NOT NULL,IPAddress varchar(45) NOT NULL,OccurredAtUtc datetime2);
 INSERT Roles VALUES(1,'Manager'),(2,'Technician'),(3,'Administrator');
 INSERT Staff VALUES(1,1,'Manager','Active',0),(2,2,'Assigned technician','Active',1),(3,2,'Replacement','Active',1),(4,2,'Blocked','Active',0),(5,2,'Inactive','Inactive',0),(6,2,'Busy','Active',1),(7,3,'Administrator','Active',0);
 INSERT Customers VALUES(1,'Active','Customer One','first@example.invalid'),(2,'Active','Customer Two','second@example.invalid');
 INSERT AppAccountSecurity(StaffID,IsBlocked) VALUES(4,1);
 INSERT InventoryItems VALUES(1,'Allocated material',10,GETDATE());
'@)
 [void](Sql $cs (Get-Content (Join-Path (Split-Path $PSScriptRoot -Parent) 'MajorProject.Web/Database/04_JobChangeWorkflow.sql') -Raw))
 # Migration must also be safe to rerun.
 [void](Sql $cs (Get-Content (Join-Path (Split-Path $PSScriptRoot -Parent) 'MajorProject.Web/Database/04_JobChangeWorkflow.sql') -Raw))
 $cancellationMigration = Get-Content (Join-Path (Split-Path $PSScriptRoot -Parent) 'MajorProject.Web/Database/05_CustomerCancellation.sql') -Raw
 [void](Sql $cs $cancellationMigration)
 [void](Sql $cs $cancellationMigration)
 [void](Sql $cs @'
 INSERT Jobs VALUES(101,2,1,GETDATE(),'In Progress',1000,GETDATE()),(102,2,1,DATEADD(day,1,GETDATE()),'Pending',333.30,GETDATE()),(103,4,1,GETDATE(),'In Progress',500,GETDATE()),(104,5,1,DATEADD(day,2,GETDATE()),'Pending',500,GETDATE()),(105,6,1,GETDATE(),'Pending',500,GETDATE()),(106,2,1,GETDATE(),'In Progress',500,GETDATE()),(107,2,1,GETDATE(),'In Progress',500,GETDATE());
 ALTER TABLE Jobs ADD TransportCost decimal(18,2) NOT NULL DEFAULT 333.30;
 INSERT JobMaterials(JobID,ItemID,QuantityUsed,TotalCost) VALUES(101,1,5,50),(106,1,5,50),(107,1,5,50);
'@)
 [void][Reflection.Assembly]::LoadFrom((Join-Path $BuildDirectory 'MajorProject.Data.dll'))
 [void][Reflection.Assembly]::LoadFrom((Join-Path $BuildDirectory 'MajorProject.Business.dll'))
 $service=New-Object MajorProject.Business.Services.JobChangeService $cs
 $jobs=New-Object MajorProject.Data.Data.JobDataAccess $cs
 Check '5% currency rounding' ([MajorProject.Business.Services.JobChangeService]::CancellationFee(333.30) -eq 16.67)
 Reject 'Negative original quote rejected' { [MajorProject.Business.Services.JobChangeService]::CancellationFee(-1) }
 Reject 'Technician cannot directly cancel' { $late=$false;$jobs.CancelTechnicianJob(101,2,[ref]$late) }
 Reject 'Technician cannot request cancellation before starting' { $service.Request(102,2,'Cancellation','Cannot proceed with work',$null,(Used 0),$false) }
 Reject 'Technician cannot request reschedule after starting' { $service.Request(101,2,'Reschedule','Cannot proceed with work',[DateTime]::Today,(Used 0),$false) }
 Reject 'Another technician cannot request change' { $service.Request(101,3,'Cancellation','Cannot proceed with work',$null,(Used 0),$false) }
 Reject 'Blocked technician cannot request change' { $service.Request(103,4,'Cancellation','Cannot proceed with work',$null,(Used 0),$false) }
 $id=$service.RequestStaffCancellation(101,7,'Client requested work to stop')
 Reject 'Manager cannot approve before material submission' { $service.Decide($id,1,$true,'Approve before report',0,$null) }
 Reject 'Duplicate pending request rejected' { $service.RequestCustomerCancellation(101,1,'Client requested work to stop') }
 Check 'Assigned technician sees material report' ($service.TechnicianMaterialReports(2).Select("RequestID=$id").Count -eq 1)
 Reject 'Another technician cannot submit material report' { $service.SubmitMaterials($id,3,(Used 2)) }
 Reject 'Excess material consumption rejected' { $service.SubmitMaterials($id,2,(Used 6)) }
 $service.SubmitMaterials($id,2,(Used 2))
 Check 'Request preserves job status' ((Sql $cs 'SELECT JobStatus FROM Jobs WHERE JobID=101').Rows[0][0] -eq 'In Progress')
 Check 'Pending request prevents completion' ($jobs.CompleteTechnicianJob(101,2) -eq 0)
 Reject 'Technician cannot approve request' { $service.Decide($id,2,$true,'Approved',0,$null) }
 Reject 'Administrator cannot approve request' { $service.Decide($id,7,$true,'Approved',0,$null) }
 $service.Decide($id,1,$true,'Reviewed materials and approved',0,$null)
 Check 'Manager approval cancels job' ((Sql $cs 'SELECT JobStatus FROM Jobs WHERE JobID=101').Rows[0][0] -eq 'Cancelled')
 Check 'Only unused materials return' ((Sql $cs 'SELECT QuantityInStock FROM InventoryItems WHERE ItemID=1').Rows[0][0] -eq 13)
 Check 'Stock return recorded once' ((Sql $cs 'SELECT SUM(Quantity) FROM InventoryTransactions WHERE JobID=101').Rows[0][0] -eq 3)
 Check 'Separate cancellation charge is 5% of saved quote' ((Sql $cs 'SELECT CancellationFee FROM JobChangeRequests WHERE JobID=101').Rows[0][0] -eq 50 -and (Sql $cs 'SELECT GrandTotal FROM Jobs WHERE JobID=101').Rows[0][0] -eq 1000)
 Reject 'Repeated approval cannot return stock twice' { $service.Decide($id,1,$true,'Repeat',0,$null) }
 Check 'Customer sees charge and consumed material value' ($service.CustomerFees(1).Select("RequestID=$id")[0].Fee -eq 70)
 Reject 'Another customer cannot settle fee' { $service.PayDemoFee(2,$id) }
 $service.PayDemoFee(1,$id)
 Reject 'Duplicate demo payment rejected' { $service.PayDemoFee(1,$id) }
 Check 'Demo settlement recorded' ($service.CustomerFees(1).Rows[0].FeePaymentStatus -eq 'DemoPaid')
 $reschedule=$service.RequestCustomerReschedule(102,1,[DateTime]::Today.AddDays(3),'Client needs a different day')
 Reject 'Cannot assign blocked technician' { $service.Decide($reschedule,1,$true,'Move schedule',4,[DateTime]::Today) }
 Reject 'Cannot assign inactive technician' { $service.Decide($reschedule,1,$true,'Move schedule',5,[DateTime]::Today) }
 Reject 'Cannot double book technician' { $service.Decide($reschedule,1,$true,'Move schedule',6,[DateTime]::Today) }
 $service.Decide($reschedule,1,$true,'Move schedule',3,[DateTime]::Today.AddDays(3))
 Check 'Approved reschedule changes technician and keeps pending status' ((Sql $cs 'SELECT StaffID FROM Jobs WHERE JobID=102').Rows[0][0] -eq 3 -and (Sql $cs 'SELECT JobStatus FROM Jobs WHERE JobID=102').Rows[0][0] -eq 'Pending')
 Check 'New technician receives allocation notification' ($service.Notifications(3).Rows.Count -eq 1)
 Check 'Blocked and inactive open jobs appear in handover queue' ($service.ReassignmentQueue(1).Rows.Count -eq 2)
 $service.Reassign(103,1,3,[DateTime]::Today,'Work already started. Check isolation and existing materials at handover.')
 Check 'Handover preserves in-progress status' ((Sql $cs 'SELECT JobStatus FROM Jobs WHERE JobID=103').Rows[0][0] -eq 'In Progress')
 Check 'Handover removes job from blocked staff queue' ($service.ReassignmentQueue(1).Rows.Count -eq 1)
 Check 'Replacement receives the handover instructions' ($service.Notifications(3).Select("JobID=103")[0].HandoverNote -like 'Work already started*')
 Check 'Handover history records old technician'  ((Sql $cs 'SELECT PreviousStaffID FROM JobAssignmentHistory WHERE JobID=103').Rows[0][0] -eq 4)
 $notice=$service.Notifications(3).Rows[0].NotificationID;$service.ReadNotification(2,$notice)
 Check 'Notification ownership protected' ($service.Notifications(3).Rows.Count -eq 2)
 $service.ReadNotification(3,$notice)
 Check 'Read acknowledgement persists' ($service.Notifications(3).Rows.Count -eq 1)
 $rejectId=$service.RequestCustomerCancellation(106,1,'Site access was temporarily refused')
 $service.Decide($rejectId,1,$false,'Client has restored access',0,$null)
 Check 'Rejected request leaves stock and job unchanged' ((Sql $cs 'SELECT QuantityInStock FROM InventoryItems WHERE ItemID=1').Rows[0][0] -eq 13 -and (Sql $cs 'SELECT JobStatus FROM Jobs WHERE JobID=106').Rows[0][0] -eq 'In Progress')
 $stale=$service.RequestCustomerCancellation(107,1,'Site access was refused again')
 $service.SubmitMaterials($stale,2,(Used 1))
 [void](Sql $cs 'UPDATE JobMaterials SET QuantityUsed=6 WHERE JobID=107')
 Reject 'Changed materials require fresh manager review' { $service.Decide($stale,1,$true,'Approved',0,$null) }
 Check 'Failed approval rolls back decision' ((Sql $cs "SELECT RequestStatus FROM JobChangeRequests WHERE RequestID=$stale").Rows[0][0] -eq 'Pending')
 $inventory=New-Object MajorProject.Business.Services.InventoryService $cs
 $inventory.AdjustInventoryQuantity(1,20000)
 Check 'Restock accepts 20,000 as an increment' ((Sql $cs 'SELECT QuantityInStock FROM InventoryItems WHERE ItemID=1').Rows[0][0] -eq 20013)
 Reject 'Restock rejects 20,001' { $inventory.AdjustInventoryQuantity(1,20001) }
 Reject 'Zero adjustment rejected' { $inventory.AdjustInventoryQuantity(1,0) }
 Reject 'Insufficient stock rejected' { $inventory.AdjustInventoryQuantity(1,-20014) }
 Check 'Failed adjustments leave balance unchanged' ((Sql $cs 'SELECT QuantityInStock FROM InventoryItems WHERE ItemID=1').Rows[0][0] -eq 20013)
 [void](Sql $cs "DECLARE @today date=DATEADD(hour,2,SYSUTCDATETIME());
 INSERT Jobs(JobID,StaffID,CustomerID,PromiseDate,JobStatus,GrandTotal,UpdatedAt) VALUES
 (110,2,1,DATEADD(day,1,@today),'Pending',8000,GETDATE()),
 (111,2,1,DATEADD(day,2,@today),'Pending',7000,GETDATE()),(112,2,1,@today,'Pending',8000,GETDATE());")
 $date=[DateTime]::Today.AddDays(15)
 Reject 'Customer cannot request another customer booking' { $service.RequestCustomerReschedule(110,2,$date,'Please change the scheduled visit') }
 $customerRequest=$service.RequestCustomerReschedule(110,1,$date,'Please change the scheduled visit')
 Reject 'Duplicate customer request rejected' { $service.RequestCustomerReschedule(110,1,$date,'Please change the scheduled visit') }
 Check 'Customer request visible to administrator and manager' ($service.Requests(7,$true).Select("RequestID=$customerRequest").Count -eq 1 -and $service.Requests(1,$true).Select("RequestID=$customerRequest").Count -eq 1)
 Check 'Another customer cannot see request' ($service.CustomerRequests(2).Rows.Count -eq 0)
 Check 'Next-day request fee based on transport only' ($service.CustomerRequests(1).Rows[0].FeeIfApproved -eq 16.67)
 $service.Decide($customerRequest,7,$true,'Approved new booking date',3,$date)
 Check 'Approved fee payable once' ($service.CustomerFees(1).Select("RequestID=$customerRequest")[0].Fee -eq 16.67)
 Reject 'Approval cannot be replayed' { $service.Decide($customerRequest,7,$true,'Approved new booking date',3,$date) }
 Check 'Original and revised booking emails target the client' ((Sql $cs "SELECT COUNT(*) FROM BookingEmailOutbox WHERE JobID=110 AND Recipient='first@example.invalid'").Rows[0][0] -eq 2)
 $outbox=New-Object MajorProject.Data.Data.JobChangeDataAccess $cs
 $lease=[guid]::NewGuid(); $mail=$outbox.ClaimBookingEmail($lease).Rows[0]
 Check 'Concurrent email senders claim different records' ($outbox.ClaimBookingEmail([guid]::NewGuid()).Rows[0].EmailID -ne $mail.EmailID)
 $outbox.MarkBookingEmailSent($mail.EmailID,$lease)
 Check 'Email delivery acknowledgement persisted' ((Sql $cs "SELECT COUNT(*) FROM BookingEmailOutbox WHERE EmailID=$($mail.EmailID) AND SentAtUtc IS NOT NULL").Rows[0][0] -eq 1)
 $early=$service.RequestCustomerReschedule(111,1,$date,'Please change the scheduled visit')
 Check 'Two days notice has no fee' ($service.CustomerRequests(1).Rows[0].FeeIfApproved -eq 0)
 $same=$service.RequestCustomerReschedule(112,1,$date,'Please change the scheduled visit')
 Check 'Same-day request quotes late fee' ($service.CustomerRequests(1).Rows[0].FeeIfApproved -eq 16.67)
 $service.Decide($same,1,$false,'Declined due to availability',0,$null)
 Check 'Rejection leaves no payable fee or revised email' ($service.CustomerFees(1).Select("RequestID=$same").Count -eq 0 -and (Sql $cs 'SELECT COUNT(*) FROM BookingEmailOutbox WHERE JobID=112 AND IsNewBooking=0').Rows[0][0] -eq 0)
 Reject 'Stale handover selection rejected' { $service.Reassign(103,1,6,$date,'New technician required',4) }
 . (Join-Path $PSScriptRoot "Test-CancellationCases.ps1")
 Write-Output "PASS: $script:checks workflow checks against disposable LocalDB."
} catch {
 Write-Output $_.Exception.GetBaseException().Message
 throw
} finally {
 [System.Data.SqlClient.SqlConnection]::ClearAllPools()
 [void](Sql $master "IF DB_ID('$db') IS NOT NULL BEGIN ALTER DATABASE [$db] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$db]; END")
}
