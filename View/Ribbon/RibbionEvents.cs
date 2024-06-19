using DevExpress.Office.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraTreeList.Nodes;
using FaultTreeAnalysis.Common;
using FaultTreeAnalysis.Model.Data;
using FaultTreeAnalysis.Properties;
using IntegratedSystem.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaultTreeAnalysis.View.Ribbon
{
    public class RibbionEvents
    {
        public Task task = null;
        public Task taskNew = null;
        private ProgramModel programModel;
        public AnalysisNew analysisNew = new AnalysisNew();
        public DataTable IncomNames = new DataTable();
        public string CheckErr = "";

        private BarCheckItem GetCheckItem(string controlName)
        {
            return General.BarCheckItems[controlName];
        }

        public RibbionEvents(ProgramModel programModel)
        {
            this.programModel = programModel;
            analysisNew = new AnalysisNew();
        }

        /// <summary>
        /// 故障树计算按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void BarButtonItem_FTACalculateOnlyCheck_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if ((null == this.programModel) ||
              (null == this.programModel.CurrentSystem))
            {
                return;
            }

            //故障树完整性检测 
            Tree_Integrity_Check_Before_Calculate();

            if (IncomNames.Rows.Count > 0)
            {
                bool checkexist = false;
                foreach (DockPanel dp in General.dockManager_FTA.Panels)
                {
                    if (dp.Name == "DP_SelectCheckErr")
                    {
                        dp.Visibility = DockVisibility.Visible;

                        dp.FloatSize = new System.Drawing.Size(670, 448);
                        int xWidth = SystemInformation.PrimaryMonitorSize.Width;//获取显示器屏幕宽度
                        int yHeight = SystemInformation.PrimaryMonitorSize.Height;//高度 
                        dp.FloatLocation = new System.Drawing.Point(20, yHeight - dp.Height - 70);

                        General.dockManager_FTA.ActivePanel = dp;
                        checkexist = true;
                    }
                }

                if (checkexist == false)
                {
                    View.Ribbon.Start.Excel.SelectCheckErr SENew = new View.Ribbon.Start.Excel.SelectCheckErr();
                    DockPanel DP = General.dockManager_FTA.AddPanel(DockingStyle.Float);
                    DP.Name = "DP_SelectCheckErr";
                    DP.Dock = DockingStyle.Float;
                    DP.Options.AllowDockBottom = false;
                    DP.Options.AllowDockFill = false;
                    DP.Options.AllowDockLeft = false;
                    DP.Options.AllowDockRight = false;
                    DP.Options.AllowDockTop = false;
                    DP.Options.AllowDockAsTabbedDocument = false;
                    DP.Text = General.FtaProgram.String.Check;
                    SENew.Dock = DockStyle.Fill;
                    General.SelectCheckErr = SENew;
                    SENew.REs = this;
                    SENew.DP = DP;
                    SENew.TopLevel = false;
                    SENew.treeList_Project.BringToFront();
                    DP.Controls.Add(SENew);
                    General.dockManager_FTA.ActivePanel = DP;

                    DP.FloatSize = new System.Drawing.Size(670, 448);
                    int xWidth = SystemInformation.PrimaryMonitorSize.Width;//获取显示器屏幕宽度
                    int yHeight = SystemInformation.PrimaryMonitorSize.Height;//高度
                    DP.FloatLocation = new System.Drawing.Point(20, yHeight - DP.Height - 70);
                }

                View.Ribbon.Start.Excel.SelectCheckErr SE = new View.Ribbon.Start.Excel.SelectCheckErr();
                if (General.SelectCheckErr != null)
                {
                    SE = General.SelectCheckErr;
                }
                else
                {
                    return;
                }

                SE.RefreshText();
                SE.treeList_Project.Nodes.Clear();

                foreach (DataRow it in IncomNames.Rows)
                {
                    if (it["info"].ToString() == General.FtaProgram.String.IntegrityCheckString_HasNoRoot)
                    {
                        MsgBox.Show(General.FtaProgram.String.IntegrityCheckString_HasNoRoot);
                        return;
                    }

                    if (it["type"].ToString() == "Error")
                    {
                        TreeListNode node_Project = SE.treeList_Project.Nodes.Add(new object[] { it["name"].ToString(), Resources.error_16x16, it["info"].ToString() });
                        node_Project.Tag = (DrawData)it["Data"];
                    }
                    else
                    {
                        TreeListNode node_Project = SE.treeList_Project.Nodes.Add(new object[] { it["name"].ToString(), Resources.warning_16x16, it["info"].ToString() });
                        node_Project.Tag = (DrawData)it["Data"];
                    }
                }

                if (SE.treeList_Project.Nodes.Count == 0)
                {
                    return;
                }

                SE.treeList_Project.ExpandAll();
                SE.Show();
            }
            else
            {
                MsgBox.Show(General.FtaProgram.String.IntegrityCheckString_CalculateSuccess);
                return;
            }
        }

        /// <summary>
        /// 故障树计算按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void BarButtonItem_FTACalculate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if ((null == this.programModel) || (null == this.programModel.CurrentSystem))
            {
                return;
            }

            //故障树完整性检测(1.末节点为事件型节点; 2.该节点FaultRate有值) 
            Tree_Integrity_Check_Before_Calculate();
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
                                return;
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
                        }

                        MsgBox.Show(str);
                        return;
                    }
                }
            }

            try
            {
                General.TableControl.ExpandAll();

                //故障计算线程
                CheckErr = "";
                task = new Task(this.ThreadStartCalculate);
                task.Start();
                General.isTaskRun = true;
                task.Wait();
                General.isTaskRun = false;
                General.DiagramControl.Refresh();

                if (CheckErr != "")
                {
                    MsgBox.Show(General.FtaProgram.String.IntegrityCheckString_CalculateFail + CheckErr);
                    return;
                }

                //检查当前故障树是否被链接门对象链接
                try
                {
                    foreach (ProjectModel pm in General.FtaProgram.Projects)
                    {
                        foreach (SystemModel sys in pm.Systems)
                        {
                            List<DrawData> das = sys.GetAllDatas();
                            IEnumerable<DrawData> drawDatas = das.Where(d => d.LinkPath == General.FtaProgram.CurrentProject.ProjectName + ":" + General.FtaProgram.CurrentSystem.SystemName);
                            foreach (DrawData drawData in drawDatas)
                            {
                                drawData.GUID = "";
                                drawData.InputType = FixedString.MODEL_LAMBDA_TAU;
                                drawData.Units = FixedString.UNITS_HOURS;
                                drawData.Comment1 = General.FtaProgram.CurrentSystem.Roots[0].Comment1;

                                drawData.InputValue = General.FtaProgram.CurrentSystem.Roots[0].QValue;
                                drawData.InputValue2 = "1";

                                drawData.LogicalCondition = FixedString.LOGICAL_CONDITION_NORMAL;
                                drawData.QValue = General.FtaProgram.CurrentSystem.Roots[0].QValue;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }

                //计算概率弹出提示，获取当前选中的节点
                if (General.DiagramItemPool.SelectedData.Count > 0)
                {
                    DrawData d = General.DiagramItemPool.SelectedData.FirstOrDefault();
                    if (d != null)
                    {
                        MsgBox.Show(General.FtaProgram.String.IntegrityCheckString_CalculateSuccessSelect + d.QValue);
                        return;
                    }
                }
                MsgBox.Show(General.FtaProgram.String.IntegrityCheckString_CalculateSuccessRoot + this.programModel.CurrentRoot.QValue);
            }
            catch (Exception ex)
            {
                c_WaitFormProgress.CloseSplashScreen();
                MsgBox.Show(ex.Message);
            }
        }


        /// <summary>
        /// 故障树计算按钮事件-计算选中分支
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void BarButtonItem_FTACalculateSelected_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if ((null == this.programModel) ||
              (null == this.programModel.CurrentSystem) || General.ftaDiagram.SelectedData == null)
            {
                return;
            }
            DrawData SelectedData = General.ftaDiagram.SelectedData.FirstOrDefault();

            if (!SelectedData.IsGateType)
            {
                return;
            }

            IncomNames.Columns.Clear();
            IncomNames.Rows.Clear();

            IncomNames.Columns.Add("Data", typeof(DrawData));
            IncomNames.Columns.Add("name");
            IncomNames.Columns.Add("type");
            IncomNames.Columns.Add("info");
            //故障树完整性检测(1.末节点为事件型节点; 2.该节点FaultRate有值) 
            Tree_Integrity_Check_Before_Calculate(SelectedData);
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
                                return;
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
                        }

                        MsgBox.Show(str);
                        return;
                    }
                }
            }

            try
            {
                General.TableControl.ExpandAll();

                //故障计算线程
                CheckErr = "";
                task = new Task(this.ThreadStartCalculateSelect);
                task.Start();
                General.isTaskRun = true;
                task.Wait();
                General.isTaskRun = false;
                General.DiagramControl.Refresh();

                if (CheckErr != "")
                {
                    MsgBox.Show(General.FtaProgram.String.IntegrityCheckString_CalculateFail + CheckErr);
                    return;
                }

                //计算概率弹出提示，获取当前选中的节点
                if (General.DiagramItemPool.SelectedData.Count > 0)
                {
                    DrawData d = General.DiagramItemPool.SelectedData.FirstOrDefault();
                    if (d != null)
                    {
                        MsgBox.Show(General.FtaProgram.String.IntegrityCheckString_CalculateSuccessSelect + d.QValue);
                        return;
                    }
                }
                MsgBox.Show(General.FtaProgram.String.IntegrityCheckString_CalculateSuccessRoot + this.programModel.CurrentRoot.QValue);
            }
            catch (Exception ex)
            {
                c_WaitFormProgress.CloseSplashScreen();
                MsgBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 计算概率线程
        /// </summary>
        void ThreadStartCalculate()
        {
            var errorMessage = string.Empty;
            try
            {
                c_WaitFormProgress.ShowSplashScreen("计算中......", "正在文件初始化（此过程可能持续几分钟，请耐心等待）......");
                if (this.programModel.CurrentSystem == null || this.programModel.CurrentRoot == null) return;

                //（不能包含中文路径）
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

                //删除计算目录下所有的临时文件
                String szTempFilePath = exePath + "\\Temp\\CalData";
                if (Directory.Exists(szTempFilePath))
                {
                    DirectoryInfo di = new DirectoryInfo(szTempFilePath);
                    di.Delete(true);
                }

                Analysis analysis = new Analysis();
                String myOPENPSAFilePath = exePath + "\\Temp\\OPENPSA.xml";
                //生成标准格式的OPENPSA文件
                analysis.GenerateDefinedID(this.programModel.CurrentSystem, this.programModel.CurrentRoot);

                analysis.GenerateOPENPSAFile(this.programModel.CurrentSystem, this.programModel.CurrentRoot, myOPENPSAFilePath);

                //生成脚本文件
                string szScriptPath = exePath + "\\Temp\\FNScript.xml";
                analysis.GenerateScriptFile(szScriptPath);

                //调用函数生成割集和概率
                CheckErr = analysis.DFSCalculateProbs(this.programModel.CurrentSystem, this.programModel.CurrentRoot, myOPENPSAFilePath, szScriptPath, exePath);
            }
            catch (Exception ex)
            {
                c_WaitFormProgress.CloseSplashScreen();
                if (ex.HResult != -2146233079 && ex.HResult != -2146232798)
                {
                    CheckErr = ex.Message;
                }
            }
        }

        /// <summary>
        /// 计算概率线程
        /// </summary>
        void ThreadStartCalculateSelect()
        {
            var errorMessage = string.Empty;
            try
            {
                DrawData SelectedData = General.ftaDiagram.SelectedData.FirstOrDefault();

                if (!SelectedData.IsGateType)
                {
                    return;
                }
                c_WaitFormProgress.ShowSplashScreen("计算中......", "正在文件初始化（此过程可能持续几分钟，请耐心等待）......");
                if (this.programModel.CurrentSystem == null || this.programModel.CurrentRoot == null) return;

                //（不能包含中文路径）
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

                //删除计算目录下所有的临时文件
                String szTempFilePath = exePath + "\\Temp\\CalData";
                if (Directory.Exists(szTempFilePath))
                {
                    DirectoryInfo di = new DirectoryInfo(szTempFilePath);
                    di.Delete(true);
                }

                Analysis analysis = new Analysis();
                String myOPENPSAFilePath = exePath + "\\Temp\\OPENPSA.xml";
                //生成标准格式的OPENPSA文件
                analysis.GenerateDefinedID(this.programModel.CurrentSystem, SelectedData);

                analysis.GenerateOPENPSAFile(this.programModel.CurrentSystem, SelectedData, myOPENPSAFilePath);

                //生成脚本文件
                string szScriptPath = exePath + "\\Temp\\FNScript.xml";
                analysis.GenerateScriptFile(szScriptPath);

                //调用函数生成割集和概率
                CheckErr = analysis.DFSCalculateProbs(this.programModel.CurrentSystem, SelectedData, myOPENPSAFilePath, szScriptPath, exePath);
            }
            catch (Exception ex)
            {
                c_WaitFormProgress.CloseSplashScreen();
                if (ex.HResult != -2146233079 && ex.HResult != -2146232798)
                {
                    CheckErr = ex.Message;
                }
            }
        }

        /// <summary>
        /// 故障树完整性检测(1.末节点为事件型节点; 2.该节点FaultRate有值)
        /// </summary>
        /// <returns>true:完整树, false:不完整树</returns>
        public void Tree_Integrity_Check_Before_Calculate()
        {
            IncomNames.Columns.Clear();
            IncomNames.Rows.Clear();

            IncomNames.Columns.Add("Data", typeof(DrawData));
            IncomNames.Columns.Add("name");
            IncomNames.Columns.Add("type");
            IncomNames.Columns.Add("info");

            //项目安全性检测
            if (this.programModel.CurrentSystem.Roots.Count == 0)
            {
                IncomNames.Rows.Add(new object[] { new DrawData(), "", "Error", General.FtaProgram.String.IntegrityCheckString_HasNoRoot });
            }

            foreach (DrawData data in this.programModel.CurrentSystem.Roots)
            {
                if (data.IsGateType && !data.HasChild)
                {
                    if (data.Type != Model.Enum.DrawType.TransferInGate)
                    {
                        IncomNames.Rows.Add(new object[] { data, data.Identifier, "Error", General.FtaProgram.String.IntegrityCheckString_HasNoChild });
                    }
                }
                Tree_Integrity_Check_Before_Calculate(data);
            }

            //检测结果

            IncomNames.DefaultView.Sort = "type";
            IncomNames = IncomNames.DefaultView.ToTable();
        }

        private void Tree_Integrity_Check_Before_Calculate(DrawData ddTreeNode)
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

            if (ddTreeNode.Type == Model.Enum.DrawType.VotingGate)
            {
                int dData = 0;
                try
                {
                    dData = Int32.Parse(ddTreeNode.ExtraValue1, System.Globalization.NumberStyles.Integer);
                }
                catch (Exception)
                {
                    dData = 0;
                }

                if (dData == 0)
                {
                    IncomNames.Rows.Add(new object[] { ddTreeNode, ddTreeNode.Identifier, "Error", General.FtaProgram.String.IntegrityCheckWarning_UnreasonableVote });
                }

                if (dData > ddTreeNode.Children.Count)
                {
                    IncomNames.Rows.Add(new object[] { ddTreeNode, ddTreeNode.Identifier, "Error", General.FtaProgram.String.IntegrityCheckWarning_UnreasonableVote });
                }
            }

            //数据检测(非空)
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

            //已经是末节点
            if (ddTreeNode.HasChild)
            {
                //遍历节点的子节点
                System.Collections.Generic.List<DrawData> ddTreeNodeChildrens = new System.Collections.Generic.List<DrawData>(ddTreeNode.Children);
                foreach (DrawData ddTreeNodeChildren in ddTreeNodeChildrens)
                {
                    Tree_Integrity_Check_Before_Calculate(ddTreeNodeChildren);
                }

                bool NoHouse = false;
                foreach (DrawData ddTreeNodeChildren in ddTreeNodeChildrens)
                {
                    if (ddTreeNodeChildren.Type != Model.Enum.DrawType.HouseEvent)
                    {
                        NoHouse = true;
                    }
                }
                if (NoHouse == false)
                {
                    IncomNames.Rows.Add(new object[] { ddTreeNode, ddTreeNode.Identifier, "Error", General.FtaProgram.String.IntegrityCheckString_HasNoChild });
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
