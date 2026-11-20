using System;
using System.Text.RegularExpressions;
using MedClinic.Models;

namespace MedClinic.Services
{
    /// <summary>
    /// Проверка корректности данных, введённых пользователем.
    /// Все методы возвращают true, если значение допустимо.
    /// </summary>
    public static class Validation
    {
        /// <summary>Строка заполнена (не пустая и не из одних пробелов).</summary>
        public static bool IsFilled(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        /// <summary>Логин: 3–32 символа, латиница, цифры, точка, дефис, подчёркивание.</summary>
        public static bool IsValidLogin(string value)
        {
            return !string.IsNullOrEmpty(value) &&
                   Regex.IsMatch(value, @"^[A-Za-z0-9_.\-]{3,32}$");
        }

        /// <summary>Пароль не короче 5 символов.</summary>
        public static bool IsValidPassword(string value)
        {
            return !string.IsNullOrEmpty(value) && value.Length >= 5 && value.Length <= 64;
        }

        /// <summary>ФИО: минимум два слова, только буквы, пробелы и дефисы.</summary>
        public static bool IsValidFullName(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            var trimmed = value.Trim();
            if (trimmed.Length < 5 || trimmed.Length > 100) return false;
            if (!Regex.IsMatch(trimmed, @"^[А-Яа-яЁёA-Za-z\s\-]+$")) return false;
            return Regex.Matches(trimmed, @"\S+").Count >= 2;
        }

        /// <summary>Телефон: 6–20 символов, цифры, пробелы, скобки, дефис, плюс.</summary>
        public static bool IsValidPhone(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            return Regex.IsMatch(value.Trim(), @"^[0-9\+\-\(\)\s]{6,20}$");
        }

        /// <summary>Номер кабинета: 1–10 символов, буквы, цифры, дефис.</summary>
        public static bool IsValidCabinet(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            return Regex.IsMatch(value.Trim(), @"^[0-9A-Za-zА-Яа-я\-]{1,10}$");
        }

        /// <summary>Цена: не отрицательная и не больше 1 000 000.</summary>
        public static bool IsValidPrice(decimal price)
        {
            return price >= 0m && price <= 1000000m;
        }

        /// <summary>Длительность услуги: от 5 до 480 минут.</summary>
        public static bool IsValidDuration(int minutes)
        {
            return minutes >= 5 && minutes <= 480;
        }

        /// <summary>Время в формате ЧЧ:мм (от 08:00 до 20:00 — часы работы клиники).</summary>
        public static bool IsValidWorkingTime(string time)
        {
            if (string.IsNullOrWhiteSpace(time)) return false;
            if (!Regex.IsMatch(time.Trim(), @"^([01]\d|2[0-3]):[0-5]\d$")) return false;

            var parts = time.Trim().Split(':');
            var hour = int.Parse(parts[0]);
            return hour >= 8 && hour <= 20;
        }

        /// <summary>Проверяет, что дата не в прошлом.</summary>
        public static bool IsFutureOrToday(DateTime date)
        {
            return date.Date >= DateTime.Today;
        }

        /// <summary>Проверяет корректность данных врача. Возвращает текст ошибки или null.</summary>
        public static string CheckDoctor(Doctor doctor)
        {
            if (doctor == null) return "Данные врача не заполнены.";
            if (!IsValidFullName(doctor.FullName)) return "ФИО врача должно содержать минимум два слова (только буквы, пробелы и дефис).";
            if (!IsFilled(doctor.Specialty)) return "Укажите специальность врача.";
            if (doctor.Specialty.Trim().Length > 50) return "Специальность не должна быть длиннее 50 символов.";
            if (IsFilled(doctor.Phone) && !IsValidPhone(doctor.Phone)) return "Телефон указан неверно. Пример: +7 (900) 111-22-33.";
            if (IsFilled(doctor.Cabinet) && !IsValidCabinet(doctor.Cabinet)) return "Номер кабинета указан неверно (до 10 символов).";
            return null;
        }

        /// <summary>Проверяет корректность данных услуги. Возвращает текст ошибки или null.</summary>
        public static string CheckService(Service service)
        {
            if (service == null) return "Данные услуги не заполнены.";
            if (!IsFilled(service.Name)) return "Укажите название услуги.";
            if (service.Name.Trim().Length > 100) return "Название услуги не должно быть длиннее 100 символов.";
            if (!IsValidPrice(service.Price)) return "Цена должна быть от 0 до 1 000 000 рублей.";
            if (!IsValidDuration(service.Duration)) return "Длительность должна быть от 5 до 480 минут.";
            return null;
        }
    }
}
