using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bingotao.Customer.GIS.Releaser
{
    /// <summary>
    /// GIS com 组件释放器
    /// </summary>
    public class AEComReleaser
    {
        /// <summary>
        /// 释放GIS资源
        /// </summary>
        /// <param name="coms"></param>
        public static void Release(params object[] coms)
        {
            if (coms != null)
            {
                int count = coms.Length;
                for (int i = 0; i < count; i++)
                {
                    object com = coms[i];
                    if (com != null)
                    {
                        ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(com);
                        com = null;
                    }
                }
            }
        }
    }
}
