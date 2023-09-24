using AttendanceFetch.Helpers.MainServerFetchApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Attendance_ZKTeco_Service.Interfaces;
using AttendanceFetch.Models;
using Attendance_ZKTeco_Service.Models;
using Newtonsoft.Json;
using System.Linq;
using Hangfire;

namespace AttendanceFetch.Helpers.MainServerFetchApi
{
    public class AttendanceFetchBL : IAttendanceFetchBL
    {
        private readonly IConfiguration _configuration;
        private readonly IZKTecoAttendance_DataFetchBL _dataFetchBL;
        public AttendanceFetchBL(IConfiguration configuration, IZKTecoAttendance_DataFetchBL dataFetchBL)
        {
            _configuration = configuration;
            _dataFetchBL = dataFetchBL;
        }


        public async Task<DataResult<List<BranchDeviceTaggingViewModel>>> BulkUserFromMainServer()
        {
            try
            {
                var BranchId = _configuration.GetSection("AppCustomSettings").GetSection("Branch").Value;
                var BaseUrl = _configuration.GetSection("AppCustomSettings").GetSection("MainServerUrl").Value;
                var requestUri = BaseUrl + HrApiRoute.Get.PushBulkUsersToDevice + BranchId;
                HttpClient _client = new HttpClient();
                HttpResponseMessage response = await _client.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    // Read the response content as a string
                    var responseBody = await response.Content.ReadAsStringAsync();

                    // Deserialize the JSON string into your desired object
                    DataResult<List<BranchDeviceTaggingViewModel>> obj = JsonConvert.DeserializeObject<DataResult<List<BranchDeviceTaggingViewModel>>>(responseBody);
                    return obj;

                }
                else
                {
                    return new DataResult<List<BranchDeviceTaggingViewModel>> { ResultType = ResultType.Failed, Message = "Failed to get data from main server" };
                }



            }
            catch (Exception ex)
            {

                return new DataResult<List<BranchDeviceTaggingViewModel>> { ResultType = ResultType.Exception, Message = ex.Message };

            }

        }
        #region Methods for Push of Data From Attendance Application to Main Server

        public async Task<DataResult<MachineInfoViewModel>> PushToMainServer(bool isDeleteDataRequired = false)
        {
            try
            {
                var isNeededToPushFromAttendanceToMainServer = Convert.ToInt32(_configuration.GetSection("AppCustomSettings").GetSection("IsNeededToPushFromAttendanceToMainServer").Value);
                if (isNeededToPushFromAttendanceToMainServer==0)
                {
                    return new DataResult<MachineInfoViewModel> { ResultType = ResultType.Failed, Message = "No Need To Push Data To Main Server" };
                }
                List<MachineInfo> machineInfos = new List<MachineInfo>();
                var branchesDeviceList = await GetBranchListDataFromMainServer();
                List<AttendanceDevice> attendanceDevicesToClearAttLogs = new List<AttendanceDevice>();
                if (branchesDeviceList.Data != null)
                {
                    foreach (var devices in branchesDeviceList.Data)
                    {
                        AttendanceDevice attendanceDevice = new AttendanceDevice
                        {
                            Id = devices.Id,
                            AttendanceDeviceTypeId = devices.AttendanceDeviceTypeId,
                            IPAddress = devices.IPAddress,
                            Port = devices.Port,
                            DeviceMachineNo = int.Parse(devices.DeviceMachineNo),
                            DeviceTypeName = devices.DeviceTypeName,
                            ModelNo = devices.ModelNo
                        };

                        var dataResult = await _dataFetchBL.GetData_Zkteco(attendanceDevice);
                        if (dataResult.ResultType == ResultType.Success)
                        {
                            attendanceDevicesToClearAttLogs.Add(attendanceDevice);
                            machineInfos.AddRange(dataResult.Data);
                        }
                        machineInfos = machineInfos.Select(c => { c.AttendanceDeviceId = devices.AttendanceDeviceId; return c; }).ToList();

                        //catch failed response
                    }

                    //pushing raw data to main server
                    if (machineInfos.Count()<=0)
                    {
                        return new DataResult<MachineInfoViewModel> { ResultType = ResultType.Failed, Message = "No data to fetch" };
                    }
                    var pushResult = await PushRawDataToMainServer(machineInfos);
                    if (isDeleteDataRequired)
                    {
                        foreach(var device in attendanceDevicesToClearAttLogs) {
                            BackgroundJob.Schedule(() => DeleteAttLog(device), TimeSpan.FromSeconds(20));
                        }
                    }
                    return pushResult;
                }
                else
                {
                    return new DataResult<MachineInfoViewModel>();
                }
                

            }
            catch (Exception ex)
            {

                return new DataResult<MachineInfoViewModel> { ResultType = ResultType.Failed, Message = "Failed to fetch data " };

            }

        }


