using System;
using System.Windows.Forms;
using System.Net;
using System.Text.RegularExpressions;

/*下面调用了其他的cs源码*/
using Sandbox7Vm;       // 检查虚拟机或者云环境
using SetStartup;       // 设置开机自启
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LoginGraphics
{
    public partial class JNCTF : Form
    {
        public JNCTF()
        {
            int res = Sandbox7Vm.PartMain.DeteEnvMain();
            if (res == 0)
            {
                // 没有任何错误，返回0，正常执行
                InitializeComponent();
            }
            else
            {
                MessageBox.Show("Using Me On Physical Machine", "Environment ERROR 0x0000");
                this.Close();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            ;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            ;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            ;
        }

        private void label3_Click(object sender, EventArgs e)
        {
            ;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ;
        }

        // 这是窗口的那个超链接
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "http://jnctf.jiangnan.edu.cn");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string studentId = textBox3.Text;
            string nickname = textBox2.Text;
            string name = textBox1.Text;

            if (studentId.Length == 0 || nickname.Length == 0 || name.Length == 0)
            {
                MessageBox.Show("Please Fill In Carefully!", "Empty Error");
                Environment.Exit(22);
            }

            // 信息格式不对，退出当前登录进程
            if(!checkstudentId(studentId) && !checkname(name))
            {
                MessageBox.Show("请填写真实的信息", "Wrong Info");
                Environment.Exit(11);
            }


            string data = $"studentId={studentId}" + "\n" + $"name={name}" + "\n" + $"nickname={nickname}" + "\n";
            try
            {
                // 将信息发送给监听80端口的服务器,并记录用户信息
                // 把这些内容写到authre.init文件中，方便ftp发包的时候标注
                //File.Create("authre.init");
                string user_info = $"name : {name}\nstuid : {studentId}\ntoken : {CreateToken(studentId)}";
                File.AppendAllText("authre.init", user_info);
                
                try
                {
                    using (var client = new WebClient())
                    {
                        client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        client.UploadString("http://82.156.14.45/", "POST", data);
                        //client.UploadString("http://192.168.220.150", "POST", data);
                    }
                    MessageBox.Show("Successfully authenticated your device and account\nHaving fun!!!\nJNCTF{H3110_Y0ung_Hack3r_We1c0me_2_7he_C0mp3t1t10n}", "认证成功");
                    
                    this.Close();
                }
                catch
                {
                    MessageBox.Show(">发生了一些错误\n>请重新填写\n>或联系管理员处理。", "UNknown Error 0x0001");
                    Environment.Exit(1);
                }

            }
            catch
            {
                // 虽然说上面有两个catch了，但是为了防止出现其他的问题，还是设置一个兜底的catch
                MessageBox.Show("发生了一些错误\n请联系管理员处理。", "UNknown Error 0x0003");
                Environment.Exit(3);
                //this.Close();
            }
        }

        // 检查学号的格式
        public static bool checkstudentId(string n)
        {
            // 首先判断长度
            if (n.Length < 10 || n.Length > 12) { return false; }

            // 然后判断是否包含特殊字符
            try
            {
                long id = long.Parse(n);
            }
            catch
            {
                return false;
            }

            // 判断开头的数字是不是1，2，6
            char[] data = n.ToCharArray();
            if (data[0] == '1' || data[0] == '2' || data[0] == '6' || data[0] == '7')
            {
                ;
            }
            else
            {
                return false;
            }
            return true;
        }

        // 检查姓名
        public static bool checkname(string n)
        {
            // 姓名长度不能低于2
            if (n.Length < 2) { return false; }

            // 判断是否有这些特殊字符
            if(n.Contains("!") || n.Contains("@") || n.Contains("%") || n.Contains("$") || n.Contains("#") || n.Contains("{")||n.Contains("}"))
            {
                return false;
            }

            // 判断是否全是中文
            bool isChinese = Regex.IsMatch(n, @"^[\u4e00-\u9fa5]+$");
            if(!isChinese) { return false; }

            return true;
        }

        // JNU+学号+jnctf+年份的md5是token
        public string CreateToken(string s)
        {
            s = "JNU" + s + "jnctf2022";
            byte[] tmpBytes = Encoding.UTF8.GetBytes(s);
            byte[] tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpBytes);

            int i;
            StringBuilder sOutput = new StringBuilder(tmpHash.Length);
            for (i = 0; i < tmpHash.Length; i++)
            {
                sOutput.Append(tmpHash[i].ToString("X2"));
            }
            // 32位大写的MD5值
            return sOutput.ToString();
        }
    }
}