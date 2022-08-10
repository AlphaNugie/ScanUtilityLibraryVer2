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
    /// 信息包类型
    /// </summary>
    public enum PacketTypes
    {
        /// <summary>
        /// 
        /// </summary>
        A = 0,

        /// <summary>
        /// 
        /// </summary>
        B = 1,

        /// <summary>
        /// 
        /// </summary>
        C = 2
    }
}
