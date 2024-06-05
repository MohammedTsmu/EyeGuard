using System;
using System.Windows.Forms;

namespace eyeguard
{
    public partial class SettingsForm : Form
    {
        public int WorkDuration { get; private set; }
        public int BreakDuration { get; private set; }
        public bool EnableSound { get; private set; }

        public SettingsForm(int workDuration, int breakDuration, bool enableSound)
        {
            InitializeComponent();

            WorkDuration = workDuration;
            BreakDuration = breakDuration;
            EnableSound = enableSound;

            workDurationUpDown.Value = workDuration / 60;
            breakDurationUpDown.Value = breakDuration;
            soundCheckBox.Checked = enableSound;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            WorkDuration = (int)workDurationUpDown.Value * 60;
            BreakDuration = (int)breakDurationUpDown.Value;
            EnableSound = soundCheckBox.Checked;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
