using MedClinic.Models;

namespace MedClinic.Services
{
    /// <summary>
    /// Текущая сессия: кто вошёл в систему.
    /// Заполняется после успешного входа и очищается при выходе.
    /// </summary>
    public static class Session
    {
        public static User CurrentUser { get; set; }

        public static bool IsAuthenticated
        {
            get { return CurrentUser != null; }
        }

        public static UserRole Role
        {
            get { return CurrentUser != null ? CurrentUser.Role : UserRole.Guest; }
        }

        public static void Clear()
        {
            CurrentUser = null;
        }
    }
}
