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
using TMTTimeKeeper.Enums;
using TMTTimeKeeper.Info;
using TMTTimeKeeper.Models;

namespace TMTTimeKeeper.Utilities
{
    internal class DeviceManipulator
    {
        public ICollection<UserInfo> GetAllUserInfo(ZkemClient objZkeeper, int machineNumber)
        {
            string sdwEnrollNumber = string.Empty, sName = string.Empty, sPassword = string.Empty, sTmpData = string.Empty;
            int iPrivilege = 0, iTmpLength = 0, iFlag = 0, idwFingerIndex;
            bool bEnabled = false;

            ICollection<UserInfo> lstFPTemplates = new List<UserInfo>();

            objZkeeper.ReadAllUserID(machineNumber);
            objZkeeper.ReadAllTemplate(machineNumber);

            while (objZkeeper.SSR_GetAllUserInfo(machineNumber, out sdwEnrollNumber, out sName, out sPassword, out iPrivilege, out bEnabled))
            {

                UserInfo fpInfo = new UserInfo();
                fpInfo.MachineNumber = machineNumber;
                fpInfo.EnrollNumber = sdwEnrollNumber;
                fpInfo.Name = sName;
                fpInfo.TmpData = sTmpData;
                fpInfo.Privelage = iPrivilege;
                fpInfo.Password = sPassword;
                fpInfo.Enabled = bEnabled;
                fpInfo.iFlag = iFlag.ToString();

                lstFPTemplates.Add(fpInfo);


            }
            return lstFPTemplates;
        }

        public ICollection<MachineInfo> GetLogDataByDate(ZkemClient objZkeeper, int machineNumber, DateTime? timeIn, DateTime? timeOut)
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


            var res = new List<MachineInfo>();
            if (timeIn.HasValue)
                res = lstEnrollData.Where(x => x.DateOnlyRecord >= timeIn.Value).ToList();
            if (timeOut.HasValue)
                res = res.Where(x => x.DateOnlyRecord <= timeOut.Value).ToList();

            return res;
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
                        var emp = GetEmp(item.IndRegID);
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

        public Models.EmployeeSync GetEmp(int idKp)
        {
            Models.EmployeeSync emp = new Models.EmployeeSync();
            var fileEmp = "Employees.json";
            var empPath = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\x86\Debug\netcoreapp3.1", string.Empty), @"Data\", fileEmp);
            var empJson = File.ReadAllText(empPath);
            var empList = JsonConvert.DeserializeObject<List<Models.EmployeeSync>>(empJson);

            if (empList != null && empList.Any())
                emp = empList.Where(x => x.IdKP == idKp).FirstOrDefault();
            else
                emp = null;
            return emp;
        }

        public ICollection<UserIDInfo> GetAllUserID(ZkemClient objZkeeper, int machineNumber)
        {
            int dwEnrollNumber = 0;
            int dwEMachineNumber = 1;
            int dwBackUpNumber = 0;
            int dwMachinePrivelage = 0;
            int dwEnabled = 0;

            ICollection<UserIDInfo> lstUserIDInfo = new List<UserIDInfo>();

            while (objZkeeper.GetAllUserID(machineNumber, ref dwEnrollNumber, ref dwEMachineNumber, ref dwBackUpNumber, ref dwMachinePrivelage, ref dwEnabled))
            {
                UserIDInfo userID = new UserIDInfo();
                userID.BackUpNumber = dwBackUpNumber;
                userID.Enabled = dwEnabled;
                userID.EnrollNumber = dwEnrollNumber;
                userID.MachineNumber = dwEMachineNumber;
                userID.Privelage = dwMachinePrivelage;
                lstUserIDInfo.Add(userID);
            }
            return lstUserIDInfo;
        }

        public void GetGeneratLog(ZkemClient objZkeeper, int machineNumber, string enrollNo)
        {
            string name = null;
            string password = null;
            int previlage = 0;
            bool enabled = false;
            byte[] byTmpData = new byte[2000];
            int tempLength = 0;

            int idwFingerIndex = 0;// [ <--- Enter your fingerprint index here ]
            int iFlag = 0;

            objZkeeper.ReadAllTemplate(machineNumber);

            while (objZkeeper.SSR_GetUserInfo(machineNumber, enrollNo, out name, out password, out previlage, out enabled))
            {
                if (objZkeeper.GetUserTmpEx(machineNumber, enrollNo, idwFingerIndex, out iFlag, out byTmpData[0], out tempLength))
                {
                    break;
                }
            }
        }

        public bool PushUserDataToDevice(ZkemClient objZkeeper, int machineNumber, string enrollNo, string Name)
        {
            string password = string.Empty;
            int privelage = 1;
            return objZkeeper.SSR_SetUserInfo(machineNumber, enrollNo, Name, password, privelage, true);
        }

