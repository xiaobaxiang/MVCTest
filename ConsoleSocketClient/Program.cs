using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleSocketClient
{
    class Program
    {
        private static Thread threadClient = null;//客户端 负责接收 服务端发来数据消息的线程
        private static Socket socketClient = null;//客户端套接字
        static void Main(string[] args)
        {
            //1.客户端发送连接请求到服务器
            //1.1 创建endpoint
            IPAddress address = IPAddress.Parse("192.168.1.107");
            IPEndPoint endPoint = new IPEndPoint(address, 10000);

            //1.2 创建客户端套接字
            socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //向指定的 ip和端口 发送连接请求
            socketClient.Connect(endPoint);
            Console.WriteLine("客户端连接成功......");

            //2.客户端创建线程 监听服务器发送的消息
            threadClient = new Thread(new Program().RecMsg);
            threadClient.IsBackground = true;
            threadClient.Start();
            Console.WriteLine(socketClient.LocalEndPoint.ToString() + "客户端启动了......");

            //Console.WriteLine("回发消息到服务器......");
            //string content = Console.ReadLine();
            //if (!string.IsNullOrWhiteSpace(content))
            //{
            //    byte[] sendBytes = new byte[content.Length + 1];
            //    sendBytes[0] = 0;
            //    System.Text.Encoding.UTF8.GetBytes(content).CopyTo(sendBytes, 1);
            //    socketClient.Send(sendBytes);
            //}
            Console.ReadKey();
        }

        /// <summary>
        /// 监听服务器端发来的消息
        /// </summary>
        void RecMsg()
        {
            while (true)
            {
                Console.WriteLine("回发消息到服务器......");
                string content = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(content))
                {
                    int byteLength = System.Text.Encoding.UTF8.GetBytes(content).Length;
                    byte[] sendBytes = new byte[byteLength + 1];
                    sendBytes[0] = 0;
                    System.Text.Encoding.UTF8.GetBytes(content).CopyTo(sendBytes, 1);
                    socketClient.Send(sendBytes);
                }

                //定义一个接收用的缓冲区 (2M字节的数组)
                byte[] arrMsgRec = new byte[1024 * 1024 * 2];

                //将接收的0数据 存入 arrMsgRec 数组，并返回真正接收到的数据的长度
                int length = socketClient.Receive(arrMsgRec);//这个方法会阻塞执行
                //此时将数组中的所有元素都转成字符串，而真正接收到的 只是服务器端发送来的 几个字符
                string strMsgRec = System.Text.Encoding.UTF8.GetString(arrMsgRec, 0, length);
                Console.WriteLine("接收到服务器端发送的消息......");
                Console.WriteLine(strMsgRec);

            }
        }
    }
}
