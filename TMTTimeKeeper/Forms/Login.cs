using Microsoft.AspNetCore.Hosting;
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
using TMTTimeKeeper.Services;

namespace TMTTimeKeeper
{
    public partial class Login : Form
    {

        AccountLoginService accountLoginObj = new AccountLoginService();
        TimeKeeperService timeKeeperObj = new TimeKeeperService();

        public Login()
        {
            InitializeComponent();
            ShowStatusBar(string.Empty, true);
            //
            this.ActiveControl = tbxCompanyName;
            tbxCompanyName.Focus();
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
            if (tbxCompanyName.Text == string.Empty)
            {
                ShowStatusBar("Địa chỉ phòng khám đang rỗng", false);
            }
            else if (tbxUsername.Text == string.Empty)
            {
                ShowStatusBar("Tên đăng nhập đang rỗng", false);
            }
            else if (tbxPassword.Text == string.Empty)
            {
                ShowStatusBar("Mật khẩu đang rỗng", false);
            }
            else
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    ShowStatusBar(string.Empty, true);

                    // Update port # in the following line.
                    if (HttpClientConfig.client.BaseAddress == null)
                        HttpClientConfig.client.BaseAddress = new Uri(tbxCompanyName.Text);
                    HttpClientConfig.client.DefaultRequestHeaders.Accept.Clear();
                    HttpClientConfig.client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

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
                                account.Id = loginResponse.user.id;
                                account.Name = loginResponse.user.name;
                                account.UserName = loginResponse.user.userName;
                                account.CompanyId = loginResponse.user.companyId;
                                account.CompanyName = tbxCompanyName.Text;
                                account.Email = loginResponse.user.email;
                                account.AccessToken = loginResponse.token;
                                account.RefeshToken = loginResponse.refreshToken;

                                accountLoginObj.AddAccount(account);
                            }
                            else
                            {
                                AccountLoginTemp.name = loginResponse.user.name;
                            }

                            // Set Authorization
                            HttpClientConfig.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse.token);

                            #region 'Demo Get employees'
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
                            #endregion

                            var timekeeper = new TimeKeeper();
                            timekeeper.CompanyName = tbxCompanyName.Text;
                            timeKeeperObj.AddTimekeeper(timekeeper);

                            DialogResult = DialogResult.OK;
                            this.Cursor = Cursors.Default;
                            Close();
                        }
                        else
                        {
                            ShowStatusBar("Thông tin đăng nhập sai!", false);
                        }
                    }
                    else
                    {
                        ShowStatusBar("Lỗi ...!", false);
                    }
                }
                catch (Exception ex)
                {
                    ShowStatusBar(ex.Message, false);
                }
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

        private void tbxCompanyName_Validating(object sender, CancelEventArgs e)
        {
            if (tbxCompanyName.Text == string.Empty)
            {
                ShowStatusBar("Tên chi nhánh đang rỗng", false);
            }
        }

        private void tbxUsername_Validating(object sender, CancelEventArgs e)
        {
            if (tbxUsername.Text == string.Empty)
            {
                ShowStatusBar("Tên đăng nhập đang rỗng", false);
            }
        }

        private void tbxPassword_Validating(object sender, CancelEventArgs e)
        {
            if (tbxPassword.Text == string.Empty)
            {
                ShowStatusBar("Mật khẩu đang rỗng", false);
            }
        }

        private void checkEnter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin.PerformClick();
            }
        }
    }
}
