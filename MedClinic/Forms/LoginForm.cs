using System;
using System.Drawing;
using System.Windows.Forms;
using MedClinic.Models;
using MedClinic.Repositories;
using MedClinic.Services;
using MedClinic.UI;
using Serilog;

namespace MedClinic.Forms
{
    /// <summary>
    /// Форма входа в систему.
    /// Проверяет логин и пароль, после успеха возвращает заполненное
    /// свойство AuthenticatedUser и закрывается с результатом OK.
    /// </summary>
    public partial class LoginForm : Form
    {
        private readonly AuthService _authService;
        private int _failedAttempts;

        /// <summary>Пользователь, успешно вошедший в систему (null, если вход не выполнен).</summary>
        public User AuthenticatedUser { get; private set; }

        public LoginForm()
        {
            InitializeComponent();

            _authService = new AuthService(new UserRepository());

            // Подсказки внутри полей: исчезают, как только пользователь начинает ввод
            PlaceholderHelper.SetPlaceholder(txtLogin, "Например: admin");
            PlaceholderHelper.SetPlaceholder(txtPassword, "Введите пароль");

            lblError.Text = string.Empty;
            lblVersion.Text = "Версия " + AppConfig.Current.Application.Version;

            // При любом изменении текста убираем старую ошибку с экрана
            txtLogin.TextChanged += delegate { lblError.Text = string.Empty; };
            txtPassword.TextChanged += delegate { lblError.Text = string.Empty; };

            // Курсор в поле логина после открытия формы
            Shown += delegate { txtLogin.Focus(); };
        }

        // ------------------------------- СОБЫТИЯ -------------------------------

        private void btnLogin_Click(object sender, EventArgs e)
        {
            PerformLogin();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Log.Information("Пользователь закрыл форму входа");
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
        }

        // --------------------------- ЛОГИКА ВХОДА ----------------------------

        private void PerformLogin()
        {
            lblError.Text = string.Empty;

            var login = txtLogin.Text.Trim();
            var password = txtPassword.Text;

            // 1. Проверка заполнения полей
            if (login.Length == 0)
            {
                ShowError("Введите логин.");
                txtLogin.Focus();
                return;
            }

            if (password.Length == 0)
            {
                ShowError("Введите пароль.");
                txtPassword.Focus();
                return;
            }

            // 2. Проверка учётных данных
            btnLogin.Enabled = false;
            btnLogin.Text = "ПРОВЕРКА...";
            Cursor = Cursors.WaitCursor;

            try
            {
                var user = _authService.Login(login, password);

                AuthenticatedUser = user;
                Session.CurrentUser = user;

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (AuthException ex)
            {
                _failedAttempts++;
                Log.Warning("Неудачный вход ({Attempt}): {Message}", _failedAttempts, ex.Message);
                ShowError(ex.Message);
                txtPassword.SelectAll();
                txtPassword.Focus();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Непредвиденная ошибка при входе в систему");
                ShowError("Не удалось выполнить вход. Подробности записаны в журнал Logs.\n" + ex.Message);
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "ВОЙТИ";
                Cursor = Cursors.Default;
            }
        }

        private void ShowError(string message)
        {
            lblError.ForeColor = Color.FromArgb(200, 45, 45);
            lblError.Text = message;
        }
    }
}
