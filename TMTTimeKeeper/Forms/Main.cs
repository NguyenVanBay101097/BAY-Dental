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

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            // Update port # in the following line.
            HttpClientConfig.client.BaseAddress = new Uri("https://localhost:44377/");
            //client.BaseAddress = new Uri($"https://{chinhanh}.tdental.vn");
            HttpClientConfig.client.DefaultRequestHeaders.Accept.Clear();
            HttpClientConfig.client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            this.Shown += Form1_Shown;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            account = accountloginObj.getAccount();
            if (account == null)
            {
                Visible = false;
                Login loginForm = new Login();
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
                    SetupTimekeeper timekeeper = new SetupTimekeeper();
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
                SetupTimekeeper timekeeper = new SetupTimekeeper();
                nav(timekeeper, content);

                lblAccountName.Text = account.Name;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetupTimekeeper timekeeper = new SetupTimekeeper();
            nav(timekeeper, content);
            actNav(button1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Employee page2 = new Employee();
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
    public static string machineName;
}

public static class AccountLoginTemp
{
    public static string name;
}