        public bool UploadFTPTemplate(ZkemClient objZkeeper, int machineNumber, List<UserInfo> lstUserInfo)
        {
            string sdwEnrollNumber = string.Empty, sName = string.Empty, sTmpData = string.Empty;
            int idwFingerIndex = 0, iPrivilege = 0, iFlag = 1, iUpdateFlag = 1;
            string sPassword = "";
            string sEnabled = "";
            bool bEnabled = false;

            if (objZkeeper.BeginBatchUpdate(machineNumber, iUpdateFlag))
            {
                string sLastEnrollNumber = "";

                for (int i = 0; i < lstUserInfo.Count; i++)
                {
                    sdwEnrollNumber = lstUserInfo[i].EnrollNumber;
                    sName = lstUserInfo[i].Name;
                    idwFingerIndex = lstUserInfo[i].FingerIndex;
                    sTmpData = lstUserInfo[i].TmpData;
                    iPrivilege = lstUserInfo[i].Privelage;
                    sPassword = lstUserInfo[i].Password;
                    sEnabled = lstUserInfo[i].Enabled.ToString();
                    iFlag = Convert.ToInt32(lstUserInfo[i].iFlag);
                    bEnabled = true;

                    /* [ Identify whether the user 
                         information(except fingerprint templates) has been uploaded */

                    if (sdwEnrollNumber != sLastEnrollNumber)
                    {
                        if (objZkeeper.SSR_SetUserInfo(machineNumber, sdwEnrollNumber, sName, sPassword, iPrivilege, bEnabled))//upload user information to the memory
                            objZkeeper.SetUserTmpExStr(machineNumber, sdwEnrollNumber, idwFingerIndex, iFlag, sTmpData);//upload templates information to the memory
                        else return false;
                    }
                    else
                    {
                        /* [ The current fingerprint and the former one belongs the same user,
                        i.e one user has more than one template ] */
                        objZkeeper.SetUserTmpExStr(machineNumber, sdwEnrollNumber, idwFingerIndex, iFlag, sTmpData);
                    }

                    sLastEnrollNumber = sdwEnrollNumber;
                }

                return true;
            }
            else
                return false;
        }

        public object ClearData(ZkemClient objZkeeper, int machineNumber, ClearFlag clearFlag)
        {
            int iDataFlag = (int)clearFlag;

            if (objZkeeper.ClearData(machineNumber, iDataFlag))
                return objZkeeper.RefreshData(machineNumber);
            else
            {
                int idwErrorCode = 0;
                objZkeeper.GetLastError(ref idwErrorCode);
                return idwErrorCode;
            }
        }

        public bool ClearGLog(ZkemClient objZkeeper, int machineNumber)
        {
            return objZkeeper.ClearGLog(machineNumber);
        }

        public string FetchDeviceInfo(ZkemClient objZkeeper, int machineNumber)
        {
            StringBuilder sb = new StringBuilder();

            string returnValue = string.Empty;


            objZkeeper.GetFirmwareVersion(machineNumber, ref returnValue);
            if (returnValue.Trim() != string.Empty)
            {
                sb.Append("Firmware V: ");
                sb.Append(returnValue);
                sb.Append(",");
            }


            returnValue = string.Empty;
            objZkeeper.GetVendor(ref returnValue);
            if (returnValue.Trim() != string.Empty)
            {
                sb.Append("Vendor: ");
                sb.Append(returnValue);
                sb.Append(",");
            }

            string sWiegandFmt = string.Empty;
            objZkeeper.GetWiegandFmt(machineNumber, ref sWiegandFmt);

            returnValue = string.Empty;
            objZkeeper.GetSDKVersion(ref returnValue);
            if (returnValue.Trim() != string.Empty)
            {
                sb.Append("SDK V: ");
                sb.Append(returnValue);
                sb.Append(",");
            }

            returnValue = string.Empty;
            objZkeeper.GetSerialNumber(machineNumber, out returnValue);
            if (returnValue.Trim() != string.Empty)
            {
                sb.Append("Serial No: ");
                sb.Append(returnValue);
                sb.Append(",");
            }

            returnValue = string.Empty;
            objZkeeper.GetDeviceMAC(machineNumber, ref returnValue);
            if (returnValue.Trim() != string.Empty)
            {
                sb.Append("Device MAC: ");
                sb.Append(returnValue);
            }

            return sb.ToString();
        }

        public ICollection<MachineInfo> GetLogData(ZkemClient objZkeeper, int machineNumber)
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

            ICollection<MachineInfo> lstEnrollData = new List<MachineInfo>();
            objZkeeper.ReadTimeGLogData(machineNumber, "2020-09-15 00:00:00", "2020-09-15 23:59:00");
            objZkeeper.ReadAllGLogData(machineNumber);

            while (objZkeeper.SSR_GetGeneralLogData(machineNumber, out dwEnrollNumber1, out dwVerifyMode, out dwInOutMode, out dwYear, out dwMonth, out dwDay, out dwHour, out dwMinute, out dwSecond, ref dwWorkCode))
            {
                string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();

                MachineInfo objInfo = new MachineInfo();
                objInfo.MachineNumber = machineNumber;
                objInfo.IndRegID = int.Parse(dwEnrollNumber1);
                objInfo.DateTimeRecord = inputDate;

                lstEnrollData.Add(objInfo);
            }

            return lstEnrollData;
        }
    }
}
