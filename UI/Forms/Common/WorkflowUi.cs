using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Major_Project.Core;
using MajorProject.Business.Helpers;

namespace Major_Project.UI.Forms.Common
{
    internal static class WorkflowUi
    {
        internal static string ConnectionString => Properties.Settings.Default.GroupWst34ConnectionString;

        internal static Button BindButton(Button button, Action action, bool primary = false)
        {
            var parent = button.Parent;
            string caption = button.Text;
            DesktopTheme.StyleButton(button, primary);
            button.Click += (sender, args) => action();
            // Existing directory pages share selection eligibility with their grid.
            var grid = parent.Parent == null ? null : FindActionGrid(parent.Parent);
            bool selectionAction = caption.StartsWith("Edit ", StringComparison.OrdinalIgnoreCase) || caption.StartsWith("Save Invoice", StringComparison.OrdinalIgnoreCase) || caption.StartsWith("Save Receipt", StringComparison.OrdinalIgnoreCase) || caption == "Activate / Deactivate" || caption == "Manage Materials" || caption == "Update Quantity" || caption == "Remove Material" || caption == "Block / Unblock" || caption == "Revoke Session" || caption == "Set Employment Status" || caption == "Set Availability" || caption == "Reset Staff Password" || caption == "Pay Invoice" || caption == "Job Details" || caption == "Set Quantity" || caption == "Remove";
            if (grid != null && selectionAction)
            {
                void UpdateSelection()
                {
                    object selected = grid.CurrentRow?.DataBoundItem;
                    button.Enabled = selected is DataRowView view ? SelectionEligible(caption, view.Row) : selected != null && (caption == "Set Quantity" || caption == "Remove");
                }

                grid.SelectionChanged += (sender, args) => UpdateSelection();
                grid.DataBindingComplete += (sender, args) => UpdateSelection();
                UpdateSelection();
            }

            return button;
        }

        private static DataGridView FindActionGrid(Control parent)
        {
            var direct = parent.Controls.OfType<DataGridView>().FirstOrDefault();
            if (direct != null)
                return direct;
            foreach (Control child in parent.Controls)
            {
                var grid = FindActionGrid(child);
                if (grid != null)
                    return grid;
            }

            return null;
        }

        internal static bool SelectionEligible(string caption, DataRow row)
        {
            if (row == null || row.RowState == DataRowState.Deleted)
                return false;
            string Value(string column) => row.Table.Columns.Contains(column) ? Convert.ToString(row[column]).Trim() : "";
            string status = Value("JobStatus"), payment = Value("PaymentStatus"), role = Value("RoleName");
            if (caption == "Pay Invoice" || caption == "Save Invoice PDF" || caption == "Save Receipt PDF")
            {
                bool invoice = Value("InvoiceFilePath").Length > 0 || Value("HasInvoicePdf") == "True";
                bool completed = status == "Completed" || status == "Archived";
                if (caption == "Save Invoice PDF")
                    return completed && invoice;
                if (caption == "Save Receipt PDF")
                    return completed && invoice && payment.Equals("Paid", StringComparison.OrdinalIgnoreCase);
                return status == "Completed" && invoice && decimal.TryParse(Value("GrandTotal"), out decimal total) && total > 0 && (payment.Length == 0 || payment.Equals("Pending", StringComparison.OrdinalIgnoreCase) || payment.Equals("Unpaid", StringComparison.OrdinalIgnoreCase));
            }

            if (caption == "Set Availability")
                return role == "Technician" && Value("EmploymentStatus") == "Active";
            if (caption == "Edit Selected Staff" || caption == "Set Employment Status" || caption == "Reset Staff Password")
                return role != "SysAdmin";
            return true;
        }

        internal static DataRow Selected(DataGridView grid)
        {
            if (!(grid.CurrentRow?.DataBoundItem is DataRowView view))
                throw new ArgumentException("Select a row first.");
            return view.Row;
        }

        internal static bool Run(Control owner, Action work, bool authenticated = true)
        {
            if (owner.IsDisposed || (authenticated && !UserSession.Check()))
                return false;
            try
            {
                work();
                return true;
            }
            catch (Exception ex)
            {
                Report(owner, ex);
            }

            return false;
        }

        internal static async Task RunAsync(Control owner, Func<Task> work, bool authenticated = true)
        {
            if (owner.IsDisposed || (authenticated && !UserSession.Check()))
                return;
            try
            {
                await work();
            }
            catch (Exception ex)
            {
                Report(owner, ex);
            }
        }

