using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
//using ScanUtilityLibrary.Model;
using CommonLib.Clients;
using System.Diagnostics;
using CommonLib.Function;
using ScanUtilityLibrary.Model;
using ScanUtilityLibrary.Model.SICK.LMS;

namespace ScanUtilityLibrary.Core.SICK.LMS
{
    /// <summary>
    /// 西克（SICK）LMS扫描仪TCP通讯类
    /// </summary>
    public class StreamTcpClient : BaseTcpClient
    {
        /// <summary>
        /// 扫描点数组
        /// </summary>
        public ScanPoint[] ScanPoints = new ScanPoint[ScanConst.SampleMaxCount];

        #region 属性
        ///// <summary>
        ///// 接收点云数据的字符串格式
        ///// </summary>
        //public string ReceivedScanData{get;set;}

        ///// <summary>
        ///// 接收到的消息
        ///// </summary>
        //public string ReceivedMessage { get; set; }

        ///// <summary>
        ///// Tcp服务端的IP
        ///// </summary>
        //public string ServerIp { get; private set; }

        ///// <summary>
        ///// Tcp服务端的端口号
        ///// </summary>
        //public int ServerPort { get; private set; }

        ///// <summary>
        ///// 是否连接
        ///// </summary>
        //public bool IsConnected { get; private set; }

        ///// <summary>
        ///// 指示是否即将结束接收数据
        ///// </summary>
        //public bool IsReceiving { get; set; }

        ///// <summary>
        ///// TcpClient对象
        ///// </summary>
        //public DerivedTcpClient Client { get; private set; }

        ///// <summary>
        ///// 网络访问数据流对象
        ///// </summary>
        //public NetworkStream NetStream { get; private set; }

        ///// <summary>
        ///// 命令发送接收对象
        ///// </summary>
        //public CommandSender CommandSender { get; private set; }

        /// <summary>
        /// 设备参数
        /// </summary>
        public ScannerParameters ScannerParams { get; set; }

        ///// <summary>
        ///// 错误信息
        ///// </summary>
        //public string ErrorMessage { get; set; }

        ///// <summary>
        ///// 数据获取超时时间
        ///// </summary>
        //public long DataFetchTimeout { get; set; }

        ///// <summary>
        ///// 数据获取超时时间阈值
        ///// </summary>
        //public long DataFetchTimeoutThres { get; set; }

        ///// <summary>
        ///// 是否数据获取超时
        ///// </summary>
        //public bool IsDataFetchTimeOut { get; set; }
        #endregion

        /// <summary>
        /// 默认构造器
        /// </summary>
        public StreamTcpClient() : base() { }

        /// <summary>
        /// TCP服务端连接
        /// </summary>
        /// <param name="server">IP地址</param>
        /// <param name="port">端口号</param>
        public StreamTcpClient(string server, string port) : base(server, port)
        {
            InitScanPoints();
            ScannerParams = new ScannerParameters();
            //Client = new DerivedTcpClient();
            //DataFetchTimeoutThres = 120 * 1000;
            //if (!string.IsNullOrWhiteSpace(server) && !string.IsNullOrWhiteSpace(port))
            //    Connect(server, port);
        }

        /// <summary>
        /// 扫描点对象初始化
        /// </summary>
        public void InitScanPoints()
        {
            for (int i = 0; i < ScanPoints.Length; i++)
                ScanPoints[i] = new ScanPoint(ScanDeviceType.LMS, 0, 0, 0);
        }

        /// <summary>
        /// 扫描点对象重置
        /// </summary>
        public void ResetScanPoints()
        {
            for (int i = 0; i < ScanPoints.Length; i++)
                ScanPoints[i].Distance = 0;
        }

        /// <summary>
        /// 初始化命令通讯对象
        /// </summary>
        public override void InitCmdSender()
        {
            CommandSender = new CommandSender(this);
        }

