namespace FaultTreeAnalysis.View.Diagram
{
    partial class NewRepeatOrTransfer
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
            this.listBoxControl_id = new DevExpress.XtraEditors.ListBoxControl();
            this.simpleButton_OK = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton_Cancel = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.listBoxControl_id)).BeginInit();
            this.SuspendLayout();
            // 
            // listBoxControl_id
            // 
            this.listBoxControl_id.Location = new System.Drawing.Point(12, 12);
            this.listBoxControl_id.Name = "listBoxControl_id";
            this.listBoxControl_id.Size = new System.Drawing.Size(241, 187);
            this.listBoxControl_id.TabIndex = 0;
            // 
            // simpleButton_OK
            // 
            this.simpleButton_OK.Location = new System.Drawing.Point(259, 68);
            this.simpleButton_OK.Name = "simpleButton_OK";
            this.simpleButton_OK.Size = new System.Drawing.Size(75, 23);
            this.simpleButton_OK.TabIndex = 1;
            this.simpleButton_OK.Text = "确定";
            this.simpleButton_OK.Click += new System.EventHandler(this.simpleButton_OK_Click);
            // 
            // simpleButton_Cancel
            // 
            this.simpleButton_Cancel.Location = new System.Drawing.Point(259, 124);
            this.simpleButton_Cancel.Name = "simpleButton_Cancel";
            this.simpleButton_Cancel.Size = new System.Drawing.Size(75, 23);
            this.simpleButton_Cancel.TabIndex = 2;
            this.simpleButton_Cancel.Text = "取消";
            this.simpleButton_Cancel.Click += new System.EventHandler(this.simpleButton_Cancel_Click);
            // 
            // simpleButton1
            // 
            this.simpleButton1.Location = new System.Drawing.Point(259, 12);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(75, 23);
            this.simpleButton1.TabIndex = 3;
            this.simpleButton1.Text = "新建";
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // NewRepeatOrTransfer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(343, 211);
            this.Controls.Add(this.simpleButton1);
            this.Controls.Add(this.simpleButton_Cancel);
            this.Controls.Add(this.simpleButton_OK);
            this.Controls.Add(this.listBoxControl_id);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "NewRepeatOrTransfer";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "XtraFormNewRepeatedEventOrTransferGate";
            ((System.ComponentModel.ISupportInitialize)(this.listBoxControl_id)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.ListBoxControl listBoxControl_id;
        private DevExpress.XtraEditors.SimpleButton simpleButton_OK;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Cancel;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
    }
}