using CommonLib.Clients;
using CommonLib.Events;
using CommonLib.Function;
using ScanUtilityLibrary.Core.SICK.Scanner;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static CommonLib.Function.TimerEventRaiser;

namespace ScanUtilityLibrary.Core.SICK
{
    /// <summary>
    /// 西克（SICK）设备基础TCP通讯类
    /// </summary>
    public abstract class BaseTcpClient
    {
        #region 事件
        /// <summary>
        /// 持续一段时间未接收到任何数据的事件
        /// </summary>
        public event NoneReceivedEventHandler OnNoneReceived;

        /// <summary>
        /// 数据接收事件
        /// </summary>
        public event CommonLib.Events.DataReceivedEventHandler DataReceived;
        #endregion

        #region 私有变量
        private readonly TimerEventRaiser _noRcvrRaiser = new TimerEventRaiser(1000); //超时未接收触发器
        #endregion

        #region 属性
        /// <summary>
        /// 线程启动器
        /// </summary>
        protected AutoResetEvent ResetEvent { get; set; }

        /// <summary>
        /// 超时未接收触发器
        /// </summary>
        public TimerEventRaiser NoRcvrRaiser { get { return _noRcvrRaiser; } }

        /// <summary>
        /// 接收数据的字符串格式
        /// </summary>
        public string ReceivedData { get; set; }

        /// <summary>
        /// 接收到的消息
        /// </summary>
        public string ReceivedMessage { get; set; }

        /// <summary>
        /// Tcp服务端的IP
        /// </summary>
        public string ServerIp { get; private set; }

        /// <summary>
        /// Tcp服务端的端口号
        /// </summary>
        public ushort ServerPort { get; private set; }

        /// <summary>
        /// 是否连接
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Tcp Socket的真实连接状态
        /// </summary>
        public bool IsConnected_Socket { get { return Client != null && Client.IsConnected_Socket; } }

        /// <summary>
        /// 指示是否即将结束接收数据
        /// </summary>
        public bool IsReceiving { get; set; }

        /// <summary>
        /// TcpClient对象
        /// </summary>
        public DerivedTcpClient Client { get; private set; }

        /// <summary>
        /// 网络访问数据流对象
        /// </summary>
        public NetworkStream NetStream { get; private set; }

