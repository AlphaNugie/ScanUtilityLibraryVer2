using CommonLib.Clients.Tasks;
using CommonLib.Extensions;
using CommonLib.Function;
using ScanUtilityLibrary.Model;
using ScanUtilityLibrary.Model.Tianhe;
using SocketHelper;
using SocketHelper.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using static CommonLib.Clients.Tasks.Task;
using static ScanUtilityLibrary.Event.EventHandlers;
using static SocketHelper.Event.EventHandlers;

namespace ScanUtilityLibrary.Core.Tianhe
{
    /// <summary>
    /// 天河Lidar扫描仪TCP通讯类
    /// </summary>
    public class StreamTcpClient
    {
        //private ushort h_num_points_scan = 361;
        private ushort h_num_points_scan, h_angl_resltn; //储存的扫描点数目、角度分辨率原始值
        private int h_start_angle;
        //private bool IsScanRead = false;
        //private string _received = string.Empty, _wrapped = string.Empty;
        //点对象数组，假定最极限情况，0°-360°角度范围内每2度角有125个点，则最大点数为360*62.5=22500
        private readonly ScanPoint[] _scanPoints = new ScanPoint[22500];
        private readonly SocketTcpClient _client = new SocketTcpClient() { ReceiveBufferSize = 65535, ReconnectWhenReceiveNone = true };

        /// <summary>
        /// 执行看门狗行为的任务，周期性发送心跳
        /// </summary>
        protected readonly BlankTask _watchdogTask = new BlankTask() { Interval = 1000 };

        #region 事件
        /// <summary>
        /// 连接状态改变时返回连接状态事件
        /// </summary>
        public event StateInfoEventHandler OnStateInfo;

        /// <summary>
        /// 数据接收事件
        /// </summary>
        public event ReceivedEventHandler OnReceive;

        /// <summary>
        /// 设备登录成功事件
        /// </summary>
        public event LoggedInEventHandler LoggedIn;
        #endregion

        #region 属性
        #region 连接
        /// <summary>
        /// 是否在发送关闭命令的状态下
        /// </summary>
        public bool SocketCloseCommand { get; set; }

        /// <summary>
        /// Tcp服务端的IP
        /// </summary>
        public string ServerIp
        {
            get { return _client.ServerIp; }
            set { _client.ServerIp = value; }
        }

        /// <summary>
        /// Tcp服务端的端口号
        /// </summary>
        public int ServerPort
        {
            get { return _client.ServerPort; }
            set { _client.ServerPort = value; }
        }

        /// <summary>
        /// TcpClient对象
        /// </summary>
        public SocketTcpClient Client { get { return _client; } }

        /// <summary>
        /// 是否连接
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Tcp Socket的真实连接状态
        /// </summary>
        public bool IsConnected_Socket { get { return Client.IsConnected_Socket; } }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg_TcpConnections { get; set; }

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

        #region 设备/扫描信息
        /// <summary>
        /// 用户名（不超过64个字符）
        /// </summary>
        public string UserName
        {
            get { return TipBodyLogin.UserName; }
            set { TipBodyLogin.UserName = value; }
        }

        /// <summary>
        /// 密码（不超过64个字符）
        /// </summary>
        public string Password
        {
            get { return TipBodyLogin.Password; }
            set { TipBodyLogin.Password = value; }
        }

        /// <summary>
        /// 报文中数据点个数
        /// </summary>
        public ushort NumOfPoints { get { return TipBodyDataOutput.TipHead.NumOfPoints; } }
        //public ushort NumOfPoints { get { return (ushort)TipBodyDataOutput.MeasDataGroups.Count; } }
        //public ushort NumOfPoints { get { return Math.Min(TipBodyDataOutput.TipHead.NumOfPoints, (ushort)TipBodyDataOutput.MeasDataGroups.Count); } }

        /// <summary>
        /// 报文中距离是否使用厘米, true: 厘米, false: 毫米
        /// </summary>
        public bool UseCenti { get { return TipBodyDataOutput.TipHead.UnitType == MeasUnitType.Centi_2bytes || TipBodyDataOutput.TipHead.UnitType == MeasUnitType.Centi_4bytes; } }
        //public bool UseCenti { get; set; }

        private EchoPulseType _prefPulseType = EchoPulseType.Farthest;
        /// <summary>
        /// 扫描点数组中每个点使用的回波脉冲类型（选择双脉冲将默认为输出最远脉冲）
        /// </summary>
        public EchoPulseType PrefPulseType
        {
            get { return _prefPulseType; }
            set { _prefPulseType = value == EchoPulseType.Both ? EchoPulseType.Farthest : value; }
        }

