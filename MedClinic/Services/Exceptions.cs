using System;

namespace MedClinic.Services
{
    /// <summary>
    /// Базовое исключение приложения. Его сообщения предназначены
    /// для показа пользователю в MessageBox как есть.
    /// </summary>
    public class AppException : Exception
    {
        public AppException(string message) : base(message) { }
        public AppException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>Ошибка входа в систему (неверный логин или пароль).</summary>
    public class AuthException : AppException
    {
        public AuthException(string message) : base(message) { }
    }

    /// <summary>
    /// Нарушение ссылочной целостности: например, попытка удалить врача,
    /// у которого есть записи на приём.
    /// </summary>
    public class ReferentialIntegrityException : AppException
    {
        public ReferentialIntegrityException(string message) : base(message) { }
    }
}
