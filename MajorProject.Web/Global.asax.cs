using System;
using System.Configuration;
using System.Web;
using System.Web.UI;
using MajorProject.Business.Helpers;
using MajorProject.Business.Services;

namespace MajorProject.Web
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            ScriptManager.ScriptResourceMapping.AddDefinition("jquery", new ScriptResourceDefinition { Path = "~/Scripts/jquery-3.7.1.min.js" });
            try
            {
                EmailQueueService.Initialize(ConfigurationManager.AppSettings["MailjetApiKey"], ConfigurationManager.AppSettings["MailjetApiSecret"], ConfigurationManager.AppSettings["MailjetFromEmail"], ConfigurationManager.AppSettings["MailjetFromName"], maxRetries: 3, retryDelaySeconds: 30, batchSize: 10, bookingConnectionString: ConfigurationManager.ConnectionStrings["GroupWst34ConnectionString"].ConnectionString);
                Logger.Info("EmailQueueService initialized successfully on Application_Start.");
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to initialize EmailQueueService on Application_Start", ex);
            // Don't crash the app – email will fail gracefully
            }
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            if (Context.Session == null || !MajorProject.Web.Helpers.WebSession.IsAuthenticated)
                return;
            bool valid = false, validationFailed = false;
            try
            {
                valid = MajorProject.Web.Helpers.SecurityGateway.Check();
            }
            catch (Exception ex)
            {
                validationFailed = true;
                Logger.Error("Validating authenticated session security.", ex);
            }

            if (validationFailed)
            {
                WriteUnavailable();
                Response.End();
                return;
            }

            if (!valid)
            {
                if (!validationFailed)
                    try
                    {
                        MajorProject.Web.Helpers.SecurityGateway.EndCurrent();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Ending a rejected session.", ex);
                    }

                System.Web.Security.FormsAuthentication.SignOut();
                Context.Session.Clear();
                Context.Session.Abandon();
                Response.Redirect(VirtualPathUtility.ToAbsolute("~/Account/Login.aspx?session=ended"), true);
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        // Initialize session variables if needed
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            Logger.Error("Unhandled application error", ex);
            for (Exception cause = ex; cause != null; cause = cause.InnerException)
                if (cause is System.Data.SqlClient.SqlException || cause is TimeoutException)
                {
                    Server.ClearError();
                    WriteUnavailable();
                    return;
                }
        }

        private void WriteUnavailable()
        {
            Response.Clear();
            Response.StatusCode = 503;
            Response.TrySkipIisCustomErrors = true;
            Response.ContentType = "text/html";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Write("<!doctype html><html lang='en'><meta charset='utf-8'><meta name='viewport' content='width=device-width,initial-scale=1'><title>THY WILL | Temporarily unavailable</title><body style='margin:0;background:#0b1728;color:#f0f5ff;font:18px/1.6 system-ui;padding:10vh 8vw'><main style='max-width:650px'><p>THY WILL</p><h1>We cannot reach the database.</h1><p>Access cannot be verified right now. Check your network or VPN and try again shortly.</p><p>If you were saving a change, check its status before submitting again.</p><a style='color:#7ee4ce' href='" + VirtualPathUtility.ToAbsolute("~/Account/Login.aspx") + "'>Return to sign in</a></main></body></html>");
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
