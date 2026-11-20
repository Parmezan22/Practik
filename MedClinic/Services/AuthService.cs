using System;
using MedClinic.Models;
using MedClinic.Repositories;
using Serilog;

namespace MedClinic.Services
{
    /// <summary>
    /// Проверка учётных данных при входе в систему.
    /// Логика вынесена из формы, чтобы её можно было покрыть модульными тестами.
    /// </summary>
    public class AuthService
    {
        private readonly UserRepository _userRepository;

        public AuthService(UserRepository userRepository)
        {
            if (userRepository == null) throw new ArgumentNullException("userRepository");
            _userRepository = userRepository;
        }

        /// <summary>
        /// Выполняет вход. При успехе возвращает пользователя,
        /// при ошибке выбрасывает AuthException с понятным текстом.
        /// </summary>
        public User Login(string login, string password)
        {
            login = (login ?? string.Empty).Trim();

            if (login.Length == 0)
                throw new AuthException("Введите логин.");

            if (string.IsNullOrEmpty(password))
                throw new AuthException("Введите пароль.");

            if (!Validation.IsValidLogin(login))
                throw new AuthException("Логин должен содержать 3–32 символа: латиница, цифры, «.», «-» или «_».");

            User user;
            try
            {
                user = _userRepository.GetByLogin(login);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка обращения к таблице Users при входе");
                throw new AuthException("Не удалось подключиться к базе данных. Попробуйте позже.");
            }

            if (user == null)
            {
                Log.Warning("Неудачная попытка входа: логин «{Login}» не найден", login);
                throw new AuthException("Пользователь с логином «" + login + "» не найден.");
            }

            if (!PasswordHasher.Verify(password, user.PasswordHash))
            {
                Log.Warning("Неудачная попытка входа: неверный пароль для «{Login}»", login);
                throw new AuthException("Неверный пароль. Проверьте раскладку клавиатуры и Caps Lock.");
            }

            Log.Information("Успешный вход: {Login}, роль {Role}", user.Login, user.RoleDisplay);
            return user;
        }
    }
}
