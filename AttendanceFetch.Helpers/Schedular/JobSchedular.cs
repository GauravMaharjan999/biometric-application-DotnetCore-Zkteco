using AttendanceFetch.Helpers.MainServerFetchApi;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceFetch.Helpers.Schedular
{
    public class JobSchedular : IJobSchedular
    {
        //private readonly IZKTecoAttendance_DataFetchBL _zKTecoAttendanceDataFetchBL;
        private readonly IAttendanceFetchBL _attendanceFetchBL; 


        public JobSchedular(
            //IZKTecoAttendance_DataFetchBL zKTecoAttendanceDataFetchBL,
            IAttendanceFetchBL attendanceFetchBL
            )
        {
            //_zKTecoAttendanceDataFetchBL = zKTecoAttendanceDataFetchBL;
            _attendanceFetchBL = attendanceFetchBL;
        }
        //public async Task ScheduleAsyncAutoGetAttendance()
        //{
        //    AttendanceDevice attendanceDevice = new AttendanceDevice();
        //    attendanceDevice.Id = 1;
        //    attendanceDevice.DeviceTypeName = "Zkteco";
        //    attendanceDevice.AttendanceDeviceTypeId = 1;
        //    attendanceDevice.ModelNo = "1";
        //    attendanceDevice.IPAddress = "192.168.20.67";
        //    attendanceDevice.Port = 4370;
        //    attendanceDevice.DeviceMachineNo = 1;


        //    var result = _zKTecoAttendanceDataFetchBL.GetData_Zkteco(attendanceDevice);
        //}

        //public async Task ScheduleAsyncAutoGetBranchListAndPostAttendance()
        //{


        //    AttendanceDevice attendanceDevice = new AttendanceDevice();
        //    attendanceDevice.Id = 1;
        //    attendanceDevice.DeviceTypeName = "Zkteco";
        //    attendanceDevice.AttendanceDeviceTypeId = 1;
        //    attendanceDevice.ModelNo = "1";
        //    attendanceDevice.IPAddress = "192.168.20.67";
        //    attendanceDevice.Port = 4370;
        //    attendanceDevice.DeviceMachineNo = 1;


        //    var result = _zKTecoAttendanceDataFetchBL.GetData_Zkteco(attendanceDevice);
        //}


        public async Task ScheduleAsyncAutoPushDataToMainServer()
        {
            var result = _attendanceFetchBL.PushToMainServer();
        }
    }
}
