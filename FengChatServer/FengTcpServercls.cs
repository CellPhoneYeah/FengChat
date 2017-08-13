using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.IO;
using Common;

///<summary>
///create by yxf 2017/8/10 11:20:23
///</summary>
namespace FengChatServer
{
    public class FengTcpServercls
    {
        public string ServerIp { get; private set; }

        public int ServerPort { get; private set; }
        /// <summary>
        /// 监听对象
        /// </summary>
        private TcpListener listener;

        /// <summary>
        /// 当前跟服务器连接的用户
        /// </summary>
        private Dictionary<string, TcpClient> clientList = new Dictionary<string, TcpClient>();

        /// <summary>
        /// 客户端连接事件方法类型
        /// </summary>
        /// <param name="sendMessage"></param>
        public delegate void ClientConnectHandler();

        /// <summary>
        /// 接收数据事件方法类型
        /// </summary>
        /// <param name="receiveMessage"></param>
        public delegate void ReceiveMessageHandler(string receiveMessage);

        public delegate void ServerStartListenHandler();

        public event ServerStartListenHandler ServerStartListenEvent;

        /// <summary>
        /// 客户端连接事件
        /// </summary>
        public event ClientConnectHandler ClientConnectEvent;

        /// <summary>
        /// 接收数据处理事件
        /// </summary>
        public event ReceiveMessageHandler ReceiveMessageEvent;

        public void LisenStart(int port)
        {
            IPAddress[] ipaddress = Dns.GetHostAddresses(Dns.GetHostName());
            listener = new TcpListener(ipaddress[3], port);
            Thread listenThread = new Thread(ListenMethod);
            listenThread.IsBackground = true;
            ServerIp = ipaddress[3].ToString();
            ServerPort = port;
            listenThread.Start();
            if (ServerStartListenEvent != null)
                ServerStartListenEvent();
        }

        /// <summary>
        /// 监听线程执行的方法
        /// </summary>
        /// <param name="obj"></param>
        private void ListenMethod(object obj)
        {
            listener.Stop();
            listener.Start();
            while (true)
            {
                try
                {
                    TcpClient curClient = listener.AcceptTcpClient();
                    if (this.ClientConnectEvent != null)
                        this.ClientConnectEvent();
                    Thread receiveThread = new Thread(ReceiveMethod);
                    receiveThread.Name = DateTime.Now.ToString();
                    receiveThread.IsBackground = true;
                    if (!clientList.ContainsKey(receiveThread.Name))
                        clientList.Add(receiveThread.Name, curClient);
                    receiveThread.Start();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 接收数据方法
        /// </summary>
        /// <param name="obj"></param>
        private void ReceiveMethod()
        {
            try
            {
                while (true)
                {
                    if (clientList == null || clientList.Count <= 0)
                        break;
                    NetworkStream curStream = clientList[Thread.CurrentThread.Name].GetStream();
                    //StreamReader reader = new StreamReader(curStream);
                    //string receiveMessage = reader.ReadLine();
                    string receiveMessage = StreamHelper.GetStrFromStream(curStream);
                    if (ReceiveMessageEvent != null)
                        ReceiveMessageEvent(receiveMessage);
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public void SendMessage(string sendMessage)
        {
            TcpClient tempClient = null;
            foreach (string key in clientList.Keys)
            {
                try
                {
                    tempClient = clientList[key];
                    if (!tempClient.Connected)
                    {
                        //clientList.Remove(key);
                        continue;
                    }
                    NetworkStream curStream = tempClient.GetStream();
                    //StreamWriter writer = new StreamWriter(curStream);
                    //writer.WriteLine(sendMessage);
                    //writer.Flush();
                    //curStream.Flush();
                    StreamHelper.SendStrToStream(curStream, sendMessage);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

    }
}
