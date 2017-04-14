using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCommon.Enums
{
    /// <summary>
    /// 缓存类型
    /// </summary>
    public enum CacheEnum
    {
        [Description("本地缓存")]
        Local = 0,
        [Description("Redis缓存")]
        Redis
    }
}
