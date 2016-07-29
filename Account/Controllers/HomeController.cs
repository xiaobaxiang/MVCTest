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
            string sqlstr = "select * from [UserInfo]";
            DataSet oDS = dbh.ExecuteQuery(sqlstr);
            List<UserInfo> lisu = oDS.Tables[0].TabeToList<UserInfo>().ToList();
            return View(lisu);
        }

        [HttpPost]
        public ActionResult Index(string UploadPriceWithUserIDDate)
        {
            string[] UserModel = UploadPriceWithUserIDDate.Split(';');
            DateTime dtnow = DateTime.Now,currDate;
            foreach(string u in UserModel)
            {
                string[] cell = u.Split('-');
                if (cell.Length <= 1) { continue; }
                int UserID,TypeID;
                decimal Cost;
                if(!int.TryParse(cell[0],out UserID)){continue; }//人
                if (!int.TryParse(cell[1], out TypeID)) { continue; }//类型
                if (!decimal.TryParse(cell[2], out Cost)) { continue; }//花费
                if(!DateTime.TryParse(cell[3],out currDate)){ continue; }//当天

                //Cost c = new Cost();
                //c.UserID = UserID;
                //c.TypeID = TypeID;
                //c.CostMoney = Cost;
                //c.AddDate = dtnow;
                //c.CurrDate = currDate;

                string sqlstr = "insert into values(@UserID,@CurrDate,@CostMoney,@TypeID,@AddDate)";
                SqlParameter[] sqlparams =
                {
                    new SqlParameter("@UserID",SqlDbType.Int,32),
                    new SqlParameter("@CurrDate",SqlDbType.DateTime),
                    new SqlParameter("@CostMoney",SqlDbType.Decimal),
                    new SqlParameter("@TypeID",SqlDbType.Int,32),
                    new SqlParameter("@AddDate",SqlDbType.DateTime)
                };
                sqlparams[0].Value = UserID;
                sqlparams[1].Value = currDate;
                sqlparams[2].Value = Cost;
                sqlparams[3].Value = TypeID;
                sqlparams[4].Value = dtnow;
                dbh.ExecuteResult(sqlstr, sqlparams);
            }
            return View();
        }
    }
}