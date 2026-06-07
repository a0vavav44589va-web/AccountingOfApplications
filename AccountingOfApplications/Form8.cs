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
    public partial class Form8 : Form
    {
        private string _userId;
        private User _user;
        private FirebaseService _firebase;
        private bool _isEdit;
        public Form8(string userId, User user, FirebaseService firebase)
        {
            InitializeComponent();
            _firebase = firebase;
            ThemeManager.ApplyTheme(this);
            if (userId != null && user != null)
            {
                _isEdit = true;
                _userId = userId;
                _user = user;
                LoadUserData();
            }
            else
            {
                _isEdit = false;
                _user = new User();
            }
        }
        private void LoadUserData()
        {
            textBox1.Text = _user.login;
            textBox5.Text = _user.last_name;
            textBox6.Text = _user.first_name;
            textBox2.Text = _user.middle_name;
            comboBox3.SelectedItem = _user.role;
            textBox4.Text = _user.password;
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Введите логин");
                return false;
            }

            if (!_isEdit && string.IsNullOrWhiteSpace(textBox4.Text))
            {
                MessageBox.Show("Введите пароль");
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBox5.Text))
            {
                MessageBox.Show("Введите фамилию");
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBox6.Text))
            {
                MessageBox.Show("Введите имя");
                return false;
            }

            if (comboBox3.SelectedItem == null)
            {
                MessageBox.Show("Выберите роль");
                return false;
            }

            return true;
        }
        private void Form8_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            _user.login = textBox1.Text;
            _user.last_name = textBox5.Text;
            _user.first_name = textBox6.Text;
            _user.middle_name = textBox2.Text;
            _user.role = comboBox3.SelectedItem.ToString();
            _user.password = textBox4.Text;

            if (!_isEdit)
            {
                _user.is_blocked = false;

                var allUsers = await _firebase.GetAllUsersAsync();

                // Находим максимальный номер среди user1, user2, user3...
                int maxNum = 0;
                foreach (var u in allUsers)
                {
                    if (u.Key.StartsWith("user"))
                    {
                        if (int.TryParse(u.Key.Substring(4), out int num))
                        {
                            if (num > maxNum) maxNum = num;
                        }
                    }
                }

                string newUserId = $"user{maxNum + 1}";
                await _firebase.AddUserAsync(newUserId, _user);
            }
            else
            {
                await _firebase.UpdateUserAsync(_userId, _user);
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
