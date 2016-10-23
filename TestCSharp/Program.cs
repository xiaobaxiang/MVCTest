using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using ServiceStack.Redis;

namespace TestCSharp
{
    public class Program
    {
        static void Main()
        {
            //WebClient wc=new WebClient();
            //wc.Encoding = Encoding.UTF8;
            //string s = wc.DownloadString("https://route.showapi.com/845-1?domain=www.mop.com&showapi_appid=23780&showapi_timestamp=20160828233537&showapi_sign=17c48144b9857e531e122a0a9b295005");
            //Console.WriteLine(s);

            #region 生成json的第二种方式:对于实体类重写tostring方法，然后list组合
            //Student s = new Student
            //{
            //    Age = 13,
            //    Name = "张三",
            //    Comment = "asdfas"
            //};
            //s.CourseList = new List<Course>();
            //Course c = new Course();
            //c.CourseID = 1;
            //c.CourseName = "aaa";
            //Course c2 = new Course();
            //c2.CourseID = 2;
            //c2.CourseName = "bbb";
            //Course c3 = new Course();
            //c3.CourseID = 3;
            //c3.CourseName = "ccc";
            //s.CourseList.Add(c);
            //s.CourseList.Add(c2);
            //s.CourseList.Add(c3);
            //Console.WriteLine(s.ToString());

            //Student s1 = new Student
            //{
            //    Age = 16,
            //    Name = "李四",
            //    Comment = "fff"
            //};
            ////s1.CourseList.Add(c2);
            ////s1.CourseList.Add(c);

            //List<Student> lst = new List<Student>();
            //lst.Add(s);
            //lst.Add(s1);

            //File.WriteAllText("e:\\aaa.txt", lst.ToJson());
            #endregion
            //Console.WriteLine(lst.ToJson());
            //二进制序列化
            //BinnerySerialODs(s);

            //使用SoapFormatter进行串行化
            // FileStream fs=new FileStream("e:\\Student.so",FileMode.OpenOrCreate);

            //使用XmlSerializer进行串行化
            //XMLSerialODs(s);

            //自定义序列化
            //OtherEmployeeClassTest();

            //string key = "zlh";
            ////清空数据库
            //RedisBase.Core.FlushAll();
            ////给list赋值
            //RedisBase.Core.PushItemToList(key, "1");
            //RedisBase.Core.PushItemToList(key, "2");
            //RedisBase.Core.AddItemToList(key, "3");
            //RedisBase.Core.PrependItemToList(key, "0");
            //RedisBase.Core.AddRangeToList(key, new List<string>() { "4", "5", "6" });
            #region 阻塞
            //启用一个线程来处理阻塞的数据集合
            //new Thread(new ThreadStart(RunBlock)).Start();
            #endregion

            #region redis
            //RedisClient Redis = new RedisClient("192.168.1.107", 6379);//redis服务IP和端口
            //Redis.AddItemToList("listUsers","zhangsan");//Add
            //Redis.AddItemToList("listUsers", "lisi");
            //Redis.AddItemToList("listUsers", "wangwu");

            //GET
            //Console.WriteLine(Redis.GetAllKeys().ToJson());
            //Console.WriteLine(Redis.GetListCount("listUsers") + Redis.GetAllItemsFromList("listUsers").ToJson());
            //Redis.GetAllItemsFromList("city").ForEach(e=> {Console.WriteLine(e.ToString());});
            //Console.WriteLine(Redis.Get<string>("city"));
            #endregion
            Console.ReadKey();
        }
        //public static void RunBlock()
        //{
        //    while (true)
        //    {
        //        //如果key为zlh的list集合中有数据，则读出，如果没有则等待2个小时，2个小时中只要有数据进入这里就可以给打印出来，类似一个简易的消息队列功能。
        //        Console.WriteLine(RedisBase.Core.BlockingPopItemFromList("zlh", TimeSpan.FromHours(2)));
        //    }
        //}

