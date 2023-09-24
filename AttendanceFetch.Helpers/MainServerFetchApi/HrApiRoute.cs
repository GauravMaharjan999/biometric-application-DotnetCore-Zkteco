using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace AttendanceFetch.Helpers.MainServerFetchApi
{


    public static class HrApiRoute
    {
        public class Get
        {
            public const string BaseUri = "api/AttendanceLogApi/";
            public const string BranchList = BaseUri + "GetBranchListForAttendanceDevice/";
            public const string PushBulkUsersToDevice = BaseUri + "PushBulkUsersToDevice/";
            public const string PostDeviceDataToMainServer = BaseUri + "PostDeviceDataToMainServer/";

        }

    }
}
