using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace FaultTreeAnalysis.MessageDialogUI
{
    /// <summary>
    /// 模式对话框
    /// </summary>
    public partial class MessageDialogWindow_Max : XtraForm
    {
        /// <summary>
        /// 对话框返回结果
        /// </summary>
        public string ReturnValue { get; set; }
        /// <summary>
        /// 模式对话框
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="button"></param>
        /// <param name="defaultButton"></param>
        public MessageDialogWindow_Max(string message, string title = "", MessageBoxButtons button = MessageBoxButtons.OK, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1, ContentAlignment textAlign = ContentAlignment.MiddleCenter, MessageBoxIcon Icon = MessageBoxIcon.Information)
        {
            //WinFormStyleFormator.SetWindowStyle(this);
            InitializeComponent();
            //this.Icon = Properties.Resources.logo;

            this.memoEdit1.Text = message;
            this.Text = title;

            if (this.Text == "")
            {
                this.Text = "SmarTree";
            } 
            btnYes.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.True;
            this.btnOK.Hide();
            this.btnCancel.Hide();
            this.btnYes.Hide();
            this.btnNo.Hide();
            List<SimpleButton> buttonShowList = new List<SimpleButton>();
            if (button == MessageBoxButtons.OK) { this.btnOK.Show(); buttonShowList.Add(btnOK); }
            else if (button == MessageBoxButtons.OKCancel) { this.btnOK.Show(); this.btnCancel.Show(); buttonShowList.Add(btnOK); buttonShowList.Add(btnCancel); Icon = MessageBoxIcon.Question; }
            else if (button == MessageBoxButtons.YesNo) { this.btnYes.Show(); this.btnNo.Show(); buttonShowList.Add(btnYes); buttonShowList.Add(btnNo); Icon = MessageBoxIcon.Question; }
            else if (button == MessageBoxButtons.YesNoCancel) { this.btnYes.Show(); this.btnNo.Show(); this.btnCancel.Show(); buttonShowList.Add(btnYes); buttonShowList.Add(btnNo); buttonShowList.Add(btnCancel); Icon = MessageBoxIcon.Question; }
            var index = 0;
            if (defaultButton == MessageBoxDefaultButton.Button1) index = 0;
            else if (defaultButton == MessageBoxDefaultButton.Button2) index = 1;
            else if (defaultButton == MessageBoxDefaultButton.Button3) index = 2;
            if (index < buttonShowList.Count) { buttonShowList[index].TabIndex = 0; }
            else buttonShowList[0].TabIndex = 0; 
        }
        private void btnCancel_Click(object sender, EventArgs e) { DialogResult = DialogResult.Cancel; }
        private void btnOK_Click(object sender, EventArgs e) { DialogResult = DialogResult.OK; }
        private void btnYes_Click(object sender, EventArgs e) { DialogResult = DialogResult.Yes; }
        private void btnNo_Click(object sender, EventArgs e) { DialogResult = DialogResult.No; }
    }
}