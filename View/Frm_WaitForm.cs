using System;
using DevExpress.XtraWaitForm;

namespace IntegratedSystem.View
{
    public partial class Frm_WaitForm : WaitForm
    {
        public Frm_WaitForm()
        {
            InitializeComponent();
            this.progressPanel1.AutoHeight = true;
        }

        #region Overrides 

        public bool isShowPercent = false;

        public override void SetCaption(string caption)
        {
            base.SetCaption(caption);
            this.progressPanel1.Caption = caption;
        }

        public override void SetDescription(string description)
        {
            base.SetDescription(description);
            this.progressPanel1.Description = description;
        }

        public override void ProcessCommand(Enum cmd, object arg)
        {
            base.ProcessCommand(cmd, arg);

            bool SetBool = true;
            if (arg != null && arg.GetType() == typeof(bool))
            {
                SetBool = (bool)arg;
            }
            switch (cmd)
            {
                case WaitFormEnum.WaitFormCommand.SetProgressPanel_ShowHide:
                    this.progressPanel1.Visible = SetBool;
                    break;
                case WaitFormEnum.WaitFormCommand.SetProgressPanelDescription_ShowHide:
                    this.progressPanel1.ShowDescription = SetBool;
                    break;
                case WaitFormEnum.WaitFormCommand.SetProgressBar_ShowHide:
                    this.progressBarControl1.Visible = SetBool;
                    break;
                case WaitFormEnum.WaitFormCommand.SetProgressShowMode_Percent:
                    isShowPercent = SetBool;
                    if (isShowPercent)
                    {
                        progressBarControl1.Properties.PercentView = true;
                    }
                    else
                    {
                        progressBarControl1.Properties.PercentView = false;
                    }
                    break;
                case WaitFormEnum.WaitFormCommand.SetProgressValue:

                    if (arg != null && arg.GetType() == typeof(Int32[]))
                    {
                        int val = ((Int32[])arg)[0];
                        int Max_val = ((Int32[])arg)[1];
                        this.progressBarControl1.Properties.Maximum = Max_val;
                        this.progressBarControl1.EditValue = val;
                    }
                    break;
                default:
                    break;
            }
        }

        #endregion

        private void progressBarControl1_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                if (!isShowPercent)
                {
                    e.DisplayText = (Math.Round((Convert.ToDouble(this.progressBarControl1.EditValue) / Convert.ToDouble(this.progressBarControl1.Properties.Maximum)) * 100, 2)).ToString() + "%，" + this.progressBarControl1.EditValue.ToString() + "/" + this.progressBarControl1.Properties.Maximum.ToString();
                }
            }
            catch (Exception)
            {
            }
        }
    }


    public class WaitFormEnum
    {
        public enum WaitFormCommand
        {
            /// <summary>
            /// 设置（显示/隐藏）标题
            /// </summary>
            SetProgressPanel_ShowHide,
            /// <summary>
            /// 设置（显示/隐藏）标题描述
            /// </summary>
            SetProgressPanelDescription_ShowHide,
            /// <summary>
            /// 设置（显示/隐藏）进度条
            /// </summary>
            SetProgressBar_ShowHide,
            /// <summary>
            /// 设置进度条文本显示值是否百分比
            /// </summary>
            SetProgressShowMode_Percent,
            /// <summary>
            /// 设置进度条值
            /// </summary>
            SetProgressValue
        }
    }
}
