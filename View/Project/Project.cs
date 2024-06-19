using DevExpress.XtraSplashScreen;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using FaultTreeAnalysis.Common;
using FaultTreeAnalysis.Properties;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using FaultTreeAnalysis.Model.Data;
using System.IO;
using System.Text;
using DevExpress.XtraDiagram;
using DevExpress.XtraBars.Docking;
using FaultTreeAnalysis.Behavior.Event;
using System.Diagnostics;
using System.Collections;

namespace FaultTreeAnalysis
{
    partial class FTAControl
    {
        ///<summary>
        /// 初始化Project面板
        /// </summary>
        private void InitializeProjectControl()
        {
            General.TryCatch(() =>
            {
                RegisterEvents();
                ImageList state_Images = new ImageList();
                state_Images.Images.Add(Resources.project_16x16);
                state_Images.Images.Add(Resources.documentmap_16x16);
                state_Images.Images.Add(Resources.documentmapEdit_16x161);
                state_Images.Images.Add(Resources.packageproduct_16x16);
                treeList_Project.StateImageList = state_Images;
            });
        }

        /// <summary>
        ///  设置左键单击节点时加载故障树表和图,右键单击时弹出菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProjectMouseDown(object sender, MouseEventArgs e)
        {
            General.TryCatch(() =>
            {
                TreeListHitInfo hit = treeList_Project.CalcHitInfo(e.Location);
                //单击工程
                //if (hit.InRowCell == false && hit.InRowStateImage == false)
                //{
                //    treeList_Project.FocusedNode = null;
                //}
                if (e.Button == MouseButtons.Right && e.Clicks == 1)
                {
                    //鼠标单击右键时菜单是否禁用
                    PopUpMenu_Project_EnableAndDisableMenu(hit);
                    popupMenu_Project.ShowPopup(MousePosition);
                    return;
                }
            });
        }

        private void ProjectFocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            General.TryCatch(() =>
            {
                if (General.IsClosed)
                {
                    return;
                }

                ribbonGalleryBarItem_GraphicTool.Enabled = false;

                SplashScreenManager.ShowDefaultWaitForm();

                //焦点在不同位置，菜单栏和工具栏部分菜单禁用启用
                if (treeList_Project.FocusedNode != null && treeList_Project.FocusedNode.Tag != null && treeList_Project.FocusedNode.Tag.GetType() == typeof(ProjectModel))
                {
                    barButtonItem_CreateNew_System.Enabled = true;
                    barButtonItem_ToolNew.Enabled = true;
                    Bbi_Import.Enabled = true;
                    Bbi_Export.Enabled = true;
                    barButtonItem_FTA.Enabled = true;
                    barButtonItem_FTADiagram.Enabled = true;
                    barButtonItem_Cutset.Enabled = true;
                }
                else if (treeList_Project.FocusedNode != null && treeList_Project.FocusedNode.Tag == null)
                {
                    barButtonItem_CreateNew_System.Enabled = true;
                    barButtonItem_ToolNew.Enabled = true;
                    Bbi_Import.Enabled = true;
                    Bbi_Export.Enabled = true;
                    barButtonItem_FTA.Enabled = true;
                    barButtonItem_FTADiagram.Enabled = true;
                    barButtonItem_Cutset.Enabled = true;
                }
                else if (treeList_Project.FocusedNode != null && treeList_Project.FocusedNode.Tag != null && treeList_Project.FocusedNode.Tag.GetType() == typeof(SystemModel))
                {
                    barButtonItem_CreateNew_System.Enabled = false;
                    barButtonItem_ToolNew.Enabled = false;
                    Bbi_Import.Enabled = true;
                    Bbi_Export.Enabled = true;
                    barButtonItem_FTA.Enabled = true;
                    barButtonItem_FTADiagram.Enabled = true;
                    barButtonItem_Cutset.Enabled = true;
                }
                else
                {
                    barButtonItem_CreateNew_System.Enabled = false;
                    barButtonItem_ToolNew.Enabled = false;
                    Bbi_Import.Enabled = false;
                    Bbi_Export.Enabled = false;
                    barButtonItem_FTA.Enabled = false;
                    barButtonItem_FTADiagram.Enabled = false;
                    barButtonItem_Cutset.Enabled = false;
                }

                if (e.Node != null)
                {
                    var type = e.Node?.Tag?.GetType();
                    if (type == typeof(SystemModel))
                    {
                        barButtonItem_InsertLevel.Enabled = true;
                        barButtonItem_TopGatePos.Enabled = true;
                        barButtonItem_PosParent.Enabled = true;
                        barButtonItem_PosChild.Enabled = true;
                        barSubItem_ChangeGateType.Enabled = true;
                        ribbonGalleryBarItem_GraphicTool.Enabled = true;
                        barSubItem_Copy.Enabled = true;
                        barSubItem_Paste.Enabled = true;
                        barSubItem_MenuDelete.Enabled = true;
                        barButtonItem_FindAndReplace.Enabled = true;
                        barButtonItem_FindAndReplaceCustom.Enabled = true;
                        barButtonItem_Check.Enabled = true;
                        Bbi_Renumber.Enabled = true;
                        barButtonItem_FTACalculate.Enabled = true;
                        barButtonItem_HighLightCutSets.Enabled = true;

                        SystemModel system = e.Node.Tag as SystemModel;

                        if (system != General.FtaProgram.CurrentSystem)
                        {
                            General.FtaProgram.SetCurrentSystem(system);
                            ProjectModel newProj = General.FtaProgram.GetFTAProjectFromFTASystem(system);
                            //项目不可能是空的
                            if (newProj == null) return;

                            //切换了项目
                            if (newProj != General.FtaProgram.CurrentProject)
                            {
                                General.FtaProgram.SetCurrentProject(newProj);
                                //重置设置项状态和值
                                ReSetFTAStyle();
                            }

                            General.StaticItem_File.Caption = General.FtaProgram.CurrentProject.ProjectName + "/" + system.SystemName;

                            if (system.Roots != null && system.Roots.Count > 0)
                            {
                                //初始化转移门与重复事件集合
                                if (system.TranferGates == null || system.RepeatedEvents == null)
                                {
                                    system.UpdateRepeatedAndTranfer();
                                }

                                //treelist加载数据，同时设置了当前系统对象
                                this.ftaTable.TableEvents.LoadDataToTableControl(system);

                                //重置宽高菜单和动态显示xx里的宽高数据,重设置项目里style宽高值
                                ReSetFTAStyleWidthHeight();
                                General.FtaProgram.CurrentProject.Style.ShapeWidth = system.ShapeWidth;
                                General.FtaProgram.CurrentProject.Style.ShapeDescriptionRectHeight = system.ShapeDescriptionRectHeight;
                                General.FtaProgram.CurrentProject.Style.ShapeIdRectHeight = system.ShapeIdRectHeight;
                                General.FtaProgram.CurrentProject.Style.ShapeSymbolRectHeight = system.ShapeSymbolRectHeight;

                                this.LoadDrawDataToDiagram(system);
                                General.DiagramControl.ScrollToPoint(new DevExpress.Utils.PointFloat(system.Roots[0].X, system.Roots[0].Y));

                                if (this.dockPanel_FTATable.Visibility == DockVisibility.Hidden || this.dockPanel_FTADiagram.Visibility == DockVisibility.Hidden)
                                {
                                    if (General.FtaProgram.Setting.Language == FixedString.LANGUAGE_CN_CN)
                                    {
                                        this.dockManager_FTA.RestoreLayoutFromXml(Application.StartupPath + "\\FaultTreeStartPage_CN.xml");
                                    }
                                    else
                                    {
                                        this.dockManager_FTA.RestoreLayoutFromXml(Application.StartupPath + "\\FaultTreeStartPage_EN.xml");
                                    }
                                }
                            }
                            //切换了其他节点，但是没有数据
                            else
                            {
                                General.StaticItem_File.Caption = "";
                                General.PanelContainer.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
                                General.FtaProgram.SetCurrentProject(null);
                            }
                            RecentFiles.AddProject(newProj.ProjectPath);//最近文件
                            RecentFiles.AddFaultTree(newProj.ProjectPath + "\\" + system.SystemName + FixedString.APP_EXTENSION);//最近文件
                        }
                        else
                        {
                            General.FtaProgram.SetCurrentSystem(system);
                            ProjectModel newProj = General.FtaProgram.GetFTAProjectFromFTASystem(system);
                            //项目不可能是空的
                            if (newProj == null) return;

                            //重置设置项状态和值
                            ReSetFTAStyle();
                            this.ftaTable.TableEvents.ClearTableControl();
                            this.ftaDiagram.ReSetDiagram();
                            ReSetCheckState();

                            General.StaticItem_File.Caption = General.FtaProgram.CurrentProject.ProjectName + "/" + system.SystemName;
                        }

                        this.dockPanel_FTADiagram.Text = system.SystemName;
                        General.dockManager_FTA.ActivePanel = this.dockPanel_FTADiagram;
                    }
                    else if (type == typeof(ProjectModel))
                    {
                        this.dockPanel_FTADiagram.Text = General.FtaProgram.String.FTADiagram;
                        barButtonItem_InsertLevel.Enabled = false;
                        barButtonItem_TopGatePos.Enabled = false;
                        barButtonItem_PosParent.Enabled = false;
                        barButtonItem_PosChild.Enabled = false;
                        barSubItem_ChangeGateType.Enabled = false;
                        ribbonGalleryBarItem_GraphicTool.Enabled = false;
                        barSubItem_Copy.Enabled = false;
                        barSubItem_Paste.Enabled = false;
                        barSubItem_MenuDelete.Enabled = false;
                        barButtonItem_FindAndReplace.Enabled = false;
                        barButtonItem_FindAndReplaceCustom.Enabled = false;
                        barButtonItem_Check.Enabled = false;
                        Bbi_Renumber.Enabled = false;
                        barButtonItem_FTACalculate.Enabled = false;
                        barButtonItem_HighLightCutSets.Enabled = false;

                        //General.PanelContainer.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
                        //this.dockPanel_FTADiagram.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;

                        ProjectModel newProj = e.Node.Tag as ProjectModel;
                        General.StaticItem_File.Caption = newProj.ProjectName;
                        General.FtaProgram.SetCurrentProject(newProj);

                        //重置设置项状态和值
                        ReSetFTAStyle();
                        this.ftaTable.TableEvents.ClearTableControl();
                        this.ftaDiagram.ReSetDiagram();
                        ReSetCheckState();
                    }
                    else
                    {
                        this.dockPanel_FTADiagram.Text = General.FtaProgram.String.FTADiagram;
                        barButtonItem_InsertLevel.Enabled = false;
                        barButtonItem_TopGatePos.Enabled = false;
                        barButtonItem_PosParent.Enabled = false;
                        barButtonItem_PosChild.Enabled = false;
                        barSubItem_ChangeGateType.Enabled = false;
                        ribbonGalleryBarItem_GraphicTool.Enabled = false;
                        barSubItem_Copy.Enabled = false;
                        barSubItem_Paste.Enabled = false;
                        barSubItem_MenuDelete.Enabled = false;
                        barButtonItem_FindAndReplace.Enabled = false;
                        barButtonItem_FindAndReplaceCustom.Enabled = false;
                        barButtonItem_Check.Enabled = false;
                        Bbi_Renumber.Enabled = false;
                        barButtonItem_FTACalculate.Enabled = false;
                        barButtonItem_HighLightCutSets.Enabled = false;

                        if (e.Node.GetValue("SortType").ToString() == "Group")
                        {
                            ProjectModel newProj = e.Node.ParentNode.Tag as ProjectModel;
                            General.StaticItem_File.Caption = newProj.ProjectName;
                            General.FtaProgram.SetCurrentProject(newProj);
                        }

                        //重置设置项状态和值
                        ReSetFTAStyle();
                        this.ftaTable.TableEvents.ClearTableControl();
                        this.ftaDiagram.ReSetDiagram();
                        ReSetCheckState();
                    }
                    this.SetTreeListTableColumnState(General.FtaProgram.CurrentProject?.ColumnFieldInfos);
                }
                else
                {
                    this.dockPanel_FTADiagram.Text = General.FtaProgram.String.FTADiagram;
                    barButtonItem_InsertLevel.Enabled = false;
                    barButtonItem_TopGatePos.Enabled = false;
                    barButtonItem_PosParent.Enabled = false;
                    barButtonItem_PosChild.Enabled = false;
                    barSubItem_ChangeGateType.Enabled = false;
                    ribbonGalleryBarItem_GraphicTool.Enabled = false;
                    barSubItem_Copy.Enabled = false;
                    barSubItem_Paste.Enabled = false;
                    barSubItem_MenuDelete.Enabled = false;
                    barButtonItem_FindAndReplace.Enabled = false;
                    barButtonItem_FindAndReplaceCustom.Enabled = false;
                    barButtonItem_Check.Enabled = false;
                    Bbi_Renumber.Enabled = false;
                    barButtonItem_FTACalculate.Enabled = false;
                    barButtonItem_HighLightCutSets.Enabled = false;
                    General.StaticItem_File.Caption = "";
                    //General.PanelContainer.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
                    //this.dockPanel_FTADiagram.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
                    General.FtaProgram.SetCurrentProject(null);

                    //重置设置项状态和值
                    ReSetFTAStyle();
                    this.ftaTable.TableEvents.ClearTableControl();
                    this.ftaDiagram.ReSetDiagram();
                    ReSetCheckState();
                }
                if (SplashScreenManager.Default != null)
                {
                    SplashScreenManager.CloseDefaultWaitForm();
                }

                //初始状态定位到顶点，防止出现多余的线条
                General.TryCatch(() =>
                {
                    if (General.FtaProgram.CurrentSystem != null && General.FtaProgram.CurrentSystem.Roots.Count > 0 && General.FtaProgram.CurrentRoot != null)
                    {
                        General.FTATree.FocusedNode = null;

                        Predicate<TreeListNode> match = new Predicate<TreeListNode>(d => (d.GetValue(FixedString.COLUMNAME_DATA) != null && d.GetValue(FixedString.COLUMNAME_DATA) == General.FtaProgram.CurrentRoot));
                        TreeListNode nd = General.FTATree.FindNode(match);
                        General.FTATree.FocusedNode = nd;
                    }
                });
            });
        }

