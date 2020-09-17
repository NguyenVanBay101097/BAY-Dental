///
///    Experimented By : Ozesh Thapa
///    Email: dablackscarlet@gmail.com
///
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using TMTTimeKeeper.Utilities;
using TMTTimeKeeper.Info;
using TMTTimeKeeper.Helpers;
using TMTTimeKeeper.Services;
using TMTTimeKeeper.Models;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Linq;

namespace TMTTimeKeeper
{
    public partial class Master : Form
    {
        DeviceManipulator manipulator = new DeviceManipulator();
        public ZkemClient objZkeeper;
        private bool isDeviceConnected = false;
        private readonly TimeKeeperService timeKeeperObj = new TimeKeeperService();
        public SDKHelper SDK = new SDKHelper();
        public DataLogEnrollService dataLogEnroll = new DataLogEnrollService();
        public ReadLogResult readLogData = new ReadLogResult();
        public readonly int MachineNumber = 1;
        private readonly Login loginForm = new Login();
        private readonly AccountLoginService accountloginObj = new AccountLoginService();

        public bool IsDeviceConnected
        {
            get { return isDeviceConnected; }
            set
            {
                isDeviceConnected = value;
                if (isDeviceConnected)
                {
                    ShowStatusBar("The device is connected !!", true);
                    btnConnect.Text = "Disconnect";
                    ToggleControls(true);
                }
                else
                {
                    ShowStatusBar("The device is diconnected !!", true);
                    objZkeeper.Disconnect();
                    btnConnect.Text = "Connect";
                    ToggleControls(false);
                }
            }
        }


        private void ToggleControls(bool value)
        {
            tbxPort.Enabled = !value;
            tbxDeviceIP.Enabled = !value;
        }

        public Master()
        {
            InitializeComponent();
            ShowStatusBar(string.Empty, true);
            DisplayEmpty();
            loadMaster();
        }

        public async void loadMaster()
        {

            var account = await CheckLoginAsync();
            if (account == null)
            {
                Visible = false;
                var result = loginForm.ShowDialog();

                if (result == DialogResult.OK)
                {
                    account = await accountloginObj.getAccountAsync();
                    if (account != null)
                    {
                        lbEmployee.Visible = true;
                        lbEmployee.Text = account.Name;
                    }
                    Visible = true;

                    //ToggleControls(false);

                }
                else if (result == DialogResult.Cancel)
                {
                    Close();
                }
            }
            else
            {
                SetHttpClient(account.CompanyName, account.AccessToken);
                lbEmployee.Visible = true;
                lbEmployee.Text = account.Name;
            }
        }