        public async Task<DataResult<List<BranchDeviceTaggingViewModel>>> GetBranchListDataFromMainServer()
        {
            try
            {
                var BranchId = _configuration.GetSection("AppCustomSettings").GetSection("Branch").Value;
                var BaseUrl = _configuration.GetSection("AppCustomSettings").GetSection("MainServerUrl").Value;
                //var requestUri = BaseUrl + "api/AttendanceLogApi/GetBranchListForAttendanceDevice/" + BranchId;
                var requestUri = BaseUrl + HrApiRoute.Get.BranchList + BranchId;
                HttpClient _client = new HttpClient();
                HttpResponseMessage response = await _client.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    // Read the response content as a string
                    var responseBody = await response.Content.ReadAsStringAsync();

                    // Deserialize the JSON string into your desired object
                    DataResult<List<BranchDeviceTaggingViewModel>> obj = JsonConvert.DeserializeObject<DataResult<List<BranchDeviceTaggingViewModel>>>(responseBody);
                    return obj;

                }
                else
                {
                    return new DataResult<List<BranchDeviceTaggingViewModel>> { ResultType = ResultType.Failed, Message = "Failed to get data from main server" };
                }



            }
            catch (Exception ex)
            {

                return new DataResult<List<BranchDeviceTaggingViewModel>> { ResultType = ResultType.Exception, Message = ex.Message };

            }

        }
        public async Task<DataResult<MachineInfoViewModel>> PushRawDataToMainServer(List<MachineInfo> machineInfos)
        {
            using (var client = new HttpClient())
            {
                var BaseUrl = _configuration.GetSection("AppCustomSettings").GetSection("MainServerUrl").Value;
                var fetchType = Convert.ToInt32(_configuration.GetSection("AppCustomSettings").GetSection("FetchType").Value);
                var requestUri = BaseUrl + HrApiRoute.Get.PostDeviceDataToMainServer;

                var machineInfoViewModel = new MachineInfoViewModel
                {
                    machineInfoList = machineInfos,
                    FetchType= fetchType,
                    ClientAlias = "Admin"
                };
                HttpContent content = new StringContent(JsonConvert.SerializeObject(machineInfoViewModel), Encoding.UTF8, "application/json");

                var result = await client.SendAsync(new HttpRequestMessage(HttpMethod.Post, requestUri) { Content = content }, CancellationToken.None);

                // make logs in table of application or in main server
                if (result.IsSuccessStatusCode)
                {
                    return new DataResult<MachineInfoViewModel> { ResultType = ResultType.Success, Message = "Successfully post in main server", Data = machineInfoViewModel };
                }
                else
                {
                    return new DataResult<MachineInfoViewModel> { ResultType = ResultType.Failed, Message = "Failed to post in main server", Data = machineInfoViewModel };
                }
            }
        }


        public async Task<DataResult> DeleteAttLog(AttendanceDevice attendanceDevice)
        {
            var result = await _dataFetchBL.DeleteData_Zkteco(attendanceDevice);
            return result;
        }

        #endregion
    }
}
