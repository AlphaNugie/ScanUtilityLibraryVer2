using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Core.TripleIN
{
    /// <summary>
    /// 所有的2进制命令功能码
    /// </summary>
    public enum FunctionCodes
    {
        /// <summary>
        /// 无定义
        /// </summary>
        NONE = -1,

        /// <summary>
        /// Version
        /// Requesting the Firmware Version
        /// </summary>
        GVER = 0,

        /// <summary>
        /// Get real time clock count
        /// Requesting the real time clock count
        /// </summary>
        GRTC,

        /// <summary>
        /// Set real time clock count
        /// Set the real time clock
        /// </summary>
        SRTC,

        /// <summary>
        /// Start scan
        /// Starts the scanning mode
        /// </summary>
        SCAN,

        /// <summary>
        /// Get scanned scan
        /// Requesting one scan measurement. Returns a scan from the scan buffer
        /// </summary>
        GSCN,

        /// <summary>
        /// Read parameter
        /// Uses a parameter identification code to obtain a single parameter from the sensor.
        /// </summary>
        GPRM,

        /// <summary>
        /// Get parameter info
        /// Gives an information text about a parameter
        /// </summary>
        GPIN,

        /// <summary>
        /// Write parameter
        /// Set a value for a single sensor parameter.
        /// </summary>
        SPRM,

        /// <summary>
        /// 错误信息
        /// </summary>
        ERR,

        /// <summary>
        /// 同步信息
        /// </summary>
        SYNC
    }
}
