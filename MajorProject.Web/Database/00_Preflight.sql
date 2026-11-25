/* READ ONLY. Shows record IDs/counts only, never identity-document values, passwords or reset codes. */
SELECT DB_NAME() AS DatabaseName;
SELECT name,definition,is_disabled,is_not_trusted FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID('dbo.Customers');
SELECT CustomerID,CustomerType,'Identity combination needs review' AS Issue
FROM dbo.Customers WHERE NOT (
(CustomerType='Residential' AND CompanyRegistrationNumber IS NULL AND (
(SouthAfricanIDNumber IS NOT NULL AND LEN(SouthAfricanIDNumber)=13 AND SouthAfricanIDNumber NOT LIKE '%[^0-9]%' AND PassportNumber IS NULL AND PassportCountry IS NULL)
OR(SouthAfricanIDNumber IS NULL AND LEN(LTRIM(RTRIM(COALESCE(PassportNumber,'')))) BETWEEN 1 AND 20 AND LEN(LTRIM(RTRIM(COALESCE(PassportCountry,'')))) BETWEEN 1 AND 50)))
OR(CustomerType='Business' AND CompanyRegistrationNumber IS NOT NULL AND SouthAfricanIDNumber IS NULL AND PassportNumber IS NULL AND PassportCountry IS NULL));
SELECT COUNT(*) AS DuplicatePassportGroups FROM(SELECT 1 AS x FROM dbo.Customers WHERE PassportNumber IS NOT NULL GROUP BY UPPER(LTRIM(RTRIM(PassportCountry))),UPPER(LTRIM(RTRIM(PassportNumber))) HAVING COUNT(*)>1)d;
SELECT COUNT(*) AS DuplicateStaffEmailGroups FROM(SELECT 1 AS x FROM dbo.Staff WHERE EmailAddress IS NOT NULL GROUP BY LOWER(LTRIM(RTRIM(EmailAddress))) HAVING COUNT(*)>1)d;
SELECT s.StaffID,'Staff email collides with customer login' AS Issue FROM dbo.Staff s WHERE EXISTS(SELECT 1 FROM dbo.Customers c WHERE c.EmailAddress=s.EmailAddress OR c.EmailAddress=s.Username);
SELECT RoleID,RoleName FROM dbo.Roles ORDER BY RoleID;
SELECT name FROM sys.tables WHERE name IN('AppAccountSecurity','AppUserSessions','AppSecurityAudit','AppSystemSettings');
SELECT 'Staff.RoleID uses dbo.Roles. dbo.StaffRoles must not be used to provision SysAdmin.' AS RoleTableFinding;
-- Existing object dependencies to review before the Unicode name/country conversion.
SELECT OBJECT_SCHEMA_NAME(d.referencing_id) AS ReferencingSchema,OBJECT_NAME(d.referencing_id) AS ReferencingObject,d.referenced_entity_name AS ReferencedTable,COL_NAME(d.referenced_id,d.referenced_minor_id) AS ReferencedColumn
FROM sys.sql_expression_dependencies d WHERE d.referenced_id IN(OBJECT_ID('dbo.Customers'),OBJECT_ID('dbo.Staff')) ORDER BY ReferencingSchema,ReferencingObject;
SELECT OBJECT_NAME(ic.object_id) AS TableName,i.name AS IndexName,c.name AS ColumnName FROM sys.index_columns ic JOIN sys.indexes i ON i.object_id=ic.object_id AND i.index_id=ic.index_id JOIN sys.columns c ON c.object_id=ic.object_id AND c.column_id=ic.column_id WHERE ic.object_id IN(OBJECT_ID('dbo.Customers'),OBJECT_ID('dbo.Staff')) AND c.name IN('FullName','PassportCountry');
