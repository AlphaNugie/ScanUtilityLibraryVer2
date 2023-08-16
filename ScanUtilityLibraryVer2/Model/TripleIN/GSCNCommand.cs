using CommonLib.Extensions;
using ScanUtilityLibrary.Core.TripleIN;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Policy;

namespace ScanUtilityLibrary.Model.TripleIN
{
    /// <summary>
    /// Getting scans from the sensor.
    /// </summary>
    public class GSCNCommand : CommandBase
    {
        ///// <summary>
        ///// GSCN Parameters block. Is transmitted exactly in this order.
        ///// </summary>
        //public enum GSCNParameters
        //{
        //    /// <summary>
        //    /// 扫描数
        //    /// </summary>
        //    PARAMETER_SCAN_NUMBER,

        //    /// <summary>
        //    /// 时间戳
        //    /// </summary>
        //    PARAMETER_TIME_STAMP,

        //    /// <summary>
        //    /// 扫描开始方向
        //    /// </summary>
        //    PARAMETER_SCAN_START_DIRECTION,

        //    /// <summary>
        //    /// 扫描角度
        //    /// </summary>
        //    PARAMETER_SCAN_ANGLE,

        //    /// <summary>
        //    /// 回波数量
        //    /// </summary>
        //    PARAMETER_NUMBER_OF_ECHOES,

        //    /// <summary>
        //    /// 角度增量编码器
        //    /// </summary>
        //    PARAMETER_INCREMENTAL_ENCODER,

        //    /// <summary>
        //    /// 温度
        //    /// </summary>
        //    PARAMETER_TEMPERATURE,

        //    /// <summary>
        //    /// 系统状态
        //    /// </summary>
        //    PARAMETER_SYSTEM_STATUS,

        //    /// <summary>
        //    /// 数据内容
        //    /// </summary>
        //    PARAMETER_DATA_CONTENT,

        //    /// <summary>
        //    /// 扫描线
        //    /// </summary>
        //    PARAMETER_SCAN_LINE,

        //    /// <summary>
        //    /// 扫描参数
        //    /// </summary>
        //    NUMBER_OF_SCAN_PARAMETER,

        //    /// <summary>
        //    /// data block is empty
        //    /// </summary>
        //    NO_DATABLOCK = 0,

        //    /// <summary>
        //    /// data block contains distances only
        //    /// </summary>
        //    DATABLOCK_WITH_DISTANCES = 4,
        //    /// <summary>
        //    /// data block contains distances and pulse witdh
        //    /// </summary>
        //    DATABLOCK_WITH_DISTANCES_PW = 8,

        //    /// <summary>
        //    /// data block contains distances, and pulse witdh with echoes
        //    /// </summary>
        //    DATABLOCK_WITH_DISTANCES_PW_INCLUDES_ECHO = 7,

        //    /// <summary>
        //    /// defines the max. number of points per profile
        //    /// </summary>
        //    MAX_POINTS_PER_SCAN = 4000,

        //    /// <summary>
        //    /// defines the max. number of echoes per point
        //    /// </summary>
        //    MAX_NUMBER_OF_ECHOS = 4
        //}


        //// GSCN command data to be sent to the sensor.
        //[StructLayout(LayoutKind.Sequential)]
        //private struct command_t
        //{
        //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        //    public char[] mCommandID;
        //    public int mLength;
        //    public int mScanNumber;
        //    public int mCRC;
        //}

        //private command_t mCommand;

        /// <summary>
        /// 扫描数，0代表获取最新的扫描数据
        /// </summary>
        public int ScanNumber { get; internal set; }

        /// <summary>
        /// 当前扫描数据
        /// </summary>
        public Scan CurrScan { get; private set; } = new Scan();

        #region 构造器
        /// <summary>
        /// 用默认的扫描数0初始化，获取最新的扫描数据
        /// </summary>
        public GSCNCommand() : this(0) { }

        /// <summary>
        /// 用给定的扫描数初始化，默认为0，为0代表获取最新的扫描数据
        /// </summary>
        /// <param name="scanNum">扫描数，0代表获取最新的扫描数据</param>
        public GSCNCommand(int scanNum) : this(string.Empty) { ScanNumber = scanNum; }

        /// <summary>
        /// 用给定的16进制字符串初始化
        /// </summary>
        /// <param name="hexString">报文的16进制字符串</param>
        public GSCNCommand(string hexString) : base(FunctionCodes.GSCN, hexString) { }

