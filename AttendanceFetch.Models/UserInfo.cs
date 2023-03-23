using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AttendanceFetch.Models
{
    public class UserInfo : BaseDeviceModel
    {
        public int DwMachineNumber { get; set; }
        public string DwEnrollNumber { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
       
    }
}
