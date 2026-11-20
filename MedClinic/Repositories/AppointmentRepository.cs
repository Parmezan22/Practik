using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using MedClinic.Data;
using MedClinic.Models;
using MedClinic.Services;
using Microsoft.Data.Sqlite;
using Serilog;

namespace MedClinic.Repositories
{
    /// <summary>Количество записей за один день (для статистики).</summary>
    public class DailyCount
    {
        /// <summary>Дата в формате "гггг-ММ-дд".</summary>
        public string Day { get; set; }
        public int Count { get; set; }
    }

    /// <summary>
    /// Работа с таблицей Appointments: создание записи с несколькими услугами,
    /// фильтрация, сортировка, пагинация, отмена, статистика.
    /// </summary>
    public class AppointmentRepository
    {
        /// <summary>Основной SELECT с присоединением врача и услуги.</summary>
        private const string SelectSql = @"
SELECT a.Id, a.PatientName, a.DoctorId, a.ServiceId, a.Date, a.Time, a.Status AS StatusDb,
       a.CreatedByUserId,
       d.FullName AS DoctorName,
       s.Name     AS ServiceName,
       s.Price    AS Price,
       IFNULL((SELECT SUM(s2.Price)
                 FROM AppointmentServices aps
                 JOIN Services s2 ON s2.Id = aps.ServiceId
                WHERE aps.AppointmentId = a.Id), 0) AS ExtraPrice
  FROM Appointments a
  JOIN Doctors  d ON d.Id = a.DoctorId
  JOIN Services s ON s.Id = a.ServiceId";

        /// <summary>Разрешённые колонки сортировки (защита от SQL-инъекций).</summary>
        private static readonly Dictionary<string, string> SortColumns =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Date", "a.Date" },
                { "Время", "a.Time" },
                { "Time", "a.Time" },
                { "PatientName", "a.PatientName" },
                { "Пациент", "a.PatientName" },
                { "DoctorName", "d.FullName" },
                { "Врач", "d.FullName" },
                { "Status", "a.Status" },
                { "Статус", "a.Status" },
                { "ServiceName", "s.Name" },
                { "Услуга", "s.Name" }
            };

        /// <summary>
        /// Записи с фильтрами по периоду, врачу и статусу, с сортировкой и пагинацией.
        /// Параметр totalCount возвращает общее число записей без учёта страницы.
        /// </summary>
        public List<Appointment> GetFiltered(DateTime? from, DateTime? to, int? doctorId,
                                             AppointmentStatus? status, string sortColumn,
                                             bool ascending, int page, int pageSize,
                                             out int totalCount)
        {
            var where = new StringBuilder(" WHERE 1 = 1");
            var parameters = new DynamicParameters();

            if (from.HasValue)
            {
                where.Append(" AND a.Date >= @from");
                parameters.Add("from", from.Value.ToString("yyyy-MM-dd"));
            }

            if (to.HasValue)
            {
                where.Append(" AND a.Date <= @to");
                parameters.Add("to", to.Value.ToString("yyyy-MM-dd") + " 23:59:59");
            }

            if (doctorId.HasValue)
            {
                where.Append(" AND a.DoctorId = @doctorId");
                parameters.Add("doctorId", doctorId.Value);
            }

            if (status.HasValue)
            {
                where.Append(" AND a.Status = @status");
                parameters.Add("status", EnumText.StatusToDb(status.Value));
            }

            if (string.IsNullOrWhiteSpace(sortColumn) || !SortColumns.ContainsKey(sortColumn))
                sortColumn = "Date";

            var direction = ascending ? "ASC" : "DESC";
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            using (var db = Database.Open())
            {
                totalCount = (int)db.ExecuteScalar<long>(
                    "SELECT COUNT(*) FROM Appointments a" + where, parameters);

                var sql = SelectSql + where +
                          " ORDER BY " + SortColumns[sortColumn] + " " + direction + ", a.Time " + direction +
                          " LIMIT @limit OFFSET @offset;";

                parameters.Add("limit", pageSize);
                parameters.Add("offset", (page - 1) * pageSize);

                return db.Query<Appointment>(sql, parameters).AsList();
            }
        }

