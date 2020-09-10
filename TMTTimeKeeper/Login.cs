﻿using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows.Forms;
using TMTTimeKeeper.APIInfo;
using TMTTimeKeeper.Models;

namespace TMTTimeKeeper
{
    public partial class Login : Form
    {


        public Login()
        {
            InitializeComponent();
            ShowStatusBar(string.Empty, true);
        }

        #region 'Panel Move'
        private bool Drag;
        private int MouseX;
        private int MouseY;

        private const int WM_NCHITTEST = 0x84;
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;

        private bool m_aeroEnabled;

        private const int CS_DROPSHADOW = 0x00020000;
        private const int WM_NCPAINT = 0x0085;
        private const int WM_ACTIVATEAPP = 0x001C;

        [System.Runtime.InteropServices.DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);
        [System.Runtime.InteropServices.DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);
        [System.Runtime.InteropServices.DllImport("dwmapi.dll")]

        public static extern int DwmIsCompositionEnabled(ref int pfEnabled);
        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
            );

        public struct MARGINS
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }
        protected override CreateParams CreateParams
        {
            get
            {
                m_aeroEnabled = CheckAeroEnabled();
                CreateParams cp = base.CreateParams;
                if (!m_aeroEnabled)
                    cp.ClassStyle |= CS_DROPSHADOW; return cp;
            }
        }
        private bool CheckAeroEnabled()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                int enabled = 0; DwmIsCompositionEnabled(ref enabled);
                return (enabled == 1) ? true : false;
            }
            return false;
        }
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCPAINT:
                    if (m_aeroEnabled)
                    {
                        var v = 2;
                        DwmSetWindowAttribute(this.Handle, 2, ref v, 4);
                        MARGINS margins = new MARGINS()
                        {
                            bottomHeight = 1,
                            leftWidth = 0,
                            rightWidth = 0,
                            topHeight = 0
                        }; DwmExtendFrameIntoClientArea(this.Handle, ref margins);
                    }
                    break;
                default: break;
            }
            base.WndProc(ref m);
            if (m.Msg == WM_NCHITTEST && (int)m.Result == HTCLIENT) m.Result = (IntPtr)HTCAPTION;
        }
        private void PanelMove_MouseDown(object sender, MouseEventArgs e)
        {
            Drag = true;
            MouseX = Cursor.Position.X - this.Left;
            MouseY = Cursor.Position.Y - this.Top;
        }
        private void PanelMove_MouseMove(object sender, MouseEventArgs e)
        {
            if (Drag)
            {
                this.Top = Cursor.Position.Y - MouseY;
                this.Left = Cursor.Position.X - MouseX;
            }
        }
        private void PanelMove_MouseUp(object sender, MouseEventArgs e)
        {
            Drag = false;
        }
        #endregion

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            LoginInfo loginInfo = new LoginInfo
            {
                userName = tbxUsername.Text,
                password = tbxPassword.Text,
                rememberMe = chkRememberMe.Checked
            };

            var response = await HttpClientConfig.client.PostAsJsonAsync("api/Account/Login", loginInfo);

            if (response.IsSuccessStatusCode)
            {
                string jsonData = response.Content.ReadAsStringAsync().Result;

                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(LoginResponse));
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonData));
                stream.Position = 0;
                LoginResponse loginResponse = (LoginResponse)jsonSerializer.ReadObject(stream);

                if (loginResponse.succeeded)
                {
                    if (chkRememberMe.Checked)
                    {
                        ///save account login

                        var account = new AccountLogin();
                        account.Name = loginResponse.user.name;
                        account.UserName = loginResponse.user.userName;
                        account.CompanyId = loginResponse.user.companyId;
                        account.CompanyName = tbxCompanyName.Text;
                        account.Email = loginResponse.user.email;
                        account.AccessToken = loginResponse.token;
                        account.RefeshToken = loginResponse.refreshToken;

                        AddAccount(account);
                    }
                    else
                    {
                        AccountLoginTemp.name = loginResponse.user.name;
                    }
                    // Set Authorization
                    HttpClientConfig.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    // Add Get employees

                    //var token = loginResponse.token;
                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    //EmployeePaged employeePaged = new EmployeePaged
                    //{
                    //    offset = 0,
                    //    limit = 20,
                    //    search = null,
                    //    //isDoctor = true,
                    //    //isAssistant = true
                    //};

                    //var url = "api/Employees?";
                    //var result = new List<string>();
                    //foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(employeePaged))
                    //{
                    //    result.Add(property.Name + "=" + property.GetValue(employeePaged));
                    //}

                    //url = url + string.Join("&", result);

                    //var pro = await client.GetAsync(url);
                    // End Demo Get employees

                    var timekeeper = new TimeKeeper();
                    timekeeper.CompanyName = tbxCompanyName.Text;
                    AddTimekeeper(timekeeper);

                    DialogResult = DialogResult.OK;
                    this.Cursor = Cursors.Default;
                    Close();
                }
                else
                {
                    ShowStatusBar("Sai tên đăng nhập hoặc mật khẩu!", false);
                }
            }
            else
            {
                ShowStatusBar("Sai tên đăng nhập hoặc mật khẩu!", false);
            }
        }

        public void ShowStatusBar(string message, bool type)
        {
            if (message.Trim() == string.Empty)
            {
                lblStatus.Visible = false;
                return;
            }

            lblStatus.Visible = true;
            lblStatus.Text = message;
            lblStatus.ForeColor = Color.White;

            if (type)
                lblStatus.BackColor = Color.FromArgb(79, 208, 154);
            else
                lblStatus.BackColor = Color.Tomato;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// luu tai khoan dang nhap
        /// </summary>
        /// <param name="account"></param>
        public void AddAccount(AccountLogin account)
        {            
            string fileName = "AccountLogin.json";
            string path = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\x86\Debug\netcoreapp3.1", string.Empty), @"Data\", fileName);
            File.WriteAllText(path, JsonConvert.SerializeObject(account));
        }

        public void AddTimekeeper(TimeKeeper val)
        {
            string fileName = "TimeKeeper.json";
            string path = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\x86\Debug\netcoreapp3.1", string.Empty), @"Data\", fileName);
            File.WriteAllText(path, JsonConvert.SerializeObject(val));
        }
    }
}
