using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ScanUtilityLibrary.Core.R2000
{
    /// <summary>
    /// 倍加福R2000系列命令操作类
    /// </summary>
    public class Commands
    {
        /// <summary>
        /// 操作是否成功，假如成功则为1，否则为0
        /// </summary>
        public short Success { get; set; }

        /// <summary>
        /// 错误代码
        /// </summary>
        public short ErrorCode { get; set; }

        /// <summary>
        /// 设备IP地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 端口号
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// 连接句柄
        /// </summary>
        public string Handle { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="ip">设备IP</param>
        public Commands(string ip) { IpAddress = ip; }

        /// <summary>
        /// 默认构造器
        /// </summary>
        public Commands() : this(string.Empty) { }

        // ******************************************************************************************
        // *    Send Command Get_Protocol_Info
        // *
        // *    Input: ip           (R2000 ip address)
        // *    Output: R2000 response string
        // *
        // *    Class variable -> int16 Success ( 1 = success from R2000) ( 0 = no success)
        // *                   -> int16 ErrorCode (Error Code from R2000)
        // ******************************************************************************************
        /// <summary>
        /// 发送获取协议信息的命令
        /// </summary>
        /// <returns></returns>
        public string Get_Protocol_Info()
        {
            return SendCommand(string.Format("http://{0}/cmd/get_protocol_info", IpAddress));
        }

        /// <summary>
        /// 获取R2000的参数
        /// </summary>
        /// <param name="parameter">参数名称</param>
        /// <returns></returns>
        public string GetParameter(string parameter)
        {
            return SendCommand(string.Format("http://{0}/cmd/get_parameter?list={1}", IpAddress, parameter));
        }

        // ******************************************************************************************
        // *    Send Command Set Parameter
        // *
        // *    Input:  ip           (R2000 ip address)
        //*             parameter    (sample: "samples_per_scan=3600")
        // *    Output: R2000 response string
        // *
        // *    Class variable -> int16 Success ( 1 = success from R2000) ( 0 = no success)
        // *                   -> int16 ErrorCode (Error Code from R2000)
        // ******************************************************************************************
        /// <summary>
        /// 发送设定参数的命令
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public string SetParameter(string parameter)
        {
            return SendCommand(string.Format("http://{0}/cmd/set_parameter?{1}", IpAddress, parameter));
        }

        /// <summary>
        /// 设置设备的扫描相关参数（需要的连接句柄的参数）
        /// </summary>
        /// <param name="parameter">参数名</param>
        /// <returns>返回结果</returns>
        public string SetGenericParameter(string parameter)
        {
            return SendCommand(string.Format("http://{0}/cmd/{1}?handle={2}", IpAddress, parameter, Handle));
        }

        /// <summary>
        /// 获取设备型号，1为UHD，3为HD
        /// </summary>
        /// <returns>返回JSON格式字符串，device_family代表型号</returns>
        public string GetDeviceFamily()
        {
            return GetParameter("device_family");
        }

        /// <summary>
        /// 获取生产商
        /// </summary>
        /// <returns></returns>
        public string GetVendor()
        {
            return GetParameter("vendor");
        }

        /// <summary>
        /// 获取产品名称
        /// </summary>
        /// <returns></returns>
        public string GetProduct()
        {
            return GetParameter("product");
        }

        /// <summary>
        /// 获取工作模式（measure, transmitter_off）
        /// </summary>
        /// <returns></returns>
        public string GetMeasureMode()
        {
            string result = GetParameter("operating_mode");
            if (Success == 0)
                return string.Empty;

            string[] arr = result.Split('{', '}', '"', ':').Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();
            return arr[1];
        }

        /// <summary>
        /// 设置扫描模式
        /// </summary>
        /// <param name="isMeasuring">是否扫描</param>
        /// <returns>返回结果</returns>
        public string SetMeasureMode(bool isMeasuring)
        {
            string measureMode = isMeasuring ? "measure" : "transmitter_off";
            return SetParameter(string.Format("operating_mode={0}", measureMode));
            //string command = string.Format("http://{0}/cmd/set_parameter?operating_mode={1}", IpAddress, measureMode);
            //return SendCommand(command);
        }

        // ******************************************************************************************
        // *    Send Command Get_Protocol_Info
        // *
        // *    Input: ip           (R2000 ip address)
        // *           packet_type  (Packet Type -> A -> B -> C)
        // *           timeout      (Watchdog timeout)
        // *           start_angle  (Scan start point)
        // *    Output: R2000 response string
        // *
        // *    Class variable -> int16 Success ( 1 = success from R2000) ( 0 = no success)
        // *                   -> int16 ErrorCode (Error Code from R2000)
        // ******************************************************************************************
        /// <summary>
        /// 获取TCP通讯句柄
        /// </summary>
        /// <param name="packet_type">包类型</param>
        /// <param name="timeout">超时时间</param>
        /// <param name="start_angle">起始角度</param>
        /// <returns></returns>
        public string Request_Handle_TCP(string packet_type, string timeout, string start_angle)
        {
            return Request_Handle_TCP(null, packet_type, timeout, start_angle);
        }

        /// <summary>
        /// 请求TCP协议连接
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="packet_type">数据包类型</param>
        /// <param name="timeout"></param>
        /// <param name="start_angle">开始角度</param>
        /// <returns></returns>
        public string Request_Handle_TCP(string port, string packet_type, string timeout, string start_angle)
        {
            //HTTP命令内容
            string command = string.Format("http://{0}/cmd/request_handle_tcp?packet_type={1}&{2}&start_angle={3}", IpAddress, packet_type, timeout, start_angle);
            //假如端口号为空或空字符串或空白字符串，则添加port参数，请求特定端口号
            if (!string.IsNullOrWhiteSpace(port))
                command += string.Format("&port={0}", port);

            string txt = SendCommand(command);

            if (Success == 1)
            {
                int first = txt.IndexOf("port") + "port".Length + 2;
                int last = first;
                do
                {
                    last += 1;
                }
                while (txt.Substring(last, 1) != ",");

                if (first >= 0)
                    Port = txt.Substring(first, last - first);

                first = txt.IndexOf("handle") + "handle".Length + 3;
                last = first;
                do
                {
                    last += 1;
                }
                while (txt.Substring(last, 1) != ",");

                if (first >= 0)
                    Handle = txt.Substring(first, last - first - 1);
            }
            else
            {
                Port = "";
                Handle = "";
            }

            return (txt);
        }

        // ******************************************************************************************
        // *    Send Command Release handle
        // *
        // *    Input: ip           (R2000 ip address)
        // *    Output: R2000 response string
        // *
        // *    Class variable -> int16 Success ( 1 = success from R2000) ( 0 = no success)
        // *                   -> int16 ErrorCode (Error Code from R2000)
        // ******************************************************************************************
        /// <summary>
        /// 释放句柄
        /// </summary>
        /// <returns></returns>
        public string Release_Handle()
        {
            return SetGenericParameter("release_handle");
            //return SendCommand(string.Format("http://{0}/cmd/release_handle?handle={1}", IpAddress, Handle));
        }
        // ******************************************************************************************
        // *    Send Command Start scan output
        // *
        // *    Input: ip           (R2000 ip address)
        // *    Output: R2000 response string
        // *
        // *    Class variable -> int16 Success ( 1 = success from R2000) ( 0 = no success)
        // *                   -> int16 ErrorCode (Error Code from R2000)
        // ******************************************************************************************
        /// <summary>
        /// 发送命令，开始输出扫描数据
        /// </summary>
        /// <returns>返回指令结果</returns>
        public string Start_Scanoutput()
        {
            return SetGenericParameter("start_scanoutput");
            //return SendCommand(string.Format("http://{0}/cmd/start_scanoutput?handle={1}", IpAddress, Handle));
        }
        // ******************************************************************************************
        // *    Send Command Stop scan output
        // *
        // *    Input: ip           (R2000 ip address)
        // *    Output: R2000 response string
        // *
        // *    Class variable -> int16 Success ( 1 = success from R2000) ( 0 = no success)
        // *                   -> int16 ErrorCode (Error Code from R2000)
        // ******************************************************************************************
        /// <summary>
        /// 发送命令终止数据输出
        /// </summary>
        /// <returns>返回指令结果</returns>
        public string Stop_Scanoutput()
        {
            return SetGenericParameter("stop_scanoutput");
            //return SendCommand(string.Format("http://{0}/cmd/stop_scanoutput?handle={1}", IpAddress, Handle));
        }

        // ******************************************************************************************
        // *    Send Command Feed Watchdog
        // *
        // *    Input:  ip           (R2000 ip address)
        // *    Output: R2000 response string
        // *
        // *    Class variable -> int16 Success ( 1 = success from R2000) ( 0 = no success)
        // *                   -> int16 ErrorCode (Error Code from R2000)
        // ******************************************************************************************
        /// <summary>
        /// 向看门狗发送反馈
        /// </summary>
        /// <returns></returns>
        public string Feed_Watchdog()
        {
            Success = 0;
            return string.IsNullOrWhiteSpace(Handle) ? string.Empty : SetGenericParameter("feed_watchdog");
            //if (!string.IsNullOrWhiteSpace(Handle))
            //    return SendCommand("http://" + IpAddress + "/cmd/feed_watchdog?handle=" + Handle);

            //return "";
        }

        // ******************************************************************************************
        // *    Send Command to R2000 using WebClient
        // *
        // *    Input: command string
        // *    Output: R2000 response string
        // *
        // *    Class variable -> int16 Success ( 1 = success from R2000) ( 0 = no success)
        // *                   -> int16 ErrorCode (Error Code from R2000)
        // ******************************************************************************************
        private string SendCommand(string command)
        {
            Success = 0;

            using (WebClient R2000webClient = new System.Net.WebClient())
            {
                string txt;
                try
                {
                    txt = R2000webClient.DownloadString(command);

                    int first = txt.IndexOf("error_code") + "error_code".Length + 2;
                    int last = txt.LastIndexOf(",");
                    string h_txt = txt.Substring(first, last - first);

                    try
                    {
                        ErrorCode = Convert.ToInt16(h_txt);
                    }
                    catch (FormatException e)
                    {
                        txt = "Error Get_Protocol_Info: " + e;
                        return (txt);
                    }

                    first = txt.IndexOf("error_text") + "error_text".Length + 3;
                    last = txt.LastIndexOf("}") - 3;
                    h_txt = txt.Substring(first, last - first);

                    if (h_txt == "success")
                        Success = 1;
                    return (txt);
                }
                catch (Exception e)
                {
                    txt = "Can't connect to R2000 - Error Get_Protocol_Info: " + e;
                    return (txt);
                }
            }
        }
    }
}
