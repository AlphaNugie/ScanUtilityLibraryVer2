using ScanUtilityLibrary.Model.Tianhe;
using ScanUtilityLibrary.Tianhe.Core.Tianhe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Tianhe.Model.Tianhe
{
    /// <summary>
    /// TIP数据头
    /// </summary>
    public class TipHeadHeartbeat : TipHeadBase
    {
        #region 属性
        #endregion

        #region 构造器
        /// <summary>
        /// 用给定的16进制字符串初始化
        /// </summary>
        /// <param name="hexString">报文的16进制字符串</param>
        public TipHeadHeartbeat(string hexString) : base(hexString, StreamDirection.ToDevice) { }

        /// <summary>
        /// 用给定的byte数组初始化
        /// </summary>
        /// <param name="bytes">报文的字节流</param>
        public TipHeadHeartbeat(byte[] bytes) : base(bytes, StreamDirection.ToDevice) { }
        #endregion

        protected override void ResolveUrOwn(byte[] bytes)
        {
            //不需进行额外解析
            return;
        }

        protected override List<byte> ComposeUrOwn()
        {
            //预留8个字节均为0
            return new List<byte>() { 0, 0, 0, 0, 0, 0, 0, 0 };
        }
    }
}
