using Attendance_ZKTeco_Service.Interfaces;
using Attendance_ZKTeco_Service.Models;
using AttendanceFetch.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using System;
using System.Net;
using System.Linq;

namespace BiometricDataFetchAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserApiController : ControllerBase
    {
        private readonly IZKTecoAttendance_UserBL _zKTecoAttendance_UserBL;
        public UserApiController(IZKTecoAttendance_UserBL zKTecoAttendance_UserBL)
        {
            _zKTecoAttendance_UserBL = zKTecoAttendance_UserBL;

        }

        [HttpPost("[Action]")]
        public async Task<DataResult> SetUser([FromBody] UserInfo userInfo)
        {
            try
            {
                DataResult result = new DataResult();
                if (userInfo.AttendanceDeviceTypeId > 0)
                {
                    if (userInfo.DeviceTypeName.ToLower() == "zkteco")
                    {
                        var resultData = await _zKTecoAttendance_UserBL.SetUser(userInfo);
                        return resultData;
                    }
                    // add with new device type 
                    else
                    {
                        result = new DataResult { ResultType = ResultType.Failed, Message = "Attendance Device Type Invalid !!" };
                    }
                }
                else
                {
                    result = new DataResult { ResultType = ResultType.Failed, Message = "Attendance Device Type not found." };
                }
                return (result);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpPost("[Action]")]
        public async Task<BulkUserCreationViewModel> SetBulkUsers([FromBody] List<UserInfo> userInfoList)
        {
            try
            {
                var SuccessCount = 0;
                var FailedCount = 0;
                BulkUserCreationViewModel userCreationViewModel = new BulkUserCreationViewModel();
                List<UserInfo> sslist = new List<UserInfo>();
                List<UserInfo> fflist = new List<UserInfo>();
                //List<UserInfo> finalListToCreate = new List<UserInfo>();
                //foreach (var userInfo in userInfoList)
                //{
                //    var existingList = _zKTecoAttendance_UserBL.GetAllUserInfo(userInfo.IPAddress, userInfo.Port).Data.ToList();

                //    string[] existingEnrollmentIdList = existingList.Select(x => x.DwEnrollNumber).ToArray();
                //    var filterList = userInfoList.Where(x =>
                //           !existingEnrollmentIdList.Contains(x.DwEnrollNumber)).ToList();

                //    finalListToCreate.AddRange(filterList); 
                //}







                var existingList = _zKTecoAttendance_UserBL.GetAllUserInfo("192.168.20.19", 4370).Data.ToList();

                string[] existingEnrollmentIdList = existingList.Select(x => x.DwEnrollNumber).ToArray();
                var filterList = userInfoList.Where(x =>
                       !existingEnrollmentIdList.Contains(x.DwEnrollNumber)).ToList();



                //foreach (var userInfo in filterList)
                //{
                //    if (userInfo.AttendanceDeviceTypeId > 0)
                //    {
                //        if (userInfo.DeviceTypeName.ToLower() == "zkteco")
                //        {
                //            var resultData = await _zKTecoAttendance_UserBL.SetUser(userInfo);


                //            if (resultData.ResultType == ResultType.Success)
                //            {
                //                sslist.Add(userInfo);
                //                SuccessCount ++;
                //            }
                //            else
                //            {
                //                fflist.Add(userInfo);
                //                FailedCount ++;
                //            }
                //        }
                //        // add with new device type 
                //        //else
                //        //{
                //        //    result = new DataResult { ResultType = ResultType.Failed, Message = "Attendance Device Type Invalid !!" };
                //        //}
                //    }
                //    else
                //    {
                //        fflist.Add(userInfo);
                //        FailedCount++;
                //    }
                //}
                var result = _zKTecoAttendance_UserBL.SetBulkUsers(filterList);


                userCreationViewModel.SuccessList = sslist; 
                userCreationViewModel.FailedList = fflist; 
                userCreationViewModel.SuccessListCount = userCreationViewModel.SuccessList.Count;
                userCreationViewModel.FailedListCount = userCreationViewModel.FailedList.Count;
               
                return (userCreationViewModel);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        [HttpPost("[Action]")]
        public async Task<DataResult> DeleteUser([FromBody] UserInfo userInfo)
        {
            try
            {
                DataResult result = new DataResult();
                if (userInfo.AttendanceDeviceTypeId > 0)
                {
                    if (userInfo.DeviceTypeName.ToLower() == "zkteco")
                    {
                        var resultData = await _zKTecoAttendance_UserBL.DeleteUser(userInfo);
                        return resultData;
                    }
                    // add with new device type 
                    else
                    {
                        result = new DataResult { ResultType = ResultType.Failed, Message = "Attendance Device Type Invalid !!" };
                    }
                }
                else
                {
                    result = new DataResult { ResultType = ResultType.Failed, Message = "Attendance Device Type not found." };
                }
                return (result);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpGet("[Action]")]
        public async Task<DataResult<List<UserInfo>>> GetAllUserInfo(string IPaddress, int Port)
        {
            try
            {
                DataResult<List<UserInfo>> result = new DataResult<List<UserInfo>>();
               
                   var resultData =  _zKTecoAttendance_UserBL.GetAllUserInfo(IPaddress, Port);
                   return resultData;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        



        [HttpGet("[Action]")]
        public async Task<DataResult<UserInfo>> GetUserInfoById(int enrollmentNumber,string IPaddress, int Port)
        {
            try
            {
                DataResult<UserInfo> result = new DataResult<UserInfo>();

                var resultData = _zKTecoAttendance_UserBL.GetUserInfoById(enrollmentNumber,IPaddress,Port);
                return resultData;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpGet("[Action]")]
        public async void getexception()
        {
           
                string text = null;
                int length = text.Length;
            

        }
        //// GET api/values/5
        //[HttpPost(Name = "AttendanceFetchData")]
        //public async Task<DataResult> AttendanceFetchData([FromBody] AttendanceDevice attendanceDevice)
        //{
        //    try
        //    {
        //        DataResult result = new DataResult();
        //        if (attendanceDevice.AttendanceDeviceTypeId > 0)
        //        {
        //            if (attendanceDevice.DeviceTypeName.ToLower() == "zkteco")
        //            {
        //                try
        //                {
        //                    CZKEUEMNetClass machine = new CZKEUEMNetClass();
        //                    bool isConected = machine.Connect_Net("192.168.20.24", 4370);
        //                    if (isConected)
        //                    {

        //                        List<MachineInfo> lstEnrollData = new List<MachineInfo>();
        //                        int iMachineNumber = 1; //the device number
        //                        string idwEnrollNumber = "";
        //                        int idwVerifyMode = 0;
        //                        int idwInOutMode = 0;
        //                        int idwYear = 0;
        //                        int idwMonth = 0;
        //                        int idwDay = 0;
        //                        int idwHour = 0;
        //                        int idwMinute = 0;
        //                        int idwSecond = 0;
        //                        int idwWorkCode = 0;

        //                        string userId = "";
        //                        string userName = "";
        //                        string userCardNo = "";
        //                        string userPassword = "";
        //                        int abcdef = 1;
        //                        bool userEnabled = false;

        //                        machine.ReadAllGLogData(iMachineNumber); //read all the attendance records to the memory       \
        //                        bool absafsaf = machine.SSR_GetGeneralLogData(iMachineNumber, ref idwEnrollNumber, ref idwVerifyMode, ref idwInOutMode,
        //                            ref idwYear, ref idwMonth, ref idwDay, ref idwHour, ref idwMinute, ref idwSecond, ref idwWorkCode);
        //                        while (machine.SSR_GetGeneralLogData(iMachineNumber, ref idwEnrollNumber, ref idwVerifyMode, ref idwInOutMode,
        //                            ref idwYear, ref idwMonth, ref idwDay, ref idwHour, ref idwMinute, ref idwSecond, ref idwWorkCode)) //get attendance data one by one from memory
        //                        {

        //                            string inputDate = new DateTime(idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond).ToString("yyyy-MM-dd HH:mm:ss");
        //                            MachineInfo objInfo = new MachineInfo();
        //                            objInfo.MachineNumber = iMachineNumber;
        //                            objInfo.IndRegID = int.Parse(idwEnrollNumber);
        //                            objInfo.Mode = idwVerifyMode.ToString();
        //                            objInfo.DateTimeRecord = inputDate;
        //                            objInfo.DeviceIP = "192.168.20.24";
        //                            machine.SSR_GetUserInfo(1, Convert.ToString(objInfo.IndRegID), ref userName, ref userPassword, ref abcdef, ref userEnabled);
        //                            objInfo.Username = userName;

        //                            lstEnrollData.Add(objInfo);
        //                            //disable
        //                        }

        //                        var dadsadad = lstEnrollData;

        //                        machine.Disconnect(); //Disconnect from the device

        //                    }
        //                    return result;
        //                }
        //                catch (Exception ex)
        //                {

        //                    throw;
        //                }
        //            }
        //            // add with new device type 
        //            else
        //            {
        //                result = new DataResult { ResultType = ResultType.Failed, Message = "Attendance Device Type Invalid !!" };
        //            }
        //        }
        //        else
        //        {
        //            result = new DataResult { ResultType = ResultType.Failed, Message = "Attendance Device Type not found." };
        //        }
        //        return (result);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }

        //}
    }
}
