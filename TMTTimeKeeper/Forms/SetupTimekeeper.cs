using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TMTTimeKeeper.Models;
using TMTTimeKeeper.Services;
using TMTTimeKeeper.Utilities;

namespace TMTTimeKeeper
{
    public partial class SetupTimekeeper : Form
    {
        public Dictionary<string, string> deviceInfo { get; set; }
        public Dictionary<string, string> connectInfo { get; set; }
        public Dictionary<string, string> userInputInfo { get; set; }
        private TimeKeeper timekeeper = null;
        TimeKeeperService timekeeperObj = new TimeKeeperService();

        public Label p_lblStatus { get { return lblStatus; } set { lblStatus = value; } }

        public SetupTimekeeper()
        {
            InitializeComponent();
            StatusBarService.ShowStatusBar(lblStatus , string.Empty, true);
            this.deviceInfo = new Dictionary<string, string>()
            {
                {"Firmware V", ""},
                {"Vendor", ""},
                {"SDK V", ""},
                {"Serial No", ""},
                {"Device MAC", ""},
            };
            this.connectInfo = new Dictionary<string, string>()
            {
                {"IP", ""},
                {"Port", ""},
            };
            this.userInputInfo = new Dictionary<string, string>()
            {
                {"Machine ID", "" },
                {"Company Name", ""},
                {"Machine Name", ""},
                {"Machine Model", ""}
            };
        }

        private async void Page1_LoadAsync(object sender, EventArgs e)
        {
            timekeeper = await timekeeperObj.getTimekeeperAsync();
            if (timekeeper != null)
            {
                tbxCompanyName.Text = timekeeper.CompanyName;
                tbxMachineSeri.Text = timekeeper.Seri;
                tbxIP.Text = timekeeper.AddressIP == null ? "192.168.1.201" : timekeeper.AddressIP;
                tbxPort.Text = timekeeper.ConnectTCP == null ? "4370" : timekeeper.ConnectTCP;
            }
        }

        #region 'Manipulator Device'
        DeviceManipulator manipulator = new DeviceManipulator();
        public ZkemClient objZkeeper;
        private bool isDeviceConnected = false;

        public bool IsDeviceConnected
        {
            get { return isDeviceConnected; }
            set
            {
                isDeviceConnected = value;
                if (isDeviceConnected)
                {
                    StatusBarService.ShowStatusBar(lblStatus, "The device is connected !!", true);
                }
                else
                {
                    StatusBarService.ShowStatusBar(lblStatus, "The device is disconnected !!", true);
                    objZkeeper.Disconnect();
                }
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (tbxIP.Text == string.Empty)
            {
                StatusBarService.ShowStatusBar(lblStatus, "Địa chỉ IP is Empty", false);
            }
            else if (tbxPort.Text == string.Empty)
            {
                StatusBarService.ShowStatusBar(lblStatus, "Cổng liên kết TCP is Empty", false);
            }
            else
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    StatusBarService.ShowStatusBar(lblStatus, string.Empty, true);

                    if (IsDeviceConnected)
                    {
                        IsDeviceConnected = false;
                        this.Cursor = Cursors.Default;

                        return;
                    }

                    var ip = tbxIP.Text.Trim();
                    string port = tbxPort.Text.Trim();
                    this.connectInfo["IP"] = ip;
                    this.connectInfo["Port"] = port;

                    if (ip == string.Empty || port == string.Empty)
                        throw new Exception("The Device IP Address and Port is mandotory !!");

                    int portNumber = 4370;
                    if (!int.TryParse(port, out portNumber))
                        throw new Exception("Not a valid port number");

                    bool isValidIpA = UniversalStatic.ValidateIP(ip);
                    if (!isValidIpA)
                        throw new Exception("The Device IP is invalid !!");

                    isValidIpA = UniversalStatic.PingTheDevice(ip);
                    if (!isValidIpA)
                        throw new Exception("The device at " + ip + ":" + port + " did not respond!!");

                    objZkeeper = new ZkemClient(RaiseDeviceEvent);
                    IsDeviceConnected = objZkeeper.Connect_Net(ip, portNumber);

                    if (IsDeviceConnected)
                    {
                        string deviceInfoString = manipulator.FetchDeviceInfo(objZkeeper, 1);
                        //
                        deviceInfoString = deviceInfoString.Substring(0, deviceInfoString.Length - 1);
                        string[] temp = deviceInfoString.Split(',');
                        foreach (string s in temp)
                        {
                            if (s.IndexOf("Serial No: ") != -1)
                            {
                                this.deviceInfo["Serial No"] = s.Replace("Serial No: ", "");
                                tbxMachineSeri.Text = s.Replace("Serial No: ", "");
                            }
                        }

                        this.Cursor = Cursors.Default;

                        var timekeeper = new TimeKeeper();
                        timekeeper.Id = 1;
                        timekeeper.CompanyName = tbxCompanyName.Text;
                        timekeeper.Seri = tbxMachineSeri.Text;
                        timekeeper.AddressIP = tbxIP.Text;
                        timekeeper.ConnectTCP = tbxPort.Text;
                        timekeeperObj.AddTimekeeper(timekeeper);

                        DataConnect.ip = tbxIP.Text;
                        DataConnect.port = tbxPort.Text;
                        DataConnect.machineID = 1;
                    }
                }
                catch (Exception ex)
                {
                    StatusBarService.ShowStatusBar(lblStatus, ex.Message, false);
                }
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
                        StatusBarService.ShowStatusBar(lblStatus, "The device is switched off", true);
                        break;
                    }

                default:
                    break;
            }
        }

        private void btnPing_Click(object sender, EventArgs e)
        {
            StatusBarService.ShowStatusBar(lblStatus, string.Empty, true);

            string ipAddress = tbxIP.Text.Trim();

            bool isValidIpA = UniversalStatic.ValidateIP(ipAddress);
            if (!isValidIpA)
               MessageBox.Show("The Device IP is invalid !!");

            isValidIpA = UniversalStatic.PingTheDevice(ipAddress);
            if (isValidIpA)
                MessageBox.Show("The device is active");
            else
                MessageBox.Show("Could not read any response");
        }
        #endregion

        private void tbxIP_Validating(object sender, CancelEventArgs e)
        {
            if (tbxIP.Text == string.Empty)
            {
                StatusBarService.ShowStatusBar(lblStatus, "Địa chỉ IP is Empty", false);
            }
        }

        private void tbxPort_Validating(object sender, CancelEventArgs e)
        {
            if (tbxPort.Text == string.Empty)
            {
                StatusBarService.ShowStatusBar(lblStatus, "Cổng liên kết TCP is Empty", false);
            }
        }

       
    }
}
