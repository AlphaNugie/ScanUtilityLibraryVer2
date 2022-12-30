using ScanUtilityLibrary.Core.Tianhe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Model.Tianhe
{
    /// <summary>
    /// 通用的TIP数据头，在继承基础类的基础上没有额外的属性与解析、组合工作，预留属性的8个字节均为0
    /// 适用于Login/LoginReply/Heartbeat/HeartbeatReply/Blocked/BlockedNoMore/Contaminated/ContaminatedNoMore/ErrorNoMore/Reboot/SetTimeReply/RequestDevInfo/RequestDevInfoReply/RequestDevParams/RequestDevParamsReply/RequestDevState/RequestDevStateReply/RequestDataFormat/
    /// 不适用于DataOutput/DataOutput3d/Error/SetPowerState/SetPowerStateReply/SetTime/RequsetDataFormatReply/SetDevIO/SetDevIOReply/RequsetDevCtrlState/RequsetDevCtrlStateReply/RequsetDevSgnlState/RequsetDevSgnlStateReply/
    /// </summary>
    public class TipHeadCommon : TipHeadBase
    {
        #region 属性
        #endregion

        #region 构造器
        /// <summary>
        /// 用给定的16进制字符串初始化
        /// </summary>
        /// <param name="hexString">报文的16进制字符串</param>
        public TipHeadCommon(string hexString) : base(hexString, StreamDirection.ToDevice) { }

        /// <summary>
        /// 用给定的byte数组初始化
        /// </summary>
        /// <param name="bytes">报文的字节流</param>
        public TipHeadCommon(byte[] bytes) : base(bytes, StreamDirection.ToDevice) { }
        #endregion

        /// <inheritdoc/>
        protected override void ResolveUrOwn(byte[] bytes)
        {
            //不需进行额外解析
            return;
        }

        /// <inheritdoc/>
        protected override List<byte> ComposeUrOwn()
        {
            //预留8个字节均为0
            return new List<byte>() { 0, 0, 0, 0, 0, 0, 0, 0 };
        }
    }
}
