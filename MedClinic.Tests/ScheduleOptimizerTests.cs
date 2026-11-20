using System.Collections.Generic;
using MedClinic.Services;
using Xunit;

namespace MedClinic.Tests
{
    /// <summary>Проверка подбора времени приёма и расчёта свободных окон врача.</summary>
    public class ScheduleOptimizerTests
    {
        private static TimeWindow W(int start, int end)
        {
            return new TimeWindow(start, end);
        }

        [Fact]
        public void FreeWindows_AreGapsBetweenBusyIntervals()
        {
            // занято 08:00–09:00 и 09:30–10:00
            var free = ScheduleOptimizer.FreeWindows(new[] { W(480, 540), W(570, 600) }, 480, 1200);

            Assert.Equal(2, free.Count);
            Assert.Equal(540, free[0].StartMinute);
            Assert.Equal(570, free[0].EndMinute);
            Assert.Equal(600, free[1].StartMinute);
            Assert.Equal(1200, free[1].EndMinute);
        }

        [Fact]
        public void FreeWindows_MergesOverlappingIntervals()
        {
            var free = ScheduleOptimizer.FreeWindows(new[] { W(520, 600), W(480, 540) }, 480, 1200);

            Assert.Single(free);
            Assert.Equal(600, free[0].StartMinute);
        }

        [Fact]
        public void FreeWindows_IgnoresTimeBeforeWorkday()
        {
            var free = ScheduleOptimizer.FreeWindows(new[] { W(400, 500) }, 480, 1200);

            Assert.Equal(500, free[0].StartMinute);
        }

        [Fact]
        public void BestFit_TakesShortestWindow_FirstFit_TakesEarliest()
        {
            // свободно: 08:30–10:00 (90 мин), 12:00–12:40 (40 мин), с 13:00 до конца дня
            var busy = new List<TimeWindow> { W(480, 510), W(600, 720), W(760, 780) };

            var first = ScheduleOptimizer.FirstFit(busy, 40, ScheduleOptimizer.WorkStart);
            var best = ScheduleOptimizer.BestFit(busy, 40, ScheduleOptimizer.WorkStart);

            Assert.Equal(510, first.StartMinute);
            Assert.Equal(720, best.StartMinute);
            Assert.Equal(760, best.EndMinute);
        }

        [Fact]
        public void BestFit_StartsNotBeforeGivenMinute()
        {
            var slot = ScheduleOptimizer.BestFit(new List<TimeWindow>(), 30, 601);

            Assert.Equal(605, slot.StartMinute);
            Assert.Equal("10:05", slot.StartText);
        }

        [Fact]
        public void BestFit_ReturnsNull_WhenNoWindowIsLongEnough()
        {
            // свободно только 19:40–20:00
            var slot = ScheduleOptimizer.BestFit(new[] { W(480, 1180) }, 30, ScheduleOptimizer.WorkStart);

            Assert.Null(slot);
        }

        [Fact]
        public void Utilization_IsShareOfBusyTime()
        {
            var load = ScheduleOptimizer.Utilization(new[] { W(480, 540), W(600, 660) }, 480, 1200);

            Assert.Equal(120.0 / 720.0, load, 6);
        }

        [Theory]
        [InlineData(600, 600)]
        [InlineData(601, 605)]
        [InlineData(604, 605)]
        public void AlignUp_RoundsToFiveMinutes(int minute, int expected)
        {
            Assert.Equal(expected, ScheduleOptimizer.AlignUp(minute));
        }
    }
}
