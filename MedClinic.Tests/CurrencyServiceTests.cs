using System;
using System.Globalization;
using System.Threading;
using MedClinic.Models;
using MedClinic.Services;
using Xunit;

namespace MedClinic.Tests
{
    /// <summary>Разбор ответа сервиса курсов валют (без обращения к интернету).</summary>
    public class CurrencyServiceTests
    {
        private const string SampleJson = @"{
          ""Date"": ""2026-11-19T11:30:00+03:00"",
          ""Valute"": {
            ""USD"": { ""CharCode"": ""USD"", ""Nominal"": 1, ""Name"": ""Доллар США"", ""Value"": 90.5, ""Previous"": 90.1 },
            ""JPY"": { ""CharCode"": ""JPY"", ""Nominal"": 100, ""Name"": ""Японских иен"", ""Value"": 60.2, ""Previous"": 60.5 }
          }
        }";

        [Fact]
        public void Parse_ReadsDateAndRates()
        {
            var result = CurrencyService.Parse(SampleJson);

            Assert.Equal("19.11.2026", result.Date.ToString("dd.MM.yyyy"));
            Assert.Equal(2, result.Rates.Count);
            Assert.Equal("JPY", result.Rates[0].Code);
            Assert.Equal("USD", result.Rates[1].Code);
        }

        [Fact]
        public void Parse_CalculatesRatePerUnitAndChange()
        {
            var usd = Find(CurrencyService.Parse(SampleJson), "USD");
            Assert.Equal(90.5m, usd.Rate);
            Assert.Equal(0.4m, usd.Change);
            Assert.StartsWith("+", usd.ChangeText);
            Assert.Equal(90.5m.ToString("N4"), usd.ValueText);
        }

        [Fact]
        public void Parse_DividesByNominal()
        {
            var jpy = Find(CurrencyService.Parse(SampleJson), "JPY");
            Assert.Equal(0.602m, jpy.Rate);
            Assert.Equal(60.2m, jpy.Value);
        }

        [Fact]
        public void Parse_WorksWithRussianCulture()
        {
            // Раньше запятая в русской культуре превращала 90.5 в 905
            var previous = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
                var usd = Find(CurrencyService.Parse(SampleJson), "USD");
                Assert.Equal(90.5m, usd.Rate);
                Assert.Equal(0.4m, usd.Change);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = previous;
            }
        }

        [Fact]
        public void Parse_ThrowsOnEmptyResponse()
        {
            Assert.Throws<AppException>(delegate { CurrencyService.Parse(""); });
        }

        [Fact]
        public void Parse_ThrowsWhenValuteIsMissing()
        {
            Assert.Throws<AppException>(delegate { CurrencyService.Parse("{\"Date\":\"2026-01-01\"}"); });
        }

        private static CurrencyRate Find(CurrencyRatesResult result, string code)
        {
            var rate = result.Rates.Find(r => r.Code == code);
            Assert.NotNull(rate);
            return rate;
        }
    }
}
