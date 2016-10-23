using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Account
{
    public class Model
    {
    }

    public class UserInfo
    {
        private int _id;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        private string _name;

        [Display(Name = "姓名")]
        [StringLength(maximumLength:10,ErrorMessage = "最大长度不超过10")]
        [Required(AllowEmptyStrings = false,ErrorMessage = "姓名必填")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _statusno;
        [Display(Name = "状态")]
        [StringLength(maximumLength:10, MinimumLength = 1, ErrorMessage="长度不超过10个字符")]
        [Required(ErrorMessage = "状态必填")]
        public string StatusNO
        {
            get { return _statusno; }
            set { _statusno = value; }
        }
    }

    public class Cost
    {
        private int _id;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        private int _userID;

        public int UserID
        {
            get { return _userID; }
            set { _userID = value; }
        }
        private System.DateTime _currDate;

        public System.DateTime CurrDate
        {
            get { return _currDate; }
            set { _currDate = value; }
        }
        private decimal _payMoney;

        public decimal PayMoney
        {
            get { return _payMoney; }
            set { _payMoney = value; }
        }
        private int _typeID;

        public int TypeID
        {
            get { return _typeID; }
            set { _typeID = value; }
        }
        private System.DateTime _addDate;

        public System.DateTime AddDate
        {
            get { return _addDate; }
            set { _addDate = value; }
        }

        private string _shareuserid;

        public string ShareUserID
        {
            get { return _shareuserid; }
            set { _shareuserid = value; }
        }
    }

    public class SumCost
    {
        private decimal _costSumMoney;

        public decimal CostSumMoney
        {
            get { return _costSumMoney; }
            set { _costSumMoney = value; }
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        /// <summary>
        /// Pay
        /// </summary>		
        private decimal _paymoney;
        public decimal PayMoney
        {
            get { return _paymoney; }
            set { _paymoney = value; }
        }
    }

    public class Reporter
    {
        private int _id;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        private int _userID;

        public int UserID
        {
            get { return _userID; }
            set { _userID = value; }
        }
        private System.DateTime _currDate;

        public System.DateTime CurrDate
        {
            get { return _currDate; }
            set { _currDate = value; }
        }
        private decimal _avgCost;

        public decimal AvgCost
        {
            get { return _avgCost; }
            set { _avgCost = value; }
        }

        private string _flag;
        public string Flag
        {
            get { return _flag; }
            set { _flag = value; }
        }

        private System.DateTime _adddate;

        public System.DateTime AddDate
        {
            get { return _adddate; }
            set { _adddate = value; }
        }
    }

    public class TypeInfo
    {
        private int _id;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        private string _modelName;

        public string ModelName
        {
            get { return _modelName; }
            set { _modelName = value; }
        }
        private string _typeName;

        public string TypeName
        {
            get { return _typeName; }
            set { _typeName = value; }
        }
        private int _parentID;

        public int ParentID
        {
            get { return _parentID; }
            set { _parentID = value; }
        }
    }

    public class ShowReporter
    {
        //报表

        /// <summary>
        /// UserID
        /// </summary>		
        private int _userid;
        public int UserID
        {
            get { return _userid; }
            set { _userid = value; }
        }

        /// <summary>
        /// CurrDate
        /// </summary>		
        private DateTime _currdate;
        public DateTime CurrDate
        {
            get { return _currdate; }
            set { _currdate = value; }
        }
        /// <summary>
        /// AvgCost
        /// </summary>		
        private decimal _avgcost;
        public decimal AvgCost
        {
            get { return _avgcost; }
            set { _avgcost = value; }
        }

        /// <summary>
        /// Flag
        /// </summary>		
        private string _flag;
        public string Flag
        {
            get { return _flag; }
            set { _flag = value; }
        }
        /// <summary>
        /// AddDate
        /// </summary>		
        private DateTime _adddate;
        public DateTime AddDate
        {
            get { return _adddate; }
            set { _adddate = value; }
        }

        //USER

        /// <summary>
        /// Name
        /// </summary>		
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// StatusNO
        /// </summary>		
        private string _statusno;
        public string StatusNO
        {
            get { return _statusno; }
            set { _statusno = value; }
        }

        //TypeInfo

        /// <summary>
        /// ModelName
        /// </summary>		
        private string _modelname;
        public string ModelName
        {
            get { return _modelname; }
            set { _modelname = value; }
        }
        /// <summary>
        /// TypeName
        /// </summary>		
        private string _typename;
        public string TypeName
        {
            get { return _typename; }
            set { _typename = value; }
        }
        /// <summary>
        /// ParentID
        /// </summary>		
        private int _parentid;
        public int ParentID
        {
            get { return _parentid; }
            set { _parentid = value; }
        }
    }

    public class ShowPay
    {
        /// <summary>
        /// UserID
        /// </summary>		
        private int _userid;
        public int UserID
        {
            get { return _userid; }
            set { _userid = value; }
        }
        /// <summary>
        /// CurrDate
        /// </summary>		
        private DateTime _currdate;
        public DateTime CurrDate
        {
            get { return _currdate; }
            set { _currdate = value; }
        }

        /// <summary>
        /// Name
        /// </summary>		
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// ModelName
        /// </summary>		
        private string _modelname;
        public string ModelName
        {
            get { return _modelname; }
            set { _modelname = value; }
        }
        /// <summary>
        /// TypeName
        /// </summary>		
        private string _typename;
        public string TypeName
        {
            get { return _typename; }
            set { _typename = value; }
        }

        private decimal _payMoney;

        public decimal PayMoney
        {
            get { return _payMoney; }
            set { _payMoney = value; }
        }
    }
}