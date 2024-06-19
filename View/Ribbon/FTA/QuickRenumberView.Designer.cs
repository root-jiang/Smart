namespace FaultTreeAnalysis.View.Ribbon.FTA
{
   partial class QuickRenumberView
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
         this.Tl_Renumber = new DevExpress.XtraTreeList.TreeList();
         this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
         this.Sb_Cancel = new DevExpress.XtraEditors.SimpleButton();
         this.Sb_Ok = new DevExpress.XtraEditors.SimpleButton();
         this.Ce_SelectAll = new DevExpress.XtraEditors.CheckEdit();
         ((System.ComponentModel.ISupportInitialize)(this.Tl_Renumber)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.Ce_SelectAll.Properties)).BeginInit();
         this.SuspendLayout();
         // 
         // Tl_Renumber
         // 
         this.Tl_Renumber.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.Tl_Renumber.Cursor = System.Windows.Forms.Cursors.Default;
         this.Tl_Renumber.DataSource = null;
         this.Tl_Renumber.Location = new System.Drawing.Point(0, 0);
         this.Tl_Renumber.Margin = new System.Windows.Forms.Padding(3, 3, 3, 50);
         this.Tl_Renumber.Name = "Tl_Renumber";
         this.Tl_Renumber.OptionsView.ShowCheckBoxes = true;
         this.Tl_Renumber.Size = new System.Drawing.Size(984, 414);
         this.Tl_Renumber.TabIndex = 0;
         // 
         // repositoryItemCheckEdit1
         // 
         this.repositoryItemCheckEdit1.AutoHeight = false;
         this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
         // 
         // Sb_Cancel
         // 
         this.Sb_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.Sb_Cancel.Location = new System.Drawing.Point(897, 426);
         this.Sb_Cancel.Name = "Sb_Cancel";
         this.Sb_Cancel.Size = new System.Drawing.Size(75, 23);
         this.Sb_Cancel.TabIndex = 1;
         this.Sb_Cancel.Text = "Cancel";
         // 
         // Sb_Ok
         // 
         this.Sb_Ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.Sb_Ok.Location = new System.Drawing.Point(806, 427);
         this.Sb_Ok.Name = "Sb_Ok";
         this.Sb_Ok.Size = new System.Drawing.Size(75, 23);
         this.Sb_Ok.TabIndex = 2;
         this.Sb_Ok.Text = "Ok";
         // 
         // Ce_SelectAll
         // 
         this.Ce_SelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.Ce_SelectAll.Location = new System.Drawing.Point(13, 426);
         this.Ce_SelectAll.Name = "Ce_SelectAll";
         this.Ce_SelectAll.Properties.AutoWidth = true;
         this.Ce_SelectAll.Properties.Caption = "Select All";
         this.Ce_SelectAll.Size = new System.Drawing.Size(72, 19);
         this.Ce_SelectAll.TabIndex = 3;
         // 
         // RenumberView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(984, 461);
         this.Controls.Add(this.Ce_SelectAll);
         this.Controls.Add(this.Sb_Ok);
         this.Controls.Add(this.Sb_Cancel);
         this.Controls.Add(this.Tl_Renumber);
         this.Name = "RenumberView";
         this.Text = "RenumberView";
         ((System.ComponentModel.ISupportInitialize)(this.Tl_Renumber)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.Ce_SelectAll.Properties)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private DevExpress.XtraTreeList.TreeList Tl_Renumber;
      private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
      private DevExpress.XtraEditors.SimpleButton Sb_Cancel;
      private DevExpress.XtraEditors.SimpleButton Sb_Ok;
      private DevExpress.XtraEditors.CheckEdit Ce_SelectAll;
   }
}