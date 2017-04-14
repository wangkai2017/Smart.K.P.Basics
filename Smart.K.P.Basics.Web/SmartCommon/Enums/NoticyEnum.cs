using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCommon.Enums
{
    /// <summary>
    /// 消息通知的方式None = 0,Mail = 1
    /// </summary>
    public enum NoticyEnum
    {
        /// <summary>
        /// 不发送
        /// </summary>
        [Description("不发送")]
        None = 0,
        /// <summary>
        /// 发送邮件
        /// </summary>
        [Description("发送邮件")]
        Mail = 1
    }
    public enum LevelEnum
    {
        /// <summary>
        /// 一般
        /// </summary>
        [Description("一般")]
        Info = 0,
        /// <summary>
        /// 警告
        /// </summary>
        [Description("警告")]
        Warnning = 1,
        /// <summary>
        /// 错误
        /// </summary>
        [Description("错误")]
        Error = 2
    }
}
