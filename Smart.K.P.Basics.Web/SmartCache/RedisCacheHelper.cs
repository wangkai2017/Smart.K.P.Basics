using SmartCommon;
using SmartRedis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCache
{
    public class RedisCacheHelper
    {
        private StringOperator strOper;
        private ListOperator listOper;

        public RedisCacheHelper()
        {
            strOper = new StringOperator();
            listOper = new ListOperator();
        }
        #region string类型

        #region 赋值
        /// <summary>
        /// 设置key的value
        /// </summary>
        public bool Set(string key, string value)
        {
            return strOper.Set(key, value);
        }
        /// <summary>
        /// 设置key的value并设置过期时间
        /// </summary>
        public bool Set(string key, string value, DateTime dt)
        {
            return strOper.Set(key, value, dt);
        }
        /// <summary>
        /// 设置key的value并设置过期时间
        /// </summary>
        public bool Set(string key, string value, TimeSpan sp)
        {
            return strOper.Set(key, value, sp);
        }
        /// <summary>
        /// 设置多个key/value
        /// </summary>
        public void Set(Dictionary<string, string> dic)
        {
            strOper.Set(dic);
        }

        #endregion
        #region 获取值
        /// <summary>
        /// 获取key的value值
        /// </summary>
        public string Get(string key)
        {
            return strOper.Get(key);
        }
        /// <summary>
        /// 获取多个key的value值
        /// </summary>
        public List<string> Get(List<string> keys)
        {
            return strOper.Get(keys);
        }
        /// <summary>
        /// 获取多个key的value值
        /// </summary>
        public List<T> Get<T>(List<string> keys)
        {
            return strOper.Get<T>(keys);
        }
        #endregion
        #region 移除
        /// <summary>
        /// 移除值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            return strOper.Remove(key);
        }
        #endregion
        #endregion
    }
}
