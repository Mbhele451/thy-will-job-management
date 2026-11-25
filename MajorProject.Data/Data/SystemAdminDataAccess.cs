using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace MajorProject.Data.Data
{
    public sealed class SystemAdminDataAccess
    {
        private readonly string cs;
        public SystemAdminDataAccess(string connectionString)
        {
            cs = connectionString;
        }

        private static SqlParameter P(string name, object value)
        {
            return new SqlParameter(name, value ?? DBNull.Value);
        }

        private static SqlCommand Cmd(SqlConnection c, SqlTransaction t, string sql, params SqlParameter[] p)
        {
            var q = new SqlCommand(sql, c, t);
            q.CommandTimeout = 30;
            if (p != null)
                q.Parameters.AddRange(p);
            return q;
        }

        private DataTable Read(string sql, params SqlParameter[] p)
        {
            using (var c = new SqlConnection(cs))
            using (var q = Cmd(c, null, sql, p))
            using (var a = new SqlDataAdapter(q))
            {
                var d = new DataTable();
                a.Fill(d);
                return d;
            }
        }

        private const string ActorSql = @"SELECT COUNT(*) FROM dbo.Staff s WITH(UPDLOCK,HOLDLOCK) JOIN dbo.Roles r ON r.RoleID=s.RoleID LEFT JOIN dbo.AppAccountSecurity x WITH(UPDLOCK,HOLDLOCK) ON x.StaffID=s.StaffID WHERE s.StaffID=@a AND r.RoleName='SysAdmin' AND s.EmploymentStatus='Active' AND COALESCE(x.IsBlocked,0)=0";
        private static void Actor(SqlConnection c, SqlTransaction t, int actor)
        {
            using (var q = Cmd(c, t, ActorSql, P("@a", actor)))
                if (Convert.ToInt32(q.ExecuteScalar()) != 1)
                    throw new UnauthorizedAccessException("SysAdmin access is required.");
        }

        private void AuthorizedRead(int actor)
        {
            using (var c = new SqlConnection(cs))
            {
                c.Open();
                using (var t = c.BeginTransaction(IsolationLevel.Serializable))
                {
                    Actor(c, t, actor);
                    t.Commit();
                }
            }
        }

        private T Write<T>(int actor, Func<SqlConnection, SqlTransaction, T> action)
        {
            using (var c = new SqlConnection(cs))
            {
                c.Open();
                using (var t = c.BeginTransaction(IsolationLevel.Serializable))
                {
                    Actor(c, t, actor);
                    var result = action(c, t);
                    t.Commit();
                    return result;
                }
            }
        }

        private static string Owner(bool staff)
        {
            return staff ? "StaffID" : "CustomerID";
        }

        private static string AccountTable(bool staff)
        {
            return staff ? "dbo.Staff" : "dbo.Customers";
        }

        private static void EnsureSecurity(SqlConnection c, SqlTransaction t, bool staff, int id)
        {
            using (var q = Cmd(c, t, "IF NOT EXISTS(SELECT 1 FROM dbo.AppAccountSecurity WITH(UPDLOCK,HOLDLOCK) WHERE " + Owner(staff) + "=@id) INSERT dbo.AppAccountSecurity(" + Owner(staff) + ") VALUES(@id);", P("@id", id)))
                q.ExecuteNonQuery();
        }

        private static void Audit(SqlConnection c, SqlTransaction t, int actor, bool staff, int id, string action, string detail, string ip)
        {
            using (var q = Cmd(c, t, "INSERT dbo.AppSecurityAudit(ActorStaffID,TargetStaffID,TargetCustomerID,ActionName,Detail,IPAddress) VALUES(@a,@s,@c,@n,@d,@ip)", P("@a", actor), P("@s", staff ? (object)id : null), P("@c", staff ? null : (object)id), P("@n", action), P("@d", detail), P("@ip", ip)))
                q.ExecuteNonQuery();
        }

        private static void Target(SqlConnection c, SqlTransaction t, bool staff, int id)
        {
            using (var q = Cmd(c, t, "SELECT COUNT(*) FROM " + AccountTable(staff) + " WITH(UPDLOCK,HOLDLOCK) WHERE " + Owner(staff) + "=@id", P("@id", id)))
                if (Convert.ToInt32(q.ExecuteScalar()) != 1)
                    throw new ArgumentException("The selected account no longer exists.");
        }

        private static void KeepSysAdmin(SqlConnection c, SqlTransaction t, int id)
        {
            using (var q = Cmd(c, t, @"IF EXISTS(SELECT 1 FROM dbo.Staff s JOIN dbo.Roles r ON r.RoleID=s.RoleID LEFT JOIN dbo.AppAccountSecurity ownSecurity ON ownSecurity.StaffID=s.StaffID WHERE s.StaffID=@id AND r.RoleName='SysAdmin' AND s.EmploymentStatus='Active' AND COALESCE(ownSecurity.IsBlocked,0)=0) AND (SELECT COUNT(*) FROM dbo.Staff s WITH(UPDLOCK,HOLDLOCK) JOIN dbo.Roles r ON r.RoleID=s.RoleID LEFT JOIN dbo.AppAccountSecurity x WITH(UPDLOCK,HOLDLOCK) ON x.StaffID=s.StaffID WHERE r.RoleName='SysAdmin' AND s.EmploymentStatus='Active' AND COALESCE(x.IsBlocked,0)=0)<=1 THROW 51100,'The last usable SysAdmin must remain available.',1;", P("@id", id)))
                q.ExecuteNonQuery();
        }

        private static void Invalidate(SqlConnection c, SqlTransaction t, bool staff, int id, string reason)
        {
            EnsureSecurity(c, t, staff, id);
            using (var q = Cmd(c, t, "UPDATE dbo.AppAccountSecurity SET SecurityVersion=SecurityVersion+1,UpdatedAtUtc=SYSUTCDATETIME() WHERE " + Owner(staff) + "=@id; UPDATE dbo.AppUserSessions SET EndedAtUtc=SYSUTCDATETIME(),EndReason=@r WHERE " + Owner(staff) + "=@id AND EndedAtUtc IS NULL;", P("@id", id), P("@r", reason)))
                q.ExecuteNonQuery();
        }

        public bool LoginAllowed(bool staff, int id)
        {
            var d = Read("SELECT 1 FROM " + AccountTable(staff) + " a LEFT JOIN dbo.AppAccountSecurity x ON x." + Owner(staff) + "=a." + Owner(staff) + " WHERE a." + Owner(staff) + "=@id AND a." + (staff ? "EmploymentStatus" : "AccountStatus") + "='Active' AND COALESCE(x.IsBlocked,0)=0", P("@id", id));
            return d.Rows.Count == 1;
        }

        public DataTable Accounts(int actor, string search)
        {
            AuthorizedRead(actor);
            return Read(@"SELECT TOP(1000) * FROM (
   SELECT 'Staff' AS AccountType,s.StaffID AS AccountID,s.FullName,s.Username,s.EmailAddress,r.RoleName,s.EmploymentStatus AS AccountStatus,CAST(COALESCE(x.IsBlocked,0) AS bit) AS IsBlocked,x.BlockReason,CASE WHEN NULLIF(s.PasswordHash,'') IS NULL THEN 'Missing' WHEN s.PasswordHash LIKE 'PBKDF2:100000:%' THEN 'Strong hash' ELSE 'Legacy hash - reset recommended' END AS PasswordPosture FROM dbo.Staff s JOIN dbo.Roles r ON r.RoleID=s.RoleID LEFT JOIN dbo.AppAccountSecurity x ON x.StaffID=s.StaffID
   UNION ALL SELECT 'Customer',c.CustomerID,c.FullName,c.EmailAddress,c.EmailAddress,'Customer',c.AccountStatus,CAST(COALESCE(x.IsBlocked,0) AS bit),x.BlockReason,CASE WHEN NULLIF(c.PasswordHash,'') IS NULL THEN 'Missing' WHEN c.PasswordHash LIKE 'PBKDF2:100000:%' THEN 'Strong hash' ELSE 'Legacy hash - reset recommended' END FROM dbo.Customers c LEFT JOIN dbo.AppAccountSecurity x ON x.CustomerID=c.CustomerID)a WHERE @q='' OR FullName LIKE @q+'%' OR Username LIKE @q+'%' OR EmailAddress LIKE @q+'%' ORDER BY AccountType,AccountID", P("@q", search));
        }

        public void Block(int actor, bool staff, int id, bool blocked, string reason, string ip)
        {
            Write(actor, (c, t) =>
            {
                Target(c, t, staff, id);
                if (staff && id == actor && blocked)
                    throw new ArgumentException("You cannot block your own account.");
                if (staff && blocked)
                    KeepSysAdmin(c, t, id);
                EnsureSecurity(c, t, staff, id);
                using (var q = Cmd(c, t, "UPDATE dbo.AppAccountSecurity SET IsBlocked=@b,BlockReason=@r,UpdatedAtUtc=SYSUTCDATETIME() WHERE " + Owner(staff) + "=@id", P("@b", blocked), P("@r", blocked ? reason : null), P("@id", id)))
                    q.ExecuteNonQuery();
                if (staff && blocked)
                    using (var q = Cmd(c, t, "UPDATE dbo.Staff SET IsAvailable=0 WHERE StaffID=@id", P("@id", id)))
                        q.ExecuteNonQuery();
                Invalidate(c, t, staff, id, blocked ? "Account blocked" : "Security changed");
                Audit(c, t, actor, staff, id, blocked ? "BlockAccount" : "UnblockAccount", reason, ip);
                return 0;
            });
        }

        private static void UniqueLogin(SqlConnection c, SqlTransaction t, bool staff, int id, string username, string email)
        {
            using (var q = Cmd(c, t, @"IF EXISTS(SELECT 1 FROM dbo.Staff WITH(UPDLOCK,HOLDLOCK) WHERE (@staff=0 OR StaffID<>@id) AND (Username=@email OR EmailAddress=@email OR (@u<>'' AND (Username=@u OR EmailAddress=@u)))) OR EXISTS(SELECT 1 FROM dbo.Customers WITH(UPDLOCK,HOLDLOCK) WHERE (@staff=1 OR CustomerID<>@id) AND (EmailAddress=@email OR (@u<>'' AND EmailAddress=@u))) THROW 51101,'The username or email conflicts with an existing login.',1;", P("@staff", staff), P("@id", id), P("@u", username), P("@email", email)))
                q.ExecuteNonQuery();
        }

        public void Credentials(int actor, bool staff, int id, string username, string email, string hash, string role, string ip)
        {
            Write(actor, (c, t) =>
            {
                Target(c, t, staff, id);
                string oldRole = "Customer";
                if (staff)
                {
                    using (var roleQuery = Cmd(c, t, "SELECT r.RoleName FROM dbo.Staff s JOIN dbo.Roles r ON r.RoleID=s.RoleID WHERE s.StaffID=@id", P("@id", id)))
                        oldRole = Convert.ToString(roleQuery.ExecuteScalar());
                    using (var jobGuard = Cmd(c, t, "IF EXISTS(SELECT 1 FROM dbo.Staff s JOIN dbo.Roles r ON r.RoleID=s.RoleID WHERE s.StaffID=@id AND r.RoleName<>@role) AND EXISTS(SELECT 1 FROM dbo.Jobs WITH(UPDLOCK,HOLDLOCK) WHERE StaffID=@id AND JobStatus NOT IN('Completed','Cancelled')) THROW 51102,'Reassign open jobs before changing this staff role.',1;", P("@id", id), P("@role", role)))
                        jobGuard.ExecuteNonQuery();
                }

                UniqueLogin(c, t, staff, id, username, email);
                if (staff && role != "SysAdmin")
                    KeepSysAdmin(c, t, id);
                string sql = staff ? @"UPDATE dbo.Staff SET Username=@u,EmailAddress=@e,RoleID=(SELECT RoleID FROM dbo.Roles WHERE RoleName=@role),PasswordHash=COALESCE(@hash,PasswordHash),ResetOTP=NULL,ResetOTPExpiry=NULL WHERE StaffID=@id" : @"UPDATE dbo.Customers SET EmailAddress=@e,PasswordHash=COALESCE(@hash,PasswordHash),ResetOTP=NULL,ResetOTPExpiry=NULL WHERE CustomerID=@id";
                using (var q = Cmd(c, t, sql, P("@u", username), P("@e", email), P("@hash", hash), P("@role", role), P("@id", id)))
                    q.ExecuteNonQuery();
                Invalidate(c, t, staff, id, "Credentials changed");
                Audit(c, t, actor, staff, id, "UpdateCredentials", "Login/email updated. Role: " + oldRole + " -> " + role + ". Password reset: " + (hash == null ? "no" : "yes") + ". Sessions revoked.", ip);
                return 0;
            });
        }

        public int CreateCustomer(int actor, string ip, string name, string phone, string email, string address, string area, string city, string province, string type, string sa, string company, string hash, string passport, string country)
        {
            return Write(actor, (c, t) =>
            {
                UniqueLogin(c, t, false, 0, "", email);
                int id;
                using (var q = Cmd(c, t, @"INSERT dbo.Customers(FullName,PhoneNumber,EmailAddress,FullAddress,Area,City,Province,CustomerType,SouthAfricanIDNumber,CompanyRegistrationNumber,PasswordHash,PassportNumber,PassportCountry,AccountStatus,DateRegistered) OUTPUT INSERTED.CustomerID VALUES(@n,@p,@e,@a,@area,@city,@prov,@type,@sa,@co,@h,@pass,@country,'Active',GETDATE())", P("@n", name), P("@p", phone), P("@e", email), P("@a", address), P("@area", area), P("@city", city), P("@prov", province), P("@type", type), P("@sa", string.IsNullOrWhiteSpace(sa) ? null : sa), P("@co", string.IsNullOrWhiteSpace(company) ? null : company), P("@h", hash), P("@pass", string.IsNullOrWhiteSpace(passport) ? null : passport), P("@country", string.IsNullOrWhiteSpace(country) ? null : country)))
                    id = Convert.ToInt32(q.ExecuteScalar());
                EnsureSecurity(c, t, false, id);
                Audit(c, t, actor, false, id, "CreateCustomerAccount", "Customer account registered by SysAdmin.", ip);
                return id;
            });
        }

        public int CreateStaff(int actor, string name, string phone, string email, string username, string hash, string role, decimal salary, string ip)
        {
            return Write(actor, (c, t) =>
            {
                UniqueLogin(c, t, true, 0, username, email);
                int id;
                using (var q = Cmd(c, t, @"INSERT dbo.Staff(RoleID,FullName,PhoneNumber,EmailAddress,HireDate,Salary,EmploymentStatus,Username,PasswordHash,IsAvailable) OUTPUT INSERTED.StaffID SELECT RoleID,@n,@p,@e,GETDATE(),@salary,'Active',@u,@h,CASE WHEN RoleName='Technician' THEN 1 ELSE 0 END FROM dbo.Roles WHERE RoleName=@role", P("@n", name), P("@p", phone), P("@e", email), P("@salary", salary), P("@u", username), P("@h", hash), P("@role", role)))
                {
                    var value = q.ExecuteScalar();
                    if (value == null)
                        throw new ArgumentException("The selected role is unavailable.");
                    id = Convert.ToInt32(value);
                }

                EnsureSecurity(c, t, true, id);
                Audit(c, t, actor, true, id, "CreateStaffAccount", "Staff account created.", ip);
                return id;
            });
        }

        public DataTable Report(int actor, string kind, DateTime from, DateTime to)
        {
            AuthorizedRead(actor);
            string sql;
            switch (kind)
            {
                case "Sessions":
                    sql = @"SELECT TOP(2000) s.SessionRecordID,CASE WHEN s.StaffID IS NOT NULL THEN 'Staff' ELSE 'Customer' END AS AccountType,COALESCE(s.StaffID,s.CustomerID) AS AccountID,COALESCE(st.FullName,c.FullName) AS FullName,s.IPAddress,s.UserAgent,s.ClientApplication,s.LoggedInAtUtc,s.LastSeenAtUtc,s.ExpiresAtUtc,s.EndedAtUtc,s.EndReason,CASE WHEN s.EndedAtUtc IS NULL AND s.ExpiresAtUtc>SYSUTCDATETIME() THEN 'Active' ELSE 'Ended or expired' END AS SessionStatus FROM dbo.AppUserSessions s LEFT JOIN dbo.Staff st ON st.StaffID=s.StaffID LEFT JOIN dbo.Customers c ON c.CustomerID=s.CustomerID WHERE s.LoggedInAtUtc>=@f AND s.LoggedInAtUtc<@t ORDER BY s.LastSeenAtUtc DESC";
                    break;
                case "Audit":
                    sql = @"SELECT TOP(2000) a.AuditID,a.OccurredAtUtc,a.ActorStaffID,a.TargetStaffID,a.TargetCustomerID,a.ActionName,a.Detail,a.IPAddress FROM dbo.AppSecurityAudit a WHERE a.OccurredAtUtc>=@f AND a.OccurredAtUtc<@t ORDER BY a.AuditID DESC";
                    break;
                default:
                    throw new ArgumentException("Choose Sessions or Audit.");
            }

            return Read(sql, P("@f", from), P("@t", to));
        }

        public void Revoke(int actor, Guid session, string ip)
        {
            Write(actor, (c, t) =>
            {
                var d = new DataTable();
                using (var q = Cmd(c, t, "SELECT StaffID,CustomerID FROM dbo.AppUserSessions WITH(UPDLOCK,HOLDLOCK) WHERE SessionRecordID=@id", P("@id", session)))
                using (var a = new SqlDataAdapter(q))
                    a.Fill(d);
                if (d.Rows.Count != 1)
                    throw new ArgumentException("Session not found.");
                var r = d.Rows[0];
                bool staff = r["StaffID"] != DBNull.Value;
                int id = Convert.ToInt32(r[Owner(staff)]);
                using (var q = Cmd(c, t, "UPDATE dbo.AppUserSessions SET EndedAtUtc=SYSUTCDATETIME(),EndReason='Revoked by SysAdmin' WHERE SessionRecordID=@id AND EndedAtUtc IS NULL", P("@id", session)))
                    q.ExecuteNonQuery();
                Audit(c, t, actor, staff, id, "RevokeSession", "Authenticated session revoked.", ip);
                return 0;
            });
        }

        public DataTable Settings(int actor)
        {
            AuthorizedRead(actor);
            return Read("SELECT SettingKey,SettingValue,UpdatedAtUtc FROM dbo.AppSystemSettings WHERE SettingKey IN('CompanyDisplayName','SupportEmail','PublicRegistrationEnabled','SessionIdleMinutes') ORDER BY SettingKey");
        }

        public string Setting(string key, string fallback)
        {
            var d = Read("SELECT SettingValue FROM dbo.AppSystemSettings WHERE SettingKey=@k", P("@k", key));
            return d.Rows.Count == 1 ? Convert.ToString(d.Rows[0][0]) : fallback;
        }

        public void SaveSetting(int actor, string key, string value, string ip)
        {
            Write(actor, (c, t) =>
            {
                using (var q = Cmd(c, t, "UPDATE dbo.AppSystemSettings SET SettingValue=@v,UpdatedAtUtc=SYSUTCDATETIME(),UpdatedByStaffID=@a WHERE SettingKey=@k", P("@k", key), P("@v", value), P("@a", actor)))
                    if (q.ExecuteNonQuery() != 1)
                        throw new ArgumentException("Setting not found.");
                if (key == "SessionIdleMinutes")
                    using (var q = Cmd(c, t, "UPDATE dbo.AppUserSessions SET ExpiresAtUtc=DATEADD(minute,@m,LastSeenAtUtc) WHERE EndedAtUtc IS NULL AND ExpiresAtUtc>DATEADD(minute,@m,LastSeenAtUtc)", P("@m", int.Parse(value))))
                        q.ExecuteNonQuery();
                Audit(c, t, actor, true, actor, "UpdateSetting", "Changed setting: " + key, ip);
                return 0;
            });
        }

        public void CheckActor(int actor)
        {
            AuthorizedRead(actor);
        }

        public string StartSession(bool staff, int id, Guid record, byte[] hash, string ip, string agent, string client, int minutes)
        {
            using (var c = new SqlConnection(cs))
            {
                c.Open();
                using (var t = c.BeginTransaction(IsolationLevel.Serializable))
                {
                    Target(c, t, staff, id);
                    EnsureSecurity(c, t, staff, id);
                    using (var q = Cmd(c, t, "SELECT COUNT(*) FROM " + AccountTable(staff) + " a JOIN dbo.AppAccountSecurity x ON x." + Owner(staff) + "=a." + Owner(staff) + " WHERE a." + Owner(staff) + "=@id AND a." + (staff ? "EmploymentStatus" : "AccountStatus") + "='Active' AND x.IsBlocked=0", P("@id", id)))
                        if (Convert.ToInt32(q.ExecuteScalar()) != 1)
                            throw new UnauthorizedAccessException("The account is unavailable.");
                    using (var q = Cmd(c, t, "INSERT dbo.AppUserSessions(SessionRecordID," + Owner(staff) + ",TokenHash,SecurityVersion,IPAddress,UserAgent,ClientApplication,ExpiresAtUtc) SELECT @r,@id,@h,SecurityVersion,@ip,@ua,@client,DATEADD(minute,@m,SYSUTCDATETIME()) FROM dbo.AppAccountSecurity WHERE " + Owner(staff) + "=@id", P("@r", record), P("@id", id), P("@h", hash), P("@ip", ip), P("@ua", agent), P("@client", client), P("@m", minutes)))
                        q.ExecuteNonQuery();
                    t.Commit();
                }
            }

            return record.ToString();
        }

        public DataTable ValidateSession(bool staff, int id, Guid record, byte[] hash, int minutes, string currentIP = null, bool refreshActivity = true)
        {
            return Read("UPDATE s SET IPAddress=COALESCE(@ip,s.IPAddress),LastSeenAtUtc=CASE WHEN @touch=1 THEN SYSUTCDATETIME() ELSE s.LastSeenAtUtc END,ExpiresAtUtc=CASE WHEN @touch=1 THEN DATEADD(minute,@m,SYSUTCDATETIME()) ELSE s.ExpiresAtUtc END OUTPUT inserted.ExpiresAtUtc, " + (staff ? "r.RoleName" : "'Customer' AS RoleName") + " FROM dbo.AppUserSessions s JOIN " + AccountTable(staff) + " a ON a." + Owner(staff) + "=s." + Owner(staff) + (staff ? " JOIN dbo.Roles r ON r.RoleID=a.RoleID" : "") + " JOIN dbo.AppAccountSecurity x ON x." + Owner(staff) + "=a." + Owner(staff) + " WHERE s.SessionRecordID=@r AND s.TokenHash=@h AND s." + Owner(staff) + "=@id AND x.IsBlocked=0 AND x.SecurityVersion=s.SecurityVersion AND a." + (staff ? "EmploymentStatus" : "AccountStatus") + "='Active' AND s.EndedAtUtc IS NULL AND s.ExpiresAtUtc>SYSUTCDATETIME()", P("@r", record), P("@h", hash), P("@id", id), P("@m", minutes), P("@ip", currentIP), P("@touch", refreshActivity));
        }

        public void EndSession(Guid record, byte[] hash)
        {
            using (var c = new SqlConnection(cs))
            {
                c.Open();
                using (var q = Cmd(c, null, "UPDATE dbo.AppUserSessions SET EndedAtUtc=SYSUTCDATETIME(),EndReason='Signed out' WHERE SessionRecordID=@r AND TokenHash=@h AND EndedAtUtc IS NULL", P("@r", record), P("@h", hash)))
                    q.ExecuteNonQuery();
            }
        }
    }
}
