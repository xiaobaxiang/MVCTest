﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            int 数字 = 123;
            System.Console.WriteLine(数字);
            string ss = Console.ReadLine();
            string bb = doSomething(ss);
            Console.WriteLine(ss);
            Console.WriteLine(bb);
            Console.ReadKey();
        }

        public static string doSomething(string a)
        {
            a = a + "112233";
            return a;
        }
    }
}
