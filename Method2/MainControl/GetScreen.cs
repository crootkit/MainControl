using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;


/*
 * 该类实现了获取屏幕截图
 * 保存屏幕截图
 * 将屏幕截图转化成为数组形势
 * https://blog.csdn.net/weixin_47954860/article/details/124968009 程序代码来源
 */

namespace getScreenShot
{
    class OneScreenCatClass
    {
        /* 引用freeconsole api来隐藏dos窗口 */
        [DllImport("kernel32.dll")]
        static extern bool FreeConsole();

        [DllImport("User32.dll")]
        private static extern int GetDC(IntPtr hDc);

        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hDc, int nIndex);

        private const int DESKTOPVERTRES = 117;
        private const int DESKTOPHORZRES = 118;

        // 获取真是屏幕宽高S
        public static Size GetScreenByDevice()
        {
            /*
                 PrimaryScreen函数会根据本机的设置进行缩放，也就会导致PrimaryScreen.bound.withd会获得当前缩放的屏幕比例。
                 GetDeviceCaps函数会一直获得本机实际的大小，获得的值是不随着缩放而变化的，所以这里采用的GetDeviceCaps函数。
             */
            IntPtr hDc = (IntPtr)GetDC(IntPtr.Zero);
            return new Size()
            {
                Width = GetDeviceCaps(hDc, DESKTOPHORZRES),
                Height = GetDeviceCaps(hDc, DESKTOPVERTRES)
            };
        }

        // 截取屏幕
        public static Bitmap CaptureScreenSnapshot()
        {
            Size sysize = GetScreenByDevice();
            Bitmap background = new Bitmap(sysize.Width, sysize.Height);
            Graphics gcs = Graphics.FromImage(background);
            gcs.CopyFromScreen(0, 0, 0, 0, sysize, CopyPixelOperation.SourceCopy);
            return background;
        }

        public static void SaveJpg(Bitmap jpg, string savepath)
        {
            jpg.Save(savepath);
        }

        /* 方法2
         * https://codedefault.com/s/how-to-convert-a-bitmap-into-a-byte-array-in-csharp-application 里面的第一个方法是可以的，这里用的是第一个方法
        */
        public static void Bitmap2byte(Bitmap ori)
        {
            var stream = new MemoryStream();

            ori.Save(stream, ImageFormat.Png);

            byte[] arry = stream.ToArray();

            string key = @"aqru;'[?094{AKZ|\%$@*&";

            for (int i = 0; i < arry.Length; i++)
            {
                arry[i] = (byte)(arry[i] ^ key[i % key.Length]);
            }

            // 设置保存截图的位置，初步定在%temp%目录，命名为SCnr.tmp
            //string logPath = @"D:\江南大学信息安全俱乐部\JNCTF-Monitor\Test2\SCnr.tmp";
            string logPath = Environment.GetEnvironmentVariable("temp");
            logPath += @"/SCnr.tmp";


            MakeScreenlog(arry, logPath);
        }

        static void MakeScreenlog(byte[] array, string OutPath)
        {
            FileStream file = new FileStream(OutPath, FileMode.Create, FileAccess.Write);
            BinaryWriter Out = new BinaryWriter(file);
            Out.Write(array);
            file.Close();
        }

        public static void ScreenShotMain()
        {
            //FreeConsole();
            Bitmap res = CaptureScreenSnapshot();
            Bitmap2byte(res);
            //SaveJpg(res, "testfun.png");
        }
    }
}