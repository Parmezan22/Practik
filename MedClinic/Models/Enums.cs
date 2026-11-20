using System;
using System.Collections.Generic;

namespace MedClinic.Models
{
    /// <summary>
    /// Роль пользователя в системе.
    /// В базе данных роли хранятся по-русски (гость, клиент, менеджер, админ),
    /// как указано в техническом задании, поэтому ниже есть методы перевода.
    /// </summary>
    public enum UserRole
    {
        Guest = 0,    // гость   — только просмотр
        Client = 1,   // клиент  — может записаться на приём
        Manager = 2,  // менеджер — ведёт справочники и записи
        Admin = 3     // админ   — полный доступ
    }

    /// <summary>Статус записи на приём.</summary>
    public enum AppointmentStatus
    {
        Booked = 0,    // записан
        Cancelled = 1, // отменён
        Completed = 2  // завершён
    }

    /// <summary>
    /// Единая точка перевода значений перечислений в текст БД и в текст для интерфейса.
    /// Такой подход убирает "магические строки" из кода репозиториев и форм.
    /// </summary>
    public static class EnumText
    {
        private static readonly Dictionary<UserRole, string> RoleDb = new Dictionary<UserRole, string>
        {
            { UserRole.Guest,   "гость" },
            { UserRole.Client,  "клиент" },
            { UserRole.Manager, "менеджер" },
            { UserRole.Admin,   "админ" }
        };

        private static readonly Dictionary<UserRole, string> RoleDisplay = new Dictionary<UserRole, string>
        {
            { UserRole.Guest,   "Гость" },
            { UserRole.Client,  "Клиент" },
            { UserRole.Manager, "Менеджер" },
            { UserRole.Admin,   "Администратор" }
        };

        private static readonly Dictionary<AppointmentStatus, string> StatusDb = new Dictionary<AppointmentStatus, string>
        {
            { AppointmentStatus.Booked,    "записан" },
            { AppointmentStatus.Cancelled, "отменён" },
            { AppointmentStatus.Completed, "завершён" }
        };

        private static readonly Dictionary<AppointmentStatus, string> StatusDisplay = new Dictionary<AppointmentStatus, string>
        {
            { AppointmentStatus.Booked,    "Записан" },
            { AppointmentStatus.Cancelled, "Отменён" },
            { AppointmentStatus.Completed, "Завершён" }
        };

        /// <summary>Роль -> значение для записи в БД.</summary>
        public static string RoleToDb(UserRole role)
        {
            return RoleDb[role];
        }

        /// <summary>Роль -> красивое название для экрана.</summary>
        public static string RoleToDisplay(UserRole role)
        {
            return RoleDisplay[role];
        }

        /// <summary>Текст из БД -> роль. Неизвестное значение считается "гость".</summary>
        public static UserRole RoleFromDb(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                foreach (var pair in RoleDb)
                {
                    if (string.Equals(pair.Value, text.Trim(), StringComparison.OrdinalIgnoreCase))
                        return pair.Key;
                }
            }
            return UserRole.Guest;
        }

        /// <summary>Статус -> значение для записи в БД.</summary>
        public static string StatusToDb(AppointmentStatus status)
        {
            return StatusDb[status];
        }

        /// <summary>Статус -> красивое название для экрана.</summary>
        public static string StatusToDisplay(AppointmentStatus status)
        {
            return StatusDisplay[status];
        }

        /// <summary>Текст из БД -> статус.</summary>
        public static AppointmentStatus StatusFromDb(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                foreach (var pair in StatusDb)
                {
                    if (string.Equals(pair.Value, text.Trim(), StringComparison.OrdinalIgnoreCase))
                        return pair.Key;
                }
            }
            return AppointmentStatus.Booked;
        }
    }
}
