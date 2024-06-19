namespace FaultTreeAnalysis
{
    partial class XtraFormNewFTAItem
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
            this.comboBoxEdit_FTAItemType = new DevExpress.XtraEditors.ComboBoxEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.simpleButton_OK = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton_Cancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit_FTAItemType.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBoxEdit_FTAItemType
            // 
            this.comboBoxEdit_FTAItemType.Location = new System.Drawing.Point(81, 15);
            this.comboBoxEdit_FTAItemType.Name = "comboBoxEdit_FTAItemType";
            this.comboBoxEdit_FTAItemType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.comboBoxEdit_FTAItemType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.comboBoxEdit_FTAItemType.Size = new System.Drawing.Size(311, 20);
            this.comboBoxEdit_FTAItemType.TabIndex = 0;
            // 
            // labelControl1
            // 
            this.labelControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelControl1.Appearance.Options.UseTextOptions = true;
            this.labelControl1.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.labelControl1.Location = new System.Drawing.Point(9, 18);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(66, 14);
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "ItemType：";
            // 
            // simpleButton_OK
            // 
            this.simpleButton_OK.Location = new System.Drawing.Point(226, 50);
            this.simpleButton_OK.Name = "simpleButton_OK";
            this.simpleButton_OK.Size = new System.Drawing.Size(75, 23);
            this.simpleButton_OK.TabIndex = 0;
            this.simpleButton_OK.Text = "OK";
            this.simpleButton_OK.Click += new System.EventHandler(this.simpleButton_OK_Click);
            // 
            // simpleButton_Cancel
            // 
            this.simpleButton_Cancel.Location = new System.Drawing.Point(317, 50);
            this.simpleButton_Cancel.Name = "simpleButton_Cancel";
            this.simpleButton_Cancel.Size = new System.Drawing.Size(75, 23);
            this.simpleButton_Cancel.TabIndex = 1;
            this.simpleButton_Cancel.Text = "Cancel";
            this.simpleButton_Cancel.Click += new System.EventHandler(this.simpleButton_Cancel_Click);
            // 
            // XtraFormNewFTAItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 90);
            this.Controls.Add(this.simpleButton_Cancel);
            this.Controls.Add(this.simpleButton_OK);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.comboBoxEdit_FTAItemType);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "XtraFormNewFTAItem";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "XtraFormNewFTAItem";
            this.Load += new System.EventHandler(this.XtraFormNewFTAItem_Load);
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit_FTAItemType.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.ComboBoxEdit comboBoxEdit_FTAItemType;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SimpleButton simpleButton_OK;
        private DevExpress.XtraEditors.SimpleButton simpleButton_Cancel;
    }
}