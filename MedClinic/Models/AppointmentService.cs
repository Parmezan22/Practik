namespace MedClinic.Models
{
    /// <summary>
    /// Строка таблицы связи "запись — услуга" (таблица AppointmentServices).
    /// Одна запись может содержать несколько услуг, одна услуга может входить в разные записи.
    /// </summary>
    public class AppointmentService
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public int ServiceId { get; set; }

        /// <summary>Название услуги (заполняется JOIN-запросом).</summary>
        public string ServiceName { get; set; }
    }
}
