﻿using System;
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
        /// <summary>

        /// <summary>
        
        /// <summary>

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