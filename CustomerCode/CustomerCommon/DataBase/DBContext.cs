using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bingotao.Customer.BaseLib
{
    public class DBContext
    {
        private OracleDataAdapter DBAdapter = null;
        private string connectionString = null;

        public DBContext()
        {
            //需要传入ConnectionString
            //this.connectionString = WK_SystemParameter.Instance.GYYD_DB;
        }

        public DBContext(string sConnectionString)
        {
            this.connectionString = sConnectionString;
        }

        public DataTable ExecuteQuery(string selectSQL)
        {
            if (this.DBAdapter != null)
            {
                this.DBAdapter.SelectCommand.CommandText = selectSQL;
            }
            else {
                this.DBAdapter = new OracleDataAdapter(selectSQL, this.connectionString);
            }
            DataTable dt = new DataTable();
            this.DBAdapter.Fill(dt);
            return dt;
        }

        /// <summary>
        /// 获取分页的DataTable
        /// </summary>
        /// <param name="selectSQL"></param>
        /// <param name="startRecordNum">从第几条数据开始取 从1开始编号</param>
        /// <param name="recordCount">获取的行数</param>
        /// <returns></returns>
        public DataTable ExecuteQuery(string selectSQL, int startRecordNum, int recordCount)
        {
            if (this.DBAdapter != null)
            {
                this.DBAdapter.SelectCommand.CommandText = selectSQL;
            }
            else {
                this.DBAdapter = new OracleDataAdapter(selectSQL, this.connectionString);
            }
            DataTable dt = new DataTable();
            this.DBAdapter.Fill(startRecordNum - 1, recordCount, dt);
            return dt;
        }


        private static DBContext mInstance = null;
        //线程锁定辅助对象
        private static readonly object lockAssistant = new object();
        /// <summary>
        /// 单例
        /// </summary>
        public static DBContext Instance
        {
            get
            {
                if (mInstance == null)
                {
                    lock (lockAssistant)
                    {
                        mInstance = new DBContext();
                    }
                }
                return mInstance;
            }
        }
    }
}
