using System;

namespace MedClinic.Models
{
    /// <summary>
    /// Курс одной валюты по данным Центрального банка РФ.
    /// Курс ЦБ задаётся за номинал (например, 10 юаней или 100 иен),
    /// поэтому для сравнения удобно использовать Rate — курс за 1 единицу.
    /// </summary>
    public class CurrencyRate
    {
        /// <summary>Буквенный код валюты: USD, EUR, CNY и т. д.</summary>
        public string Code { get; set; }

        /// <summary>Название валюты по-русски.</summary>
        public string Name { get; set; }

        /// <summary>Номинал, за который указан курс (1, 10, 100 …).</summary>
        public int Nominal { get; set; }

        /// <summary>Курс за номинал в рублях.</summary>
        public decimal Value { get; set; }

        /// <summary>Курс на предыдущую дату, руб. за номинал.</summary>
        public decimal Previous { get; set; }

        /// <summary>Курс за одну единицу валюты, руб.</summary>
        public decimal Rate
        {
            get { return Nominal > 0 ? Value / Nominal : Value; }
        }

        /// <summary>Изменение курса за номинал: плюс — валюта подорожала.</summary>
        public decimal Change
        {
            get { return Value - Previous; }
        }

        /// <summary>Изменение курса в процентах.</summary>
        public decimal ChangePercent
        {
            get { return Previous != 0 ? (Value - Previous) / Previous * 100m : 0m; }
        }

        /// <summary>Курс за номинал для таблицы, 4 знака после запятой.</summary>
        public string ValueText
        {
            get { return Value.ToString("N4"); }
        }

        /// <summary>Курс за одну единицу валюты для таблицы.</summary>
        public string RateText
        {
            get { return Rate.ToString("N4"); }
        }

        /// <summary>Изменение со знаком: «+0,1234» или «-0,0567».</summary>
        public string ChangeText
        {
            get { return (Change > 0 ? "+" : string.Empty) + Change.ToString("N4"); }
        }

        /// <summary>Изменение в процентах со знаком.</summary>
        public string ChangePercentText
        {
            get { return (ChangePercent > 0 ? "+" : string.Empty) + ChangePercent.ToString("N2") + " %"; }
        }

        public override string ToString()
        {
            return Code + " — " + Name;
        }
    }
}
