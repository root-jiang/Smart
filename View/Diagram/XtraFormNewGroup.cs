using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using FaultTreeAnalysis.Properties;
using FaultTreeAnalysis.Model.Data;
using FaultTreeAnalysis.Common;
using System.Data;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace FaultTreeAnalysis
{
    /// <summary>
    /// 让用户新建项目的图形窗口
    /// </summary>
    public partial class XtraFormNewGroup : XtraForm
    {
        public string GrouName = "";

        public List<string> GetGroups()
        {
            try
            {
                List<string> Groups = new List<string>();
                foreach (DataRow da in General.EventsLibDB.Rows)
                {
                    string Group = da["Group"]?.ToString();
                    if (!Groups.Contains(Group) && Group != "")
                    {
                        Groups.Add(Group);
                    }
                }
                return Groups;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// 添加工程窗体构造函数
        /// </summary>
        /// <param name="projectNames">已有项目名列表</param>
        /// <param name="ftaString">用于国际化的字符串对象</param>
        /// <param name="projectName">默认的项目名</param>
        /// <param name="projectPath">默认的路径</param>
        public XtraFormNewGroup(string GrouName = "")
        {
            General.TryCatch(() =>
            {
                this.GrouName = GrouName;
                InitializeComponent();
                Icon = Icon.FromHandle(Resources.project_32x32.GetHicon());

                if (string.IsNullOrEmpty(GrouName))
                {
                    textEdit_ProjectName.Text = GrouName;
                }

                textEdit_ProjectName.Properties.Items.Clear();
                textEdit_ProjectName.Properties.Items.AddRange(GetGroups());
            });
        }

        /// <summary>
        /// 新建项目窗体确认按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton_OK_Click(object sender, EventArgs e)
        {
            General.TryCatch(() =>
            {
                if (string.IsNullOrEmpty(textEdit_ProjectName.Text) || string.IsNullOrEmpty(textEdit_ProjectName.Text.Trim()))
                {
                    return;
                }
                foreach (var val in textEdit_ProjectName.Text)
                {
                    if ("(，,。.？?！!：:;；*['\"@#$%/^&~\\])`=+-{}/|<>".IndexOf(val) > 0)
                    {
                        MsgBox.Show(General.FtaProgram.String.AllNameCheck);
                        return;
                    }
                }

                this.GrouName = textEdit_ProjectName.Text;
                this.DialogResult = DialogResult.OK;
                this.Close();
            });
        }

        /// <summary>
        /// 新建项目窗体取消按钮点击事件
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

        private void XtraFormNewGroup_Load(object sender, EventArgs e)
        {

        }
    }
}