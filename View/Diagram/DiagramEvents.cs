using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Diagram.Core;
using DevExpress.Xpo.DB;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraDiagram;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraTreeList.Nodes;
using FaultTreeAnalysis.Common;
using FaultTreeAnalysis.FTAControlEventHandle.FTADiagram;
using FaultTreeAnalysis.Model.Data;
using FaultTreeAnalysis.Model.Enum;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace FaultTreeAnalysis.View.Diagram
{
    public class DiagramEvents
    {
        /// <summary>
        /// 预定义鼠标图形大小
        /// </summary>
        private readonly Size cursorSize = new Size(100, 100);

        private DiagramControl diagramControl;

        private ProgramModel programModel;

        /// <summary>
        /// 实现动态刷新的工具
        /// </summary>
        public DiagramItemPool DiagramItemPool { get; set; }

        /// <summary>
        /// 上一次自定义的鼠标对象
        /// </summary>
        private Cursor cursorLast = null;

        /// <summary>
        /// 当前选中的图形类型,决定了在画布上绘制什么
        /// </summary>
        public DrawType AddingType { get; set; } = DrawType.NULL;

        /// <summary>
        /// FTA图和表插入的重复事件的数据对象
        /// </summary>
        public DrawData RepeatedEvent { get; set; }

        /// <summary>
        /// 可以设置图形鼠标的参数对象
        /// </summary>
        private DiagramCustomCursorEventArgs diagramCustomCursorEventArgs;

        /// <summary>
        /// 是否允许鼠标在画布按下时创建新的节点
        /// </summary>
        public bool IsCreateNew { get; set; } = false;

        /// <summary>
        /// 是否处于添加节点状态
        /// </summary>
        public bool IsInsertingNode { get; set; } = false;

        /// <summary>
        /// 当前图的缩放比例
        /// </summary>
        private float scalingRate = 1f;

        public EventHandler<DiagramCustomCursorEventArgs> CustomCursor;
        public EventHandler<CustomDrawItemEventArgs> CustomDrawItem;
        public EventHandler<DiagramZoomFactorChangedEventArgs> ZoomFactorChanged;
        public MouseEventHandler MouseUp;
        public MouseEventHandler MouseMove;
        public MouseEventHandler MouseDown;

        public DiagramEvents(DiagramControl diagramControl, ProgramModel programModel)
        {
            this.diagramControl = diagramControl;
            this.programModel = programModel;
            this.DiagramItemPool = new DiagramItemPool(this.diagramControl, programModel.Setting);
        }

        public void RegisterDiagramEvent()
        {
            this.CustomCursor = this.DiagramCustomCursor;
            this.diagramControl.CustomCursor += DiagramCustomCursor;

            this.MouseDown = this.DiagramMouseDown;
            this.diagramControl.MouseDown += DiagramMouseDown;

            this.MouseMove = this.DiagramMouseMove;
            this.diagramControl.MouseMove += DiagramMouseMove;

            this.MouseUp = this.DiagramMouseUp;
            this.diagramControl.MouseUp += DiagramMouseUp;

            this.CustomDrawItem = this.DiagramCustomDrawItem;
            this.diagramControl.CustomDrawItem += DiagramCustomDrawItem;

            this.ZoomFactorChanged = this.DiagramZoomFactorChanged;
            this.diagramControl.ZoomFactorChanged += DiagramZoomFactorChanged;

            this.diagramControl.Commands.RegisterHotKeys(o =>
            {
                o.ClearHotKeys(DiagramCommandsBase.UndoCommand);
            });

            this.diagramControl.KeyDown += KeyDown;

            this.diagramControl.MouseDoubleClick += DiagramControl_MouseDoubleClick;
            this.diagramControl.SelectionChanged += currentDiagram_SelectionChanged;
        }

        /// <summary>
        /// 切换选中图形对象时改变右键菜单按钮和主菜单按钮停用启用状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void currentDiagram_SelectionChanged(object sender, DiagramSelectionChangedEventArgs e)
        {
            if (this.diagramControl.SelectedItems.Count == 1)
            {
                if (this.diagramControl.SelectedItems[0].Tag == null)
                {
                    return;
                }

                DrawData SeData = (DrawData)this.diagramControl.SelectedItems[0].Tag;

                General.barButtonItem_ExportToModel.Enabled = true;
                General.barButtonItem_LoadFromModel.Enabled = true;

                if (SeData.IsGateType == false || SeData.Type == DrawType.TransferInGate)
                {
                    General.barButtonItem_LoadFromModel.Enabled = false;
                }

                General.barButtonItem_BasicLib.Enabled = true;
                if (SeData.IsGateType)
                {
                    if (SeData.Type == DrawType.TransferInGate)
                    {
                        General.barButtonItem_InsertFromBasicEventLibrary.Enabled = false;
                    }
                    else
                    {
                        General.barButtonItem_InsertFromBasicEventLibrary.Enabled = true;
                    }
                    General.barButtonItem_AddToBasicEventLibrary.Enabled = false;
                    General.barButtonItem_SynchronizeFromBasicEventLibrary.Enabled = false;
                }
                else
                {
                    General.barButtonItem_AddToBasicEventLibrary.Enabled = true;
                    General.barButtonItem_SynchronizeFromBasicEventLibrary.Enabled = true;
                    General.barButtonItem_InsertFromBasicEventLibrary.Enabled = false;
                }

                if (SeData.IsGateType == false && SeData.Type != DrawType.TransferInGate)
                {
                    General.barButtonItem_AddToBasicEventLibrary.Enabled = true;
                    General.barButtonItem_SynchronizeFromBasicEventLibrary.Enabled = true;
                }
                if (SeData.IsGateType && SeData.Type != DrawType.TransferInGate)
                {
                    General.barButtonItem_InsertFromBasicEventLibrary.Enabled = true;
                }

                General.barButtonItem_ShapeEdit.Enabled = true;
                General.barButtonItem_Cut.Enabled = true;
                General.barButtonItem_Copy.Enabled = true;
                General.barButtonItem_MenuCut.Enabled = true;
                General.barButtonItem_MenuCopy.Enabled = true;
                General.barButtonItem_MenuCopyCurrentView.Enabled = true;
                General.barButtonItem_MenuPaste.Enabled = true;
                General.barButtonItem_MenuPasteRepeated.Enabled = true;
                General.barButtonItem_DeleteTransferRibbon.Enabled = true;

                if (SeData.Type == DrawType.TransferInGate)
                {
                    //转移门不可复制剪切
                    General.barButtonItem_Cut.Enabled = false;
                    General.barButtonItem_Copy.Enabled = false;
                    General.barButtonItem_MenuCopy.Enabled = false;
                    General.barButtonItem_MenuCut.Enabled = false;
                }

                //当前是顶层节点      
                General.barButtonItem_DeleteNodesTable.Caption = General.FtaProgram.String.DeleteNodes;
                General.barButtonItem_DeleteNodesDiagram.Caption = General.FtaProgram.String.DeleteNodes;
                General.barButtonItem_DeleteNodesRibbon.Caption = General.FtaProgram.String.DeleteNodes;

                if (SeData.ThisGuid == General.FtaProgram.CurrentRoot.ThisGuid)
                {
                    General.barButtonItem_DeleteTopTable.Visibility = BarItemVisibility.Always;
                    General.barButtonItem_DeleteTopDiagram.Visibility = BarItemVisibility.Always;

                    General.barButtonItem_DeleteLevelTable.Visibility = BarItemVisibility.Never;
                    General.barButtonItem_DeleteLevelDiagram.Visibility = BarItemVisibility.Never;

                    General.barButtonItem_DeleteNodeTable.Visibility = BarItemVisibility.Never;
                    General.barButtonItem_DeleteNodeDiagram.Visibility = BarItemVisibility.Never;

                    General.barButtonItem_DeleteNodesTable.Visibility = BarItemVisibility.Never;
                    General.barButtonItem_DeleteNodesDiagram.Visibility = BarItemVisibility.Never;

                    General.barButtonItem_DeleteTransferTable.Visibility = BarItemVisibility.Never;
                    General.barButtonItem_DeleteTransferDiagram.Visibility = BarItemVisibility.Never;

                    General.barButtonItem_DeleteTopRibbon.Visibility = BarItemVisibility.Always;
                    General.barButtonItem_DeleteLevelRibbon.Visibility = BarItemVisibility.Never;
                    General.barButtonItem_DeleteNodeRibbon.Visibility = BarItemVisibility.Never;
                    General.barButtonItem_DeleteNodesRibbon.Visibility = BarItemVisibility.Never;
                    General.barButtonItem_DeleteTransferRibbon.Visibility = BarItemVisibility.Never;
                }
                //当前节点只有一个子节点（删除级别可见，删除子节点和删除子节点包含转移门这两种根据是否有转移门判断是否可见）
                else if (SeData?.Parent != null && SeData.Children.Count == 1)
                {
                    General.barButtonItem_DeleteTopTable.Visibility = BarItemVisibility.Never;
                    General.barButtonItem_DeleteTopDiagram.Visibility = BarItemVisibility.Never;

                    General.barButtonItem_DeleteLevelTable.Visibility = BarItemVisibility.Always;
                    General.barButtonItem_DeleteLevelDiagram.Visibility = BarItemVisibility.Always;

                    General.barButtonItem_DeleteNodeTable.Visibility = BarItemVisibility.Never;
                    General.barButtonItem_DeleteNodeDiagram.Visibility = BarItemVisibility.Never;

                    //判断是否有转移门
                    List<DrawData> Alldatas = SeData.GetAllData(General.FtaProgram.CurrentSystem, true);
                    bool ck = false;
                    foreach (DrawData da in Alldatas)
                    {
                        if (da.Type == DrawType.TransferInGate)
                        {
                            ck = true;
                        }
                    }

                    if (ck)//有转移门
                    {
                        General.barButtonItem_DeleteNodesTable.Visibility = BarItemVisibility.Always;
                        General.barButtonItem_DeleteNodesDiagram.Visibility = BarItemVisibility.Always;

                        General.barButtonItem_DeleteTransferTable.Visibility = BarItemVisibility.Never;
                        General.barButtonItem_DeleteTransferDiagram.Visibility = BarItemVisibility.Never;

                        General.barButtonItem_DeleteNodesRibbon.Visibility = BarItemVisibility.Always;
                        General.barButtonItem_DeleteTransferRibbon.Visibility = BarItemVisibility.Never;

                        General.barButtonItem_DeleteNodesTable.Caption = General.FtaProgram.String.DeleteNodes_HasTransfer;
                        General.barButtonItem_DeleteNodesDiagram.Caption = General.FtaProgram.String.DeleteNodes_HasTransfer;
                        General.barButtonItem_DeleteNodesRibbon.Caption = General.FtaProgram.String.DeleteNodes_HasTransfer;
                    }
                    else
                    {
                        General.barButtonItem_DeleteNodesTable.Visibility = BarItemVisibility.Always;
                        General.barButtonItem_DeleteNodesDiagram.Visibility = BarItemVisibility.Always;

                        General.barButtonItem_DeleteTransferTable.Visibility = BarItemVisibility.Never;
                        General.barButtonItem_DeleteTransferDiagram.Visibility = BarItemVisibility.Never;

                        General.barButtonItem_DeleteNodesRibbon.Visibility = BarItemVisibility.Always;
                        General.barButtonItem_DeleteTransferRibbon.Visibility = BarItemVisibility.Never;

                        General.barButtonItem_DeleteNodesTable.Caption = General.FtaProgram.String.DeleteNodes;
                        General.barButtonItem_DeleteNodesDiagram.Caption = General.FtaProgram.String.DeleteNodes;
                        General.barButtonItem_DeleteNodesRibbon.Caption = General.FtaProgram.String.DeleteNodes;
                    }

                    General.barButtonItem_DeleteTopRibbon.Visibility = BarItemVisibility.Never;
                    General.barButtonItem_DeleteLevelRibbon.Visibility = BarItemVisibility.Always;
                    General.barButtonItem_DeleteNodeRibbon.Visibility = BarItemVisibility.Never;
                }
                //当前节点没有子节点，只显示删除当前节点选项
                else if (SeData?.Parent != null && SeData.Children.Count == 0)
                {
                    General.barButtonItem_DeleteTopTable.Visibility = BarItemVisibility.Never;
                    General.barButtonItem_DeleteTopDiagram.Visibility = BarItemVisibility.Never;

                    General.barButtonItem_DeleteLevelTable.Visibility = BarItemVisibility.Never;
                    General.barButtonItem_DeleteLevelDiagram.Visibility = BarItemVisibility.Never;

                    General.barButtonItem_DeleteNodeTable.Visibility = BarItemVisibility.Always;
                    General.barButtonItem_DeleteNodeDiagram.Visibility = BarItemVisibility.Always;

                    General.barButtonItem_DeleteNodesTable.Visibility = BarItemVisibility.Never;
                    General.barButtonItem_DeleteNodesDiagram.Visibility = BarItemVisibility.Never;

                    General.barButtonItem_DeleteTopRibbon.Visibility = BarItemVisibility.Never;
                    General.barButtonItem_DeleteLevelRibbon.Visibility = BarItemVisibility.Never;
                    General.barButtonItem_DeleteNodeRibbon.Visibility = BarItemVisibility.Always;
                    General.barButtonItem_DeleteNodesRibbon.Visibility = BarItemVisibility.Never;

                    General.barButtonItem_DeleteTransferTable.Visibility = BarItemVisibility.Never;
                    General.barButtonItem_DeleteTransferDiagram.Visibility = BarItemVisibility.Never;
                    General.barButtonItem_DeleteTransferRibbon.Visibility = BarItemVisibility.Never;
                }
                //当前节点有多个子节点（删除级别不可见，删除子节点和删除子节点包含转移门这两种根据是否有转移门判断是否可见）
                else
                {
                    General.barButtonItem_DeleteTopTable.Visibility = BarItemVisibility.Never;
                    General.barButtonItem_DeleteTopDiagram.Visibility = BarItemVisibility.Never;

                    General.barButtonItem_DeleteLevelTable.Visibility = BarItemVisibility.Never;
                    General.barButtonItem_DeleteLevelDiagram.Visibility = BarItemVisibility.Never;

                    General.barButtonItem_DeleteNodeTable.Visibility = BarItemVisibility.Never;
                    General.barButtonItem_DeleteNodeDiagram.Visibility = BarItemVisibility.Never;

                    General.barButtonItem_DeleteTopRibbon.Visibility = BarItemVisibility.Never;
                    General.barButtonItem_DeleteLevelRibbon.Visibility = BarItemVisibility.Never;
                    General.barButtonItem_DeleteNodeRibbon.Visibility = BarItemVisibility.Never;

                    List<DrawData> Alldatas = SeData.GetAllData(General.FtaProgram.CurrentSystem, true);
                    bool ck = false;
                    foreach (DrawData da in Alldatas)
                    {
                        if (da.Type == DrawType.TransferInGate)
                        {
                            ck = true;
                        }
                    }

                    if (ck)//有转移门
                    {
                        General.barButtonItem_DeleteNodesTable.Visibility = BarItemVisibility.Always;
                        General.barButtonItem_DeleteNodesDiagram.Visibility = BarItemVisibility.Always;

                        General.barButtonItem_DeleteTransferTable.Visibility = BarItemVisibility.Never;
                        General.barButtonItem_DeleteTransferDiagram.Visibility = BarItemVisibility.Never;

                        General.barButtonItem_DeleteNodesRibbon.Visibility = BarItemVisibility.Always;
                        General.barButtonItem_DeleteTransferRibbon.Visibility = BarItemVisibility.Never;

                        General.barButtonItem_DeleteNodesTable.Caption = General.FtaProgram.String.DeleteNodes_HasTransfer;
                        General.barButtonItem_DeleteNodesDiagram.Caption = General.FtaProgram.String.DeleteNodes_HasTransfer;
                        General.barButtonItem_DeleteNodesRibbon.Caption = General.FtaProgram.String.DeleteNodes_HasTransfer;
                    }
                    else
                    {
                        General.barButtonItem_DeleteNodesTable.Visibility = BarItemVisibility.Always;
                        General.barButtonItem_DeleteNodesDiagram.Visibility = BarItemVisibility.Always;

                        General.barButtonItem_DeleteTransferTable.Visibility = BarItemVisibility.Never;
                        General.barButtonItem_DeleteTransferDiagram.Visibility = BarItemVisibility.Never;

                        General.barButtonItem_DeleteNodesRibbon.Visibility = BarItemVisibility.Always;
                        General.barButtonItem_DeleteTransferRibbon.Visibility = BarItemVisibility.Never;

                        General.barButtonItem_DeleteNodesTable.Caption = General.FtaProgram.String.DeleteNodes;
                        General.barButtonItem_DeleteNodesDiagram.Caption = General.FtaProgram.String.DeleteNodes;
                        General.barButtonItem_DeleteNodesRibbon.Caption = General.FtaProgram.String.DeleteNodes;
                    }
                }

                if (SeData.IsGateType && SeData.Type != DrawType.TransferInGate)
                {
                    General.barButtonItem_InsertLevel.Enabled = true;
                    General.barButtonItem_MenuPaste.Enabled = true;
                    General.barButtonItem_MenuPasteRepeated.Enabled = true;
                }
                else
                {
                    General.barButtonItem_InsertLevel.Enabled = false;
                    General.barButtonItem_MenuPaste.Enabled = false;
                    General.barButtonItem_MenuPasteRepeated.Enabled = false;
                }

                General.barButtonItem_MenuBreakIntoTransfer.Enabled = false;
                General.barButtonItem_MenuCollapseTransfer.Enabled = false;
                General.barSubItem_TransferTo.Enabled = false;

                //进入转移门 
                if (SeData.IsGateType == false)
                {
                    return;
                }

                if (SeData != null)
                {
                    //非顶层节点,非已存在转移门
                    if (SeData.Parent?.Children != null &&
                       General.FtaProgram.CurrentSystem?.Roots?.Contains(SeData) == false &&
                       General.FtaProgram.CurrentSystem?.TranferGates?.ContainsKey(SeData.Identifier) == false)
                    {
                        General.barButtonItem_MenuBreakIntoTransfer.Enabled = true;
                    }
                }
                //折叠转移门
                //转到 
                if (SeData != null && General.FtaProgram.CurrentSystem != null && General.FtaProgram.CurrentSystem.TranferGates != null &&
                    General.FtaProgram.CurrentSystem.TranferGates.ContainsKey(SeData.Identifier))
                {
                    HashSet<DrawData> transfer = General.FtaProgram.CurrentSystem.TranferGates[SeData.Identifier];
                    if (transfer != null && transfer.Contains(SeData))
                    {
                        //转移门集合只剩2个
                        if (transfer.Count == 2)
                        {
                            General.barButtonItem_MenuCollapseTransfer.Enabled = true;
                        }

                        General.barSubItem_TransferTo.Enabled = true;
                    }
                }
            }
            else//多选或未作选择
            {
                General.barButtonItem_BasicLib.Enabled = true;
                General.barButtonItem_AddToBasicEventLibrary.Enabled = false;
                General.barButtonItem_SynchronizeFromBasicEventLibrary.Enabled = false;
                General.barButtonItem_InsertFromBasicEventLibrary.Enabled = false;
                if (this.diagramControl.SelectedItems.Count == 0)
                {
                    if (General.FTATree.FocusedNode != null && General.FTATree.FocusedNode.GetValue(FixedString.COLUMNAME_DATA) != null)
                    {
                        var tag = General.FTATree.FocusedNode.GetValue(FixedString.COLUMNAME_DATA);
                        foreach (DiagramItem item in General.DiagramControl.Items)
                        {
                            if (item.Tag != null && item.Tag == tag)
                            {
                                General.DiagramControl.SelectItem(item, ModifySelectionMode.ReplaceSelection);
                            }
                        }
                    }
                }

                if (this.diagramControl.SelectedItems.Count == 0)
                {
                    General.barButtonItem_ShapeEdit.Enabled = false;
                    General.barButtonItem_MenuCut.Enabled = false;
                    General.barButtonItem_MenuCopy.Enabled = false;
                    General.barButtonItem_MenuPaste.Enabled = false;
                    General.barButtonItem_MenuPasteRepeated.Enabled = false;

                    General.barButtonItem_DeleteTopRibbon.Visibility = BarItemVisibility.Never;
                    General.barButtonItem_DeleteLevelRibbon.Visibility = BarItemVisibility.Never;
                    General.barButtonItem_DeleteNodeRibbon.Visibility = BarItemVisibility.Never;
                    General.barButtonItem_DeleteNodesRibbon.Visibility = BarItemVisibility.Never;
                    General.barButtonItem_DeleteTransferRibbon.Visibility = BarItemVisibility.Never;

                    General.barButtonItem_DeleteTransferRibbon.Enabled = false;
                    General.barButtonItem_InsertLevel.Enabled = false;
                    General.barButtonItem_MenuBreakIntoTransfer.Enabled = false;
                    General.barButtonItem_MenuCollapseTransfer.Enabled = false;
                    General.barSubItem_TransferTo.Enabled = false;
                    General.barButtonItem_ExportToModel.Enabled = false;
                    General.barButtonItem_LoadFromModel.Enabled = false;
                }
            }
        }

        /// <summary>
        /// 双击左键弹出属性编辑框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DiagramControl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //双击左键弹出属性编辑框
            if (this.IsInsertingNode == false)
            {
                var item = default(DiagramItem);
                item = General.DiagramControl.CalcHitItem(General.DiagramControl.PointToClient(Control.MousePosition));
                if (item != null)
                {
                    if (item.GetType() == typeof(DiagramShape) && item.Tag is DrawData && ((DrawData)item.Tag).Type != DrawType.NULL)
                    {
                        //设置该元素选中
                        General.DiagramItemPool.SelectedData.Clear();
                        General.DiagramItemPool.SelectedData.Add(item.Tag as DrawData);
                        var drawData = item.Tag as DrawData;

                        bool isTransfer = false;//转移门禁止编辑类型
                        if (drawData.Type == DrawType.TransferInGate)
                        {
                            isTransfer = true;
                            HashSet<DrawData> transfer = General.FtaProgram.CurrentSystem.TranferGates[drawData.Identifier];
                            DrawData trans_True = transfer.Where(obj => obj.Type != DrawType.TransferInGate).FirstOrDefault();
                            General.ftaDiagram.FocusOn(trans_True, true);
                            General.ftaTable.FocusOn(trans_True);

                            //切换视图必须加，否则会有多余图形
                            General.DiagramControl.ClearSelection();
                            General.ftaDiagram.Refresh(true);
                            return;
                        }
                        else if (drawData.LinkPath != "")
                        {
                            Predicate<TreeListNode> match = new Predicate<TreeListNode>(d => d.Tag != null && d.Tag.GetType() == typeof(SystemModel) && ((SystemModel)d.Tag).SystemName == drawData.LinkPath.Split(':')[1] && ((d.ParentNode != null && d.ParentNode.Tag != null && d.ParentNode.Tag.GetType() == typeof(ProjectModel) && ((ProjectModel)d.ParentNode.Tag).ProjectName == drawData.LinkPath.Split(':')[0]) || (d.ParentNode != null && d.ParentNode.ParentNode != null && d.ParentNode.ParentNode.Tag != null && d.ParentNode.ParentNode.Tag.GetType() == typeof(ProjectModel) && ((ProjectModel)d.ParentNode.ParentNode.Tag).ProjectName == drawData.LinkPath.Split(':')[0])));
                            TreeListNode nd = General.ProjectControl.FindNode(match);

                            if (nd != null && nd.ParentNode != null)
                            {
                                //有父节点直接追踪
                                General.ProjectControl.ExpandAll();
                                General.ProjectControl.FocusedNode = nd;
                                return;
                            }
                            else
                            {
                                DialogResult rsC = MsgBox.Show("链接对象[" + drawData.LinkPath + "]未找到(请检查对象工程是否打开)，是否重新指定链接对象?", "", System.Windows.Forms.MessageBoxButtons.YesNo);
                                if (rsC == DialogResult.Yes)
                                {
                                    //让用户选取id
                                    List<string> paths = new List<string>();
                                    foreach (var p in General.FtaProgram.Projects)
                                    {
                                        foreach (var ss in p.Systems)
                                        {
                                            if (General.FtaProgram.CurrentSystem != ss)
                                            {
                                                paths.Add(p.ProjectName + ":" + ss.SystemName);
                                            }
                                        }
                                    }

                                    NewLinkGate form = new NewLinkGate(General.FtaProgram.String, General.FtaProgram.String.InsertLink, paths);
                                    if (form.ShowDialog() == DialogResult.Cancel)
                                    {
                                        form.Dispose();
                                        return;
                                    }
                                    string id = form.GetInfo();
                                    form.Dispose();

                                    if (!string.IsNullOrEmpty(id))
                                    {
                                        foreach (var p in General.FtaProgram.Projects)
                                        {
                                            foreach (var ss in p.Systems)
                                            {
                                                if (p.ProjectName + ":" + ss.SystemName == id)
                                                {
                                                    drawData.LinkPath = id;
                                                    drawData.GUID = "";
                                                    drawData.InputType = FixedString.MODEL_LAMBDA_TAU;
                                                    drawData.Units = FixedString.UNITS_HOURS;
                                                    drawData.Comment1 = ss.Roots[0].Comment1;

                                                    drawData.InputValue = ss.Roots[0].QValue;
                                                    drawData.InputValue2 = "1";

                                                    drawData.LogicalCondition = FixedString.LOGICAL_CONDITION_NORMAL;
                                                    drawData.QValue = ss.Roots[0].QValue;
                                                }
                                            }
                                        }
                                    }
                                }

                                return;
                            }
                        }

                        var editPropertyView = new EditPropertyView(this.programModel, drawData, isTransfer);
                        editPropertyView.ShowDialog();

                        if (editPropertyView.Result.Length > 0)
                        {
                            try
                            {
                                General.IsIgnoreTreeListFocusNodeChangeEvent = true;
                                General.InvokeHandler(GlobalEvent.PropertiesEdited, new Tuple<DrawData, string[]>(drawData, editPropertyView.Result));
                            }
                            finally
                            {
                                General.IsIgnoreTreeListFocusNodeChangeEvent = false;
                            }
                        }
                    }
                }
            }
        }

        private void KeyDown(object sender, KeyEventArgs e)
        {
            var eventValue = string.Empty;
            bool isEmpty = false;
            if (e.Control && e.KeyCode == Keys.C) eventValue = nameof(StringModel.CopyNodes);
            else if (e.Control && e.KeyCode == Keys.V) eventValue = nameof(StringModel.Paste);
            else if (e.Control && e.KeyCode == Keys.X) eventValue = nameof(StringModel.CutNodes);
            else if (e.Control && e.KeyCode == Keys.R) eventValue = nameof(StringModel.Redo);
            else if (e.Control && e.KeyCode == Keys.Z) eventValue = nameof(StringModel.Undo);
            else if (e.KeyCode == Keys.Up) eventValue = nameof(StringModel.PosParent);
            else if (e.KeyCode == Keys.Down) eventValue = nameof(StringModel.PosChild);
            else if (e.KeyCode == Keys.Left) eventValue = nameof(StringModel.PosLeft);
            else if (e.KeyCode == Keys.Right) eventValue = nameof(StringModel.PosRight);
            else if (e.KeyCode == Keys.Delete)//快捷键删除时，按当前选中项情况区分删除
            {
                if (this.diagramControl.SelectedItems.Count == 1)
                {
                    if (this.diagramControl.SelectedItems[0].Tag == null)
                    {
                        return;
                    }

                    DrawData SeData = (DrawData)this.diagramControl.SelectedItems[0].Tag;

                    if (SeData == General.FtaProgram.CurrentRoot)//顶层节点
                    {
                        eventValue = nameof(StringModel.DeleteTop);
                    }
                    else if (SeData?.Parent != null && SeData.Children.Count == 1)//只有一个子节点
                    {
                        eventValue = nameof(StringModel.DeleteNodes);
                    }
                    else if (SeData?.Parent != null && SeData.Children.Count == 0)//没有子节点
                    {
                        eventValue = nameof(StringModel.DeleteNode);
                    }
                    else
                    {
                        eventValue = nameof(StringModel.DeleteNodes);//多个子节点
                    }
                }
                else
                {
                    return;
                }
            }
            else isEmpty = true;

            if (isEmpty == false) General.InvokeHandler(GlobalEvent.TableShortCut, eventValue);
        }

        /// <summary>
        /// 自定义鼠标的显示图标
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        private void DiagramCustomCursor(object s, DiagramCustomCursorEventArgs e)
        {
            this.diagramCustomCursorEventArgs = e;
            this.ChangeCursor(this.diagramCustomCursorEventArgs, this.scalingRate);
        }

        /// <summary>
        /// 动态变更鼠标样式
        /// </summary>
        /// <param name="args"></param>
        /// <param name="scalingRate">缩放率</param>
        private void ChangeCursor(EventArgs args, float scalingRate)
        {
            General.TryCatch(() =>
            {
                //TODO:lable鼠标图形特殊处理
                //if (this.AddingType != DrawType.NULL && this.AddingType != DrawType.Label)
                //{
                if (this.AddingType != DrawType.NULL)
                {
                    var cursor = args as DiagramCustomCursorEventArgs;
                    bool? isHit = null;
                    DiagramItem item = General.DiagramControl.CalcHitItem(General.DiagramControl.PointToClient(System.Windows.Forms.Control.MousePosition));

                    if (item?.GetType() == typeof(DiagramShape) && item.Tag?.GetType() == typeof(DrawData))
                    {
                        isHit = this.IsCreateNew ? true : false;
                    }
                    //释放之前的图标
                    if (cursorLast != null)
                    {
                        General.DestroyIcon(cursorLast.Handle);
                        cursorLast.Dispose();
                        cursorLast = null;
                    }
                    //TODO:当鼠标类型大小不变时，这里没必要指定新的鼠标
                    cursorLast = this.GetCursor(this.AddingType, isHit, scalingRate);
                    cursor.Cursor = cursorLast;
                }
            });
        }

        /// <summary>
        /// 根据门事件类型、是否可以插入以及缩放比例返回鼠标样式
        /// </summary>
        /// <param name="type">图形类型</param>
        /// <param name="isHit">要显示OK还是No图形</param>
        /// <param name="zoomFactor">缩放比例</param>
        /// <returns>鼠标对象</returns>
        private Cursor GetCursor(DrawType type, bool? isHit, float zoomFactor = 1f)
        {
            var result = default(Cursor);
            var newWidth = this.cursorSize.Width * zoomFactor;
            var newHeight = this.cursorSize.Height * zoomFactor;
            using (var bitmap = new Bitmap((int)newWidth, (int)newHeight))
            {
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    DrawBase.DrawComponent(null, type, graphics, 0, 0, newWidth);
                    if (isHit.HasValue)
                    {
                        var bit_OK = isHit.Value ? Properties.Resources.OK : Properties.Resources.NO;
                        var bit_Width = newWidth / 2;
                        var bit_Height = newWidth * bit_OK.Height / 2 / bit_OK.Width;
                        graphics.DrawImage(bit_OK, (newWidth - bit_Width) / 2, 1, bit_Width + 1, bit_Height + 1);
                    }
                }
                result = new Cursor(bitmap.GetHicon());
                result.Tag = result.Handle;
            }
            return result;
        }

        /// <summary>
        /// 鼠标移动时，检查是否允许用户创建新的元素图形
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DiagramMouseMove(object sender, MouseEventArgs e)
        {
            General.InvokeHandler(GlobalEvent.DiagramMouseMove, e);
        }

        /// <summary>
        /// 自定义绘制画布上的图形
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DiagramCustomDrawItem(object sender, CustomDrawItemEventArgs e)
        {
            General.TryCatch(() =>
            {
                //如果没有设置信息，不去绘制了
                StyleModel style = programModel?.CurrentProject?.Style;
                if (style == null) return;

                //如果正常画布绘制时不在可视区就不绘制了
                if (e.Context == DiagramDrawingContext.Canvas || e.Context == DiagramDrawingContext.DragPreview)
                {
                    //测试要绘制的图形是否在可视区域内
                    General.DiagramControl = sender as DiagramControl;
                    PointF start = General.DiagramControl.DiagramViewInfo.RulersOffset;
                    float factor = General.DiagramControl.DiagramViewInfo.ZoomFactor;
                    float width = General.DiagramControl.DiagramViewInfo.ContentRect.Width;
                    float height = General.DiagramControl.DiagramViewInfo.ContentRect.Height;
                    RectangleF rect = new RectangleF(-start.X / factor, -start.Y / factor, width / factor, height / factor);
                    RectangleF item_Rect = new RectangleF(e.Item.X, e.Item.Y, e.Item.Width, e.Item.Height);
                    if (!rect.IntersectsWith(item_Rect)) return;
                }

                //图形绘制
                if (e.Item != null && e.Item.GetType() == typeof(DiagramShape) && e.Item.Tag != null && e.Item.Tag.GetType() == typeof(DrawData))
                {
                    //自定义绘制的数据
                    DrawData data = e.Item.Tag as DrawData;
                    if (data.Type == DrawType.NULL) return;

                    //高亮选中的图形和他的父图形  
                    if (style.ShapeBackSelectedColor != Color.Transparent && CustomDrawItem_diagramControl_FTADiagram_SelectShape_Shape(e))
                    //if (!string.IsNullOrEmpty(style.ShapeBackColor_Selected) && style.ShapeBackColor_Selected.Substring(0, 2) != "0," && CustomDrawItem_diagramControl_FTADiagram_SelectShape_Shape(e))
                    {
                    }
                    //真门/事件背景色
                    else if (style.ShapeBackTrueGateColor != Color.Transparent && data.LogicalCondition == General.GetKeyName(bool.TrueString))
                    {
                        e.Appearance.BackColor = style.ShapeBackTrueGateColor;
                        e.DefaultDraw(CustomDrawItemMode.Background);
                    }
                    //假门/事件背景色
                    else if (style.ShapeBackFalseGateColor != Color.Transparent && data.LogicalCondition == General.GetKeyName(bool.FalseString))
                    {
                        e.Appearance.BackColor = style.ShapeBackFalseGateColor;
                        e.DefaultDraw(CustomDrawItemMode.Background);
                    }
                    //设置图形的默认背景色
                    else if (style.ShapeBackColor != Color.Transparent)
                    {
                        e.Appearance.BackColor = style.ShapeBackColor;
                        e.DefaultDraw(CustomDrawItemMode.Background);
                    }
                    //其他禁用默认的绘制
                    else
                    {
                        e.DefaultDraw(CustomDrawItemMode.None);
                    }

                    //保持3段矩形框的高度比例和设置里一致
                    float textRectHeight = (e.Item.Height - General.PEN_WIDTH) * style.ShapeDescriptionRectHeight / (style.ShapeDescriptionRectHeight + style.ShapeIdRectHeight + style.ShapeSymbolRectHeight);
                    float symbolHeight = (e.Item.Height - General.PEN_WIDTH) * style.ShapeSymbolRectHeight / (style.ShapeDescriptionRectHeight + style.ShapeIdRectHeight + style.ShapeSymbolRectHeight);

                    float centerRectHeight = (e.Item.Height - General.PEN_WIDTH) - textRectHeight - symbolHeight;

                    //高亮割集
                    Color? cor_CutSet = null;

                    if (this.programModel.CurrentSystem == null)
                    {
                        return;
                    }

                    if
                 (style.CutSetColor != Color.Transparent && this.programModel.CurrentSystem != null
                 && this.programModel.CurrentSystem.CurrentSelectedCutset?.Cutset != null
                 && this.programModel.CurrentSystem.CurrentSelectedCutset?.HighLightData != null
                 && this.programModel.CurrentSystem.CurrentSelectedCutset?.HighLightData.Contains(data) == true)
                    {
                        cor_CutSet = style.CutSetColor;
                    }

                    //重复事件绘制
                    if (data.CanRepeatedType && data.Repeats >= 1)
                    {
                        DrawBase.DrawComponent(data, data.Type, e.Graphics, 0, 0, e.Item.Width - General.PEN_WIDTH, textRectHeight, centerRectHeight, symbolHeight, data.Comment1, data.Identifier, data.QValue, style.ShapeFontName, style.ShapeFontSize, false, style.ShapeBackRepeatEventColor, data.Repeats, cor_CutSet);
                    }
                    //转移门本体绘制
                    else if (this.programModel.CurrentSystem.TranferGates != null && this.programModel.CurrentSystem.TranferGates.ContainsKey(data.Identifier) && data.Type != DrawType.TransferInGate)
                    {
                        DrawBase.DrawComponent(data, data.Type, e.Graphics, 0, 0, e.Item.Width - General.PEN_WIDTH, textRectHeight, centerRectHeight, symbolHeight, data.Comment1, data.Identifier, data.QValue, style.ShapeFontName, style.ShapeFontSize, true, null, null, cor_CutSet);
                    }
                    //正常绘制
                    else
                    {
                        if (data.Type == DrawType.VotingGate)
                        {
                            DrawBase.DrawComponent(data, data.Type, e.Graphics, 0, 0, e.Item.Width - General.PEN_WIDTH, textRectHeight, centerRectHeight, symbolHeight, data.Comment1, data.Identifier, data.QValue + " M:" + data.ExtraValue1 + ":" + data.Children.Count.ToString(), style.ShapeFontName, style.ShapeFontSize, false, null, null, cor_CutSet);
                        }
                        else
                        {
                            DrawBase.DrawComponent(data, data.Type, e.Graphics, 0, 0, e.Item.Width - General.PEN_WIDTH, textRectHeight, centerRectHeight, symbolHeight, data.Comment1, data.Identifier, data.QValue, style.ShapeFontName, style.ShapeFontSize, false, null, null, cor_CutSet);
                        }
                    }
                    e.Handled = true;
                }
                //绘制选中图形线条
                //else if (CustomDrawItem_diagramControl_FTADiagram_SelectShape_Line(e))
                //{

                //}
                //割集线条颜色
                else if (e.Item?.GetType() == typeof(DiagramConnector)
                   && e.Item.Tag == null
                   && style.CutSetColor != Color.Transparent
                   && this.programModel.CurrentSystem?.CurrentSelectedCutset?.Cutset != null
                   && this.programModel.CurrentSystem.CurrentSelectedCutset?.HighLightData != null)
                {
                    DiagramConnector con = e.Item as DiagramConnector;
                    if (con.BeginItem != null && con.BeginItem.Tag != null && con.BeginItem.Tag.GetType() == typeof(DrawData)
                        && con.EndItem != null && con.EndItem.Tag != null && con.EndItem.Tag.GetType() == typeof(DrawData))
                    {
                        DrawData data_Begin = con.BeginItem.Tag as DrawData;
                        DrawData data_End = con.EndItem.Tag as DrawData;
                        if
                        (this.programModel.CurrentSystem.CurrentSelectedCutset?.HighLightData.Contains(data_Begin) == true
                        && this.programModel.CurrentSystem.CurrentSelectedCutset?.HighLightData.Contains(data_End) == true)
                        {
                            Color cor_CutSet = style.CutSetColor;
                            e.Appearance.BorderColor = cor_CutSet;
                            e.DefaultDraw(CustomDrawItemMode.All);
                            e.Handled = true;
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 选中图形时，图形及其父图形背景色变化
        /// </summary>
        /// <param name="e"></param>
        /// <returns>是否成功处理</returns>
        private bool CustomDrawItem_diagramControl_FTADiagram_SelectShape_Shape(CustomDrawItemEventArgs e)
        {
            return General.TryCatch(() =>
            {
                //高亮选中的图形和他的父图形                        
                //if (e.Item?.Tag != null && General.DiagramItemPool.SelectedData.Count == 1 &&
                // (General.DiagramItemPool.SelectedData.FirstOrDefault() == e.Item.Tag ||
                // General.DiagramItemPool.SelectedData.FirstOrDefault().Parent == e.Item.Tag))
                //{
                if (e.Item?.Tag != null && General.DiagramItemPool.SelectedData.Count == 1 &&
               (General.DiagramItemPool.SelectedData.FirstOrDefault() == e.Item.Tag))
                {
                    //设置选中图形的默认背景色
                    e.Appearance.BackColor = programModel.CurrentProject.Style.ShapeBackSelectedColor;
                    e.DefaultDraw(CustomDrawItemMode.Background);
                    return true;
                }
                return false;
            });
        }

        /// <summary>
        /// 选中图形时线条高亮
        /// </summary>
        /// <param name="e"></param>
        /// <returns>是否成功处理</returns>
        private bool CustomDrawItem_diagramControl_FTADiagram_SelectShape_Line(CustomDrawItemEventArgs e)
        {
            return General.TryCatch(() =>
            {
                if (e.Item != null && e.Item.GetType() == typeof(DiagramConnector)
                 && General.DiagramItemPool.SelectedData.Count == 1)
                {
                    DiagramConnector con = (DiagramConnector)e.Item;
                    DrawData data = General.DiagramItemPool.SelectedData.FirstOrDefault();
                    if (con.BeginItem?.Tag != null && data.Parent != null && con.EndItem?.Tag != null)
                    {
                        if (con.BeginItem?.Tag == data.Parent && con.EndItem.Tag == data)
                        {
                            e.Appearance.BorderColor = programModel.CurrentProject.Style.ShapeBackSelectedColor;
                            e.Appearance.BorderSize = 2;
                            e.DefaultDraw(CustomDrawItemMode.All);
                            e.Handled = true;
                            return true;
                        }
                    }
                }
                return false;
            });
        }

        /// <summary>
        /// DiagramControl控件缩放比例变更事件（换鼠标形状）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DiagramZoomFactorChanged(object sender, DiagramZoomFactorChangedEventArgs e)
        {
            this.scalingRate = (sender as DiagramControl).OptionsView.ZoomFactor;
            this.ChangeCursor(this.diagramCustomCursorEventArgs, this.scalingRate);
        }

        /// <summary>
        /// 自定义右键菜单"高亮割集"的子项
        /// </summary>
        private void GetItemData_BarButtonItem_HighLightCutSet()
        {
            General.TryCatch(() =>
            {
                if (General.DiagramItemPool.SelectedData.Count == 1 && General.HighLightCutSet?.Gallery?.Groups?.Count > 0)
                {
                    DrawData data_Selected = General.DiagramItemPool.SelectedData.FirstOrDefault();
                    if (this.programModel.CurrentSystem != null && data_Selected.Cutset != null && data_Selected.Cutset.ListCutsets_Real != null
                        && data_Selected.Cutset.ListCutsets_Real.Count > 0)
                    {
                        //添加本次子菜单
                        GalleryItemGroup gp = General.HighLightCutSet.Gallery.Groups[0];
                        gp.Items.Clear();
                        List<GalleryItem> sub_Items = new List<GalleryItem>();
                        List<OneCutsetModel> ordered_CutSet = data_Selected.Cutset.ListCutsets_Real.OrderBy(obj => obj.szProb).ToList();
                        for (int i = 0; i < ordered_CutSet.Count; i++)
                        {
                            if (ordered_CutSet[i] != null && !string.IsNullOrEmpty(ordered_CutSet[i].szProb) && ordered_CutSet[i].Events != null && ordered_CutSet[i].Events.Count > 0)
                            {
                                GalleryItem sub_Item = new GalleryItem();
                                sub_Item.Caption = i + ":Q = " + ordered_CutSet[i].szProb;
                                sub_Item.Tag = ordered_CutSet[i];
                                sub_Items.Add(sub_Item);
                                //如果是已经选择的割集
                                if (this.programModel.CurrentSystem.CurrentSelectedCutset != null && this.programModel.CurrentSystem.CurrentSelectedCutset.Cutset != null
                                    && this.programModel.CurrentSystem.CurrentSelectedCutset.Cutset == ordered_CutSet[i])
                                {
                                    //if (!string.IsNullOrEmpty(ftaProgram.Setting.CutSetColor))
                                    //{
                                    //    string[] argb = ftaProgram.Setting.CutSetColor.Split(',');
                                    //    Color cor_CutSet = Color.FromArgb(int.Parse(argb[0]), int.Parse(argb[1]), int.Parse(argb[2]), int.Parse(argb[3]));
                                    //    Color cor_Font = Color.FromArgb(255, (int.Parse(argb[0]) + 125) & 255, (125 + int.Parse(argb[2])) & 255, (125 + int.Parse(argb[3])) & 255);
                                    //    sub_Item.AppearanceCaption.Normal.BackColor = cor_CutSet;
                                    //    sub_Item.AppearanceCaption.Normal.BackColor2 = cor_CutSet;
                                    //    sub_Item.AppearanceCaption.Normal.ForeColor = cor_Font;
                                    //    sub_Item.AppearanceDescription.Normal.BackColor = cor_CutSet;
                                    //    sub_Item.AppearanceDescription.Normal.BackColor2 = cor_CutSet;
                                    //    sub_Item.AppearanceDescription.Normal.ForeColor = cor_Font;
                                    //}
                                    sub_Item.Checked = true;
                                }
                            }
                        }
                        gp.Items.AddRange(sub_Items.ToArray());
                        if (sub_Items.Count < 10) General.HighLightCutSet.Gallery.RowCount = sub_Items.Count;
                        else General.HighLightCutSet.Gallery.RowCount = 10;
                    }
                }
            });
        }

        /// <summary>
        /// 高亮显示选中的图形和他的父节点,鼠标单击右键时取消当前工具的选中状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DiagramMouseDown(object sender, MouseEventArgs e)
        {
            General.TryCatch(() =>
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        {
                            switch (e.Clicks)
                            {
                                case 1:
                                    {
                                        //单击图形，保持表和图的数据为同步选中状态
                                        if (this.AddingType == DrawType.NULL)
                                        {
                                            DiagramItem item = General.DiagramControl.CalcHitItem(General.DiagramControl.PointToClient(Control.MousePosition));
                                            if (item?.GetType() == typeof(DiagramShape) && item?.Tag?.GetType() == typeof(DrawData))
                                            {
                                                DrawData SeData = (DrawData)item.Tag;

                                                foreach (DiagramItem tagItem in General.DiagramControl.Items)
                                                {
                                                    if (tagItem.Tag != null && ((DrawData)tagItem.Tag).ThisGuid == ((DrawData)item.Tag).ThisGuid && ((DrawData)tagItem.Tag).Children != null)
                                                    {
                                                        SeData = (DrawData)tagItem.Tag;
                                                    }
                                                }

                                                General.InvokeHandler(GlobalEvent.TableFocused, SeData);
                                                General.DiagramControl.Refresh();
                                            }
                                        }
                                        break;
                                    }
                                default: break;
                            }
                            break;
                        }
                    case MouseButtons.Right:
                        {
                            if (e.Clicks == 1)
                            {
                                //右键单击图形，保持表和图的数据为同步选中状态
                                if (this.AddingType == DrawType.NULL)
                                {
                                    DiagramItem item = General.DiagramControl.CalcHitItem(General.DiagramControl.PointToClient(Control.MousePosition));
                                    if (item?.GetType() == typeof(DiagramShape) && item?.Tag?.GetType() == typeof(DrawData))
                                    {
                                        DrawData SeData = (DrawData)item.Tag;

                                        foreach (DiagramItem tagItem in General.DiagramControl.Items)
                                        {
                                            if (tagItem.Tag != null && ((DrawData)tagItem.Tag).ThisGuid == ((DrawData)item.Tag).ThisGuid && ((DrawData)tagItem.Tag).Children != null)
                                            {
                                                SeData = (DrawData)tagItem.Tag;
                                            }
                                        }

                                        General.InvokeHandler(GlobalEvent.TableFocused, SeData);
                                        General.DiagramControl.Refresh();
                                    }
                                }
                            }
                            break;
                        }
                    default: break;
                }
            });
        }

        /// <summary>
        /// 鼠标在画布上单击时创建新的图形元素到画布里
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DiagramMouseUp(object sender, MouseEventArgs e)
        {
            General.TryCatch(() =>
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        {
                            switch (e.Clicks)
                            {
                                case 1:
                                    {
                                        if (this.IsCreateNew)
                                        {
                                            //当前父图形对象
                                            PointF pos = General.DiagramControl.PointToClient(System.Windows.Forms.Control.MousePosition);
                                            DiagramItem item = General.DiagramControl.CalcHitItem(pos);
                                            if (item != null)
                                            {
                                                DrawData parentData = item.Tag as DrawData;
                                                General.InvokeHandler(GlobalEvent.InsertNode, new Tuple<DrawData, DrawType, MouseEventArgs>(parentData, this.AddingType, e));
                                            }
                                        }
                                        break;
                                    }
                            }
                            break;
                        }
                    case MouseButtons.Right:
                        {
                            DiagramItem item = General.DiagramControl.CalcHitItem(General.DiagramControl.PointToClient(Control.MousePosition));
                            if (item?.GetType() == typeof(DiagramShape) && item?.Tag?.GetType() == typeof(DrawData) && ((DrawData)item.Tag).Type != DrawType.NULL)
                            {
                                //禁用启用菜单
                                DrawData data_Selected = item.Tag as DrawData;
                                foreach (BarItemLink link in General.DiagramMenuControl.ItemLinks)
                                {
                                    link.Item.Enabled = General.GetBarItemIsEnabled(link.Item, data_Selected, null, null, null);

                                    if (link.Item == General.HighLightCutSet && link.Item.Enabled)
                                    {
                                        //添加高亮割集子菜单
                                        GetItemData_BarButtonItem_HighLightCutSet();
                                    }
                                }
                                //弹出右键菜单                  
                                General.DiagramMenuControl.ShowPopup(Control.MousePosition);
                                //return true;
                            }
                            General.InvokeHandler(GlobalEvent.CheckStateReset);
                            this.IsInsertingNode = false;
                            break;
                        }
                }
            });
        }


        /// <summary>
        /// 高亮割集子项单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void GalleryItemClick(object sender, GalleryItemClickEventArgs e)
        {
            General.TryCatch(() =>
            {
                if (General.DiagramItemPool.SelectedData.Count == 1)
                {
                    //选中的对象
                    DrawData data_Selected = General.DiagramItemPool.SelectedData.FirstOrDefault();
                    if (e.Item.Tag != null && e.Item.Tag.GetType() == typeof(OneCutsetModel))
                    {
                        OneCutsetModel cutSet = e.Item.Tag as OneCutsetModel;
                        if (this.programModel.CurrentSystem.CurrentSelectedCutset != null && this.programModel.CurrentSystem.CurrentSelectedCutset.Cutset == cutSet)
                        {
                            this.programModel.CurrentSystem.CurrentSelectedCutset = null;
                        }
                        else
                        {
                            this.programModel.CurrentSystem.CurrentSelectedCutset = new HighLightCutSet();
                            this.programModel.CurrentSystem.CurrentSelectedCutset.Cutset = cutSet;
                            this.programModel.CurrentSystem.CurrentSelectedCutset.HighLightData = new HashSet<DrawData>();
                            //重新计算路径
                            List<DrawData> allDatas = this.programModel.CurrentSystem.GetAllDatas().ToList();
                            var ids = cutSet.Events.Distinct();
                            foreach (string id in ids)
                            {
                                var evts = allDatas.Where(obj => obj.CanRepeatedType && obj.Identifier == id);
                                foreach (DrawData evt in evts)
                                {
                                    List<DrawData> path = evt.GetPath(data_Selected, this.programModel.CurrentSystem.TranferGates);
                                    if (path != null)
                                    {
                                        foreach (DrawData tmp in path)
                                        {
                                            this.programModel.CurrentSystem.CurrentSelectedCutset.HighLightData.Add(tmp);
                                        }
                                    }
                                }
                            }
                        }
                        this.diagramControl.Refresh();
                    }
                }
            });
        }


        /// <summary>
        /// 高亮割集子项移入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void GalleryItemHover(object sender, GalleryItemEventArgs e)
        {
            General.TryCatch(() => { });
        }


        /// <summary>
        /// 高亮割集子菜单关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void GalleryPopupClose_RibbonGalleryBarItem_HighLightCutSet(object sender, InplaceGalleryEventArgs e)
        {
            General.TryCatch(() => { });
        }
    }
}
