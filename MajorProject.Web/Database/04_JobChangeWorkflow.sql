-- Deploy before the updated clients. Existing jobs and prices are preserved.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID('dbo.JobChangeRequests') IS NULL
BEGIN
 CREATE TABLE dbo.JobChangeRequests(
  RequestID int IDENTITY PRIMARY KEY, JobID int NOT NULL REFERENCES dbo.Jobs(JobID),
  RequestedBy int NOT NULL REFERENCES dbo.Staff(StaffID),
  RequestType varchar(20) NOT NULL CHECK(RequestType IN('Cancellation','Reschedule')),
  Reason nvarchar(1000) NOT NULL, RequestedDate date NULL,
  RequestStatus varchar(20) NOT NULL DEFAULT 'Pending' CHECK(RequestStatus IN('Pending','Approved','Rejected','Superseded')),
  OriginalTotal decimal(18,2) NOT NULL CHECK(OriginalTotal>=0),
  CancellationFee decimal(18,2) NOT NULL CHECK(CancellationFee>=0),
  FeePaymentStatus varchar(10) NOT NULL DEFAULT 'Unpaid' CHECK(FeePaymentStatus IN('Unpaid','DemoPaid')),
  FeePaidAtUtc datetime2 NULL,
  CreatedAtUtc datetime2 NOT NULL DEFAULT SYSUTCDATETIME(),
  DecidedBy int NULL REFERENCES dbo.Staff(StaffID), DecisionNote nvarchar(1000) NULL, DecidedAtUtc datetime2 NULL);
 CREATE UNIQUE INDEX UX_JobChange_Pending ON dbo.JobChangeRequests(JobID) WHERE RequestStatus='Pending';
 CREATE TABLE dbo.JobChangeMaterials(
  RequestID int NOT NULL REFERENCES dbo.JobChangeRequests(RequestID), ItemID int NOT NULL REFERENCES dbo.InventoryItems(ItemID),
  AllocatedQuantity int NOT NULL CHECK(AllocatedQuantity>=0), UsedQuantity int NOT NULL,
  CHECK(UsedQuantity>=0 AND UsedQuantity<=AllocatedQuantity), PRIMARY KEY(RequestID,ItemID));
END;
-- Upgrade existing staff requests in place.
IF COL_LENGTH('dbo.JobChangeRequests','RequestedByCustomerID') IS NULL
BEGIN
 ALTER TABLE dbo.JobChangeRequests ALTER COLUMN RequestedBy int NULL;
 ALTER TABLE dbo.JobChangeRequests ADD RequestedByCustomerID int NULL REFERENCES dbo.Customers(CustomerID),
     ReschedulingFee decimal(18,2) NOT NULL DEFAULT 0 CHECK (ReschedulingFee>=0);
 EXEC(N'ALTER TABLE dbo.JobChangeRequests ADD CONSTRAINT CK_JobChange_Requester CHECK
    ((RequestedBy IS NOT NULL AND RequestedByCustomerID IS NULL) OR
     (RequestedBy IS NULL AND RequestedByCustomerID IS NOT NULL AND RequestType=''Reschedule''))');
END;
IF OBJECT_ID('dbo.BookingEmailOutbox') IS NULL
BEGIN
 CREATE TABLE dbo.BookingEmailOutbox(
  EmailID bigint IDENTITY PRIMARY KEY, JobID int NOT NULL, PromiseDate date NOT NULL,
  Recipient nvarchar(320) NOT NULL, IsNewBooking bit NOT NULL,
  AvailableAtUtc datetime2 NOT NULL DEFAULT SYSUTCDATETIME(), LeaseID uniqueidentifier NULL,
  SentAtUtc datetime2 NULL);
 CREATE INDEX IX_BookingEmail_Ready ON dbo.BookingEmailOutbox(AvailableAtUtc,EmailID) WHERE SentAtUtc IS NULL;
END;
IF OBJECT_ID('dbo.JobAssignmentHistory') IS NULL
 CREATE TABLE dbo.JobAssignmentHistory(AssignmentID bigint IDENTITY PRIMARY KEY,
 JobID int NOT NULL REFERENCES dbo.Jobs(JobID),PreviousStaffID int NULL REFERENCES dbo.Staff(StaffID),
 NewStaffID int NOT NULL REFERENCES dbo.Staff(StaffID),ChangedBy int NOT NULL REFERENCES dbo.Staff(StaffID),
 PromiseDate date NOT NULL,HandoverNote nvarchar(1000) NOT NULL,ChangedAtUtc datetime2 NOT NULL DEFAULT SYSUTCDATETIME());
IF OBJECT_ID('dbo.JobAllocationNotifications') IS NULL
BEGIN
 CREATE TABLE dbo.JobAllocationNotifications(NotificationID bigint IDENTITY PRIMARY KEY,
  JobID int NOT NULL REFERENCES dbo.Jobs(JobID), StaffID int NOT NULL REFERENCES dbo.Staff(StaffID),
  Message nvarchar(300) NOT NULL, CreatedAtUtc datetime2 NOT NULL DEFAULT SYSUTCDATETIME(), ReadAtUtc datetime2 NULL);
 CREATE INDEX IX_Allocation_Unread ON dbo.JobAllocationNotifications(StaffID,ReadAtUtc);
END;
IF COL_LENGTH('dbo.JobAllocationNotifications','HandoverNote') IS NULL
 ALTER TABLE dbo.JobAllocationNotifications ADD HandoverNote nvarchar(1000) NULL;
EXEC(N'CREATE OR ALTER TRIGGER dbo.TR_Jobs_AllocationNotice ON dbo.Jobs AFTER INSERT,UPDATE AS
BEGIN
 SET NOCOUNT ON;
 INSERT dbo.BookingEmailOutbox(JobID,PromiseDate,Recipient,IsNewBooking)
 SELECT i.JobID,i.PromiseDate,c.EmailAddress,CASE WHEN d.JobID IS NULL THEN 1 ELSE 0 END
 FROM inserted i LEFT JOIN deleted d ON d.JobID=i.JobID JOIN dbo.Customers c ON c.CustomerID=i.CustomerID
 WHERE i.PromiseDate IS NOT NULL AND i.JobStatus IN(''Pending'',''In Progress'')
 AND NULLIF(LTRIM(RTRIM(c.EmailAddress)),'''') IS NOT NULL
 AND (d.JobID IS NULL OR d.PromiseDate IS NULL OR CONVERT(date,d.PromiseDate)<>CONVERT(date,i.PromiseDate));
 INSERT dbo.JobAllocationNotifications(JobID,StaffID,Message)
 SELECT i.JobID,i.StaffID,CONCAT(''Job #'',i.JobID,'' allocated for '',CONVERT(varchar(10),i.PromiseDate,23),''. Open My Jobs to review it.'')
 FROM inserted i LEFT JOIN deleted d ON d.JobID=i.JobID
 WHERE i.StaffID IS NOT NULL AND i.JobStatus IN(''Pending'',''In Progress'')
 AND (d.JobID IS NULL OR ISNULL(d.StaffID,0)<>i.StaffID OR ISNULL(d.PromiseDate,''19000101'')<>ISNULL(i.PromiseDate,''19000101''));
END');
COMMIT;
