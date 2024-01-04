using Attendance_ZKTeco_Service.Interfaces;
using AttendanceFetch.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Attendance_ZKTeco_Service.Logics;

namespace BiometricDataFetchAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceStateMonitoringController : Controller
    {
        private readonly IZKTecoAttendance_UserBL _zKTecoAttendance_UserBL;
        private readonly IZKTecoAttendance_DataFetchBL zKTecoAttendance_DataFetchBL;
        public DeviceStateMonitoringController(IZKTecoAttendance_UserBL zKTecoAttendance_UserBL, IZKTecoAttendance_DataFetchBL zKTecoAttendance_DataFetchBL)
        {
            _zKTecoAttendance_UserBL = zKTecoAttendance_UserBL;
            this.zKTecoAttendance_DataFetchBL = zKTecoAttendance_DataFetchBL;
        }

        [HttpGet("[Action]")]
        public async Task<DataResult> PingDevice(string IPAddress, int Port = 4370)
        {

            try
            {
                return await zKTecoAttendance_DataFetchBL.PingDevice(IPAddress, Port);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        [HttpGet("[Action]")]
        public async Task<DataResult> CheckDeviceConnectionStatus([FromQuery] string IPAddress, [FromQuery] int Port = 4370)
        {

            try
            {
                return await zKTecoAttendance_DataFetchBL.CheckDeviceConnectionStatus(IPAddress, Port);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpGet("[Action]")]
        public async Task<DataResult> SetDeviceTime(string IpAddress, int port, DateTime dateTime)
        {
            try
            {
                return await _zKTecoAttendance_UserBL.SetDeviceTime(IpAddress, port, dateTime);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpGet("[Action]")]
        public DataResult<string> GetDeviceTime(string IpAddress, int port)
        {
            try
            {
                return _zKTecoAttendance_UserBL.GetDeviceTime(IpAddress, port);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

    }
}
