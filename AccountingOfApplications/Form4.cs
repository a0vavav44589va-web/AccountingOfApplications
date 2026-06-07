using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AccountingOfApplications
{
    public partial class Form4 : Form
    {
        private FirebaseService _firebase;
        public Form4(FirebaseService firebase, bool isDarkMode)
        {
            InitializeComponent();
            _firebase = firebase;
            ThemeManager.ApplyTheme(this);;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string keyword = textBox1.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(keyword))
            {
                MessageBox.Show("Введите поисковый запрос");
                return;
            }

            button1.Enabled = false;

            try
            {
                var allRequests = await _firebase.GetAllRequestsAsync();

                if (allRequests == null || allRequests.Count == 0)
                {
                    MessageBox.Show("Нет данных для поиска");
                    return;
                }

                var results = allRequests
                    .Where(r => r.Value.address.ToLower().Contains(keyword) ||
                                (r.Value.operator_comment != null && r.Value.operator_comment.ToLower().Contains(keyword)))
                    .Select(r => new
                    {
                        ID = r.Key,
                        Адрес = r.Value.address,
                        Тип = r.Value.work_type,
                        Статус = r.Value.status,
                        Приоритет = r.Value.priority,
                        Телефон = r.Value.customer_phone,
                        Комментарий = r.Value.operator_comment
                    })
                    .ToList();

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = results;

                if (results.Count == 0)
                {
                    MessageBox.Show("Ничего не найдено");
                }

                Logger.Write($"Выполнен поиск по запросу: {keyword}, найдено {results.Count} записей");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска: {ex.Message}");
            }
            finally
            {
                button1.Enabled = true;
                ThemeManager.ApplyTheme(this);
            }
        }
    }
}
