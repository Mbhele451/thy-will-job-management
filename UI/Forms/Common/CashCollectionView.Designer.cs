namespace Major_Project.UI.Forms.Common {
        partial class CashCollectionView {
        private System.ComponentModel.IContainer components = null;
private System.Windows.Forms.Label heading;
internal System.Windows.Forms.Label description;
internal System.Windows.Forms.Label AmountDue;
private System.Windows.Forms.Label AmountDueLabel;
internal System.Windows.Forms.Label Received;
private System.Windows.Forms.Label ReceivedLabel;
internal System.Windows.Forms.TextBox Amount;
private System.Windows.Forms.Label AmountLabel;
internal System.Windows.Forms.Label JobNumber;
private System.Windows.Forms.Label JobNumberLabel;
internal System.Windows.Forms.Button SubmitButton;
            protected override void Dispose(bool disposing) {
        if(disposing && components != null) components.Dispose();
            base.Dispose(disposing);
           
    } private void InitializeComponent() {
            this.heading = new System.Windows.Forms.Label();
            this.description = new System.Windows.Forms.Label();
            this.AmountDueLabel = new System.Windows.Forms.Label();
            this.AmountDue = new System.Windows.Forms.Label();
            this.ReceivedLabel = new System.Windows.Forms.Label();
            this.Received = new System.Windows.Forms.Label();
            this.AmountLabel = new System.Windows.Forms.Label();
            this.Amount = new System.Windows.Forms.TextBox();
            this.JobNumberLabel = new System.Windows.Forms.Label();
            this.JobNumber = new System.Windows.Forms.Label();
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
            this.heading.Size = new System.Drawing.Size(326, 36);
            this.heading.TabIndex = 0;
            this.heading.Text = "Collect Cash Payment";
            // 
            // description
            // 
            this.description.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(94)))), ((int)(((byte)(121)))));
            this.description.Location = new System.Drawing.Point(30, 76);
            this.description.Name = "description";
            this.description.Size = new System.Drawing.Size(690, 44);
            this.description.TabIndex = 1;
            this.description.Text = "Record an in-person cash payment for a completed customer job.";
            // 
            // AmountDueLabel
            // 
            this.AmountDueLabel.Location = new System.Drawing.Point(30, 140);
            this.AmountDueLabel.Name = "AmountDueLabel";
            this.AmountDueLabel.Size = new System.Drawing.Size(328, 24);
            this.AmountDueLabel.TabIndex = 2;
            this.AmountDueLabel.Text = "Amount due";
            // 
            // AmountDue
            // 
            this.AmountDue.AccessibleName = "Amount due";
            this.AmountDue.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold);
            this.AmountDue.Location = new System.Drawing.Point(30, 170);
            this.AmountDue.Name = "AmountDue";
            this.AmountDue.Size = new System.Drawing.Size(320, 44);
            this.AmountDue.TabIndex = 0;
            // 
            // ReceivedLabel
            // 
            this.ReceivedLabel.Location = new System.Drawing.Point(380, 140);
            this.ReceivedLabel.Name = "ReceivedLabel";
            this.ReceivedLabel.Size = new System.Drawing.Size(328, 24);
            this.ReceivedLabel.TabIndex = 3;
            this.ReceivedLabel.Text = "Cash received so far";
            // 
            // Received
            // 
            this.Received.AccessibleName = "Cash received so far";
            this.Received.Location = new System.Drawing.Point(380, 170);
            this.Received.Name = "Received";
            this.Received.Size = new System.Drawing.Size(320, 32);
            this.Received.TabIndex = 1;
            // 
            // AmountLabel
            // 
            this.AmountLabel.Location = new System.Drawing.Point(30, 234);
            this.AmountLabel.Name = "AmountLabel";
            this.AmountLabel.Size = new System.Drawing.Size(328, 24);
            this.AmountLabel.TabIndex = 4;
            this.AmountLabel.Text = "Cash received";
            // 
            // Amount
            // 
            this.Amount.AccessibleName = "Cash received";
            this.Amount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Amount.Location = new System.Drawing.Point(30, 264);
            this.Amount.Name = "Amount";
            this.Amount.Size = new System.Drawing.Size(320, 27);
            this.Amount.TabIndex = 2;
            // 
            // JobNumberLabel
            // 
            this.JobNumberLabel.Location = new System.Drawing.Point(380, 234);
            this.JobNumberLabel.Name = "JobNumberLabel";
            this.JobNumberLabel.Size = new System.Drawing.Size(328, 24);
            this.JobNumberLabel.TabIndex = 5;
            this.JobNumberLabel.Text = "Job number";
            // 
            // JobNumber
            // 
            this.JobNumber.AccessibleName = "Job number";
            this.JobNumber.Location = new System.Drawing.Point(380, 264);
            this.JobNumber.Name = "JobNumber";
            this.JobNumber.Size = new System.Drawing.Size(320, 32);
            this.JobNumber.TabIndex = 3;
            // 
            // SubmitButton
            // 
            this.SubmitButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(91)))), ((int)(((byte)(156)))));
            this.SubmitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SubmitButton.ForeColor = System.Drawing.Color.White;
            this.SubmitButton.Location = new System.Drawing.Point(30, 336);
            this.SubmitButton.Name = "SubmitButton";
            this.SubmitButton.Size = new System.Drawing.Size(220, 44);
            this.SubmitButton.TabIndex = 4;
            this.SubmitButton.Text = "Continue";
            this.SubmitButton.UseVisualStyleBackColor = false;
            // 
            // CashCollectionView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(740, 416);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.heading);
            this.Controls.Add(this.description);
            this.Controls.Add(this.AmountDueLabel);
            this.Controls.Add(this.AmountDue);
            this.Controls.Add(this.ReceivedLabel);
            this.Controls.Add(this.Received);
            this.Controls.Add(this.AmountLabel);
            this.Controls.Add(this.Amount);
            this.Controls.Add(this.JobNumberLabel);
            this.Controls.Add(this.JobNumber);
            this.Controls.Add(this.SubmitButton);
            this.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.Name = "CashCollectionView";
            this.Size = new System.Drawing.Size(1445, 567);
            this.ResumeLayout(false);
            this.PerformLayout();

    }
    }
    }