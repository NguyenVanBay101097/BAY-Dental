using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using TMTTimeKeeper.Models;
using zkemkeeper;

namespace TMTTimeKeeper.Helpers
{
    public class SDKHelper
    {
        public CZKEMClass axCZKEM1 = new CZKEMClass();
        private static int iMachineNumber = 1;
        private static bool bIsConnected = false;
        private static int idwErrorCode = 0;

        public int GetMachineNumber()
        {
            return iMachineNumber;
        }

        public bool GetConnectState()
        {
            return bIsConnected;
        }

        public void SetConnectState(bool state)
        {
            bIsConnected = state;
        }

        public ReadLogResult sta_readLogByPeriod(string fromTime, string toTime)
        {
            if (GetConnectState() == false)
            {
                return new ReadLogResult
                {
                    StatusCode = -1024,
                    Message = "*Please connect first!"
                };
            }

            int ret = 0;

            //axCZKEM1.EnableDevice(GetMachineNumber(), false);//disable the device

            string sdwEnrollNumber = "";
            int idwVerifyMode = 0;
            int idwInOutMode = 0;
            int idwYear = 0;
            int idwMonth = 0;
            int idwDay = 0;
            int idwHour = 0;
            int idwMinute = 0;
            int idwSecond = 0;
            int idwWorkcode = 0;

            var logData = new List<ReadLogResultData>();
            if (axCZKEM1.ReadTimeGLogData(GetMachineNumber(), fromTime, toTime))
            {
                while (axCZKEM1.SSR_GetGeneralLogData(GetMachineNumber(), out sdwEnrollNumber, out idwVerifyMode,
                            out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idwWorkcode))//get records from the memory
                {
                    logData.Add(new ReadLogResultData
                    {
                        UserId = sdwEnrollNumber,
                        VerifyDate = idwYear + "-" + idwMonth + "-" + idwDay + " " + idwHour + ":" + idwMinute + ":" + idwSecond,
                        VerifyType = idwVerifyMode,
                        VerifyState = idwInOutMode,
                        WorkCode = idwWorkcode
                    });
                }
                ret = 1;
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                ret = idwErrorCode;

                if (idwErrorCode != 0)
                {
                    return new ReadLogResult
                    {
                        StatusCode = idwErrorCode,
                        Message = "*Read attlog by period failed"
                    };
                }
                else
                {
                    return new ReadLogResult
                    {
                        StatusCode = idwErrorCode,
                        Message = "No data from terminal returns!"
                    };
                }
            }

            //axCZKEM1.EnableDevice(GetMachineNumber(), true);//enable the device

            return new ReadLogResult 
            { 
                StatusCode = ret,
                Data = logData
            };
        }

        public ReadLogResult sta_readAllLog()
        {
            if (GetConnectState() == false)
            {
                return new ReadLogResult
                {
                    StatusCode = -1024,
                    Message = "*Please connect first!"
                };
            }

            int ret = 0;

            //axCZKEM1.EnableDevice(GetMachineNumber(), false);//disable the device

            string sdwEnrollNumber = "";
            int idwVerifyMode = 0;
            int idwInOutMode = 0;
            int idwYear = 0;
            int idwMonth = 0;
            int idwDay = 0;
            int idwHour = 0;
            int idwMinute = 0;
            int idwSecond = 0;
            int idwWorkcode = 0;

            var logData = new List<ReadLogResultData>();
            if (axCZKEM1.ReadAllGLogData(GetMachineNumber()))
            {
                while (axCZKEM1.SSR_GetGeneralLogData(GetMachineNumber(), out sdwEnrollNumber, out idwVerifyMode,
                            out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idwWorkcode))//get records from the memory
                {
                    logData.Add(new ReadLogResultData
                    {
                        UserId = sdwEnrollNumber,
                        VerifyDate = idwYear + "-" + idwMonth + "-" + idwDay + " " + idwHour + ":" + idwMinute + ":" + idwSecond,
                        VerifyType = idwVerifyMode,
                        VerifyState = idwInOutMode,
                        WorkCode = idwWorkcode
                    });
                }
                ret = 1;
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                ret = idwErrorCode;

                if (idwErrorCode != 0)
                {
                    return new ReadLogResult
                    {
                        StatusCode = idwErrorCode,
                        Message = "*Read attlog by period failed"
                    };
                }
                else
                {
                    return new ReadLogResult
                    {
                        StatusCode = idwErrorCode,
                        Message = "No data from terminal returns!"
                    };
                }
            }

            //axCZKEM1.EnableDevice(GetMachineNumber(), true);//enable the device

            return new ReadLogResult
            {
                StatusCode = ret,
                Data = logData
            };
        }

        public int sta_ConnectTCP(string ip, string port)
        {
            if (ip == "" || port == "")
            {
                return -1;// ip or port is null
            }

            if (Convert.ToInt32(port) <= 0 || Convert.ToInt32(port) > 65535)
            {
                return -1;
            }

            int idwErrorCode = 0;

            if (bIsConnected == true)
            {
                axCZKEM1.Disconnect();
                //connected = false;
                bIsConnected = false;
                return -2; //disconnect
            }

            if (axCZKEM1.Connect_Net(ip, Convert.ToInt32(port)) == true)
            {
                SetConnectState(true);
                return 1;
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                return idwErrorCode;
            }
        }

        public int sta_GetDeviceInfo(out string sFirmver, out string sMac, out string sPlatform, out string sSN, out string sProductTime, out string sDeviceName, out int iFPAlg, out int iFaceAlg, out string sProducter)
        {
            int iRet = 0;

            sFirmver = "";
            sMac = "";
            sPlatform = "";
            sSN = "";
            sProducter = "";
            sDeviceName = "";
            iFPAlg = 0;
            iFaceAlg = 0;
            sProductTime = "";
            string strTemp = "";

            if (GetConnectState() == false)
            {
                return -1024;
            }

            axCZKEM1.EnableDevice(GetMachineNumber(), false);//disable the device

            axCZKEM1.GetSysOption(GetMachineNumber(), "~ZKFPVersion", out strTemp);
            iFPAlg = Convert.ToInt32(strTemp);

            axCZKEM1.GetSysOption(GetMachineNumber(), "ZKFaceVersion", out strTemp);
            iFaceAlg = Convert.ToInt32(strTemp);

            /*
            axCZKEM1.GetDeviceInfo(GetMachineNumber(), 72, ref iFPAlg);
            axCZKEM1.GetDeviceInfo(GetMachineNumber(), 73, ref iFaceAlg);
            */

            axCZKEM1.GetVendor(ref sProducter);
            axCZKEM1.GetProductCode(GetMachineNumber(), out sDeviceName);
            axCZKEM1.GetDeviceMAC(GetMachineNumber(), ref sMac);
            axCZKEM1.GetFirmwareVersion(GetMachineNumber(), ref sFirmver);

            /*
            if (sta_GetDeviceType() == 1)
            {
                axCZKEM1.GetDeviceFirmwareVersion(GetMachineNumber(), ref sFirmver);
            }
             */
            //lblOutputInfo.Items.Add("[func GetDeviceFirmwareVersion]Temporarily unsupported");

            axCZKEM1.GetPlatform(GetMachineNumber(), ref sPlatform);
            axCZKEM1.GetSerialNumber(GetMachineNumber(), out sSN);
            axCZKEM1.GetDeviceStrInfo(GetMachineNumber(), 1, out sProductTime);

            axCZKEM1.EnableDevice(GetMachineNumber(), true);//enable the device

            iRet = 1;
            return iRet;
        }
    }
}
