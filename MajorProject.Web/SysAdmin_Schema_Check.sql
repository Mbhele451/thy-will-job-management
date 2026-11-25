/*
THY WILL: read-only schema check for nationality/permit registration and SysAdmin.
Select your application database in SSMS, then execute the complete script.
Return all result grids. No rows, tables, roles or permissions are changed.
No password hashes, reset codes, identity numbers or account contact values are selected.
If metadata visibility is limited, return the errors or empty grids too.
*/
SET NOCOUNT ON;

-- 1. Confirm which database was inspected.
SELECT DB_NAME() AS DatabaseName, compatibility_level AS CompatibilityLevel
FROM sys.databases WHERE name = DB_NAME();

-- 2. Existing table and column definitions, including any current security tables.
SELECT s.name AS SchemaName, t.name AS TableName, c.column_id AS ColumnOrder,
       c.name AS ColumnName, ty.name AS DataType,
       c.max_length AS MaxLengthBytes, c.precision AS NumericPrecision,
       c.scale AS NumericScale, c.is_nullable AS IsNullable,
       c.is_identity AS IsIdentity, c.is_computed AS IsComputed,
       dc.definition AS DefaultDefinition
FROM sys.tables AS t
JOIN sys.schemas AS s ON s.schema_id = t.schema_id
JOIN sys.columns AS c ON c.object_id = t.object_id
JOIN sys.types AS ty ON ty.user_type_id = c.user_type_id
LEFT JOIN sys.default_constraints AS dc ON dc.object_id = c.default_object_id
WHERE t.is_ms_shipped = 0
ORDER BY s.name, t.name, c.column_id;

-- 3. Validation constraints affecting identity, statuses or role values.
SELECT OBJECT_SCHEMA_NAME(parent_object_id) AS SchemaName,
       OBJECT_NAME(parent_object_id) AS TableName,
       name AS ConstraintName, definition AS CheckDefinition,
       is_disabled AS IsDisabled, is_not_trusted AS IsNotTrusted
FROM sys.check_constraints
ORDER BY SchemaName, TableName, ConstraintName;

-- 4. Existing relationships, including account dependencies.
SELECT OBJECT_SCHEMA_NAME(fk.parent_object_id) AS SchemaName,
       OBJECT_NAME(fk.parent_object_id) AS TableName,
       fk.name AS ForeignKeyName, pc.name AS ColumnName,
       OBJECT_SCHEMA_NAME(fk.referenced_object_id) AS ReferencedSchema,
       OBJECT_NAME(fk.referenced_object_id) AS ReferencedTable,
       rc.name AS ReferencedColumn,
       fk.delete_referential_action_desc AS OnDelete,
       fk.update_referential_action_desc AS OnUpdate
FROM sys.foreign_keys AS fk
JOIN sys.foreign_key_columns AS fc ON fc.constraint_object_id = fk.object_id
JOIN sys.columns AS pc ON pc.object_id = fc.parent_object_id AND pc.column_id = fc.parent_column_id
JOIN sys.columns AS rc ON rc.object_id = fc.referenced_object_id AND rc.column_id = fc.referenced_column_id
ORDER BY SchemaName, TableName, ForeignKeyName, fc.constraint_column_id;

-- 5. Indexes, including filtered uniqueness for IDs, permits, emails and usernames.
SELECT s.name AS SchemaName, t.name AS TableName, i.name AS IndexName,
       i.is_unique AS IsUnique, i.is_primary_key AS IsPrimaryKey,
       i.filter_definition AS FilterDefinition, c.name AS ColumnName,
       ic.key_ordinal AS KeyOrder, ic.is_included_column AS IsIncludedColumn
FROM sys.tables AS t
JOIN sys.schemas AS s ON s.schema_id = t.schema_id
JOIN sys.indexes AS i ON i.object_id = t.object_id AND i.index_id > 0
JOIN sys.index_columns AS ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id
JOIN sys.columns AS c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
WHERE t.is_ms_shipped = 0
ORDER BY s.name, t.name, i.name, ic.key_ordinal, ic.index_column_id;

-- 6. Role names and counts. No individual account details.
IF OBJECT_ID(N'dbo.Roles', N'U') IS NOT NULL
   AND COL_LENGTH(N'dbo.Roles', N'RoleID') IS NOT NULL
   AND COL_LENGTH(N'dbo.Roles', N'RoleName') IS NOT NULL
BEGIN
    EXEC sys.sp_executesql N'SELECT RoleID, RoleName FROM dbo.Roles ORDER BY RoleID;';
    IF OBJECT_ID(N'dbo.Staff', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.Staff', N'RoleID') IS NOT NULL
        EXEC sys.sp_executesql N'SELECT r.RoleName, COUNT_BIG(s.StaffID) AS AccountCount
        FROM dbo.Roles AS r LEFT JOIN dbo.Staff AS s ON s.RoleID = r.RoleID
        GROUP BY r.RoleName ORDER BY r.RoleName;';
END;

-- 7. Status values already in use, grouped only.
IF COL_LENGTH(N'dbo.Staff', N'EmploymentStatus') IS NOT NULL
    EXEC sys.sp_executesql N'SELECT EmploymentStatus, COUNT_BIG(*) AS AccountCount
        FROM dbo.Staff GROUP BY EmploymentStatus;';
IF COL_LENGTH(N'dbo.Customers', N'AccountStatus') IS NOT NULL
    EXEC sys.sp_executesql N'SELECT AccountStatus, COUNT_BIG(*) AS AccountCount
        FROM dbo.Customers GROUP BY AccountStatus;';

-- 8. Triggers and stored procedure names, to identify hidden validation rules.
SELECT OBJECT_SCHEMA_NAME(tr.parent_id) AS SchemaName,
       OBJECT_NAME(tr.parent_id) AS TableName, tr.name AS TriggerName,
       tr.is_disabled AS IsDisabled, sm.definition AS TriggerDefinition
FROM sys.triggers AS tr
LEFT JOIN sys.sql_modules AS sm ON sm.object_id = tr.object_id
WHERE tr.parent_class = 1
  AND OBJECT_NAME(tr.parent_id) IN (N'Customers', N'Staff', N'Roles')
ORDER BY TableName, TriggerName;
SELECT SCHEMA_NAME(schema_id) AS SchemaName, name AS ProcedureName
FROM sys.procedures WHERE is_ms_shipped = 0 ORDER BY SchemaName, ProcedureName;
