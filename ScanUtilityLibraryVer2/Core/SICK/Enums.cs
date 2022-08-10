using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Core.SICK
{
    /// <summary>
    /// 用户级别
    /// </summary>
    public enum UserLevel
    {
        /// <summary>
        /// 设备正在运行
        /// </summary>
        Run = 0,

        /// <summary>
        /// 操作员（不需登录，显示参数与测量值）
        /// </summary>
        Operator = 1,

        /// <summary>
        /// 维护检查（显示参数与测量值）
        /// </summary>
        Maintenance = 2,

        /// <summary>
        /// 授权用户（显示参数与测量值，修改参数）
        /// </summary>
        AuthorisedClient = 3,

        /// <summary>
        /// 检修（显示参数与测量值，修改参数，升级固件）
        /// </summary>
        Service = 4
    }
}
