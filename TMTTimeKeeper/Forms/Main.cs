﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TMTTimeKeeper.Models;
using TMTTimeKeeper.Services;

namespace TMTTimeKeeper
{
    public partial class Main : Form
    {
        private Button stateNav = null;
        private AccountLogin account = null;

        AccountLoginService accountloginObj = new AccountLoginService();

        Login loginForm = new Login();
        SetupTimekeeper timekeeper = new SetupTimekeeper();
        Employee employee = new Employee();
        SetupTimekeeper setupTimekeeper = new SetupTimekeeper();

        public Main()
        {
            InitializeComponent();
        }

        private async void Main_Load(object sender, EventArgs e)
        {
            // Update port # in the following line.
            HttpClientConfig.client.BaseAddress = new Uri("https://localhost:44377/");
            //client.BaseAddress = new Uri($"https://{chinhanh}.tdental.vn");
            HttpClientConfig.client.DefaultRequestHeaders.Accept.Clear();
            HttpClientConfig.client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var acc = await GetAccount();
            if (acc != null && acc.AccessToken != null)
                HttpClientConfig.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", acc.AccessToken);
            this.Shown += Form1_Shown;
        }

        public async Task<AccountLogin> GetAccount()
        {
            var acc = new AccountLogin();
            string fileName = "AccountLogin.json";
            string path = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\x86\Debug\netcoreapp3.1", string.Empty), @"Data\", fileName);
            string json = await File.ReadAllTextAsync(path);
            if (json != null)
            {
                acc = JsonConvert.DeserializeObject<AccountLogin>(json);
            }
            return acc;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            account = accountloginObj.getAccount();
            if (account == null)
            {
                Visible = false;
                var result = loginForm.ShowDialog();

                if (result == DialogResult.OK)
                {
                    account = accountloginObj.getAccount();
                    if (account != null)
                        lblAccountName.Text = account.Name;
                    else
                        lblAccountName.Text = AccountLoginTemp.name;
                    //
                    stateNav = button1;
                    nav(timekeeper, content);
                    //
                    Visible = true;
                }
                else if (result == DialogResult.Cancel)
                {
                    Close();
                }
            }
            else
            {
                stateNav = button1;
                nav(timekeeper, content);

                lblAccountName.Text = account.Name;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            nav(timekeeper, content);
            actNav(button1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (DataConnect.ip == null || DataConnect.port == null)
            {
                StatusBarService.ShowStatusBar(timekeeper.p_lblStatus, "Chưa kết nối máy chấm công !!", false);
            }
            else
            {
                nav(employee, content);
                actNav(button2);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (DataConnect.ip == null || DataConnect.port == null)
            {
                StatusBarService.ShowStatusBar(timekeeper.p_lblStatus, "Chưa kết nối máy chấm công !!", false);
            }
            else
            {
                nav(setupTimekeeper, content);
                actNav(button3);
            }
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

        private void lblLogout_Click(object sender, EventArgs e)
        {
            string fileName = "AccountLogin.json";
            string path = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\x86\Debug\netcoreapp3.1", string.Empty), @"Data\", fileName);
            File.WriteAllText(path, String.Empty);
            Form1_Shown(sender, e);
        }
    }
}

public static class HttpClientConfig
{
    public static HttpClient client = new HttpClient();
}

public static class DataConnect
{
    public static string ip;
    public static string port;
    public static int machineID;
}

public static class AccountLoginTemp
{
    public static string name;
}

