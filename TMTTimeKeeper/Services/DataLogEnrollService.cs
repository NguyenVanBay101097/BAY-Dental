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

        public async Task<ResponseDataLogViewModel> SyncLogData(ReadLogResult model)
        {
            try
            {
                var url = "api/ChamCongs/SyncToTimeKeeper";
                var request = await HttpClientConfig.client.PostAsJsonAsync(url, model.Data);
                var content = await request.Content.ReadAsStringAsync();
                var responses = JsonConvert.DeserializeObject<ResponseDataLogViewModel>(content);
                return responses;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}
