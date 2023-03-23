﻿using Attendance_ZKTeco_Service.Models;
using AttendanceFetch.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Attendance_ZKTeco_Service.Interfaces
{
    public interface IZKTecoAttendance_DataFetchBL
    {
        Task<DataResult<List<MachineInfo>>> GetData_Zkteco(AttendanceDevice model);
    }
}