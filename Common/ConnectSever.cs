using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaultTreeAnalysis.Common
{
    public class ConnectSever
    {

        public static string dbfile = "";

        public static bool Init(string filename)
        {
            try
            {
                //初始化连接数据库 
                DBHelp.SQLiteHelp.ConnInit(filename);
                dbfile = filename;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //读取数据
        public static DataTable ReadInitial(string TableName, string SQLTiaoJian)
        {
            try
            {
                StringBuilder sqlStringBuilder = new StringBuilder();
                string sql = "SELECT * FROM " + TableName + " " + SQLTiaoJian;
                sqlStringBuilder.Append(sql);

                DataTable dt = DBHelp.SQLiteHelp.ExecuteDataTable(sqlStringBuilder);
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static void CloseDB()
        {
            try
            {
                DBHelp.SQLiteHelp.ConnDispose();
            }
            catch (Exception)
            {
            }
        }


        public static DataTable SelectOne(string GUID)
        {
            try
            {
                StringBuilder sqlStringBuilder = new StringBuilder();
                string sql = "SELECT * FROM BasicEvents where GUID='" + GUID + "'";
                sqlStringBuilder.Append(sql);

                DataTable dt = DBHelp.SQLiteHelp.ExecuteDataTable(sqlStringBuilder);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool CheckExist(string GUID)
        {
            try
            {
                StringBuilder sqlStringBuilder = new StringBuilder();
                string sql = "SELECT * FROM BasicEvents where GUID='" + GUID + "'";
                sqlStringBuilder.Append(sql);

                DataTable dt = DBHelp.SQLiteHelp.ExecuteDataTable(sqlStringBuilder);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //插入数据
        public static bool InsertOne(object[] datas)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("Insert into BasicEvents(GUID,[Group],Identifier,Type,Description,LogicalCondition,InputType,InputValue,InputValue2,ExtraValue1,Units) values('" + datas[0] + "','" + datas[1] + "','" + datas[2] + "','" + datas[3] + "','" + datas[4] + "','" + datas[5] + "','" + datas[6] + "','" + datas[7] + "','" + datas[8] + "','" + datas[9] + "','" + datas[10] + "')");
                int rel = DBHelp.SQLiteHelp.ExecuteNonQuery(stringBuilder);//返回值是数据库中受影响的行数 
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //插入数据
        public static bool UpdateOne(object[] datas)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("Update BasicEvents Set [Group]='" + datas[1] + "',Identifier='" + datas[2] + "', Type='" + datas[3] + "',Description='" + datas[4] + "',LogicalCondition='" + datas[5] + "',InputType='" + datas[6] + "',InputValue='" + datas[7] + "',InputValue2='" + datas[8] + "',ExtraValue1='" + datas[9] + "',Units='" + datas[10] + "' where GUID='" + datas[0] + "'");
                int rel = DBHelp.SQLiteHelp.ExecuteNonQuery(stringBuilder);//返回值是数据库中受影响的行数 
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //插入数据
        public static bool DeleteOne(string GUID)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("Delete From BasicEvents Where GUID='" + GUID + "'");
                int rel = DBHelp.SQLiteHelp.ExecuteNonQuery(stringBuilder);//返回值是数据库中受影响的行数 
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //插入数据
        public static bool InsertAll(DataTable dt)
        {
            try
            {
                DBHelp.SQLiteHelp.Insert_SqlBulkCopy(dt);//返回值是数据库中受影响的行数 
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
