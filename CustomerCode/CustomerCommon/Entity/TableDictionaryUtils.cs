using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bingotao.Customer.BaseLib.Entity
{
    public static class TableDictionaryUtils
    {
        /// <summary>
        /// DataRow 转换为 Dictionary
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static Dictionary<string, object> DataRowToDict(DataRow dr)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            if (dr != null)
            {
                DataTable dt = dr.Table;
                foreach (DataColumn dc in dt.Columns)
                {
                    string colName = dc.ColumnName;
                    object value = dr[colName];
                    if (value == DBNull.Value)
                    {
                        value = TypeConvert.ConvertType(value, dc.DataType);
                    }
                    dict[colName] = value;
                }
            }
            return dict;
        }

        /// <summary>
        /// 将DataTable的第一行转换为 Dictionary。如果第一行为空，则返回空 Dictionary。
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static Dictionary<string, object> DataTableToDict(DataTable dt)
        {
            return DataRowToDict(dt == null ? null : (dt.Rows.Count > 0 ? dt.Rows[0] : null));
        }

        /// <summary>
        /// DataTable 转 List
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<Dictionary<string, object>> DataTableToList(DataTable dt)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Dictionary<string, object> dic = DataRowToDict(dr);
                    list.Add(dic);
                }
            }
            return list;
        }

        /// <summary>
        /// DataTable 转 List
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="startIndex">起始索引行数 从1开始</param>
        /// <param name="count">总个数</param>
        /// <returns></returns>
        public static List<Dictionary<string, object>> DataTableToList(DataTable dt, int startIndex, int count)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            if (dt != null)
            {
                int rowsCount = dt.Rows.Count;
                int endIndex = startIndex + count - 1;
                for (int i = startIndex - 1; i < endIndex && i < rowsCount; i++)
                {
                    DataRow dr = dt.Rows[i];
                    Dictionary<string, object> dic = DataRowToDict(dr);
                    list.Add(dic);
                }
            }
            return list;
        }
    }
}