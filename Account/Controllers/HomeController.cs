using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace Account.Controllers
{
    public class HomeController : Controller
    {
        DBhelper dbh = new DBhelper();
        // GET: Home
        public ActionResult Index()
        {
            string sqlstr = "select * from [UserInfo] where StatusNO='A'";
            DataSet oDS = dbh.ExecuteQuery(sqlstr);
            List<UserInfo> lisu = oDS.Tables[0].TabeToList<UserInfo>().ToList();
            sqlstr = "select * from [TypeInfo] where ModelName = 'Cost' order by ID";
            oDS = dbh.ExecuteQuery(sqlstr);
            List<TypeInfo> list = oDS.Tables[0].TabeToList<TypeInfo>().ToList();
            ViewBag.listTypeInfo = list;
            return View(lisu);
        }

        /// <summary>
        /// 计算过程
        /// 1.Reporter中当天日期已经是T的是已经结账的，数据就不能变了，所以操作前先检查日期如果查到是F就要先删除，是T的直接报错退出。
        /// 2.如果已经有了当天的记录要先删除，然后再添加
        /// 3.计算每天每个人的平均价的来源是表单中的数据
        /// 4.然后
        /// </summary>
        /// <param name="UploadPriceWithUserIDDate"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(string UploadPriceWithUserIDDate, string AvgPriceWithUser, string payDate)
        {
            string sqlstr = "select * from Reporter where CurrDate='" + payDate + "'";
            DataSet oDS = dbh.ExecuteQuery(sqlstr);
            if (oDS.Tables[0].Rows.Count > 0)//已经存在记录
            {
                if (oDS.Tables[0].Rows[0]["Flag"].ToString().Equals("F"))//还是活动状态可删除，重新计算
                {
                    sqlstr = "delete from Reporter where CurrDate='" + payDate + "'";
                    int Result = dbh.ExecuteResult(sqlstr);
                }
                else
                {
                    Response.Redirect("~/home/indx");
                }
            }
            sqlstr = "select * from Cost where CurrDate='" + payDate + "'";
            oDS = dbh.ExecuteQuery(sqlstr);
            if (oDS.Tables[0].Rows.Count > 0)//已经存在记录
            {
                sqlstr = "delete from Cost where CurrDate='" + payDate + "'";
               int Result = dbh.ExecuteResult(sqlstr);
            }
            string[] UserModel = UploadPriceWithUserIDDate.Split(';');
            DateTime dtnow = DateTime.Now;
            oDS = dbh.ExecuteQuery(sqlstr);
            foreach (string u in UserModel)
            {
                string[] cell = u.Split('_');
                if (cell.Length < 5) { continue; }
                int UserID = cell[0].StringConvert<int>(), TypeID = cell[1].StringConvert<int>();
                DateTime currDate = cell[2].StringConvert<DateTime>();
                decimal Cost = cell[3].StringConvert<decimal>();
                //if (!int.TryParse(cell[0], out UserID)) { continue; }//人
                //if (!int.TryParse(cell[1], out TypeID)) { continue; }//类型
                //if (!DateTime.TryParse(cell[2], out currDate)) { continue; }//当天
                //if (!decimal.TryParse(cell[3], out Cost)) { continue; }//花费

                sqlstr = "insert into Cost values(@UserID,@CurrDate,@CostMoney,@TypeID,@AddDate,@ShareUserID)";
                SqlParameter[] sqlparams =
                {
                    new SqlParameter("@UserID",SqlDbType.Int,32),
                    new SqlParameter("@CurrDate",SqlDbType.DateTime),
                    new SqlParameter("@CostMoney",SqlDbType.Decimal),
                    new SqlParameter("@TypeID",SqlDbType.Int,32),
                    new SqlParameter("@AddDate",SqlDbType.DateTime),
                    new SqlParameter("@ShareUserID",SqlDbType.VarChar,50)
                };
                sqlparams[0].Value = UserID;
                sqlparams[1].Value = currDate;
                sqlparams[2].Value = Cost;
                sqlparams[3].Value = TypeID;
                sqlparams[4].Value = dtnow;
                sqlparams[5].Value = cell[4];
                int Result = dbh.ExecuteResult(sqlstr, sqlparams);
            }
            sqlstr = "select * from UserInfo where StatusNO='A'";
            oDS = dbh.ExecuteQuery(sqlstr);
            string[] ShareUserMOdels = AvgPriceWithUser.Split(';');
            List<UserInfo> lisu = oDS.Tables[0].TabeToList<UserInfo>().ToList();
            Dictionary<int, Reporter> diccost = new Dictionary<int, Reporter>();
            lisu.ForEach(e =>
            {
                Reporter r = new Reporter();
                r.UserID = e.Id;
                r.AvgCost = 0.00M;
                r.CurrDate = new DateTime(Convert.ToInt32(payDate.Split('-')[0]), Convert.ToInt32(payDate.Split('-')[1]), Convert.ToInt32(payDate.Split('-')[2]));
                r.Flag = "F";
                r.AddDate = dtnow;
                diccost.Add(e.Id, r);
            });
            foreach (string shareModel in ShareUserMOdels)
            {
                string[] cell = shareModel.Split('_');
                if (cell.Length < 2) { continue; }
                int UserID = cell[0].StringConvert<int>();
                decimal avg = cell[1].StringConvert<decimal>();
                //if (!int.TryParse(cell[0], out UserID)) { continue; }//人
                //if (!decimal.TryParse(cell[1], out avg)) { continue; }//均摊
                Reporter r = diccost[UserID];
                if (r == null) { continue; }
                r.AvgCost = avg;
            }
            foreach (int key in diccost.Keys)
            {
                sqlstr = "insert into Reporter values(@UserID,@CurrDate,@AvgCost,@Flag,@AddDate,null)";
                SqlParameter[] spm =
                {
                    new SqlParameter("@UserID",SqlDbType.Int,32),
                    new SqlParameter("@CurrDate",SqlDbType.DateTime),
                    new SqlParameter("@AvgCost",SqlDbType.Decimal),
                    new SqlParameter("@Flag",SqlDbType.VarChar,10),
                    new SqlParameter("@AddDate",SqlDbType.DateTime)
                };
                Reporter r = diccost[key];
                spm[0].Value = r.UserID;
                spm[1].Value = r.CurrDate;
                spm[2].Value = r.AvgCost;
                spm[3].Value = r.Flag;
                spm[4].Value = r.AddDate;
                int Result = dbh.ExecuteResult(sqlstr, spm);
            }
            Response.Redirect("~/Home/ShowCost");
            return View();
        }

        [HttpGet]
        public ActionResult ShowCost(string dateStart = "", string dateEnd = "", int pageindex = 0, int pagesize = 10)
        {
            if (dateStart.Equals("") || dateEnd.Equals(""))
            {
                dateStart = DateTime.Now.getWeekOfMonday().ToString("yyyy-MM-dd");
                dateEnd = DateTime.Now.getWeekOfMonday().AddDays(6).ToString("yyyy-MM-dd");
            }
            ViewBag.dateStart = dateStart;
            ViewBag.dateEnd = dateEnd;
            string sqlstrselect = @"a.[UserID],a.[CurrDate],a.[AvgCost],a.[Flag],a.[AddDate]
                            ,b.Name,b.StatusNO
                            ,c.NO,c.TypeName,c.ModelName";
            string sqlstrfrom = @"Reporter a
                            left join UserInfo b on a.UserID=b.ID
                            left join TypeInfo c on a.Flag=c.NO and c.ModelName='Reporter'";
            string sqlstrwhere = "a.CurrDate between @dateStart and @dateEnd order by [CurrDate]";

            SqlParameter[] spm =
            {
                new SqlParameter("@dateStart",SqlDbType.DateTime),
                new SqlParameter("@dateEnd",SqlDbType.DateTime)
            };
            spm[0].Value = dateStart;
            spm[1].Value = dateEnd;
            //DataSet oDS = dbh.ExecuteQuery(sqlstr, spm);
            int rowcount = 0;
            DataSet oDS = dbh.Pagination(sqlstrselect,sqlstrfrom,sqlstrwhere, out rowcount, pageindex, pagesize, spm);
            List<ShowReporter> lisr = oDS.Tables[0].TabeToList<ShowReporter>().ToList();
            ViewBag.RowCount = rowcount;
            ViewBag.CurrPage = pageindex;
            return View(lisr);
        }

        [HttpPost]
        public void ShowCost(string hdateStart = "", string hdateEnd = "", string flage = "F")
        {
            if (hdateStart.Equals("") || hdateEnd.Equals(""))
            {
                Response.Redirect(Url.Action("ShowCost", "Home"));
            }
            string sqlstr = @"update Reporter set Flag = 'T'
                            where CurrDate between @dateStart and @dateEnd";
            SqlParameter[] spm =
            {
                new SqlParameter("@dateStart",SqlDbType.DateTime),
                new SqlParameter("@dateEnd",SqlDbType.DateTime)
            };
            spm[0].Value = hdateStart;
            spm[1].Value = hdateEnd+"23:59:59";
            int nResutl = dbh.ExecuteResult(sqlstr, spm);
            Response.Redirect(Url.Action("ShowCost", "Home") + "?dateStart=" + hdateStart + "&dateEnd=" + hdateEnd);
        }
    }
}