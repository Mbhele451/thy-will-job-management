namespace Major_Project.UI.Forms.Common {
        partial class BookingDetailsView {
        private System.ComponentModel.IContainer components = null;
private System.Windows.Forms.Label heading;
private System.Windows.Forms.Label description;
internal System.Windows.Forms.DateTimePicker ServiceDate;
private System.Windows.Forms.Label ServiceDateLabel;
internal System.Windows.Forms.Label Summary;
private System.Windows.Forms.Label SummaryLabel;
internal System.Windows.Forms.TextBox Notes;
private System.Windows.Forms.Label NotesLabel;
internal System.Windows.Forms.Button SubmitButton;
            protected override void Dispose(bool disposing) {
        if(disposing && components != null) components.Dispose();
            base.Dispose(disposing);
           
    } private void InitializeComponent() {
            this.heading = new System.Windows.Forms.Label();
            this.description = new System.Windows.Forms.Label();
            this.ServiceDateLabel = new System.Windows.Forms.Label();
            this.ServiceDate = new System.Windows.Forms.DateTimePicker();
            this.SummaryLabel = new System.Windows.Forms.Label();
            this.Summary = new System.Windows.Forms.Label();
            this.NotesLabel = new System.Windows.Forms.Label();
            this.Notes = new System.Windows.Forms.TextBox();
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
            this.heading.Size = new System.Drawing.Size(314, 36);
            this.heading.TabIndex = 0;
            this.heading.Text = "Review your booking";
            // 
            // description
            // 
            this.description.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(94)))), ((int)(((byte)(121)))));
            this.description.Location = new System.Drawing.Point(30, 76);
            this.description.Name = "description";
            this.description.Size = new System.Drawing.Size(690, 44);
            this.description.TabIndex = 1;
            this.description.Text = "Describe the work and choose your preferred service date.";
            // 
            // ServiceDateLabel
            // 
            this.ServiceDateLabel.Location = new System.Drawing.Point(30, 140);
            this.ServiceDateLabel.Name = "ServiceDateLabel";
            this.ServiceDateLabel.Size = new System.Drawing.Size(328, 24);
            this.ServiceDateLabel.TabIndex = 2;
            this.ServiceDateLabel.Text = "Preferred date";
            // 
            // ServiceDate
            // 
            this.ServiceDate.AccessibleName = "Preferred date";
            this.ServiceDate.Location = new System.Drawing.Point(30, 170);
            this.ServiceDate.Name = "ServiceDate";
            this.ServiceDate.Size = new System.Drawing.Size(320, 27);
            this.ServiceDate.TabIndex = 0;
            // 
            // SummaryLabel
            // 
            this.SummaryLabel.Location = new System.Drawing.Point(380, 140);
            this.SummaryLabel.Name = "SummaryLabel";
            this.SummaryLabel.Size = new System.Drawing.Size(328, 24);
            this.SummaryLabel.TabIndex = 3;
            this.SummaryLabel.Text = "Your services";
            // 
            // Summary
            // 
            this.Summary.AccessibleName = "Your services";
            this.Summary.Location = new System.Drawing.Point(380, 170);
            this.Summary.Name = "Summary";
            this.Summary.Size = new System.Drawing.Size(320, 32);
            this.Summary.TabIndex = 1;
            // 
            // NotesLabel
            // 
            this.NotesLabel.Location = new System.Drawing.Point(30, 234);
            this.NotesLabel.Name = "NotesLabel";
            this.NotesLabel.Size = new System.Drawing.Size(328, 24);
            this.NotesLabel.TabIndex = 4;
            this.NotesLabel.Text = "Describe the work";
            // 
            // Notes
            // 
            this.Notes.AccessibleName = "Describe the work";
            this.Notes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Notes.Location = new System.Drawing.Point(30, 264);
            this.Notes.MaxLength = 2000;
            this.Notes.Multiline = true;
            this.Notes.Name = "Notes";
            this.Notes.Size = new System.Drawing.Size(670, 72);
            this.Notes.TabIndex = 2;
            // 
            // SubmitButton
            // 
            this.SubmitButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(91)))), ((int)(((byte)(156)))));
            this.SubmitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SubmitButton.ForeColor = System.Drawing.Color.White;
            this.SubmitButton.Location = new System.Drawing.Point(30, 372);
            this.SubmitButton.Name = "SubmitButton";
            this.SubmitButton.Size = new System.Drawing.Size(220, 44);
            this.SubmitButton.TabIndex = 3;
            this.SubmitButton.Text = "Book Job";
            this.SubmitButton.UseVisualStyleBackColor = false;
            // 
            // BookingDetailsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(740, 416);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.heading);
            this.Controls.Add(this.description);
            this.Controls.Add(this.ServiceDateLabel);
            this.Controls.Add(this.ServiceDate);
            this.Controls.Add(this.SummaryLabel);
            this.Controls.Add(this.Summary);
            this.Controls.Add(this.NotesLabel);
            this.Controls.Add(this.Notes);
            this.Controls.Add(this.SubmitButton);
            this.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.Name = "BookingDetailsView";
            this.Size = new System.Drawing.Size(1333, 597);
            this.ResumeLayout(false);
            this.PerformLayout();

    }
    }
    }