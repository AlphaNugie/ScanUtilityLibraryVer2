using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Core.SICK.Dx
{
    /// <summary>
    /// 持续输出的数据类型
    /// </summary>
    public enum ContinualOutputType
    {
        /// <summary>
        /// 关闭持续输出
        /// </summary>
        OutputOff = 0,

        /// <summary>
        /// 只输出距离
        /// </summary>
        Distance = 1,

        /// <summary>
        /// 输出距离+速度
        /// </summary>
        DistVel = 2,

        /// <summary>
        /// 输出距离+状态字（32位bit，8个16进制数）
        /// </summary>
        DistStatus = 3,

        /// <summary>
        /// 距离+信号级别(Signal Level/RSSI)
        /// </summary>
        DistSigLevel = 4
    }

    /// <summary>
    /// 连续输出的报文协议
    /// </summary>
    public enum ContinualOutputProtocol
    {
        /// <summary>
        /// 连续输出的STX/ETX协议：每次输出以报文协议中的起始、终止字符进行开始与结尾
        /// </summary>
        STXETX = 0,

        /// <summary>
        /// 连续输出的CRLF协议：每次输出以回车换行符结尾
        /// </summary>
        CRLF = 1
    }

    /// <summary>
    /// 主动获取的数据类型
    /// </summary>
    public enum RequiredDataType
    {
        /// <summary>
        /// 距离
        /// </summary>
        Distance = 0,

        /// <summary>
        /// 速度
        /// </summary>
        Velocity,

        /// <summary>
        /// 信号级别（Signal Level/RSSI）
        /// </summary>
        SignalLevel,

        /// <summary>
        /// 设备温度
        /// </summary>
        DeviceTemperature,

        /// <summary>
        /// 设备使用小时数
        /// </summary>
        OperatingHours,

        /// <summary>
        /// 设备状态字
        /// </summary>
        DeviceStatusWord
    }
}
