﻿using AttendanceFetch.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Attendance_ZKTeco_Service.Interfaces
{
    public interface IJobSchedular
    {
        Task ScheduleAsyncAutoGetAttendance();


    }
}