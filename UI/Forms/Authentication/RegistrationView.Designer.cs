namespace Major_Project.UI.Forms.Common {
        partial class RegistrationView {
        private System.ComponentModel.IContainer components = null;
private System.Windows.Forms.Label heading;
private System.Windows.Forms.Label description;
internal System.Windows.Forms.TextBox FullName;
private System.Windows.Forms.Label FullNameLabel;
internal System.Windows.Forms.TextBox Phone;
private System.Windows.Forms.Label PhoneLabel;
internal System.Windows.Forms.TextBox Email;
private System.Windows.Forms.Label EmailLabel;
internal System.Windows.Forms.TextBox Address;
private System.Windows.Forms.Label AddressLabel;
internal System.Windows.Forms.ComboBox Area;
private System.Windows.Forms.Label AreaLabel;
internal System.Windows.Forms.ComboBox IdentityType;
private System.Windows.Forms.Label IdentityTypeLabel;
internal System.Windows.Forms.TextBox Document;
private System.Windows.Forms.Label DocumentLabel;
internal System.Windows.Forms.TextBox Country;
private System.Windows.Forms.Label CountryLabel;
internal System.Windows.Forms.TextBox Password;
private System.Windows.Forms.Label PasswordLabel;
internal System.Windows.Forms.TextBox ConfirmPassword;
private System.Windows.Forms.Label ConfirmPasswordLabel;
internal System.Windows.Forms.Button SubmitButton;
            protected override void Dispose(bool disposing) {
        if(disposing && components != null) components.Dispose();
            base.Dispose(disposing);
           
    } private void InitializeComponent() {
            this.heading = new System.Windows.Forms.Label();
            this.description = new System.Windows.Forms.Label();
            this.FullNameLabel = new System.Windows.Forms.Label();
            this.FullName = new System.Windows.Forms.TextBox();
            this.PhoneLabel = new System.Windows.Forms.Label();
            this.Phone = new System.Windows.Forms.TextBox();
            this.EmailLabel = new System.Windows.Forms.Label();
            this.Email = new System.Windows.Forms.TextBox();
            this.AddressLabel = new System.Windows.Forms.Label();
            this.Address = new System.Windows.Forms.TextBox();
            this.AreaLabel = new System.Windows.Forms.Label();
            this.Area = new System.Windows.Forms.ComboBox();
            this.IdentityTypeLabel = new System.Windows.Forms.Label();
            this.IdentityType = new System.Windows.Forms.ComboBox();
            this.DocumentLabel = new System.Windows.Forms.Label();
            this.Document = new System.Windows.Forms.TextBox();
            this.CountryLabel = new System.Windows.Forms.Label();
            this.Country = new System.Windows.Forms.TextBox();
            this.PasswordLabel = new System.Windows.Forms.Label();
            this.Password = new System.Windows.Forms.TextBox();
            this.ConfirmPasswordLabel = new System.Windows.Forms.Label();
            this.ConfirmPassword = new System.Windows.Forms.TextBox();
            this.SubmitButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // heading
            // 
            this.heading.AutoSize = true;
            this.heading.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Bold);
            this.heading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.heading.Location = new System.Drawing.Point(28, 24);
            this.heading.Name = "heading";
            this.heading.Size = new System.Drawing.Size(301, 36);
            this.heading.TabIndex = 0;
            this.heading.Text = "Create your account";
            // 
            // description
            // 
            this.description.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(94)))), ((int)(((byte)(121)))));
            this.description.Location = new System.Drawing.Point(30, 76);
            this.description.Name = "description";
            this.description.Size = new System.Drawing.Size(690, 44);
            this.description.TabIndex = 1;
            this.description.Text = "Your contact information, service address and account security.";
            // 
            // FullNameLabel
            // 
            this.FullNameLabel.Location = new System.Drawing.Point(30, 140);
            this.FullNameLabel.Name = "FullNameLabel";
            this.FullNameLabel.Size = new System.Drawing.Size(328, 24);
            this.FullNameLabel.TabIndex = 2;
            this.FullNameLabel.Text = "Full name";
            // 
            // FullName
            // 
            this.FullName.AccessibleName = "Full name";
            this.FullName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FullName.Location = new System.Drawing.Point(30, 170);
            this.FullName.Name = "FullName";
            this.FullName.Size = new System.Drawing.Size(320, 23);
            this.FullName.TabIndex = 0;
            // 
            // PhoneLabel
            // 
            this.PhoneLabel.Location = new System.Drawing.Point(380, 140);
            this.PhoneLabel.Name = "PhoneLabel";
            this.PhoneLabel.Size = new System.Drawing.Size(328, 24);
            this.PhoneLabel.TabIndex = 3;
            this.PhoneLabel.Text = "Phone number";
            // 
            // Phone
            // 
            this.Phone.AccessibleName = "Phone number";
            this.Phone.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Phone.Location = new System.Drawing.Point(380, 170);
            this.Phone.Name = "Phone";
            this.Phone.Size = new System.Drawing.Size(320, 23);
            this.Phone.TabIndex = 1;
            // 
            // EmailLabel
            // 
            this.EmailLabel.Location = new System.Drawing.Point(30, 234);
            this.EmailLabel.Name = "EmailLabel";
            this.EmailLabel.Size = new System.Drawing.Size(328, 24);
            this.EmailLabel.TabIndex = 4;
            this.EmailLabel.Text = "Email address";
            // 
            // Email
            // 
            this.Email.AccessibleName = "Email address";
            this.Email.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Email.Location = new System.Drawing.Point(30, 264);
            this.Email.Name = "Email";
            this.Email.Size = new System.Drawing.Size(320, 23);
            this.Email.TabIndex = 2;
            // 
            // AddressLabel
            // 
            this.AddressLabel.Location = new System.Drawing.Point(380, 234);
            this.AddressLabel.Name = "AddressLabel";
            this.AddressLabel.Size = new System.Drawing.Size(328, 24);
            this.AddressLabel.TabIndex = 5;
            this.AddressLabel.Text = "Full address";
            // 
            // Address
            // 
            this.Address.AccessibleName = "Full address";
            this.Address.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Address.Location = new System.Drawing.Point(380, 264);
            this.Address.Name = "Address";
            this.Address.Size = new System.Drawing.Size(320, 23);
            this.Address.TabIndex = 3;
            // 
            // AreaLabel
            // 
            this.AreaLabel.Location = new System.Drawing.Point(30, 328);
            this.AreaLabel.Name = "AreaLabel";
            this.AreaLabel.Size = new System.Drawing.Size(328, 24);
            this.AreaLabel.TabIndex = 6;
            this.AreaLabel.Text = "Durban service area";
            // 
            // Area
            // 
            this.Area.AccessibleName = "Durban service area";
            this.Area.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Area.Location = new System.Drawing.Point(30, 358);
            this.Area.Name = "Area";
            this.Area.Size = new System.Drawing.Size(320, 24);
            this.Area.TabIndex = 4;
            // 
            // IdentityTypeLabel
            // 
            this.IdentityTypeLabel.Location = new System.Drawing.Point(380, 328);
            this.IdentityTypeLabel.Name = "IdentityTypeLabel";
            this.IdentityTypeLabel.Size = new System.Drawing.Size(328, 24);
            this.IdentityTypeLabel.TabIndex = 7;
            this.IdentityTypeLabel.Text = "Customer identity";
            // 
            // IdentityType
            // 
            this.IdentityType.AccessibleName = "Customer identity";
            this.IdentityType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.IdentityType.Location = new System.Drawing.Point(380, 358);
            this.IdentityType.Name = "IdentityType";
            this.IdentityType.Size = new System.Drawing.Size(320, 24);
            this.IdentityType.TabIndex = 5;
            // 
            // DocumentLabel
            // 
            this.DocumentLabel.Location = new System.Drawing.Point(30, 422);
            this.DocumentLabel.Name = "DocumentLabel";
            this.DocumentLabel.Size = new System.Drawing.Size(328, 24);
            this.DocumentLabel.TabIndex = 8;
            this.DocumentLabel.Text = "Identity or registration number";
            // 
            // Document
            // 
            this.Document.AccessibleName = "Identity or registration number";
            this.Document.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Document.Location = new System.Drawing.Point(30, 452);
            this.Document.Name = "Document";
            this.Document.Size = new System.Drawing.Size(320, 23);
            this.Document.TabIndex = 6;
            // 
            // CountryLabel
            // 
            this.CountryLabel.Location = new System.Drawing.Point(380, 422);
            this.CountryLabel.Name = "CountryLabel";
            this.CountryLabel.Size = new System.Drawing.Size(328, 24);
            this.CountryLabel.TabIndex = 9;
            this.CountryLabel.Text = "Passport issuing country";
            // 
            // Country
            // 
            this.Country.AccessibleName = "Passport issuing country";
            this.Country.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Country.Location = new System.Drawing.Point(380, 452);
            this.Country.Name = "Country";
            this.Country.Size = new System.Drawing.Size(320, 23);
            this.Country.TabIndex = 7;
            // 
            // PasswordLabel
            // 
            this.PasswordLabel.Location = new System.Drawing.Point(30, 516);
            this.PasswordLabel.Name = "PasswordLabel";
            this.PasswordLabel.Size = new System.Drawing.Size(328, 24);
            this.PasswordLabel.TabIndex = 10;
            this.PasswordLabel.Text = "Password";
            // 
            // Password
            // 
            this.Password.AccessibleName = "Password";
            this.Password.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Password.Location = new System.Drawing.Point(30, 546);
            this.Password.Name = "Password";
            this.Password.Size = new System.Drawing.Size(320, 23);
            this.Password.TabIndex = 8;
            // 
            // ConfirmPasswordLabel
            // 
            this.ConfirmPasswordLabel.Location = new System.Drawing.Point(380, 516);
            this.ConfirmPasswordLabel.Name = "ConfirmPasswordLabel";
            this.ConfirmPasswordLabel.Size = new System.Drawing.Size(328, 24);
            this.ConfirmPasswordLabel.TabIndex = 11;
            this.ConfirmPasswordLabel.Text = "Confirm password";
            // 
            // ConfirmPassword
            // 
            this.ConfirmPassword.AccessibleName = "Confirm password";
            this.ConfirmPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ConfirmPassword.Location = new System.Drawing.Point(380, 546);
            this.ConfirmPassword.Name = "ConfirmPassword";
            this.ConfirmPassword.Size = new System.Drawing.Size(320, 23);
            this.ConfirmPassword.TabIndex = 9;
            // 
            // SubmitButton
            // 
            this.SubmitButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(91)))), ((int)(((byte)(156)))));
            this.SubmitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SubmitButton.ForeColor = System.Drawing.Color.White;
            this.SubmitButton.Location = new System.Drawing.Point(30, 618);
            this.SubmitButton.Name = "SubmitButton";
            this.SubmitButton.Size = new System.Drawing.Size(220, 44);
            this.SubmitButton.TabIndex = 10;
            this.SubmitButton.Text = "Create Account";
            this.SubmitButton.UseVisualStyleBackColor = false;
            // 
            // RegistrationView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(740, 698);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.heading);
            this.Controls.Add(this.description);
            this.Controls.Add(this.FullNameLabel);
            this.Controls.Add(this.FullName);
            this.Controls.Add(this.PhoneLabel);
            this.Controls.Add(this.Phone);
            this.Controls.Add(this.EmailLabel);
            this.Controls.Add(this.Email);
            this.Controls.Add(this.AddressLabel);
            this.Controls.Add(this.Address);
            this.Controls.Add(this.AreaLabel);
            this.Controls.Add(this.Area);
            this.Controls.Add(this.IdentityTypeLabel);
            this.Controls.Add(this.IdentityType);
            this.Controls.Add(this.DocumentLabel);
            this.Controls.Add(this.Document);
            this.Controls.Add(this.CountryLabel);
            this.Controls.Add(this.Country);
            this.Controls.Add(this.PasswordLabel);
            this.Controls.Add(this.Password);
            this.Controls.Add(this.ConfirmPasswordLabel);
            this.Controls.Add(this.ConfirmPassword);
            this.Controls.Add(this.SubmitButton);
            this.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.Name = "RegistrationView";
            this.Size = new System.Drawing.Size(1316, 597);
            this.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.ResumeLayout(false);
            this.PerformLayout();

    }
    }
    }