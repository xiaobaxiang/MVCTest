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
        public ActionResult Index(string CurrDate="")
        {
            string sqlstr = "select * from [UserInfo] where StatusNO='A'";
            DataSet oDS = dbh.ExecuteQuery(sqlstr);
            List<UserInfo> lisu = oDS.Tables[0].TabeToList<UserInfo>().ToList();
            sqlstr = "select * from [TypeInfo] where ModelName = 'Cost' order by ID";
            oDS = dbh.ExecuteQuery(sqlstr);
            List<TypeInfo> list = oDS.Tables[0].TabeToList<TypeInfo>().ToList();
            ViewBag.listTypeInfo = list;
            DateTime dt;
            if (CurrDate == ""||!DateTime.TryParse(CurrDate,out dt)) CurrDate = DateTime.Now.ToString("yyyy-MM-dd");
            sqlstr = "select * from Cost where CurrDate=@CurrDate";
            SqlParameter[] spm = {new SqlParameter("@CurrDate",SqlDbType.DateTime) };
            spm[0].Value = CurrDate;
            oDS = dbh.ExecuteQuery(sqlstr, spm);
            List<Cost> listc = oDS.Tables[0].TabeToList<Cost>().ToList();
            ViewBag.listCostInfo = listc;
            sqlstr = "select * from Reporter where CurrDate=@CurrDate";
            SqlParameter[] spmr = { new SqlParameter("@CurrDate", SqlDbType.DateTime) };
            spmr[0].Value = CurrDate;
            oDS = dbh.ExecuteQuery(sqlstr, spmr);
            List<Reporter> listr = oDS.Tables[0].TabeToList<Reporter>().ToList();
            ViewBag.listReportInfo = listr;
            ViewBag.CurrDate = CurrDate;
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
            DateTime currDate = payDate.StringConvert<DateTime>();
            foreach (string u in UserModel)
            {
                string[] cell = u.Split('_');
                if (cell.Length < 5) { continue; }
                int UserID = cell[0].StringConvert<int>(), TypeID = cell[1].StringConvert<int>();
                //DateTime currDate = payDate.StringConvert<DateTime>(); //取一致的payDate cell[2].StringConvert<DateTime>();
                decimal Cost = cell[3].StringConvert<decimal>();
                //if (!int.TryParse(cell[0], out UserID)) { continue; }//人
                //if (!int.TryParse(cell[1], out TypeID)) { continue; }//类型
                //if (!DateTime.TryParse(cell[2], out currDate)) { continue; }//当天
                //if (!decimal.TryParse(cell[3], out Cost)) { continue; }//花费

                sqlstr = "insert into Cost values(@UserID,@CurrDate,@PayMoney,@TypeID,@AddDate,@ShareUserID)";
                SqlParameter[] sqlparams =
                {
                    new SqlParameter("@UserID",SqlDbType.Int,32),
                    new SqlParameter("@CurrDate",SqlDbType.DateTime),
                    new SqlParameter("@PayMoney",SqlDbType.Decimal),
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
            string[] shareUserMOdels = AvgPriceWithUser.Split(';');
            List<UserInfo> lisu = oDS.Tables[0].TabeToList<UserInfo>().ToList();
            Dictionary<int, Reporter> diccost = new Dictionary<int, Reporter>();
            lisu.ForEach(e =>
            {
                Reporter r = new Reporter();
                r.UserID = e.Id;
                r.AvgCost = 0.00M;
                r.CurrDate = currDate;//new DateTime(Convert.ToInt32(payDate.Split('-')[0]), Convert.ToInt32(payDate.Split('-')[1]), Convert.ToInt32(payDate.Split('-')[2]));
                r.Flag = "F";
                r.AddDate = dtnow;
                diccost.Add(e.Id, r);
            });
            foreach (string shareModel in shareUserMOdels)
            {
                string[] cell = shareModel.Split('_');
                if (cell.Length < 2) { continue; }
                int userId = cell[0].StringConvert<int>();
                decimal avg = cell[1].StringConvert<decimal>();
                //if (!int.TryParse(cell[0], out UserID)) { continue; }//人
                //if (!decimal.TryParse(cell[1], out avg)) { continue; }//均摊
                Reporter r = diccost[userId];
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
                dateStart = DateTime.Now.GetWeekOfMonday().ToString("yyyy-MM-dd");
                dateEnd = DateTime.Now.GetWeekOfMonday().AddDays(6).ToString("yyyy-MM-dd");
            }
            ViewBag.dateStart = dateStart;
            ViewBag.dateEnd = dateEnd;
            string sqlstrselect = @"a.[UserID],a.[CurrDate],a.[AvgCost],a.[Flag],a.[AddDate]
                            ,b.Name,b.StatusNO
                            ,c.NO,c.TypeName,c.ModelName";
            string sqlstrfrom = @"Reporter a
                            left join UserInfo b on a.UserID=b.ID
                            left join TypeInfo c on a.Flag=c.NO and c.ModelName='Reporter'";
            string sqlstrwhere = "a.CurrDate between @dateStart and @dateEnd";

            SqlParameter[] spm =
            {
                new SqlParameter("@dateStart",SqlDbType.DateTime),
                new SqlParameter("@dateEnd",SqlDbType.DateTime)
            };
            spm[0].Value = dateStart + " 00:00:00";
            spm[1].Value = dateEnd + " 23:59:59";
            //DataSet oDS = dbh.ExecuteQuery(sqlstr, spm);
            int rowcount = 0;
            DataSet oDS = dbh.Pagination(sqlstrselect, sqlstrfrom, sqlstrwhere, "CurrDate ASC", out rowcount, pageindex, pagesize, spm);
            List<ShowReporter> lisr = oDS.Tables[0].TabeToList<ShowReporter>().ToList();
            ViewBag.RowCount = rowcount;
            ViewBag.CurrPage = pageindex;
            string sql = "select a.Name as UserName,b.CostSumMoney,c.PayMoney from UserInfo a left join (";
            sql += " select UserID,sum(AvgCost) CostSumMoney from Reporter where CurrDate between @dateStart and @dateEnd group by UserID ) b on a.ID=b.UserID";
            sql += " left join  (select UserID,sum(PayMoney) PayMoney from Cost where CurrDate between @dateStart and @dateEnd group by UserID) c on a.ID=c.UserID";

            SqlParameter[] spm1 =
            {
                new SqlParameter("@dateStart",SqlDbType.DateTime),
                new SqlParameter("@dateEnd",SqlDbType.DateTime)
            };
            spm1[0].Value = dateStart;
            spm1[1].Value = dateEnd;
            List<SumCost> lssc = dbh.ExecuteQuery(sql, spm1).Tables[0].TabeToList<SumCost>().ToList();
            ViewBag.UserCost = lssc;

            sql = "select a.UserID,a.CurrDate,c.Name,b.ModelName,b.TypeName,a.PayMoney from Cost a left join TypeInfo b on a.TypeID=b.ID and b.ModelName='Cost'";
            sql += " left join UserInfo c on a.UserID=c.ID where a.CurrDate between @dateStart and @dateEnd order by CurrDate";
            SqlParameter[] spm2 =
            {
                new SqlParameter("@dateStart",SqlDbType.DateTime),
                new SqlParameter("@dateEnd",SqlDbType.DateTime)
            };

            //string aaa = "1233".sss(delegate() { return true; });
            //string aaa = "1233".sss(() => true);
            spm2[0].Value = dateStart;
            spm2[1].Value = dateEnd;
            List<ShowPay> lssp = dbh.ExecuteQuery(sql, spm2).Tables[0].TabeToList<ShowPay>().ToList();
            ViewBag.UserPay = lssp;
            return View(lisr);
        }

        [HttpPost]
        public void ShowCost(string hdateStart = "", string hdateEnd = "", string flage = "F")
        {
            if (hdateStart.Equals("") || hdateEnd.Equals(""))
                Response.Redirect(Url.Action("ShowCost", "Home"));
            string sqlstr = @"update Reporter set Flag = 'T'
                            where CurrDate between @dateStart and @dateEnd";
            SqlParameter[] spm =
            {
                new SqlParameter("@dateStart",SqlDbType.DateTime),
                new SqlParameter("@dateEnd",SqlDbType.DateTime)
            };
            spm[0].Value = hdateStart + " 00:00:00";
            spm[1].Value = hdateEnd + " 23:59:59";
            int nResutl = dbh.ExecuteResult(sqlstr, spm);
            Response.Redirect(Url.Action("ShowCost", "Home") + "?dateStart=" + hdateStart + "&dateEnd=" + hdateEnd);
        }

        [HttpGet]
        public ActionResult TestDataAnnotations()
        {
            UserInfo u=new UserInfo();
            return View(u);
        }
        [HttpPost]
        public ActionResult TestDataAnnotations(UserInfo u)
        {
            return RedirectToAction("Index");
        }
    }
}