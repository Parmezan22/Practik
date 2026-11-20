namespace MedClinic.Models
{
    /// <summary>Медицинская услуга (таблица Services).</summary>
    public class Service
    {
        public int Id { get; set; }

        /// <summary>Название услуги.</summary>
        public string Name { get; set; }

        /// <summary>Цена в рублях.</summary>
        public decimal Price { get; set; }

        /// <summary>Длительность в минутах.</summary>
        public int Duration { get; set; }

        /// <summary>Цена в виде строки: "1 500,00 руб."</summary>
        public string PriceText
        {
            get { return Price.ToString("N2") + " руб."; }
        }

        /// <summary>Длительность в виде строки: "30 мин."</summary>
        public string DurationText
        {
            get { return Duration + " мин."; }
        }

        /// <summary>Текст для выпадающего списка.</summary>
        public string DisplayName
        {
            get { return Name + " (" + PriceText + ", " + DurationText + ")"; }
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
