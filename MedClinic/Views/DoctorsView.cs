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
    /// Справочник врачей: список с поиском, сортировкой по клику на заголовок,
    /// постраничным выводом, добавлением, изменением и удалением.
    /// Кнопки включаются по роли пользователя.
    /// </summary>
    public partial class DoctorsView : UserControl
    {
        private readonly DoctorRepository _repository = new DoctorRepository();

        private string _search = string.Empty;
        private int _page = 1;
        private int _pageSize = 10;
        private int _totalCount;
        private string _sortColumn = "FullName";
        private bool _sortAscending = true;
        private bool _canEdit;
        private bool _canDelete;
        private bool _initializingPageSize;

        public DoctorsView()
        {
            InitializeComponent();

            PlaceholderHelper.SetPlaceholder(txtSearch, "ФИО, специальность, телефон или кабинет");

            // Заполняем список "строк на странице" без вызова обработчика
            _initializingPageSize = true;
            cmbPageSize.Items.Add("10");
            cmbPageSize.Items.Add("20");
            cmbPageSize.Items.Add("50");
            cmbPageSize.SelectedIndex = 0;
            _initializingPageSize = false;

            // Поиск по нажатию Enter
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

        /// <summary>Применяет права текущей роли: кто может менять и удалять.</summary>
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
                var doctors = _repository.GetPaged(_search, _sortColumn, _sortAscending,
                                                   _page, _pageSize, out total);

                // Если после удаления записей страница стала лишней — возвращаемся на последнюю
                var pageCount = Math.Max(1, (int)Math.Ceiling(total / (double)_pageSize));
                if (_page > pageCount)
                {
                    _page = pageCount;
                    doctors = _repository.GetPaged(_search, _sortColumn, _sortAscending,
                                                   _page, _pageSize, out total);
                }

                _totalCount = total;

                dgvDoctors.DataSource = null;
                dgvDoctors.DataSource = doctors;

                lblFound.Text = "Найдено: " + total;
                lblTotal.Text = "Всего врачей: " + total;
                lblPage.Text = "Страница " + _page + " из " + pageCount;
                btnPrev.Enabled = _page > 1;
                btnNext.Enabled = _page < pageCount;
                UpdateSortLabel();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка загрузки справочника врачей");
                MessageBox.Show("Не удалось загрузить список врачей.\n\n" + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateSortLabel()
        {
            string title;
            switch (_sortColumn)
            {
                case "Specialty": title = "по специальности"; break;
                case "Phone": title = "по телефону"; break;
                case "Cabinet": title = "по кабинету"; break;
                default: title = "по ФИО"; break;
            }

            lblSort.Text = "Сортировка: " + title + (_sortAscending ? " (А→Я)" : " (Я→А)");
        }

        /// <summary>Выбранный в таблице врач (null, если ничего не выбрано).</summary>
        private Doctor SelectedDoctor
        {
            get
            {
                if (dgvDoctors.CurrentRow == null) return null;
                return dgvDoctors.CurrentRow.DataBoundItem as Doctor;
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
            _sortColumn = "FullName";
            _sortAscending = true;
            LoadData();
        }

        // ------------------------------ CRUD ------------------------------

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!_canEdit) return;

            using (var form = new DoctorEditForm(null))
            {
                if (form.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    _repository.Add(form.Doctor);
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
                    Log.Error(ex, "Ошибка добавления врача");
                    MessageBox.Show("Не удалось добавить врача.\n\n" + ex.Message,
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            EditSelected();
        }

        private void dgvDoctors_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && _canEdit) EditSelected();
        }

        private void EditSelected()
        {
            var doctor = SelectedDoctor;
            if (doctor == null)
            {
                MessageBox.Show("Выберите врача в таблице.", "Нет выбора",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var form = new DoctorEditForm(doctor))
            {
                if (form.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    _repository.Update(form.Doctor);
                    LoadData();
                }
                catch (AppException ex)
                {
                    MessageBox.Show(ex.Message, "Проверьте данные",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Ошибка изменения врача Id={Id}", doctor.Id);
                    MessageBox.Show("Не удалось изменить врача.\n\n" + ex.Message,
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!_canDelete) return;

            var doctor = SelectedDoctor;
            if (doctor == null)
            {
                MessageBox.Show("Выберите врача в таблице.", "Нет выбора",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var answer = MessageBox.Show(
                "Удалить врача «" + doctor.FullName + "»?\nДействие нельзя отменить.",
                "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (answer != DialogResult.Yes) return;

            try
            {
                _repository.Delete(doctor.Id);
                LoadData();
            }
            catch (ReferentialIntegrityException ex)
            {
                // Ссылочная целостность: у врача есть записи на приём
                MessageBox.Show(ex.Message, "Удаление запрещено",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка удаления врача Id={Id}", doctor.Id);
                MessageBox.Show("Не удалось удалить врача.\n\n" + ex.Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        // --------------------------- СОРТИРОВКА И ПАГИНАЦИЯ ---------------------------

        private void dgvDoctors_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var column = dgvDoctors.Columns[e.ColumnIndex];
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
