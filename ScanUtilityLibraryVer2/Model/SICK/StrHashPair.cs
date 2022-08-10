using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Model.SICK
{
    /// <summary>
    /// 字符串与哈希值的键值对
    /// </summary>
    public class StrHashPair
    {
        /// <summary>
        /// 字符串的值
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// 哈希值
        /// </summary>
        public string Hash { get; private set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="value"></param>
        /// <param name="hash"></param>
        internal StrHashPair(string value, string hash)
        {
            Value = value;
            Hash = hash;
        }
    }
}
