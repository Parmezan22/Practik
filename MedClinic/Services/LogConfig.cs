using System;
using System.IO;
using Serilog;
using Serilog.Events;

namespace MedClinic.Services
{
    /// <summary>
    /// Настройка Serilog. Журнал пишется в файл Logs\clinic-ГГГГММДД.log,
    /// новый файл создаётся каждый день, старые удаляются автоматически.
    /// </summary>
    public static class LogConfig
    {
        /// <summary>Создаёт и включает глобальный логгер. Вызывать один раз при старте.</summary>
        public static void Configure()
        {
            var settings = AppConfig.Current.Logging;
            var logDirectory = AppConfig.Current.LogFolderPath;

            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);

            var level = ParseLevel(settings.MinimumLevel);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Is(level)
                .WriteTo.File(
                    path: Path.Combine(logDirectory, "clinic-.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: settings.RetainedFileCountLimit,
                    shared: true,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            Log.Information("Логирование запущено. Файл: {Folder}\\clinic-дата.log, уровень: {Level}",
                logDirectory, level);
        }

        /// <summary>Преобразует текст из конфигурации в уровень Serilog.</summary>
        private static LogEventLevel ParseLevel(string text)
        {
            LogEventLevel level;
            if (Enum.TryParse(text, true, out level))
                return level;

            return LogEventLevel.Information;
        }
    }
}