        /// <summary>
        /// 处理接收到的消息
        /// </summary>
        /// <param name="content"></param>
        public override void AnalyzeReceivedMessage(string content)
        {
            string[] results;

            //匹配返回扫描数据
            results = RegexMatcher.FindMatches(content, Patterns.DataOutput);
            if (results == null || results.Length == 0)
                ReceivedMessage = content;
            else
            {
                ReceivedData = BaseCmdSender.StripMessage(results[0]);
                DataFetchTimeout = 0;
            }
        }

        /// <summary>
        /// 解析扫描数据
        /// </summary>
        /// <param name="message"></param>
        public override void ResolveData(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;
            try
            {
                string[] results = message.Split(' ');
                ScannerParams.ResolveParameters(results);
                int offset = ScannerParams.AmountOfEncoder == 1 ? 2 : 0;
                int firstIndex = 26 + offset;
                for (int i = 0; i < ScannerParams.AmountOfData; i++)
                {
                    ScanPoint point = ScanPoints[i];
                    point.Distance = (uint)(Convert.ToUInt32(results[firstIndex + i], 16) * ScannerParams.ScaleFactor);
                    point.Angle = ScannerParams.StartAngle + ScannerParams.AngleResolution * i;
                }
            }
            catch (Exception) { throw; }
        }

        ///// <summary>
        ///// 解析扫描仪参数
        ///// </summary>
        ///// <param name="results"></param>
        //private void ResolveScannerParams(string[] results)
        //{
        //    ScannerParams.VersionNumber = Convert.ToUInt16(results[2], 16); //设备版本号
        //    ScannerParams.DeviceNumber = Convert.ToUInt16(results[3], 16); //设备ID
        //    ScannerParams.SerialNumber = Convert.ToUInt32(results[4], 16); //设备序列号
        //    ScannerParams.DeviceStatus = Convert.ToByte(results[5] + results[6]); //设备状态
        //    ScannerParams.TelegramCounter = Convert.ToUInt16(results[7], 16); //指令计数
        //    ScannerParams.ScanCounter = Convert.ToUInt16(results[8], 16); //扫描计数
        //    ScannerParams.TimeSinceStartUp = Convert.ToUInt64(results[9], 16); //扫描开始时间
        //    ScannerParams.TimeOfTransmission = Convert.ToUInt64(results[10], 16); //扫描结束时间
        //    ScannerParams.StatusOfDigitalInputs = Convert.ToByte(results[11] + results[12]); //设备开关量输入状态
        //    //ScannerParams.StatusOfDigitalOutputs = Convert.ToByte(results[15], 16); //设备开关量输出状态
        //    ScannerParams.StatusOfDigitalOutputs = results[13] + " " + results[14]; //设备开关量输出状态
        //    ScannerParams.LayerAngle = Convert.ToUInt16(results[15]);
        //    ScannerParams.ScanFrequency = Convert.ToUInt32(results[16], 16) / 100; //扫描频率
        //    ScannerParams.MeasurementFrequency = Convert.ToUInt32(results[17], 16); //每次扫描频率
        //    ScannerParams.AmountOfEncoder = Convert.ToByte(results[18]); //编码器数量
        //    if (ScannerParams.AmountOfEncoder > 0)
        //    {
        //        ScannerParams.EncoderPosition = Convert.ToUInt32(results[19], 16); //编码器位置
        //        ScannerParams.EncoderSpeed = Convert.ToUInt16(results[20], 16); //编码器速度
        //    }
        //    int offset = ScannerParams.AmountOfEncoder > 0 ? 2 : 0;
        //    ScannerParams.AmountOfChannels = Convert.ToUInt16(results[19 + offset]); //回波层数量
        //    ScannerParams.OutputChannel = results[20 + offset]; //回波层序号
        //    ScannerParams.ScaleFactorString = results[21 + offset]; //系数
        //    ScannerParams.ScaleOffset = Convert.ToInt32(results[22 + offset], 16); //系数偏移量
        //    ScannerParams.StartAngle = (double)Convert.ToInt32(results[23 + offset], 16) / 10000;
        //    ScannerParams.AngleResolution = (double)Convert.ToInt32(results[24 + offset], 16) / 10000;
        //    ScannerParams.AmountOfData = Convert.ToUInt16(results[25 + offset], 16);
        //}

