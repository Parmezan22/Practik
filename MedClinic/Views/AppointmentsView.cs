using System;
using System.Windows.Forms;
using MedClinic.Forms;
using MedClinic.Models;
using MedClinic.Repositories;
using MedClinic.Services;
using Serilog;

namespace MedClinic.Views
{
    /// <summary>
    /// Журнал записей на приём: фильтры по периоду, врачу и статусу,
    /// сортировка по клику на заголовок, постраничный вывод,
    /// создание, отмена и завершение приёма с учётом роли пользователя.
    /// </summary>
    public partial class AppointmentsView : UserControl
    {
        private readonly AppointmentRepository _repository = new AppointmentRepository();
        private readonly DoctorRepository _doctorRepository = new DoctorRepository();

        // Текущий пользователь и его права
        private User _user;
        private bool _canCreate;
        private bool _canCancelAny;
        private bool _canComplete;

        // Состояние таблицы
        private int _page = 1;
        private int _pageSize = 10;
        private string _sortColumn = "Date";
        private bool _sortAscending = true;
        private bool _initializingPageSize;
        private bool _initializingFilters;

        /// <summary>Событие «данные изменились» — главное окно обновляет счётчики и статистику.</summary>
        public event EventHandler DataChanged;

        public AppointmentsView()
        {
            InitializeComponent();
            CreateTicketButton();

            _initializingFilters = true;

            // Размер страницы
            _initializingPageSize = true;
            cmbPageSize.Items.Add("10");
            cmbPageSize.Items.Add("20");
            cmbPageSize.Items.Add("50");
            cmbPageSize.SelectedIndex = 0;
            _initializingPageSize = false;

            // Фильтр по статусу
            cmbStatus.Items.Add(new StatusFilter(null, "Все статусы"));
            cmbStatus.Items.Add(new StatusFilter(AppointmentStatus.Booked, "Записан"));
            cmbStatus.Items.Add(new StatusFilter(AppointmentStatus.Cancelled, "Отменён"));
            cmbStatus.Items.Add(new StatusFilter(AppointmentStatus.Completed, "Завершён"));
            cmbStatus.SelectedIndex = 0;

            // Период по умолчанию: месяц назад — месяц вперёд
            dtpFrom.Value = DateTime.Today.AddDays(-30);
            dtpTo.Value = DateTime.Today.AddDays(30);

            LoadDoctors();

            _initializingFilters = false;

            LoadData();
        }

        // ------------------------------ ПРАВА И ДАННЫЕ ------------------------------

        /// <summary>Применяет права пользователя к кнопкам журнала.</summary>
        public void ApplyRole(User user)
        {
            _user = user;

            var role = user != null ? user.Role : UserRole.Guest;

            _canCreate = Permissions.CanCreateAppointment(role);
            _canCancelAny = Permissions.CanCancelAnyAppointment(role);
            _canComplete = Permissions.CanCompleteAppointment(role);

            btnNew.Enabled = _canCreate;
            btnCancel.Enabled = Permissions.CanCancelOwnAppointment(role);
            btnComplete.Enabled = _canComplete;
        }

        /// <summary>Обновляет список записей (например, после создания записи из меню).</summary>
        public void ReloadData()
        {
            LoadDoctors();
            LoadData();
        }

        /// <summary>
        /// Показывает только записи на сегодня.
        /// Вызывается с главной страницы при щелчке по карточке «Записей сегодня».
        /// </summary>
        public void ShowToday()
        {
            _initializingFilters = true;

            chkToday.Checked = true;
            dtpFrom.Value = DateTime.Today;
            dtpTo.Value = DateTime.Today;
            dtpFrom.Enabled = false;
            dtpTo.Enabled = false;

            _initializingFilters = false;

            _page = 1;
            LoadData();
        }

