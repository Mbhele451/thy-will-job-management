using System;
using System.Web;

namespace MajorProject.Web.Helpers
{
    public static class SecurityHelper
    {
        public static void RegenerateSession()
        {
            HttpContext context = HttpContext.Current;
            if (context == null)
                return;
            if (context.Session != null)
            {
                context.Session.Clear();
                context.Session.RemoveAll();
                context.Session.Abandon();
            }
        }

        public static void PreventCaching()
        {
            HttpContext context = HttpContext.Current;
            if (context == null)
                return;
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoStore();
            context.Response.Cache.SetExpires(DateTime.UtcNow.AddYears(-1));
            context.Response.Cache.AppendCacheExtension("must-revalidate, proxy-revalidate");
        }
    }
}
