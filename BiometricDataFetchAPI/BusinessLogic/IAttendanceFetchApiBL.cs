using Attendance_ZKTeco_Service.Models;
using AttendanceFetch.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BiometricDataFetchAPI.BusinessLogic
{
    public interface IAttendanceFetchApiBL
    {
        Task<DataResult<List<BranchDeviceTaggingViewModel>>> GetBranchListDataFromMainServer();
        Task<DataResult<MachineInfoViewModel>> PushToMainServer();
    }
}
