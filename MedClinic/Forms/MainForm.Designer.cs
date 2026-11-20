namespace MedClinic.Forms
{
    partial class MainForm
    {
        /// <summary>Обязательная переменная конструктора.</summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>Освобождение использованных ресурсов.</summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора — не изменяйте
        /// содержимое этого метода вручную.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuMain = new System.Windows.Forms.MenuStrip();
            this.menuHome = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuChangeUser = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuBackup = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSep2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDictionaries = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDoctors = new System.Windows.Forms.ToolStripMenuItem();
            this.menuServices = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAppointments = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAppointmentsList = new System.Windows.Forms.ToolStripMenuItem();
            this.menuNewAppointment = new System.Windows.Forms.ToolStripMenuItem();
            this.menuReports = new System.Windows.Forms.ToolStripMenuItem();
            this.menuReportPeriod = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStatistics = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAdmin = new System.Windows.Forms.ToolStripMenuItem();
            this.menuUsers = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLogs = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelpStart = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelpSep = new System.Windows.Forms.ToolStripSeparator();
            this.menuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.statusMain = new System.Windows.Forms.StatusStrip();
            this.tsUser = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsRole = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsHint = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsDate = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabHome = new System.Windows.Forms.TabPage();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.lblRole = new System.Windows.Forms.Label();
            this.panelCardDoctors = new System.Windows.Forms.Panel();
            this.lblCountDoctors = new System.Windows.Forms.Label();
            this.lblDoctorsCaption = new System.Windows.Forms.Label();
            this.panelCardServices = new System.Windows.Forms.Panel();
            this.lblCountServices = new System.Windows.Forms.Label();
            this.lblServicesCaption = new System.Windows.Forms.Label();
            this.panelCardAppointments = new System.Windows.Forms.Panel();
            this.lblCountAppointments = new System.Windows.Forms.Label();
            this.lblAppointmentsCaption = new System.Windows.Forms.Label();
            this.panelCardToday = new System.Windows.Forms.Panel();
            this.lblCountToday = new System.Windows.Forms.Label();
            this.lblTodayCaption = new System.Windows.Forms.Label();
            this.groupCapabilities = new System.Windows.Forms.GroupBox();
            this.lblCapabilities = new System.Windows.Forms.Label();
            this.groupQuick = new System.Windows.Forms.GroupBox();
            this.btnQuickDoctors = new System.Windows.Forms.Button();
            this.btnQuickNewAppointment = new System.Windows.Forms.Button();
            this.btnQuickReports = new System.Windows.Forms.Button();
            this.btnQuickHelp = new System.Windows.Forms.Button();
            this.lblDbInfo = new System.Windows.Forms.Label();
            this.lblStageInfo = new System.Windows.Forms.Label();
            this.tabDoctors = new System.Windows.Forms.TabPage();
            this.lblDoctorsStub = new System.Windows.Forms.Label();
            this.tabServices = new System.Windows.Forms.TabPage();
            this.lblServicesStub = new System.Windows.Forms.Label();
            this.tabAppointments = new System.Windows.Forms.TabPage();
            this.lblAppointmentsStub = new System.Windows.Forms.Label();
            this.tabReports = new System.Windows.Forms.TabPage();
            this.lblReportsStub = new System.Windows.Forms.Label();
            this.tabStatistics = new System.Windows.Forms.TabPage();
            this.lblStatisticsStub = new System.Windows.Forms.Label();
            this.tabUsers = new System.Windows.Forms.TabPage();
            this.lblUsersStub = new System.Windows.Forms.Label();
            this.timerClock = new System.Windows.Forms.Timer(this.components);
            this.menuMain.SuspendLayout();
            this.statusMain.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabHome.SuspendLayout();
            this.panelCardDoctors.SuspendLayout();
            this.panelCardServices.SuspendLayout();
            this.panelCardAppointments.SuspendLayout();
            this.panelCardToday.SuspendLayout();
            this.groupCapabilities.SuspendLayout();
            this.groupQuick.SuspendLayout();
            this.tabDoctors.SuspendLayout();
            this.tabServices.SuspendLayout();
            this.tabAppointments.SuspendLayout();
            this.tabReports.SuspendLayout();
            this.tabStatistics.SuspendLayout();
            this.tabUsers.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuMain
            // 
            this.menuMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.menuMain.BackColor = System.Drawing.Color.White;
            this.menuMain.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuHome,
            this.menuFile,
            this.menuDictionaries,
            this.menuAppointments,
            this.menuReports,
            this.menuAdmin,
            this.menuHelp});
            this.menuMain.Location = new System.Drawing.Point(0, 0);
            this.menuMain.Name = "menuMain";
            this.menuMain.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuMain.Size = new System.Drawing.Size(1180, 28);
            this.menuMain.TabIndex = 0;
            // 
            // menuHome
            // 
            this.menuHome.Name = "menuHome";
            this.menuHome.Size = new System.Drawing.Size(70, 24);
            this.menuHome.Text = "Главная";
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuChangeUser,
            this.menuSep1,
            this.menuBackup,
            this.menuSep2,
            this.menuExit});
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(51, 24);
            this.menuFile.Text = "Файл";
            // 
            // menuChangeUser
            // 
            this.menuChangeUser.Name = "menuChangeUser";
            this.menuChangeUser.Size = new System.Drawing.Size(220, 22);
            this.menuChangeUser.Text = "Сменить пользователя";
            this.menuChangeUser.Click += new System.EventHandler(this.menuChangeUser_Click);
            // 
            // menuSep1
            // 
            this.menuSep1.Name = "menuSep1";
            this.menuSep1.Size = new System.Drawing.Size(217, 6);
            // 
            // menuBackup
            // 
            this.menuBackup.Name = "menuBackup";
            this.menuBackup.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.menuBackup.Size = new System.Drawing.Size(220, 22);
            this.menuBackup.Text = "Резервная копия базы…";
            this.menuBackup.Click += new System.EventHandler(this.menuBackup_Click);
            // 
            // menuSep2
            // 
            this.menuSep2.Name = "menuSep2";
            this.menuSep2.Size = new System.Drawing.Size(217, 6);
            // 
            // menuExit
            // 
            this.menuExit.Name = "menuExit";
            this.menuExit.Size = new System.Drawing.Size(220, 22);
            this.menuExit.Text = "Выход";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // menuDictionaries
            // 
            this.menuDictionaries.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuDoctors,
            this.menuServices});
            this.menuDictionaries.Name = "menuDictionaries";
            this.menuDictionaries.Size = new System.Drawing.Size(95, 24);
            this.menuDictionaries.Text = "Справочники";
            // 
            // menuDoctors
            // 
            this.menuDoctors.Name = "menuDoctors";
            this.menuDoctors.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D1)));
            this.menuDoctors.Size = new System.Drawing.Size(180, 22);
            this.menuDoctors.Text = "Врачи";
            // 
            // menuServices
            // 
            this.menuServices.Name = "menuServices";
            this.menuServices.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D2)));
            this.menuServices.Size = new System.Drawing.Size(180, 22);
            this.menuServices.Text = "Услуги";
            // 
            // menuAppointments
            // 
            this.menuAppointments.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAppointmentsList,
            this.menuNewAppointment});
            this.menuAppointments.Name = "menuAppointments";
            this.menuAppointments.Size = new System.Drawing.Size(62, 24);
            this.menuAppointments.Text = "Записи";
            // 
            // menuAppointmentsList
            // 
            this.menuAppointmentsList.Name = "menuAppointmentsList";
            this.menuAppointmentsList.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D3)));
            this.menuAppointmentsList.Size = new System.Drawing.Size(200, 22);
            this.menuAppointmentsList.Text = "Журнал записей";
            // 
            // menuNewAppointment
            // 
            this.menuNewAppointment.Name = "menuNewAppointment";
            this.menuNewAppointment.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.menuNewAppointment.Size = new System.Drawing.Size(200, 22);
            this.menuNewAppointment.Text = "Новая запись";
            this.menuNewAppointment.Click += new System.EventHandler(this.menuNewAppointment_Click);
            // 
            // menuReports
            // 
            this.menuReports.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuReportPeriod,
            this.menuStatistics});
            this.menuReports.Name = "menuReports";
            this.menuReports.Size = new System.Drawing.Size(60, 24);
            this.menuReports.Text = "Отчёты";
            // 
            // menuReportPeriod
            // 
            this.menuReportPeriod.Name = "menuReportPeriod";
            this.menuReportPeriod.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D4)));
            this.menuReportPeriod.Size = new System.Drawing.Size(200, 22);
            this.menuReportPeriod.Text = "Записи за период";
            // 
            // menuStatistics
            // 
            this.menuStatistics.Name = "menuStatistics";
            this.menuStatistics.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D5)));
            this.menuStatistics.Size = new System.Drawing.Size(200, 22);
            this.menuStatistics.Text = "Статистика";
            // 
            // menuAdmin
            // 
            this.menuAdmin.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuUsers,
            this.menuLogs});
            this.menuAdmin.Name = "menuAdmin";
            this.menuAdmin.Size = new System.Drawing.Size(130, 24);
            this.menuAdmin.Text = "Администрирование";
            // 
            // menuUsers
            // 
            this.menuUsers.Name = "menuUsers";
            this.menuUsers.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D6)));
            this.menuUsers.Size = new System.Drawing.Size(190, 22);
            this.menuUsers.Text = "Пользователи";
            // 
            // menuLogs
            // 
            this.menuLogs.Name = "menuLogs";
            this.menuLogs.Size = new System.Drawing.Size(190, 22);
            this.menuLogs.Text = "Журнал событий";
            this.menuLogs.Click += new System.EventHandler(this.menuLogs_Click);
            // 
            // menuHelp
            // 
            this.menuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuHelpStart,
            this.menuHelpSep,
            this.menuAbout});
            this.menuHelp.Name = "menuHelp";
            this.menuHelp.Size = new System.Drawing.Size(65, 24);
            this.menuHelp.Text = "Справка";
            // 
            // menuHelpStart
            // 
            this.menuHelpStart.Name = "menuHelpStart";
            this.menuHelpStart.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.menuHelpStart.Size = new System.Drawing.Size(180, 22);
            this.menuHelpStart.Text = "Быстрый старт";
            this.menuHelpStart.Click += new System.EventHandler(this.menuHelpStart_Click);
            // 
            // menuHelpSep
            // 
            this.menuHelpSep.Name = "menuHelpSep";
            this.menuHelpSep.Size = new System.Drawing.Size(177, 6);
            // 
            // menuAbout
            // 
            this.menuAbout.Name = "menuAbout";
            this.menuAbout.Size = new System.Drawing.Size(180, 22);
            this.menuAbout.Text = "О программе";
            this.menuAbout.Click += new System.EventHandler(this.menuAbout_Click);
            // 
            // statusMain
            // 
            this.statusMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.statusMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(74)))), ((int)(((byte)(105)))));
            this.statusMain.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.statusMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsUser,
            this.tsRole,
            this.tsHint,
            this.tsDate});
            this.statusMain.Location = new System.Drawing.Point(0, 698);
            this.statusMain.Name = "statusMain";
            this.statusMain.Size = new System.Drawing.Size(1180, 22);
            this.statusMain.TabIndex = 2;
            // 
            // tsUser
            // 
            this.tsUser.ForeColor = System.Drawing.Color.White;
            this.tsUser.Name = "tsUser";
            this.tsUser.Size = new System.Drawing.Size(120, 17);
            this.tsUser.Text = "Пользователь: —";
            // 
            // tsRole
            // 
            this.tsRole.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(205)))), ((int)(((byte)(225)))));
            this.tsRole.Name = "tsRole";
            this.tsRole.Size = new System.Drawing.Size(120, 17);
            this.tsRole.Text = "Роль: —";
            // 
            // tsHint
            // 
            this.tsHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(222)))), ((int)(((byte)(238)))));
            this.tsHint.Name = "tsHint";
            this.tsHint.Size = new System.Drawing.Size(890, 17);
            this.tsHint.Spring = true;
            this.tsHint.Text = "Совет: разделы открываются из меню или с главной страницы.";
            // 
            // tsDate
            // 
            this.tsDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(205)))), ((int)(((byte)(225)))));
            this.tsDate.Name = "tsDate";
            this.tsDate.Size = new System.Drawing.Size(120, 17);
            this.tsDate.Text = "—";
            // 
            // tabMain
            // 
            this.tabMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.tabMain.Controls.Add(this.tabHome);
            this.tabMain.Controls.Add(this.tabDoctors);
            this.tabMain.Controls.Add(this.tabServices);
            this.tabMain.Controls.Add(this.tabAppointments);
            this.tabMain.Controls.Add(this.tabReports);
            this.tabMain.Controls.Add(this.tabStatistics);
            this.tabMain.Controls.Add(this.tabUsers);
            this.tabMain.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.tabMain.Location = new System.Drawing.Point(0, 28);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(1180, 670);
            this.tabMain.TabIndex = 1;
            this.tabMain.SelectedIndexChanged += new System.EventHandler(this.tabMain_SelectedIndexChanged);
            // 
            // tabHome
            // 
            this.tabHome.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.tabHome.Controls.Add(this.lblWelcome);
            this.tabHome.Controls.Add(this.lblRole);
            this.tabHome.Controls.Add(this.panelCardDoctors);
            this.tabHome.Controls.Add(this.panelCardServices);
            this.tabHome.Controls.Add(this.panelCardAppointments);
            this.tabHome.Controls.Add(this.panelCardToday);
            this.tabHome.Controls.Add(this.groupCapabilities);
            this.tabHome.Controls.Add(this.groupQuick);
            this.tabHome.Controls.Add(this.lblDbInfo);
            this.tabHome.Controls.Add(this.lblStageInfo);
            this.tabHome.Location = new System.Drawing.Point(4, 26);
            this.tabHome.Name = "tabHome";
            this.tabHome.Padding = new System.Windows.Forms.Padding(3);
            this.tabHome.Size = new System.Drawing.Size(1172, 640);
            this.tabHome.TabIndex = 0;
            this.tabHome.Text = "Главная";
            // 
            // lblWelcome
            // 
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblWelcome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(74)))), ((int)(((byte)(105)))));
            this.lblWelcome.Location = new System.Drawing.Point(24, 18);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(230, 30);
            this.lblWelcome.TabIndex = 0;
            this.lblWelcome.Text = "Здравствуйте!";
            // 
            // lblRole
            // 
            this.lblRole.AutoSize = true;
            this.lblRole.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblRole.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.lblRole.Location = new System.Drawing.Point(26, 54);
            this.lblRole.Name = "lblRole";
            this.lblRole.Size = new System.Drawing.Size(100, 17);
            this.lblRole.TabIndex = 1;
            this.lblRole.Text = "Ваша роль: —";
            // 
            // panelCardDoctors
            // 
            this.panelCardDoctors.BackColor = System.Drawing.Color.White;
            this.panelCardDoctors.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelCardDoctors.Controls.Add(this.lblCountDoctors);
            this.panelCardDoctors.Controls.Add(this.lblDoctorsCaption);
            this.panelCardDoctors.Location = new System.Drawing.Point(24, 90);
            this.panelCardDoctors.Name = "panelCardDoctors";
            this.panelCardDoctors.Size = new System.Drawing.Size(240, 96);
            this.panelCardDoctors.TabIndex = 2;
            // 
            // lblCountDoctors
            // 
            this.lblCountDoctors.AutoSize = true;
            this.lblCountDoctors.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblCountDoctors.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(168)))), ((int)(((byte)(204)))));
            this.lblCountDoctors.Location = new System.Drawing.Point(14, 8);
            this.lblCountDoctors.Name = "lblCountDoctors";
            this.lblCountDoctors.Size = new System.Drawing.Size(38, 45);
            this.lblCountDoctors.TabIndex = 0;
            this.lblCountDoctors.Text = "0";
            // 
            // lblDoctorsCaption
            // 
            this.lblDoctorsCaption.AutoSize = true;
            this.lblDoctorsCaption.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDoctorsCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.lblDoctorsCaption.Location = new System.Drawing.Point(16, 62);
            this.lblDoctorsCaption.Name = "lblDoctorsCaption";
            this.lblDoctorsCaption.Size = new System.Drawing.Size(150, 15);
            this.lblDoctorsCaption.TabIndex = 1;
            this.lblDoctorsCaption.Text = "Врачей в справочнике";
            // 
            // panelCardServices
            // 
            this.panelCardServices.BackColor = System.Drawing.Color.White;
            this.panelCardServices.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelCardServices.Controls.Add(this.lblCountServices);
            this.panelCardServices.Controls.Add(this.lblServicesCaption);
            this.panelCardServices.Location = new System.Drawing.Point(288, 90);
            this.panelCardServices.Name = "panelCardServices";
            this.panelCardServices.Size = new System.Drawing.Size(240, 96);
            this.panelCardServices.TabIndex = 3;
            // 
            // lblCountServices
            // 
            this.lblCountServices.AutoSize = true;
            this.lblCountServices.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblCountServices.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(168)))), ((int)(((byte)(204)))));
            this.lblCountServices.Location = new System.Drawing.Point(14, 8);
            this.lblCountServices.Name = "lblCountServices";
            this.lblCountServices.Size = new System.Drawing.Size(38, 45);
            this.lblCountServices.TabIndex = 0;
            this.lblCountServices.Text = "0";
            // 
            // lblServicesCaption
            // 
            this.lblServicesCaption.AutoSize = true;
            this.lblServicesCaption.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblServicesCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.lblServicesCaption.Location = new System.Drawing.Point(16, 62);
            this.lblServicesCaption.Name = "lblServicesCaption";
            this.lblServicesCaption.Size = new System.Drawing.Size(140, 15);
            this.lblServicesCaption.TabIndex = 1;
            this.lblServicesCaption.Text = "Услуг в прайс-листе";
            // 
            // panelCardAppointments
            // 
            this.panelCardAppointments.BackColor = System.Drawing.Color.White;
            this.panelCardAppointments.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelCardAppointments.Controls.Add(this.lblCountAppointments);
            this.panelCardAppointments.Controls.Add(this.lblAppointmentsCaption);
            this.panelCardAppointments.Location = new System.Drawing.Point(552, 90);
            this.panelCardAppointments.Name = "panelCardAppointments";
            this.panelCardAppointments.Size = new System.Drawing.Size(240, 96);
            this.panelCardAppointments.TabIndex = 4;
            // 
            // lblCountAppointments
            // 
            this.lblCountAppointments.AutoSize = true;
            this.lblCountAppointments.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblCountAppointments.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(168)))), ((int)(((byte)(204)))));
            this.lblCountAppointments.Location = new System.Drawing.Point(14, 8);
            this.lblCountAppointments.Name = "lblCountAppointments";
            this.lblCountAppointments.Size = new System.Drawing.Size(38, 45);
            this.lblCountAppointments.TabIndex = 0;
            this.lblCountAppointments.Text = "0";
            // 
            // lblAppointmentsCaption
            // 
            this.lblAppointmentsCaption.AutoSize = true;
            this.lblAppointmentsCaption.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblAppointmentsCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.lblAppointmentsCaption.Location = new System.Drawing.Point(16, 62);
            this.lblAppointmentsCaption.Name = "lblAppointmentsCaption";
            this.lblAppointmentsCaption.Size = new System.Drawing.Size(150, 15);
            this.lblAppointmentsCaption.TabIndex = 1;
            this.lblAppointmentsCaption.Text = "Всего записей на приём";
            // 
            // panelCardToday
            // 
            this.panelCardToday.BackColor = System.Drawing.Color.White;
            this.panelCardToday.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelCardToday.Controls.Add(this.lblCountToday);
            this.panelCardToday.Controls.Add(this.lblTodayCaption);
            this.panelCardToday.Location = new System.Drawing.Point(816, 90);
            this.panelCardToday.Name = "panelCardToday";
            this.panelCardToday.Size = new System.Drawing.Size(240, 96);
            this.panelCardToday.TabIndex = 5;
            // 
            // lblCountToday
            // 
            this.lblCountToday.AutoSize = true;
            this.lblCountToday.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblCountToday.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(168)))), ((int)(((byte)(204)))));
            this.lblCountToday.Location = new System.Drawing.Point(14, 8);
            this.lblCountToday.Name = "lblCountToday";
            this.lblCountToday.Size = new System.Drawing.Size(38, 45);
            this.lblCountToday.TabIndex = 0;
            this.lblCountToday.Text = "0";
            // 
            // lblTodayCaption
            // 
            this.lblTodayCaption.AutoSize = true;
            this.lblTodayCaption.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblTodayCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.lblTodayCaption.Location = new System.Drawing.Point(16, 62);
            this.lblTodayCaption.Name = "lblTodayCaption";
            this.lblTodayCaption.Size = new System.Drawing.Size(100, 15);
            this.lblTodayCaption.TabIndex = 1;
            this.lblTodayCaption.Text = "Записей сегодня";
            // 
            // groupCapabilities
            // 
            this.groupCapabilities.Controls.Add(this.lblCapabilities);
            this.groupCapabilities.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.groupCapabilities.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(74)))), ((int)(((byte)(105)))));
            this.groupCapabilities.Location = new System.Drawing.Point(24, 206);
            this.groupCapabilities.Name = "groupCapabilities";
            this.groupCapabilities.Size = new System.Drawing.Size(740, 230);
            this.groupCapabilities.TabIndex = 6;
            this.groupCapabilities.TabStop = false;
            this.groupCapabilities.Text = "Ваши права в системе";
            // 
            // lblCapabilities
            // 
            this.lblCapabilities.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblCapabilities.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.lblCapabilities.Location = new System.Drawing.Point(16, 26);
            this.lblCapabilities.Name = "lblCapabilities";
            this.lblCapabilities.Size = new System.Drawing.Size(706, 196);
            this.lblCapabilities.TabIndex = 0;
            // 
            // groupQuick
            // 
            this.groupQuick.Controls.Add(this.btnQuickDoctors);
            this.groupQuick.Controls.Add(this.btnQuickNewAppointment);
            this.groupQuick.Controls.Add(this.btnQuickReports);
            this.groupQuick.Controls.Add(this.btnQuickHelp);
            this.groupQuick.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.groupQuick.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(74)))), ((int)(((byte)(105)))));
            this.groupQuick.Location = new System.Drawing.Point(788, 206);
            this.groupQuick.Name = "groupQuick";
            this.groupQuick.Size = new System.Drawing.Size(360, 230);
            this.groupQuick.TabIndex = 7;
            this.groupQuick.TabStop = false;
            this.groupQuick.Text = "Быстрый переход";
            // 
            // btnQuickDoctors
            // 
            this.btnQuickDoctors.BackColor = System.Drawing.Color.White;
            this.btnQuickDoctors.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnQuickDoctors.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(226)))), ((int)(((byte)(230)))));
            this.btnQuickDoctors.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnQuickDoctors.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnQuickDoctors.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.btnQuickDoctors.Location = new System.Drawing.Point(16, 30);
            this.btnQuickDoctors.Name = "btnQuickDoctors";
            this.btnQuickDoctors.Size = new System.Drawing.Size(328, 38);
            this.btnQuickDoctors.TabIndex = 0;
            this.btnQuickDoctors.Text = "Справочник врачей";
            this.btnQuickDoctors.UseVisualStyleBackColor = false;
            this.btnQuickDoctors.Click += new System.EventHandler(this.btnQuickDoctors_Click);
            // 
            // btnQuickNewAppointment
            // 
            this.btnQuickNewAppointment.BackColor = System.Drawing.Color.White;
            this.btnQuickNewAppointment.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnQuickNewAppointment.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(226)))), ((int)(((byte)(230)))));
            this.btnQuickNewAppointment.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnQuickNewAppointment.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnQuickNewAppointment.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.btnQuickNewAppointment.Location = new System.Drawing.Point(16, 78);
            this.btnQuickNewAppointment.Name = "btnQuickNewAppointment";
            this.btnQuickNewAppointment.Size = new System.Drawing.Size(328, 38);
            this.btnQuickNewAppointment.TabIndex = 1;
            this.btnQuickNewAppointment.Text = "Новая запись на приём";
            this.btnQuickNewAppointment.UseVisualStyleBackColor = false;
            this.btnQuickNewAppointment.Click += new System.EventHandler(this.btnQuickNewAppointment_Click);
            // 
            // btnQuickReports
            // 
            this.btnQuickReports.BackColor = System.Drawing.Color.White;
            this.btnQuickReports.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnQuickReports.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(226)))), ((int)(((byte)(230)))));
            this.btnQuickReports.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnQuickReports.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnQuickReports.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.btnQuickReports.Location = new System.Drawing.Point(16, 126);
            this.btnQuickReports.Name = "btnQuickReports";
            this.btnQuickReports.Size = new System.Drawing.Size(328, 38);
            this.btnQuickReports.TabIndex = 2;
            this.btnQuickReports.Text = "Отчёт «Записи за период»";
            this.btnQuickReports.UseVisualStyleBackColor = false;
            this.btnQuickReports.Click += new System.EventHandler(this.btnQuickReports_Click);
            // 
            // btnQuickHelp
            // 
            this.btnQuickHelp.BackColor = System.Drawing.Color.White;
            this.btnQuickHelp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnQuickHelp.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(226)))), ((int)(((byte)(230)))));
            this.btnQuickHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnQuickHelp.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnQuickHelp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.btnQuickHelp.Location = new System.Drawing.Point(16, 174);
            this.btnQuickHelp.Name = "btnQuickHelp";
            this.btnQuickHelp.Size = new System.Drawing.Size(328, 38);
            this.btnQuickHelp.TabIndex = 3;
            this.btnQuickHelp.Text = "Справка и горячие клавиши";
            this.btnQuickHelp.UseVisualStyleBackColor = false;
            this.btnQuickHelp.Click += new System.EventHandler(this.btnQuickHelp_Click);
            // 
            // lblDbInfo
            // 
            this.lblDbInfo.AutoSize = true;
            this.lblDbInfo.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblDbInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.lblDbInfo.Location = new System.Drawing.Point(26, 452);
            this.lblDbInfo.Name = "lblDbInfo";
            this.lblDbInfo.Size = new System.Drawing.Size(60, 15);
            this.lblDbInfo.TabIndex = 8;
            this.lblDbInfo.Text = "База данных: —";
            // 
            // lblStageInfo
            // 
            this.lblStageInfo.AutoSize = true;
            this.lblStageInfo.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblStageInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.lblStageInfo.Location = new System.Drawing.Point(26, 476);
            this.lblStageInfo.Name = "lblStageInfo";
            this.lblStageInfo.Size = new System.Drawing.Size(400, 15);
            this.lblStageInfo.TabIndex = 9;
            this.lblStageInfo.Text = "Версия 1.0.0: справочники, записи, отчёты, статистика, курсы валют.";
            // 
            // tabDoctors
            // 
            this.tabDoctors.BackColor = System.Drawing.Color.White;
            this.tabDoctors.Controls.Add(this.lblDoctorsStub);
            this.tabDoctors.Location = new System.Drawing.Point(4, 26);
            this.tabDoctors.Name = "tabDoctors";
            this.tabDoctors.Padding = new System.Windows.Forms.Padding(3);
            this.tabDoctors.Size = new System.Drawing.Size(1172, 640);
            this.tabDoctors.TabIndex = 1;
            this.tabDoctors.Text = "Врачи";
            // 
            // lblDoctorsStub
            // 
            this.lblDoctorsStub.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDoctorsStub.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblDoctorsStub.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.lblDoctorsStub.Location = new System.Drawing.Point(3, 3);
            this.lblDoctorsStub.Name = "lblDoctorsStub";
            this.lblDoctorsStub.Size = new System.Drawing.Size(1166, 634);
            this.lblDoctorsStub.TabIndex = 0;
            this.lblDoctorsStub.Text = "Справочник врачей: поиск, сортировка, добавление и изменение.";
            this.lblDoctorsStub.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabServices
            // 
            this.tabServices.BackColor = System.Drawing.Color.White;
            this.tabServices.Controls.Add(this.lblServicesStub);
            this.tabServices.Location = new System.Drawing.Point(4, 26);
            this.tabServices.Name = "tabServices";
            this.tabServices.Padding = new System.Windows.Forms.Padding(3);
            this.tabServices.Size = new System.Drawing.Size(1172, 640);
            this.tabServices.TabIndex = 2;
            this.tabServices.Text = "Услуги";
            // 
            // lblServicesStub
            // 
            this.lblServicesStub.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblServicesStub.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblServicesStub.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.lblServicesStub.Location = new System.Drawing.Point(3, 3);
            this.lblServicesStub.Name = "lblServicesStub";
            this.lblServicesStub.Size = new System.Drawing.Size(1166, 634);
            this.lblServicesStub.TabIndex = 0;
            this.lblServicesStub.Text = "Справочник услуг: название, цена и длительность приёма.";
            this.lblServicesStub.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabAppointments
            // 
            this.tabAppointments.BackColor = System.Drawing.Color.White;
            this.tabAppointments.Controls.Add(this.lblAppointmentsStub);
            this.tabAppointments.Location = new System.Drawing.Point(4, 26);
            this.tabAppointments.Name = "tabAppointments";
            this.tabAppointments.Padding = new System.Windows.Forms.Padding(3);
            this.tabAppointments.Size = new System.Drawing.Size(1172, 640);
            this.tabAppointments.TabIndex = 3;
            this.tabAppointments.Text = "Записи";
            // 
            // lblAppointmentsStub
            // 
            this.lblAppointmentsStub.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAppointmentsStub.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblAppointmentsStub.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.lblAppointmentsStub.Location = new System.Drawing.Point(3, 3);
            this.lblAppointmentsStub.Name = "lblAppointmentsStub";
            this.lblAppointmentsStub.Size = new System.Drawing.Size(1166, 634);
            this.lblAppointmentsStub.TabIndex = 0;
            this.lblAppointmentsStub.Text = "Журнал записей: фильтры, отмена и завершение приёма.";
            this.lblAppointmentsStub.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabReports
            // 
            this.tabReports.BackColor = System.Drawing.Color.White;
            this.tabReports.Controls.Add(this.lblReportsStub);
            this.tabReports.Location = new System.Drawing.Point(4, 26);
            this.tabReports.Name = "tabReports";
            this.tabReports.Padding = new System.Windows.Forms.Padding(3);
            this.tabReports.Size = new System.Drawing.Size(1172, 640);
            this.tabReports.TabIndex = 4;
            this.tabReports.Text = "Отчёты";
            // 
            // lblReportsStub
            // 
            this.lblReportsStub.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblReportsStub.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblReportsStub.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.lblReportsStub.Location = new System.Drawing.Point(3, 3);
            this.lblReportsStub.Name = "lblReportsStub";
            this.lblReportsStub.Size = new System.Drawing.Size(1166, 634);
            this.lblReportsStub.TabIndex = 0;
            this.lblReportsStub.Text = "Отчёт за период: итоги, выгрузка в Excel и печать.";
            this.lblReportsStub.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabStatistics
            // 
            this.tabStatistics.BackColor = System.Drawing.Color.White;
            this.tabStatistics.Controls.Add(this.lblStatisticsStub);
            this.tabStatistics.Location = new System.Drawing.Point(4, 26);
            this.tabStatistics.Name = "tabStatistics";
            this.tabStatistics.Padding = new System.Windows.Forms.Padding(3);
            this.tabStatistics.Size = new System.Drawing.Size(1172, 640);
            this.tabStatistics.TabIndex = 5;
            this.tabStatistics.Text = "Статистика";
            // 
            // lblStatisticsStub
            // 
            this.lblStatisticsStub.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStatisticsStub.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblStatisticsStub.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.lblStatisticsStub.Location = new System.Drawing.Point(3, 3);
            this.lblStatisticsStub.Name = "lblStatisticsStub";
            this.lblStatisticsStub.Size = new System.Drawing.Size(1166, 634);
            this.lblStatisticsStub.TabIndex = 0;
            this.lblStatisticsStub.Text = "Статистика за период: карточки показателей и диаграмма по дням.";
            this.lblStatisticsStub.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabUsers
            // 
            this.tabUsers.BackColor = System.Drawing.Color.White;
            this.tabUsers.Controls.Add(this.lblUsersStub);
            this.tabUsers.Location = new System.Drawing.Point(4, 26);
            this.tabUsers.Name = "tabUsers";
            this.tabUsers.Padding = new System.Windows.Forms.Padding(3);
            this.tabUsers.Size = new System.Drawing.Size(1172, 640);
            this.tabUsers.TabIndex = 6;
            this.tabUsers.Text = "Пользователи";
            // 
            // lblUsersStub
            // 
            this.lblUsersStub.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblUsersStub.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblUsersStub.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
            this.lblUsersStub.Location = new System.Drawing.Point(3, 3);
            this.lblUsersStub.Name = "lblUsersStub";
            this.lblUsersStub.Size = new System.Drawing.Size(1166, 634);
            this.lblUsersStub.TabIndex = 0;
            this.lblUsersStub.Text = "Управление учётными записями: роли, пароли и удаление.";
            this.lblUsersStub.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timerClock
            // 
            this.timerClock.Enabled = true;
            this.timerClock.Interval = 1000;
            this.timerClock.Tick += new System.EventHandler(this.timerClock_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(1180, 720);
            this.Controls.Add(this.tabMain);
            this.Controls.Add(this.statusMain);
            this.Controls.Add(this.menuMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MainMenuStrip = this.menuMain;
            this.MinimumSize = new System.Drawing.Size(1000, 640);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Система записи на приём";
            this.menuMain.ResumeLayout(false);
            this.menuMain.PerformLayout();
            this.statusMain.ResumeLayout(false);
            this.statusMain.PerformLayout();
            this.tabMain.ResumeLayout(false);
            this.tabHome.ResumeLayout(false);
            this.tabHome.PerformLayout();
            this.panelCardDoctors.ResumeLayout(false);
            this.panelCardDoctors.PerformLayout();
            this.panelCardServices.ResumeLayout(false);
            this.panelCardServices.PerformLayout();
            this.panelCardAppointments.ResumeLayout(false);
            this.panelCardAppointments.PerformLayout();
            this.panelCardToday.ResumeLayout(false);
            this.panelCardToday.PerformLayout();
            this.groupCapabilities.ResumeLayout(false);
            this.groupQuick.ResumeLayout(false);
            this.tabDoctors.ResumeLayout(false);
            this.tabServices.ResumeLayout(false);
            this.tabAppointments.ResumeLayout(false);
            this.tabReports.ResumeLayout(false);
            this.tabStatistics.ResumeLayout(false);
            this.tabUsers.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip menuMain;
        private System.Windows.Forms.ToolStripMenuItem menuHome;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuChangeUser;
        private System.Windows.Forms.ToolStripSeparator menuSep1;
        private System.Windows.Forms.ToolStripMenuItem menuBackup;
        private System.Windows.Forms.ToolStripSeparator menuSep2;
        private System.Windows.Forms.ToolStripMenuItem menuExit;
        private System.Windows.Forms.ToolStripMenuItem menuDictionaries;
        private System.Windows.Forms.ToolStripMenuItem menuDoctors;
        private System.Windows.Forms.ToolStripMenuItem menuServices;
        private System.Windows.Forms.ToolStripMenuItem menuAppointments;
        private System.Windows.Forms.ToolStripMenuItem menuAppointmentsList;
        private System.Windows.Forms.ToolStripMenuItem menuNewAppointment;
        private System.Windows.Forms.ToolStripMenuItem menuReports;
        private System.Windows.Forms.ToolStripMenuItem menuReportPeriod;
        private System.Windows.Forms.ToolStripMenuItem menuStatistics;
        private System.Windows.Forms.ToolStripMenuItem menuAdmin;
        private System.Windows.Forms.ToolStripMenuItem menuUsers;
        private System.Windows.Forms.ToolStripMenuItem menuLogs;
        private System.Windows.Forms.ToolStripMenuItem menuHelp;
        private System.Windows.Forms.ToolStripMenuItem menuHelpStart;
        private System.Windows.Forms.ToolStripSeparator menuHelpSep;
        private System.Windows.Forms.ToolStripMenuItem menuAbout;
        private System.Windows.Forms.StatusStrip statusMain;
        private System.Windows.Forms.ToolStripStatusLabel tsUser;
        private System.Windows.Forms.ToolStripStatusLabel tsRole;
        private System.Windows.Forms.ToolStripStatusLabel tsHint;
        private System.Windows.Forms.ToolStripStatusLabel tsDate;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabHome;
        private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.Label lblRole;
        private System.Windows.Forms.Panel panelCardDoctors;
        private System.Windows.Forms.Label lblCountDoctors;
        private System.Windows.Forms.Label lblDoctorsCaption;
        private System.Windows.Forms.Panel panelCardServices;
        private System.Windows.Forms.Label lblCountServices;
        private System.Windows.Forms.Label lblServicesCaption;
        private System.Windows.Forms.Panel panelCardAppointments;
        private System.Windows.Forms.Label lblCountAppointments;
        private System.Windows.Forms.Label lblAppointmentsCaption;
        private System.Windows.Forms.Panel panelCardToday;
        private System.Windows.Forms.Label lblCountToday;
        private System.Windows.Forms.Label lblTodayCaption;
        private System.Windows.Forms.GroupBox groupCapabilities;
        private System.Windows.Forms.Label lblCapabilities;
        private System.Windows.Forms.GroupBox groupQuick;
        private System.Windows.Forms.Button btnQuickDoctors;
        private System.Windows.Forms.Button btnQuickNewAppointment;
        private System.Windows.Forms.Button btnQuickReports;
        private System.Windows.Forms.Button btnQuickHelp;
        private System.Windows.Forms.Label lblDbInfo;
        private System.Windows.Forms.Label lblStageInfo;
        private System.Windows.Forms.TabPage tabDoctors;
        private System.Windows.Forms.Label lblDoctorsStub;
        private System.Windows.Forms.TabPage tabServices;
        private System.Windows.Forms.Label lblServicesStub;
        private System.Windows.Forms.TabPage tabAppointments;
        private System.Windows.Forms.Label lblAppointmentsStub;
        private System.Windows.Forms.TabPage tabReports;
        private System.Windows.Forms.Label lblReportsStub;
        private System.Windows.Forms.TabPage tabStatistics;
        private System.Windows.Forms.Label lblStatisticsStub;
        private System.Windows.Forms.TabPage tabUsers;
        private System.Windows.Forms.Label lblUsersStub;
        private System.Windows.Forms.Timer timerClock;
    }
}
