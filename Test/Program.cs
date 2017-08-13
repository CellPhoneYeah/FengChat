using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseName = "游离绒毛膜性腺激素β亚基测定试剂盒时间分辨荧光法";
            baseName = CharacterToCoding(baseName);
            string nameLength = (baseName.Replace(" ", "").Length / 2).ToString("X");
            string test = "测试";
            test = CharacterToCoding(test);
            Console.Read();
        }

        private static string CharacterToCoding(string character)
        {
            string coding = "";
            for (int i = 0; i < character.Length; i++)
            {
                byte[] bytes = System.Text.Encoding.GetEncoding("GB2312").GetBytes(character.Substring(i, 1)); //取出二进制编码内容
                string lowCode = bytes[0].ToString("X"); //取出低字节编码内容（两位16进制）
                if (lowCode.Length == 1)
                {
                    lowCode = "0" + lowCode;
                }

                string hightCode = "00";
                if (bytes.Length > 1)
                {
                    hightCode = bytes[1].ToString("X"); ;//取出高字节编码内容（两位16进制）
                    if (hightCode.Length == 1)
                    {
                        hightCode = "0" + hightCode;
                    }
                }

                coding += (lowCode + " " + hightCode) + " ";//加入到字符串中,
            }
            return coding;
        }
    }
}
