using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace MedClinic.Data
{
    /// <summary>
    /// Класс подключения к файлу базы данных SQLite.
    /// SQLite не требует сервера: вся база — один файл (по умолчанию Data\clinic.db).
    /// Все репозитории открывают соединение только через этот класс.
    /// </summary>
    public static class Database
    {
        private static string _connectionString;

        /// <summary>Полный путь к файлу базы данных.</summary>
        public static string DatabasePath { get; private set; }

        /// <summary>
        /// Настраивает строку подключения. Вызывается один раз при старте приложения.
        /// </summary>
        public static void Configure(string databasePath)
        {
            if (string.IsNullOrWhiteSpace(databasePath))
                throw new ArgumentException("Не задан путь к файлу базы данных.", "databasePath");

            DatabasePath = databasePath;

            _connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = databasePath,
                Mode = SqliteOpenMode.ReadWriteCreate
            }.ToString();

            // Регистрируем обработчики типов Dapper (роль, статус, дата)
            SqliteMapping.Register();
        }

        /// <summary>
        /// Открывает новое соединение с базой и включает проверку внешних ключей.
        /// Соединение обязательно закрывать: using (var db = Database.Open()) { ... }
        /// </summary>
        public static SqliteConnection Open()
        {
            if (_connectionString == null)
                throw new InvalidOperationException("База данных не настроена: не вызван Database.Configure().");

            var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // В SQLite проверка внешних ключей по умолчанию выключена — включаем вручную.
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "PRAGMA foreign_keys = ON;";
                command.ExecuteNonQuery();
            }

            return connection;
        }

        /// <summary>
        /// Создаёт резервную копию базы в указанный файл.
        /// Используется штатный механизм SQLite Backup API: копия корректна даже
        /// при открытой программе, потому что SQLite сам согласует чтение страниц.
        /// </summary>
        public static void BackupTo(string destinationPath)
        {
            if (string.IsNullOrWhiteSpace(destinationPath))
                throw new ArgumentException("Не задан путь для резервной копии.", "destinationPath");

            if (_connectionString == null)
                throw new InvalidOperationException("База данных не настроена: не вызван Database.Configure().");

            var destinationFullPath = Path.GetFullPath(destinationPath);

            if (string.Equals(destinationFullPath, Path.GetFullPath(DatabasePath),
                              StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Нельзя сохранить резервную копию поверх работающей базы данных. Выберите другой файл.");
            }

            var folder = Path.GetDirectoryName(destinationFullPath);
            if (!string.IsNullOrWhiteSpace(folder) && !Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            // Старый файл удаляем, чтобы копия гарантированно содержала только актуальные данные
            if (File.Exists(destinationFullPath))
                File.Delete(destinationFullPath);

            var destinationConnectionString = new SqliteConnectionStringBuilder
            {
                DataSource = destinationFullPath,
                Mode = SqliteOpenMode.ReadWriteCreate
            }.ToString();

            using (var source = Open())
            using (var destination = new SqliteConnection(destinationConnectionString))
            {
                destination.Open();
                source.BackupDatabase(destination);
            }
        }
    }
}
