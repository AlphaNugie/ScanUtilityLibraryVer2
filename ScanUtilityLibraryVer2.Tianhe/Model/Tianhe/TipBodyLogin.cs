using CommonLib.Function;
using ScanUtilityLibrary.Core.Tianhe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Model.Tianhe
{
    /// <summary>
    /// TIP协议数据体：登录
    /// </summary>
    public class TipBodyLogin : TipBodyBase
    {
        /// <summary>
        /// 默认用户名
        /// </summary>
        public const string DEFAULT_USERNAME = "gjdt1d";

        /// <summary>
        /// 默认密码
        /// </summary>
        public const string DEFAULT_PASSWORD = "gc343j46";

        #region 属性
        /// <summary>
        /// 协议数据头
        /// </summary>
        public new TipHeadCommon TipHead { get; set; }

        private string _userName = string.Empty;
        /// <summary>
        /// 用户名（不超过64个字符）
        /// </summary>
        public string UserName
        {
            get { return _userName; }
            internal set
            {
                value = string.IsNullOrWhiteSpace(value) ? string.Empty : value;
                if (value.Length > 64)
                    value = value.Substring(0, 64);
                _userName = value;
            }
        }

        private string _password = string.Empty;
        /// <summary>
        /// 密码（不超过64个字符）
        /// </summary>
        public string Password
        {
            get { return _password; }
            internal set
            {
                value = string.IsNullOrWhiteSpace(value) ? string.Empty : value;
                if (value.Length > 64)
                    value = value.Substring(0, 64);
                _password = value;
            }
        }
        #endregion

        #region 构造器
        /// <summary>
        /// 默认构造器
        /// </summary>
        public TipBodyLogin() : base(string.Empty) { }

        /// <summary>
        /// 用给定的16进制字符串初始化
        /// </summary>
        /// <param name="hexString"></param>
        public TipBodyLogin(string hexString) : base(hexString) { }

        /// <summary>
        /// 用给定的byte数组初始化
        /// </summary>
        /// <param name="bytes"></param>
        public TipBodyLogin(byte[] bytes) : base(bytes) { }
        #endregion

        /// <summary>
        /// 更新用户名密码
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public void UpdateUserInfos(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        /// <summary>
        /// 恢复到默认的用户名密码
        /// </summary>
        public void RestoreDefUserInfos()
        {
            UpdateUserInfos(DEFAULT_USERNAME, DEFAULT_PASSWORD);
            //UserName = DEFAULT_USERNAME;
            //Password = DEFAULT_PASSWORD;
        }

        #region 抽象方法实现
        /// <inheritdoc/>
        protected override List<byte> ComposeUrOwn()
        {
            byte[] nameBytes = Encoding.ASCII.GetBytes(UserName), passBytes = Encoding.ASCII.GetBytes(Password);
            List<byte> bytes = new List<byte>();
            //添加由用户名编码而成的字节数组，假如数组长度不足64则补充0直至达到64个字节长度
            bytes.AddRange(nameBytes);
            for (int i = 0; i < 64 - UserName.Length; i++)
                bytes.Add(0);
            //添加由密码编码而成的字节数组，假如数组长度不足64则补充0直至达到64个字节长度
            bytes.AddRange(passBytes);
            for (int i = 0; i < 64 - Password.Length; i++)
                bytes.Add(0);
            return bytes;
        }

        /// <inheritdoc/>
        protected override byte[] GetTipHeadBytes()
        {
            return TipHead.Compose();
        }

        /// <inheritdoc/>
        protected override void InitTipHead(byte[] bytes)
        {
            TipHead = new TipHeadCommon(bytes);
        }

        /// <inheritdoc/>
        protected override void ResolveUrOwn(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 168)
                return;
            byte[] nameBytes = bytes.Skip(40).Take(64).ToArray(), passBytes = bytes.Skip(104).Take(64).ToArray();
            //字节序列解码为字符串并移除末尾的空字符
            string userName = Encoding.ASCII.GetString(nameBytes).TrimEnd('\0'), password = Encoding.ASCII.GetString(passBytes).TrimEnd('\0');
            UpdateUserInfos(userName, password);
        }

        /// <inheritdoc/>
        protected override void UpdateAllBytesLen_Composing(uint abtLen)
        {
            TipHead.UpdateAllBytesLen(abtLen);
        }

        /// <inheritdoc/>
        protected override void UpdateTipCode_Composing(TipCode code = TipCode.None)
        {
            TipHead.UpdateTipCode(TipCode.Login);
        }
        #endregion
    }
}
