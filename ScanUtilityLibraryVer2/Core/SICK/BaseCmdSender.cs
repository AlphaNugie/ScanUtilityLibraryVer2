using CommonLib.Function;
using ScanUtilityLibrary.Core.SICK.Dx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Core.SICK
{
    /// <summary>
    /// 西克（SICK）设备的命令处理基类
    /// </summary>
    public abstract class BaseCmdSender
    {
        private readonly BaseTcpClient StreamClient = null;

        ///// <summary>
        ///// 特殊字符，代表正文开始
        ///// </summary>
        //public const char STX = (char)2;

        ///// <summary>
        ///// 特殊字符，代表正文结束
        ///// </summary>
        //public const char ETX = (char)3;

        /// <summary>
        /// 连续停或连续读的标志，大于0代表连续读，0代表连续停
        /// </summary>
        public int OutputFlag { get; set; }

        /// <summary>
        /// 最后从设备获取到的消息
        /// </summary>
        public string LastMessage { get; set; }

        /// <summary>
        /// 以指定的tcp通讯对象初始化
        /// </summary>
        /// <param name="client"></param>
        protected BaseCmdSender(BaseTcpClient client)
        {
            StreamClient = client;
        }

        /// <summary>
        /// 向设备发送指令，接收设备返回的指令或错误信息
        /// </summary>
        /// <param name="command">指令</param>
        /// <returns>返回从设备获取的信息</returns>
        public string SendCommand(string command)
        {
            try
            {
                //转换为byte类型数组并发送指令
                int wait_interval = 10; //每次等待的毫秒数
                int timeout = 1000; //等待超时时间
                StreamClient.ReceivedMessage = string.Empty;
                StreamClient.Client.SendString(Const.STX + command + Const.ETX);
                //byte[] bytes = System.Text.Encoding.ASCII.GetBytes(Base.STX + command + Base.ETX);//将字符串转换为byte数组
                //StreamClient.NetStream.Write(bytes, 0, bytes.Length);

                //接收指令并转换为字符串
                string info = string.Empty;
                if (StreamClient.Client.BaseClient.Available > 0)
                {
                    byte[] buffer = new byte[8192];
                    StreamClient.NetStream.Read(buffer, 0, buffer.Length);
                    info = Encoding.ASCII.GetString(buffer);
                }
                else
                {
                    int i = 0;
                    while (string.IsNullOrWhiteSpace(info) && i < timeout)
                    {
                        info = StreamClient.ReceivedMessage;
                        i += wait_interval;
                        Thread.Sleep(wait_interval);
                    }
                }
                return StripMessage(info);
                //int startIndex = info[0] == Base.STX ? 1 : 0;
                //info = info.Substring(startIndex, info.IndexOf(Base.ETX) - startIndex);//根据正文开始、正文结束字符截取字符串
                //return info;
            }
            catch (Exception e) { return "Error: " + e.Message; }//假如引发异常，在字符串开头添加字符“e”
        }

        /// <summary>
        /// 指令字符串剥除正文开始、正文结束字符
        /// </summary>
        /// <param name="origin">原始字符串</param>
        /// <returns></returns>
        public static string StripMessage(string origin)
        {
            if (string.IsNullOrWhiteSpace(origin))
                return string.Empty;
            //int startIndex = origin[0] == Const.STX ? 1 : 0;
            //origin = origin.Substring(startIndex, origin.IndexOf(Const.ETX) - startIndex);//根据正文开始、正文结束字符截取字符串
            Match match = Regex.Match(origin, BasePatterns.OutputInGeneral);
            origin = match.Value.Trim(Const.STX, Const.ETX); //根据正文开始、正文结束字符截取字符串
            return origin;
        }

        /// <summary>
        /// 根据用户级别获取密码哈希值
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        protected static string GetPwdHashByUserLevel(UserLevel level)
        {
            string hash = string.Empty;
            switch (level)
            {
                case UserLevel.Maintenance:
                    hash = FtyUserPwds.Maintenance.Hash;
                    break;
                case UserLevel.AuthorisedClient:
                    hash = FtyUserPwds.AuthorisedClient.Hash;
                    break;
                case UserLevel.Service:
                    hash = FtyUserPwds.Service.Hash;
                    break;
                default:
                    break;
            }
            return hash;
        }

        /// <summary>
        /// 发送用户登录的指令
        /// </summary>
        /// <param name="userLevel">用户级别</param>
        /// <param name="userPwdHash">登录密码的8位16进制哈希值</param>
        /// <returns>返回从设备获取的信息</returns>
        public abstract string Login(UserLevel userLevel, string userPwdHash);

        /// <summary>
        /// 注销登录
        /// </summary>
        /// <returns></returns>
        public string Logout()
        {
            try
            {
                string command = "sMN Run";
                string info = SendCommand(command);//发送指令
                //假如传送、接受指令的过程中出现异常，在字符串开头添加字符'e'
                if (info.StartsWith("Error:"))
                    return 'e' + info.Replace("Error:", string.Empty).TrimStart();
                //否则将字符串最后一个字符添加到第一个字符前(0/1)
                else
                    return info.Length > 0 ? info[info.Length - 1] + info : '0' + info;
            }
            catch (Exception e) { return 'e' + e.Message; }
        }

        /// <summary>
        /// 向设备传送将参数固化的指令
        /// </summary>
        /// <returns></returns>
        public string SaveSettingToDevice()
        {
            try
            {
                string command = "sMN mEEwriteall";
                string info = SendCommand(command);
                //假如传送、接受指令的过程中出现异常，在字符串开头添加字符'e'
                if (info.StartsWith("Error:"))
                    return 'e' + info.Replace("Error:", string.Empty).TrimStart();
                //否则将字符串最后一个字符添加到第一个字符前(0/1)
                else
                    return info.Length > 0 ? info[info.Length - 1] + info : '0' + info;
            }
            catch (Exception e) { return 'e' + e.Message; }
        }
    }
}
