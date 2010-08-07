using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Baby.Framework.DALProfile
{
    public sealed class SqlHelper
    {

        public static readonly string ConnectionStringLocal = ConfigurationManager.ConnectionStrings["SQL_DB"].ConnectionString;

        /// <summary>
        /// 预处理Command
        /// </summary>
        /// <param name="sqlcmd">要处理的sqlCommand</param>
        /// <param name="sqlconn">传入的sqlconnection</param>
        /// <param name="trans">Transaction</param>
        /// <param name="cmdType">CommandType</param>
        /// <param name="cmdText">CommandText</param>
        /// <param name="cmdParams">Parameters</param>
        private static void PrepareCommand(SqlCommand sqlcmd, SqlConnection sqlconn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParams)
        {
            if (sqlconn.State != ConnectionState.Open)
            {
                sqlconn.Open();
            }

            sqlcmd.Connection = sqlconn;
            sqlcmd.CommandText = cmdText;

            if (trans != null)
            {
                sqlcmd.Transaction = trans;
            }

            sqlcmd.CommandType = cmdType;

            if (cmdParams != null)
            {
                foreach (SqlParameter param in cmdParams)
                    sqlcmd.Parameters.Add(param);
            }
        }

        public static int ExecuteNonQuery(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            return ExecuteNonQuery(ConnectionStringLocal, cmdType, cmdText, commandParameters);
        }

        /// <summary>
        /// Execute 一条 sql语句
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="cmdType">commandType</param>
        /// <param name="cmdText">commandText</param>
        /// <param name="commandParameters">parameters</param>
        /// <returns>int值</returns>
        public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// Execute 一条 sql语句
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(connection, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="cmdType">commandType</param>
        /// <param name="cmdText">commandText</param>
        /// <param name="commandParameters">parameters</param>
        /// <returns>int值</returns>
        public static int ExecuteNonQuery(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();

            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// Execute 一条 sql语句
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(SqlTransaction, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="cmdType">commandType</param>
        /// <param name="cmdText">commandText</param>
        /// <param name="commandParameters">parameters</param>
        /// <returns>int值</returns>
        public static int ExecuteNonQuery(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();

            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        public static SqlDataReader ExecuteReader(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            return ExecuteReader(ConnectionStringLocal, cmdType, cmdText, commandParameters);
        }

        public static SqlDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(connectionString);

            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }

        public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();

            try
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                connection.Close();
                throw;
            }
        }

        public static object ExecuteScalar(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            return ExecuteScalar(ConnectionStringLocal, cmdType, cmdText, commandParameters);
        }

        public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        public static object ExecuteScalar(SqlConnection conn, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();

            PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }

        public static DataTable ExecuteTable(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            return ExecuteTable(ConnectionStringLocal, cmdType, cmdText, commandParameters);
        }

        public static DataTable ExecuteTable(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                SqlDataAdapter sqlda = new SqlDataAdapter();
                sqlda.SelectCommand = cmd;
                DataSet ds = new DataSet();
                sqlda.Fill(ds, "Result");
                cmd.Parameters.Clear();
                return ds.Tables["Result"];
            }
        }

        public static DataTable ExecuteTable(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();

            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            SqlDataAdapter sqlda = new SqlDataAdapter();
            sqlda.SelectCommand = cmd;
            DataSet ds = new DataSet();
            sqlda.Fill(ds, "Result");
            cmd.Parameters.Clear();
            return ds.Tables["Result"];
        }
    }
}
