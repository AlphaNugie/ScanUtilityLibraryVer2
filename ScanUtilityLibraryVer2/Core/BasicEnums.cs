using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Core
{
    /// <summary>
    /// 所有扫描仪的型号（不包括倍加福扫描仪）
    /// </summary>
    public enum AllScannerVersion
    {
        /// <summary>
        /// 不属于任何型号
        /// </summary>
        None = 0,

        /// <summary>
        /// LMS 100系列
        /// </summary>
        LMS_1xx = 1,

        /// <summary>
        /// LMS 500系列
        /// </summary>
        LMS_5xx = 2,

        /// <summary>
        /// LD-LRS 36x1系列
        /// </summary>
        LRS_36x1 = 3,

        /// <summary>
        /// 保定天河GL11系列雷达
        /// </summary>
        Tianhe_GL11xx = 4,
    }
}
