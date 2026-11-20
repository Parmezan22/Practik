using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MedClinic.UI
{
    /// <summary>
    /// Подсказка внутри пустого поля ввода (как в современных программах).
    /// Используется системное сообщение EM_SETCUEBANNER: текст виден только пока
    /// поле пустое и автоматически исчезает, как только пользователь начинает ввод.
    /// Работает в Windows 7 и новее, не требует сторонних библиотек.
    /// </summary>
    public static class PlaceholderHelper
    {
        private const int EM_SETCUEBANNER = 0x1501;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);

        /// <summary>Устанавливает подсказку для текстового поля.</summary>
        public static void SetPlaceholder(TextBox textBox, string text)
        {
            if (textBox == null) throw new ArgumentNullException("textBox");
            if (string.IsNullOrEmpty(text)) return;

            // Показ подсказки и при получении фокуса (wParam = 1)
            var showOnFocus = new IntPtr(1);

            if (textBox.IsHandleCreated)
            {
                SendMessage(textBox.Handle, EM_SETCUEBANNER, showOnFocus, text);
            }
            else
            {
                // Если окно ещё не создано — ждём появления дескриптора
                textBox.HandleCreated += delegate
                {
                    SendMessage(textBox.Handle, EM_SETCUEBANNER, showOnFocus, text);
                };
            }
        }
    }
}
