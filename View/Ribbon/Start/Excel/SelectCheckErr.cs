using System;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using FaultTreeAnalysis.Properties;
using FaultTreeAnalysis.Common;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList;
using System.Data;
using DevExpress.XtraBars.Docking;

namespace FaultTreeAnalysis.View.Ribbon.Start.Excel
{
    public partial class SelectCheckErr : XtraForm
    {
        public SelectCheckErr()
        {
            InitializeComponent();
        }

        public RibbionEvents REs = null;
        public DockPanel DP = null;

        private void SelectCheckErr_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 自动定位错误或警告对象在图上的位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeList_Project_MouseDown(object sender, MouseEventArgs e)
        {
            TreeListHitInfo hit = treeList_Project.CalcHitInfo(e.Location);
            if (hit.Node != null && hit.Node.Tag != null)
            {
                General.FTATree.FocusedNode = null;
                General.FTATree.ExpandAll();
                DrawData it = (DrawData)hit.Node.Tag;

                Predicate<TreeListNode> match = new Predicate<TreeListNode>(d => (d.GetValue(FixedString.COLUMNAME_DATA) != null && d.GetValue(FixedString.COLUMNAME_DATA) == it));
                TreeListNode nd = General.FTATree.FindNode(match);
                General.FTATree.FocusedNode = nd;
            }
        }

        public void RefreshText()
        {
            DP.Text = General.FtaProgram.String.Check;
            col_type.Caption = General.FtaProgram.String.Check_Type;
            col_name.Caption = General.FtaProgram.String.Check_Name;
            col_info.Caption = General.FtaProgram.String.Check_Info;
        }

        /// <summary>
        /// 刷新错误和警告
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            if (REs != null)
            {
                REs.Tree_Integrity_Check_Before_Calculate();
            }
            else
            {
                return;
            }

            treeList_Project.Nodes.Clear();
            if (REs != null && REs.IncomNames.Rows.Count > 0)
            {
                foreach (DataRow it in REs.IncomNames.Rows)
                {
                    if (it["info"].ToString() == General.FtaProgram.String.IntegrityCheckString_HasNoRoot)
                    {
                        MsgBox.Show(General.FtaProgram.String.IntegrityCheckString_HasNoRoot);
                        return;
                    }

                    if (it["type"].ToString() == "Error")
                    {
                        TreeListNode node_Project = treeList_Project.Nodes.Add(new object[] { it["name"].ToString(), Resources.error_16x16, it["info"].ToString() });
                        node_Project.Tag = (DrawData)it["Data"];
                    }
                    else
                    {
                        TreeListNode node_Project = treeList_Project.Nodes.Add(new object[] { it["name"].ToString(), Resources.warning_16x16, it["info"].ToString() });
                        node_Project.Tag = (DrawData)it["Data"];
                    }
                }

                if (treeList_Project.Nodes.Count == 0)
                {
                    return;
                }

                treeList_Project.ExpandAll();
            }
            else
            {
                MsgBox.Show(General.FtaProgram.String.IntegrityCheckString_CalculateSuccess); 
            }
        }
    }
}