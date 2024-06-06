namespace eyeguard
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.workDurationUpDown = new System.Windows.Forms.NumericUpDown();
            this.breakDurationUpDown = new System.Windows.Forms.NumericUpDown();
            this.startupCheckBox = new System.Windows.Forms.CheckBox();
            this.saveButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.workDurationUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.breakDurationUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // workDurationUpDown
            // 
            this.workDurationUpDown.Location = new System.Drawing.Point(12, 12);
            this.workDurationUpDown.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.workDurationUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.workDurationUpDown.Name = "workDurationUpDown";
            this.workDurationUpDown.Size = new System.Drawing.Size(120, 22);
            this.workDurationUpDown.TabIndex = 0;
            this.workDurationUpDown.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // breakDurationUpDown
            // 
            this.breakDurationUpDown.Location = new System.Drawing.Point(12, 40);
            this.breakDurationUpDown.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.breakDurationUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.breakDurationUpDown.Name = "breakDurationUpDown";
            this.breakDurationUpDown.Size = new System.Drawing.Size(120, 22);
            this.breakDurationUpDown.TabIndex = 1;
            this.breakDurationUpDown.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // startupCheckBox
            // 
            this.startupCheckBox.AutoSize = true;
            this.startupCheckBox.Location = new System.Drawing.Point(12, 68);
            this.startupCheckBox.Name = "startupCheckBox";
            this.startupCheckBox.Size = new System.Drawing.Size(139, 20);
            this.startupCheckBox.TabIndex = 2;
            this.startupCheckBox.Text = "Start with Windows";
            this.startupCheckBox.UseVisualStyleBackColor = true;
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(12, 95);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 5;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // SettingsForm
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.startupCheckBox);
            this.Controls.Add(this.breakDurationUpDown);
            this.Controls.Add(this.workDurationUpDown);
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.workDurationUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.breakDurationUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.NumericUpDown workDurationUpDown;
        private System.Windows.Forms.NumericUpDown breakDurationUpDown;
        private System.Windows.Forms.CheckBox startupCheckBox;
        private System.Windows.Forms.Button saveButton; // تغيير النوع إلى Button لحل المشكلة
    }
}
