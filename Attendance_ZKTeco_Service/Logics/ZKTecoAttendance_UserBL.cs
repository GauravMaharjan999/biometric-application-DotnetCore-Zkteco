﻿using Attendance_ZKTeco_Service.Interfaces;
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
    public class ZKTecoAttendance_UserBL : IZKTecoAttendance_UserBL
    {

        public async Task<DataResult> SetUser(UserInfo model)
        {
            DeviceManipulator manipulator = new DeviceManipulator();
            try
            {
                //Validation
                if (model.IPAddress == string.Empty || model.Port <= 0)
                {
                    return new DataResult { ResultType = ResultType.Failed, Message = "The Device IP Address or Port is mandotory !!" };
                }

                bool Isconnected = manipulator.Connect_device(model.IPAddress, model.Port);
                if (!Isconnected)
                {
                    return new DataResult { ResultType = ResultType.Failed, Message = "Device not Connected!!" };
                }
                try
                {
                   var dr= manipulator.SetUserInfo(model);
                    if (dr==true)
                    {

                        return new DataResult { ResultType = ResultType.Success, Message = $" User Created  successfully in device of IP: !! {model.IPAddress}" };
                    }
                    else
                    {
                        return new DataResult { ResultType = ResultType.Failed, Message = $" Failed to Create  in device of IP: !! {model.IPAddress}" };

                    }

                }
                catch (Exception ex)
                {
                    return new DataResult{ ResultType = ResultType.Failed, Message = $"Data pull failed from attendance device!! {ex.Message}" };
                };
            }
            catch (Exception ex)
            {
                return new DataResult { ResultType = ResultType.Failed, Message = $"Device not connected!! {ex.Message}." };
            };
        }
    }

}