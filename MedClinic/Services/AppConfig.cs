using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace MedClinic.Services
{
    /// <summary>Настройки базы данных из appsettings.json.</summary>
    public class DatabaseSettings
    {
        /// <summary>Папка с файлом базы (относительно папки программы).</summary>
        public string Folder { get; set; } = "Data";

        /// <summary>Имя файла базы данных.</summary>
        public string FileName { get; set; } = "clinic.db";
    }

    /// <summary>Настройки логирования из appsettings.json.</summary>
    public class LoggingSettings
    {
        /// <summary>Папка для файлов журнала.</summary>
        public string Folder { get; set; } = "Logs";

        /// <summary>Минимальный уровень: Verbose, Debug, Information, Warning, Error, Fatal.</summary>
        public string MinimumLevel { get; set; } = "Information";

        /// <summary>Сколько дней хранить файлы журнала.</summary>
        public int RetainedFileCountLimit { get; set; } = 30;
    }

    /// <summary>Общие настройки приложения.</summary>
    public class ApplicationSettings
    {
        public string Title { get; set; } = "Система записи на приём";
        public string ClinicName { get; set; } = "Медицинский центр «Здоровье+»";
        public string Version { get; set; } = "1.0.0";
    }

    /// <summary>Настройки загрузки курсов валют с сайта Центрального банка.</summary>
    public class CurrencySettings
    {
        /// <summary>Адрес файла с курсами в формате JSON.</summary>
        public string Url { get; set; } = "https://www.cbr-xml-daily.ru/daily_json.js";

        /// <summary>Таймаут запроса в секундах.</summary>
        public int TimeoutSeconds { get; set; } = 15;
    }

    /// <summary>
    /// Чтение файла appsettings.json.
    /// Файл лежит рядом с exe и копируется туда при сборке.
    /// </summary>
    public sealed class AppConfig
    {
        public DatabaseSettings Database { get; set; } = new DatabaseSettings();
        public LoggingSettings Logging { get; set; } = new LoggingSettings();
        public ApplicationSettings Application { get; set; } = new ApplicationSettings();
        public CurrencySettings Currency { get; set; } = new CurrencySettings();

        /// <summary>Текущая конфигурация (доступна после вызова Load).</summary>
        public static AppConfig Current { get; private set; } = new AppConfig();

        /// <summary>Путь к прочитанному файлу конфигурации.</summary>
        public static string ConfigFilePath { get; private set; }

        /// <summary>Полный путь к файлу базы данных.</summary>
        public string DatabasePath
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                    Database.Folder ?? "Data",
                                    Database.FileName ?? "clinic.db");
            }
        }

        /// <summary>Полный путь к папке журналов.</summary>
        public string LogFolderPath
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Logging.Folder ?? "Logs");
            }
        }

        /// <summary>
        /// Загружает конфигурацию. Если файла нет или он повреждён — используются
        /// значения по умолчанию, приложение продолжает работать.
        /// Вызывается до настройки логгера, поэтому здесь нет записи в журнал.
        /// </summary>
        public static void Load(string filePath = null)
        {
            ConfigFilePath = filePath ??
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

            AppConfig config = null;

            if (File.Exists(ConfigFilePath))
            {
                var json = File.ReadAllText(ConfigFilePath, Encoding.UTF8);
                config = JsonConvert.DeserializeObject<AppConfig>(json);
            }

            if (config == null)
                config = new AppConfig();

            config.ApplyDefaults();
            Current = config;
        }

        /// <summary>Подставляет значения по умолчанию для пустых полей.</summary>
        private void ApplyDefaults()
        {
            if (Database == null) Database = new DatabaseSettings();
            if (Logging == null) Logging = new LoggingSettings();
            if (Application == null) Application = new ApplicationSettings();
            if (Currency == null) Currency = new CurrencySettings();

            if (string.IsNullOrWhiteSpace(Database.Folder)) Database.Folder = "Data";
            if (string.IsNullOrWhiteSpace(Database.FileName)) Database.FileName = "clinic.db";
            if (string.IsNullOrWhiteSpace(Logging.Folder)) Logging.Folder = "Logs";
            if (string.IsNullOrWhiteSpace(Logging.MinimumLevel)) Logging.MinimumLevel = "Information";
            if (Logging.RetainedFileCountLimit <= 0) Logging.RetainedFileCountLimit = 30;
            if (string.IsNullOrWhiteSpace(Application.Title)) Application.Title = "Система записи на приём";
            if (string.IsNullOrWhiteSpace(Application.ClinicName)) Application.ClinicName = "Медицинский центр «Здоровье+»";
            if (string.IsNullOrWhiteSpace(Application.Version)) Application.Version = "1.0.0";
            if (string.IsNullOrWhiteSpace(Currency.Url))
                Currency.Url = "https://www.cbr-xml-daily.ru/daily_json.js";
            if (Currency.TimeoutSeconds <= 0) Currency.TimeoutSeconds = 15;
        }
    }
}
