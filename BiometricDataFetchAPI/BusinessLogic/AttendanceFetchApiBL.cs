using Attendance_ZKTeco_Service.Models;
using AttendanceFetch.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using Attendance_ZKTeco_Service.Interfaces;
using System.Threading;
using System.Text;
using System.Security.Policy;
using System.Text.RegularExpressions;

namespace BiometricDataFetchAPI.BusinessLogic
{
    public class AttendanceFetchApiBL : IAttendanceFetchApiBL
    {
        private readonly IConfiguration _configuration;
        private readonly IZKTecoAttendance_DataFetchBL _dataFetchBL;
        public AttendanceFetchApiBL(IConfiguration configuration, IZKTecoAttendance_DataFetchBL dataFetchBL)
        {
                _configuration = configuration; 
                _dataFetchBL = dataFetchBL; 
        }
        public async Task<DataResult<List<BranchDeviceTaggingViewModel>>> GetBranchListDataFromMainServer()
        {
            try
            {
                var BranchId = _configuration.GetSection("AppCustomSettings").GetSection("Branch").Value;
                var BaseUrl = _configuration.GetSection("AppCustomSettings").GetSection("MainServerUrl").Value;
                var requestUri = BaseUrl+"api/AttendanceLogApi/GetBranchListForAttendanceDevice/" + BranchId;
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

        public async Task<DataResult<List<BranchDeviceTaggingViewModel>>> BulkUserFromMainServer()
        {
            try
            {
                var BranchId = _configuration.GetSection("AppCustomSettings").GetSection("Branch").Value;
                var BaseUrl = _configuration.GetSection("AppCustomSettings").GetSection("MainServerUrl").Value;
                var requestUri = BaseUrl + "api/AttendanceLogApi/PushBulkUsersToDevice/" + BranchId;
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

        public async Task<DataResult<MachineInfoViewModel>> PushToMainServer()
        {
            try
            {
                List<MachineInfo> machineInfos = new List<MachineInfo>();    
                var branches = await GetBranchListDataFromMainServer();
                foreach (var branch in branches.Data)
                {
                    AttendanceDevice attendanceDevice = new AttendanceDevice
                    {
                        Id = branch.Id,
                        AttendanceDeviceTypeId = branch.AttendanceDeviceTypeId,
                        IPAddress = branch.IPAddress,
                        Port = branch.Port,
                        DeviceMachineNo = int.Parse(branch.DeviceMachineNo),
                        DeviceTypeName = branch.DeviceTypeName,
                        ModelNo = branch.ModelNo

                    };

                    var dataResult = await _dataFetchBL.GetData_Zkteco(attendanceDevice);
                    if (dataResult.ResultType== ResultType.Success)
                    {
                        machineInfos.AddRange(dataResult.Data);
                    }

                }

                using (var client = new HttpClient())
                {
                    var BaseUrl = _configuration.GetSection("AppCustomSettings").GetSection("MainServerUrl").Value; 
                    var AttendanceDeviceId = _configuration.GetSection("AppCustomSettings").GetSection("AttendanceDeviceId").Value;

                    var AttendanceDeviceIdinInt = int.Parse(AttendanceDeviceId);
                    
                    var requestUri = BaseUrl + "api/AttendanceLogApi/GetDataFromAttendanceDevice/";

                    var machineInfoViewModel = new MachineInfoViewModel
                    {
                        machineInfoList = machineInfos,
                        AttendanceDeviceId = AttendanceDeviceIdinInt,
                        ClientAlias = "Admin"
                    };
                    HttpContent content = new StringContent(JsonConvert.SerializeObject(machineInfoViewModel), Encoding.UTF8, "application/json");

                    var result = await client.SendAsync(new HttpRequestMessage(HttpMethod.Post, requestUri) { Content = content }, CancellationToken.None);

                    //var result = await client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(machineInfos), Encoding.UTF8, "application/json"));


              

                    if (result.IsSuccessStatusCode)
                    {
                        //string aa = result.Content.ReadAsStringAsync().Result;
                        //DataResult<List<MachineInfo>> dataresult = JsonConvert.DeserializeObject<DataResult<List<MachineInfo>>>(aa);

                        return new DataResult<MachineInfoViewModel> { ResultType = ResultType.Success, Message = "Successfully post in main server", Data = machineInfoViewModel };
                    }
                    else
                    {
                        return new DataResult<MachineInfoViewModel> { ResultType = ResultType.Failed, Message="Failed to post in main server", Data = machineInfoViewModel };
                    }
                }

             


            }
            catch (Exception ex)
            {

                return new DataResult<MachineInfoViewModel> { ResultType = ResultType.Failed, Message= "Failed to fetch data " };

            }

        }
    }
}
