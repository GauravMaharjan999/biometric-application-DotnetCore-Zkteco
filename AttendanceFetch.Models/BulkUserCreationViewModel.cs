using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AttendanceFetch.Models
{
    public class BulkUserCreationViewModel : BaseDeviceModel
    {
        public List<UserInfo> SuccessList { get; set; }
        public List<UserInfo> FailedList { get; set; }
        public int SuccessListCount { get; set; }
        public int FailedListCount { get; set; }



    }
}
