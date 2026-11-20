using System;
using System.Windows.Forms;
using MedClinic.Models;
using MedClinic.Services;

namespace MedClinic.Forms
{
    /// <summary>
    /// Диалог добавления или редактирования пользователя.
    /// При редактировании пароль можно не менять — тогда поле остаётся пустым.
    /// После нажатия «Сохранить» данные доступны в свойствах User и NewPassword.
    /// </summary>
    public partial class UserEditForm : Form
    {
        private readonly bool _isNew;

        /// <summary>Данные пользователя, заполненные в форме.</summary>
        public User User { get; private set; }

        /// <summary>Новый пароль или null, если пароль менять не нужно.</summary>
        public string NewPassword { get; private set; }

        public UserEditForm(User user)
        {
            InitializeComponent();

            _isNew = (user == null);
            User = user ?? new User();

            Text = _isNew ? "Новый пользователь" : "Редактирование пользователя";
            lblHeader.Text = Text;

            FillRoles();

            if (_isNew)
            {
                cmbRole.SelectedIndex = 1; // по умолчанию «Клиент»
            }
            else
            {
                txtLogin.Text = User.Login;
                SelectRole(User.Role);

                lblPassword.Text = "Новый пароль";
                lblPasswordHint.Text = "Оставьте поле пустым, чтобы не менять пароль.";
                lblConfirm.Text = "Повтор нового пароля";
            }

            // Свою роль менять нельзя: иначе администратор может лишить себя доступа
            if (!_isNew && Session.CurrentUser != null && User.Id == Session.CurrentUser.Id)
            {
                cmbRole.Enabled = false;
                lblRole.Text = "Роль (свою роль изменить нельзя)";
            }
        }

        /// <summary>Заполняет список ролей.</summary>
        private void FillRoles()
        {
            cmbRole.Items.Clear();
            cmbRole.Items.Add(new RoleItem(UserRole.Guest, "Гость"));
            cmbRole.Items.Add(new RoleItem(UserRole.Client, "Клиент"));
            cmbRole.Items.Add(new RoleItem(UserRole.Manager, "Менеджер"));
            cmbRole.Items.Add(new RoleItem(UserRole.Admin, "Администратор"));
        }

        private void SelectRole(UserRole role)
        {
            for (var i = 0; i < cmbRole.Items.Count; i++)
            {
                var item = cmbRole.Items[i] as RoleItem;
                if (item != null && item.Value == role)
                {
                    cmbRole.SelectedIndex = i;
                    return;
                }
            }

            cmbRole.SelectedIndex = 0;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var login = txtLogin.Text.Trim();

            if (!Validation.IsValidLogin(login))
            {
                MessageBox.Show("Логин должен содержать от 3 до 32 символов: латиница, цифры, " +
                                "точка, дефис или подчёркивание.",
                    "Проверьте данные", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLogin.Focus();
                return;
            }

            var password = txtPassword.Text;

            // При добавлении пароль обязателен, при изменении — только если введён
            if (_isNew || password.Length > 0)
            {
                if (!Validation.IsValidPassword(password))
                {
                    MessageBox.Show("Пароль должен содержать от 5 до 64 символов.",
                        "Проверьте данные", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPassword.Focus();
                    return;
                }

                if (password != txtConfirm.Text)
                {
                    MessageBox.Show("Пароль и его повтор не совпадают.",
                        "Проверьте данные", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtConfirm.Focus();
                    return;
                }
            }
            else if (txtConfirm.Text.Length > 0)
            {
                MessageBox.Show("Введите новый пароль или очистите оба поля.",
                    "Проверьте данные", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            var roleItem = cmbRole.SelectedItem as RoleItem;
            if (roleItem == null)
            {
                MessageBox.Show("Выберите роль пользователя.", "Проверьте данные",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbRole.Focus();
                return;
            }

            User = new User
            {
                Id = User.Id,
                Login = login,
                Role = roleItem.Value
            };

            NewPassword = password.Length > 0 ? password : null;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>Элемент списка ролей: значение перечисления и русская подпись.</summary>
        private class RoleItem
        {
            public UserRole Value { get; private set; }
            public string Text { get; private set; }

            public RoleItem(UserRole value, string text)
            {
                Value = value;
                Text = text;
            }

            public override string ToString()
            {
                return Text;
            }
        }
    }
}
