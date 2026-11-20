using System;
using System.Collections.Generic;
using Dapper;
using MedClinic.Data;
using MedClinic.Models;
using MedClinic.Services;
using Serilog;

namespace MedClinic.Repositories
{
    /// <summary>
    /// Работа с таблицей Users: выборка, добавление, изменение, удаление.
    /// </summary>
    public class UserRepository
    {
        private const string SelectColumns = "Id, Login, PasswordHash, Role AS RoleDb";

        /// <summary>Все пользователи в алфавитном порядке логинов.</summary>
        public List<User> GetAll()
        {
            using (var db = Database.Open())
            {
                return db.Query<User>(
                    "SELECT " + SelectColumns + " FROM Users ORDER BY Login;").AsList();
            }
        }

        /// <summary>Поиск пользователя по логину (регистр не важен). null — если не найден.</summary>
        public User GetByLogin(string login)
        {
            if (string.IsNullOrWhiteSpace(login)) return null;

            using (var db = Database.Open())
            {
                return db.QueryFirstOrDefault<User>(
                    "SELECT " + SelectColumns + " FROM Users WHERE Login = @login;",
                    new { login = login.Trim() });
            }
        }

        /// <summary>Пользователь по идентификатору.</summary>
        public User GetById(int id)
        {
            using (var db = Database.Open())
            {
                return db.QueryFirstOrDefault<User>(
                    "SELECT " + SelectColumns + " FROM Users WHERE Id = @id;", new { id });
            }
        }

        /// <summary>Количество пользователей.</summary>
        public int CountAll()
        {
            using (var db = Database.Open())
            {
                return (int)db.ExecuteScalar<long>("SELECT COUNT(*) FROM Users;");
            }
        }

        /// <summary>Проверяет, занят ли логин.</summary>
        public bool LoginExists(string login, int exceptId = 0)
        {
            using (var db = Database.Open())
            {
                var count = db.ExecuteScalar<long>(
                    "SELECT COUNT(*) FROM Users WHERE Login = @login AND Id <> @exceptId;",
                    new { login = (login ?? string.Empty).Trim(), exceptId });
                return count > 0;
            }
        }

        /// <summary>Добавляет пользователя, возвращает новый Id.</summary>
        public int Add(User user, string plainPassword)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (string.IsNullOrWhiteSpace(user.Login))
                throw new AppException("Не указан логин пользователя.");
            if (!Validation.IsValidLogin(user.Login.Trim()))
                throw new AppException("Логин: 3–32 символа, латиница, цифры, точка, дефис или подчёркивание.");
            if (!Validation.IsValidPassword(plainPassword))
                throw new AppException("Пароль должен содержать от 5 до 64 символов.");
            if (LoginExists(user.Login))
                throw new AppException("Пользователь с логином «" + user.Login.Trim() + "» уже существует.");

            using (var db = Database.Open())
            {
                db.Execute(
                    "INSERT INTO Users (Login, PasswordHash, Role) VALUES (@Login, @PasswordHash, @Role);",
                    new
                    {
                        Login = user.Login.Trim(),
                        PasswordHash = PasswordHasher.Hash(plainPassword),
                        // Перечисление переводим в текст: в БД роль хранится по-русски
                        Role = EnumText.RoleToDb(user.Role)
                    });

                var id = (int)db.ExecuteScalar<long>("SELECT last_insert_rowid();");
                Log.Information("Добавлен пользователь {Login} с ролью {Role}", user.Login, user.Role);
                return id;
            }
        }

        /// <summary>Изменяет логин и роль пользователя.</summary>
        public void Update(User user)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (LoginExists(user.Login, user.Id))
                throw new AppException("Пользователь с логином «" + user.Login.Trim() + "» уже существует.");

            using (var db = Database.Open())
            {
                db.Execute(
                    "UPDATE Users SET Login = @Login, Role = @Role WHERE Id = @Id;",
                    new { Login = user.Login.Trim(), Role = EnumText.RoleToDb(user.Role), user.Id });
            }
        }

        /// <summary>Задаёт новый пароль (сохраняется только хэш).</summary>
        public void ChangePassword(int userId, string newPassword)
        {
            if (!Validation.IsValidPassword(newPassword))
                throw new AppException("Пароль должен содержать от 5 до 64 символов.");

            using (var db = Database.Open())
            {
                db.Execute("UPDATE Users SET PasswordHash = @hash WHERE Id = @userId;",
                    new { hash = PasswordHasher.Hash(newPassword), userId });
            }

            Log.Information("Изменён пароль пользователя с Id={UserId}", userId);
        }

        /// <summary>Удаляет пользователя.</summary>
        public void Delete(int userId)
        {
            using (var db = Database.Open())
            {
                db.Execute("DELETE FROM Users WHERE Id = @userId;", new { userId });
            }
            Log.Warning("Удалён пользователь с Id={UserId}", userId);
        }
    }
}
