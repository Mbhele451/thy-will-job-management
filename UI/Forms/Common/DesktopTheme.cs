using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Major_Project.UI.Forms.Common
{
    internal static class DesktopTheme
    {
        internal static readonly Color Background = Color.FromArgb(237, 243, 252);
        internal static readonly Color Surface = Color.FromArgb(249, 251, 255);
        internal static readonly Color Raised = Color.FromArgb(228, 237, 249);
        internal static readonly Color Border = Color.FromArgb(200, 214, 232);
        internal static readonly Color Ink = Color.FromArgb(23, 41, 65);
        internal static readonly Color Muted = Color.FromArgb(70, 94, 121);
        internal static readonly Color Primary = Color.FromArgb(40, 91, 156);
        internal static readonly Color Selected = Color.FromArgb(224, 234, 250);
        private static readonly Color PrimaryHover = Color.FromArgb(32, 75, 130);
        internal static readonly Font BodyFont = new Font("Segoe UI", 11F);
        private static readonly Font StrongFont = new Font("Segoe UI", 11F, FontStyle.Bold);

        internal static void StyleButton(Button button, bool primary)
        {
            bool danger = button.Text.StartsWith("Archive", StringComparison.OrdinalIgnoreCase) || button.Text.StartsWith("Delete", StringComparison.OrdinalIgnoreCase) || button.Text.StartsWith("Remove", StringComparison.OrdinalIgnoreCase) || button.Text.Equals("Cancel Job", StringComparison.OrdinalIgnoreCase) || button.Text == "Activate / Deactivate" || button.Text == "Block / Unblock";
            button.UseVisualStyleBackColor = false;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderColor = primary ? Primary : Border;
            button.FlatAppearance.MouseOverBackColor = primary ? PrimaryHover : Selected;
            button.FlatAppearance.MouseDownBackColor = button.FlatAppearance.MouseOverBackColor;
            button.BackColor = primary ? Primary : Surface;
            button.ForeColor = primary ? Color.White : Ink;
            if (danger)
            {
                button.BackColor = Color.FromArgb(185, 50, 66);
                button.ForeColor = Color.White;
                button.FlatAppearance.BorderColor = button.BackColor;
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(150, 38, 52);
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(150, 38, 52);
            }
        }

        internal static void Apply(Control control)
        {
            control.ForeColor = Ink;
            if (control is Form || control is TabPage || control is Panel)
                control.BackColor = Background;
            else if (control is GroupBox || control is TextBoxBase || control is ComboBox || control is ListControl)
                control.BackColor = Surface;
            else if (control is Label || control is CheckBox || control is RadioButton)
                control.BackColor = Color.Transparent;
            if (control is Button button)
            {
                string caption = button.Text.Trim();
                bool secondary = caption.StartsWith("Cancel", StringComparison.OrdinalIgnoreCase) || caption.StartsWith("Clear", StringComparison.OrdinalIgnoreCase) || caption.StartsWith("Help", StringComparison.OrdinalIgnoreCase) || caption.Replace(" ", "").Equals("Logout", StringComparison.OrdinalIgnoreCase);
                StyleButton(button, !secondary);
                if (caption.Replace(" ", "").Equals("Logout", StringComparison.OrdinalIgnoreCase))
                {
                    button.BackgroundImage = null;
                    button.Image = null;
                    button.Text = "Sign Out";
                }
            }
            else if (control is LinkLabel link)
            {
                link.LinkColor = Primary;
                link.ActiveLinkColor = PrimaryHover;
                link.VisitedLinkColor = Primary;
            }
            else if (control is DataGridView grid)
            {
                GridPresentation.StyleGrid(grid);
                grid.BackgroundColor = Surface;
                grid.BorderStyle = BorderStyle.FixedSingle;
                grid.GridColor = Border;
                grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                grid.EnableHeadersVisualStyles = false;
                grid.ColumnHeadersDefaultCellStyle.BackColor = Raised;
                grid.ColumnHeadersDefaultCellStyle.ForeColor = Ink;
                grid.ColumnHeadersDefaultCellStyle.Font = StrongFont;
                grid.DefaultCellStyle.BackColor = Surface;
                grid.DefaultCellStyle.ForeColor = Ink;
                grid.DefaultCellStyle.SelectionBackColor = Primary;
                grid.DefaultCellStyle.SelectionForeColor = Color.White;
                grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 252);
            }

            foreach (Control child in control.Controls)
                Apply(child);
        }
    }

    public sealed class WorkspaceTabs : TabControl
    {
        internal void HideHeaders()
        {
            _hideHeaders = true;
            Appearance = TabAppearance.FlatButtons;
            SizeMode = TabSizeMode.Fixed;
            ItemSize = new Size(1, 1);
            TabStop = false;
            UpdateStyles();
        }

        private bool _hideHeaders;
        protected override void WndProc(ref Message message)
        {
            const int AdjustTabDisplayRectangle = 0x1328;
            if (_hideHeaders && message.Msg == AdjustTabDisplayRectangle)
            {
                message.Result = IntPtr.Zero;
                return;
            }

            base.WndProc(ref message);
        }
    }
}
