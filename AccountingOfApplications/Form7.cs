using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AccountingOfApplications
{
    public partial class Form7 : Form
    {
        private string _requestId;
        private Request _request;
        private FirebaseService _firebase;
        private bool _isEdit;
        public Form7(string requestId, Request request, FirebaseService firebase, bool isDarkMode)
        {
            InitializeComponent();
            _firebase = firebase;
            ThemeManager.ApplyTheme(this);
            if (requestId != null && request != null)
            {
                _isEdit = true;
                _requestId = requestId;
                _request = request;
                LoadRequestData();
            }
            else
            {
                _isEdit = false;
                _request = new Request();
            }
        }
        private void LoadRequestData()
        {
            textBox1.Text = _request.address;
            comboBox1.SelectedItem = _request.work_type;
            comboBox2.SelectedItem = _request.status;
            comboBox3.SelectedItem = _request.priority;
            textBox2.Text = _request.customer_phone;
            textBox3.Text = _request.operator_comment;
            checkBox1.Checked = _request.need_approval;
        }
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Введите адрес объекта");
                return false;
            }
            Regex phoneRegex = new Regex(@"^\+7[0-9]{10}$");
            if (!phoneRegex.IsMatch(textBox2.Text))
            {
                MessageBox.Show("Телефон должен быть в формате +7XXXXXXXXXX");
                return false;
            }
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Выберите тип работ");
                return false;
            }
            if (comboBox2.SelectedItem == null)
            {
                MessageBox.Show("Выберите статус");
                return false;
            }
            if (comboBox3.SelectedItem == null)
            {
                MessageBox.Show("Выберите приоритет");
                return false;
            }
            if (textBox3.Text.Length > 400)
            {
                MessageBox.Show("Комментарий не должен превышать 400 символов");
                return false;
            }
            return true;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            _request.address = textBox1.Text;
            _request.work_type = comboBox1.SelectedItem.ToString();
            _request.status = comboBox2.SelectedItem.ToString();
            _request.priority = comboBox3.SelectedItem.ToString();
            _request.customer_phone = textBox2.Text;
            _request.operator_comment = textBox3.Text;
            _request.need_approval = checkBox1.Checked;
            _request.created_date = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
            if (_isEdit)
            {
                await _firebase.UpdateRequestAsync(_requestId, _request);
            }
            else
            {
                var allRequests = await _firebase.GetAllRequestsAsync();
                int nextNum = (allRequests?.Count ?? 0) + 1;
                string newId = $"МТ-{nextNum:D3}";
                await _firebase.AddRequestAsync(newId, _request);
            }
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
