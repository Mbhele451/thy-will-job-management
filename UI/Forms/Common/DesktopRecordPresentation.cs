using System;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Major_Project.UI.Forms.Common
{
    internal static class DesktopRecordPresentation
    {
        internal static decimal CartTotal(DataTable cart)
        {
            if (!cart.Columns.Contains("GrandTotal"))
                cart.Columns.Add("GrandTotal", typeof(decimal));
            decimal total = 0;
            foreach (DataRow row in cart.Rows)
            {
                if (row.RowState == DataRowState.Deleted)
                    continue;
                decimal subtotal = Money(row, "Quantity") * (Money(row, "BaseLabourCost") + Money(row, "BaseTransportCost") + Money(row, "MaterialCost"));
                decimal line = subtotal + subtotal * Money(row, "TaxPercentage") / 100m;
                row["GrandTotal"] = line;
                total += line;
            }

            return total;
        }

        internal static bool HasTechnician(ComboBox combo) => int.TryParse(Convert.ToString(combo.SelectedValue), out int id) && id > 0;
        internal static void BindTechnicians(ComboBox combo, DataTable technicians, Label availability)
        {
            int.TryParse(Convert.ToString(combo.SelectedValue), out int previous);
            var options = technicians.Copy();
            var placeholder = options.NewRow();
            placeholder["StaffID"] = 0;
            placeholder["FullName"] = technicians.Rows.Count == 0 ? "No technicians available — choose another date" : "Select a technician";
            if (options.Columns.Contains("CompletedJobs"))
                placeholder["CompletedJobs"] = 0;
            options.Rows.InsertAt(placeholder, 0);
            combo.DataSource = null;
            combo.DisplayMember = "FullName";
            combo.ValueMember = "StaffID";
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.DropDownWidth = Math.Max(combo.Width, 420);
            combo.DataSource = options;
            combo.SelectedIndex = 0;
            foreach (DataRow row in technicians.Rows)
                if (Convert.ToInt32(row["StaffID"]) == previous)
                {
                    combo.SelectedValue = previous;
                    break;
                }

            availability.Text = technicians.Rows.Count == 0 ? "No active, available technicians for this date." : technicians.Rows.Count + " technician(s) available for this date.";
        }

        internal static string StaffSummary(DataRow row)
        {
            if (row == null)
                return "Select a staff member to view their profile.";
            return "STAFF PROFILE\r\n\r\n" + "Staff #: " + Text(row, "StaffID") + "\r\nName: " + Text(row, "FullName") + "\r\nRole: " + Text(row, "RoleName") + "\r\nUsername: " + Text(row, "Username") + "\r\nEmail: " + Text(row, "EmailAddress") + "\r\nPhone: " + Text(row, "PhoneNumber") + "\r\nEmployment: " + Text(row, "EmploymentStatus") + "\r\nAvailability: " + Availability(row) + "\r\nHire date: " + Date(row, "HireDate") + "\r\nSalary: " + Money(row, "Salary").ToString("C2", GridPresentation.Rand);
        }

        internal static string Availability(DataRow row)
        {
            if (Text(row, "RoleName") != "Technician")
                return "Not applicable";
            if (Text(row, "EmploymentStatus") != "Active")
                return "Inactive";
            return row.Table.Columns.Contains("IsAvailable") && row["IsAvailable"] != DBNull.Value && Convert.ToBoolean(row["IsAvailable"]) ? "Available" : "Unavailable";
        }

        internal static string JobDetails(DataRow row)
        {
            if (row == null)
                return "Select a job from Jobs to view its full details.";
            var text = new StringBuilder();
            text.AppendLine("JOB #" + Text(row, "JobID"));
            text.AppendLine("Status: " + Text(row, "JobStatus") + "    Payment: " + Text(row, "PaymentStatus"));
            text.AppendLine("Promise date: " + Date(row, "PromiseDate"));
            text.AppendLine("Created: " + Date(row, "CreatedAt"));
            text.AppendLine().AppendLine("CUSTOMER");
            foreach (string field in new[]
            {
                "CustomerName",
                "PhoneNumber",
                "EmailAddress",
                "FullAddress",
                "ServiceArea"
            }

            )
                text.AppendLine(Label(field) + ": " + Text(row, field));
            text.AppendLine().AppendLine("TECHNICIAN").AppendLine(Text(row, "TechnicianName"));
            text.AppendLine().AppendLine("SERVICES").AppendLine(Text(row, "Services"));
            text.AppendLine().AppendLine("COST BREAKDOWN");
            foreach (string field in new[]
            {
                "LabourCost",
                "TotalMaterialCost",
                "TransportCost",
                "TaxAmount",
                "GrandTotal"
            }

            )
                text.AppendLine(Label(field) + ": " + Money(row, field).ToString("C2", GridPresentation.Rand));
            text.AppendLine().AppendLine("WORK DESCRIPTION / NOTES").AppendLine(Text(row, "Notes"));
            return text.ToString();
        }

        private static string Label(string field)
        {
            switch (field)
            {
                case "CustomerName":
                    return "Name";
                case "PhoneNumber":
                    return "Phone";
                case "EmailAddress":
                    return "Email";
                case "FullAddress":
                    return "Address";
                case "ServiceArea":
                    return "Service area";
                case "LabourCost":
                    return "Labour";
                case "TotalMaterialCost":
                    return "Materials";
                case "TransportCost":
                    return "Transport";
                case "TaxAmount":
                    return "VAT";
                case "GrandTotal":
                    return "Grand total";
                default:
                    return field;
            }
        }

        private static string Text(DataRow row, string column) => row.Table.Columns.Contains(column) && row[column] != DBNull.Value ? Convert.ToString(row[column]) : "—";
        private static decimal Money(DataRow row, string column) => row.Table.Columns.Contains(column) && row[column] != DBNull.Value ? Convert.ToDecimal(row[column]) : 0m;
        private static string Date(DataRow row, string column) => row.Table.Columns.Contains(column) && row[column] != DBNull.Value ? Convert.ToDateTime(row[column]).ToString("dd MMM yyyy") : "—";
    }
}
