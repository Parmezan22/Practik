using System;
using System.Windows.Forms;
using MedClinic.Models;
using MedClinic.Services;
using MedClinic.UI;

namespace MedClinic.Forms
{
    /// <summary>
    /// Диалог добавления или редактирования врача.
    /// Если передать null — создаётся новый врач.
    /// После нажатия "Сохранить" результат доступен в свойстве Doctor.
    /// </summary>
    public partial class DoctorEditForm : Form
    {
        private readonly bool _isNew;

        /// <summary>Данные врача, заполненные пользователем.</summary>
        public Doctor Doctor { get; private set; }

        public DoctorEditForm(Doctor doctor)
        {
            InitializeComponent();

            _isNew = (doctor == null);
            Doctor = doctor ?? new Doctor();

            Text = _isNew ? "Новый врач" : "Редактирование врача";
            lblHeader.Text = _isNew ? "Новый врач" : "Редактирование врача";

            // Подсказки внутри пустых полей
            PlaceholderHelper.SetPlaceholder(txtFullName, "Например: Иванова Мария Петровна");
            PlaceholderHelper.SetPlaceholder(txtSpecialty, "Например: Терапевт");
            PlaceholderHelper.SetPlaceholder(txtPhone, "Например: +7 (900) 111-22-33");
            PlaceholderHelper.SetPlaceholder(txtCabinet, "Например: 101");

            // Автодополнение специальности — ускоряет ввод и уменьшает опечатки
            txtSpecialty.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtSpecialty.AutoCompleteSource = AutoCompleteSource.CustomSource;
            txtSpecialty.AutoCompleteCustomSource.AddRange(new string[]
            {
                "Терапевт", "Кардиолог", "Невролог", "Хирург", "Педиатр",
                "Офтальмолог", "Лор", "Дерматолог", "Стоматолог", "Гинеколог"
            });

            if (!_isNew)
                FillFields();
        }

        private void FillFields()
        {
            txtFullName.Text = Doctor.FullName;
            txtSpecialty.Text = Doctor.Specialty;
            txtPhone.Text = Doctor.Phone;
            txtCabinet.Text = Doctor.Cabinet;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Собираем данные в новый объект: исходный меняем только после успешной проверки
            var candidate = new Doctor
            {
                Id = Doctor.Id,
                FullName = txtFullName.Text.Trim(),
                Specialty = txtSpecialty.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                Cabinet = txtCabinet.Text.Trim()
            };

            var error = Validation.CheckDoctor(candidate);
            if (error != null)
            {
                MessageBox.Show(error, "Проверьте данные", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FocusField(candidate);
                return;
            }

            Doctor = candidate;
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>Ставит курсор в поле, из-за которого не прошла проверка.</summary>
        private void FocusField(Doctor doctor)
        {
            if (!Validation.IsValidFullName(doctor.FullName))
                txtFullName.Focus();
            else if (!Validation.IsFilled(doctor.Specialty))
                txtSpecialty.Focus();
            else if (Validation.IsFilled(doctor.Phone) && !Validation.IsValidPhone(doctor.Phone))
                txtPhone.Focus();
            else
                txtCabinet.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
