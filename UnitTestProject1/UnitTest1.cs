using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Windows.Forms;
using AccountingOfApplications;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Test_InvalidLogin_ShowsErrorMessage()
        {
            Form1 loginForm = new Form1();
            loginForm.Show();
            Thread.Sleep(500);

            TextBox txtLogin = (TextBox)FindControl(loginForm, "textBox1");
            TextBox txtPassword = (TextBox)FindControl(loginForm, "textBox2");
            Button btnLogin = (Button)FindControl(loginForm, "button1");

            txtLogin.Text = "fake";
            txtPassword.Text = "123";
            btnLogin.PerformClick();
            Thread.Sleep(500);

            bool isForm2Open = false;
            foreach (Form form in Application.OpenForms)
            {
                if (form is Form2)
                {
                    isForm2Open = true;
                    break;
                }
            }

            Assert.IsFalse(isForm2Open, "Авторизация прошла с несуществующим логином!");
        }

        private Control FindControl(Control parent, string name)
        {
            foreach (Control child in parent.Controls)
            {
                if (child.Name == name) return child;
                Control found = FindControl(child, name);
                if (found != null) return found;
            }
            return null;
        }
    }
}
