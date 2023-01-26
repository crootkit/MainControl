using System;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;

namespace Sandbox7Vm
{
    ///* 尝试定义一个全局变量 */
    //public static class GlobalVar{public static void GVar(int i){int RES = i;}}

    /* 当前系统；磁盘大小；CPU的信息 */
    class DetectHardware
    {
        // 检测是否是 微软的云服务；vmware虚拟机；微步(服务器CPU),基本只能挡一些很垃圾的，一般防不住
        public int OSvering()
        {
            string systemName = Environment.OSVersion.ToString();

            if (systemName.Contains("Microsoft Azure") || systemName.Contains("VMware") || systemName.Contains("Service Pack"))
            {
                return 1;
            }
            return 0;
        }

        // 检查磁盘大小， 256， >=512， 
        public int DiskInfo()
        {
            long totalSize = 0;

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");

            foreach (ManagementObject drive in searcher.Get())
            {
                long size = long.Parse(drive["Size"].ToString());
                totalSize += size;
            }
            // 计算多少G
            totalSize = totalSize / 1024 / 1024 / 1024;

            return (totalSize <= 200 ? 1 : 0);
        }

        // CPU的信息,不是一个cpu的设备考虑为虚拟机
        public int CPUinfo()
        {
            int totalCPUnum = 0;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            foreach (ManagementObject mo in searcher.Get())
            {
                //Console.WriteLine("CPU type: {0}", mo["Name"]);
                totalCPUnum++;
            }
            return (totalCPUnum > 1 ? 1 : 0);
        }
    }

    /*网卡信息；Mac地址；能否通网*/
    class NetInfo
    {
        // 检测网络环境，这个挡不住
        public int NetworkensInfo()
        {
            int res = 0;
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in interfaces)
            {
                PhysicalAddress address = ni.GetPhysicalAddress();
                if (address.ToString().StartsWith("00:15:5D"))
                {
                    res++;
                    break;
                }
                else if (address.ToString().StartsWith("00:0D:3A"))
                {
                    res++;
                    break;
                }
            }
            return (int)res;
        }

        // 检测本地mac地址，但是也没啥用
        public void MacInfo()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration");
            Random random = new Random();
            foreach (ManagementObject mo in searcher.Get())
            {
                if (mo["MACAddress"] != null)
                {
                    //Console.WriteLine("MAC address: {0}", mo["MACAddress"]);
                    Thread.Sleep(random.Next(400, 500));
                }
            }
        }

        // 检测网络是否联通,联通返回0，否则返回1
        public int IsNetworing()
        {
            string targetHost = "www.baidu.com";      // 测试用
            //string targetHost = "http://jnctf.jiangnan.edu.cn"; 

            Ping pingSender = new Ping();
            PingOptions options = new PingOptions();

            // Set the ping options:
            // Don't fragment the packet.
            options.DontFragment = true;

            // Create a buffer of 32 bytes of data to be transmitted.
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 100;

            PingReply reply = pingSender.Send(targetHost, timeout, buffer, options);

            return (reply.Status == IPStatus.Success ? 0 : 1);
        }
    }

    /* 判断当前进程中是否存在虚拟机相关进程[关键] */
    class DetectProcess
    {
        public int ProcessMon()
        {
            Process[] processes = Process.GetProcesses();

            foreach (Process process in processes)
            {
                if (process.ProcessName.Contains("VMware Tools"))
                {
                    //Console.WriteLine($"run me in vmware");
                    return 1;
                    //break;
                }
            }
            return 0;
            //Console.WriteLine("hello");
        }
    }

    class PartMain
    {
        public static int DeteEnvMain()
        {
            int RES = 0;

            DetectHardware hard = new DetectHardware();
            DetectProcess proc = new DetectProcess();
            NetInfo netinfo = new NetInfo();

            RES += netinfo.IsNetworing();
            //Console.WriteLine("check network connection " + " finish " + $"{RES}");
            if (RES == 1)
            {
                MessageBox.Show("\nplz check your Internet", "NET ERROR 0xFFF");
                Environment.Exit(0);
            }

            RES += proc.ProcessMon();
            //Console.WriteLine("check process " + " finish " + $"{RES}");

            RES += hard.DiskInfo();
            //Console.WriteLine("check disk space" + " finish " + $"{RES}");

            RES += hard.CPUinfo();
            //Console.WriteLine("check CPU number" + " finish " + $"{RES}");

            RES += hard.OSvering();
            //Console.WriteLine("check system" + " finish " + $"{RES}");

            if (RES > 0)
            {
                //Console.WriteLine("you are IN vmware or sandbox");
            }
            else
            {
                //Console.WriteLine("you are NOT IN vmware or sandbox");
                netinfo.MacInfo();
            }
            //Console.ReadKey();
            return RES;
        }
    }
}