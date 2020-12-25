using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MySQL_Shop
{
    public class MySQLHelper
    {
        private MySqlConnection conn = null;

        public MySQLHelper(string connString)
        {
            conn = new MySqlConnection(connString);
            conn.Open();
        }
        /// <summary>
        /// 执行一条sql，返回影响的行数
        /// </summary>
        /// <param name="commandText">sql语句</param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public int Execute(string commandText, params MySqlParameter[] commandParameters)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = commandText;
            cmd.CommandType = CommandType.Text;
            if (commandParameters != null)
            {
                foreach (MySqlParameter parm in commandParameters)
                    cmd.Parameters.Add(parm);
            }
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 返回DataSet
        /// </summary>
        /// <param name="cmdText">存储过程名称或者sql命令语句</param>
        /// <param name="commandParameters">执行命令所用参数的集合</param>
        /// <returns></returns>
        public DataSet GetDataSet(string commandText, params MySqlParameter[] commandParameters)
        {
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = commandText;
            cmd.CommandType = CommandType.Text;
            adapter.SelectCommand = cmd;
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            cmd.Parameters.Clear();
            return ds;
        }

        public object GetData(string commandText, params MySqlParameter[] commandParameters)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = commandText;
            object result = null;
            try
            {
                result = (object)cmd.ExecuteScalar();
            }
            catch (Exception e)
            {
                throw new Exception(commandText + "\n\n" + e.ToString()); ;
            }
            return result;
        }

        public void Dispose()
        {
            conn.Close();
        }
    }
}
