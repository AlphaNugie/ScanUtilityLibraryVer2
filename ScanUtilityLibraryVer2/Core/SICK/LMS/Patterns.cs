using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Core.SICK.LMS
{
    /// <summary>
    /// 各指令正则表达式模式
    /// </summary>
    public class Patterns : BasePatterns
    {
        ///// <summary>
        ///// 返回指令的笼统格式
        ///// </summary>
        //public const string OutputInGeneral = @"[\u0002]s[\s\S]*[\u0003]";

        /// <summary>
        /// 单次读，\u0002匹配STX（正文开始），\u0003匹配ETX（正文结束）
        /// </summary>
        public const string SingleOutput = @"[\u0002]sSN\sLMDscandata[\s\S]*[\u0003]";

        /// <summary>
        /// 连续读，\u0002匹配STX（正文开始），\u0003匹配ETX（正文结束）
        /// </summary>
        public const string ContinuousOutput = @"[\u0002]sRA\sLMDscandata[\s\S]*[\u0003]";

        /// <summary>
        /// 返回数据格式，连续读或单次读，\u0002匹配STX（正文开始），\u0003匹配ETX（正文结束）
        /// </summary>
        public const string DataOutput = @"[\u0002](sSN|sRA)\sLMDscandata[\s\S]*[\u0003]";

        ///// <summary>
        ///// 登录回应，\u0002匹配STX（正文开始），\u0003匹配ETX（正文结束）
        ///// </summary>
        //public const string LoginResponse = @"[\u0002]sAN\sSetAccessMode[\s\S]*[\u0003]";
    }
}