        /// <summary>
        /// 命令发送接收对象
        /// </summary>
        public BaseCmdSender CommandSender { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 数据获取超时时间
        /// </summary>
        public long DataFetchTimeout { get; set; }

        /// <summary>
        /// 数据获取超时时间阈值
        /// </summary>
        public long DataFetchTimeoutThres { get; set; }

        /// <summary>
        /// 是否数据获取超时
        /// </summary>
        public bool IsDataFetchTimeOut { get; set; }

        /// <summary>
        /// 超时未接收到数据的计时阈值，计时达到此值时触发事件，单位毫秒，默认5000
        /// </summary>
        public int NoneReceivedThreshold
        {
            get { return (int)_noRcvrRaiser.RaiseThreshold; }
            set { _noRcvrRaiser.RaiseThreshold = value < 0 ? 0 : (ulong)value; }
        }

        /// <summary>
        /// 在无返回数据时是否重新连接
        /// </summary>
        public bool ReconnectWhenReceiveNone { get; set; }

        /// <summary>
        /// 重新连接计数
        /// </summary>
        public int ReconnCounter { get; set; }
        #endregion

        /// <summary>
        /// 析构函数
        /// </summary>
        ~BaseTcpClient()
        {
            Close();
        }

        /// <summary>
        /// 默认构造器
        /// </summary>
        protected BaseTcpClient() : this(string.Empty, string.Empty) { }

        /// <summary>
        /// TCP服务端连接
        /// </summary>
        /// <param name="server">IP地址</param>
        /// <param name="port">端口号</param>
        protected BaseTcpClient(string server, string port)
        {
            //Client = new DerivedTcpClient();
            InitTcpClient();
            DataFetchTimeoutThres = 120 * 1000;
            ResetEvent = new AutoResetEvent(true);
            //if (!string.IsNullOrWhiteSpace(server) && !string.IsNullOrWhiteSpace(port))
            //    Connect(server, port);
            ServerIp = server;
            try { ServerPort = ushort.Parse(port); }
            catch (Exception) { }
            //超时未接收数据的触发器，无论是否连接都一直运行
            _noRcvrRaiser.RaiseThreshold = 10000;
            _noRcvrRaiser.RaiseInterval = 5000;
            _noRcvrRaiser.ThresholdReached += new ThresholdReachedEventHandler(NoneReceived_ThresholdReached);
            _noRcvrRaiser.Run();
        }

        #region 抽象方法
        /// <summary>
        /// 初始化初始化命令通讯对象
        /// </summary>
        protected abstract void InitCmdSender();

        /// <summary>
        /// 处理接收到的消息
        /// </summary>
        /// <param name="content"></param>
        public abstract void AnalyzeReceivedMessage(string content);

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="message"></param>
        public abstract void ResolveData(string message);

        /// <summary>
        /// 进行重连后的后续处理
        /// </summary>
        protected abstract void ReconnectUrself();
        #endregion

        #region 功能
        #region 连接
        /// <summary>
        /// 初始化TCP客户端
        /// </summary>
        private void InitTcpClient()
        {
            Client = new DerivedTcpClient()/* { ReconnectWhenReceiveNone = true }*/;
            //Client.OnNoneReceived += new NoneReceivedEventHandler(Client_OnNoneReceived);
        }

        /// <summary>
        /// 使用默认的IP地址和端口连接
        /// </summary>
        /// <returns></returns>
        public int Connect()
        {
            return Connect(ServerIp, ServerPort);
        }

        /// <summary>
        /// 利用特定的IP与端口与TCP服务端连接
        /// </summary>
        /// <param name="server">TCP服务端IP</param>
        /// <param name="portstr">端口号</param>
        /// <returns>假如成功，返回0，否则返回1</returns>
        public int Connect(string server, string portstr)
        {
            ErrorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(portstr))
            {
                ErrorMessage = "端口号为空";
                return 1;
            }
            ushort port = 2111; //默认端口
            try { port = ushort.Parse(portstr); }
            catch (Exception e)
            {
                ErrorMessage = string.Format("端口号不正确，应为0-65535的整数，将使用默认端口{0}：{1}", port, e.Message);
                //return 1;
            }

            return Connect(server, port);
        }

        /// <summary>
        /// 利用特定的端口与TCP服务端连接
        /// </summary>
        /// <param name="server">TCP服务端IP</param>
        /// <param name="port">端口号</param>
        /// <returns>假如成功，返回0，否则返回1</returns>
        public int Connect(string server, ushort port)
        {
            int result = ObscureConnect(server, port);
            if (result == 0)
                InitCmdSender();
            return result;
        }

        /// <summary>
        /// 与已配置了IP地址和端口号的TCP服务端连接
        /// </summary>
        /// <returns>假如成功，返回0，否则返回1</returns>
        internal int ObscureConnect()
        {
            return ObscureConnect(ServerIp, ServerPort);
        }

        /// <summary>
        /// 与给出了IP地址和端口号的TCP服务端连接
        /// </summary>
        /// <param name="server">TCP服务端IP</param>
        /// <param name="port">端口号</param>
        /// <returns>假如成功，返回0，否则返回1</returns>
        internal int ObscureConnect(string server, ushort port)
        {
            if (string.IsNullOrWhiteSpace(server) || port == 0)
            {
                ErrorMessage = "IP地址为空或端口号为0";
                return 1;
            }

            ServerIp = server;
            ServerPort = port;
            IsConnected = false;
            ErrorMessage = string.Empty;
            if (Client == null)
                //Client = new DerivedTcpClient();
                InitTcpClient();
            if (!Client.IsConnected)
            {
                try
                {
                    //Client = new DerivedTcpClient();
                    Client.ReceiveBufferSize = 65535;
                    Client.ReconnectWhenReceiveNone = true;
                    Client.Connect(ServerIp, ServerPort);
                    Client.DataReceived += new CommonLib.Events.DataReceivedEventHandler(Client_DataReceived);
                    NetStream = Client.BaseClient.GetStream();
                }
                catch (SocketException e)
                {
                    ErrorMessage = string.Format("无法连接TCP服务端{0}: {1}", Client.Name, e.ToString());
                    return 1;
                }
                catch (Exception e)
                {
                    ErrorMessage = string.Format("连接TCP服务端{0}过程中出现异常: {1}", Client.Name, e.ToString());
                    return 1;
                }
            }
            IsConnected = true;
            ResetEvent.Set();
            _noRcvrRaiser.Run();
            return 0;
        }

