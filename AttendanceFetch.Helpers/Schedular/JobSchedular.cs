using AttendanceFetch.Helpers.MainServerFetchApi;
using AttendanceFetch.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace AttendanceFetch.Helpers.Schedular
{
    public class JobSchedular : IJobSchedular
    {
        //private readonly IZKTecoAttendance_DataFetchBL _zKTecoAttendanceDataFetchBL;
        private readonly IAttendanceFetchBL _attendanceFetchBL; 
        private readonly ILogger<JobSchedular> _logger; 


        public JobSchedular(
            //IZKTecoAttendance_DataFetchBL zKTecoAttendanceDataFetchBL,
            IAttendanceFetchBL attendanceFetchBL, ILogger<JobSchedular> logging
            )
        {
            _attendanceFetchBL = attendanceFetchBL;
            _logger = logging;
        }


        public async Task ScheduleAsyncAutoPushDataToMainServer()
        {
            _logger.LogInformation($"#Process {TriggeredFrom.ScheduleAsyncAutoPushDataToMainServer} : Step 1 Started : Start Visit at {DateTime.Now}");
            await _attendanceFetchBL.PushToMainServer(TriggeredFrom.ScheduleAsyncAutoPushDataToMainServer, false);
            _logger.LogInformation($"#Process {TriggeredFrom.ScheduleAsyncAutoPushDataToMainServer} : Step 1  Completed :  End Visit at {DateTime.Now}");
        }
        public async Task ScheduleAsyncAutoPushDataToMainServerAndDeleteAttLogMorning()
        {
            _logger.LogInformation($"#Process {TriggeredFrom.ScheduleAsyncAutoPushDataToMainServerAndDeleteAttLogMorning} : Step 1 Started : Start Visit at {DateTime.Now}");
            await _attendanceFetchBL.PushToMainServer(TriggeredFrom.ScheduleAsyncAutoPushDataToMainServerAndDeleteAttLogMorning, true);
            _logger.LogInformation($"#Process {TriggeredFrom.ScheduleAsyncAutoPushDataToMainServerAndDeleteAttLogMorning} : Step 1 Started : End Visit at {DateTime.Now}");

        }
        public async Task ScheduleAsyncAutoPushDataToMainServerAndDeleteAttLogEvening()
        {
            _logger.LogInformation($"ScheduleAsyncAutoPushDataToMainServerAndDeleteAttLogEvening Start Visit at {DateTime.Now}");
            await _attendanceFetchBL.PushToMainServer(TriggeredFrom.ScheduleAsyncAutoPushDataToMainServerAndDeleteAttLogEvening, true);
            _logger.LogInformation($"ScheduleAsyncAutoPushDataToMainServerAndDeleteAttLogEvening End Visit at {DateTime.Now}");

        }
    }
}
