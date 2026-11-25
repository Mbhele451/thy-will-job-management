/* Set the existing staff username explicitly. No username is assumed and no password is stored in this script.
   This one-time promotion does not turn ordinary Administrators into SysAdmins. */
SET XACT_ABORT ON; SET NOCOUNT ON;
DECLARE @StaffUsername varchar(50) = 'REPLACE_WITH_EXISTING_STAFF_USERNAME';
IF @StaffUsername='REPLACE_WITH_EXISTING_STAFF_USERNAME' THROW 51010,'Set @StaffUsername to the existing staff account you want to promote.',1;
BEGIN TRY
 BEGIN TRANSACTION;
 DECLARE @StaffID int,@RoleID int,@WasAdmin bit;
 SELECT @RoleID=RoleID FROM dbo.Roles WITH(UPDLOCK,HOLDLOCK) WHERE RoleName='SysAdmin';
 IF @RoleID IS NULL THROW 51011,'Run migration 01 first.',1;
 SELECT @StaffID=s.StaffID,@WasAdmin=CASE WHEN r.RoleName='Administrator' THEN 1 ELSE 0 END FROM dbo.Staff s WITH(UPDLOCK,HOLDLOCK) JOIN dbo.Roles r ON r.RoleID=s.RoleID WHERE s.Username=@StaffUsername AND s.EmploymentStatus='Active' AND r.RoleName IN('Administrator','SysAdmin');
 IF @StaffID IS NULL THROW 51012,'Choose an active Administrator account for bootstrap. Manager and Technician operational roles are not promoted by this script.',1;
 IF @WasAdmin=1 AND EXISTS(SELECT 1 FROM dbo.Jobs WITH(UPDLOCK,HOLDLOCK) WHERE StaffID=@StaffID AND JobStatus NOT IN('Completed','Cancelled')) THROW 51015,'The selected staff account has open assigned jobs. Reassign those jobs before changing its role.',1;
 IF EXISTS(SELECT 1 FROM dbo.AppAccountSecurity WHERE StaffID=@StaffID AND IsBlocked=1) THROW 51013,'The selected account is blocked. Review the block before promotion.',1;
 IF EXISTS(SELECT 1 FROM dbo.Staff s JOIN dbo.Roles r ON r.RoleID=s.RoleID WHERE r.RoleName='SysAdmin' AND s.StaffID<>@StaffID)
 THROW 51014,'A SysAdmin already exists. Use its Users screen for further role changes.',1;
 UPDATE dbo.Staff SET RoleID=@RoleID WHERE StaffID=@StaffID;
 UPDATE dbo.AppUserSessions SET EndedAtUtc=SYSUTCDATETIME(),EndReason='Bootstrap role change' WHERE StaffID=@StaffID AND EndedAtUtc IS NULL;
 INSERT dbo.AppSecurityAudit(ActorStaffID,TargetStaffID,ActionName,Detail,IPAddress) VALUES(NULL,@StaffID,'BootstrapSysAdmin',N'Existing active staff account promoted by explicit SQL bootstrap.','SQL');
 COMMIT;
 SELECT StaffID,Username,r.RoleName FROM dbo.Staff s JOIN dbo.Roles r ON r.RoleID=s.RoleID WHERE StaffID=@StaffID;
END TRY
BEGIN CATCH
 IF @@TRANCOUNT>0 ROLLBACK;
 THROW;
END CATCH;
