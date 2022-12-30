using CommonLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Core.Tianhe
{
    /// <summary>
    /// 报文数据流通方向
    /// </summary>
    public enum StreamDirection
    {
        /// <summary>
        /// 向设备发送
        /// </summary>
        ToDevice = 0,

        /// <summary>
        /// 从设备接收
        /// </summary>
        FromDevice = 1,
    }

    /// <summary>
    /// TIP包功能代码
    /// </summary>
    public enum TipCode
    {
        /// <summary>
        /// 不代表任何功能
        /// </summary>
        None = 0,

        /// <summary>
        /// 心跳发送
        /// </summary>
        Heartbeat = 1,

        /// <summary>
        /// 心跳回复
        /// </summary>
        HeartbeatReply = 2,

        /// <summary>
        /// 数据输出
        /// </summary>
        DataOutput = 110,

        /// <summary>
        /// 数据输出（3维）
        /// </summary>
        DataOutput3d = 118,

        /// <summary>
        /// 登录
        /// </summary>
        Login = 3050,

        /// <summary>
        /// 登录回复
        /// </summary>
        LoginReply = 3051,

        /// <summary>
        /// 查询设备工作状态
        /// </summary>
        RequestDevState = 3730,

        /// <summary>
        /// 查询设备工作状态的回复
        /// </summary>
        RequestDevStateReply = 3731,

        /// <summary>
        /// 重新启动
        /// </summary>
        Reboot = 4530,

        /// <summary>
        /// 设置时间（登录后）
        /// </summary>
        SetTime = 6028,

        /// <summary>
        /// 设置时间的回复
        /// </summary>
        SetTimeReply = 6029,

        /// <summary>
        /// 查询设备信息
        /// </summary>
        RequestDevInfo = 7000,

        /// <summary>
        /// 查询设备信息的回复
        /// </summary>
        RequestDevInfoReply = 7001,

        /// <summary>
        /// 处于遮挡状态
        /// </summary>
        Blocked = 7105,

        /// <summary>
        /// 从被遮挡状态中恢复
        /// </summary>
        BlockedNoMore = 7106,

        /// <summary>
        /// 处于故障状态
        /// </summary>
        Error = 7109,

        /// <summary>
        /// 从故障状态恢复
        /// </summary>
        ErrorNoMore = 7110,

        /// <summary>
        /// 获取设备标准输出数据格式
        /// </summary>
        RequestDataFormat = 7129,

        /// <summary>
        /// 获取设备标准输出数据格式的回复
        /// </summary>
        RequsetDataFormatReply = 7130,

        /// <summary>
        /// 获取雷达遥信状态
        /// </summary>
        RequsetDevSgnlState = 7131,

        /// <summary>
        /// 获取雷达遥信状态的回复
        /// </summary>
        RequsetDevSgnlStateReply = 7132,

        /// <summary>
        /// 获取雷达遥控状态
        /// </summary>
        RequsetDevCtrlState = 7133,

        /// <summary>
        /// 获取雷达遥控状态的回复
        /// </summary>
        RequsetDevCtrlStateReply = 7134,

        /// <summary>
        /// 控制雷达IO
        /// </summary>
        SetDevIO = 7135,

        /// <summary>
        /// 控制雷达IO的回复
        /// </summary>
        SetDevIOReply = 7136,

        /// <summary>
        /// 查询设备参数
        /// </summary>
        RequestDevParams = 7137,

        /// <summary>
        /// 查询设备参数的回复
        /// </summary>
        RequestDevParamsReply = 7138,

        /// <summary>
        /// 设置雷达启动/待机
        /// </summary>
        SetPowerState = 7139,

        /// <summary>
        /// 雷达启动/待机的回复
        /// </summary>
        SetPowerStateReply = 7140,

        /// <summary>
        /// 处于脏污状态
        /// </summary>
        Contaminated = 7147,

        /// <summary>
        /// 从脏污状态中恢复
        /// </summary>
        ContaminatedNoMore = 7146,
    }

    /// <summary>
    /// 包类型
    /// </summary>
    public enum PackType
    {
        /// <summary>
        /// 
        /// </summary>
        Downstream_all = 0x0,

        /// <summary>
        /// 
        /// </summary>
        Downstream_up = 0x1,

        /// <summary>
        /// 
        /// </summary>
        Downstream_down = 0x2,

        /// <summary>
        /// 
        /// </summary>
        Upstream_all = 0x80,

        /// <summary>
        /// 
        /// </summary>
        Upstream_up = 0x81,

        /// <summary>
        /// 
        /// </summary>
        Upstream_down = 0x82,
    }

    /// <summary>
    /// 时间戳类型（DataFormatMap）
    /// </summary>
    public enum TimeStampType
    {
        /// <summary>
        /// 相对时间
        /// </summary>
        Relative = 0,

        /// <summary>
        /// 日历时间
        /// </summary>
        Calendar = 1
    }

    /// <summary>
    /// 坐标系类型（DataFormatMap）
    /// </summary>
    public enum CoordSystemType
    {
        /// <summary>
        /// 直角坐标系（笛卡尔坐标系）
        /// </summary>
        Cartesian = 0,
    }

    /// <summary>
    /// 角度分辨率类型（DataFormatMap）
    /// </summary>
    public enum AngleResolutionType
    {
        /// <summary>
        /// 毫度
        /// </summary>
        [EnumDescription("毫度")]
        MilliDeg = 0,

        /// <summary>
        /// 0.01毫度
        /// </summary>
        [EnumDescription("0.01毫度")]
        MilliDegDivBy100 = 1,

        /// <summary>
        /// 一圈360°内点的总数（使用360除以这个值得到最终角分辨率）
        /// </summary>
        [EnumDescription("一圈360°内点的总数")]
        TotalPoints360 = 3
    }

    /// <summary>
    /// 测量的距离单位（DataFormatMap）
    /// </summary>
    public enum MeasUnitType
    {
        /// <summary>
        /// 2字节厘米
        /// </summary>
        [EnumDescription("2字节厘米")]
        Centi_2bytes = 0,

        /// <summary>
        /// 2字节毫米
        /// </summary>
        [EnumDescription("2字节毫米")]
        Milli_2bytes = 1,

        /// <summary>
        /// 4字节厘米
        /// </summary>
        [EnumDescription("4字节厘米")]
        Centi_4bytes = 6,

        /// <summary>
        /// 4字节毫米
        /// </summary>
        [EnumDescription("4字节毫米")]
        Milli_4bytes = 7,
    }

    /// <summary>
    /// 回波脉冲类型（DataFormatMap）
    /// </summary>
    public enum EchoPulseType
    {
        /// <summary>
        /// 最远脉冲
        /// </summary>
        [EnumDescription("最远脉冲")]
        Farthest = 0,

        /// <summary>
        /// 最强脉冲
        /// </summary>
        [EnumDescription("最强脉冲")]
        Strongest = 1,

        /// <summary>
        /// 双脉冲
        /// </summary>
        [EnumDescription("双脉冲")]
        Both = 2,
    }
}