        /// <summary>Все записи без пагинации (для отчёта и экспорта).</summary>
        public List<Appointment> GetAllForPeriod(DateTime from, DateTime to, int? doctorId)
        {
            int total;
            return GetFiltered(from, to, doctorId, null, "Date", true, 1, int.MaxValue, out total);
        }

        public Appointment GetById(int id)
        {
            using (var db = Database.Open())
            {
                var appointment = db.QueryFirstOrDefault<Appointment>(SelectSql + " WHERE a.Id = @id;", new { id });
                if (appointment != null)
                    appointment.AdditionalServices = GetAdditionalServices(id);
                return appointment;
            }
        }

        /// <summary>Все услуги записи: основная + дополнительные.</summary>
        public List<Service> GetServices(int appointmentId)
        {
            using (var db = Database.Open())
            {
                var main = db.Query<Service>(@"
SELECT s.Id, s.Name, s.Price, s.Duration
  FROM Services s
  JOIN Appointments a ON a.ServiceId = s.Id
 WHERE a.Id = @appointmentId;", new { appointmentId }).AsList();

                main.AddRange(GetAdditionalServices(appointmentId));
                return main;
            }
        }

        /// <summary>Только дополнительные услуги записи.</summary>
        public List<Service> GetAdditionalServices(int appointmentId)
        {
            using (var db = Database.Open())
            {
                return db.Query<Service>(@"
SELECT s.Id, s.Name, s.Price, s.Duration
  FROM Services s
  JOIN AppointmentServices aps ON aps.ServiceId = s.Id
 WHERE aps.AppointmentId = @appointmentId
 ORDER BY s.Name;", new { appointmentId }).AsList();
            }
        }

        /// <summary>
        /// Создаёт запись и дополнительные услуги в одной транзакции.
        /// Перед вставкой проверяет, что интервал приёма не пересекается
        /// с другими записями врача (с учётом длительности всех услуг).
        /// Возвращает Id новой записи.
        /// </summary>
        public int Create(Appointment appointment, IEnumerable<int> additionalServiceIds)
        {
            if (appointment == null) throw new ArgumentNullException("appointment");

            var error = CheckAppointment(appointment);
            if (error != null) throw new AppException(error);

            var additional = NormalizeAdditionalIds(appointment.ServiceId, additionalServiceIds);

            using (var db = Database.Open())
            using (var transaction = db.BeginTransaction())
            {
                try
                {
                    // Длительность считаем по всем услугам записи, а не только по основной
                    var duration = GetTotalDurationMinutes(db, transaction,
                        appointment.ServiceId, additional);

                    EnsureTimeRangeFree(db, transaction, appointment.DoctorId, appointment.Date,
                        appointment.Time, duration, 0);

                    db.Execute(@"
INSERT INTO Appointments (PatientName, DoctorId, ServiceId, Date, Time, Status, CreatedByUserId)
VALUES (@PatientName, @DoctorId, @ServiceId, @Date, @Time, @Status, @CreatedByUserId);",
                        new
                        {
                            PatientName = appointment.PatientName.Trim(),
                            appointment.DoctorId,
                            appointment.ServiceId,
                            Date = appointment.Date.ToString("yyyy-MM-dd"),
                            Time = appointment.Time,
                            Status = EnumText.StatusToDb(appointment.Status),
                            appointment.CreatedByUserId
                        }, transaction);

                    var id = (int)db.ExecuteScalar<long>("SELECT last_insert_rowid();", null, transaction);

                    foreach (var serviceId in additional)
                    {
                        db.Execute(
                            "INSERT INTO AppointmentServices (AppointmentId, ServiceId) VALUES (@id, @serviceId);",
                            new { id, serviceId }, transaction);
                    }

                    transaction.Commit();
                    Log.Information("Создана запись Id={Id}: {Patient}, врач {DoctorId}, {Date} {Time}",
                        id, appointment.PatientName, appointment.DoctorId, appointment.DateText, appointment.Time);
                    return id;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Log.Error(ex, "Ошибка создания записи на приём");
                    throw;
                }
            }
        }

        /// <summary>Убирает повторы и основную услугу из списка дополнительных.</summary>
        private static List<int> NormalizeAdditionalIds(int primaryServiceId, IEnumerable<int> serviceIds)
        {
            var result = new List<int>();
            if (serviceIds == null) return result;

            var unique = new HashSet<int>();

            foreach (var serviceId in serviceIds)
            {
                if (serviceId <= 0 || serviceId == primaryServiceId) continue;
                if (!unique.Add(serviceId)) continue;

                result.Add(serviceId);
            }

            return result;
        }

        /// <summary>Суммарная длительность основной и дополнительных услуг, минимум 30 минут.</summary>
        private static int GetTotalDurationMinutes(SqliteConnection db, SqliteTransaction transaction,
                                                   int primaryServiceId, List<int> additionalIds)
        {
            var ids = new List<int> { primaryServiceId };
            ids.AddRange(additionalIds);

            var total = db.ExecuteScalar<long>(@"
SELECT IFNULL(SUM(CASE WHEN Duration > 0 THEN Duration ELSE 30 END), 0)
  FROM Services
 WHERE Id IN @ids;", new { ids }, transaction);

            return total > 0 ? (int)total : 30;
        }

        /// <summary>
        /// Проверяет, что интервал [начало; начало + длительность) свободен у врача.
        /// Если время занято, выбрасывает AppException с понятным текстом.
        /// </summary>
        private static void EnsureTimeRangeFree(SqliteConnection db, SqliteTransaction transaction,
                                                int doctorId, DateTime date, string time,
                                                int durationMinutes, int excludeAppointmentId)
        {
            int start;
            if (!TimeRange.TryParseMinutes(time, out start))
                throw new AppException("Не удалось определить время приёма.");

            if (durationMinutes <= 0) durationMinutes = 30;

            var end = start + durationMinutes;

            if (end > 20 * 60)
            {
                throw new AppException(
                    "Приём заканчивается в " + TimeRange.Format(end) +
                    ", а клиника работает до 20:00.\nВыберите более раннее время или сократите набор услуг.");
            }

            var dateText = date.ToString("yyyy-MM-dd");

            var overlaps = CountOverlaps(db, transaction, doctorId, dateText,
                start, end, excludeAppointmentId);

            if (overlaps > 0)
            {
                throw new AppException(
                    "На " + date.ToString("dd.MM.yyyy") + " время с " + TimeRange.Format(start) +
                    " до " + TimeRange.Format(end) + " у выбранного врача уже занято другой записью.\n" +
                    "Выберите другое время или другого врача.");
            }
        }

        /// <summary>Считает записи врача, интервалы которых пересекаются с заданным.</summary>
        private static long CountOverlaps(SqliteConnection db, SqliteTransaction transaction,
                                          int doctorId, string dateText, int start, int end,
                                          int excludeAppointmentId)
        {
            return db.ExecuteScalar<long>(@"
SELECT COUNT(*)
  FROM Appointments a
  JOIN Services s ON s.Id = a.ServiceId
  LEFT JOIN (
        SELECT aps.AppointmentId, SUM(CASE WHEN s2.Duration > 0 THEN s2.Duration ELSE 30 END) AS ExtraDuration
          FROM AppointmentServices aps
          JOIN Services s2 ON s2.Id = aps.ServiceId
         GROUP BY aps.AppointmentId
  ) extra ON extra.AppointmentId = a.Id
 WHERE a.DoctorId = @doctorId
   AND a.Date     = @dateText
   AND a.Status  <> @cancelled
   AND a.Id      <> @exclude
   AND (CAST(substr(a.Time, 1, 2) AS INTEGER) * 60 + CAST(substr(a.Time, 4, 2) AS INTEGER)) < @end
   AND @start < (CAST(substr(a.Time, 1, 2) AS INTEGER) * 60 + CAST(substr(a.Time, 4, 2) AS INTEGER)
                 + CASE WHEN s.Duration > 0 THEN s.Duration ELSE 30 END
                 + IFNULL(extra.ExtraDuration, 0));",
                new
                {
                    doctorId,
                    dateText,
                    cancelled = EnumText.StatusToDb(AppointmentStatus.Cancelled),
                    exclude = excludeAppointmentId,
                    start,
                    end
                }, transaction);
        }

        /// <summary>
        /// Проверяет занятость интервала с учётом длительности приёма.
        /// Касание границ (09:00–09:30 и 09:30–10:00) занятостью не считается.
        /// </summary>
        public bool IsTimeRangeBusy(int doctorId, DateTime date, string time,
                                    int durationMinutes, int excludeAppointmentId = 0)
        {
            int start;
            if (!TimeRange.TryParseMinutes(time, out start)) return false;

            if (durationMinutes <= 0) durationMinutes = 30;

            using (var db = Database.Open())
            {
                return CountOverlaps(db, null, doctorId, date.ToString("yyyy-MM-dd"),
                    start, start + durationMinutes, excludeAppointmentId) > 0;
            }
        }

        /// <summary>Меняет статус записи.</summary>
        public void UpdateStatus(int appointmentId, AppointmentStatus status)
        {
            using (var db = Database.Open())
            {
                db.Execute("UPDATE Appointments SET Status = @status WHERE Id = @appointmentId;",
                    new { status = EnumText.StatusToDb(status), appointmentId });
            }

            Log.Information("Запись Id={Id} переведена в статус «{Status}»",
                appointmentId, EnumText.StatusToDisplay(status));
        }

        /// <summary>Отмена записи (статус "отменён").</summary>
        public void Cancel(int appointmentId)
        {
            EnsureBooked(appointmentId, "отменить");
            UpdateStatus(appointmentId, AppointmentStatus.Cancelled);
        }

        /// <summary>Завершение приёма (статус "завершён").</summary>
        public void Complete(int appointmentId)
        {
            EnsureBooked(appointmentId, "завершить");
            UpdateStatus(appointmentId, AppointmentStatus.Completed);
        }

        /// <summary>
        /// Проверяет, что запись существует и ещё не отменена/не завершена.
        /// Правило продублировано в базе: менять можно только активную запись.
        /// </summary>
        private void EnsureBooked(int appointmentId, string action)
        {
            var appointment = GetById(appointmentId);

            if (appointment == null)
                throw new AppException("Запись на приём не найдена.");

            if (appointment.Status != AppointmentStatus.Booked)
                throw new AppException("Нельзя " + action + " запись со статусом «" +
                                       appointment.StatusText + "».");
        }

        /// <summary>Удаление записи (связанные услуги удалятся каскадно).</summary>
        public void Delete(int appointmentId)
        {
            using (var db = Database.Open())
            {
                db.Execute("DELETE FROM Appointments WHERE Id = @appointmentId;", new { appointmentId });
            }
            Log.Warning("Удалена запись Id={AppointmentId}", appointmentId);
        }

        /// <summary>
        /// Занятые интервалы врача на дату (кроме отменённых записей) в минутах от 00:00.
        /// Длительность записи - сумма длительностей основной и дополнительных услуг,
        /// как в проверке пересечений. Нужны для подбора времени (ScheduleOptimizer).
        /// </summary>
        public List<TimeWindow> GetBusyIntervals(int doctorId, DateTime date)
        {
            using (var db = Database.Open())
            {
                return db.Query<TimeWindow>(@"
SELECT t.StartMinute AS StartMinute,
       t.StartMinute + t.Duration AS EndMinute
  FROM (SELECT (CAST(substr(a.Time, 1, 2) AS INTEGER) * 60 + CAST(substr(a.Time, 4, 2) AS INTEGER)) AS StartMinute,
               (CASE WHEN s.Duration > 0 THEN s.Duration ELSE 30 END
                + IFNULL(extra.ExtraDuration, 0)) AS Duration
          FROM Appointments a
          JOIN Services s ON s.Id = a.ServiceId
          LEFT JOIN (
                SELECT aps.AppointmentId,
                       SUM(CASE WHEN s2.Duration > 0 THEN s2.Duration ELSE 30 END) AS ExtraDuration
                  FROM AppointmentServices aps
                  JOIN Services s2 ON s2.Id = aps.ServiceId
                 GROUP BY aps.AppointmentId
          ) extra ON extra.AppointmentId = a.Id
         WHERE a.DoctorId = @doctorId
           AND a.Date     = @dateText
           AND a.Status  <> @cancelled) t
 ORDER BY t.StartMinute;",
                    new
                    {
                        doctorId,
                        dateText = date.ToString("yyyy-MM-dd"),
                        cancelled = EnumText.StatusToDb(AppointmentStatus.Cancelled)
                    }).AsList();
            }
        }

        // ------------------------- СЧЁТЧИКИ И СТАТИСТИКА -------------------------

        /// <summary>
        /// Проверяет, занято ли время у врача (учитываются все статусы, кроме «отменён»).
        /// excludeAppointmentId — запись, которую не нужно учитывать (при редактировании).
        /// </summary>
        public bool IsSlotBusy(int doctorId, DateTime date, string time, int excludeAppointmentId = 0)
        {
            using (var db = Database.Open())
            {
                var count = db.ExecuteScalar<long>(@"
SELECT COUNT(*)
  FROM Appointments
 WHERE DoctorId = @doctorId
   AND Date     = @dateText
   AND Time     = @time
   AND Status  <> @cancelled
   AND Id      <> @exclude;",
                    new
                    {
                        doctorId,
                        dateText = date.ToString("yyyy-MM-dd"),
                        time,
                        cancelled = EnumText.StatusToDb(AppointmentStatus.Cancelled),
                        exclude = excludeAppointmentId
                    });

                return count > 0;
            }
        }

        public int CountAll()
        {
            using (var db = Database.Open())
            {
                return (int)db.ExecuteScalar<long>("SELECT COUNT(*) FROM Appointments;");
            }
        }

        public int CountToday()
        {
            using (var db = Database.Open())
            {
                return (int)db.ExecuteScalar<long>(
                    "SELECT COUNT(*) FROM Appointments WHERE substr(Date, 1, 10) = @day;",
                    new { day = DateTime.Today.ToString("yyyy-MM-dd") });
            }
        }

        public int CountByDoctor(int doctorId)
        {
            using (var db = Database.Open())
            {
                return (int)db.ExecuteScalar<long>(
                    "SELECT COUNT(*) FROM Appointments WHERE DoctorId = @doctorId;", new { doctorId });
            }
        }

        /// <summary>
        /// Количество записей по каждому дню за период — основа для статистики
        /// (среднее, медиана, дисперсия) и графика.
        /// Отменённые записи не учитываются: приём не состоялся.
        /// </summary>
        public List<DailyCount> GetDailyCounts(DateTime from, DateTime to)
        {
            using (var db = Database.Open())
            {
                return db.Query<DailyCount>(@"
SELECT substr(Date, 1, 10) AS Day, COUNT(*) AS Count
  FROM Appointments
 WHERE Date >= @from AND Date <= @to
   AND Status <> @cancelled
 GROUP BY substr(Date, 1, 10)
 ORDER BY Day;",
                    new
                    {
                        from = from.ToString("yyyy-MM-dd"),
                        to = to.ToString("yyyy-MM-dd") + " 23:59:59",
                        cancelled = EnumText.StatusToDb(AppointmentStatus.Cancelled)
                    }).AsList();
            }
        }

        /// <summary>Проверка данных записи. Возвращает текст ошибки или null.</summary>
        public static string CheckAppointment(Appointment appointment)
        {
            if (appointment == null) return "Запись не заполнена.";
            if (!Validation.IsValidFullName(appointment.PatientName))
                return "ФИО пациента должно содержать минимум два слова (только буквы, пробелы и дефис).";
            if (appointment.DoctorId <= 0) return "Выберите врача.";
            if (appointment.ServiceId <= 0) return "Выберите услугу.";
            if (!Validation.IsFutureOrToday(appointment.Date))
                return "Дата приёма не может быть в прошлом.";
            if (!Validation.IsValidWorkingTime(appointment.Time))
                return "Время должно быть в формате ЧЧ:мм и в пределах 08:00–20:00.";
            return null;
        }
    }
}
