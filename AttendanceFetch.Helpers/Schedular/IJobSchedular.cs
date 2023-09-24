using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceFetch.Helpers.Schedular
{
    public interface IJobSchedular
    {
        Task ScheduleAsyncAutoPushDataToMainServer();
        Task ScheduleAsyncAutoPushDataToMainServerAndDeleteAttLog();
    }
}
