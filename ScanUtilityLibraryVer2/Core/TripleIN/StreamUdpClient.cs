using CommonLib.Clients;
using CommonLib.Clients.Tasks;
using CommonLib.Events;
using CommonLib.Extensions;
using CommonLib.Function;
using ScanUtilityLibrary.Model;
using ScanUtilityLibrary.Model.TripleIN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using static CommonLib.Clients.Tasks.Task;
using static CommonLib.Function.TimerEventRaiser;

namespace ScanUtilityLibrary.Core.TripleIN
{
    /// <summary>
    /// 天河Lidar扫描仪UDP通讯类
    /// </summary>
    public class StreamUdpClient
    {
        #region 事件
        /// <summary>
        /// 持续一段时间未接收到任何数据的事件
        /// </summary>
        public event NoneReceivedEventHandler OnNoneReceived;

        /// <summary>
        /// UDP连接事件
        /// </summary>
        public event ConnectedEventHandler Connected;

        /// <summary>
        /// UDP断开事件
        /// </summary>
        public event DisconnectedEventHandler Disconnected;

        /// <summary>
        /// 数据接收事件
        /// </summary>
        public event DataReceivedEventHandler DataReceived;

        ///// <summary>
        ///// 设备登录成功事件
        ///// </summary>
        //public event LoggedInEventHandler LoggedIn;
        #endregion

        #region 私有变量
        private const int ERRORS_COUNT = 10; //储存的错误条数
        private ushort h_num_points_scan; //储存的扫描点数目
        private int h_scan_angle; //扫描角度
        private int h_start_angle; //起始角度
        //点对象数组，假定最极限情况，0°-360°角度范围内每90度角有1000个点，则最大点数为4000
        private readonly ScanPoint[] _scanPoints = new ScanPoint[4000];
        private readonly TimerEventRaiser _noRcvrRaiser = new TimerEventRaiser(1000); //超时未接收触发器

        /// <summary>
        /// 执行看门狗行为的任务，周期性发送心跳
        /// </summary>
        protected readonly BlankTask _watchdogTask = new BlankTask() { Interval = 1000 };
        #endregion

        #region 属性
        #region 连接
        /// <summary>
        /// 是否在发送关闭命令的状态下
        /// </summary>
        public bool SocketCloseCommand { get; set; }

        /// <summary>
        /// Udp服务端的IP
        /// </summary>
        public string ServerIp
        {
            get { return Client.ServerIp; }
            set
            {
                Client.ServerIp = value;
                LocalIp = Functions.GetIPAddressV4Alike(Client.ServerIp);
            }
        }

        /// <summary>
        /// Udp服务端的端口号
        /// </summary>
        public int ServerPort
        {
            get { return Client.ServerPort; }
            set { Client.ServerPort = value; }
        }

        /// <summary>
        /// Udp本地端的IP，自动检测为与服务端IP同网段地址
        /// </summary>
        public string LocalIp
        {
            get { return Client.LocalIp; }
            set { Client.LocalIp = value; }
        }

        /// <summary>
        /// Udp本地端的端口号，默认为1025
        /// </summary>
        public int LocalPort
        {
            get { return Client.LocalPort; }
            set { Client.LocalPort = value; }
        }

        /// <summary>
        /// UdpClient对象
        /// </summary>
        public DerivedUdpClient Client { get; private set; }

        /// <summary>
        /// 是否连接（假如为true则发送报文时不包含服务端IP地址、端口号，否则发送时需指定）
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Udp Socket的真实连接状态
        /// </summary>
        public bool IsConnected_Socket { get { return Client.IsConnected_Socket; } }

        /// <summary>
        /// 连接的错误信息
        /// </summary>
        public string ErrorMsg_UdpConnections { get; set; }

        /// <summary>
        /// 报文解析的错误信息
        /// </summary>
        public string ErrorMsg_Resolving { get; set; }

        /// <summary>
        /// 超时未接收到数据的计时阈值，计时达到此值时触发事件，单位毫秒，默认5000
        /// </summary>
        public int NoneReceivedThreshold
        {
            get { return (int)_noRcvrRaiser.RaiseThreshold; }
            set { _noRcvrRaiser.RaiseThreshold = value < 0 ? 0 : (ulong)value; }
        }
        #endregion

        #region 设备/扫描信息
        ///// <summary>
        ///// 用户名（不超过64个字符）
        ///// </summary>
        //public string UserName
        //{
        //    get { return TipBodyLogin.UserName; }
        //    set { TipBodyLogin.UserName = value; }
        //}

