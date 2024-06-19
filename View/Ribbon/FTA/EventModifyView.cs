using FaultTreeAnalysis.Model.Data;
using System;
using System.Collections.Generic;

namespace FaultTreeAnalysis.FTAControlEventHandle.Ribbon.FTA.Tool.FTA
{
    /// <summary>
    /// 让用户批量选择事件图形后，重命名这些事件为相同的重复事件
    /// </summary>
    public partial class EventModifyView : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// 是否是取消关闭窗体
        /// </summary>
        public bool Result;

        /// <summary>
        /// 当前系统对象
        /// </summary>
        private SystemModel ftaSystem;

        /// <summary>
        /// 这个变量有用？
        /// </summary>
        private IEnumerable<DrawData> allEvents;

        /// <summary>
        /// 用户选择的事件集合
        /// </summary>
        private IEnumerable<DrawData> selectedEvents;

        /// <summary>
        /// 重命名事件窗体构造函数
        /// </summary>
        /// <param name="ftaProgram">当前程序对象</param>
        /// <param name="ftaSystem">当前系统</param>
        /// <param name="allEvents">未知</param>
        /// <param name="selectedEvents">选中的事件</param>
        public EventModifyView(ProgramModel ftaProgram, IEnumerable<DrawData> allEvents, IEnumerable<DrawData> selectedEvents)
        {
            InitializeComponent();
            this.allEvents = allEvents;
            this.selectedEvents = selectedEvents;
            this.textEdit1.Text = "Event0";
            this.SubscribeEvents();
            this.ftaSystem = ftaProgram.CurrentSystem;
            this.BindLangage(ftaProgram.String);
        }

        /// <summary>
        /// 重命名事件窗体语言绑定
        /// </summary>
        /// <param name="ftaString">国际化字符串</param>
        private void BindLangage(StringModel ftaString)
        {
            this.Text = ftaString.EventModify;
            this.groupControl1.Text = ftaString.EventModifyDescription;
            this.labelControl2.Text = ftaString.EventName;
            this.simpleButton1.Text = ftaString.OK;
            this.simpleButton2.Text = ftaString.Cancel;
        }

        /// <summary>
        /// 重命名事件窗体注册事件
        /// </summary>
        private void SubscribeEvents()
        {
            this.simpleButton1.Click += SimpleButton_Click;
            this.simpleButton2.Click += SimpleButton_Click;
        }

        /// <summary>
        /// 重命名事件窗体注销事件
        /// </summary>
        private void UnsubscribeEvents()
        {
            this.simpleButton1.Click -= SimpleButton_Click;
            this.simpleButton2.Click -= SimpleButton_Click;
        }

        /// <summary>
        /// 移除重复事件
        /// </summary>
        private void RemoveRepeatEvent()
        {
            foreach (var item in this.selectedEvents)
            {
                this.ftaSystem.RemoveRepeatedEvent(item, false);
            }
        }

        /// <summary>
        /// 添加重复事件
        /// </summary>
        private void AddRepeatEvent()
        {
            foreach (var item in this.selectedEvents)
            {
                this.ftaSystem.AddRepeatedEvent(item);
            }
            //this.ftaSystem.AddRepeatedEvent(this.selectedEvents.ToList());
        }

        /// <summary>
        /// 重命名事件窗体的按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SimpleButton_Click(object sender, EventArgs e)
        {
            var isCancel = sender.Equals(this.simpleButton1) ? false : true;
            this.Close(isCancel);
        }

        /// <summary>
        /// 关闭重命名事件窗体
        /// </summary>
        /// <param name="isCancel">是否取消重命名事件</param>
        private void Close(bool isCancel)
        {
            if (isCancel == false)
            {
                int index = 0;
                var firstItem = default(DrawData);
                foreach (var item in this.selectedEvents)
                {
                    item.Identifier = this.textEdit1.Text;
                    if (index == 0) firstItem = item;
                    else item.CopyIntoTransferOrRepeatedEvent(firstItem);
                    index++;
                }
                this.ftaSystem.RaiseRepeatedEvent();
                this.Result = true;
            }
            this.Close();
            this.UnsubscribeEvents();
        }
    }
}