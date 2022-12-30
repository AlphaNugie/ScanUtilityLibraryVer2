using ScanUtilityLibrary.Core.Tianhe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Model.Tianhe
{
    /// <summary>
    /// 包含每点测量数据的结构体
    /// </summary>
    public class MeasDataGroup
    {
        /// <summary>
        /// 角度分辨率类型
        /// 0:毫度，1:0.01毫度，2：保留，3:360度的点数（使用360除以这个值得到最终角分辨率）
        /// </summary>
        public AngleResolutionType AnglResltnType { get; private set; }

        /// <summary>
        /// 数据单位
        /// 0:2字节厘米，1：2字节毫米，6:4字节厘米，7：4字节毫米，2-5保留
        /// </summary>
        public MeasUnitType UnitType { get; private set; }

        ///// <summary>
        ///// 脉冲1的测量距离
        ///// </summary>
        //public ushort Pulse1 { get;set; }

        ///// <summary>
        ///// 脉冲1的反射率
        ///// </summary>
        //public byte Albedo1 { get; set; }

        /// <summary>
        /// 最远脉冲的测量距离
        /// </summary>
        public ushort PulseFarthest { get;set; }

        /// <summary>
        /// 脉冲1的反射率
        /// </summary>
        public byte AlbedoFarthest { get; set; }

        ///// <summary>
        ///// 脉冲2的测量距离
        ///// </summary>
        //public ushort Pulse2 { get; set; }

        ///// <summary>
        ///// 脉冲2的反射率
        ///// </summary>
        //public byte Albedo2 { get; set; }

        /// <summary>
        /// 最强脉冲的测量距离
        /// </summary>
        public ushort PulseStrongest { get;set; }

        /// <summary>
        /// 脉冲2的反射率
        /// </summary>
        public byte AlbedoStrongest { get; set; }

        /// <summary>
        /// 用给定的各属性值初始化
        /// </summary>
        /// <param name="resltnType">角度分辨率类型</param>
        /// <param name="unitType">测量的距离单位</param>
        /// <param name="f_pulse">最远脉冲的距离</param>
        /// <param name="f_albedo">最远脉冲的反射率</param>
        /// <param name="s_pulse">最强脉冲的距离</param>
        /// <param name="s_albedo">最强脉冲的反射率</param>
        public MeasDataGroup(AngleResolutionType resltnType = AngleResolutionType.MilliDeg, MeasUnitType unitType = MeasUnitType.Centi_2bytes, ushort f_pulse = 0, byte f_albedo = 0, ushort s_pulse = 0, byte s_albedo = 0)
        {
            AnglResltnType = resltnType;
            UnitType = unitType;
            PulseFarthest = f_pulse;
            AlbedoFarthest = f_albedo;
            PulseStrongest = s_pulse;
            AlbedoStrongest = s_albedo;
        }
    }
}
