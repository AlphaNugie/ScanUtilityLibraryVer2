using CommonLib.Function;
using CommonLib.Helpers;
using ScanUtilityLibrary.Model.Tianhe;
using ScanUtilityLibrary.Core.Tianhe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Model.Tianhe
{
    /// <summary>
    /// TIP协议数据体基础类
    /// </summary>
    public abstract class TipBodyBase
    {
        /// <summary>
        /// 协议数据头
        /// </summary>
        public virtual TipHeadBase TipHead { get; set; }

        #region 构造器
        /// <summary>
        /// 默认构造器
        /// </summary>
        public TipBodyBase() : this(string.Empty) { }

        /// <summary>
        /// 用给定的16进制字符串初始化
        /// </summary>
        /// <param name="hexString"></param>
        public TipBodyBase(string hexString)
        {
            InitTipHead(null);
            Resolve(hexString);
        }

        /// <summary>
        /// 用给定的byte数组初始化
        /// </summary>
        /// <param name="bytes"></param>
        public TipBodyBase(byte[] bytes)
        {
            InitTipHead(null);
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
            InitTipHead(bytes);
            ResolveUrOwn(bytes);
        }

        /// <summary>
        /// 每个子类均需要初始化自己的TIP数据头（即使提供的字节数组初始化数据头的各属性值
        /// </summary>
        /// <param name="bytes"></param>
        protected abstract void InitTipHead(byte[] bytes);

        /// <summary>
        /// 每个子类根据当前字节数组进行一些自己独有的解析工作
        /// </summary>
        /// <param name="bytes"></param>
        protected abstract void ResolveUrOwn(byte[] bytes);

        /// <summary>
        /// 将各属性值组合为16进制字符串
        /// </summary>
        /// <returns></returns>
        public string ComposeHexString()
        {
            byte[] bytes = Compose();
            return bytes == null || bytes.Length == 0 ? string.Empty : HexHelper.ByteArray2HexString(bytes);
        }

        /// <summary>
        /// 将各属性值组合为byte数组
        /// </summary>
        /// <returns></returns>
        public byte[] Compose()
        {
            List<byte> bytes = new List<byte>(), ownBytes = ComposeUrOwn();
            ownBytes = ownBytes ?? new List<byte>();
            //计算完整报文的字节数，TIP数据头长度默认为40
            //uint abtLen = (uint)(40 + (ownBytes == null ? 0 : ownBytes.Count));
            uint abtLen = (uint)(40 + ownBytes.Count);
            UpdateAllBytesLen_Composing(abtLen);
            UpdateTipCode_Composing();
            byte[] headBytes = GetTipHeadBytes();
            bytes.AddRange(headBytes);
            bytes.AddRange(ownBytes);
            //bytes.AddRange(GetTipHeadBytes());
            //bytes.AddRange(ComposeUrOwn());
            return bytes.ToArray();
        }

        /// <summary>
        /// 更新TIP数据头中的全报文长度（AllBytesLen）；假如不做其它调用，则仅在组合字节流时使用
        /// </summary>
        /// <param name="abtLen"></param>
        protected abstract void UpdateAllBytesLen_Composing(uint abtLen);

        /// <summary>
        /// 更新TIP数据头中的TIP功能代码（TipCode），可根据需要进行自定义处理，也可考虑用给定的参数赋值，否则；假如不做其它调用，则仅在组合字节流时使用
        /// </summary>
        /// <param name="code">TIP功能码，可考虑用此给定的参数赋值</param>
        protected abstract void UpdateTipCode_Composing(TipCode code = TipCode.None);

        /// <summary>
        /// 获取TIP数据头的byte数组
        /// </summary>
        /// <returns></returns>
        protected abstract byte[] GetTipHeadBytes();

        /// <summary>
        /// 每个子类根据自己特有属性值进行一些自己独有的byte数组组合工作（没有需要则返回null）
        /// </summary>
        /// <returns></returns>
        protected abstract List<byte> ComposeUrOwn();
        #endregion
    }
}
