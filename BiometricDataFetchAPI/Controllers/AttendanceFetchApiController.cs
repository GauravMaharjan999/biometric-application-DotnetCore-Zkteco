using Attendance_ZKTeco_Service.Interfaces;
using Attendance_ZKTeco_Service.Models;
using AttendanceFetch.Models;
//using BiometricDataFetchAPI.BusinessLogic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BiometricDataFetchAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceFetchApiController : ControllerBase
    {
        private readonly IZKTecoAttendance_DataFetchBL _zKTecoAttendanceDataFetchBL;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AttendanceFetchApiController> _logger;
        //private readonly IAttendanceFetchApiBL _attendanceFetchApiBL;
        public AttendanceFetchApiController(IZKTecoAttendance_DataFetchBL zKTecoAttendanceDataFetchBL, IConfiguration configuration, ILogger<AttendanceFetchApiController> logger
            //, IAttendanceFetchApiBL attendanceFetchApiBL
            )
        {
            _zKTecoAttendanceDataFetchBL = zKTecoAttendanceDataFetchBL;
            _configuration = configuration;
            _logger = logger;
            //_attendanceFetchApiBL = attendanceFetchApiBL;
        }

        [HttpPost("[Action]")]  
        public async Task<DataResult<List<MachineInfo>>> GetData([FromBody] AttendanceDevice attendanceDevice)
        {
            
            try
            {

                DataResult<List<MachineInfo>> result = new DataResult<List<MachineInfo>>();
                if (attendanceDevice.AttendanceDeviceTypeId > 0)
                {
                    if (attendanceDevice.DeviceTypeName.ToLower().Equals("zkteco"))
                    {
                        var resultData = await _zKTecoAttendanceDataFetchBL.GetData_Zkteco(attendanceDevice);

                        //_logger.LogInformation($"GetData - Successfully Attendance Data Retieved,total data: {resultData.Data.Count}");
                        return resultData;

                    }
                    // add with new device type 
                    else
                    {
                        result = new DataResult<List<MachineInfo>> { ResultType = ResultType.Failed, Message = "Attendance Device Type Invalid !!" };
                    }
                }
                else
                {
                    result = new DataResult<List<MachineInfo>> { ResultType = ResultType.Failed, Message = "Attendance Device Type not found." };
                }
                return (result);
            }
            catch (Exception ex)
            {

                throw;
            }

        }


        [HttpPost("[Action]")]
        public async Task<DataResult> DeleteData([FromBody] AttendanceDevice attendanceDevice)
        {
            try
            {
                if (attendanceDevice.AttendanceDeviceTypeId > 0)
                {
                    if (attendanceDevice.DeviceTypeName.ToLower() == "zkteco")
                    {
                        return await _zKTecoAttendanceDataFetchBL.DeleteData_Zkteco(attendanceDevice); ;
                    }
                    // add with new device type 
                    else
                    {
                        return new DataResult { ResultType = ResultType.Failed, Message = "Attendance Device Type Invalid !!" };
                    }
                }
                else
                {
                    return new DataResult { ResultType = ResultType.Failed, Message = "Attendance Device Type not found." };
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpPost("[Action]")]
        public async Task<DataResult> CheckDeviceConnectionStatus([FromQuery] string IPAddress, [FromQuery] int Port=4370)
        {

            try
            {
                return await _zKTecoAttendanceDataFetchBL.CheckDeviceConnectionStatus(IPAddress, Port);
            }
            catch (Exception ex)
            {

                throw;
            }

        }


        //[HttpPost("[Action]")]
        //public async Task<DataResult<List<MachineInfo>>> GetTestData()
        //{
        //    try
        //    {

        //      return new DataResult<List<MachineInfo>> { ResultType = ResultType.Failed, Message = "Attendance Device Type Invalid !!" };
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }

        //}


        //[HttpGet("[Action]")]
        //public async Task<DataResult<List<BranchDeviceTaggingViewModel>>> GetBranchListDataFromMainServer()
        //{
        //    try
        //    {
        //        return await _attendanceFetchApiBL.GetBranchListDataFromMainServer();



        //    }
        //    catch (Exception ex)
        //    {

        //        return new DataResult<List<BranchDeviceTaggingViewModel>> { ResultType = ResultType.Exception, Message = ex.Message };

        //    }

        //}



        //[HttpGet("[Action]")]
        //public async Task<DataResult<MachineInfoViewModel>> PushToMainServer()
        //{
        //    try
        //    {
        //        return await _attendanceFetchApiBL.PushToMainServer();

        //    }
        //    catch (Exception ex)
        //    {

        //        return new DataResult<MachineInfoViewModel> { ResultType = ResultType.Exception, Message = ex.Message };

        //    }

        //}

    }
}
