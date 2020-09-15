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
        private TimeKeeperService timekeeperObj = new TimeKeeperService();
        public Response SaveLogDataToJson(ICollection<MachineInfo> listLog)
        {
            if (listLog == null || listLog.Count() <= 0)
            {
                return new Response() { Success = false, Errors = new List<string>() { "List bị null" } };
            }
            try
            {
                string fileName = "DatalogEnroll.json";
                timekeeperObj.SetListJson<MachineInfo>(fileName, listLog);
            }
            catch (Exception e)
            {

                return new Response() { Success = false, Errors = new List<string>() { e.Message } };
            }

            return new Response() { Success = true, Message = "Đồng bộ thành công" };
        }

        public async Task<ICollection<MachineInfo>> GetAllLogData(ZkemClient objZkeeper, int machineNumber)
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
            string fileLastUpdate = "LastGetLogData.json";
            string fileEnrollData = "DataLogEnroll.json";
            log = await timekeeperObj.GetModelByJson<LastUpdateLogData>(fileLastUpdate);

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
                objInfo.Type = dwInOutMode == 1 ? "check-out" : (dwInOutMode == 0 ? "check-in" : "");
                objInfo.Status = "Chưa đồng bộ";
                lstEnrollData.Add(objInfo);
            }

            if (log != null && log.LastUpdate.HasValue)
            {
                lstEnrollData = lstEnrollData.Where(x => DateTime.Parse(x.DateTimeRecord) >= log.LastUpdate.Value).ToList();
                timekeeperObj.SetListJson<MachineInfo>(fileEnrollData, lstEnrollData.ToList());
            }
            else
            {
                timekeeperObj.SetListJson<MachineInfo>(fileEnrollData, lstEnrollData.ToList());
            }

            return lstEnrollData;
        }

        public async Task<Response> SyncLogData()
        {
            var listLogs = new List<MachineInfo>();
            AccountLogin acc = new AccountLogin();
            Response res = new Response();
            var listChamCongSync = new List<ChamCongSync>();
            var lastUpdate = new LastUpdateLogData();
            string lastUpdateLog = "LastGetLogData.json";
            var url = "api/ChamCongs/SyncToTimeKeeper";
            string fileErrorEnroll = "DataLogEnrollErrors.json";
            string logEnroll = "DatalogEnroll.json";
            listLogs = (await timekeeperObj.GetListModelByJson<MachineInfo>(logEnroll)).ToList();
            string account = "AccountLogin.json";
            acc = await timekeeperObj.GetModelByJson<AccountLogin>(account);
            if (acc != null)
            {
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
                        timekeeperObj.SetListJson<Response>(fileErrorEnroll, responses.ErrorDatas);
                    }
                    //Xóa logFile
                    timekeeperObj.SetJson<DataLogEnroll>(logEnroll, null);

                    //Ghi lại last update                  
                    lastUpdate.LastUpdate = DateTime.Now;
                    timekeeperObj.SetJson<LastUpdateLogData>(lastUpdateLog, lastUpdate);
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
            EmployeeSync emp = new EmployeeSync();
            var fileEmp = "Employees.json";
            var listEmp = await timekeeperObj.GetListModelByJson<EmployeeSync>(fileEmp);
            if (listEmp != null && listEmp.Any())
                emp = listEmp.Where(x => x.IdKP == idKp).FirstOrDefault();
            else
                emp = null;
            return emp;
        }
    }
}
