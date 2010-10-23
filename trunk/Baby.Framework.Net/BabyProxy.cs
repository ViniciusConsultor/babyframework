using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace Baby.Framework.Net
{
    public class BabyProxy
    {
        Socket clientSocket;//接收和返回
        byte[] read = null;//存储来自客户端请求数据包
        byte[] sendBytes = null;//存储中转请求发送的数据
        byte[] recvBytes = null;//存储中转请求返回的数据
        bool isConnect = false;
        byte[] qqSendBytes=new byte[4096];//QQ发送缓冲
        byte[] qqRecvBytes = new byte[4096];//QQ接收缓冲
        int sendLength = 0, recvLength = 0;//实际发送和接收长度
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="socket"></param>
        public BabyProxy(Socket socket)
        {
            clientSocket = socket;
            recvBytes = new Byte[1024 * 1024];
            clientSocket.ReceiveBufferSize = recvBytes.Length;
            clientSocket.SendBufferSize = recvBytes.Length;
        }
        
        /// <summary>
        /// 主运行代码
        /// </summary>
        public void Run()
        {
            #region 获取客户端请求数据
            read = new byte[clientSocket.Available];
            IPAddress ipAddress = IPAddress.Any;
            string host = "";//主机
            int port = 80;//端口

            int bytes = ReadMessage(read, ref clientSocket, ref ipAddress, ref host, ref port);
            if (bytes == 0)
            {
                //读取不到数据
                CloseSocket(clientSocket);
                return;
            }
            #endregion
            #region 创建中转Socket及建立连接
            IPEndPoint ipEndpoint = new IPEndPoint(ipAddress, port);
            Socket IPsocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                IPsocket.Connect(ipEndpoint); 
                //-----Socket 建立连接
            }
            catch (Exception err)
            {
                //连接失败
                //退出请求!!!
                CloseSocket(IPsocket, false);
                return;
            }

            #endregion
            if (isConnect)
            {
                byte[] qqOkData = QQokProxyData();
                clientSocket.Send(qqOkData, 0, qqOkData.Length, 0);
            }
            else
            {
                IPsocket.Send(sendBytes, 0);
            }

            #region 发送/接收中转请求

            int length = 0, count = 0;
            if (isConnect)
            {
                System.Threading.Thread.Sleep(100);//关键
                //循环发送客户端请求,接收服务器返回
                while (IPsocket.Available != 0 || clientSocket.Available != 0 || isConnect)
                {
                    if (!IPsocket.Connected || !clientSocket.Connected)
                    {
                        clientSocket.Close();
                        IPsocket.Close();
                        return;
                    }
                    try
                    {
                        while (clientSocket.Available != 0)
                        {
                            sendLength = clientSocket.Receive(qqSendBytes, qqSendBytes.Length, 0);
                            IPsocket.Send(qqSendBytes, sendLength, 0);
                            Console.WriteLine("发送字节数: " + sendLength.ToString());
                        }

                        System.Threading.Thread.Sleep(500);
                        while (IPsocket.Available != 0)
                        {
                            recvLength = IPsocket.Receive(qqRecvBytes, qqRecvBytes.Length, 0);

                            clientSocket.Send(qqRecvBytes, recvLength, 0);
                            Console.WriteLine("接收字节数: " + recvLength.ToString());
                            //System.Threading.Thread.Sleep(100);
                        }
                    }
                    catch
                    {
                        //throw;
                    }

                }

            }
            else
            {
                try
                {
                    do
                    {
                        length = IPsocket.Receive(recvBytes, count, IPsocket.Available, 0);
                        count = count + length;
                        //接收转发请求返回的数据中
                        System.Threading.Thread.Sleep(200);//关键点,请求太快数据接收不全
                    }
                    while (IPsocket.Available > 0);

                    clientSocket.Send(recvBytes, 0, count, 0);
                }
                catch(Exception ex)
                {
                    //throw;
                    Console.WriteLine(ex.Message);
                }

            }
            #endregion

            #region 结束请求,关闭客户端Socket
            //接收完成。返回客户端数据
            CloseSocket(IPsocket);
            CloseSocket(clientSocket);
            recvBytes = null;
            //本次请求完成,已关闭连接
            //-----------------------------请求结束---------------------------
            #endregion
        }
        
        /// <summary>
        /// 从请求头里解析出url和端口号
        /// </summary>
        /// <param name="clientmessage">客户端发送来的信息</param>
        /// <param name="port">端口</param>
        /// <returns>url</returns>
        private string GetUrl(string clientmessage, ref int port)
        {
            if (clientmessage.IndexOf("CONNECT") != -1)
            {
                isConnect = true;
            }
            int index1 = clientmessage.IndexOf(' ');
            int index2 = clientmessage.IndexOf(' ', index1 + 1);
            if ((index1 == -1) || (index2 == -1))
            {
                return "";
            }
            string part1 = clientmessage.Substring(index1 + 1, index2 - index1).Trim();
            string url = string.Empty;
            if (!part1.Contains("http://"))
            {
                if (part1.Substring(0, 1) == "/")
                {
                    part1 = "127.0.0.1" + part1;
                }
                part1 = "http://" + part1;
            }
            Uri uri = null;
            try
            {
                uri = new Uri(part1);
            }
            catch
            {
                return "";
            }
            url = uri.Host;
            port = uri.Port;
            return url;
        }
        
        /// <summary>
        /// 接收客户端的HTTP请求数据
        /// </summary>
        /// <param name="readByte"></param>
        /// <param name="s"></param>
        /// <param name="ipAddress"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        private int ReadMessage(byte[] readByte, ref Socket s, ref IPAddress ipAddress, ref string host, ref int port)
        {
            try
            {
                int bytes = s.Receive(readByte, readByte.Length, 0);
                //收到原始请求数据
                string header = Encoding.ASCII.GetString(readByte);

                host = GetUrl(header, ref port);
                if (Filter(host))
                {
                   //系统过滤
                    return 0;
                }
                //Write(header);
                ipAddress = Dns.GetHostAddresses(host)[0];
                if (!isConnect)
                {
                    header = header.Replace("http://" + host, "");
                }
                sendBytes = Encoding.ASCII.GetBytes(header);
                System.Threading.Thread.Sleep(50);
                //转发请求数据
                //Write(Encoding.ASCII.GetString(sendBytes));
                return bytes;
            }
            catch
            {
                System.Threading.Thread.Sleep(300);
                return 0;
            }
        }
        
        /// <summary>
        /// 关闭socket
        /// </summary>
        /// <param name="socket">要关闭的socket</param>
        private void CloseSocket(Socket socket)
        {
            CloseSocket(socket, true);
        }

        private void CloseSocket(Socket socket, bool shutdown)
        {
            if (socket != null)
            {
                if (shutdown)
                {
                    socket.Shutdown(SocketShutdown.Both);
                }
                socket.Close();
            }
        }
        
        /// <summary>
        /// QQ代理测试返回
        /// </summary>
        /// <returns></returns>
        private byte[] QQokProxyData()
        {
            string data = "HTTP/1.0 200 Connection established";//返回建立成功
            return System.Text.Encoding.ASCII.GetBytes(data);
        }
        
        /// <summary>
        /// firfox默认会发送一些请求，很烦，所以加过滤
        /// </summary>
        /// <param name="url">要过滤的地址</param>
        /// <returns>true 过滤；false 不过滤</returns>
        private bool Filter(string url)
        {
            switch (url.ToLower())
            {
                case "fffocus.cn":
                    return true;
            }
            return false;
        }
    }
}
