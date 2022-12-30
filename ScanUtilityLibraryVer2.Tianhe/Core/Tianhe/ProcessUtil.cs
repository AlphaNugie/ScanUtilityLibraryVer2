using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Core.Tianhe
{
    /// <summary>
    /// 天河激光雷达的输出处理工具类
    /// </summary>
    public class ProcessUtil
    {
        /// <summary>
        /// 返回接收信息中包含的包裹消息集合
        /// </summary>
        /// <param name="input">接收信息，处理成功后会修改为数据输出报文的内容</param>
        /// <returns></returns>
        public static bool GetWrappedMessage(ref string input)
        {
            return GetWrappedMessage(ref input, out _);
        }

        /// <summary>
        /// 返回接收信息中包含的包裹消息集合
        /// </summary>
        /// <param name="input">接收信息，处理成功后会修改为数据输出报文的内容</param>
        /// <param name="msgList">输出的所有报文消息列表</param>
        /// <returns></returns>
        public static bool GetWrappedMessage(ref string input, out List<string> msgList)
        {
            //string[] splits = string.IsNullOrWhiteSpace(input) ? new string[0] : input.Split(new string[] { "AD 49" }, StringSplitOptions.RemoveEmptyEntries);
            //msgList = splits.Select(split => "AD 49 " + split.Trim()).ToList();
            //input = msgList.FirstOrDefault(msg => msg.Contains("AD 49 6E 00"));
            string separator = "AD 49 ";
            string[] splits = string.IsNullOrWhiteSpace(input) || input.Contains(separator) ? new string[0] : input.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            msgList = splits.Select(split => separator + split).ToList();
            input = msgList.FirstOrDefault(msg => msg.Contains(separator + "6E 00"));
            bool result;
            //if (!(result = !string.IsNullOrWhiteSpace(input)))
            if (!(result = msgList == null || msgList.Count == 0))
                input = string.Empty;
            return result;
        }
    }
}