        ///// <summary>
        ///// 密码（不超过64个字符）
        ///// </summary>
        //public string Password
        //{
        //    get { return TipBodyLogin.Password; }
        //    set { TipBodyLogin.Password = value; }
        //}

        /// <summary>
        /// 扫描仪固件版本描述
        /// </summary>
        public string FirmwareVersion { get; private set; }

        /// <summary>
        /// 报文中数据点个数
        /// </summary>
        public ushort NumOfPoints { get { return (ushort)GSCNCommand.CurrScan.NumberOfPoints; } }

        ///// <summary>
        ///// 报文中距离是否使用厘米, true: 厘米, false: 毫米
        ///// </summary>
        //public bool UseCenti { get { return GSCNCommand.TipHead.UnitType == MeasUnitType.Centi_2bytes || GSCNCommand.TipHead.UnitType == MeasUnitType.Centi_4bytes; } }
        ////public bool UseCenti { get; set; }

        //private EchoPulseType _prefPulseType = EchoPulseType.Farthest;
        ///// <summary>
        ///// 扫描点数组中每个点使用的回波脉冲类型（选择双脉冲将默认为输出最远脉冲）
        ///// </summary>
        //public EchoPulseType PrefPulseType
        //{
        //    get { return _prefPulseType; }
        //    set { _prefPulseType = value == EchoPulseType.Both ? EchoPulseType.Farthest : value; }
        //}

        ///// <summary>
        ///// 报文中回波脉冲的类型
        ///// </summary>
        //public EchoPulseType EchoPulseType { get { return GSCNCommand.DataFormat.DataFormatMap.EchoPulseType; } }

        /// <summary>
        /// 报文中是否输出反射率
        /// </summary>
        public bool AlbedoOn { get { return GSCNCommand.CurrScan.Parameters.PARAMETER_DATA_CONTENT == 7 || GSCNCommand.CurrScan.Parameters.PARAMETER_DATA_CONTENT == 8; } }

        /// <summary>
        /// 实际角分辨率（°）
        /// </summary>
        public double AngularIncrement { get; set; }

        /// <summary>
        /// 扫描起始角度（°）
        /// </summary>
        public double StartAngle { get; set; }

        /// <summary>
        /// 扫描结束角度（°）
        /// </summary>
        public double StopAngle { get; set; }

        /// <summary>
        /// 扫描点数组
        /// </summary>
        public ScanPoint[] ScanPoints { get { return _scanPoints; } }
        //public ScanPoint[] ScanPoints = new ScanPoint[25200];
        #endregion

        #region 报文对象
        ///// <summary>
        ///// TIP心跳报文（向设备发送）
        ///// </summary>
        //public TipBodyCommon TipBodyHeartbeat { get; set; }

        ///// <summary>
        ///// TIP登录报文（向设备发送）
        ///// </summary>
        //public TipBodyLogin TipBodyLogin { get; set; }

        /// <summary>
        /// 固件版本报文（向设备发送以及从设备接收）
        /// </summary>
        public GVERCommand GVERCommand { get; set; } = new GVERCommand();

        /// <summary>
        /// SCAN开启或关闭扫描报文（向设备发送）
        /// </summary>
        public SCANCommand SCANCommand { get; set; } = new SCANCommand();

        ///// <summary>
        ///// ERR错误报文（从设备接收）
        ///// </summary>
        //public ERRCommand ERRCommand { get; set; } = new ERRCommand();

        /// <summary>
        /// GSCN数据输出报文（向设备发送以及从设备接收）
        /// </summary>
        public GSCNCommand GSCNCommand { get; set; } = new GSCNCommand();
        #endregion

        #region 故障
        /// <summary>
        /// 最后的若干个错误
        /// </summary>
        public Queue<ERRCommand> LastErrors { get; private set; } = new Queue<ERRCommand>(ERRORS_COUNT);

        ///// <summary>
        ///// 处于被遮挡状态
        ///// </summary>
        //public bool Blocked { get; set; }

        ///// <summary>
        ///// 处于脏污状态
        ///// </summary>
        //public bool Contaminated { get; set; }

        ///// <summary>
        ///// 处于故障状态
        ///// </summary>
        //public bool Error { get; set; }
        #endregion
        #endregion

