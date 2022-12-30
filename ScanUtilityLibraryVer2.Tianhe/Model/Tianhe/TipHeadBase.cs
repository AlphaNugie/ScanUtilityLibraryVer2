using CommonLib.Function;
using CommonLib.Helpers;
using ScanUtilityLibrary.Core.Tianhe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Model.Tianhe
{
    /// <summary>
    /// TCP协议数据头基础类
    /// </summary>
    public abstract class TipHeadBase
    {
        #region 属性
        /// <summary>
        /// 校验通过
        /// </summary>
        public bool ChecksumPassed { get; private set; }

        /// <summary>
        /// 报文流通方向
        /// </summary>
        public StreamDirection Direction { get; private set; }

        #region 报文字段
        /// <summary>
        /// 开头固定字节0xAD
        /// </summary>
        public byte HeadAd { get; private set; }

        /// <summary>
        /// 开头固定字节0x49
        /// </summary>
        public byte Head49 { get; private set; }

        /// <summary>
        /// TIP代码（类型），长度2字节unsigned short
        /// </summary>
        public TipCode TipCode { get; private set; }
        //public ushort TipCode { get; private set; }

        /// <summary>
        /// 包类型，downstream或upstream，默认值0，长度1字节byte
        /// downstream(0x0:all,0x1:up,0x2:down);
        /// upstream(0x80:all,0x81:up,0x82:down);
        /// </summary>
        public PackType PackType { get; private set; }
        //public byte PackType { get; private set; }

        /// <summary>
        /// 源类型，默认值0（从设备接收时为0x5B）
        /// </summary>
        public byte SourceObjType { get; private set; }

        /// <summary>
        /// 源地址，默认值0（从设备接收时为0x0002）
        /// </summary>
        public ushort SourceId { get; private set; }

        /// <summary>
        /// 网络重发次数和优先级（无另行说明为默认值0x11，否则以说明值为准）
        /// </summary>
        public byte RnPri { get; private set; }

        /// <summary>
        /// 目的类型，默认值0
        /// </summary>
        public byte DestObjType { get; private set; }

        /// <summary>
        /// 目的地址，默认值0
        /// </summary>
        public ushort DestId { get; private set; }

        /// <summary>
        /// 版本号，默认值0x5
        /// </summary>
        public byte Version { get; private set; }

        /// <summary>
        /// 运行模式，默认值0
        /// </summary>
        public byte SysRunMode { get; private set; }

        /// <summary>
        /// 校验和，TIP代码与报文长度的校验和
        /// </summary>
        public ushort Checksum { get; private set; }

        /// <summary>
        /// 完整报文长度（TIP头+TIP后数据包总字节数），无另行说明为默认值40，否则以说明值为准
        /// </summary>
        public uint AllBytesLen { get; private set; }

        /// <summary>
        /// 应用编号，默认值0（从设备接收时为0x00000001）
        /// </summary>
        public uint Tid { get; private set; }

        /// <summary>
        /// TIP创建时间，长度4字节unsigned int
        /// </summary>
        public DateTime CreatedTime { get; private set; }
        //public uint CreatedTime { get; set; }

        /// <summary>
        /// 网络传输编号，发送时默认填0（设备接收时为0x0000000F）
        /// 0 means posted by PostTIP.
        /// non-zero means by physical channel.
        /// </summary>
        public int ConnId { get; private set; }
        #endregion
        #endregion

        #region 构造器
        /// <summary>
        /// 默认构造器
        /// </summary>
        /// <param name="dir"></param>
        public TipHeadBase(StreamDirection dir) : this(string.Empty, dir) { }

        /// <summary>
        /// 用给定的16进制字符串初始化
        /// </summary>
        /// <param name="hexString">报文的16进制字符串</param>
        /// <param name="dir">报文流通方向</param>
        public TipHeadBase(string hexString, StreamDirection dir)
        {
            Direction = dir;
            Resolve(hexString);
        }

        /// <summary>
        /// 用给定的byte数组初始化
        /// </summary>
        /// <param name="bytes">报文的字节流</param>
        /// <param name="dir">报文流通方向</param>
        public TipHeadBase(byte[] bytes, StreamDirection dir)
        {
            Direction = dir;
            Resolve(bytes);
        }
        #endregion

        #region static方法
        /// <summary>
        /// 根据16进制字符串来解析TIP代码
        /// </summary>
        /// <param name="hexString"></param>
        public static TipCode ResolveTipCode(string hexString)
        {
            return string.IsNullOrWhiteSpace(hexString) ? 0 : ResolveTipCode(HexHelper.HexString2Bytes(hexString));
        }

        /// <summary>
        /// 根据byte数组来解析TIP代码
        /// </summary>
        /// <param name="bytes"></param>
        public static TipCode ResolveTipCode(byte[] bytes)
        {
            TipCode code = TipCode.None;
            if (bytes == null || bytes.Length < 4)
                goto END;
            byte headAd = bytes[0], head49 = bytes[1];
            if (headAd != 0xAD || head49 != 0x49)
                goto END;
            code = (TipCode)BitConverter.ToUInt16(bytes, 2);
            END:
            return code;
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
            if (bytes == null || bytes.Length < 40)
                return;
            HeadAd = bytes[0];
            Head49 = bytes[1];
            if (HeadAd != 0xAD || Head49 != 0x49)
                return;
            TipCode = (TipCode)BitConverter.ToUInt16(bytes, 2);
            PackType = (PackType)bytes[4];
            SourceObjType = bytes[5];
            SourceId = BitConverter.ToUInt16(bytes, 6);
            RnPri = bytes[8];
            DestObjType = bytes[9];
            DestId = BitConverter.ToUInt16(bytes, 10);
            Version = bytes[12];
            SysRunMode = bytes[13];
            Checksum = BitConverter.ToUInt16(bytes, 14);
            AllBytesLen = BitConverter.ToUInt32(bytes, 16);
            ChecksumPassed = IsChecksumPassed(Checksum, (ushort)TipCode, (ushort)AllBytesLen);
            Tid = BitConverter.ToUInt32(bytes, 20);
            CreatedTime = DateTimeHelper.GetUtcTimeByTimeStampSec(BitConverter.ToUInt32(bytes, 24).ToString());
            //ConnId = BitConverter.ToUInt16(bytes, 28);
            ConnId = BitConverter.ToInt32(bytes, 28);
            ResolveUrOwn(bytes);
        }

        /// <summary>
        /// 每个子类根据当前字节数组进行一些自己独有的解析工作（通常是解析预留的8字节Resparm信息）
        /// </summary>
        /// <param name="bytes"></param>
        protected abstract void ResolveUrOwn(byte[] bytes);

        /// <summary>
        /// 更新自定义时所需的TIP功能代码
        /// </summary>
        /// <param name="code"></param>
        public void UpdateTipCode(TipCode code)
        {
            TipCode = code;
        }

        /// <summary>
        /// 更新自定义时所需的全报文长度
        /// </summary>
        /// <param name="abtLen"></param>
        public void UpdateAllBytesLen(uint abtLen)
        {
            AllBytesLen = abtLen;
        }

        /// <summary>
        /// 将各属性值组合为byte数组
        /// </summary>
        /// <returns></returns>
        internal byte[] Compose()
        {
            #region 设备默认值
            HeadAd = 0xAD;
            Head49 = 0x49;
            //需在外部定义TipCode
            if (Direction == StreamDirection.ToDevice)
            {
                PackType = PackType.Downstream_all;
                SourceObjType = 0;
                SourceId = 0;
                RnPri = 0x11;
                DestObjType = 0;
                DestId = 0;
                Version = 5;
                SysRunMode = 0;
                Tid = 0;
                ConnId = 0;
            }
            Checksum = GetChecksum((ushort)TipCode, (ushort)AllBytesLen);
            ChecksumPassed = true;
            //需在外部定义AllBytesLen
            CreatedTime = DateTime.UtcNow;
            #endregion

            #region 组合字节流
            List<byte> bytes = new List<byte>
            {
                HeadAd,
                Head49
            };
            bytes.AddRange(BitConverter.GetBytes((short)TipCode));
            bytes.Add((byte)PackType);
            bytes.Add(SourceObjType);
            bytes.AddRange(BitConverter.GetBytes(SourceId));
            bytes.Add(RnPri);
            bytes.Add(DestObjType);
            bytes.AddRange(BitConverter.GetBytes(DestId));
            bytes.Add(Version);
            bytes.Add(SysRunMode);
            bytes.AddRange(BitConverter.GetBytes(Checksum));
            bytes.AddRange(BitConverter.GetBytes(AllBytesLen));
            bytes.AddRange(BitConverter.GetBytes(Tid));
            bytes.AddRange(BitConverter.GetBytes(uint.Parse(DateTimeHelper.GetTimeStampBySeconds(CreatedTime))));
            bytes.AddRange(BitConverter.GetBytes(ConnId));
            var ownBytes = ComposeUrOwn();
            if (ownBytes == null || ownBytes.Count != 8)
                throw new FormatException("TIP数据头中resparm字段组合出的byte数组不满足长度为8的条件");
            //bytes.AddRange(ComposeUrOwn());
            bytes.AddRange(ownBytes);
            #endregion
            //END:
            return bytes.ToArray();
        }

        /// <summary>
        /// 每个子类根据自己特有属性值进行一些自己独有的byte数组组合工作，长度应保证为8（通常是组合与预留的8字节Resparm信息相关的属性）
        /// </summary>
        /// <returns></returns>
        protected abstract List<byte> ComposeUrOwn();

        /// <summary>
        /// 根据TIP头的代码以及TIP完整报文字节数计算校验和
        /// </summary>
        /// <param name="tipCode">TIP代码</param>
        /// <param name="abtLen">TIP完整报文字节数</param>
        /// <returns></returns>
        public ushort GetChecksum(ushort tipCode, ushort abtLen)
        {
            ushort res = (ushort)(tipCode + abtLen);
            res = (ushort)~res;
            return res;
        }

        /// <summary>
        /// 根据TIP头的代码以及TIP完整报文字节数计算校验和，并验证是否与报文中的校验和相符
        /// </summary>
        /// <param name="givenChecksum">与之验证的给定校验和</param>
        /// <param name="tipCode">TIP代码</param>
        /// <param name="abtLen">TIP完整报文字节数</param>
        /// <returns></returns>
        public bool IsChecksumPassed(ushort givenChecksum, ushort tipCode, ushort abtLen)
        {
            return givenChecksum == GetChecksum(tipCode, abtLen);
        }
        #endregion
    }
}
