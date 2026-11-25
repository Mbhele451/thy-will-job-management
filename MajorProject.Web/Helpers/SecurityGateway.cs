using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using MajorProject.Business.Services;
using MajorProject.Data.Data;

namespace MajorProject.Web.Helpers
{
    public static class SecurityGateway
    {
        private static string CS
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["GroupWst34ConnectionString"].ConnectionString;
            }
        }

        public static string IP
        {
            get
            {
                var value = HttpContext.Current.Request.UserHostAddress ?? "unknown";
                return value.Length > 45 ? value.Substring(0, 45) : value;
            }
        }

        private static byte[] Hash(string token)
        {
            using (var sha = SHA256.Create())
                return sha.ComputeHash(Encoding.UTF8.GetBytes(token));
        }

        public static bool Check()
        {
            var c = HttpContext.Current;
            if (c == null || c.Session == null || !WebSession.IsAuthenticated)
                return true;
            bool staff = WebSession.StaffID > 0;
            int id = staff ? WebSession.StaffID : WebSession.CustomerID;
            string expected = staff ? id.ToString(System.Globalization.CultureInfo.InvariantCulture) : "Customer-" + id.ToString(System.Globalization.CultureInfo.InvariantCulture);
            if (c.User == null || c.User.Identity == null || !c.User.Identity.IsAuthenticated || c.User.Identity.Name != expected)
                return false;
            var data = new SystemAdminDataAccess(CS);
            int minutes = int.Parse(new SystemAdminService(CS).Setting("SessionIdleMinutes", "20"));
            c.Session.Timeout = minutes;
            Guid record;
            string token = Convert.ToString(c.Session["SecurityToken"]);
            if (!Guid.TryParse(Convert.ToString(c.Session["SecurityRecord"]), out record) || token.Length == 0)
            {
                record = Guid.NewGuid();
                var bytes = new byte[32];
                using (var rng = RandomNumberGenerator.Create())
                    rng.GetBytes(bytes);
                token = Convert.ToBase64String(bytes);
                var agent = c.Request.UserAgent ?? "Unknown browser";
                if (agent.Length > 256)
                    agent = agent.Substring(0, 256);
                data.StartSession(staff, id, record, Hash(token), IP, agent, "Web", minutes);
                c.Session["SecurityRecord"] = record.ToString();
                c.Session["SecurityToken"] = token;
            }

            bool polling = c.Request.AppRelativeCurrentExecutionFilePath.Equals("~/Secure/Technician/AllocationCount.ashx", StringComparison.OrdinalIgnoreCase);
            var table = data.ValidateSession(staff, id, record, Hash(token), minutes, IP, !polling);
            return table.Rows.Count == 1 && string.Equals(Convert.ToString(table.Rows[0]["RoleName"]), WebSession.Role, StringComparison.OrdinalIgnoreCase);
        }

        public static void EndCurrent()
        {
            var c = HttpContext.Current;
            Guid id;
            if (c == null || c.Session == null || !Guid.TryParse(Convert.ToString(c.Session["SecurityRecord"]), out id))
                return;
            var token = Convert.ToString(c.Session["SecurityToken"]);
            if (token.Length > 0)
                new SystemAdminDataAccess(CS).EndSession(id, Hash(token));
        }

        public static string CompanyName
        {
            get
            {
                try
                {
                    return Setting("CompanyDisplayName", "THY WILL");
                }
                catch
                {
                    return "THY WILL";
                }
            }
        }

        public static string Setting(string key, string fallback)
        {
            string cacheKey = "AppSetting:" + key;
            var cached = HttpRuntime.Cache[cacheKey] as string;
            if (cached != null)
                return cached;
            string value = new SystemAdminService(CS).Setting(key, fallback);
            HttpRuntime.Cache.Insert(cacheKey, value, null, DateTime.UtcNow.AddSeconds(30), System.Web.Caching.Cache.NoSlidingExpiration);
            return value;
        }
    }

    public abstract class SysAdminBasePage : BasePage
    {
        protected override void OnInit(EventArgs e)
        {
            AccessControl.RequireRole("SysAdmin");
            base.OnInit(e);
        }
    }
}
