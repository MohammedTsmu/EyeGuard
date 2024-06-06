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
        private NotifyIcon notifyIcon;
        private ContextMenuStrip trayMenu;


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
            this.Height = 30;
            this.Width = 120;
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
            this.Location = new Point(50, 20);
            this.Width = 100;
            this.Height = 0; // زيادة ارتفاع النافذة لتناسب شريط التقدم الأكبر
            this.FormBorderStyle = FormBorderStyle.None; // إزالة شريط العنوان
            this.ControlBox = false; // إزالة أزرار التحكم (التصغير، التكبير، الإغلاق)
            this.TopMost = true; // جعل النافذة تظهر دائمًا فوق النوافذ الأخرى
            this.BackColor = Color.Orange; // تعيين لون الخلفية ليكون شفافاً
            this.Sizable = false;


            // إعداد NotifyIcon
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon= Properties.Resources.vision; // تأكد من إضافة أيقونة التطبيق إلى الموارد
            notifyIcon.Visible = true;
            notifyIcon.Text = "EyeGuard";

            // إعداد قائمة سياق النظام
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Settings", null, SettingsItem_Click);
            trayMenu.Items.Add("Start with Windows", null, (s, e) => SetStartup(true));
            trayMenu.Items.Add("Exit", null, ExitItem_Click);

            notifyIcon.ContextMenuStrip = trayMenu;

            // إخفاء النموذج الرئيسي عند التشغيل
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = false;
            notifyIcon.DoubleClick += (s, e) => this.Show();


            progressBar = new MaterialProgressBar();
            progressBar.Maximum = workDuration;
            progressBar.Value = workDuration;
            progressBar.Width = 100;
            progressBar.Location = new Point(0, 25);

            countdownLabel = new Label();
            countdownLabel.TextAlign = ContentAlignment.MiddleCenter;
            countdownLabel.Font = new Font("Arial", 10, FontStyle.Bold);
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
            pinPictureBox.Height = 30; // ضبط ارتفاع الأيقونة
            pinPictureBox.Location = new Point(100, 0);
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

        private void ShowMainForm()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            notifyIcon.Visible = false;
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.ShowInTaskbar = false;
        }

        private void ExitItem_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            Application.Exit();
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

                // تشغيل صوت التنبيه قبل 10 ثوانٍ من بدء فترة الراحة
                if (workTimeLeft == 10)
                {
                    System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.mixkit_alert_bells_echo_765);
                    player.Play();
                }
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


            // تشغيل صوت التنبيه قبل 3 ثوانٍ من بدء فترة العمل
            if (breakTimeLeft == 3)
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.mixkit_electric_charge_hum_3201);
                player.Play();
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