        /// <summary>
        /// 报文中回波脉冲的类型
        /// </summary>
        public EchoPulseType EchoPulseType { get { return TipBodyDataOutput.DataFormat.DataFormatMap.EchoPulseType; } }

        /// <summary>
        /// 报文中是否输出反射率
        /// </summary>
        public bool AlbedoOn { get { return TipBodyDataOutput.DataFormat.DataFormatMap.AlbedoOn; } }

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

        #region TIP报文对象
        /// <summary>
        /// TIP心跳报文（向设备发送）
        /// </summary>
        public TipBodyCommon TipBodyHeartbeat { get; set; }

        /// <summary>
        /// TIP登录报文（向设备发送）
        /// </summary>
        public TipBodyLogin TipBodyLogin { get; set; }

        /// <summary>
        /// TIP数据输出报文（从设备接收）
        /// </summary>
        public TipBodyDataOutput TipBodyDataOutput { get; set; }
        #endregion

        #region 故障
        /// <summary>
        /// 处于被遮挡状态
        /// </summary>
        public bool Blocked { get; set; }

        /// <summary>
        /// 处于脏污状态
        /// </summary>
        public bool Contaminated { get; set; }

        /// <summary>
        /// 处于故障状态
        /// </summary>
        public bool Error { get; set; }
        #endregion
        #endregion

        /// <summary>
        /// 使用给定的希望使用的回波脉冲类型初始化
        /// </summary>
        /// <param name="prefPulseType">扫描点数组采用的回波脉冲类型</param>
        public StreamTcpClient(EchoPulseType prefPulseType = EchoPulseType.Farthest)
        {
            Client.OnReceive += new ReceivedEventHandler(SocketTcpClient_OnReceive);
            Client.OnStateInfo += new StateInfoEventHandler(SocketTcpClient_OnStateInfo);
            _watchdogTask.ContentLooped += new TaskContentLoopedEventHandler(WatchdogTask_ContentLooped);
            TipBodyHeartbeat = new TipBodyCommon(TipCode.Heartbeat);
            TipBodyLogin = new TipBodyLogin();
            TipBodyLogin.RestoreDefUserInfos();
            TipBodyDataOutput = new TipBodyDataOutput();
            PrefPulseType = prefPulseType;
        }

        ///// <summary>
        ///// TCP服务端连接
        ///// </summary>
        ///// <param name="server">IP地址</param>
        ///// <param name="port">端口号</param>
        //public StreamTcpClient(string server, int port)
        //{
        //    Connect(server, port);
        //}

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
        /// 利用特定的IP、端口与TCP服务端连接
        /// </summary>
        /// <param name="server">TCP服务端IP</param>
        /// <param name="port">端口号</param>
        /// <param name="userName">用户名，假如为null/空/空白字符串则使用上一次定义的值，假如从未定义过则使用默认值</param>
        /// <param name="password">密码，假如为null/空/空白字符串则使用上一次定义的值，假如从未定义过则使用默认值</param>
        /// <returns>假如成功，返回0，否则返回1</returns>
        public int Connect(string server, int port, string userName = "", string password = "")
        {
            ServerIp = server;
            ServerPort = port;
            //假如用户名不为空，则更新
            if (!string.IsNullOrWhiteSpace(UserName))
                UserName = userName;
            //假如密码不为空，则更新
            if (!string.IsNullOrWhiteSpace(password))
                Password = password;
            IsConnected = false;
            ErrorMsg_TcpConnections = string.Empty;
            //if (string.IsNullOrWhiteSpace(port))
            //    return (1);
            try { Client.StartConnection(); }
            catch (Exception e)
            {
                ErrorMsg_TcpConnections = string.Format("无法连接到SOCKET {0}: {1}", Client.Name, e.ToString());
                return 1;
            }
            IsConnected = true;
            return 0;
        }

        /// <summary>
        /// 登录
        /// </summary>
        public void Login()
        {
            Client.SendData(TipBodyLogin.Compose());
            _watchdogTask.Run();
        }

        /// <summary>
        /// 发送心跳数据
        /// </summary>
        public void SendHeartbeat()
        {
            Client.SendData(TipBodyHeartbeat.Compose());
        }

