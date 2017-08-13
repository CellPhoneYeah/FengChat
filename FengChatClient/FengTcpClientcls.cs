using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Common;

///<summary>
///create by yxf 2017/8/10 10:57:39
///</summary>
namespace FengChatClient
{
    public class FengTcpClientcls
    {
        /// <summary>
        /// 客户端联系协议对象
        /// </summary>
        private TcpClient tcpClient;

        /// <summary>
        /// 接收消息处理方法类型
        /// </summary>
        /// <param name="message"></param>
        public delegate void ReceiveMsgHandler(string message);

        /// <summary>
        /// 接收消息处理事件
        /// </summary>
        public event ReceiveMsgHandler ReceiveMsgEvent;

        /// <summary>
        /// 登陆成功时执行事件类型
        /// </summary>
        /// <param name="showMessage"></param>
        public delegate void ConnectHandler();

        /// <summary>
        /// 连接成功时执行事件
        /// </summary>
        public event ConnectHandler ConnectEvent;

        /// <summary>
        /// 接收数据线程
        /// </summary>
        private Thread receiveMsgThread;

        public event ReceiveMsgHandler ExceptionShowEvent;

        /// <summary>
        /// 发送数据前配置端口并启动监听消息
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void SendConnectSet(string ip, int port)
        {
            IPAddress ipaddress = IPAddress.Parse(ip);
            tcpClient = new TcpClient();
            tcpClient.Connect(ipaddress, port);
            if (ConnectEvent != null)
                ConnectEvent();
            receiveMsgThread = new Thread(receiveMsgMethod);
            receiveMsgThread.IsBackground = true;
            receiveMsgThread.Start();
        }

        public bool SendMessage(string messageToSend)
        {
            try
            {
                NetworkStream curStream = tcpClient.GetStream();//获取数据流
                //StreamWriter writer = new StreamWriter(curStream);
                //writer.WriteLine(messageToSend);
                //writer.Flush();
                //curStream.Flush();
                StreamHelper.SendStrToStream(curStream, messageToSend);
                return true;
            }
            catch (Exception ex)
            {
                if (ExceptionShowEvent != null)
                    ExceptionShowEvent("发送消息失败"+ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 接收数据方法
        /// </summary>
        private void receiveMsgMethod()
        {
            try
            {
                while (true)
                {

                    if (!tcpClient.Connected)
                        break;
                    NetworkStream curStream = tcpClient.GetStream();
                    //StreamReader reader = new StreamReader(curStream);
                    //string messageStr = reader.ReadLine();
                    string messageStr = StreamHelper.GetStrFromStream(curStream);
                    if (ReceiveMsgEvent != null)
                        ReceiveMsgEvent(messageStr);
                }
            }
            catch (Exception ex)
            {
                if (ExceptionShowEvent != null)
                    ExceptionShowEvent(ex.Message);
            }
        }

        /// <summary>
        /// 停止连接服务器
        /// </summary>
        public void CloseClient()
        {
            this.tcpClient.Close();
        }
    }
}
