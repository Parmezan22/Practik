using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MedClinic.Forms;
using MedClinic.Models;
using MedClinic.Repositories;
using MedClinic.Services;
using MedClinic.UI;
using Serilog;

namespace MedClinic.Views
{
    /// <summary>
    /// Управление пользователями (доступно только администратору):
    /// список, поиск, добавление, изменение роли и логина, смена пароля, удаление.
    /// Защита: нельзя удалить свою учётную запись и последнего администратора.
    /// </summary>
    public partial class UsersView : UserControl
    {
        private readonly UserRepository _repository = new UserRepository();
        private List<User> _users = new List<User>();

        public UsersView()
        {
            InitializeComponent();

            PlaceholderHelper.SetPlaceholder(txtSearch, "Логин или роль");

            txtSearch.KeyDown += delegate(object s, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    LoadData();
                }
            };

            LoadData();
        }

        // ------------------------------ ЗАГРУЗКА ------------------------------

        private void LoadData()
        {
            try
            {
                _users = _repository.GetAll();

                var search = txtSearch.Text.Trim();
                var shown = new List<User>();

                foreach (var user in _users)
                {
                    if (search.Length == 0 ||
                        user.Login.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        user.RoleDisplay.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        shown.Add(user);
                    }
                }

                dgvUsers.DataSource = null;
                dgvUsers.DataSource = shown;

                lblStatus.Text = search.Length == 0
                    ? "Всего пользователей: " + _users.Count
                    : "Найдено: " + shown.Count + " из " + _users.Count;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка загрузки списка пользователей");
                MessageBox.Show("Не удалось загрузить список пользователей.\n\n" + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Выбранный пользователь (null, если ничего не выбрано).</summary>
        private User SelectedUser
        {
            get
            {
                if (dgvUsers.CurrentRow == null) return null;
                return dgvUsers.CurrentRow.DataBoundItem as User;
            }
        }

        private int AdminCount()
        {
            var count = 0;
            foreach (var user in _users)
                if (user.Role == UserRole.Admin) count++;
            return count;
        }

        private bool IsCurrentUser(User user)
        {
            return user != null && Session.CurrentUser != null && user.Id == Session.CurrentUser.Id;
        }

        // ------------------------------ ПОИСК ------------------------------

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadData();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        // ------------------------------ CRUD ------------------------------

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new UserEditForm(null))
            {
                if (form.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    _repository.Add(form.User, form.NewPassword);
                    LoadData();
                }
                catch (AppException ex)
                {
                    MessageBox.Show(ex.Message, "Проверьте данные",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Ошибка добавления пользователя");
                    MessageBox.Show("Не удалось добавить пользователя.\n\n" + ex.Message,
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            EditSelected();
        }

        private void dgvUsers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) EditSelected();
        }

        private void EditSelected()
        {
            var user = SelectedUser;
            if (user == null)
            {
                MessageBox.Show("Выберите пользователя в таблице.", "Нет выбора",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var wasAdmin = user.Role == UserRole.Admin;

            using (var form = new UserEditForm(user))
            {
                if (form.ShowDialog(this) != DialogResult.OK) return;

                // Нельзя оставить систему без администратора
                if (wasAdmin && form.User.Role != UserRole.Admin && AdminCount() <= 1)
                {
                    MessageBox.Show("Нельзя изменить роль последнего администратора — " +
                                    "в системе должен остаться хотя бы один администратор.",
                        "Операция запрещена", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    _repository.Update(form.User);

                    if (!string.IsNullOrEmpty(form.NewPassword))
                        _repository.ChangePassword(form.User.Id, form.NewPassword);

                    Log.Information("Пользователь {Login} изменён администратором {Admin}",
                        form.User.Login, Session.CurrentUser != null ? Session.CurrentUser.Login : "?");

                    LoadData();
                }
                catch (AppException ex)
                {
                    MessageBox.Show(ex.Message, "Проверьте данные",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Ошибка изменения пользователя Id={Id}", user.Id);
                    MessageBox.Show("Не удалось изменить пользователя.\n\n" + ex.Message,
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var user = SelectedUser;
            if (user == null)
            {
                MessageBox.Show("Выберите пользователя в таблице.", "Нет выбора",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (IsCurrentUser(user))
            {
                MessageBox.Show("Нельзя удалить учётную запись, под которой вы работаете.",
                    "Операция запрещена", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (user.Role == UserRole.Admin && AdminCount() <= 1)
            {
                MessageBox.Show("Нельзя удалить последнего администратора — " +
                                "в системе должен остаться хотя бы один администратор.",
                    "Операция запрещена", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var answer = MessageBox.Show(
                "Удалить пользователя «" + user.Login + "»?\nДействие нельзя отменить.",
                "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (answer != DialogResult.Yes) return;

            try
            {
                _repository.Delete(user.Id);
                Log.Information("Пользователь {Login} удалён администратором {Admin}",
                    user.Login, Session.CurrentUser != null ? Session.CurrentUser.Login : "?");
                LoadData();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка удаления пользователя Id={Id}", user.Id);
                MessageBox.Show("Не удалось удалить пользователя.\n\n" + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
