using System;
using System.IO;
using System.Linq;
using Dapper;
using MedClinic.Models;
using MedClinic.Services;
using Microsoft.Data.Sqlite;
using Serilog;

namespace MedClinic.Data
{
    /// <summary>
    /// Создаёт базу данных при первом запуске, применяет миграции
    /// и заполняет таблицы начальными данными.
    /// </summary>
    public static class DatabaseInitializer
    {
        /// <summary>Главный метод: подготовить базу к работе.</summary>
        public static void Initialize()
        {
            var databasePath = AppConfig.Current.DatabasePath;

            // Папка Data может отсутствовать при первом запуске — создаём её
            var folder = Path.GetDirectoryName(databasePath);
            if (!string.IsNullOrEmpty(folder) && !Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            Database.Configure(databasePath);
            Log.Information("Файл базы данных: {Path}", databasePath);

            using (var db = Database.Open())
            {
                EnsureSchemaInfoTable(db);
                var currentVersion = GetCurrentVersion(db);
                var pending = Migrations.All
                    .Where(m => m.Version > currentVersion)
                    .OrderBy(m => m.Version)
                    .ToList();

                if (pending.Count == 0)
                {
                    Log.Information("Схема БД актуальна, версия {Version}", currentVersion);
                }
                else
                {
                    foreach (var migration in pending)
                        ApplyMigration(db, migration);
                }

                SeedDemoData(db);
            }
        }

        /// <summary>Таблица с историей применённых миграций.</summary>
        private static void EnsureSchemaInfoTable(SqliteConnection db)
        {
            db.Execute(@"
CREATE TABLE IF NOT EXISTS SchemaInfo (
    Version   INTEGER NOT NULL PRIMARY KEY,
    Name      TEXT    NOT NULL,
    AppliedAt TEXT    NOT NULL
);");
        }

        private static int GetCurrentVersion(SqliteConnection db)
        {
            var version = db.ExecuteScalar<long>("SELECT IFNULL(MAX(Version), 0) FROM SchemaInfo;");
            return (int)version;
        }

        /// <summary>Применяет одну миграцию в транзакции: либо всё, либо ничего.</summary>
        private static void ApplyMigration(SqliteConnection db, Migration migration)
        {
            using (var transaction = db.BeginTransaction())
            {
                try
                {
                    db.Execute(migration.Sql, null, transaction);
                    db.Execute(
                        "INSERT INTO SchemaInfo (Version, Name, AppliedAt) VALUES (@Version, @Name, @AppliedAt);",
                        new
                        {
                            migration.Version,
                            migration.Name,
                            AppliedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                        }, transaction);

                    transaction.Commit();
                    Log.Information("Применена миграция {Version}: {Name}", migration.Version, migration.Name);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Log.Fatal(ex, "Не удалось применить миграцию {Version}", migration.Version);
                    throw new Exception("Не удалось обновить структуру базы данных (миграция " +
                                        migration.Version + "). Подробности в журнале Logs.", ex);
                }
            }
        }

        // ---------------------------------------------------------------------
        //                  НАЧАЛЬНЫЕ (ДЕМОНСТРАЦИОННЫЕ) ДАННЫЕ
        // ---------------------------------------------------------------------

        private static void SeedDemoData(SqliteConnection db)
        {
            SeedUsers(db);
            SeedDoctors(db);
            SeedServices(db);
            SeedAppointments(db);
        }

        /// <summary>Четыре учётные записи по одной на каждую роль.</summary>
        private static void SeedUsers(SqliteConnection db)
        {
            if (db.ExecuteScalar<long>("SELECT COUNT(*) FROM Users;") > 0) return;

            var users = new[]
            {
                new { Login = "admin",   Password = "admin123",   Role = UserRole.Admin },
                new { Login = "manager", Password = "manager123", Role = UserRole.Manager },
                new { Login = "client",  Password = "client123",  Role = UserRole.Client },
                new { Login = "guest",   Password = "guest123",   Role = UserRole.Guest }
            };

            foreach (var user in users)
            {
                // Dapper при записи параметров-перечислений превращает их в числа,
                // поэтому роль переводим в русский текст явно (в БД стоит CHECK).
                db.Execute(
                    "INSERT INTO Users (Login, PasswordHash, Role) VALUES (@Login, @PasswordHash, @Role);",
                    new
                    {
                        user.Login,
                        PasswordHash = PasswordHasher.Hash(user.Password),
                        Role = EnumText.RoleToDb(user.Role)
                    });
            }

            Log.Information("Созданы учётные записи по умолчанию: admin, manager, client, guest");
        }

        private static void SeedDoctors(SqliteConnection db)
        {
            if (db.ExecuteScalar<long>("SELECT COUNT(*) FROM Doctors;") > 0) return;

            var doctors = new[]
            {
                new { FullName = "Иванова Мария Петровна",      Specialty = "Терапевт",   Phone = "+7 (900) 111-22-33", Cabinet = "101" },
                new { FullName = "Петров Сергей Николаевич",    Specialty = "Кардиолог",  Phone = "+7 (900) 111-22-34", Cabinet = "202" },
                new { FullName = "Сидорова Анна Викторовна",    Specialty = "Невролог",   Phone = "+7 (900) 111-22-35", Cabinet = "203" },
                new { FullName = "Кузнецов Олег Иванович",      Specialty = "Хирург",     Phone = "+7 (900) 111-22-36", Cabinet = "305" },
                new { FullName = "Смирнова Елена Алексеевна",   Specialty = "Педиатр",    Phone = "+7 (900) 111-22-37", Cabinet = "104" },
                new { FullName = "Морозов Дмитрий Сергеевич",   Specialty = "Офтальмолог", Phone = "+7 (900) 111-22-38", Cabinet = "210" }
            };

            foreach (var doctor in doctors)
            {
                db.Execute(
                    "INSERT INTO Doctors (FullName, Specialty, Phone, Cabinet) VALUES (@FullName, @Specialty, @Phone, @Cabinet);",
                    doctor);
            }

            Log.Information("Добавлено врачей: {Count}", doctors.Length);
        }

        private static void SeedServices(SqliteConnection db)
        {
            if (db.ExecuteScalar<long>("SELECT COUNT(*) FROM Services;") > 0) return;

            var services = new[]
            {
                new { Name = "Приём терапевта",              Price = 1200m, Duration = 20 },
                new { Name = "Приём кардиолога",             Price = 1800m, Duration = 30 },
                new { Name = "Приём невролога",              Price = 1700m, Duration = 30 },
                new { Name = "Приём хирурга",                Price = 1600m, Duration = 25 },
                new { Name = "Приём педиатра",               Price = 1300m, Duration = 25 },
                new { Name = "УЗИ внутренних органов",       Price = 2400m, Duration = 40 },
                new { Name = "ЭКГ",                          Price = 900m,  Duration = 15 },
                new { Name = "Общий анализ крови",           Price = 700m,  Duration = 10 }
            };

            foreach (var service in services)
            {
                db.Execute(
                    "INSERT INTO Services (Name, Price, Duration) VALUES (@Name, @Price, @Duration);",
                    service);
            }

            Log.Information("Добавлено услуг: {Count}", services.Length);
        }

        /// <summary>Три демонстрационные записи, чтобы статистика была не пустой.</summary>
        private static void SeedAppointments(SqliteConnection db)
        {
            if (db.ExecuteScalar<long>("SELECT COUNT(*) FROM Appointments;") > 0) return;

            var doctorIds = db.Query<int>("SELECT Id FROM Doctors ORDER BY Id LIMIT 3;").ToList();
            var serviceIds = db.Query<int>("SELECT Id FROM Services ORDER BY Id LIMIT 3;").ToList();
            if (doctorIds.Count < 3 || serviceIds.Count < 3) return;

            // Даты записываем строкой "гггг-ММ-дд" — так же, как это делает репозиторий записей
            var demo = new[]
            {
                new { PatientName = "Иванов Иван Иванович", DoctorId = doctorIds[0], ServiceId = serviceIds[0],
                      Date = DateTime.Today.ToString("yyyy-MM-dd"),               Time = "09:30",
                      Status = AppointmentStatus.Booked },
                new { PatientName = "Смирнова Ольга Петровна", DoctorId = doctorIds[1], ServiceId = serviceIds[1],
                      Date = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd"),     Time = "11:00",
                      Status = AppointmentStatus.Booked },
                new { PatientName = "Кузнецов Артём Ильич", DoctorId = doctorIds[2], ServiceId = serviceIds[2],
                      Date = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"),    Time = "15:00",
                      Status = AppointmentStatus.Completed }
            };

            foreach (var item in demo)
            {
                // Статус тоже переводим в русский текст вручную (см. пояснение в SeedUsers)
                db.Execute(@"
INSERT INTO Appointments (PatientName, DoctorId, ServiceId, Date, Time, Status)
VALUES (@PatientName, @DoctorId, @ServiceId, @Date, @Time, @Status);",
                    new
                    {
                        item.PatientName,
                        item.DoctorId,
                        item.ServiceId,
                        item.Date,
                        item.Time,
                        Status = EnumText.StatusToDb(item.Status)
                    });
            }

            Log.Information("Добавлены демонстрационные записи на приём: {Count}", demo.Length);
        }
    }
}
