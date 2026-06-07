using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AccountingOfApplications
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            _firebase = new FirebaseService();
            textBox2.PasswordChar = '*';
            label4.Visible = false;
        }
        private FirebaseService _firebase;

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string login = textBox1.Text.Trim();
            string password = textBox2.Text;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                label4.Text = "Введите логин и пароль";
                label4.Visible = true;
                return;
            }
            button1.Enabled = false;
            try
            {
                var user = await _firebase.AuthenticateAsync(login, password);
                if (user != null && !user.is_blocked)
                {
                    Logger.Write($"Пользователь {login} успешно авторизовался");
                    Form2 mainForm = new Form2(user, _firebase);
                    mainForm.Show();
                    this.Hide();
                }
                else if (user != null && user.is_blocked)
                {
                    Logger.WriteWarning($"Попытка входа заблокированным пользователем {login}");
                    label4.Text = "Учётная запись заблокирована";
                    label4.Visible = true;
                }
                else
                {
                    Logger.WriteWarning($"Неудачная попытка входа: {login}");
                    label4.Text = "Неверный логин или пароль";
                    label4.Visible = true;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteError($"Ошибка подключения: {ex.Message}");
                MessageBox.Show($"Ошибка подключения", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                button1.Enabled = true;
            }
        }
    }
}