        /// <summary>
        /// 使用服务端IP地址、端口号初始化StreamUdpClient
        /// </summary>
        /// <param name="serverIp">服务端（扫描仪）IP地址，本地端IP地址自动检测为同网段地址</param>
        /// <param name="serverPort">服务端（扫描仪）端口号</param>
        ///// <param name="prefPulseType">扫描点数组采用的回波脉冲类型</param>
        public StreamUdpClient(/*EchoPulseType prefPulseType = EchoPulseType.Farthest*/string serverIp, int serverPort)
        {
            string localIp = Functions.GetIPAddressV4Alike(serverIp); //本地端IP使用与服务端IP地址相同网段的地址
            int localPort = 1025; //本地端端口号默认1025
            Client = new DerivedUdpClient(localIp, localPort) { ReconnectWhenReceiveNone = true };
            LocalIp = LocalIp;
            LocalPort = localPort;
            if (!string.IsNullOrWhiteSpace(serverIp))
                ServerIp = serverIp;
            if (serverPort > 0 && serverPort < 65535)
                ServerPort = serverPort;

            //超时未接收数据的触发器，无论是否连接都一直运行
            _noRcvrRaiser.RaiseThreshold = 10000;
            _noRcvrRaiser.RaiseInterval = 5000;
            _noRcvrRaiser.ThresholdReached += new ThresholdReachedEventHandler(NoneReceived_ThresholdReached);
            _noRcvrRaiser.Run();

            Client.DataReceived += new DataReceivedEventHandler(Client_DataReceived);
            Client.Connected += new ConnectedEventHandler(Client_Connected);
            Client.Disconnected += new DisconnectedEventHandler(Client_Disconnected);
            _watchdogTask.ContentLooped += new TaskContentLoopedEventHandler(WatchdogTask_ContentLooped);
        }

        #region 功能
        /// <summary>
        /// 使用当前设定的IP、端口、用户名和密码与TCP服务端连接
        /// </summary>
        /// <returns>假如成功，返回0，否则返回1</returns>
        public int Connect()
        {
            return Connect(ServerIp, ServerPort);
        }

        /// <summary>
        /// 利用特定的IP、端口与TCP服务端连接<para/>
        /// 假如连接则发送报文时不包含服务端IP地址、端口号，否则发送时需指定
        /// </summary>
        /// <param name="server">TCP服务端IP</param>
        /// <param name="port">端口号</param>
        /// <returns>假如成功，返回0，否则返回1</returns>
        public int Connect(string server, int port)
        {
            ServerIp = server;
            ServerPort = port;
            IsConnected = false;
            ErrorMsg_UdpConnections = string.Empty;
            try { Client.Connect(); }
            catch (Exception e)
            {
                ErrorMsg_UdpConnections = string.Format("无法连接到SOCKET {0}: {1}", Client.Name, e.ToString());
                return 1;
            }
            IsConnected = true;
            return 0;
        }

        /// <summary>
        /// 关闭TCP连接
        /// </summary>
        public void Close()
        {
            ErrorMsg_UdpConnections = string.Empty;
            IsConnected = false;
            try { Client.Close(); }
            catch (Exception e)
            {
                ErrorMsg_UdpConnections = string.Format("与SOCKET {0} 断开失败: {1}", Client.Name, e.ToString());
            }
            return;
        }

        /// <summary>
        /// 根据16进制字符串解析报文数据
        /// </summary>
        /// <param name="hexString"></param>
        private void ResolveCommands(string hexString)
        {
            if (string.IsNullOrWhiteSpace(hexString))
                return;
            ResolveCommands(HexHelper.HexString2Bytes(hexString));
        }

        /// <summary>
        /// 根据byte数组解析TIP数据
        /// </summary>
        /// <param name="bytes"></param>
        private void ResolveCommands(byte[] bytes)
        {
            var code = CommandBase.ResolveFuncCode(bytes);
            switch (code)
            {
                case FunctionCodes.GPRM:
                    break;
                case FunctionCodes.SPRM:
                    break;
                case FunctionCodes.GRTC:
                    break;
                case FunctionCodes.GPIN:
                    break;
                case FunctionCodes.SYNC:
                    break;
                case FunctionCodes.ERR:
                    var error = new ERRCommand(bytes);
                    //假如出现索引超出范围的错误，可能是发送GSCN时指定的ScanNumber过大，重置为0
                    if (error.Error == ErrorID.ERR_INDEX_OUT_OF_RANGE)
                        GSCNCommand.ScanNumber = 0;
                    LastErrors.Enqueue(new ERRCommand(bytes));
                    if (LastErrors.Count > ERRORS_COUNT)
                        LastErrors.Dequeue();
                    break;
                case FunctionCodes.GVER:
                    GVERCommand.Resolve(bytes);
                    FirmwareVersion = GVERCommand.Version;
                    break;
                case FunctionCodes.GSCN:
                    GSCNCommand.Resolve(bytes);
                    break;
            }
        }

