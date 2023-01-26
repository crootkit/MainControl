using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text;

namespace LoginGraphics
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] StartFromDisp)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 通过添加启动参数，确保只能被调用而不是被单独使用 
            // 第一个参数：DevelopedByDRootkit123
            // 第二个参数：1234098(长度是7就行)
            if (StartFromDisp.Length == 2)
            {
                if (StartFromDisp[0].Length > 10 && "A478D4095F7603C241EB8DD6C03473AD".Equals(Encrypto(StartFromDisp[0])) && StartFromDisp[1].Length == 7)
                {
                    Application.Run(new JNCTF());
                }
            }
            else
            {
                System.Diagnostics.Process.Start("explorer.exe", "http://jwc.jiangnan.edu.cn");
            }


        }

        static string Encrypto(string s)
        {
            byte[] tmpBytes = Encoding.UTF8.GetBytes(s);
            byte[] tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpBytes);

            int i;
            StringBuilder sOutput = new StringBuilder(tmpHash.Length);
            for (i = 0; i < tmpHash.Length; i++)
            {
                sOutput.Append(tmpHash[i].ToString("X2"));
            }
            return sOutput.ToString();
        }
    }
}
