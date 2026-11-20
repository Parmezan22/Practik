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
    /// Работа с таблицей Services: список, поиск, добавление,
    /// редактирование, удаление с проверкой использования в записях.
    /// </summary>
    public class ServiceRepository
    {
        private const string SelectColumns = "Id, Name, Price, Duration";

        public List<Service> GetAll(string search = null)
        {
            var sql = "SELECT " + SelectColumns + " FROM Services";
            object parameters = null;

            if (!string.IsNullOrWhiteSpace(search))
            {
                sql += " WHERE Name LIKE @q";
                parameters = new { q = "%" + search.Trim() + "%" };
            }

            sql += " ORDER BY Name;";

            using (var db = Database.Open())
            {
                return db.Query<Service>(sql, parameters).AsList();
            }
        }

        /// <summary>
        /// Постраничный список услуг с поиском и сортировкой.
        /// sortColumn проверяется по белому списку (защита от SQL-инъекций).
        /// </summary>
        public List<Service> GetPaged(string search, string sortColumn, bool ascending,
                                      int page, int pageSize, out int totalCount)
        {
            var allowed = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Name", "Name" },
                { "Price", "Price" },
                { "Duration", "Duration" }
            };

            if (string.IsNullOrWhiteSpace(sortColumn) || !allowed.ContainsKey(sortColumn))
                sortColumn = "Name";

            var orderColumn = allowed[sortColumn];
            var direction = ascending ? "ASC" : "DESC";

            var where = string.Empty;
            object parameters = null;
            if (!string.IsNullOrWhiteSpace(search))
            {
                where = " WHERE Name LIKE @q";
                parameters = new { q = "%" + search.Trim() + "%" };
            }

            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            using (var db = Database.Open())
            {
                totalCount = (int)db.ExecuteScalar<long>(
                    "SELECT COUNT(*) FROM Services" + where, parameters);

                var sql = "SELECT " + SelectColumns + " FROM Services" + where +
                          " ORDER BY " + orderColumn + " " + direction +
                          " LIMIT @limit OFFSET @offset;";

                return db.Query<Service>(sql, new
                {
                    q = string.IsNullOrWhiteSpace(search) ? null : "%" + search.Trim() + "%",
                    limit = pageSize,
                    offset = (page - 1) * pageSize
                }).AsList();
            }
        }

        public Service GetById(int id)
        {            using (var db = Database.Open())
            {
                return db.QueryFirstOrDefault<Service>(
                    "SELECT " + SelectColumns + " FROM Services WHERE Id = @id;", new { id });
            }
        }

        public int CountAll()
        {
            using (var db = Database.Open())
            {
                return (int)db.ExecuteScalar<long>("SELECT COUNT(*) FROM Services;");
            }
        }

        /// <summary>Проверяет, используется ли услуга в записях (основная или дополнительная).</summary>
        public int CountUsage(int serviceId)
        {
            using (var db = Database.Open())
            {
                return (int)db.ExecuteScalar<long>(@"
SELECT (SELECT COUNT(*) FROM Appointments WHERE ServiceId = @serviceId)
     + (SELECT COUNT(*) FROM AppointmentServices WHERE ServiceId = @serviceId);",
                    new { serviceId });
            }
        }

        public int Add(Service service)
        {
            var error = Validation.CheckService(service);
            if (error != null) throw new AppException(error);

            using (var db = Database.Open())
            {
                if (NameExists(service.Name))
                    throw new AppException("Услуга с названием «" + service.Name.Trim() + "» уже существует.");

                db.Execute(
                    "INSERT INTO Services (Name, Price, Duration) VALUES (@Name, @Price, @Duration);",
                    new
                    {
                        Name = service.Name.Trim(),
                        service.Price,
                        service.Duration
                    });

                var id = (int)db.ExecuteScalar<long>("SELECT last_insert_rowid();");
                Log.Information("Добавлена услуга: {Name}, цена {Price}", service.Name, service.Price);
                return id;
            }
        }

        public void Update(Service service)
        {
            var error = Validation.CheckService(service);
            if (error != null) throw new AppException(error);

            using (var db = Database.Open())
            {
                if (NameExists(service.Name, service.Id))
                    throw new AppException("Услуга с названием «" + service.Name.Trim() + "» уже существует.");

                db.Execute(@"
UPDATE Services
   SET Name = @Name, Price = @Price, Duration = @Duration
 WHERE Id = @Id;",
                    new
                    {
                        Name = service.Name.Trim(),
                        service.Price,
                        service.Duration,
                        service.Id
                    });
            }

            Log.Information("Изменена услуга Id={Id}: {Name}", service.Id, service.Name);
        }

        /// <summary>Удаляет услугу, если она не используется в записях.</summary>
        public void Delete(int serviceId)
        {
            var usage = CountUsage(serviceId);
            if (usage > 0)
            {
                throw new ReferentialIntegrityException(
                    "Нельзя удалить услугу: она используется в записях на приём (" + usage +
                    " раз). Сначала отмените или удалите эти записи.");
            }

            using (var db = Database.Open())
            {
                db.Execute("DELETE FROM Services WHERE Id = @serviceId;", new { serviceId });
            }

            Log.Warning("Удалена услуга Id={ServiceId}", serviceId);
        }

        private bool NameExists(string name, int exceptId = 0)
        {
            using (var db = Database.Open())
            {
                var count = db.ExecuteScalar<long>(
                    "SELECT COUNT(*) FROM Services WHERE Name = @name AND Id <> @exceptId;",
                    new { name = (name ?? string.Empty).Trim(), exceptId });
                return count > 0;
            }
        }
    }
}
