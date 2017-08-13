using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Common
{
    public class StreamHelper
    {
        public static string GetStrFromStream(NetworkStream networkStream)
        {
            try
            {
                string result = string.Empty;
                byte[] resultBytes = new byte[4096];
                int byteLength = 0;
                byteLength = networkStream.Read(resultBytes, 0, resultBytes.Length);
                result = Encoding.UTF8.GetString(resultBytes);
                return result.Trim('\0');
            }
            catch (Exception ex)
            {
                throw new Exception("转换流失败" + ex.Message);
            }
        }

        public static void SendStrToStream(NetworkStream networtStream, string message)
        {
            try
            {
                message = message.Trim('\0');
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                networtStream.Write(messageBytes, 0, messageBytes.Length);
                networtStream.Flush();
            }
            catch (Exception ex)
            {
                throw new Exception("发送数据到流失败" + ex.Message);
            }
        }
    }
}
