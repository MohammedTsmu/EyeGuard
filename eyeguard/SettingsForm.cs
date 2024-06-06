using System;
using System.Windows.Forms;

namespace eyeguard
{
    public partial class SettingsForm : Form
    {
        public int WorkDuration { get; private set; }
        public int BreakDuration { get; private set; }
        public bool EnableStartup { get; private set; }

        public SettingsForm(int workDuration, int breakDuration, bool enableStartup)
        {
            InitializeComponent();

            WorkDuration = workDuration;
            BreakDuration = breakDuration;
            EnableStartup = enableStartup;

            // تعيين القيم والتأكد من أنها ضمن النطاقات الصحيحة
            workDurationUpDown.Value = Math.Min(workDurationUpDown.Maximum, Math.Max(workDurationUpDown.Minimum, workDuration / 60));
            breakDurationUpDown.Value = Math.Min(breakDurationUpDown.Maximum, Math.Max(breakDurationUpDown.Minimum, breakDuration));
            startupCheckBox.Checked = enableStartup;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            WorkDuration = (int)workDurationUpDown.Value * 60;
            BreakDuration = (int)breakDurationUpDown.Value;
            EnableStartup = startupCheckBox.Checked;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {

        }
    }
}