        ///// <summary>
        ///// 刷新扫描点信息
        ///// </summary>
        //public void GetDataStream()
        //{
        //    int interval = 1; //循环时间间隔
        //    Stopwatch watch = new Stopwatch();
        //    while (true)
        //    {
        //        try
        //        {
        //            watch.Stop();
        //            //假如连续读标志为1，时间累积
        //            if (CommandSender.OutputFlag == 1)
        //                DataFetchTimeout += watch.ElapsedMilliseconds;
        //            else
        //                DataFetchTimeout = 0;
        //            IsDataFetchTimeOut = DataFetchTimeout > DataFetchTimeoutThres; //获取数据是否超时
        //            watch.Reset();
        //            watch.Start();

        //            Thread.Sleep(interval);
        //            if (Client == null || !Client.IsConnected || string.IsNullOrWhiteSpace(ReceivedScanData) || !IsReceiving)
        //            {
        //                //Thread.Sleep(interval);
        //                continue;
        //            }
        //            string message = ReceivedScanData;
        //            ResolveData(message);
        //        }
        //        catch (ArgumentNullException e) { ErrorMessage = "ArgumentNullException: " + e.Message; }
        //        catch (SocketException e) { ErrorMessage = "SocketException: " + e.Message; }
        //        catch (Exception e) { ErrorMessage = "Error Socket: " + e.Message; }
        //        finally
        //        {
        //            //watch.Stop();
        //            //DataFetchTimeout += watch.ElapsedMilliseconds;
        //            //watch.Reset();
        //        }
        //    }
        //}

        ///// <summary>
        ///// 获取TCP信息抬头
        ///// </summary>
        ///// <returns></returns>
        //public string GetTcpHeader()
        //{
        //    string txt = "TCP是否已连接：" + IsConnected + "\r\n";
        //    txt += "TCP连接状态: " + ErrorMessage + "\r\n" + "\r\n";

        //    if (ScannerParams == null)
        //        return txt;

        //    txt += "设备版本号：" + ScannerParams.VersionNumber + "\r\n";
        //    txt += "设备ID：" + ScannerParams.DeviceNumber + "\r\n";
        //    txt += "设备序列号：" + ScannerParams.SerialNumber + "\r\n";
        //    txt += "指令计数：" + ScannerParams.TelegramCounter.ToString("D") + "\r\n";
        //    txt += "扫描计数：" + ScannerParams.ScanCounter.ToString("D") + "\r\n";
        //    txt += "扫描起始时间：" + ScannerParams.TimeSinceStartUp.ToString("D") + "\r\n";
        //    txt += "扫描结束时间：" + ScannerParams.TimeOfTransmission.ToString("D") + "\r\n";
        //    txt += "设备开关量输入状态：" + ScannerParams.StatusOfDigitalInputs.ToString("D") + "\r\n";
        //    txt += "设备开关量输出状态：" + ScannerParams.StatusOfDigitalOutputs.ToString("D") + "\r\n";
        //    txt += "扫描频率：" + ScannerParams.ScanFrequency.ToString("D") + "\r\n";
        //    txt += "有无编码器：" + ScannerParams.AmountOfEncoder.ToString("D") + "\r\n";

        //    //假如有编码器
        //    if (ScannerParams.AmountOfEncoder == 1)
        //    {
        //        txt += "编码器位置：" + ScannerParams.EncoderPosition.ToString("D") + "\r\n";
        //        txt += "编码器速度：" + ScannerParams.EncoderSpeed.ToString("D") + "\r\n";
        //    }

