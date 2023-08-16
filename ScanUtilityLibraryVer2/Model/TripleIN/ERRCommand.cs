using ScanUtilityLibrary.Core.TripleIN;
using ScanUtilityLibrary.Model.TripleIN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Model.TripleIN
{
    /// <summary>
    /// Reading the firmware version.
    /// </summary>
    public class ERRCommand : CommandBase
    {
        /// <summary>
        /// 发生的错误
        /// </summary>
        public ErrorID Error { get; private set; }

        /// <summary>
        /// 发生时间
        /// </summary>
        public DateTime Time { get; private set; }

        #region 构造器
        /// <summary>
        /// 默认构造器
        /// </summary>
        public ERRCommand() : this(string.Empty) { }
        //public ERRCommand() : base(FunctionCodes.ERR, string.Empty) { }

        /// <summary>
        /// 用给定的16进制字符串初始化
        /// </summary>
        /// <param name="hexString">报文的16进制字符串</param>
        public ERRCommand(string hexString) : base(FunctionCodes.ERR, hexString) { }

        /// <summary>
        /// 用给定的byte数组初始化
        /// </summary>
        /// <param name="bytes">报文的字节流</param>
        public ERRCommand(byte[] bytes) : base(FunctionCodes.ERR, bytes) { }
        #endregion

        /// <inheritdoc/>
        protected override List<byte> ComposeUrOwn()
        {
            return null;
        }

        /// <inheritdoc/>
        protected override void ResolveUrOwn(byte[] bytes)
        {
            int result = (int)ErrorID.ERR_SUCCESS;
            if (bytes != null && bytes.Length >= 4)
                result = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, 0));
            if (result < 0)
                Time = DateTime.Now.AddMilliseconds(0);
            Error = ErrorIDConverter.IntToError(result);
        }
    }

}