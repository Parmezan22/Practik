using System.Collections.Generic;

namespace MedClinic.Data
{
    /// <summary>Одна миграция: номер версии, название и SQL-скрипт.</summary>
    internal class Migration
    {
        public Migration(int version, string name, string sql)
        {
            Version = version;
            Name = name;
            Sql = sql;
        }

        public int Version { get; private set; }
        public string Name { get; private set; }
        public string Sql { get; private set; }
    }

    /// <summary>
    /// Список миграций базы данных.
    /// При запуске приложение применяет только те скрипты, чья версия больше текущей.
    /// Чтобы доработать схему — добавьте новую миграцию с бо́льшим номером,
    /// существующие скрипты менять нельзя.
    /// </summary>
    internal static class Migrations
    {
        public static readonly List<Migration> All = new List<Migration>
        {
            new Migration(1, "Начальная схема: пользователи, врачи, услуги, записи", @"
-- 1. Пользователи системы ---------------------------------------------------
CREATE TABLE IF NOT EXISTS Users (
    Id           INTEGER PRIMARY KEY AUTOINCREMENT,
    Login        TEXT    NOT NULL COLLATE NOCASE UNIQUE,
    PasswordHash TEXT    NOT NULL,
    Role         TEXT    NOT NULL CHECK (Role IN ('гость', 'клиент', 'менеджер', 'админ'))
);

-- 2. Врачи -----------------------------------------------------------------
CREATE TABLE IF NOT EXISTS Doctors (
    Id        INTEGER PRIMARY KEY AUTOINCREMENT,
    FullName  TEXT NOT NULL,
    Specialty TEXT NOT NULL,
    Phone     TEXT NULL,
    Cabinet   TEXT NULL
);

-- 3. Услуги ----------------------------------------------------------------
CREATE TABLE IF NOT EXISTS Services (
    Id       INTEGER PRIMARY KEY AUTOINCREMENT,
    Name     TEXT    NOT NULL UNIQUE,
    Price    REAL    NOT NULL CHECK (Price >= 0),
    Duration INTEGER NOT NULL CHECK (Duration > 0)
);

-- 4. Записи на приём -------------------------------------------------------
-- ON DELETE RESTRICT запрещает удалять врача или услугу, если на них есть записи.
CREATE TABLE IF NOT EXISTS Appointments (
    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
    PatientName TEXT    NOT NULL,
    DoctorId    INTEGER NOT NULL,
    ServiceId   INTEGER NOT NULL,
    Date        TEXT    NOT NULL,
    Time        TEXT    NOT NULL,
    Status      TEXT    NOT NULL CHECK (Status IN ('записан', 'отменён', 'завершён')),
    FOREIGN KEY (DoctorId)  REFERENCES Doctors(Id)  ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (ServiceId) REFERENCES Services(Id) ON DELETE RESTRICT ON UPDATE CASCADE
);
"),

            new Migration(2, "Несколько услуг в одной записи + индексы", @"
-- Таблица связи: запись <-> услуга (требование ""несколько услуг в одной записи"")
CREATE TABLE IF NOT EXISTS AppointmentServices (
    Id            INTEGER PRIMARY KEY AUTOINCREMENT,
    AppointmentId INTEGER NOT NULL,
    ServiceId     INTEGER NOT NULL,
    UNIQUE (AppointmentId, ServiceId),
    FOREIGN KEY (AppointmentId) REFERENCES Appointments(Id) ON DELETE CASCADE,
    FOREIGN KEY (ServiceId)     REFERENCES Services(Id)     ON DELETE RESTRICT
);

-- Индексы ускоряют фильтрацию и сортировку записей
CREATE INDEX IF NOT EXISTS IX_Appointments_Date       ON Appointments(Date);
CREATE INDEX IF NOT EXISTS IX_Appointments_DoctorId   ON Appointments(DoctorId);
CREATE INDEX IF NOT EXISTS IX_Appointments_Status     ON Appointments(Status);
CREATE INDEX IF NOT EXISTS IX_AppointmentServices_App ON AppointmentServices(AppointmentId);
"),

            new Migration(3, "Автор записи (для ограничения отмены клиентом)", @"
-- Кто создал запись: нужно, чтобы клиент мог отменять только свои записи.
ALTER TABLE Appointments ADD COLUMN CreatedByUserId INTEGER NULL
    REFERENCES Users(Id) ON DELETE SET NULL;
")
        };
    }
}
