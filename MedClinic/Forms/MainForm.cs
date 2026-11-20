using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using MedClinic.Data;
using MedClinic.Models;
using MedClinic.Repositories;
using MedClinic.Services;
using MedClinic.Views;
using Serilog;

namespace MedClinic.Forms
{
    /// <summary>
    /// Главное окно приложения: меню, вкладки, строка состояния.
    /// Доступ к разделам зависит от роли пользователя.
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly User _currentUser;

        // Рабочие представления (создаются в BuildViews)
        private AppointmentsView _appointmentsView;
        private StatisticsView _statisticsView;
        private ReportsView _reportsView;
        private UsersView _usersView;

        // Соответствие "вкладка <-> пункт меню" для синхронизации
        private Dictionary<TabPage, ToolStripMenuItem> _tabMenuMap;

        // Всплывающие подсказки для кнопок и карточек
        private readonly ToolTip _toolTip = new ToolTip();

        /// <summary>true — пользователь нажал "Сменить пользователя" (нужно снова показать вход).</summary>
        public bool LogoutRequested { get; private set; }

        public MainForm(User user)
        {
            if (user == null) throw new ArgumentNullException("user");

            _currentUser = user;

            InitializeComponent();

            BuildNavigationMap();
            ApplyUserInfo();
            LoadDashboardCounters();
            ConfigureAccessByRole();
            BuildViews();
            ConfigureTooltips();
            WireDashboardCards();

            timerClock.Start();
            tabMain_SelectedIndexChanged(null, EventArgs.Empty);
        }

        /// <summary>
        /// Наполняет вкладки рабочими представлениями (этап 2).
        /// Заглушки, созданные в Designer, при этом удаляются.
        /// </summary>
        private void BuildViews()
        {
            if (tabMain.TabPages.Contains(tabDoctors))
            {
                tabDoctors.Controls.Clear();

                var doctorsView = new DoctorsView();
                doctorsView.Dock = DockStyle.Fill;
                doctorsView.ApplyRole(_currentUser.Role);

                tabDoctors.Controls.Add(doctorsView);
            }

            if (tabMain.TabPages.Contains(tabServices))
            {
                tabServices.Controls.Clear();

                var servicesView = new ServicesView();
                servicesView.Dock = DockStyle.Fill;
                servicesView.ApplyRole(_currentUser.Role);

                tabServices.Controls.Add(servicesView);
            }

            if (tabMain.TabPages.Contains(tabAppointments))
            {
                tabAppointments.Controls.Clear();

                _appointmentsView = new AppointmentsView();
                _appointmentsView.Dock = DockStyle.Fill;
                _appointmentsView.ApplyRole(_currentUser);
                _appointmentsView.DataChanged += AppointmentsView_DataChanged;

                tabAppointments.Controls.Add(_appointmentsView);
            }

            if (tabMain.TabPages.Contains(tabStatistics))
            {
                tabStatistics.Controls.Clear();

                _statisticsView = new StatisticsView();
                _statisticsView.Dock = DockStyle.Fill;

                tabStatistics.Controls.Add(_statisticsView);
            }

            if (tabMain.TabPages.Contains(tabReports))
            {
                tabReports.Controls.Clear();

                _reportsView = new ReportsView();
                _reportsView.Dock = DockStyle.Fill;

                tabReports.Controls.Add(_reportsView);
            }

            if (tabMain.TabPages.Contains(tabUsers))
            {
                tabUsers.Controls.Clear();

                _usersView = new UsersView();
                _usersView.Dock = DockStyle.Fill;

                tabUsers.Controls.Add(_usersView);
            }
        }

        /// <summary>После изменения записей обновляем счётчики, статистику и отчёт.</summary>
        private void AppointmentsView_DataChanged(object sender, EventArgs e)
        {
            LoadDashboardCounters();

            if (_statisticsView != null)
                _statisticsView.ReloadData();

            if (_reportsView != null)
                _reportsView.ReloadData();
        }

        // --------------------------- НАСТРОЙКА ОКНА ---------------------------

        /// <summary>Связывает каждую вкладку с пунктом меню и подписывается на переходы.</summary>
        private void BuildNavigationMap()
        {
            _tabMenuMap = new Dictionary<TabPage, ToolStripMenuItem>
            {
                { tabHome,         menuHome },
                { tabDoctors,      menuDoctors },
                { tabServices,     menuServices },
                { tabAppointments, menuAppointmentsList },
                { tabReports,      menuReportPeriod },
                { tabStatistics,   menuStatistics },
                { tabUsers,        menuUsers }
            };

            foreach (var pair in _tabMenuMap)
                pair.Value.Click += NavigationItem_Click;
        }