        /// <summary>
        /// 用给定的byte数组初始化
        /// </summary>
        /// <param name="bytes">报文的字节流</param>
        public GSCNCommand(byte[] bytes) : base(FunctionCodes.GSCN, bytes) { }
        #endregion

        ///// <summary>
        ///// Parses the receiver buffer and copy the result into the scan structure.
        ///// Must be called after performCommand().
        ///// </summary>
        ///// <param name="theScan">Structure of type Scan_t to store the results.</param>
        ///// <returns>ERR_SUCCESS on success, otherwise a negative error code.</returns>
        //public ErrorID ParseScan(ref Scan theScan)
        //{
        //    //这个函数的作用是将接收缓冲区中的数据解析并复制到扫描结构中。它首先设置一个指向整数的指针，然后跳过命令ID和长度。接下来，它获取参数的数量并检查固件和控制程序的兼容性。然后，它将已知参数复制到扫描结构中，并跳过未知参数。接着，它获取回波的数量，如果为0，则传输主回波而不是数量。然后，它获取点数并检查限制。最后，它根据数据内容复制数据块。如果数据内容为NO_DATABLOCK，则没有数据可用。如果数据内容为DATABLOCK_WITH_DISTANCES，则只复制距离。如果数据内容为DATABLOCK_WITH_DISTANCES_PW或DATABLOCK_WITH_DISTANCES_PW_INCLUDES_ECHO，则复制距离和脉冲宽度。如果数据内容为DATABLOCK_WITH_DISTANCES_PW_INCLUDES_ECHO，则需要删除回波编号。最后，它返回错误ID。
        //    // set moving integer pointer; skip command ID and length
        //    int[] lDataPtr = mBuffer;
        //    int lIndex = 2;
        //    int lNumberOfParameter = lDataPtr[lIndex++];

        //    // check compatibility of firmware and control program
        //    if (lNumberOfParameter >= (int)GSCNParameters.NUMBER_OF_SCAN_PARAMETER)
        //    {
        //        NumberOfParameter = (int)GSCNParameters.NUMBER_OF_SCAN_PARAMETER;
        //    }
        //    else
        //    {
        //        NumberOfParameter = lNumberOfParameter;
        //    }

        //    // copy known parameter to scan
        //    for (int l = 0; l < NumberOfParameter; l++)
        //    {
        //        Parameters[l] = lDataPtr[lIndex++];
        //    }

        //    // skip unkown parameter
        //    for (int l = NumberOfParameter; l < lNumberOfParameter; l++)
        //    {
        //        lIndex++;
        //    }

        //    // get number of echoes. If 0, then the master echo is transfered instead of the number
        //    int lNumberOfEchoes = Parameters[(int)GSCNParameters.PARAMETER_NUMBER_OF_ECHOES];
        //    if (0 == lNumberOfEchoes)
        //    {
        //        lNumberOfEchoes = 1;
        //    }

        //    // take number of points, check limits
        //    NumberOfPoints = lDataPtr[lIndex++];
        //    if ((int)GSCNParameters.MAX_NUMBER_OF_ECHOS < lNumberOfEchoes || (int)GSCNParameters.MAX_POINTS_PER_SCAN < NumberOfPoints)
        //    {
        //        clearScan(ref theScan);
        //        return ErrorID_t.ERR_BUFFER_OVERFLOW;
        //    }

        //    // copy data block according to the data content.
        //    switch ((int)Parameters[(int)GSCNParameters.PARAMETER_DATA_CONTENT])
        //    {
        //        case (int)GSCNParameters.NO_DATABLOCK:
        //            // no data available
        //            break;

        //        // copy distances only
        //        // loop through all points to copy distances and pulse width
        //        case (int)GSCNParameters.DATABLOCK_WITH_DISTANCES:
        //            for (int lPoints = 0; lPoints < NumberOfPoints; lPoints++)
        //            {
        //                // loop for each point through all echos
        //                for (int lEchos = 0; lEchos < lNumberOfEchoes; lEchos++)
        //                {
        //                    ScanData[lPoints, lEchos].mDistance = lDataPtr[lIndex++];
        //                } // end echos
        //            } // end points
        //            break;

