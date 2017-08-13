using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FengChatClient
{
    public partial class FengChatClientFrm : Form
    {
        private FengTcpClientcls chatClient;
        public FengChatClientFrm()
        {
            InitializeComponent();
            this.Load += FengChatClient_Load;
            this.FormClosed += FengChatClientFrm_FormClosed;
        }

        void FengChatClientFrm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Close();
        }

        void FengChatClient_Load(object sender, EventArgs e)
        {
            chatClient = new FengTcpClientcls();
            btnSend.Enabled = false;
            this.AcceptButton = btnSend;
            chatClient.ConnectEvent += chatClient_ConnectEvent;
            chatClient.ExceptionShowEvent += chatClient_ExceptionShowEvent;
        }

        void chatClient_ExceptionShowEvent(string message)
        {
            MessageBox.Show(message);
        }

        void chatClient_ConnectEvent()
        {
            ShowMessage("登陆成功");
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConnect_Click(object sender, EventArgs e)
        {
            string ip = txtServerIp.Text;
            string name = txtName.Text;
            int port;
            if (!int.TryParse(txtServerPort.Text, out port))
                MessageBox.Show("端口设置错误");
            if (string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(name))
            {
                MessageBox.Show("ip或者名字没有设置");
            }
            try
            {
                chatClient.SendConnectSet(ip, port);
                chatClient.ReceiveMsgEvent += chatClient_ReceiveMsgEvent;
                btnSend.Enabled = true;
                btnConnect.Enabled = false;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("连接失败" + ex.Message);
            }
        }

        void chatClient_ReceiveMsgEvent(string message)
        {
            try
            {

                if (this.InvokeRequired)//当主线程不在该线程时通过异步调用重新调用方法
                {
                    FengTcpClientcls.ReceiveMsgHandler update = new FengTcpClientcls.ReceiveMsgHandler(chatClient_ReceiveMsgEvent);
                    this.Invoke(update, new object[] { message });
                }
                else
                {
                    ShowMessage(message);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("接收数据发生错误：" + ex.Message);
            }

        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(rtbToSend.Text))
                {
                    MessageBox.Show("不能发送空消息");
                    return;
                }
                chatClient.SendMessage(rtbToSend.Text);
                rtbToSend.Clear();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("发送消息错误：" + ex.Message);
            }
        }

        private string ShowMessage(string curMessage, string userName = "")
        {
            StringBuilder sb = new StringBuilder();
            if (!curMessage.StartsWith("\r\n"))
                sb.Append("\r\n");
            string timeString = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            if (!string.IsNullOrEmpty(userName))
                sb.Append(userName + ":");
            sb.Append(timeString + "\r\n");
            sb.Append(txtName.Text + ":" + curMessage);
            mainMessage.Text += sb.ToString();
            return sb.ToString();
        }
    }
}
