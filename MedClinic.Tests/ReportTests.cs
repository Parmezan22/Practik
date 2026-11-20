using System;
using System.Collections.Generic;
using System.IO;
using MedClinic.Models;
using MedClinic.Services;
using OfficeOpenXml;
using Xunit;

namespace MedClinic.Tests
{
    /// <summary>Стоимость записи и экспорт отчёта в Excel.</summary>
    public class ReportTests
    {
        [Fact]
        public void TotalPrice_SumsPrimaryAndExtraServices()
        {
            var appointment = new Appointment { Price = 1500m, ExtraPrice = 450.50m };

            Assert.Equal(1950.50m, appointment.TotalPrice);
            Assert.Equal(1950.50m.ToString("N2"), appointment.TotalPriceText);
        }

        [Fact]
        public void ExcelExport_CreatesFileWithHeaderAndTotal()
        {
            var path = Path.Combine(Path.GetTempPath(),
                "medclinic-test-" + Guid.NewGuid().ToString("N") + ".xlsx");

            var rows = new List<Appointment>
            {
                new Appointment
                {
                    PatientName = "Иванов Иван Иванович",
                    DoctorName = "Петрова Анна Сергеевна",
                    ServiceName = "Приём терапевта",
                    Date = new DateTime(2026, 11, 19),
                    Time = "10:00",
                    Status = AppointmentStatus.Completed,
                    Price = 1500m,
                    ExtraPrice = 500m
                },
                new Appointment
                {
                    PatientName = "Сидоров Пётр Петрович",
                    DoctorName = "Петрова Анна Сергеевна",
                    ServiceName = "Приём терапевта",
                    Date = new DateTime(2026, 11, 20),
                    Time = "12:00",
                    Status = AppointmentStatus.Cancelled,
                    Price = 700m
                }
            };

            try
            {
                ExcelReportService.Export(path,
                    new DateTime(2026, 11, 1), new DateTime(2026, 11, 30),
                    "Все врачи", false, rows);

                Assert.True(File.Exists(path));
                Assert.True(new FileInfo(path).Length > 0);

                using (var package = new ExcelPackage(new FileInfo(path)))
                {
                    var sheet = package.Workbook.Worksheets[1];

                    Assert.Equal("Отчёт «Записи за период»", sheet.Cells["A1"].Text);
                    Assert.Contains("01.11.2026", sheet.Cells["A2"].Text);
                    Assert.Contains("Период", sheet.Cells["A2"].Text);

                    // Строки 6–7 — данные, строка 8 — итог
                    Assert.Equal("Дата", sheet.Cells[5, 1].Text);
                    Assert.Equal("Иванов Иван Иванович", sheet.Cells[6, 3].Text);
                    Assert.Equal(2000m, sheet.Cells[6, 7].GetValue<decimal>());
                    Assert.Equal("Сидоров Пётр Петрович", sheet.Cells[7, 3].Text);
                    Assert.Equal(2000m, sheet.Cells[8, 7].GetValue<decimal>());
                    Assert.Equal("Итого записей: 2", sheet.Cells[8, 1].Text);
                    Assert.Contains("без отменённых", sheet.Cells["A3"].Text);
                }
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        [Fact]
        public void ExcelExport_RejectsEmptyPath()
        {
            Assert.Throws<ArgumentException>(delegate
            {
                ExcelReportService.Export("", DateTime.Today, DateTime.Today,
                    "Все врачи", false, new List<Appointment>());
            });
        }
    }
}
