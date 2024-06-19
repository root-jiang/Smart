using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;
using FaultTreeAnalysis.Model.Data;
using FaultTreeAnalysis.Properties;
using FaultTreeAnalysis.Common;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList;
using System.Collections;

namespace FaultTreeAnalysis.View.Ribbon.Start.Excel
{
    public partial class SwitchExport : XtraForm
    {
        public bool isChangeCheck = false;
        public string parentPath = "";
        public List<SystemModel> sysNames = new List<SystemModel>();
        public Dictionary<SystemModel, ProjectModel> sysNamesAll = new Dictionary<SystemModel, ProjectModel>();

        public SwitchExport()
        {
            InitializeComponent();

            this.simpleButton1.Click += SimpleButtonClick;
            this.simpleButton2.Click += SimpleButtonClick;
        }

        private void SimpleButtonClick(object sender, EventArgs e)
        {
            if (sender == this.simpleButton1)
            {//选择要导出的故障树对象
                parentPath = "";
                sysNamesAll.Clear();
                sysNames = new List<SystemModel>();
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    List<TreeListNode> nodes = treeList_Project.GetAllCheckedNodes();

                    foreach (TreeListNode node in nodes)
                    {
                        if (node.Tag != null && node.Tag.ToString().Contains("SystemModel"))
                        {
                            if (node.Level == 2)//带分组的
                            {
                                sysNamesAll.Add((SystemModel)node.Tag, (ProjectModel)node.ParentNode.ParentNode.Tag);
                            }
                            else
                            {
                                sysNamesAll.Add((SystemModel)node.Tag, (ProjectModel)node.ParentNode.Tag);
                            }
                        }
                    }

                    parentPath = dialog.SelectedPath;

                    if (Directory.Exists(parentPath))
                    {
                        this.Close();
                    }
                }
            }
            else
            {
                this.Close();
            }
        }

        private void SwitchExport_Load(object sender, EventArgs e)
        {
            ImageList state_Images = new ImageList();
            state_Images.Images.Add(Resources.project_16x16);
            state_Images.Images.Add(Resources.documentmap_16x16);
            state_Images.Images.Add(Resources.documentmapEdit_16x161);
            state_Images.Images.Add(Resources.packageproduct_16x16);
            treeList_Project.StateImageList = state_Images;

            try
            {
                parentPath = "";
                sysNames = new List<SystemModel>();
                this.simpleButton1.Text = General.FtaProgram.String.OK;
                this.simpleButton2.Text = General.FtaProgram.String.Cancel;
                this.Text = General.FtaProgram.String.SwitchExport_Text;
                checkEdit1.Text = General.FtaProgram.String.SwitchExport_Check;
            }
            catch (Exception)
            {
            }
        }

        private void checkEdit1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkEdit1.Checked)
            {
                treeList_Project.CheckAll();
            }
            else
            {
                treeList_Project.UncheckAll();
            }
        }

        private void treeList_Project_AfterCheckNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            e.Node.Checked = !e.Node.Checked;
        }

        private void treeList_Project_MouseDown(object sender, MouseEventArgs e)
        {
            TreeListHitInfo hit = treeList_Project.CalcHitInfo(e.Location);
            if (hit.Node != null)
            {
                hit.Node.Checked = !hit.Node.Checked;
                hit.Node.SyncNodeCheckState(hit.Node.CheckState);
            }
        }

        /// <summary>
        /// 按类型和名称进行自动排序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeList_Project_CustomColumnSort(object sender, CustomColumnSortEventArgs e)
        {
            if (e.Column != null & e.Column.FieldName == "name")
            {
                string tempPacks1 = "";
                string tempPacks2 = "";

                string Type1 = e.Node1.GetValue("SortType").ToString();
                string Type2 = e.Node2.GetValue("SortType").ToString();

                if (Type1 != "Group")
                {
                    tempPacks1 = e.NodeValue1.ToString();
                }
                else
                {
                    tempPacks1 = "!" + e.NodeValue1.ToString();
                }

                if (Type2 != "Group")
                {
                    tempPacks2 = e.NodeValue2.ToString();
                }
                else
                {
                    tempPacks2 = "!" + e.NodeValue2.ToString();
                }

                int res = Comparer.Default.Compare(tempPacks1, tempPacks2);

                e.Result = res;
            }
        }
    }
}
