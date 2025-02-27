using CommonLib.Function;
using ScanUtilityLibrary.Core.SICK.Dx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Model.SICK.Dx
{
    /// <summary>
    /// 距离传感器传出的各项数据
    /// </summary>
    public class SensorData
    {
        /// <summary>
        /// 目标距离（米）
        /// </summary>
        public double Distance { get; set; }

        /// <summary>
        /// 目标速度（米/秒）
        /// </summary>
        public double Velocity { get; set; }

        /// <summary>
        /// 信号级别
        /// </summary>
        public int SignalLevel { get; set; }

        /// <summary>
        /// 设备温度（°C）
        /// </summary>
        public byte Temperature { get; set; }

        /// <summary>
        /// 操作总时长（小时）
        /// </summary>
        public uint OperatingHours { get; set; }

        /// <summary>
        /// 设备状态字，具体说明见用户手册或解决方案根目录“测距仪状态字定义.xlsx”文档
        /// </summary>
        public DeviceStatusWord StatusWord { get; set; }

        /// <summary>
        /// 默认构造器
        /// </summary>
        public SensorData()
        {
            StatusWord = new DeviceStatusWord();
        }

        /// <summary>
        /// 从其它实例中复制数据值
        /// </summary>
        /// <param name="other"></param>
        public void Clone(SensorData other)
        {
            if (other == null)
                return;

            //除StatusWord外的每个属性赋值
            List<PropertyInfo> props = typeof(SensorData).GetProperties().Where(prop => !prop.Name.Equals("StatusWord")).ToList();
            props.ForEach(prop => prop.SetValue(this, prop.GetValue(other)));
            //StatusWord赋值
            if (other.StatusWord != null)
                StatusWord.Update(other.StatusWord.SourceValue);
        }

        /// <summary>
        /// 获取测距仪数据的字符串描述
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(@"距离：{0}米，速度：{1}米/秒，温度：{2}℃，状态字：{3}，信号级别：{4}，操作时间：{5}小时", Distance, Velocity, Temperature, StatusWord.SourceValue, SignalLevel, OperatingHours);
        }

        /// <summary>
        /// 解析连续输出报文
        /// </summary>
        /// <param name="info">连续输出报文内容</param>
        public void ResolveTelegram(string info)
        {
            string result = RegexMatcher.FindLastMatch(info, Patterns.ContiAnyOutput);
            if (string.IsNullOrWhiteSpace(result))
                return;
            string code = result.Substring(0, 4);
            switch (code)
            {
                case "0321":
                    ResolveTelegram(ContinualOutputType.DistStatus, result);
                    break;
                case "0322":
                    ResolveTelegram(ContinualOutputType.Distance, result);
                    break;
                case "0323":
                    ResolveTelegram(ContinualOutputType.DistSigLevel, result);
                    break;
                case "0324":
                    ResolveTelegram(ContinualOutputType.DistVel, result);
                    break;
            }
        }

        /// <summary>
        /// 距离输出分类处理报文
        /// </summary>
        /// <param name="type"></param>
        /// <param name="input"></param>
        private void ResolveTelegram(ContinualOutputType type, string input)
        {
            if (type == ContinualOutputType.OutputOff)
                return;
            //排除开头4个字符，找出距离符号、距离值，以及这之后的尾巴部分
            string main = input.Substring(4), sign = main.Substring(0, 1), value = main.Substring(1, 7), rear = string.Empty;
            if (main.Length > 8)
                rear = main.Substring(8);
            Distance = Math.Round(double.Parse(value) / 1000 * (sign.Equals("-") ? -1 : 1), 3);
            if (string.IsNullOrWhiteSpace(rear))
                return;
            //假如有尾巴，分解为符号部分以及数值部分
            sign = rear.Substring(0, 1);
            value = rear.Substring(1);
            switch (type)
            {
                case ContinualOutputType.DistVel:
                    Velocity = Math.Round(double.Parse(value) / 1000 * (sign.Equals("-") ? -1 : 1), 3);
                    break;
                case ContinualOutputType.DistStatus:
                    StatusWord.Update(Convert.ToUInt32(value, 16));
                    break;
                case ContinualOutputType.DistSigLevel:
                    SignalLevel = int.Parse(value);
                    break;
            }
        }
    }
}
