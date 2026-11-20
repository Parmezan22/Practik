using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MedClinic.Models;
using Newtonsoft.Json.Linq;

namespace MedClinic.Services
{
    /// <summary>Результат загрузки курсов: дата курса и список валют.</summary>
    public class CurrencyRatesResult
    {
        /// <summary>Дата, на которую установлены курсы.</summary>
        public DateTime Date { get; set; }

        /// <summary>Курсы валют, отсортированные по коду.</summary>
        public List<CurrencyRate> Rates { get; set; } = new List<CurrencyRate>();
    }

    /// <summary>
    /// Загрузка официальных курсов валют Центрального банка РФ.
    ///
    /// Данные берутся с открытого сервиса cbr-xml-daily.ru в формате JSON.
    /// Адрес и таймаут задаются в appsettings.json (раздел Currency).
    /// При любой сетевой ошибке показывается понятное сообщение на русском.
    /// </summary>
    public static class CurrencyService
    {
        /// <summary>
        /// Загружает курсы валют. Метод асинхронный, чтобы окно программы
        /// не «замирало» на время запроса к интернету.
        /// </summary>
        public static async Task<CurrencyRatesResult> LoadAsync()
        {
            var settings = AppConfig.Current.Currency;

            // Windows 7 по умолчанию может не включить современный протокол TLS 1.2
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);
                    client.DefaultRequestHeaders.Add("User-Agent", "MedClinic/1.0");

                    using (var response = await client.GetAsync(settings.Url).ConfigureAwait(false))
                    {
                        response.EnsureSuccessStatusCode();

                        var bytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                        var charset = response.Content.Headers.ContentType != null
                            ? response.Content.Headers.ContentType.CharSet
                            : null;

                        var json = ResolveEncoding(charset).GetString(bytes);
                        return Parse(json);
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                throw new AppException(
                    "Сервер курсов валют не ответил за " + settings.TimeoutSeconds +
                    " секунд.\n\nПроверьте подключение к интернету и попробуйте ещё раз.", ex);
            }
            catch (HttpRequestException ex)
            {
                throw new AppException(
                    "Не удалось подключиться к серверу курсов валют.\n\n" +
                    "Проверьте подключение к интернету, настройки прокси-сервера " +
                    "и попробуйте ещё раз.", ex);
            }
            catch (AppException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AppException(
                    "Не удалось загрузить курсы валют.\n\n" + ex.Message, ex);
            }
        }

        /// <summary>
        /// Разбирает JSON с курсами. Выделен отдельным методом,
        /// чтобы его можно было проверить тестами без интернета.
        /// </summary>
        public static CurrencyRatesResult Parse(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new AppException("Сервер вернул пустой ответ вместо курсов валют.");

            JObject root;
            try
            {
                root = JObject.Parse(json);
            }
            catch (Exception ex)
            {
                throw new AppException(
                    "Ответ сервера не похож на файл курсов валют. Возможно, сайт недоступен " +
                    "или изменил формат данных.", ex);
            }

            var result = new CurrencyRatesResult();

            var dateToken = root["Date"];
            DateTime date;
            if (dateToken != null &&
                DateTime.TryParse(dateToken.ToString(), CultureInfo.InvariantCulture,
                                  DateTimeStyles.None, out date))
            {
                result.Date = date;
            }
            else
            {
                result.Date = DateTime.Today;
            }

            var valute = root["Valute"] as JObject;
            if (valute == null)
                throw new AppException("В ответе сервера нет списка валют.");

            foreach (var property in valute.Properties())
            {
                var item = property.Value as JObject;
                if (item == null) continue;

                var code = GetString(item, "CharCode");
                if (string.IsNullOrWhiteSpace(code)) code = property.Name;

                var rate = new CurrencyRate
                {
                    Code = code,
                    Name = GetString(item, "Name"),
                    Nominal = GetInt(item, "Nominal", 1),
                    Value = GetDecimal(item, "Value"),
                    Previous = GetDecimal(item, "Previous")
                };

                if (rate.Nominal <= 0) rate.Nominal = 1;
                result.Rates.Add(rate);
            }

            if (result.Rates.Count == 0)
                throw new AppException("Сервер вернул пустой список валют.");

            result.Rates.Sort(delegate (CurrencyRate first, CurrencyRate second)
            {
                return string.Compare(first.Code, second.Code, StringComparison.OrdinalIgnoreCase);
            });

            return result;
        }

        /// <summary>Кодировка ответа: ЦБ отдаёт UTF-8, но на всякий случай учитываем charset.</summary>
        private static Encoding ResolveEncoding(string charset)
        {
            if (!string.IsNullOrWhiteSpace(charset))
            {
                var name = charset.Trim().Trim('"').ToLowerInvariant();
                try
                {
                    if (name == "windows-1251" || name == "cp1251")
                        return Encoding.GetEncoding(1251);
                    return Encoding.GetEncoding(name);
                }
                catch (ArgumentException)
                {
                    // Неизвестная кодировка — используем UTF-8
                }
            }

            return new UTF8Encoding(false);
        }

        private static string GetString(JObject item, string name)
        {
            var token = item[name];
            return token != null ? token.ToString() : string.Empty;
        }

        private static int GetInt(JObject item, string name, int defaultValue)
        {
            var token = item[name];
            if (token == null || token.Type == JTokenType.Null) return defaultValue;

            try
            {
                return token.Value<int>();
            }
            catch (Exception)
            {
                int value;
                return int.TryParse(token.ToString(), NumberStyles.Integer,
                                    CultureInfo.InvariantCulture, out value) ? value : defaultValue;
            }
        }

        private static decimal GetDecimal(JObject item, string name)
        {
            var token = item[name];
            if (token == null || token.Type == JTokenType.Null) return 0m;

            try
            {
                // Value<decimal> разбирает число независимо от региональных настроек
                return token.Value<decimal>();
            }
            catch (Exception)
            {
                decimal value;
                return decimal.TryParse(token.ToString(), NumberStyles.Number,
                                        CultureInfo.InvariantCulture, out value) ? value : 0m;
            }
        }
    }
}
