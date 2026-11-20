using System;
using System.Collections.Generic;
using MedClinic.Repositories;
using MedClinic.Services;
using Xunit;

namespace MedClinic.Tests
{
    /// <summary>Проверка статистического расчёта: среднее, медиана, дисперсия, СКО.</summary>
    public class StatisticsServiceTests
    {
        [Fact]
        public void Calculate_IncludesAllCalendarDays()
        {
            var daily = new List<DailyCount>
            {
                new DailyCount { Day = "2026-01-01", Count = 1 },
                new DailyCount { Day = "2026-01-02", Count = 3 },
                new DailyCount { Day = "2026-01-05", Count = 5 }
            };

            var result = StatisticsService.Calculate(daily,
                new DateTime(2026, 1, 1), new DateTime(2026, 1, 5));

            Assert.Equal(5, result.DaysInPeriod);
            Assert.Equal(3, result.DaysWithAppointments);
            Assert.Equal(9, result.TotalCount);
            Assert.Equal(1.8, result.Average, 10);
            Assert.Equal(1.0, result.Median, 10);
            Assert.Equal(3.76, result.Variance, 10);
            Assert.Equal(Math.Sqrt(3.76), result.StandardDeviation, 10);
            Assert.Equal(0, result.Min);
            Assert.Equal(5, result.Max);
        }

        [Fact]
        public void Calculate_MedianForEvenNumberOfDays()
        {
            var daily = new List<DailyCount>
            {
                new DailyCount { Day = "2026-02-02", Count = 2 }
            };

            var result = StatisticsService.Calculate(daily,
                new DateTime(2026, 2, 1), new DateTime(2026, 2, 2));

            Assert.Equal(2, result.DaysInPeriod);
            Assert.Equal(1.0, result.Median, 10);
            Assert.Equal(1.0, result.Average, 10);
            Assert.Equal(0, result.Min);
            Assert.Equal(2, result.Max);
        }

        [Fact]
        public void Calculate_EmptyPeriod_ReturnsZeros()
        {
            var result = StatisticsService.Calculate(new List<DailyCount>(),
                new DateTime(2026, 3, 10), new DateTime(2026, 3, 10));

            Assert.Equal(1, result.DaysInPeriod);
            Assert.Equal(0, result.TotalCount);
            Assert.Equal(0, result.DaysWithAppointments);
            Assert.Equal(0.0, result.Average, 10);
            Assert.Equal(0.0, result.Median, 10);
            Assert.Equal(0.0, result.Variance, 10);
            Assert.Equal(0.0, result.StandardDeviation, 10);
        }

        [Fact]
        public void Calculate_SwapsReversedDates()
        {
            var daily = new List<DailyCount>
            {
                new DailyCount { Day = "2026-04-03", Count = 4 }
            };

            var result = StatisticsService.Calculate(daily,
                new DateTime(2026, 4, 3), new DateTime(2026, 4, 1));

            Assert.Equal(3, result.DaysInPeriod);
            Assert.Equal(4, result.TotalCount);
            Assert.Equal(4.0 / 3.0, result.Average, 10);
        }

        [Fact]
        public void Calculate_SkipsInvalidRows()
        {
            var daily = new List<DailyCount>
            {
                null,
                new DailyCount { Day = null, Count = 7 },
                new DailyCount { Day = "2026-05-01", Count = 2 }
            };

            var result = StatisticsService.Calculate(daily,
                new DateTime(2026, 5, 1), new DateTime(2026, 5, 1));

            Assert.Equal(1, result.DaysInPeriod);
            Assert.Equal(2, result.TotalCount);
            Assert.Single(result.Daily);
        }
    }
}
