using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MedClinic.Models;
using MedClinic.Repositories;
using MedClinic.Services;
using MedClinic.UI;
using Serilog;

namespace MedClinic.Forms
{
    /// <summary>
    /// Форма создания записи на приём.
    /// Позволяет выбрать врача, основную услугу, дополнительные услуги
    /// (несколько сразу), дату и время. Проверяет занятость времени.
    /// </summary>
    public partial class AppointmentEditForm : Form
    {
        private readonly DoctorRepository _doctorRepository = new DoctorRepository();
        private readonly ServiceRepository _serviceRepository = new ServiceRepository();
        private readonly AppointmentRepository _appointmentRepository = new AppointmentRepository();

        /// <summary>Созданная запись (заполняется после нажатия «Записать на приём»).</summary>
        public Appointment Appointment { get; private set; }

        /// <summary>Идентификаторы выбранных дополнительных услуг.</summary>
        public List<int> AdditionalServiceIds { get; private set; }

        public AppointmentEditForm()
        {
            InitializeComponent();

            AdditionalServiceIds = new List<int>();

            PlaceholderHelper.SetPlaceholder(txtPatient, "Например: Иванов Иван Иванович");

            // Прошедшие даты выбирать нельзя
            dtpDate.MinDate = DateTime.Today;
            dtpDate.Value = DateTime.Today;

            LoadDoctors();
            LoadServices();
            LoadTimeSlots();
            CreateSuggestButton();

            UpdateTotals();
        }

        // --------------------------- ПОДБОР ВРЕМЕНИ (МОДУЛЬ В) ---------------------------

        /// <summary>Кнопка «Подобрать время» справа от списка времени.</summary>
        private void CreateSuggestButton()
        {
            var button = new Button
            {
                Text = "Подобрать время",
                Location = new Point(cmbTime.Right + 10, cmbTime.Top - 2),
                Size = new Size(150, cmbTime.Height + 4),
                TabIndex = cmbTime.TabIndex + 1
            };
            button.Click += btnSuggest_Click;
            cmbTime.Parent.Controls.Add(button);
            button.BringToFront();
        }

