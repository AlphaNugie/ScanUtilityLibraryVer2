using ScanUtilityLibrary.Core.Tianhe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Model.Tianhe
{
    /// <summary>
    /// TIP协议数据体：设备数据输出
    /// </summary>
    public class TipBodyDataOutput : TipBodyBase
    {
        #region 属性
        /// <summary>
        /// 回波脉冲选项
        /// </summary>
        public EchoPulseType EchoPulseType { get { return DataFormat.DataFormatMap.EchoPulseType; } }

        /// <summary>
        /// 反射率输出开关
        /// </summary>
        public bool AlbedoOn { get { return DataFormat.DataFormatMap.AlbedoOn; } }

        #region 协议成员
        /// <summary>
        /// 协议数据头
        /// </summary>
        public new TipHeadDataOutput TipHead { get; set; }

        /// <summary>
        /// 数据格式字节数
        /// </summary>
        public ushort DataFormatLen { get; set; }

        /// <summary>
        /// 数据格式结构的实体类对象
        /// </summary>
        public DataFormat DataFormat { get; set; } = new DataFormat();

        /// <summary>
        /// 同步信息字节数
        /// </summary>
        public ushort SyncInfoLen { get; set; }

        /// <summary>
        /// 设备同步信息结构的实体类对象
        /// </summary>
        public SyncInfo SyncInfo { get; set; } = new SyncInfo();

        /// <summary>
        /// 时间戳信息字节数
        /// </summary>
        public ushort TimeStampLen { get; set; }

        /// <summary>
        /// 设备时间戳信息结构的实体类对象
        /// </summary>
        public TimeStamp TimeStamp { get; set; } = new TimeStamp();

        /// <summary>
        /// 测量数据字节数
        /// </summary>
        public ushort MeasDataLen { get; set; }

        /// <summary>
        /// 测量数据的列表
        /// </summary>
        public List<MeasDataGroup> MeasDataGroups { get; set; } = new List<MeasDataGroup>();
        #endregion
        #endregion

        #region 构造器
        /// <summary>
        /// 默认构造器
        /// </summary>
        public TipBodyDataOutput() : base(string.Empty) { }

        /// <summary>
        /// 用给定的16进制字符串初始化
        /// </summary>
        /// <param name="hexString"></param>
        public TipBodyDataOutput(string hexString) : base(hexString) { }

        /// <summary>
        /// 用给定的byte数组初始化
        /// </summary>
        /// <param name="bytes"></param>
        public TipBodyDataOutput(byte[] bytes) : base(bytes) { }
        #endregion

        #region 抽象方法实现
        /// <inheritdoc/>
        protected override void InitTipHead(byte[] bytes)
        {
            TipHead = new TipHeadDataOutput(bytes/*, StreamDirection.FromDevice*/);
        }

        /// <inheritdoc/>
        protected override void ResolveUrOwn(byte[] bytes)
        {
            //下一个索引的值
            int nextIndex = 40;
            //假如接下来的byte数量不足
            if (bytes == null || bytes.Length < nextIndex + 2)
                return;
            DataFormatLen = BitConverter.ToUInt16(bytes, nextIndex);
            //nextIndex = nextIndex + 2 + DataFormatLen;
            nextIndex += 2;
            //假如接下来的byte数量不足
            if (bytes.Length < nextIndex + DataFormatLen)
                return;
            DataFormat.Resolve(bytes.Skip(nextIndex).Take(DataFormatLen).ToArray());
            nextIndex += DataFormatLen;
            //处理同步信息
            if (DataFormat.DataFormatMap.SyncInfoOn)
            {
                if (bytes.Length < nextIndex + 2)
                    return;
                SyncInfoLen = BitConverter.ToUInt16(bytes, nextIndex);
                nextIndex += 2;
                if (bytes.Length < nextIndex + SyncInfoLen)
                    return;
                SyncInfo.Resolve(bytes.Skip(nextIndex).Take(SyncInfoLen).ToArray());
                nextIndex += SyncInfoLen;
            }
            else
                SyncInfoLen = 0;
            //处理时间戳信息
            if (DataFormat.DataFormatMap.TimeStampOn)
            {
                if (bytes.Length < nextIndex + 2)
                    return;
                TimeStampLen = BitConverter.ToUInt16(bytes, nextIndex);
                nextIndex += 2;
                if (bytes.Length < nextIndex + TimeStampLen)
                    return;
                TimeStamp.Resolve(bytes.Skip(nextIndex).Take(TimeStampLen).ToArray());
                nextIndex += TimeStampLen;
            }
            else
                TimeStampLen = 0;
            //TODO 为3D信息（Info3D）预留的位置
            //处理测量数据
            if (bytes.Length < nextIndex + 2)
                return;
            MeasDataLen = BitConverter.ToUInt16(bytes, nextIndex);
            nextIndex += 2;
            ////是否是2个脉冲
            //bool bothPulses = /*DataFormat.DataFormatMap.*/EchoPulseType == EchoPulseType.Both/* && (DataFormat.DataFormatMap.UnitType == MeasUnitType.Centi_4bytes || DataFormat.DataFormatMap.UnitType == MeasUnitType.Milli_4bytes)*/;
            //反射率单位长度
            int albedoLen = /*DataFormat.DataFormatMap.*/AlbedoOn ? 1 : 0;
            #region 测量数据储存（旧）
            //List<MeasDataGroup> groups = new List<MeasDataGroup>();
            //for (int i = 0; i < TipHead.NumOfPoints; i++)
            //{
            //    if (bytes.Length < nextIndex + 2 + albedoLen)
            //        return;
            //    //处理第一个脉冲的测量值
            //    MeasDataGroup group = new MeasDataGroup(/*DataFormat.DataFormatMap.AnglResltnType, DataFormat.DataFormatMap.UnitType*/) { Pulse1 = BitConverter.ToUInt16(bytes, nextIndex) };
            //    groups.Add(group);
            //    nextIndex += 2;
            //    //if (DataFormat.DataFormatMap.AlbedoOn)
            //    if (albedoLen == 1)
            //    {
            //        group.Albedo1 = bytes[nextIndex];
            //        nextIndex += 1;
            //    }
            //    //假如输出双脉冲，再处理第二个脉冲的测量值
            //    if (!bothPulses || bytes.Length < nextIndex + 2 + albedoLen)
            //        continue;
            //    group.Pulse2 = BitConverter.ToUInt16(bytes, nextIndex);
            //    nextIndex += 2;
            //    //if (DataFormat.DataFormatMap.AlbedoOn)
            //    if (albedoLen == 1)
            //    {
            //        group.Albedo2 = bytes[nextIndex];
            //        nextIndex += 1;
            //    }
            //}
            //MeasDataGroups.Clear();
            //MeasDataGroups.AddRange(groups);
            #endregion
            #region 测量数据储存
            for (int i = 0; i < TipHead.NumOfPoints; i++)
            {
                if (bytes.Length < nextIndex + 2 + albedoLen)
                    return;
                //假如存储长度不够则初始化并添加
                if (MeasDataGroups.Count <= i)
                    MeasDataGroups.Add(new MeasDataGroup());
                //处理第一个脉冲的测量值
                MeasDataGroup group = MeasDataGroups[i];
                ushort pulse = BitConverter.ToUInt16(bytes, nextIndex);
                byte albedo = 0;
                //group.PulseFarthest = BitConverter.ToUInt16(bytes, nextIndex);
                nextIndex += 2;
                if (albedoLen == 1)
                {
                    albedo = bytes[nextIndex];
                    //group.AlbedoFarthest = bytes[nextIndex];
                    nextIndex += 1;
                }
                if (EchoPulseType == EchoPulseType.Strongest)
                {
                    group.PulseStrongest = pulse;
                    group.AlbedoStrongest = albedo;
                }
                else
                {
                    group.PulseFarthest = pulse;
                    group.AlbedoFarthest = albedo;
                }
                //假如输出双脉冲，再处理第二个脉冲的测量值
                //if (!bothPulses || bytes.Length < nextIndex + 2 + albedoLen)
                if (EchoPulseType != EchoPulseType.Both || bytes.Length < nextIndex + 2 + albedoLen)
                    continue;
                group.PulseStrongest = BitConverter.ToUInt16(bytes, nextIndex);
                nextIndex += 2;
                if (albedoLen == 1)
                {
                    group.AlbedoStrongest = bytes[nextIndex];
                    nextIndex += 1;
                }
            }
            #endregion
        }

        /// <inheritdoc/>
        protected override void UpdateAllBytesLen_Composing(uint abtLen)
        {
            TipHead.UpdateAllBytesLen(abtLen);
            //TipHead.AllBytesLen = abtLen;
        }

        /// <inheritdoc/>
        protected override void UpdateTipCode_Composing(TipCode code = TipCode.None)
        {
            TipHead.UpdateTipCode(TipCode.DataOutput);
        }

        /// <inheritdoc/>
        protected override byte[] GetTipHeadBytes()
        {
            return TipHead.Compose();
        }

        /// <inheritdoc/>
        protected override List<byte> ComposeUrOwn()
        {
            //TODO 补充将各属性组合为byte数组的代码部分
            throw new NotImplementedException();
        }
        #endregion
    }
}
