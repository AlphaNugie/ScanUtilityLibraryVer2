using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using ScanUtilityLibrary.Model;
using CommonLib.Extensions;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace ScanUtilityLibrary.Core.R2000
{
    /// <summary>
    /// 连接激光扫描仪的TCP客户端
    /// </summary>
    public class StreamTcpClient
    {
        private ushort h_num_points_scan = 361;
        private bool IsScanRead = false;

        /// <summary>
        /// 点云数组
        /// </summary>
        public ScanPoint[] ScanPoints = new ScanPoint[25200];

        #region 属性
        /// <summary>
        /// 是否在发送关闭命令的状态下
        /// </summary>
        public bool SocketCloseCommand { get; set; }

        /// <summary>
        /// Tcp服务端的IP
        /// </summary>
        public string ServerIp { get; private set; }

        /// <summary>
        /// Tcp服务端的端口号
        /// </summary>
        public int ServerPort { get; private set; }

        /// <summary>
        /// 是否连接
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// TcpClient对象
        /// </summary>
        public TcpClient R2000Client { get; private set; }

        /// <summary>
        /// Tcp Socket的真实连接状态
        /// </summary>
        public bool IsConnected_Socket { get { return R2000Client.IsSocketConnected(); } }

        /// <summary>
        /// 网络访问数据流对象
        /// </summary>
        public NetworkStream NetStream { get; private set; }

        /// <summary>
        /// 数据包类型，A：距离；B：距离+能量反馈；C：距离+能量反馈(C)
        /// </summary>
        public PacketType PacketType { get; set; }
        //public ushort PacketType { get; set; }

        /// <summary>
        /// 数据包大小
        /// </summary>
        public uint PacketSize { get; set; }

        /// <summary>
        /// 包头大小
        /// </summary>
        public ushort HeaderSize { get; set; }
        /// <summary>
        /// 数据包序号
        /// </summary>
        public ushort ScanNumber { get; set; }
        /// <summary>
        /// 单圈数据包序号
        /// </summary>
        public ushort PacketNumber { get; set; }
        /// <summary>
        /// 原始时间戳
        /// </summary>
        public ulong Timestamp_Raw { get; set; }
        /// <summary>
        /// 同步时间戳
        /// </summary>
        public ulong Timestamp_Sync { get; set; }
        /// <summary>
        /// 状态位
        /// </summary>
        public uint StatusFlags { get; set; }
        /// <summary>
        /// 扫描频率(单位：赫兹)
        /// </summary>
        public uint ScanFrequency { get; set; }
        /// <summary>
        /// 单圈样本数
        /// </summary>
        public ushort Num_Points_Scan { get; set; }
        /// <summary>
        /// 数据包样本数
        /// </summary>
        public ushort Num_Points_Packet { get; set; }
        /// <summary>
        /// 第一有效数据序号
        /// </summary>
        public ushort FirstIndex { get; set; }
        /// <summary>
        /// 第一有效数据角度
        /// </summary>
        public int FirstAngle { get; set; }
        /// <summary>
        /// 角分辨率，逆时针转动时小于0
        /// </summary>
        public int AngularIncrement { get; set; }
        /// <summary>
        /// 实际角分辨率(360° / 单圈样本数)
        /// </summary>
        public double AngularIncrement_Real { get; set; }
        /// <summary>
        /// 输出状态
        /// </summary>
        public uint OutputStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public uint FieldStatus { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg_TcpConnections { get; set; }
        #endregion

        /// <summary>
        /// 默认构造器
        /// </summary>
        public StreamTcpClient() { }

        /// <summary>
        /// TCP服务端连接
        /// </summary>
        /// <param name="server">IP地址</param>
        /// <param name="port">端口号</param>
        public StreamTcpClient(string server, string port)
        {
            Connect(server, port);
        }

        /// <summary>
        /// 利用特定的端口与TCP服务端连接
        /// </summary>
        /// <param name="server">TCP服务端IP</param>
        /// <param name="port">端口号</param>
        /// <returns>假如成功，返回0，否则返回1</returns>
        public int Connect(string server, string port)
        {
            ServerIp = server;
            IsConnected = false;
            ErrorMsg_TcpConnections = string.Empty;
            if (string.IsNullOrWhiteSpace(port))
                return (1);
            ServerPort = int.Parse(port);
            if (R2000Client == null)
            {
                try
                {
                    R2000Client = new TcpClient(ServerIp, ServerPort) { ReceiveBufferSize = 65535 };
                    NetStream = R2000Client.GetStream();
                }
                catch (SocketException e)
                {
                    ErrorMsg_TcpConnections = "Unable to connect to server: " + e.ToString();
                    return (1);
                }
                catch (Exception e)
                {
                    ErrorMsg_TcpConnections = "Unable to connect to server: " + e.ToString();
                    return (1);
                }
            }
            IsConnected = true;
            return (0);
        }

        /// <summary>
        /// 关闭TCP连接
        /// </summary>
        private void Close()
        {
            ErrorMsg_TcpConnections = string.Empty;
            IsConnected = false;
            try
            {
                if (R2000Client == null)
                    return;
                if (R2000Client.Connected == true)
                {
                    // Close stream
                    NetStream.Close();
                    // Close Client
                    R2000Client.Close();
                    R2000Client = null;
                    IsScanRead = false;
                }
            }
            catch (ArgumentNullException e)
            {
                ErrorMsg_TcpConnections = "ArgumentNullException: " + e.ToString();
            }
            catch (SocketException e)
            {
                ErrorMsg_TcpConnections = "SocketException: " + e.ToString();
            }
            catch (Exception e)
            {
                ErrorMsg_TcpConnections = "Error Close Socket: " + e.ToString();
            }
            return;
        }

        /// <summary>
        /// 刷新扫描点信息
        /// </summary>
        public void R2000RecDataStream()
        {
            while (true)
            {
                try
                {
                    if (R2000Client != null)
                    {
                        if (R2000Client.Connected && R2000Client.Available > 0)
                        {
                            byte[] data;
                            // Buffer to store the response bytes.
                            data = new byte[2048];

                            // String to store the response ASCII representation.

                            // Read telegram header
                            if (!IsScanRead && (R2000Client.Available >= 60))
                            {
                                IsScanRead = true;
                                // Read the Header from R2000 TCP stream. HEADER_SIZE == 60
                                int bytes = NetStream.Read(data, 0, 60);

                                if ((data[1] == 0xa2) && (data[0] == 0x5c))
                                {
                                    //PacketType = BitConverter.ToUInt16(data, 2);
                                    PacketType = Enum.TryParse(BitConverter.ToChar(data, 2) + string.Empty, false, out PacketType type) ? type : PacketType.NONE;
                                    PacketSize = BitConverter.ToUInt32(data, 4);
                                    HeaderSize = BitConverter.ToUInt16(data, 8);
                                    ScanNumber = BitConverter.ToUInt16(data, 10);
                                    PacketNumber = BitConverter.ToUInt16(data, 12);
                                    Timestamp_Raw = BitConverter.ToUInt64(data, 14);
                                    Timestamp_Sync = BitConverter.ToUInt64(data, 22);
                                    StatusFlags = BitConverter.ToUInt32(data, 30);
                                    ScanFrequency = BitConverter.ToUInt32(data, 34);
                                    Num_Points_Scan = BitConverter.ToUInt16(data, 38);
                                    Num_Points_Packet = BitConverter.ToUInt16(data, 40);
                                    FirstIndex = BitConverter.ToUInt16(data, 42);
                                    FirstAngle = BitConverter.ToInt32(data, 44);
                                    AngularIncrement = BitConverter.ToInt32(data, 48);
                                    OutputStatus = BitConverter.ToUInt32(data, 52);
                                    FieldStatus = BitConverter.ToUInt32(data, 56);

                                    if (h_num_points_scan != Num_Points_Scan)
                                    {
                                        h_num_points_scan = Num_Points_Scan;
                                        AngularIncrement_Real = 360 / Convert.ToDouble(Num_Points_Scan); // Calculation of real angular_increment value
                                        //int i;
                                        for (int i = 0; i < ScanPoints.Length; i++)
                                        {
                                            ScanPoints[i].Distance = 0;
                                            ScanPoints[i].EchoValue = 0;
                                            if (i < Num_Points_Scan)
                                                ScanPoints[i].Angle = AngularIncrement_Real * i;
                                            else
                                                ScanPoints[i].Angle = 0;
                                        }
                                    }
                                    // if header > 60 then receive last header bytes
                                    if (HeaderSize > 60)
                                        bytes = NetStream.Read(data, 0, HeaderSize - 60);
                                }
                            }

                            // Read incomming data if all data from packet available - length is (packet_size-header_size)
                            if (IsScanRead && (R2000Client.Available >= PacketSize - HeaderSize))
                            {
                                ushort i = 0;
                                //z_point++;//Z轴坐标增加10

                                IsScanRead = false;
                                // Read the data response as bytes
                                int bytes = NetStream.Read(data, 0, (int)(PacketSize - HeaderSize));
                                int AnzPoints = 0;
                                uint distance;
                                //double angle;
                                DateTime time = DateTime.Now;//数据提取时间为当下系统时间
                                switch (PacketType)
                                {
                                    //case 65: // Type A
                                    case PacketType.A: // Type A
                                        AnzPoints = bytes / 4;
                                        for (i = 0; i < AnzPoints; i++)
                                        {
                                            distance = BitConverter.ToUInt32(data, i * 4);
                                            ScanPoints[FirstIndex + i].Distance = distance;//距离
                                            ScanPoints[FirstIndex + i].ScannedTime = time;//输出时间
                                        }
                                        break;
                                    //case 66: // Type B
                                    case PacketType.B: // Type B
                                        AnzPoints = bytes / 6;
                                        for (i = 0; i < AnzPoints; i++)
                                        {
                                            distance = BitConverter.ToUInt32(data, i * 6);
                                            ScanPoints[FirstIndex + i].Distance = distance;//距离
                                            ScanPoints[FirstIndex + i].EchoValue = BitConverter.ToUInt16(data, i * 6 + 4);
                                            ScanPoints[FirstIndex + i].ScannedTime = time;//输出时间
                                        }
                                        break;
                                    //case 67: // Type C
                                    case PacketType.C: // Type C
                                        AnzPoints = bytes / 4;
                                        for (i = 0; i < AnzPoints; i++)
                                        {
                                            distance = BitConverter.ToUInt32(data, i * 4) & 0xfffff;
                                            ScanPoints[FirstIndex + i].Distance = distance;//距离
                                            ScanPoints[FirstIndex + i].EchoValue = BitConverter.ToUInt32(data, i * 4) >> 20;
                                            ScanPoints[FirstIndex + i].ScannedTime = time;//输出时间
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                                Thread.Sleep(1);
                        }
                        else
                            Thread.Sleep(1);
                    }
                    if (SocketCloseCommand)
                    {
                        SocketCloseCommand = false;
                        Close();
                    }
                }
                catch (ArgumentNullException e)
                {
                    ErrorMsg_TcpConnections = "ArgumentNullException: " + e.ToString();
                }
                catch (SocketException e)
                {
                    ErrorMsg_TcpConnections = "SocketException: " + e.ToString();
                }
                catch (Exception e)
                {
                    ErrorMsg_TcpConnections = "Error Socket: " + e.ToString();
                }
            }
        }

        /// <summary>
        /// 获取TCP信息抬头
        /// </summary>
        /// <returns></returns>
        public string GetTcpHeader()
        {
            //string txt = "数据包类型 = ";
            //switch (PacketType)
            //{
            //    case 65:
            //        txt += "A" + (char)13 + (char)10;
            //        break;
            //    case 66:
            //        txt += "B" + (char)13 + (char)10;
            //        break;
            //    case 67:
            //        txt += "C" + (char)13 + (char)10;
            //        break;
            //    default:
            //        txt += "" + (char)13 + (char)10;
            //        break;
            //}
            //txt += "数据包大小 = " + PacketSize.ToString("D") + (char)13 + (char)10;
            //txt += "包头大小 = " + HeaderSize.ToString("D") + (char)13 + (char)10;
            //txt += "数据包序号 = " + ScanNumber.ToString("D") + (char)13 + (char)10;
            //txt += "单圈数据包序号 = " + PacketNumber.ToString("D") + (char)13 + (char)10;
            //txt += "原始时间戳 = " + Timestamp_Raw.ToString("D") + (char)13 + (char)10;
            //txt += "同步时间戳 = " + Timestamp_Sync.ToString("D") + (char)13 + (char)10;
            //txt += "扫描频率 = " + ScanFrequency.ToString("D") + (char)13 + (char)10;
            //txt += "单圈测量样本数 = " + Num_Points_Scan.ToString("D") + (char)13 + (char)10;
            //txt += "数据包样本数 = " + Num_Points_Packet.ToString("D") + (char)13 + (char)10;
            //txt += "第一有效数据序号 = " + FirstIndex.ToString("D") + (char)13 + (char)10;
            //txt += "第一有效数据角度 = " + FirstAngle.ToString("D") + (char)13 + (char)10;
            //txt += "测量角分辨率 = " + AngularIncrement.ToString("D") + (char)13 + (char)10;//逆时针转动时角分辨率小于0
            //txt += "" + (char)13 + (char)10 + "TCP是否已连接 = " + IsConnected + (char)13 + (char)10;
            //txt += "TCP连接状态: " + ErrorMsg_TcpConnections + (char)13 + (char)10;
            List<string> list = new List<string>
            {
                "数据包类型 = " + PacketType.ToString(),
                "数据包大小 = " + PacketSize.ToString("D"),
                "包头大小 = " + HeaderSize.ToString("D"),
                "数据包序号 = " + ScanNumber.ToString("D"),
                "单圈数据包序号 = " + PacketNumber.ToString("D"),
                "原始时间戳 = " + Timestamp_Raw.ToString("D"),
                "同步时间戳 = " + Timestamp_Sync.ToString("D"),
                "扫描频率 = " + ScanFrequency.ToString("D"),
                "单圈测量样本数 = " + Num_Points_Scan.ToString("D"),
                "数据包样本数 = " + Num_Points_Packet.ToString("D"),
                "第一有效数据序号 = " + FirstIndex.ToString("D"),
                "第一有效数据角度 = " + FirstAngle.ToString("D"),
                //逆时针转动时角分辨率小于0
                "测量角分辨率 = " + AngularIncrement.ToString("D"),
                string.Empty,
                "TCP是否已连接 = " + IsConnected,
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
            if (startValue < 0)
                startValue = 0;
            else if (startValue > 25164)
                startValue = 25164;

            List<string> list = new List<string> { string.Format("扫描数据点 {0}-{1}:", startValue, startValue + 35) };
            //string txt = "扫描数据点 " + startValue + "-" + (startValue + 35) + ":" + (char)13 + (char)10;
            if (ScanPoints == null)
                //return txt;
                goto END;

            #region 显示X、Y、Z轴坐标
            string blank = "        ";
            list.Add(string.Format("起始数据点:{0}x:{0}y:{0}echo:", blank));
            //txt += "起始数据点:        x:          y:          echo:" + (char)13 + (char)10;
            for (int i = startValue; i < (startValue + 36); i++)
            {
                if (ScanPoints[i] == null)
                    continue;
                bool fault = ScanPoints[i].Distance == 0xFFFFFFFF;
                string text = string.Format("{1:00000}{0}{2}{0}{3}{0}{4}", blank, i, fault ? "Error" : ScanPoints[i].X.ToString("000"), fault ? "Error" : ScanPoints[i].Y.ToString("000"), fault ? "Error" : ScanPoints[i].Z.ToString("000"));
                list.Add(text);
                //txt += i.ToString("00000") + "         ";
                //if (!(ScanPoints[i].Distance == 0xFFFFFFFF))
                //    txt += ScanPoints[i].X.ToString("000.0000") + "     " + ScanPoints[i].Y.ToString("000.0000") + "     " + ScanPoints[i].EchoValue.ToString();
                //else
                //    txt += "Error" + "     " + "Error" + "     " + "Error";
                //txt += (char)13 + (char)10;
            }
        #endregion

        END:
            //return txt;
            return string.Join("\r\n", list);
        }
    }
}