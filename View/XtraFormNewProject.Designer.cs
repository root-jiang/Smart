namespace FaultTreeAnalysis
{
    partial class XtraFormNewProject
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
            this.components = new System.ComponentModel.Container();
            this.labelControl_ProjectName = new DevExpress.XtraEditors.LabelControl();
            this.textEdit_ProjectName = new DevExpress.XtraEditors.TextEdit();
            this.simpleButton_OK = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton_Cancel = new DevExpress.XtraEditors.SimpleButton();
            this.xtraFolderBrowserDialog1 = new DevExpress.XtraEditors.XtraFolderBrowserDialog(this.components);
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit_ProjectName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelControl_ProjectName
            // 
            this.labelControl_ProjectName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl_ProjectName.Location = new System.Drawing.Point(5, 15);
            this.labelControl_ProjectName.Name = "labelControl_ProjectName";
            this.labelControl_ProjectName.Size = new System.Drawing.Size(82, 19);
            this.labelControl_ProjectName.TabIndex = 0;
            this.labelControl_ProjectName.Text = "名称：";
            // 
            // textEdit_ProjectName
            // 
            this.textEdit_ProjectName.Location = new System.Drawing.Point(93, 14);
            this.textEdit_ProjectName.Name = "textEdit_ProjectName";
            this.textEdit_ProjectName.Size = new System.Drawing.Size(304, 20);
            this.textEdit_ProjectName.TabIndex = 1;
            // 
            // simpleButton_OK
            // 
            this.simpleButton_OK.Location = new System.Drawing.Point(73, 73);
            this.simpleButton_OK.Name = "simpleButton_OK";
            this.simpleButton_OK.Size = new System.Drawing.Size(75, 23);
            this.simpleButton_OK.TabIndex = 2;
            this.simpleButton_OK.Text = "确定";
            this.simpleButton_OK.Click += new System.EventHandler(this.simpleButton_OK_Click);
            // 
            // simpleButton_Cancel
            // 
            this.simpleButton_Cancel.Location = new System.Drawing.Point(279, 73);
            this.simpleButton_Cancel.Name = "simpleButton_Cancel";
            this.simpleButton_Cancel.Size = new System.Drawing.Size(75, 23);
            this.simpleButton_Cancel.TabIndex = 2;
            this.simpleButton_Cancel.Text = "取消";
            this.simpleButton_Cancel.Click += new System.EventHandler(this.simpleButton_Cancel_Click);
            // 
            // xtraFolderBrowserDialog1
            // 
            this.xtraFolderBrowserDialog1.SelectedPath = "xtraFolderBrowserDialog1";
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.labelControl_ProjectName);
            this.panelControl1.Controls.Add(this.textEdit_ProjectName);
            this.panelControl1.Location = new System.Drawing.Point(12, 12);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(402, 55);
            this.panelControl1.TabIndex = 5;
            // 
            // XtraFormNewProject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(426, 106);
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.simpleButton_Cancel);
            this.Controls.Add(this.simpleButton_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "XtraFormNewProject";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "新建项目";
            ((System.ComponentModel.ISupportInitialize)(this.textEdit_ProjectName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl_ProjectName;
        private DevExpress.XtraEditors.TextEdit textEdit_ProjectName;
        private DevExpress.XtraEditors.SimpleButton simpleButton_OK;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Cancel;
        private DevExpress.XtraEditors.XtraFolderBrowserDialog xtraFolderBrowserDialog1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
    }
}