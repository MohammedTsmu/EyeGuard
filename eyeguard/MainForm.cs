using System;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.Win32;
using System.IO;

namespace eyeguard
{
    public partial class MainForm : Form
    {
        private Timer workTimer;
        private Timer breakTimer;
        private ProgressBar progressBar;
        private Label countdownLabel;
        private int workDuration = 20 * 60; // 20 minutes
        private int breakDuration = 30; // 30 seconds
        private int workTimeLeft;
        private int breakTimeLeft;
        private Form breakForm;
        private ProgressBar breakProgressBar;
        private Label breakCountdownLabel;

        public MainForm()
        {
            InitializeComponent();
            LoadSettings();
            InitializeTimers();
            InitializeUI();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // يمكنك إضافة أي كود ترغب في تنفيذه عند تحميل النموذج هنا
        }

        private void InitializeTimers()
        {
            workTimer = new Timer();
            workTimer.Interval = 1000; // 1 second
            workTimer.Tick += WorkTimer_Tick;

            breakTimer = new Timer();
            breakTimer.Interval = 1000; // 1 second
            breakTimer.Tick += BreakTimer_Tick;

            workTimeLeft = workDuration;
            breakTimeLeft = breakDuration;
            workTimer.Start();
        }

        private void InitializeUI()
        {
            this.Text = "EyeGuard";
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0);
            this.Width = 200;
            this.Height = 30; // تقليل ارتفاع النافذة

            progressBar = new ProgressBar();
            progressBar.Dock = DockStyle.Fill;
            progressBar.Maximum = workDuration;
            progressBar.Value = workDuration;
            progressBar.Height = 15; // تقليل ارتفاع شريط التقدم

            countdownLabel = new Label();
            countdownLabel.Dock = DockStyle.Fill;
            countdownLabel.TextAlign = ContentAlignment.MiddleCenter;
            countdownLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            countdownLabel.ForeColor = Color.Black;
            countdownLabel.BackColor = Color.Transparent;
            countdownLabel.Text = TimeSpan.FromSeconds(workTimeLeft).ToString(@"mm\:ss");

            Button pinButton = new Button();
            pinButton.Text = "Pin";
            pinButton.Dock = DockStyle.Right;
            pinButton.Click += PinButton_Click;

            this.Controls.Add(progressBar);
            this.Controls.Add(countdownLabel);
            this.Controls.Add(pinButton);

            this.ContextMenuStrip = new ContextMenuStrip();
            var settingsItem = new ToolStripMenuItem("Settings");
            settingsItem.Click += SettingsItem_Click;
            this.ContextMenuStrip.Items.Add(settingsItem);

            var startupItem = new ToolStripMenuItem("Start with Windows");
            startupItem.Click += (s, args) => SetStartup(true);
            this.ContextMenuStrip.Items.Add(startupItem);
        }

        private void PinButton_Click(object sender, EventArgs e)
        {
            this.TopMost = !this.TopMost;
            Button pinButton = sender as Button;
            pinButton.Text = this.TopMost ? "Unpin" : "Pin";
        }

        private void SetStartup(bool enable)
        {
            string runKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            string appName = "EyeGuard";
            string appPath = Application.ExecutablePath;

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(runKey, true))
            {
                if (enable)
                {
                    key.SetValue(appName, appPath);
                }
                else
                {
                    key.DeleteValue(appName, false);
                }
            }
        }

        private void WorkTimer_Tick(object sender, EventArgs e)
        {
            workTimeLeft--;
            progressBar.Value = workTimeLeft;
            countdownLabel.Text = TimeSpan.FromSeconds(workTimeLeft).ToString(@"mm\:ss");

            if (workTimeLeft <= 0)
            {
                workTimer.Stop();
                ShowBreakScreen();
                breakTimer.Start();
            }
        }

        private void BreakTimer_Tick(object sender, EventArgs e)
        {
            breakTimeLeft--;
            breakCountdownLabel.Text = breakTimeLeft.ToString();
            breakProgressBar.Value = breakTimeLeft;

            if (breakTimeLeft <= 0)
            {
                breakTimer.Stop();
                HideBreakScreen();
                workTimeLeft = workDuration;
                progressBar.Maximum = workDuration;
                progressBar.Value = workDuration;
                workTimeLeft = workDuration;
                breakTimeLeft = breakDuration;
                workTimer.Start();
            }
        }

        private void ShowBreakScreen()
        {
            breakForm = new Form();
            breakForm.WindowState = FormWindowState.Maximized;
            breakForm.BackColor = Color.Black;
            breakForm.FormBorderStyle = FormBorderStyle.None;
            breakForm.TopMost = true;

            breakProgressBar = new ProgressBar();
            breakProgressBar.Dock = DockStyle.Top;
            breakProgressBar.Maximum = breakDuration;
            breakProgressBar.Value = breakDuration;
            breakProgressBar.Height = 15; // تقليل ارتفاع شريط التقدم في شاشة الاستراحة

            breakCountdownLabel = new Label();
            breakCountdownLabel.ForeColor = Color.White;
            breakCountdownLabel.Font = new Font("Arial", 48);
            breakCountdownLabel.Dock = DockStyle.Fill;
            breakCountdownLabel.TextAlign = ContentAlignment.MiddleCenter;
            breakCountdownLabel.Text = breakTimeLeft.ToString();

            breakForm.Controls.Add(breakProgressBar);
            breakForm.Controls.Add(breakCountdownLabel);
            breakForm.Show();
        }

        private void HideBreakScreen()
        {
            if (breakForm != null)
            {
                breakForm.Close();
                breakForm = null;
            }
        }

        private void SettingsItem_Click(object sender, EventArgs e)
        {
            using (SettingsForm settingsForm = new SettingsForm(workDuration, breakDuration, false))
            {
                if (settingsForm.ShowDialog() == DialogResult.OK)
                {
                    workDuration = settingsForm.WorkDuration;
                    breakDuration = settingsForm.BreakDuration;
                    SaveSettings(); // حفظ الإعدادات الجديدة بعد تغييرها
                }
            }
        }

        private void SaveSettings()
        {
            using (StreamWriter writer = new StreamWriter("settings.txt"))
            {
                writer.WriteLine(workDuration);
                writer.WriteLine(breakDuration);
            }
        }

        private void LoadSettings()
        {
            if (File.Exists("settings.txt"))
            {
                using (StreamReader reader = new StreamReader("settings.txt"))
                {
                    if (int.TryParse(reader.ReadLine(), out int savedWorkDuration))
                    {
                        workDuration = savedWorkDuration;
                    }

                    if (int.TryParse(reader.ReadLine(), out int savedBreakDuration))
                    {
                        breakDuration = savedBreakDuration;
                    }
                }
            }
        }
    }
}
