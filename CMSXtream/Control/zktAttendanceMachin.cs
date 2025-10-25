using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSXtream.Control
{

    public class zktAttendanceMachin
    {
        public zkemkeeper.CZKEMClass axCZKEM1 = new zkemkeeper.CZKEMClass();

        #region Communication
        public bool bIsConnected = false;//the boolean value identifies whether the device is connected
        private int iMachineNumber = 1;//the serial number of the device.After connecting the device ,this value will be changed.

        private String machineIP = System.Configuration.ConfigurationManager.AppSettings.Get("Sync_Attendance_IP").ToString();
        private String machinePort = System.Configuration.ConfigurationManager.AppSettings.Get("Sync_Attendance_Port").ToString();
        private String errorMsg = string.Empty;

        //If your device supports the TCP/IP communications, you can refer to this.
        //when you are using the tcp/ip communication,you can distinguish different devices by their IP address.
        public String connectMachin()
        {
            try
            {
                if (machineIP.Trim() == "" || machinePort.Trim() == "")
                {
                    errorMsg = "IP and Port cannot be null";
                    return errorMsg;
                }
                int idwErrorCode = 0;

                //if (bIsConnected == true)
                //{
                //    axCZKEM1.Disconnect();
                //    bIsConnected = false;
                //    return errorMsg;
                //}

                bIsConnected = axCZKEM1.Connect_Net(machineIP.Trim(), Convert.ToInt32(machinePort.Trim()));
                if (bIsConnected == true)
                {
                    iMachineNumber = 1;//In fact,when you are using the tcp/ip communication,this parameter will be ignored,that is any integer will all right.Here we use 1.
                    axCZKEM1.RegEvent(iMachineNumber, 65535);//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
                }
                else
                {
                    axCZKEM1.GetLastError(ref idwErrorCode);
                    errorMsg = "Unable to connect the device,ErrorCode=" + idwErrorCode.ToString();
                }
                return errorMsg;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        public bool SyncAttendanceDevice()
        {
            if (SyncMembers() && SyncAttendance())
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool SyncMembers()
        {
            try
            {
                return false;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                return false;
            }
        }

        private bool SyncAttendance()
        {
            try
            {
                return false;
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                return false;
            }
        }

    }
}
