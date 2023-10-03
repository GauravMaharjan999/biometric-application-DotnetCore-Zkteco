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
using Microsoft.Extensions.Logging;

namespace AttendanceFetch.Helpers.MainServerFetchApi
{
    public class AttendanceFetchBL : IAttendanceFetchBL
    {
        private readonly IConfiguration _configuration;
        private readonly IZKTecoAttendance_DataFetchBL _dataFetchBL;
        private readonly ILogger<AttendanceFetchBL> _logger;

        public AttendanceFetchBL(IConfiguration configuration, IZKTecoAttendance_DataFetchBL dataFetchBL, ILogger<AttendanceFetchBL> logger)
        {
            _configuration = configuration;
            _dataFetchBL = dataFetchBL;
            _logger = logger;

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

        public async Task<DataResult<MachineInfoViewModel>> PushToMainServer(TriggeredFrom triggeredFrom,bool isDeleteDataRequired = false)
        {
            try
            {
               _logger.LogInformation($"Process {triggeredFrom} : Step 2 Started :  #PushToMainServer Start visit and isDeleteDataRequired is {isDeleteDataRequired} at {DateTime.Now}");


                var isNeededToPushFromAttendanceToMainServer = Convert.ToInt32(_configuration.GetSection("AppCustomSettings").GetSection("IsNeededToPushFromAttendanceToMainServer").Value);
                if (isNeededToPushFromAttendanceToMainServer==0)
                {
                    return new DataResult<MachineInfoViewModel> { ResultType = ResultType.Failed, Message = "No Need To Push Data To Main Server" };
                }
                List<MachineInfo> machineInfos = new List<MachineInfo>();
                var branchesDeviceList = await GetBranchListDataFromMainServer(triggeredFrom);
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

                        _logger.LogInformation($"Process {triggeredFrom} : Step 4 Started :  #GetData_Zkteco Started visit at {DateTime.Now}");
                        var dataResult = await _dataFetchBL.GetData_Zkteco(attendanceDevice);
                        
                        _logger.LogInformation($"Process {triggeredFrom} : Step 4 Ended :  #GetData_Zkteco Ended visit at {DateTime.Now}");

                        if (dataResult.ResultType == ResultType.Success)
                        {
                            var dataCount = dataResult.Data  == null ? 0 : dataResult.Data.Count();
                            _logger.LogInformation($"#Data Successfully retrieved in PushToMainServer at {DateTime.Now} with Data Count {dataCount}");

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
                    var pushResult = await PushRawDataToMainServer(machineInfos,triggeredFrom);
                    if (isDeleteDataRequired)
                    {
                        foreach(var device in attendanceDevicesToClearAttLogs) {
                            _logger.LogInformation($"Process {triggeredFrom} : Step 5 Started :  #DeleteAttLog Start for device {device} at {DateTime.Now}");

                            BackgroundJob.Schedule(() => DeleteAttLog(device), TimeSpan.FromSeconds(20));
                            _logger.LogInformation($"Process {triggeredFrom} : Step 5 Started :  #DeleteAttLog End for device {device}  at {DateTime.Now}");

                        }
                    }

                    _logger.LogInformation($"#Process {triggeredFrom} : Step 2 Ended :  #PushToMainServer Start visit and isDeleteDataRequired is {isDeleteDataRequired} at {DateTime.Now}");

                    return pushResult;

                }
                else
                {
                    _logger.LogInformation($"Process {triggeredFrom} : Step 2 Started :  #PushToMainServer End visit and isDeleteDataRequired is {isDeleteDataRequired} at {DateTime.Now}");
                    return new DataResult<MachineInfoViewModel>();

                }


            }
            catch (Exception ex)
            {

                return new DataResult<MachineInfoViewModel> { ResultType = ResultType.Failed, Message = "Failed to fetch data " };

            }

        }


        public async Task<DataResult<List<BranchDeviceTaggingViewModel>>> GetBranchListDataFromMainServer(TriggeredFrom triggeredFrom)
        {
            try
            {
                _logger.LogInformation($"Process {triggeredFrom} : Step 3 Started :  #GetBranchListDataFromMainServer Start visit at {DateTime.Now}");
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
                    _logger.LogInformation($"#GetBranchListDataFromMainServer Successfully came from HRIS with DataResult message :{obj.Message} at {DateTime.Now}");
                    _logger.LogInformation($"Process {triggeredFrom} : Step 3 Ended :  #GetBranchListDataFromMainServer End visit Successfully at {DateTime.Now}");
                    return obj;

                }
                else
                {
                    _logger.LogInformation($"Process {triggeredFrom} : Step 3 Ended :  #GetBranchListDataFromMainServer End visit Failed at {DateTime.Now}");
                    return new DataResult<List<BranchDeviceTaggingViewModel>> { ResultType = ResultType.Failed, Message = "Failed to get data from main server" };
                }



            }
            catch (Exception ex)
            {

                return new DataResult<List<BranchDeviceTaggingViewModel>> { ResultType = ResultType.Exception, Message = ex.Message };

            }

        }
        public async Task<DataResult<MachineInfoViewModel>> PushRawDataToMainServer(List<MachineInfo> machineInfos, TriggeredFrom triggeredFrom)
        {
            using (var client = new HttpClient())
            {
                _logger.LogInformation($"Process {triggeredFrom} : Step 4 Started :  #PushRawDataToMainServer Start visit at {DateTime.Now}");


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
                    _logger.LogInformation($"Process {triggeredFrom} : Step 4 Started :  #PushRawDataToMainServer Start with SUCCESS visit at {DateTime.Now}");


                    return new DataResult<MachineInfoViewModel> { ResultType = ResultType.Success, Message = "Successfully post in main server", Data = machineInfoViewModel };
                }
                else
                {
                    _logger.LogInformation($"Process {triggeredFrom} : Step 4 Started :  #PushRawDataToMainServer Start with Fail visit at {DateTime.Now}");

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
