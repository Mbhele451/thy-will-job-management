using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Major_Project.Utilities
{
    public class HelpInfo
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public HelpInfo(string title, string description)
        {
            Title = title;
            Description = description;
        }
    }

    public class HelpManager : IDisposable
    {
        private readonly ToolTip _toolTip;
        private readonly Dictionary<Control, HelpInfo> _registry = new Dictionary<Control, HelpInfo>();
        private bool _disposed;
        public Control ActiveControl { get; private set; }

        public HelpManager()
        {
            _toolTip = new ToolTip
            {
                ToolTipIcon = ToolTipIcon.Info,
                IsBalloon = true,
                AutoPopDelay = 5000,
                InitialDelay = 500,
                ReshowDelay = 100
            };
        }

        /// <summary>
        /// Registers a control with help metadata.
        /// Also hooks the Disposed event to clean up.
        /// </summary>
        public void RegisterControl(Control control, string title, string description)
        {
            if (control == null || control.IsDisposed)
                return;
            var info = new HelpInfo(title, description);
            _registry[control] = info;
            // Set hover tooltip
            _toolTip.SetToolTip(control, $"{title}: {description}");
            control.MouseDown += (sender, e) => ActiveControl = control;
            // Track focus changes
            control.Enter += (sender, e) => ActiveControl = control;
            // Clean up when the control is disposed
            control.Disposed += (sender, e) =>
            {
                if (_registry.ContainsKey(control))
                    _registry.Remove(control);
                if (ActiveControl == control)
                    ActiveControl = null;
            };
        }

        /// <summary>
        /// Registers multiple controls at once.
        /// </summary>
        public void RegisterControlSection(string sectionName, params (Control control, string title, string description)[] controls)
        {
            foreach (var item in controls)
            {
                RegisterControl(item.control, item.title, item.description);
            }
        }

        /// <summary>
        /// Shows help for the currently focused control, or a fallback if none.
        /// </summary>
        public void ShowHelpForActiveControl(Control fallbackControl = null)
        {
            Control target = ActiveControl != null && ActiveControl.Visible ? ActiveControl : fallbackControl;
            if (target == null || target.IsDisposed)
            {
                UIHelper.ShowInfo("Select a field to view its help.");
                return;
            }

            ShowHelpForControl(target);
        }

        /// <summary>
        /// Shows help for a specific control.
        /// </summary>
        public void ShowHelpForControl(Control control)
        {
            if (control == null || control.IsDisposed)
                return;
            if (_registry.TryGetValue(control, out HelpInfo info))
            {
                Major_Project.UI.Forms.Common.WorkflowUi.Notify(control, info.Title + " — " + info.Description);
            }
            else
            {
                _toolTip.ToolTipTitle = "Help";
                _toolTip.Show("No help available for this control.", control, 0, -40, 3000);
            }
        }

        /// <summary>
        /// Alias for ShowHelpForControl – kept for compatibility with ResetPasswordForm.
        /// </summary>
        public void DisplayHelpToolTip(Control helpButton, Control targetControl)
        {
            ShowHelpForActiveControl(targetControl);
        }

        /// <summary>
        /// Attaches a help button to show help for the active control (or a default).
        /// </summary>
        public void AttachHelpButton(Button helpButton, Control defaultControl = null)
        {
            if (helpButton == null)
                return;
            helpButton.Click += (sender, e) => ShowHelpForActiveControl(defaultControl);
        }

        /// <summary>
        /// Registers all controls in a container (form or panel) – use with caution as it registers all children.
        /// </summary>
        public void RegisterContainer(Control container)
        {
            if (container == null)
                return;
            foreach (Control ctrl in container.Controls)
            {
                if (_registry.ContainsKey(ctrl))
                    continue;
                if (ctrl is Button button)
                {
                    if (!string.Equals(button.Text.Trim(), "Help", StringComparison.OrdinalIgnoreCase))
                        RegisterControl(button, button.Text.Trim(), button.AccessibleDescription ?? "Choose " + button.Text.Trim() + " for the current record.");
                    continue;
                }

                if (ctrl is RadioButton || ctrl is CheckBox)
                {
                    RegisterControl(ctrl, ctrl.Text, "Select this option for the current record.");
                    continue;
                }

                if (ctrl is TextBoxBase || ctrl is ComboBox || ctrl is NumericUpDown || ctrl is DateTimePicker || ctrl is DataGridView)
                {
                    string caption = ctrl.AccessibleName;
                    if (string.IsNullOrWhiteSpace(caption) && container is TableLayoutPanel table)
                    {
                        var position = table.GetPositionFromControl(ctrl);
                        if (position.Column > 0 && table.GetControlFromPosition(position.Column - 1, position.Row)is Label label)
                            caption = label.Text;
                    }

                    if (string.IsNullOrWhiteSpace(caption))
                        caption = container is GroupBox ? container.Text : "Selected field";
                    string description = ctrl is DataGridView ? "Select a row to work with that record." : ctrl is ComboBox ? "Choose a value from the list." : ctrl is DateTimePicker ? "Choose a calendar date." : ctrl is NumericUpDown ? "Type a quantity or use the arrows to adjust it." : ((TextBoxBase)ctrl).ReadOnly ? "Displays the value for the selected record." : "Enter the value for this field.";
                    RegisterControl(ctrl, caption, ctrl.AccessibleDescription ?? description);
                    continue;
                }

                if (ctrl.HasChildren)
                    RegisterContainer(ctrl);
            }
        }

#region IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _toolTip?.Dispose();
                    _registry.Clear();
                }

                _disposed = true;
            }
        }
#endregion
    }
}
