using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Core.SICK.Dx
{
    /// <summary>
    /// 各指令正则表达式模式，\u0002匹配STX（正文开始），\u0003匹配ETX（正文结束）
    /// </summary>
    public class Patterns : BasePatterns
    {
        ///// <summary>
        ///// 返回指令的笼统格式
        ///// </summary>
        //public const string OutputInGeneral = @"[\u0002]s[\s\S]*[\u0003]";

        /// <summary>
        /// 连续输出的任何类型值（[0321-0324][正负符号]7*[10进制数字]，后面或者不跟任何内容，或者包含一个 [正负符号/下划线]5~8*[16进制数字]）
        /// </summary>
        public const string ContiAnyOutput = @"[\u0002]032[1-4][+-][0-9]{7}([+-_][0-9a-fA-F]{5,8})?[\u0003]";

        /// <summary>
        /// 连续输出的距离值（[STX]0322[正负符号]7*[10进制数字][ETX]，如[STX]0322+0001800[ETX]）
        /// </summary>
        public const string ContiDistOutput = @"[\u0002]0322[+-][0-9]{7}[\u0003]";

        /// <summary>
        /// 连续输出的距离、速度值（[STX]0324[正负符号]7*[10进制数字][正负符号]5*[0...9][ETX]，如[STX]0324+0001800+02000[ETX]）
        /// </summary>
        public const string ContiDistVelOutput = @"[\u0002]0324[+-][0-9]{7}[+-][0-9]{5}[\u0003]";

        /// <summary>
        /// 连续输出的距离、状态字值（[STX]0321[正负符号]7*[10进制数字]_8*[16进制数字][ETX]，如[STX]0321+0001800_0010C100[ETX]）
        /// </summary>
        public const string ContiDistStatusOutput = @"[\u0002]0321[+-][0-9]{7}[_][0-9a-fA-F]{8}[\u0003]";

        /// <summary>
        /// 连续输出的距离、信号级别值（[STX]0323[正负符号]7*[10进制数字]_5*[10进制数字][ETX]，如[STX]0323+0001800_02300[ETX]）
        /// </summary>
        public const string ContiDistSigLevelOutput = @"[\u0002]0323[+-][0-9]{7}[_][0-9]{5}[\u0003]";
    }
}