        /// <summary>
        /// 刷新扫描点信息
        /// </summary>
        public void RecDataStream()
        {
            while (true)
            {
                Thread.Sleep(10);
                //循环发送GSCN命令，在连接的情况下不指定服务端地址，不连接时指定服务端的ip和端口号，假如发生错误则记录错误信息
                //TODO 模仿ScanUtilityLibrary.Core.SICK.Dx.StreamTcpClient，做循环发送命令的方法
                //发送GSCN时指定ScanNumber为0，防止出现索引超出范围的错误
                GSCNCommand.ScanNumber = 0;
                string errorMessage, gscn = GSCNCommand.ComposeHexString();
                try { bool result = IsConnected ? Client.SendData(gscn, out errorMessage) : Client.SendData(gscn, ServerIp, ServerPort, out errorMessage); }
                //try { Client.SendData(GSCNCommand.ComposeHexString(), out errorMessage); }
                catch (Exception e) { errorMessage = "GSCN命令发送失败：" + e.Message + "\r\n" + e.StackTrace; }
                if (!string.IsNullOrWhiteSpace(errorMessage))
                    ErrorMsg_UdpConnections = errorMessage;
                try
                {
                    int count = GSCNCommand.CurrScan.ScanDatas.GetLength(0); //每个回波的扫描样本点的数量
                    //if (GSCNCommand.CurrScan.ScanDatas.Length == 0)
                    if (count == 0 || GSCNCommand.CurrScan.Parameters.PARAMETER_NUMBER_OF_ECHOES == 0)
                        continue;
                    //if (h_num_points_scan != NumOfPoints || h_start_angle != GSCNCommand.CurrScan.Parameters.PARAMETER_SCAN_START_ANGLE || h_angl_resltn != GSCNCommand.CurrScan.AngleResolution)
                    if (h_num_points_scan != NumOfPoints || h_start_angle != GSCNCommand.CurrScan.Parameters.PARAMETER_SCAN_START_ANGLE || h_scan_angle != GSCNCommand.CurrScan.Parameters.PARAMETER_SCAN_ANGLE)
                    {
                        h_num_points_scan = NumOfPoints;
                        h_start_angle = GSCNCommand.CurrScan.Parameters.PARAMETER_SCAN_START_ANGLE;
                        //h_angl_resltn = GSCNCommand.DataFormat.AngleResolution;
                        h_scan_angle = GSCNCommand.CurrScan.Parameters.PARAMETER_SCAN_ANGLE;
                        StartAngle = Math.Round((double)h_start_angle / 1000, 3);
                        StopAngle = Math.Round(((double)h_start_angle + h_scan_angle) / 1000, 3);
                        AngularIncrement = (double)h_scan_angle / h_num_points_scan / 1000; //不要舍入，防止计算出现误差

                        for (int i = 0; i < h_num_points_scan; i++)
                        {
                            if (ScanPoints[i] == null)
                                ScanPoints[i] = new ScanPoint(ScanDeviceType.LMS, 0, 0, 0, 0);
                            var point = ScanPoints[i];
                            point.Distance = 0;
                            point.EchoValue = 0;
                            point.Angle = StartAngle + AngularIncrement * i;
                        }
                    }

                    DateTime time = DateTime.Now;//数据提取时间为当下系统时间
                    for (int i = 0; i < h_num_points_scan; i++)
                    {
                        //假如出于某种原因测量数据的列表的长度小于当前应处理的数目，则跳出循环（处理结束）
                        if (count <= i)
                            break;
                        //var measGroup = GSCNCommand.CurrScan.ScanDatas[i, GSCNCommand.CurrScan.Parameters.PARAMETER_NUMBER_OF_ECHOES - 1]; //最后一个回波的点
                        var measGroup = GSCNCommand.CurrScan.ScanDatas[i, 0]; //第一个回波的点
                        var point = ScanPoints[i];
                        int dist = measGroup.Distance == int.MinValue || measGroup.Distance == int.MaxValue ? 0 : measGroup.Distance / 10; //测量距离，假如回波信号过低或过高则置为0
                        point.Distance = (uint)dist; //假如测距打到最大距离，则置为0
                        point.EchoValue = (uint)measGroup.PulseWidth;
                        point.ScannedTime = time;//输出时间
                    }

                    if (SocketCloseCommand)
                    {
                        SocketCloseCommand = false;
                        Close();
                        continue;
                    }
                }
                catch (Exception e)
                {
                    ErrorMsg_UdpConnections = string.Format("处理SOCKET {0} 返回的扫描点信息时出现错误: {1}", Client.Name, e.ToString());
                }
            }
        }

