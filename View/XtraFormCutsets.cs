using FaultTreeAnalysis.Common;
using FaultTreeAnalysis.Model.Enum;
using System;
using System.IO;
using System.Windows.Forms;

namespace FaultTreeAnalysis
{
    /// <summary>
    /// 实现FTA表和图的插入功能界面
    /// </summary>
    public partial class XtraFormCutsets : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// 设置可选的图形种类，以及标题
        /// </summary>
        /// <param name="title">窗口的标题</param>
        public XtraFormCutsets(string title = "")
        {
            InitializeComponent();

            if (title != "")
                this.Text = title;
        }

        /// <summary>
        /// 确定按钮关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton_OK_Click(object sender, EventArgs e)
        {
            try
            {
                string savingDataPath = $"{AppDomain.CurrentDomain.BaseDirectory}" + "//MaximumOrderSetting.txt";
                File.WriteAllText(savingDataPath, comboBoxEdit_FTAItemType.Text);
            }
            catch (Exception)
            {
            }
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

        private void XtraFormCutsets_Load(object sender, EventArgs e)
        {
            try
            {
                this.Text = General.FtaProgram.String.CusetStr;
                labelControl1.Text = General.FtaProgram.String.CusetStr + ":";
                simpleButton_OK.Text = General.FtaProgram.String.OK;
                simpleButton_Cancel.Text = General.FtaProgram.String.Cancel;

                string maximumorder = "";
                try
                {
                    string savingDataPath = $"{AppDomain.CurrentDomain.BaseDirectory}" + "//MaximumOrderSetting.txt";
                    if (File.Exists(savingDataPath))
                    {
                        maximumorder = File.ReadAllText(savingDataPath);
                    }
                    else
                    {
                        File.WriteAllText(savingDataPath, "7");
                        maximumorder = File.ReadAllText(savingDataPath);
                    }
                }
                catch (Exception)
                {
                    maximumorder = "";
                }
                comboBoxEdit_FTAItemType.Text = maximumorder;
            }
            catch (Exception)
            {
            }
        }
    }
}