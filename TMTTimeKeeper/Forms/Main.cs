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
        TimeKeeperService timekeeperObj = new TimeKeeperService();

        Login loginForm = new Login();

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            this.Shown += Form1_ShownAsync;
        }


        private async void Form1_ShownAsync(object sender, EventArgs e)
        {
            account =await accountloginObj.getAccountAsync();
            if (account == null)
            {
                Visible = false;
                var result = loginForm.ShowDialog();

                if (result == DialogResult.OK)
                {
                    account = await accountloginObj.getAccountAsync();
                    if (account != null)
                        lblAccountName.Text = account.Name;
                    else
                        lblAccountName.Text = AccountLoginTemp.name;
                    //
                    stateNav = button1;
                    //nav(timekeeper, content);
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
                // Update port # in the following line.
                if (HttpClientConfig.client.BaseAddress == null)
                    HttpClientConfig.client.BaseAddress = new Uri(account.CompanyName);
                HttpClientConfig.client.DefaultRequestHeaders.Accept.Clear();
                HttpClientConfig.client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

                // Set Authorization
                HttpClientConfig.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", account.AccessToken);
                account = await accountloginObj.RefreshAccesstokenAsync(account);

                stateNav = button1;
                //nav(timekeeper, content);

                lblAccountName.Text = account.Name;
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
            timekeeperObj.SetJson<AccountLogin>(fileName, null);
            Form1_ShownAsync(sender, e);
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

