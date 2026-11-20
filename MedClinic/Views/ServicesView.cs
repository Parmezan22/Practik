using System;
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
    /// Справочник услуг: поиск, сортировка по клику на заголовок,
    /// постраничный вывод, добавление, изменение, удаление.
    /// </summary>
    public partial class ServicesView : UserControl
    {
        private readonly ServiceRepository _repository = new ServiceRepository();

        private string _search = string.Empty;
        private int _page = 1;
        private int _pageSize = 10;
        private string _sortColumn = "Name";
        private bool _sortAscending = true;
        private bool _canEdit;
        private bool _canDelete;
        private bool _initializingPageSize;

        public ServicesView()
        {
            InitializeComponent();

            PlaceholderHelper.SetPlaceholder(txtSearch, "Название услуги");

            _initializingPageSize = true;
            cmbPageSize.Items.Add("10");
            cmbPageSize.Items.Add("20");
            cmbPageSize.Items.Add("50");
            cmbPageSize.SelectedIndex = 0;
            _initializingPageSize = false;

            txtSearch.KeyDown += delegate(object s, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    StartSearch();
                }
            };

            LoadData();
        }

        /// <summary>Права роли: менять и удалять справочник могут не все.</summary>
        public void ApplyRole(UserRole role)
        {
            _canEdit = Permissions.CanEditDictionaries(role);
            _canDelete = Permissions.CanDeleteDictionaries(role);

            btnAdd.Enabled = _canEdit;
            btnEdit.Enabled = _canEdit;
            btnDelete.Enabled = _canDelete;
        }

        // ------------------------------ ЗАГРУЗКА ------------------------------

        private void LoadData()
        {
            try
            {
                int total;
                var services = _repository.GetPaged(_search, _sortColumn, _sortAscending,
                                                    _page, _pageSize, out total);

                var pageCount = Math.Max(1, (int)Math.Ceiling(total / (double)_pageSize));
                if (_page > pageCount)
                {
                    _page = pageCount;
                    services = _repository.GetPaged(_search, _sortColumn, _sortAscending,
                                                    _page, _pageSize, out total);
                }

                dgvServices.DataSource = null;
                dgvServices.DataSource = services;

                lblFound.Text = "Найдено: " + total;
                lblTotal.Text = "Всего услуг: " + total;
                lblPage.Text = "Страница " + _page + " из " + pageCount;
                btnPrev.Enabled = _page > 1;
                btnNext.Enabled = _page < pageCount;
                UpdateSortLabel();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка загрузки справочника услуг");
                MessageBox.Show("Не удалось загрузить список услуг.\n\n" + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateSortLabel()
        {
            string title;
            switch (_sortColumn)
            {
                case "Price": title = "по цене"; break;
                case "Duration": title = "по длительности"; break;
                default: title = "по названию"; break;
            }

            lblSort.Text = "Сортировка: " + title + (_sortAscending ? " (А→Я)" : " (Я→А)");
        }

        private Service SelectedService
        {
            get
            {
                if (dgvServices.CurrentRow == null) return null;
                return dgvServices.CurrentRow.DataBoundItem as Service;
            }
        }

        // ------------------------------ ПОИСК ------------------------------

        private void StartSearch()
        {
            _search = txtSearch.Text.Trim();
            _page = 1;
            LoadData();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            StartSearch();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            _search = string.Empty;
            _page = 1;
            _sortColumn = "Name";
            _sortAscending = true;
            LoadData();
        }

        // ------------------------------ CRUD ------------------------------

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!_canEdit) return;

            using (var form = new ServiceEditForm(null))
            {
                if (form.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    _repository.Add(form.Service);
                    _page = 1;
                    LoadData();
                }
                catch (AppException ex)
                {
                    MessageBox.Show(ex.Message, "Проверьте данные",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Ошибка добавления услуги");
                    MessageBox.Show("Не удалось добавить услугу.\n\n" + ex.Message,
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            EditSelected();
        }

        private void dgvServices_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && _canEdit) EditSelected();
        }

        private void EditSelected()
        {
            var service = SelectedService;
            if (service == null)
            {
                MessageBox.Show("Выберите услугу в таблице.", "Нет выбора",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var form = new ServiceEditForm(service))
            {
                if (form.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    _repository.Update(form.Service);
                    LoadData();
                }
                catch (AppException ex)
                {
                    MessageBox.Show(ex.Message, "Проверьте данные",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Ошибка изменения услуги Id={Id}", service.Id);
                    MessageBox.Show("Не удалось изменить услугу.\n\n" + ex.Message,
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!_canDelete) return;

            var service = SelectedService;
            if (service == null)
            {
                MessageBox.Show("Выберите услугу в таблице.", "Нет выбора",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var answer = MessageBox.Show(
                "Удалить услугу «" + service.Name + "»?\nДействие нельзя отменить.",
                "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (answer != DialogResult.Yes) return;

            try
            {
                _repository.Delete(service.Id);
                LoadData();
            }
            catch (ReferentialIntegrityException ex)
            {
                MessageBox.Show(ex.Message, "Удаление запрещено",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка удаления услуги Id={Id}", service.Id);
                MessageBox.Show("Не удалось удалить услугу.\n\n" + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        // --------------------------- СОРТИРОВКА И ПАГИНАЦИЯ ---------------------------

        private void dgvServices_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var column = dgvServices.Columns[e.ColumnIndex];
            var key = column.DataPropertyName;

            if (string.IsNullOrEmpty(key)) return;

            if (_sortColumn == key)
                _sortAscending = !_sortAscending;
            else
            {
                _sortColumn = key;
                _sortAscending = true;
            }

            _page = 1;
            LoadData();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (_page <= 1) return;
            _page--;
            LoadData();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            _page++;
            LoadData();
        }

        private void cmbPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_initializingPageSize) return;

            int size;
            if (int.TryParse(Convert.ToString(cmbPageSize.SelectedItem), out size) && size > 0)
            {
                _pageSize = size;
                _page = 1;
                LoadData();
            }
        }
    }
}
