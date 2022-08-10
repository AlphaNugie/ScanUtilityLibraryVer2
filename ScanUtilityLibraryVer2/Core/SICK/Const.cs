using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Core.SICK
{
    /// <summary>
    /// 公有变量类
    /// </summary>
    public class Const
    {
        /// <summary>
        /// 登录后设备返回的信息
        /// </summary>
        public static string LoginInfo = string.Empty;

        /// <summary>
        /// 用户级别代码(维护人员 - 2/授权用户 - 3/维修 - 4)
        /// </summary>
        public static byte UserCode = 2;

        /// <summary>
        /// 用户级别(维护人员/授权用户/维修)
        /// </summary>
        public static string UserLevel = string.Empty;

        /// <summary>
        /// 登录密码(8位16进制哈希值)
        /// </summary>
        public static string UserPassword = "00000000";

        /// <summary>
        /// 特殊字符，代表正文开始
        /// </summary>
        public const char STX = (char)2;

        /// <summary>
        /// 特殊字符，代表正文结束
        /// </summary>
        public const char ETX = (char)3;
    }
}
