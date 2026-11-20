using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using MedClinic.Data;
using MedClinic.Models;
using MedClinic.Services;
using Serilog;

namespace MedClinic.Repositories
{
    /// <summary>
    /// Работа с таблицей Doctors: список с поиском, добавление,
    /// редактирование, удаление с проверкой ссылочной целостности.
    /// </summary>
    public class DoctorRepository
    {
        private const string SelectColumns = "Id, FullName, Specialty, Phone, Cabinet";

        /// <summary>Все врачи с необязательным поиском по ФИО, специальности, телефону, кабинету.</summary>
        public List<Doctor> GetAll(string search = null)
        {
            var sql = "SELECT " + SelectColumns + " FROM Doctors";
            object parameters = null;

            if (!string.IsNullOrWhiteSpace(search))
            {
                sql += " WHERE FullName LIKE @q OR Specialty LIKE @q OR Phone LIKE @q OR Cabinet LIKE @q";
                parameters = new { q = "%" + search.Trim() + "%" };
            }

            sql += " ORDER BY FullName;";

            using (var db = Database.Open())
            {
                return db.Query<Doctor>(sql, parameters).AsList();
            }
        }

        /// <summary>
        /// Постраничный список с сортировкой (нужен на этапе 2 для пагинации).
        /// sortColumn — имя колонки, проверяется по белому списку.
        /// </summary>
        public List<Doctor> GetPaged(string search, string sortColumn, bool ascending,
                                     int page, int pageSize, out int totalCount)
        {
            var allowed = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "FullName", "FullName" },
                { "Specialty", "Specialty" },
                { "Phone", "Phone" },
                { "Cabinet", "Cabinet" }
            };

            if (string.IsNullOrWhiteSpace(sortColumn) || !allowed.ContainsKey(sortColumn))
                sortColumn = "FullName";

            var orderColumn = allowed[sortColumn];
            var direction = ascending ? "ASC" : "DESC";

            var where = string.Empty;
            object parameters = null;
            if (!string.IsNullOrWhiteSpace(search))
            {
                where = " WHERE FullName LIKE @q OR Specialty LIKE @q OR Phone LIKE @q OR Cabinet LIKE @q";
                parameters = new { q = "%" + search.Trim() + "%" };
            }

            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            using (var db = Database.Open())
            {
                totalCount = (int)db.ExecuteScalar<long>(
                    "SELECT COUNT(*) FROM Doctors" + where, parameters);

                var sql = "SELECT " + SelectColumns + " FROM Doctors" + where +
                          " ORDER BY " + orderColumn + " " + direction +
                          " LIMIT @limit OFFSET @offset;";

                return db.Query<Doctor>(sql, new
                {
                    q = string.IsNullOrWhiteSpace(search) ? null : "%" + search.Trim() + "%",
                    limit = pageSize,
                    offset = (page - 1) * pageSize
                }).AsList();
            }
        }

        public Doctor GetById(int id)
        {
            using (var db = Database.Open())
            {
                return db.QueryFirstOrDefault<Doctor>(
                    "SELECT " + SelectColumns + " FROM Doctors WHERE Id = @id;", new { id });
            }
        }

        public int CountAll()
        {
            using (var db = Database.Open())
            {
                return (int)db.ExecuteScalar<long>("SELECT COUNT(*) FROM Doctors;");
            }
        }

        /// <summary>Добавляет врача, возвращает новый Id.</summary>
        public int Add(Doctor doctor)
        {
            var error = Validation.CheckDoctor(doctor);
            if (error != null) throw new AppException(error);

            using (var db = Database.Open())
            {
                db.Execute(@"
INSERT INTO Doctors (FullName, Specialty, Phone, Cabinet)
VALUES (@FullName, @Specialty, @Phone, @Cabinet);",
                    new
                    {
                        FullName = doctor.FullName.Trim(),
                        Specialty = doctor.Specialty.Trim(),
                        Phone = (doctor.Phone ?? string.Empty).Trim(),
                        Cabinet = (doctor.Cabinet ?? string.Empty).Trim()
                    });

                var id = (int)db.ExecuteScalar<long>("SELECT last_insert_rowid();");
                Log.Information("Добавлен врач: {FullName} ({Specialty})", doctor.FullName, doctor.Specialty);
                return id;
            }
        }

        public void Update(Doctor doctor)
        {
            var error = Validation.CheckDoctor(doctor);
            if (error != null) throw new AppException(error);

            using (var db = Database.Open())
            {
                db.Execute(@"
UPDATE Doctors
   SET FullName = @FullName, Specialty = @Specialty, Phone = @Phone, Cabinet = @Cabinet
 WHERE Id = @Id;",
                    new
                    {
                        FullName = doctor.FullName.Trim(),
                        Specialty = doctor.Specialty.Trim(),
                        Phone = (doctor.Phone ?? string.Empty).Trim(),
                        Cabinet = (doctor.Cabinet ?? string.Empty).Trim(),
                        doctor.Id
                    });
            }

            Log.Information("Изменён врач Id={Id}: {FullName}", doctor.Id, doctor.FullName);
        }

        /// <summary>Сколько записей на приём связано с врачом.</summary>
        public int CountAppointments(int doctorId)
        {
            using (var db = Database.Open())
            {
                return (int)db.ExecuteScalar<long>(
                    "SELECT COUNT(*) FROM Appointments WHERE DoctorId = @doctorId;", new { doctorId });
            }
        }

        /// <summary>
        /// Удаляет врача. Если у него есть записи — выбрасывает
        /// ReferentialIntegrityException: удаление запрещено.
        /// </summary>
        public void Delete(int doctorId)
        {
            var appointments = CountAppointments(doctorId);
            if (appointments > 0)
            {
                throw new ReferentialIntegrityException(
                    "Нельзя удалить врача: с ним связано записей на приём — " + appointments +
                    ". Сначала отмените или удалите эти записи.");
            }

            using (var db = Database.Open())
            {
                db.Execute("DELETE FROM Doctors WHERE Id = @doctorId;", new { doctorId });
            }

            Log.Warning("Удалён врач Id={DoctorId}", doctorId);
        }
    }
}
