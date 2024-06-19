using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace FaultTreeAnalysis.Model.Data
{
    /// <summary>
    /// 用于发送win32固定WM_COPYDATA消息，实现进程间传递数据
    /// </summary>
    public class CSendMessage
    {
        /// <summary>
        /// win32发送数据的固定的结构体
        /// </summary>
        private struct COPYDATASTRUCT
        {
            public int operationType; //随意指定值
            public int strLength; //字符数组大小+1
            [MarshalAs(UnmanagedType.LPStr)]
            public string strData;//字符串
        }

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern bool SendMessage
        (
        IntPtr hWnd,        // 信息发住的窗口的句柄
        int Msg,            // 消息ID
        int wParam,         // 参数1
        ref COPYDATASTRUCT lParam // 参数2   [MarshalAs(UnmanagedType.LPTStr)]StringBuilder lParam
        );

        /// <summary>
        /// 固定win32复制数据的消息值
        /// </summary>
        public const int WM_COPYDATA = 0x004A;

        /// <summary>
        /// 发送消息到指定win32窗口，可以发数字，字符串
        /// </summary>
        /// <param name="target">目标窗体句柄</param>
        /// <param name="wParam">当前使用的标志</param>
        /// <param name="operationType">后期扩展使用</param>
        /// <param name="str">传递的字符串信息</param>
        /// <returns></returns>
        public static bool SendMessage(IntPtr target, int wParam, int operationType, string str)
        {
            if (str == null) str = "";
            COPYDATASTRUCT data = new COPYDATASTRUCT();
            data.operationType = operationType;
            data.strLength = Encoding.Default.GetBytes(str).Length + 1;//中文需要这样
            data.strData = str;
            return SendMessage(target, WM_COPYDATA, wParam, ref data);
        }
        /// <summary>
        /// 解析收到的WM_COPYDATA信息
        /// </summary>
        /// <param name="m">收到的消息</param>
        /// <param name="wParam">当前使用的标志</param>
        /// <param name="operationType">后期扩展使用</param>
        /// <param name="str">传递的字符串信息</param>
        public static void ParseMessage(Message m, out int wParam, out int operationType, out string str)
        {
            COPYDATASTRUCT ms = (COPYDATASTRUCT)m.GetLParam(typeof(COPYDATASTRUCT));
            wParam = (int)m.WParam;
            operationType = ms.operationType;
            str = ms.strData;
        }

    }
}
