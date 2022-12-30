using ScanUtilityLibrary.Core.SICK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Model.SICK
{
    /// <summary>
    /// 西克（SICK）设备的用户信息类
    /// </summary>
    public class UserInfo
    {
        ///// <summary>
        ///// 用户级别代码(维护人员 - 2/授权用户 - 3/维修 - 4)
        ///// </summary>
        //public byte UserCode { get; set; }

        /// <summary>
        /// 用户级别(维护人员/授权用户/维修)
        /// </summary>
        public UserLevel UserLevel { get; set; }

        /// <summary>
        /// 登录密码(8位16进制哈希值)
        /// </summary>
        public string UserPassword { get; set; }

        /// <summary>
        /// 登录后设备返回的信息
        /// </summary>
        public string LoginInfo { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        public UserInfo()
        {
            Reset();
        }

        /// <summary>
        /// 信息重置
        /// </summary>
        public void Reset()
        {
            //UserCode = 0;
            UserLevel = UserLevel.Run;
            UserPassword = "00000000";
            LoginInfo = string.Empty;
        }
    }
}
