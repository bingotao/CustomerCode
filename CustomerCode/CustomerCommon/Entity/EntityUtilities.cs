using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bingotao.Customer.BaseLib.Entity
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

    }
}
