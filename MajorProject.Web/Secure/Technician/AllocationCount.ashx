<%@ WebHandler Language="C#" Class="AllocationCount" %>
using System;
using System.Web;
using System.Web.SessionState;
using MajorProject.Web.Helpers;
using MajorProject.Business.Services;

public class AllocationCount : IHttpHandler, IReadOnlySessionState
{
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

    public void ProcessRequest(HttpContext c)
    {
        c.Response.ContentType = "application/json";
        c.Response.Cache.SetNoStore();
        if (!WebSession.IsAuthenticated || !WebSession.HasRole("Technician"))
        {
            c.Response.StatusCode = 403;
            c.Response.Write("{}");
            return;
        }

        try
        {
            var cs = System.Configuration.ConfigurationManager.ConnectionStrings["GroupWst34ConnectionString"].ConnectionString;
            int count = new JobChangeService(cs).Notifications(WebSession.StaffID).Rows.Count;
            c.Response.Write("{\"count\":" + count + "}");
        }
        catch (Exception ex)
        {
            MajorProject.Business.Helpers.Logger.Error("Allocation notification check failed.", ex);
            c.Response.StatusCode = 503;
            c.Response.TrySkipIisCustomErrors = true;
            c.Response.Write("{\"unavailable\":true}");
        }
    }
}
