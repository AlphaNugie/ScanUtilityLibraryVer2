using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScanUtilityLibrary.Model
{
    /// <summary>
    /// 扫描点对象
    /// </summary>
    public class ScanPoint
    {
        #region 私有成员
        private ScanDeviceType type; //扫描仪类型
        private uint h_distance; //距离
        private double h_angle; //角度
        private double h_x; //X轴坐标
        private double h_y; //Y轴坐标
        private double h_z; //Y轴坐标
        #endregion

        #region 属性
        /// <summary>
        /// 扫描仪类型
        /// </summary>
        public ScanDeviceType Type
        {
            get { return type; }
            set
            {
                type = value;
                RefreshCoordinates();
            }
        }

        /// <summary>
        /// 与某角度被扫描点的距离（单位：mm）
        /// </summary>
        public uint Distance
        {
            get { return h_distance; }
            set
            {
                h_distance = value;
                RefreshCoordinates();
            }
        }

        /// <summary>
        /// 反馈能量
        /// </summary>
        public uint EchoValue { get; set; }

        /// <summary>
        /// 角度
        /// </summary>
        public double Angle
        {
            get { return h_angle; }
            set
            {
                h_angle = value;
                RefreshCoordinates();
            }
        }

        /// <summary>
        /// 被扫描点数据提取时间
        /// </summary>
        public DateTime ScannedTime { get; set; }

        /// <summary>
        /// X轴坐标（单位：mm）
        /// </summary>
        public double X
        {
            get { return h_x; }
            set
            {
                h_x = Math.Round(value, 3);
                RefreshDistanceAngle();
            }
        }

        /// <summary>
        /// Y轴坐标（单位：mm）
        /// </summary>
        public double Y
        {
            get { return h_y; }
            set
            {
                h_y = Math.Round(value, 3);
                RefreshDistanceAngle();
            }
        }

        /// <summary>
        /// Z轴坐标（单位：mm）
        /// </summary>
        public double Z
        {
            get { return h_z; }
            set { h_z = Math.Round(value, 3); }
        }
        #endregion

        /// <summary>
        /// 扫描点对象构造器
        /// </summary>
        public ScanPoint() : this(0, 0, 0, 0) { }

        /// <summary>
        /// 扫描点对象构造器
        /// </summary>
        /// <param name="type">扫描仪类型</param>
        /// <param name="dist">距离（毫米）</param>
        /// <param name="echo"></param>
        /// <param name="angle">角度</param>
        /// <param name="z">Z轴方向坐标</param>
        public ScanPoint(ScanDeviceType type, uint dist, uint echo = 0, double angle = 0, double z = 0)
        {
            this.type = type;
            h_distance = dist;
            EchoValue = echo;
            h_angle = angle;
            Z = z;
            RefreshCoordinates();
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="type">扫描仪类型</param>
        /// <param name="x">X坐标（毫米）</param>
        /// <param name="y">Y坐标（毫米）</param>
        /// <param name="z">Z坐标（毫米）</param>
        public ScanPoint(ScanDeviceType type, double x, double y, double z)
        {
            this.type = type;
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// 转换为字符串，XY坐标向下取整，Z坐标向上取整
        /// </summary>
        /// <param name="scale">保存时对各坐标进行缩放的比例</param>
        /// <param name="z">保存时给定的Z坐标（毫米），假如不为null则覆盖原本对象的Z值</param>
        /// <returns></returns>
        public string ToString(double scale = 1, double? z = null)
        {
            double z_value = z == null ? Z : z.Value;
            //return string.Format("{0} {1} {2}", Math.Floor(X * scale), Math.Floor(Y * scale), Math.Ceiling(Z * scale));
            return string.Format("{0} {1} {2}", Math.Floor(X * scale), Math.Floor(Y * scale), Math.Ceiling(z_value * scale));
        }

        /// <summary>
        /// 根据距离与角度刷新XY坐标
        /// </summary>
        public void RefreshCoordinates()
        {
            if (type == ScanDeviceType.R2000)
            {
                //先将角度转换为弧度
                h_x = h_distance * Math.Sin(h_angle * Math.PI / 180); //X轴坐标
                h_y = h_distance * Math.Cos(h_angle * Math.PI / 180); //Y轴坐标
            }
            else if (type == ScanDeviceType.LMS)
            {
                //先将角度转换为弧度
                h_x = h_distance * Math.Cos(h_angle * Math.PI / 180); //X轴坐标
                h_y = h_distance * Math.Sin(h_angle * Math.PI / 180); //Y轴坐标
            }
        }

        /// <summary>
        /// 根据XY坐标刷新距离与角度
        /// </summary>
        public void RefreshDistanceAngle()
        {
            double distance = Math.Sqrt(Math.Pow(h_x, 2) + Math.Pow(h_y, 2));
            h_distance = (uint)distance;
            if (type == ScanDeviceType.R2000)
            {
                h_angle = Math.Acos(h_y / distance) * 180 / Math.PI;
                //假如X坐标小于0，找到关于180°角对称的那个角
                if (h_x < 0)
                    h_angle = 360 - h_angle;
            }
            else if (type == ScanDeviceType.LMS)
            {
                h_angle = Math.Asin(h_y / distance) * 180 / Math.PI;
                //假如X坐标小于0，找到关于180°角对称的那个角
                if (h_x < 0)
                    h_angle = 180 - h_angle;
            }
        }

        /// <summary>
        /// 对象拷贝，拷贝距离与角度
        /// </summary>
        /// <returns></returns>
        public ScanPoint Copy()
        {
            return Copy(true);
        }

        /// <summary>
        /// 对象拷贝，决定拷贝距离角度还是XYZ坐标
        /// </summary>
        /// <param name="use_distance"></param>
        /// <returns></returns>
        public ScanPoint Copy(bool use_distance)
        {
            return use_distance ? Copy(Z) : Copy(h_x, h_y, Z);
        }

        /// <summary>
        /// 对象拷贝，Z轴坐标为给定值，复制距离与角度
        /// </summary>
        /// <param name="z">给定Z轴坐标值</param>
        /// <returns></returns>
        public ScanPoint Copy(double z)
        {
            return new ScanPoint(type, h_distance, EchoValue, h_angle, z);
        }

        /// <summary>
        /// 对象拷贝，x y z坐标为给定值
        /// </summary>
        /// <param name="x">X轴坐标</param>
        /// <param name="y">Y轴坐标</param>
        /// <param name="z">Z轴坐标</param>
        /// <returns></returns>
        public ScanPoint Copy(double x, double y, double z)
        {
            return new ScanPoint(type, x, y, z);
        }
    }

    /// <summary>
    /// 扫描仪类型
    /// </summary>
    public enum ScanDeviceType
    {
        /// <summary>
        /// 倍加福R2000系列扫描仪
        /// </summary>
        R2000 = 1,

        /// <summary>
        /// 西克(Sick)LMS系列扫描仪
        /// </summary>
        LMS = 2,

        ///// <summary>
        ///// 保定天河扫描仪
        ///// </summary>
        //TIANHE = 3
    }
}
