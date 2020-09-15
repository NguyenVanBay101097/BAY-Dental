using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Demo.Info;
using Demo.Models;
using Demo.Utilities;

namespace Demo.Services
{
    public class EmployeeService
    {
        public ZkemClient objZkeeper;
        TimeKeeperService timeKeeperObj = new TimeKeeperService();
        DeviceManipulator manipulator = new DeviceManipulator();

        public async Task<List<EmployeeSync>> getEmployee()
        {
            var employees = new List<EmployeeSync>();
            string fileName = "Employees.json";
            employees = await timeKeeperObj.GetListModelByJson<EmployeeSync>(fileName);
            return employees;
        }

        public string RemoveVietnamese(string text)
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

        public async Task AddEmployee(IList<EmployeeSync> vals)
        {
            string fileName = "Employees.json";
            string path = Path.Combine(System.Windows.Forms.Application.UserAppDataPath, fileName);
            if (!File.Exists(path))
                File.Create(path);
            var list = await getEmployee() != null ? await getEmployee() : new List<EmployeeSync>() ;
            if (list.Any())
            {
                list.AddRange(vals);

            }

            timeKeeperObj.SetListJson<EmployeeSync>(fileName, vals);

            MessageBox.Show("Đồng bộ thành công");
        }

        public async Task<EmployeePagging> GetEmployeePC()
        {
            EmployeePaged employeePaged = new EmployeePaged
            {
                offset = 0,
                limit = 100,
                search = null
            };

            var response = await HttpClientConfig.client.PostAsJsonAsync("api/Employees/SearchRead", employeePaged);
            var rs = response.Content.ReadAsStringAsync().Result;
            var res = JsonConvert.DeserializeObject<EmployeePagging>(rs);
            return res;
        }

        public async Task<EmployeeSync> CreateEmployee(UserInfo val)
        {
            var empSave = new EmployeeDisplay();
            empSave.Name = val.Name;
            var response = await HttpClientConfig.client.PostAsJsonAsync("api/Employees", empSave);
            var rs = response.Content.ReadAsStringAsync().Result;
            var res = JsonConvert.DeserializeObject<EmployeeDisplay>(rs);
            var emp = new EmployeeSync();
            if (res != null)
            {
                emp.Id = res.Id;
                emp.IdKP = Int32.Parse(val.EnrollNumber);
                emp.Name = val.Name;
                emp.MachineNumber = val.MachineNumber;
                emp.Password = val.Password;
                emp.Privelage = 1;
                emp.Enabled = val.Enabled;
            }

            return emp;
        }

        public async Task<ICollection<UserInfo>> LoadDataEmployeeAsync()
        {
            objZkeeper = new ZkemClient(RaiseDeviceEvent);
            var timeKeeperJson = await timeKeeperObj.getTimekeeper();

            ICollection<UserInfo> lstFingerPrintTemplates = manipulator.GetAllUserInfo(objZkeeper, DataConnect.machineID);
            if (timeKeeperJson != null)
            {
                try
                {
                    if (lstFingerPrintTemplates != null && lstFingerPrintTemplates.Any())
                    {
                        foreach (var emp in lstFingerPrintTemplates)
                        {
                            UserInfo fpInfo = new UserInfo();
                            fpInfo.MachineNumber = emp.MachineNumber;
                            fpInfo.EnrollNumber = emp.EnrollNumber.ToString();
                            fpInfo.Name = emp.Name;
                            fpInfo.TmpData = emp.TmpData;
                            fpInfo.Privelage = emp.Privelage;
                            fpInfo.Password = emp.Password;
                            fpInfo.Enabled = emp.Enabled;
                            fpInfo.iFlag = emp.iFlag == null ? 0.ToString() : emp.iFlag;

                            lstFingerPrintTemplates.Add(fpInfo);
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
            else
            {
                MessageBox.Show("Chưa kết nối máy chấm công");
            }



            return lstFingerPrintTemplates;
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
                        MessageBox.Show("The device is switched off");
                        break;
                    }

                default:
                    break;
            }
        }



    }
}