        private void dockPanel_FTADiagram_ClosingPanel(object sender, DockPanelCancelEventArgs e)
        {
            General.TryCatch(() =>
            {
                General.ProjectControl.ExpandAll();
                DiagramControl dc = (DiagramControl)((DockPanel)sender).Controls[0].Controls[0];
                DiagramControl[] NewDiagrams = General.DiagramControlList.Where(o => o == dc).ToArray();
                General.DiagramControlList.Remove(NewDiagrams[0]);
                this.dockManager_FTA.RemovePanel(e.Panel);
            });
        }

        /// <summary>
        /// 切换故障树Tab时自动切换Project树的焦点来刷新数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dockPanel_FTADiagram_Enter(object sender, EventArgs e)
        {
            General.TryCatch(() =>
            {
                General.ProjectControl.ExpandAll();
                DiagramControl dc = (DiagramControl)((DockPanel)sender).Controls[0].Controls[0];
                DiagramControl[] NewDiagrams = General.DiagramControlList.Where(o => o == dc).ToArray();

                foreach (TreeListNode node in General.ProjectControl.Nodes)
                {
                    foreach (TreeListNode childnode in node.Nodes)
                    {
                        if ((SystemModel)childnode.Tag == (SystemModel)NewDiagrams[0].Tag)
                        {
                            General.ProjectControl.FocusedNode = childnode;
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 装载系统对象下的Diagram集合到字典对象
        /// </summary>
        private void LoadDrawDataToDiagram(SystemModel system)
        {
            if (system != null)
            {
                if (system.Roots != null && General.FtaProgram?.CurrentProject?.Style != null && system.Roots.Count > 0)
                {
                    //图形加载数据
                    this.ftaDiagram.Load(system.Roots[0], General.FtaProgram.CurrentProject.Style);
                }
            }
        }

        /// <summary>
        /// 重置项目管理面板状态，会释放当前所有数据，这个方法用于切换FTAProgram程序对象
        /// </summary>
        private void Project_Reset()
        {
            General.TryCatch(() =>
            {
                //TODO:保存项目
                foreach (ProjectModel project in General.FtaProgram.Projects)
                {
                    foreach (SystemModel system in project.Systems)
                    {
                        //解绑DrawData对象，防止内存泄漏
                        foreach (DrawData data in system.Roots)
                        {
                            data.Delete();
                        }
                    }
                }
                treeList_Project.Nodes.Clear();
            });
        }

        /// <summary>
        /// 项目管理面板加载项目数据(添加方式)，刷新树视图
        /// </summary>
        /// <param name="projects">项目对象集合</param>
        /// <param name="FocusNodePath">焦点位置</param>
        private void Project_Load(List<ProjectModel> projects, string FocusNodePath)
        {
            General.TryCatch(() =>
            {
                treeList_Project.Nodes.Clear();
                if (projects != null && projects.Count > 0)
                {
                    for (int i = 0; i < projects.Count; i++)
                    {
                        if (projects[i] != null)
                        {
                            //添加项目节点
                            TreeListNode node_Project = treeList_Project.Nodes.Add(new object[] { projects[i].ProjectName, "Project" });
                            if (node_Project != null)
                            {
                                node_Project.Tag = projects[i];
                                node_Project.StateImageIndex = 0;
                                if (projects[i].Systems != null && projects[i].Systems.Count > 0)
                                {
                                    for (int j = 0; j < projects[i].Systems.Count; j++)
                                    {
                                        if (projects[i].Systems[j] != null)
                                        {
                                            //添加分组
                                            TreeListNode node_GroupLevel = null;
                                            if (projects[i].Systems[j].GroupLevel != null && projects[i].Systems[j].GroupLevel != "")
                                            {
                                                foreach (TreeListNode nd in node_Project.Nodes)
                                                {
                                                    if (nd.GetDisplayText("name") == projects[i].Systems[j].GroupLevel)
                                                    {
                                                        node_GroupLevel = nd;
                                                    }
                                                }
                                                if (node_GroupLevel == null)
                                                {
                                                    node_GroupLevel = node_Project.Nodes.Add(new object[] { projects[i].Systems[j].GroupLevel, "Group" });
                                                }

                                                node_GroupLevel.StateImageIndex = 3;
                                                if (!node_GroupLevel.Expanded) node_GroupLevel.Expand();
                                            }

                                            //添加系统节点
                                            if (node_GroupLevel == null)
                                            {
                                                TreeListNode node_System = node_Project.Nodes.Add(new object[] { projects[i].Systems[j].SystemName, "System" });
                                                node_System.Tag = projects[i].Systems[j];
                                                node_System.StateImageIndex = 1;
                                                if (!node_Project.Expanded) node_Project.Expand();

                                                //为系统注册事件
                                                projects[i].Systems[j].PropertyChanged -= SystemModel_PropertyChanged;
                                                projects[i].Systems[j].PropertyChanged += SystemModel_PropertyChanged;
                                            }
                                            else
                                            {
                                                TreeListNode node_System = node_GroupLevel.Nodes.Add(new object[] { projects[i].Systems[j].SystemName, "System" });
                                                node_System.Tag = projects[i].Systems[j];
                                                node_System.StateImageIndex = 1;
                                                if (!node_GroupLevel.Expanded) node_GroupLevel.Expand();

                                                //为系统注册事件
                                                projects[i].Systems[j].PropertyChanged -= SystemModel_PropertyChanged;
                                                projects[i].Systems[j].PropertyChanged += SystemModel_PropertyChanged;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            });
        }

        #region 增加删除系统和项目函数
        /// <summary>
        /// 项目管理面板里，添加项目（加节点加项目对象，可调序）
        /// </summary>
        /// <param name="projectName">项目名字</param>
        /// <param name="order">指定在所有项目里的顺序，不指定在最后插入</param>
        /// <param name="tag">项目的额外信息</param>
        /// <returns>产生的父节点</returns>
        private TreeListNode AddProject(string projectName, string Path, int? order = null, object tag = null)
        {
            TreeListNode node_Project = null;
            try
            {
                if (!string.IsNullOrEmpty(projectName))
                {
                    //生成项目对象
                    ProjectModel project = new ProjectModel();
                    project.ProjectPath = Path + "\\" + projectName;
                    project.InitializeColumnFieldInfos();
                    project.ProjectName = projectName;
                    project.Systems = new List<SystemModel>();
                    project.Tag = tag;

                    if (order != null && order >= 0 && order <= General.FtaProgram.Projects.Count)
                    {
                        General.FtaProgram.Projects.Insert((int)order, project);
                        //添加项目节点
                        node_Project = treeList_Project.Nodes.Add(new object[] { project.ProjectName, "Project" });
                        treeList_Project.MoveNode(node_Project, node_Project.ParentNode, false, (int)order);
                    }
                    else
                    {
                        General.FtaProgram.Projects.Add(project);
                        //添加项目节点
                        node_Project = treeList_Project.Nodes.Add(new object[] { project.ProjectName, "Project" });
                    }

                    if (node_Project != null)
                    {
                        node_Project.Tag = project;
                        node_Project.StateImageIndex = 0;

                        SaveData();//自动保存新建的

                        RecentFiles.AddProject(project.ProjectPath);//最近文件
                        ReloadRecentFiles();
                    }
                }

            }
            catch (Exception ex)
            {
                MsgBox.Show(ex.Message);
            }
            return node_Project;
        }

        /// <summary>
        /// 项目管理面板里，添加系统（加节点加系统对象，可调序）
        /// </summary>
        /// <param name="node_Project">项目节点对象</param>
        /// <param name="systemName">系统名字</param>
        /// <param name="roots">和系统绑定的数据对象</param>
        /// <param name="order">系统里的顺序</param>
        /// <returns>新系统的节点</returns>
        private TreeListNode AddSystem(TreeListNode node_Project, string systemName, List<DrawData> roots = null, int? order = null)
        {
            TreeListNode node_System = null;
            try
            {
                if (node_Project != null && node_Project.Tag != null && node_Project.Tag.GetType() == typeof(ProjectModel) && !string.IsNullOrEmpty(systemName))
                {
                    ProjectModel project = node_Project.Tag as ProjectModel;

                    while (true)
                    {
                        if (project.Systems.Where(o => o.SystemName == systemName).Count() > 0)
                        {
                            systemName = systemName + "_1";
                        }
                        else
                        {
                            break;
                        }
                    }

                    //生成系统对象
                    SystemModel system = new SystemModel();
                    system.UpdateRenumberItem(systemName);
                    if (roots == null) system.CreateOneRootItem();
                    else system.Roots = roots;

                    if (order != null && order >= 0 && order <= project.Systems.Count)
                    {
                        project.Systems.Insert((int)order, system);
                        //添加项目节点
                        node_System = node_Project.Nodes.Add(new object[] { system.SystemName, "System" });
                        treeList_Project.MoveNode(node_System, node_Project, false, (int)order);
                    }
                    else
                    {
                        project.Systems.Add(system);
                        //添加项目节点
                        node_System = node_Project.Nodes.Add(new object[] { system.SystemName, "System" });
                    }

                    node_System.Tag = system;
                    node_System.StateImageIndex = 1;
                    if (!node_Project.Expanded) node_Project.Expand();


                    //为系统注册事件
                    system.PropertyChanged -= SystemModel_PropertyChanged;
                    system.PropertyChanged += SystemModel_PropertyChanged;

                    SaveDataFaultTree(project, system);//自动保存新建的

                    RecentFiles.AddProject(project.ProjectPath);//最近文件
                    RecentFiles.AddFaultTree(project.ProjectPath + "\\" + system.SystemName + FixedString.APP_EXTENSION);//最近文件
                    ReloadRecentFiles();
                }
                else if (node_Project != null && node_Project.Tag == null && node_Project.ParentNode != null && node_Project.ParentNode.Tag != null && node_Project.ParentNode.Tag.GetType() == typeof(ProjectModel) && !string.IsNullOrEmpty(systemName))
                {
                    ProjectModel project = node_Project.ParentNode.Tag as ProjectModel;

                    while (true)
                    {
                        if (project.Systems.Where(o => o.SystemName == systemName).Count() > 0)
                        {
                            systemName = systemName + "_1";
                        }
                        else
                        {
                            break;
                        }
                    }

                    //生成系统对象
                    SystemModel system = new SystemModel();
                    system.UpdateRenumberItem(systemName);
                    if (roots == null) system.CreateOneRootItem();
                    else system.Roots = roots;

                    if (order != null && order >= 0 && order <= project.Systems.Count)
                    {
                        project.Systems.Insert((int)order, system);
                        //添加项目节点
                        node_System = node_Project.Nodes.Add(new object[] { system.SystemName, "System" });
                        treeList_Project.MoveNode(node_System, node_Project, false, (int)order);
                    }
                    else
                    {
                        project.Systems.Add(system);
                        //添加项目节点
                        node_System = node_Project.Nodes.Add(new object[] { system.SystemName, "System" });
                    }

                    system.GroupLevel = node_Project.GetDisplayText("name");
                    node_System.Tag = system;
                    node_System.StateImageIndex = 1;
                    if (!node_Project.Expanded) node_Project.Expand();


                    //为系统注册事件
                    system.PropertyChanged -= SystemModel_PropertyChanged;
                    system.PropertyChanged += SystemModel_PropertyChanged;

                    SaveDataFaultTree(project, system);//自动保存新建的

                    RecentFiles.AddProject(project.ProjectPath);//最近文件
                    RecentFiles.AddFaultTree(project.ProjectPath + "\\" + system.SystemName + FixedString.APP_EXTENSION);//最近文件
                    ReloadRecentFiles();
                }
            }
            catch (Exception ex)
            {
                MsgBox.Show(ex.Message);
            }
            return node_System;
        }

        /// <summary>
        /// 项目管理面板里，添加系统(加到最后)
        /// </summary>
        /// <param name="node_Project">项目节点对象</param>
        /// <param name="systemName">系统名字</param>
        /// <returns>新系统节点</returns>
        private TreeListNode AddSystem(TreeListNode node_Project, string systemName)
        {
            return General.TryCatch(() =>
            {
                TreeListNode node_System = null;
                if (node_Project != null && node_Project.Tag != null && node_Project.Tag.GetType() == typeof(ProjectModel) && !string.IsNullOrEmpty(systemName))
                {
                    ProjectModel project = node_Project.Tag as ProjectModel;
                    //生成系统对象
                    SystemModel system = new SystemModel();
                    system.UpdateRenumberItem(systemName);
                    project.Systems.Add(system);
                    system.CreateOneRootItem();
                    //system.LoadDrawDataFromASPECT(repo, new )
                    //添加项目节点
                    node_System = node_Project.Nodes.Add(new object[] { system.SystemName, "System" });
                    node_System.Tag = system;
                    node_System.StateImageIndex = 1;
                    if (!node_Project.Expanded) node_Project.Expand();

                    //为系统注册事件
                    system.PropertyChanged -= SystemModel_PropertyChanged;
                    system.PropertyChanged += SystemModel_PropertyChanged;

                    SaveDataFaultTree(project, system);//自动保存新建的

                    RecentFiles.AddProject(project.ProjectPath);//最近文件
                    RecentFiles.AddFaultTree(project.ProjectPath + "\\" + system.SystemName + FixedString.APP_EXTENSION);//最近文件
                    ReloadRecentFiles();
                }
                else if (node_Project != null && node_Project.Tag == null && node_Project.ParentNode != null && node_Project.ParentNode.Tag != null && node_Project.ParentNode.Tag.GetType() == typeof(ProjectModel) && !string.IsNullOrEmpty(systemName))
                {
                    ProjectModel project = node_Project.ParentNode.Tag as ProjectModel;
                    //生成系统对象
                    SystemModel system = new SystemModel();
                    system.UpdateRenumberItem(systemName);
                    project.Systems.Add(system);
                    system.CreateOneRootItem();
                    //system.LoadDrawDataFromASPECT(repo, new )
                    //添加项目节点
                    node_System = node_Project.Nodes.Add(new object[] { system.SystemName, "System" });
                    node_System.Tag = system;
                    system.GroupLevel = node_Project.GetDisplayText("name");
                    node_System.StateImageIndex = 1;
                    if (!node_Project.Expanded) node_Project.Expand();

                    //为系统注册事件
                    system.PropertyChanged -= SystemModel_PropertyChanged;
                    system.PropertyChanged += SystemModel_PropertyChanged;

                    SaveDataFaultTree(project, system);//自动保存新建的

                    RecentFiles.AddProject(project.ProjectPath);//最近文件
                    RecentFiles.AddFaultTree(project.ProjectPath + "\\" + system.SystemName + FixedString.APP_EXTENSION);//最近文件
                    ReloadRecentFiles();
                }
                return node_System;
            });
        }

        /// <summary>
        /// 移除删除某个项目节点
        /// </summary>
        /// <param name="node_Project">项目节点对象</param>
        /// <param name="IsDelete">是否删除数据，还是移除</param>
        private void RemoveOrDeleteProject(TreeListNode node_Project, bool IsDelete = false)
        {
            General.TryCatch(() =>
            {
                if (node_Project != null && node_Project.Tag != null && node_Project.Tag.GetType() == typeof(ProjectModel))
                {
                    if (IsDelete)
                    {
                        //TODO:删除项目
                    }
                    else
                    {
                        //TODO:保存项目
                    }

                    ProjectModel project = node_Project.Tag as ProjectModel;

                    string ProjectPath = project.ProjectPath;

                    if (project.Systems != null && project.Systems.Count > 0)
                    {
                        foreach (SystemModel system in project.Systems)
                        {
                            //system.RootDictionary.Clear();
                            if (system.Roots != null)
                            {
                                //如果当前FTA表,FTA图展示的是该系统
                                if (General.FtaProgram.CurrentSystem == system)
                                {
                                    this.ftaTable.TableEvents.ClearTableControl();
                                    this.ftaDiagram.ReSetDiagram();
                                    ReSetCheckState();
                                    General.FtaProgram.SetCurrentProject(null);
                                }

                                //解绑DrawData对象，防止内存泄漏
                                foreach (DrawData data in system.Roots)
                                {
                                    data.Delete();
                                }
                            }
                        }
                        project.Systems.Clear();
                    }
                    node_Project.Remove();
                    General.FtaProgram.Projects.Remove(project);

                    //原始.json文件和项目文件夹删除 
                    string DataDir = ProjectPath;
                    if (!Directory.Exists(DataDir))
                    {
                        Directory.CreateDirectory(DataDir);
                    }
                    Directory.Delete(DataDir, true);
                }
            });
        }

        /// <summary>
        /// 移除删除某个系统节点
        /// </summary>
        /// <param name="systemNode">系统节点</param>
        /// <param name="isDelete">删除还是移除</param>
        private void RemoveOrDeleteSystem(TreeListNode systemNode, bool isDelete = false)
        {
            General.TryCatch(() =>
            {
                if (systemNode?.Tag?.GetType() == typeof(SystemModel) && systemNode.ParentNode.Tag != null && systemNode.ParentNode?.Tag?.GetType() == typeof(ProjectModel))
                {
                    var project = systemNode.ParentNode.Tag as ProjectModel;
                    var system = systemNode.Tag as SystemModel;

                    string FaultTreePath = project.ProjectPath + "\\" + system.SystemName + FixedString.APP_EXTENSION;

                    if (system.Roots != null)
                    {
                        if (General.FtaProgram.CurrentSystem == system)
                        {
                            this.ftaTable.TableEvents.ClearTableControl();
                            ftaDiagram.ReSetDiagram();
                            ReSetCheckState();
                        }

                        if (isDelete)
                        {
                            //解绑DrawData和DrawDataTransfer对象，防止内存泄漏
                            foreach (DrawData data in system.Roots)
                            {
                                data.Delete();
                            }
                            project.Systems.Remove(system);

                            //TODO:如果是数据库就要删除xxx
                        }
                    }
                    //system.RootDictionary.Clear();
                    systemNode.Remove();

                    //原始.json文件  
                    File.Delete(FaultTreePath);
                }
                else if (systemNode?.Tag?.GetType() == typeof(SystemModel) && systemNode.ParentNode.Tag == null && systemNode.ParentNode.ParentNode?.Tag != null)
                {
                    var project = systemNode.ParentNode.ParentNode.Tag as ProjectModel;
                    var system = systemNode.Tag as SystemModel;

                    string FaultTreePath = project.ProjectPath + "\\" + system.SystemName + FixedString.APP_EXTENSION;

                    if (system.Roots != null)
                    {
                        if (General.FtaProgram.CurrentSystem == system)
                        {
                            this.ftaTable.TableEvents.ClearTableControl();
                            ftaDiagram.ReSetDiagram();
                            ReSetCheckState();
                        }

                        if (isDelete)
                        {
                            //解绑DrawData和DrawDataTransfer对象，防止内存泄漏
                            foreach (DrawData data in system.Roots)
                            {
                                data.Delete();
                            }
                            project.Systems.Remove(system);

                            //TODO:如果是数据库就要删除xxx
                        }
                    }
                    //system.RootDictionary.Clear();
                    systemNode.Remove();

                    //原始.json文件  
                    File.Delete(FaultTreePath);
                }
            });
        }
        #endregion

        private void SetTreeListTableColumnState(List<HeaderInfoModel> infos)
        {
            try
            {
                General.TableControl.BeginUpdate();
                if (infos != null)
                {
                    infos.Sort((o1, o2) => o1.Index - o2.Index);
                    foreach (var item in infos)
                    {
                        if (General.TableControl.Columns[item.Name] == null)
                            continue;
                        General.TableControl.Columns[item.Name].VisibleIndex = item.Index;
                        General.TableControl.Columns[item.Name].Caption = item.Caption;
                        General.TableControl.Columns[item.Name].Visible = item.Visible;
                    }
                    this.ChangeTreetableHeaderLangage();
                }
            }
            catch (Exception ex)
            {
                MsgBox.Show(ex.Message);
            }
            finally
            {
                General.TableControl?.EndUpdate();
            }
        }

        /// <summary>
        /// 在右键菜单弹出时使某些菜单可用，某些不可用
        /// </summary>
        /// <param name="hit">鼠标位置信息对象</param>
        private void PopUpMenu_Project_EnableAndDisableMenu(TreeListHitInfo hit)
        {
            barButtonItem_GroupLevelNew.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            barButtonItem_GroupLevelRename.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            barButtonItem_GroupLevelDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            General.TryCatch(() =>
            {
                if ((hit.InRowCell || hit.InRowStateImage) && hit.Node != null)
                {
                    int NodeType = -1;
                    if (hit.Node.Tag == null)
                    {
                        NodeType = 1;
                    }
                    else if (hit.Node?.Tag?.GetType() == typeof(SystemModel))
                    {
                        NodeType = 2;
                    }
                    else if (hit.Node.Tag?.GetType() == typeof(ProjectModel))
                    {
                        NodeType = 0;
                    }
                    switch (NodeType)
                    {
                        case 0://在项目上
                            barButtonItem_Project_CreateProject.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

                            barButtonItem_Project_RenameProject.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                            barButtonItem_Project_RemoveProject.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                            barButtonItem_Project_DeleteProject.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

                            barButtonItem_Project_CreateSystem.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                            barButtonItem_Project_RenameSystem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                            barButtonItem_Project_DeleteSystem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                            barButtonItem_GroupLevelNew.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                            barButtonItem_GroupLevelRename.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                            barButtonItem_GroupLevelDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                            barButtonItem_OpenDir.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                            barButtonItem_CloseDir.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

                            barButtonItem_FaultTreeCopy.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                            barButtonItem_FaultTreePaste.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                            break;
                        case 1://在分组上
                            barButtonItem_Project_CreateProject.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                            barButtonItem_Project_RenameProject.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                            barButtonItem_Project_RemoveProject.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                            barButtonItem_Project_DeleteProject.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                            barButtonItem_Project_CreateSystem.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                            barButtonItem_Project_RenameSystem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                            barButtonItem_Project_DeleteSystem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                            barButtonItem_GroupLevelNew.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                            barButtonItem_GroupLevelRename.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                            barButtonItem_GroupLevelDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

                            barButtonItem_OpenDir.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                            barButtonItem_CloseDir.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                            barButtonItem_FaultTreeCopy.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                            barButtonItem_FaultTreePaste.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                            break;
                        case 2://在系统上
                            barButtonItem_Project_CreateProject.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                            barButtonItem_Project_RenameProject.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                            barButtonItem_Project_RemoveProject.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                            barButtonItem_Project_DeleteProject.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                            barButtonItem_Project_CreateSystem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                            barButtonItem_Project_RenameSystem.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                            barButtonItem_Project_DeleteSystem.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

                            barButtonItem_GroupLevelNew.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                            barButtonItem_GroupLevelRename.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                            barButtonItem_GroupLevelDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                            barButtonItem_OpenDir.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                            barButtonItem_CloseDir.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

                            barButtonItem_FaultTreeCopy.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                            barButtonItem_FaultTreePaste.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                            break;
                        default:
                            barButtonItem_Project_RenameProject.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                            barButtonItem_Project_RemoveProject.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                            barButtonItem_Project_DeleteProject.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                            barButtonItem_Project_CreateSystem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                            barButtonItem_Project_RenameSystem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                            barButtonItem_Project_DeleteSystem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                            barButtonItem_Project_CreateProject.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                            barButtonItem_OpenDir.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                            barButtonItem_CloseDir.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                            barButtonItem_GroupLevelNew.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                            barButtonItem_GroupLevelRename.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                            barButtonItem_GroupLevelDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                            barButtonItem_FaultTreeCopy.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                            barButtonItem_FaultTreePaste.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                            break;
                    }
                }
                else
                {
                    barButtonItem_Project_RenameProject.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    barButtonItem_Project_RemoveProject.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    barButtonItem_Project_DeleteProject.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    barButtonItem_Project_CreateSystem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    barButtonItem_Project_RenameSystem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    barButtonItem_Project_DeleteSystem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    barButtonItem_Project_CreateProject.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    barButtonItem_OpenDir.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    barButtonItem_CloseDir.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    barButtonItem_GroupLevelNew.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    barButtonItem_GroupLevelRename.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    barButtonItem_GroupLevelDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    barButtonItem_FaultTreeCopy.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    barButtonItem_FaultTreePaste.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                }
            });
        }

        /// <summary>
        /// 对name字段按名称进行排序，为了保证分组在上面，分组名称前面加上特殊字符后再做排序计算
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

        #region Project右键菜单事件注册以及操作
        /// <summary>
        /// project面板里右键菜单重命名的实现
        /// </summary>
        private void RegisterEvents()
        {
            //每次编辑完毕先检查新的名字
            General.ProjectControl.ValidatingEditor += ValidatingEditor;

            //然后关闭编辑模式
            General.ProjectControl.HiddenEditor += HiddenEditor;

            //把修改后的值更新回维护的对象属性
            General.ProjectControl.CellValueChanged += CellValueChanged;

            //自动排序
            General.ProjectControl.CustomColumnSort += treeList_Project_CustomColumnSort;

            //设置左键单击节点时加载故障树表和图,右键单击时弹出菜单
            General.ProjectControl.MouseDown += ProjectMouseDown;

            //设置焦点改变状态切换
            General.ProjectControl.FocusedNodeChanged += ProjectFocusedNodeChanged;

            //新建工程
            barButtonItem_Project_CreateProject.ItemClick += ProjectItemClick;

            //重命名工程
            barButtonItem_Project_RenameProject.ItemClick += ProjectItemClick;

            //移除工程
            barButtonItem_Project_RemoveProject.ItemClick += ProjectItemClick;

            //删除工程
            barButtonItem_Project_DeleteProject.ItemClick += ProjectItemClick;

            //新建文件夹
            barButtonItem_GroupLevelNew.ItemClick += ProjectItemClick;

            //重命名文件夹
            barButtonItem_GroupLevelRename.ItemClick += ProjectItemClick;

            //删除文件夹
            barButtonItem_GroupLevelDelete.ItemClick += ProjectItemClick;

            //新建系统
            barButtonItem_Project_CreateSystem.ItemClick += ProjectItemClick;

            //重命名系统
            barButtonItem_Project_RenameSystem.ItemClick += ProjectItemClick;

            //删除系统
            barButtonItem_Project_DeleteSystem.ItemClick += ProjectItemClick;

            //打开目录
            barButtonItem_OpenDir.ItemClick += ProjectItemClick;

            //关闭工程
            barButtonItem_CloseDir.ItemClick += ProjectItemClick;

            //复制故障树
            barButtonItem_FaultTreeCopy.ItemClick += ProjectItemClick;

            //粘贴故障树
            barButtonItem_FaultTreePaste.ItemClick += ProjectItemClick;
        }

        /// <summary>
        /// Project面板里的右键菜单事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProjectItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            General.TryCatch(() =>
            {
                if (dockPanel_Project.Visibility == DockVisibility.Hidden)
                {
                    //重新加载布局，工程列表，FTA表，FTA图，三部分面板全部显示，支持中英文切换
                    if (General.FtaProgram.Setting.Language == FixedString.LANGUAGE_CN_CN)
                    {
                        this.dockManager_FTA.RestoreLayoutFromXml(Application.StartupPath + "\\FaultTreeStartPage_CN.xml");
                    }
                    else
                    {
                        this.dockManager_FTA.RestoreLayoutFromXml(Application.StartupPath + "\\FaultTreeStartPage_EN.xml");
                    }
                }

                if (treeList_Project.FocusedNode != null)
                {
                    ProjectModel project = null;
                    if (treeList_Project.FocusedNode.Tag != null && treeList_Project.FocusedNode.Tag.GetType() == typeof(ProjectModel))
                    {
                        project = treeList_Project.FocusedNode.Tag as ProjectModel;
                    }
                    else if (treeList_Project.FocusedNode.Tag == null && treeList_Project.FocusedNode.ParentNode != null && treeList_Project.FocusedNode.ParentNode.Tag.GetType() == typeof(ProjectModel))
                    {
                        project = treeList_Project.FocusedNode.ParentNode.Tag as ProjectModel;
                    }
                    else if (treeList_Project.FocusedNode.Tag != null && treeList_Project.FocusedNode.Tag.GetType() == typeof(SystemModel))
                    {
                        if (treeList_Project.FocusedNode.ParentNode.Tag != null && treeList_Project.FocusedNode.ParentNode.Tag.GetType() == typeof(ProjectModel))
                        {
                            project = treeList_Project.FocusedNode.ParentNode.Tag as ProjectModel;
                        }
                        else
                        {
                            project = treeList_Project.FocusedNode.ParentNode.ParentNode.Tag as ProjectModel;
                        }
                    }

                    if (project.ProjectPath == Application.StartupPath + "\\Example")
                    {
                        if (e.Item != barButtonItem_OpenDir && e.Item != barButtonItem_CloseDir && e.Item != barButtonItem_FaultTreeCopy)
                        {
                            MsgBox.Show(General.FtaProgram.String.ExampleNoDo);
                            return;
                        }
                    }
                }

                //新建工程
                if (e.Item == barButtonItem_Project_CreateProject || e.Item == barButtonItem_CreateNew_Project)
                {
                    FolderBrowserDialog dialog = new FolderBrowserDialog();

                    if (General.FtaProgram.Setting.DefaultFilePath != "" && Directory.Exists(General.FtaProgram.Setting.DefaultFilePath))
                    {
                        dialog.SelectedPath = General.FtaProgram.Setting.DefaultFilePath;
                    }

                    DialogResult result = dialog.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        List<string> projectNames = new List<string>();
                        foreach (ProjectModel project in General.FtaProgram.Projects)
                        {
                            projectNames.Add(project.ProjectName);
                        }
                        XtraFormNewProject dlg_Project = new XtraFormNewProject(projectNames, General.FtaProgram.String);

                        if (dlg_Project.ShowDialog() == DialogResult.Cancel)
                            return;
                        string name = dlg_Project.GetProjectName();
                        dlg_Project.Dispose();

                        TreeListNode FNode = AddProject(name, dialog.SelectedPath);
                        this.ActivateLastSystem(FNode);
                    }
                }
                //重命名工程
                else if (e.Item == barButtonItem_Project_RenameProject)
                {
                    if (treeList_Project.FocusedNode != null && treeList_Project.FocusedNode.Tag != null && treeList_Project.FocusedNode.Tag.GetType() == typeof(ProjectModel))
                    {
                        EnterEditor();
                    }
                }
                //移除工程
                else if (e.Item == barButtonItem_Project_RemoveProject)
                {
                    if (treeList_Project.FocusedNode != null && treeList_Project.FocusedNode.Tag != null && treeList_Project.FocusedNode.Tag.GetType() == typeof(ProjectModel))
                    {
                        if (MsgBox.Show(General.FtaProgram.String.ConfirmDeletionMessage, General.FtaProgram.String.MessageBoxCaption, MessageBoxButtons.OKCancel) == DialogResult.OK) RemoveOrDeleteProject(treeList_Project.FocusedNode);
                    }
                }
                //删除工程
                else if (e.Item == barButtonItem_Project_DeleteProject)
                {
                    if (treeList_Project.FocusedNode != null && treeList_Project.FocusedNode.Tag != null && treeList_Project.FocusedNode.Tag.GetType() == typeof(ProjectModel))
                    {
                        if (MsgBox.Show(General.FtaProgram.String.ConfirmDeletionMessage, General.FtaProgram.String.MessageBoxCaption, MessageBoxButtons.OKCancel) == DialogResult.OK) RemoveOrDeleteProject(treeList_Project.FocusedNode, true);
                    }
                }
                //新建系统
                else if (e.Item == barButtonItem_Project_CreateSystem || e.Item == barButtonItem_CreateNew_System || e.Item == barButtonItem_ToolNew)
                {
                    if (treeList_Project.FocusedNode != null)
                    {
                        ProjectModel project = null;
                        if (treeList_Project.FocusedNode.Tag != null && treeList_Project.FocusedNode.Tag.GetType() == typeof(ProjectModel))
                        {
                            project = treeList_Project.FocusedNode.Tag as ProjectModel;
                        }
                        else if (treeList_Project.FocusedNode.Tag == null && treeList_Project.FocusedNode.ParentNode != null && treeList_Project.FocusedNode.ParentNode.Tag.GetType() == typeof(ProjectModel))
                        {
                            project = treeList_Project.FocusedNode.ParentNode.Tag as ProjectModel;
                        }

                        List<string> systemNames = new List<string>();
                        foreach (SystemModel system in project.Systems)
                        {
                            systemNames.Add(system.SystemName);
                        }
                        XtraFormNewSystem dlg_System = new XtraFormNewSystem(systemNames, General.FtaProgram.String);
                        if (dlg_System.ShowDialog() == DialogResult.Cancel)
                            return;
                        string name = dlg_System.GetSystemName();
                        dlg_System.Dispose();
                        TreeListNode sys = AddSystem(treeList_Project.FocusedNode, name);

                        this.ActivateLastSystem(sys);
                    }
                }
                //重命名系统
                else if (e.Item == barButtonItem_Project_RenameSystem)
                {
                    if (treeList_Project.FocusedNode != null && treeList_Project.FocusedNode.Tag != null && treeList_Project.FocusedNode.Tag.GetType() == typeof(SystemModel))
                    {
                        EnterEditor();
                    }
                }
                //移除系统
                else if (e.Item == barButtonItem_Project_RemoveSystem)
                {
                    if (treeList_Project.FocusedNode != null && treeList_Project.FocusedNode.Tag != null && treeList_Project.FocusedNode.Tag.GetType() == typeof(SystemModel))
                    {
                        RemoveOrDeleteSystem(treeList_Project.FocusedNode);
                    }
                }
                //删除系统
                else if (e.Item == barButtonItem_Project_DeleteSystem)
                {
                    if (MsgBox.Show(General.FtaProgram.String.ConfirmDeletionMessage, General.FtaProgram.String.MessageBoxCaption, MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        if (treeList_Project.FocusedNode != null && treeList_Project.FocusedNode.Tag != null && treeList_Project.FocusedNode.Tag.GetType() == typeof(SystemModel))
                        {
                            RemoveOrDeleteSystem(treeList_Project.FocusedNode, true);
                        }
                    }
                }
                //打开目录
                else if (e.Item == barButtonItem_OpenDir)
                {
                    if (treeList_Project.FocusedNode != null && treeList_Project.FocusedNode.Tag != null)
                    {
                        if (treeList_Project.FocusedNode.Tag.GetType() == typeof(ProjectModel))
                        {
                            ProjectModel project = treeList_Project.FocusedNode.Tag as ProjectModel;
                            Process.Start(project.ProjectPath);
                        }
                        else if (treeList_Project.FocusedNode.Tag.GetType() == typeof(SystemModel))
                        {
                            if (treeList_Project.FocusedNode.ParentNode.Tag != null)
                            {
                                ProjectModel project = treeList_Project.FocusedNode.ParentNode.Tag as ProjectModel;
                                Process.Start(project.ProjectPath);
                            }
                            else
                            {
                                ProjectModel project = treeList_Project.FocusedNode.ParentNode.ParentNode.Tag as ProjectModel;
                                Process.Start(project.ProjectPath);
                            }
                        }
                    }
                }
                //关闭工程
                else if (e.Item == barButtonItem_CloseDir)
                {
                    if (treeList_Project.FocusedNode != null && treeList_Project.FocusedNode.Tag != null)
                    {
                        if (HaveVirtualizationSystem())
                        {
                            if (MsgBox.Show(General.FtaProgram.String.ConfirmSavingMessage, General.FtaProgram.String.MessageBoxCaption, MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                SaveData();
                            }
                        }

                        if (treeList_Project.FocusedNode.Tag.GetType() == typeof(ProjectModel))
                        {
                            ProjectModel project = treeList_Project.FocusedNode.Tag as ProjectModel;
                            General.FtaProgram.Projects.Remove(project);
                            treeList_Project.FocusedNode.Remove();
                        }
                        else if (treeList_Project.FocusedNode.Tag.GetType() == typeof(SystemModel))
                        {
                            if (treeList_Project.FocusedNode.ParentNode.Tag != null)
                            {
                                ProjectModel project = treeList_Project.FocusedNode.ParentNode.Tag as ProjectModel;
                                General.FtaProgram.Projects.Remove(project);
                                treeList_Project.FocusedNode.ParentNode.Remove();
                            }
                            else
                            {
                                ProjectModel project = treeList_Project.FocusedNode.ParentNode.ParentNode.Tag as ProjectModel;
                                General.FtaProgram.Projects.Remove(project);
                                treeList_Project.FocusedNode.ParentNode.ParentNode.Remove();
                            }
                        }
                    }
                }
                //新建分组
                else if (e.Item == barButtonItem_GroupLevelNew)
                {
                    if (treeList_Project.FocusedNode != null && treeList_Project.FocusedNode.Tag != null && treeList_Project.FocusedNode.Tag.GetType() == typeof(ProjectModel))
                    {
                        List<string> groupNames = new List<string>();
                        foreach (TreeListNode nd in treeList_Project.FocusedNode.Nodes)
                        {
                            if (nd.Tag == null)
                            {
                                groupNames.Add(nd.GetDisplayText("name"));
                            }
                        }
                        XtraFormNewFolder dlg_Group = new XtraFormNewFolder(groupNames, General.FtaProgram.String);
                        if (dlg_Group.ShowDialog() == DialogResult.Cancel)
                            return;
                        string name = dlg_Group.GetGroupName();

                        TreeListNode node_GroupLevel = treeList_Project.FocusedNode.Nodes.Add(new object[] { name, "Group" });
                        node_GroupLevel.Tag = null;
                        node_GroupLevel.StateImageIndex = 3;
                        this.ActivateLastSystem(node_GroupLevel);
                    }
                }
                //重命名分组
                else if (e.Item == barButtonItem_GroupLevelRename)
                {
                    if (treeList_Project.FocusedNode != null && treeList_Project.FocusedNode.Tag == null)
                    {
                        EnterEditor();
                    }
                }
                //删除分组
                else if (e.Item == barButtonItem_GroupLevelDelete)
                {
                    if (treeList_Project.FocusedNode != null && treeList_Project.FocusedNode.Tag == null)
                    {
                        if (MsgBox.Show(General.FtaProgram.String.ConfirmDeletionMessage, General.FtaProgram.String.MessageBoxCaption, MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            List<TreeListNode> nds = treeList_Project.FocusedNode.Nodes.ToList();
                            foreach (TreeListNode nd in nds)
                            {
                                RemoveOrDeleteSystem(nd, true);
                            }

                            TreeListNode ndP = treeList_Project.FocusedNode;
                            treeList_Project.FocusedNode = treeList_Project.FocusedNode.ParentNode;
                            treeList_Project.FocusedNode.Nodes.Remove(ndP);
                        }
                    }
                }
                //复制故障树
                else if (e.Item == barButtonItem_FaultTreeCopy)
                {
                    if (treeList_Project.FocusedNode != null && treeList_Project.FocusedNode.Tag != null && treeList_Project.FocusedNode.Tag.GetType() == typeof(SystemModel))
                    {
                        CopySys.Clear();
                        CopySys.Add((SystemModel)(treeList_Project.FocusedNode.Tag));
                        barButtonItem_FaultTreePaste.Enabled = true;
                    }
                }
                //粘贴故障树
                else if (e.Item == barButtonItem_FaultTreePaste)
                {
                    if (CopySys != null && CopySys.Count > 0)
                    {
                        TreeListNode LastNode = null;
                        foreach (SystemModel CSys in CopySys)
                        {
                            if (treeList_Project.FocusedNode != null)
                            {
                                ProjectModel project = null;
                                if (treeList_Project.FocusedNode.Tag != null && treeList_Project.FocusedNode.Tag.GetType() == typeof(ProjectModel))
                                {
                                    project = treeList_Project.FocusedNode.Tag as ProjectModel;
                                }
                                else if (treeList_Project.FocusedNode.Tag == null && treeList_Project.FocusedNode.ParentNode != null && treeList_Project.FocusedNode.ParentNode.Tag.GetType() == typeof(ProjectModel))
                                {
                                    project = treeList_Project.FocusedNode.ParentNode.Tag as ProjectModel;
                                }

                                List<string> systemNames = new List<string>();
                                foreach (SystemModel system in project.Systems)
                                {
                                    systemNames.Add(system.SystemName);
                                }

                                string sysname = CSys.SystemName;
                                while (true)
                                {
                                    if (systemNames != null && systemNames.Contains(sysname))
                                    {
                                        sysname = sysname + "_1";
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                TreeListNode sys = AddSystem(treeList_Project.FocusedNode, sysname);
                                ((SystemModel)sys.Tag).Roots.Clear();
                                ((SystemModel)sys.Tag).AddRoots(CSys);
                                LastNode = sys;
                            }
                        }

                        if (LastNode != null)
                        {
                            this.ActivateLastSystem(LastNode);
                        }

                        SaveData();
                    }
                }
            });
        }
        #endregion

        #region 用于project面板树里让选中的单元格开始编辑模式(用于重命名功能)
        /// <summary>
        /// 用于project面板树里让选中的单元格开始编辑模式(用于重命名功能)
        /// </summary>
        private void EnterEditor()
        {
            General.TryCatch(() =>
            {
                if (treeList_Project.Columns[0] != null)
                {
                    treeList_Project.Columns[0].OptionsColumn.AllowEdit = true;
                    treeList_Project.ShowEditor();
                }
            });
        }

        /// <summary>
        /// 每次编辑完毕先检查新的名字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            General.TryCatch(() =>
            {
                if (string.IsNullOrEmpty(e.Value as string) || string.IsNullOrEmpty(((string)e.Value).Trim()))
                {
                    e.ErrorText = General.FtaProgram.String.ProjectNameCannotBeEmpty;
                    e.Valid = false;
                    return;
                }
                else
                {
                    foreach (var val in e.Value.ToString())
                    {
                        if ("(，,。.？?！!：:;；*['\"@#$%/^&~\\])`=+-{}/|<>".IndexOf(val) > 0)
                        {
                            e.ErrorText = General.FtaProgram.String.AllNameCheck;
                            e.Valid = false;
                            return;
                        }
                    }
                    if (treeList_Project.FocusedNode != null && treeList_Project.FocusedNode.Tag != null)
                    {
                        if (treeList_Project.FocusedNode.Tag.GetType() == typeof(ProjectModel))
                        {
                            List<string> projectNames = new List<string>();
                            foreach (ProjectModel project in General.FtaProgram.Projects)
                            {
                                if (project == treeList_Project.FocusedNode.Tag) continue;
                                projectNames.Add(project.ProjectName);
                            }
                            if (projectNames.Contains(e.Value))
                            {
                                e.ErrorText = General.FtaProgram.String.Projectnamealreadyexists;
                                e.Valid = false;
                                return;
                            }

                            string PM_DirectoryDest = new DirectoryInfo(((ProjectModel)treeList_Project.FocusedNode.Tag).ProjectPath).Parent.FullName + "\\" + e.Value.ToString();
                            if (Directory.Exists(PM_DirectoryDest) == true)
                            {
                                e.ErrorText = General.FtaProgram.String.ExistProject;
                                e.Valid = false;
                                return;
                            }
                        }
                        else if (treeList_Project.FocusedNode.Tag.GetType() == typeof(SystemModel))
                        {
                            ProjectModel project = null;
                            if (treeList_Project.FocusedNode.ParentNode.Tag == null)
                            {
                                project = treeList_Project.FocusedNode.ParentNode.ParentNode.Tag as ProjectModel;
                            }
                            else
                            {
                                project = treeList_Project.FocusedNode.ParentNode.Tag as ProjectModel;
                            }

                            List<string> systemNames = new List<string>();
                            foreach (SystemModel system in project.Systems)
                            {
                                if (system == treeList_Project.FocusedNode.Tag) continue;
                                systemNames.Add(system.SystemName);
                            }
                            if (systemNames.Contains(e.Value))
                            {
                                e.ErrorText = General.FtaProgram.String.Systemnamealreadyexists;
                                e.Valid = false;
                                return;
                            }

                            string PM_DirectoryDest = project.ProjectPath + "\\" + e.Value.ToString();
                            if (File.Exists(PM_DirectoryDest) == true)
                            {
                                e.ErrorText = General.FtaProgram.String.ExistProject;
                                e.Valid = false;
                                return;
                            }
                        }
                    }
                    else if (treeList_Project.FocusedNode != null && treeList_Project.FocusedNode.Tag == null)
                    {
                        if (treeList_Project.FocusedNode.ParentNode.Tag.GetType() == typeof(ProjectModel))
                        {
                            List<string> projectNames = new List<string>();
                            foreach (TreeListNode nd in treeList_Project.FocusedNode.ParentNode.Nodes)
                            {
                                projectNames.Add(nd.GetDisplayText("name"));
                            }

                            if (projectNames.Contains(e.Value))
                            {
                                e.ErrorText = General.FtaProgram.String.Projectnamealreadyexists;
                                e.Valid = false;
                                return;
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 然后关闭编辑模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HiddenEditor(object sender, EventArgs e)
        {
            General.TryCatch(() =>
            {
                if (treeList_Project.Columns[0] != null)
                {
                    if (treeList_Project.Columns[0].OptionsColumn.AllowEdit) treeList_Project.Columns[0].OptionsColumn.AllowEdit = false;
                }
            });
        }

        /// <summary>
        /// 把修改后的值更新回维护的对象属性
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            General.TryCatch(() =>
            {
                if (e.Node != null && e.Node.Tag != null && e.Value != null && e.Value.GetType() == typeof(string))
                {
                    if (e.Node.Tag.GetType() == typeof(ProjectModel))
                    {
                        string PreName = ((ProjectModel)(e.Node.Tag)).ProjectName;
                        string PrePath = ((ProjectModel)(e.Node.Tag)).ProjectPath;

                        ((ProjectModel)(e.Node.Tag)).ProjectName = (string)e.Value;

                        DirectoryInfo DirInfo = new DirectoryInfo(PrePath);

                        string PM_DirectorySource = DirInfo.FullName;
                        string PM_DirectoryDest = DirInfo.Parent.FullName + "\\" + ((ProjectModel)(e.Node.Tag)).ProjectName;

                        Directory.Move(PM_DirectorySource, PM_DirectoryDest);
                        ((ProjectModel)(e.Node.Tag)).ProjectPath = PM_DirectoryDest;
                    }
                    else if (e.Node.Tag.GetType() == typeof(SystemModel))
                    {
                        string PreName = ((SystemModel)(e.Node.Tag)).SystemName;

                        string PrePath = "";
                        if (e.Node.ParentNode.Tag == null)
                        {
                            PrePath = ((ProjectModel)(e.Node.ParentNode.ParentNode.Tag)).ProjectPath;
                        }
                        else
                        {
                            PrePath = ((ProjectModel)(e.Node.ParentNode.Tag)).ProjectPath;
                        }

                        ((SystemModel)(e.Node.Tag)).SystemName = (string)e.Value;

                        string sys_Source = PrePath + "\\" + PreName + FixedString.APP_EXTENSION;
                        string sys_Dest = PrePath + "\\" + ((SystemModel)(e.Node.Tag)).SystemName + FixedString.APP_EXTENSION;

                        File.Move(sys_Source, sys_Dest);
                        File.WriteAllText(sys_Dest, Newtonsoft.Json.JsonConvert.SerializeObject(((SystemModel)(e.Node.Tag))), Encoding.UTF8);

                        RecentFiles.AddFaultTree(sys_Dest);//最近文件
                    }
                }
                else if (e.Node != null && e.Node.Tag == null)
                {
                    foreach (TreeListNode nd in e.Node.Nodes)
                    {
                        ((SystemModel)(nd.Tag)).GroupLevel = e.Value.ToString();
                        SaveData();
                    }
                }

                treeList_Project.Refresh();
            });
        }
        #endregion

        private void ActivateLastSystem(TreeListNode FNode)
        {
            if (FNode == null)
            {
                return;
            }

            if (General.FtaProgram.CurrentProject == null || FNode.GetValue("SortType").ToString() == "Project")//新建工程时定位到最后一个工程节点
            {
                General.FtaProgram.SetCurrentProject(General.FtaProgram.Projects[General.FtaProgram.Projects.Count - 1]);
                //重置设置项状态和值
                ReSetFTAStyle();
                this.treeList_Project.FocusedNode = FNode;
                return;
            }

            if (FNode.Tag != null && FNode.Tag.GetType() == typeof(SystemModel))
            {
                var sys = FNode.Tag as SystemModel;
                if (sys.TranferGates == null || sys.RepeatedEvents == null)
                {
                    sys.UpdateRepeatedAndTranfer();
                }
                this.LoadDrawDataToDiagram(sys);
                this.treeList_Project.FocusedNode = FNode;
                this.ftaTable.TableEvents.LoadDataToTableControl(sys);
                General.DiagramControl.Refresh();
                this.treeList_FTATable.Nodes.FirstNode.ExpandAll();
                ReSetFTAStyleWidthHeight();

                General.DiagramControl.ScrollToPoint(new DevExpress.Utils.PointFloat(sys.Roots[0].X, sys.Roots[0].Y));
                General.FtaProgram.CurrentProject.Style.ShapeWidth = sys.ShapeWidth;
                General.FtaProgram.CurrentProject.Style.ShapeDescriptionRectHeight = sys.ShapeDescriptionRectHeight;
                General.FtaProgram.CurrentProject.Style.ShapeIdRectHeight = sys.ShapeIdRectHeight;
                General.FtaProgram.CurrentProject.Style.ShapeSymbolRectHeight = sys.ShapeSymbolRectHeight;
            }
            else if (FNode.Tag != null && FNode.Tag.GetType() == typeof(ProjectModel))
            {
                this.treeList_Project.FocusedNode = FNode;
                ProjectModel newProj = FNode.Tag as ProjectModel;
                //切换了项目
                if (newProj != General.FtaProgram.CurrentProject)
                {
                    General.FtaProgram.SetCurrentProject(newProj);

                }
                //重置设置项状态和值
                ReSetFTAStyle();
                this.ftaTable.TableEvents.ClearTableControl();
                this.ftaDiagram.ReSetDiagram();
                ReSetCheckState();
            }
            else
            {
                this.treeList_Project.FocusedNode = FNode;
                //重置设置项状态和值
                ReSetFTAStyle();
                this.ftaTable.TableEvents.ClearTableControl();
                this.ftaDiagram.ReSetDiagram();
                ReSetCheckState();
            }
        }


        /// <summary>
        /// 系统模型属性改变事件
        /// </summary>
        private void SystemModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SystemModel oSystemModel = sender as SystemModel;
            if (null == oSystemModel)
            {
                throw new ArgumentNullException("Parameter sender is null.");
            }
            ExPropertyChangedEventsArgs oEventArgs = e as ExPropertyChangedEventsArgs;
            if (null == oEventArgs)
            {
                throw new ArgumentNullException("Parameter e is null.");
            }

            //查找系统节点
            TreeListNode oSystemModelNode = this.treeList_Project.FindNode((o) =>
            {
                if (null != (o.Tag as SystemModel))
                {
                    return ((SystemModel)o.Tag)?.SystemName == oSystemModel.SystemName;
                }
                return false;
            });
            if (null != oSystemModelNode)
            {
                //不置虚拟化名
                if (Behavior.Enum.PropertyStatus.Applied == oEventArgs.Status)
                {
                    oSystemModelNode.StateImageIndex = 0x01;
                }
                //设置虚拟化名
                if (Behavior.Enum.PropertyStatus.NoApply == oEventArgs.Status)
                {
                    oSystemModelNode.StateImageIndex = 0x02;
                }
            }
        }
        /// <summary>
        /// 是否有虚拟化系统项目
        /// </summary>
        /// <returns></returns>
        internal bool HaveVirtualizationSystem()
        {
            foreach (ProjectModel itemProjects in General.FtaProgram?.Projects)
            {
                foreach (SystemModel itemSystem in itemProjects?.Systems)
                {
                    if ((bool)itemSystem?.Virtualized)
                    {
                        //有虚拟化系统项目
                        return true;
                    }
                }
            }

            //没有虚拟化系统项目
            return false;
        }
    }
}
