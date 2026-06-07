using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace AccountingOfApplications
{
    public partial class Form5 : Form
    {
        private FirebaseService _firebase;
        public Form5(FirebaseService firebase, bool isDarkMode)
        {
            InitializeComponent();
            _firebase = firebase;
            ThemeManager.ApplyTheme(this);
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;

            try
            {
                var allRequests = await _firebase.GetAllRequestsAsync();

                if (allRequests == null || allRequests.Count == 0)
                {
                    MessageBox.Show("Нет данных для отображения графика");
                    return;
                }

                var filteredRequests = FilterByPeriod(allRequests);
                chart1.Series.Clear();

                if (comboBox1.SelectedItem.ToString() == "По типам работ")
                {
                    BuildChartByWorkType(filteredRequests);
                }
                else
                {
                    BuildChartByPriority(filteredRequests);
                }

                Logger.Write($"Построен график аналитики: {comboBox1.SelectedItem}, период: {comboBox2.SelectedItem}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка построения графика: {ex.Message}");
            }
            finally
            {
                button1.Enabled = true;
                ThemeManager.ApplyTheme(this);
            }
        }
        private System.Collections.Generic.Dictionary<string, Request> FilterByPeriod(System.Collections.Generic.Dictionary<string, Request> requests)
        {
            DateTime now = DateTime.Now;
            DateTime startDate;

            if (comboBox2.SelectedItem.ToString() == "Неделя")
                startDate = now.AddDays(-7);
            else
                startDate = now.AddMonths(-1);

            var filtered = new System.Collections.Generic.Dictionary<string, Request>();

            foreach (var req in requests)
            {
                if (DateTime.TryParse(req.Value.created_date, out DateTime createdDate))
                {
                    if (createdDate >= startDate)
                        filtered.Add(req.Key, req.Value);
                }
            }

            return filtered;
        }

        private void BuildChartByWorkType(System.Collections.Generic.Dictionary<string, Request> requests)
        {
            var series = new Series
            {
                Name = "Заявки",
                ChartType = SeriesChartType.Column,
                IsValueShownAsLabel = true
            };

            var counts = requests
                .GroupBy(r => r.Value.work_type)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .OrderBy(x => x.Type);

            foreach (var item in counts)
                series.Points.AddXY(item.Type, item.Count);

            chart1.Series.Add(series);
            chart1.Titles.Clear();
            chart1.Titles.Add("Распределение заявок по типам работ");
            chart1.ChartAreas[0].AxisX.Title = "Тип работ";
            chart1.ChartAreas[0].AxisY.Title = "Количество заявок";
        }

        private void BuildChartByPriority(System.Collections.Generic.Dictionary<string, Request> requests)
        {
            var series = new Series
            {
                Name = "Заявки",
                ChartType = SeriesChartType.Pie,
                IsValueShownAsLabel = true
            };

            var counts = requests
                .GroupBy(r => r.Value.priority)
                .Select(g => new { Priority = g.Key, Count = g.Count() });

            foreach (var item in counts)
                series.Points.AddXY(item.Priority, item.Count);

            chart1.Series.Add(series);
            chart1.Titles.Clear();
            chart1.Titles.Add("Распределение заявок по приоритетам");
        }
    }
}
