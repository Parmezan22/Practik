using System;
using System.Globalization;

namespace MedClinic.Services
{
    /// <summary>
    /// Работа с временем приёма в минутах от начала суток.
    /// Помогает проверить, пересекаются ли два интервала:
    /// запись с длительностью 60 минут занимает не одну «точку», а целый час.
    /// </summary>
    public static class TimeRange
    {
        /// <summary>Разбирает время "ЧЧ:мм" в минуты от 00:00. false — если формат неверный.</summary>
        public static bool TryParseMinutes(string time, out int minutes)
        {
            minutes = 0;

            if (string.IsNullOrWhiteSpace(time)) return false;

            var text = time.Trim();

            // Строгий формат ЧЧ:мм — как в базе данных и в интерфейсе
            if (text.Length != 5 || text[2] != ':') return false;

            var parts = text.Split(':');
            if (parts.Length != 2) return false;

            int hours;
            int mins;

            if (!int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out hours))
                return false;

            if (!int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out mins))
                return false;

            if (hours < 0 || hours > 23 || mins < 0 || mins > 59)
                return false;

            minutes = hours * 60 + mins;
            return true;
        }

        /// <summary>Переводит время "ЧЧ:мм" в минуты. При ошибке — исключение.</summary>
        public static int ParseMinutes(string time)
        {
            int minutes;
            if (!TryParseMinutes(time, out minutes))
                throw new ArgumentException("Время должно быть в формате ЧЧ:мм.", "time");

            return minutes;
        }

        /// <summary>Переводит минуты от 00:00 обратно в текст "ЧЧ:мм".</summary>
        public static string Format(int minutes)
        {
            if (minutes < 0) minutes = 0;

            var hours = minutes / 60;
            var mins = minutes % 60;

            return hours.ToString("00", CultureInfo.InvariantCulture) + ":" +
                   mins.ToString("00", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// true, если интервалы пересекаются.
        /// Касание границ пересечением не считается: 09:00–09:30 и 09:30–10:00 совместимы.
        /// Нулевая или отрицательная длительность заменяется на 30 минут.
        /// </summary>
        public static bool Overlaps(int startA, int durationA, int startB, int durationB)
        {
            if (durationA <= 0) durationA = 30;
            if (durationB <= 0) durationB = 30;

            return startA < startB + durationB && startB < startA + durationA;
        }
    }
}
