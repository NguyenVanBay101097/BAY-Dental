using System;
using System.Collections.Generic;
using System.Text;
using TMTTimeKeeper.Enums;
using TMTTimeKeeper.Info;
using TMTTimeKeeper.Utilities;

namespace TMTTimeKeeper.IService
{
    public interface IDeviceManipulatorService
    {
        ICollection<UserInfo> GetAllUserInfo(ZkemClient objZkeeper, int machineNumber);
        ICollection<MachineInfo> GetAllLogData(ZkemClient objZkeeper, int machineNumber);
        ICollection<MachineInfo> GetLogDataByDate(ZkemClient objZkeeper, int machineNumber, DateTime? timeIn, DateTime? timeOut);
        ICollection<UserIDInfo> GetAllUserID(ZkemClient objZkeeper, int machineNumber);
        void GetGeneratLog(ZkemClient objZkeeper, int machineNumber, string enrollNo);
        bool PushUserDataToDevice(ZkemClient objZkeeper, int machineNumber, string enrollNo);
        bool UploadFTPTemplate(ZkemClient objZkeeper, int machineNumber, List<UserInfo> lstUserInfo);
        object ClearData(ZkemClient objZkeeper, int machineNumber, ClearFlag clearFlag);
        bool ClearGLog(ZkemClient objZkeeper, int machineNumber);
        string FetchDeviceInfo(ZkemClient objZkeeper, int machineNumber);
    }
}
