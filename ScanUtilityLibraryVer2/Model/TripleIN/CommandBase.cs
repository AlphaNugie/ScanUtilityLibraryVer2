using CommonLib.Clients;
using CommonLib.Extensions;
using CommonLib.Function;
using CommonLib.Helpers;
using ScanUtilityLibrary.Core.TripleIN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;

namespace ScanUtilityLibrary.Model.TripleIN
{
    /// <summary>
    /// PS激光扫描仪2进制命令基础类
    /// Base class for PS Laser Scanner commands.
    /// </summary>
    public abstract class CommandBase
    {
        /// <summary>
        /// Length of the command ID
        /// </summary>
        protected const int COMMAND_ID_SIZE = 4;

        private readonly CRC32 cRC32 = new CRC32();

        //// Maximum command/response size
        //private const int MAX_COMMAND_SIZE = 64 * 1024;
        //// “ERR\0” in integer format
        //private const int ERR_COMMAND_ID = 0x00525245;
        //// SYNC in integer format
        //private const int SYNC_COMMAND_ID = 0x434e5953;

        //// Buffer to store received data
        //private byte[] mBuffer = new byte[MAX_COMMAND_SIZE];
        //// Number of bytes in the buffer
        //private int mBytesReceived;
        //// Casting pointer on the buffer
        //private IntPtr mBufferPtr;
        // The data stream to read and write bytes
        //private IDataStream mDataStream;

        #region 属性
        /// <summary>
        /// 功能码（命令ID）
        /// </summary>
        public FunctionCodes FunctionCode { get; protected set; }

        /// <summary>
        /// 数据部分占用的字节数
        /// </summary>
        public int Length { get; protected set; }

        /// <summary>
        /// 数据部分
        /// </summary>
        public byte[] Data { get; protected set; }

        /// <summary>
        /// CRC32校验值
        /// </summary>
        protected uint Checksum { get; private set; }

        /// <summary>
        /// CRC32校验值是否通过
        /// </summary>
        public bool ChecksumPassed { get; private set; }
        #endregion

        /// <summary>
        /// 默认构造器
        /// </summary>
        /// <param name="funcCode">报文的功能码</param>
        public CommandBase(FunctionCodes funcCode) : this(funcCode, string.Empty) { }

        /// <summary>
        /// 用给定的16进制字符串初始化
        /// </summary>
        /// <param name="funcCode">报文的功能码</param>
        /// <param name="hexString">报文的16进制字符串</param>
        public CommandBase(FunctionCodes funcCode, string hexString)
        {
            Resolve(hexString);
            FunctionCode = funcCode;
        }

        /// <summary>
        /// 用给定的byte数组初始化
        /// </summary>
        /// <param name="funcCode">报文的功能码</param>
        /// <param name="bytes">2进制命令的字节流</param>
        ///// <param name="dataStream"></param>
        public CommandBase(FunctionCodes funcCode, byte[] bytes)
        {
            Resolve(bytes);
            FunctionCode = funcCode;
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~CommandBase()
        {
            // Clean up unmanaged resources
            //Marshal.FreeHGlobal(mBufferPtr);
        }

        #region static方法
        /// <summary>
        /// 根据16进制字符串来解析功能码
        /// </summary>
        /// <param name="hexString"></param>
        public static FunctionCodes ResolveFuncCode(string hexString)
        {
            return string.IsNullOrWhiteSpace(hexString) ? 0 : ResolveFuncCode(HexHelper.HexString2Bytes(hexString));
        }

        /// <summary>
        /// 根据byte数组来解析功能码
        /// </summary>
        /// <param name="bytes"></param>
        public static FunctionCodes ResolveFuncCode(byte[] bytes)
        {
            FunctionCodes code = FunctionCodes.NONE;
            if (bytes == null || bytes.Length < 4)
                goto END;
            string codeStr = Encoding.Default.GetString(bytes, 0, 4).Trim('\0');
            if (!Enum.TryParse(codeStr, out FunctionCodes funcCode))
                goto END;
            code = funcCode;
        END:
            return code;
        }

        /// <summary>
        /// Calculates the CRC of a transmitter buffer and stores it in the last 4 bytes of the buffer.
        /// The CRC is stored in network byte order. After this, the buffer is ready to send on the transmitter channel.
        /// </summary>
        /// <param name="theBuffer"></param>
        /// <returns></returns>
        public static ErrorID AppendCRC(List<byte> theBuffer)
        {
            if (theBuffer == null || theBuffer.Count == 0)
                return ErrorID.ERR_INVALID_PARAMETER;
            CRC32 lCRC = new CRC32();
            uint lCRCValue = lCRC.GetCRC32(theBuffer, theBuffer.Count);
            theBuffer.AddRange(lCRCValue.ToBytes());
            return ErrorID.ERR_SUCCESS;
        }
        #endregion

        #region 抽象方法
        /// <summary>
        /// 每个子类根据自己特有属性值进行一些自己独有的byte数组组合工作（没有需要则返回null）
        /// </summary>
        /// <returns></returns>
        protected abstract List<byte> ComposeUrOwn();

        /// <summary>
        /// 每个子类根据当前命令结构中的数据部分进行一些自己独有的解析工作
        /// </summary>
        /// <param name="bytes"></param>
        protected abstract void ResolveUrOwn(byte[] bytes);
        #endregion

        #region 功能
        /// <summary>
        /// 根据16进制字符串解析
        /// </summary>
        /// <param name="hexString"></param>
        public void Resolve(string hexString)
        {
            if (string.IsNullOrWhiteSpace(hexString))
                return;
            Resolve(HexHelper.HexString2Bytes(hexString));
        }

        /// <summary>
        /// 根据byte数组解析
        /// </summary>
        /// <param name="bytes"></param>
        public void Resolve(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 12)
                return;
            string code = Encoding.Default.GetString(bytes, 0, 4).Trim('\0');
            if (!Enum.TryParse(code, out FunctionCodes funcCode))
                return;
            FunctionCode = funcCode;
            //Length = (uint)IPAddress.NetworkToHostOrder((int)BitConverter.ToUInt32(bytes, 4)); //uint
            //Length = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, 4)); //int
            Length = bytes.ToInt32(4);
            Data = Length == 0 ? new byte[0] : bytes.Skip(8).Take(Length).ToArray();
            //Checksum = (uint)IPAddress.NetworkToHostOrder((int)BitConverter.ToUInt32(bytes, 8 + Length));
            Checksum = bytes.ToUInt32(8 + Length);
            ChecksumPassed = cRC32.CheckCRC32(bytes, 4);
            ResolveUrOwn(Data);
        }