        /// <summary>Подписи с логином, ролью и текущим временем.</summary>
        private void ApplyUserInfo()
        {
            Text = "Система записи на приём — " + _currentUser.Login +
                   " (" + _currentUser.RoleDisplay + ")";

            lblWelcome.Text = "Здравствуйте, " + _currentUser.Login + "!";
            lblRole.Text = "Ваша роль: " + _currentUser.RoleDisplay;
            lblCapabilities.Text = Permissions.Describe(_currentUser.Role);

            tsUser.Text = "Пользователь: " + _currentUser.Login;
            tsRole.Text = "Роль: " + _currentUser.RoleDisplay;
            tsDate.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");

            lblDbInfo.Text = "База данных: " + AppConfig.Current.DatabasePath;
        }

        /// <summary>Загружает числа на карточках главной вкладки из базы данных.</summary>
        private void LoadDashboardCounters()
        {
            try
            {
                var doctors = new DoctorRepository();
                var services = new ServiceRepository();
                var appointments = new AppointmentRepository();

                lblCountDoctors.Text = doctors.CountAll().ToString();
                lblCountServices.Text = services.CountAll().ToString();
                lblCountAppointments.Text = appointments.CountAll().ToString();
                lblCountToday.Text = appointments.CountToday().ToString();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Не удалось загрузить счётчики для главной вкладки");
                lblCountDoctors.Text = "—";
                lblCountServices.Text = "—";
                lblCountAppointments.Text = "—";
                lblCountToday.Text = "—";
            }
        }

        /// <summary>Скрывает недоступные роли разделы (меню и вкладки).</summary>
        private void ConfigureAccessByRole()
        {
            var role = _currentUser.Role;

            // Управление пользователями — только админ
            if (!Permissions.CanManageUsers(role))
            {
                RemoveTab(tabUsers);
                menuUsers.Visible = false;
            }

            // Журнал событий — только админ
            if (!Permissions.CanViewLogs(role))
                menuLogs.Visible = false;

            // Отчёты и статистика — менеджер и админ
            if (!Permissions.CanViewReports(role))
            {
                RemoveTab(tabReports);
                RemoveTab(tabStatistics);
                menuReportPeriod.Visible = false;
                menuStatistics.Visible = false;
                menuReports.Visible = false;
                btnQuickReports.Enabled = false;
            }

            // Пункт "Администрирование" не нужен, если в нём не осталось доступных команд
            menuAdmin.Visible = menuUsers.Visible || menuLogs.Visible;

            // Создание записи — клиент и выше
            var canCreate = Permissions.CanCreateAppointment(role);
            menuNewAppointment.Enabled = canCreate;
            btnQuickNewAppointment.Enabled = canCreate;

            // Если текущая вкладка была скрыта — вернуться на главную
            if (tabMain.SelectedTab == null)
                tabMain.SelectedTab = tabHome;
        }

        private void RemoveTab(TabPage page)
        {
            if (tabMain.TabPages.Contains(page))
                tabMain.TabPages.Remove(page);
        }

        // ---------------------- СИНХРОНИЗАЦИЯ МЕНЮ И ВКЛАДОК ----------------------

        private void NavigationItem_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            if (item == null || _tabMenuMap == null) return;