        private static void Report(Control owner, Exception ex)
        {
            if (ex is ArgumentException || ex is InvalidOperationException || (ex is System.Data.SqlClient.SqlException sql && sql.Number >= 51000 && sql.Number < 52000))
            {
                ShowError(owner, ex.Message);
                return;
            }

            Logger.Error("Desktop workflow failed.", ex);
            ShowError(owner, "The operation could not be completed. Please try again.");
        }

        internal static void ShowError(Control owner, string message) => Feedback(owner, message, true);
        internal static void Notify(Control owner, string message) => Feedback(owner, message, false);
        internal static void Warn(Control owner, string message) => Feedback(owner, message, false, true);
        internal static Control ActiveSurface()
        {
            Control control = Form.ActiveForm;
            while (control is ContainerControl container && container.ActiveControl != null)
                control = container.ActiveControl;
            return control;
        }

        internal static Control Surface(Control owner)
        {
            for (Control current = owner; current != null; current = current.Parent)
            {
                if (current is TabPage)
                    return current;
                if (current is Form)
                    return FindSelectedPage(current) ?? current;
            }

            return owner;
        }

        private static Control FindSelectedPage(Control parent)
        {
            if (parent is WorkspaceTabs tabs && tabs.Visible)
                return tabs.SelectedTab;
            foreach (Control child in parent.Controls)
            {
                if (!child.Visible)
                    continue;
                Control selected = FindSelectedPage(child);
                if (selected != null)
                    return selected;
            }

            return null;
        }

        private static void Feedback(Control owner, string message, bool error, bool warning = false)
        {
            owner = owner ?? ActiveSurface();
            if (owner == null || owner.IsDisposed)
                return;
            if (owner.InvokeRequired)
            {
                owner.BeginInvoke(new Action(() => Feedback(owner, message, error, warning)));
                return;
            }

            Control surface = Surface(owner);
            var inline = surface.Controls.OfType<WorkflowPanel>().FirstOrDefault(panel => panel.Visible);
            if (inline != null)
                surface = inline;
            var fixedFeedback = surface.Controls.Find("InlineFeedback", true).OfType<Label>().FirstOrDefault(candidate => candidate.Visible);
            if (fixedFeedback != null)
            {
                fixedFeedback.ForeColor = error ? Color.Maroon : DesktopTheme.Ink;
                fixedFeedback.Text = message;
                return;
            }

            var label = surface is WorkflowPanel workflow
                ? workflow.FeedbackLabel
                : surface.Controls["WorkflowFeedback"] as Label;
            if (label == null)
                return;
            label.Text = message;
            label.Height = Math.Max(46, TextRenderer.MeasureText(message, label.Font, new Size(Math.Max(200, surface.ClientSize.Width - 28), int.MaxValue), TextFormatFlags.WordBreak).Height + 12);
            label.ForeColor = error ? Color.Maroon : warning ? Color.FromArgb(146, 85, 9) : DesktopTheme.Ink;
            label.BackColor = error ? Color.MistyRose : warning ? Color.FromArgb(255, 242, 219) : DesktopTheme.Selected;
            label.Show();

        }

        internal static async Task<bool> ConfirmAsync(Control owner, string message, string title = "Confirm")
        {
            using (var panel = new WorkflowPanel(title))
            {
                panel.Note(message);
                panel.Submit("Confirm", () =>
                {
                });
                return await panel.OpenAsync(owner);
            }
        }

        internal static async Task SaveAsync(Control owner, string name, Action<Stream> write)
        {
            using (var panel = new WorkflowPanel("Save " + name, new FilePathEditor()))
            {
                var file = panel.Input("File path", 1024, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", name));
                panel.Submit("Save File", () =>
                {
                    string path = file.Text.Trim();
                    if (!Path.IsPathRooted(path) || !string.Equals(Path.GetExtension(path), Path.GetExtension(name), StringComparison.OrdinalIgnoreCase))
                        throw new ArgumentException("Enter a full file path with the " + Path.GetExtension(name) + " extension.");
                    if (File.Exists(path))
                        throw new ArgumentException("A file with that name already exists. Choose another name.");
                    using (var stream = new FileStream(path, FileMode.CreateNew))
                        write(stream);
                });
                if (await panel.OpenAsync(owner))
                    Notify(owner, "Saved to " + file.Text.Trim());
            }
        }
    }

    internal sealed class WorkflowPanel : Panel
    {
        private readonly WorkflowFrame frame = new WorkflowFrame();
        private readonly FlowLayoutPanel fields;
        private readonly TaskCompletionSource<bool> _completion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        private Button _submit;

        internal WorkflowPanel(string title) : this(title, null)
        {
        }

