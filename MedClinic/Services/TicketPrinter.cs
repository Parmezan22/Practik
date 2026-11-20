using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using MedClinic.Models;

namespace MedClinic.Services
{
    /// <summary>
    /// Талон на приём (модуль Г): номер записи, пациент, врач, кабинет,
    /// дата и интервал приёма, услуги и сумма. Печать с предварительным просмотром.
    /// </summary>
    public static class TicketPrinter
    {
        /// <summary>
        /// Строки талона. Вынесены в отдельный метод, чтобы содержимое проверялось тестами
        /// без принтера.
        /// </summary>
        public static List<string> BuildLines(Appointment appointment, Doctor doctor,
                                              IList<Service> services, string clinicName)
        {
            if (appointment == null) throw new ArgumentNullException("appointment");

            var ru = CultureInfo.GetCultureInfo("ru-RU");
            var list = services ?? new List<Service>();
            var minutes = list.Sum(s => s.Duration > 0 ? s.Duration : 30);
            if (minutes <= 0) minutes = 30;

            var lines = new List<string>
            {
                string.IsNullOrWhiteSpace(clinicName) ? "Медицинский центр" : clinicName,
                "ТАЛОН НА ПРИЁМ № " + appointment.Id.ToString("000000", CultureInfo.InvariantCulture),
                "",
                "Пациент: " + appointment.PatientName,
                "Врач: " + (doctor != null ? doctor.FullName : appointment.DoctorName),
            };

            if (doctor != null && !string.IsNullOrWhiteSpace(doctor.Specialty))
                lines.Add("Специальность: " + doctor.Specialty);
            if (doctor != null && !string.IsNullOrWhiteSpace(doctor.Cabinet))
                lines.Add("Кабинет: " + doctor.Cabinet);

            int start;
            var interval = appointment.Time;
            if (TimeRange.TryParseMinutes(appointment.Time, out start))
                interval = TimeRange.Format(start) + "–" + TimeRange.Format(start + minutes);

            lines.Add("Дата: " + appointment.Date.ToString("dd.MM.yyyy (dddd)", ru));
            lines.Add("Время: " + interval + " (" + minutes + " мин.)");
            lines.Add("");
            lines.Add("Услуги:");

            decimal total = 0;
            foreach (var service in list)
            {
                lines.Add("  " + service.Name + " - " + service.Price.ToString("N2", ru) + " руб.");
                total += service.Price;
            }

            lines.Add("Итого к оплате: " + total.ToString("N2", ru) + " руб.");
            lines.Add("");
            lines.Add("Просьба прийти за 10 минут до начала приёма.");
            lines.Add("Отменить запись можно по телефону регистратуры.");
            return lines;
        }

        /// <summary>Открывает предварительный просмотр талона, оттуда его можно напечатать.</summary>
        public static void ShowPreview(IWin32Window owner, IList<string> lines)
        {
            using (var document = new TicketDocument(lines))
            using (var preview = new PrintPreviewDialog())
            {
                preview.Document = document;
                preview.Width = 700;
                preview.Height = 800;
                preview.UseAntiAlias = true;
                preview.StartPosition = FormStartPosition.CenterParent;
                preview.ShowDialog(owner);
            }
        }

        /// <summary>Документ печати: рамка, заголовок крупным шрифтом, строки талона.</summary>
        private class TicketDocument : PrintDocument
        {
            private readonly IList<string> _lines;

            public TicketDocument(IList<string> lines)
            {
                _lines = lines ?? new List<string>();
                DocumentName = "Талон на приём";
            }

            protected override void OnPrintPage(PrintPageEventArgs e)
            {
                base.OnPrintPage(e);

                var area = e.MarginBounds;
                var box = new RectangleF(area.Left, area.Top, Math.Min(area.Width, 420), 0);

                using (var title = new Font("Segoe UI", 13f, FontStyle.Bold))
                using (var text = new Font("Segoe UI", 10.5f))
                using (var pen = new Pen(Color.Black, 1.5f))
                {
                    var y = box.Top + 12;
                    for (var i = 0; i < _lines.Count; i++)
                    {
                        var font = i < 2 ? title : text;
                        e.Graphics.DrawString(_lines[i], font, Brushes.Black, box.Left + 14, y);
                        y += font.GetHeight(e.Graphics) + 4;
                    }

                    e.Graphics.DrawRectangle(pen, box.Left, box.Top, box.Width, y - box.Top + 10);
                }

                e.HasMorePages = false;
            }
        }
    }
}
