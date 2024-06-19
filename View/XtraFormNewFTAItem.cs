using FaultTreeAnalysis.Common;
using FaultTreeAnalysis.Model.Enum;
using System;
using System.Windows.Forms;

namespace FaultTreeAnalysis
{
    /// <summary>
    /// 实现FTA表和图的插入功能界面
    /// </summary>
    public partial class XtraFormNewFTAItem : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// 设置可选的图形种类，以及标题
        /// </summary>
        /// <param name="title">窗口的标题</param>
        public XtraFormNewFTAItem(string title = "")
        {
            InitializeComponent();

            string[] Types = Enum.GetNames(typeof(DrawType));

            DrawData da = new DrawData();
            da.Type = DrawType.AndGate;


            if (title == "New Level")
            {
                da.Type = DrawType.AndGate;
                da.Children.Add(new DrawData());
            }
            else if (title == "New Transfer")
            {
                da.Type = DrawType.AndGate;
                da.Children.Add(new DrawData());
            }
            else
            {
                da.Type = DrawType.AndGate;
            }

            comboBoxEdit_FTAItemType.Properties.Items.AddRange(da.GetAvailableTypeSource(General.FtaProgram.CurrentSystem, General.FtaProgram.String));
            this.comboBoxEdit_FTAItemType.SelectedIndex = 1;
            if (title != "")
                this.Text = title;
        }

        /// <summary>
        /// 设置下拉框默认选择的类型
        /// </summary>
        /// <param name="type">图形类型</param>
        public void SetInfo(DrawType type)
        {
            this.comboBoxEdit_FTAItemType.SelectedItem = DrawData.GetDescriptionByEnum(type);
        }

        /// <summary>
        /// 通过选中项的名字获得要创建的图形或节点类型
        /// </summary>
        /// <returns></returns>
        public DrawType GetInfo()
        {
            var typeName = General.GetKeyName(this.comboBoxEdit_FTAItemType.SelectedItem as string);
            return (DrawType)Enum.Parse(typeof(DrawType), typeName);
        }

        /// <summary>
        /// 确定按钮关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton_OK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// 取消按钮，关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void XtraFormNewFTAItem_Load(object sender, EventArgs e)
        {
            try
            {
                this.Text = General.FtaProgram.String.NewFTAItem_Text;
                labelControl1.Text = General.FtaProgram.String.NewFTAItem_Label;
                simpleButton_OK.Text = General.FtaProgram.String.OK;
                simpleButton_Cancel.Text = General.FtaProgram.String.Cancel;
            }
            catch (Exception)
            {
            }
        }
    }
}