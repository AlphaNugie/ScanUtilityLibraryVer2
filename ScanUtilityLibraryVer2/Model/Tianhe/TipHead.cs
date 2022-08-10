using CommonLib.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Model.Tianhe
{
    /// <summary>
    /// TCP协议数据头
    /// </summary>
    public class TipHead
    {
        /// <summary>
        /// 开头固定字节0xAD
        /// </summary>
        public byte HeadAd { get; set; }

        /// <summary>
        /// 开头固定字节0x49
        /// </summary>
        public byte Head49 { get; set; }

        /// <summary>
        /// TIP代码（类型）
        /// </summary>
        public ushort Code { get; set; }

        /// <summary>
        /// 包类型，downstream或upstream，默认值0
        /// downstream(0x0:all,0x1:up,0x2:down);
        /// upstream(0x80:all,0x81:up,0x82:down);
        /// </summary>
        public byte PackType { get; set; }

        /// <summary>
        /// 源类型，默认值0
        /// </summary>
        public byte SourceType { get; set; }

        /// <summary>
        /// 源地址，默认值0
        /// </summary>
        public ushort SourceId { get; set; }

        /// <summary>
        /// 网络重发次数和优先级（无另行说明为默认值0x11，否则以说明值为准）
        /// </summary>
        public byte RnPri { get; set; }

        /// <summary>
        /// 目的类型，默认值0
        /// </summary>
        public byte DestType { get; set; }

        /// <summary>
        /// 目的地址，默认值0
        /// </summary>
        public ushort DestId { get; set; }

        /// <summary>
        /// 版本号，默认值0x5
        /// </summary>
        public byte Version { get; set; }

        /// <summary>
        /// 运行模式，默认值0
        /// </summary>
        public byte RunMode { get; set; }

        /// <summary>
        /// 校验和，TIP代码与报文长度的校验和
        /// </summary>
        public ushort Checksum { get; set; }

        /// <summary>
        /// 完整报文长度（TIP头+TIP后数据包总字节数），无另行说明为默认值40，否则以说明值为准
        /// </summary>
        public uint AbtLen { get; set; }

        /// <summary>
        /// 应用编号，默认值0
        /// </summary>
        public uint TID { get; set; }

        /// <summary>
        /// TIP创建时间
        /// </summary>
        public uint CreatedTime { get; set; }

        /// <summary>
        /// 网络传输编号，发送时默认填0
        /// 0 means posted by PostTIP.
        /// non-zero means by physical channel.
        /// </summary>
        public int ConnId { get; set; }

        /// <summary>
        /// 保留字，长度8字节
        /// </summary>
        public object ReservedParams { get; set; }

        /// <summary>
        /// 根据16进制字符串解析为TIP
        /// </summary>
        /// <param name="hexString"></param>
        public void Resolve(string hexString)
        {
            byte[] bytes = HexHelper.HexString2Bytes(hexString);
            if (bytes == null || bytes.Length < 40)
                return;
            HeadAd = bytes[0];
            Head49 = bytes[1];
            //
            if (HeadAd != 0xAD || Head49 != 0x49)
                return;
        }

        ///// <summary>
        ///// 重新构成为16进制字符串
        ///// </summary>
        ///// <returns></returns>
        //public string Compose()
        //{

        //}
    }
}
