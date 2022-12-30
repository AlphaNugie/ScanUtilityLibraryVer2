using CommonLib.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Model.Tianhe
{
    /// <summary>
    /// 雷达时间戳信息结构体
    /// </summary>
    public class TimeStamp
    {
        /// <summary>
        /// 秒数
        /// </summary>
        public uint Seconds { get; set; }

        /// <summary>
        /// 微秒数
        /// </summary>
        public uint Microseconds { get; set; }

        #region 构造器
        /// <summary>
        /// 默认构造器
        /// </summary>
        public TimeStamp() : this(string.Empty) { }

        /// <summary>
        /// 用给定的16进制字符串初始化
        /// </summary>
        /// <param name="hexString"></param>
        public TimeStamp(string hexString)
        {
            Resolve(hexString);
        }

        /// <summary>
        /// 用给定的byte数组初始化
        /// </summary>
        /// <param name="bytes"></param>
        public TimeStamp(byte[] bytes)
        {
            Resolve(bytes);
        }
        #endregion

        #region 功能
        /// <summary>
        /// 根据16进制字符串解析
        /// </summary>
        /// <param name="hexString"></param>
        public void Resolve(string hexString)
        {
            if (string.IsNullOrWhiteSpace(hexString))
                return;
            Resolve(HexHelper.HexString2Bytes(hexString));
        }

        /// <summary>
        /// 根据byte数组解析
        /// </summary>
        /// <param name="bytes"></param>
        public void Resolve(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 8)
                return;
            Seconds = BitConverter.ToUInt32(bytes, 0);
            Microseconds = BitConverter.ToUInt32(bytes, 4);
        }
        #endregion
    }
}
