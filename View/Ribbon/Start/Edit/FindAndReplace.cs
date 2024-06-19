using System;
using System.Collections.Generic;
using System.Linq;
using FaultTreeAnalysis.Common;
using DevExpress.XtraTreeList.Nodes;
using FaultTreeAnalysis.Model.Enum;

namespace FaultTreeAnalysis.View.Ribbon.Start.Edit
{
    public partial class FindAndReplace : DevExpress.XtraEditors.XtraForm
    {
        public FindAndReplace()
        {
            InitializeComponent();
        }

        public string filterText = "";
        public int NowNodeNum = -1;
        public DrawData NowID = null;

        private void SearchNodes(TreeListNodes nodes)
        {
            foreach (TreeListNode node in nodes)
            {
                HighLightNodes(node);
                // 如果当前节点下还包括子节点，就调用递归
                if (node.Nodes.Count > 0)
                {
                    SearchNodes(node.Nodes);
                }
            }
        }

        private void ReplaceNodes(TreeListNodes nodes)
        {
            foreach (TreeListNode node in nodes)
            {
                ReplaceNode(node, false);
                // 如果当前节点下还包括子节点，就调用递归
                if (node.Nodes.Count > 0)
                {
                    ReplaceNodes(node.Nodes);
                }
            }
        }

        private void ReplaceNodesAll(TreeListNodes nodes)
        {
            foreach (TreeListNode node in nodes)
            {
                ReplaceNode(node, true);
                // 如果当前节点下还包括子节点，就调用递归
                if (node.Nodes.Count > 0)
                {
                    ReplaceNodesAll(node.Nodes);
                }
            }
        }

        private void HighLightNodes(TreeListNode node)
        {
            if (node.GetValue("Data") != null)
            {
                DrawData nodeData = (DrawData)node.GetValue("Data");
                if (nodeData == NowID)
                {
                    General.FTATree.FocusedNode = node;
                }
            }
        }

        private void ReplaceNode(TreeListNode node, bool isall)
        {
            if (node.GetValue("Data") != null)
            {
                DrawData nodeData = (DrawData)node.GetValue("Data");
                if (isall == false)
                {
                    if (nodeData == NowID)
                    {
                        if (checkedComboBoxEdit2.EditValue.ToString().Contains("Identifier") || checkedComboBoxEdit2.EditValue.ToString().Contains("编号"))
                        {
                            string newstr = node.GetValue("Identifier").ToString();
                            if (newstr.Contains(textEdit2.Text))
                            {
                                newstr = newstr.Replace(textEdit2.Text, textEdit3.Text);
                                node.SetValue("Identifier", newstr);

                                //刷新图
                                DrawData dt = ((FaultTreeAnalysis.VirtualDrawData)General.FTATree.DataSource).data;
                                List<DrawData> rows = dt.GetAllData(General.FtaProgram.CurrentSystem);
                                for (int i = 0; i <= rows.Count - 1; i++)
                                {
                                    if (rows[i].Identifier != null && rows[i] == nodeData)
                                    {
                                        rows[i].Identifier = newstr;
                                        NowID = rows[i];
                                    }
                                }
                            }
                        }
                        if (checkedComboBoxEdit2.EditValue.ToString().Contains("Description") || checkedComboBoxEdit2.EditValue.ToString().Contains("描述"))
                        {
                            string newstr = node.GetValue("Comment1").ToString();
                            if (newstr.Contains(textEdit2.Text))
                            {
                                newstr = newstr.Replace(textEdit2.Text, textEdit3.Text);
                                node.SetValue("Comment1", newstr);

                                //刷新图
                                DrawData dt = ((FaultTreeAnalysis.VirtualDrawData)General.FTATree.DataSource).data;
                                List<DrawData> rows = dt.GetAllData(General.FtaProgram.CurrentSystem);
                                for (int i = 0; i <= rows.Count - 1; i++)
                                {
                                    if (rows[i].Identifier != null && rows[i] == nodeData)
                                    {
                                        rows[i].Comment1 = newstr;
                                        NowID = rows[i];
                                    }
                                }
                            }
                        }
                        HighLightNodes(node);
                    }
                }
                else
                {
                    NowID = null;
                    if (checkedComboBoxEdit2.EditValue.ToString().Contains("Identifier") || checkedComboBoxEdit2.EditValue.ToString().Contains("编号"))
                    {
                        string newstr = node.GetValue("Identifier").ToString();
                        if (newstr.Contains(textEdit2.Text))
                        {
                            newstr = newstr.Replace(textEdit2.Text, textEdit3.Text);
                            node.SetValue("Identifier", newstr);

                            //刷新图
                            DrawData dt = ((FaultTreeAnalysis.VirtualDrawData)General.FTATree.DataSource).data;
                            List<DrawData> rows = dt.GetAllData(General.FtaProgram.CurrentSystem);
                            for (int i = 0; i <= rows.Count - 1; i++)
                            {
                                if (rows[i].Identifier != null && rows[i] == nodeData)
                                {
                                    rows[i].Identifier = newstr;
                                    NowID = rows[i];
                                }
                            }
                        }
                    }
                    if (checkedComboBoxEdit2.EditValue.ToString().Contains("Description") || checkedComboBoxEdit2.EditValue.ToString().Contains("描述"))
                    {
                        string newstr = node.GetValue("Comment1").ToString();
                        if (newstr.Contains(textEdit2.Text))
                        {
                            newstr = newstr.Replace(textEdit2.Text, textEdit3.Text);
                            node.SetValue("Comment1", newstr);

                            //刷新图
                            DrawData dt = ((FaultTreeAnalysis.VirtualDrawData)General.FTATree.DataSource).data;
                            List<DrawData> rows = dt.GetAllData(General.FtaProgram.CurrentSystem);
                            for (int i = 0; i <= rows.Count - 1; i++)
                            {
                                if (rows[i].Identifier != null && rows[i] == nodeData)
                                {
                                    rows[i].Comment1 = newstr;
                                    NowID = rows[i];
                                }
                            }
                        }
                    }
                    HighLightNodes(node);
                }
            }
        }

