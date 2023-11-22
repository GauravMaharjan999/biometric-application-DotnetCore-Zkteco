using Attendance_ZKTeco_Service.Models;
using AttendanceFetch.Helpers.MainServerFetchApi;
using AttendanceFetch.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BiometricDataFetchAPI.Controllers
{
	public class DeviceDashboardController : Controller
	{
		private readonly IAttendanceFetchBL _attendanceFetchBL;

		public DeviceDashboardController(IAttendanceFetchBL attendanceFetchBL)
        {
			_attendanceFetchBL = attendanceFetchBL;
		}

		public async Task<IActionResult>Index()
		{
			List<BranchDeviceTaggingViewModel> branchDeviceTaggingViewModels= new List<BranchDeviceTaggingViewModel>();	
			var res = await _attendanceFetchBL.GetBranchListDataFromMainServer(TriggeredFrom.ScheduleAsyncAutoPushDataToMainServer);
			if (res.ResultType==ResultType.Success)
			{
				branchDeviceTaggingViewModels = res.Data;
			}
			return View(branchDeviceTaggingViewModels);
		}

    }
}
