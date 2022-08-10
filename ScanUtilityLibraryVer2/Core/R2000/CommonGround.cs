using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Core.R2000
{
    /// <summary>
    /// 扫描功能公用常量
    /// </summary>
    public struct ScanConst
    {
        /// <summary>
        /// OMDxxx-R2000 UHD型号可选的单圈扫描点数量
        /// </summary>
        public static List<int> ScanPointsNumList_UHD = new List<int>() { 72, 90, 120, 144, 180, 240, 360, 400, 450, 480, 600, 720, 800, 900, 1200, 1440, 1800, 2400, 3600, 4200, 5040, 5600, 6300, 7200, 8400, 10080, 12600, 16800, 25200 };

        /// <summary>
        /// OMDxxx-R2000 HD型号可选的单圈扫描点数量
        /// </summary>
        public static List<int> ScanPointsNumList_HD = new List<int>() { 72, 90, 120, 144, 180, 240, 360, 400, 450, 480, 600, 720, 800, 900, 1200, 1440, 1680, 1800, 2100, 2400, 2800, 3600, 4200, 5040, 5600, 6300, 7200, 8400 };
    }

    /// <summary>
    /// 扫描公用方法
    /// </summary>
    public static class ScanFunc
    {
        /// <summary>
        /// 根据设备型号获取单圈扫描点数数据源，VALUE列为单圈扫描点数，TEXT列为显示角分辨率/单圈点数的文本
        /// </summary>
        /// <param name="family">设备型号</param>
        /// <returns></returns>
        public static DataTable GetScanPointsNum_DataTable(DeviceFamily family)
        {
            if (family != DeviceFamily.OMDxxx_R2000_HD && family != DeviceFamily.OMDxxx_R2000_UHD)
                return null;

            List<int> list = family == DeviceFamily.OMDxxx_R2000_HD ? ScanConst.ScanPointsNumList_HD : ScanConst.ScanPointsNumList_UHD;
            if (list == null || list.Count == 0)
                return null;

            DataTable table = new DataTable();
            table.Columns.AddRange(new DataColumn[] { new DataColumn("VALUE", typeof(int)), new DataColumn("TEXT", typeof(string)) });
            foreach (int num in list)
                table.Rows.Add(num, string.Format("{0}° / {1}", ScanFunc.GetAngleResolution(num), num));

            return table;
        }

        /// <summary>
        /// 根据产品型号获取扫描点数量List
        /// </summary>
        /// <param name="family">产品型号</param>
        /// <returns></returns>
        public static List<int> GetScanPointsNum_List(DeviceFamily family)
        {
            if (family != DeviceFamily.OMDxxx_R2000_HD && family != DeviceFamily.OMDxxx_R2000_UHD)
                return null;

            return family == DeviceFamily.OMDxxx_R2000_HD ? ScanConst.ScanPointsNumList_HD : ScanConst.ScanPointsNumList_UHD;
        }

        /// <summary>
        /// 根据单圈扫描点数获取角分辨率
        /// </summary>
        /// <param name="num">单圈扫描点数</param>
        /// <returns>返回字符串形式</returns>
        public static string GetAngleResolution(int num)
        {
            return ((double)360 / num).ToString("f3");
        }
    }
}
