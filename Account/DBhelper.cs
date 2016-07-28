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
            if (string.IsNullOrWhiteSpace(sqlstr))
                return -1;
            using (SqlConnection sqlcon = new SqlConnection(ConnectStr))
            {
                try
                {
                    if (sqlcon.State == ConnectionState.Broken || sqlcon.State == ConnectionState.Closed)
                    {
                        sqlcon.Open();
                    }
                    using (SqlCommand sqlcmd = sqlcon.CreateCommand())
                    {
                        sqlcmd.CommandText = sqlstr;
                        if (parameters != null)
                            sqlcmd.Parameters.AddRange(parameters);
                        return sqlcmd.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    logError(e);
                }
                finally
                {
                    if (sqlcon.State == ConnectionState.Open || sqlcon.State == ConnectionState.Connecting)
                        sqlcon.Close();
                    sqlcon.Dispose();
                }
            }
            return 0;
        }

        public DataSet ExecuteQuery(string sqlstr, params SqlParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(sqlstr))
                return null;
            using (SqlConnection sqlcon = new SqlConnection(ConnectStr))
            {
                try
                {
                    if (sqlcon.State == ConnectionState.Broken || sqlcon.State == ConnectionState.Closed)
                    {
                        sqlcon.Open();
                    }
                    using (SqlCommand sqlcmd = sqlcon.CreateCommand())
                    {
                        sqlcmd.CommandText = sqlstr;
                        if (parameters != null)
                            sqlcmd.Parameters.AddRange(parameters);
                        DataSet ds = new DataSet();
                        SqlDataAdapter sda = new SqlDataAdapter(sqlcmd);
                        sda.Fill(ds);
                        return ds;
                    }
                }
                catch (Exception e)
                {
                    logError(e);
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

        public DataSet Pagination(string sqlstr, out int RowCount, int PageIndex = 0, int PageSize = 10, params SqlParameter[] parameters)
        {
            RowCount = 0;
            if (string.IsNullOrWhiteSpace(sqlstr))
                return null;
            if (PageIndex < 0) PageIndex = 0;
            if (PageSize < 0) PageSize = 10;
            using (SqlConnection sqlcon = new SqlConnection(ConnectStr))
            {
                try
                {
                    if (sqlcon.State == ConnectionState.Broken || sqlcon.State == ConnectionState.Closed)
                    {
                        sqlcon.Open();
                    }
                    using (SqlCommand sqlcmd = sqlcon.CreateCommand())
                    {
                        sqlcmd.CommandText = sqlstr;
                        if (parameters != null)
                        {
                            sqlcmd.Parameters.AddRange(parameters);
                            if (sqlcmd.Parameters.Contains("@PageIndex") || sqlcmd.Parameters.Contains("@PageSize") || sqlcmd.Parameters.Contains("@RowCount"))
                            {
                                return null;
                            }
                            else
                            {
                                SqlParameter pageindex = new SqlParameter("@PageIndex", SqlDbType.Int, 4);
                                pageindex.Value = PageIndex;
                                sqlcmd.Parameters.Add(pageindex);
                                SqlParameter pagesize = new SqlParameter("@PageSize", SqlDbType.Int, 4);
                                pageindex.Value = PageSize;
                                sqlcmd.Parameters.Add(pageindex);
                                SqlParameter rowcount = new SqlParameter("@RowCount", SqlDbType.Int, 4);
                                rowcount.Direction = ParameterDirection.Output;
                                sqlstr = "SELECT ROW_NUMBER() OVER(ORDER BY @@IDENTITY) AS rownum,* INTO ##PAGATION FROM( " + sqlstr;
                                sqlstr += " ) temp";
                                sqlstr += "SELECT * FROM ##PAGATION WHERE rownum>@PageIndex*@PageSize AND  rownum>(@PageIndex+1)*@PageSize-1";
                                sqlstr += "SELECT @RowCount=COUNT(*) FROM ##PAGATION";
                                sqlstr += "DROP TABLE ##PAGATION";
                                DataSet oDs = new DataSet();
                                SqlDataAdapter sda = new SqlDataAdapter(sqlcmd);
                                sda.Fill(oDs);
                                if(!int.TryParse(rowcount.Value.ToString(),out RowCount))
                                {
                                    RowCount = 0;
                                    return null;
                                }
                                return oDs;
                            }

                        }
                    }
                }
                catch (Exception e)
                {
                    logError(e);
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

        private Exception logError(Exception e)
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
            return e;
        }
    }
}