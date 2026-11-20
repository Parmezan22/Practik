using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Windows.Forms;
using MedClinic.Models;

namespace MedClinic.Services
{
    /// <summary>
    /// Печатная форма отчёта «Записи за период».
    /// Показывает предварительный просмотр и позволяет отправить отчёт на принтер.
    /// Таблица рисуется вручную средствами GDI+, внешние библиотеки не нужны.
    /// </summary>
    public static class ReportPrinter
    {
        /// <summary>Открывает окно предварительного просмотра печати.</summary>
        public static void ShowPreview(IWin32Window owner, IList<Appointment> rows,
                                       DateTime from, DateTime to,
                                       string doctorTitle, bool onlyCompleted)
        {
            using (var document = new PeriodReportDocument(rows, from, to, doctorTitle, onlyCompleted))
            using (var preview = new PrintPreviewDialog())
            {
                preview.Document = document;
                preview.Width = 1000;
                preview.Height = 720;
                preview.UseAntiAlias = true;
                preview.StartPosition = FormStartPosition.CenterParent;
                preview.ShowDialog(owner);
            }
        }

        /// <summary>Документ печати: шапка, таблица, итоги, номера страниц.</summary>
        private class PeriodReportDocument : PrintDocument
        {
            private static readonly string[] Headers =
            {
                "Дата", "Время", "Пациент", "Врач", "Услуга", "Статус", "Стоимость"
            };

            // Доли ширины столбцов (в сумме 100)
            private static readonly float[] Weights = { 9f, 7f, 25f, 24f, 20f, 8f, 7f };

            private readonly IList<Appointment> _rows;
            private readonly DateTime _from;
            private readonly DateTime _to;
            private readonly string _doctorTitle;
            private readonly bool _onlyCompleted;

            private int _rowIndex;
            private int _pageNumber = 1;
            private decimal _sum;

            public PeriodReportDocument(IList<Appointment> rows, DateTime from, DateTime to,
                                        string doctorTitle, bool onlyCompleted)
            {
                _rows = rows ?? new List<Appointment>();
                _from = from;
                _to = to;
                _doctorTitle = doctorTitle;
                _onlyCompleted = onlyCompleted;

                DocumentName = "Отчёт «Записи за период»";
                DefaultPageSettings.Landscape = true;
                DefaultPageSettings.Margins = new Margins(40, 30, 40, 40);
            }

            protected override void OnBeginPrint(PrintEventArgs e)
            {
                _rowIndex = 0;
                _pageNumber = 1;
                _sum = 0;

                base.OnBeginPrint(e);
            }

            protected override void OnPrintPage(PrintPageEventArgs e)
            {
                base.OnPrintPage(e);

                var bounds = e.MarginBounds;
                var graphics = e.Graphics;

                // Ширины столбцов в единицах страницы (1/100 дюйма)
                var widths = new float[Headers.Length];
                float totalWeight = 0;
                foreach (var weight in Weights) totalWeight += weight;

                for (var i = 0; i < widths.Length; i++)
                    widths[i] = bounds.Width * Weights[i] / totalWeight;

                float y = DrawHeader(graphics, bounds);

                const float rowHeight = 26f;
                const float footerHeight = 40f;
                float bottom = bounds.Bottom - footerHeight;

                y = DrawTableHeader(graphics, bounds.Left, y, widths, rowHeight);

                using (var rowFont = new Font("Arial", 8f))
                using (var rowBrush = new SolidBrush(Color.FromArgb(33, 37, 41)))
                using (var linePen = new Pen(Color.FromArgb(220, 226, 232), 0.5f))
                using (var alternateBrush = new SolidBrush(Color.FromArgb(248, 250, 252)))
                using (var leftFormat = CreateFormat(StringAlignment.Near))
                using (var centerFormat = CreateFormat(StringAlignment.Center))
                using (var rightFormat = CreateFormat(StringAlignment.Far))
                {
                    while (_rowIndex < _rows.Count)
                    {
                        if (y + rowHeight > bottom)
                        {
                            DrawFooter(graphics, bounds, footerHeight, rowFont);
                            e.HasMorePages = true;
                            _pageNumber++;
                            return;
                        }

                        var item = _rows[_rowIndex];
                        float x = bounds.Left;

                        // Чередование фона строк
                        if (_rowIndex % 2 == 1)
                            graphics.FillRectangle(alternateBrush, x, y, bounds.Width, rowHeight);

                        DrawText(graphics, item.DateText, rowFont, rowBrush, x, y, widths[0], rowHeight, centerFormat);
                        x += widths[0];
                        DrawText(graphics, item.Time, rowFont, rowBrush, x, y, widths[1], rowHeight, centerFormat);
                        x += widths[1];
                        DrawText(graphics, item.PatientName, rowFont, rowBrush, x, y, widths[2], rowHeight, leftFormat);
                        x += widths[2];
                        DrawText(graphics, item.DoctorName, rowFont, rowBrush, x, y, widths[3], rowHeight, leftFormat);
                        x += widths[3];
                        DrawText(graphics, item.ServiceName, rowFont, rowBrush, x, y, widths[4], rowHeight, leftFormat);
                        x += widths[4];
                        DrawText(graphics, item.StatusText, rowFont, rowBrush, x, y, widths[5], rowHeight, centerFormat);
                        x += widths[5];
                        DrawText(graphics, item.TotalPrice.ToString("N2"), rowFont, rowBrush,
                                 x, y, widths[6], rowHeight, rightFormat);

                        // Горизонтальная линия под строкой
                        graphics.DrawLine(linePen, bounds.Left, y + rowHeight, bounds.Right, y + rowHeight);

                        if (item.Status != AppointmentStatus.Cancelled)
                            _sum += item.TotalPrice;

                        _rowIndex++;
                        y += rowHeight;
                    }

                    // Итоговая строка
                    if (y + rowHeight * 1.6f > bottom)
                    {
                        DrawFooter(graphics, bounds, footerHeight, rowFont);
                        e.HasMorePages = true;
                        _pageNumber++;
                        return;
                    }

                    y += 12;

                    using (var totalFont = new Font("Arial", 9.5f, FontStyle.Bold))
                    using (var totalBrush = new SolidBrush(Color.FromArgb(23, 74, 105)))
                    {
                        var text = "Итого записей: " + _rows.Count +
                                   "     Сумма (без отменённых): " + _sum.ToString("N2") + " руб.";
                        DrawText(graphics, text, totalFont, totalBrush,
                                 bounds.Left, y, bounds.Width, 22f, rightFormat);
                    }

                    DrawFooter(graphics, bounds, footerHeight, rowFont);
                    e.HasMorePages = false;
                }
            }

