using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using MedClinic.Models;
using MedClinic.Services;
using Serilog;

namespace MedClinic.Forms
{
    /// <summary>
    /// Окно «Курсы валют ЦБ РФ».
    /// Показывает официальные курсы, полученные через интернет,
    /// и позволяет обновить их кнопкой «Обновить».
    /// </summary>
    public partial class CurrencyRatesForm : Form
    {
        private bool _loading;

        public CurrencyRatesForm()
        {
            InitializeComponent();
        }

        private async void CurrencyRatesForm_Shown(object sender, EventArgs e)
        {
            await LoadRatesAsync();
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadRatesAsync();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>Загружает курсы и заполняет таблицу.</summary>
        private async Task LoadRatesAsync()
        {
            if (_loading) return;

            _loading = true;
            btnRefresh.Enabled = false;
            Cursor = Cursors.WaitCursor;
            lblStatus.Text = "Загрузка курсов с сервера ЦБ РФ…";

            try
            {
                var result = await CurrencyService.LoadAsync();

                dgvRates.DataSource = null;
                dgvRates.DataSource = result.Rates;

                lblDate.Text = "Дата курса: " + result.Date.ToString("dd.MM.yyyy HH:mm");
                lblStatus.Text = "Валют загружено: " + result.Rates.Count +
                                 ". Источник: cbr-xml-daily.ru (данные ЦБ РФ).";

                Log.Information("Курсы валют загружены: {Count} валют на {Date:dd.MM.yyyy}",
                    result.Rates.Count, result.Date);
            }
            catch (AppException ex)
            {
                ShowLoadError(ex.Message);
                Log.Warning(ex, "Не удалось загрузить курсы валют");
            }
            catch (Exception ex)
            {
                ShowLoadError("Не удалось загрузить курсы валют.\n\n" + ex.Message);
                Log.Error(ex, "Неожиданная ошибка загрузки курсов валют");
            }
            finally
            {
                _loading = false;
                btnRefresh.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        /// <summary>Показывает сообщение об ошибке и подсказку под таблицей.</summary>
        private void ShowLoadError(string message)
        {
            lblStatus.Text = "Курсы не загружены. Проверьте интернет и нажмите «Обновить».";
            lblDate.Text = "Дата курса: нет данных";

            MessageBox.Show(message, "Курсы валют",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>Подсвечивает рост курса зелёным, падение — красным.</summary>
        private void dgvRates_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvRates.Rows)
            {
                var rate = row.DataBoundItem as CurrencyRate;
                if (rate == null) continue;

                if (rate.Change > 0)
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(0, 110, 70);
                else if (rate.Change < 0)
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(190, 50, 50);
                else
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(33, 37, 41);
            }
        }
    }
}
