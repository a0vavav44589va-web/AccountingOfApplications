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
    public partial class Form6 : Form
    {
        private FirebaseService _firebase;
        private System.Collections.Generic.Dictionary<string, User> _users;
        public Form6(FirebaseService firebase, bool isDarkMode)
        {
            InitializeComponent();
            _firebase = firebase;
            ThemeManager.ApplyTheme(this);
            LoadUsers();
        }
        private async void LoadUsers()
        {
            try
            {
                _users = await _firebase.GetAllUsersAsync();
                RefreshDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки пользователей: {ex.Message}");
            }
        }

        private void RefreshDataGridView()
        {
            if (_users == null) return;

            var list = _users.Select(u => new
            {
                ID = u.Key,
                Логин = u.Value.login,
                Фамилия = u.Value.last_name,
                Имя = u.Value.first_name,
                Роль = u.Value.role,
                Заблокирован = u.Value.is_blocked ? "Да" : "Нет"
            }).ToList();

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = list;

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells["Заблокирован"].Value.ToString() == "Да")
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightPink;
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form8 editForm = new Form8(null, null, _firebase);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadUsers();
                Logger.Write("Администратор добавил нового пользователя");
            }
            ThemeManager.ApplyTheme(this);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите пользователя для редактирования");
                return;
            }

            string userId = dataGridView1.SelectedRows[0].Cells["ID"].Value.ToString();
            var user = _users[userId];

            Form8 editForm = new Form8(userId, user, _firebase);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadUsers();
                Logger.Write($"Администратор отредактировал пользователя {user.login}");
            }
            ThemeManager.ApplyTheme(this);
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите пользователя для удаления");
                return;
            }

            string userId = dataGridView1.SelectedRows[0].Cells["ID"].Value.ToString();
            var user = _users[userId];

            if (user.role == "admin")
            {
                MessageBox.Show("Нельзя удалить администратора");
                return;
            }

            if (MessageBox.Show($"Удалить пользователя {user.login}?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                await _firebase.DeleteUserAsync(userId);
                LoadUsers();
                Logger.Write($"Администратор удалил пользователя {user.login}");
            }
            ThemeManager.ApplyTheme(this);
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите пользователя");
                return;
            }

            string userId = dataGridView1.SelectedRows[0].Cells["ID"].Value.ToString();
            var user = _users[userId];

            if (user.role == "admin")
            {
                MessageBox.Show("Нельзя заблокировать администратора");
                return;
            }

            user.is_blocked = !user.is_blocked;
            await _firebase.UpdateUserAsync(userId, user);
            LoadUsers();

            string action = user.is_blocked ? "заблокировал" : "разблокировал";
            Logger.Write($"Администратор {action} пользователя {user.login}");
            ThemeManager.ApplyTheme(this);
        }
    }
}
