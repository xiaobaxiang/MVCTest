using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Data.Sql;
using System.IO;

namespace Account
{
    public class DBhelper
    {
        protected readonly static string ConnectStr = "Data Source=.;Initial Catalog=Account;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public int ExecuteResult(string sqlstr, params SqlParameter[] parameters)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConnectStr))
            {
                try
                {
                    if (sqlcon.State == ConnectionState.Broken || sqlcon.State == ConnectionState.Closed)
                    {
                        sqlcon.Open();
                    }
                    SqlCommand sqlcmd = sqlcon.CreateCommand();
                    if (string.IsNullOrWhiteSpace(sqlstr))
                        return -1;
                    sqlcmd.CommandText = sqlstr;
                    if (parameters != null)
                        sqlcmd.Parameters.AddRange(parameters);
                    return sqlcmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
                    if (!File.Exists(path))
                        File.Create(path);
                    string content = "&&&&&&&&&&&&&&&出错啦&&&&&&&&&&&&&&&\r\n";
                    content += DateTime.Now.ToString("yyyy-MM-dd hh24:mm:ss.fff") + "\r\n";
                    content += e.Message + "\r\n";
                    content += e.StackTrace + "\r\n";
                    content += "*******************************\r\n";
                    File.AppendAllText(path, content);
                }
                finally
                {
                    if (sqlcon.State == ConnectionState.Open|| sqlcon.State==ConnectionState.Connecting)
                        sqlcon.Close();
                    sqlcon.Dispose();
                }

            }
            return 0;
        }

        public DataSet ExecuteQuery(string sqlstr,params SqlParameter[] parameters)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConnectStr))
            {
                try
                {
                    if (sqlcon.State == ConnectionState.Broken || sqlcon.State == ConnectionState.Closed)
                    {
                        sqlcon.Open();
                    }
                    SqlCommand sqlcmd = sqlcon.CreateCommand();
                    if (string.IsNullOrWhiteSpace(sqlstr))
                        return null;
                    sqlcmd.CommandText = sqlstr;
                    if (parameters != null)
                        sqlcmd.Parameters.AddRange(parameters);
                    DataSet ds = new DataSet();
                    SqlDataAdapter sda = new SqlDataAdapter(sqlcmd);
                    sda.Fill(ds);
                    return ds;
                }
                catch (Exception e)
                {
                    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
                    if (!File.Exists(path))
                        File.Create(path);
                    string content = "&&&&&&&&&&&&&&&出错啦&&&&&&&&&&&&&&&\r\n";
                    content += DateTime.Now.ToString("yyyy-MM-dd hh24:mm:ss.fff") + "\r\n";
                    content += e.Message + "\r\n";
                    content += e.StackTrace + "\r\n";
                    content += "*******************************\r\n";
                    File.AppendAllText(path, content);
                }
                finally
                {
                    if (sqlcon.State == ConnectionState.Open || sqlcon.State == ConnectionState.Connecting)
                        sqlcon.Close();
                    sqlcon.Dispose();
                }

            }
            return null;
        }
    }
}