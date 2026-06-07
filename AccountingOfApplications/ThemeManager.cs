using System;
using System.Drawing;
using System.Windows.Forms;

namespace AccountingOfApplications
{
    public static class ThemeManager
    {
        // Статическая переменная для хранения состояния темы
        public static bool IsDarkMode { get; set; } = false;

        // Переключение темы
        public static void ToggleTheme(Form form)
        {
            IsDarkMode = !IsDarkMode;
            ApplyTheme(form);
        }

        // Применение темы к форме
        public static void ApplyTheme(Form form)
        {
            if (IsDarkMode)
                SetDarkTheme(form);
            else
                SetLightTheme(form);
        }

        // Установка тёмной темы
        public static void SetDarkTheme(Form form)
        {
            form.BackColor = Color.FromArgb(45, 45, 48);
            ApplyDarkThemeToControl(form);
        }

        private static void ApplyDarkThemeToControl(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is Button btn)
                {
                    btn.BackColor = Color.FromArgb(63, 63, 70);
                    btn.ForeColor = Color.White;
                }
                else if (ctrl is Label lbl)
                {
                    lbl.ForeColor = Color.White;
                }
                else if (ctrl is CheckBox chk)
                {
                    chk.ForeColor = Color.White;
                }
                else if (ctrl is RadioButton rb)
                {
                    rb.ForeColor = Color.White;
                }
                else if (ctrl is TextBox txt)
                {
                    txt.BackColor = Color.FromArgb(63, 63, 70);
                    txt.ForeColor = Color.White;
                }
                else if (ctrl is ComboBox cb)  // 👈 ДОБАВЛЕНО
                {
                    cb.BackColor = Color.FromArgb(63, 63, 70);
                    cb.ForeColor = Color.White;
                    cb.FlatStyle = FlatStyle.Flat;
                }
                else if (ctrl is ListBox lb)  // 👈 ДОБАВЛЕНО
                {
                    lb.BackColor = Color.FromArgb(63, 63, 70);
                    lb.ForeColor = Color.White;
                }
                else if (ctrl is DataGridView dgv)
                {
                    dgv.BackgroundColor = Color.FromArgb(45, 45, 48);
                    dgv.GridColor = Color.FromArgb(100, 100, 100);
                    dgv.DefaultCellStyle.BackColor = Color.FromArgb(63, 63, 70);
                    dgv.DefaultCellStyle.ForeColor = Color.White;
                    dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                }
                else if (ctrl is GroupBox gb)  // 👈 ДОБАВЛЕНО
                {
                    gb.ForeColor = Color.White;
                }
                else if (ctrl is Panel pnl)  // 👈 ДОБАВЛЕНО
                {
                    pnl.BackColor = Color.FromArgb(45, 45, 48);
                }

                // Рекурсивный обход дочерних элементов
                if (ctrl.HasChildren)
                {
                    ApplyDarkThemeToControl(ctrl);
                }
            }
        }

        // Установка светлой темы
        public static void SetLightTheme(Form form)
        {
            form.BackColor = Color.White;
            ApplyLightThemeToControl(form);
        }

        private static void ApplyLightThemeToControl(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is Button btn)
                {
                    btn.BackColor = SystemColors.Control;
                    btn.ForeColor = Color.Black;
                }
                else if (ctrl is Label lbl)
                {
                    lbl.ForeColor = Color.Black;
                }
                else if (ctrl is CheckBox chk)
                {
                    chk.ForeColor = Color.Black;
                }
                else if (ctrl is RadioButton rb)
                {
                    rb.ForeColor = Color.Black;
                }
                else if (ctrl is TextBox txt)
                {
                    txt.BackColor = Color.White;
                    txt.ForeColor = Color.Black;
                }
                else if (ctrl is ComboBox cb)  // 👈 ДОБАВЛЕНО
                {
                    cb.BackColor = Color.White;
                    cb.ForeColor = Color.Black;
                    cb.FlatStyle = FlatStyle.Standard;
                }
                else if (ctrl is ListBox lb)  // 👈 ДОБАВЛЕНО
                {
                    lb.BackColor = Color.White;
                    lb.ForeColor = Color.Black;
                }
                else if (ctrl is DataGridView dgv)
                {
                    dgv.BackgroundColor = Color.White;
                    dgv.GridColor = Color.FromArgb(200, 200, 200);
                    dgv.DefaultCellStyle.BackColor = Color.White;
                    dgv.DefaultCellStyle.ForeColor = Color.Black;
                    dgv.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
                }
                else if (ctrl is GroupBox gb)  // 👈 ДОБАВЛЕНО
                {
                    gb.ForeColor = Color.Black;
                }
                else if (ctrl is Panel pnl)  // 👈 ДОБАВЛЕНО
                {
                    pnl.BackColor = Color.White;
                }

                // Рекурсивный обход дочерних элементов
                if (ctrl.HasChildren)
                {
                    ApplyLightThemeToControl(ctrl);
                }
            }
        }
    }
}