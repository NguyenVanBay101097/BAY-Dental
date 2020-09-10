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
using TMTTimeKeeper.Utilities;

namespace TMTTimeKeeper
{
    public partial class Page1 : Form
    {
        public Dictionary<string, string> deviceInfo { get; set; }
        public Dictionary<string, string> connectInfo { get; set; }
        public Dictionary<string, string> userInputInfo { get; set; }
        private TimeKeeper timekeeper = null;
        public Page1()
        {
            InitializeComponent();
            ShowStatusBar(string.Empty, true);
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

        private void Page1_Load(object sender, EventArgs e)
        {
            timekeeper = getTimekeeper();
            if (timekeeper != null)
            {
                tbxCompanyName.Text = timekeeper.CompanyName;
                tbxMachineName.Text = timekeeper.Name;
                tbxMachineSeri.Text = timekeeper.Seri;
                tbxIP.Text = timekeeper.AddressIP;
                tbxPort.Text = timekeeper.ConnectTCP;
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
                    ShowStatusBar("The device is connected !!", true);
                }
                else
                {
                    ShowStatusBar("The device is disconnected !!", true);
                    objZkeeper.Disconnect();
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

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (tbxMachineName.Text == string.Empty)
            {
                ShowStatusBar("Tên máy is Empty", false);
            }
            else if (tbxIP.Text == string.Empty)
            {
                ShowStatusBar("Địa chỉ IP is Empty", false);
            }
            else if (tbxPort.Text == string.Empty)
            {
                ShowStatusBar("Cổng liên kết TCP is Empty", false);
            }
            else
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    ShowStatusBar(string.Empty, true);

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
                        timekeeper.Name = tbxMachineName.Text;
                        timekeeper.Seri = tbxMachineSeri.Text;
                        timekeeper.AddressIP = tbxIP.Text;
                        timekeeper.ConnectTCP = tbxPort.Text;
                        AddTimekeeper(timekeeper);

                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    ShowStatusBar(ex.Message, false);
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
                        ShowStatusBar("The device is switched off", true);
                        break;
                    }

                default:
                    break;
            }
        }

        private void btnPing_Click(object sender, EventArgs e)
        {
            ShowStatusBar(string.Empty, true);

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
                ShowStatusBar("Địa chỉ IP is Empty", false);
            }
        }

        private void tbxMachineName_Validating(object sender, CancelEventArgs e)
        {
            if (tbxMachineName.Text == string.Empty)
            {
                ShowStatusBar("Tên máy is Empty", false);
            }
        }

        private void tbxPort_Validating(object sender, CancelEventArgs e)
        {
            if (tbxPort.Text == string.Empty)
            {
                ShowStatusBar("Cổng liên kết TCP is Empty", false);
            }
        }

        public void AddTimekeeper(TimeKeeper val)
        {
            string fileName = "TimeKeeper.json";
            string path = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\x86\Debug\netcoreapp3.1", string.Empty), @"Data\", fileName);
            File.WriteAllText(path, JsonConvert.SerializeObject(val));
        }

        public TimeKeeper getTimekeeper()
        {
            var timekeeper = new TimeKeeper();
            string fileName = "TimeKeeper.json";
            string path = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\x86\Debug\netcoreapp3.1", string.Empty), @"Data\", fileName);
            using (StreamReader sr = File.OpenText(path))
            {
                timekeeper = JsonConvert.DeserializeObject<TimeKeeper>(sr.ReadToEnd());
            }
            return timekeeper;
        }
    }
}
