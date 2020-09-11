using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TMTTimeKeeper.Info;
using TMTTimeKeeper.Models;
using TMTTimeKeeper.Utilities;

namespace TMTTimeKeeper.Services
{
    public class EmployeeService
    {
        public List<EmployeeSync> getEmployee()
        {
            var employees = new List<EmployeeSync>();
            string fileName = "Employees.json";
            string path = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\x86\Debug\netcoreapp3.1", string.Empty), @"Data\", fileName);
            string json = File.ReadAllText(path);
            if (string.IsNullOrEmpty(json))
                return employees;
            employees = JsonConvert.DeserializeObject<List<EmployeeSync>>(json);
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

        public void AddEmployee(IList<EmployeeSync> vals)
        {
            string fileName = "Employees.json";
            string path = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\x86\Debug\netcoreapp3.1", string.Empty), @"Data\", fileName);
            File.AppendAllText(path, JsonConvert.SerializeObject(vals));
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
            List<EmployeeSync> listEmp = new List<EmployeeSync>();
            var res = JsonConvert.DeserializeObject<EmployeePagging>(rs);
            return res;
        }

        public async Task<UserInfo> GetAdmin(ZkemClient objZkeeper, int machineNumber)
        {
            UserInfo admin = new UserInfo();
            int MachineNumber = 1, Manipulation = 0, Number = 0, Params1 = 0, Params2 = 0, Params3 = 0;
            string Admin = string.Empty, User = string.Empty, Time = string.Empty;
            bool super = objZkeeper.SSR_GetSuperLogData(machineNumber, out Number, out Admin, out User, out Manipulation, out Time, out Params1, out Params2, out Params3);
            if (super)
            {

            }
            return admin;
        }
    }
}
