using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Core.SICK.LMS
{
    /// <summary>
    /// LMS扫描仪型号
    /// </summary>
    public enum ScannerVersion
    {
        /// <summary>
        /// LMS 100系列
        /// </summary>
        LMS_1xx = 1,

        /// <summary>
        /// LMS 500系列
        /// </summary>
        LMS_5xx = 2
    }

    /// <summary>
    /// 设备状态
    /// </summary>
    [Flags]
    public enum DeviceStatus
    {
        /// <summary>
        /// 没有问题
        /// </summary>
        OK = 0,

        /// <summary>
        /// 有错误
        /// </summary>
        Error = 1,

        /// <summary>
        /// 污染警告
        /// </summary>
        PollutionWarning = 2,

        /// <summary>
        /// 污染错误（无设备错误）
        /// </summary>
        PollutionError = 4,

        /// <summary>
        /// 污染错误+设备错误
        /// </summary>
        PollutionErrorWithDeviceError = 5,
    }

    /// <summary>
    /// 设备开关量输入的可能状态
    /// </summary>
    public enum DigitalInputStatus
    {
        /// <summary>
        /// 所有输入位于低位
        /// </summary>
        AllInputsLow = 0,

        /// <summary>
        /// 所有输入位于高位
        /// </summary>
        AllInputsHigh = 30
    }

    /// <summary>
    /// 设备开关量输出的可能状态
    /// </summary>
    public enum DigitalOutputStatus
    {
        /// <summary>
        /// 所有输出位于低位
        /// </summary>
        AllOutputsLow = 0,

        /// <summary>
        /// 所有内部输出位于高位
        /// </summary>
        AllInternalOutputsHigh = 0x3F0,

        /// <summary>
        /// 所有输出位于高位（包括外部输出）
        /// </summary>
        AllOutputsHigh = 0x3FFF
    }
}
