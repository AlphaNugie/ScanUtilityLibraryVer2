using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLib.Extensions;
using ScanUtilityLibrary.Model;

namespace ScanUtilityLibrary.Core
{
    /// <summary>
    /// 点云数据处理类
    /// </summary>
    public static class DataProcessUtil
    {
        ///// <summary>
        ///// 将某路径下的点云数据转换为对象
        ///// </summary>
        ///// <param name="filePath">点云文件完整路径</param>
        ///// <param name="scaleLimited">是否限制点云数据范围</param>
        ///// <param name="xfloor">X坐标下限</param>
        ///// <param name="xceil">X坐标上限</param>
        ///// <param name="yfloor">Y坐标下限</param>
        ///// <param name="yceil">Y坐标上限</param>
        //public static List<ScanPoint> GetScanPointGroup_File(string filePath, bool scaleLimited, int xfloor, int xceil, int yfloor, int yceil)
        //{
        //    if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
        //        return null;
        //    string[] infos = File.ReadAllLines(filePath);
        //    if (infos == null || infos.Length == 0)
        //        return null;
        //    List<ScanPoint> list = new List<ScanPoint>();
        //    //int index = 0;
        //    //double shifting = 0;
        //    foreach (string info in infos)
        //    {
        //        //index++;
        //        if (string.IsNullOrWhiteSpace(info))
        //            continue;
        //        string[] temp = info.Split(' ');
        //        //if (index == 1)
        //        //    shifting = double.Parse(temp[2]);
        //        ScanPoint point = new ScanPoint(ScanDeviceType.R2000, double.Parse(temp[0]), double.Parse(temp[1]), double.Parse(temp[2])); //Z坐标校准
        //        //是否限制扫描点的范围
        //        if (!scaleLimited || (point.X.Between(xfloor, xceil) && point.Y.Between(yfloor, yceil)))
        //            list.Add(point);
        //    }
        //    double min = list.Select(point => point.Z).Min();
        //    list.ForEach(point => point.Z -= min);
        //    return list;
        //}

        /// <summary>
        /// 将某路径下的点云数据转换为对象
        /// </summary>
        /// <param name="filePath">点云文件完整路径</param>
        /// <param name="scale">根据此比例对点云坐标值进行放大或缩小</param>
        /// <param name="xlims">X坐标范围（上限、下限），假如为空或数量不满足条件则不限制</param>
        /// <param name="ylims">Y坐标范围（上限、下限），假如为空或数量不满足条件则不限制</param>
        /// <param name="zlims">Z坐标范围（上限、下限），假如为空或数量不满足条件则不限制</param>
        /// <param name="xextrs">X坐标极值（最小值、最大值），假如没有符合条件的值则分别填入Int32的最大值、最小值</param>
        /// <param name="yextrs">Y坐标极值（最小值、最大值），假如没有符合条件的值则分别填入Int32的最大值、最小值</param>
        /// <param name="zextrs">Z坐标极值（最小值、最大值），假如没有符合条件的值则分别填入Int32的最大值、最小值</param>
        public static List<ScanPoint> GetScanPointGroup_File(string filePath, double scale, int[] xlims, int[] ylims, int[] zlims, out double[] xextrs, out double[] yextrs, out double[] zextrs)
        {
            xextrs = new double[] { int.MaxValue, int.MinValue };
            yextrs = new double[] { int.MaxValue, int.MinValue };
            zextrs = new double[] { int.MaxValue, int.MinValue };
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return null;
            string[] infos = File.ReadAllLines(filePath);
            if (infos == null || infos.Length == 0)
                return null;
            List<ScanPoint> list = new List<ScanPoint>();
            //int index = 0;
            //double shifting = 0;
            foreach (string info in infos)
            {
                //index++;
                if (string.IsNullOrWhiteSpace(info))
                    continue;
                string[] temp = info.Split(' ');
                //if (index == 1)
                //    shifting = double.Parse(temp[2]);
                ScanPoint point = new ScanPoint(ScanDeviceType.R2000, double.Parse(temp[0]) * scale, double.Parse(temp[1]) * scale, double.Parse(temp[2]) * scale); //Z坐标校准
                //是否在XYZ坐标上限制扫描点的范围（假如范围数组不满足条件则不限制）
                bool xcon = xlims == null || xlims.Length != 2 || point.X.Between(xlims[0], xlims[1]),
                     ycon = ylims == null || ylims.Length != 2 || point.Y.Between(ylims[0], ylims[1]),
                     zcon = zlims == null || zlims.Length != 2 || point.Z.Between(zlims[0], zlims[1]);
                if (xcon && ycon && zcon)
                {
                    list.Add(point);
                    //XYZ坐标分别与最大最小值比较
                    xextrs[0] = Math.Min(xextrs[0], point.X);
                    xextrs[1] = Math.Max(xextrs[1], point.X);
                    yextrs[0] = Math.Min(yextrs[0], point.Y);
                    yextrs[1] = Math.Max(yextrs[1], point.Y);
                    zextrs[0] = Math.Min(zextrs[0], point.Z);
                    zextrs[1] = Math.Max(zextrs[1], point.Z);
                }
            }
            //double min = list.Select(point => point.Z).Min();
            //list.ForEach(point => point.Z -= min);
            return list;
        }

