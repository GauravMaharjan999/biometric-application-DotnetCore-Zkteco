using Attendance_ZKTeco_Service.Interfaces;
using Attendance_ZKTeco_Service.Models;
using AttendanceFetch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZkSoftwareEU;

namespace Attendance_ZKTeco_Service.Logics
{
    public class JobSchedular : IJobSchedular
    {
        private readonly IZKTecoAttendance_DataFetchBL _zKTecoAttendanceDataFetchBL;

        public JobSchedular(IZKTecoAttendance_DataFetchBL zKTecoAttendanceDataFetchBL)
        {
            _zKTecoAttendanceDataFetchBL = zKTecoAttendanceDataFetchBL;
        }
        public async Task ScheduleAsyncAutoGetAttendance()
        {
            AttendanceDevice attendanceDevice = new AttendanceDevice();
            attendanceDevice.Id = 1;
            attendanceDevice.DeviceTypeName = "Zkteco";
            attendanceDevice.AttendanceDeviceTypeId = 1;
            attendanceDevice.ModelNo = "1";
            attendanceDevice.IPAddress = "192.168.20.67";
            attendanceDevice.Port = 4370;
            attendanceDevice.DeviceMachineNo = 1;


            var result = _zKTecoAttendanceDataFetchBL.GetData_Zkteco(attendanceDevice);
        }

        public async Task ScheduleAsyncAutoGetBranchListAndPostAttendance()
        {


            AttendanceDevice attendanceDevice = new AttendanceDevice();
            attendanceDevice.Id = 1;
            attendanceDevice.DeviceTypeName = "Zkteco";
            attendanceDevice.AttendanceDeviceTypeId = 1;
            attendanceDevice.ModelNo = "1";
            attendanceDevice.IPAddress = "192.168.20.67";
            attendanceDevice.Port = 4370;
            attendanceDevice.DeviceMachineNo = 1;


            var result = _zKTecoAttendanceDataFetchBL.GetData_Zkteco(attendanceDevice);
        }


        //public async Task ScheduleAsyncAutoPushDataToMainServer()
        //{
        //    var result = _attendanceFetchApiBL.PushToMainServer();
        //}



    }

}
