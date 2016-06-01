using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bingotao.Customer.GIS.Feature
{
    public class FeatureUtilities
    {
        /// <summary>
        /// 获取字段的索引号
        /// </summary>
        /// <param name="ftCls"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static int GetFieldIndex(IFeatureClass ftCls, string field)
        {
            int index = ftCls.FindField(field);
            ValidateFieldIndex(field, index);
            return index;
        }


        /// <summary>
        /// 获取字段的索引号
        /// </summary>
        /// <param name="ft"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static int GetFieldIndex(IFeature ft, string field)
        {
            int index = ft.Fields.FindField(field);
            ValidateFieldIndex(field, index);
            return index;
        }

        /// <summary>
        /// 获取要素的属性
        /// </summary>
        /// <param name="ft"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static object GetValue(IFeature ft, string field)
        {
            int index = ft.Fields.FindField(field);
            return ft.Value[index];
        }

        /// <summary>
        /// 获取要素的属性（添加验证字段是否存在）
        /// </summary>
        /// <param name="ft"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static object GetValue1(IFeature ft, string field)
        {
            int index = GetFieldIndex(ft, field);
            return ft.Value[index];
        }


        private static void ValidateFieldIndex(string field, int index)
        {
            if (string.IsNullOrEmpty(field) || index < 0)
            {
                throw new Exception(string.Format("未找到名称为“{0}”的字段！", field));
            }
        }
    }
}
