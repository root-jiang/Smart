using System.Drawing;
using System.Windows.Forms;

namespace FaultTreeAnalysis.MessageDialogUI
{
    /// <summary>
    /// 消息显示窗口
    /// </summary>
    public class MessageDialog
    {
        /// <summary>
        /// 显示消息窗口
        /// </summary>
        /// <param name="message">消息体</param>
        /// <param name="title">标题</param>
        /// <param name="button">按钮组合</param>
        /// <param name="defaultButton">默认选择按钮</param>
        /// <param name="textAlign">文本显示位置</param>
        /// <returns>对话框返回结果</returns>
        public static DialogResult Show(string message, string title = "", MessageBoxButtons button = MessageBoxButtons.OK, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1, ContentAlignment textAlign = ContentAlignment.MiddleCenter, MessageBoxIcon Icon = MessageBoxIcon.Information)
        {
            MessageDialogWindow dialog = new MessageDialogWindow(message, title, button, defaultButton, textAlign, Icon);
            dialog.TopMost = true;
            dialog.StartPosition = FormStartPosition.CenterScreen;
            return dialog.ShowDialog();
        }

        /// <summary>
        /// 显示消息窗口(大量信息)
        /// </summary>
        /// <param name="message">消息体</param>
        /// <param name="title">标题</param>
        /// <param name="button">按钮组合</param>
        /// <param name="defaultButton">默认选择按钮</param>
        /// <param name="textAlign">文本显示位置</param>
        /// <returns>对话框返回结果</returns>
        public static DialogResult ShowMax(string message, string title = "", MessageBoxButtons button = MessageBoxButtons.OK, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1, ContentAlignment textAlign = ContentAlignment.MiddleCenter, MessageBoxIcon Icon = MessageBoxIcon.Information)
        {
            MessageDialogWindow_Max dialog = new MessageDialogWindow_Max(message, title, button, defaultButton, textAlign, Icon);
            dialog.TopMost = true;
            dialog.StartPosition = FormStartPosition.CenterScreen;
            return dialog.ShowDialog();
        }

        /// <summary>
        /// 显示消息窗口（在异步线程调用）
        /// </summary>
        /// <param name="message">消息体</param>
        /// <param name="title">标题</param>
        /// <param name="button">按钮组合</param>
        /// <param name="defaultButton">默认选择按钮</param>
        /// <param name="textAlign">文本显示位置</param>
        /// <returns>对话框返回结果</returns>
        public static DialogResult ShowInAsync(string message, string title = "", MessageBoxButtons button = MessageBoxButtons.OK, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1, ContentAlignment textAlign = ContentAlignment.MiddleCenter, MessageBoxIcon Icon = MessageBoxIcon.Information)
        {
            MessageDialogWindow dialog = new MessageDialogWindow(message, title, button, defaultButton, textAlign, Icon);
            dialog.TopMost = true;
            dialog.StartPosition = FormStartPosition.CenterScreen;
            return dialog.ShowDialog();
        }
    }
}
