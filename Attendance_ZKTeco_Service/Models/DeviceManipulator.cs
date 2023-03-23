using AttendanceFetch.Models;
using System;
using System.Collections.Generic;
using System.Text;
using ZkSoftwareEU;

namespace Attendance_ZKTeco_Service.Models
{
    public class DeviceManipulator
    {
        private readonly CZKEUEMNetClass machine;
        public DeviceManipulator()
        {
            machine = new CZKEUEMNetClass();
        }
        public bool Connect_device(string IPaddress, int Port)
        {
            try
            {
                //CZKEUEMNetClass machine = new CZKEUEMNetClass();
                string ipAddress = IPaddress;
                int portNumber = Port; //4370;
                                       //string port = Port.ToString();

                //int portNumber = Port; //4370;
                //if (!int.TryParse(port, out portNumber))
                //    throw new Exception("Not a valid port number");

                bool isValidIpA = UniversalStatic.ValidateIP(ipAddress);
                if (!isValidIpA)
                    throw new Exception("The Device IP is invalid !!");

                isValidIpA = UniversalStatic.PingTheDevice(ipAddress);
                if (!isValidIpA)
                    throw new Exception("The device at " + ipAddress + ":" + portNumber + " did not respond!! Ping failed.");

                //machine = new ZkemClient(RaiseDeviceEvent);
                bool connectNet = machine.Connect_Net(ipAddress, portNumber);
                if (!connectNet)
                    return false;
                //throw new Exception("Device not connected!!.");

                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
            // return false;
        }
        public bool Check_DeviceTime(string IPaddress, int Port)
        {
            int dwYear = int.Parse(System.DateTime.Now.Year.ToString());
            int dwMonth = System.DateTime.Now.Month;
            int dwDay = System.DateTime.Now.Day;
            int dwHour = System.DateTime.Now.Hour;
            int dwMinute = System.DateTime.Now.Minute;
            int dwSecond = System.DateTime.Now.Second;


            Connect_device(IPaddress, Port);
            try
            {
                return machine.GetDeviceTime(1, ref dwYear, ref dwMonth, ref dwDay, ref dwHour, ref dwMinute, ref dwSecond);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<MachineInfo> GetLogData(int machineNumber, string IPaddress)
        {
            try
            {
                List<MachineInfo> lstEnrollData = new List<MachineInfo>();

                //CZKEUEMNetClass machine1 = new CZKEUEMNetClass();
                bool isConected = machine.Connect_Net("192.168.20.24", 4370);
                if (isConected)
                {
                    machine.SSR_SetUserInfo(1, "529", "Bikal Maharjan", "bikal529", 0, true); // User Set



                    int iMachineNumber = 1; //the device number
                    string idwEnrollNumber = "";
                    int idwVerifyMode = 0;
                    int idwInOutMode = 0;
                    int idwYear = 0;
                    int idwMonth = 0;
                    int idwDay = 0;
                    int idwHour = 0;
                    int idwMinute = 0;
                    int idwSecond = 0;
                    int idwWorkCode = 0;

                    string userId = "";
                    string userName = "";
                    string userCardNo = "";
                    string userPassword = "";
                    int abcdef = 1;
                    bool userEnabled = false;

                    machine.ReadAllGLogData(iMachineNumber); //read all the attendance records to the memory       \
                    bool absafsaf = machine.SSR_GetGeneralLogData(iMachineNumber, ref idwEnrollNumber, ref idwVerifyMode, ref idwInOutMode,
                        ref idwYear, ref idwMonth, ref idwDay, ref idwHour, ref idwMinute, ref idwSecond, ref idwWorkCode);
                    while (machine.SSR_GetGeneralLogData(iMachineNumber, ref idwEnrollNumber, ref idwVerifyMode, ref idwInOutMode,
                        ref idwYear, ref idwMonth, ref idwDay, ref idwHour, ref idwMinute, ref idwSecond, ref idwWorkCode)) //get attendance data one by one from memory
                    {
                        //// Do something with attendance data, for example:
                        //Console.WriteLine("Enroll Number: " + idwEnrollNumber.ToString());
                        //Console.WriteLine("Verify Mode: " + idwVerifyMode.ToString());
                        //Console.WriteLine("In Out Mode: " + idwInOutMode.ToString());
                        //Console.WriteLine("Date: " + idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString());
                        //Console.WriteLine("Time: " + idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString());
                        //Console.WriteLine("Work Code: " + idwWorkCode.ToString());

                        string inputDate = new DateTime(idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond).ToString("yyyy-MM-dd HH:mm:ss");
                        MachineInfo objInfo = new MachineInfo();
                        objInfo.MachineNumber = iMachineNumber;
                        objInfo.IndRegID = int.Parse(idwEnrollNumber);
                        objInfo.Mode = idwVerifyMode.ToString();
                        objInfo.DateTimeRecord = inputDate;
                        objInfo.DeviceIP = "192.168.20.24";
                        machine.SSR_GetUserInfo(1, Convert.ToString(objInfo.IndRegID), ref userName, ref userPassword, ref abcdef, ref userEnabled);
                        objInfo.Username = userName;

                        lstEnrollData.Add(objInfo);
                        //disable
                    }

                    var dadsadad = lstEnrollData;
                    //var data= await GetLogData(iMachineNumber, "192.168.20.24");
                    // var abcasdsa = "";
                    //string idwEnrollNumber = "";
                    //int idwVerifyMode = 0;
                    //int idwInOutMode = 0;
                    //int idwYear = 0;
                    //int idwMonth = 0;
                    //int idwDay = 0;
                    //int idwHour = 0;
                    //int idwMinute = 0;
                    //int idwSecond = 0;
                    //int idwWorkCode = 0; 
                    //machine.ReadAllGLogData(iMachineNumber); //read all the attendance records to the memory       \
                    //bool absafsaf = machine.SSR_GetGeneralLogData(iMachineNumber, ref idwEnrollNumber, ref idwVerifyMode, ref idwInOutMode,
                    //    ref idwYear, ref idwMonth, ref idwDay, ref idwHour, ref idwMinute, ref idwSecond, ref idwWorkCode);
                    //while (machine.SSR_GetGeneralLogData(iMachineNumber, ref idwEnrollNumber, ref idwVerifyMode, ref idwInOutMode,
                    //    ref idwYear, ref idwMonth, ref idwDay, ref idwHour, ref idwMinute, ref idwSecond, ref idwWorkCode)) //get attendance data one by one from memory
                    //{
                    //    // Do something with attendance data, for example:
                    //    Console.WriteLine("Enroll Number: " + idwEnrollNumber.ToString());
                    //    Console.WriteLine("Verify Mode: " + idwVerifyMode.ToString());
                    //    Console.WriteLine("In Out Mode: " + idwInOutMode.ToString());
                    //    Console.WriteLine("Date: " + idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString());
                    //    Console.WriteLine("Time: " + idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString());
                    //    Console.WriteLine("Work Code: " + idwWorkCode.ToString());
                    //}
                    //machine.Disconnect(); //Disconnect from the device
                    return lstEnrollData;


                }
                else
                {
                    return lstEnrollData;
                }




                //#region MyRegion


                ////machine.EnableDevice(machineNumber, true);
                //List<MachineInfo> lstEnrollData = new List<MachineInfo>();
                //int iMachineNumber = 1; //the device number
                //string idwEnrollNumber = "";
                //int idwVerifyMode = 0;
                //int idwInOutMode = 0;
                //int idwYear = 0;
                //int idwMonth = 0;
                //int idwDay = 0;
                //int idwHour = 0;
                //int idwMinute = 0;
                //int idwSecond = 0;
                //int idwWorkCode = 0;

                //string userId = "";
                //string userName = "";
                //string userCardNo = "";
                //string userPassword = "";
                //int abcdef = 1;
                //bool userEnabled = false;
                //Connect_device(IPaddress, 4370);
                //machine.ReadAllGLogData(iMachineNumber); //read all the attendance records to the memory       \
                //bool absafsaf = machine.SSR_GetGeneralLogData(iMachineNumber, ref idwEnrollNumber, ref idwVerifyMode, ref idwInOutMode,
                //    ref idwYear, ref idwMonth, ref idwDay, ref idwHour, ref idwMinute, ref idwSecond, ref idwWorkCode);
                //while (machine.SSR_GetGeneralLogData(iMachineNumber, ref idwEnrollNumber, ref idwVerifyMode, ref idwInOutMode,
                //    ref idwYear, ref idwMonth, ref idwDay, ref idwHour, ref idwMinute, ref idwSecond, ref idwWorkCode)) //get attendance data one by one from memory
                //{
                //    //// Do something with attendance data, for example:
                //    //Console.WriteLine("Enroll Number: " + idwEnrollNumber.ToString());
                //    //Console.WriteLine("Verify Mode: " + idwVerifyMode.ToString());
                //    //Console.WriteLine("In Out Mode: " + idwInOutMode.ToString());
                //    //Console.WriteLine("Date: " + idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString());
                //    //Console.WriteLine("Time: " + idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString());
                //    //Console.WriteLine("Work Code: " + idwWorkCode.ToString());

                //    string inputDate = new DateTime(idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond).ToString("yyyy-MM-dd HH:mm:ss");
                //    MachineInfo objInfo = new MachineInfo();
                //    objInfo.MachineNumber = iMachineNumber;
                //    objInfo.IndRegID = int.Parse(idwEnrollNumber);
                //    objInfo.Mode = idwVerifyMode.ToString();
                //    objInfo.DateTimeRecord = inputDate;
                //    objInfo.DeviceIP = "192.168.20.24";
                //    machine.SSR_GetUserInfo(1, Convert.ToString(objInfo.IndRegID), ref userName, ref userPassword, ref abcdef, ref userEnabled);
                //    objInfo.Username = userName;

                //    lstEnrollData.Add(objInfo);
                //    //disable
                //} 
                //#endregion

                //return lstEnrollData;
            }
            catch (Exception ex)
            {
                List<MachineInfo> lst = new List<MachineInfo>();
                return lst;
            }
        }
        public bool ClearGLog(int machineNumber, string IPaddress)
        {
            bool IsClear = machine.ClearGLog(machineNumber);
            bool enable = machine.EnableDevice(machineNumber, true);
            machine.Disconnect();
            return IsClear;
        }
        public void Disconnect()
        {
            machine.Disconnect();
        }
        #region UserPart
        public bool SetUserInfo(UserInfo userInfo)
        {
            try
            {
                if (userInfo == null)
                {
                    return false;
                }
                else
                {
                    var result = machine.SSR_SetUserInfo(userInfo.DwMachineNumber, userInfo.DwEnrollNumber, userInfo.Name, userInfo.Password, 0, true); // User Set
                    return result;
                }
            }
            catch (Exception ex)
            {

                return false;
            }

        } 
        #endregion
    }
}