            foreach (var pair in _tabMenuMap)
            {
                if (pair.Value != item) continue;

                if (tabMain.TabPages.Contains(pair.Key))
                    tabMain.SelectedTab = pair.Key;
                else
                    MessageBox.Show("Раздел недоступен для вашей роли.", "Доступ ограничен",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        private void tabMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Событие срабатывает ещё при создании формы, когда карта переходов не готова
            if (_tabMenuMap == null) return;

            foreach (var pair in _tabMenuMap)
                pair.Value.Checked = (pair.Key == tabMain.SelectedTab);

            UpdateStatusHint();

            // Счётчики на главной могли устареть после работы с записями
            if (tabMain.SelectedTab == tabHome)
                LoadDashboardCounters();

            // Отчёт перестраивается при открытии вкладки: данные могли измениться
            if (tabMain.SelectedTab == tabReports && _reportsView != null)
                _reportsView.ReloadData();
        }

        // ---------------------- ПОДСКАЗКИ И БЫСТРЫЕ КАРТОЧКИ ----------------------

        /// <summary>Показывает в строке состояния короткую подсказку для открытого раздела.</summary>
        private void UpdateStatusHint()
        {
            if (tsHint == null) return;

            var page = tabMain.SelectedTab;

            if (page == tabDoctors)
                tsHint.Text = "Врачи: сортировка — щелчок по заголовку, редактирование — двойной щелчок.";
            else if (page == tabServices)
                tsHint.Text = "Услуги: цена и длительность подставляются в форму записи.";
            else if (page == tabAppointments)
                tsHint.Text = "Записи: двойной щелчок по активной строке завершает приём.";
            else if (page == tabReports)
                tsHint.Text = "Отчёт: выберите период и нажмите «Построить», затем Excel или Печать.";
            else if (page == tabStatistics)
                tsHint.Text = "Статистика: период можно задать кнопками «Текущий месяц» и «Прошлый месяц».";
            else if (page == tabUsers)
                tsHint.Text = "Пользователи: свою запись и последнего администратора удалить нельзя.";
            else
                tsHint.Text = "Совет: карточки и кнопки на главной открывают нужный раздел.";
        }

        /// <summary>Настраивает всплывающие подсказки для кнопок быстрого перехода.</summary>
        private void ConfigureTooltips()
        {
            _toolTip.AutoPopDelay = 8000;
            _toolTip.InitialDelay = 400;
            _toolTip.ReshowDelay = 100;
            _toolTip.ShowAlways = true;

            _toolTip.SetToolTip(btnQuickDoctors, "Открыть справочник врачей");
            _toolTip.SetToolTip(btnQuickNewAppointment, "Создать новую запись на приём (Ctrl+N)");
            _toolTip.SetToolTip(btnQuickReports, "Открыть отчёт «Записи за период»");
            _toolTip.SetToolTip(btnQuickHelp, "Краткая инструкция и горячие клавиши (F1)");
        }

        /// <summary>Делает карточки-счётчики кликабельными: они открывают нужный раздел.</summary>
        private void WireDashboardCards()
        {
            WireCard(panelCardDoctors, "Открыть справочник врачей", DashboardDoctors_Click);
            WireCard(panelCardServices, "Открыть справочник услуг", DashboardServices_Click);
            WireCard(panelCardAppointments, "Открыть журнал всех записей", DashboardAppointments_Click);
            WireCard(panelCardToday, "Показать только записи на сегодня", DashboardToday_Click);
        }

        private void WireCard(Control card, string tooltip, EventHandler handler)
        {
            card.Cursor = Cursors.Hand;
            card.Click += handler;
            _toolTip.SetToolTip(card, tooltip);

            foreach (Control child in card.Controls)
            {
                child.Cursor = Cursors.Hand;
                child.Click += handler;
                _toolTip.SetToolTip(child, tooltip);
            }
        }

        private void DashboardDoctors_Click(object sender, EventArgs e)
        {
            OpenSection(tabDoctors);
        }

        private void DashboardServices_Click(object sender, EventArgs e)
        {
            OpenSection(tabServices);
        }

        private void DashboardAppointments_Click(object sender, EventArgs e)
        {
            OpenSection(tabAppointments);
        }

        private void DashboardToday_Click(object sender, EventArgs e)
        {
            OpenSection(tabAppointments);

            if (_appointmentsView != null)
                _appointmentsView.ShowToday();
        }

        /// <summary>Открывает вкладку, если она доступна текущей роли.</summary>
        private void OpenSection(TabPage page)
        {
            if (tabMain.TabPages.Contains(page))
                tabMain.SelectedTab = page;
        }

        // ------------------------------ МЕНЮ ------------------------------

        private void menuChangeUser_Click(object sender, EventArgs e)
        {
            var answer = MessageBox.Show("Выйти из учётной записи «" + _currentUser.Login + "»?",
                "Смена пользователя", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (answer != DialogResult.Yes) return;

            Log.Information("Пользователь {Login} сменил учётную запись", _currentUser.Login);
            LogoutRequested = true;
            Close();
        }

        private void menuExit_Click(object sender, EventArgs e)
        {
            LogoutRequested = false;
            Close();
        }

        private void menuNewAppointment_Click(object sender, EventArgs e)
        {
            CreateAppointment();
        }

        /// <summary>
        /// Открывает форму записи на приём из меню или быстрой кнопки.
        /// После сохранения журнал, счётчики и статистика обновляются сами.
        /// </summary>
        private void CreateAppointment()
        {
            if (!Permissions.CanCreateAppointment(_currentUser.Role))
            {
                MessageBox.Show("Ваша роль не позволяет создавать записи на приём.",
                    "Доступ ограничен", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_appointmentsView == null)
            {
                MessageBox.Show("Раздел записей недоступен.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (tabMain.TabPages.Contains(tabAppointments))
                tabMain.SelectedTab = tabAppointments;

            _appointmentsView.CreateNewAppointment();
        }

        private void menuLogs_Click(object sender, EventArgs e)
        {
            try
            {
                var folder = AppConfig.Current.LogFolderPath;
                Process.Start("explorer.exe", "\"" + folder + "\"");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Не удалось открыть папку журналов");
                MessageBox.Show("Не удалось открыть папку журналов: " + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void menuAbout_Click(object sender, EventArgs e)
        {
            ShowAbout();
        }

        /// <summary>
        /// Создаёт резервную копию базы в выбранный файл.
        /// Копию можно хранить на флешке или сетевом диске.
        /// </summary>
        private void menuBackup_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Title = "Резервная копия базы данных";
                dialog.Filter = "База SQLite (*.db)|*.db|Все файлы (*.*)|*.*";
                dialog.DefaultExt = "db";
                dialog.AddExtension = true;
                dialog.OverwritePrompt = true;
                dialog.FileName = "clinic-backup-" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm") + ".db";

                if (dialog.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    Database.BackupTo(dialog.FileName);

                    Log.Information("Создана резервная копия базы: {Path} (пользователь {Login})",
                        dialog.FileName, _currentUser.Login);

                    MessageBox.Show(
                        "Резервная копия создана.\n\n" + dialog.FileName +
                        "\n\nХраните копию отдельно от рабочей базы: на флешке, " +
                        "сетевом диске или в облаке.",
                        "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Не удалось создать резервную копию базы");
                    MessageBox.Show(
                        "Не удалось создать резервную копию.\n\n" + ex.Message +
                        "\n\nПроверьте, что выбрана доступная папка и файл не занят другой программой.",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // --------------------------- БЫСТРЫЙ ПЕРЕХОД ---------------------------

        private void btnQuickDoctors_Click(object sender, EventArgs e)
        {
            if (tabMain.TabPages.Contains(tabDoctors))
                tabMain.SelectedTab = tabDoctors;
        }

        private void btnQuickNewAppointment_Click(object sender, EventArgs e)
        {
            CreateAppointment();
        }

        private void btnQuickReports_Click(object sender, EventArgs e)
        {
            if (!Permissions.CanViewReports(_currentUser.Role))
            {
                MessageBox.Show("Отчёты доступны менеджеру и администратору.",
                    "Доступ ограничен", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (tabMain.TabPages.Contains(tabReports))
                tabMain.SelectedTab = tabReports;
        }

        private void btnQuickHelp_Click(object sender, EventArgs e)
        {
            ShowHelp();
        }

        private void menuHelpStart_Click(object sender, EventArgs e)
        {
            ShowHelp();
        }

        // --------------------------- ВСПОМОГАТЕЛЬНОЕ ---------------------------

        private void ShowHelp()
        {
            using (var form = new HelpForm())
                form.ShowDialog(this);
        }

        private void ShowAbout()
        {
            MessageBox.Show(
                "Система записи на приём\n" +
                "Медицинский центр «Здоровье+»\n\n" +
                "Версия: " + AppConfig.Current.Application.Version + "\n" +
                "Платформа: .NET Framework 4.7.2, Windows Forms\n" +
                "База данных: SQLite\n\n" +
                "Файл базы: " + AppConfig.Current.DatabasePath + "\n" +
                "Журнал: " + AppConfig.Current.LogFolderPath,
                "О программе", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void timerClock_Tick(object sender, EventArgs e)
        {
            tsDate.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Log.Information("Закрыто главное окно (пользователь {Login}, смена пользователя: {Change})",
                _currentUser.Login, LogoutRequested);

            _toolTip.Dispose();
            base.OnFormClosed(e);
        }
    }
}
