using ScanUtilityLibrary.Model.Tianhe;
using ScanUtilityLibrary.Core.Tianhe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Model.Tianhe
{
    /// <summary>
    /// 设备数据输出TIP数据头
    /// </summary>
    public class TipHeadDataOutput : TipHeadBase
    {
        #region 属性
        /// <summary>
        /// 数据单位，长度1字节byte
        /// 0:2字节厘米，1：2字节毫米，6:4字节厘米，7：4字节毫米，2-5保留
        /// </summary>
        public MeasUnitType UnitType { get; private set; }
        //public byte UnitType { get; private set; }

        /// <summary>
        /// 回波脉冲选项，长度1字节byte
        /// 0：最远脉冲，1：最强脉冲，2：双脉冲，3：保留
        /// </summary>
        public EchoPulseType EchoPulseType { get; private set; }
        //public byte FarthestPulse { get; private set; }

        /// <summary>
        /// 雷达线数，0表示单层雷达
        /// </summary>
        public byte LineNum { get; private set; }

        /// <summary>
        /// 到雷达距离输出部分的偏移量，基本固定为0x0020，代表从“数据点个数”到“雷达扫描数据长度” 偏移的字节个数；可利用此偏移量直接找到数据部分
        /// </summary>
        public ushort OffsetToData { get; private set; }

        /// <summary>
        /// 数据点个数，假如为540，就是共540个点的数据（270°，0.5°角分辨率，每度里两个点）
        /// </summary>
        public ushort NumOfPoints { get; private set; }
        #endregion

        #region 构造器
        /// <summary>
        /// 用给定的16进制字符串初始化
        /// </summary>
        /// <param name="hexString">报文的16进制字符串</param>
        public TipHeadDataOutput(string hexString) : base(hexString, StreamDirection.FromDevice) { }

        /// <summary>
        /// 用给定的byte数组初始化
        /// </summary>
        /// <param name="bytes">报文的字节流</param>
        public TipHeadDataOutput(byte[] bytes) : base(bytes, StreamDirection.FromDevice) { }
        #endregion

        /// <inheritdoc/>
        protected override void ResolveUrOwn(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 40)
                return;

            //UnitType = bytes[32];
            //FarthestPulse = bytes[33];
            UnitType = (MeasUnitType)bytes[32];
            EchoPulseType = (EchoPulseType)bytes[33];
            LineNum = bytes[34];
            //索引为35处的字节为保留字
            OffsetToData = BitConverter.ToUInt16(bytes, 36);
            NumOfPoints = BitConverter.ToUInt16(bytes, 38);
        }

        /// <inheritdoc/>
        protected override List<byte> ComposeUrOwn()
        {
            List<byte> bytes = new List<byte>()
            {
                (byte)UnitType,
                (byte)EchoPulseType,
                LineNum,
                0,
            };
            bytes.AddRange(BitConverter.GetBytes(OffsetToData));
            bytes.AddRange(BitConverter.GetBytes(NumOfPoints));
            return bytes;
        }
    }
}
