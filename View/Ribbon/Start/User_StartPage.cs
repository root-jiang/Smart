using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using FaultTreeAnalysis.Common;
using FaultTreeAnalysis.Properties;
using FaultTreeAnalysis.Model.Data;
using System.IO;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace FaultTreeAnalysis.View.Ribbon.Start
{
    public partial class User_StartPage : DevExpress.XtraEditors.XtraUserControl
    {
        public User_StartPage()
        {
            InitializeComponent();
        }
        public DataTable dt_Project = new DataTable();
        public DataTable dt_FaultTree = new DataTable();

        private void User_StartPage_Load(object sender, EventArgs e)
        {
            groupControl1.Text = General.FtaProgram.String.RecentProjects;
            groupControl2.Text = General.FtaProgram.String.RecentFiles;
            groupControl_Help.Text = General.FtaProgram.String.Help;
        }

        public void ReloadRecentFiles()
        {
            groupControl1.Text = General.FtaProgram.String.RecentProjects;
            groupControl2.Text = General.FtaProgram.String.RecentFiles;
            groupControl_Help.Text = General.FtaProgram.String.Help;

            labelControl_Tile.Text = General.FtaProgram.String.StartPageWelcome;
            col_Pic.Caption = General.FtaProgram.String.col_Pic;
            col_Name.Caption = General.FtaProgram.String.col_Name;
            col_Path.Caption = General.FtaProgram.String.col_Path;
            col_PicF.Caption = General.FtaProgram.String.col_Pic;
            col_NameF.Caption = General.FtaProgram.String.col_Name;
            col_PathF.Caption = General.FtaProgram.String.col_Path;

            groupControl_About.Text = General.FtaProgram.String.AboutSmarTree;
            hyperlink_UserManual.Text = General.FtaProgram.String.UserManual;
            hyperlink_Examples.Text = General.FtaProgram.String.Examples;

            memoEdit__About.Text = General.FtaProgram.String.TextAbout;

            dt_Project = new DataTable();
            dt_Project.Columns.Add("Pic", Resources.project_32x32.GetType());
            dt_Project.Columns.Add("File");
            dt_Project.Columns.Add("Description");
            dt_Project.Columns.Add("LastTime");
            dt_FaultTree = new DataTable();
            dt_FaultTree.Columns.Add("Pic", Resources.documentmap_32x32.GetType());
            dt_FaultTree.Columns.Add("File");
            dt_FaultTree.Columns.Add("Description");
            dt_FaultTree.Columns.Add("LastTime");

            //加载最近工程 
            RecentModel RM_Project = RecentFiles.GetRecentModel();

            if (RM_Project.RecentProject != null)
            {
                var result = from pair in RM_Project.RecentProject orderby pair.Value descending select pair;
                foreach (KeyValuePair<string, string> pair in result)
                {
                    dt_Project.Rows.Add(new object[] { Resources.project_32x32, new DirectoryInfo(pair.Key).Name, pair.Key, pair.Value });
                }
                gridControl1.DataSource = dt_Project;
            }

            //加载最近文件
            RecentModel RM = RecentFiles.GetRecentModel();
            if (RM_Project.RecentFaultTree != null)
            {
                var result = from pair in RM_Project.RecentFaultTree orderby pair.Value descending select pair;
                foreach (KeyValuePair<string, string> pair in result)
                {
                    dt_FaultTree.Rows.Add(new object[] { Resources.documentmap_32x32, new FileInfo(pair.Key).Name, pair.Key, pair.Value });
                }
                gridControl2.DataSource = dt_FaultTree;
            }
        }

        private void panelControl2_SizeChanged(object sender, EventArgs e)
        {
            groupControl1.Height = panelControl2.Height / 2;
        }

        private void panelControl1_SizeChanged(object sender, EventArgs e)
        {
            panelControl2.Location = new Point(panelControl2.Location.X, (panelControl1.Height - panelControl1.Height * 2 / 3) / 2);

            labelControl_Tile.Location = new Point(panelControl2.Location.X, (panelControl2.Location.Y - labelControl_Tile.Height) / 2);

            panelControl2.Width = panelControl1.Width * 17 / 30;
            panelControl2.Height = panelControl1.Height * 2 / 3;

            panelControl3.Location = new Point(panelControl2.Location.X + panelControl2.Width + 30, panelControl2.Location.Y + 11);

            panelControl3.Height = panelControl2.Height - 11;

            panelControl3.Width = this.Width - panelControl2.Width - panelControl2.Location.X - 80;
        }

        private void gridView1_MouseDown(object sender, MouseEventArgs e)
        {
            GridHitInfo info = gridView1.CalcHitInfo(e.Location);

            if (info.InRow && info.Column != null && (info.Column.FieldName == "File" || info.Column.FieldName == "Description"))
            {
                string Item = gridView1.GetRowCellDisplayText(info.RowHandle, "Description");

                if (Directory.Exists(Item))
                {
                    ((FTAControl)this.Parent).LoadData(true, Item);

                    //定位
                    foreach (TreeListNode PNode in General.ProjectControl.Nodes)
                    {
                        if (((ProjectModel)PNode.Tag).ProjectPath == Item)
                        {
                            General.ProjectControl.FocusedNode = PNode;
                        }
                    }
                }
            }
        }

        private void gridView2_MouseDown(object sender, MouseEventArgs e)
        {
            GridHitInfo info = gridView2.CalcHitInfo(e.Location);

            if (info.InRow && info.Column != null && (info.Column.FieldName == "File" || info.Column.FieldName == "Description"))
            {
                string Item = gridView2.GetRowCellDisplayText(info.RowHandle, "Description");

                if (File.Exists(Item))
                {
                    ((FTAControl)this.Parent).LoadData(false, Item);

                    //定位
                    foreach (TreeListNode PNode in General.ProjectControl.Nodes)
                    {
                        foreach (TreeListNode GNode in PNode.Nodes)
                        {
                            if (GNode.Tag != null && ((ProjectModel)PNode.Tag).ProjectPath + "\\" + ((SystemModel)GNode.Tag).SystemName + FixedString.APP_EXTENSION == Item)
                            {
                                General.ProjectControl.FocusedNode = GNode;
                                break;
                            }
                            foreach (TreeListNode SNode in GNode.Nodes)
                            {
                                if (((ProjectModel)PNode.Tag).ProjectPath + "\\" + ((SystemModel)SNode.Tag).SystemName + FixedString.APP_EXTENSION == Item)
                                {
                                    General.ProjectControl.FocusedNode = SNode;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void hyperlink_UserManual_Click(object sender, EventArgs e)
        {
            Frm_UserManual f = new View.Ribbon.Start.Frm_UserManual();
            f.Show();
        }

        private void hyperlink_Examples_Click(object sender, EventArgs e)
        {
            if (File.Exists(Application.StartupPath + "\\Example\\Example.fta"))
            {
                ((FTAControl)this.Parent).LoadData(false, Application.StartupPath + "\\Example\\Example.fta");
                 
                //定位
                foreach (TreeListNode PNode in General.ProjectControl.Nodes)
                {
                    foreach (TreeListNode GNode in PNode.Nodes)
                    {
                        if (GNode.Tag != null && ((ProjectModel)PNode.Tag).ProjectPath + "\\" + ((SystemModel)GNode.Tag).SystemName + FixedString.APP_EXTENSION == Application.StartupPath + "\\Example\\Example.fta")
                        {
                            General.ProjectControl.FocusedNode = GNode;
                            break;
                        }
                        foreach (TreeListNode SNode in GNode.Nodes)
                        {
                            if (((ProjectModel)PNode.Tag).ProjectPath + "\\" + ((SystemModel)SNode.Tag).SystemName + FixedString.APP_EXTENSION == Application.StartupPath + "\\Example\\Example.fta")
                            {
                                General.ProjectControl.FocusedNode = SNode;
                            }
                        }
                    }
                }
            }
        }

        private void panelControl1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
