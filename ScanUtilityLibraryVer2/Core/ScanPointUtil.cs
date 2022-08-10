using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ScanUtilityLibrary.Model;

namespace ScanUtilityLibrary.Core
{
    /// <summary>
    /// 扫描点对象相关操作类
    /// </summary>
    public static class ScanPointUtil
    {
        /// <summary>
        /// 将扫描点对象List
        /// </summary>
        /// <param name="enumScans">扫描点数据集合</param>
        /// <param name="z">特定的Z轴坐标</param>
        /// <returns></returns>
        public static List<string> Convert_CoorStrings(IEnumerable<ScanPoint> enumScans, double z)
        {
            //List<string> list = new List<string>();
            if (enumScans == null || enumScans.Count() == 0)
                return new List<string>();

            return enumScans.Where(p => p != null).Select(point => string.Format("{0} {1} {2}", Math.Floor(point.X).ToString(), Math.Floor(point.Y).ToString(), Math.Ceiling(z).ToString())).ToList();
            //foreach (ScanPoint point in listScans)
            //    if (point != null)
            //        list.Add(string.Format("{0} {1} {2}", Math.Floor(point.X).ToString(), Math.Floor(point.Y).ToString(), Math.Ceiling(z).ToString()));

            //return list;
        }

        /// <summary>
        /// 将扫描点对象List
        /// </summary>
        /// <param name="enumScans">扫描点数据集合</param>
        /// <returns></returns>
        public static List<string> Convert_CoorStrings(IEnumerable<ScanPoint> enumScans)
        {
            List<string> list = new List<string>();
            if (enumScans == null || enumScans.Count() == 0)
                return list;

            return enumScans.Where(p => p != null).Select(point => string.Format("{0} {1} {2}", Math.Floor(point.X).ToString(), Math.Floor(point.Y).ToString(), Math.Ceiling(point.Z).ToString())).ToList();
            //foreach (ScanPoint point in enumScans)
            //    if (point != null)
            //        list.Add(string.Format("{0} {1} {2}", Math.Floor(point.X).ToString(), Math.Floor(point.Y).ToString(), Math.Ceiling(point.Z).ToString()));

            //return list;
        }

        /// <summary>
        /// 将扫描点对象List的XY轴坐标转换为字符串List
        /// </summary>
        /// <param name="enumScans">扫描点数据集合</param>
        /// <returns></returns>
        public static List<string> Convert_CoorStrings_Xy(IEnumerable<ScanPoint> enumScans)
        {
            List<string> list = new List<string>();
            if (enumScans == null || enumScans.Count() == 0)
                return list;

            return enumScans.Where(p => p != null).Select(point => string.Format("{0} {1}", Math.Floor(point.X).ToString(), Math.Floor(point.Y).ToString())).ToList();
        }

        /// <summary>
        /// 将扫描点对象List的XY轴坐标转换为字符串，格式为“x1,y1;x2,y2;...;xn,yn”
        /// </summary>
        /// <param name="enumScans">扫描点数据集合</param>
        /// <returns></returns>
        public static string Convert_CoorString_Xy_Alter(IEnumerable<ScanPoint> enumScans)
        {
            string coordinates = string.Empty;
            if (enumScans != null && enumScans.Count() > 0)
                coordinates = string.Join(";", enumScans.Where(p => p != null).Select(point => string.Format("{0},{1}", Math.Floor(point.X).ToString(), Math.Floor(point.Y).ToString())));
            return coordinates;
        }
    }
}
