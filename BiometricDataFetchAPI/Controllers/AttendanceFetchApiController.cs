using Attendance_ZKTeco_Service.Interfaces;
using Attendance_ZKTeco_Service.Models;
using AttendanceFetch.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZkSoftwareEU;

namespace BiometricDataFetchAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceFetchApiController : ControllerBase
    {
        private readonly IZKTecoAttendance_DataFetchBL _zKTecoAttendanceDataFetchBL;
        public AttendanceFetchApiController(IZKTecoAttendance_DataFetchBL zKTecoAttendanceDataFetchBL)
        {
            _zKTecoAttendanceDataFetchBL = zKTecoAttendanceDataFetchBL;
            
        }

        [HttpPost("[Action]")]  
        public async Task<DataResult<List<MachineInfo>>> GetData([FromBody] AttendanceDevice attendanceDevice)
        {
            try
            {
                DataResult<List<MachineInfo>> result = new DataResult<List<MachineInfo>>();
                if (attendanceDevice.AttendanceDeviceTypeId > 0)
                {
                    if (attendanceDevice.DeviceTypeName.ToLower() == "zkteco")
                    {
                        var resultData = await _zKTecoAttendanceDataFetchBL.GetData_Zkteco(attendanceDevice);
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

    }
}
