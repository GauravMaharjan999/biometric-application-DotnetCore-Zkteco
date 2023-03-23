using AttendanceFetch.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Attendance_ZKTeco_Service.Interfaces
{
    public interface IZKTecoAttendance_UserBL
    {
        Task<DataResult> SetUser(UserInfo model);
        Task<DataResult> DeleteUser(UserInfo model);
        DataResult<List<UserInfo>> GetAllUserInfo(string IPaddress, int Port);
    }
}
