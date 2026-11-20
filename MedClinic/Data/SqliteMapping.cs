using System;
using System.Globalization;
using System.Data;
using Dapper;

namespace MedClinic.Data
{
    /// <summary>
    /// Обработчик типов Dapper для даты.
    ///
    /// Важно: Dapper НЕ применяет TypeHandler к перечислениям — он всегда
    /// превращает enum в число. Поэтому роль и статус в моделях хранятся
    /// текстовыми полями RoleDb и StatusDb, которые переводятся в enum
    /// свойствами Role и Status (см. папку Models).
    /// </summary>
    internal static class SqliteMapping
    {
        private static bool _registered;

        /// <summary>Регистрирует обработчики один раз за всё время работы приложения.</summary>
        public static void Register()
        {
            if (_registered) return;

            SqlMapper.AddTypeHandler(new DateTimeHandler());
            _registered = true;
        }

        /// <summary>
        /// Дата: в БД хранится текстом "гггг-ММ-дд чч:мм:сс",
        /// поэтому строки корректно сравниваются и сортируются.
        /// </summary>
        private class DateTimeHandler : SqlMapper.TypeHandler<DateTime>
        {
            private static readonly string[] Formats =
            {
                "yyyy-MM-dd HH:mm:ss",
                "yyyy-MM-dd",
                "yyyy-MM-ddTHH:mm:ss"
            };

            public override void SetValue(IDbDataParameter parameter, DateTime value)
            {
                parameter.Value = value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            }

            public override DateTime Parse(object value)
            {
                var text = Convert.ToString(value);

                DateTime result;
                if (DateTime.TryParseExact(text, Formats, CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out result))
                    return result;

                // Резервный вариант — на случай нестандартного текста в БД
                return Convert.ToDateTime(text, CultureInfo.InvariantCulture);
            }
        }
    }
}
