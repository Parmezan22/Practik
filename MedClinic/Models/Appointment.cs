using System;
using System.Collections.Generic;

namespace MedClinic.Models
{
    /// <summary>
    /// Запись на приём (таблица Appointments).
    /// ServiceId — основная услуга, дополнительные услуги хранятся
    /// в таблице связи AppointmentServices (требование "несколько услуг в одной записи").
    /// Поля DoctorName и ServiceName заполняются запросами с JOIN и нужны только для показа.
    /// </summary>
    public class Appointment
    {
        public int Id { get; set; }

        /// <summary>ФИО пациента (вводится вручную).</summary>
        public string PatientName { get; set; }

        public int DoctorId { get; set; }

        /// <summary>Основная услуга.</summary>
        public int ServiceId { get; set; }

        /// <summary>Дата приёма (хранится в БД как текст "гггг-ММ-дд").</summary>
        public DateTime Date { get; set; }

        /// <summary>Время приёма в формате "ЧЧ:мм".</summary>
        public string Time { get; set; }

        /// <summary>
        /// Статус в виде русского текста из базы данных.
        /// Dapper не переводит текст в перечисление автоматически,
        /// поэтому храним строку, а свойство Status переводит её.
        /// </summary>
        public string StatusDb { get; set; }

        /// <summary>Статус записи: записан, отменён или завершён.</summary>
        public AppointmentStatus Status
        {
            get { return EnumText.StatusFromDb(StatusDb); }
            set { StatusDb = EnumText.StatusToDb(value); }
        }

        /// <summary>
        /// Кто создал запись (Id пользователя). Нужно, чтобы клиент мог отменять
        /// только собственные записи. null — запись добавлена системой/админом.
        /// </summary>
        public int? CreatedByUserId { get; set; }

        // ----- Поля только для отображения (заполняются JOIN-запросом) -----

        /// <summary>ФИО врача.</summary>
        public string DoctorName { get; set; }

        /// <summary>Название основной услуги.</summary>
        public string ServiceName { get; set; }

        /// <summary>Цена основной услуги, руб. (заполняется для отчёта).</summary>
        public decimal Price { get; set; }

        /// <summary>Сумма дополнительных услуг, руб. (заполняется для отчёта).</summary>
        public decimal ExtraPrice { get; set; }

        /// <summary>Общая стоимость записи: основная и дополнительные услуги.</summary>
        public decimal TotalPrice
        {
            get { return Price + ExtraPrice; }
        }

        /// <summary>Общая стоимость в виде строки для таблицы.</summary>
        public string TotalPriceText
        {
            get { return TotalPrice.ToString("N2"); }
        }

        /// <summary>Дополнительные услуги (заполняются отдельным запросом).</summary>
        public List<Service> AdditionalServices { get; set; } = new List<Service>();

        /// <summary>Дата в виде "дд.ММ.гггг".</summary>
        public string DateText
        {
            get { return Date.ToString("dd.MM.yyyy"); }
        }

        /// <summary>Дата и время одной строкой.</summary>
        public string DateTimeText
        {
            get { return Date.ToString("dd.MM.yyyy") + " " + Time; }
        }

        /// <summary>Статус по-русски для экрана.</summary>
        public string StatusText
        {
            get { return EnumText.StatusToDisplay(Status); }
        }

        /// <summary>Можно ли ещё отменить запись.</summary>
        public bool CanCancel
        {
            get { return Status == AppointmentStatus.Booked; }
        }

        public override string ToString()
        {
            return DateTimeText + " — " + PatientName;
        }
    }
}
