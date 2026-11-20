using System;
using System.Collections.Generic;
using MedClinic.Models;
using MedClinic.Services;
using Xunit;

namespace MedClinic.Tests
{
    /// <summary>Проверка содержимого талона на приём.</summary>
    public class TicketPrinterTests
    {
        private static Appointment Sample()
        {
            return new Appointment
            {
                Id = 42,
                PatientName = "Иванов Иван Иванович",
                DoctorName = "Петрова А. С.",
                Date = new DateTime(2026, 11, 24),
                Time = "09:00"
            };
        }

        private static List<Service> Services()
        {
            return new List<Service>
            {
                new Service { Name = "Консультация", Price = 1500m, Duration = 30 },
                new Service { Name = "УЗИ", Price = 2000m, Duration = 40 }
            };
        }

        [Fact]
        public void Ticket_ContainsNumberCabinetAndInterval()
        {
            var doctor = new Doctor { FullName = "Петрова Анна Сергеевна", Specialty = "Терапевт", Cabinet = "12" };

            var lines = TicketPrinter.BuildLines(Sample(), doctor, Services(), "Здоровье+");

            Assert.Equal("Здоровье+", lines[0]);
            Assert.Contains("ТАЛОН НА ПРИЁМ № 000042", lines);
            Assert.Contains("Кабинет: 12", lines);
            Assert.Contains("Время: 09:00–10:10 (70 мин.)", lines);
            Assert.Contains(lines, l => l.StartsWith("Итого к оплате:") && l.Contains("500,00"));
        }

        [Fact]
        public void Ticket_UsesDoctorNameFromAppointment_WhenDoctorUnknown()
        {
            var lines = TicketPrinter.BuildLines(Sample(), null, Services(), null);

            Assert.Equal("Медицинский центр", lines[0]);
            Assert.Contains("Врач: Петрова А. С.", lines);
        }

        [Fact]
        public void Ticket_RequiresAppointment()
        {
            Assert.Throws<ArgumentNullException>(() =>
                TicketPrinter.BuildLines(null, null, Services(), "Клиника"));
        }
    }
}
