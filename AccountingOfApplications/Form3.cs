using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;

namespace AccountingOfApplications
{
    public partial class Form3 : Form
    {
        private User _currentUser;
        private FirebaseService _firebase;
        private Dictionary<string, Request> _requests;
        public Form3(User user, FirebaseService firebase, bool isDarkMode)
        {
            InitializeComponent();
            _currentUser = user;
            _firebase = firebase;
            LoadRequests();
            ThemeManager.ApplyTheme(this);
            if (isDarkMode)
                ThemeManager.SetDarkTheme(this);
            else
                ThemeManager.SetLightTheme(this);
        }

        private async void LoadRequests()
        {
            try
            {
                _requests = await _firebase.GetAllRequestsAsync();
                if (_requests != null)
                {
                    RefreshDataGridView();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        private void RefreshDataGridView()
        {
            var list = _requests.Select(r => new
            {
                ID = r.Key,
                Адрес = r.Value.address,
                Тип = r.Value.work_type,
                Статус = r.Value.status,
                Приоритет = r.Value.priority,
                Телефон = r.Value.customer_phone,
                Комментарий = r.Value.operator_comment,
                Согласование = r.Value.need_approval ? "Да" : "Нет"
            }).ToList();
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = list;
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form7 editForm = new Form7(null, null, _firebase, ThemeManager.IsDarkMode);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadRequests();
                Logger.Write($"Пользователь {_currentUser.login} добавил заявку");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите заявку для редактирования");
                return;
            }

            string requestId = dataGridView1.SelectedRows[0].Cells["ID"].Value.ToString();
            var request = _requests[requestId];

            Form7 editForm = new Form7(requestId, request, _firebase, ThemeManager.IsDarkMode);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadRequests();
                Logger.Write($"Пользователь {_currentUser.login} отредактировал заявку {requestId}");
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите заявку для удаления");
                return;
            }

            string requestId = dataGridView1.SelectedRows[0].Cells["ID"].Value.ToString();
            if (MessageBox.Show($"Удалить заявку {requestId}?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                await _firebase.DeleteRequestAsync(requestId);
                LoadRequests();
                Logger.Write($"Пользователь {_currentUser.login} удалил заявку {requestId}");
            }
        }
        private void SortDataGridView()
        {
            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите поле для сортировки");
                return;
            }

            if (_requests == null || _requests.Count == 0)
            {
                MessageBox.Show("Нет данных для сортировки");
                return;
            }

            string sortField = "";
            switch (listBox1.SelectedIndex)
            {
                case 0: sortField = "ID"; break;
                case 1: sortField = "Статус"; break;
                case 2: sortField = "Приоритет"; break;
                default: return;
            }

            bool isAscending = radioButton1.Checked; // radioButton1 - по возрастанию? Уточните

            // Сортируем _requests и создаём новый список
            List<KeyValuePair<string, Request>> sortedList;

            if (sortField == "ID")
            {
                sortedList = isAscending
                    ? _requests.OrderBy(x => x.Key).ToList()
                    : _requests.OrderByDescending(x => x.Key).ToList();
            }
            else if (sortField == "Статус")
            {
                sortedList = isAscending
                    ? _requests.OrderBy(x => x.Value.status).ToList()
                    : _requests.OrderByDescending(x => x.Value.status).ToList();
            }
            else // Приоритет
            {
                sortedList = isAscending
                    ? _requests.OrderBy(x => x.Value.priority).ToList()
                    : _requests.OrderByDescending(x => x.Value.priority).ToList();
            }

            // Формируем список для отображения
            var displayList = sortedList.Select(r => new
            {
                ID = r.Key,
                Адрес = r.Value.address,
                Тип = r.Value.work_type,
                Статус = r.Value.status,
                Приоритет = r.Value.priority,
                Телефон = r.Value.customer_phone,
                Комментарий = r.Value.operator_comment,
                Согласование = r.Value.need_approval ? "Да" : "Нет"
            }).ToList();

            // Обновляем DataGridView
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = displayList;
        }

        private object GetPropertyValue(dynamic obj, string propertyName)
        {
            var type = obj.GetType();
            var property = type.GetProperty(propertyName);
            return property?.GetValue(obj, null);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SortDataGridView();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("Нет данных для экспорта");
                    return;
                }
                DataTable dt = new DataTable();
                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    dt.Columns.Add(col.HeaderText);
                }
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < dataGridView1.Columns.Count; i++)
                        {
                            dr[i] = row.Cells[i].Value?.ToString() ?? "";
                        }
                        dt.Rows.Add(dr);
                    }
                }

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Нет данных для экспорта");
                    return;
                }

                // Создаём Excel приложение
                Excel.Application excelApp = new Excel.Application();
                excelApp.Visible = true;
                Excel.Workbook workbook = excelApp.Workbooks.Add();
                Excel.Worksheet worksheet = workbook.ActiveSheet;

                // Заголовки
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    worksheet.Cells[1, j + 1] = dt.Columns[j].ColumnName;
                    // Делаем заголовки жирными
                    ((Excel.Range)worksheet.Cells[1, j + 1]).Font.Bold = true;
                }

                // Данные
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        worksheet.Cells[i + 2, j + 1] = dt.Rows[i][j].ToString();
                    }
                }

                worksheet.Columns.AutoFit();

                worksheet.Cells[1, 1].Select();

                Logger.Write($"Пользователь {_currentUser.login} экспортировал заявки в Excel");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта в Excel: {ex.Message}");
                Logger.WriteError($"Ошибка экспорта в Excel: {ex.Message}");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                // Получаем данные из DataGridView
                if (dataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("Нет данных для экспорта");
                    return;
                }

                // Создаём DataTable для экспорта
                DataTable dt = new DataTable();

                // Добавляем колонки
                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    dt.Columns.Add(col.HeaderText);
                }

                // Добавляем строки
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < dataGridView1.Columns.Count; i++)
                        {
                            dr[i] = row.Cells[i].Value?.ToString() ?? "";
                        }
                        dt.Rows.Add(dr);
                    }
                }

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Нет данных для экспорта");
                    return;
                }

                // Создаём Word приложение
                Word.Application wordApp = new Word.Application();
                wordApp.Visible = true;
                Word.Document doc = wordApp.Documents.Add();

                // Добавляем заголовок документа
                Word.Paragraph titlePara = doc.Paragraphs.Add();
                titlePara.Range.Text = "Реестр заявок на монтаж и техническое обслуживание";
                titlePara.Range.Font.Bold = 1;
                titlePara.Range.Font.Size = 16;
                titlePara.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                titlePara.Range.InsertParagraphAfter();

                // Добавляем дату
                Word.Paragraph datePara = doc.Paragraphs.Add();
                datePara.Range.Text = $"Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm}";
                datePara.Range.Font.Size = 10;
                datePara.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
                datePara.Range.InsertParagraphAfter();

                // Добавляем пустую строку
                doc.Paragraphs.Add().Range.InsertParagraphAfter();

                // Создаём таблицу в Word
                Word.Table table = doc.Tables.Add(
                    doc.Paragraphs.Add().Range,
                    dt.Rows.Count + 1,
                    dt.Columns.Count);

                table.Borders.Enable = 1;
                table.Range.Font.Size = 11;

                // Заголовки таблицы
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    table.Cell(1, j + 1).Range.Text = dt.Columns[j].ColumnName;
                    table.Cell(1, j + 1).Range.Font.Bold = 1;
                    table.Cell(1, j + 1).Shading.BackgroundPatternColor = Word.WdColor.wdColorGray15;
                }

                // Данные таблицы
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        table.Cell(i + 2, j + 1).Range.Text = dt.Rows[i][j].ToString();
                    }
                }

                // Автоподбор ширины колонок
                table.AutoFitBehavior(Word.WdAutoFitBehavior.wdAutoFitContent);

                Logger.Write($"Пользователь {_currentUser.login} экспортировал заявки в Word");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта в Word");
                Logger.WriteError($"Ошибка экспорта в Word: {ex.Message}");
            }
        }
    }
}
