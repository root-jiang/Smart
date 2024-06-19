namespace FaultTreeAnalysis.View.Ribbon.Start.Excel
{
   partial class SelectCheckErr
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectCheckErr));
            this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            this.treeList_Project = new DevExpress.XtraTreeList.TreeList();
            this.col_name = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.col_type = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.repositoryItemPictureEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
            this.col_info = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this.treeList_Project)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemPictureEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // simpleButton2
            // 
            this.simpleButton2.Dock = System.Windows.Forms.DockStyle.Right;
            this.simpleButton2.Location = new System.Drawing.Point(595, 0);
            this.simpleButton2.Name = "simpleButton2";
            this.simpleButton2.Size = new System.Drawing.Size(75, 30);
            this.simpleButton2.TabIndex = 4;
            this.simpleButton2.Text = "Refresh";
            this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
            // 
            // treeList_Project
            // 
            this.treeList_Project.Appearance.FocusedCell.Options.UseBackColor = true;
            this.treeList_Project.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.col_name,
            this.col_type,
            this.col_info});
            this.treeList_Project.Cursor = System.Windows.Forms.Cursors.Default;
            this.treeList_Project.DataSource = null;
            this.treeList_Project.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeList_Project.Location = new System.Drawing.Point(0, 0);
            this.treeList_Project.Name = "treeList_Project";
            this.treeList_Project.OptionsBehavior.ReadOnly = true;
            this.treeList_Project.OptionsSelection.SelectNodesOnRightClick = true;
            this.treeList_Project.OptionsView.BestFitNodes = DevExpress.XtraTreeList.TreeListBestFitNodes.Display;
            this.treeList_Project.OptionsView.ShowColumns = true;
            this.treeList_Project.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemPictureEdit1});
            this.treeList_Project.Size = new System.Drawing.Size(670, 418);
            this.treeList_Project.TabIndex = 7;
            this.treeList_Project.ViewStyle = DevExpress.XtraTreeList.TreeListViewStyle.TreeView;
            this.treeList_Project.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeList_Project_MouseDown);
            // 
            // col_name
            // 
            this.col_name.AppearanceCell.Options.UseTextOptions = true;
            this.col_name.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.col_name.AppearanceHeader.Options.UseTextOptions = true;
            this.col_name.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.col_name.Caption = "Name";
            this.col_name.FieldName = "name";
            this.col_name.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
            this.col_name.Name = "col_name";
            this.col_name.OptionsColumn.AllowEdit = false;
            this.col_name.Visible = true;
            this.col_name.VisibleIndex = 1;
            this.col_name.Width = 148;
            // 
            // col_type
            // 
            this.col_type.AppearanceCell.Options.UseTextOptions = true;
            this.col_type.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.col_type.AppearanceHeader.Options.UseTextOptions = true;
            this.col_type.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.col_type.Caption = "Type";
            this.col_type.ColumnEdit = this.repositoryItemPictureEdit1;
            this.col_type.FieldName = "type";
            this.col_type.Name = "col_type";
            this.col_type.OptionsColumn.AllowEdit = false;
            this.col_type.Visible = true;
            this.col_type.VisibleIndex = 0;
            this.col_type.Width = 56;
            // 
            // repositoryItemPictureEdit1
            // 
            this.repositoryItemPictureEdit1.Name = "repositoryItemPictureEdit1";
            this.repositoryItemPictureEdit1.ReadOnly = true;
            this.repositoryItemPictureEdit1.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
            // 
            // col_info
            // 
            this.col_info.AppearanceCell.Options.UseTextOptions = true;
            this.col_info.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.col_info.AppearanceHeader.Options.UseTextOptions = true;
            this.col_info.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.col_info.Caption = "Infomation";
            this.col_info.FieldName = "info";
            this.col_info.Name = "col_info";
            this.col_info.OptionsColumn.AllowEdit = false;
            this.col_info.Visible = true;
            this.col_info.VisibleIndex = 2;
            this.col_info.Width = 440;
            // 
            // panelControl1
            // 
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl1.Controls.Add(this.simpleButton2);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl1.Location = new System.Drawing.Point(0, 418);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(670, 30);
            this.panelControl1.TabIndex = 8;
            // 
            // SelectCheckErr
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(670, 448);
            this.ControlBox = false;
            this.Controls.Add(this.treeList_Project);
            this.Controls.Add(this.panelControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectCheckErr";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SelectCheckErr";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.SelectCheckErr_Load);
            ((System.ComponentModel.ISupportInitialize)(this.treeList_Project)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemPictureEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.ResumeLayout(false);

      }

      #endregion
      private DevExpress.XtraEditors.SimpleButton simpleButton2;
        public DevExpress.XtraTreeList.TreeList treeList_Project;
        private DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit repositoryItemPictureEdit1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        public DevExpress.XtraTreeList.Columns.TreeListColumn col_name;
        public DevExpress.XtraTreeList.Columns.TreeListColumn col_type;
        public DevExpress.XtraTreeList.Columns.TreeListColumn col_info;
    }
}