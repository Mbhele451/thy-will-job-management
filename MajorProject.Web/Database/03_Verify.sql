/* Read-only verification after migration and bootstrap. Run under the application's database principal to check its permissions too. */
SELECT DB_NAME() AS DatabaseName,USER_NAME() AS DatabaseUser;
SELECT name,is_disabled,is_not_trusted,definition FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID('dbo.Customers') AND name IN('CK_Customers_ID_Validation','CK_Customers_IdentityByCustomerType');
SELECT r.RoleID,r.RoleName,COUNT(s.StaffID) AS StaffAccounts FROM dbo.Roles r LEFT JOIN dbo.Staff s ON s.RoleID=r.RoleID GROUP BY r.RoleID,r.RoleName ORDER BY r.RoleID;
SELECT t.name AS NewTable,c.name AS ColumnName,TYPE_NAME(c.user_type_id) AS DataType,c.max_length FROM sys.tables t JOIN sys.columns c ON c.object_id=t.object_id WHERE t.name IN('AppAccountSecurity','AppUserSessions','AppSecurityAudit','AppSystemSettings') ORDER BY t.name,c.column_id;
SELECT n.TableName,HAS_PERMS_BY_NAME('dbo.'+n.TableName,'OBJECT','SELECT') AS CanRead,HAS_PERMS_BY_NAME('dbo.'+n.TableName,'OBJECT','INSERT') AS CanInsert,HAS_PERMS_BY_NAME('dbo.'+n.TableName,'OBJECT','UPDATE') AS CanUpdate FROM(VALUES('AppAccountSecurity'),('AppUserSessions'),('AppSecurityAudit'),('AppSystemSettings'))n(TableName);
SELECT SettingKey,SettingValue FROM dbo.AppSystemSettings ORDER BY SettingKey;
SELECT COUNT(*) AS UsableSysAdminAccounts FROM dbo.Staff s JOIN dbo.Roles r ON r.RoleID=s.RoleID LEFT JOIN dbo.AppAccountSecurity x ON x.StaffID=s.StaffID WHERE r.RoleName='SysAdmin' AND s.EmploymentStatus='Active' AND COALESCE(x.IsBlocked,0)=0;
