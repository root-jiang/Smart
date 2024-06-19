namespace FaultTreeAnalysis.View.Ribbon.Start.Excel
{
   partial class SwitchExport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SwitchExport));
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            this.checkEdit1 = new DevExpress.XtraEditors.CheckEdit();
            this.treeList_Project = new DevExpress.XtraTreeList.TreeList();
            this.name = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.treeList_Project)).BeginInit();
            this.SuspendLayout();
            // 
            // simpleButton1
            // 
            this.simpleButton1.Location = new System.Drawing.Point(499, 334);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(75, 23);
            this.simpleButton1.TabIndex = 3;
            this.simpleButton1.Text = "OK";
            // 
            // simpleButton2
            // 
            this.simpleButton2.Location = new System.Drawing.Point(594, 334);
            this.simpleButton2.Name = "simpleButton2";
            this.simpleButton2.Size = new System.Drawing.Size(75, 23);
            this.simpleButton2.TabIndex = 4;
            this.simpleButton2.Text = "Cancel";
            // 
            // checkEdit1
            // 
            this.checkEdit1.Location = new System.Drawing.Point(404, 336);
            this.checkEdit1.Name = "checkEdit1";
            this.checkEdit1.Properties.Caption = "SelectAll";
            this.checkEdit1.Size = new System.Drawing.Size(75, 19);
            this.checkEdit1.TabIndex = 6;
            this.checkEdit1.CheckedChanged += new System.EventHandler(this.checkEdit1_CheckedChanged);
            // 
            // treeList_Project
            // 
            this.treeList_Project.Appearance.FocusedCell.Options.UseBackColor = true;
            this.treeList_Project.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.name,
            this.treeListColumn1});
            this.treeList_Project.Cursor = System.Windows.Forms.Cursors.Default;
            this.treeList_Project.DataSource = null;
            this.treeList_Project.Location = new System.Drawing.Point(12, 12);
            this.treeList_Project.Name = "treeList_Project";
            this.treeList_Project.OptionsSelection.SelectNodesOnRightClick = true;
            this.treeList_Project.OptionsView.BestFitNodes = DevExpress.XtraTreeList.TreeListBestFitNodes.Display;
            this.treeList_Project.OptionsView.ShowCheckBoxes = true;
            this.treeList_Project.Size = new System.Drawing.Size(657, 314);
            this.treeList_Project.TabIndex = 7;
            this.treeList_Project.ViewStyle = DevExpress.XtraTreeList.TreeListViewStyle.TreeView;
            this.treeList_Project.AfterCheckNode += new DevExpress.XtraTreeList.NodeEventHandler(this.treeList_Project_AfterCheckNode);
            this.treeList_Project.CustomColumnSort += new DevExpress.XtraTreeList.CustomColumnSortEventHandler(this.treeList_Project_CustomColumnSort);
            this.treeList_Project.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeList_Project_MouseDown);
            // 
            // name
            // 
            this.name.Caption = "name";
            this.name.FieldName = "name";
            this.name.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
            this.name.Name = "name";
            this.name.OptionsColumn.AllowEdit = false;
            this.name.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            this.name.SortOrder = System.Windows.Forms.SortOrder.Ascending;
            this.name.Visible = true;
            this.name.VisibleIndex = 0;
            // 
            // treeListColumn1
            // 
            this.treeListColumn1.Caption = "treeListColumn1";
            this.treeListColumn1.FieldName = "SortType";
            this.treeListColumn1.Name = "treeListColumn1";
            // 
            // SwitchExport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(681, 367);
            this.Controls.Add(this.checkEdit1);
            this.Controls.Add(this.simpleButton2);
            this.Controls.Add(this.simpleButton1);
            this.Controls.Add(this.treeList_Project);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SwitchExport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Export";
            this.Load += new System.EventHandler(this.SwitchExport_Load);
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.treeList_Project)).EndInit();
            this.ResumeLayout(false);

      }

      #endregion
      private DevExpress.XtraEditors.SimpleButton simpleButton1;
      private DevExpress.XtraEditors.SimpleButton simpleButton2;
        private DevExpress.XtraEditors.CheckEdit checkEdit1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn name;
        public DevExpress.XtraTreeList.TreeList treeList_Project;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn1;
    }
}