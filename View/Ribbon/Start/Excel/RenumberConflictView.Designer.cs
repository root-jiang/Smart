namespace FaultTreeAnalysis.View.Ribbon.Start.Excel
{
   partial class RenumberConflictView
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
         this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
         this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
         this.checkEdit1 = new DevExpress.XtraEditors.CheckEdit();
         this.radioGroup1 = new DevExpress.XtraEditors.RadioGroup();
         this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
         this.Te_2 = new DevExpress.XtraEditors.TextEdit();
         this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
         this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
         this.Te_1 = new DevExpress.XtraEditors.TextEdit();
         this.Lc_1 = new DevExpress.XtraEditors.LabelControl();
         this.Lc_0 = new DevExpress.XtraEditors.LabelControl();
         this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
         ((System.ComponentModel.ISupportInitialize)(this.checkEdit1.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.radioGroup1.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.Te_2.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.Te_1.Properties)).BeginInit();
         this.SuspendLayout();
         // 
         // labelControl1
         // 
         this.labelControl1.Location = new System.Drawing.Point(12, 12);
         this.labelControl1.Name = "labelControl1";
         this.labelControl1.Size = new System.Drawing.Size(108, 14);
         this.labelControl1.TabIndex = 0;
         this.labelControl1.Text = "编号冲突请另行指定";
         // 
         // labelControl2
         // 
         this.labelControl2.Location = new System.Drawing.Point(14, 63);
         this.labelControl2.Name = "labelControl2";
         this.labelControl2.Size = new System.Drawing.Size(60, 14);
         this.labelControl2.TabIndex = 7;
         this.labelControl2.Text = "变更后的ID";
         // 
         // checkEdit1
         // 
         this.checkEdit1.Location = new System.Drawing.Point(191, 151);
         this.checkEdit1.Name = "checkEdit1";
         this.checkEdit1.Properties.Caption = "批量处理";
         this.checkEdit1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
         this.checkEdit1.Size = new System.Drawing.Size(75, 19);
         this.checkEdit1.TabIndex = 6;
         // 
         // radioGroup1
         // 
         this.radioGroup1.Location = new System.Drawing.Point(13, 88);
         this.radioGroup1.Name = "radioGroup1";
         this.radioGroup1.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
            new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "关键字后置"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "关键字前置")});
         this.radioGroup1.Size = new System.Drawing.Size(254, 50);
         this.radioGroup1.TabIndex = 8;
         // 
         // labelControl3
         // 
         this.labelControl3.Location = new System.Drawing.Point(15, 176);
         this.labelControl3.Name = "labelControl3";
         this.labelControl3.Size = new System.Drawing.Size(36, 14);
         this.labelControl3.TabIndex = 10;
         this.labelControl3.Text = "关键字";
         // 
         // Te_2
         // 
         this.Te_2.EditValue = "";
         this.Te_2.Location = new System.Drawing.Point(60, 176);
         this.Te_2.Name = "Te_2";
         this.Te_2.Size = new System.Drawing.Size(100, 20);
         this.Te_2.TabIndex = 9;
         this.Te_2.TextChanged += new System.EventHandler(this.KeyWorldChanged);
         // 
         // simpleButton1
         // 
         this.simpleButton1.Location = new System.Drawing.Point(191, 226);
         this.simpleButton1.Name = "simpleButton1";
         this.simpleButton1.Size = new System.Drawing.Size(75, 23);
         this.simpleButton1.TabIndex = 11;
         this.simpleButton1.Text = "OK";
         this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
         // 
         // labelControl4
         // 
         this.labelControl4.Location = new System.Drawing.Point(16, 153);
         this.labelControl4.Name = "labelControl4";
         this.labelControl4.Size = new System.Drawing.Size(24, 14);
         this.labelControl4.TabIndex = 12;
         this.labelControl4.Text = "编号";
         // 
         // Te_1
         // 
         this.Te_1.EditValue = "";
         this.Te_1.Location = new System.Drawing.Point(60, 150);
         this.Te_1.Name = "Te_1";
         this.Te_1.Size = new System.Drawing.Size(100, 20);
         this.Te_1.TabIndex = 13;
         this.Te_1.EditValueChanged += new System.EventHandler(this.NewValueChanged);
         // 
         // Lc_1
         // 
         this.Lc_1.Location = new System.Drawing.Point(86, 63);
         this.Lc_1.Name = "Lc_1";
         this.Lc_1.Size = new System.Drawing.Size(24, 14);
         this.Lc_1.TabIndex = 14;
         this.Lc_1.Text = "未知";
         // 
         // Lc_0
         // 
         this.Lc_0.Location = new System.Drawing.Point(86, 43);
         this.Lc_0.Name = "Lc_0";
         this.Lc_0.Size = new System.Drawing.Size(24, 14);
         this.Lc_0.TabIndex = 16;
         this.Lc_0.Text = "未知";
         // 
         // labelControl6
         // 
         this.labelControl6.Location = new System.Drawing.Point(14, 43);
         this.labelControl6.Name = "labelControl6";
         this.labelControl6.Size = new System.Drawing.Size(60, 14);
         this.labelControl6.TabIndex = 15;
         this.labelControl6.Text = "有冲突的ID";
         // 
         // RenumberConflictView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(284, 261);
         this.ControlBox = false;
         this.Controls.Add(this.Lc_0);
         this.Controls.Add(this.labelControl6);
         this.Controls.Add(this.Lc_1);
         this.Controls.Add(this.Te_1);
         this.Controls.Add(this.labelControl4);
         this.Controls.Add(this.simpleButton1);
         this.Controls.Add(this.labelControl3);
         this.Controls.Add(this.Te_2);
         this.Controls.Add(this.radioGroup1);
         this.Controls.Add(this.labelControl2);
         this.Controls.Add(this.checkEdit1);
         this.Controls.Add(this.labelControl1);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.Name = "RenumberConflictView";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
         this.Text = "RenumberEditView";
         ((System.ComponentModel.ISupportInitialize)(this.checkEdit1.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.radioGroup1.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.Te_2.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.Te_1.Properties)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private DevExpress.XtraEditors.LabelControl labelControl1;
      private DevExpress.XtraEditors.LabelControl labelControl2;
      private DevExpress.XtraEditors.CheckEdit checkEdit1;
      private DevExpress.XtraEditors.RadioGroup radioGroup1;
      private DevExpress.XtraEditors.LabelControl labelControl3;
      private DevExpress.XtraEditors.TextEdit Te_2;
      private DevExpress.XtraEditors.SimpleButton simpleButton1;
      private DevExpress.XtraEditors.LabelControl labelControl4;
      private DevExpress.XtraEditors.TextEdit Te_1;
      private DevExpress.XtraEditors.LabelControl Lc_1;
      private DevExpress.XtraEditors.LabelControl Lc_0;
      private DevExpress.XtraEditors.LabelControl labelControl6;
   }
}