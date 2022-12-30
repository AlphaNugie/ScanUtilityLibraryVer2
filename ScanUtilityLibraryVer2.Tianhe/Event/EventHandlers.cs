using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtilityLibrary.Event
{
    /// <summary>
    /// 各事件的委托
    /// </summary>
    public class EventHandlers
    {
        /// <summary>
        /// 登录成功事件的委托
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <param name="sender"></param>
        public delegate void LoggedInEventHandler(object sender, EventArgs eventArgs);
    }
}
