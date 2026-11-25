/* Only if 03_Verify.sql reports missing permissions for the application's database user.
   Set an existing DATABASE principal explicitly. This grants no server role and no db_owner access. */
DECLARE @AppDatabaseUser sysname=N'REPLACE_WITH_APP_DATABASE_USER';
IF @AppDatabaseUser=N'REPLACE_WITH_APP_DATABASE_USER' THROW 51020,'Set the existing application database principal first.',1;
IF NOT EXISTS(SELECT 1 FROM sys.database_principals WHERE name=@AppDatabaseUser AND type IN('S','U','G','R')) THROW 51021,'The database principal does not exist.',1;
DECLARE @who nvarchar(258)=QUOTENAME(@AppDatabaseUser);
EXEC(N'GRANT SELECT,INSERT,UPDATE ON dbo.AppAccountSecurity TO '+@who+N'; GRANT SELECT,INSERT,UPDATE ON dbo.AppUserSessions TO '+@who+N'; GRANT SELECT,INSERT ON dbo.AppSecurityAudit TO '+@who+N'; GRANT SELECT,UPDATE ON dbo.AppSystemSettings TO '+@who+N';');
