using CommonLib.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Model.Tianhe
{
    /// <summary>
    /// 雷达同步信息结构体
    /// </summary>
    public class SyncInfo
    {
        /// <summary>
        /// 最后一次同步信号以来的精确时间
        /// </summary>
        public uint Microseconds { get; set; }

        #region 构造器
        /// <summary>
        /// 默认构造器
        /// </summary>
        public SyncInfo() : this(string.Empty) { }

        /// <summary>
        /// 用给定的16进制字符串初始化
        /// </summary>
        /// <param name="hexString"></param>
        public SyncInfo(string hexString)
        {
            Resolve(hexString);
        }

        /// <summary>
        /// 用给定的byte数组初始化
        /// </summary>
        /// <param name="bytes"></param>
        public SyncInfo(byte[] bytes)
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
            if (bytes == null || bytes.Length < 4)
                return;
            Microseconds = BitConverter.ToUInt32(bytes, 0);
        }
        #endregion
    }
}
