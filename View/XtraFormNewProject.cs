using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using FaultTreeAnalysis.Properties;
using FaultTreeAnalysis.Model.Data;
using FaultTreeAnalysis.Common;

namespace FaultTreeAnalysis
{
    /// <summary>
    /// 让用户新建项目的图形窗口
    /// </summary>
    public partial class XtraFormNewProject : XtraForm
    {
        /// <summary>
        /// 已有项目名列表，防止重名
        /// </summary>
        private List<string> projectNames = null;

        /// <summary>
        /// 用于国际化的字符串对象
        /// </summary>
        private StringModel ftaString = null;

        /// <summary>
        /// 添加工程窗体构造函数
        /// </summary>
        /// <param name="projectNames">已有项目名列表</param>
        /// <param name="ftaString">用于国际化的字符串对象</param>
        /// <param name="projectName">默认的项目名</param>
        /// <param name="projectPath">默认的路径</param>
        public XtraFormNewProject(List<string> projectNames, StringModel ftaString, string projectName = null, string projectPath = null)
        {
            General.TryCatch(() =>
            {
                this.ftaString = ftaString;
                InitializeComponent();
                Icon = Icon.FromHandle(Resources.project_32x32.GetHicon());

                #region 国际化
                this.labelControl_ProjectName.Text = ftaString.ProjectName;
                this.simpleButton_OK.Text = ftaString.OK;
                this.simpleButton_Cancel.Text = ftaString.Cancel;
                //this.labelControl_ProjectPath.Text = ftaString.Path;
                this.Text = ftaString.NewProject;
                #endregion

                if (string.IsNullOrEmpty(projectName))
                {
                    textEdit_ProjectName.Text = projectName;
                }

                if (string.IsNullOrEmpty(projectPath))
                {
                    //textEdit_ProjectPath.Text = projectPath;
                }

                this.projectNames = projectNames;
            });
        }

        /// <summary>
        /// 获得项目名称
        /// </summary>
        /// <returns></returns>
        public string GetProjectName()
        {
            return General.TryCatch(() =>
            {
                string name = null;
                name = textEdit_ProjectName.Text;
                if (name != null) name = name.Trim();
                return name;
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
                    MsgBox.Show(ftaString.ProjectNameCannotBeEmpty);
                    return;
                }
                else if (projectNames != null && projectNames.Contains(textEdit_ProjectName.Text))
                {
                    MsgBox.Show(ftaString.Projectnamealreadyexists);
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

                if (textEdit_ProjectName.Text.Length > 100)
                {
                    MsgBox.Show(ftaString.ProjectNameCannotOver);
                    return;
                }

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
    }
}