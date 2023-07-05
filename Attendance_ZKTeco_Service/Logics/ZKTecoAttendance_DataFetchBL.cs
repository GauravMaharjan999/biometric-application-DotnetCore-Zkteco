using Attendance_ZKTeco_Service.Interfaces;
using Attendance_ZKTeco_Service.Models;
using AttendanceFetch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ZkSoftwareEU;

namespace Attendance_ZKTeco_Service.Logics
{
    public class ZKTecoAttendance_DataFetchBL : IZKTecoAttendance_DataFetchBL
    {
        public ZKTecoAttendance_DataFetchBL()
        {

        }
        public async Task<DataResult<List<MachineInfo>>> GetData_Zkteco(AttendanceDevice model)
        {
            DeviceManipulator manipulator = new DeviceManipulator();
            try
            {
                //Validation
                if (model.IPAddress == string.Empty || model.Port <= 0)
                {
                    return new DataResult<List<MachineInfo>> { ResultType = ResultType.Failed, Message = "The Device IP Address or Port is mandotory !!" };
                }

                bool Isconnected = manipulator.Connect_device(model.IPAddress, model.Port);
                if (!Isconnected)
                {
                    return new DataResult<List<MachineInfo>> { ResultType = ResultType.Failed, Message = "Device not Connected!!" };
                }
                try
                {
                    List<MachineInfo> lstEnrollData = new List<MachineInfo>();

                    CZKEUEMNetClass machine1 = new CZKEUEMNetClass();
                    bool isConected = machine1.Connect_Net(model.IPAddress, model.Port);
                    if (isConected)
                    {
                        //machine.SSR_SetUserInfo(1, "866", "Gaurav Maharjan", "gaurav866", 0, true); // User Set


                        #region DirectFetch

                        //int iMachineNumber = 1; //the device number
                        //string idwEnrollNumber = "";
                        //int idwVerifyMode = 0;
                        //int idwInOutMode = 0;
                        //int idwYear = 0;
                        //int idwMonth = 0;
                        //int idwDay = 0;
                        //int idwHour = 0;
                        //int idwMinute = 0;
                        //int idwSecond = 0;
                        //int idwWorkCode = 0;

                        //string userId = "";
                        //string userName = "";
                        //string userCardNo = "";
                        //string userPassword = "";
                        //int abcdef = 1;
                        //bool userEnabled = false;

                        //machine1.ReadAllGLogData(iMachineNumber); //read all the attendance records to the memory       \
                        //bool absafsaf = machine1.SSR_GetGeneralLogData(iMachineNumber, ref idwEnrollNumber, ref idwVerifyMode, ref idwInOutMode,
                        //    ref idwYear, ref idwMonth, ref idwDay, ref idwHour, ref idwMinute, ref idwSecond, ref idwWorkCode);
                        //while (machine1.SSR_GetGeneralLogData(iMachineNumber, ref idwEnrollNumber, ref idwVerifyMode, ref idwInOutMode,
                        //    ref idwYear, ref idwMonth, ref idwDay, ref idwHour, ref idwMinute, ref idwSecond, ref idwWorkCode)) //get attendance data one by one from memory
                        //{

                        //    string inputDate = new DateTime(idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond).ToString("yyyy-MM-dd HH:mm:ss");
                        //    MachineInfo objInfo = new MachineInfo();
                        //    objInfo.MachineNumber = iMachineNumber;
                        //    objInfo.IndRegID = int.Parse(idwEnrollNumber);
                        //    objInfo.Mode = idwVerifyMode.ToString();
                        //    objInfo.DateTimeRecord = inputDate;
                        //    objInfo.DeviceIP = "192.168.20.24";
                        //    machine1.SSR_GetUserInfo(1, Convert.ToString(objInfo.IndRegID), ref userName, ref userPassword, ref abcdef, ref userEnabled);
                        //    objInfo.Username = userName;

                        //    lstEnrollData.Add(objInfo); 
                        //}
                        #endregion
                        //disable

                    }
                    List<MachineInfo> data = manipulator.GetLogData(model.DeviceMachineNo, model.IPAddress,model.Port);
                    if (data.Count > 0)
                    {
                        #region Push  Attendance Log Data to HR Server
                        //
                        //DataResult result = PushData(data, model.Id, model.StatusChgUserId, model.ClientAlias);
                        //if (result.ResultType == ResultType.Success)
                        //{
                        //    //manipulator.ClearGLog(model.DeviceMachineNo, model.IPAddress);
                        //    return new DataResult { ResultType = ResultType.Success, Message = "Data Pull and Push Success!!", dataRow = data };
                        //}
                        //else
                        //{
                        //    manipulator.Enable_Device(model.DeviceMachineNo);
                        //    manipulator.Disconnect();
                        //    return new DataResult { ResultType = ResultType.Failed, Message = "Data Push Failed on Application!!" };
                        //}
                        #endregion

                        return new DataResult<List<MachineInfo>> { ResultType = ResultType.Success, Message = "Data Retrived from Attendance Device Successfully!",Data=data.ToList() };
                    }
                    else
                    {
                        //manipulator.ClearGLog(model.DeviceMachineNo, model.IPAddress);
                        return new DataResult<List<MachineInfo>> { ResultType = ResultType.Failed, Message = "No any data found to pull from Attendance Device!!" };
                    }
                }
                catch (Exception ex)
                {
                    return new DataResult<List<MachineInfo>> { ResultType = ResultType.Failed, Message = $"Data pull failed from attendance device!! {ex.Message}" };
                };
            }
            catch (Exception ex)
            {
                return new DataResult<List<MachineInfo>> { ResultType = ResultType.Failed, Message = $"Device not connected!! {ex.Message}." };
            };
        }


