using ScanUtilityLibrary.Core.TripleIN;
using ScanUtilityLibrary.Model.TripleIN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Model.TripleIN
{
    /// <summary>
    /// Reading the firmware version.
    /// </summary>
    public class GVERCommand : CommandBase
    {
        //// the command data to be sent to the sensor.
        //private struct CommandData
        //{
        //    public char[] CommandID;
        //    public int Length;
        //    public int CRC;
        //}

        //private CommandData mCommand;
        //private string mVersion = string.Empty;

        /// <summary>
        /// 固件版本信息
        /// </summary>
        public string Version { get; private set; }

        #region 构造器
        /// <summary>
        /// 默认构造器
        /// </summary>
        public GVERCommand() : this(string.Empty) { }
        //public GVERCommand() : base(FunctionCodes.GVER, string.Empty) { }

        /// <summary>
        /// 用给定的16进制字符串初始化
        /// </summary>
        /// <param name="hexString">报文的16进制字符串</param>
        public GVERCommand(string hexString) : base(FunctionCodes.GVER, hexString) { }

        /// <summary>
        /// 用给定的byte数组初始化
        /// </summary>
        /// <param name="bytes">报文的字节流</param>
        public GVERCommand(byte[] bytes) : base(FunctionCodes.GVER, bytes) { }
        #endregion

        ///// <summary>
        ///// Performs the GVER command.
        ///// </summary>
        ///// <returns></returns>
        //public ErrorID PerformCommand()
        //{
        //    // Convert command data to byte array.
        //    byte[] commandBytes = new byte[12];
        //    Encoding.ASCII.GetBytes(mCommand.CommandID).CopyTo(commandBytes, 0);
        //    BitConverter.GetBytes(mCommand.Length).CopyTo(commandBytes, 4);
        //    BitConverter.GetBytes(mCommand.CRC).CopyTo(commandBytes, 8);

        //    // Send command to sensor.
        //    IDataStream.SendData(commandBytes);

        //    // Receive response from sensor.
        //    byte[] responseBytes = IDataStream.ReceiveData();

        //    // Parse response.
        //    if (responseBytes.Length >= 4 && Encoding.ASCII.GetString(responseBytes, 0, 4) == "GVER")
        //    {
        //        mVersion = Encoding.ASCII.GetString(responseBytes, 4, responseBytes.Length - 4);
        //        return ErrorID_t.ERR_SUCCESS;
        //    }
        //    else
        //    {
        //        return ErrorID_t.ERR_FAILURE;
        //    }
        //}

/// <inheritdoc/>
        protected override List<byte> ComposeUrOwn()
        {
            return null;
        }

/// <inheritdoc/>
        protected override void ResolveUrOwn(byte[] bytes)
        {
            Version = Encoding.Default.GetString(bytes).Trim('\0');
        }
    }

}