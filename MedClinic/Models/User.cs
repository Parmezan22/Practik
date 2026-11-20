namespace MedClinic.Models
{
    /// <summary>
    /// Пользователь системы (таблица Users).
    /// Пароль в базе не хранится в открытом виде — только хэш SHA256.
    /// </summary>
    public class User
    {
        public int Id { get; set; }

        /// <summary>Логин (уникальный, регистр не важен).</summary>
        public string Login { get; set; }

        /// <summary>Хэш пароля SHA256 (64 символа в нижнем регистре).</summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Роль в виде русского текста из базы данных.
        /// Dapper не умеет сам переводить текст в перечисление, поэтому
        /// значение хранится строкой, а свойство Role переводит его.
        /// </summary>
        public string RoleDb { get; set; }

        /// <summary>Роль пользователя (гость, клиент, менеджер, админ).</summary>
        public UserRole Role
        {
            get { return EnumText.RoleFromDb(RoleDb); }
            set { RoleDb = EnumText.RoleToDb(value); }
        }

        /// <summary>Название роли для отображения на экране.</summary>
        public string RoleDisplay
        {
            get { return EnumText.RoleToDisplay(Role); }
        }

        public override string ToString()
        {
            return Login + " (" + RoleDisplay + ")";
        }
    }
}
