using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace Baby.Framework.Net
{
    public class BabySocket
    {
        public static void Listen(int port)
        {
            //准备监听端口
            System.Net.IPAddress[] ips = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName());
            System.Net.IPAddress ipp = System.Net.IPAddress.Any;
            TcpListener tcplistener = new TcpListener(ipp, port);
            //tcplistener.ExclusiveAddressUse = false;
            tcplistener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            //tcplistener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.,ProtocolType.IP);
            try
            {
                tcplistener.Start();
            }
            catch (Exception err)
            {
                ReListen(tcplistener);
                Console.WriteLine(err.Message);
                throw new Exception("该端口已被占用,请更换端口号");
            }

           //确认:y/n (yes or no)
            string isOK = "y";// Console.ReadLine();
            if (isOK == "y")
            {
                //成功监听端口
                //侦听端口号 
                Socket socket;
                while (true)
                {
                    socket = tcplistener.AcceptSocket();
                    //并获取传送和接收数据的Scoket实例 
                    BabyProxy proxy = new BabyProxy(socket);
                    //Proxy类实例化 
                    Thread thread = new Thread(new ThreadStart(proxy.Run));
                    //创建线程 
                    thread.Start();
                    // System.Threading.Thread.Sleep(1000);
                    //启动线程 
                }
            }
            else
            {
                ReListen(tcplistener);
            }
        }

        public static void ReListen(TcpListener listener)
        {
            if (listener != null)
            {
                listener.Stop();
                listener = null;
            }

            Console.WriteLine("请输入监听端口号");
            string newPort = Console.ReadLine();
            int port;
            if (int.TryParse(newPort, out port))
            {
                Listen(port);
            }
            else
            {
                ReListen(listener);
            }
        }
    }
}
