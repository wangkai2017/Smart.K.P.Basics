using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartRedis
{
    public class RedisManager
    {
        //// <summary>
         /// redis配置文件信息
         /// </summary>
        private static RedisConfigInfo RedisConfigInfo = RedisConfigInfo.GetConfig();

        private static PooledRedisClientManager _prcm;

        /// <summary>  
        /// 静态构造方法，初始化链接池管理对象  
        /// </summary>  
        static RedisManager()
        {
            CreateManager();            
        }

        /// <summary>  
        /// 创建链接池管理对象  
        /// </summary>  
        private static void CreateManager()
        {
            var writeServerList = SplitString(RedisConfigInfo.WriteServerList, ",");
            var readServerList = SplitString(RedisConfigInfo.ReadServerList, ",");

            _prcm = new PooledRedisClientManager(writeServerList, readServerList,
                             new RedisClientManagerConfig
                             {
                                 MaxWritePoolSize = RedisConfigInfo.MaxWritePoolSize,
                                 MaxReadPoolSize = RedisConfigInfo.MaxReadPoolSize,
                                 AutoStart = RedisConfigInfo.AutoStart,                                 
                             });
            
        }

        private static IEnumerable<string> SplitString(string strSource, string split)
        {
            return strSource.Split(split.ToArray());
        }

        /// <summary>  
        /// 客户端缓存操作对象  
        /// </summary>  
        public static IRedisClient GetClient()
        {
            IRedisClient client;
            if (_prcm == null)
            {
                CreateManager();
            }
            //返回一个读/写的客户端（默认）使用定义在ReadWriteHosts主机
            client = _prcm.GetClient();
            //认证密码
            client.Password = RedisConfigInfo.Password;
            return client;
        }

    }
}
