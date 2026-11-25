# Invoked by Invoke-JobWorkflowRegression.ps1 inside its isolated LocalDB database.
[void](Sql $cs @'
DECLARE @today date=DATEADD(hour,2,SYSUTCDATETIME());
INSERT Jobs (JobID,StaffID,CustomerID,PromiseDate,JobStatus,GrandTotal,TransportCost,UpdatedAt)
VALUES (901,2,1,DATEADD(day,2,@today),'Pending',1000,100,GETDATE()),
       (902,2,1,DATEADD(day,1,@today),'Pending',333.30,100,GETDATE()),
       (903,2,1,@today,'Pending',1000,100,GETDATE()),
       (904,2,1,DATEADD(day,-4,@today),'In Progress',1000,100,GETDATE()),
       (905,2,1,@today,'Completed',1000,100,GETDATE()),
       (906,2,1,@today,'In Progress',1000,100,GETDATE()),
       (907,2,1,@today,'In Progress',1000,100,GETDATE());
INSERT JobMaterials(JobID,ItemID,QuantityUsed,TotalCost) VALUES(904,1,5,126.75),(906,1,2,50);
'@)
$reason = 'Customer requests cancellation.'
Reject 'Customer cannot cancel another account booking' { $service.RequestCustomerCancellation(901,2,$reason) }
Reject 'Technician cannot record an in-person cancellation' { $service.RequestStaffCancellation(901,2,$reason) }
Reject 'Completed job cannot be cancelled' { $service.RequestCustomerCancellation(905,1,$reason) }
Reject 'Technician cannot read approval conversations' { $service.Requests(2,$false) }
$early = $service.RequestCustomerCancellation(901,1,$reason)
$service.Decide($early,1,$true,'Approved early cancellation',0,$null)
Check 'Early cancellation has no payable invoice' ($service.CustomerFees(1).Select("RequestID=$early").Length -eq 0)
Reject 'A zero fee cannot be paid' { $service.PayDemoFee(1,$early) }
$previous = $service.RequestStaffCancellation(902,7,$reason)
$service.Decide($previous,1,$true,'Approved walk-in cancellation',0,$null)
Check 'Prior-day cancellation uses original total and rounds half up' ($service.CustomerFees(1).Select("RequestID=$previous")[0].Fee -eq 16.67)
$same = $service.RequestCustomerCancellation(903,1,$reason)
$service.Decide($same,1,$true,'Approved same-day cancellation',0,$null)
Check 'Same-day cancellation uses the late cancellation fee' ($service.CustomerFees(1).Select("RequestID=$same")[0].Fee -eq 50)
$mid = $service.RequestCustomerCancellation(904,1,$reason)
Check 'Assigned technician sees material task' ($service.TechnicianMaterialReports(2).Select("RequestID=$mid").Length -eq 1)
Check 'Other technician cannot see material task' ($service.TechnicianMaterialReports(3).Select("RequestID=$mid").Length -eq 0)
Reject 'Cannot decide before technician records actual usage' { $service.Decide($mid,1,$true,'Approve before report',0,$null) }
Reject 'Office staff cannot impersonate reporting technician' { $service.SubmitMaterials($mid,1,(Used 2)) }
Reject 'Cannot over-report consumed stock' { $service.SubmitMaterials($mid,2,(Used 6)) }
$service.SubmitMaterials($mid,2,(Used 2))
Reject 'Report is accepted once' { $service.SubmitMaterials($mid,2,(Used 0)) }
$service.Decide($mid,1,$true,'Checked site materials',0,$null)
$invoice = $service.CustomerFees(1).Select("RequestID=$mid")[0]
Check 'Mid-job charge uses historical allocated unit price' ($invoice.MaterialCharge -eq 50.70 -and $invoice.Fee -eq 100.70)
Check 'Stock ledger reverses exactly three unopened units' ((Sql $cs 'SELECT SUM(Quantity) FROM InventoryTransactions WHERE JobID=904').Rows[0][0] -eq 3)
$bytes = $service.GetFeeInvoicePdf(1,$mid)
Check 'Customer receives a real fee PDF' ([Text.Encoding]::ASCII.GetString($bytes,0,5) -eq '%PDF-')
Reject 'Customer cannot download another account fee invoice' { $service.GetFeeInvoicePdf(2,$mid) }
Check 'One email queued for the correct recipient' ((Sql $cs "SELECT COUNT(*) FROM JobFeeInvoiceOutbox WHERE RequestID=$mid AND Recipient='first@example.invalid'").Rows[0][0] -eq 1)
Check 'Technician receives final cancellation notice' ($service.Notifications(2).Select("JobID=904 AND Message LIKE 'Cancellation approved%'").Length -eq 1)
$rejected = $service.RequestCustomerCancellation(906,1,$reason)
$service.Decide($rejected,1,$false,'Customer wants to continue',0,$null)
Check 'Rejected request retains job, stock and no fee invoice' ((Sql $cs "SELECT COUNT(*) FROM Jobs WHERE JobID=906 AND JobStatus='In Progress' AND NOT EXISTS (SELECT 1 FROM InventoryTransactions WHERE JobID=906) AND NOT EXISTS (SELECT 1 FROM JobFeeInvoiceOutbox WHERE RequestID=$rejected)").Rows[0][0] -eq 1)
$none = $service.RequestCustomerCancellation(907,1,$reason)
$service.SubmitMaterials($none,2,[System.Data.DataTable]::new())
$service.Decide($none,1,$true,'No materials used',0,$null)
Check 'In-progress job with no materials still requires confirmation and bills only the fee' ($service.CustomerFees(1).Select("RequestID=$none")[0].Fee -eq 50)

# Run both calls on different CLR threads with separate SQL connections.
Add-Type -ReferencedAssemblies @('System.Data.dll', (Join-Path $BuildDirectory 'MajorProject.Business.dll')) -TypeDefinition @'
using System;
using System.Threading;
using System.Threading.Tasks;
using MajorProject.Business.Services;
public static class CancellationRace
{
    private static int Race(Action action)
    {
        var gate = new Barrier(2);
        int successes = 0;
        Action attempt = () => {
            gate.SignalAndWait();
            try { action(); Interlocked.Increment(ref successes); }
            catch (InvalidOperationException) { }
            catch (System.Data.SqlClient.SqlException e) { if(e.Number != 1205) throw; }
        };
        Task.WaitAll(Task.Factory.StartNew(attempt),Task.Factory.StartNew(attempt));
        gate.Dispose();
        return successes;
    }
    public static int Request(string cs) {
        return Race(() => new JobChangeService(cs).RequestCustomerCancellation(908,1,"Customer requests cancellation."));
    }
    public static int Approve(string cs, int request) {
        return Race(() => new JobChangeService(cs).Decide(request,1,true,"Reviewed cancellation",0,null));
    }
    public static int Pay(string cs, int request) {
        return Race(() => new JobChangeService(cs).PayDemoFee(1,request));
    }
}
'@
[void](Sql $cs "INSERT Jobs(JobID,StaffID,CustomerID,PromiseDate,JobStatus,GrandTotal,TransportCost,UpdatedAt) VALUES(908,2,1,DATEADD(hour,2,SYSUTCDATETIME()),'Pending',1000,100,GETDATE()); INSERT JobMaterials(JobID,ItemID,QuantityUsed,TotalCost) VALUES(908,1,4,40);")
Check 'Two simultaneous customer cancellations create one request' ([CancellationRace]::Request($cs) -eq 1)
$raceRequest = (Sql $cs 'SELECT RequestID FROM JobChangeRequests WHERE JobID=908').Rows[0][0]
Check 'Two simultaneous decisions approve once' ([CancellationRace]::Approve($cs,$raceRequest) -eq 1)
Check 'Concurrent approval writes one stock return and one invoice email' ((Sql $cs "SELECT COUNT(*) FROM InventoryTransactions WHERE JobID=908 AND Quantity=4").Rows[0][0] -eq 1 -and (Sql $cs "SELECT COUNT(*) FROM JobFeeInvoiceOutbox WHERE RequestID=$raceRequest").Rows[0][0] -eq 1)
Check 'Two simultaneous fee payments settle once' ([CancellationRace]::Pay($cs,$raceRequest) -eq 1)
[void](Sql $cs "UPDATE JobFeeInvoiceOutbox SET AvailableAtUtc=DATEADD(day,1,SYSUTCDATETIME()) WHERE RequestID<>$raceRequest")
$claim = [guid]::NewGuid()
$feeMail = $outbox.ClaimFeeEmail($claim)
Check 'Fee email worker claims correct invoice' ($feeMail.Rows.Count -eq 1 -and $feeMail.Rows[0].RequestID -eq $raceRequest)
Check 'Second worker cannot send a leased invoice' ($outbox.ClaimFeeEmail([guid]::NewGuid()).Rows.Count -eq 0)
Check 'Worker renders the approved amount' ($outbox.FeeEmailDetails($raceRequest,$claim).Rows[0].Fee -eq 50)
$outbox.MarkFeeEmailSent($raceRequest,[guid]::NewGuid())
Check 'Unrelated worker cannot mark invoice delivered' ($outbox.FeeEmailDetails($raceRequest,$claim).Rows.Count -eq 1)
$outbox.MarkFeeEmailSent($raceRequest,$claim)
Check 'Acknowledged fee invoice is not claimed again' ($outbox.ClaimFeeEmail([guid]::NewGuid()).Rows.Count -eq 0)
[void](Sql $cs "INSERT Jobs(JobID,StaffID,CustomerID,PromiseDate,JobStatus,GrandTotal,TransportCost,UpdatedAt) VALUES(909,2,1,GETDATE(),'Pending',500,100,GETDATE()); INSERT Payments(JobID,PaymentStatus) VALUES(909,'Paid')")
$paidOriginal=$service.RequestStaffCancellation(909,7,$reason)
Reject 'Previously settled service invoice requires reconciliation before new charges' { $service.Decide($paidOriginal,1,$true,'Attempt approval',0,$null) }
Check 'Previously settled invoice is retained without another charge or stock return' ((Sql $cs "SELECT COUNT(*) FROM Payments WHERE JobID=909 AND PaymentStatus='Paid' AND NOT EXISTS(SELECT 1 FROM JobFeeInvoiceOutbox WHERE RequestID=$paidOriginal) AND NOT EXISTS(SELECT 1 FROM InventoryTransactions WHERE JobID=909)").Rows[0][0] -eq 1)

# Permissive fixture procedures make the shared application's eligibility check observable.
[void](Sql $cs "CREATE PROCEDURE dbo.usp_ProcessSimulatedInvoicePayment @JobID int,@CustomerID int,@PaymentMethod varchar(30) AS INSERT Payments VALUES(@JobID,'Paid'); SELECT 1 AS IsSuccessful;")
[void](Sql $cs "CREATE PROCEDURE dbo.usp_ProcessStaffCashPayment @JobID int,@AmountReceived decimal(10,2),@StaffID int AS INSERT Payments VALUES(@JobID,'Paid'); SELECT 1 AS IsSuccessful;")
[void](Sql $cs "CREATE PROCEDURE dbo.usp_GetCustomerInvoicePaymentReceipt @JobID int,@CustomerID int AS SELECT @JobID AS JobID;")
$payments = New-Object MajorProject.Data.Data.PaymentDataAccess $cs
foreach ($job in @(909,906,904)) {
    Reject "Customer cannot pay the original service invoice for open or cancelled job $job" { $payments.ProcessSimulatedInvoicePayment($job,1,'EFT') }
    Reject "Office cannot collect the original service invoice for open or cancelled job $job" { $payments.ProcessStaffCashPayment($job,1000,1) }
}
Reject 'Other customer cannot settle a completed service invoice' { $payments.ProcessSimulatedInvoicePayment(905,2,'EFT') }
Check 'Completed service invoice can be settled by its owner' ($payments.ProcessSimulatedInvoicePayment(905,1,'EFT').Rows.Count -eq 1)
Reject 'Cash cannot settle the same service invoice a second time' { $payments.ProcessStaffCashPayment(905,1000,1) }
Check 'Settled completed service has its receipt' ($payments.GetCustomerInvoicePaymentReceipt(905,1).Rows.Count -eq 1)
Check 'Cancelling a job does not produce a service payment or receipt' ($payments.GetCustomerInvoicePaymentReceipt(904,1).Rows.Count -eq 0)
[void](Sql $cs "UPDATE Jobs SET JobStatus='Archived' WHERE JobID=905")
Check 'An archived completed service keeps its payment receipt' ($payments.GetCustomerInvoicePaymentReceipt(905,1).Rows.Count -eq 1)
