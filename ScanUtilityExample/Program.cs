using ScanUtilityLibrary.Core.SICK;
using ScanUtilityLibrary.Core.SICK.Dx;
using ScanUtilityLibrary.Model.SICK.Dx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScanUtilityExample
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            #region test
            StreamTcpClient client = new StreamTcpClient("192.168.0.66", "2112");
            //StreamTcpClient client = new StreamTcpClient("127.0.0.1", "2112");
            client.Connect();
            //var sender = client.CommandSender;
            //string getFilter = "sRN filterSelection";
            //string getFilterResult = sender.SendCommand(getFilter);
            //string message = sender.Login(UserLevel.Service);
            //message = sender.SetContinualOutputProtocol(ContinualOutputProtocol.STXETX);
            //message = sender.SetContinualOutputRate(100);
            //message = sender.SetContinualOutputType(ContinualOutputType.DistVel);
            //message = sender.SetDistanceOffset(0);
            //message = sender.SaveSettingToDevice();
            //message = sender.Logout();
            //bool result = sender.RequireDistance(out double dist);
            //result = sender.RequireVelocity(out double vel);
            //result = sender.RequireSignalLevel(out int level);
            //result = sender.RequireDeviceTemperature(out byte temp);
            //result = sender.RequireDeviceOpHours(out uint hours);
            //DeviceStatusWord word = new DeviceStatusWord();
            //result = sender.RequireStatusWord(ref word);
            //client.RequiredDataTypes = new List<RequiredDataType>() { RequiredDataType.Distance, RequiredDataType.Velocity, RequiredDataType.SignalLevel, RequiredDataType.DeviceTemperature, RequiredDataType.OperatingHours, RequiredDataType.DeviceStatusWord };
            //while (true)
            //{
            //    Thread.Sleep(100);
            //}
            #endregion

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }
    }
}