        /// <summary>
        /// 关闭TCP连接
        /// </summary>
        public void Close()
        {
            ErrorMsg_TcpConnections = string.Empty;
            IsConnected = false;
            try { Client.StopConnection(); }
            catch (Exception e)
            {
                ErrorMsg_TcpConnections = string.Format("与SOCKET {0} 断开失败: {1}", Client.Name, e.ToString());
            }
            return;
        }

        /// <summary>
        /// 根据16进制字符串解析TIP数据
        /// </summary>
        /// <param name="hexString"></param>
        private void ResolveTipBody(string hexString)
        {
            if (string.IsNullOrWhiteSpace(hexString))
                return;
            ResolveTipBody(HexHelper.HexString2Bytes(hexString));
        }

        /// <summary>
        /// 根据byte数组解析TIP数据
        /// </summary>
        /// <param name="bytes"></param>
        private void ResolveTipBody(byte[] bytes)
        {
            var code = TipHeadBase.ResolveTipCode(bytes);
            switch (code)
            {
                case TipCode.HeartbeatReply:
                    break;
                case TipCode.LoginReply:
                    LoggedIn?.Invoke(this, new EventArgs());
                    break;
                case TipCode.Blocked:
                    Blocked = true;
                    break;
                case TipCode.BlockedNoMore:
                    Blocked = false;
                    break;
                case TipCode.Error:
                    Error = true;
                    break;
                case TipCode.ErrorNoMore:
                    Error = false;
                    break;
                case TipCode.Contaminated:
                    Contaminated = true;
                    break;
                case TipCode.ContaminatedNoMore:
                    Contaminated = false;
                    break;
                case TipCode.DataOutput:
                    TipBodyDataOutput.Resolve(bytes);
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
                Thread.Sleep(1);
                try
                {
                    //_wrapped = _received;
                    //if (!ProcessUtil.GetWrappedMessage(ref _wrapped, out List<string> msgList))
                    //    continue;
                    //_received = string.Empty;
                    //ResolveTipBody(_wrapped);
                    if (TipBodyDataOutput.MeasDataGroups.Count == 0)
                        continue;
                    //if (h_num_points_scan != NumOfPoints)
                    //if (h_num_points_scan != TipBodyDataOutput.MeasDataGroups.Count || h_start_angle != TipBodyDataOutput.DataFormat.StartAngle || h_angl_resltn != TipBodyDataOutput.DataFormat.AngleResolution)
                    if (h_num_points_scan != NumOfPoints || h_start_angle != TipBodyDataOutput.DataFormat.StartAngle || h_angl_resltn != TipBodyDataOutput.DataFormat.AngleResolution)
                    {
                        h_num_points_scan = NumOfPoints;
                        //h_num_points_scan = (ushort)TipBodyDataOutput.MeasDataGroups.Count;
                        h_start_angle = TipBodyDataOutput.DataFormat.StartAngle;
                        h_angl_resltn = TipBodyDataOutput.DataFormat.AngleResolution;
                        //UseCenti = TipBodyDataOutput.TipHead.UnitType == MeasUnitType.Centi_2bytes || TipBodyDataOutput.TipHead.UnitType == MeasUnitType.Centi_4bytes;
                        //StartAngle = (double)TipBodyDataOutput.DataFormat.StartAngle / 1000;
                        StartAngle = Math.Round((double)h_start_angle / 1000, 3);
                        StopAngle = Math.Round((double)TipBodyDataOutput.DataFormat.StopAngle / 1000, 3);
                        switch (TipBodyDataOutput.DataFormat.DataFormatMap.AnglResltnType)
                        {
                            case AngleResolutionType.MilliDeg:
                                AngularIncrement = (double)h_angl_resltn / 1000;
                                break;
                            case AngleResolutionType.MilliDegDivBy100:
                                AngularIncrement = (double)h_angl_resltn / 1000 / 100;
                                break;
                            case AngleResolutionType.TotalPoints360:
                                AngularIncrement = 360 / (double)h_angl_resltn;
                                break;
                        }
                        //AngularIncrement = Math.Round(AngularIncrement, 3);

                        //for (int i = 0; i < ScanPoints.Length; i++)
                        for (int i = 0; i < h_num_points_scan; i++)
                        {
                            if (ScanPoints[i] == null)
                                ScanPoints[i] = new ScanPoint(ScanDeviceType.LMS, 0, 0, 0, 0);
                            //ScanPoints[i].Distance = 0;
                            //ScanPoints[i].EchoValue = 0;
                            //ScanPoints[i].Angle = StartAngle + AngularIncrement * i;
                            var point = ScanPoints[i];
                            point.Distance = 0;
                            point.EchoValue = 0;
                            point.Angle = StartAngle + AngularIncrement * i;
                            //if (i < h_num_points_scan)
                            //    ScanPoints[i].Angle = StartAngle + AngularIncrement * i;
                            //else
                            //    ScanPoints[i].Angle = 0;
                        }
                    }

                    DateTime time = DateTime.Now;//数据提取时间为当下系统时间
                    for (int i = 0; i < h_num_points_scan; i++)
                    {
                        //假如出于某种原因测量数据的列表的长度小于当前应处理的数目，则跳出循环（处理结束）
                        if (TipBodyDataOutput.MeasDataGroups.Count <= i)
                            break;
                        var measGroup = TipBodyDataOutput.MeasDataGroups[i];
                        //ScanPoints[i].Distance = (PrefPulseType == EchoPulseType.Farthest ? measGroup.PulseFarthest : measGroup.PulseStrongest) * (UseCenti ? 10u : 1); //测量距离，假如单位为厘米则乘以10
                        //ScanPoints[i].EchoValue = PrefPulseType == EchoPulseType.Farthest ? measGroup.AlbedoFarthest : measGroup.AlbedoStrongest;
                        //ScanPoints[i].ScannedTime = time;//输出时间
                        var point = ScanPoints[i];
                        uint dist = (PrefPulseType == EchoPulseType.Farthest ? measGroup.PulseFarthest : measGroup.PulseStrongest) * (UseCenti ? 10u : 1); //测量距离，假如单位为厘米则乘以10
                        point.Distance = dist == 120010 ? 0 : dist; //假如测距打到最大距离，则置为0
                        point.EchoValue = PrefPulseType == EchoPulseType.Farthest ? measGroup.AlbedoFarthest : measGroup.AlbedoStrongest;
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
                    ErrorMsg_TcpConnections = string.Format("处理SOCKET {0} 返回的扫描点信息时出现错误: {1}", Client.Name, e.ToString());
                }
            }
        }

        /// <summary>
        /// 获取TCP信息抬头
        /// </summary>
        /// <returns></returns>
        public string GetTcpHeader()
        {
            List<string> list = new List<string>
            {
                "校验通过：" + TipBodyDataOutput.TipHead.ChecksumPassed,
                "TIP创建时间：" + TipBodyDataOutput.TipHead.CreatedTime.ToString("yyyy-MM-dd HH:mm:ss"),
                "雷达型号：" + TipBodyDataOutput.DataFormat.RadarModel,
                "雷达线数：" + TipBodyDataOutput.DataFormat.LineNum,
                "总包数：" + TipBodyDataOutput.DataFormat.PackSum,
                "扫描包序号：" + TipBodyDataOutput.DataFormat.PackNo,
                "数据单位：" + TipBodyDataOutput.TipHead.UnitType.GetDescription(),
                "角分辨率类型：" + TipBodyDataOutput.DataFormat.DataFormatMap.AnglResltnType.GetDescription(),
                "脉冲类型：" + TipBodyDataOutput.TipHead.EchoPulseType.GetDescription(),
                "输出反射率：" + AlbedoOn,
                "扫描点个数：" + NumOfPoints,
                "实际获得扫描点：" + TipBodyDataOutput.MeasDataGroups.Count,
                //"角分辨率：" + AngularIncrement,
                "角分辨率：" + Math.Round(AngularIncrement, 3),
                "起始角度：" + StartAngle,
                "终止角度：" + StopAngle,
                string.Empty,
                //"TCP是否已连接 = " + IsConnected,
                "TCP是否已连接 = " + (IsConnected && Client.IsConnected_Socket),
                "TCP连接状态: " + ErrorMsg_TcpConnections
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
            //if (startValue < 0)
            //    startValue = 0;
            //else if (startValue > 25164)
            //    startValue = 25164;
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
        private void SocketTcpClient_OnReceive(object sender, ReceivedEventArgs args)
        {
            //if (_received.Length > 1024 * 1024)
            //    _received = string.Empty;
            //_received += args.ReceivedHexString;
            OnReceive?.Invoke(this, args);
            ResolveTipBody(args.ReceivedHexString);
        }

        private void SocketTcpClient_OnStateInfo(object sender, StateInfoEventArgs eventArgs)
        {
            OnStateInfo?.Invoke(this, eventArgs);
            if (eventArgs.State == SocketState.Connected)
                Login();
        }

        private void WatchdogTask_ContentLooped(object sender, TaskContentLoopedEventArgs e)
        {
            SendHeartbeat();
        }
        #endregion
    }
}
