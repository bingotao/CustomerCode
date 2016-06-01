using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bingotao.Customer.BaseLib
{
    public class SystemParameters
    {
        /// <summary>        /// 线程锁定对象        /// </summary>        static readonly object _lockObject = new object();

        /// <summary>        /// 静态实例对象        /// </summary>        private static dynamic _Instance;
        
        /// <summary>        /// 系统配置文件位置        /// </summary>        static string configPath = ConfigurationManager.AppSettings["ConfigPath"];
        /// <summary>        /// 静态实例属性        /// </summary>        public static dynamic Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (_lockObject)
                    {
                        StreamReader sr = new StreamReader(configPath);
                        string json = sr.ReadToEnd();
                        _Instance = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                    }
                }
                return _Instance;
            }
        }
    }

}
