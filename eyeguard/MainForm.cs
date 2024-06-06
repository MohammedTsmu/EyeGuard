using System;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.Win32;
using System.IO;
using MaterialSkin;
using MaterialSkin.Controls;

namespace eyeguard
{
    public partial class MainForm : MaterialForm
    {
        private Timer workTimer;
        private Timer breakTimer;
        private MaterialProgressBar progressBar;
        private Label countdownLabel;
        private int workDuration = 20 * 60; // 20 minutes
        private int breakDuration = 30; // 30 seconds
        private int workTimeLeft;
        private int breakTimeLeft;
        private Form breakForm;
        private MaterialProgressBar breakProgressBar;
        private Label breakCountdownLabel;
        private bool enableStartup;
        private PictureBox pinPictureBox;
        private Image pinImage;
        private Image unpinImage;

        public MainForm()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK; // تمكين الوضع الداكن
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Blue600, Primary.Blue700,
                Primary.Blue200, Accent.LightBlue200,
                TextShade.WHITE);

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
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0);
            this.Width = 300;
            this.Height = 50; // زيادة ارتفاع النافذة لتناسب شريط التقدم الأكبر
            this.FormBorderStyle = FormBorderStyle.None; // إزالة شريط العنوان
            this.ControlBox = false; // إزالة أزرار التحكم (التصغير، التكبير، الإغلاق)
            this.TopMost = true; // جعل النافذة تظهر دائمًا فوق النوافذ الأخرى
            this.BackColor = Color.Magenta; // تعيين لون الخلفية ليكون شفافاً
            this.TransparencyKey = Color.Magenta; // تعيين لون الشفافية ليكون نفس لون الخلفية

            progressBar = new MaterialProgressBar();
            progressBar.Maximum = workDuration;
            progressBar.Value = workDuration;
            progressBar.Height = 30; // زيادة ارتفاع شريط التقدم
            progressBar.Width = this.Width - 60; // ضبط العرض ليكون مناسباً ويترك مساحة لأيقونة الدبوس
            progressBar.Location = new Point(0, 15);

            countdownLabel = new Label();
            countdownLabel.TextAlign = ContentAlignment.MiddleCenter;
            countdownLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            countdownLabel.ForeColor = Color.White; // تعيين لون النص ليكون أبيض
            countdownLabel.BackColor = Color.Transparent;
            countdownLabel.Text = TimeSpan.FromSeconds(workTimeLeft).ToString(@"mm\:ss");
            countdownLabel.Width = progressBar.Width; // تعيين عرض مناسب لعرض النص
            countdownLabel.Location = new Point(0, 0); // وضع المؤقت داخل شريط التقدم

            pinImage = Properties.Resources.pin; // تحميل الصور من الموارد
            unpinImage = Properties.Resources.unpin; // تحميل الصور من الموارد
            pinPictureBox = new PictureBox();
            pinPictureBox.Image = unpinImage;
            pinPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pinPictureBox.Width = 20; // ضبط عرض الأيقونة
            pinPictureBox.Height = 20; // ضبط ارتفاع الأيقونة
            pinPictureBox.Location = new Point(this.Width - 20, 15); // ضبط موقع أيقونة الدبوس
            pinPictureBox.Click += PinPictureBox_Click;

            // إضافة العناصر مباشرة إلى النموذج
            this.Controls.Add(progressBar);
            this.Controls.Add(countdownLabel);
            this.Controls.Add(pinPictureBox);

            // إعداد ContextMenuStrip
            this.ContextMenuStrip = new MaterialContextMenuStrip();
            var settingsItem = new ToolStripMenuItem("Settings");
            settingsItem.Click += SettingsItem_Click;
            this.ContextMenuStrip.Items.Add(settingsItem);

            var startupItem = new ToolStripMenuItem("Start with Windows");
            startupItem.Click += (s, args) => SetStartup(true);
            this.ContextMenuStrip.Items.Add(startupItem);
        }

        private void PinPictureBox_Click(object sender, EventArgs e)
        {
            this.TopMost = !this.TopMost;
            pinPictureBox.Image = this.TopMost ? pinImage : unpinImage;
        }

        private void SetStartup(bool enable)
        {
            string runKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            string appName = "EyeGuard";
            string appPath = Application.ExecutablePath;

            using (var key = Registry.CurrentUser.OpenSubKey(runKey, true))
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
            if (workTimeLeft >= 0)
            {
                progressBar.Value = workTimeLeft;
                countdownLabel.Text = TimeSpan.FromSeconds(workTimeLeft).ToString(@"mm\:ss");
            }

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
            if (breakTimeLeft >= 0)
            {
                breakCountdownLabel.Text = breakTimeLeft.ToString();
                breakProgressBar.Value = breakTimeLeft;
            }

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

            breakProgressBar = new MaterialProgressBar();
            breakProgressBar.Dock = DockStyle.Top;
            breakProgressBar.Maximum = breakDuration;
            breakProgressBar.Value = breakDuration;
            breakProgressBar.Height = 30; // زيادة ارتفاع شريط التقدم في شاشة الاستراحة

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
            using (var settingsForm = new SettingsForm(workDuration, breakDuration, enableStartup))
            {
                if (settingsForm.ShowDialog() == DialogResult.OK)
                {
                    workDuration = settingsForm.WorkDuration;
                    breakDuration = settingsForm.BreakDuration;
                    enableStartup = settingsForm.EnableStartup;

                    SetStartup(enableStartup);
                    SaveSettings(); // حفظ الإعدادات الجديدة بعد تغييرها
                }
            }
        }

        /*private void SaveSettings()
        {
            using (var writer = new StreamWriter("settings.txt"))
            {
                writer.WriteLine(workDuration);
                writer.WriteLine(breakDuration);
                writer.WriteLine(enableStartup);
            }
        }*/
        private void SaveSettings()
        {
            string settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "settings.txt");
            using (var writer = new StreamWriter(settingsPath))
            {
                writer.WriteLine(workDuration);
                writer.WriteLine(breakDuration);
                writer.WriteLine(enableStartup);
            }
        }

        /*private void LoadSettings()
        {
            if (File.Exists("settings.txt"))
            {
                using (var reader = new StreamReader("settings.txt"))
                {
                    if (int.TryParse(reader.ReadLine(), out int savedWorkDuration))
                    {
                        workDuration = savedWorkDuration;
                    }

                    if (int.TryParse(reader.ReadLine(), out int savedBreakDuration))
                    {
                        breakDuration = savedBreakDuration;
                    }

                    if (bool.TryParse(reader.ReadLine(), out bool savedEnableStartup))
                    {
                        enableStartup = savedEnableStartup;
                        SetStartup(enableStartup);
                    }
                }
            }
        }*/
        private void LoadSettings()
        {
            string settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "settings.txt");
            if (File.Exists(settingsPath))
            {
                using (var reader = new StreamReader(settingsPath))
                {
                    if (int.TryParse(reader.ReadLine(), out int savedWorkDuration))
                    {
                        workDuration = savedWorkDuration;
                    }

                    if (int.TryParse(reader.ReadLine(), out int savedBreakDuration))
                    {
                        breakDuration = savedBreakDuration;
                    }

                    if (bool.TryParse(reader.ReadLine(), out bool savedEnableStartup))
                    {
                        enableStartup = savedEnableStartup;
                        SetStartup(enableStartup);
                    }
                }
            }
        }
    }
}