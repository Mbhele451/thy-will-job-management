using System;
using System.Data;
using System.Text;

namespace MajorProject.Web.Helpers
{
    public static class CsvReportWriter
    {
        public static string Field(object value)
        {
            string s = value == null || value == DBNull.Value ? "" : value is DateTime ? ((DateTime)value).ToString("o", System.Globalization.CultureInfo.InvariantCulture) : Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
            string trim = s.TrimStart();
            if (trim.Length > 0 && "=+-@".IndexOf(trim[0]) >= 0)
                s = "'" + s;
            return "\"" + s.Replace("\"", "\"\"") + "\"";
        }

        public static string Write(DataTable data)
        {
            var b = new StringBuilder();
            for (int i = 0; i < data.Columns.Count; i++)
            {
                if (i > 0)
                    b.Append(',');
                b.Append(Field(data.Columns[i].ColumnName));
            }

            b.Append("\r\n");
            foreach (DataRow row in data.Rows)
            {
                for (int i = 0; i < data.Columns.Count; i++)
                {
                    if (i > 0)
                        b.Append(',');
                    b.Append(Field(row[i]));
                }

                b.Append("\r\n");
            }

            return b.ToString();
        }
    }
}
