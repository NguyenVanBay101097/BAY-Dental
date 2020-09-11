using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TMTTimeKeeper.Info;
using TMTTimeKeeper.Services;
using TMTTimeKeeper.Utilities;

namespace TMTTimeKeeper
{
    public partial class Page3 : Form
    {
        DeviceManipulator manipulator = new DeviceManipulator();
        public ZkemClient objZkeeper;
        private bool isDeviceConnected = false;
        public Page3()
        {
            InitializeComponent();
            StatusBarService.ShowStatusBar(lblStatus, string.Empty, true);
        }

        private void Page3_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = new Collection<MachineInfo>();
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
                    StatusBarService.ShowStatusBar(lblStatus, string.Empty, true);

                    ICollection<MachineInfo> lstMachineInfo = manipulator.GetAllLogData(objZkeeper, DataConnect.machineID);
                    if (lstMachineInfo != null)
                    {
                        BindToGridView(lstMachineInfo);
                        StatusBarService.ShowStatusBar(lblStatus, lstMachineInfo.Count + " kết quả được tìm thấy !!", true);
                    }

                }
                catch (Exception ex)
                {
                    StatusBarService.ShowStatusBar(lblStatus, ex.Message, true);
                }
            }
        }

        private async void btn_sync_Click(object sender, EventArgs e)
        {
            objZkeeper = new ZkemClient(RaiseDeviceEvent);
            IsDeviceConnected = objZkeeper.Connect_Net(DataConnect.ip, int.Parse(DataConnect.port));
            if (IsDeviceConnected)
            {
                try
                {
                    StatusBarService.ShowStatusBar(lblStatus, string.Empty, true);

                    var response = await manipulator.SyncLogData();

                    if (response != null && response.Success)
                    {
                        StatusBarService.ShowStatusBar(lblStatus, "Đồng bộ thành công", true);
                    }
                    else
                    {
                        foreach (var item in response.Errors)
                        {
                            StatusBarService.ShowStatusBar(lblStatus, item, true);
                        }
                    }
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
                        StatusBarService.ShowStatusBar(lblStatus, "The device is switched off", true);
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
                    StatusBarService.ShowStatusBar(lblStatus, "The device is connected !!", true);
                }
                else
                {
                    StatusBarService.ShowStatusBar(lblStatus, "The device is disconnected !!", true);
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
            dataGridView1.Columns[4].HeaderText = "Giờ";
            dataGridView1.Columns[5].HeaderText = "Trạng Thái";
        }


    }
}
