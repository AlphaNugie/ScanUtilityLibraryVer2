using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Model.SICK.Dx
{
    /// <summary>
    /// 设备状态字
    /// </summary>
    public class DeviceStatusWord
    {
        #region 属性
        /// <summary>
        /// 状态字的源32位无符号整型值
        /// </summary>
        public uint SourceValue { get; set; }

        /// <summary>
        /// 红外激光错误
        /// </summary>
        public bool ErrorLaser { get; set; }

        /// <summary>
        /// 硬件错误
        /// </summary>
        public bool ErrorHardware { get; set; }

        /// <summary>
        /// 测量错误
        /// </summary>
        public bool ErrorMeasurement { get; set; }

        /// <summary>
        /// 温度错误
        /// </summary>
        public bool ErrorTemperature { get; set; }

        /// <summary>
        /// 环境光错误
        /// </summary>
        public bool ErrorAmbientLight { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //public bool Reserved { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //public bool Reserved { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //public bool Reserved { get; set; }

        /// <summary>
        /// 红外激光警告
        /// </summary>
        public bool WarningLaser { get; set; }

        /// <summary>
        /// 固件警告
        /// </summary>
        public bool WarningFirmware { get; set; }

        /// <summary>
        /// Warning: Short-circuit at switching output
        /// </summary>
        public bool WarningShortCircuit { get; set; }

        /// <summary>
        /// 温度警告
        /// </summary>
        public bool WarningTemperature { get; set; }

        /// <summary>
        /// 环境光警告
        /// </summary>
        public bool WarningAmbientLight { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //public bool Reserved { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //public bool Reserved { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //public bool Reserved { get; set; }

        /// <summary>
        /// 位置校对激光启用
        /// </summary>
        public bool AlignmentLaserActive { get; set; }

        /// <summary>
        /// 测量激光启用
        /// </summary>
        public bool MeasurementLaserActive { get; set; }

        /// <summary>
        /// 设备加热启用
        /// </summary>
        public bool HeatingActive { get; set; }

        /// <summary>
        /// 无回波（超时？）
        /// </summary>
        public bool NoEcho_DelayTimeActive { get; set; }

        /// <summary>
        /// 无回波
        /// </summary>
        public bool NoEcho { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool StatusIn2 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool StatusQ4 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool StatusQ3 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool StatusQ2 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool StatusQ1_In1 { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //public bool Reserved { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool SignalLevelQ4 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool SignalLevelQ3 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool SignalLevelQ2 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool SignalLevelQ1_In1 { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //public bool Reserved { get; set; }
        #endregion

        /// <summary>
        /// 构造器
        /// </summary>
        public DeviceStatusWord() { }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="word"></param>
        public DeviceStatusWord(uint word)
        {
            Update(word);
        }

        /// <summary>
        /// 更新状态字
        /// </summary>
        /// <param name="word"></param>
        public void Update(uint word)
        {
            SourceValue = word;
            string binary = Convert.ToString(SourceValue, 2).PadLeft(32, '0');
            //将bit位前后顺序调换以适应官方文档的高低位索引
            char[] array = binary.ToArray().Reverse().ToArray();
            ErrorLaser = array[31].Equals('1');
            ErrorHardware = array[30].Equals('1');
            ErrorMeasurement = array[29].Equals('1');
            ErrorTemperature = array[28].Equals('1');
            ErrorAmbientLight = array[27].Equals('1');
            WarningLaser = array[23].Equals('1');
            WarningFirmware = array[22].Equals('1');
            WarningShortCircuit = array[21].Equals('1');
            WarningTemperature = array[20].Equals('1');
            WarningAmbientLight = array[19].Equals('1');
            AlignmentLaserActive = array[15].Equals('1');
            MeasurementLaserActive = array[14].Equals('1');
            HeatingActive = array[13].Equals('1');
            NoEcho_DelayTimeActive = array[12].Equals('1');
            NoEcho = array[11].Equals('1');
            StatusIn2 = array[10].Equals('1');
            StatusQ4 = array[9].Equals('1');
            StatusQ3 = array[8].Equals('1');
            StatusQ2 = array[7].Equals('1');
            StatusQ1_In1 = array[6].Equals('1');
            SignalLevelQ4 = array[4].Equals('1');
            SignalLevelQ3 = array[3].Equals('1');
            SignalLevelQ2 = array[2].Equals('1');
            SignalLevelQ1_In1 = array[1].Equals('1');
        }
    }
}