        /// <summary>
        /// 将某路径下的点云数据转换为对象
        /// </summary>
        /// <param name="filePath">点云文件完整路径</param>
        /// <param name="scaleLimited">是否限制点云数据范围</param>
        /// <param name="xfloor">X坐标下限</param>
        /// <param name="xceil">X坐标上限</param>
        /// <param name="yfloor">Y坐标下限</param>
        /// <param name="yceil">Y坐标上限</param>
        public static List<ScanPoint> GetScanPointGroup_File(string filePath, bool scaleLimited, int xfloor, int xceil, int yfloor, int yceil)
        {
            int[] xlims = scaleLimited ? new int[] { xfloor, xceil } : null, ylims = scaleLimited ? new int[] { yfloor, yceil } : null;
            List<ScanPoint> points = GetScanPointGroup_File(filePath, 1, xlims, ylims, null, out double[] xextrs, out double[] yextrs, out double[] zextrs);
            double zmin = zextrs[0] == int.MaxValue ? 0 : zextrs[0];
            points.ForEach(point => point.Z -= zmin);
            return points;
        }

        /// <summary>
        /// 将某路径下的点云数据转换为对象
        /// </summary>
        /// <param name="filePath">点云文件完整路径</param>
        /// <returns></returns>
        public static List<ScanPoint> GetScanPointGroup_File_Unlimited(string filePath)
        {
            return GetScanPointGroup_File(filePath, false, int.MinValue, int.MaxValue, int.MinValue, int.MaxValue);
        }

        /// <summary>
        /// 将某路径下的点云数据转换为对象
        /// </summary>
        /// <param name="filePath">点云文件完整路径</param>
        /// <param name="scaleLimited">是否限制点云数据范围</param>
        /// <param name="xfloor">X坐标下限</param>
        /// <param name="xceil">X坐标上限</param>
        /// <param name="yfloor">Y坐标下限</param>
        /// <param name="yceil">Y坐标上限</param>
        /// <returns></returns>
        public static List<ScanPoint[]> GetFullList_File(string filePath, bool scaleLimited, int xfloor, int xceil, int yfloor, int yceil)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return null;
            //FileInfo file = new FileInfo(filePath);
            string[] infos = File.ReadAllLines(filePath);
            if (infos == null || infos.Length == 0)
                return null;
            string current = string.Empty;
            List<ScanPoint[]> full_list = new List<ScanPoint[]>();
            List<ScanPoint> list = new List<ScanPoint>();
            int index = 0;
            double shifting = 0;
            foreach (string info in infos)
            {
                index++;
                string last = current;
                current = string.Empty;
                if (string.IsNullOrWhiteSpace(info))
                    continue;
                string[] temp = info.Split(' ');
                if (index == 1)
                    shifting = double.Parse(temp[2]);
                ScanPoint point = new ScanPoint(ScanDeviceType.R2000, double.Parse(temp[0]), double.Parse(temp[1]), double.Parse(temp[2]) - shifting); //Z坐标校准
                current = temp[2];
                if (current != last)
                {
                    if (!string.IsNullOrWhiteSpace(last) && list != null)
                        full_list.Add(list.ToArray());
                    list = new List<ScanPoint>();
                }
                //是否限制扫描点的范围
                if (!scaleLimited || (point.X.Between(xfloor, xceil) && point.Y.Between(yfloor, yceil)))
                    list.Add(point);
                if (index == infos.Length)
                    full_list.Add(list.ToArray());
            }
            return full_list;
        }

        /// <summary>
        /// 将某路径下的点云数据转换为对象，根据坐标范围过滤
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="xfloor"></param>
        /// <param name="xceil"></param>
        /// <param name="yfloor"></param>
        /// <param name="yceil"></param>
        /// <returns></returns>
        public static List<ScanPoint[]> GetFullList_File_Limited(string filePath, int xfloor, int xceil, int yfloor, int yceil)
        {
            return GetFullList_File(filePath, true, xfloor, xceil, yfloor, yceil);
        }

        /// <summary>
        /// 将某路径下的点云数据转换为对象，无坐标范围限制
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<ScanPoint[]> GetFullList_File_Unlimited(string filePath)
        {
            return GetFullList_File(filePath, false, int.MinValue, int.MaxValue, int.MinValue, int.MaxValue);
        }
    }
}