        /// <summary>
        /// Ставит приём в самое короткое свободное окно врача, куда помещается
        /// длительность выбранных услуг (ScheduleOptimizer.BestFit).
        /// </summary>
        private void btnSuggest_Click(object sender, EventArgs e)
        {
            var doctor = cmbDoctor.SelectedItem as Doctor;
            if (doctor == null)
            {
                MessageBox.Show("Сначала выберите врача.", "Подбор времени",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            decimal sum;
            int minutes;
            CalculateSelection(out sum, out minutes);

            var date = dtpDate.Value.Date;
            var notBefore = ScheduleOptimizer.WorkStart;
            if (date == DateTime.Today)
                notBefore = Math.Max(notBefore, (int)DateTime.Now.TimeOfDay.TotalMinutes + 1);

            try
            {
                var busy = _appointmentRepository.GetBusyIntervals(doctor.Id, date);
                var slot = ScheduleOptimizer.BestFit(busy, minutes, notBefore);

                if (slot == null)
                {
                    MessageBox.Show("На " + date.ToString("dd.MM.yyyy") + " у врача нет свободного окна на " +
                                    minutes + " мин. Выберите другой день или другого врача.",
                        "Подбор времени", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SelectTime(slot.StartText);

                var load = ScheduleOptimizer.Utilization(busy, ScheduleOptimizer.WorkStart, ScheduleOptimizer.WorkEnd);
                Log.Information("Подбор времени: врач {DoctorId}, {Date:dd.MM.yyyy}, {Minutes} мин -> {Slot}, окно {Window}",
                    doctor.Id, date, minutes, slot.ToString(), slot.Window.ToString());

                MessageBox.Show("Предложено время " + slot + ".\n" +
                                "Это самое короткое свободное окно (" + slot.Window + "), " +
                                "в которое помещается приём на " + minutes + " мин.\n" +
                                "Загрузка врача в этот день: " + (load * 100).ToString("N0") + " %.",
                    "Подбор времени", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка подбора времени для врача Id={DoctorId}", doctor.Id);
                MessageBox.Show("Не удалось подобрать время.\n\n" + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Выбирает время в списке, при необходимости добавляя его по порядку.</summary>
        private void SelectTime(string time)
        {
            if (!cmbTime.Items.Contains(time))
            {
                var index = 0;
                while (index < cmbTime.Items.Count &&
                       string.CompareOrdinal(Convert.ToString(cmbTime.Items[index]), time) < 0)
                    index++;
                cmbTime.Items.Insert(index, time);
            }

            cmbTime.SelectedItem = time;
        }

        // --------------------------- ЗАПОЛНЕНИЕ СПИСКОВ ---------------------------

        private void LoadDoctors()
        {
            try
            {
                var doctors = _doctorRepository.GetAll();
                cmbDoctor.DataSource = doctors;
                cmbDoctor.DisplayMember = "DisplayName";
                cmbDoctor.ValueMember = "Id";

                if (doctors.Count == 0)
                {
                    MessageBox.Show("В справочнике нет врачей. Сначала добавьте врача.",
                        "Нет данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Не удалось загрузить список врачей для записи");
                MessageBox.Show("Не удалось загрузить список врачей.\n\n" + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnSave.Enabled = false;
            }
        }

        private void LoadServices()
        {
            try
            {
                var services = _serviceRepository.GetAll();

                // Основная услуга
                cmbService.DataSource = services;
                cmbService.DisplayMember = "DisplayName";
                cmbService.ValueMember = "Id";

                // Список дополнительных услуг — отдельная копия списка
                clbServices.DataSource = new List<Service>(services);
                clbServices.DisplayMember = "DisplayName";

                if (services.Count == 0)
                {
                    MessageBox.Show("В прайс-листе нет услуг. Сначала добавьте услугу.",
                        "Нет данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Не удалось загрузить список услуг для записи");
                MessageBox.Show("Не удалось загрузить список услуг.\n\n" + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnSave.Enabled = false;
            }
        }

        /// <summary>Интервал приёма — каждые 30 минут с 08:00 до 19:30.</summary>
        private void LoadTimeSlots()
        {
            cmbTime.Items.Clear();

            var time = new TimeSpan(8, 0, 0);
            var last = new TimeSpan(19, 30, 0);
            while (time <= last)
            {
                cmbTime.Items.Add(time.ToString(@"hh\:mm"));
                time = time.Add(TimeSpan.FromMinutes(30));
            }

            if (cmbTime.Items.Count > 0)
                cmbTime.SelectedIndex = 0;
        }

        // --------------------------- ПОДСЧЁТ СТОИМОСТИ ---------------------------

        private void cmbService_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTotals();
        }

        private void clbServices_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Состояние флажка меняется после события, поэтому пересчёт — чуть позже
            BeginInvoke(new Action(UpdateTotals));
        }

        private void cmbTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTotals();
        }

        /// <summary>
        /// Считает стоимость и длительность выбранного набора услуг.
        /// Длительность нужна, чтобы не записать двух пациентов на пересекающееся время.
        /// </summary>
        private void CalculateSelection(out decimal sum, out int minutes)
        {
            sum = 0;
            minutes = 0;

            var primary = cmbService.SelectedItem as Service;
            if (primary != null)
            {
                sum += primary.Price;
                minutes += primary.Duration;
            }

            foreach (var item in clbServices.CheckedItems)
            {
                var service = item as Service;
                if (service == null || service == primary) continue;

                sum += service.Price;
                minutes += service.Duration;
            }

            if (minutes <= 0) minutes = 30;
        }

        private void UpdateTotals()
        {
            decimal sum;
            int minutes;
            CalculateSelection(out sum, out minutes);

            var text = "Итого: " + sum.ToString("N2") + " руб., " + minutes + " мин.";

            // Показываем реальный интервал приёма: 09:00–10:00, а не только время начала
            int start;
            if (TimeRange.TryParseMinutes(Convert.ToString(cmbTime.SelectedItem), out start))
            {
                var end = start + minutes;
                text += " (" + TimeRange.Format(start) + "–" + TimeRange.Format(end) + ")";

                if (end > 20 * 60)
                {
                    text += " — выходит за 20:00";
                    lblPrice.ForeColor = Color.FromArgb(190, 50, 50);
                }
                else
                {
                    lblPrice.ForeColor = Color.FromArgb(0, 120, 80);
                }
            }

            lblPrice.Text = text;
        }

        // ------------------------------ СОХРАНЕНИЕ ------------------------------

        private void btnSave_Click(object sender, EventArgs e)
        {
            var doctor = cmbDoctor.SelectedItem as Doctor;
            var primaryService = cmbService.SelectedItem as Service;
            var time = Convert.ToString(cmbTime.SelectedItem);

            if (doctor == null)
            {
                MessageBox.Show("Выберите врача.", "Проверьте данные",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbDoctor.Focus();
                return;
            }

            if (primaryService == null)
            {
                MessageBox.Show("Выберите основную услугу.", "Проверьте данные",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbService.Focus();
                return;
            }

            var appointment = new Appointment
            {
                PatientName = txtPatient.Text.Trim(),
                DoctorId = doctor.Id,
                ServiceId = primaryService.Id,
                Date = dtpDate.Value.Date,
                Time = time,
                Status = AppointmentStatus.Booked,
                CreatedByUserId = Session.CurrentUser != null ? Session.CurrentUser.Id : (int?)null
            };

            // Общая валидация данных записи
            var error = AppointmentRepository.CheckAppointment(appointment);
            if (error != null)
            {
                MessageBox.Show(error, "Проверьте данные", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (!Validation.IsValidFullName(appointment.PatientName)) txtPatient.Focus();
                return;
            }

            // Проверка занятости интервала у врача с учётом длительности услуг
            try
            {
                decimal sum;
                int minutes;
                CalculateSelection(out sum, out minutes);

                int start;
                if (!TimeRange.TryParseMinutes(appointment.Time, out start))
                {
                    MessageBox.Show("Выберите время приёма.", "Проверьте данные",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbTime.Focus();
                    return;
                }

                if (start + minutes > 20 * 60)
                {
                    MessageBox.Show(
                        "Приём заканчивается в " + TimeRange.Format(start + minutes) +
                        ", а клиника работает до 20:00.\n" +
                        "Выберите более раннее время или сократите набор услуг.",
                        "Поздно для приёма", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbTime.Focus();
                    return;
                }

                if (_appointmentRepository.IsTimeRangeBusy(appointment.DoctorId, appointment.Date,
                                                           appointment.Time, minutes))
                {
                    MessageBox.Show(
                        "У врача «" + doctor.FullName + "» время с " +
                        TimeRange.Format(start) + " до " + TimeRange.Format(start + minutes) +
                        " уже занято другой записью.\n" +
                        "Выберите другое время или другого врача.",
                        "Время занято", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbTime.Focus();
                    return;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка проверки занятости времени");
                MessageBox.Show("Не удалось проверить занятость времени.\n\n" + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Собираем дополнительные услуги (без основной и без повторов)
            AdditionalServiceIds = new List<int>();
            foreach (var item in clbServices.CheckedItems)
            {
                var service = item as Service;
                if (service == null) continue;
                if (service.Id == primaryService.Id) continue;
                if (!AdditionalServiceIds.Contains(service.Id))
                    AdditionalServiceIds.Add(service.Id);
            }

            Appointment = appointment;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
