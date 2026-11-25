using System;
using System.Windows.Forms;
using MajorProject.Business.Models;

namespace Major_Project.UI.Forms.Common
{
    internal sealed class CustomerIdentityFields
    {
        private readonly ComboBox _type;
        private readonly ComboBox _document;
        private readonly TextBox _passport;
        private readonly TextBox _country;
        private readonly TextBox _saId;
        private readonly TextBox _company;
        private readonly Label _saLabel;
        private readonly Label _companyLabel;
        private readonly Label _documentLabel;
        private readonly Label _passportLabel;
        private readonly Label _countryLabel;
        internal CustomerIdentityFields(ComboBox type, TextBox saId, Label saLabel, TextBox company, Label companyLabel, ComboBox document, TextBox passport, TextBox country, Label documentLabel, Label passportLabel, Label countryLabel)
        {
            _type = type;
            _saId = saId;
            _company = company;
            _saLabel = saLabel;
            _companyLabel = companyLabel;
            _document = document;
            _passport = passport;
            _country = country;
            _documentLabel = documentLabel;
            _passportLabel = passportLabel;
            _countryLabel = countryLabel;
            _document.AccessibleName = "Identity document";
            _passport.AccessibleName = "Passport number";
            _country.AccessibleName = "Issuing country";
            _document.SelectedIndex = 0;
            _document.SelectedIndexChanged += (sender, args) => Refresh();
            _type.EnabledChanged += (sender, args) => Refresh();
            Refresh();
        }

        internal string SouthAfricanID => Residential && !PassportSelected ? _saId.Text.Trim() : "";
        internal string Company => Business ? _company.Text.Trim() : "";
        internal string Passport => Residential && PassportSelected ? _passport.Text.Trim() : "";
        internal string Country => Residential && PassportSelected ? _country.Text.Trim() : "";
        private bool Residential => _type.Text == "Residential";
        private bool Business => _type.Text == "Business";
        private bool PassportSelected => _document.SelectedIndex == 1;

        internal void Refresh()
        {
            bool editable = _type.Enabled;
            bool showPassport = Residential && PassportSelected;
            bool showSa = Residential && !PassportSelected;
            _document.Visible = _documentLabel.Visible = Residential;
            _document.Enabled = editable;
            _saId.Visible = _saLabel.Visible = showSa;
            _saId.Enabled = editable;
            _passport.Visible = _passportLabel.Visible = _country.Visible = _countryLabel.Visible = showPassport;
            _passport.Enabled = _country.Enabled = editable;
            _company.Visible = _companyLabel.Visible = Business;
            _company.Enabled = editable;
        }

        internal void Reset()
        {
            _document.SelectedIndex = 0;
            _saId.Clear();
            _company.Clear();
            _passport.Clear();
            _country.Clear();
            Refresh();
        }

        internal void Validate()
        {
            CustomerIdentity.Validate(_type.Text, SouthAfricanID, Company, Passport, Country);
        }
    }
}
