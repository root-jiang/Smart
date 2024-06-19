namespace FaultTreeAnalysis
{
    partial class XtraFormNewSystem
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.textEdit_SystemName = new DevExpress.XtraEditors.TextEdit();
            this.labelControl_SystemName = new DevExpress.XtraEditors.LabelControl();
            this.simpleButton_OK = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton_Cancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit_SystemName.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.textEdit_SystemName);
            this.panelControl1.Controls.Add(this.labelControl_SystemName);
            this.panelControl1.Location = new System.Drawing.Point(12, 32);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(402, 53);
            this.panelControl1.TabIndex = 0;
            // 
            // textEdit_SystemName
            // 
            this.textEdit_SystemName.Location = new System.Drawing.Point(116, 18);
            this.textEdit_SystemName.Name = "textEdit_SystemName";
            this.textEdit_SystemName.Size = new System.Drawing.Size(281, 20);
            this.textEdit_SystemName.TabIndex = 1;
            // 
            // labelControl_SystemName
            // 
            this.labelControl_SystemName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl_SystemName.Location = new System.Drawing.Point(5, 19);
            this.labelControl_SystemName.Name = "labelControl_SystemName";
            this.labelControl_SystemName.Size = new System.Drawing.Size(105, 17);
            this.labelControl_SystemName.TabIndex = 0;
            this.labelControl_SystemName.Text = "名称：";
            // 
            // simpleButton_OK
            // 
            this.simpleButton_OK.Location = new System.Drawing.Point(72, 110);
            this.simpleButton_OK.Name = "simpleButton_OK";
            this.simpleButton_OK.Size = new System.Drawing.Size(75, 23);
            this.simpleButton_OK.TabIndex = 1;
            this.simpleButton_OK.Text = "确定";
            this.simpleButton_OK.Click += new System.EventHandler(this.simpleButton_OK_Click);
            // 
            // simpleButton_Cancel
            // 
            this.simpleButton_Cancel.Location = new System.Drawing.Point(256, 110);
            this.simpleButton_Cancel.Name = "simpleButton_Cancel";
            this.simpleButton_Cancel.Size = new System.Drawing.Size(75, 23);
            this.simpleButton_Cancel.TabIndex = 2;
            this.simpleButton_Cancel.Text = "取消";
            this.simpleButton_Cancel.Click += new System.EventHandler(this.simpleButton_Cancel_Click);
            // 
            // XtraFormNewSystem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(426, 153);
            this.Controls.Add(this.simpleButton_Cancel);
            this.Controls.Add(this.simpleButton_OK);
            this.Controls.Add(this.panelControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "XtraFormNewSystem";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "新建系统";
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.textEdit_SystemName.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.TextEdit textEdit_SystemName;
        private DevExpress.XtraEditors.LabelControl labelControl_SystemName;
        private DevExpress.XtraEditors.SimpleButton simpleButton_OK;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Cancel;
    }
}