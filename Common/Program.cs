using Aspect.AddinFramework;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace FaultTreeAnalysis
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //注册试用期
            //if (!Validity.CheckClientValidity(out string message))
            //{
            //    MessageBox.Show(message ?? "Client license has expired!", "Information");
            //    return;
            //}

            //使得单进程
            Process[] ps = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
            if (ps.Length > 1)
            {
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
