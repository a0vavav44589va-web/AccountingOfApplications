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
    public partial class Form2 : Form
    {
        private User _currentUser;
        private FirebaseService _firebase;
        public Form2(User user, FirebaseService firebase)
        {
            InitializeComponent();
            themeToggle.CheckedChanged += themeToggle_CheckedChanged;
            ThemeManager.SetLightTheme(this);
            _currentUser = user;
            _firebase = firebase;
            if (user.role != "admin")
            {
                button5.Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form1 loginForm = new Form1();
            loginForm.Show();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form3 requestsForm = new Form3(_currentUser, _firebase, ThemeManager.IsDarkMode);
            requestsForm.ShowDialog();
        }

        private void themeToggle_CheckedChanged(object sender, EventArgs e)
        {
            ThemeManager.IsDarkMode = themeToggle.Checked;

            if (themeToggle.Checked)
                ThemeManager.SetDarkTheme(this);
            else
                ThemeManager.SetLightTheme(this);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Form4 requestsForm = new Form4(_firebase, ThemeManager.IsDarkMode);
            requestsForm.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form5 requestsForm = new Form5(_firebase, ThemeManager.IsDarkMode);
            requestsForm.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form6 requestsForm = new Form6(_firebase, ThemeManager.IsDarkMode);
            requestsForm.ShowDialog();
        }
    }
}
