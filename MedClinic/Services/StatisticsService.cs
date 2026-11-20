using System;
using System.Collections.Generic;
using MedClinic.Repositories;

namespace MedClinic.Services
{
    /// <summary>Результаты статистического расчёта за период.</summary>
    public class StatisticsResult
    {
        /// <summary>Всего записей за период.</summary>
        public int TotalCount { get; set; }

        /// <summary>Число календарных дней в периоде.</summary>
        public int DaysInPeriod { get; set; }

        /// <summary>Число дней, в которые были записи.</summary>
        public int DaysWithAppointments { get; set; }

        /// <summary>Среднее число записей в день (по всем дням периода).</summary>
        public double Average { get; set; }

        /// <summary>Медиана дневного числа записей.</summary>
        public double Median { get; set; }

        /// <summary>Дисперсия (генеральная, деление на n).</summary>
        public double Variance { get; set; }

        /// <summary>Среднее квадратическое отклонение.</summary>
        public double StandardDeviation { get; set; }

        /// <summary>Минимум записей в день.</summary>
        public int Min { get; set; }

        /// <summary>Максимум записей в день.</summary>
        public int Max { get; set; }

        /// <summary>Разбивка по дням (только дни, где есть записи).</summary>
        public List<DailyCount> Daily { get; set; }
    }

    /// <summary>
    /// Расчёт статистики по дневному числу записей.
    ///
    /// Формулы:
    ///   среднее:            x̄ = Σxᵢ / n
    ///   медиана:            середина упорядоченного ряда;
    ///                       при чётном n — полусумма двух средних значений
    ///   дисперсия:          D = Σ(xᵢ - x̄)² / n
    ///   СКО:                σ = √D
    ///
    /// В расчёт включаются ВСЕ календарные дни периода, в том числе дни
    /// без записей (xᵢ = 0), иначе среднее «съезжает» вверх.
    /// </summary>
    public static class StatisticsService
    {
        public static StatisticsResult Calculate(IEnumerable<DailyCount> daily, DateTime from, DateTime to)
        {
            if (to.Date < from.Date)
            {
                var temp = from;
                from = to;
                to = temp;
            }

            var result = new StatisticsResult
            {
                Daily = new List<DailyCount>()
            };

            // Значения по дням: собираем в словарь "дата -> количество"
            var counts = new Dictionary<string, int>();
            if (daily != null)
            {
                foreach (var item in daily)
                {
                    if (item == null || string.IsNullOrEmpty(item.Day)) continue;
                    counts[item.Day] = item.Count;

                    result.Daily.Add(item);
                    result.TotalCount += item.Count;
                    if (item.Count > 0) result.DaysWithAppointments++;
                }
            }

            // Формируем полный ряд по всем дням периода, включая пустые дни
            var values = new List<double>();
            var day = from.Date;
            while (day <= to.Date)
            {
                int count;
                counts.TryGetValue(day.ToString("yyyy-MM-dd"), out count);
                values.Add(count);
                day = day.AddDays(1);
            }

            result.DaysInPeriod = values.Count;
            if (values.Count == 0)
                return result;

            // Среднее арифметическое
            double sum = 0;
            foreach (var value in values) sum += value;
            result.Average = sum / values.Count;

            // Медиана
            var sorted = new List<double>(values);
            sorted.Sort();
            var middle = sorted.Count / 2;
            result.Median = (sorted.Count % 2 == 1)
                ? sorted[middle]
                : (sorted[middle - 1] + sorted[middle]) / 2.0;

            // Дисперсия и СКО
            double squared = 0;
            foreach (var value in values)
            {
                var diff = value - result.Average;
                squared += diff * diff;
            }
            result.Variance = squared / values.Count;
            result.StandardDeviation = Math.Sqrt(result.Variance);

            // Минимум и максимум
            result.Min = (int)sorted[0];
            result.Max = (int)sorted[sorted.Count - 1];

            return result;
        }
    }
}
