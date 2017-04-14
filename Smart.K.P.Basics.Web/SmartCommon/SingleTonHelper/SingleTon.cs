using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCommon.SingleTonHelper
{
    public class SingleTon<T> where T : class, new()
    {
        protected static T _instance;
        private static readonly object _lockObj;

        static SingleTon()
        {
            _lockObj = new object();
        }

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            //如果是引用类型创建一个T实例，如果是值类型返回值的默认值  
                            _instance = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
                        }
                    }
                }

                return _instance;
            }
        }
    }
}
