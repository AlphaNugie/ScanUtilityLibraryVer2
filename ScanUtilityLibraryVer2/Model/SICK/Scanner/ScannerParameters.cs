using CommonLib.Function;
using ScanUtilityLibrary.Core.SICK.Scanner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Model.SICK.Scanner
{
    /// <summary>
    /// LMS扫描仪参数
    /// </summary>
    public class ScannerParameters
    {
        #region 属性
        /// <summary>
        /// 设备版本号
        /// </summary>
        public ushort VersionNumber { get; set; }

        /// <summary>
        /// 设备ID
        /// </summary>
        public ushort DeviceNumber { get; set; }

        /// <summary>
        /// 设备序列号
        /// </summary>
        public uint SerialNumber { get; set; }

        /// <summary>
        /// 设备状态
        /// 00 OK；01 错误；02 污染警告；04 有污染但设备无问题；05 有污染且设备有问题
        /// </summary>
        public DeviceStatus DeviceStatus { get; set; }

        /// <summary>
        /// 指令计数，设备内处理完成的指令数量
        /// </summary>
        public ushort TelegramCounter { get; set; }

        /// <summary>
        /// 扫描计数，实际进行的扫描次数
        /// </summary>
        public ushort ScanCounter { get; set; }

        /// <summary>
        /// 扫描开始时间（单位微秒）
        /// </summary>
        public ulong TimeSinceStartUp { get; set; }

        /// <summary>
        /// 扫描传输时间（单位微秒）
        /// </summary>
        public ulong TimeOfTransmission { get; set; }

        /// <summary>
        /// 设备开关量输入状态
        /// 00: All inputs low; 30: All inputs high
        /// </summary>
        public DigitalInputStatus StatusOfDigitalInputs { get; set; }

        /// <summary>
        /// 设备开关量输出状态
        /// 0 0: All outputs low; 3F 0: All internal outputs high; 3F FF: All outputs high (inkl. Ext. Out)
        /// </summary>
        public DigitalOutputStatus StatusOfDigitalOutputs { get; set; }

        /// <summary>
        /// 回波扇区角度（一直为0）
        /// </summary>
        public ushort LayerAngle { get; set; }

        /// <summary>
        /// 扫描频率(单位：HZ)
        /// </summary>
        public double ScanFrequency { get; set; }
        //public uint ScanFrequency { get; set; }

        /// <summary>
        /// 每次扫描频率
        /// </summary>
        public uint MeasurementFrequency { get; set; }

        /// <summary>
        /// 编码器数量(0~3，若为0则无编码器)
        /// </summary>
        public byte AmountOfEncoder { get; set; }

        /// <summary>
        /// 编码器位置
        /// </summary>
        public uint EncoderPosition { get; set; }

        /// <summary>
        /// 编码器速度
        /// </summary>
        public ushort EncoderSpeed { get; set; }

        /// <summary>
        /// 回波层数量（16 bit，对于LMS 5xx来说这个值是1或5）
        /// </summary>
        public ushort AmountOfChannels { get; set; }

        /// <summary>
        /// 回波层序号（DIST/RSSI 1~5）
        /// </summary>
        public string OutputChannel { get; set; }

        //private string _scaleStr;
        ///// <summary>
        ///// 测量值比例系数的16进制字符串描述
        ///// </summary>
        //public string ScaleFactorString
        //{
        //    get { return _scaleStr; }
        //    set
        //    {
        //        _scaleStr = value;
        //        ScaleFactor = HexHelper.HexString2Single(_scaleStr);
        //        //switch (_scaleStr)
        //        //{
        //        //    case "3F800000":
        //        //        ScaleFactor = 1;
        //        //        break;
        //        //    case "40000000":
        //        //        ScaleFactor = 2;
        //        //        break;
        //        //    default:
        //        //        ScaleFactor = 0;
        //        //        break;
        //        //}
        //    }
        //}

        /// <summary>
        /// 测量值比例系数
        /// </summary>
        public double ScaleFactor { get; set; }

        /// <summary>
        /// 系数偏移量（一直为0）
        /// </summary>
        public double ScaleOffset { get; set; }

        /// <summary>
        /// 开始角度(单位：°)
        /// </summary>
        public double StartAngle { get; set; }

        /// <summary>
        /// 角分辨率(单位：°)
        /// </summary>
        public double AngleResolution { get; set; }

        /// <summary>
        /// 测量数据数量
        /// </summary>
        public ushort AmountOfData { get; set; }
        #endregion

        /// <summary>
        /// 构造器
        /// </summary>
        public ScannerParameters() { }
        
        /// <summary>
        /// 解析参数
        /// </summary>
        /// <param name="results">设备返回扫描数据分解后的字符串数组（不包括STX, ETX）</param>
        public void ResolveParameters(string[] results)
        {
            if (results == null || results.Length < 26)
                return;

            VersionNumber = Convert.ToUInt16(results[2], 16); //设备版本号
            DeviceNumber = Convert.ToUInt16(results[3], 16); //设备ID
            SerialNumber = Convert.ToUInt32(results[4], 16); //设备序列号
            DeviceStatus = (DeviceStatus)Convert.ToByte(results[5] + results[6]); //设备状态
            TelegramCounter = Convert.ToUInt16(results[7], 16); //指令计数
            ScanCounter = Convert.ToUInt16(results[8], 16); //扫描计数
            TimeSinceStartUp = Convert.ToUInt64(results[9], 16); //扫描开始时间
            TimeOfTransmission = Convert.ToUInt64(results[10], 16); //扫描结束时间
            StatusOfDigitalInputs = (DigitalInputStatus)Convert.ToByte(results[11] + results[12]); //设备开关量输入状态
            //StatusOfDigitalOutputs = Convert.ToByte(results[15], 16); //设备开关量输出状态
            StatusOfDigitalOutputs = (DigitalOutputStatus)Convert.ToUInt16(results[13] + results[14], 16); //设备开关量输出状态
            LayerAngle = Convert.ToUInt16(results[15]);
            ScanFrequency = (double)Convert.ToUInt32(results[16], 16) / 100; //扫描频率
            MeasurementFrequency = Convert.ToUInt32(results[17], 16); //每次扫描频率
            AmountOfEncoder = Convert.ToByte(results[18]); //编码器数量
            if (AmountOfEncoder > 0)
            {
                EncoderPosition = Convert.ToUInt32(results[19], 16); //编码器位置
                EncoderSpeed = Convert.ToUInt16(results[20], 16); //编码器速度
            }
            int offset = AmountOfEncoder > 0 ? 2 : 0;
            AmountOfChannels = Convert.ToUInt16(results[19 + offset]); //回波层数量
            OutputChannel = results[20 + offset]; //回波层序号
            //ScaleFactorString = results[21 + offset]; //系数
            //ScaleOffset = Convert.ToInt32(results[22 + offset], 16); //系数偏移量
            ScaleFactor = HexHelper.HexString2Single(results[21 + offset]); //系数
            ScaleOffset = HexHelper.HexString2Single(results[22 + offset]); //系数偏移量
            StartAngle = (double)Convert.ToInt32(results[23 + offset], 16) / 10000;
            AngleResolution = (double)Convert.ToInt32(results[24 + offset], 16) / 10000;
            AmountOfData = Convert.ToUInt16(results[25 + offset], 16);
        }
    }
}
