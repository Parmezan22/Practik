using System;
using System.Windows.Forms;
using MedClinic.Data;
using MedClinic.Forms;
using MedClinic.Models;
using MedClinic.Services;
using Serilog;

namespace MedClinic
{
    /// <summary>
    /// Точка входа приложения.
    /// Порядок запуска: конфигурация -> логирование -> база данных -> окно входа -> главное окно.
    /// </summary>
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 1. Чтение appsettings.json
            try
            {
                AppConfig.Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось прочитать файл конфигурации appsettings.json.\n\n" + ex.Message,
                    "Ошибка запуска", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 2. Настройка журнала Serilog (до этого сообщения писать некуда)
            LogConfig.Configure();
            Log.Information("==================================================");
            Log.Information("Запуск приложения «{Title}» версии {Version}",
                AppConfig.Current.Application.Title, AppConfig.Current.Application.Version);

            // 3. Подготовка базы данных: создание файла, миграции, начальные данные
            try
            {
                DatabaseInitializer.Initialize();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Критическая ошибка инициализации базы данных");
                MessageBox.Show(
                    "Не удалось подготовить базу данных.\n\n" + ex.Message +
                    "\n\nПодробности записаны в папку Logs.",
                    "Ошибка базы данных", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.CloseAndFlush();
                return;
            }

            // 4. Цикл "вход -> работа -> смена пользователя"
            try
            {
                var showLoginAgain = true;

                while (showLoginAgain)
                {
                    User user;

                    using (var loginForm = new LoginForm())
                    {
                        if (loginForm.ShowDialog() != DialogResult.OK || loginForm.AuthenticatedUser == null)
                        {
                            Log.Information("Вход не выполнен, приложение закрывается");
                            break;
                        }
                        user = loginForm.AuthenticatedUser;
                    }

                    Session.CurrentUser = user;

                    using (var mainForm = new MainForm(user))
                    {
                        Application.Run(mainForm);
                        showLoginAgain = mainForm.LogoutRequested;
                    }

                    Session.Clear();
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Непредвиденная ошибка в работе приложения");
                MessageBox.Show(
                    "Произошла непредвиденная ошибка.\n\n" + ex.Message +
                    "\n\nПодробности записаны в папку Logs.",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Log.Information("Завершение работы приложения");
                Log.CloseAndFlush();
            }
        }
    }
}
