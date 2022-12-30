using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScanUtilityLibrary.Core.R2000
{
    /// <summary>
    /// 设备类型
    /// </summary>
    public enum DeviceFamily
    {
        /// <summary>
        /// 位置类型
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// R2000 UHD
        /// </summary>
        OMDxxx_R2000_UHD = 1,

        /// <summary>
        /// R2000 HD
        /// </summary>
        OMDxxx_R2000_HD = 3
    }

    /// <summary>
    /// 数据包类型
    /// </summary>
    public enum PacketType
    {
        /// <summary>
        /// 无类型
        /// </summary>
        NONE,

        /// <summary>
        /// 距离
        /// </summary>
        A,

        /// <summary>
        /// 距离+能量反馈
        /// </summary>
        B,

        /// <summary>
        /// 距离+能量反馈(C)
        /// </summary>
        C
    }
}
