using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bingotao.Customer.BaseLib
{
    public static class EntityUtilities
    {
        public static T DictionaryToEntiity<T>(Dictionary<string, object> dict) where T : class, new()
        {
            T entity = null;
            if (dict != null)
            {
                entity = new T();

                Type type = typeof(T);
                PropertyInfo[] properties = type.GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    string propertyName = property.Name;
                    object value = dict[propertyName];
                    if (dict.Keys.Contains(propertyName) && value.GetType().Equals(property.PropertyType))
                    {
                        property.SetValue(entity, value);
                    }
                }
            }

            return entity;
        }


        /// <summary>
        /// DataRow转换为指定的实体类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static T DataRowToEntity<T>(DataRow dr) where T : new()
        {
            return CreateEntity<T>(dr, GetSetProperties<T>());
        }

        /// <summary>
        /// 获取类型具有set方法的属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static PropertyInfo[] GetSetProperties<T>() where T : new()
        {
            Type type = typeof(T);
            return (from p in type.GetProperties()
                    where p.GetSetMethod() != null
                    select p).ToArray();
        }

        /// <summary>
        /// 根据DataRow以及指定的属性创建实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <param name="props"></param>
        /// <returns></returns>
        private static T CreateEntity<T>(DataRow dr, PropertyInfo[] props) where T : new()
        {
            T entity = new T();
            foreach (DataColumn dc in dr.Table.Columns)
            {
                var colName = dc.ColumnName;
                var prop = props.Where(p => p.Name == colName).FirstOrDefault();
                if (prop != null)
                {
                    var pType = prop.PropertyType;
                    var value = dr[colName];
                    var valueNew = DataConvert.ChangeType(value, pType);
                    prop.SetValue(entity, valueNew);
                }
            }
            return entity;
        }

        /// <summary>
        /// 将DataTable转化为实体List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> DataTableToList<T>(DataTable dt) where T : new()
        {
            List<T> list = new List<T>();
            PropertyInfo[] props = GetSetProperties<T>();

            foreach (DataRow dr in dt.Rows)
            {
                list.Add(CreateEntity<T>(dr, props));
            }
            return list;
        }
    }
}