        internal WorkflowPanel(string title, UserControl editor)
        {
            Name = "InlineWorkflow";
            AccessibleName = title;
            Dock = DockStyle.Fill;
            frame.Dock = DockStyle.Fill;
            Controls.Add(frame);
            frame.Heading.Text = title;
            frame.Notes.Text = "";
            frame.Notes.Hide();
            frame.HelpAction.Hide();
            frame.SubmitAction.Hide();
            frame.Actions.Hide();
            WorkflowUi.BindButton(frame.Back, () => Finish(false));
            if (editor == null)
                return;
            editor.Dock = DockStyle.Fill;
            frame.Body.Controls.Add(editor);
            fields = editor.Controls.OfType<FlowLayoutPanel>().Single();
            foreach (Control control in fields.Controls)
                control.Hide();
        }

        internal Label FeedbackLabel => frame.workflowFeedback;

        internal Panel UseContent() => frame.Body;

        internal void AddComponentHelp(params (Control field, string description)[] entries)
        {
            var help = new Major_Project.Utilities.HelpManager();
            Disposed += (sender, args) => help.Dispose();
            foreach (var entry in entries)
                help.RegisterControl(entry.field, entry.field.AccessibleName, entry.description);
            frame.HelpAction.Show();
            help.AttachHelpButton(frame.HelpAction, entries[0].field);
        }

        internal void Note(string text)
        {
            frame.Notes.Text += (frame.Notes.Text.Length == 0 ? "" : Environment.NewLine) + text;
            frame.Notes.Show();
        }

        internal T Field<T>(string label, Action<T> configure) where T : Control
        {
            T control = fields.Controls.OfType<T>().Single(item => item.AccessibleName == label);
            var caption = fields.Controls[control.Name.Substring(0, control.Name.Length - "Field".Length) + "Label"];
            control.Show();
            caption.Show();
            configure(control);
            void Fit() => control.Width = Math.Max(110, Math.Min(540, fields.ClientSize.Width - 66));
            fields.ClientSizeChanged += (sender, args) => Fit();
            Fit();
            return control;
        }

        internal TextBox Input(string label, int maxLength, string value = "", bool password = false)
        {
            return Field<TextBox>(label, box =>
            {
                box.MaxLength = maxLength;
                box.Text = value;
                box.UseSystemPasswordChar = password;
            });
        }

        internal ComboBox Choice(string label, params string[] choices)
        {
            return Field<ComboBox>(label, box =>
            {
                box.Items.AddRange(choices);
                if (choices.Length > 0)
                    box.SelectedIndex = 0;
            });
        }

        internal Button Submit(string caption, Action work, bool authenticated = true)
        {
            frame.SubmitAction.Text = caption;
            frame.Actions.Show();
            frame.SubmitAction.Show();
            DesktopTheme.StyleButton(frame.SubmitAction, true);
            BindSubmit(frame.SubmitAction, work, authenticated);
            return frame.SubmitAction;
        }

        internal void BindSubmit(Button button, Action work, bool authenticated = true)
        {
            _submit = button;
            button.Click += (sender, args) =>
            {
                if (_completion.Task.IsCompleted)
                    return;
                button.Enabled = false;
                if (WorkflowUi.Run(this, work, authenticated))
                    Finish(true);
                else
                    button.Enabled = true;
            };
        }

        internal async Task<bool> OpenAsync(Control owner)
        {
            var surface = WorkflowUi.Surface(owner);
            if (surface.IsDisposed || !surface.Visible)
                return false;
            var previous = surface.Controls.Cast<Control>().Where(control => control.Visible).ToArray();
            foreach (var control in previous)
                control.Hide();
            surface.Controls.Add(this);
            BringToFront();
            SelectNextControl(null, true, true, true, false);
            try
            {
                return await _completion.Task;
            }
            finally
            {
                if (!surface.IsDisposed && !surface.Disposing)
                {
                    surface.Controls.Remove(this);
                    foreach (var control in previous)
                        if (!control.IsDisposed)
                            control.Show();
                }
            }
        }

        internal void Finish(bool saved)
        {
            if (_completion.Task.IsCompleted)
                return;
            // Disable before publishing completion, so awaiting code cannot remove
            // this view while WinForms is still moving its focus to the next control.
            Enabled = false;
            _completion.TrySetResult(saved);
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                Finish(false);
                return true;
            }

            if (keyData == Keys.Enter && _submit?.CanSelect == true)
            {
                _submit.PerformClick();
                return true;
            }

            return base.ProcessDialogKey(keyData);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Finish(false);
            base.Dispose(disposing);
        }
    }
}
