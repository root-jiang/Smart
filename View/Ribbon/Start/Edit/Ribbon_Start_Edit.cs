using DevExpress.XtraBars.Docking;
using DevExpress.XtraTreeList.Nodes;
using FaultTreeAnalysis.Common;
using FaultTreeAnalysis.FTAControlEventHandle.Ribbon.Start.Edit;
using FaultTreeAnalysis.Model.Data;
using FaultTreeAnalysis.View.Ribbon.Start.Edit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace FaultTreeAnalysis
{
    partial class FTAControl
    {
        /// <summary>
        /// 初始化Ribbon-Start-Edit的菜单
        /// </summary>
        private void Init_Ribbon_Start_Edit()
        {
            //撤销
            barButtonItem_Cancel.ItemClick += ItemClick_BarButtonItem_Ribbon_Start_Edit;
            barButtonItem_ToolUndo.ItemClick += ItemClick_BarButtonItem_Ribbon_Start_Edit;

            //重做
            barButtonItem_Redo.ItemClick += ItemClick_BarButtonItem_Ribbon_Start_Edit;
            barButtonItem_ToolRedo.ItemClick += ItemClick_BarButtonItem_Ribbon_Start_Edit;

            //查找和替换
            barButtonItem_FindAndReplace.ItemClick += ItemClick_BarButtonItem_Ribbon_Start_Edit;

            //自定义查找和替换
            barButtonItem_FindAndReplaceCustom.ItemClick += ItemClick_BarButtonItem_Ribbon_Start_Edit;
        }

        /// <summary>
        /// 撤销,重做,查找和替换按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemClick_BarButtonItem_Ribbon_Start_Edit(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            General.TryCatch(() =>
            {
                //撤销
                if (e.Item == barButtonItem_Cancel || e.Item == barButtonItem_ToolUndo)
                {
                    //TODO:撤销时图形无法保存数据对象
                    //currentDiagram.UndoManager.Undo();
                    //MsgBox.Show("To be completed", "Information");
                    CurrentSystem_Undo_Event();
                }
                //重做
                else if (e.Item == barButtonItem_Redo || e.Item == barButtonItem_ToolRedo)
                {
                    //diagramControl_FTADiagram.UndoManager.Redo();
                    //var selectedDrawData = this.treeList_FTATable.FocusedNode?.GetValue(FixedString.COLUMNAME_DATA) as DrawData;
                    //this.FTATable_Load(selectedDrawData);
                    //MsgBox.Show("To be completed", "Information");
                    CurrentSystem_Redo_Event();
                }
                //查找和替换
                else if (e.Item == barButtonItem_FindAndReplace)
                {
                    if (General.PanelContainer.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible)
                    {
                        var searchView = new SearchView(General.FtaProgram, this.treeList_FTATable.VisibleColumns.Select(o => o.Caption).ToArray());
                        searchView.ShowDialog();
                        if (searchView.Result == null) this.treeList_FTATable.OptionsFind.FindFilterColumns = string.Join(FixedString.SEMICOLON, this.treeList_FTATable.VisibleColumns.Select(o => o.FieldName).ToArray());
                        else
                        {
                            var fieldNames = new List<string>(searchView.Result.Length);
                            foreach (var item in searchView.Result) fieldNames.Add(this.treeList_FTATable.Columns.FirstOrDefault(o => o.Caption == item.Trim()).FieldName);
                            this.treeList_FTATable.OptionsFind.FindFilterColumns = string.Join(FixedString.SEMICOLON, fieldNames.ToArray());
                            this.treeList_FTATable.ShowFindPanel();
                        }
                    }
                }
                //查找和替换
                else if (e.Item == barButtonItem_FindAndReplaceCustom)
                {
                    if (General.PanelContainer.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible)
                    {
                        bool checkexist = false;
                        foreach (DockPanel dp in General.dockManager_FTA.Panels)
                        {
                            if (dp.Name == "DP_FindAndReplace")
                            {
                                dp.Visibility = DockVisibility.Visible;

                                dp.FloatSize = new System.Drawing.Size(622, 300);
                                int xWidth = SystemInformation.PrimaryMonitorSize.Width;//获取显示器屏幕宽度
                                int yHeight = SystemInformation.PrimaryMonitorSize.Height;//高度
                                dp.FloatLocation = new System.Drawing.Point((xWidth - dp.Width) / 2, (yHeight - dp.Height) / 2);

                                General.dockManager_FTA.ActivePanel = dp;
                                checkexist = true;
                            }
                        }

                        if (checkexist == false)
                        {
                            FindAndReplace SENew = new FindAndReplace();
                            DockPanel DP = General.dockManager_FTA.AddPanel(DockingStyle.Float);
                            DP.Name = "DP_FindAndReplace";
                            DP.Dock = DockingStyle.Float;
                            DP.Options.AllowDockBottom = false;
                            DP.Options.AllowDockFill = false;
                            DP.Options.AllowDockLeft = false;
                            DP.Options.AllowDockRight = false;
                            DP.Options.AllowDockTop = false;
                            DP.Options.AllowDockAsTabbedDocument = false;
                            DP.Text = General.FtaProgram.String.FindAndReplaceCustom;
                            SENew.Dock = DockStyle.Fill;
                            SENew.TopLevel = false;
                            SENew.FormBorderStyle = FormBorderStyle.None;
                            DP.Controls.Add(SENew);
                            General.dockManager_FTA.ActivePanel = DP;
                            General.FindAndReplace = SENew;

                            DP.FloatSize = new System.Drawing.Size(622, 300);
                            int xWidth = SystemInformation.PrimaryMonitorSize.Width;//获取显示器屏幕宽度
                            int yHeight = SystemInformation.PrimaryMonitorSize.Height;//高度
                            DP.FloatLocation = new System.Drawing.Point((xWidth - DP.Width) / 2, (yHeight - DP.Height) / 2);
                        }

                        FindAndReplace SE = new FindAndReplace();
                        if (General.FindAndReplace != null)
                        {
                            SE = General.FindAndReplace;
                        }
                        else
                        {
                            return;
                        }

                        SE.RefreshText();

                        SE.checkedComboBoxEdit1.Properties.Items.Clear();
                        SE.checkedComboBoxEdit2.Properties.Items.Clear();

                        if (General.FtaProgram.Setting.Language == FixedString.LANGUAGE_CN_CN)
                        {
                            SE.checkedComboBoxEdit1.Properties.Items.AddRange(new string[] { "编号", "描述", "父节点", "逻辑条件" });
                            SE.checkedComboBoxEdit2.Properties.Items.AddRange(new string[] { "描述" });
                            SE.checkedComboBoxEdit1.EditValue = "编号,描述";
                            SE.checkedComboBoxEdit2.EditValue = "描述";
                        }
                        else
                        {
                            SE.checkedComboBoxEdit1.Properties.Items.AddRange(new string[] { "Identifier", "Description", "ParentID", "LogicalCondition" });
                            SE.checkedComboBoxEdit2.Properties.Items.AddRange(new string[] { "Description" });
                            SE.checkedComboBoxEdit1.EditValue = "Identifier,Description";
                            SE.checkedComboBoxEdit2.EditValue = "Description";
                        }

                        SE.Show();
                    }
                }
            });
        }


        /// <summary>
        /// 撤销按钮
        /// </summary>
        private void CurrentSystem_Undo_Event()
        {
            //事件
            Model.Data.SystemModel modelCurrentSystem = General.FtaProgram?.CurrentSystem;
            if (null != modelCurrentSystem)
            {
                modelCurrentSystem.RetrospectCompleted -= CurrentSystem_RetrospectCompleted;
                modelCurrentSystem.RetrospectCompleted += CurrentSystem_RetrospectCompleted;

                //push行为
                if (true == (bool)(modelCurrentSystem.Undo()))
                {
                    //
                }
            }
        }

        /// <summary>
        /// 重做按钮
        /// </summary>
        private void CurrentSystem_Redo_Event()
        {
            //事件
            Model.Data.SystemModel modelCurrentSystem = General.FtaProgram?.CurrentSystem;
            if (null != modelCurrentSystem)
            {
                modelCurrentSystem.RetrospectCompleted -= CurrentSystem_RetrospectCompleted;
                modelCurrentSystem.RetrospectCompleted += CurrentSystem_RetrospectCompleted;

                //pop行为
                if (true == (bool)(modelCurrentSystem.Redo()))
                {
                    //
                }
            }
        }

        private void CurrentSystem_RetrospectCompleted(object sender, Behavior.Event.RetrospectiveArgs e)
        {
            //记录当前节点 
            DrawData SeData = new DrawData();
            object obj = General.TableControl.FocusedNode.GetValue(FixedString.COLUMNAME_DATA);
            if (obj != null)
            {
                SeData = (DrawData)obj;
            }
            else
            {
                SeData = null;
            }

            Model.Data.SystemModel modelCurrentSystem = sender as Model.Data.SystemModel;
            if ((null == modelCurrentSystem)
                || (null == e))
            {
                return;
            }
            //资源刷新
            // modelCurrentSystem.SetCurrentSystem(system);
            modelCurrentSystem.UpdateRepeatedAndTranfer();

            VirtualDrawData vData = this.treeList_FTATable.DataSource as VirtualDrawData;
            if ((null != vData)
                && (null != modelCurrentSystem.Roots))
            {
                vData.data?.Children?.Clear();
                vData.data?.Children?.AddRange(modelCurrentSystem.Roots);
            }
            //
            this.treeList_FTATable.RefreshDataSource();
            this.treeList_FTATable.FireChanged();
            //聚焦
            DrawData oFocused = null;
            if (Behavior.Enum.RetrospectiveInstruction.Redo == e.Instruction)
            {
                if (Behavior.Enum.ElementOperate.AlterProperty == (e.Cause & Behavior.Enum.ElementOperate.AlterProperty))
                {
                    oFocused = modelCurrentSystem.FindDrawDataBy((o) => { return o.ThisGuid == (Guid)e.PresentObject?.ThisGuid; });
                }

                if (Behavior.Enum.ElementOperate.Creation == (e.Cause & Behavior.Enum.ElementOperate.Creation))
                {
                    oFocused = modelCurrentSystem.FindDrawDataBy((o) => { return o.ThisGuid == (Guid)e.PresentObject?.ThisGuid; });
                }
            }
            if (Behavior.Enum.RetrospectiveInstruction.Undo == e.Instruction)
            {
                if (Behavior.Enum.ElementOperate.AlterProperty == (e.Effect & Behavior.Enum.ElementOperate.AlterProperty))
                {
                    oFocused = modelCurrentSystem.FindDrawDataBy((o) => { return o.ThisGuid == (Guid)e.PresentObject?.ThisGuid; });
                }

                if (Behavior.Enum.ElementOperate.Deletion == (e.Effect & Behavior.Enum.ElementOperate.Deletion))
                {
                    //根
                    if (null == e.DependencyObject?.ThisGuid)
                    {
                        oFocused = vData.data?.Children?.First();
                    }
                    else
                    {
                        oFocused = modelCurrentSystem.FindDrawDataBy((o) => { return o.ThisGuid == (Guid)e.DependencyObject?.ThisGuid; });
                    }
                }
            }

            if (Behavior.Enum.ElementOperate.Remove == (e.Cause & Behavior.Enum.ElementOperate.Remove))
            {
                //重刷（临时，后期改进布局算法） 
                SaveDataFaultTree(General.FtaProgram.CurrentProject, General.FtaProgram.CurrentSystem);
                string filename = General.FtaProgram.CurrentProject.ProjectPath + "\\" + modelCurrentSystem.SystemName + FixedString.APP_EXTENSION;
                LoadData(false, filename, false);

                //定位到打开的故障树（打开故障树时会同时加载该故障树所在工程，以及工程下所有故障树）
                foreach (TreeListNode PNode in treeList_Project.Nodes)
                {
                    foreach (TreeListNode GNode in PNode.Nodes)
                    {
                        //如果是工程节点下面的故障树定位
                        if (GNode.Tag != null && ((ProjectModel)PNode.Tag).ProjectPath + "\\" + ((SystemModel)GNode.Tag).SystemName + FixedString.APP_EXTENSION == filename)
                        {
                            treeList_Project.FocusedNode = GNode;
                            break;
                        }
                        foreach (TreeListNode SNode in GNode.Nodes)
                        {
                            //如果是分组下面的故障树定位
                            if (((ProjectModel)PNode.Tag).ProjectPath + "\\" + ((SystemModel)SNode.Tag).SystemName + FixedString.APP_EXTENSION == filename)
                            {
                                treeList_Project.FocusedNode = SNode;
                            }
                        }
                    }
                }

                //定位图形节点
                if (SeData != null)
                {
                    General.TableControl.ExpandAll();
                    General.TableControl.FocusedNode = null;

                    Predicate<TreeListNode> match = new Predicate<TreeListNode>(d => (d.GetValue(FixedString.COLUMNAME_DATA) != null && ((DrawData)d.GetValue(FixedString.COLUMNAME_DATA)).Identifier == SeData.Identifier));
                    TreeListNode nd = General.FTATree.FindNode(match);

                    if (nd == null)
                    {
                        match = new Predicate<TreeListNode>(d => (d.GetValue(FixedString.COLUMNAME_DATA) != null && ((DrawData)d.GetValue(FixedString.COLUMNAME_DATA)).Identifier == SeData.Parent.Identifier));
                        nd = General.FTATree.FindNode(match);
                    }
                    General.TableControl.FocusedNode = nd;
                }
            }
            else
            {
                //切换视图必须加，否则会有多余图形 
                this.ftaDiagram.ResetData();
                General.InvokeHandler(Model.Enum.GlobalEvent.UpdateLayout);
                General.InvokeHandler(Model.Enum.GlobalEvent.FTADiagram_MakeVisable, oFocused);
                General.InvokeHandler(Model.Enum.GlobalEvent.TableFocused, oFocused);
                this.ftaDiagram.Refresh(true);
            }
        }
    }
}