        //    txt += "输出通道：" + ScannerParams.AmountOfChannels.ToString("D") + "\r\n";
        //    txt += "回波层序号：" + ScannerParams.OutputChannel + "\r\n";
        //    txt += "系数：" + ScannerParams.ScaleFactorString + "\r\n";
        //    txt += "系数偏移量：" + ScannerParams.ScaleOffset + "\r\n";
        //    txt += "起始角度：" + ScannerParams.StartAngle.ToString("0.00") + "\r\n";
        //    txt += "角度分辨率：" + ScannerParams.AngleResolution.ToString("0.00") + "\r\n";
        //    txt += "测量数据个数：" + ScannerParams.AmountOfData.ToString("D");

        //    return txt;
        //}

        /// <summary>
        /// 获取TCP信息抬头
        /// </summary>
        /// <returns></returns>
        public string GetTcpHeader()
        {
            string txt = "TCP是否已连接：" + IsConnected + "\r\n";
            txt += "TCP连接状态: " + ErrorMessage + "\r\n" + "\r\n";

            if (ScannerParams == null)
                return txt;

            txt += "设备版本号：" + ScannerParams.VersionNumber + "\r\n";
            txt += "设备ID：" + ScannerParams.DeviceNumber + "\r\n";
            txt += "设备序列号：" + ScannerParams.SerialNumber + "\r\n";
            txt += "设备状态：" + ScannerParams.DeviceStatus + "\r\n";
            txt += "指令计数：" + ScannerParams.TelegramCounter + "\r\n";
            txt += "扫描计数：" + ScannerParams.ScanCounter + "\r\n";
            txt += "扫描起始时间：" + ScannerParams.TimeSinceStartUp + "\r\n";
            txt += "扫描结束时间：" + ScannerParams.TimeOfTransmission + "\r\n";
            txt += "设备开关量输入状态：" + ScannerParams.StatusOfDigitalInputs + "\r\n";
            txt += "设备开关量输出状态：" + ScannerParams.StatusOfDigitalOutputs + "\r\n";
            txt += "扫描频率：" + ScannerParams.ScanFrequency + "HZ\r\n";
            txt += "编码器数量：" + ScannerParams.AmountOfEncoder + "\r\n";

            //假如有编码器
            if (ScannerParams.AmountOfEncoder > 0)
            {
                txt += "编码器位置：" + ScannerParams.EncoderPosition + "\r\n";
                txt += "编码器速度：" + ScannerParams.EncoderSpeed + "\r\n";
            }

            txt += "回波层数量：" + ScannerParams.AmountOfChannels + "\r\n";
            txt += "回波层序号：" + ScannerParams.OutputChannel + "\r\n";
            txt += "比例系数：" + ScannerParams.ScaleFactor + "\r\n";
            txt += "系数偏移量：" + ScannerParams.ScaleOffset + "\r\n";
            txt += "起始角度：" + ScannerParams.StartAngle.ToString("0.00") + "\r\n";
            txt += "角分辨率：" + ScannerParams.AngleResolution.ToString("0.0000") + "\r\n";
            txt += "测量数据个数：" + ScannerParams.AmountOfData;

            return txt;
        }

        /// <summary>
        /// 获取扫描点信息（前36个坐标点）的字符串形式
        /// </summary>
        /// <returns></returns>
        public string GetScanDataString()
        {
            int startValue = 0;

            string txt = "扫描数据点 " + startValue + "-" + (startValue + 35) + ":" + (char)13 + (char)10;
            if (ScanPoints == null)
                return txt;

            #region 显示X、Y、Z轴坐标
            txt += "起始数据点:        x:          y:          " + (char)13 + (char)10;
            for (int i = startValue; i < (startValue + 36); i++)
            {
                if (ScanPoints[i] == null)
                    continue;
                txt += i.ToString("00000") + "         ";
                if (!(ScanPoints[i].Distance == 0xFFFFFFFF))
                    txt += ScanPoints[i].X.ToString("00000") + "     " + ScanPoints[i].Y.ToString("00000");
                else
                    txt += "Error" + "     " + "Error" + "     " + "Error";
                txt += (char)13 + (char)10;
            }
            #endregion

            return txt;
        }
    }
}