        //        // default: distance and pulse width. If the echo number is included, we remove it.
        //        case (int)GSCNParameters.DATABLOCK_WITH_DISTANCES_PW:
        //        case (int)GSCNParameters.DATABLOCK_WITH_DISTANCES_PW_INCLUDES_ECHO:
        //        default:
        //            for (int lPoints = 0; lPoints < NumberOfPoints; lPoints++)
        //            {
        //                // loop for each point through all echos
        //                for (int lEchos = 0; lEchos < lNumberOfEchoes; lEchos++)
        //                {
        //                    ScanData[lPoints, lEchos].mDistance = lDataPtr[lIndex++];
        //                    ScanData[lPoints, lEchos].mPulseWidth = lDataPtr[lIndex++];
        //                } // endechos
        //            } // end points

        //            // if echo number is included, remove it
        //            if ((int)GSCNParameters.DATABLOCK_WITH_DISTANCES_PW_INCLUDES_ECHO == Parameters[(int)GSCNParameters.PARAMETER_DATA_CONTENT])
        //            {
        //                for (int lPoints = 0; lPoints < NumberOfPoints; lPoints++)
        //                {
        //                    for (int lEchos = 0; lEchos < lNumberOfEchoes; lEchos++)
        //                    {
        //                        ScanData[lPoints, lEchos].mEchoNumber = lEchos;
        //                    }
        //                }
        //            }
        //            break;
        //    }

        //    return ErrorID_t.ERR_NO_ERROR;
        //}

        ///// <summary>
        ///// Clears a Scan_t structure.
        ///// </summary>
        ///// <param name="theScan">Structure of type Scan_t to be cleared.</param>
        //public void ClearScan(ref Scan theScan)
        //{
        //    theScan.NumberOfParameter = 0;
        //    theScan.NumberOfPoints = 0;
        //    theScan.Parameters = new GSCNParameters();
        //    //Parameters = new int[(int)GSCNParameters.NUMBER_OF_SCAN_PARAMETER];
        //    //int paramCount = theScan.Parameters.NUMBER_OF_SCAN_PARAMETER;
        //    //for (int l = 0; l < paramCount; l++)
        //    //    Parameters[l] = 0;
        //    //ScanData = new ScanData_t[(int)GSCNParameters.MAX_POINTS_PER_SCAN];
        //    //for (int i = 0; i < MAX_POINTS_PER_SCAN; i++)
        //    //{
        //    //    ScanData[i] = new ScanData_t[MAX_NUMBER_OF_ECHOS];
        //    //}
        //}

        ///// <summary>
        ///// Performs the GSCN command.
        ///// </summary>
        ///// <param name="theScanNumber">the scan number; 0 means the latest scan measured.</param>
        ///// <param name="theScan"></param>
        ///// <returns></returns>
        //public ErrorID PerformCommand(int theScanNumber, ref Scan theScan)
        //{
        //    //// Initialize command data.
        //    //ScanNumber = theScanNumber;
        //    //Length = Marshal.SizeOf(typeof(Scan_t));
        //    //// Convert command data to byte array.
        //    //byte[] commandBytes = new byte[Length];
        //    //IntPtr ptr = Marshal.AllocHGlobal(Length);
        //    //Marshal.StructureToPtr(theScan, ptr, true);
        //    //Marshal.Copy(ptr, commandBytes, 0, Length);
        //    //Marshal.FreeHGlobal(ptr);

        //    //// Calculate CRC.
        //    //mCommand.mCRC = CalculateCRC(commandBytes, Length);

        //    //// Convert command data to byte array.
        //    //commandBytes = new byte[Marshal.SizeOf(typeof(command_t))];
        //    //ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(command_t)));
        //    //Marshal.StructureToPtr(mCommand, ptr, true);
        //    //Marshal.Copy(ptr, commandBytes, 0, Marshal.SizeOf(typeof(command_t)));
        //    //Marshal.FreeHGlobal(ptr);

        //    //// Send command to the sensor.
        //    //ErrorID error = mDataStream.SendData(commandBytes, commandBytes.Length);
        //    //if (error != ErrorID.NO_ERROR)
        //    //{
        //    //    return error;
        //    //}

        //    //// Receive scan data from the sensor.
        //    //byte[] scanBytes = new byte[Length];
        //    //error = mDataStream.ReceiveData(scanBytes, scanBytes.Length);
        //    //if (error != ErrorID.NO_ERROR)
        //    //{
        //    //    return error;
        //    //}

