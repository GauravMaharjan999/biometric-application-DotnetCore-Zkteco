using Attendance_ZKTeco_Service.Models;
using AttendanceFetch.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceFetch.Helpers.MainServerFetchApi
{
    public interface IAttendanceFetchBL
    {
        Task<DataResult<List<BranchDeviceTaggingViewModel>>> GetBranchListDataFromMainServer();
        Task<DataResult<MachineInfoViewModel>> PushToMainServer();
    }
}