        /// <summary>
        /// 查找下一个
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            General.TryCatch(() =>
            {
                if (General.FTATree.DataSource != null && textEdit1.Text != "")
                {
                    General.FTATree.ExpandAll();

                    filterText = textEdit1.Text;

                    DrawData dt = ((FaultTreeAnalysis.VirtualDrawData)General.FTATree.DataSource).data;

                    List<DrawData> rows = dt.GetAllData(General.FtaProgram.CurrentSystem);

                    if (General.FTATree.FocusedNode != null)
                    {
                        if ((bool)radioGroup2.EditValue)//从上往下
                        {
                            for (int i = 1; i <= rows.Count - 1; i++)
                            {
                                if (rows[i].Identifier != null && rows[i] == (DrawData)General.FTATree.FocusedNode.GetValue("Data"))
                                {
                                    NowNodeNum = i;
                                    break;
                                }
                            }
                        }
                        else//反向
                        {
                            for (int i = rows.Count - 1; i >= 1; i--)
                            {
                                if (rows[i].Identifier != null && rows[i] == (DrawData)General.FTATree.FocusedNode.GetValue("Data"))
                                {
                                    NowNodeNum = i;
                                    break;
                                }
                            }
                        }
                    }

                    bool check = false;

                    if ((bool)radioGroup2.EditValue)//从上往下
                    {
                        for (int i = 1; i <= rows.Count - 1; i++)
                        {
                            if (i > NowNodeNum)
                            {
                                if (checkEdit1.Checked)//区分大小写
                                {
                                    if (comboBoxEdit1.SelectedIndex == 1)//全字匹配
                                    {
                                        int Num1 = rows.Count;
                                        int Num2 = rows.Count;
                                        int Num3 = rows.Count;
                                        int Num4 = rows.Count;
                                        if (checkedComboBoxEdit1.EditValue.ToString().Contains("Identifier") || checkedComboBoxEdit1.EditValue.ToString().Contains("编号"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].Identifier == filterText)
                                            {
                                                NowID = rows[i];
                                                Num1 = i;
                                                check = true;
                                            }
                                        }
                                        if (checkedComboBoxEdit1.EditValue.ToString().Contains("Description") || checkedComboBoxEdit1.EditValue.ToString().Contains("描述"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].Comment1 != null && rows[i].Comment1 == filterText)
                                            {
                                                NowID = rows[i];
                                                Num2 = i;
                                                check = true;
                                            }
                                        }
                                        if (checkedComboBoxEdit1.EditValue.ToString().Contains("ParentID") || checkedComboBoxEdit1.EditValue.ToString().Contains("父节点"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].ParentID != null && rows[i].ParentID == filterText)
                                            {
                                                NowID = rows[i];
                                                Num3 = i;
                                                check = true;
                                            }
                                        }
                                        if (checkedComboBoxEdit1.EditValue.ToString().Contains("LogicalCondition") || checkedComboBoxEdit1.EditValue.ToString().Contains("逻辑条件"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].LogicalCondition != null && rows[i].LogicalCondition == filterText)
                                            {
                                                NowID = rows[i];
                                                Num4 = i;
                                                check = true;
                                            }
                                        }

                                        //取最小
                                        if (check)
                                        {
                                            NowNodeNum = (new Int32[] { Num1, Num2, Num3, Num4 }).Min();
                                            break;
                                        }
                                    }
                                    else if (comboBoxEdit1.SelectedIndex == 0)//包含
                                    {
                                        int Num1 = rows.Count;
                                        int Num2 = rows.Count;
                                        int Num3 = rows.Count;
                                        int Num4 = rows.Count;
                                        if (checkedComboBoxEdit1.EditValue.ToString().Contains("Identifier") || checkedComboBoxEdit1.EditValue.ToString().Contains("编号"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].Identifier.Contains(filterText))
                                            {
                                                NowID = rows[i];
                                                Num1 = i;
                                                check = true;
                                            }
                                        }
                                        if (checkedComboBoxEdit1.EditValue.ToString().Contains("Description") || checkedComboBoxEdit1.EditValue.ToString().Contains("描述"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].Comment1 != null && rows[i].Comment1.Contains(filterText))
                                            {
                                                NowID = rows[i];
                                                Num2 = i;
                                                check = true;
                                            }
                                        }
                                        if (checkedComboBoxEdit1.EditValue.ToString().Contains("ParentID") || checkedComboBoxEdit1.EditValue.ToString().Contains("父节点"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].ParentID != null && rows[i].ParentID.Contains(filterText))
                                            {
                                                NowID = rows[i];
                                                Num3 = i;
                                                check = true;
                                            }
                                        }
                                        if (checkedComboBoxEdit1.EditValue.ToString().Contains("LogicalCondition") || checkedComboBoxEdit1.EditValue.ToString().Contains("逻辑条件"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].LogicalCondition != null && rows[i].LogicalCondition.Contains(filterText))
                                            {
                                                NowID = rows[i];
                                                Num4 = i;
                                                check = true;
                                            }
                                        }

