using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using MedClinic.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace MedClinic.Services
{
    /// <summary>
    /// Экспорт отчёта «Записи за период» в книгу Excel (.xlsx).
    /// Используется библиотека EPPlus версии 4.5 (бесплатная для учебных проектов).
    /// </summary>
    public static class ExcelReportService
    {
        /// <summary>Заголовки столбцов таблицы отчёта.</summary>
        private static readonly string[] Headers =
        {
            "Дата", "Время", "Пациент", "Врач", "Услуга", "Статус", "Стоимость, руб."
        };

        /// <summary>Создаёт и сохраняет файл Excel с отчётом.</summary>
        public static void Export(string path, DateTime from, DateTime to,
                                  string doctorTitle, bool onlyCompleted,
                                  IList<Appointment> rows)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Не указан путь для сохранения файла.", "path");
            if (rows == null)
                throw new ArgumentNullException("rows");

            var file = new FileInfo(path);

            using (var package = new ExcelPackage())
            {
                package.Workbook.Properties.Title = "Записи за период";
                package.Workbook.Properties.Author = "Система записи на приём";
                package.Workbook.Properties.Company = "Медицинский центр «Здоровье+»";

                var sheet = package.Workbook.Worksheets.Add("Записи за период");

                // ------------------------- ШАПКА ОТЧЁТА -------------------------
                sheet.Cells["A1:G1"].Merge = true;
                sheet.Cells["A1"].Value = "Отчёт «Записи за период»";
                sheet.Cells["A1"].Style.Font.Size = 16;
                sheet.Cells["A1"].Style.Font.Bold = true;
                sheet.Cells["A1"].Style.Font.Color.SetColor(Color.FromArgb(23, 74, 105));
                sheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Row(1).Height = 26;

                sheet.Cells["A2:G2"].Merge = true;
                sheet.Cells["A2"].Value = "Период: " + from.ToString("dd.MM.yyyy") +
                                          " — " + to.ToString("dd.MM.yyyy") +
                                          "     Врач: " + doctorTitle +
                                          (onlyCompleted ? "     Только завершённые" : string.Empty);
                sheet.Cells["A2"].Style.Font.Size = 10;
                sheet.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                sheet.Cells["A3:G3"].Merge = true;
                sheet.Cells["A3"].Value = "Сформировано: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm") +
                                          "     Сумма указана без отменённых записей";
                sheet.Cells["A3"].Style.Font.Size = 9;
                sheet.Cells["A3"].Style.Font.Color.SetColor(Color.Gray);
                sheet.Cells["A3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                // ------------------------- ШАПКА ТАБЛИЦЫ -------------------------
                const int headerRow = 5;
                for (var i = 0; i < Headers.Length; i++)
                {
                    var cell = sheet.Cells[headerRow, i + 1];
                    cell.Value = Headers[i];
                    cell.Style.Font.Bold = true;
                    cell.Style.Font.Color.SetColor(Color.White);
                    cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(23, 74, 105));
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }
                sheet.Row(headerRow).Height = 22;

                // ------------------------- СТРОКИ ТАБЛИЦЫ -------------------------
                var row = headerRow + 1;
                decimal sum = 0;

                foreach (var item in rows)
                {
                    if (item == null) continue;

                    sheet.Cells[row, 1].Value = item.DateText;
                    sheet.Cells[row, 2].Value = item.Time;
                    sheet.Cells[row, 3].Value = item.PatientName;
                    sheet.Cells[row, 4].Value = item.DoctorName;
                    sheet.Cells[row, 5].Value = item.ServiceName;
                    sheet.Cells[row, 6].Value = item.StatusText;
                    sheet.Cells[row, 7].Value = item.TotalPrice;

                    sheet.Cells[row, 7].Style.Numberformat.Format = "#,##0.00";
                    if (item.Status == AppointmentStatus.Cancelled)
                        sheet.Cells[row, 1, row, 7].Style.Font.Color.SetColor(Color.Gray);
                    else
                        sum += item.TotalPrice; // отменённые приёмы в сумму не входят

                    row++;
                }

                var lastDataRow = row - 1;

                // ------------------------- ИТОГИ -------------------------
                sheet.Cells[row, 1, row, 6].Merge = true;
                sheet.Cells[row, 1].Value = "Итого записей: " + rows.Count;
                sheet.Cells[row, 1].Style.Font.Bold = true;
                sheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                sheet.Cells[row, 7].Value = sum;
                sheet.Cells[row, 7].Style.Font.Bold = true;
                sheet.Cells[row, 7].Style.Numberformat.Format = "#,##0.00";

                // Рамки вокруг всей таблицы (включая строку итогов)
                var table = sheet.Cells[headerRow, 1, row, 7];
                table.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                table.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                table.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                table.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                table.Style.Border.Top.Color.SetColor(Color.FromArgb(200, 208, 215));
                table.Style.Border.Bottom.Color.SetColor(Color.FromArgb(200, 208, 215));
                table.Style.Border.Left.Color.SetColor(Color.FromArgb(200, 208, 215));
                table.Style.Border.Right.Color.SetColor(Color.FromArgb(200, 208, 215));

                // ------------------------- ОФОРМЛЕНИЕ -------------------------
                sheet.Column(1).Width = 12;
                sheet.Column(2).Width = 8;
                sheet.Column(3).Width = 30;
                sheet.Column(4).Width = 30;
                sheet.Column(5).Width = 28;
                sheet.Column(6).Width = 12;
                sheet.Column(7).Width = 16;

                sheet.Cells[headerRow, 1, lastDataRow, 7].Style.VerticalAlignment =
                    ExcelVerticalAlignment.Center;

                sheet.View.FreezePanes(headerRow + 1, 1);
                if (lastDataRow >= headerRow + 1)
                    sheet.Cells[headerRow, 1, lastDataRow, 7].AutoFilter = true;

                // Параметры печати листа
                sheet.PrinterSettings.Orientation = eOrientation.Landscape;
                sheet.PrinterSettings.FitToPage = true;
                sheet.PrinterSettings.FitToWidth = 1;
                sheet.PrinterSettings.FitToHeight = 0;

                package.SaveAs(file);
            }
        }
    }
}
