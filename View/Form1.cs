using DevExpress.XtraBars.Ribbon;
using FaultTreeAnalysis.Properties;
using System;
using System.Windows.Forms;
using System.ComponentModel;
using FaultTreeAnalysis.Common;
using System.Collections.Generic;
using DevExpress.XtraBars;
using FaultTreeAnalysis.Model.Data;

namespace FaultTreeAnalysis
{
    /// <summary>
    /// 简单的窗体外壳，盛放FTA控件对象
    /// </summary>
    public partial class Form1 : RibbonForm
    {
        /// <summary>
        /// Main窗体构造函数，把自定义控件里的ribbon设置给该窗体，实现效果
        /// </summary>
        public Form1()
        {
            General.IsClosed = false;
            InitializeComponent();
            //Icon = Icon.FromHandle(Resources.formicon_32x32.GetHicon());
            Icon = Resources.SRMT;
            this.Controls.Add(ftaControl1.ribbonControl_FTA);
            this.Controls.Add(ftaControl1.ribbonStatusBar_FTA);
            this.Ribbon = ftaControl1.ribbonControl_FTA;
            this.StatusBar = ftaControl1.ribbonStatusBar_FTA;
            this.MainMenuStrip = null;
            this.Text = "SmarTree";

            var font = new System.Drawing.Font("MS Gothic", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            General.BarCheckItems = new Dictionary<string, BarCheckItem>();
            foreach (BarItem item in this.Ribbon.Items)
            {
                if (item is BarCheckItem)
                {
                    BarCheckItem checkItem = item as BarCheckItem;
                    General.BarCheckItems.Add(item.Name, checkItem);
                }
            }
        }

        /// <summary>
        /// 通过弹出系统菜单事件发送ESC按键来关闭菜单
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMenuStart(EventArgs e) => SendKeys.Send("{ESC}");

        /// <summary>
        /// 退出故障树程序前提示确认，并且提示是否保存当前改动
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            //保存前检查
            if (this.ftaControl1.HaveVirtualizationSystem())
            {
                if (MsgBox.Show(General.FtaProgram.String.ConfirmSavingMessage, General.FtaProgram.String.ConfirmTitleSave, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    this.ftaControl1.SaveData();
                }
            }

            if (MsgBox.Show(General.FtaProgram.String.ConfirmExitMessage, General.FtaProgram.String.ConfirmTitleExit, MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                General.IsClosed = true;
            }
            else
            {
                General.IsClosed = false;
                e.Cancel = true;
            }

            try
            {
                this.ftaControl1.CloseDB();
                //故障计算线程
                if (General.isTaskRun)
                {
                    Environment.Exit(1);
                }
            }
            catch (Exception)
            {
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case CSendMessage.WM_COPYDATA:
                    {
                        try
                        {
                            //当前使用的标志
                            int wParam;
                            //后期扩展使用
                            int operationType;
                            //传递的字符串信息
                            string strData;
                            CSendMessage.ParseMessage(m, out wParam, out operationType, out strData);
                            if (strData != "")
                            {
                                General.MessageFromSimfia = strData;
                                //General.ABC = strData;
                                //General.InvokeHandler(Model.Enum.GlobalEvent.ImprotInfoFromProcess, strData);
                                if (General.IsTreeListLoaded == true)
                                {
                                    General.InvokeHandler(Model.Enum.GlobalEvent.ImprotInfoFromProcess, General.MessageFromSimfia);
                                }
                            }
                        }
                        catch (Exception ex) { MsgBox.Show("The FormExcel form message handling exception：" + ex.Message); }
                        return;
                    }
                default:
                    break;
            }
            base.WndProc(ref m);
        }
    }
}
