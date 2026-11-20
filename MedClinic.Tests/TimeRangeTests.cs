using System;
using MedClinic.Services;
using Xunit;

namespace MedClinic.Tests
{
    /// <summary>Проверка расчёта времени и пересечения интервалов приёма.</summary>
    public class TimeRangeTests
    {
        [Theory]
        [InlineData("08:00", 480)]
        [InlineData("09:30", 570)]
        [InlineData("19:30", 1170)]
        [InlineData("00:00", 0)]
        public void ParseMinutes_ValidTime(string time, int expected)
        {
            Assert.Equal(expected, TimeRange.ParseMinutes(time));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("8:00")]
        [InlineData("25:00")]
        [InlineData("09:70")]
        [InlineData("09-00")]
        public void ParseMinutes_InvalidTime(string time)
        {
            Assert.Throws<ArgumentException>(delegate { TimeRange.ParseMinutes(time); });
        }

        [Theory]
        [InlineData(0, "00:00")]
        [InlineData(480, "08:00")]
        [InlineData(570, "09:30")]
        [InlineData(1200, "20:00")]
        public void Format_ReturnsHourAndMinutes(int minutes, string expected)
        {
            Assert.Equal(expected, TimeRange.Format(minutes));
        }

        [Fact]
        public void Overlaps_DetectsIntersection()
        {
            // 09:00–10:00 и 09:30–10:30 пересекаются
            Assert.True(TimeRange.Overlaps(540, 60, 570, 60));
        }

        [Fact]
        public void Overlaps_AllowsTouchingBoundaries()
        {
            // 09:00–09:30 и 09:30–10:00 идут друг за другом и не пересекаются
            Assert.False(TimeRange.Overlaps(540, 30, 570, 30));
            Assert.False(TimeRange.Overlaps(570, 30, 540, 30));
        }

        [Fact]
        public void Overlaps_DetectsContainment()
        {
            // Короткая запись внутри длинной
            Assert.True(TimeRange.Overlaps(540, 120, 570, 30));
            Assert.True(TimeRange.Overlaps(570, 30, 540, 120));
        }

        [Fact]
        public void Overlaps_UsesThirtyMinutesForZeroDuration()
        {
            // Нулевая длительность считается получасовым приёмом
            Assert.True(TimeRange.Overlaps(540, 0, 555, 0));
            Assert.False(TimeRange.Overlaps(540, 0, 570, 0));
        }

        [Fact]
        public void Overlaps_SeparateDaysDoNotMatterWithinOneDay()
        {
            // Проверка работает в пределах одного дня; 19:30 + 30 мин = 20:00
            Assert.Equal(20 * 60, TimeRange.ParseMinutes("19:30") + 30);
        }
    }
}
