using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Model.SICK.Scanner
{
    /// <summary>
    /// 扫描仪扫描分区的实体类
    /// </summary>
    public class ScannerSector
    {
        ///// <summary>
        ///// 分区编号（1~4）
        ///// </summary>
        //public int No { get;set;}

        /// <summary>
        /// 角分辨率
        /// </summary>
        public double AngularResolution { get;set; }

        /// <summary>
        /// 起始角度
        /// </summary>
        public double StartAngle { get; set; }

        /// <summary>
        /// 终止角度
        /// </summary>
        public double StopAngle { get; set; }

        /// <summary>
        /// 用给定的分区编号、角分辨率以及起止角度将实体类对象初始化
        /// </summary>
        /// <param name="angRes">角分辨率</param>
        /// <param name="startAng">起始角度</param>
        /// <param name="stopAng">终止角度</param>
        ///// <param name="no">分区编号</param>
        public ScannerSector(/*int no, */double angRes, double startAng, double stopAng)
        {
            //No = no;
            AngularResolution = angRes;
            StartAngle = startAng;
            StopAngle = stopAng;
        }

        /// <summary>
        /// 转换为向设备发送的命令格式
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string angleRes = '+' + (AngularResolution * 10000).ToString("0");
            string degree1 = (StartAngle <= 0 ? string.Empty : "+") + (StartAngle * 10000).ToString("0");
            string degree2 = (StopAngle <= 0 ? string.Empty : "+") + (StopAngle * 10000).ToString("0");
            //return string.Format("{0} {1} {2}", angleRes, degree1, degree2);
            return $"{angleRes} {degree1} {degree2}";
        }
    }
}