        /// <summary>Заполняет фильтр врачами. Первый пункт — «Все врачи».</summary>
        private void LoadDoctors()
        {
            try
            {
                var previousId = SelectedDoctorFilterId();

                cmbDoctor.Items.Clear();
                cmbDoctor.Items.Add(new DoctorFilter(0, "Все врачи"));

                foreach (var doctor in _doctorRepository.GetAll())
                    cmbDoctor.Items.Add(new DoctorFilter(doctor.Id, doctor.DisplayName));

                // Возвращаем ранее выбранного врача, если он ещё существует
                var restored = false;
                for (var i = 0; i < cmbDoctor.Items.Count; i++)
                {
                    var item = cmbDoctor.Items[i] as DoctorFilter;
                    if (item != null && item.Id == previousId)
                    {
                        cmbDoctor.SelectedIndex = i;
                        restored = true;
                        break;
                    }
                }

                if (!restored)
                    cmbDoctor.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка загрузки врачей для фильтра записей");
                MessageBox.Show("Не удалось загрузить список врачей.\n\n" + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int SelectedDoctorFilterId()
        {
            var item = cmbDoctor.SelectedItem as DoctorFilter;
            return item != null ? item.Id : 0;
        }

        private AppointmentStatus? SelectedStatusFilter()
        {
            var item = cmbStatus.SelectedItem as StatusFilter;
            return item != null ? item.Value : null;
        }

        // ------------------------------ ЗАГРУЗКА ТАБЛИЦЫ ------------------------------

        private void LoadData()
        {
            try
            {
                var from = dtpFrom.Value.Date;
                var to = dtpTo.Value.Date;

                if (from > to)
                {
                    var temp = from;
                    from = to;
                    to = temp;
                    dtpFrom.Value = from;
                    dtpTo.Value = to;
                }

                var doctorId = SelectedDoctorFilterId();
                int? doctor = doctorId > 0 ? (int?)doctorId : null;

                int total;
                var appointments = _repository.GetFiltered(from, to, doctor, SelectedStatusFilter(),
                                                          _sortColumn, _sortAscending,
                                                          _page, _pageSize, out total);

                var pageCount = Math.Max(1, (int)Math.Ceiling(total / (double)_pageSize));
                if (_page > pageCount)
                {
                    _page = pageCount;
                    appointments = _repository.GetFiltered(from, to, doctor, SelectedStatusFilter(),
                                                          _sortColumn, _sortAscending,
                                                          _page, _pageSize, out total);
                }

                dgvAppointments.DataSource = null;
                dgvAppointments.DataSource = appointments;

                // Если по фильтрам ничего не нашлось — показываем понятную подсказку
                lblEmpty.Visible = appointments.Count == 0;
                if (lblEmpty.Visible) lblEmpty.BringToFront();

                lblPage.Text = "Страница " + _page + " из " + pageCount;
                lblTotal.Text = "Всего записей: " + total;
                btnPrev.Enabled = _page > 1;
                btnNext.Enabled = _page < pageCount;
                UpdateSortLabel();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка загрузки журнала записей");
                MessageBox.Show("Не удалось загрузить список записей.\n\n" + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateSortLabel()
        {
            string title;
            switch (_sortColumn)
            {
                case "Time": title = "по времени"; break;
                case "PatientName": title = "по пациенту"; break;
                case "DoctorName": title = "по врачу"; break;
                case "ServiceName": title = "по услуге"; break;
                case "Status": title = "по статусу"; break;
                default: title = "по дате"; break;
            }

            lblSort.Text = "Сортировка: " + title +
                           (_sortAscending ? " (по возрастанию)" : " (по убыванию)");
        }

        /// <summary>Выбранная запись (null, если ничего не выбрано).</summary>
        private Appointment SelectedAppointment
        {
            get
            {
                if (dgvAppointments.CurrentRow == null) return null;
                return dgvAppointments.CurrentRow.DataBoundItem as Appointment;
            }
        }

        // --------------------------- ЦВЕТА СТРОК ПО СТАТУСУ ---------------------------

        private void dgvAppointments_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvAppointments.Rows)
            {
                var appointment = row.DataBoundItem as Appointment;
                if (appointment == null) continue;

                switch (appointment.Status)
                {
                    case AppointmentStatus.Cancelled:
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(243, 244, 246);
                        row.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(140, 146, 152);
                        break;

                    case AppointmentStatus.Completed:
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(235, 247, 240);
                        row.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(0, 110, 70);
                        break;

                    default:
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.White;
                        row.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(33, 37, 41);
                        break;
                }
            }
        }

        // ------------------------------ ФИЛЬТРЫ ------------------------------

        private void btnApply_Click(object sender, EventArgs e)
        {
            _page = 1;
            LoadData();
        }

        private void btnResetFilters_Click(object sender, EventArgs e)
        {
            _initializingFilters = true;

            chkToday.Checked = false;
            dtpFrom.Value = DateTime.Today.AddDays(-30);
            dtpTo.Value = DateTime.Today.AddDays(30);
            dtpFrom.Enabled = true;
            dtpTo.Enabled = true;

            if (cmbDoctor.Items.Count > 0)
                cmbDoctor.SelectedIndex = 0;

            if (cmbStatus.Items.Count > 0)
                cmbStatus.SelectedIndex = 0;

            _initializingFilters = false;

            _page = 1;
            _sortColumn = "Date";
            _sortAscending = true;
            LoadData();
        }

        private void chkToday_CheckedChanged(object sender, EventArgs e)
        {
            if (_initializingFilters) return;

            if (chkToday.Checked)
            {
                dtpFrom.Value = DateTime.Today;
                dtpTo.Value = DateTime.Today;
                dtpFrom.Enabled = false;
                dtpTo.Enabled = false;
            }
            else
            {
                dtpFrom.Enabled = true;
                dtpTo.Enabled = true;
            }

            _page = 1;
            LoadData();
        }

        // --------------------------- СОРТИРОВКА И ПАГИНАЦИЯ ---------------------------

        private void dgvAppointments_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var column = dgvAppointments.Columns[e.ColumnIndex];
            var key = column.DataPropertyName;

            if (string.IsNullOrEmpty(key)) return;

            // Отображаемые поля сопоставляем с колонками базы
            if (key == "DateText") key = "Date";
            if (key == "StatusText") key = "Status";

            if (_sortColumn == key)
                _sortAscending = !_sortAscending;
            else
            {
                _sortColumn = key;
                _sortAscending = true;
            }

            _page = 1;
            LoadData();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (_page <= 1) return;
            _page--;
            LoadData();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            _page++;
            LoadData();
        }

        private void cmbPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_initializingPageSize) return;

            int size;
            if (int.TryParse(Convert.ToString(cmbPageSize.SelectedItem), out size) && size > 0)
            {
                _pageSize = size;
                _page = 1;
                LoadData();
            }
        }

        // --------------------------- ДЕЙСТВИЯ С ЗАПИСЯМИ ---------------------------

        private void btnNew_Click(object sender, EventArgs e)
        {
            CreateNewAppointment();
        }

        /// <summary>
        /// Открывает форму новой записи и сохраняет её в базе.
        /// Возвращает true, если запись создана.
        /// </summary>
        public bool CreateNewAppointment()
        {
            if (!_canCreate)
            {
                MessageBox.Show("Ваша роль не позволяет создавать записи на приём.",
                    "Доступ ограничен", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            using (var form = new AppointmentEditForm())
            {
                if (form.ShowDialog(this) != DialogResult.OK) return false;

                try
                {
                    var id = _repository.Create(form.Appointment, form.AdditionalServiceIds);

                    Log.Information("Создана запись Id={Id} на {Date} {Time} (пользователь {Login})",
                        id, form.Appointment.DateText, form.Appointment.Time,
                        _user != null ? _user.Login : "?");

                    _page = 1;
                    LoadData();
                    RaiseDataChanged();

                    MessageBox.Show(
                        "Запись создана.\n\n" +
                        "Пациент: " + form.Appointment.PatientName + "\n" +
                        "Дата и время: " + form.Appointment.DateTimeText + "\n" +
                        "Номер записи: " + id,
                        "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return true;
                }
                catch (AppException ex)
                {
                    MessageBox.Show(ex.Message, "Проверьте данные",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Ошибка создания записи на приём");
                    MessageBox.Show("Не удалось создать запись.\n\n" + ex.Message,
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return false;
            }
        }

        private void dgvAppointments_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || !_canComplete) return;

            var appointment = SelectedAppointment;
            if (appointment != null && appointment.Status == AppointmentStatus.Booked)
                CompleteSelected();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            var appointment = SelectedAppointment;
            if (appointment == null)
            {
                MessageBox.Show("Выберите запись в таблице.", "Нет выбора",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (appointment.Status != AppointmentStatus.Booked)
            {
                MessageBox.Show("Отменить можно только запись со статусом «Записан».\n" +
                                "Текущий статус: «" + appointment.StatusText + "».",
                    "Отмена невозможна", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Клиент может отменять только свои записи
            if (!_canCancelAny)
            {
                var currentId = _user != null ? (int?)_user.Id : null;
                if (appointment.CreatedByUserId != currentId)
                {
                    MessageBox.Show(
                        "Вы можете отменять только собственные записи.\n" +
                        "Эта запись создана другим пользователем — обратитесь к менеджеру.",
                        "Доступ ограничен", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            var answer = MessageBox.Show(
                "Отменить запись пациента «" + appointment.PatientName + "» на " +
                appointment.DateTimeText + "?",
                "Подтверждение отмены", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (answer != DialogResult.Yes) return;

            try
            {
                _repository.Cancel(appointment.Id);
                Log.Information("Запись Id={Id} отменена пользователем {Login}",
                    appointment.Id, _user != null ? _user.Login : "?");

                LoadData();
                RaiseDataChanged();
            }
            catch (AppException ex)
            {
                MessageBox.Show(ex.Message, "Отмена невозможна",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка отмены записи Id={Id}", appointment.Id);
                MessageBox.Show("Не удалось отменить запись.\n\n" + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnComplete_Click(object sender, EventArgs e)
        {
            CompleteSelected();
        }

        // ------------------------------ ТАЛОН (МОДУЛЬ Г) ------------------------------

        /// <summary>Кнопка «Талон» справа от «Обновить».</summary>
        private void CreateTicketButton()
        {
            var button = new Button
            {
                Text = "Талон",
                Location = new System.Drawing.Point(btnRefresh.Right + 10, btnRefresh.Top),
                Size = new System.Drawing.Size(120, btnRefresh.Height)
            };
            button.Click += btnTicket_Click;
            panelTools.Controls.Add(button);
        }

        /// <summary>Печать талона на приём по выбранной записи с предварительным просмотром.</summary>
        private void btnTicket_Click(object sender, EventArgs e)
        {
            var appointment = SelectedAppointment;
            if (appointment == null)
            {
                MessageBox.Show("Выберите запись в таблице.", "Нет выбора",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (appointment.Status == AppointmentStatus.Cancelled)
            {
                MessageBox.Show("Запись отменена, талон по ней не печатается.", "Талон",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                var doctor = new DoctorRepository().GetById(appointment.DoctorId);
                var services = _repository.GetServices(appointment.Id);
                var lines = TicketPrinter.BuildLines(appointment, doctor, services,
                    AppConfig.Current.Application.ClinicName);

                Log.Information("Печать талона по записи Id={Id}", appointment.Id);
                TicketPrinter.ShowPreview(FindForm(), lines);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка печати талона Id={Id}", appointment.Id);
                MessageBox.Show("Не удалось сформировать талон.\n\n" + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CompleteSelected()
        {
            if (!_canComplete)
            {
                MessageBox.Show("Завершать приём может только менеджер или администратор.",
                    "Доступ ограничен", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var appointment = SelectedAppointment;
            if (appointment == null)
            {
                MessageBox.Show("Выберите запись в таблице.", "Нет выбора",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (appointment.Status != AppointmentStatus.Booked)
            {
                MessageBox.Show("Завершить можно только запись со статусом «Записан».\n" +
                                "Текущий статус: «" + appointment.StatusText + "».",
                    "Действие невозможно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var answer = MessageBox.Show(
                "Отметить приём пациента «" + appointment.PatientName + "» на " +
                appointment.DateTimeText + " как завершённый?",
                "Завершение приёма", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (answer != DialogResult.Yes) return;

            try
            {
                _repository.Complete(appointment.Id);
                Log.Information("Приём Id={Id} завершён пользователем {Login}",
                    appointment.Id, _user != null ? _user.Login : "?");

                LoadData();
                RaiseDataChanged();
            }
            catch (AppException ex)
            {
                MessageBox.Show(ex.Message, "Завершение невозможно",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка завершения приёма Id={Id}", appointment.Id);
                MessageBox.Show("Не удалось завершить приём.\n\n" + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ReloadData();
        }

        private void RaiseDataChanged()
        {
            if (DataChanged != null)
                DataChanged(this, EventArgs.Empty);
        }

        // --------------------------- ВСПОМОГАТЕЛЬНЫЕ КЛАССЫ ---------------------------

        /// <summary>Пункт фильтра «врач»: Id = 0 означает «Все врачи».</summary>
        private class DoctorFilter
        {
            public int Id { get; private set; }
            public string DisplayName { get; private set; }

            public DoctorFilter(int id, string displayName)
            {
                Id = id;
                DisplayName = displayName;
            }

            public override string ToString()
            {
                return DisplayName;
            }
        }

        /// <summary>Пункт фильтра «статус»: Value = null означает «Все статусы».</summary>
        private class StatusFilter
        {
            public AppointmentStatus? Value { get; private set; }
            public string Text { get; private set; }

            public StatusFilter(AppointmentStatus? value, string text)
            {
                Value = value;
                Text = text;
            }

            public override string ToString()
            {
                return Text;
            }
        }
    }
}
