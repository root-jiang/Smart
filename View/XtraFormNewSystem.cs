using DevExpress.XtraEditors;
using FaultTreeAnalysis.Common;
using FaultTreeAnalysis.Model.Data;
using FaultTreeAnalysis.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FaultTreeAnalysis
{
    /// <summary>
    /// 让用户新建系统的窗体
    /// </summary>
    public partial class XtraFormNewSystem : XtraForm
    {
        /// <summary>
        /// 已有的系统名字
        /// </summary>
        private List<string> systemNames = null;

        /// <summary>
        /// 国际化字符串
        /// </summary>
        private StringModel ftaString = null;

        /// <summary>
        /// 国际化，设置文本框默认值
        /// </summary>
        /// <param name="systemNames">已有的系统名字</param>
        /// <param name="ftaString">国际化字符串</param>
        /// <param name="systemName">默认系统名字</param>
        public XtraFormNewSystem(List<string> systemNames, StringModel ftaString, string systemName = null)
        {
            General.TryCatch(() =>
            {
                this.ftaString = ftaString;
                InitializeComponent();

                #region 国际化
                this.labelControl_SystemName.Text = ftaString.SystemName;
                this.simpleButton_OK.Text = ftaString.OK;
                this.simpleButton_Cancel.Text = ftaString.Cancel;
                this.Text = ftaString.NewSystem;
                #endregion

                Icon = Icon.FromHandle(Resources.documentmap_32x32.GetHicon());
                if (!string.IsNullOrEmpty(systemName))
                {
                    textEdit_SystemName.Text = systemName;
                }

                this.systemNames = systemNames;
            });
        }

        /// <summary>
        /// 获取系统名
        /// </summary>
        /// <returns></returns>
        public string GetSystemName()
        {
            return General.TryCatch(() =>
            {
                string name = null;
                name = textEdit_SystemName.Text;
                if (name != null) name = name.Trim();
                return name;
            });
        }

        /// <summary>
        /// 新建系统窗体确认按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton_OK_Click(object sender, EventArgs e)
        {
            General.TryCatch(() =>
            {
                if (string.IsNullOrEmpty(textEdit_SystemName.Text) || string.IsNullOrEmpty(textEdit_SystemName.Text.Trim()))
                {
                    MsgBox.Show(ftaString.Systemnamecannotbeempty);
                    return;
                }
                else if (systemNames != null && systemNames.Contains(textEdit_SystemName.Text))
                {
                    MsgBox.Show(ftaString.Systemnamealreadyexists);
                    return;
                }
                else
                {
                    foreach (var val in textEdit_SystemName.Text)
                    {
                        if ("(，,。.？?！!：:;；*['\"@#$%/^&~\\])`=+-{}/|<>".IndexOf(val) > 0)
                        {
                            MsgBox.Show(General.FtaProgram.String.AllNameCheck);
                            return;
                        }
                    }
                }


                if (textEdit_SystemName.Text.Length > 100)
                {
                    MsgBox.Show(ftaString.SmarTreeNameCannotOver);
                    return;
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            });
        }

        /// <summary>
        /// 新建系统窗体取消按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton_Cancel_Click(object sender, EventArgs e)
        {
            General.TryCatch(() =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            });
        }
    }
}