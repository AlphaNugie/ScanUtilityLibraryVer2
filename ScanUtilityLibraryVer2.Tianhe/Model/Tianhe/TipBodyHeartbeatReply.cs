using ScanUtilityLibrary.Tianhe.Core.Tianhe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Tianhe.Model.Tianhe
{
    public class TipBodyHeartbeatReply : TipBodyBase
    {
        #region 属性
        /// <summary>
        /// 协议数据头
        /// </summary>
        public new TipHeadCommon TipHead { get; set; }
        #endregion

        #region 构造器
        /// <summary>
        /// 默认构造器
        /// </summary>
        public TipBodyHeartbeatReply() : base(string.Empty) { }

        /// <summary>
        /// 用给定的16进制字符串初始化
        /// </summary>
        /// <param name="hexString"></param>
        public TipBodyHeartbeatReply(string hexString) : base(hexString) { }

        /// <summary>
        /// 用给定的byte数组初始化
        /// </summary>
        /// <param name="bytes"></param>
        public TipBodyHeartbeatReply(byte[] bytes) : base(bytes) { }
        #endregion

        #region 抽象方法实现
        protected override List<byte> ComposeUrOwn()
        {
            return null;
        }

        protected override byte[] GetTipHeadBytes()
        {
            return TipHead.Compose();
        }

        protected override void InitTipHead(byte[] bytes)
        {
            TipHead = new TipHeadCommon(bytes);
        }

        protected override void ResolveUrOwn(byte[] bytes)
        {
            //不需进行额外解析
            return;
        }

        protected override void UpdateAllBytesLen_Composing(uint abtLen)
        {
            TipHead.UpdateAllBytesLen(abtLen);
        }

        protected override void UpdateTipCode_Composing(TipCode code = TipCode.None)
        {
            TipHead.UpdateTipCode(TipCode.HeartbeatReply);
        }
        #endregion
    }
}
