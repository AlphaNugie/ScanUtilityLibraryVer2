using ScanUtilityLibrary.Core.Tianhe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Model.Tianhe
{
    /// <summary>
    /// 通用的TIP数据体，在继承基础类的基础上没有额外的属性与解析、组合工作
    /// 适用于LoginReply/Heartbeat/HeartbeatReply/Blocked/BlockedNoMore/Contaminated/ContaminatedNoMore/ErrorNoMore/Reboot/SetTimeReply/RequestDevInfo/RequestDevParams/RequestDevState/RequestDevStateReply/RequestDataFormat/
    /// 不适用于Login/DataOutput/DataOutput3d/Error/SetPowerState/SetPowerStateReply/SetTime/RequestDevInfoReply/RequestDevParamsReply/RequsetDataFormatReply/SetDevIO/SetDevIOReply/RequsetDevCtrlState/RequsetDevCtrlStateReply/RequsetDevSgnlState/RequsetDevSgnlStateReply/
    /// </summary>
    public class TipBodyCommon : TipBodyBase
    {
        #region 属性
        /// <summary>
        /// 协议数据头
        /// </summary>
        public new TipHeadCommon TipHead { get; set; }
        #endregion

        #region 构造器
        /// <summary>
        /// 用给定的TIP功能码初始化
        /// </summary>
        /// <param name="code">给定的TIP功能码</param>
        public TipBodyCommon(TipCode code) : this(code, string.Empty) { }

        /// <summary>
        /// 用给定的TIP功能码、16进制字符串初始化
        /// </summary>
        /// <param name="code">给定的TIP功能码</param>
        /// <param name="hexString"></param>
        public TipBodyCommon(TipCode code, string hexString) : base(hexString)
        {
            UpdateTipCode_Composing(code);
        }

        /// <summary>
        /// 用给定的TIP功能码、byte数组初始化
        /// </summary>
        /// <param name="code">给定的TIP功能码</param>
        /// <param name="bytes"></param>
        public TipBodyCommon(TipCode code, byte[] bytes) : base(bytes)
        {
            UpdateTipCode_Composing(code);
        }
        #endregion

        #region 抽象方法实现
        /// <inheritdoc/>
        protected override List<byte> ComposeUrOwn()
        {
            //不需要组合额外的字节流
            return null;
        }

        /// <inheritdoc/>
        protected override byte[] GetTipHeadBytes()
        {
            return TipHead.Compose();
        }

        /// <inheritdoc/>
        protected override void InitTipHead(byte[] bytes)
        {
            TipHead = new TipHeadCommon(bytes);
        }

        /// <inheritdoc/>
        protected override void ResolveUrOwn(byte[] bytes)
        {
            //不需进行额外解析
            return;
        }

        /// <inheritdoc/>
        protected override void UpdateAllBytesLen_Composing(uint abtLen)
        {
            TipHead.UpdateAllBytesLen(abtLen);
        }

        /// <inheritdoc/>
        protected override void UpdateTipCode_Composing(TipCode code = TipCode.None)
        {
            //仅当给定的参数不为NONE才可进行赋值
            //父类的字节流组合方法Compose会调用使用NONE作为默认参数的无参版本，假如不进行条件判断，则每次组合字节流时初始化的功能码都将被NONE覆盖掉
            if (code != TipCode.None)
                TipHead.UpdateTipCode(code);
        }
        #endregion
    }
}
