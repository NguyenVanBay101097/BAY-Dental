using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TMTTimeKeeper.APIInfo;
using TMTTimeKeeper.Info;
using TMTTimeKeeper.Services;
using TMTTimeKeeper.Utilities;

namespace TMTTimeKeeper.Forms
{
    public partial class DataLogEnrollError : Form
    {

        public ZkemClient objZkeeper;
        private DataLogEnrollService dataLogEnrollService = new DataLogEnrollService();
        private TimeKeeperService timeKeeperService = new TimeKeeperService();
        private bool isDeviceConnected = false;
        public DataLogEnrollError()
        {
            InitializeComponent();
        }

        private void DataLogEnrollError_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = new Collection<MachineInfo>();
            SetHeaderText();
            MyUniversalStatic.ChangeGridProperties(dataGridView1);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            objZkeeper = new ZkemClient(RaiseDeviceEvent);
            IsDeviceConnected = objZkeeper.Connect_Net(DataConnect.ip, int.Parse(DataConnect.port));

            if (IsDeviceConnected)
            {
                try
                {
                    var file = "DataLogEnrollErrors.json";
                    var listMachinInfo = new List<MachineInfo>();

                    StatusBarService.ShowStatusBar(lblStatus, string.Empty, true);
                    var responses = await timeKeeperService.GetListModelByJson<Response>(file);
                    if (responses == null)
                    {
                        BindToGridView(null);
                        StatusBarService.ShowStatusBar(lblStatus, "Không tìm thấy dữ liệu", true);
                    }
                    else
                    {
                        foreach (var item in responses)
                        {
                            var machin = new MachineInfo();
                            machin.DateTimeRecord = item.ModelError.Date.Value.ToLongDateString();
                            machin.Status = item.Errors[0];
                            machin.MyTimeOnlyRecord = item.ModelError.Time.ToShortTimeString();
                            listMachinInfo.Add(machin);
                        }
                        BindToGridView(listMachinInfo);
                        StatusBarService.ShowStatusBar(lblStatus, responses.Count + " kết quả được tìm thấy !!", true);
                    }
                    //if (lstMachineInfo != null)
                    //{
                    //    BindToGridView(lstMachineInfo);
                    //    StatusBarService.ShowStatusBar(lblStatus, lstMachineInfo.Count + " kết quả được tìm thấy !!", true);
                    //}
                }
                catch (Exception ex)
                {
                    StatusBarService.ShowStatusBar(lblStatus, ex.Message, true);
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
                        StatusBarService.ShowStatusBar(lblStatus, "Máy chấm công đã bị tắt !!", true);
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
            dataGridView1.Columns[2].HeaderText = "Ngày Giờ";
            dataGridView1.Columns[3].HeaderText = "Ngày";
        }

    }
}
