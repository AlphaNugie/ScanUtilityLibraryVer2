﻿using ScanUtilityLibrary.Model.SICK.Scanner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Core.SICK.Scanner
{
    /// <summary>
    /// 西克（SICK）LMS扫描仪的命令处理类
    /// </summary>
    public class CommandSender : BaseCmdSender
    {
        /// <summary>
        /// 以指定的tcp通讯对象初始化
        /// </summary>
        /// <param name="client"></param>
        public CommandSender(BaseTcpClient client) : base(client) { }

        /// <summary>
        /// 发送用户登录的指令
        /// </summary>
        /// <param name="userLevel">用户级别</param>
        /// <param name="userPwdHash">登录密码的8位16进制哈希值</param>
        /// <returns>返回从设备获取的信息</returns>
        public override string Login(UserLevel userLevel, string userPwdHash)
        {
            try
            {
                string command = string.Format("sMN SetAccessMode {0:00} {1}", (int)userLevel, userPwdHash); //将用户级别的代码转为2位带前导零的10进制数字
                string info = SendCommand(command);//发送指令
                //假如传送、接受指令的过程中出现异常，在字符串开头添加字符'e'
                if (info.StartsWith("Error:"))
                    return 'e' + info.Replace("Error:", string.Empty).TrimStart();
                //否则将字符串最后一个字符添加到第一个字符前(0/1)
                else
                    return info.Length > 0 ? info[info.Length - 1] + info : '0' + info;
            }
            catch (Exception e) { return 'e' + e.Message; }//假如引发异常，在字符串开头添加字符“e”
        }

        /// <summary>
        /// 发送用户登录的指令
        /// </summary>
        /// <param name="userLevel">用户级别</param>
        /// <returns>返回从设备获取的信息</returns>
        public string Login(UserLevel userLevel)
        {
            return Login(userLevel, GetPwdHashByUserLevel(userLevel));
        }

        ///// <summary>
        ///// 向设备传送更改扫描设定的指令（仅限LMS系列）
        ///// </summary>
        ///// <param name="version">扫描仪型号</param>
        ///// <param name="scanFreq">扫描频率(+扫描频率(HZ)x100)</param>
        ///// <param name="angleReso">角分辨率(+角分辨率x10000)</param>
        ///// <returns>返回从设备获取的信息</returns>
        //public string ChangeScanSetting(ScannerVersion version, double scanFreq, double angleReso)
        //{
        //    string info = string.Empty;
        //    if (version == ScannerVersion.LMS_1xx)
        //        info = ChangeScanSetting(version, scanFreq, angleReso, -45, 225);
        //    else if (version == ScannerVersion.LMS_5xx)
        //        info = ChangeScanSetting(version, scanFreq, angleReso, -5, 185);
        //    else if (version == ScannerVersion.LRS_36x1)
        //        throw new ArgumentException($"对于{version}系列雷达必须提供起始与终止角度");
        //    //if (version == ScannerVersion.LMS_1xx)
        //    //    info = ChangeScanSetting(scanFreq, new List<ScannerSector>() { new ScannerSector(angleReso, -45, 225) });
        //    //else if (version == ScannerVersion.LMS_5xx)
        //    //    info = ChangeScanSetting(scanFreq, new List<ScannerSector>() { new ScannerSector(angleReso, -5, 185) });
        //    return info;
        //}

        ///// <summary>
        ///// 向设备传送更改扫描设定的指令
        ///// </summary>
        ///// <param name="version">设备型号</param>
        ///// <param name="scanFreq">扫描频率(+扫描频率(HZ)x100)</param>
        ///// <param name="angleReso">角分辨率(+角分辨率x10000)</param>
        ///// <param name="degreeFrom">扫描开始角度(+/-角度x10000)</param>
        ///// <param name="degreeTo">扫描结束角度(+/-角度x10000)</param>
        ///// <returns>返回从设备获取的信息</returns>
        //public string ChangeScanSetting(ScannerVersion version, double scanFreq, double angleReso, double degreeFrom, double degreeTo)
        //{
        //    var sectors = new List<ScannerSector>() { new ScannerSector(angleReso, degreeFrom, degreeTo) };
        //    var emptySector = new ScannerSector(angleReso, 0, 0);
        //    //对于LRS36x1系列雷达来说，需要添加3个空白分区
        //    if (version == ScannerVersion.LRS_36x1)
        //        for (int i = 0; i< 3; i++)
        //            sectors.Add(emptySector);
        //    return ChangeScanSetting(scanFreq, sectors);
        //    //try
        //    //{
        //    //    string scanFrequency = '+' + (scanFreq * 100).ToString();
        //    //    string angleResolution = '+' + (angleReso * 10000).ToString("0");
        //    //    string degree1 = (degreeFrom <= 0 ? string.Empty : "+") + (degreeFrom * 10000).ToString("0");
        //    //    string degree2 = (degreeTo <= 0 ? string.Empty : "+") + (degreeTo * 10000).ToString("0");

        //    //    string command = string.Format("sMN mLMPsetscancfg {0} +1 {1} {2} {3}", scanFrequency, angleResolution, degree1, degree2);
        //    //    string info = SendCommand(command);//发送指令
        //    //    return info;
        //    //    //return info.Length > 0 ? info[info.Length - 1] + info : '0' + info;//将字符串最后一个字符添加到第一个字符前()
        //    //}
        //    //catch (Exception e) { return "Error: " + e.Message; }//假如引发异常，在字符串开头添加字符“e”
        //}

        /// <summary>
        /// 向设备传送更改扫描设定的指令（仅限LMS系列）
        /// </summary>
        /// <param name="version">扫描仪型号</param>
        /// <param name="scanFreq">扫描频率(+扫描频率(HZ)x100)</param>
        /// <param name="angleReso">角分辨率(+角分辨率x10000)</param>
        /// <returns>返回从设备获取的信息</returns>
        public string ChangeScanSetting(AllScannerVersion version, double scanFreq, double angleReso)
        {
            string info = string.Empty;
            if (version == AllScannerVersion.LMS_1xx)
                info = ChangeScanSetting(version, scanFreq, angleReso, -45, 225);
            else if (version == AllScannerVersion.LMS_5xx)
                info = ChangeScanSetting(version, scanFreq, angleReso, -5, 185);
            else if (version == AllScannerVersion.LRS_36x1)
                throw new ArgumentException($"对于{version}系列雷达必须提供起始与终止角度");
            else
                throw new ArgumentException($"不支持{version}系列雷达");
            //if (version == ScannerVersion.LMS_1xx)
            //    info = ChangeScanSetting(scanFreq, new List<ScannerSector>() { new ScannerSector(angleReso, -45, 225) });
            //else if (version == ScannerVersion.LMS_5xx)
            //    info = ChangeScanSetting(scanFreq, new List<ScannerSector>() { new ScannerSector(angleReso, -5, 185) });
            return info;
        }

        /// <summary>
        /// 向设备传送更改扫描设定的指令
        /// </summary>
        /// <param name="version">设备型号</param>
        /// <param name="scanFreq">扫描频率(+扫描频率(HZ)x100)</param>
        /// <param name="angleReso">角分辨率(+角分辨率x10000)</param>
        /// <param name="degreeFrom">扫描开始角度(+/-角度x10000)</param>
        /// <param name="degreeTo">扫描结束角度(+/-角度x10000)</param>
        /// <returns>返回从设备获取的信息</returns>
        public string ChangeScanSetting(AllScannerVersion version, double scanFreq, double angleReso, double degreeFrom, double degreeTo)
        {
            var sectors = new List<ScannerSector>() { new ScannerSector(angleReso, degreeFrom, degreeTo) };
            var emptySector = new ScannerSector(angleReso, 0, 0);
            //对于LRS36x1系列雷达来说，需要添加3个空白分区
            if (version == AllScannerVersion.LRS_36x1)
                for (int i = 0; i< 3; i++)
                    sectors.Add(emptySector);
            return ChangeScanSetting(scanFreq, sectors);
            //try
            //{
            //    string scanFrequency = '+' + (scanFreq * 100).ToString();
            //    string angleResolution = '+' + (angleReso * 10000).ToString("0");
            //    string degree1 = (degreeFrom <= 0 ? string.Empty : "+") + (degreeFrom * 10000).ToString("0");
            //    string degree2 = (degreeTo <= 0 ? string.Empty : "+") + (degreeTo * 10000).ToString("0");

            //    string command = string.Format("sMN mLMPsetscancfg {0} +1 {1} {2} {3}", scanFrequency, angleResolution, degree1, degree2);
            //    string info = SendCommand(command);//发送指令
            //    return info;
            //    //return info.Length > 0 ? info[info.Length - 1] + info : '0' + info;//将字符串最后一个字符添加到第一个字符前()
            //}
            //catch (Exception e) { return "Error: " + e.Message; }//假如引发异常，在字符串开头添加字符“e”
        }

        /// <summary>
        /// 向设备传送更改扫描设定的指令
        /// </summary>
        /// <param name="scanFreq">扫描频率(+扫描频率(HZ)x100)</param>
        /// <param name="sectors">扫描设备分区列表</param>
        /// <returns>返回从设备获取的信息</returns>
        public string ChangeScanSetting(double scanFreq, IEnumerable<ScannerSector> sectors)
        {
            if (sectors == null || sectors.Count() == 0)
                return "Error: 提供的分区数量不足";
            try
            {
                //string scanFrequency = '+' + (scanFreq * 100).ToString();
                //string sectorCount = '+' + sectors.Count().ToString();
                string sectorParams = string.Join(" ", sectors.Select(sector => sector.ToString()));
                string command = string.Format("sMN mLMPsetscancfg +{0} +{1} {2}", scanFreq * 100, sectors.Count(), sectorParams);
                string info = SendCommand(command);//发送指令
                return info;
                //return info.Length > 0 ? info[info.Length - 1] + info : '0' + info;//将字符串最后一个字符添加到第一个字符前()
            }
            catch (Exception e) { return "Error: " + e.Message; }//假如引发异常，在字符串开头添加字符“e”
        }

        /// <summary>
        /// 向设备传送更改输出设定的指令
        /// </summary>
        /// <param name="angleReso">角分辨率(角分辨率x10000 -> 16进制)</param>
        /// <param name="degreeFrom">扫描开始角度(角度x10000 -> 16进制)</param>
        /// <param name="degreeTo">扫描结束角度(角度x10000 -> 16进制)</param>
        /// <returns>返回从设备获取的信息</returns>
        public string ChangeOutputSetting(string angleReso, string degreeFrom, string degreeTo)
        {
            try
            {
                string command = string.Format("sWN LMPoutputRange 1 {0} {1} {2}", angleReso, degreeFrom, degreeTo);
                string info = SendCommand(command);
                return info;
            }
            catch (Exception e) { return "Error: " + e.Message; }
        }

        /// <summary>
        /// 单次取数据
        /// </summary>
        /// <returns>返回从设备获取的信息</returns>
        public string DataSingleOutput()
        {
            try
            {
                string command = "sRN LMDscandata";
                string info = SendCommand(command);
                return info;
            }
            catch (Exception e) { return "Error: " + e.Message; }
        }

        /// <summary>
        /// 连续读或连续停
        /// </summary>
        /// <param name="indicator">指示连续读或连续停的数字，0代表连续停，1代表连续读</param>
        /// <returns>返回从设备获取的信息</returns>
        public string ContinueOrStopOutput(int indicator)
        {
            try
            {
                string command = string.Format("sEN LMDscandata {0}", indicator);
                string info = SendCommand(command);
                if (info != null && info.EndsWith(" 1"))
                    OutputFlag = indicator;
                return info;
            }
            catch (Exception e) { return "Error: " + e.Message; }
        }
    }
}