        public async Task<DataResult<List<MachineInfo>>> DeleteData_Zkteco(AttendanceDevice model)
        {
            DeviceManipulator manipulator = new DeviceManipulator();
            try
            {
                //Validation
                if (model.IPAddress == string.Empty || model.Port <= 0)
                {
                    return new DataResult<List<MachineInfo>> { ResultType = ResultType.Failed, Message = "The Device IP Address or Port is mandotory !!" };
                }

                bool Isconnected = manipulator.Connect_device(model.IPAddress, model.Port);
                if (!Isconnected)
                {
                    return new DataResult<List<MachineInfo>> { ResultType = ResultType.Failed, Message = "Device not Connected!!" };
                }
                try
                {
                    List<MachineInfo> lstEnrollData = new List<MachineInfo>();

                    CZKEUEMNetClass machine1 = new CZKEUEMNetClass();
                    bool isConected = machine1.Connect_Net(model.IPAddress, model.Port);
                    if (isConected)
                    {
                        //machine.SSR_SetUserInfo(1, "866", "Gaurav Maharjan", "gaurav866", 0, true); // User Set


                        #region DirectFetch

                        //int iMachineNumber = 1; //the device number
                        //string idwEnrollNumber = "";
                        //int idwVerifyMode = 0;
                        //int idwInOutMode = 0;
                        //int idwYear = 0;
                        //int idwMonth = 0;
                        //int idwDay = 0;
                        //int idwHour = 0;
                        //int idwMinute = 0;
                        //int idwSecond = 0;
                        //int idwWorkCode = 0;

                        //string userId = "";
                        //string userName = "";
                        //string userCardNo = "";
                        //string userPassword = "";
                        //int abcdef = 1;
                        //bool userEnabled = false;

                        //machine1.ReadAllGLogData(iMachineNumber); //read all the attendance records to the memory       \
                        //bool absafsaf = machine1.SSR_GetGeneralLogData(iMachineNumber, ref idwEnrollNumber, ref idwVerifyMode, ref idwInOutMode,
                        //    ref idwYear, ref idwMonth, ref idwDay, ref idwHour, ref idwMinute, ref idwSecond, ref idwWorkCode);
                        //while (machine1.SSR_GetGeneralLogData(iMachineNumber, ref idwEnrollNumber, ref idwVerifyMode, ref idwInOutMode,
                        //    ref idwYear, ref idwMonth, ref idwDay, ref idwHour, ref idwMinute, ref idwSecond, ref idwWorkCode)) //get attendance data one by one from memory
                        //{

                        //    string inputDate = new DateTime(idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond).ToString("yyyy-MM-dd HH:mm:ss");
                        //    MachineInfo objInfo = new MachineInfo();
                        //    objInfo.MachineNumber = iMachineNumber;
                        //    objInfo.IndRegID = int.Parse(idwEnrollNumber);
                        //    objInfo.Mode = idwVerifyMode.ToString();
                        //    objInfo.DateTimeRecord = inputDate;
                        //    objInfo.DeviceIP = "192.168.20.24";
                        //    machine1.SSR_GetUserInfo(1, Convert.ToString(objInfo.IndRegID), ref userName, ref userPassword, ref abcdef, ref userEnabled);
                        //    objInfo.Username = userName;

                        //    lstEnrollData.Add(objInfo); 
                        //}
                        #endregion
                        //disable

                    }
                    bool result  = manipulator.ClearLogData(model.DeviceMachineNo, model.IPAddress);
                    if (result==true)
                    {
                        return new DataResult<List<MachineInfo>> { ResultType = ResultType.Success, Message = "Data Deleted from Attendance Device Successfully!"};
                    }
                    else
                    {
                        //manipulator.ClearGLog(model.DeviceMachineNo, model.IPAddress);
                        return new DataResult<List<MachineInfo>> { ResultType = ResultType.Failed, Message = "No any data found to pull from Attendance Device!!" };
                    }
                }
                catch (Exception ex)
                {
                    return new DataResult<List<MachineInfo>> { ResultType = ResultType.Failed, Message = $"Data pull failed from attendance device!! {ex.Message}" };
                };
            }
            catch (Exception ex)
            {
                return new DataResult<List<MachineInfo>> { ResultType = ResultType.Failed, Message = $"Device not connected!! {ex.Message}." };
            };
        }




    }
}
