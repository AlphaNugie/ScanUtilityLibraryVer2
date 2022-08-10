using ScanUtilityLibrary.Model.SICK.Dx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Core.SICK.Dx
{
    /// <summary>
    /// 西克（SICK）LMS扫描仪的命令处理类
    /// </summary>
    public class CommandSender : BaseCmdSender
    {
        /// <summary>
        /// 以指定的tcp通讯对象初始化
        /// </summary>
        /// <param name="client"></param>
        public CommandSender(BaseTcpClient client) : base(client) { }

        /// <summary>
        /// 发送用户登录的指令
        /// </summary>
        /// <param name="userLevel">用户级别</param>
        /// <param name="userPwdHash">登录密码的8位16进制哈希值</param>
        /// <returns>返回从设备获取的信息</returns>
        public override string Login(UserLevel userLevel, string userPwdHash)
        {
            try
            {
                string command = string.Format("sMN SetAccessMode {0} {1}", (int)userLevel, userPwdHash); //将用户级别的代码转为2位带前导零的10进制数字
                string info = SendCommand(command); //发送指令
                //假如传送、接受指令的过程中出现异常，在字符串开头添加字符'e'
                if (info.StartsWith("Error:"))
                    return 'e' + info.Replace("Error:", string.Empty).TrimStart();
                //否则将字符串最后一个字符添加到第一个字符前(0/1)
                else
                    return info.Length > 0 ? info[info.Length - 1] + info : '0' + info;
            }
            catch (Exception e) { return 'e' + e.Message; }//假如引发异常，在字符串开头添加字符“e”
        }

        /// <summary>
        /// 发送用户登录的指令
        /// </summary>
        /// <param name="userLevel">用户级别</param>
        /// <returns>返回从设备获取的信息</returns>
        public string Login(UserLevel userLevel)
        {
            return Login(userLevel, GetPwdHashByUserLevel(userLevel));
        }

        /// <summary>
        /// 设置连续读取的状态
        /// </summary>
        /// <param name="type">持续输出的类型</param>
        /// <returns>返回从设备获取的信息</returns>
        public string SetContinualOutputType(ContinualOutputType type)
        {
            try
            {
                string command = string.Format("sWN rs422PeriodicOutputContent {0}", (int)type);
                string info = SendCommand(command);
                if (info != null && info.Equals("sWA rs422PeriodicOutputContent"))
                    OutputFlag = (int)type;
                return info;
            }
            catch (Exception e) { return "Error: " + e.Message; }
        }

        /// <summary>
        /// 设置连续读取的协议
        /// </summary>
        /// <param name="prot">持续输出的协议</param>
        /// <returns>返回从设备获取的信息</returns>
        public string SetContinualOutputProtocol(ContinualOutputProtocol prot)
        {
            try
            {
                string command = string.Format("sWN rs422PeriodicOutputFormat {0}", (int)prot);
                return SendCommand(command);
            }
            catch (Exception e) { return "Error: " + e.Message; }
        }

        /// <summary>
        /// 设置连续读取的速率（0~1000毫秒）
        /// </summary>
        /// <param name="rate">持续输出的速率（0~1000毫秒，超出部分按最大值算）</param>
        /// <returns>返回从设备获取的信息</returns>
        public string SetContinualOutputRate(ushort rate)
        {
            try
            {
                rate = rate > 1000 ? (ushort)1000 : rate;
                //将速率转换为16进制
                string command = string.Format("sWN rs422PeriodicDuration {0}", Convert.ToString(rate, 16));
                return SendCommand(command);
            }
            catch (Exception e) { return "Error: " + e.Message; }
        }

        /// <summary>
        /// 设置距离值的校正值（-4500米~4500米）
        /// </summary>
        /// <param name="offset">距离值的校正值（-4500米~4500米，超出部分按最大或最小值算）</param>
        /// <returns>返回从设备获取的信息</returns>
        public string SetDistanceOffset(double offset)
        {
            try
            {
                int ioff = (int)(offset * 1000);
                if (ioff < -4500000)
                    ioff = -4500000;
                else if (ioff > 4500000)
                    ioff = 4500000;
                //将速率转换为16进制
                string command = string.Format("sWN offset {0}", ioff);
                return SendCommand(command);
            }
            catch (Exception e) { return "Error: " + e.Message; }
        }

        /// <summary>
        /// 从设备获取距离
        /// </summary>
        /// <param name="dist">从报文拿到的距离值</param>
        /// <returns>报文的获取结果</returns>
        public bool RequireDistance(out double dist)
        {
            dist = 0;
            bool result = RequireDeviceData(RequiredDataType.Distance, out int data);
            if (result)
                dist = Math.Round((double)data / 1000, 3);
            return result;
        }

        /// <summary>
        /// 从设备获取速度
        /// </summary>
        /// <param name="vel">从报文拿到的速度值</param>
        /// <returns>报文的获取结果</returns>
        public bool RequireVelocity(out double vel)
        {
            vel = 0;
            bool result = RequireDeviceData(RequiredDataType.Velocity, out int data);
            if (result)
                vel = Math.Round((double)data / 1000, 3);
            return result;
        }

        /// <summary>
        /// 从设备获取信号强度
        /// </summary>
        /// <param name="level">从报文拿到的信号强度值</param>
        /// <returns>报文的获取结果</returns>
        public bool RequireSignalLevel(out int level)
        {
            bool result = RequireDeviceData(RequiredDataType.SignalLevel, out level);
            return result;
        }

        /// <summary>
        /// 从设备获取设备温度
        /// </summary>
        /// <param name="temp">从报文拿到的设备温度值</param>
        /// <returns>报文的获取结果</returns>
        public bool RequireDeviceTemperature(out byte temp)
        {
            bool result = RequireDeviceData(RequiredDataType.DeviceTemperature, out temp);
            return result;
        }

        /// <summary>
        /// 从设备获取操作时长（小时）
        /// </summary>
        /// <param name="hours">从报文拿到的操作时长值（小时）</param>
        /// <returns>报文的获取结果</returns>
        public bool RequireDeviceOpHours(out uint hours)
        {
            bool result = RequireDeviceData(RequiredDataType.OperatingHours, out hours);
            return result;
        }

        /// <summary>
        /// 从设备获取状态字
        /// </summary>
        /// <param name="statusWord">从报文拿到的状态字</param>
        /// <returns>报文的获取结果</returns>
        public bool RequireStatusWord(ref DeviceStatusWord statusWord)
        {
            if (statusWord == null)
                statusWord = new DeviceStatusWord();
            bool result = RequireDeviceData(RequiredDataType.DeviceStatusWord, out uint data);
            if (result)
                statusWord.Update(data);
            return result;
        }

        /// <summary>
        /// 从设备获取特定数据类型的数据（输出类型仅限于Int32, UInt32）
        /// </summary>
        /// <param name="dataType">数据类型</param>
        /// <param name="data">从报文拿到的数据</param>
        /// <returns>报文的获取结果</returns>
        public bool RequireDeviceData<T>(RequiredDataType dataType, out T data) where T : struct, IComparable
        {
            Type type = typeof(T), targetType = null;
            string template = "sRN {0}", code = string.Empty;
            switch (dataType)
            {
                case RequiredDataType.Distance:
                    code = "Distance";
                    targetType = typeof(int);
                    break;
                case RequiredDataType.Velocity:
                    code = "Velocity";
                    targetType = typeof(int);
                    break;
                case RequiredDataType.SignalLevel:
                    code = "RSSI";
                    targetType = typeof(int);
                    break;
                case RequiredDataType.DeviceTemperature:
                    code = "deviceTemperature";
                    targetType = typeof(byte);
                    break;
                case RequiredDataType.OperatingHours:
                    code = "OpHoursDevice";
                    targetType = typeof(uint);
                    break;
                case RequiredDataType.DeviceStatusWord:
                    code = "deviceStatusWord";
                    targetType = typeof(uint);
                    break;
            }
            data = default;
            //data = 0;
            //假如泛型并非对应参数的目标类型，退出
            if (type != targetType)
                return false;
            try
            {
                string command = string.Format(template, code);
                //用空格分隔，将最后一项从16进制转为10进制
                string info = SendCommand(command), part = info.Split(' ')[2];
                //根据泛型类型进行对应转换
                if (type == typeof(int))
                    data = (T)Convert.ChangeType(Convert.ToInt32(part, 16), type);
                else if (type == typeof(uint))
                    data = (T)Convert.ChangeType(Convert.ToUInt32(part, 16), type);
                else if (type == typeof(byte))
                    data = (T)Convert.ChangeType(Convert.ToByte(part, 16), type);
                //data = type == typeof(int) ? (T)Convert.ChangeType(Convert.ToInt32(part, 16), type) : (T)Convert.ChangeType(Convert.ToUInt32(part, 16), type);
                return true;
            }
            catch (Exception e)
            {
                LastMessage = string.Format("Error: 获取数据类型{0}失败, {1}", dataType.ToString(), e.Message);
                return false;
            }
        }
    }
}
