using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bingotao.Customer.BaseLib
{
    public class TypeConvert
    {
        public static object ConvertType(object value, Type type)
        {
            //为空，并且类型为内置类型返回默认值
            if (value == null)
            {
                //内置类型返回默认值，引用类型返回空
                return type.IsPrimitive ? Activator.CreateInstance(type) : null;
            }

            //目标类型和原始类型相同
            if (type == value.GetType()) return value;

            //目标类型为枚举类型
            if (type.IsEnum)
            {
                if (value is string)
                    return Enum.Parse(type, value as string);
                else
                    return Enum.ToObject(type, value);
            }

            if (!type.IsInterface && type.IsGenericType)
            {
                Type innerTypeTo = type.GetGenericArguments()[0];
                object innerValue = ConvertType(value, innerTypeTo);
                return Activator.CreateInstance(type, innerValue);
            }
            if (value is string && type == typeof(Guid)) return new Guid(value as string);
            if (value is string && type == typeof(Version)) return new Version(value as string);
            if (!(value is IConvertible)) return value;
            return Convert.ChangeType(value, type);
        }
    }
}
