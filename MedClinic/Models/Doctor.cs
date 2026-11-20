namespace MedClinic.Models
{
    /// <summary>Врач клиники (таблица Doctors).</summary>
    public class Doctor
    {
        public int Id { get; set; }

        /// <summary>ФИО врача.</summary>
        public string FullName { get; set; }

        /// <summary>Специальность (терапевт, кардиолог и т.д.).</summary>
        public string Specialty { get; set; }

        /// <summary>Телефон.</summary>
        public string Phone { get; set; }

        /// <summary>Номер кабинета.</summary>
        public string Cabinet { get; set; }

        /// <summary>Текст для выпадающего списка: "Иванова М.П. — Терапевт".</summary>
        public string DisplayName
        {
            get { return FullName + " — " + Specialty; }
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