        /// <summary>
        /// 将各属性值组合为16进制字符串
        /// </summary>
        /// <returns></returns>
        public string ComposeHexString()
        {
            byte[] bytes = Compose();
            return bytes == null || bytes.Length == 0 ? string.Empty : HexHelper.ByteArray2HexString(bytes);
        }

        /// <summary>
        /// 将各属性值组合为byte数组
        /// </summary>
        /// <returns></returns>
        public byte[] Compose()
        {
            List<byte> bytes = new List<byte>(), ownBytes = ComposeUrOwn();
            ownBytes = ownBytes ?? new List<byte>();
            bytes.AddRange(Encoding.Default.GetBytes(FunctionCode.ToString()));
            //假如功能代码不足4位，在右侧补0
            for (int i = 0; i < COMMAND_ID_SIZE - bytes.Count; i++)
                bytes.Add(0x00);
            //Length = (uint)ownBytes.Count;
            ////添加长度
            //bytes.AddRange(Length.ToBytes());
            Length = ownBytes.Count;
            //添加长度
            bytes.AddRange(((uint)Length).ToBytes());
            //添加各子类自定义内容
            bytes.AddRange(ownBytes);
            //添加CRC校验值
            AppendCRC(bytes);
            return bytes.ToArray();
        }

        #region CheckErrors
        ///// <summary>
        ///// Checks a buffer for ERR responses.
        ///// </summary>
        ///// <returns></returns>
        //public ErrorID CheckErrors()
        //{
        //    int result = (int)ErrorID.ERR_SUCCESS;
        //    if (FunctionCode == FunctionCodes.ERR && Data != null && Data.Length >= 4)
        //        //result = IPAddress.NetworkToHostOrder(Marshal.ReadInt32(lData, 8));
        //        result = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(Data, 0));
        //    return ErrorIDConverter.IntToError(result);
        //}
        #endregion

        //// Transmits a buffer with a command and receives the result.
        //// Checks the data received for CRC errors and ERR responses. Received data are stored in mBuffer.
        //public ErrorID SendCommand(IntPtr theCommandPtr, int theCommandSize)
        //{
        //    ErrorID result = ErrorID.ERR_SUCCESS;
        //    try
        //    {
        //        if (mDataStream.write(theCommandPtr, theCommandSize) < 0)
        //        {
        //            throw ErrorID.ERR_WRITE;
        //        }
        //        mBytesReceived = mDataStream.read(mBuffer, mBuffer.Length);
        //        if (mBytesReceived < 0)
        //        {
        //            throw ErrorID.ERR_READ;
        //        }
        //        if (ErrorID_t.ERR_SUCCESS != checkCRC())
        //        {
        //            throw ErrorID.ERR_CRC;
        //        }
        //        result = checkErrors();
        //    }
        //    catch (ErrorID_t e)
        //    {
        //        result = e;
        //    }
        //    return result;
        //}
        #endregion
    }
}