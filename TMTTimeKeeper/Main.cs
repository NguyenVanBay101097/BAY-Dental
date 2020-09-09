using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using TMTTimeKeeper.Models;

namespace TMTTimeKeeper
{
    public partial class Main : Form
    {
        private Button stateNav = null;
        private AccountLogin account = null;
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            this.Shown += Form1_Shown;
            //
            stateNav = button1;
            Page1 page1 = new Page1();
            nav(page1, content);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            account = getAccount();
            if (account == null)
            {
                Visible = false;
                Login loginForm = new Login();
                var result = loginForm.ShowDialog();

                if (result == DialogResult.OK)
                {
                    account = getAccount();
                    lblAccountName.Text = account.Name;
                    Visible = true;
                }
                else if (result == DialogResult.Cancel)
                {
                    Close();
                }
            }
            else
            {
                lblAccountName.Text = account.Name;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Page1 page1 = new Page1();
            nav(page1, content);
            actNav(button1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Page2 page2 = new Page2();
            nav(page2, content);
            actNav(button2);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Page3 page3 = new Page3();
            nav(page3, content);
            actNav(button3);
        }

        public void nav(Form form, Panel panel)
        {
            form.TopLevel = false;
            form.Size = panel.Size;
            content.Controls.Clear();
            content.Controls.Add(form);
            form.Show();
        }

        public void actNav(Button button)
        {
            stateNav.BackColor = SystemColors.MenuHighlight;
            stateNav.ForeColor = SystemColors.GradientActiveCaption;
            button.BackColor = SystemColors.ControlLightLight;
            button.ForeColor = SystemColors.ControlText;
            stateNav = button;
        }

        public AccountLogin getAccount()
        {
            var account = new AccountLogin();
            string fileName = "AccountLogin.json";
            string path = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\x86\Debug\netcoreapp3.1", string.Empty), @"Data\", fileName);
            using (StreamReader sr = File.OpenText(path))
            {
                account = JsonConvert.DeserializeObject<AccountLogin>(sr.ReadToEnd());
            }
            return account;
        }

        public TimeKeeper getTimeKepper()
        {
            var timeKeeper = new TimeKeeper();
            string fileName = "TimeKeeper.json";
            string path = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\x86\Debug\netcoreapp3.1", string.Empty), @"Data\", fileName);
            using (StreamReader sr = File.OpenText(path))
            {
                timeKeeper = JsonConvert.DeserializeObject<TimeKeeper>(sr.ReadToEnd());
            }
            return timeKeeper;
        }

        public List<Employee> getEmployee()
        {
            var employees = new List<Employee>();
            string fileName = "Employees.json";
            string path = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\x86\Debug\netcoreapp3.1", string.Empty), @"Data\", fileName);
            using (StreamReader sr = File.OpenText(path))
            {
                employees = JsonConvert.DeserializeObject<List<Employee>>(sr.ReadToEnd());
            }
            return employees;
        }

        private void lblLogout_Click(object sender, EventArgs e)
        {
            string fileName = "AccountLogin.json";
            string path = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\x86\Debug\netcoreapp3.1", string.Empty), @"Data\", fileName);
            File.WriteAllText(path, String.Empty);
            this.Shown += Form1_Shown;
        }
    }
}

public static class DataConnect
{
    public static string ip;
    public static string port;
    public static string machineID;
    public static string machineName;
}

public static class DataTablePage1
{
    public static DataTable dataSource = new DataTable();
}
