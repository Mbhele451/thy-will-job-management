/* Run on GroupWst34 before deploying all three updated projects. No passwords or account roles are changed here.
   DDL and seed settings run in one transaction. Invalid legacy rows stop the migration without data cleanup. */
SET NOCOUNT ON; SET XACT_ABORT ON;
IF DB_NAME() <> N'GroupWst34' THROW 51000,'Select GroupWst34 before running this migration.',1;
BEGIN TRY
 BEGIN TRANSACTION;
 IF COL_LENGTH('dbo.Customers','PassportNumber') IS NULL ALTER TABLE dbo.Customers ADD PassportNumber varchar(20) NULL;
 IF COL_LENGTH('dbo.Customers','PassportCountry') IS NULL ALTER TABLE dbo.Customers ADD PassportCountry varchar(50) NULL;
 -- Unicode names and issuing-country names preserve international characters. No indexed key is changed.
 IF EXISTS(SELECT 1 FROM sys.columns WHERE object_id=OBJECT_ID('dbo.Customers') AND name='FullName' AND (TYPE_NAME(user_type_id)<>'nvarchar' OR max_length<>200)) ALTER TABLE dbo.Customers ALTER COLUMN FullName nvarchar(100) NOT NULL;
 IF EXISTS(SELECT 1 FROM sys.columns WHERE object_id=OBJECT_ID('dbo.Staff') AND name='FullName' AND (TYPE_NAME(user_type_id)<>'nvarchar' OR max_length<>200)) ALTER TABLE dbo.Staff ALTER COLUMN FullName nvarchar(100) NULL;
 IF EXISTS(SELECT 1 FROM sys.columns WHERE object_id=OBJECT_ID('dbo.Customers') AND name='PassportCountry' AND (TYPE_NAME(user_type_id)<>'nvarchar' OR max_length<>100)) ALTER TABLE dbo.Customers ALTER COLUMN PassportCountry nvarchar(50) NULL;
 -- Dynamic SQL allows columns added in this batch to be compiled after ALTER TABLE.
 EXEC sys.sp_executesql N'
 IF EXISTS(SELECT 1 FROM dbo.Customers WHERE NOT (
  (CustomerType=''Residential'' AND CompanyRegistrationNumber IS NULL AND
   ((SouthAfricanIDNumber IS NOT NULL AND LEN(SouthAfricanIDNumber)=13 AND SouthAfricanIDNumber NOT LIKE ''%[^0-9]%'' AND PassportNumber IS NULL AND PassportCountry IS NULL)
    OR (SouthAfricanIDNumber IS NULL AND LEN(LTRIM(RTRIM(COALESCE(PassportNumber,'''')))) BETWEEN 1 AND 20 AND LEN(LTRIM(RTRIM(COALESCE(PassportCountry,'''')))) BETWEEN 1 AND 50)))
  OR (CustomerType=''Business'' AND CompanyRegistrationNumber IS NOT NULL AND SouthAfricanIDNumber IS NULL AND PassportNumber IS NULL AND PassportCountry IS NULL)))
 THROW 51001,''Identity preflight failed. Run 00_Preflight.sql and correct the flagged customers before migration.'',1;';
 -- Reject duplicates by issuing country and document, rather than incorrectly making passports globally unique.
 EXEC sys.sp_executesql N'IF EXISTS(SELECT 1 FROM dbo.Customers WHERE PassportNumber IS NOT NULL GROUP BY UPPER(LTRIM(RTRIM(PassportCountry))),UPPER(LTRIM(RTRIM(PassportNumber))) HAVING COUNT(*)>1) THROW 51002,''Duplicate passport/country pairs require review.'',1;';
 IF OBJECT_ID('dbo.CK_Customers_IdentityByCustomerType','C') IS NOT NULL ALTER TABLE dbo.Customers DROP CONSTRAINT CK_Customers_IdentityByCustomerType;
 IF OBJECT_ID('dbo.CK_Customers_ID_Validation','C') IS NOT NULL ALTER TABLE dbo.Customers DROP CONSTRAINT CK_Customers_ID_Validation;
 EXEC sys.sp_executesql N'ALTER TABLE dbo.Customers WITH CHECK ADD CONSTRAINT CK_Customers_ID_Validation CHECK (
 (CustomerType=''Residential'' AND CompanyRegistrationNumber IS NULL AND (
 (SouthAfricanIDNumber IS NOT NULL AND LEN(SouthAfricanIDNumber)=13 AND SouthAfricanIDNumber NOT LIKE ''%[^0-9]%'' AND PassportNumber IS NULL AND PassportCountry IS NULL)
 OR (SouthAfricanIDNumber IS NULL AND PassportNumber IS NOT NULL AND PassportCountry IS NOT NULL AND LEN(LTRIM(RTRIM(PassportNumber))) BETWEEN 1 AND 20 AND LEN(LTRIM(RTRIM(PassportCountry))) BETWEEN 1 AND 50)))
 OR (CustomerType=''Business'' AND CompanyRegistrationNumber IS NOT NULL AND SouthAfricanIDNumber IS NULL AND PassportNumber IS NULL AND PassportCountry IS NULL));';
 IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('dbo.Customers') AND name='UX_Customers_PassportCountry_Number')
 EXEC(N'CREATE UNIQUE INDEX UX_Customers_PassportCountry_Number ON dbo.Customers(PassportCountry,PassportNumber) WHERE PassportNumber IS NOT NULL AND PassportCountry IS NOT NULL;');
 IF NOT EXISTS(SELECT 1 FROM dbo.Roles WITH(UPDLOCK,HOLDLOCK) WHERE RoleName='SysAdmin')
 INSERT dbo.Roles(RoleName,RoleDescription) VALUES('SysAdmin','System security, user accounts, sessions, audit and application configuration');
 -- Staff.RoleID references dbo.Roles. dbo.StaffRoles is not used or modified.
 IF OBJECT_ID('dbo.AppAccountSecurity','U') IS NULL
 BEGIN
 CREATE TABLE dbo.AppAccountSecurity(SecurityID bigint IDENTITY PRIMARY KEY, StaffID int NULL REFERENCES dbo.Staff(StaffID), CustomerID int NULL REFERENCES dbo.Customers(CustomerID), IsBlocked bit NOT NULL DEFAULT(0), SecurityVersion int NOT NULL DEFAULT(1), BlockReason nvarchar(300) NULL, UpdatedAtUtc datetime2 NOT NULL DEFAULT(SYSUTCDATETIME()), CONSTRAINT CK_AppAccountSecurity_Owner CHECK((StaffID IS NOT NULL AND CustomerID IS NULL) OR (StaffID IS NULL AND CustomerID IS NOT NULL)));
 CREATE UNIQUE INDEX UX_AppAccountSecurity_Staff ON dbo.AppAccountSecurity(StaffID) WHERE StaffID IS NOT NULL;
 CREATE UNIQUE INDEX UX_AppAccountSecurity_Customer ON dbo.AppAccountSecurity(CustomerID) WHERE CustomerID IS NOT NULL;
 END;
 IF OBJECT_ID('dbo.AppUserSessions','U') IS NULL
 BEGIN
 CREATE TABLE dbo.AppUserSessions(SessionRecordID uniqueidentifier NOT NULL PRIMARY KEY, StaffID int NULL REFERENCES dbo.Staff(StaffID), CustomerID int NULL REFERENCES dbo.Customers(CustomerID), TokenHash varbinary(32) NOT NULL, SecurityVersion int NOT NULL, IPAddress varchar(45) NOT NULL, UserAgent nvarchar(256) NOT NULL, ClientApplication varchar(30) NOT NULL, LoggedInAtUtc datetime2 NOT NULL DEFAULT(SYSUTCDATETIME()), LastSeenAtUtc datetime2 NOT NULL DEFAULT(SYSUTCDATETIME()), ExpiresAtUtc datetime2 NOT NULL, EndedAtUtc datetime2 NULL, EndReason varchar(50) NULL, CONSTRAINT CK_AppUserSessions_Owner CHECK((StaffID IS NOT NULL AND CustomerID IS NULL) OR(StaffID IS NULL AND CustomerID IS NOT NULL)));
 CREATE INDEX IX_AppUserSessions_Staff ON dbo.AppUserSessions(StaffID,EndedAtUtc,ExpiresAtUtc);
 CREATE INDEX IX_AppUserSessions_Customer ON dbo.AppUserSessions(CustomerID,EndedAtUtc,ExpiresAtUtc);
 END;
 IF OBJECT_ID('dbo.AppSecurityAudit','U') IS NULL
 BEGIN
 CREATE TABLE dbo.AppSecurityAudit(AuditID bigint IDENTITY PRIMARY KEY, ActorStaffID int NULL REFERENCES dbo.Staff(StaffID), TargetStaffID int NULL REFERENCES dbo.Staff(StaffID), TargetCustomerID int NULL REFERENCES dbo.Customers(CustomerID), ActionName varchar(50) NOT NULL, Detail nvarchar(500) NOT NULL, IPAddress varchar(45) NOT NULL, OccurredAtUtc datetime2 NOT NULL DEFAULT(SYSUTCDATETIME()));
 CREATE INDEX IX_AppSecurityAudit_Date ON dbo.AppSecurityAudit(OccurredAtUtc);
 END;
 IF OBJECT_ID('dbo.AppSystemSettings','U') IS NULL
 CREATE TABLE dbo.AppSystemSettings(SettingKey varchar(100) NOT NULL PRIMARY KEY, SettingValue nvarchar(500) NOT NULL, UpdatedAtUtc datetime2 NOT NULL DEFAULT(SYSUTCDATETIME()), UpdatedByStaffID int NULL REFERENCES dbo.Staff(StaffID));
 INSERT dbo.AppSystemSettings(SettingKey,SettingValue) SELECT v.k,v.v FROM(VALUES('CompanyDisplayName',N'THY WILL'),('SupportEmail',N''),('PublicRegistrationEnabled',N'true'),('SessionIdleMinutes',N'20'))v(k,v) WHERE NOT EXISTS(SELECT 1 FROM dbo.AppSystemSettings s WHERE s.SettingKey=v.k);
 COMMIT;
 SELECT 'Migration complete. Run 02_Bootstrap_SysAdmin.sql with your chosen staff username.' AS Result;
END TRY
BEGIN CATCH
 IF @@TRANCOUNT>0 ROLLBACK;
 THROW;
END CATCH;