        /// <summary>
        /// 获取TCP信息抬头
        /// </summary>
        /// <returns></returns>
        public string GetUdpHeader()
        {
            List<string> list = new List<string>
            {
                "本地IP：" + LocalIp,
                "本地端口号：" + LocalPort,
                "校验通过：" + GSCNCommand.ChecksumPassed,
                "处理时间：" + GSCNCommand.CurrScan.ParsedTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                //"时间戳：" + GSCNCommand.CurrScan.Parameters.PARAMETER_TIME_STAMP,
                "运行时间（天.时:分:秒.毫秒）：" + GSCNCommand.CurrScan.RunningTime.ToString("c"),
                //"雷达型号：PSXXX-270",
                "雷达线数：" + GSCNCommand.CurrScan.Parameters.PARAMETER_SCAN_LINE,
                "输出反射率：" + GSCNCommand.CurrScan.Parameters.PARAMETER_DATA_CONTENT.Between(7, 8),
                "扫描点个数：" + NumOfPoints,
                //"实际获得扫描点：" + GSCNCommand.CurrScan.ScanDatas.GetLength(0),
                "角分辨率：" + Math.Round(AngularIncrement, 3),
                "起始角度：" + StartAngle,
                "终止角度：" + StopAngle,
                string.Empty,
                "UDP是否已连接 = " + (IsConnected && Client.IsConnected_Socket),
                "UDP连接状态: " + ErrorMsg_UdpConnections
            };

            //return txt;
            return string.Join("\r\n", list);
        }

        /// <summary>
        /// 获取扫描点信息（前36个坐标点）的字符串形式
        /// </summary>
        /// <returns></returns>
        public string GetScanDataString()
        {
            return GetScanDataString(0);
        }


        /// <summary>
        /// 获取扫描点信息（从指定索引处开始的36个坐标点）的字符串形式
        /// </summary>
        /// <param name="startValue">起始坐标点索引</param>
        /// <returns></returns>
        public string GetScanDataString(int startValue)
        {
            if (startValue > h_num_points_scan - 36)
                startValue = h_num_points_scan - 36;
            if (startValue < 0)
                startValue = 0;

            List<string> list = new List<string> { string.Format("扫描数据点 {0}-{1}:", startValue, startValue + 35) };
            //string txt = "扫描数据点 " + startValue + "-" + (startValue + 35) + ":" + (char)13 + (char)10;
            if (ScanPoints == null || ScanPoints.Length < startValue + 36)
                //return txt;
                goto END;

            #region 显示X、Y、Z轴坐标
            string blank = "        ";
            //string blank = string.Empty + '\t';
            list.Add(string.Format("起始数据点:{0}x:{0}y:{0}echo:", blank));
            //txt += "起始数据点:        x:          y:          echo:" + (char)13 + (char)10;
            for (int i = startValue; i < startValue + 36; i++)
            {
                if (ScanPoints[i] == null)
                    continue;
                bool fault = ScanPoints[i].Distance == 0xFFFFFFFF;
                string text = string.Format("{1:00000}{0}{2}{0}{3}{0}{4}", blank, i, fault ? "Error" : ScanPoints[i].X.ToString("000"), fault ? "Error" : ScanPoints[i].Y.ToString("000"), fault ? "Error" : ScanPoints[i].Z.ToString("000"));
                list.Add(text);
            }
        #endregion

        END:
            //return txt;
            return string.Join("\r\n", list);
        }
        #endregion

        #region 事件
        private void NoneReceived_ThresholdReached(object sender, ThresholdReachedEventArgs e)
        {
            OnNoneReceived?.Invoke(this, new NoneReceivedEventArgs((ulong)NoneReceivedThreshold)); //调用超时未接收的事件委托
            //if (!ReconnectWhenReceiveNone)
            //    return;

            //Reconnect();
        }
        private void Client_DataReceived(object sender, DataReceivedEventArgs eventArgs)
        {
            DataReceived?.Invoke(this, eventArgs);
            _noRcvrRaiser.Click();
            try { ResolveCommands(eventArgs.ReceivedInfo_HexString); }
            catch (Exception e) { ErrorMsg_Resolving = "报文解析出现问题：" + e.Message + "\r\n" + e.StackTrace; }
        }

        private void Client_Connected(object sender, EventArgs eventArgs)
        {
            Connected?.Invoke(this, eventArgs);
        }

        private void Client_Disconnected(object sender, EventArgs eventArgs)
        {
            Disconnected?.Invoke(this, eventArgs);
        }

        private void WatchdogTask_ContentLooped(object sender, TaskContentLoopedEventArgs e)
        {
            //SendHeartbeat();
        }
        #endregion
    }
}