                                        //取最小
                                        if (check)
                                        {
                                            NowNodeNum = (new Int32[] { Num1, Num2, Num3, Num4 }).Min();
                                            break;
                                        }
                                    }
                                }
                                else//不区分大小写
                                {
                                    if (comboBoxEdit1.SelectedIndex == 1)//全字匹配
                                    {
                                        int Num1 = rows.Count;
                                        int Num2 = rows.Count;
                                        int Num3 = rows.Count;
                                        int Num4 = rows.Count;
                                        if (checkedComboBoxEdit1.EditValue.ToString().Contains("Identifier") || checkedComboBoxEdit1.EditValue.ToString().Contains("编号"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].Identifier.ToUpper() == filterText.ToUpper())
                                            {
                                                NowID = rows[i];
                                                Num1 = i;
                                                check = true;
                                            }
                                        }
                                        if (checkedComboBoxEdit1.EditValue.ToString().Contains("Description") || checkedComboBoxEdit1.EditValue.ToString().Contains("描述"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].Comment1 != null && rows[i].Comment1.ToUpper() == filterText.ToUpper())
                                            {
                                                NowID = rows[i];
                                                Num2 = i;
                                                check = true;
                                            }
                                        }
                                        if (checkedComboBoxEdit1.EditValue.ToString().Contains("ParentID") || checkedComboBoxEdit1.EditValue.ToString().Contains("父节点"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].ParentID != null && rows[i].ParentID.ToUpper() == filterText.ToUpper())
                                            {
                                                NowID = rows[i];
                                                Num3 = i;
                                                check = true;
                                            }
                                        }
                                        if (checkedComboBoxEdit1.EditValue.ToString().Contains("LogicalCondition") || checkedComboBoxEdit1.EditValue.ToString().Contains("逻辑条件"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].LogicalCondition != null && rows[i].LogicalCondition.ToUpper() == filterText.ToUpper())
                                            {
                                                NowID = rows[i];
                                                Num4 = i;
                                                check = true;
                                            }
                                        }

                                        //取最小
                                        if (check)
                                        {
                                            NowNodeNum = (new Int32[] { Num1, Num2, Num3, Num4 }).Min();
                                            break;
                                        }
                                    }
                                    else if (comboBoxEdit1.SelectedIndex == 0)//包含
                                    {
                                        int Num1 = rows.Count;
                                        int Num2 = rows.Count;
                                        int Num3 = rows.Count;
                                        int Num4 = rows.Count;
                                        if (checkedComboBoxEdit1.EditValue.ToString().Contains("Identifier") || checkedComboBoxEdit1.EditValue.ToString().Contains("编号"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].Identifier.ToUpper().Contains(filterText.ToUpper()))
                                            {
                                                NowID = rows[i];
                                                Num1 = i;
                                                check = true;
                                            }
                                        }
                                        if (checkedComboBoxEdit1.EditValue.ToString().Contains("Description") || checkedComboBoxEdit1.EditValue.ToString().Contains("描述"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].Comment1 != null && rows[i].Comment1.ToUpper().Contains(filterText.ToUpper()))
                                            {
                                                NowID = rows[i];
                                                Num2 = i;
                                                check = true;
                                            }
                                        }
                                        if (checkedComboBoxEdit1.EditValue.ToString().Contains("ParentID") || checkedComboBoxEdit1.EditValue.ToString().Contains("父节点"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].ParentID != null && rows[i].ParentID.ToUpper().Contains(filterText.ToUpper()))
                                            {
                                                NowID = rows[i];
                                                Num3 = i;
                                                check = true;
                                            }
                                        }
                                        if (checkedComboBoxEdit1.EditValue.ToString().Contains("LogicalCondition") || checkedComboBoxEdit1.EditValue.ToString().Contains("逻辑条件"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].LogicalCondition != null && rows[i].LogicalCondition.ToUpper().Contains(filterText.ToUpper()))
                                            {
                                                NowID = rows[i];
                                                Num4 = i;
                                                check = true;
                                            }
                                        }

                                        //取最小
                                        if (check)
                                        {
                                            NowNodeNum = (new Int32[] { Num1, Num2, Num3, Num4 }).Min();
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else//反向
                    {
                        for (int i = rows.Count - 1; i >= 1; i--)
                        {
                            if (i < NowNodeNum)
                            {
                                if (checkEdit1.Checked)//区分大小写
                                {
                                    if (comboBoxEdit1.SelectedIndex == 1)//全字匹配
                                    {
                                        int Num1 = -1;
                                        int Num2 = -1;
                                        int Num3 = -1;
                                        int Num4 = -1;
                                        if (checkedComboBoxEdit1.EditValue.ToString().Contains("Identifier") || checkedComboBoxEdit1.EditValue.ToString().Contains("编号"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].Identifier == filterText)
                                            {
                                                NowID = rows[i];
                                                Num1 = i;
                                                check = true;
                                            }
                                        }
                                        if (checkedComboBoxEdit1.EditValue.ToString().Contains("Description") || checkedComboBoxEdit1.EditValue.ToString().Contains("描述"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].Comment1 != null && rows[i].Comment1 == filterText)
                                            {
                                                NowID = rows[i];
                                                Num2 = i;
                                                check = true;
                                            }
                                        }
                                        if (checkedComboBoxEdit1.EditValue.ToString().Contains("ParentID") || checkedComboBoxEdit1.EditValue.ToString().Contains("父节点"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].ParentID != null && rows[i].ParentID == filterText)
                                            {
                                                NowID = rows[i];
                                                Num3 = i;
                                                check = true;
                                            }
                                        }
                                        if (checkedComboBoxEdit1.EditValue.ToString().Contains("LogicalCondition") || checkedComboBoxEdit1.EditValue.ToString().Contains("逻辑条件"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].LogicalCondition != null && rows[i].LogicalCondition == filterText)
                                            {
                                                NowID = rows[i];
                                                Num4 = i;
                                                check = true;
                                            }
                                        }

                                        //取最大
                                        if (check)
                                        {
                                            NowNodeNum = (new Int32[] { Num1, Num2, Num3, Num4 }).Max();
                                            break;
                                        }
                                    }
                                    else if (comboBoxEdit1.SelectedIndex == 0)//包含
                                    {
                                        int Num1 = -1;
                                        int Num2 = -1;
                                        int Num3 = -1;
                                        int Num4 = -1;
                                        if (checkedComboBoxEdit1.EditValue.ToString().Contains("Identifier") || checkedComboBoxEdit1.EditValue.ToString().Contains("编号"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].Identifier.Contains(filterText))
                                            {
                                                NowID = rows[i];
                                                Num1 = i;
                                                check = true;
                                            }
                                        }
                                        if (checkedComboBoxEdit1.EditValue.ToString().Contains("Description") || checkedComboBoxEdit1.EditValue.ToString().Contains("描述"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].Comment1 != null && rows[i].Comment1.Contains(filterText))
                                            {
                                                NowID = rows[i];
                                                Num2 = i;
                                                check = true;
                                            }
                                        }
                                        if (checkedComboBoxEdit1.EditValue.ToString().Contains("ParentID") || checkedComboBoxEdit1.EditValue.ToString().Contains("父节点"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].ParentID != null && rows[i].ParentID.Contains(filterText.ToUpper()))
                                            {
                                                NowID = rows[i];
                                                Num3 = i;
                                                check = true;
                                            }
                                        }
                                        if (checkedComboBoxEdit1.EditValue.ToString().Contains("LogicalCondition") || checkedComboBoxEdit1.EditValue.ToString().Contains("逻辑条件"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].LogicalCondition != null && rows[i].LogicalCondition.Contains(filterText.ToUpper()))
                                            {
                                                NowID = rows[i];
                                                Num4 = i;
                                                check = true;
                                            }
                                        }

                                        //取最大
                                        if (check)
                                        {
                                            NowNodeNum = (new Int32[] { Num1, Num2, Num3, Num4 }).Max();
                                            break;
                                        }
                                    }
                                }
                                else//不区分大小写
                                {
                                    if (comboBoxEdit1.SelectedIndex == 1)//全字匹配
                                    {
                                        int Num1 = -1;
                                        int Num2 = -1;
                                        int Num3 = -1;
                                        int Num4 = -1;
                                        if (checkedComboBoxEdit1.EditValue.ToString().Contains("Identifier") || checkedComboBoxEdit1.EditValue.ToString().Contains("编号"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].Identifier.ToUpper() == filterText.ToUpper())
                                            {
                                                NowID = rows[i];
                                                Num1 = i;
                                                check = true;
                                            }
                                        }
                                        if (checkedComboBoxEdit1.EditValue.ToString().Contains("Description") || checkedComboBoxEdit1.EditValue.ToString().Contains("描述"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].Comment1 != null && rows[i].Comment1.ToUpper() == filterText.ToUpper())
                                            {
                                                NowID = rows[i];
                                                Num2 = i;
                                                check = true;
                                            }
                                        }
                                        if (checkedComboBoxEdit1.EditValue.ToString().Contains("ParentID") || checkedComboBoxEdit1.EditValue.ToString().Contains("父节点"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].ParentID != null && rows[i].ParentID.ToUpper() == filterText.ToUpper())
                                            {
                                                NowID = rows[i];
                                                Num3 = i;
                                                check = true;
                                            }
                                        }
                                        if (checkedComboBoxEdit1.EditValue.ToString().Contains("LogicalCondition") || checkedComboBoxEdit1.EditValue.ToString().Contains("逻辑条件"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].LogicalCondition != null && rows[i].LogicalCondition.ToUpper() == filterText.ToUpper())
                                            {
                                                NowID = rows[i];
                                                Num4 = i;
                                                check = true;
                                            }
                                        }

                                        //取最大
                                        if (check)
                                        {
                                            NowNodeNum = (new Int32[] { Num1, Num2, Num3, Num4 }).Max();
                                            break;
                                        }
                                    }
                                    else if (comboBoxEdit1.SelectedIndex == 0)//包含
                                    {
                                        int Num1 = -1;
                                        int Num2 = -1;
                                        int Num3 = -1;
                                        int Num4 = -1;
                                        if (checkedComboBoxEdit1.EditValue.ToString().Contains("Identifier") || checkedComboBoxEdit1.EditValue.ToString().Contains("编号"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].Identifier.ToUpper().Contains(filterText.ToUpper()))
                                            {
                                                NowID = rows[i];
                                                Num1 = i;
                                                check = true;
                                            }
                                        }
                                        if (checkedComboBoxEdit1.EditValue.ToString().Contains("Description") || checkedComboBoxEdit1.EditValue.ToString().Contains("描述"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].Comment1 != null && rows[i].Comment1.ToUpper().Contains(filterText.ToUpper()))
                                            {
                                                NowID = rows[i];
                                                Num2 = i;
                                                check = true;
                                            }
                                        }
                                        if (checkedComboBoxEdit1.EditValue.ToString().Contains("ParentID") || checkedComboBoxEdit1.EditValue.ToString().Contains("父节点"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].ParentID != null && rows[i].ParentID.ToUpper().Contains(filterText.ToUpper()))
                                            {
                                                NowID = rows[i];
                                                Num3 = i;
                                                check = true;
                                            }
                                        }
                                        if (checkedComboBoxEdit1.EditValue.ToString().Contains("LogicalCondition") || checkedComboBoxEdit1.EditValue.ToString().Contains("逻辑条件"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].LogicalCondition != null && rows[i].LogicalCondition.Contains(filterText.ToUpper()))
                                            {
                                                NowID = rows[i];
                                                Num4 = i;
                                                check = true;
                                            }
                                        }

                                        //取最大
                                        if (check)
                                        {
                                            NowNodeNum = (new Int32[] { Num1, Num2, Num3, Num4 }).Max();
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    SearchNodes(General.FTATree.Nodes);

                    if (check == false)
                    {
                        if ((bool)radioGroup2.EditValue)//从上往下
                        {
                            NowNodeNum = -1;
                        }
                        else
                        {
                            NowNodeNum = rows.Count;
                        }
                        NowID = null;
                        General.FTATree.FocusedNode = null;
                    }
                }
            });
        }

        /// <summary>
        /// 替换下一个
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton4_Click(object sender, EventArgs e)
        {
            General.TryCatch(() =>
            {
                if (General.FTATree.DataSource != null && textEdit2.Text != "")
                {
                    General.FTATree.ExpandAll();

                    filterText = textEdit2.Text;

                    DrawData dt = ((FaultTreeAnalysis.VirtualDrawData)General.FTATree.DataSource).data;

                    List<DrawData> rows = dt.GetAllData(General.FtaProgram.CurrentSystem);

                    if (General.FTATree.FocusedNode != null)
                    {
                        if ((bool)radioGroup1.EditValue)//从上往下
                        {
                            for (int i = 1; i <= rows.Count - 1; i++)
                            {
                                if (rows[i].Identifier != null && rows[i] == (DrawData)General.FTATree.FocusedNode.GetValue("Data"))
                                {
                                    NowNodeNum = i;
                                    break;
                                }
                            }
                        }
                        else//反向
                        {
                            for (int i = rows.Count - 1; i >= 1; i--)
                            {
                                if (rows[i].Identifier != null && rows[i] == (DrawData)General.FTATree.FocusedNode.GetValue("Data"))
                                {
                                    NowNodeNum = i;
                                    break;
                                }
                            }
                        }
                    }

                    bool check = false;

                    if ((bool)radioGroup1.EditValue)//从上往下
                    {
                        for (int i = 1; i <= rows.Count - 1; i++)
                        {
                            if (i > NowNodeNum)
                            {
                                if (checkEdit2.Checked)//区分大小写
                                {
                                    if (comboBoxEdit2.SelectedIndex == 1)//全字匹配
                                    {
                                        int Num1 = rows.Count;
                                        int Num2 = rows.Count;
                                        if (checkedComboBoxEdit2.EditValue.ToString().Contains("Identifier") || checkedComboBoxEdit2.EditValue.ToString().Contains("编号"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].Identifier == filterText)
                                            {
                                                NowID = rows[i];
                                                Num1 = i;
                                                check = true;
                                            }
                                        }
                                        if (checkedComboBoxEdit2.EditValue.ToString().Contains("Description") || checkedComboBoxEdit2.EditValue.ToString().Contains("描述"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].Comment1 != null && rows[i].Comment1 == filterText)
                                            {
                                                NowID = rows[i];
                                                Num2 = i;
                                                check = true;
                                            }
                                        }

                                        //取最小
                                        if (check)
                                        {
                                            NowNodeNum = (new Int32[] { Num1, Num2 }).Min();
                                            break;
                                        }
                                    }
                                    else if (comboBoxEdit2.SelectedIndex == 0)//包含
                                    {
                                        int Num1 = rows.Count;
                                        int Num2 = rows.Count;
                                        if (checkedComboBoxEdit2.EditValue.ToString().Contains("Identifier") || checkedComboBoxEdit2.EditValue.ToString().Contains("编号"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].Identifier.Contains(filterText))
                                            {
                                                NowID = rows[i];
                                                Num1 = i;
                                                check = true;
                                            }
                                        }
                                        if (checkedComboBoxEdit2.EditValue.ToString().Contains("Description") || checkedComboBoxEdit2.EditValue.ToString().Contains("描述"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].Comment1 != null && rows[i].Comment1.Contains(filterText))
                                            {
                                                NowID = rows[i];
                                                Num2 = i;
                                                check = true;
                                            }
                                        }

                                        //取最小
                                        if (check)
                                        {
                                            NowNodeNum = (new Int32[] { Num1, Num2 }).Min();
                                            break;
                                        }
                                    }
                                }
                                else//不区分大小写
                                {
                                    if (comboBoxEdit2.SelectedIndex == 1)//全字匹配
                                    {
                                        int Num1 = rows.Count;
                                        int Num2 = rows.Count;
                                        if (checkedComboBoxEdit2.EditValue.ToString().Contains("Identifier") || checkedComboBoxEdit2.EditValue.ToString().Contains("编号"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].Identifier.ToUpper() == filterText.ToUpper())
                                            {
                                                NowID = rows[i];
                                                Num1 = i;
                                                check = true;
                                            }
                                        }
                                        if (checkedComboBoxEdit2.EditValue.ToString().Contains("Description") || checkedComboBoxEdit2.EditValue.ToString().Contains("描述"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].Comment1 != null && rows[i].Comment1.ToUpper() == filterText.ToUpper())
                                            {
                                                NowID = rows[i];
                                                Num2 = i;
                                                check = true;
                                            }
                                        }

                                        //取最小
                                        if (check)
                                        {
                                            NowNodeNum = (new Int32[] { Num1, Num2 }).Min();
                                            break;
                                        }
                                    }
                                    else if (comboBoxEdit2.SelectedIndex == 0)//包含
                                    {
                                        int Num1 = rows.Count;
                                        int Num2 = rows.Count;
                                        if (checkedComboBoxEdit2.EditValue.ToString().Contains("Identifier") || checkedComboBoxEdit2.EditValue.ToString().Contains("编号"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].Identifier.ToUpper().Contains(filterText.ToUpper()))
                                            {
                                                NowID = rows[i];
                                                Num1 = i;
                                                check = true;
                                            }
                                        }
                                        if (checkedComboBoxEdit2.EditValue.ToString().Contains("Description") || checkedComboBoxEdit2.EditValue.ToString().Contains("描述"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].Comment1 != null && rows[i].Comment1.ToUpper().Contains(filterText.ToUpper()))
                                            {
                                                NowID = rows[i];
                                                Num2 = i;
                                                check = true;
                                            }
                                        }

                                        //取最小
                                        if (check)
                                        {
                                            NowNodeNum = (new Int32[] { Num1, Num2 }).Min();
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else//反向
                    {
                        for (int i = rows.Count - 1; i >= 1; i--)
                        {
                            if (i < NowNodeNum)
                            {
                                if (checkEdit2.Checked)//区分大小写
                                {
                                    if (comboBoxEdit2.SelectedIndex == 1)//全字匹配
                                    {
                                        int Num1 = -1;
                                        int Num2 = -1;
                                        if (checkedComboBoxEdit2.EditValue.ToString().Contains("Identifier") || checkedComboBoxEdit2.EditValue.ToString().Contains("编号"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].Identifier == filterText)
                                            {
                                                NowID = rows[i];
                                                Num1 = i;
                                                check = true;
                                            }
                                        }
                                        if (checkedComboBoxEdit2.EditValue.ToString().Contains("Description") || checkedComboBoxEdit2.EditValue.ToString().Contains("描述"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].Comment1 != null && rows[i].Comment1 == filterText)
                                            {
                                                NowID = rows[i];
                                                Num2 = i;
                                                check = true;
                                            }
                                        }

                                        //取最大
                                        if (check)
                                        {
                                            NowNodeNum = (new Int32[] { Num1, Num2 }).Max();
                                            break;
                                        }
                                    }
                                    else if (comboBoxEdit2.SelectedIndex == 0)//包含
                                    {
                                        int Num1 = -1;
                                        int Num2 = -1;
                                        if (checkedComboBoxEdit2.EditValue.ToString().Contains("Identifier") || checkedComboBoxEdit2.EditValue.ToString().Contains("编号"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].Identifier.Contains(filterText))
                                            {
                                                NowID = rows[i];
                                                Num1 = i;
                                                check = true;
                                            }
                                        }
                                        if (checkedComboBoxEdit2.EditValue.ToString().Contains("Description") || checkedComboBoxEdit2.EditValue.ToString().Contains("描述"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].Comment1 != null && rows[i].Comment1.Contains(filterText))
                                            {
                                                NowID = rows[i];
                                                Num2 = i;
                                                check = true;
                                            }
                                        }

                                        //取最大
                                        if (check)
                                        {
                                            NowNodeNum = (new Int32[] { Num1, Num2 }).Max();
                                            break;
                                        }
                                    }
                                }
                                else//不区分大小写
                                {
                                    if (comboBoxEdit2.SelectedIndex == 1)//全字匹配
                                    {
                                        int Num1 = -1;
                                        int Num2 = -1;
                                        if (checkedComboBoxEdit2.EditValue.ToString().Contains("Identifier") || checkedComboBoxEdit2.EditValue.ToString().Contains("编号"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].Identifier.ToUpper() == filterText.ToUpper())
                                            {
                                                NowID = rows[i];
                                                Num1 = i;
                                                check = true;
                                            }
                                        }
                                        if (checkedComboBoxEdit2.EditValue.ToString().Contains("Description") || checkedComboBoxEdit2.EditValue.ToString().Contains("描述"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].Comment1 != null && rows[i].Comment1.ToUpper() == filterText.ToUpper())
                                            {
                                                NowID = rows[i];
                                                Num2 = i;
                                                check = true;
                                            }
                                        }

                                        //取最大
                                        if (check)
                                        {
                                            NowNodeNum = (new Int32[] { Num1, Num2 }).Max();
                                            break;
                                        }
                                    }
                                    else if (comboBoxEdit2.SelectedIndex == 0)//包含
                                    {
                                        int Num1 = -1;
                                        int Num2 = -1;
                                        if (checkedComboBoxEdit2.EditValue.ToString().Contains("Identifier") || checkedComboBoxEdit2.EditValue.ToString().Contains("编号"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].Identifier.ToUpper().Contains(filterText.ToUpper()))
                                            {
                                                NowID = rows[i];
                                                Num1 = i;
                                                check = true;
                                            }
                                        }
                                        if (checkedComboBoxEdit2.EditValue.ToString().Contains("Description") || checkedComboBoxEdit2.EditValue.ToString().Contains("描述"))
                                        {
                                            if (rows[i].Identifier != null && rows[i].Comment1 != null && rows[i].Comment1.ToUpper().Contains(filterText.ToUpper()))
                                            {
                                                NowID = rows[i];
                                                Num2 = i;
                                                check = true;
                                            }
                                        }

                                        //取最大
                                        if (check)
                                        {
                                            NowNodeNum = (new Int32[] { Num1, Num2 }).Max();
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    ReplaceNodes(General.FTATree.Nodes);

                    //做历史记录
                    General.FtaProgram.CurrentSystem.TakeBehavor();

                    //刷新
                    General.FTATree.CloseEditor();
                    General.FTATree.Update();
                    General.InvokeHandler(GlobalEvent.UpdateLayout);

                    if (check == false)
                    {
                        if ((bool)radioGroup1.EditValue)//从上往下
                        {
                            NowNodeNum = -1;
                        }
                        else
                        {
                            NowNodeNum = rows.Count;
                        }
                        NowID = null;
                        General.FTATree.FocusedNode = null;
                    }
                }
            });
        }

        /// <summary>
        /// 替换全部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton5_Click(object sender, EventArgs e)
        {
            General.TryCatch(() =>
            {
                General.FTATree.ExpandAll();
                ReplaceNodesAll(General.FTATree.Nodes);

                //做历史记录
                General.FtaProgram.CurrentSystem.TakeBehavor();

                //刷新
                General.FTATree.CloseEditor();
                General.FTATree.Update();
                General.InvokeHandler(GlobalEvent.UpdateLayout);
            });
        }

        public void RefreshText()
        {
            try
            {
                this.Text = General.FtaProgram.String.FindAndReplace_Text;
                xtraTabPage1.Text = General.FtaProgram.String.FindAndReplace_TabFind;
                xtraTabPage2.Text = General.FtaProgram.String.FindAndReplace_TabReplace;
                labelControl1.Text = General.FtaProgram.String.FindAndReplace_SearchContent;
                labelControl2.Text = General.FtaProgram.String.FindAndReplace_ColumnRange;
                labelControl3.Text = General.FtaProgram.String.FindAndReplace_Matching;
                checkEdit1.Text = General.FtaProgram.String.FindAndReplace_CaseSensitive;
                this.simpleButton1.Text = General.FtaProgram.String.FindAndReplace_Next;


                labelControl6.Text = General.FtaProgram.String.FindAndReplace_SearchContent;
                labelControl7.Text = General.FtaProgram.String.FindAndReplace_ReplaceContent;
                labelControl5.Text = General.FtaProgram.String.FindAndReplace_ColumnRange;
                labelControl4.Text = General.FtaProgram.String.FindAndReplace_Matching;
                checkEdit2.Text = General.FtaProgram.String.FindAndReplace_CaseSensitive;
                this.simpleButton4.Text = General.FtaProgram.String.FindAndReplace_Next;
                this.simpleButton5.Text = General.FtaProgram.String.FindAndReplace_ReplaceAll;

                radioGroup1.Properties.Items.Clear();
                radioGroup1.Properties.Items.Add(new DevExpress.XtraEditors.Controls.RadioGroupItem { Description = General.FtaProgram.String.FindAndReplace_AboveDown, Value = true });
                radioGroup1.Properties.Items.Add(new DevExpress.XtraEditors.Controls.RadioGroupItem { Description = General.FtaProgram.String.FindAndReplace_DownUp, Value = false });

                radioGroup2.Properties.Items.Clear();
                radioGroup2.Properties.Items.Add(new DevExpress.XtraEditors.Controls.RadioGroupItem { Description = General.FtaProgram.String.FindAndReplace_AboveDown, Value = true });
                radioGroup2.Properties.Items.Add(new DevExpress.XtraEditors.Controls.RadioGroupItem { Description = General.FtaProgram.String.FindAndReplace_DownUp, Value = false });

                comboBoxEdit1.Properties.Items.Clear();
                comboBoxEdit1.Properties.Items.Add(General.FtaProgram.String.FindAndReplace_Inclusion);
                comboBoxEdit1.Properties.Items.Add(General.FtaProgram.String.FindAndReplace_WholeWord);
                comboBoxEdit2.Properties.Items.Clear();
                comboBoxEdit2.Properties.Items.Add(General.FtaProgram.String.FindAndReplace_Inclusion);
                comboBoxEdit2.Properties.Items.Add(General.FtaProgram.String.FindAndReplace_WholeWord);
                comboBoxEdit1.SelectedIndex = 0;
                comboBoxEdit2.SelectedIndex = 0;
            }
            catch (Exception)
            {
            }
        }

        private void FindAndReplace_Load(object sender, EventArgs e)
        {
            General.FTATree.ExpandAll();
            RefreshText();
        }
    }
}