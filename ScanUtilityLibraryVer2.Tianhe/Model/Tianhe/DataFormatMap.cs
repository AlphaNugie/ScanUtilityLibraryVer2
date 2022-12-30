using CommonLib.Clients;
using ScanUtilityLibrary.Core.Tianhe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Model.Tianhe
{
    /// <summary>
    /// 按位定义的数据格式的实体类（4字节长度）
    /// </summary>
    public class DataFormatMap
    {
        private readonly CustomBitConverter<uint> _bitConv = new CustomBitConverter<uint>();

        #region 属性
        /// <summary>
        /// 同步信息（SyncInfo结构）输出开关
        /// </summary>
        public bool SyncInfoOn { get; private set; }

        /// <summary>
        /// 时间戳（TimeStamp结构）输出开关
        /// </summary>
        public bool TimeStampOn { get; private set; }

        /// <summary>
        /// 时间戳类型，长度1比特
        /// 0：相对时间，1：日历时间
        /// </summary>
        public TimeStampType TimeStampType { get; private set; }
        //public int TimeStampType { get; private set; }

        /// <summary>
        /// 三维标志输出
        /// </summary>
        public bool Info3dOn { get; private set; }

        /// <summary>
        /// 三维坐标系类型，长度2比特
        /// 0：直角坐标，其他保留
        /// </summary>
        public CoordSystemType Info3dCoordType { get; private set; }
        //public byte Data3dCoordType { get; private set; }

        /// <summary>
        /// 角度分辨率类型，长度2比特
        /// 0:毫度，1:0.01毫度，2：保留，3:360度的点数（使用360除以这个值得到最终角分辨率）
        /// </summary>
        public AngleResolutionType AnglResltnType { get; private set; }
        //public byte AnglResltnType { get; private set; }

        /// <summary>
        /// 数据单位，长度3比特
        /// 0:2字节厘米，1：2字节毫米，6:4字节厘米，7：4字节毫米，2-5保留
        /// </summary>
        public MeasUnitType UnitType { get; private set; }
        //public byte UnitType { get; private set; }

        /// <summary>
        /// 回波脉冲选项，长度2比特
        /// 0：最远脉冲，1：最强脉冲，2：双脉冲，3：保留
        /// </summary>
        public EchoPulseType EchoPulseType { get; private set; }
        //public byte EchoPulseType { get; private set; }

        /// <summary>
        /// 反射率输出开关
        /// </summary>
        public bool AlbedoOn { get; private set; }

        /// <summary>
        /// 是否限制整圈输出扫描数据，false：整圈输出扫描数据，true：限长输出扫描数据（即时扫描输出）
        /// </summary>
        public bool OutputRestricted { get; private set; }
        #endregion

        #region 构造器
        /// <summary>
        /// 使用默认值0初始化
        /// </summary>
        public DataFormatMap() : this(0) { }

        /// <summary>
        /// 使用给定的值初始化
        /// </summary>
        /// <param name="value"></param>
        public DataFormatMap(uint value)
        {
            SetValue(value);
        }
        #endregion

        /// <summary>
        /// 用给定值设置各项属性的具体值
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(uint value)
        {
            _bitConv.SetValue(value);
            SyncInfoOn = _bitConv.Bits[0];
            TimeStampOn = _bitConv.Bits[1];
            //TimeStampType = (byte)_bitConv.GetValue(2, 1);
            TimeStampType = (TimeStampType)_bitConv.GetValue(2, 1);
            Info3dOn = _bitConv.Bits[3];
            //Info3dCoordType = (byte)_bitConv.GetValue(4, 2);
            //AnglResltnType = (byte)_bitConv.GetValue(14, 2);
            //UnitType = (byte)_bitConv.GetValue(16, 3);
            //EchoPulseType = (byte)_bitConv.GetValue(19, 2);
            Info3dCoordType = (CoordSystemType)_bitConv.GetValue(4, 2);
            AnglResltnType = (AngleResolutionType)_bitConv.GetValue(14, 2);
            UnitType = (MeasUnitType)_bitConv.GetValue(16, 3);
            EchoPulseType = (EchoPulseType)_bitConv.GetValue(19, 2);
            AlbedoOn = _bitConv.Bits[21];
            OutputRestricted = _bitConv.Bits[22];
        }
    }
}
