using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Data.Sql;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Text;

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
                        if (parameters != null) { sqlcmd.Parameters.AddRange(parameters); }

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
                            sqlstr += "SELECT @RowCount=COUNT(*) FROM ##PAGATION";
                            sqlstr += "SELECT * FROM ##PAGATION WHERE rownum > @PageIndex*@PageSize AND rownum<(@PageIndex+1)*@PageSize-1";
                            sqlstr += "DROP TABLE ##PAGATION";
                            DataSet oDs = new DataSet();
                            SqlDataAdapter sda = new SqlDataAdapter(sqlcmd);
                            sda.Fill(oDs);
                            if (!int.TryParse(rowcount.Value.ToString(), out RowCount))
                            {
                                RowCount = 0;
                                return null;
                            }
                            return oDs;
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

        private void logError(Exception e)
        {
            string fullpath = AppDomain.CurrentDomain.BaseDirectory + "Error";
            if (!Directory.Exists(fullpath)) { Directory.CreateDirectory(fullpath); }
            string path = Path.Combine(fullpath, DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
            if (!File.Exists(path))
                File.Create(path);
            string content = "&&&&&&&&&&&&&&&出错啦&&&&&&&&&&&&&&&\r\n";
            content += DateTime.Now.ToString("yyyy-MM-dd hh24:mm:ss.fff") + "\r\n";
            content += e.Message + "\r\n";
            content += e.StackTrace + "\r\n";
            content += "*******************************\r\n";
            File.AppendAllText(path, content,Encoding.UTF8);
            return ;
        }
    }

    public static class DataTableExtensions
    {
        /// <summary>    
        /// 将一个IEmnumberable集合转化一个DataTable
        /// </summary>    
        /// <typeparamname="T">每个实体的泛型</typeparam>    
        /// <param name="list">待转换元素集合</param>    
        /// <returns></returns>    
        public static DataTable ToDataTable<T>(this IEnumerable<T> list)
        {
            if (list != null)
            {
                //创建属性的集合    
                List<PropertyInfo> pList = new List<PropertyInfo>();
                //获得反射的入口    
                Type type = typeof(T);
                DataTable dt = new DataTable();
                //把所有的public属性加入到集合 并添加DataTable的列    
                Array.ForEach<PropertyInfo>(type.GetProperties()
                , p =>
                {
                    pList.Add(p);
                    dt.Columns.Add(p.Name, p.PropertyType);
                });
                foreach (var item in list)
                {
                    //创建一个DataRow实例    
                    DataRow row = dt.NewRow();
                    //给row 赋值    
                    pList.ForEach(p => row[p.Name] = p.GetValue(item, null));
                    //加入到DataTable    
                    dt.Rows.Add(row);
                }
                return dt;
            }
            return null;
        }

        /// <summary>    
        /// DataTable 转换为List 集合    
        /// </summary>    
        /// <typeparam name="TResult">类型</typeparam>    
        /// <param name="dt">DataTable</param>    
        /// <returns></returns>    
        public static List<T> ToList<T>(this DataTable dt) where T : class, new()
        {
            //创建一个属性的列表    
            List<PropertyInfo> prlist = new List<PropertyInfo>();

            //获取TResult的类型实例  反射的入口    
            Type t = typeof(T);
            //获得TResult 的所有的Public 属性 并找出TResult属性和DataTable的列名称相同的属性(PropertyInfo) 并加入到属性列表     
            Array.ForEach<PropertyInfo>(t.GetProperties(), p =>
            {
                if (dt.Columns.IndexOf(p.Name) != -1) prlist.Add(p);
            });

            //创建返回的集合 
            List<T> oblist = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                //创建TResult的实例    
                T ob = new T();
                //找到对应的数据  并赋值    
                //prlist.ForEach(p => { if (row[p.Name] != DBNull.Value) p.SetValue(ob, row[p.Name], null); });

                //放入到返回的集合中.    
                oblist.Add(ob);
            }
            return oblist;
        }
        /// <summary>    
        /// 将集合类转换成DataTable    
        /// </summary>    
        /// <param name="list">集合</param>    
        /// <returns></returns>    
        public static DataTable ToDataTableTow(IList list)
        {
            DataTable result = new DataTable();
            if (list != null && list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    result.Columns.Add(pi.Name, pi.PropertyType);
                }
                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        object obj = pi.GetValue(list[i], null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }

        /// <summary>    
        /// 将泛型集合类转换成DataTable    
        /// </summary>    
        /// <typeparam name="T">集合项类型</typeparam>    
        /// <param name="list">集合</param>    
        /// <returns>数据集(表)</returns>    
        public static DataTable ToDataTable<T>(IList<T> list)
        {
            return ToDataTable<T>(list, null);
        }

        /// <summary>    
        /// 将泛型集合类转换成DataTable    
        /// </summary>    
        /// <typeparam name="T">集合项类型</typeparam>    
        /// <param name="list">集合</param>    
        /// <param name="propertyName">需要返回的列的列名</param>    
        /// <returns>数据集(表)</returns>    
        public static DataTable ToDataTable<T>(IList<T> list, params string[] propertyName)
        {
            List<string> propertyNameList = new List<string>();
            if (propertyName != null)
                propertyNameList.AddRange(propertyName);
            DataTable result = new DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    if (propertyNameList.Count == 0)
                    {
                        result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                    else
                    {
                        if (propertyNameList.Contains(pi.Name))
                            result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                }
                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        if (propertyNameList.Count == 0)
                        {
                            object obj = pi.GetValue(list[i], null);
                            tempList.Add(obj);
                        }
                        else
                        {
                            if (propertyNameList.Contains(pi.Name))
                            {
                                object obj = pi.GetValue(list[i], null);
                                tempList.Add(obj);
                            }
                        }
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }
    }

    public static class ListExten
    {
        //封装一个方法首先要明确目的，要实现什么效果
        //然后设计实现步骤，先做师门，再做什么，最后做什么
        //在这个过程中，要清楚需要传入什么参数，返回什么结果，传入的参数是否可能为null，返回的结果是否可能为null
        //最后想清楚调用这个方法的大环境是什么，是独立的静态函数还是类的成员函数

        /// <summary>
        /// 集合中的每一个元素都将执行一遍act委托方法
        /// </summary>
        /// <typeparam name="T">集合中操作的泛型类型</typeparam>
        /// <param name="eles">集合</param>
        /// <param name="act">委托方法</param>
        /// <returns></returns>
        public static IEnumerable<T> ForeachOne<T>(this IEnumerable<T> eles, Action<T> act)
        {
            if (act != null)
                foreach (T e in eles)
                {
                    act(e);
                }
            return eles;//返回原操作对象
        }
    }

    public static class DataTableExten
    {
        /// <summary>
        /// 根据泛型将DataTable转成对应的list集合
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="dt">操作的数据源DataTable</param>
        /// <returns></returns>
        public static IEnumerable<T> TabeToList<T>(this DataTable dt) where T : class, new()
        {
            if (dt == null) { return null; }
            List<T> lt = new List<T>();//用于返回的集合
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                //T t = default(T);//获得当前对象的默认值
                T s = new T();//创建该泛型对象的实例
                for (int j = 0; j < dr.ItemArray.Length; j++)
                {
                    s.GetType()
                    .GetProperties()
                    .Where(e => e.Name.ToLower().Equals(dt.Columns[j].ColumnName.ToLower()))
                    .ForeachOne(e =>
                    {
                        object ores = null;
                        if (dr[j].Equals(DBNull.Value))
                        {
                            if (e.PropertyType.IsValueType)
                                ores = default(int);
                            //switch (e.PropertyType.Name)//针对数据库中DBNull类型数值类型就0，引用就null,下面可能没有考虑完全，可以自己在添加上
                            //{
                            //    case "int":
                            //    case "Int16":
                            //    case "Int32":
                            //    case "Int64":
                            //    case "float":
                            //    case "double": ores = 0; break;
                            //    default: ores = default(object); break;
                            //}
                        }
                        else
                        {
                            ores = dr[j];
                        }
                        e.SetValue(s, ores);
                    });
                }
                lt.Add(s);
            }
            return lt;
        }
    }

    public static class StringExten
    {
        public static T StringConvert<T>(this string str) where T:struct
        {
            T obj = default(T);
            Type type = typeof(T);
            MethodInfo memberinfo= type.GetMethod("TryParse");
            object o= memberinfo.Invoke(str,null);
            bool b = (bool)o;
            return obj;
        }
    }
}