        //    //// Convert scan data to Scan_t structure.
        //    //ptr = Marshal.AllocHGlobal(scanBytes.Length);
        //    //Marshal.Copy(scanBytes, 0, ptr, scanBytes.Length);
        //    //theScan = (Scan_t)Marshal.PtrToStructure(ptr, typeof(Scan_t));
        //    //Marshal.FreeHGlobal(ptr);

        //    return ErrorID.ERR_NONE;
        //}

        //// Calculates the CRC of a byte array.
        //private int CalculateCRC(byte[] data, int length)
        //{
        //    int crc = 0;
        //    for (int i = 0; i < length; i++)
        //    {
        //        crc ^= data[i] << 8;
        //        for (int j = 0; j < 8; j++)
        //        {
        //            if ((crc & 0x8000) != 0)
        //            {
        //                crc = (crc << 1) ^ 0x1021;
        //            }
        //            else
        //            {
        //                crc <<= 1;
        //            }
        //        }
        //    }
        //    return crc;
        //}

        /// <inheritdoc/>
        protected override List<byte> ComposeUrOwn()
        {
            uint source = (uint)ScanNumber; //组合为byte数组的uint数据源
            return source.ToBytes().ToList();
        }

        /// <inheritdoc/>
        protected override void ResolveUrOwn(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 4) return;
            if (bytes.Length == 4)
            {
                ScanNumber = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, 0));
                return;
            }
            //CurrScan = new Scan();
            CurrScan.Parse(bytes);
        }
    }

    /// <summary>
    /// GSCN Parameters block. Is transmitted exactly in this order.
    /// </summary>
    public class GSCNParameters
    {
        /// <summary>
        /// 参数1：扫描数
        /// </summary>
        public int PARAMETER_SCAN_NUMBER { get; set; }

        /// <summary>
        /// 参数2：时间戳
        /// </summary>
        public int PARAMETER_TIME_STAMP { get; set; }

        /// <summary>
        /// 参数3：扫描开始角度（单位为milidegree，0.001°，PSXXX-270为45000)
        /// </summary>
        public int PARAMETER_SCAN_START_ANGLE { get; set; }

        /// <summary>
        /// 参数4：扫描区域的角度（单位为milidegree，0.001°，PSXXX-270为270000）
        /// </summary>
        public int PARAMETER_SCAN_ANGLE { get; set; }

        /// <summary>
        /// 参数5：回波数量
        /// </summary>
        public int PARAMETER_NUMBER_OF_ECHOES { get; set; }

        /// <summary>
        /// 参数6：角度增量编码器
        /// </summary>
        public int PARAMETER_INCREMENTAL_ENCODER { get; set; }

        /// <summary>
        /// 参数7：温度（单位为0.1℃）
        /// </summary>
        public int PARAMETER_TEMPERATURE { get; set; }

        /// <summary>
        /// 参数8：系统状态
        /// </summary>
        public int PARAMETER_SYSTEM_STATUS { get; set; }

        /// <summary>
        /// 参数9：数据内容
        /// </summary>
        public int PARAMETER_DATA_CONTENT { get; set; }

        /// <summary>
        /// 参数10：扫描线
        /// </summary>
        public int PARAMETER_SCAN_LINE { get; set; }

        ///// <summary>
        ///// 扫描参数
        ///// </summary>
        //public int NUMBER_OF_SCAN_PARAMETER { get; set; }

        #region 常数
        /// <summary>
        /// data block is empty
        /// </summary>
        public const int NO_DATABLOCK = 0;

        /// <summary>
        /// data block contains distances only
        /// </summary>
        public const int DATABLOCK_WITH_DISTANCES = 4;

        /// <summary>
        /// data block contains distances and pulse witdh
        /// </summary>
        public const int DATABLOCK_WITH_DISTANCES_PW = 8;

        /// <summary>
        /// data block contains distances, and pulse witdh with echoes
        /// </summary>
        public const int DATABLOCK_WITH_DISTANCES_PW_INCLUDES_ECHO = 7;

        /// <summary>
        /// defines the max. number of points per profile
        /// </summary>
        public const int MAX_POINTS_PER_SCAN = 4000;

        /// <summary>
        /// defines the max. number of echoes per point
        /// </summary>
        public const int MAX_NUMBER_OF_ECHOS = 4;
        #endregion
    }

    /// <summary>
    /// Scan data structure.
    /// </summary>
    //[StructLayout(LayoutKind.Sequential)]
    public struct ScanData
    {
        /// <summary>
        /// 距离（单位0.1mm），回波信号过低时的值为-2147483648 (-0x80000000)，过高时的值为2147483647 (0x7FFFFFFF)
        /// </summary>
        public int Distance { get; set; }

        /// <summary>
        /// 回波序号（最多4个回波，序号从0~3）
        /// </summary>
        public int EchoNumber { get; set; }

        /// <summary>
        /// 回波脉冲宽度，单位为皮秒（picoseconds）
        /// </summary>
        public int PulseWidth { get; set; }

        /// <summary>
        /// 重置元素值
        /// </summary>
        public void Clear()
        {
            Distance = 0; 
            EchoNumber = 0;
            PulseWidth = 0;
        }
    }

    /// <summary>
    /// GSCN scan Type definition.
    /// </summary>
    //[StructLayout(LayoutKind.Sequential)]
    public class Scan
    {
        /// <summary>
        /// number of parameter in the parameter array
        /// </summary>
        public int NumberOfParameters { get; set; }

        /// <summary>
        /// 参数列表
        /// </summary>
        public GSCNParameters Parameters { get; set; }

        ///// <summary>
        ///// scan parameter array
        ///// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)GSCNParameters.NUMBER_OF_SCAN_PARAMETER)]
        //public int[] mParameter;

        /// <summary>
        /// number of points in the scan
        /// </summary>
        public int NumberOfPoints { get; set; }

        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime ParsedTime { get; private set; }

        /// <summary>
        /// 已运行时间
        /// </summary>
        public TimeSpan RunningTime { get; private set; }

        ///// <summary>
        ///// 角度分辨率，单位milidegree，0.001°
        ///// </summary>
        //public ushort AngleResolution { get { return (ushort)(NumberOfPoints == 0 ? 90 : Parameters.PARAMETER_SCAN_ANGLE / NumberOfPoints); } }

        /// <summary>
        /// 一个二维数组，1维索引为返回点序号，2维索引为回波组序号（最多4组（索引0~3），一般1组（索引0）），数组中元素为ScanData结构（包含距离、回波组序号、回波脉冲宽度）
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)GSCNParameters.MAX_POINTS_PER_SCAN)]
        public ScanData[,] ScanDatas { get; set; }

        /// <summary>
        /// 默认构造器
        /// </summary>
        public Scan()
        {
            //初始化二维数组与其中元素
            ScanDatas = new ScanData[GSCNParameters.MAX_POINTS_PER_SCAN, GSCNParameters.MAX_NUMBER_OF_ECHOS];
            int dim1 = ScanDatas.GetLength(0), dim2 = ScanDatas.GetLength(1);
            for (int lPoints = 0; lPoints < dim1; lPoints++)
                for (int lEchos = 0; lEchos < dim2; lEchos++)
                    ScanDatas[lPoints, lEchos] = new ScanData();
            Clear();
        }

        /// <summary>
        /// 清空结构内容
        /// </summary>
        public void Clear()
        {
            NumberOfParameters = 0;
            Parameters = new GSCNParameters();
            NumberOfPoints = 0;
            foreach (var data in ScanDatas)
                data.Clear();
        }

        /// <summary>
        /// Parses the receiver buffer and copy the result into the scan structure.
        /// Must be called after performCommand().
        /// </summary>
        /// <param name="bytes">待转换的字节流</param>
        /// <returns>ERR_SUCCESS on success, otherwise a negative error code.</returns>
        public ErrorID Parse(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 48)
                throw new ArgumentException("提供的字节流为为空或长度过短，无法解析", nameof(bytes));
            int index = 0;
            //NumberOfParameters = bytes.ToInt32();
            int lNumberOfParameter = bytes.ToInt32(index++ * 4); //0
            // check compatibility of firmware and control program
            NumberOfParameters = lNumberOfParameter >= 10 ? 10 : lNumberOfParameter;
            Parameters.PARAMETER_SCAN_NUMBER = bytes.ToInt32(index++ * 4);
            Parameters.PARAMETER_TIME_STAMP = bytes.ToInt32(index++ * 4);
            RunningTime = new TimeSpan(0, 0, 0, 0, Parameters.PARAMETER_TIME_STAMP);
            Parameters.PARAMETER_SCAN_START_ANGLE = bytes.ToInt32(index++ * 4);
            Parameters.PARAMETER_SCAN_ANGLE = bytes.ToInt32(index++ * 4);
            Parameters.PARAMETER_NUMBER_OF_ECHOES = bytes.ToInt32(index++ * 4);
            Parameters.PARAMETER_INCREMENTAL_ENCODER = bytes.ToInt32(index++ * 4);
            Parameters.PARAMETER_TEMPERATURE = bytes.ToInt32(index++ * 4);
            Parameters.PARAMETER_SYSTEM_STATUS = bytes.ToInt32(index++ * 4);
            Parameters.PARAMETER_DATA_CONTENT = bytes.ToInt32(index++ * 4);
            if (NumberOfParameters >= 10)
                Parameters.PARAMETER_SCAN_LINE = bytes.ToInt32(index++ * 4);
            NumberOfPoints = bytes.ToInt32(index++ * 4); //44

            // get number of echoes. If 0, then the master echo is transfered instead of the number
            int lNumberOfEchoes = Parameters.PARAMETER_NUMBER_OF_ECHOES == 0 ? 1 : Parameters.PARAMETER_NUMBER_OF_ECHOES;
            // check limits of number of points
            if (lNumberOfEchoes > GSCNParameters.MAX_NUMBER_OF_ECHOS || NumberOfPoints > GSCNParameters.MAX_POINTS_PER_SCAN)
            {
                Clear();
                return ErrorID.ERR_BUFFER_OVERFLOW;
            }

            // copy data block according to the data content.
            switch (Parameters.PARAMETER_DATA_CONTENT)
            {
                // no data available
                case GSCNParameters.NO_DATABLOCK:
                    break;
                // copy distances only
                // loop through all points to copy distances and pulse width
                case GSCNParameters.DATABLOCK_WITH_DISTANCES:
                    for (int lPoints = 0; lPoints < NumberOfPoints; lPoints++)
                        // loop for each point through all echos
                        for (int lEchos = 0; lEchos < lNumberOfEchoes; lEchos++)
                            ScanDatas[lPoints, lEchos].Distance = bytes.ToInt32(index++ * 4);
                    break;
                // default: distance and pulse width. If the echo number is included, we remove it.
                case GSCNParameters.DATABLOCK_WITH_DISTANCES_PW:
                case GSCNParameters.DATABLOCK_WITH_DISTANCES_PW_INCLUDES_ECHO:
                default:
                    for (int lPoints = 0; lPoints < NumberOfPoints; lPoints++)
                    {
                        // loop for each point through all echos
                        for (int lEchos = 0; lEchos < lNumberOfEchoes; lEchos++)
                        {
                            ScanDatas[lPoints, lEchos].Distance = bytes.ToInt32(index++ * 4);
                            //获取回波值（仅在PARAMETER_DATA_CONTENT为7时有意义）
                            //ScanDatas[lPoints, lEchos].EchoNumber = bytes[index * 4];
                            ScanDatas[lPoints, lEchos].EchoNumber = lEchos;
                            //将回波值的索引位置元素赋值为0防止干扰脉冲宽度的计算，假如需要回波值则在此步前保存
                            bytes[index * 4] = 0;
                            ScanDatas[lPoints, lEchos].PulseWidth = bytes.ToInt32(index++ * 4);
                        } // endechos
                    } // end points
                    break;
            }

            ParsedTime = DateTime.Now.AddMilliseconds(0);
            return ErrorID.ERR_NONE;
        }

        //这个函数的作用是将接收缓冲区中的数据解析并复制到扫描结构中。它首先设置一个指向整数的指针，然后跳过命令ID和长度。接下来，它获取参数的数量并检查固件和控制程序的兼容性。然后，它将已知参数复制到扫描结构中，并跳过未知参数。接着，它获取回波的数量，如果为0，则传输主回波而不是数量。然后，它获取点数并检查限制。最后，它根据数据内容复制数据块。如果数据内容为NO_DATABLOCK，则没有数据可用。如果数据内容为DATABLOCK_WITH_DISTANCES，则只复制距离。如果数据内容为DATABLOCK_WITH_DISTANCES_PW或DATABLOCK_WITH_DISTANCES_PW_INCLUDES_ECHO，则复制距离和脉冲宽度。如果数据内容为DATABLOCK_WITH_DISTANCES_PW_INCLUDES_ECHO，则需要删除回波编号。最后，它返回错误ID。
    }
}