        private static void OtherEmployeeClassTest()
        {
            Employee mp = new Employee
            {
                EmpId = 10,
                EmpName = "邱枫",
                NoSerialString = "你好呀"
            };
            Stream steam = File.Open("e:\\empinfo.dat", FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            // XmlSerializer bf = new XmlSerializer(mp.GetType());
            Console.WriteLine("Output :" + "Writing Employee Info:");
            bf.Serialize(steam, mp);
            steam.Close();
            mp = null;
            //反序列化
            Stream steam2 = File.Open("e:\\empinfo.dat", FileMode.Open);
            BinaryFormatter bf2 = new BinaryFormatter();
            // XmlSerializer bf2 = new XmlSerializer(typeof(Employee));
            Console.WriteLine("Output :" + "Reading Employee Info:");
            Employee mp2 = (Employee)bf2.Deserialize(steam2);
            steam2.Close();
            Console.WriteLine("Output :" + mp2.EmpId);
            Console.WriteLine("Output :" + mp2.EmpName);
            Console.WriteLine("Output :" + mp2.NoSerialString);
        }

        private static void XMLSerialODs(Student s)
        {
            Stream fs = new FileStream("e:\\student.xml", FileMode.OpenOrCreate);
            XmlSerializer xs = new XmlSerializer(typeof(Student));
            xs.Serialize(fs, s);
            fs.Close();
            Console.WriteLine("序列化成功");

            fs = new FileStream("e:\\student.xml", FileMode.OpenOrCreate);
            Student s2 = (Student)xs.Deserialize(fs);
            Console.WriteLine("反序列化成功" + s2.Name);
            fs.Close();

        }

        private static void BinnerySerialODs(Student s)
        {
            FileStream fs = new FileStream("e:\\Student.dat", FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter(); //二进制序列化
            bf.Serialize(fs, s);
            Console.WriteLine("student被序列化了");
            fs.Dispose();

            FileStream fs2 = new FileStream("e:\\student.dat", FileMode.Open, FileAccess.Read);
            Student s2 = (Student)bf.Deserialize(fs2);//二进制序反列化
            Console.WriteLine("student被反列化了" + s2.Name);
            fs2.Close();
        }

    }

    [Serializable]
    public class Student
    {
        private int _age;

        public int Age
        {
            get { return _age; }
            set { _age = value; }
        }
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [NonSerialized]
        private string _comment;

        public string Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }

        public List<Course> CourseList;

        public override string ToString()
        {
            return "{\"Age\":\"" + Age + "\",\"Name\":\"" + Name + "\",\"Comment\":\"" + Comment + "\",\"CourseList\":" + (CourseList == null ? "null" : CourseList.ToJson()) + "}";
        }
    }

    public class Course
    {
        private int _courseid;
        public int CourseID { get { return _courseid; } set { _courseid = value; } }
        private string _coursename;
        public string CourseName { get { return _coursename; } set { _coursename = value; } }
        public override string ToString()
        {
            return "{\"CourseID\":\"" + CourseID + "\",\"CourseName\":\"" + CourseName + "\"}";
        }
    }

    [Serializable]
    public class Employee : ISerializable//自定义序列化(实现ISerializable接口)只对二进制序列化有效果NonSerialized才能起作用
    {
        public int EmpId = 100;
        public string EmpName = "work hard work smart work happy";
        [NonSerialized]
        public string NoSerialString = "NoSerialString-Test";
        public Employee()
        {
        }
        private Employee(SerializationInfo info, StreamingContext ctxt)
        {
            EmpId = (int)info.GetValue("EmpId", typeof(int));
            EmpName = (String)info.GetValue("EmpId", typeof(string));
            Console.WriteLine("反序列化生成对象");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("EmpId", EmpId);
            info.AddValue("EmpName", EmpName);
            Console.WriteLine("序列化对象");
        }
    }


    public static class ExtendMethod
    {
        public static string ToJson<T>(this List<T> lt)
        {
            //if(lt==null)
            //    return "{\"LstName\":\"objName\",\"LstCount\":\"0\",\"LstArray\":null]}";
            return "{\"LstName\":\"" + lt.GetType().Name + "\",\"LstCount\":\"" + lt.Count + "\",\"LstArray\":[" + string.Join(",", lt) + "]}";
        }
    }
}
