using System;
using System.Security.Cryptography;
using System.Text;

namespace MedClinic.Services
{
    /// <summary>
    /// Хэширование паролей алгоритмом SHA256.
    /// В базе хранится только хэш, поэтому восстановить пароль из БД нельзя.
    /// (В учебном проекте соль не используется — это указано в отчёте.)
    /// </summary>
    public static class PasswordHasher
    {
        /// <summary>Возвращает хэш пароля: 64 символа, нижний регистр.</summary>
        public static string Hash(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Пароль не может быть пустым.", "password");

            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder(bytes.Length * 2);

                foreach (var b in bytes)
                    builder.Append(b.ToString("x2"));

                return builder.ToString();
            }
        }

        /// <summary>Проверяет, что пароль соответствует сохранённому хэшу.</summary>
        public static bool Verify(string password, string hash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
                return false;

            return string.Equals(Hash(password), hash.Trim(), StringComparison.OrdinalIgnoreCase);
        }
    }
}
