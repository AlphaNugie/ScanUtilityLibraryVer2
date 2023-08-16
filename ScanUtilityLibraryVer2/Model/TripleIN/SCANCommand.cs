using CommonLib.Extensions;
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
    public class SCANCommand : CommandBase
    {
        /// <summary>
        /// 扫描数，默认为1，大于0时触发扫描仪开始扫描，设置为0则扫描功能关闭
        /// </summary>
        public int ScanNumber { get; private set; }

        #region 构造器
        /// <summary>
        /// 用默认的扫描数1初始化，触发扫描仪开始扫描
        /// </summary>
        public SCANCommand() : this(1) { }
        //public SCANCommand() : base(FunctionCodes.SCAN, string.Empty) { }

        /// <summary>
        /// 用给定的扫描数初始化，默认为1，大于0时触发扫描仪开始扫描，设置为0则扫描功能关闭
        /// </summary>
        /// <param name="scanNum"></param>
        public SCANCommand(int scanNum) : this(string.Empty) { ScanNumber = scanNum; }

        /// <summary>
        /// 用给定的16进制字符串初始化
        /// </summary>
        /// <param name="hexString">报文的16进制字符串</param>
        public SCANCommand(string hexString) : base(FunctionCodes.SCAN, hexString) { }

        /// <summary>
        /// 用给定的byte数组初始化
        /// </summary>
        /// <param name="bytes">报文的字节流</param>
        public SCANCommand(byte[] bytes) : base(FunctionCodes.SCAN, bytes) { }
        #endregion

        /// <inheritdoc/>
        protected override List<byte> ComposeUrOwn()
        {
            uint source = (uint)ScanNumber; //组合为byte数组的uint数据源
            return source.ToBytes().ToList();
        }

        /// <inheritdoc/>
        protected override void ResolveUrOwn(byte[] bytes)
        {
            //ScanNumber = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, 0));
            ScanNumber = bytes.ToInt32();
        }
    }

}