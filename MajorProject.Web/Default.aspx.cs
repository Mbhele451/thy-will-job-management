using System;
using MajorProject.Business.Helpers;
using MajorProject.Web.Helpers;

namespace MajorProject.Web
{
    /// <summary>
    /// THY WILL public landing page.
    ///
    /// This page is intentionally kept lightweight.
    ///
    /// CUSTOMER FLOW:
    ///
    /// New customer:
    ///     Landing Page
    ///          ↓
    ///     Create Your Profile
    ///          ↓
    ///     Register.aspx
    ///          ↓
    ///     Login.aspx
    ///          ↓
    ///     Customer Dashboard
    ///          ↓
    ///     Create Service Request / Job
    ///
    /// Existing customer:
    ///     Landing Page
    ///          ↓
    ///     Login.aspx
    ///          ↓
    ///     Customer Dashboard
    ///
    /// Staff:
    ///     Landing Page
    ///          ↓
    ///     Login.aspx
    ///          ↓
    ///     Role-based Dashboard
    ///
    /// The landing page itself does NOT:
    /// - create customers
    /// - create jobs
    /// - submit service requests
    /// - check customer emails
    /// - calculate job estimates
    /// - send booking emails
    ///
    /// Those operations belong to the appropriate application pages.
    /// </summary>
    public partial class _Default : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitialisePage();
            }
        }

        /// <summary>
        /// Performs initial setup for the landing page.
        ///
        /// At the moment the landing page is primarily
        /// presentation-based, so there is no database
        /// or booking initialisation required here.
        /// </summary>
        private void InitialisePage()
        {
            try
            {
            /*
                 * Landing page intentionally has no database
                 * initialisation.
                 *
                 * Keep this method available for future
                 * presentation-related setup if required.
                 */
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to initialise THY WILL landing page.", ex);
            }
        }
    }
}
