using System;
using System.Windows.Forms;
using MedClinic.Models;
using MedClinic.Services;
using MedClinic.UI;

namespace MedClinic.Forms
{
    /// <summary>
    /// Диалог добавления или редактирования услуги.
    /// null в конструкторе — создание новой услуги.
    /// </summary>
    public partial class ServiceEditForm : Form
    {
        private readonly bool _isNew;

        /// <summary>Данные услуги, заполненные пользователем.</summary>
        public Service Service { get; private set; }

        public ServiceEditForm(Service service)
        {
            InitializeComponent();

            _isNew = (service == null);
            Service = service ?? new Service();

            Text = _isNew ? "Новая услуга" : "Редактирование услуги";
            lblHeader.Text = Text;

            PlaceholderHelper.SetPlaceholder(txtName, "Например: Приём кардиолога");

            if (!_isNew)
                FillFields();
        }

        private void FillFields()
        {
            txtName.Text = Service.Name;

            // Значение NumericUpDown не может выходить за Min/Max — ограничиваем
            var price = Service.Price;
            if (price < nudPrice.Minimum) price = nudPrice.Minimum;
            if (price > nudPrice.Maximum) price = nudPrice.Maximum;
            nudPrice.Value = price;

            var duration = Service.Duration;
            if (duration < (int)nudDuration.Minimum) duration = (int)nudDuration.Minimum;
            if (duration > (int)nudDuration.Maximum) duration = (int)nudDuration.Maximum;
            nudDuration.Value = duration;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var candidate = new Service
            {
                Id = Service.Id,
                Name = txtName.Text.Trim(),
                Price = nudPrice.Value,
                Duration = (int)nudDuration.Value
            };

            var error = Validation.CheckService(candidate);
            if (error != null)
            {
                MessageBox.Show(error, "Проверьте данные", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (!Validation.IsFilled(candidate.Name)) txtName.Focus();
                return;
            }

            Service = candidate;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
