using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FengChatServer
{
    public partial class FengChatServer : Form
    {
        private FengTcpServercls fengServer;

        public FengChatServer()
        {
            InitializeComponent();
            this.Load += FengChatServer_Load;
        }

        void FengChatServer_Load(object sender, EventArgs e)
        {
            try
            {
                txtPort.Text = "8099";
                fengServer = new FengTcpServercls();
                fengServer.ClientConnectEvent += fengServer_ConnectEvent;
                fengServer.ReceiveMessageEvent += fengServer_ReceiveMessageEvent;
                fengServer.ServerStartListenEvent += fengServer_ServerStartListenEvent;
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载出错" + ex.Message);
            }
        }

        void fengServer_ServerStartListenEvent()
        {
            ShowMessage("开始监听\r\n");
        }

        private string ShowMessage(string curMessage,string userName = "")
        {
            StringBuilder sb = new StringBuilder();
            if (!curMessage.StartsWith("\r\n"))
                sb.Append("\r\n");
            string timeString = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            if (!string.IsNullOrEmpty(userName))
                sb.Append(userName + ":");
            sb.Append(timeString+"\r\n");
            sb.Append(curMessage);
            rtbMain.Text += sb.ToString();
            return sb.ToString();
        }

        void fengServer_ReceiveMessageEvent(string receiveMessage)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    FengTcpServercls.ReceiveMessageHandler update = new FengTcpServercls
                        .ReceiveMessageHandler(fengServer_ReceiveMessageEvent);
                    this.Invoke(update, receiveMessage);
                }
                else
                {
                    ShowMessage(receiveMessage);
                    fengServer.SendMessage(receiveMessage);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void fengServer_ConnectEvent()
        {
            try
            {
                if (InvokeRequired)
                {
                    FengTcpServercls.ClientConnectHandler update = new FengTcpServercls.ClientConnectHandler(fengServer_ConnectEvent);
                    this.Invoke(update);
                }
                else
                {
                    ShowMessage("用户连接成功连接成功");
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("连接错误" + ex.Message);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPort.Text))
            {
                MessageBox.Show("未设置服务端口！");
                return;
            }
            int port;
            if (!int.TryParse(txtPort.Text, out port))
            {
                MessageBox.Show("服务端口设置错误，只能是0~65535！");
                return;
            }
            fengServer.LisenStart(port);
            btnStart.Enabled = false;
        }
    }
}