        /// <summary>
        /// Your Events will reach here if implemented
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="actionType"></param>
        private void RaiseDeviceEvent(object sender, string actionType)
        {
            switch (actionType)
            {
                case UniversalStatic.acx_Disconnect:
                    {
                        ShowStatusBar("The device is switched off", true);
                        DisplayEmpty();
                        btnConnect.Text = "Connect";
                        ToggleControls(false);
                        break;
                    }

                default:
                    break;
            }

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string ip = tbxDeviceIP.Text.Trim();
            string port = tbxPort.Text.Trim();

            var result = SDK.sta_ConnectTCP(ip, port);
            if (result == 1)
            {
                btnConnect.Text = "Ngắt kết nối";
                string returnValue = string.Empty;
                ShowStatusBar("Kết nối thành công", true);
                var seri = SDK.axCZKEM1.GetSerialNumber(MachineNumber, out returnValue);
                if (seri)
                {
                    lblDeviceInfo.Text = returnValue;
                    lblDeviceInfo.Visible = true;
                }
            }
            else if (result == -2)
            {
                ShowStatusBar("Đã ngắt kết nối", true);
                btnConnect.Text = "Kết nối";
                lblDeviceInfo.Visible = false;
            }
            else
            {
                ShowStatusBar("Kết nối thất bại", false);
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
                lblStatus.BackColor = Color.FromArgb(230, 112, 134);
        }


        //private void btnPingDevice_Click(object sender, EventArgs e)
        //{
        //    ShowStatusBar(string.Empty, true);

        //    string ipAddress = tbxDeviceIP.Text.Trim();

        //    bool isValidIpA = UniversalStatic.ValidateIP(ipAddress);
        //    if (!isValidIpA)
        //        throw new Exception("The Device IP is invalid !!");

        //    isValidIpA = UniversalStatic.PingTheDevice(ipAddress);
        //    if (isValidIpA)
        //        ShowStatusBar("The device is active", true);
        //    else
        //        ShowStatusBar("Could not read any response", false);
        //}

        private void btnGetAllUserID_Click(object sender, EventArgs e)
        {
            try
            {
                ICollection<UserIDInfo> lstUserIDInfo = manipulator.GetAllUserID(objZkeeper, MachineNumber);

                if (lstUserIDInfo != null && lstUserIDInfo.Count > 0)
                {
                    BindToGridView(lstUserIDInfo);
                    ShowStatusBar(lstUserIDInfo.Count + " records found !!", true);
                }
                else
                {
                    DisplayEmpty();
                    DisplayListOutput("No records found");
                }

            }
            catch (Exception ex)
            {
                DisplayListOutput(ex.Message);
            }

        }

        private void btnPullData_Click(object sender, EventArgs e)
        {
            string fromTime = dpDateFrom.Value.Date.ToString("yyyy-MM-dd HH:mm:ss");
            string toTime = dpDateTo.Value.Date.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");
            var result = SDK.sta_readLogByPeriod(fromTime, toTime);
            readLogData = result;
            ShowStatusBar($"Tìm thấy {result.Data.Count()} chấm công", true);
            BindToGridView(result.Data);
            dgvRecords.Columns[0].HeaderText = "Mã vân tay";
            dgvRecords.Columns[1].HeaderText = "Ngày chấm công";
            dgvRecords.Columns[2].HeaderText = "Kiểu";
            dgvRecords.Columns[3].HeaderText = "Vào/Ra";
            dgvRecords.Columns[4].HeaderText = "Loại chấm công";
        }

        private void ClearGrid()
        {
            if (dgvRecords.Controls.Count > 2)
            { dgvRecords.Controls.RemoveAt(2); }


            dgvRecords.DataSource = null;
            dgvRecords.Controls.Clear();
            dgvRecords.Rows.Clear();
            dgvRecords.Columns.Clear();
        }

        private void BindToGridView(object list)
        {
            ClearGrid();
            dgvRecords.DataSource = list;
            dgvRecords.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            UniversalStatic.ChangeGridProperties(dgvRecords);
        }

        public async Task<AccountLogin> CheckLoginAsync()
        {
            AccountLogin acc = new AccountLogin();
            var fileName = "AccountLogin.json";
            acc = await timeKeeperObj.GetModelByJson<AccountLogin>(fileName);
            return acc;
        }

        private void DisplayListOutput(string message)
        {
            if (dgvRecords.Controls.Count > 2)
            { dgvRecords.Controls.RemoveAt(2); }

            ShowStatusBar(message, false);
        }

        private void DisplayEmpty()
        {
            ClearGrid();
            //dgvRecords.Controls.Add(new DataEmpty());
        }

        public static void SetHttpClient(string url, string accessToken = "")
        {

            HttpClientConfig.client.BaseAddress = new Uri(url);
            HttpClientConfig.client.DefaultRequestHeaders.Accept.Clear();
            HttpClientConfig.client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            if (!string.IsNullOrEmpty(accessToken))
                HttpClientConfig.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        private void pnlHeader_Paint(object sender, PaintEventArgs e)
        { UniversalStatic.DrawLineInFooter(pnlHeader, Color.FromArgb(204, 204, 204), 2); }


        private void tbxPort_TextChanged(object sender, EventArgs e)
        { UniversalStatic.ValidateInteger(tbxPort); }

        private async void btnSyncData_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                var response = new ResponseDataLogViewModel();
                if (readLogData != null)
                {
                    response = await dataLogEnroll.SyncLogData(readLogData);
                    ShowStatusBar($"Thành công: {response.isSuccess}, Lỗi: {response.isError}", true);
                }
                else
                {
                    ShowStatusBar("Lỗi hệ thống", false);
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

            this.Cursor = Cursors.Default;
        }
    }
}
