using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Net;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace DBHelp
{
    /// <summary>
    /// SQLite数据库的通用访问代码，抽象类
    /// 不允许实例化，在应用时直接调用即可
    /// </summary>
    public abstract class SQLiteHelp
    {
        #region 字段
        /// <summary>
        /// 连接字符串(.db文件路径)
        /// 例："Data Source=" + Environment.CurrentDirectory + "\\active.db"
        /// </summary>
        public static string _connString = "Data Source=" + Environment.CurrentDirectory + "\\test.db";
        /// <summary>
        /// 连接对象
        /// </summary>
        public static SQLiteConnection _conn;
        /// <summary>
        /// 参数集合
        /// </summary>
        public static List<SQLiteParameter> _parmList;
        #endregion

        #region 连接对象设置，参数设置
        /// <summary>
        /// 初始化连接对象
        /// </summary>
        /// <param name="connString">文件名</param>
        /// <exception cref="Exception"></exception>
        public static void ConnInit(string connString)
        {
            try
            {
                string connectionString = string.Format(@"Data Source={0};Version=3;", connString);
                _connString = connectionString;
                _conn = new SQLiteConnection(_connString);
                if (_conn.State != ConnectionState.Open)
                {
                    _conn.Open();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        /// <summary>
        /// 释放连接对象
        /// </summary>
        public static void ConnDispose()
        {
            if (_conn != null)
            {
                _conn.Close();
                _conn.Dispose();
            }
            _conn = null;
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="paramName">参数名</param>
        /// <param name="value">参数值</param>
        /// <param name="type">参数类型，不传的话默认为字符串</param>
        public static void AddParamter(string paramName, object value, object type = null)
        {
            try
            {
                if (_parmList == null)
                {
                    _parmList = new List<SQLiteParameter>();
                }
                DbType DbType = DbType.String;
                if (type != null)
                {
                    DbType = (DbType)type;
                }
                SQLiteParameter parameter = new SQLiteParameter(paramName, DbType);
                parameter.Value = value;

                _parmList.Add(parameter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        /// <summary>
        /// 清空参数
        /// </summary>
        public static void ClearParamter()
        {
            if (_parmList != null)
            {
                _parmList.Clear();
            }
        }

        /// <summary>
        /// 指令类型转换
        /// </summary>
        /// <param name="TypeName">SQL语句，表，存储过程"三种固定字符串</param>
        /// <returns></returns>
        public static CommandType TransferCommandType(string TypeName)
        {
            CommandType _commandType = CommandType.Text;
            try
            {
                switch (TypeName)
                {
                    case "SQL语句":
                        _commandType = CommandType.Text;
                        break;
                    case "表":
                        _commandType = CommandType.TableDirect;
                        break;
                    case "存储过程":
                        _commandType = CommandType.StoredProcedure;
                        break;
                    default:
                        _commandType = CommandType.Text;
                        break;
                }
            }
            catch (Exception)
            {
                _commandType = CommandType.Text;
            }
            return _commandType;
        }
        #endregion

        #region 执行操作 
        /// <summary>
        /// 执行非查询操作
        /// </summary>
        /// <param name="sqlStr">SQL语句字符串，表名称，存储过程名称</param>
        /// <param name="commandType">"SQL语句，表，存储过程"三种固定字符串，不填默认为SQL语句</param>
        /// <returns>执行结果</returns>
        /// <exception cref="Exception">错误</exception>
        public static int ExecuteNonQuery(StringBuilder sqlStr, string commandType = "")
        {
            CommandType _commandType = TransferCommandType(commandType);
            int result = 0;
            using (SQLiteTransaction transaction = _conn.BeginTransaction())
            {
                try
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(sqlStr.ToString(), _conn))
                    {
                        cmd.CommandType = _commandType;
                        if (_parmList != null && _parmList.Count > 0)
                        {
                            foreach (SQLiteParameter parm in _parmList)
                            {
                                cmd.Parameters.Add(parm);
                            }
                        }
                        //定义执行和返回内容
                        result = cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message.ToString());
                }
            }
            return result;
        }

        /// <summary>
        /// 执行查询，返回首行首列
        /// </summary>
        /// <typeparam name="T">传入的类型</typeparam>
        /// <param name="sqlStr">SQL语句字符串，表名称，存储过程名称</param>
        /// <param name="commandType">"SQL语句，表，存储过程"三种固定字符串，不填默认为SQL语句</param>
        /// <returns>执行结果T</returns>
        /// <exception cref="Exception">错误</exception>
        public static T ExecuteScalar<T>(StringBuilder sqlStr, string commandType = "")
        {
            CommandType _commandType = TransferCommandType(commandType);
            object obj = null;
            using (SQLiteTransaction transaction = _conn.BeginTransaction())
            {
                try
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(sqlStr.ToString(), _conn))
                    {
                        cmd.CommandType = _commandType;
                        if (_parmList != null && _parmList.Count > 0)
                        {
                            foreach (SQLiteParameter parm in _parmList)
                            {
                                cmd.Parameters.Add(parm);
                            }
                        }
                        //定义执行和返回内容  
                        obj = cmd.ExecuteScalar();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message.ToString());
                }
            }
            return (T)obj;
        }

        /// <summary>
        /// 执行查询，返回DataTable
        /// </summary>
        /// <param name="sqlStr">SQL语句字符串，表名称，存储过程名称</param>
        /// <param name="commandType">"SQL语句，表，存储过程"三种固定字符串，不填默认为SQL语句</param>
        /// <returns>执行结果DataTable</returns>
        /// <exception cref="Exception">错误</exception>
        public static DataTable ExecuteDataTable(StringBuilder sqlStr, string commandType = "")
        {
            CommandType _commandType = TransferCommandType(commandType);
            DataTable dt = null;
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sqlStr.ToString(), _conn))
                {
                    cmd.CommandType = _commandType;
                    if (_parmList != null && _parmList.Count > 0)
                    {
                        foreach (SQLiteParameter parm in _parmList)
                        {
                            cmd.Parameters.Add(parm);
                        }
                    }
                    //定义执行和返回内容  
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter();
                    adapter.SelectCommand = cmd;
                    dt = new DataTable();
                    adapter.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
            return dt;
        }

        /// <summary>
        /// 执行查询，返回DataSet结果集
        /// </summary>
        /// <param name="sqlStr">SQL语句字符串，表名称，存储过程名称</param>
        /// <param name="commandType">"SQL语句，表，存储过程"三种固定字符串，不填默认为SQL语句</param>
        /// <returns>执行结果DataSet</returns>
        /// <exception cref="Exception">错误</exception>
        public static DataSet ExecuteDataSet(StringBuilder sqlStr, string commandType = "")
        {
            CommandType _commandType = TransferCommandType(commandType);
            DataSet ds = null;
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sqlStr.ToString(), _conn))
                {
                    cmd.CommandType = _commandType;
                    if (_parmList != null && _parmList.Count > 0)
                    {
                        foreach (SQLiteParameter parm in _parmList)
                        {
                            cmd.Parameters.Add(parm);
                        }
                    }
                    //定义执行和返回内容 
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter();
                    adapter.SelectCommand = cmd;
                    ds = new DataSet();
                    adapter.Fill(ds);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
            return ds;
        }

        /// <summary>
        /// 执行查询，返回DataReader结果集
        /// </summary>
        /// <param name="sqlStr">SQL语句字符串，表名称，存储过程名称</param>
        /// <param name="commandType">"SQL语句，表，存储过程"三种固定字符串，不填默认为SQL语句</param>
        /// <returns>执行结果SQLiteDataReader</returns>
        /// <exception cref="Exception">错误</exception>
        public static SQLiteDataReader ExecuteReader(StringBuilder sqlStr, string commandType = "")
        {
            CommandType _commandType = TransferCommandType(commandType);
            SQLiteDataReader result = null;
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sqlStr.ToString(), _conn))
                {
                    cmd.CommandType = _commandType;
                    if (_parmList != null && _parmList.Count > 0)
                    {
                        foreach (SQLiteParameter parm in _parmList)
                        {
                            cmd.Parameters.Add(parm);
                        }
                    }
                    //定义执行和返回内容
                    SQLiteDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
            return result;
        }

        /// <summary>
        /// 回收内存
        /// </summary>
        /// <returns>执行结果</returns>
        /// <exception cref="Exception">错误</exception>
        public static int VACUUM()
        {
            CommandType _commandType = TransferCommandType("");
            int result = 0;
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand("VACUUM", _conn))
                {
                    cmd.CommandType = _commandType;
                    if (_parmList != null && _parmList.Count > 0)
                    {
                        foreach (SQLiteParameter parm in _parmList)
                        {
                            cmd.Parameters.Add(parm);
                        }
                    }
                    //定义执行和返回内容
                    result = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
            return result;
        }
        #endregion

        #region 批量插入
        /// <summary>
        /// 将 DataTable 的数据批量插入到数据库中。
        /// </summary>
        /// <param name="dataTable">要批量插入的DataTable</param> 
        public static void Insert_SqlBulkCopy(DataTable dataTable)
        {
            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return;
            }
            using (SQLiteTransaction transcation = _conn.BeginTransaction())
            {
                try
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(_conn))
                    {
                        cmd.CommandType = CommandType.Text;

                        cmd.CommandText = GenerateInserSql(dataTable);
                        if (cmd.CommandText == string.Empty)
                        {
                            return;
                        }

                        int i = 0;
                        foreach (DataRow row in dataTable.Rows)
                        {
                            i += 1;
                            ProcessCommandParameters(dataTable, cmd, row, i);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    transcation.Commit();
                }
                catch (Exception ex)
                {
                    if (transcation != null)
                    {
                        transcation.Rollback();
                    }
                    throw new Exception(ex.Message.ToString());
                }
            }
        }

        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="command"></param>
        /// <param name="row"></param>
        /// <param name="first"></param>
        private static void ProcessCommandParameters(DataTable dataTable, SQLiteCommand command, DataRow row, int first)
        {
            for (var c = 0; c < dataTable.Columns.Count; c++)
            {
                SQLiteParameter parameter;
                //首次创建参数，是为了使用缓存
                if (first == 1)
                {
                    parameter = new SQLiteParameter();
                    parameter.ParameterName = dataTable.Columns[c].ColumnName;
                    command.Parameters.Add(parameter);
                }
                else
                {
                    parameter = command.Parameters[c];
                }
                parameter.Value = row[c];
            }
        }

        /// <summary>
        /// 生成插入数据的sql语句。
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private static string GenerateInserSql(DataTable table)
        {
            var names = new StringBuilder();
            var values = new StringBuilder();
            int i = 0;
            foreach (DataColumn column in table.Columns)
            {
                i += 1;
                if (i != 1)
                {
                    names.Append(",");
                    values.Append(",");
                }
                names.Append(column.ColumnName);
                values.Append("@" + column.ColumnName);
            }
            return string.Format("INSERT INTO {0}({1}) VALUES ({2})", table.TableName, names, values);
        }
        #endregion 
    }
}
