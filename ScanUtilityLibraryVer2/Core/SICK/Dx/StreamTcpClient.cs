using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using CommonLib.Clients;
using System.Diagnostics;
using CommonLib.Function;
using ScanUtilityLibrary.Model;
using ScanUtilityLibrary.Model.SICK.Dx;
using System.Collections.Generic;

namespace ScanUtilityLibrary.Core.SICK.Dx
{
    /// <summary>
    /// 西克（SICK）Dx距离传感器系列TCP通讯类
    /// </summary>
    public class StreamTcpClient : BaseTcpClient
    {
        private readonly Thread _threadRequire;

        #region 属性
        /// <summary>
        /// 命令发送接收对象
        /// </summary>
        public new CommandSender CommandSender { get; set; }

        /// <summary>
        /// 设备参数
        /// </summary>
        public SensorData SensorData { get; set; }

        /// <summary>
        /// 包含所有有待请求的数据的类型列表
        /// </summary>
        public List<RequiredDataType> RequiredDataTypes { get; set; }

        /// <summary>
        /// 循环请求数据的时间间隔（单位毫秒，默认1000毫秒）
        /// </summary>
        public int RequiredInterval { get; set; }
        #endregion

        /// <summary>
        /// 默认构造器
        /// </summary>
        public StreamTcpClient() : this(string.Empty, string.Empty) { }

        /// <summary>
        /// TCP服务端连接
        /// </summary>
        /// <param name="server">IP地址</param>
        /// <param name="port">端口号</param>
        public StreamTcpClient(string server, string port) : base(server, port)
        {
            SensorData = new SensorData();
            //RequiredDataTypes = new List<RequiredDataType>();
            RequiredInterval = 1000;
            _threadRequire = new Thread(new ThreadStart(RequireDataLoop)) { IsBackground = true };
            _threadRequire.Start();
        }

        /// <inheritdoc/>
        protected override void InitCmdSender()
        {
            CommandSender = new CommandSender(this);
        }

        /// <inheritdoc/>
        public override void AnalyzeReceivedMessage(string content)
        {
            string result;

            //匹配返回测量数据
            //results = RegexMatcher.FindMatches(content, Patterns.DataOutput);
            result = RegexMatcher.FindLastMatch(content, Patterns.ContiAnyOutput);
            if (string.IsNullOrWhiteSpace(result))
                ReceivedMessage = content;
            else
            {
                ReceivedData = BaseCmdSender.StripMessage(result); //找到最后一条数据
                DataFetchTimeout = 0;
            }
        }

        /// <inheritdoc/>
        public override void ResolveData(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;
            try
            {
                SensorData.ResolveTelegram(message);
            }
            catch (Exception) { /*throw;*/ }
        }

        /// <inheritdoc/>
        protected override void ReconnectUrself()
        {
            //throw new NotImplementedException();
        }

        private void RequireDataLoop()
        {
            while (true)
            {
                Thread.Sleep(RequiredInterval);
                ////假如未连接，暂停线程
                //if (!IsConnected)
                //    ResetEvent.WaitOne();
                //假如未连接，进行下一次循环
                if (!IsConnected)
                    continue;
                if (RequiredDataTypes == null || RequiredDataTypes.Count == 0 || CommandSender == null)
                    continue;
                if (RequiredDataTypes.Contains(RequiredDataType.Distance))
                {
                    CommandSender.RequireDistance(out double value);
                    SensorData.Distance = value;
                }
                if (RequiredDataTypes.Contains(RequiredDataType.Velocity))
                {
                    CommandSender.RequireVelocity(out double value);
                    SensorData.Velocity = value;
                }
                if (RequiredDataTypes.Contains(RequiredDataType.SignalLevel))
                {
                    CommandSender.RequireSignalLevel(out int value);
                    SensorData.SignalLevel = value;
                }
                if (RequiredDataTypes.Contains(RequiredDataType.DeviceTemperature))
                {
                    CommandSender.RequireDeviceTemperature(out byte value);
                    SensorData.Temperature = value;
                }
                if (RequiredDataTypes.Contains(RequiredDataType.OperatingHours))
                {
                    CommandSender.RequireDeviceOpHours(out uint value);
                    SensorData.OperatingHours = value;
                }
                if (RequiredDataTypes.Contains(RequiredDataType.DeviceStatusWord))
                {
                    DeviceStatusWord word = SensorData.StatusWord;
                    CommandSender.RequireStatusWord(ref word);
                    SensorData.StatusWord = word;
                }
            }
        }

        /// <summary>
        /// 获取TCP信息抬头
        /// </summary>
        /// <returns></returns>
        public string GetTcpHeader()
        {
            string txt = "TCP是否已连接：" + IsConnected + "\r\n";
            txt += "TCP连接状态: " + ErrorMessage + "\r\n" + "\r\n";

            if (SensorData == null)
                return txt;

            txt += "距离：" + SensorData.Distance + "\r\n";
            txt += "速度：" + SensorData.Velocity + "\r\n";
            txt += "状态字：" + SensorData.StatusWord + "\r\n";
            txt += "信号级别：" + SensorData.SignalLevel + "\r\n";
            txt += "设备温度：" + SensorData.Temperature + "\r\n";
            txt += "使用时长：" + SensorData.OperatingHours + "\r\n";

            return txt;
        }
    }
}

