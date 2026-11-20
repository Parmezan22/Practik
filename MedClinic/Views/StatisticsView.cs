using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using MedClinic.Repositories;
using MedClinic.Services;
using Serilog;

namespace MedClinic.Views
{
    /// <summary>
    /// Статистика записей за период: всего записей, среднее за день,
    /// медиана, дисперсия, среднеквадратическое отклонение, минимум и максимум.
    /// Ниже — разбивка по дням.
    /// </summary>
    public partial class StatisticsView : UserControl
    {
        private readonly AppointmentRepository _repository = new AppointmentRepository();

        public StatisticsView()
        {
            InitializeComponent();

            // По умолчанию — текущий месяц
            dtpFrom.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpTo.Value = DateTime.Today;

            Calculate();
        }

        /// <summary>Пересчитывает статистику (вызывается главным окном после изменения записей).</summary>
        public void ReloadData()
        {
            Calculate();
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            Calculate();
        }

        private void btnThisMonth_Click(object sender, EventArgs e)
        {
            dtpFrom.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpTo.Value = DateTime.Today;
            Calculate();
        }

        private void btnLastMonth_Click(object sender, EventArgs e)
        {
            var firstOfThisMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var lastMonthEnd = firstOfThisMonth.AddDays(-1);
            var lastMonthStart = new DateTime(lastMonthEnd.Year, lastMonthEnd.Month, 1);

            dtpFrom.Value = lastMonthStart;
            dtpTo.Value = lastMonthEnd;
            Calculate();
        }

        /// <summary>Главный расчёт: данные из базы → статистика → карточки и таблица.</summary>
        private void Calculate()
        {
            try
            {
                var from = dtpFrom.Value.Date;
                var to = dtpTo.Value.Date;

                // Если пользователь задал границы наоборот — меняем местами
                if (to < from)
                {
                    var temp = from;
                    from = to;
                    to = temp;
                    dtpFrom.Value = from;
                    dtpTo.Value = to;
                }

                var daily = _repository.GetDailyCounts(from, to);
                var result = StatisticsService.Calculate(daily, from, to);

                // Карточки с числами
                lblTotalValue.Text = result.TotalCount.ToString();
                lblAverageValue.Text = result.Average.ToString("N2");
                lblMedianValue.Text = result.Median.ToString("N2");
                lblVarianceValue.Text = result.Variance.ToString("N2");
                lblSigmaValue.Text = result.StandardDeviation.ToString("N2");

                lblPeriodInfo.Text = "Период: " + from.ToString("dd.MM.yyyy") + " — " +
                                     to.ToString("dd.MM.yyyy") +
                                     "   •   дней: " + result.DaysInPeriod +
                                     "   •   дней с записями: " + result.DaysWithAppointments;

                lblMinMax.Text = "Минимум: " + result.Min + "   Максимум: " + result.Max;

                // Таблица по дням: только дни, в которые были записи
                var rows = new List<DailyRow>();
                foreach (var item in result.Daily)
                {
                    DateTime day;
                    var text = DateTime.TryParseExact(item.Day, "yyyy-MM-dd",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out day)
                        ? day.ToString("dd.MM.yyyy")
                        : item.Day;

                    rows.Add(new DailyRow { DayText = text, Count = item.Count, SortKey = day });
                }

                rows.Sort(delegate(DailyRow a, DailyRow b) { return a.SortKey.CompareTo(b.SortKey); });

                dgvDaily.DataSource = null;
                dgvDaily.DataSource = rows;

                UpdateChart(result, from, to);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка расчёта статистики");
                MessageBox.Show("Не удалось рассчитать статистику.\n\n" + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Строит столбчатую диаграмму числа записей по дням.
        /// Диаграмма показывается справа от таблицы.
        /// </summary>
        private void UpdateChart(StatisticsResult result, DateTime from, DateTime to)
        {
            try
            {
                var plot = formsPlot.Plot;
                plot.Clear();

                var xs = new List<double>();
                var ys = new List<double>();
                var labels = new List<string>();

                // Берём только дни, в которые были записи, и сортируем по дате
                var days = new List<DailyCount>(result.Daily ?? new List<DailyCount>());
                days.Sort(delegate(DailyCount a, DailyCount b)
                {
                    return string.CompareOrdinal(a.Day, b.Day);
                });

                foreach (var item in days)
                {
                    DateTime day;
                    if (!DateTime.TryParseExact(item.Day, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                                                DateTimeStyles.None, out day))
                        continue;

                    xs.Add((day.Date - from.Date).TotalDays);
                    ys.Add(item.Count);
                    labels.Add(day.ToString("dd.MM"));
                }

                if (ys.Count == 0)
                {
                    plot.Title("За выбранный период записей нет");
                    plot.XTicks(new double[0], new string[0]);
                    plot.SetAxisLimits(0, 1, 0, 1);
                    formsPlot.Refresh();
                    return;
                }

                var bars = plot.AddBar(ys.ToArray(), xs.ToArray());
                bars.FillColor = Color.FromArgb(23, 74, 105);
                bars.BorderLineWidth = 0;

                // Подписываем не более 10 дат, иначе подписи накладываются друг на друга
                var step = Math.Max(1, (int)Math.Ceiling(labels.Count / 10.0));
                var ticks = new List<double>();
                var tickLabels = new List<string>();

                for (var i = 0; i < labels.Count; i += step)
                {
                    ticks.Add(xs[i]);
                    tickLabels.Add(labels[i]);
                }

                plot.XTicks(ticks.ToArray(), tickLabels.ToArray());
                plot.Title("Записи по дням: " + from.ToString("dd.MM.yyyy") + " — " +
                           to.ToString("dd.MM.yyyy"));
                plot.YLabel("Количество записей");
                plot.XLabel("Дата");

                var lastX = Math.Max(xs[xs.Count - 1], (to.Date - from.Date).TotalDays);
                plot.SetAxisLimits(-1, lastX + 1, 0, Math.Max(1, result.Max + 1));

                formsPlot.Refresh();
            }
            catch (Exception ex)
            {
                // Диаграмма — вспомогательный элемент: ошибка не должна ломать расчёт
                Log.Error(ex, "Ошибка построения диаграммы статистики");
            }
        }

        /// <summary>Строка таблицы «разбивка по дням».</summary>
        private class DailyRow
        {
            public string DayText { get; set; }
            public int Count { get; set; }
            public DateTime SortKey { get; set; }
        }
    }
}
