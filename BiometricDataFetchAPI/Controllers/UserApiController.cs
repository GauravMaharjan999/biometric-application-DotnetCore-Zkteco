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
using Attendance_ZKTeco_Service.Logics;

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
        public async Task<BulkUserCreationViewModel> SetBulkUsers(BulkUserWithDeviceInfoViewModel model)
        {
            try
            {
               return await _zKTecoAttendance_UserBL.SetBulkUsers(model); 
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

    }
}
