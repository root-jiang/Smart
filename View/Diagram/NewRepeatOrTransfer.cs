using FaultTreeAnalysis.Common;
using FaultTreeAnalysis.Model.Data;
using FaultTreeAnalysis.Model.Enum;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FaultTreeAnalysis.View.Diagram
{
    /// <summary>
    /// 用于FTA图和表的新建重复事件或新建转移门菜单
    /// </summary>
    public partial class NewRepeatOrTransfer : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// 国际化菜单，设置可选id选项
        /// </summary>
        /// <param name="ftaString">国际化字符串</param>
        /// <param name="title">窗体标题</param>
        /// <param name="ids">让用户可选的id</param>
        public NewRepeatOrTransfer(StringModel ftaString, string title, List<string> ids)
        {
            General.TryCatch(() =>
            {
                InitializeComponent();
                simpleButton_OK.Text = ftaString.OK;
                simpleButton_Cancel.Text = ftaString.Cancel;
                simpleButton1.Text = ftaString.New;
                if (!string.IsNullOrEmpty(title)) this.Text = title;
                if (ids != null && ids.Count > 0)
                {
                    listBoxControl_id.Items.AddRange(ids.ToArray());
                    listBoxControl_id.SelectedIndex = 0;
                }

                if (title == General.FtaProgram.String.InsertRepeatedEvent)
                {
                    simpleButton1.Visible = false;
                }
                else
                {
                    simpleButton1.Visible = true;
                }
            });
        }

        /// <summary>
        /// 获取当前选中的id值
        /// </summary>
        /// <returns></returns>
        public string GetInfo()
        {
            return General.TryCatch(() =>
            {
                if (listBoxControl_id.SelectedItem != null && listBoxControl_id.SelectedItem.GetType() == typeof(string))
                {
                    return listBoxControl_id.SelectedItem as string;
                }
                return null;
            });
        }

        /// <summary>
        /// 从重复事件或转入门新建窗体确认按钮的点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton_OK_Click(object sender, EventArgs e)
        {
            General.TryCatch(() =>
            {
                if (listBoxControl_id.SelectedItem != null && listBoxControl_id.SelectedItem.GetType() == typeof(string))
                {
                    DialogResult = DialogResult.OK;
                    this.Close();
                }
            });
        }

        /// <summary>
        /// 从重复事件或转入门新建窗体取消按钮的点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton_Cancel_Click(object sender, EventArgs e)
        {
            General.TryCatch(() =>
            {
                DialogResult = DialogResult.Cancel;
                this.Close();
            });
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            General.TryCatch(() =>
            {
                DrawType type = DrawType.NULL;
                XtraFormNewFTAItem newFtaItem = new XtraFormNewFTAItem("New Transfer");

                if (newFtaItem.ShowDialog() == DialogResult.OK)
                {
                    type = newFtaItem.GetInfo();
                }
                else
                {
                    return;
                }

                DrawData root = General.FtaProgram.CurrentSystem.CreateOneRootItem(type);
                VirtualDrawData vData = General.FTATree.DataSource as VirtualDrawData;
                vData.data.Children.Add(root);
                General.FTATree.RefreshDataSource();

                //情况: 插入一个新顶层节点
                if ((null != General.FtaProgram)
                    && (null != General.FtaProgram.CurrentSystem))
                {
                    //做历史记录
                    General.FtaProgram.CurrentSystem.TakeBehavor(
                        root
                        , Behavior.Enum.ElementOperate.Creation
                        , Behavior.Enum.ElementOperate.Deletion
                        , null
                        , null);
                }

                listBoxControl_id.Items.Add(root.Identifier);
                listBoxControl_id.SelectedIndex = listBoxControl_id.ItemCount - 1;

                if (listBoxControl_id.SelectedItem != null && listBoxControl_id.SelectedItem.GetType() == typeof(string))
                {
                    DialogResult = DialogResult.OK;
                    this.Close();
                }
            }); 
        }
    }
}