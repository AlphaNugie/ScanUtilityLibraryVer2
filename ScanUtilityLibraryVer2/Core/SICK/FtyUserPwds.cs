using ScanUtilityLibrary.Model.SICK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Core.SICK
{
    /// <summary>
    /// 出厂用户名密码与哈希值
    /// </summary>
    public static class FtyUserPwds
    {
        /// <summary>
        /// 维护检查
        /// </summary>
        public static StrHashPair Maintenance { get; set; } = new StrHashPair("main", "B21ACE26");

        /// <summary>
        /// 授权用户
        /// </summary>
        public static StrHashPair AuthorisedClient { get; set; } = new StrHashPair("client", "F4724744");

        /// <summary>
        /// 检修
        /// </summary>
        public static StrHashPair Service { get; set; } = new StrHashPair("servicelevel", "81BE23AA");
    }
}
