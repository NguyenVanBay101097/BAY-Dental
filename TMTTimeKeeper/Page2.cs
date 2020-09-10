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
using TMTTimeKeeper.APIInfo;
using TMTTimeKeeper.Info;
using TMTTimeKeeper.Models;
using TMTTimeKeeper.Utilities;
using zkemkeeper;

namespace TMTTimeKeeper
{
    public partial class Page2 : Form
    {
        DeviceManipulator manipulator = new DeviceManipulator();
        public ZkemClient objZkeeper;
        private bool isDeviceConnected = false;
        Main main = new Main();
        public Page2()
        {
            InitializeComponent();
            ShowStatusBar(string.Empty, true);
        }

        private void Page2_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = new Collection<UserInfo>();
            SetHeaderText();
            MyUniversalStatic.ChangeGridProperties(dataGridView1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            objZkeeper = new ZkemClient(RaiseDeviceEvent);
            IsDeviceConnected = objZkeeper.Connect_Net(DataConnect.ip, int.Parse(DataConnect.port));

            if (IsDeviceConnected)
            {
                try
                {
                    ShowStatusBar(string.Empty, true);

                    ICollection<UserInfo> lstFingerPrintTemplates = manipulator.GetAllUserInfo(objZkeeper, DataConnect.machineID);
                    if (lstFingerPrintTemplates != null && lstFingerPrintTemplates.Count > 0)
                    {
                        BindToGridView(lstFingerPrintTemplates);
                        ShowStatusBar(lstFingerPrintTemplates.Count + " records found !!", true);
                    }
                    else
                    {
                        //DisplayListOutput("No records found");
                    }
                }
                catch (Exception ex)
                {
                    //DisplayListOutput(ex.Message);
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
                    ShowStatusBar(string.Empty, true);
                    var account = main.getAccount();
                    if (account == null)
                        MessageBox.Show("lỗi đăng nhập");

                    var timeKeeper = main.getTimeKepper();
                    if (timeKeeper == null)
                        MessageBox.Show("chưa kết nối máy chấm công");

                    // Update port # in the following line.
                    HttpClientConfig.client.BaseAddress = new Uri("https://localhost:44377/");
                    //client.BaseAddress = new Uri($"https://{chinhanh}.tdental.vn");
                    HttpClientConfig.client.DefaultRequestHeaders.Accept.Clear();
                    HttpClientConfig.client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpClientConfig.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", account.AccessToken);

                    EmployeePaged employeePaged = new EmployeePaged
                    {
                        offset = 0,
                        limit = 100,
                        search = null
                    };

                    var response = await HttpClientConfig.client.PostAsJsonAsync("api/Employees/SearchRead", employeePaged);
                    var rs = response.Content.ReadAsStringAsync().Result;
                    List<Employee> listEmp = new List<Employee>();
                    var res = JsonConvert.DeserializeObject<EmployeePagging>(rs);
                    var empJsons = main.getEmployee();
                    listEmp = res.Items.Where(x => !empJsons.Any(s => s.Id == x.Id)).ToList();


                    var listSave = new List<Employee>();
                    ///Set User
                    for (int i = 0; i < listEmp.Count(); i++)
                    {
                        var emp = listEmp[i];
                        int MachineNumber = timeKeeper.Id;
                        string EnrollNumber = (i + 1).ToString();
                        string Name = RemoveVietnamese(emp.Name);
                        string Password = "123";
                        int Privilege = 1;
                        bool Enabled = true;

                        bool result = manipulator.PushUserDataToDevice(objZkeeper, MachineNumber, EnrollNumber, Name);

                        if (result)
                        {
                            //Add json
                            var employee = new Employee();
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
                            MessageBox.Show("Result Not Stored in the Device");
                        }
                    }
                    if (listSave.Count() > 0)
                        AddEmployee(listSave);

                }
                catch (Exception ex)
                {
                    //DisplayListOutput(ex.Message);
                }
            }
        }

        public static string RemoveVietnamese(string text)
        {
            string result = text.ToUpper();
            result = Regex.Replace(result, "à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ|/g", "a");
            result = Regex.Replace(result, "è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ|/g", "e");
            result = Regex.Replace(result, "ì|í|ị|ỉ|ĩ|/g", "i");
            result = Regex.Replace(result, "ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ|/g", "o");
            result = Regex.Replace(result, "ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ|/g", "u");
            result = Regex.Replace(result, "ỳ|ý|ỵ|ỷ|ỹ|/g", "y");
            result = Regex.Replace(result, "đ", "d");
            return result;
        }

        public void AddEmployee(IList<Employee> vals)
        {
            string fileName = "Employees.json";
            string path = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\x86\Debug\netcoreapp3.1", string.Empty), @"Data\", fileName);
            File.AppendAllText(path, JsonConvert.SerializeObject(vals));
            MessageBox.Show("Đồng bộ thành công");
        }
    }
}
