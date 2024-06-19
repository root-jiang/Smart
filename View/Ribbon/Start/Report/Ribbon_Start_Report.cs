using DevExpress.XtraSplashScreen;
using DevExpress.XtraTreeList.Nodes;
using FaultTreeAnalysis.Common;
using FaultTreeAnalysis.Model.Data;
using FaultTreeAnalysis.View.Ribbon;
using FaultTreeAnalysis.View.Ribbon.Start.Excel;
using IntegratedSystem.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace FaultTreeAnalysis
{
    partial class FTAControl
    {
        public DataTable IncomNames = new DataTable();
        /// <summary>
        /// 初始化Ribbon-Start-Report下的菜单
        /// </summary>
        private void Init_Ribbon_Start_Report()
        {
            this.barButtonItem_FTA.ItemClick += BarButtonItem_FTAReport_ItemClick;
            this.barButtonItem_FTADiagram.ItemClick += BarButtonItem_FTAReport_ItemClick;
            this.barButtonItem_Cutset.ItemClick += BarButtonItem_FTAReport_ItemClick;
        }

        /// <summary>
        /// Ribbon按钮项的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BarButtonItem_FTAReport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (e.Item == barButtonItem_FTA)
            {
                DoAction(0);
            }
            else if (e.Item == barButtonItem_FTADiagram)
            {
                DoAction(1);
            }
            else
            {
                DoAction(2);
            }
        }

        //遍历替换转移门
        public void ReplaceTrans(DrawData da, Dictionary<string, HashSet<DrawData>> ds)
        {
            try
            {
                for (int i = da.Children.Count - 1; i >= 0; i--)
                {
                    if (da.Children[i].Type == Model.Enum.DrawType.TransferInGate)
                    {
                        Predicate<DrawData> match = new Predicate<DrawData>(d => (d.Identifier == da.Children[i].Identifier));
                        HashSet<DrawData> hs = new HashSet<DrawData>();
                        ds.TryGetValue(da.Children[i].Identifier, out hs);

                        if (hs != null)
                        {
                            foreach (DrawData val in hs)
                            {
                                if (val.Type != Model.Enum.DrawType.TransferInGate)
                                {
                                    da.Children[i] = val.CopyDrawDataRecurse();
                                    break;
                                }
                            }
                        }
                    }
                    else if (da.Children[i].HasChild)
                    {
                        ReplaceTrans(da.Children[i], ds);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void DoAction(int printingViewIndex)
        {
            this.ccaFunction = new CCAFunction();
            switch (printingViewIndex)
            {
                case 0:
                    {
                        SwitchExport SE = new View.Ribbon.Start.Excel.SwitchExport();
                        SE.treeList_Project.BringToFront();

                        foreach (ProjectModel PM in General.FtaProgram.Projects)
                        {
                            TreeListNode node_Project = SE.treeList_Project.Nodes.Add(new object[] { PM.ProjectName, "Project" });
                            node_Project.StateImageIndex = 0;
                            node_Project.Tag = PM;

                            foreach (SystemModel sys in PM.Systems)
                            {
                                //添加分组
                                TreeListNode node_GroupLevel = null;
                                if (sys.GroupLevel != null && sys.GroupLevel != "")
                                {
                                    foreach (TreeListNode nd in node_Project.Nodes)
                                    {
                                        if (nd.GetDisplayText("name") == sys.GroupLevel)
                                        {
                                            node_GroupLevel = nd;
                                        }
                                    }
                                    if (node_GroupLevel == null)
                                    {
                                        node_GroupLevel = node_Project.Nodes.Add(new object[] { sys.GroupLevel, "Group" });
                                    }

                                    node_GroupLevel.StateImageIndex = 3;
                                    if (!node_GroupLevel.Expanded) node_GroupLevel.Expand();
                                }

                                //添加系统节点
                                if (node_GroupLevel == null)
                                {
                                    TreeListNode node_sys = node_Project.Nodes.Add(new object[] { sys.SystemName, "System" });
                                    node_sys.StateImageIndex = 1;
                                    node_sys.Tag = sys;
                                }
                                else
                                {
                                    TreeListNode node_sys = node_GroupLevel.Nodes.Add(new object[] { sys.SystemName, "System" });
                                    node_sys.StateImageIndex = 1;
                                    node_sys.Tag = sys;
                                }
                            }
                        }

                        if (SE.treeList_Project.Nodes.Count == 0)
                        {
                            return;
                        }

                        SE.treeList_Project.ExpandAll();
                        SE.ShowDialog();

                        if (SE.sysNamesAll.Count > 0 && SE.parentPath != "")
                        {
                            string AllErr = "";
                            foreach (KeyValuePair<SystemModel, ProjectModel> sm in SE.sysNamesAll)
                            {
                                //检查
                                string ErrStr = "";
                                Tree_Integrity_Check_Before_Calculate(sm.Key, false);
                                if (IncomNames.Rows.Count > 0)
                                {
                                    foreach (DataRow it in IncomNames.Rows)
                                    {
                                        if (it["type"].ToString() == "Error")
                                        {
                                            string str = it["name"].ToString() + " : " + it["info"].ToString();

                                            if (it["info"].ToString() == General.FtaProgram.String.IntegrityCheckString_HasNoRoot)
                                            {
                                                if (General.StaticItem_File.Caption.Replace(" ", "") == "")
                                                {
                                                    ErrStr += "\r\n" + str;
                                                }
                                            }
                                            else
                                            {
                                                General.FTATree.FocusedNode = null;
                                                General.FTATree.ExpandAll();
                                                DrawData fData = (DrawData)(it["Data"]);

                                                Predicate<TreeListNode> match = new Predicate<TreeListNode>(d => (d.GetValue(FixedString.COLUMNAME_DATA) != null && d.GetValue(FixedString.COLUMNAME_DATA) == fData));
                                                TreeListNode nd = General.FTATree.FindNode(match);
                                                General.FTATree.FocusedNode = nd;
                                                ErrStr += "\r\n" + str;
                                            }
                                        }
                                    }
                                }

                                if (ErrStr != "")
                                {
                                    AllErr += sm.Value.ProjectName + "/" + sm.Key.SystemName + "：\r\n" + ErrStr.TrimStart(new char[] { '\r' }).TrimStart(new char[] { '\n' }) + "\r\n\r\n";
                                    continue;
                                }
                                //初始化转移门与重复事件集合
                                if (sm.Key.TranferGates == null || sm.Key.RepeatedEvents == null)
                                {
                                    sm.Key.UpdateRepeatedAndTranfer();
                                }
                                foreach (DrawData root in sm.Key.Roots)
                                {
                                    if (sm.Key.TranferGates != null && sm.Key.TranferGates.ContainsKey(root.Identifier) == false)//不是转移门的顶点
                                    {
                                        DrawData NewRoot = root.CopyDrawDataRecurse();

                                        ReplaceTrans(NewRoot, sm.Key.TranferGates);

                                        SystemModel Newsys = new Model.Data.SystemModel();
                                        Newsys.SystemName = sm.Key.SystemName;
                                        Newsys.Roots.Add(NewRoot);
                                        this.ccaFunction.FTARpt(Newsys, NewRoot, SE.parentPath + "\\FTA Report_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + "_" + sm.Key.SystemName + "_" + root.Identifier + ".pdf");
                                    }
                                }
                            }

                            if (AllErr != "")
                            {
                                MsgBox.ShowMax(AllErr.TrimStart(new char[] { '\r' }).TrimStart(new char[] { '\n' }));
                            }
                            else
                            {
                                DialogResult rs = MsgBox.Show("Export succeeded,whether to open the directory?", "", System.Windows.Forms.MessageBoxButtons.YesNo);
                                if (rs == DialogResult.Yes)
                                {
                                    Process.Start(SE.parentPath);
                                }
                            }
                        }
                        break;
                    }
                case 1:
                    {
                        SwitchExport SE = new View.Ribbon.Start.Excel.SwitchExport();
                        SE.treeList_Project.BringToFront();

                        foreach (ProjectModel PM in General.FtaProgram.Projects)
                        {
                            TreeListNode node_Project = SE.treeList_Project.Nodes.Add(new object[] { PM.ProjectName, "Project" });
                            node_Project.StateImageIndex = 0;
                            node_Project.Tag = PM;

                            foreach (SystemModel sys in PM.Systems)
                            {
                                //添加分组
                                TreeListNode node_GroupLevel = null;
                                if (sys.GroupLevel != null && sys.GroupLevel != "")
                                {
                                    foreach (TreeListNode nd in node_Project.Nodes)
                                    {
                                        if (nd.GetDisplayText("name") == sys.GroupLevel)
                                        {
                                            node_GroupLevel = nd;
                                        }
                                    }
                                    if (node_GroupLevel == null)
                                    {
                                        node_GroupLevel = node_Project.Nodes.Add(new object[] { sys.GroupLevel, "Group" });
                                    }

                                    node_GroupLevel.StateImageIndex = 3;
                                    if (!node_GroupLevel.Expanded) node_GroupLevel.Expand();
                                }

                                //添加系统节点
                                if (node_GroupLevel == null)
                                {
                                    TreeListNode node_sys = node_Project.Nodes.Add(new object[] { sys.SystemName, "System" });
                                    node_sys.StateImageIndex = 1;
                                    node_sys.Tag = sys;
                                }
                                else
                                {
                                    TreeListNode node_sys = node_GroupLevel.Nodes.Add(new object[] { sys.SystemName, "System" });
                                    node_sys.StateImageIndex = 1;
                                    node_sys.Tag = sys;
                                }
                            }
                        }

                        if (SE.treeList_Project.Nodes.Count == 0)
                        {
                            return;
                        }

                        SE.treeList_Project.ExpandAll();
                        SE.ShowDialog();

                        if (SE.sysNamesAll.Count > 0 && SE.parentPath != "")
                        {
                            string AllErr = "";
                            foreach (KeyValuePair<SystemModel, ProjectModel> sm in SE.sysNamesAll)
                            {
                                //检查
                                string ErrStr = "";
                                Tree_Integrity_Check_Before_Calculate(sm.Key, false);
                                if (IncomNames.Rows.Count > 0)
                                {
                                    foreach (DataRow it in IncomNames.Rows)
                                    {
                                        if (it["type"].ToString() == "Error")
                                        {
                                            string str = it["name"].ToString() + " : " + it["info"].ToString();

                                            if (it["info"].ToString() == General.FtaProgram.String.IntegrityCheckString_HasNoRoot)
                                            {
                                                if (General.StaticItem_File.Caption.Replace(" ", "") == "")
                                                {
                                                    ErrStr += "\r\n" + str;
                                                }
                                            }
                                            else
                                            {
                                                General.FTATree.FocusedNode = null;
                                                General.FTATree.ExpandAll();
                                                DrawData fData = (DrawData)(it["Data"]);

                                                Predicate<TreeListNode> match = new Predicate<TreeListNode>(d => (d.GetValue(FixedString.COLUMNAME_DATA) != null && d.GetValue(FixedString.COLUMNAME_DATA) == fData));
                                                TreeListNode nd = General.FTATree.FindNode(match);
                                                General.FTATree.FocusedNode = nd;
                                                ErrStr += "\r\n" + str;
                                            }
                                        }
                                    }
                                }

                                if (ErrStr != "")
                                {
                                    AllErr += sm.Value.ProjectName + "/" + sm.Key.SystemName + "：\r\n" + ErrStr.TrimStart(new char[] { '\r' }).TrimStart(new char[] { '\n' }) + "\r\n\r\n";
                                    continue;
                                }

                                //初始化转移门与重复事件集合
                                if (sm.Key.TranferGates == null || sm.Key.RepeatedEvents == null)
                                {
                                    sm.Key.UpdateRepeatedAndTranfer();
                                }
                                foreach (DrawData root in sm.Key.Roots)
                                {
                                    if (sm.Key.TranferGates != null && sm.Key.TranferGates.ContainsKey(root.Identifier) == false)//不是转移门的顶点
                                    {
                                        DrawData NewRoot = root.CopyDrawDataRecurse();

                                        ReplaceTrans(NewRoot, sm.Key.TranferGates);

                                        SystemModel Newsys = new Model.Data.SystemModel();
                                        Newsys.SystemName = sm.Key.SystemName;
                                        Newsys.Roots.Add(NewRoot);
                                        this.ccaFunction.FTARpt_OnlyDiagram(Newsys, NewRoot, SE.parentPath + "\\FTA Report_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + "_" + sm.Key.SystemName + "_" + root.Identifier + ".pdf");
                                    }
                                }
                            }

                            if (AllErr != "")
                            {
                                MsgBox.ShowMax(AllErr.TrimStart(new char[] { '\r' }).TrimStart(new char[] { '\n' }));
                            }
                            else
                            {
                                DialogResult rs = MsgBox.Show("Export succeeded,whether to open the directory?", "", System.Windows.Forms.MessageBoxButtons.YesNo);
                                if (rs == DialogResult.Yes)
                                {
                                    Process.Start(SE.parentPath);
                                }
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        SwitchExport SE = new View.Ribbon.Start.Excel.SwitchExport();
                        SE.treeList_Project.BringToFront();

                        foreach (ProjectModel PM in General.FtaProgram.Projects)
                        {
                            TreeListNode node_Project = SE.treeList_Project.Nodes.Add(new object[] { PM.ProjectName, "Project" });
                            node_Project.StateImageIndex = 0;
                            node_Project.Tag = PM;

                            foreach (SystemModel sys in PM.Systems)
                            {
                                //添加分组
                                TreeListNode node_GroupLevel = null;
                                if (sys.GroupLevel != null && sys.GroupLevel != "")
                                {
                                    foreach (TreeListNode nd in node_Project.Nodes)
                                    {
                                        if (nd.GetDisplayText("name") == sys.GroupLevel)
                                        {
                                            node_GroupLevel = nd;
                                        }
                                    }
                                    if (node_GroupLevel == null)
                                    {
                                        node_GroupLevel = node_Project.Nodes.Add(new object[] { sys.GroupLevel, "Group" });
                                    }

                                    node_GroupLevel.StateImageIndex = 3;
                                    if (!node_GroupLevel.Expanded) node_GroupLevel.Expand();
                                }

                                //添加系统节点
                                if (node_GroupLevel == null)
                                {
                                    TreeListNode node_sys = node_Project.Nodes.Add(new object[] { sys.SystemName, "System" });
                                    node_sys.StateImageIndex = 1;
                                    node_sys.Tag = sys;
                                }
                                else
                                {
                                    TreeListNode node_sys = node_GroupLevel.Nodes.Add(new object[] { sys.SystemName, "System" });
                                    node_sys.StateImageIndex = 1;
                                    node_sys.Tag = sys;
                                }
                            }
                        }

                        if (SE.treeList_Project.Nodes.Count == 0)
                        {
                            return;
                        }

                        SE.treeList_Project.ExpandAll();
                        SE.ShowDialog();

                        if (SE.sysNamesAll.Count > 0 && SE.parentPath != "")
                        {
                            string AllErr = "";
                            foreach (KeyValuePair<SystemModel, ProjectModel> sm in SE.sysNamesAll)
                            {
                                //检查
                                string ErrStr = "";
                                Tree_Integrity_Check_Before_Calculate(sm.Key, true);
                                if (IncomNames.Rows.Count > 0)
                                {
                                    foreach (DataRow it in IncomNames.Rows)
                                    {
                                        if (it["type"].ToString() == "Error")
                                        {
                                            string str = it["name"].ToString() + " : " + it["info"].ToString();

                                            if (it["info"].ToString() == General.FtaProgram.String.IntegrityCheckString_HasNoRoot)
                                            {
                                                if (General.StaticItem_File.Caption.Replace(" ", "") == "")
                                                {
                                                    ErrStr += "\r\n" + str;
                                                }
                                            }
                                            else
                                            {
                                                General.FTATree.FocusedNode = null;
                                                General.FTATree.ExpandAll();
                                                DrawData fData = (DrawData)(it["Data"]);

                                                Predicate<TreeListNode> match = new Predicate<TreeListNode>(d => (d.GetValue(FixedString.COLUMNAME_DATA) != null && d.GetValue(FixedString.COLUMNAME_DATA) == fData));
                                                TreeListNode nd = General.FTATree.FindNode(match);
                                                General.FTATree.FocusedNode = nd;
                                                ErrStr += "\r\n" + str;
                                            }
                                        }
                                    }
                                }

                                if (ErrStr != "")
                                {
                                    AllErr += sm.Value.ProjectName + "/" + sm.Key.SystemName + "：\r\n" + ErrStr.TrimStart(new char[] { '\r' }).TrimStart(new char[] { '\n' }) + "\r\n\r\n";
                                    continue;
                                }

                                //初始化转移门与重复事件集合
                                if (sm.Key.TranferGates == null || sm.Key.RepeatedEvents == null)
                                {
                                    sm.Key.UpdateRepeatedAndTranfer();
                                }
                                foreach (DrawData root in sm.Key.Roots)
                                {
                                    if (sm.Key.TranferGates != null && sm.Key.TranferGates.ContainsKey(root.Identifier) == false)//不是转移门的顶点
                                    {
                                        try
                                        {  //（不能包含中文路径）

                                            c_WaitFormProgress.ShowSplashScreen("计算中......", "正在文件初始化（此过程可能持续几分钟，请耐心等待）......");
                                            string ApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                                            if (Directory.Exists(ApplicationData + "\\ASPECT\\Xfta") == false)
                                            {
                                                Directory.CreateDirectory(ApplicationData + "\\ASPECT\\Xfta");
                                            }
                                            if (Directory.Exists(ApplicationData + "\\ASPECT\\Xfta\\Temp") == false)
                                            {
                                                Directory.CreateDirectory(ApplicationData + "\\ASPECT\\Xfta\\Temp");
                                            }

                                            string exePath = ApplicationData + "\\ASPECT\\Xfta";

                                            Analysis analysis = new Analysis();
                                            String myOPENPSAFilePath = exePath + "\\Temp\\OPENPSA.xml";
                                            //生成标准格式的OPENPSA文件
                                            analysis.GenerateDefinedID(sm.Key, root);

                                            analysis.GenerateOPENPSAFile(sm.Key, root, myOPENPSAFilePath);

                                            //生成脚本文件
                                            string szScriptPath = exePath + "\\Temp\\FNScript.xml";

                                            analysis.GenerateScriptFile(szScriptPath);

                                            //调用函数生成割集和概率
                                            analysis.DFSCalculateProbs(sm.Key, root, myOPENPSAFilePath, szScriptPath, exePath);
                                            c_WaitFormProgress.CloseSplashScreen();
                                        }
                                        catch (Exception ex)
                                        {
                                            c_WaitFormProgress.CloseSplashScreen();
                                            if (ex.HResult != -2146233079 && ex.HResult != -2146232798)
                                            {
                                                AllErr += "\r\n" + sm.Value.ProjectName + "/" + sm.Key.SystemName + "：\r\n" + General.FtaProgram.String.IntegrityCheckString_CalculateFailed + ex.Message;
                                                continue;
                                            }
                                        }

                                        this.ccaFunction.MinimalCutsetList(sm.Key, root, SE.parentPath + "\\Cutset Report_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + "_" + sm.Key.SystemName + "_" + root.Identifier + ".pdf");
                                    }
                                }
                            }

                            if (AllErr != "")
                            {
                                MsgBox.ShowMax(AllErr.TrimStart(new char[] { '\r' }).TrimStart(new char[] { '\n' }));
                            }
                            else
                            {
                                DialogResult rs = MsgBox.Show("Export succeeded,whether to open the directory?", "", System.Windows.Forms.MessageBoxButtons.YesNo);
                                if (rs == DialogResult.Yes)
                                {
                                    Process.Start(SE.parentPath);
                                }
                            }
                        }
                        break;
                    }
            }
        }


        /// <summary>
        /// 故障树完整性检测(1.末节点为事件型节点; 2.该节点FaultRate有值)
        /// </summary>
        /// <returns>true:完整树, false:不完整树</returns>
        public void Tree_Integrity_Check_Before_Calculate(SystemModel sys, bool ifCheckValue)
        {
            IncomNames.Columns.Clear();
            IncomNames.Rows.Clear();

            IncomNames.Columns.Add("Data", typeof(DrawData));
            IncomNames.Columns.Add("name");
            IncomNames.Columns.Add("type");
            IncomNames.Columns.Add("info");

            //项目安全性检测
            if (sys.Roots.Count == 0)
            {
                IncomNames.Rows.Add(new object[] { new DrawData(), "", "Error", General.FtaProgram.String.IntegrityCheckString_HasNoRoot });
            }

            foreach (DrawData data in sys.Roots)
            {
                if (data.IsGateType && !data.HasChild)
                {
                    if (data.Type != Model.Enum.DrawType.TransferInGate)
                    {
                        IncomNames.Rows.Add(new object[] { data, data.Identifier, "Error", General.FtaProgram.String.IntegrityCheckString_HasNoChild });
                    }
                }
                Tree_Integrity_Check_Before_Calculate(data, ifCheckValue);
            }

            //检测结果

            IncomNames.DefaultView.Sort = "type";
            IncomNames = IncomNames.DefaultView.ToTable();
        }

        private void Tree_Integrity_Check_Before_Calculate(DrawData ddTreeNode, bool ifCheckValue)
        {
            if (null == ddTreeNode)
            {
                return;
            }

            //描述
            if (ddTreeNode.Comment1 == null || ddTreeNode.Comment1 == "")
            {
                IncomNames.Rows.Add(new object[] { ddTreeNode, ddTreeNode.Identifier, "Warning", General.FtaProgram.String.IntegrityCheckWarning_Nodescription });
            }

            //if (ddTreeNode.IsGateType && ddTreeNode.Identifier.Contains("Gate") == false)
            //{
            //    IncomNames.Rows.Add(new object[] { ddTreeNode, ddTreeNode.Identifier, "Warning", General.FtaProgram.String.IntegrityCheckWarning_Abnormalidentifier });
            //}

            //if (ddTreeNode.IsGateType == false && ddTreeNode.Identifier.Contains("Event") == false)
            //{
            //    IncomNames.Rows.Add(new object[] { ddTreeNode, ddTreeNode.Identifier, "Warning", General.FtaProgram.String.IntegrityCheckWarning_Abnormalidentifier });
            //}

            //数据检测(非空)
            if (ifCheckValue)
            {

                if (!ddTreeNode.IsGateType && ddTreeNode.Type != Model.Enum.DrawType.HouseEvent && (null == ddTreeNode.InputValue || null == ddTreeNode.InputValue2 || string.Empty == ddTreeNode.InputValue || string.Empty == ddTreeNode.InputValue2))
                {
                    string V1 = "";

                    if (ddTreeNode.InputType == General.FtaProgram.String.ConstantProbability)
                    {
                        V1 = General.FtaProgram.String.InputValue_Constant;
                    }
                    else
                    {
                        V1 = General.FtaProgram.String.InputValue;
                    }

                    string V2 = General.FtaProgram.String.InputValue2;

                    string V = "";
                    if (null == ddTreeNode.InputValue || string.Empty == ddTreeNode.InputValue)
                    {
                        V += V1;
                    }

                    if (null == ddTreeNode.InputValue2 || string.Empty == ddTreeNode.InputValue2)
                    {
                        if (V == "")
                        {
                            V += V2;
                        }
                        else
                        {
                            V += "," + V2;
                        }
                    }
                    IncomNames.Rows.Add(new object[] { ddTreeNode, ddTreeNode.Identifier, "Error", V + General.FtaProgram.String.IntegrityCheckString_HasNoValue });
                }
                else if (!ddTreeNode.IsGateType)
                {
                    if (ddTreeNode.InputType == FixedString.MODEL_LAMBDA_TAU)
                    {
                        try
                        {
                            double dData = 0.0;
                            dData = double.Parse(ddTreeNode.QValue, System.Globalization.NumberStyles.Float);
                            if (dData > 1)
                            {
                                IncomNames.Rows.Add(new object[] { ddTreeNode, ddTreeNode.Identifier, "Warning", General.FtaProgram.String.IntegrityCheckWarning_Unreasonable });
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }

            //已经是末节点
            if (ddTreeNode.HasChild)
            {
                //遍历节点的子节点
                System.Collections.Generic.List<DrawData> ddTreeNodeChildrens = new System.Collections.Generic.List<DrawData>(ddTreeNode.Children);
                foreach (DrawData ddTreeNodeChildren in ddTreeNodeChildrens)
                {
                    Tree_Integrity_Check_Before_Calculate(ddTreeNodeChildren, ifCheckValue);
                }
            }
            else
            {
                //判断其类型(非事件)
                if (ddTreeNode.IsGateType && ddTreeNode.Type != Model.Enum.DrawType.TransferInGate)
                {
                    IncomNames.Rows.Add(new object[] { ddTreeNode, ddTreeNode.Identifier, "Error", General.FtaProgram.String.IntegrityCheckString_HasNoChild });
                }
            }
        }
    }
}
