using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLib.Clients;
using CommonLib.Extensions;
using ScanUtilityLibrary.Model;

namespace ScanUtilityLibrary.Core
{
    /// <summary>
    /// 点云数据处理类
    /// </summary>
    public static class DataProcessUtil
    {
        #region 从文件获取点云数据
        /// <summary>
        /// 将某路径下的点云数据转换为对象
        /// </summary>
        /// <param name="filePath">点云文件完整路径</param>
        /// <param name="scale">根据此比例对点云坐标值进行放大或缩小</param>
        /// <param name="xlims">X坐标范围（上限、下限，缩放后坐标），假如为空或数量不满足条件则不限制</param>
        /// <param name="ylims">Y坐标范围（上限、下限，缩放后坐标），假如为空或数量不满足条件则不限制</param>
        /// <param name="zlims">Z坐标范围（上限、下限，缩放后坐标），假如为空或数量不满足条件则不限制</param>
        /// <param name="xextrs">X坐标极值（最小值、最大值，缩放后坐标），假如没有符合条件的值则分别填入Int32的最大值、最小值</param>
        /// <param name="yextrs">Y坐标极值（最小值、最大值，缩放后坐标），假如没有符合条件的值则分别填入Int32的最大值、最小值</param>
        /// <param name="zextrs">Z坐标极值（最小值、最大值，缩放后坐标），假如没有符合条件的值则分别填入Int32的最大值、最小值</param>
        public static List<ScanPoint> GetScanPointGroup_File(string filePath, double scale, double[] xlims, double[] ylims, double[] zlims, out double[] xextrs, out double[] yextrs, out double[] zextrs)
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
        public static List<ScanPoint> GetScanPointGroup_File(string filePath, bool scaleLimited = false, int xfloor = int.MinValue, int xceil = int.MaxValue, int yfloor = int.MinValue, int yceil = int.MaxValue)
        {
            //int[] xlims = scaleLimited ? new int[] { xfloor, xceil } : null, ylims = scaleLimited ? new int[] { yfloor, yceil } : null;
            double[] xlims = scaleLimited ? new double[] { xfloor, xceil } : null, ylims = scaleLimited ? new double[] { yfloor, yceil } : null;
            List<ScanPoint> points = GetScanPointGroup_File(filePath, 1, xlims, ylims, null, out double[] xextrs, out double[] yextrs, out double[] zextrs);
            double zmin = zextrs[0] == int.MaxValue ? 0 : zextrs[0];
            points.ForEach(point => point.Z -= zmin);
            return points;
        }

        /// <summary>
        /// （已弃用，直接使用GetScanPointGroup_File方法）将某路径下的点云数据转换为对象
        /// </summary>
        /// <param name="filePath">点云文件完整路径</param>
        /// <returns></returns>
        [Obsolete]
        public static List<ScanPoint> GetScanPointGroup_File_Unlimited(string filePath)
        {
            //return GetScanPointGroup_File(filePath, false, int.MinValue, int.MaxValue, int.MinValue, int.MaxValue);
            return GetScanPointGroup_File(filePath);
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
        public static List<ScanPoint[]> GetFullList_File(string filePath, bool scaleLimited = false, int xfloor = int.MinValue, int xceil = int.MaxValue, int yfloor = int.MinValue, int yceil = int.MaxValue)
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
                ScanPoint point = new ScanPoint(ScanDeviceType.R2000, double.Parse(temp[0]), double.Parse(temp[1]), double.Parse(temp[2]) - shifting); //Z坐标校准，以第一行的Z轴坐标为基准
                current = temp[2];
                //当Z轴坐标变化时变更为下一组
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
        /// （已弃用，直接使用GetFullList_File方法）将某路径下的点云数据转换为对象，无坐标范围限制
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [Obsolete]
        public static List<ScanPoint[]> GetFullList_File_Unlimited(string filePath)
        {
            //return GetFullList_File(filePath, false, int.MinValue, int.MaxValue, int.MinValue, int.MaxValue);
            return GetFullList_File(filePath);
        }
        #endregion

        #region 点云数据写入文件
        /// <summary>
        /// 将扫描点信息保存到XYZ文件中
        /// </summary>
        /// <param name="listPoints_full">包含扫描点数组的集合，逐数组保存</param>
        /// <param name="filePath">保存文件路径</param>
        /// <param name="fileName">保存文件名</param>
        /// <param name="extension">文件扩展名，假如不为空则添加到文件名后</param>
        /// <param name="scale">保存时对各坐标进行缩放的比例</param>
        /// <param name="dividedBy">总点云数量将被除以此值</param>
        /// <param name="overriding">是否覆盖文本</param>
        public static void WritePointsListToFile(IEnumerable<ScanPoint[]> listPoints_full, string filePath, string fileName, string extension = "", double scale = 1, uint dividedBy = 1, bool overriding = false)
        {
            if (listPoints_full == null || listPoints_full.Count() == 0)
                return;

            foreach (ScanPoint[] points in listPoints_full)
                WritePointsToFile(points, filePath, fileName, extension, scale, dividedBy, overriding);
        }

        /// <summary>
        /// 将扫描点信息保存到文件中
        /// </summary>
        /// <param name="points">包含扫描点对象的集合</param>
        /// <param name="filePath">保存文件路径</param>
        /// <param name="fileName">保存文件名</param>
        /// <param name="extension">文件扩展名，假如不为空则添加到文件名后</param>
        /// <param name="scale">保存时对各坐标进行缩放的比例</param>
        /// <param name="dividedBy">总点云数量将被除以此值</param>
        /// <param name="overriding">是否覆盖文本</param>
        public static void WritePointsToFile(IEnumerable<ScanPoint> points, string filePath, string fileName, string extension = "", double scale = 1, uint dividedBy = 1, bool overriding = false)
        {
            if (points == null || points.Count() == 0)
                return;
            //try { FileClient.WriteLinesToFile(ScanPointUtil.Convert_CoorStrings(points), filePath, fileName, extension, overriding); }
            //catch (Exception) { }
            //List<string> list = points.Where(point => point != null).Select(point => point.ToString(scale)).ToList();
            List<string> list = points.Select((point, index) => new { point, index }).Where(group => group.point != null && group.index % dividedBy == 0).Select(group => group.point.ToString(scale)).ToList();
            try { FileClient.WriteLinesToFile(list, filePath, fileName, extension, overriding); }
            catch (Exception) { }
        }
        #endregion
    }
}
