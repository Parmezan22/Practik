using System.Text;
using MedClinic.Models;

namespace MedClinic.Services
{
    /// <summary>
    /// Правила разграничения доступа по ролям.
    /// Все проверки собраны в одном месте, чтобы формы не дублировали условия.
    /// </summary>
    public static class Permissions
    {
        /// <summary>Справочники может смотреть любой авторизованный пользователь.</summary>
        public static bool CanViewDictionaries(UserRole role)
        {
            return true;
        }

        /// <summary>Создавать запись на приём может клиент, менеджер и админ.</summary>
        public static bool CanCreateAppointment(UserRole role)
        {
            return role == UserRole.Client || role == UserRole.Manager || role == UserRole.Admin;
        }

        /// <summary>Отменять свои записи может клиент и выше.</summary>
        public static bool CanCancelOwnAppointment(UserRole role)
        {
            return role == UserRole.Client || role == UserRole.Manager || role == UserRole.Admin;
        }

        /// <summary>Отменять любые записи может только менеджер и админ.</summary>
        public static bool CanCancelAnyAppointment(UserRole role)
        {
            return role == UserRole.Manager || role == UserRole.Admin;
        }

        /// <summary>Отмечать приём завершённым может только менеджер и админ.</summary>
        public static bool CanCompleteAppointment(UserRole role)
        {
            return role == UserRole.Manager || role == UserRole.Admin;
        }

        /// <summary>Добавлять и изменять врачей/услуги может менеджер и админ.</summary>
        public static bool CanEditDictionaries(UserRole role)
        {
            return role == UserRole.Manager || role == UserRole.Admin;
        }

        /// <summary>Удалять записи справочников может только админ.</summary>
        public static bool CanDeleteDictionaries(UserRole role)
        {
            return role == UserRole.Admin;
        }

        /// <summary>Отчёты доступны менеджеру и админу.</summary>
        public static bool CanViewReports(UserRole role)
        {
            return role == UserRole.Manager || role == UserRole.Admin;
        }

        /// <summary>Статистика доступна менеджеру и админу.</summary>
        public static bool CanViewStatistics(UserRole role)
        {
            return role == UserRole.Manager || role == UserRole.Admin;
        }

        /// <summary>Управление пользователями — только админ.</summary>
        public static bool CanManageUsers(UserRole role)
        {
            return role == UserRole.Admin;
        }

        /// <summary>Просмотр журнала событий — только админ.</summary>
        public static bool CanViewLogs(UserRole role)
        {
            return role == UserRole.Admin;
        }

        /// <summary>Текст со списком прав — показывается на главной вкладке.</summary>
        public static string Describe(UserRole role)
        {
            var sb = new StringBuilder();
            sb.AppendLine("• просмотр справочников врачей и услуг — разрешён;");
            sb.AppendLine(CanCreateAppointment(role)
                ? "• создание записи на приём — разрешено;"
                : "• создание записи на приём — запрещено (только просмотр);");
            sb.AppendLine(CanEditDictionaries(role)
                ? "• добавление и редактирование врачей и услуг — разрешено;"
                : "• добавление и редактирование справочников — запрещено;");
            sb.AppendLine(CanDeleteDictionaries(role)
                ? "• удаление записей справочников — разрешено;"
                : "• удаление записей справочников — запрещено;");
            sb.AppendLine(CanCancelAnyAppointment(role)
                ? "• отмена любых записей — разрешена;"
                : "• отмена чужих записей — запрещена;");
            sb.AppendLine(CanViewReports(role)
                ? "• отчёты и экспорт в Excel — разрешены;"
                : "• отчёты и экспорт — недоступны;");
            sb.AppendLine(CanViewStatistics(role)
                ? "• статистика и графики — разрешены;"
                : "• статистика и графики — недоступны;");
            sb.AppendLine(CanManageUsers(role)
                ? "• управление пользователями — разрешено;"
                : "• управление пользователями — недоступно;");
            sb.Append(CanViewLogs(role)
                ? "• просмотр журнала событий — разрешён."
                : "• просмотр журнала событий — недоступен.");

            return sb.ToString();
        }
    }
}
