using System;
using System.Collections.Generic;
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

        public string Name
        {
            get { return _name; }
            set { _name = value; }
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
        private decimal _costMoney;

        public decimal CostMoney
        {
            get { return _costMoney; }
            set { _costMoney = value; }
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
}