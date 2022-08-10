using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Core.SICK
{
    /// <summary>
    /// 指令正则表达式模式
    /// </summary>
    public class BasePatterns
    {
        /// <summary>
        /// 返回指令的笼统格式，开头结尾为STX, ETX，中间可包含除STX ETX外的一切字符
        /// </summary>
        public const string OutputInGeneral = @"[\u0002][^\u0002\u0003]+[\u0003]";
        //public static string OutputInGeneral = @"[\u0002]s[\s\S]*[\u0003]";

        /// <summary>
        /// 登录回应，\u0002匹配STX（正文开始），\u0003匹配ETX（正文结束）
        /// </summary>
        public const string LoginResponse = @"[\u0002]sAN\sSetAccessMode[\s\S]*[\u0003]";
    }
}
