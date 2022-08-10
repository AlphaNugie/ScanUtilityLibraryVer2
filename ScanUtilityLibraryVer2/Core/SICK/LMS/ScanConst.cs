using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using System.Security.Cryptography;

namespace ScanUtilityLibrary.Core.SICK.LMS
{
    /// <summary>
    /// 公有变量类
    /// </summary>
    public class ScanConst : Const
    {
        /// <summary>
        /// 保存的.xyz文件的路径(包括文件名与类型后缀)
        /// </summary>
        public static string FilePath = string.Empty;

        /// <summary>
        /// 保存XML文件的路径(包括文件名与类型后缀)
        /// </summary>
        public static string XMLPath = string.Empty;

        /// <summary>
        /// 样本可能出现的最大数目(LMS511扫描角度为-45°至225°、角分辨率为0.1667°时为1621个)
        /// </summary>
        public static int SampleMaxCount = 2160;

        ///// <summary>
        ///// 指示是否即将结束接收数据
        ///// </summary>
        //public static bool IsReceiving = false;

        ///// <summary>
        ///// LMS扫描仪型号
        ///// </summary>
        //public static ScannerVersion Version;

        /// <summary>
        /// LMS-1xx扫描仪可选扫描频率
        /// </summary>
        public static List<int> ScanFrequencies_1xx = new List<int>() { 25, 50 };

        /// <summary>
        /// LMS-5xx扫描仪可选扫描频率
        /// </summary>
        public static List<int> ScanFrequencies_5xx = new List<int>() { 25, 35, 50, 75, 100 };

        /// <summary>
        /// LMS-1xx扫描仪可选角分辨率
        /// </summary>
        public static List<double> AngleResolutions_1xx = new List<double>() { 0.25, 0.5 };

        /// <summary>
        /// LMS-5xx扫描仪可选角分辨率
        /// </summary>
        public static List<double> AngleResolutions_5xx = new List<double>() { 0.1667, 0.25, 0.333, 0.5, 0.667, 1 };
    }
}
