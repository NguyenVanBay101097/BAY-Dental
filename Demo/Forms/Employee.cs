using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Demo.APIInfo;
using Demo.Info;
using Demo.Models;
using Demo.Services;
using Demo.Utilities;
using zkemkeeper;

namespace Demo
{
    public partial class Employee : Form
    {
        DeviceManipulator manipulator = new DeviceManipulator();
        public ZkemClient objZkeeper;
        private bool isDeviceConnected = false;
        AccountLoginService accountLoginObj = new AccountLoginService();
        TimeKeeperService timeKeeperObj = new TimeKeeperService();
        EmployeeService employeeObj = new EmployeeService();
        public Employee()
        {
            InitializeComponent();
            StatusBarService.ShowStatusBar(lblStatus, string.Empty, true);
        }

        private void Page2_Load(object sender, EventArgs e)
        {
            LoadDataEmployeeAsync();
        }

        private void button1_ClickAsync(object sender, EventArgs e)
        {
            objZkeeper = new ZkemClient(RaiseDeviceEvent);
            IsDeviceConnected = objZkeeper.Connect_Net(DataConnect.ip, int.Parse(DataConnect.port));

            if (IsDeviceConnected)
            {
                try
                {
                    LoadDataEmployeeAsync();
                }
                catch (Exception ex)
                {
                    StatusBarService.ShowStatusBar(lblStatus,ex.Message, true);
                }
            }
        }

        public void LoadDataEmployeeAsync()
        {
            objZkeeper = new ZkemClient(RaiseDeviceEvent);
            StatusBarService.ShowStatusBar(lblStatus, string.Empty, true);
            
            ICollection<UserInfo> lstFingerPrintTemplates = manipulator.GetAllUserInfo(objZkeeper, DataConnect.machineID);
            if (lstFingerPrintTemplates != null && lstFingerPrintTemplates.Count > 0)
            {
                BindToGridView(lstFingerPrintTemplates);
                StatusBarService.ShowStatusBar(lblStatus, lstFingerPrintTemplates.Count + " kết quả được tìm thấy !!", true);
            }
            else
            {
                BindToGridView(lstFingerPrintTemplates);
                StatusBarService.ShowStatusBar(lblStatus, lstFingerPrintTemplates.Count + " kết quả được tìm thấy !!", true);
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
                        StatusBarService.ShowStatusBar(lblStatus, "Máy chấm công đã bị tắt !!", false);
                        break;
                    }

                default:
                    break;
            }
        }

        public bool IsDeviceConnected
        {
            get { return isDeviceConnected; }
            set
            {
                isDeviceConnected = value;
                if (isDeviceConnected)
                {
                    StatusBarService.ShowStatusBar(lblStatus, "Đã kết nối máy chấm công !!", true);
                }
                else
                {
                    StatusBarService.ShowStatusBar(lblStatus, "Đã ngắt kết nối máy chấm công !!", true);
                    objZkeeper.Disconnect();
                }
            }
        }

        private void BindToGridView(object list)
        {
            ClearGrid();
            dataGridView1.DataSource = list;
            SetHeaderText();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            MyUniversalStatic.ChangeGridProperties(dataGridView1);
        }

        private void ClearGrid()
        {
            if (dataGridView1.Controls.Count > 2)
            { dataGridView1.Controls.RemoveAt(2); }

            dataGridView1.DataSource = null;
            dataGridView1.Controls.Clear();
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
        }

        private void SetHeaderText()
        {
            dataGridView1.Columns[0].HeaderText = "ID Máy";
            dataGridView1.Columns[1].HeaderText = "ID TK";
            dataGridView1.Columns[2].HeaderText = "Tên TK";
            dataGridView1.Columns[3].HeaderText = "Ngón Tay";
            dataGridView1.Columns[4].HeaderText = "Dữ liệu";
            dataGridView1.Columns[5].HeaderText = "Quyền";
            dataGridView1.Columns[6].HeaderText = "Mật Khẩu";
            dataGridView1.Columns[7].HeaderText = "Trạng thái";
            dataGridView1.Columns[8].HeaderText = "iFlag";
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            objZkeeper = new ZkemClient(RaiseDeviceEvent);
            IsDeviceConnected = objZkeeper.Connect_Net(DataConnect.ip, int.Parse(DataConnect.port));

            if (IsDeviceConnected)
            {
                try
                {
                    StatusBarService.ShowStatusBar(lblStatus, string.Empty, true);
                    var account = await accountLoginObj.getAccount();
                    if (account == null)
                        StatusBarService.ShowStatusBar(lblStatus, "Lỗi đăng nhập !!", false);

                    var timeKeeper = await timeKeeperObj.getTimekeeper();
                    if (timeKeeper == null)
                        StatusBarService.ShowStatusBar(lblStatus, "Chưa kết nối máy chấm công !!", false);

                    List<EmployeeSync> listEmp = new List<EmployeeSync>();
                    var res = await employeeObj.GetEmployeePC();
                    var empJsons = await employeeObj.getEmployee() != null ? await employeeObj.getEmployee() : new List<EmployeeSync>();
                    listEmp = res.Items.Where(x => !empJsons.Any(s => s.Id == x.Id)).ToList();
                   
                    var listSave = new List<EmployeeSync>();                    

                    ///Set User
                    for (int i = 0; i < listEmp.Count(); i++)
                    {
                        var emp = listEmp[i];
                        int MachineNumber = DataConnect.machineID;
                        string EnrollNumber = (i + 1).ToString();
                        string Name = employeeObj.RemoveVietnamese(emp.Name);
                        string Password = "123";
                        int Privilege = 1;
                        bool Enabled = true;

                        bool result = manipulator.PushUserDataToDevice(objZkeeper, MachineNumber, EnrollNumber, Name);

                        if (result)
                        {
                            //Add json
                            var employee = new EmployeeSync();
                            employee.Id = emp.Id;
                            employee.IdKP = Int32.Parse(EnrollNumber);
                            employee.Name = Name;
                            employee.MachineNumber = MachineNumber;
                            employee.Password = Password;
                            employee.Privelage = Privilege;
                            employee.Enabled = Enabled;
                            listSave.Add(employee);
                        }
                        else
                        {
                            StatusBarService.ShowStatusBar(lblStatus, "Kết quả không được lưu trữ trong thiết bị", false);
                        }
                    }
                    if (listSave.Count() > 0)
                        await employeeObj.AddEmployee(listSave);

                    await employeeObj.LoadDataEmployeeAsync();

                }
                catch (Exception ex)
                {
                    StatusBarService.ShowStatusBar(lblStatus, ex.Message, false);
                }
            }
        }
    }
}
