using Microsoft.Win32;
using System.IO;

namespace SetStartup
{
    class StartUp
    {
        // 这里的路径是需要自启的文件路径，不包括那个GUI登录界面
        public static int Set_StartUp(string fullpath)
        {
            // 应用程序的路径,自动获取当前程序的完整路径带文件名。
            //string fullPath = Path.GetFullPath(System.Reflection.Assembly.GetEntryAssembly().Location);
            string appPath = fullpath;

            // 设置启动项的键名
            string keyName = "Micrsosft Authentication";

            // 获取启动项所在的注册表路径
            string runKeyPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

            // 获取启动项对应的注册表键
            RegistryKey runKey = Registry.CurrentUser.OpenSubKey(runKeyPath, true);

            // 在注册表中添加启动项
            runKey.SetValue(keyName, appPath);

            // 关闭注册表
            runKey.Close();

            return 0;
        }
    }
}