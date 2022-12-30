using CommonLib.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Model.Tianhe
{
    /// <summary>
    /// 数据格式的结构类，总长20个字节
    /// </summary>
    public class DataFormat
    {
        //private readonly DataFormatMap _formatMap = new DataFormatMap();

        #region 属性
        /// <summary>
        /// 按位定义的数据格式（4字节长度）
        /// </summary>
        public DataFormatMap DataFormatMap { get; set; } = new DataFormatMap();
        //public DataFormatMap DataFormatMap { get { return _formatMap; } }

        /// <summary>
        /// 起始角度（毫度）
        /// </summary>
        public int StartAngle { get; set; }

        /// <summary>
        /// 终止角度（毫度）
        /// </summary>
        public int StopAngle { get; set; }

        /// <summary>
        /// 角度分辨率，其含义由DataFormatMap中《角分辨率类型》字段决定
        /// </summary>
        public ushort AngleResolution { get; set; }

        /// <summary>
        /// 雷达型号
        /// </summary>
        public ushort RadarModel { get; set; }

        /// <summary>
        /// 雷达线数，0表示单层雷达
        /// </summary>
        public byte LineNum { get; set; }

        /// <summary>
        /// 总包数m：当前扫描帧数据的总包数
        /// </summary>
        public byte PackSum { get; set; }

        /// <summary>
        /// 扫描包序号：0至(m-1)
        /// </summary>
        public byte PackNo { get; set; }

        /// <summary>
        /// 相移信息
        /// bit0-3：当前相移次数，bit4-7：相移总次数
        /// </summary>
        public byte Phaseshift { get; set; }
        #endregion

        #region 构造器
        /// <summary>
        /// 默认构造器
        /// </summary>
        public DataFormat() : this(string.Empty) { }

        /// <summary>
        /// 用给定的16进制字符串初始化
        /// </summary>
        /// <param name="hexString"></param>
        public DataFormat(string hexString)
        {
            Resolve(hexString);
        }

        /// <summary>
        /// 用给定的byte数组初始化
        /// </summary>
        /// <param name="bytes"></param>
        public DataFormat(byte[] bytes)
        {
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
            if (bytes == null || bytes.Length < 20)
                return;
            uint fmtmap = BitConverter.ToUInt32(bytes, 0);
            //_formatMap.SetValue(fmtmap);
            DataFormatMap.SetValue(fmtmap);
            StartAngle = BitConverter.ToInt32(bytes, 4);
            StopAngle = BitConverter.ToInt32(bytes, 8);
            AngleResolution = BitConverter.ToUInt16(bytes, 12);
            RadarModel = BitConverter.ToUInt16(bytes, 14);
            LineNum = bytes[16];
            PackSum = bytes[17];
            PackNo = bytes[18];
            Phaseshift = bytes[19];
        }
        #endregion
    }
}
