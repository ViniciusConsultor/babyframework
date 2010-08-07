using System.Data;
using System.Data.OleDb;
using System.Configuration;

namespace Baby.Framework.DALProfile
{
    /// <summary>
    ///AccessHelper 的摘要说明
    /// </summary>
    public class AccessHelper
    {
        public AccessHelper()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        /// <summary>
        /// 提供后台管理时的Access数据库连接字符串
        /// </summary>
        /// <returns>后台管理时的Access数据库连接字符串</returns>
        public static string GetManageDBConnectionStr()
        {
            string sConn = ConfigurationManager.AppSettings["AccessConnString"].ToString() + System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["manage_dbPath"].ToString()) + ";";
            return sConn;
        }

        /// <summary>
        /// 提供前台的Access数据库连接字符串
        /// </summary>
        /// <returns>前台的Access数据库连接字符串</returns>
        public static string GetDBConnectionStr()
        {
            string sConn = ConfigurationManager.AppSettings["AccessConnString"].ToString() + System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["dbPath"].ToString()) + ";";
            return sConn;
        }

        /// <summary>
        /// 预处理OledbCommand
        /// </summary>
        /// <param name="oledbCmd">欲处理的OledbCommand</param>
        /// <param name="oledbConn">oledbConnection</param>
        /// <param name="oledbTrans">Transaction</param>
        /// <param name="cmdType">command类型（存储过程或一般sql操作）</param>
        /// <param name="cmdText">sql字符串</param>
        /// <param name="oledbParams">oledbParams参数</param>
        private static void PrepareCommand(OleDbCommand oledbCmd, OleDbConnection oledbConn, OleDbTransaction oledbTrans, CommandType cmdType, string cmdText, OleDbParameter[] oledbParams)
        {
            if (oledbConn.State != ConnectionState.Open)
            {
                oledbConn.Open();
            }

            oledbCmd.Connection = oledbConn;
            oledbCmd.CommandText = cmdText;

            if (oledbTrans != null)
            {
                oledbCmd.Transaction = oledbTrans;
            }

            oledbCmd.CommandType = cmdType;

            if (oledbParams != null)
            {
                foreach (OleDbParameter param in oledbParams)
                    oledbCmd.Parameters.Add(param);
            }
        }

        public static int ExecuteNonQuery(CommandType cmdType, string cmdText, params OleDbParameter[] oledbParams)
        {
            return ExecuteNonQuery(GetManageDBConnectionStr(), cmdType, cmdText, oledbParams);
        }

        public static int ExecuteNonQueryForProcenium(CommandType cmdType, string cmdText, params OleDbParameter[] oledbParams)
        {
            return ExecuteNonQuery(GetDBConnectionStr(), cmdType, cmdText, oledbParams);
        }

        /// <summary>
        /// Execute 一条 sql语句
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="cmdType">cmdType</param>
        /// <param name="cmdText">执行的sql语句</param>
        /// <param name="oledbParams">参数</param>
        /// <returns>0失败,大于0成功</returns>
        public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params OleDbParameter[] oledbParams)
        {
            OleDbCommand oledbCmd = new OleDbCommand();

            using (OleDbConnection oledbConn = new OleDbConnection(connectionString))
            {
                PrepareCommand(oledbCmd, oledbConn, null, cmdType, cmdText, oledbParams);
                int val = oledbCmd.ExecuteNonQuery();
                oledbCmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// Execute 一条 sql语句
        /// </summary>
        /// <remarks>
        ///  e.g.:  
        ///  int result = ExecuteNonQuery(connection, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="oledbConn">oledbConnection对象</param>
        /// <param name="cmdType">cmdType</param>
        /// <param name="cmdText">执行的sql语句</param>
        /// <param name="oledbParams">参数</param>
        /// <returns>0失败,大于0成功</returns>
        public static int ExecuteNonQuery(OleDbConnection oledbConn, CommandType cmdType, string cmdText, params OleDbParameter[] oledbParams)
        {
            OleDbCommand oledbCmd = new OleDbCommand();

            PrepareCommand(oledbCmd, oledbConn, null, cmdType, cmdText, oledbParams);
            int val = oledbCmd.ExecuteNonQuery();
            oledbCmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// Execute 一条 sql语句
        /// </summary>
        /// <param name="oledbTrans">OleDbTransaction对象</param>
        /// <param name="cmdType">cmdType</param>
        /// <param name="cmdText">执行的sql语句</param>
        /// <param name="oledbParams">参数</param>
        /// <returns>0失败,大于0成功</returns>
        public static int ExecuteNonQuery(OleDbTransaction oledbTrans, CommandType cmdType, string cmdText, params OleDbParameter[] oledbParams)
        {
            OleDbCommand oledbCmd = new OleDbCommand();

            PrepareCommand(oledbCmd, oledbTrans.Connection, oledbTrans, cmdType, cmdText, oledbParams);
            int val = oledbCmd.ExecuteNonQuery();
            oledbCmd.Parameters.Clear();
            return val;
        }

        public static OleDbDataReader ExecuteReader(CommandType cmdType, string cmdText, params OleDbParameter[] oledbParams)
        {
            return ExecuteReader(GetManageDBConnectionStr(), cmdType, cmdText, oledbParams);
        }

        public static OleDbDataReader ExecuteReaderForProcenium(CommandType cmdType, string cmdText, params OleDbParameter[] oledbParams)
        {
            return ExecuteReader(GetDBConnectionStr(), cmdType, cmdText, oledbParams);
        }
        public static OleDbDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params OleDbParameter[] oledbParams)
        {
            OleDbCommand oledbCmd = new OleDbCommand();
            OleDbConnection oledbConn = new OleDbConnection(connectionString);

            try
            {
                PrepareCommand(oledbCmd, oledbConn, null, cmdType, cmdText, oledbParams);
                OleDbDataReader oReader = oledbCmd.ExecuteReader(CommandBehavior.CloseConnection);
                oledbCmd.Parameters.Clear();
                return oReader;
            }
            catch
            {
                oledbConn.Close();
                throw;
            }
        }

        public static OleDbDataReader ExecuteReader(OleDbConnection oledbConn, CommandType cmdType, string cmdText, params OleDbParameter[] oledbParams)
        {
            OleDbCommand oledbCmd = new OleDbCommand();

            try
            {
                PrepareCommand(oledbCmd, oledbConn, null, cmdType, cmdText, oledbParams);
                OleDbDataReader oReader = oledbCmd.ExecuteReader(CommandBehavior.CloseConnection);
                oledbCmd.Parameters.Clear();
                return oReader;
            }
            catch
            {
                oledbConn.Close();
                throw;
            }
        }

        public static object ExecuteScalar(CommandType cmdType, string cmdText, params OleDbParameter[] oledbParams)
        {
            return ExecuteScalar(GetManageDBConnectionStr(), cmdType, cmdText, oledbParams);
        }

        public static object ExecuteScalarForProcenium(CommandType cmdType, string cmdText, params OleDbParameter[] oledbParams)
        {
            return ExecuteScalar(GetDBConnectionStr(), cmdType, cmdText, oledbParams);
        }

        public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params OleDbParameter[] oledbParams)
        {
            OleDbCommand oledbCmd = new OleDbCommand();

            using (OleDbConnection oledbConn = new OleDbConnection(connectionString))
            {
                PrepareCommand(oledbCmd, oledbConn, null, cmdType, cmdText, oledbParams);
                object val = oledbCmd.ExecuteScalar();
                oledbCmd.Parameters.Clear();
                return val;
            }
        }

        public static object ExecuteScalar(OleDbConnection oledbConn, CommandType cmdType, string cmdText, params OleDbParameter[] oledbParams)
        {
            OleDbCommand oledbCmd = new OleDbCommand();

            PrepareCommand(oledbCmd, oledbConn, null, cmdType, cmdText, oledbParams);
            object val = oledbCmd.ExecuteScalar();
            oledbCmd.Parameters.Clear();
            return val;
        }

        public static DataTable ExecuteTable(CommandType cmdType, string cmdText, params OleDbParameter[] oledbParams)
        {
            return ExecuteTable(GetManageDBConnectionStr(), cmdType, cmdText, oledbParams);
        }

        public static DataTable ExecuteTableForProcenium(CommandType cmdType, string cmdText, params OleDbParameter[] oledbParams)
        {
            return ExecuteTable(GetDBConnectionStr(), cmdType, cmdText, oledbParams);
        }

        public static DataTable ExecuteTable(string connectionString, CommandType cmdType, string cmdText, params OleDbParameter[] oledbParams)
        {
            OleDbCommand oledbCmd = new OleDbCommand();

            using (OleDbConnection oledbConn = new OleDbConnection(connectionString))
            {
                PrepareCommand(oledbCmd, oledbConn, null, cmdType, cmdText, oledbParams);
                OleDbDataAdapter oAdapter = new OleDbDataAdapter();
                oAdapter.SelectCommand = oledbCmd;
                DataSet ds = new DataSet();
                oAdapter.Fill(ds, "Result");
                oledbCmd.Parameters.Clear();
                return ds.Tables["Result"];
            }
        }

        public static DataTable ExecuteTable(OleDbConnection oledbConn, CommandType cmdType, string cmdText, params OleDbParameter[] oledbParams)
        {
            OleDbCommand oledbCmd = new OleDbCommand();

            PrepareCommand(oledbCmd, oledbConn, null, cmdType, cmdText, oledbParams);
            OleDbDataAdapter oAdapter = new OleDbDataAdapter();
            oAdapter.SelectCommand = oledbCmd;
            DataSet ds = new DataSet();
            oAdapter.Fill(ds, "Result");
            oledbCmd.Parameters.Clear();
            return ds.Tables["Result"];
        }
    }
}
