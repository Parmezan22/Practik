using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using MedClinic.Forms;
using MedClinic.Models;
using MedClinic.Repositories;
using MedClinic.Services;
using Serilog;

namespace MedClinic.Views
{
    /// <summary>
    /// Отчёт «Записи за период»: таблица записей, итоговая сумма,
    /// экспорт в Excel и печать (предварительный просмотр).
    /// Доступен менеджеру и администратору.
    /// </summary>
    public partial class ReportsView : UserControl
    {
        private readonly AppointmentRepository _appointmentRepository = new AppointmentRepository();
        private readonly DoctorRepository _doctorRepository = new DoctorRepository();

        // Данные последнего построенного отчёта — нужны для Excel и печати
        private List<Appointment> _rows = new List<Appointment>();
        private DateTime _from = DateTime.Today;
        private DateTime _to = DateTime.Today;
        private string _doctorTitle = "Все врачи";
        private bool _onlyCompleted;

        public ReportsView()
        {
            InitializeComponent();

            // По умолчанию — с начала текущего месяца по сегодня
            dtpFrom.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpTo.Value = DateTime.Today;

            LoadDoctors();
            BuildReport();
        }

        /// <summary>Перестраивает отчёт (например, после изменения записей).</summary>
        public void ReloadData()
        {
            LoadDoctors();
            BuildReport();
        }

        // ------------------------------ ФИЛЬТРЫ ------------------------------

        /// <summary>Заполняет список врачей, сохраняя выбранного.</summary>
        private void LoadDoctors()
        {
            try
            {
                var previousId = SelectedDoctorId();

                cmbDoctor.Items.Clear();
                cmbDoctor.Items.Add(new DoctorFilter(0, "Все врачи"));

                foreach (var doctor in _doctorRepository.GetAll())
                    cmbDoctor.Items.Add(new DoctorFilter(doctor.Id, doctor.DisplayName));

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
                Log.Error(ex, "Ошибка загрузки врачей для отчёта");
                MessageBox.Show("Не удалось загрузить список врачей.\n\n" + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int SelectedDoctorId()
        {
            var item = cmbDoctor.SelectedItem as DoctorFilter;
            return item != null ? item.Id : 0;
        }

        private string SelectedDoctorTitle()
        {
            var item = cmbDoctor.SelectedItem as DoctorFilter;
            return item != null ? item.DisplayName : "Все врачи";
        }

        // ------------------------------ ПОСТРОЕНИЕ ------------------------------

        private void btnBuild_Click(object sender, EventArgs e)
        {
            BuildReport();
        }

        private void BuildReport()
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

                _from = from;
                _to = to;
                _onlyCompleted = chkCompleted.Checked;
                _doctorTitle = SelectedDoctorTitle();

                var doctorId = SelectedDoctorId();
                int? doctor = doctorId > 0 ? (int?)doctorId : null;
                AppointmentStatus? status = _onlyCompleted ? AppointmentStatus.Completed : (AppointmentStatus?)null;

                int total;
                _rows = _appointmentRepository.GetFiltered(from, to, doctor, status,
                                                           "Date", true, 1, int.MaxValue, out total);

                dgvReport.DataSource = null;
                dgvReport.DataSource = _rows;

                UpdateSummary();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка построения отчёта за период");
                MessageBox.Show("Не удалось построить отчёт.\n\n" + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Считает итоги: количество, сумму и средний чек.</summary>
        private void UpdateSummary()
        {
            var completed = 0;
            var cancelled = 0;
            var paidCount = 0;
            decimal sum = 0;

            foreach (var row in _rows)
            {
                if (row.Status == AppointmentStatus.Completed) completed++;
                if (row.Status == AppointmentStatus.Cancelled)
                {
                    cancelled++;
                    continue; // отменённые приёмы в сумму не входят
                }

                sum += row.TotalPrice;
                paidCount++;
            }

            var average = paidCount > 0 ? sum / paidCount : 0;

            lblSummary.Text = "Записей: " + _rows.Count +
                              "   •   завершено: " + completed +
                              "   •   отменено: " + cancelled +
                              "   •   сумма: " + sum.ToString("N2") + " руб.";
            lblAverage.Text = "Средний чек: " + average.ToString("N2") + " руб.";
        }

        private void dgvReport_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvReport.Rows)
            {
                var appointment = row.DataBoundItem as Appointment;
                if (appointment == null) continue;

                if (appointment.Status == AppointmentStatus.Cancelled)
                {
                    row.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(243, 244, 246);
                    row.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(140, 146, 152);
                }
                else if (appointment.Status == AppointmentStatus.Completed)
                {
                    row.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(235, 247, 240);
                    row.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(0, 110, 70);
                }
                else
                {
                    row.DefaultCellStyle.BackColor = System.Drawing.Color.White;
                    row.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(33, 37, 41);
                }
            }
        }

        // ------------------------------ ЭКСПОРТ ------------------------------

        private void btnExcel_Click(object sender, EventArgs e)
        {
            if (_rows.Count == 0)
            {
                MessageBox.Show("За выбранный период записей нет — экспортировать нечего.",
                    "Нет данных", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var dialog = new SaveFileDialog())
            {
                dialog.Title = "Сохранение отчёта в Excel";
                dialog.Filter = "Книга Excel (*.xlsx)|*.xlsx";
                dialog.FileName = "Записи_за_период_" + _from.ToString("dd.MM.yyyy") +
                                  "_" + _to.ToString("dd.MM.yyyy") + ".xlsx";

                if (dialog.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    ExcelReportService.Export(dialog.FileName, _from, _to, _doctorTitle,
                                              _onlyCompleted, _rows);

                    Log.Information("Отчёт за период экспортирован в Excel: {Path}", dialog.FileName);

                    var answer = MessageBox.Show(
                        "Отчёт сохранён:\n" + dialog.FileName + "\n\nОткрыть файл?",
                        "Готово", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    if (answer == DialogResult.Yes)
                        Process.Start(new ProcessStartInfo(dialog.FileName) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Ошибка экспорта отчёта в Excel");
                    MessageBox.Show("Не удалось сохранить файл Excel.\n\n" + ex.Message +
                                    "\n\nПроверьте, что файл не открыт в другой программе.",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ------------------------------ ПЕЧАТЬ ------------------------------

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (_rows.Count == 0)
            {
                MessageBox.Show("За выбранный период записей нет — печатать нечего.",
                    "Нет данных", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                ReportPrinter.ShowPreview(this, _rows, _from, _to, _doctorTitle, _onlyCompleted);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка подготовки отчёта к печати");
                MessageBox.Show("Не удалось открыть предварительный просмотр печати.\n\n" + ex.Message,
                    "Ошибка печати", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ------------------------------ КУРСЫ ВАЛЮТ ------------------------------

        private void btnRates_Click(object sender, EventArgs e)
        {
            using (var form = new CurrencyRatesForm())
                form.ShowDialog(this);
        }

        /// <summary>Пункт фильтра «врач»; Id = 0 — все врачи.</summary>
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
    }
}
