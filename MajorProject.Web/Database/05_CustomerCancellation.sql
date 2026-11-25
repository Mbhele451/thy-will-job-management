-- Run after 04_JobChangeWorkflow.sql, before deploying either updated client.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF COL_LENGTH('dbo.JobChangeRequests','AwaitingMaterials') IS NULL
BEGIN
    ALTER TABLE dbo.JobChangeRequests ADD
        AwaitingMaterials bit NOT NULL CONSTRAINT DF_JobChange_AwaitingMaterials DEFAULT 0,
        MaterialCharge decimal(18,2) NOT NULL CONSTRAINT DF_JobChange_MaterialCharge DEFAULT 0 CHECK(MaterialCharge>=0),
        FeeReason nvarchar(200) NULL;
    ALTER TABLE dbo.JobChangeRequests DROP CONSTRAINT CK_JobChange_Requester;
    ALTER TABLE dbo.JobChangeRequests ADD CONSTRAINT CK_JobChange_Requester CHECK
        ((RequestedBy IS NULL AND RequestedByCustomerID IS NOT NULL) OR
         (RequestedBy IS NOT NULL AND RequestedByCustomerID IS NULL));
END;
IF OBJECT_ID('dbo.JobFeeInvoiceOutbox') IS NULL
BEGIN
    CREATE TABLE dbo.JobFeeInvoiceOutbox (
        RequestID int PRIMARY KEY REFERENCES dbo.JobChangeRequests(RequestID),
        Recipient nvarchar(320) NOT NULL,
        AvailableAtUtc datetime2 NOT NULL DEFAULT SYSUTCDATETIME(),
        LeaseID uniqueidentifier NULL,
        SentAtUtc datetime2 NULL
    );
END;
COMMIT;