        /// <summary>
        /// 关闭TCP连接
        /// </summary>
        public void Close()
        {
            _noRcvrRaiser.Stop();
            ErrorMessage = string.Empty;
            IsConnected = false;
            IsReceiving = false;
            if (CommandSender != null)
                CommandSender.OutputFlag = 0;
            try
            {
                if (Client == null)
                    return;
                if (Client.IsConnected)
                {
                    Client.Close();
                    //Client = null;
                }
            }
            catch (ArgumentNullException e) { ErrorMessage = "ArgumentNullException: " + e.ToString(); }
            catch (SocketException e) { ErrorMessage = "SocketException: " + e.ToString(); }
            catch (Exception e) { ErrorMessage = "Error Close Socket: " + e.ToString(); }
            //_noRcvrRaiser.Stop();
        }

        /// <summary>
        /// 重新连接
        /// </summary>
        public void Reconnect()
        {
            if (IsConnected)
            {
                Close();
                Client = null;
            }
            //扫描仪这里无需重置用户信息
            ObscureConnect(/*ServerIp, ServerPort*/);
            ReconnectUrself();
            ReconnCounter++;
            //Scanner独有部分开始
            //ResetScanPoints(); //扫描点重置
            //IsReceiving = true;
            //CommandSender.ContinueOrStopOutput(1);
            //Scanner独有部分结束
        }
        #endregion

        /// <summary>
        /// 循环解析数据
        /// </summary>
        public void GetDataStream()
        {
            int interval = 1; //循环时间间隔
            Stopwatch watch = new Stopwatch();
            while (true)
            {
                try
                {
                    watch.Stop();
                    //假如连续读标志为1，时间累积
                    if (CommandSender != null && CommandSender.OutputFlag > 0)
                        DataFetchTimeout += watch.ElapsedMilliseconds;
                    else
                        DataFetchTimeout = 0;
                    IsDataFetchTimeOut = DataFetchTimeout > DataFetchTimeoutThres; //获取数据是否超时
                    watch.Reset();

                    ////假如未连接，暂停线程
                    //if (!IsConnected)
                    //    ResetEvent.WaitOne();
                    //假如未连接，进入下一次循环
                    if (!IsConnected)
                        continue;
                    watch.Start();
                    Thread.Sleep(interval);
                    if (Client == null || !Client.IsConnected || string.IsNullOrWhiteSpace(ReceivedData) || !IsReceiving)
                    {
                        //Thread.Sleep(interval);
                        continue;
                    }
                    string message = ReceivedData;
                    ResolveData(message);
                }
                catch (ArgumentNullException e) { ErrorMessage = "ArgumentNullException: " + e.Message; }
                catch (SocketException e) { ErrorMessage = "SocketException: " + e.Message; }
                catch (Exception e) { ErrorMessage = "Error Socket: " + e.Message; }
                finally
                {
                    //watch.Stop();
                    //DataFetchTimeout += watch.ElapsedMilliseconds;
                    //watch.Reset();
                }
            }
        }
        #endregion

        #region 事件
        private void NoneReceived_ThresholdReached(object sender, ThresholdReachedEventArgs e)
        {
            OnNoneReceived?.Invoke(this, new NoneReceivedEventArgs((ulong)NoneReceivedThreshold)); //调用超时未接收的事件委托
            if (!ReconnectWhenReceiveNone)
                return;

            Reconnect();
        }

        //private void Client_OnNoneReceived(object sender, NoneReceivedEventArgs args)
        //{
        //    //throw new NotImplementedException();
        //    OnNoneReceived.BeginInvoke(sender, args, null, null);
        //}

        private void Client_DataReceived(object sender, CommonLib.Events.DataReceivedEventArgs eventArgs)
        {
            DataReceived.BeginInvoke(sender, eventArgs, null, null);
            _noRcvrRaiser.Click();
            AnalyzeReceivedMessage(eventArgs.ReceivedInfo_String);
        }
        #endregion
    }
}