            /// <summary>Рисует шапку отчёта и возвращает Y для таблицы.</summary>
            private float DrawHeader(Graphics graphics, Rectangle bounds)
            {
                float y = bounds.Top;

                using (var titleFont = new Font("Arial", 14f, FontStyle.Bold))
                using (var subtitleFont = new Font("Arial", 10f))
                using (var infoFont = new Font("Arial", 8.5f))
                using (var smallFont = new Font("Arial", 8f))
                using (var darkBrush = new SolidBrush(Color.FromArgb(23, 74, 105)))
                using (var grayBrush = new SolidBrush(Color.FromArgb(108, 117, 125)))
                using (var centerFormat = CreateFormat(StringAlignment.Center))
                using (var rightFormat = CreateFormat(StringAlignment.Far))
                {
                    DrawText(graphics, "Медицинский центр «Здоровье+»", titleFont, darkBrush,
                             bounds.Left, y, bounds.Width, 28f, centerFormat);
                    y += 28f;

                    DrawText(graphics, "Отчёт «Записи за период»", subtitleFont, darkBrush,
                             bounds.Left, y, bounds.Width, 20f, centerFormat);
                    y += 20f;

                    var info = "Период: " + _from.ToString("dd.MM.yyyy") + " — " +
                               _to.ToString("dd.MM.yyyy") + "     Врач: " + _doctorTitle +
                               (_onlyCompleted ? "     Только завершённые" : string.Empty);

                    DrawText(graphics, info, infoFont, grayBrush,
                             bounds.Left, y, bounds.Width, 18f, centerFormat);
                    y += 18f;

                    DrawText(graphics, "Сформировано: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm"),
                             smallFont, grayBrush, bounds.Left, y, bounds.Width, 16f, rightFormat);
                    y += 20f;
                }

                using (var pen = new Pen(Color.FromArgb(23, 74, 105), 1.2f))
                    graphics.DrawLine(pen, bounds.Left, y, bounds.Right, y);

                return y + 6f;
            }

            /// <summary>Рисует строку заголовков таблицы.</summary>
            private float DrawTableHeader(Graphics graphics, float left, float y,
                                          float[] widths, float rowHeight)
            {
                using (var background = new SolidBrush(Color.FromArgb(23, 74, 105)))
                using (var font = new Font("Arial", 8.5f, FontStyle.Bold))
                using (var brush = new SolidBrush(Color.White))
                using (var format = CreateFormat(StringAlignment.Center))
                {
                    graphics.FillRectangle(background, left, y, Sum(widths), rowHeight + 2f);

                    var x = left;
                    for (var i = 0; i < Headers.Length; i++)
                    {
                        DrawText(graphics, Headers[i], font, brush, x, y, widths[i], rowHeight, format);
                        x += widths[i];
                    }
                }

                return y + rowHeight + 2f;
            }

            /// <summary>Рисует нижнюю часть страницы: линию и номер страницы.</summary>
            private void DrawFooter(Graphics graphics, Rectangle bounds, float footerHeight, Font font)
            {
                var lineY = bounds.Bottom - footerHeight + 12f;

                using (var pen = new Pen(Color.FromArgb(200, 208, 215), 0.8f))
                using (var grayBrush = new SolidBrush(Color.FromArgb(108, 117, 125)))
                using (var leftFormat = CreateFormat(StringAlignment.Near))
                using (var rightFormat = CreateFormat(StringAlignment.Far))
                {
                    graphics.DrawLine(pen, bounds.Left, lineY, bounds.Right, lineY);

                    DrawText(graphics, "Система записи на приём", font, grayBrush,
                             bounds.Left, lineY + 2f, bounds.Width / 2f, 18f, leftFormat);

                    DrawText(graphics, "Страница " + _pageNumber, font, grayBrush,
                             bounds.Left + bounds.Width / 2f, lineY + 2f,
                             bounds.Width / 2f, 18f, rightFormat);
                }
            }

            /// <summary>Рисует текст в прямоугольнике с выравниванием и многоточием.</summary>
            private static void DrawText(Graphics graphics, string text, Font font, Brush brush,
                                         float x, float y, float width, float height,
                                         StringFormat format)
            {
                var rectangle = new RectangleF(x + 3f, y, width - 6f, height);
                graphics.DrawString(text ?? string.Empty, font, brush, rectangle, format);
            }

            private static StringFormat CreateFormat(StringAlignment alignment)
            {
                var format = new StringFormat();
                format.Alignment = alignment;
                format.LineAlignment = StringAlignment.Center;
                format.Trimming = StringTrimming.EllipsisCharacter;
                format.FormatFlags = StringFormatFlags.NoWrap;
                return format;
            }

            private static float Sum(float[] values)
            {
                float result = 0;
                foreach (var value in values) result += value;
                return result;
            }
        }
    }
}
