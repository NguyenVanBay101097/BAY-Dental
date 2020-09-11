using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TMTTimeKeeper.APIInfo;
using TMTTimeKeeper.Info;
using TMTTimeKeeper.Models;
using TMTTimeKeeper.Utilities;

namespace TMTTimeKeeper.Services
{
    public class DataLogEnrollService
    {
        private EmployeeService empService = new EmployeeService();

        public Response SaveLogDataToJson(ICollection<MachineInfo> listLog)
        {
            if (listLog == null || listLog.Count() <= 0)
            {
                return new Response() { Success = false, Errors = new List<string>() { "List bị null" } };
            }
            try
            {
                string fileName = "DatalogEnroll.json";
                string path = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\x86\Debug\netcoreapp3.1", string.Empty), @"Data\", fileName);
                File.WriteAllText(path, JsonConvert.SerializeObject(listLog));
            }
            catch (Exception e)
            {

                return new Response() { Success = false, Errors = new List<string>() { e.Message } };
            }

            return new Response() { Success = true, Message = "Đồng bộ thành công" };
        }

        public ICollection<MachineInfo> GetAllLogData(ZkemClient objZkeeper, int machineNumber)
        {
            string dwEnrollNumber1 = "";
            int dwVerifyMode = 0;
            int dwInOutMode = 0;
            int dwYear = 0;
            int dwMonth = 0;
            int dwDay = 0;
            int dwHour = 0;
            int dwMinute = 0;
            int dwSecond = 0;
            int dwWorkCode = 0;
            LastUpdateLogData log = new LastUpdateLogData();
            //Lấy lastUpdate
            string fileName = "LastGetLogData.json";
            string path = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\x86\Debug\netcoreapp3.1", string.Empty), @"Data\", fileName);
            string json = File.ReadAllText(path);

            if (!string.IsNullOrEmpty(json))
            {
                log = JsonConvert.DeserializeObject<LastUpdateLogData>(json);
            }


            ICollection<MachineInfo> lstEnrollData = new List<MachineInfo>();

            objZkeeper.ReadAllGLogData(machineNumber);


            while (objZkeeper.SSR_GetGeneralLogData(machineNumber, out dwEnrollNumber1, out dwVerifyMode, out dwInOutMode, out dwYear, out dwMonth, out dwDay, out dwHour, out dwMinute, out dwSecond, ref dwWorkCode))
            {
                string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();

                MachineInfo objInfo = new MachineInfo();
                objInfo.MachineNumber = machineNumber;
                objInfo.IndRegID = int.Parse(dwEnrollNumber1);
                objInfo.DateTimeRecord = inputDate;
                // custom
                objInfo.MyTimeOnlyRecord = DateTime.Parse(inputDate).ToString("hh:mm:ss tt");
                objInfo.dwInOutMode = dwInOutMode;

                lstEnrollData.Add(objInfo);
            }

            if (log.LastUpdate.HasValue)
            {
                lstEnrollData = lstEnrollData.Where(x => DateTime.Parse(x.DateTimeRecord) >= log.LastUpdate.Value).ToList();
                SaveLogDataToJson(lstEnrollData);
            }
            else
            {
                SaveLogDataToJson(lstEnrollData);
            }

            return lstEnrollData;
        }

        public async Task<Response> SyncLogData()
        {
            List<MachineInfo> listLogs = new List<MachineInfo>();
            AccountLogin acc = new AccountLogin();
            var listChamCongSync = new List<ChamCongSync>();
            var lastUpdate = new LastUpdateLogData();

            string logEnroll = "DatalogEnroll.json";
            string pathLogEnroll = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\x86\Debug\netcoreapp3.1", string.Empty), @"Data\", logEnroll);
            var jsonLogEnroll = File.ReadAllText(pathLogEnroll);

            string account = "AccountLogin.json";
            string pathAccountLogin = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\x86\Debug\netcoreapp3.1", string.Empty), @"Data\", account);
            var jsonAccount = File.ReadAllText(pathAccountLogin);

            string lastUpdateLog = "LastGetLogData.json";
            string pathLastUpdate = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\x86\Debug\netcoreapp3.1", string.Empty), @"Data\", lastUpdateLog);

            string fileErrorEnroll = "DataLogEnrollErrors.json";
            string pathErrorEnroll = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\x86\Debug\netcoreapp3.1", string.Empty), @"Data\", fileErrorEnroll);

            listLogs = JsonConvert.DeserializeObject<List<MachineInfo>>(jsonLogEnroll);
            acc = JsonConvert.DeserializeObject<AccountLogin>(jsonAccount);



            if (acc != null)
            {
                var url = "api/ChamCongs/SyncToTimeKeeper";

                try
                {
                    foreach (var item in listLogs)
                    {
                        ChamCongSync val = new ChamCongSync();
                        var emp = await GetEmp(item.IndRegID);
                        if (emp == null)
                            continue;
                        val.EmpId = emp.Id;
                        val.Date = item.DateOnlyRecord;
                        val.IdMayChamCong = emp.MachineNumber.ToString();
                        val.Time = DateTime.Parse(item.DateTimeRecord);
                        if (item.dwInOutMode == 1)
                            val.Type = "check-out";
                        else if (item.dwInOutMode == 0)
                            val.Type = "check-in";
                        val.WorkId = new Guid("e10bb73b-0b58-4c38-2c07-08d83dcc1e22");
                        listChamCongSync.Add(val);
                    }
                    var request = await HttpClientConfig.client.PostAsJsonAsync(url, listChamCongSync);
                    var content = await request.Content.ReadAsStringAsync();
                    var responses = JsonConvert.DeserializeObject<TimekeepingResponse>(content);
                    if (responses != null && responses.ErrorDatas != null && responses.ErrorDatas.Count() > 0)
                    {
                        File.WriteAllText(pathErrorEnroll, JsonConvert.SerializeObject(responses.ErrorDatas));
                    }
                    //Xóa logFile
                    File.WriteAllText(pathLogEnroll, string.Empty);

                    //Ghi lại last update
                    lastUpdate.Count += 1;
                    lastUpdate.LastUpdate = DateTime.Now;
                    File.WriteAllText(pathLastUpdate, JsonConvert.SerializeObject(lastUpdate));

                }
                catch (Exception e)
                {
                    return new Response() { Success = false, Errors = new List<string>() { e.Message } };
                }
            }

            return new Response() { Success = true };
        }

        public async Task<EmployeeSync> GetEmp(int idKp)
        {
            Models.EmployeeSync emp = new Models.EmployeeSync();
            var fileEmp = "Employees.json";
            var empPath = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\x86\Debug\netcoreapp3.1", string.Empty), @"Data\", fileEmp);
            var empJson = await File.ReadAllTextAsync(empPath);
            var empList = JsonConvert.DeserializeObject<List<Models.EmployeeSync>>(empJson);

            if (empList != null && empList.Any())
                emp = empList.Where(x => x.IdKP == idKp).FirstOrDefault();
            else
                emp = null;
            return emp;
        }
    }
}
