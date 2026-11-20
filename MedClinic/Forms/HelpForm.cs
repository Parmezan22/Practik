using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using MedClinic.Services;
using Serilog;

namespace MedClinic.Forms
{
    /// <summary>
    /// Встроенная справка: краткая инструкция и горячие клавиши.
    /// Открывается по F1, из меню «Справка» и с главной страницы.
    /// </summary>
    public partial class HelpForm : Form
    {
        public HelpForm()
        {
            InitializeComponent();

            richTextHelp.Text = BuildHelpText();
            richTextHelp.SelectionStart = 0;
            richTextHelp.SelectionLength = 0;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>Открывает полное руководство пользователя из папки docs.</summary>
        private void btnOpenManual_Click(object sender, EventArgs e)
        {
            try
            {
                var path = FindManual();

                if (path == null)
                {
                    MessageBox.Show(
                        "Файл руководства не найден.\n\n" +
                        "Он находится в папке docs рядом с программой: " +
                        "docs\\Руководство_пользователя.md",
                        "Руководство", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Не удалось открыть руководство пользователя");
                MessageBox.Show("Не удалось открыть руководство.\n\n" + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>Ищет руководство рядом с программой и в папке проекта.</summary>
        private static string FindManual()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;

            var candidates = new[]
            {
                Path.Combine(baseDir, "docs", "Руководство_пользователя.md"),
                Path.Combine(baseDir, "..", "..", "..", "docs", "Руководство_пользователя.md"),
                Path.Combine(baseDir, "..", "..", "..", "..", "docs", "Руководство_пользователя.md")
            };

            foreach (var candidate in candidates)
            {
                var fullPath = Path.GetFullPath(candidate);
                if (File.Exists(fullPath)) return fullPath;
            }

            return null;
        }

        /// <summary>Текст справки. Оформлен простыми абзацами, чтобы легко читался.</summary>
        private static string BuildHelpText()
        {
            return
@"БЫСТРЫЙ СТАРТ

1. Войдите под своей учётной записью.
2. Выберите раздел в меню сверху или на главной странице.
3. В справочниках нажмите «Добавить», чтобы создать врача или услугу.
4. Чтобы записать пациента, нажмите «Новая запись» (Ctrl+N).
5. В разделе «Отчёты» выберите период и нажмите «Построить».

ГЛАВНАЯ СТРАНИЦА

• Карточки с числами кликабельны: они открывают нужный раздел.
• Карточка «Записей сегодня» сразу включает фильтр «Только сегодня».
• Кнопки справа открывают частые действия и эту справку.

ЗАПИСИ НА ПРИЁМ

• ФИО пациента — не менее двух слов, только буквы, пробел и дефис.
• Можно выбрать основную и несколько дополнительных услуг.
• Стоимость и длительность пересчитываются автоматически.
• Время проверяется с учётом длительности услуг: пересечения не допускаются.
• Двойной щелчок по активной записи завершает приём.
• Отменённые записи не занимают время и не входят в сумму отчёта.

ОТЧЁТЫ И ПЕЧАТЬ

• Период по умолчанию — с первого числа текущего месяца по сегодня.
• Кнопка «В Excel» сохраняет книгу .xlsx, «Печать» открывает предпросмотр.
• В сумму включаются только неотменённые записи.
• Кнопка «Курсы валют» загружает официальные курсы ЦБ РФ (нужен интернет).

ГОРЯЧИЕ КЛАВИШИ

Ctrl+N      — новая запись на приём
Ctrl+1      — справочник врачей
Ctrl+2      — справочник услуг
Ctrl+3      — журнал записей
Ctrl+4      — отчёт за период
Ctrl+5      — статистика
Ctrl+6      — пользователи (только админ)
Ctrl+B      — резервная копия базы
F1          — эта справка
Enter       — основное действие в форме
Esc         — закрыть окно без сохранения

ЕСЛИ ЧТО-ТО НЕ РАБОТАЕТ

• «Время занято» — выберите другое время или другого врача.
• «Не удалось подключиться к серверу курсов» — проверьте интернет.
• «Не удалось сохранить файл Excel» — закройте файл в Excel и повторите.
• Подробности ошибок всегда записываются в папку Logs.
• Перед важными изменениями делайте резервную копию: Файл → Резервная копия базы.

Полная инструкция открывается кнопкой «Открыть руководство» внизу окна.";
        }
    }
}
