using System.Windows.Forms;

namespace Major_Project.UI.Forms.Common
{
    public partial class ProfileDetailsView : UserControl
    {
        public ProfileDetailsView()
        {
            InitializeComponent();
        }

        internal void ShowProfile(System.Data.DataRow row)
        {
            txtProfileDetails.Text = DesktopRecordPresentation.StaffSummary(row);
        }

        internal void ShowRecord(System.Data.DataRow row)
        {
            var lines = new System.Collections.Generic.List<string>();
            foreach (System.Data.DataColumn column in row.Table.Columns)
                lines.Add(column.Caption + ": " + System.Convert.ToString(row[column]));
            txtProfileDetails.Lines = lines.ToArray();
        }
    }
}
