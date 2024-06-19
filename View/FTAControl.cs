using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraDiagram;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using FaultTreeAnalysis.Common;
using FaultTreeAnalysis.Model.Enum;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using DevExpress.Diagram.Core;
using FaultTreeAnalysis.View.Diagram;
using FaultTreeAnalysis.Model.Data;
using DevExpress.XtraEditors.Repository;
using FaultTreeAnalysis.View.Table;
using FaultTreeAnalysis.Properties;
using FaultTreeAnalysis.View.Ribbon;
using System.Diagnostics;
using DevExpress.XtraBars.Docking;
using System.Data;
using FaultTreeAnalysis.View.Ribbon.Start;
using FaultTreeAnalysis.FTAControlEventHandle.FTADiagram;
using System.Threading;
using IntegratedSystem.View.TestFunction;
using static Aspose.Pdf.Operator;
using Newtonsoft.Json.Linq;
using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Xpo.DB;
using Aspose.Pdf;
using FaultTreeAnalysis.View.Ribbon.Start.Excel;

namespace FaultTreeAnalysis
{
    /// <summary>
    /// 最大的自定义控件类对象，开发的目标控件
    /// </summary>
    public partial class FTAControl : XtraUserControl
    {
        /// <summary>
        /// 保存数据到本地文本的路径(工程文件保存路径)
        /// </summary> 
        private readonly string savingDataPath = $"{AppDomain.CurrentDomain.BaseDirectory}{Properties.Resources.SavingDataPath}";

        /// <summary>
        /// FTA图和表插入的转移门的数据对象
        /// </summary>
        private DrawData transfer;

        /// <summary>
        /// 系统数据对象
        /// </summary>
        private FtaDiagram ftaDiagram;

        /// <summary>
        /// FTA表数据对象
        /// </summary>
        private FtaTable ftaTable;

        /// <summary>
        /// 主菜单对象
        /// </summary>
        private FtaRibbon ftaRibbon;

        /// <summary>
        /// 计算等数据处理类
        /// </summary>
        private CCAFunction ccaFunction = new CCAFunction();

        /// <summary>
        /// 开始页
        /// </summary>
        public User_StartPage StartPage = new User_StartPage();

        /// <summary>
        /// 复制的故障树对象集合
        /// </summary>
        public List<SystemModel> CopySys = new List<SystemModel>();

        /// <summary>
        /// 窗体构造函数
        /// </summary>
        public FTAControl()
        {
            //默认设置保存路径 
            savingDataPath = $"{AppDomain.CurrentDomain.BaseDirectory}{Properties.Resources.SavingDataPath}";

            General.TryCatch(() =>
            {
                //加载设置（不存在自动创建默认设置）
                FileInfo file = new FileInfo(savingDataPath);
                if (!file.Directory.Exists) file.Directory.Create();

                if (File.Exists(this.savingDataPath) == false)
                {
                    ProgramModel PM = new ProgramModel();
                    File.WriteAllText(this.savingDataPath, Newtonsoft.Json.JsonConvert.SerializeObject(PM), Encoding.UTF8);
                }
                string Datas = File.ReadAllText(this.savingDataPath, Encoding.UTF8);
                if (General.FtaProgram == null)
                {
                    General.FtaProgram = Newtonsoft.Json.JsonConvert.DeserializeObject<ProgramModel>(Datas);
                }

                //语言文本对象，应该一开始初始化，后面可能有人用到，保存时不保存固定语言
                General.FtaProgram.String = StringModel.GetENFTAString();
                //由设置信息初始化软件
                SettingModel setting_Default = new SettingModel(true);
                if (General.FtaProgram.Setting == null) General.FtaProgram.Setting = setting_Default;
                else
                {
                    #region 如果发现由设置没有的（一般是开发过程中新加的设置项,后期项目扩展，用户之前的工程要能打开）
                    foreach (PropertyInfo info in typeof(SettingModel).GetProperties())
                    {
                        var obj = info.GetValue(General.FtaProgram.Setting);
                        if (obj == null) info.SetValue(General.FtaProgram.Setting, info.GetValue(setting_Default));
                    }
                    #endregion
                }

                //设置默认皮肤
                UserLookAndFeel.Default.SkinName = General.FtaProgram.Setting.Skin_Name;
                SplashScreenManager splashScreenManager_FTA = new SplashScreenManager(this, typeof(SplashScreen_Start), true, true, typeof(System.Windows.Forms.UserControl));
                splashScreenManager_FTA.ClosingDelay = 500;
                //注册皮肤
                DevExpress.UserSkins.BonusSkins.Register();
                DevExpress.Skins.SkinManager.EnableFormSkins();

                //启动画面动态追踪
                General.FtaProgram.String.SplashScreenCommand_Initial1 = "正在初始化配置";
                General.FtaProgram.String.SplashScreenCommand_Initial2 = "正在初始化界面";
                General.FtaProgram.String.SplashScreenCommand_Initial3 = "正在初始化开始页";

                splashScreenManager_FTA.SendCommand(SplashScreen_Start.SplashScreenCommand.CHANGEINFO, General.FtaProgram.String.SplashScreenCommand_Initial1);

                Thread.Sleep(200);

                splashScreenManager_FTA.SendCommand(SplashScreen_Start.SplashScreenCommand.CHANGEINFO, General.FtaProgram.String.SplashScreenCommand_Initial2);

                //初始化控件
                InitializeComponent();

                splashScreenManager_FTA.SendCommand(SplashScreen_Start.SplashScreenCommand.CHANGEINFO, General.FtaProgram.String.SplashScreenCommand_Initial3);

                //初始化开始页
                StartPage = new User_StartPage();
                StartPage.ReloadRecentFiles();
                General.StartPage = StartPage;

                //切换语言和设置
                General.DiagramControl = this.currentDiagram;
                ChangeFTASettting(General.FtaProgram.Setting);
                ChangeLanguage(General.FtaProgram.Setting.Language);

                //加载历史记录
                ReloadRecentFiles();

                //基本事件库
                if (File.Exists(System.Environment.CurrentDirectory + "\\BasicEvents.db"))
                {
                    ConnectSever.Init(System.Environment.CurrentDirectory + "\\BasicEvents.db");

                    //开启接收线程，读取数据
                    General.EventsLibDB = ConnectSever.ReadInitial("BasicEvents", " ORDER BY ID");
                    General.EventsLibDB.TableName = "BasicEvents";
                }

                splashScreenManager_FTA.CloseWaitForm();
                splashScreenManager_FTA.WaitForSplashFormClose();
            });
        }

        /// <summary>
        /// 加载最近故障树文件
        /// </summary>
        public void ReloadRecentFiles()
        {
            //获取历史记录集合
            this.barSubItem_OpenRecentFiles.ItemLinks.Clear();
            RecentModel RM = RecentFiles.GetRecentModel();

            //最近打开的故障树文件集合
            if (RM.RecentFaultTree != null)
            {
                //对集合，按时间进行倒序排列
                var result = from pair in RM.RecentFaultTree orderby pair.Key descending select pair;

                //自动生成最近文件Button
                foreach (KeyValuePair<string, string> pair in result)
                {
                    BarButtonItem bar = new BarButtonItem();
                    bar.ImageOptions.LargeImage = Properties.Resources.documentmap_32x32;
                    bar.RibbonStyle = RibbonItemStyles.Large;

                    //文件名称超长截取部分显示
                    string NewCap = new FileInfo(pair.Key).Name.Replace(new FileInfo(pair.Key).Extension, "");
                    if (NewCap.Length > 50)
                    {
                        bar.Caption = NewCap.Substring(0, 25) + "......" + NewCap.Substring(NewCap.Length - 25, 25);
                    }
                    else
                    {
                        bar.Caption = NewCap + new string(' ', 80 - NewCap.Length);
                    }

                    bar.Tag = pair.Key;

                    //描述文字（全路径）超长截取部分显示
                    if (pair.Key.Length > 50)
                    {
                        bar.Description = pair.Key.Substring(0, 25) + "......" + pair.Key.Substring(pair.Key.Length - 25, 25) + "\r\n" + pair.Value;
                    }
                    else
                    {
                        bar.Description = pair.Key + "\r\n" + pair.Value;
                    }

                    bar.ItemClick += RecentClick;//绑定按钮事件
                    this.barSubItem_OpenRecentFiles.ItemLinks.Add(bar);
                }
            }
        }

        /// <summary>
        /// 最近故障树文件单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RecentClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (e.Item.Tag != null && File.Exists(e.Item.Tag.ToString()))
            {
                //打开故障树文件
                LoadData(false, e.Item.Tag.ToString());

                //定位到打开的故障树（打开故障树时会同时加载该故障树所在工程，以及工程下所有故障树）
                foreach (TreeListNode PNode in treeList_Project.Nodes)
                {
                    foreach (TreeListNode GNode in PNode.Nodes)
                    {
                        //如果是工程节点下面的故障树定位
                        if (GNode.Tag != null && ((ProjectModel)PNode.Tag).ProjectPath + "\\" + ((SystemModel)GNode.Tag).SystemName + FixedString.APP_EXTENSION == e.Item.Tag.ToString())
                        {
                            treeList_Project.FocusedNode = GNode;
                            break;
                        }
                        foreach (TreeListNode SNode in GNode.Nodes)
                        {
                            //如果是分组下面的故障树定位
                            if (((ProjectModel)PNode.Tag).ProjectPath + "\\" + ((SystemModel)SNode.Tag).SystemName + FixedString.APP_EXTENSION == e.Item.Tag.ToString())
                            {
                                treeList_Project.FocusedNode = SNode;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 通用事件（全局调用，快捷键触发等等）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void General_GlobalHandler(object sender, GlobalEventArgs e)
        {
            try
            {
                switch (e.GlobalEvent)
                {
                    case GlobalEvent.InsertNode: { this.InsertNode(e.Value); break; }//插入节点（FTA表和FTA图都用到）
                    case GlobalEvent.DiagramMouseMove: { this.DiagramMouseMove(e.Value); break; }//主菜单插入门事件点击后鼠标在FTA图上显示浮动图形
                    case GlobalEvent.PropertyEdited: { this.ftaTable.EditProperty(e.Value); break; }//单个属性修改
                    case GlobalEvent.PropertiesEdited: { this.ftaTable.EditProperties(e.Value); break; }//多个属性修改（编辑）
                    case GlobalEvent.CheckStateReset: { this.ReSetCheckState(); break; }//解除主菜单插入图形选中状态
                    case GlobalEvent.TableFocused: { this.ftaTable.FocusOn((DrawData)e.Value); break; }//FTA表设置哪些图形可见（图太大部分显示）
                    case GlobalEvent.FTADiagram_MakeVisable: { this.ftaDiagram.FocusOn((DrawData)e.Value, true); break; }//FTA图设置哪些图形可见（同上）
                    case GlobalEvent.CommonCellValueChanged: { this.ftaTable.ChangeCommonCellValue(e.Value); break; }//FTA表普通文本值修改通用的操作
                    case GlobalEvent.UpdateData: { this.UpdateData((bool)e.Value); break; }//刷新界面上的表格和图表控件
                    case GlobalEvent.UpdateLayout: { this.UpdateLayout(); break; }//刷新布局
                    case GlobalEvent.TableShortCut: { this.TableControlMenuClick(e.Value.ToString()); break; }//快捷键响应事件
                    case GlobalEvent.ImprotInfoFromProcess: { if (e?.Value != null) this.ImportInfoFromProcess(e.Value.ToString()); break; }//导入Excel文件
                }
            }
            catch (Exception ex)
            {
                MsgBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// FTA表和FTA图右键菜单通用的返回某菜单项是否可见或可用的通用函数
        /// </summary>
        /// <param name="item">控件对象</param>
        /// <param name="selectedDrawData">当前选择的作用对象</param>
        /// <param name="FTATableDiagram_DrawData_CopyOrCut">如果是复制剪切操作，这个就是要复制剪切的源对象</param>
        /// <param name="FTATableDiagram_Is_CopyNode">如果是复制剪切操作，指示是否是复制</param>
        /// <param name="FTATableDiagram_Is_CopyOrCut_Recurse">如果是复制剪切操作，指示是否是递归操作</param>
        /// <returns>该菜单是否可用</returns>
        public bool GetBarItemIsEnabled(object item, DrawData selectedDrawData, DrawData FTATableDiagram_DrawData_CopyOrCut = null, bool? FTATableDiagram_Is_CopyNode = null, bool? FTATableDiagram_Is_CopyOrCut_Recurse = null)
        {
            return General.TryCatch(() =>
            {
                //获取属性或操作的名称
                var barItem = item as BarItem;
                var keyName = General.GetKeyName(barItem.Caption);

                if (selectedDrawData != null)
                {
                    //重复事件（上一个下一个）
                    this.barButtonItem_PreviousRepeatEvent.Visibility = selectedDrawData.Repeats < 1 ? BarItemVisibility.Never : BarItemVisibility.Always;
                    this.barButtonItem_NextRepeatEvent.Visibility = selectedDrawData.Repeats < 1 ? BarItemVisibility.Never : BarItemVisibility.Always;

                    //删除菜单单独判断（根据门事件类型）
                    //当前是顶层节点      
                    this.barButtonItem_DeleteNodesTable.Caption = General.FtaProgram.String.DeleteNodes;
                    this.barButtonItem_DeleteNodesDiagram.Caption = General.FtaProgram.String.DeleteNodes;
                    barButtonItem_DeleteNodesRibbon.Caption = General.FtaProgram.String.DeleteNodes;

                    if (selectedDrawData.ThisGuid == General.FtaProgram.CurrentRoot.ThisGuid)
                    {
                        this.barButtonItem_DeleteTopTable.Visibility = BarItemVisibility.Always;
                        this.barButtonItem_DeleteTopDiagram.Visibility = BarItemVisibility.Always;

                        this.barButtonItem_DeleteLevelTable.Visibility = BarItemVisibility.Never;
                        this.barButtonItem_DeleteLevelDiagram.Visibility = BarItemVisibility.Never;

                        this.barButtonItem_DeleteNodeTable.Visibility = BarItemVisibility.Never;
                        this.barButtonItem_DeleteNodeDiagram.Visibility = BarItemVisibility.Never;

                        this.barButtonItem_DeleteNodesTable.Visibility = BarItemVisibility.Never;
                        this.barButtonItem_DeleteNodesDiagram.Visibility = BarItemVisibility.Never;

                        this.barButtonItem_DeleteTransferTable.Visibility = BarItemVisibility.Never;
                        this.barButtonItem_DeleteTransferDiagram.Visibility = BarItemVisibility.Never;

                        this.barButtonItem_DeleteTopRibbon.Visibility = BarItemVisibility.Always;
                        this.barButtonItem_DeleteLevelRibbon.Visibility = BarItemVisibility.Never;
                        this.barButtonItem_DeleteNodeRibbon.Visibility = BarItemVisibility.Never;
                        barButtonItem_DeleteNodesRibbon.Visibility = BarItemVisibility.Never;
                        barButtonItem_DeleteTransferRibbon.Visibility = BarItemVisibility.Never;
                    }
                    //当前节点只有一个子节点（删除级别可见，删除子节点和删除子节点包含转移门这两种根据是否有转移门判断是否可见）
                    else if (selectedDrawData?.Parent != null && selectedDrawData.Children.Count == 1)
                    {
                        this.barButtonItem_DeleteTopTable.Visibility = BarItemVisibility.Never;
                        this.barButtonItem_DeleteTopDiagram.Visibility = BarItemVisibility.Never;

                        this.barButtonItem_DeleteLevelTable.Visibility = BarItemVisibility.Always;
                        this.barButtonItem_DeleteLevelDiagram.Visibility = BarItemVisibility.Always;

                        this.barButtonItem_DeleteNodeTable.Visibility = BarItemVisibility.Never;
                        this.barButtonItem_DeleteNodeDiagram.Visibility = BarItemVisibility.Never;

                        //判断是否有转移门
                        List<DrawData> Alldatas = selectedDrawData.GetAllData(General.FtaProgram.CurrentSystem, true);
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
                            this.barButtonItem_DeleteNodesTable.Visibility = BarItemVisibility.Always;
                            this.barButtonItem_DeleteNodesDiagram.Visibility = BarItemVisibility.Always;

                            this.barButtonItem_DeleteTransferTable.Visibility = BarItemVisibility.Never;
                            this.barButtonItem_DeleteTransferDiagram.Visibility = BarItemVisibility.Never;

                            barButtonItem_DeleteNodesRibbon.Visibility = BarItemVisibility.Always;
                            barButtonItem_DeleteTransferRibbon.Visibility = BarItemVisibility.Never;

                            this.barButtonItem_DeleteNodesTable.Caption = General.FtaProgram.String.DeleteNodes_HasTransfer;
                            this.barButtonItem_DeleteNodesDiagram.Caption = General.FtaProgram.String.DeleteNodes_HasTransfer;
                            barButtonItem_DeleteNodesRibbon.Caption = General.FtaProgram.String.DeleteNodes_HasTransfer;
                        }
                        else
                        {
                            this.barButtonItem_DeleteNodesTable.Visibility = BarItemVisibility.Always;
                            this.barButtonItem_DeleteNodesDiagram.Visibility = BarItemVisibility.Always;

                            this.barButtonItem_DeleteTransferTable.Visibility = BarItemVisibility.Never;
                            this.barButtonItem_DeleteTransferDiagram.Visibility = BarItemVisibility.Never;

                            barButtonItem_DeleteNodesRibbon.Visibility = BarItemVisibility.Always;
                            barButtonItem_DeleteTransferRibbon.Visibility = BarItemVisibility.Never;

                            this.barButtonItem_DeleteNodesTable.Caption = General.FtaProgram.String.DeleteNodes;
                            this.barButtonItem_DeleteNodesDiagram.Caption = General.FtaProgram.String.DeleteNodes;
                            barButtonItem_DeleteNodesRibbon.Caption = General.FtaProgram.String.DeleteNodes;
                        }

                        this.barButtonItem_DeleteTopRibbon.Visibility = BarItemVisibility.Never;
                        this.barButtonItem_DeleteLevelRibbon.Visibility = BarItemVisibility.Always;
                        this.barButtonItem_DeleteNodeRibbon.Visibility = BarItemVisibility.Never;
                    }
                    //当前节点没有子节点，只显示删除当前节点选项
                    else if (selectedDrawData?.Parent != null && selectedDrawData.Children.Count == 0)
                    {
                        this.barButtonItem_DeleteTopTable.Visibility = BarItemVisibility.Never;
                        this.barButtonItem_DeleteTopDiagram.Visibility = BarItemVisibility.Never;

                        this.barButtonItem_DeleteLevelTable.Visibility = BarItemVisibility.Never;
                        this.barButtonItem_DeleteLevelDiagram.Visibility = BarItemVisibility.Never;

                        this.barButtonItem_DeleteNodeTable.Visibility = BarItemVisibility.Always;
                        this.barButtonItem_DeleteNodeDiagram.Visibility = BarItemVisibility.Always;

                        this.barButtonItem_DeleteNodesTable.Visibility = BarItemVisibility.Never;
                        this.barButtonItem_DeleteNodesDiagram.Visibility = BarItemVisibility.Never;

                        this.barButtonItem_DeleteTopRibbon.Visibility = BarItemVisibility.Never;
                        this.barButtonItem_DeleteLevelRibbon.Visibility = BarItemVisibility.Never;
                        this.barButtonItem_DeleteNodeRibbon.Visibility = BarItemVisibility.Always;
                        barButtonItem_DeleteNodesRibbon.Visibility = BarItemVisibility.Never;

                        this.barButtonItem_DeleteTransferTable.Visibility = BarItemVisibility.Never;
                        this.barButtonItem_DeleteTransferDiagram.Visibility = BarItemVisibility.Never;
                        barButtonItem_DeleteTransferRibbon.Visibility = BarItemVisibility.Never;
                    }
                    //当前节点有多个子节点（删除级别不可见，删除子节点和删除子节点包含转移门这两种根据是否有转移门判断是否可见）
                    else
                    {
                        this.barButtonItem_DeleteTopTable.Visibility = BarItemVisibility.Never;
                        this.barButtonItem_DeleteTopDiagram.Visibility = BarItemVisibility.Never;

                        this.barButtonItem_DeleteLevelTable.Visibility = BarItemVisibility.Never;
                        this.barButtonItem_DeleteLevelDiagram.Visibility = BarItemVisibility.Never;

                        this.barButtonItem_DeleteNodeTable.Visibility = BarItemVisibility.Never;
                        this.barButtonItem_DeleteNodeDiagram.Visibility = BarItemVisibility.Never;

                        this.barButtonItem_DeleteTopRibbon.Visibility = BarItemVisibility.Never;
                        this.barButtonItem_DeleteLevelRibbon.Visibility = BarItemVisibility.Never;
                        this.barButtonItem_DeleteNodeRibbon.Visibility = BarItemVisibility.Never;

                        List<DrawData> Alldatas = selectedDrawData.GetAllData(General.FtaProgram.CurrentSystem, true);
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
                            this.barButtonItem_DeleteNodesTable.Visibility = BarItemVisibility.Always;
                            this.barButtonItem_DeleteNodesDiagram.Visibility = BarItemVisibility.Always;

                            this.barButtonItem_DeleteTransferTable.Visibility = BarItemVisibility.Never;
                            this.barButtonItem_DeleteTransferDiagram.Visibility = BarItemVisibility.Never;

                            barButtonItem_DeleteNodesRibbon.Visibility = BarItemVisibility.Always;
                            barButtonItem_DeleteTransferRibbon.Visibility = BarItemVisibility.Never;

                            this.barButtonItem_DeleteNodesTable.Caption = General.FtaProgram.String.DeleteNodes_HasTransfer;
                            this.barButtonItem_DeleteNodesDiagram.Caption = General.FtaProgram.String.DeleteNodes_HasTransfer;
                            barButtonItem_DeleteNodesRibbon.Caption = General.FtaProgram.String.DeleteNodes_HasTransfer;
                        }
                        else
                        {
                            this.barButtonItem_DeleteNodesTable.Visibility = BarItemVisibility.Always;
                            this.barButtonItem_DeleteNodesDiagram.Visibility = BarItemVisibility.Always;

                            this.barButtonItem_DeleteTransferTable.Visibility = BarItemVisibility.Never;
                            this.barButtonItem_DeleteTransferDiagram.Visibility = BarItemVisibility.Never;

                            barButtonItem_DeleteNodesRibbon.Visibility = BarItemVisibility.Always;
                            barButtonItem_DeleteTransferRibbon.Visibility = BarItemVisibility.Never;

                            this.barButtonItem_DeleteNodesTable.Caption = General.FtaProgram.String.DeleteNodes;
                            this.barButtonItem_DeleteNodesDiagram.Caption = General.FtaProgram.String.DeleteNodes;
                            barButtonItem_DeleteNodesRibbon.Caption = General.FtaProgram.String.DeleteNodes;
                        }
                    }

                    //设置Ribbon菜单可见性，和部分其它菜单（插入）
                    if (ftaDiagram.SelectedData.Count > 1 && ftaDiagram.SelectedData.Contains(selectedDrawData))
                    {
                        this.ChangeBarButtonItemVisibility(false, selectedDrawData);
                        barButtonItem_BasicLib.Enabled = true;
                        barButtonItem_AddToBasicEventLibrary.Enabled = false;
                        barButtonItem_SynchronizeFromBasicEventLibrary.Enabled = false;
                        barButtonItem_InsertFromBasicEventLibrary.Enabled = false;
                    }
                    else
                    {
                        this.ChangeBarButtonItemVisibility(true, selectedDrawData);
                    }
                }

                //该参数为了方便fta表拖拽
                if (FTATableDiagram_DrawData_CopyOrCut == null) FTATableDiagram_DrawData_CopyOrCut = General.CopyCutObject;
                if (FTATableDiagram_Is_CopyNode == null) FTATableDiagram_Is_CopyNode = this.ftaTable.TableEvents.IsCopyNode;
                if (FTATableDiagram_Is_CopyOrCut_Recurse == null) FTATableDiagram_Is_CopyOrCut_Recurse = General.FTATableDiagram_Is_CopyOrCut_Recurse;
                //插入、插入重复事件、fta表的插入
                if (item == this.Blci_Insert || item == barButtonItem_Insert || item == barButtonItem_InsertRepeatEvent || item == barButtonItem_FTATable_Insertinput || item == barButtonItem_AddLinkGate)
                {
                    if
                    (
                       selectedDrawData != null &&
                       (
                          selectedDrawData.Type == DrawType.AndGate ||
                          selectedDrawData.Type == DrawType.OrGate ||
                          selectedDrawData.Type == DrawType.PriorityAndGate ||
                          selectedDrawData.Type == DrawType.XORGate ||
                          selectedDrawData.Type == DrawType.VotingGate ||
                          selectedDrawData.Type == DrawType.RemarksGate
                       )
                    )
                    {
                        return true;
                    }
                }
                else if (
                keyName == nameof(StringModel.AndGate) ||
                keyName == nameof(StringModel.OrGate) ||
                keyName == nameof(StringModel.BasicEvent) ||
                keyName == nameof(StringModel.PriorityAndGate) ||
                keyName == nameof(StringModel.XORGate) ||
                keyName == nameof(StringModel.VotingGate) ||
                keyName == nameof(StringModel.RemarksGate) ||
                keyName == nameof(StringModel.UndevelopedEvent) ||
                keyName == nameof(StringModel.TransferInGate) ||
                keyName == nameof(StringModel.HouseEvent) ||
                keyName == nameof(StringModel.ConditionEvent))
                {
                    return true;
                }
                //删除、剪切、复制、FTA表剪切、FTA表复制、FTA表删除
                else if (
                   keyName == nameof(StringModel.Cut) ||
                   keyName == nameof(StringModel.Copy) ||
                   keyName == nameof(StringModel.CutNodes) ||
                   keyName == nameof(StringModel.CopyNode) ||
                   keyName == nameof(StringModel.CopyNodes) ||
                   keyName == nameof(StringModel.DeleteNode) ||
                   keyName == nameof(StringModel.DeleteNodes) ||
                   keyName == nameof(StringModel.DeleteNodesAndTransfer) ||
                   keyName == nameof(StringModel.Delete) ||
                   keyName == nameof(StringModel.PreviousRepeatEvent) ||
                   keyName == nameof(StringModel.NextRepeatEvent) ||
                   keyName == nameof(StringModel.DeleteTop) ||
                   keyName == nameof(StringModel.DeleteLevel) ||
                   keyName == nameof(StringModel.DeleteNodes_HasTransfer))
                {
                    if (
                    selectedDrawData != null &&
                    General.FtaProgram.CurrentSystem != null &&
                    General.FtaProgram.CurrentSystem.RepeatedEvents != null &&
                    General.FtaProgram.CurrentSystem.TranferGates != null)
                    {
                        return true;
                    }
                }
                //进入转移门
                else if (selectedDrawData != null && (item == barButtonItem_BreakIntoTransfer || item == barButtonItem_MenuBreakIntoTransfer))
                {
                    if (GetBarItemIsEnabled(barButtonItem_Insert, selectedDrawData))
                    {
                        //非顶层节点,非已存在转移门
                        if (
                           selectedDrawData.Parent?.Children != null &&
                           General.FtaProgram.CurrentSystem?.Roots?.Contains(selectedDrawData) == false &&
                           General.FtaProgram.CurrentSystem?.TranferGates?.ContainsKey(selectedDrawData.Identifier) == false)
                        {
                            return true;
                        }
                    }
                }
                //折叠转移门
                //转到
                else if (item == barButtonItem_CollapseTransfer || item == barButtonItem_MenuCollapseTransfer || item == barButtonItem_TransferTo || item == barSubItem_TransferTo)
                {
                    if (selectedDrawData != null && General.FtaProgram.CurrentSystem != null && General.FtaProgram.CurrentSystem.TranferGates != null &&
                        General.FtaProgram.CurrentSystem.TranferGates.ContainsKey(selectedDrawData.Identifier))
                    {
                        HashSet<DrawData> transfer = General.FtaProgram.CurrentSystem.TranferGates[selectedDrawData.Identifier];
                        if (transfer != null && transfer.Contains(selectedDrawData))
                        {
                            //转移门集合只剩2个
                            if ((item == barButtonItem_CollapseTransfer || item == barButtonItem_MenuCollapseTransfer) && transfer.Count == 2)
                            {
                                return true;
                            }
                            else if (item == barButtonItem_TransferTo || item == barSubItem_TransferTo)
                            {
                                return true;
                            }
                        }
                    }
                }
                //转到的子菜单
                else if (item != null && item.GetType() == typeof(BarButtonItem) && (barButtonItem_TransferTo.ContainsItem(item as BarItem) || barSubItem_TransferTo.ContainsItem(item as BarItem)))
                {
                    BarItem item_Tmp = item as BarItem;
                    if (item_Tmp.Tag != null && item_Tmp.Tag.GetType() == typeof(DrawData)) return true;
                }
                //高亮割集
                else if (item == General.HighLightCutSet)
                {
                    if (selectedDrawData != null && selectedDrawData.Cutset != null && selectedDrawData.Cutset.ListCutsets_Real != null
                        && selectedDrawData.Cutset.ListCutsets_Real.Count > 0 && General.HighLightCutSet.Gallery.Groups != null
                        && General.HighLightCutSet.Gallery.Groups.Count > 0)
                        return true;
                }
                //粘贴
                //FTA表粘贴
                else if (item == barButtonItem_Paste || item == barButtonItem_FTATable_Paste || item == barButtonItem_MenuPaste)
                {
                    if (selectedDrawData != null && FTATableDiagram_DrawData_CopyOrCut != null && GetBarItemIsEnabled(barButtonItem_Insert, selectedDrawData)
                        && General.FtaProgram.CurrentSystem != null && General.FtaProgram.CurrentSystem.TranferGates != null)
                    {
                        //剪切不能粘贴给自己的子节点,不能粘贴给已经是他父亲的节点
                        if (!(bool)FTATableDiagram_Is_CopyNode && (FTATableDiagram_DrawData_CopyOrCut == selectedDrawData || selectedDrawData.IsChildOfParent(FTATableDiagram_DrawData_CopyOrCut))) return false;

                        //如果包含转移门
                        List<DrawData> allData = FTATableDiagram_DrawData_CopyOrCut.GetAllData(General.FtaProgram.CurrentSystem);//General.FtaProgram.CurrentSystem.GetAllDatas(FTATableDiagram_DrawData_CopyOrCut);
                        List<DrawData> transfer = allData.Where(obj => General.FtaProgram.CurrentSystem.TranferGates.ContainsKey(obj.Identifier)).ToList();
                        //如果本身是转移门，那么移除他
                        transfer.Remove(FTATableDiagram_DrawData_CopyOrCut);
                        List<DrawData> transfer_TRANSFER_SYMBOL = transfer.Where(obj => obj.Type == DrawType.TransferInGate).ToList();
                        //剪切时，如果他包含转移门检查下
                        if (!(bool)FTATableDiagram_Is_CopyNode && transfer != null && transfer.Count > 0)
                        {
                            foreach (DrawData tran in transfer)
                            {
                                //检查如果是转移门时是否允许粘贴
                                if (!this.ftaTable.TableEvents.CanPasteTransGate(tran, selectedDrawData)) return false;
                            }
                        }
                        //递归复制时，如果他包含转移门副本，检查下
                        else if ((bool)FTATableDiagram_Is_CopyNode && (bool)FTATableDiagram_Is_CopyOrCut_Recurse && transfer_TRANSFER_SYMBOL != null && transfer_TRANSFER_SYMBOL.Count > 0)
                        {
                            foreach (DrawData tran in transfer_TRANSFER_SYMBOL)
                            {
                                //检查如果是转移门时是否允许粘贴
                                if (!this.ftaTable.TableEvents.CanPasteTransGate(tran, selectedDrawData))
                                    return false;
                            }
                        }

                        //检查如果本身是转移门时是否允许粘贴
                        if (!this.ftaTable.TableEvents.CanPasteTransGate(FTATableDiagram_DrawData_CopyOrCut, selectedDrawData)) return false;
                        return true;
                    }
                }
                //粘贴重复事件
                //FTA表粘贴重复事件
                else if (item == barButtonItem_PasteRepeatEvent || item == barButtonItem_FTATable_PasteRepeatedEvent || item == barButtonItem_MenuPasteRepeated)
                {
                    if (selectedDrawData != null && FTATableDiagram_DrawData_CopyOrCut != null)
                    {
                        //只有复制并且符合粘贴规则
                        if ((bool)FTATableDiagram_Is_CopyNode && GetBarItemIsEnabled(barButtonItem_Paste, selectedDrawData))
                        {
                            //只有包含事件才能粘贴
                            DrawData evt = FTATableDiagram_DrawData_CopyOrCut.GetAllData(General.FtaProgram.CurrentSystem).Where(obj => obj.CanRepeatedType).FirstOrDefault();
                            if ((bool)FTATableDiagram_Is_CopyOrCut_Recurse && evt != null) return true;
                            else if (!(bool)FTATableDiagram_Is_CopyOrCut_Recurse && FTATableDiagram_DrawData_CopyOrCut.CanRepeatedType) return true;
                        }
                    }
                }
                //FAT表插入新顶级门
                else if (item == barButtonItem_FTATable_Insertnewtopleveldoor)
                {
                    if (treeList_FTATable.DataSource != null && General.FtaProgram.CurrentSystem != null && General.FtaProgram.CurrentSystem.Roots != null
                        && treeList_FTATable.DataSource.GetType() == typeof(VirtualDrawData))
                    {
                        VirtualDrawData vData = treeList_FTATable.DataSource as VirtualDrawData;
                        if (vData != null && vData.data != null)
                        {
                            if (vData.data.Children == null) vData.data.Children = new List<DrawData>();
                            return true;
                        }
                    }
                }
                //FTA表粘贴为顶级门
                else if (item == barButtonItem_FTATable_PasteastopGate)
                {
                    if (
                    FTATableDiagram_DrawData_CopyOrCut != null &&
                    GetBarItemIsEnabled(barButtonItem_FTATable_Insertnewtopleveldoor, null) &&
                    GetBarItemIsEnabled(barButtonItem_Insert, FTATableDiagram_DrawData_CopyOrCut))
                    {
                        return true;
                    }
                }
                //FTA表取消复制和剪切
                else if (item == barButtonItem_FTATable_Cancelcopyorcut)
                {
                    if (FTATableDiagram_DrawData_CopyOrCut != null) return true;
                }
                //FTA表扩展开所有子节点
                //FTA表收缩所有子节点
                else if (item == barButtonItem_FTATable_Expandallnodes || item == barButtonItem_FTATable_Collapseallnodes)
                {
                    if (!VirtualDrawData.is_FilterAllMode && treeList_FTATable.Nodes?.Count > 0) return true;
                }
                //FTA表冻结列
                else if (keyName == nameof(StringModel.FreezeColumn))
                {
                    if (treeList_FTATable.VisibleColumns?.Count > 0 && treeList_FTATable.FocusedColumn != null) return true;
                }
                //FTA表取消冻结列
                else if (keyName == nameof(StringModel.UnfreezeColumns))
                {
                    if (treeList_FTATable.VisibleColumns != null && treeList_FTATable.VisibleColumns?.Count > 0)
                    {
                        foreach (TreeListColumn tmp in treeList_FTATable.VisibleColumns)
                        {
                            if (tmp.Fixed != FixedStyle.None) return true;
                        }
                    }

                }
                else if (item == barButtonItem_ShowFTACalEvent)
                {
                    return true;
                }
                else if (item == barButtonItem_UpDateLinkGate)
                {
                    if (selectedDrawData != null && selectedDrawData.Type == DrawType.BasicEvent)
                    {
                        return true;
                    }
                }
                else if (item == barButtonItem_MenuCopyCurrentView)
                {
                    return true;
                }
                else if (item == barButtonItem_MenuCopyCurrentSelected)
                {
                    return true;
                }
                else if (item == barButtonItem_ShapeEdit)
                {
                    return true;
                }
                else if (item == barButtonItem_DiaExportToModel)
                {
                    return true;
                }
                return false;
            });
        }

        /// <summary>
        /// 初始化全局（General）控件（部分需要全局操作的控件赋值）
        /// </summary>
        private void InitializeGeneralControl()
        {
            General.dockManager_FTA = dockManager_FTA;
            General.DiagramControlList = new List<DiagramControl>();
            General.DiagramControl = this.currentDiagram;
            General.ControlContainer = this.dockPanel_Container_FTADiagram;
            General.PanelContainer = this.dockPanel_FTATable;
            General.ProjectContainer = this.dockPanel_Project;
            General.FTATree = this.treeList_FTATable;
            General.ProjectControl = this.treeList_Project;
            General.TableControl = this.treeList_FTATable;
            General.DiagramMenuControl = this.popupMenu_FTADiagram;
            General.RibbonControl = this.ribbonControl_FTA;
            General.StaticItem_File = this.barStaticItem_File;
            General.HighLightCutSet = this.ribbonGalleryBarItem_HighLightCutSet;
            General.TransferTo = this.barButtonItem_TransferTo;
            General.SubTransferTo = this.barSubItem_TransferTo;
            General.Delete = this.Sub_DeleteDiagramNode;
            General.DeleteTable = this.Sub_Delete;
            General.CopyTable = this.Sub_Copy;
            General.GlobalHandler += General_GlobalHandler;

            General.barButtonItem_ExportToModel = barButtonItem_ExportToModel;
            General.barButtonItem_LoadFromModel = barButtonItem_LoadFromModel;
            General.barButtonItem_BasicLib = barButtonItem_BasicLib;
            General.barButtonItem_AddToBasicEventLibrary = barButtonItem_AddToBasicEventLibrary;
            General.barButtonItem_SynchronizeFromBasicEventLibrary = barButtonItem_SynchronizeFromBasicEventLibrary;
            General.barButtonItem_InsertFromBasicEventLibrary = barButtonItem_InsertFromBasicEventLibrary;
            General.barButtonItem_Cut = barButtonItem_Cut;
            General.barButtonItem_MenuCut = barButtonItem_MenuCut;
            General.barButtonItem_ShapeEdit = barButtonItem_ShapeEdit;
            General.barButtonItem_Copy = barButtonItem_Copy;
            General.barButtonItem_MenuCopy = barButtonItem_MenuCopy;
            General.barButtonItem_MenuCopyCurrentView = barButtonItem_MenuCopyCurrentView;
            General.barButtonItem_MenuCopyCurrentSelected = barButtonItem_MenuCopyCurrentSelected;
            General.barButtonItem_MenuPaste = barButtonItem_MenuPaste;
            General.barButtonItem_MenuPasteRepeated = barButtonItem_MenuPasteRepeated;

            General.barButtonItem_DeleteTopRibbon = barButtonItem_DeleteTopRibbon;
            General.barButtonItem_DeleteLevelRibbon = barButtonItem_DeleteLevelRibbon;
            General.barButtonItem_DeleteNodeRibbon = barButtonItem_DeleteNodeRibbon;
            General.barButtonItem_DeleteNodesRibbon = barButtonItem_DeleteNodesRibbon;
            General.barButtonItem_DeleteTransferRibbon = barButtonItem_DeleteTransferRibbon;

            General.barButtonItem_DeleteTopTable = barButtonItem_DeleteTopTable;
            General.barButtonItem_DeleteLevelTable = barButtonItem_DeleteLevelTable;
            General.barButtonItem_DeleteNodeTable = barButtonItem_DeleteNodeTable;
            General.barButtonItem_DeleteNodesTable = barButtonItem_DeleteNodesTable;
            General.barButtonItem_DeleteTransferTable = barButtonItem_DeleteTransferTable;

            General.barButtonItem_DeleteTopDiagram = barButtonItem_DeleteTopDiagram;
            General.barButtonItem_DeleteLevelDiagram = barButtonItem_DeleteLevelDiagram;
            General.barButtonItem_DeleteNodeDiagram = barButtonItem_DeleteNodeDiagram;
            General.barButtonItem_DeleteNodesDiagram = barButtonItem_DeleteNodesDiagram;
            General.barButtonItem_DeleteTransferDiagram = barButtonItem_DeleteTransferDiagram;

            General.barSubItem_ChangeGateType = barSubItem_ChangeGateType;
            General.barButtonItem_InsertLevel = barButtonItem_InsertLevel;
            General.barSubItem_TransferTo = barSubItem_TransferTo;
            General.barButtonItem_MenuBreakIntoTransfer = barButtonItem_MenuBreakIntoTransfer;
            General.barButtonItem_MenuCollapseTransfer = barButtonItem_MenuCollapseTransfer;
        }

        /// <summary>
        /// FTA表和FTA图右键菜单通用的返回某菜单项是否可见或可用的通用函数全局化
        /// </summary>
        private void InitializeGeneralFunc()
        {
            General.GetBarItemIsEnabled = this.GetBarItemIsEnabled;
        }

        ///<summary>
        /// FTA表和图通用的用于插入一个节点和图形
        /// </summary>
        /// <param name="parentData">父节点目标对象</param>
        /// <param name="type">类型</param>
        /// <returns>新插入的数据对象</returns>
        private DrawData InsertNode(DrawData parentData, DrawType type = DrawType.NULL)
        {
            return General.TryCatch(() =>
            {
                DrawData item = null;
                if (parentData != null)
                {
                    //获取要插入的类型
                    if (type == DrawType.NULL)
                    {
                        XtraFormNewFTAItem newFtaItem = new XtraFormNewFTAItem("New Item");
                        if (newFtaItem.ShowDialog() == DialogResult.OK) type = newFtaItem.GetInfo();
                    }

                    if (type != DrawType.NULL)
                    {
                        //如果是插入重复事件模式
                        if (this.ftaDiagram.DiagramEvents.RepeatedEvent != null)
                        {
                            DrawData result = this.ftaTable.PasteRepeatedEvent(parentData, true, this.ftaDiagram.DiagramEvents.RepeatedEvent, false);
                            item = result;

                            //情况: 插入一个子级(重复事件--已经选定的)
                            if ((null != result)
                                && (null != General.FtaProgram)
                                && (null != General.FtaProgram.CurrentSystem))
                            {
                                //做历史记录
                                General.FtaProgram.CurrentSystem.TakeBehavor(
                                    result
                                    , Behavior.Enum.ElementOperate.Creation
                                    , Behavior.Enum.ElementOperate.Deletion
                                    , parentData
                                    , null);
                            }
                        }
                        //如果要插入Label
                        //else if (type == DrawType.Label)
                        //{
                        //    //TODO:插入label
                        //}
                        //插入转移门
                        else if (type == DrawType.TransferInGate && General.FtaProgram.CurrentSystem?.Roots != null && General.FtaProgram.CurrentSystem.TranferGates != null)
                        {
                            //复制源
                            DrawData source = null;

                            if (this.transfer == null)
                            {
                                //找到所有顶层节点和已存在转移门                    
                                List<string> ids = new List<string>();
                                //添加根节点
                                foreach (DrawData data in General.FtaProgram.CurrentSystem.Roots)
                                {
                                    if (data != parentData && !parentData.IsChildOfParent(data) && !General.FtaProgram.CurrentSystem.TranferGates.ContainsKey(data.Identifier))
                                        ids.Add(data.Identifier);
                                }

                                List<string> transferId = General.FtaProgram.CurrentSystem.TranferGates.Keys.ToList();
                                if (transferId != null) ids.AddRange(transferId);
                                //如果目标是转移门本体或其儿子，要去掉id
                                ids.Remove(parentData.Identifier);
                                foreach (var pair in General.FtaProgram.CurrentSystem.TranferGates)
                                {
                                    foreach (DrawData tmp in pair.Value)
                                    {
                                        //找到一个副本
                                        if (tmp.Type == DrawType.TransferInGate)
                                        {
                                            if (!this.ftaTable.TableEvents.CanPasteTransGate(tmp, parentData))
                                            {
                                                ids.Remove(tmp.Identifier);
                                            }
                                            break;
                                        }
                                    }
                                }

                                //让用户选取id
                                NewRepeatOrTransfer form = new NewRepeatOrTransfer(General.FtaProgram.String, General.FtaProgram.String.InsertNode, ids);
                                if (form.ShowDialog() == DialogResult.Cancel)
                                {
                                    form.Dispose();
                                    return null;
                                }
                                string id = form.GetInfo();
                                form.Dispose();

                                if (!string.IsNullOrEmpty(id))
                                {
                                    if (General.FtaProgram.CurrentSystem.TranferGates.ContainsKey(id))
                                    {
                                        HashSet<DrawData> transfer = General.FtaProgram.CurrentSystem.TranferGates[id];
                                        if (transfer != null && transfer.Count > 0) source = transfer.FirstOrDefault();
                                    }
                                    else
                                    {
                                        foreach (DrawData data in General.FtaProgram.CurrentSystem.Roots)
                                        {
                                            if (data.Identifier == id)
                                            {
                                                source = data;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            else source = this.transfer;


                            //复制选择的转移门对象并插入
                            if (source != null)
                            {
                                DrawData transfer = source.CopyDrawData();
                                transfer.Identifier = source.Identifier;
                                transfer.Parent = parentData;
                                if (parentData.Children == null) parentData.Children = new List<DrawData>();
                                parentData.Children.Add(transfer);
                                transfer.Type = DrawType.TransferInGate;
                                //第一次操作
                                if (this.transfer == null)
                                {
                                    if (this.ftaDiagram.DiagramEvents.AddingType != DrawType.TransferInGate)
                                    {
                                        ReSetCheckState();
                                        this.ftaDiagram.DiagramEvents.AddingType = DrawType.TransferInGate;
                                        var item_Trans = ribbonGalleryBarItem_GraphicTool.Gallery.GetItemByValue(DrawData.GetDescriptionByEnum(DrawType.TransferInGate));
                                        item_Trans.Checked = true;
                                    }
                                    this.transfer = transfer;
                                }
                                //维护转移门集合
                                //这里防止第一次新建转移门
                                General.FtaProgram.CurrentSystem.AddTranferGate(source);
                                General.FtaProgram.CurrentSystem.AddTranferGate(transfer);
                                General.InvokeHandler(GlobalEvent.UpdateLayout);
                                item = transfer;

                                //情况: 插入一个子级(转移门)
                                if ((null != transfer)
                                    && (null != General.FtaProgram)
                                    && (null != General.FtaProgram.CurrentSystem))
                                {
                                    //做历史记录
                                    General.FtaProgram.CurrentSystem.TakeBehavor(
                                        transfer
                                        , Behavior.Enum.ElementOperate.Creation
                                        , Behavior.Enum.ElementOperate.Deletion
                                        , parentData
                                        , null);
                                }
                            }
                        }
                        //else if (type == DrawType.InhibitGate)
                        //{
                        //    //TODO:禁止门可以实现，但会增加大量处理，布局，增删改等
                        //}
                        else
                        {
                            item = new DrawData();

                            //插入链接门
                            if (General.isLinkGateInsert == true)
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
                                    ReSetCheckState();
                                    return null;
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
                                                item.LinkPath = id;
                                                item.GUID = "";
                                                item.InputType = FixedString.MODEL_LAMBDA_TAU;
                                                item.Units = FixedString.UNITS_HOURS;
                                                item.Comment1 = ss.Roots[0].Comment1;

                                                item.InputValue = ss.Roots[0].QValue;
                                                item.InputValue2 = "1";

                                                item.LogicalCondition = FixedString.LOGICAL_CONDITION_NORMAL;
                                                item.QValue = ss.Roots[0].QValue;
                                            }
                                        }
                                    }
                                }
                            }

                            item.Type = type;
                            HashSet<string> ids = General.FtaProgram.CurrentSystem.GetAllIDs();
                            item.Identifier = item.GetNewID(ids, item.Type);
                            item.Parent = parentData;
                            if (!item.IsGateType && General.isLinkGateInsert == false)
                            {
                                item.InputType = FixedString.MODEL_LAMBDA_TAU;
                                item.Units = FixedString.UNITS_HOURS;
                            }

                            parentData.Children.Add(item);
                            General.InvokeHandler(GlobalEvent.UpdateLayout);

                            //情况: 插入一个子级
                            if ((null != General.FtaProgram)
                                && (null != General.FtaProgram.CurrentSystem))
                            {
                                //做历史记录
                                General.FtaProgram.CurrentSystem.TakeBehavor(
                                    item
                                    , Behavior.Enum.ElementOperate.Creation
                                    , Behavior.Enum.ElementOperate.Deletion
                                    , parentData
                                    , null);
                            }
                        }
                    }
                }
                return item;
            });
        }

        ///<summary>
        /// 插入一个中间层级（门）
        /// </summary>
        /// <param name="parentData">父节点目标对象</param>
        /// <param name="type">类型</param>
        /// <returns>新插入的数据对象</returns>
        private DrawData InsertLevelNode(DrawData parentData, DrawType type = DrawType.NULL)
        {
            return General.TryCatch(() =>
            {
                DrawData item = null;
                if (parentData != null)
                {
                    //获取要插入的类型
                    if (type == DrawType.NULL)
                    {
                        XtraFormNewFTAItem newFtaItem = new XtraFormNewFTAItem("New Level");
                        if (newFtaItem.ShowDialog() == DialogResult.OK) type = newFtaItem.GetInfo();
                    }

                    if (type != DrawType.NULL)
                    {
                        item = new DrawData();
                        item.Type = type;
                        HashSet<string> ids = General.FtaProgram.CurrentSystem.GetAllIDs();
                        item.Identifier = item.GetNewID(ids, item.Type);
                        item.Parent = parentData;
                        if (!item.IsGateType)
                        {
                            item.InputType = FixedString.MODEL_LAMBDA_TAU;
                            item.Units = FixedString.UNITS_HOURS;
                        }

                        //复制父节点的子节点集合
                        DrawData[] das = parentData.Children.ToArray();
                        parentData.Children.Clear();

                        item.Children.AddRange(das);
                        foreach (DrawData da in das)
                        {
                            da.Parent = item;
                        }

                        parentData.Children.Add(item);
                        General.InvokeHandler(GlobalEvent.UpdateLayout);

                        //情况: 插入一个子级(中间)
                        if ((null != General.FtaProgram)
                            && (null != General.FtaProgram.CurrentSystem))
                        {
                            //做历史记录
                            General.FtaProgram.CurrentSystem.TakeBehavor(
                                item
                                , Behavior.Enum.ElementOperate.Creation
                                , Behavior.Enum.ElementOperate.Deletion
                                , parentData
                                ,
                                (((null != item.Children)
                                && (0x00 < item.Children.Count))
                                ? new List<Behavior.ObjectBehavior>(item.Children)
                                : null));
                        }

                    }
                }
                return item;
            });
        }

        /// <summary>
        /// 通用插入节点事件响应
        /// </summary>
        /// <param name="param"></param>
        private void InsertNode(object param)
        {
            var value = (Tuple<DrawData, DrawType, MouseEventArgs>)param;
            if (CanInsertOrNot(value.Item3))
            {
                try
                {
                    General.IsIgnoreTreeListFocusNodeChangeEvent = true;
                    DrawData childData = InsertNode(value.Item1, value.Item2);
                    if (childData != null)
                    {
                        this.ftaTable.FocusOn(childData);
                        this.ftaDiagram.FocusOn(value.Item1);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    General.IsIgnoreTreeListFocusNodeChangeEvent = false;
                }
            }
        }

        /// <summary>
        /// 主菜单插入门事件点击后鼠标在FTA图上显示浮动图形
        /// </summary>
        /// <param name="param"></param>
        private void DiagramMouseMove(object param)
        {
            this.ftaDiagram.DiagramEvents.IsCreateNew = this.CanInsertOrNot(param as MouseEventArgs);
        }

        /// <summary>
        /// 判断是否能插入
        /// </summary>
        /// <param name="mouseEventArgs"></param>
        /// <returns></returns>
        private bool CanInsertOrNot(MouseEventArgs mouseEventArgs)
        {
            return General.TryCatch(() =>
            {
                var result = false;
                var item = General.DiagramControl.CalcHitItem(General.DiagramControl.PointToClient(MousePosition));
                if (mouseEventArgs.Button.HasLeft() == false) Cursor.Clip = System.Drawing.Rectangle.Empty;

                if (this.ftaDiagram.DiagramEvents.AddingType != DrawType.NULL
                && item?.GetType() == typeof(DiagramShape)
                && item.Tag?.GetType() == typeof(DrawData))
                {
                    DrawData data = item.Tag as DrawData;
                    //能简单插入操作
                    if (GetBarItemIsEnabled(barButtonItem_Insert, data))
                    {
                        if (this.ftaDiagram.DiagramEvents.AddingType == DrawType.TransferInGate && this.transfer != null)
                        {
                            //插入转移门判断可用性
                            result = this.ftaTable.TableEvents.CanPasteTransGate(this.transfer, data);
                        }
                        else result = true;
                    }
                }
                return result;
            });
        }

        /// <summary>
        /// 内容窗体装载重载事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //初始化全局（General）控件（部分需要全局操作的控件赋值）
            this.InitializeGeneralControl();

            //FTA表和FTA图右键菜单通用的返回某菜单项是否可见或可用的通用函数全局化
            this.InitializeGeneralFunc();

            //总的初始化方法，注册事件，菜单显示，画布等等
            this.Init();

            //加载起始页 
            dockPanel_Project.Visibility = DockVisibility.Hidden;
            dockPanel_FTATable.Visibility = DockVisibility.Hidden;
            dockPanel_FTADiagram.Visibility = DockVisibility.Hidden;
            StartPage.Dock = DockStyle.Fill;
            this.Controls.Add(StartPage);
            Thread td = new Thread(RunStartPage);//延迟显示开始页
            td.Start();
        }

        /// <summary>
        /// 延迟显示开始页
        /// </summary>
        public void RunStartPage()
        {
            try
            {
                this.Invoke(new Action(() => SplashScreenManager.ShowDefaultWaitForm(General.FtaProgram.String.SplashScreenCommand_Initial3, " ")));
                Thread.Sleep(200);
                if (SplashScreenManager.Default != null)
                {
                    this.Invoke(new Action(() => SplashScreenManager.CloseDefaultWaitForm()));
                }
                this.Invoke(new Action(() => StartPage.BringToFront()));
                this.Invoke(new Action(() => StartPage.Visible = true));
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 发送消息给其它进程
        /// </summary>
        private void SendMessageToSimfia()
        {
            var ps = Process.GetProcessesByName("WindowsFormsApplicationSimfia");
            if (ps?.Length > 0) CSendMessage.SendMessage(ps[0].MainWindowHandle, -1, -1, "started");
        }

        /// <summary>
        /// 按照全局的设置对象，重新设置软件菜单选中状态等
        /// </summary>
        /// <param name="ftaSetting">设置对象</param>
        private void ChangeFTASettting(SettingModel ftaSetting)
        {
            General.TryCatch(() =>
            {
                //设置默认皮肤
                UserLookAndFeel.Default.SkinName = ftaSetting.Skin_Name;

                //是否显示画布的标尺
                Bci_Ruler.Checked = ftaSetting.Is_Show_Ruler;
                General.DiagramControl.OptionsView.ShowRulers = ftaSetting.Is_Show_Ruler;

                //是否显示画布网格
                Bci_Grid.Checked = ftaSetting.Is_Show_Grid;
                General.DiagramControl.OptionsView.ShowGrid = ftaSetting.Is_Show_Grid;

                //是否显示分页虚线
                Bci_PageBreak.Checked = ftaSetting.Is_Show_PageBreak;
                General.DiagramControl.OptionsView.ShowPageBreaks = ftaSetting.Is_Show_PageBreak;

                //是否画布独占panel模式
                Bci_CanvasFillMode.Checked = ftaSetting.Is_CanvasFillMode;
                if (ftaSetting.Is_CanvasFillMode) General.DiagramControl.OptionsView.CanvasSizeMode = DevExpress.Diagram.Core.CanvasSizeMode.Fill;
                else General.DiagramControl.OptionsView.CanvasSizeMode = DevExpress.Diagram.Core.CanvasSizeMode.AutoSize;

                //是按照页滚动模式，还是内容滚动模式
                Bci_PageScrollMode.Checked = false;
                if (Bci_PageScrollMode.Checked) General.DiagramControl.OptionsBehavior.ScrollMode = DevExpress.Diagram.Core.DiagramScrollMode.Page;
                else General.DiagramControl.OptionsBehavior.ScrollMode = DevExpress.Diagram.Core.DiagramScrollMode.Content;

                //线条颜色
                barEditItem_LineColor.EditValue = ftaSetting.LineColor;

                //连线样式
                barEditItem_LineStyle.EditValue = ftaSetting.LineStyle;

                //箭头样式
                barEditItem_ArrowStyle.EditValue = ftaSetting.ArrowStyle;

                //箭头大小
                barEditItem_ArrowSize.EditValue = ftaSetting.ArrowSize;

                //缩放 
                this.Bci_ScaleAble.Checked = true;
                General.FtaProgram.Setting.Is_ScaleAble = Bci_ScaleAble.Checked;
                this.ftaDiagram = new FtaDiagram(General.DiagramControl, General.FtaProgram);
                this.ftaDiagram.ResetSetting(ResetType.ScaleAble);

                //FTA表指示器
                this.barCheckItem_ShowIndicator.Checked = ftaSetting.Is_ShowIndicator;
                treeList_FTATable.OptionsView.ShowIndicator = ftaSetting.Is_ShowIndicator;
                if (barCheckItem_ShowIndicator.Checked)
                {
                    barEditItem_IndicatorTopGateColor.Enabled = true;
                    barEditItem_IndicatorTransInGateColor.Enabled = true;
                }
                else
                {
                    barEditItem_IndicatorTopGateColor.Enabled = false;
                    barEditItem_IndicatorTransInGateColor.Enabled = false;
                }

                //指示器里顶级门图形颜色
                barEditItem_IndicatorTopGateColor.EditValue = ftaSetting.FTATableIndicatorTopGateColor;

                //指示器里转移门图形颜色
                barEditItem_IndicatorTransInGateColor.EditValue = ftaSetting.FTATableIndicatorTransInGateColor;

                //语言
                barEditItem_Language.EditValue = ftaSetting.Language;
            });
        }

        /// <summary>
        /// 重新设置各菜单的名称(国际化) 
        /// </summary>
        /// <param name="Language">要切换为的语言</param>
        private void ChangeLanguage(string Language)
        {
            General.TryCatch(() =>
            {
                if (!(Language == FixedString.LANGUAGE_CN_CN || Language == FixedString.LANGUAGE_EN_EN || Language == FixedString.LANGUAGE_CN_EN || Language == FixedString.LANGUAGE_EN_CN))
                {
                    return;
                }

                //中英文状态下拉选项正确切换
                if (Language == FixedString.LANGUAGE_CN_EN)
                {
                    Language = FixedString.LANGUAGE_EN_EN;
                }

                if (Language == FixedString.LANGUAGE_EN_CN)
                {
                    Language = FixedString.LANGUAGE_CN_CN;
                }

                //中英文模式下ToolBox设置
                if (Language == FixedString.LANGUAGE_EN_EN)
                {
                    General.FtaProgram.String = StringModel.GetENFTAString();
                    ribbonGalleryBarItem_GraphicTool.Gallery.DistanceBetweenItems = 15;
                    ribbonGalleryBarItem_GraphicTool.Gallery.ColumnCount = 7;
                    ribbonGalleryBarItem_GraphicTool.Gallery.RowCount = 1;
                }
                else if (Language == FixedString.LANGUAGE_CN_CN)
                {
                    General.FtaProgram.String = StringModel.GetCNFTAString();
                    ribbonGalleryBarItem_GraphicTool.Gallery.DistanceBetweenItems = 15;
                    ribbonGalleryBarItem_GraphicTool.Gallery.ColumnCount = 7;
                    ribbonGalleryBarItem_GraphicTool.Gallery.RowCount = 1;
                }

                //中英文模式Ribbon菜单部分控件大小分开设置
                this.ChangeRibbonControlSize(Language);

                //菜单中英文文本赋值
                this.barButtonItem_CreateNew.Caption = General.FtaProgram.String.New;
                this.barButtonItem_CreateNew_Project.Caption = General.FtaProgram.String.Project;
                this.barButtonItem_CreateNew_System.Caption = General.FtaProgram.String.System;
                this.barButtonItem_ToolNew.Caption = General.FtaProgram.String.System;
                this.barSubItem_OpenRecentFiles.Caption = General.FtaProgram.String.Open;
                this.barButtonItem_Open_LocalProject.Caption = General.FtaProgram.String.LocalProject;
                this.barButtonItem_Open_ASPECTProject.Caption = General.FtaProgram.String.CurrentAspectProject;
                this.barButtonItem_SaveAll.Caption = General.FtaProgram.String.SaveAll;
                this.barButtonItem_SaveAs.Caption = General.FtaProgram.String.SaveAs;
                this.barButtonItem_Exit.Caption = General.FtaProgram.String.Exit;
                barButtonItem_Save.Caption = General.FtaProgram.String.Save;
                barButtonItem_ToolSave.Caption = General.FtaProgram.String.Save;
                barSubItem_OpenRecentFiles.Caption = General.FtaProgram.String.OpenRecentFiles;
                barButtonItem_OpenProject.Caption = General.FtaProgram.String.OpenProject;
                barButtonItem_OpenFaultTree.Caption = General.FtaProgram.String.OpenFaultTree;
                barButtonItem_ToolOpen.Caption = General.FtaProgram.String.OpenFaultTree;
                this.barEditItem_Skin.Caption = General.FtaProgram.String.Skin;
                this.barButtonItem_SetPassword.Caption = General.FtaProgram.String.SetPassword;
                this.barButtonItem_PrintPreview.Caption = General.FtaProgram.String.PrintPreview;
                this.barButtonItem_PrintSelected.Caption = General.FtaProgram.String.PrintSelectedRecords;
                this.barButtonItem_PrintTable.Caption = General.FtaProgram.String.PrintTable;
                this.barButtonItem_PrintSetting.Caption = General.FtaProgram.String.PrintSettting;
                this.File_Open_System.Caption = General.FtaProgram.String.System;
                this.ribbonGalleryBarItem_GraphicTool.Caption = General.FtaProgram.String.GraphicTools;

                if (ribbonGalleryBarItem_GraphicTool.Gallery.Groups.Count == 1)
                {
                    ribbonGalleryBarItem_GraphicTool.Gallery.Groups[0].Caption = General.FtaProgram.String.BasicShape;
                }

                foreach (GalleryItem item in this.ribbonGalleryBarItem_GraphicTool.Gallery.GetAllItems())
                {
                    if (item.Value != null && item.Value.GetType() == typeof(string))
                    {
                        var typeName = General.GetKeyName(item.Value as string);
                        if (typeName == "")
                        {
                            continue;
                        }

                        DrawType type = DrawType.BasicEvent;
                        if (item.Value.ToString() == "LinkGate")
                        {
                            type = DrawType.BasicEvent;
                            item.Caption = General.FtaProgram.String.LinkGate;
                            item.Hint = General.FtaProgram.String.LinkGate;
                        }
                        else
                        {
                            type = (DrawType)Enum.Parse(typeof(DrawType), typeName);

                            switch (type)
                            {
                                case DrawType.RemarksGate:
                                    item.Caption = General.FtaProgram.String.RemarksGate;
                                    item.Hint = General.FtaProgram.String.RemarksGate;
                                    break;
                                case DrawType.AndGate:
                                    item.Caption = General.FtaProgram.String.AndGate;
                                    item.Hint = General.FtaProgram.String.AndGate;
                                    break;
                                case DrawType.OrGate:
                                    item.Caption = General.FtaProgram.String.OrGate;
                                    item.Hint = General.FtaProgram.String.OrGate;
                                    break;
                                case DrawType.BasicEvent:
                                    item.Caption = General.FtaProgram.String.BasicEvent;
                                    item.Hint = General.FtaProgram.String.BasicEvent;
                                    break;
                                case DrawType.UndevelopedEvent:
                                    item.Caption = General.FtaProgram.String.UndevelopedEvent;
                                    item.Hint = General.FtaProgram.String.UndevelopedEvent;
                                    break;
                                case DrawType.TransferInGate:
                                    item.Caption = General.FtaProgram.String.TransferInGate;
                                    item.Hint = General.FtaProgram.String.TransferInGate;
                                    break;
                                case DrawType.HouseEvent:
                                    item.Caption = General.FtaProgram.String.HouseEvent;
                                    item.Hint = General.FtaProgram.String.HouseEvent;
                                    break;
                                case DrawType.ConditionEvent:
                                    item.Caption = General.FtaProgram.String.ConditionEvent;
                                    item.Hint = General.FtaProgram.String.ConditionEvent;
                                    break;
                                case DrawType.PriorityAndGate:
                                    item.Caption = General.FtaProgram.String.PriorityAndGate;
                                    item.Hint = General.FtaProgram.String.PriorityAndGate;
                                    break;
                                case DrawType.XORGate:
                                    item.Caption = General.FtaProgram.String.XORGate;
                                    item.Hint = General.FtaProgram.String.XORGate;
                                    break;
                                //case DrawType.InhibitGate:
                                //    item.Caption = General.FtaProgram.String.InhibitGate;
                                //    item.Hint = General.FtaProgram.String.InhibitGate;
                                //    break;
                                case DrawType.VotingGate:
                                    item.Caption = General.FtaProgram.String.VotingGate;
                                    item.Hint = General.FtaProgram.String.VotingGate;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                this.barButtonItem_PreviousRepeatEvent.Caption = General.FtaProgram.String.PreviousRepeatEvent;
                this.barButtonItem_NextRepeatEvent.Caption = General.FtaProgram.String.NextRepeatEvent;
                this.barButtonItem_Bold.Caption = General.FtaProgram.String.Bold;
                this.barButtonItem_Italic.Caption = General.FtaProgram.String.Italic;
                this.barButtonItem_Underline.Caption = General.FtaProgram.String.Underline;
                this.barButtonItem_LeftAlign.Caption = General.FtaProgram.String.AlignLeft;
                this.barButtonItem_CenterAlign.Caption = General.FtaProgram.String.AlignCenter;
                this.barButtonItem_RightAlign.Caption = General.FtaProgram.String.AlignRight;
                this.barButtonItem_Cut.Caption = General.FtaProgram.String.Cut;
                barButtonItem_MenuCut.Caption = General.FtaProgram.String.Cut;
                barButtonItem_ShapeEdit.Caption = General.FtaProgram.String.Edit;
                this.barButtonItem_Copy.Caption = General.FtaProgram.String.Copy;
                barSubItem_Copy.Caption = General.FtaProgram.String.Copy;
                barButtonItem_MenuCopy.Caption = General.FtaProgram.String.Copy;
                barButtonItem_MenuCopyCurrentView.Caption = General.FtaProgram.String.CopyCurrentView;
                barButtonItem_MenuCopyCurrentSelected.Caption = General.FtaProgram.String.CopyCurrentSelected;
                this.barButtonItem_Paste.Caption = General.FtaProgram.String.Paste;
                this.barButtonItem_Cancel.Caption = General.FtaProgram.String.Undo;
                this.barButtonItem_Redo.Caption = General.FtaProgram.String.Redo;
                ribbonPageGroup_FindAndReplace.Text = General.FtaProgram.String.FindAndReplace;
                this.barButtonItem_FindAndReplace.Caption = General.FtaProgram.String.FindAndReplace;
                this.barButtonItem_FindAndReplaceCustom.Caption = General.FtaProgram.String.FindAndReplaceCustom;
                this.barButtonItem_CheckWord.Caption = General.FtaProgram.String.SpellingCheck;
                this.barButtonItem_DiagramTable.Caption = General.FtaProgram.String.Graph;
                this.barButtonItem_GolbalUpdate.Caption = General.FtaProgram.String.GlobalChange;
                this.Bbi_Import.Caption = General.FtaProgram.String.Import;
                this.Bbi_Export.Caption = General.FtaProgram.String.Export;
                this.barButtonItem_Filter.Caption = General.FtaProgram.String.Filter;
                this.Bci_Ruler.Caption = General.FtaProgram.String.Ruler;
                this.Bsi_Ruler.Caption = General.FtaProgram.String.Ruler;
                this.Bci_Grid.Caption = General.FtaProgram.String.Grid;
                this.Bsi_Grid.Caption = General.FtaProgram.String.Grid;
                this.Bci_PageBreak.Caption = General.FtaProgram.String.PageBreaks;
                this.Bsi_PageBreaks.Caption = General.FtaProgram.String.PageBreaks;
                this.Bci_CanvasFillMode.Caption = General.FtaProgram.String.CanvasFillMode;
                this.Bsi_CanvasFillMode.Caption = General.FtaProgram.String.CanvasFillMode;
                this.Bci_PageScrollMode.Caption = General.FtaProgram.String.PageScrollMode;
                this.Bsi_PageScrollMode.Caption = General.FtaProgram.String.PageScrollMode;
                this.barEditItem_ShapeBackColor.Caption = General.FtaProgram.String.ShapeColor;
                this.Bsi_ShapeBackColor.Caption = General.FtaProgram.String.ShapeColor;
                this.Bei_ShapeRectWidth.Caption = General.FtaProgram.String.ShapeRectWidth;
                this.Bsi_ShapeRectWidth.Caption = General.FtaProgram.String.ShapeRectWidth;
                this.Bei_UnUse.Caption = General.FtaProgram.String.IdRectHeight;
                this.Bei_ShapeRectHeight.Caption = General.FtaProgram.String.ShapeRectHeight;
                this.Bsi_ShapeRectHeight.Caption = General.FtaProgram.String.ShapeRectHeight;
                this.Bei_SymbolSize.Caption = General.FtaProgram.String.SymbolRectSize;
                this.Bsi_SymbolSize.Caption = General.FtaProgram.String.SymbolRectSize;
                this.Bsi_ShapeFontName.Caption = General.FtaProgram.String.FontName;
                this.Bsi_ShapeFontSize.Caption = General.FtaProgram.String.FontSize;
                this.barButtonItem_PasteRepeatEvent.Caption = General.FtaProgram.String.PasteRepeatedEvent;
                this.barEditItem_ShapeBackColor_RepeatEvent.Caption = General.FtaProgram.String.RepeatedEventColor;
                this.Bsi_ShapeBackColor_RepeatEvent.Caption = General.FtaProgram.String.RepeatedEventColor;
                this.barEditItem_ShapeBackColor_TrueGate.Caption = General.FtaProgram.String.TrueGateColor;
                this.Bsi_ShapeBackColor_TrueGate.Caption = General.FtaProgram.String.TrueGateColor;
                this.barEditItem_ShapeBackColor_FalseGate.Caption = General.FtaProgram.String.FalseGateColor;
                this.Bsi_ShapeBackColor_FalseGate.Caption = General.FtaProgram.String.FalseGateColor;
                this.barEditItem_ShapeBackColor_Selected.Caption = General.FtaProgram.String.SelectedShapeColor;
                this.Bsi_ShapeBackColor_Selected.Caption = General.FtaProgram.String.SelectedShapeColor;

                this.barButtonItem_FTA.Caption = General.FtaProgram.String.PrintingSelection_Item1;
                this.barButtonItem_Cutset.Caption = General.FtaProgram.String.PrintingSelection_Item2;
                this.barButtonItem_FTADiagram.Caption = General.FtaProgram.String.PrintingSelection_Item3;

                this.Bbi_Renumber.Caption = General.FtaProgram.String.RenumberFaultTree;
                this.barButtonItem_ToolRenumber.Caption = General.FtaProgram.String.RenumberFaultTree;
                this.Bbi_QuickRenumber.Caption = General.FtaProgram.String.QuickRenumber;
                barButtonItem_Check.Caption = General.FtaProgram.String.Check;
                barButtonItem_ToolCheck.Caption = General.FtaProgram.String.Check;
                this.barButtonItem_Refresh.Caption = General.FtaProgram.String.Refresh;
                this.barButtonItem_ZoomIn.Caption = General.FtaProgram.String.ZoomIn;
                this.barButtonItem_ZoomOut.Caption = General.FtaProgram.String.ZoomOut;
                this.barButtonItem_FTACalculate.Caption = General.FtaProgram.String.FTACalculate;
                this.barButtonItem_CalSelected.Caption = General.FtaProgram.String.FTACalculateSelected;
                this.barButtonItem_CalSelectedMenu.Caption = General.FtaProgram.String.FTACalculateSelected;
                this.barButtonItem5.Caption = General.FtaProgram.String.CusetStr;
                this.barButtonItem_ToolCal.Caption = General.FtaProgram.String.FTACalculate;
                this.barButtonItem_Insert.Caption = General.FtaProgram.String.Insert;
                this.barButtonItem_DeleteNodesDiagram.Caption = General.FtaProgram.String.Delete;
                this.barButtonItem_InsertRepeatEvent.Caption = General.FtaProgram.String.InsertRepeatedEvent;
                this.barButtonItem_AddLinkGate.Caption = General.FtaProgram.String.AddLinkGate;
                this.barButtonItem_UpDateLinkGate.Caption = General.FtaProgram.String.UpDateLinkGate;
                this.barButtonItem_BreakIntoTransfer.Caption = General.FtaProgram.String.BreakintoTransfer;
                this.barButtonItem_CollapseTransfer.Caption = General.FtaProgram.String.CollapseTransfer;
                this.barButtonItem_MenuBreakIntoTransfer.Caption = General.FtaProgram.String.BreakintoTransfer;
                this.barButtonItem_MenuCollapseTransfer.Caption = General.FtaProgram.String.CollapseTransfer;
                this.barButtonItem_TransferTo.Caption = General.FtaProgram.String.TransferTo;
                this.barSubItem_TransferTo.Caption = General.FtaProgram.String.TransferTo;
                ribbonPageGroup_GateSettings.Text = General.FtaProgram.String.GateSettings;
                ribbonPageGroup_Transfer.Text = General.FtaProgram.String.TransferTo;
                ribbonPageGroup_Excel.Text = General.FtaProgram.String.ImportExportGtoup;
                this.ribbonGalleryBarItem_HighLightCutSet.Caption = General.FtaProgram.String.HighlightCutSets;
                barButtonItem_HighLightCutSets.Caption = General.FtaProgram.String.HighlightCutSets;
                barButtonItem_StartPage.Caption = General.FtaProgram.String.StartPage;
                this.barButtonItem_Project_DeleteSystem.Caption = General.FtaProgram.String.DeleteSystem;
                this.barButtonItem_Project_DeleteProject.Caption = General.FtaProgram.String.DeleteProject;
                this.barButtonItem_Project_CreateProject.Caption = General.FtaProgram.String.NewProject;
                this.barButtonItem_Project_CreateSystem.Caption = General.FtaProgram.String.NewSystem;

                this.barButtonItem_GroupLevelNew.Caption = General.FtaProgram.String.NewFolder;
                this.barButtonItem_GroupLevelRename.Caption = General.FtaProgram.String.RenameFolder;
                this.barButtonItem_GroupLevelDelete.Caption = General.FtaProgram.String.DeleteFolder;

                this.barButtonItem_FaultTreeCopy.Caption = General.FtaProgram.String.FaultTreeCopy;
                this.barButtonItem_FaultTreePaste.Caption = General.FtaProgram.String.FaultTreePaste;

                this.barButtonItem_OpenDir.Caption = General.FtaProgram.String.OpenDir;
                this.barButtonItem_CloseDir.Caption = General.FtaProgram.String.CloseDir;
                this.barButtonItem_Project_RemoveProject.Caption = General.FtaProgram.String.RemoveProject;
                this.barButtonItem_Project_RemoveSystem.Caption = General.FtaProgram.String.RemoveSystem;
                this.barButtonItem_Project_RenameProject.Caption = General.FtaProgram.String.RenameProject;
                this.barButtonItem_Project_RenameSystem.Caption = General.FtaProgram.String.RenameSystem;
                //this.ribbonControl_FTA.ApplicationButtonText = General.FtaProgram.String.RibbonFile;
                this.ribbonPage_Modeling.Text = General.FtaProgram.String.Modeling;
                this.ribbonPage_Edit.Text = General.FtaProgram.String.Edit;
                this.ribbonPage_View.Text = General.FtaProgram.String.View;
                this.ribbonPage_Help.Text = General.FtaProgram.String.Help;
                this.ribbonPageGroup_Edit.Text = General.FtaProgram.String.Edit;
                this.ribbonPageGroup_Parameters.Text = General.FtaProgram.String.Parameters;
                this.ribbonPageGroup_Color.Text = General.FtaProgram.String.Color;
                this.ribbonPageGroup_Report.Text = General.FtaProgram.String.Report;
                this.ribbonPageGroup_ToolBox.Text = General.FtaProgram.String.ToolBox;
                this.ribbonPageGroup_View.Text = General.FtaProgram.String.View;
                this.ribbonPageGroup_Calculate.Text = General.FtaProgram.String.Calculation;
                this.ribbonPageGroup_Skin.Text = General.FtaProgram.String.Skin;
                ribbonPageGroup_Project.Text = General.FtaProgram.String.Project;
                barButtonItem_FoldProject.Caption = General.FtaProgram.String.FoldProject;
                barButtonItem_ExpandProject.Caption = General.FtaProgram.String.ExpandProject;
                barButtonItem_Properties.Caption = General.FtaProgram.String.Properties;
                ribbonPageGroup_CloseProperties.Text = General.FtaProgram.String.Properties;
                barButtonItem_CloseProperties.Caption = General.FtaProgram.String.CloseProperties;
                ribbonPage_Properties.Text = General.FtaProgram.String.Properties;

                this.ribbonPageGroup_Canvas.Text = General.FtaProgram.String.Canvas;
                this.dockPanel_Project.Text = General.FtaProgram.String.ProjectNavigator;
                this.groupControl_Project.Text = General.FtaProgram.String.Project;
                this.dockPanel_FTATable.Text = General.FtaProgram.String.FTATable;
                this.dockPanel_FTADiagram.Text = General.FtaProgram.String.FTADiagram;
                ribbonPageGroup_Font.Text = General.FtaProgram.String.FontName;

                ribbonPageGroup_Help.Text = General.FtaProgram.String.Help;
                ribbonPageGroup_Soft.Text = General.FtaProgram.String.Soft;
                barButtonItem_UserManual.Caption = General.FtaProgram.String.UserManual;
                barButtonItem_ShowKeymap.Caption = General.FtaProgram.String.ShowKeymap;
                barButtonItem_CheckforUpdates.Caption = General.FtaProgram.String.CheckforUpdates;
                barButtonItem_AboutSmarTree.Caption = General.FtaProgram.String.AboutSmarTree;

                this.barEditItem_Language.Caption = General.FtaProgram.String.Language;
                ribbonPageGroup_Language.Text = General.FtaProgram.String.Language;
                this.barButtonItem_InsertLevel.Caption = General.FtaProgram.String.InsertLevel;
                this.barButtonItem_TopGatePos.Caption = General.FtaProgram.String.TopGatePos;
                barButtonItem_PosParent.Caption = General.FtaProgram.String.PosParent;
                barButtonItem_PosChild.Caption = General.FtaProgram.String.PosChild;
                this.barSubItem_ChangeGateType.Caption = General.FtaProgram.String.ChangeGateType;

                this.barButtonItem_FTATable_Insertinput.Caption = General.FtaProgram.String.InsertNode;
                this.Blci_Insert.Caption = General.FtaProgram.String.InsertNode;
                this.barButtonItem_FTATable_Insertnewtopleveldoor.Caption = General.FtaProgram.String.InsertTopNode;
                this.barButtonItem_FTATable_Cutrecursivechildnode.Caption = General.FtaProgram.String.CutNodes;
                this.barButtonItem_FTATable_Copysinglenode.Caption = General.FtaProgram.String.CopyNode;
                this.barButtonItem_FTATable_Copyrecursivechildnode.Caption = General.FtaProgram.String.CopyNodes;
                this.barButtonItem_FTATable_Paste.Caption = General.FtaProgram.String.Paste;
                this.barButtonItem_FTATable_PasteRepeatedEvent.Caption = General.FtaProgram.String.PasteRepeatedEvent;
                this.barButtonItem_FTATable_PasteastopGate.Caption = General.FtaProgram.String.PasteAsTopNode;
                barSubItem_Paste.Caption = General.FtaProgram.String.Paste;
                barButtonItem_MenuPaste.Caption = General.FtaProgram.String.Paste;
                barButtonItem_MenuPasteRepeated.Caption = General.FtaProgram.String.PasteRepeatedEvent;

                this.barButtonItem_FTATable_Cancelcopyorcut.Caption = General.FtaProgram.String.CancelCopyOrCut;
                this.barButtonItem_DeleteNodesTable.Caption = General.FtaProgram.String.DeleteNodes;
                this.barButtonItem_DeleteTransferTable.Caption = General.FtaProgram.String.DeleteNodesAndTransfer;
                this.barButtonItem_DeleteTransferDiagram.Caption = General.FtaProgram.String.DeleteNodesAndTransfer;
                this.barButtonItem_DeleteTransferRibbon.Caption = General.FtaProgram.String.DeleteNodesAndTransfer;
                this.barButtonItem_DeleteNodeTable.Caption = General.FtaProgram.String.DeleteNode;
                this.barButtonItem_DeleteTopTable.Caption = General.FtaProgram.String.DeleteTop;
                this.barButtonItem_DeleteLevelTable.Caption = General.FtaProgram.String.DeleteLevel;
                this.barButtonItem_FTATable_Expandallnodes.Caption = General.FtaProgram.String.ExpandNodes;
                this.barButtonItem_FTATable_Collapseallnodes.Caption = General.FtaProgram.String.CollapseNodes;
                this.barButtonItem_FTATable_Freezecolumn.Caption = General.FtaProgram.String.FreezeColumn;
                this.barButtonItem_FTATable_Unfreezecolumns.Caption = General.FtaProgram.String.UnfreezeColumns;

                this.barEditItem_LineColor.Caption = General.FtaProgram.String.LineColor;
                this.Bsi_LineColor.Caption = General.FtaProgram.String.LineColor;
                this.barEditItem_LineStyle.Caption = General.FtaProgram.String.LineStyle;
                this.Bsi_LineStyle.Caption = General.FtaProgram.String.LineStyle;
                this.barEditItem_ArrowStyle.Caption = General.FtaProgram.String.ArrowStyle;
                this.Bsi_ArrowStyle.Caption = General.FtaProgram.String.ArrowStyle;
                this.barEditItem_ArrowSize.Caption = General.FtaProgram.String.ArrowSize;
                this.Bsi_ArrowSize.Caption = General.FtaProgram.String.ArrowSize;
                this.barStaticItem_Bci_ScaleAble.Caption = General.FtaProgram.String.Scale;
                this.Bsi_MoveAble.Caption = General.FtaProgram.String.Move;
                this.ribbonPageGroup_FTATable.Text = General.FtaProgram.String.FTATableEditor;
                this.ribbonPageGroup_FTA.Text = General.FtaProgram.String.FTATableEditor;
                this.barCheckItem_ShowIndicator.Caption = General.FtaProgram.String.Indicator;
                this.Bsi_ShowIndicator.Caption = General.FtaProgram.String.Indicator;
                this.barEditItem_IndicatorTopGateColor.Caption = General.FtaProgram.String.IndicatorTopGate;
                this.Bsi_IndicatorTopGateColor.Caption = General.FtaProgram.String.IndicatorTopGate;
                this.barEditItem_IndicatorTransInGateColor.Caption = General.FtaProgram.String.IndicatorTransInGate;
                this.Bsi_IndicatorTransInGateColor.Caption = General.FtaProgram.String.IndicatorTransInGate;
                this.barEditItem_CutSetColor.Caption = General.FtaProgram.String.CutSetColor;
                this.Bsi_CutSetColor.Caption = General.FtaProgram.String.CutSetColor;
                this.ribbonPageGroup_Appearance.Text = General.FtaProgram.String.WindowLayout;
                this.barButtonItem_ShowFullScreen.Caption = General.FtaProgram.String.ShowFullScreen;
                this.barButtonItem_HideShowToolbar.Caption = General.FtaProgram.String.HideShowToolbar;
                this.barButtonItem_HideShowProjectNavigator.Caption = General.FtaProgram.String.HideShowProjectNavigator;
                this.barSubItem_Windows.Caption = General.FtaProgram.String.Window;

                //中英文切换时，可能存在的部分Dock和小界面中英文切换
                //开始页中英文切换
                if (General.StartPage != null)
                {
                    General.StartPage.ReloadRecentFiles();
                }
                //检查界面中英文切换
                if (General.SelectCheckErr != null)
                {
                    General.SelectCheckErr.RefreshText();
                }
                //高亮割级界面中英文切换
                if (General.ShowCut != null)
                {
                    General.ShowCut.RefreshText();
                }
                //查找替换界面中英文切换
                if (General.FindAndReplace != null)
                {
                    General.FindAndReplace.RefreshText();
                }

                //中英文切换之后自动保存设置
                SaveSettings();
            });
        }

        /// <summary>
        /// 总的初始化方法，注册事件，菜单显示，画布等等
        /// </summary>
        private void Init()
        {
            this.ftaDiagram = new FtaDiagram(General.DiagramControl, General.FtaProgram);
            this.ftaTable = new FtaTable(General.TableControl, General.FtaProgram);
            this.ftaRibbon = new FtaRibbon(General.FtaProgram);
            General.ftaDiagram = this.ftaDiagram;
            General.ftaTable = this.ftaTable;
            //General.Logger.Info("wahaha");

            //初始化主菜单（左上角菜单，添加事件）
            Init_ApplicationMenu();

            //初始化Project面板
            InitializeProjectControl();

            //初始化FTA表
            InitializeTableControl();

            //初始化FTA图事件
            InitializeDiagramControl();

            //Ribbon菜单中和右键菜单公用的功能和一些新功能事件绑定
            InitializeRibbonDiagramControl();

            //初始化Ribbon菜单项
            Init_Ribbon_Start_Edit();
            Init_Ribbon_Start_Excel();
            Init_Ribbon_Start_FTAStyle();
            Init_Ribbon_Start_Report();
            Init_Ribbon_Tool_FTA();
            Init_Ribbon_Tool_ToolBox();
            Init_Ribbon_Tool_View();

            //故障树计算和检查按钮事件绑定
            barButtonItem_ToolCal.ItemClick += this.ftaRibbon.RibbionEvents.BarButtonItem_FTACalculate_ItemClick;
            this.barButtonItem_FTACalculate.ItemClick += this.ftaRibbon.RibbionEvents.BarButtonItem_FTACalculate_ItemClick;

            barButtonItem_CalSelectedMenu.ItemClick += this.ftaRibbon.RibbionEvents.BarButtonItem_FTACalculateSelected_ItemClick;
            barButtonItem_CalSelected.ItemClick += this.ftaRibbon.RibbionEvents.BarButtonItem_FTACalculateSelected_ItemClick;

            this.barButtonItem_Check.ItemClick += this.ftaRibbon.RibbionEvents.BarButtonItem_FTACalculateOnlyCheck_ItemClick;
            barButtonItem_ToolCheck.ItemClick += this.ftaRibbon.RibbionEvents.BarButtonItem_FTACalculateOnlyCheck_ItemClick;

            //初始化部分Ribbon菜单中的下拉选项
            Init_Ribbon_Setting_Skin();
            Init_Ribbon_Setting_FTATableEditor();
            Init_Ribbon_Setting_Canvas();
            Init_Ribbon_Setting_WindowLayout();
            Init_Ribbon_Setting_Language();

            General.ProjectControl.FocusedNode = null;

            //部分Ribbon菜单初始状态禁用
            barButtonItem_InsertLevel.Enabled = false;
            barButtonItem_TopGatePos.Enabled = false;
            barButtonItem_PosParent.Enabled = false;
            barButtonItem_PosChild.Enabled = false;
            barSubItem_ChangeGateType.Enabled = false;
            ribbonGalleryBarItem_GraphicTool.Enabled = true;
            barButtonItem_MenuCut.Enabled = false;
            barButtonItem_ShapeEdit.Enabled = false;
            barSubItem_Copy.Enabled = false;
            barSubItem_Paste.Enabled = false;
            barSubItem_MenuDelete.Enabled = false;
            barButtonItem_FindAndReplace.Enabled = false;
            barButtonItem_FindAndReplaceCustom.Enabled = false;
            barButtonItem_Check.Enabled = false;
            Bbi_Renumber.Enabled = false;
            barButtonItem_FTACalculate.Enabled = false;
            barButtonItem_HighLightCutSets.Enabled = false;
            barButtonItem_ExportToModel.Enabled = false;
            barButtonItem_LoadFromModel.Enabled = false;
            barButtonItem_BasicLib.Enabled = true;
            barButtonItem_AddToBasicEventLibrary.Enabled = false;
            barButtonItem_SynchronizeFromBasicEventLibrary.Enabled = false;
            barButtonItem_InsertFromBasicEventLibrary.Enabled = false;
            Bbi_Import.Enabled = false;
            Bbi_Export.Enabled = false;
            barButtonItem_FTA.Enabled = false;
            barButtonItem_FTADiagram.Enabled = false;
            barButtonItem_Cutset.Enabled = false;
        }

        /// <summary>
        /// 把当前故障树数据保存到本地
        /// </summary>
        public void SaveDataFaultTree(ProjectModel PM, SystemModel sys)
        {
            if (sys != null && PM != null)
            {
                if (Directory.Exists(PM.ProjectPath) == false)
                {
                    Directory.CreateDirectory(PM.ProjectPath);
                }

                FileInfo filesys = new FileInfo(PM.ProjectPath + "\\" + sys.SystemName + FixedString.APP_EXTENSION);
                File.WriteAllText(filesys.FullName, Newtonsoft.Json.JsonConvert.SerializeObject(sys), Encoding.UTF8);

                //系统模型虚拟化应用
                sys.ApplyVirtualization();

                //重新刷新图形的选取状态
                General.CopyCutObject = null;
                General.CopyCutSystem = null;
                treeList_FTATable.Refresh();
                if (treeList_FTATable.FocusedNode != null && treeList_FTATable.FocusedNode.GetValue(FixedString.COLUMNAME_DATA) != null
                    && treeList_FTATable.FocusedNode.GetValue(FixedString.COLUMNAME_DATA).GetType() == typeof(DrawData))
                {
                    ftaDiagram.SelectedData.Clear();
                    ftaDiagram.SelectedData.Add(treeList_FTATable.FocusedNode.GetValue(FixedString.COLUMNAME_DATA) as DrawData);
                    ftaDiagram.Refresh(true);
                }
            }
        }

        public void CloseDB()
        {
            try
            {
                ConnectSever.CloseDB();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 把工程数据保存到本地
        /// </summary>
        public void SaveData()
        {
            foreach (ProjectModel PM in General.FtaProgram.Projects)
            {
                if (Directory.Exists(PM.ProjectPath) == false)
                {
                    Directory.CreateDirectory(PM.ProjectPath);
                }
                foreach (SystemModel sys in PM.Systems)
                {
                    FileInfo filesys = new FileInfo(PM.ProjectPath + "\\" + sys.SystemName + FixedString.APP_EXTENSION);
                    File.WriteAllText(filesys.FullName, Newtonsoft.Json.JsonConvert.SerializeObject(sys), Encoding.UTF8);

                    //系统模型虚拟化应用
                    sys.ApplyVirtualization();
                }
            }

            //保存除Project信息之外的其他内容到公共Data.json文件
            ProjectModel[] preP = new Model.Data.ProjectModel[General.FtaProgram.Projects.Count];
            General.FtaProgram.Projects.CopyTo(preP);
            General.FtaProgram.Projects.Clear();

            File.WriteAllText(this.savingDataPath, Newtonsoft.Json.JsonConvert.SerializeObject(General.FtaProgram), Encoding.UTF8);

            General.FtaProgram.Projects.AddRange(preP);

            //重新刷新图形的选取状态
            General.CopyCutObject = null;
            General.CopyCutSystem = null;
            treeList_FTATable.Refresh();
            if (treeList_FTATable.FocusedNode != null && treeList_FTATable.FocusedNode.GetValue(FixedString.COLUMNAME_DATA) != null
                && treeList_FTATable.FocusedNode.GetValue(FixedString.COLUMNAME_DATA).GetType() == typeof(DrawData))
            {
                ftaDiagram.SelectedData.Clear();
                ftaDiagram.SelectedData.Add(treeList_FTATable.FocusedNode.GetValue(FixedString.COLUMNAME_DATA) as DrawData);
                ftaDiagram.Refresh(true);
            }
        }

        /// <summary>
        /// 把通用设置保存到本地
        /// </summary>
        public void SaveSettings()
        {
            if (General.FtaProgram != null && General.FtaProgram.Projects.Count > 0)
            {
                //保存除Project信息之外的其他内容到公共Data.json文件
                ProjectModel[] preP = new Model.Data.ProjectModel[General.FtaProgram.Projects.Count];
                General.FtaProgram.Projects.CopyTo(preP);
                General.FtaProgram.Projects.Clear();

                File.WriteAllText(this.savingDataPath, Newtonsoft.Json.JsonConvert.SerializeObject(General.FtaProgram), Encoding.UTF8);

                General.FtaProgram.Projects.AddRange(preP);
            }
        }

        /// <summary>
        /// 读取本地数据到内存
        /// </summary>
        public void LoadData(bool isProject, string LoadPath, bool LoadXml = true)
        {
            //重新加载布局，工程列表，FTA表，FTA图，三部分面板全部显示，支持中英文切换
            if (LoadXml)
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

            //保存前检查
            if (this.HaveVirtualizationSystem())
            {
                if (MsgBox.Show(General.FtaProgram.String.ConfirmSavingMessage, General.FtaProgram.String.ConfirmTitleSave, MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1, ContentAlignment.MiddleCenter, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SaveData();
                }
            }

            if (isProject)//打开工程
            {
                foreach (ProjectModel PM in General.FtaProgram.Projects)
                {
                    if (PM.ProjectPath == LoadPath)
                    {
                        PM.Systems.Clear();
                        //定位工程，并刷新
                        string[] syss1 = Directory.GetFiles(LoadPath);

                        foreach (string sys in syss1)
                        {
                            if (new FileInfo(sys).Extension == FixedString.APP_EXTENSION)
                            {
                                SystemModel SM = General.GetSystemFromJson(sys);
                                PM.Systems.Add(SM);
                            }
                        }
                        General.FtaProgram.Initialize();
                        RecentFiles.AddProject(LoadPath);//最近文件对象添加
                        //因为这里图标是init完毕才有的
                        Project_Load(General.FtaProgram.Projects, LoadPath);

                        //重新加载最近文件
                        ReloadRecentFiles();
                        return;
                    }
                }

                //加载工程和工程文件夹下的所有故障树
                ProjectModel PMsys = new ProjectModel();
                PMsys.ProjectPath = LoadPath;
                PMsys.ProjectName = new DirectoryInfo(LoadPath).Name;
                PMsys.Systems = new List<SystemModel>();

                string[] syss = Directory.GetFiles(LoadPath);

                foreach (string sys in syss)
                {
                    if (new FileInfo(sys).Extension == FixedString.APP_EXTENSION)
                    {
                        SystemModel SM = General.GetSystemFromJson(sys);
                        PMsys.Systems.Add(SM);
                    }
                }

                General.FtaProgram.Projects.Add(PMsys);
                RecentFiles.AddProject(LoadPath);//最近文件对象添加
            }
            else//打开故障树
            {
                string LoadPathParnet = new FileInfo(LoadPath).DirectoryName;
                bool ProjectExist = false;
                foreach (ProjectModel PM in General.FtaProgram.Projects)
                {
                    if (PM.ProjectPath == LoadPathParnet)
                    {
                        PM.Systems.Clear();
                        //定位工程，并刷新
                        string[] syss1 = Directory.GetFiles(LoadPathParnet);

                        foreach (string sys in syss1)
                        {
                            if (new FileInfo(sys).Extension == FixedString.APP_EXTENSION)
                            {
                                SystemModel SM = General.GetSystemFromJson(sys);
                                PM.Systems.Add(SM);
                            }
                        }
                        ProjectExist = true;
                        RecentFiles.AddProject(PM.ProjectPath);//最近文件
                    }
                }

                //不存在工程节点的先创建工程，并加载出其下所有故障树
                if (ProjectExist == false)
                {
                    ProjectModel PMsys = new ProjectModel();
                    PMsys.ProjectPath = LoadPathParnet;
                    PMsys.ProjectName = new DirectoryInfo(LoadPathParnet).Name;
                    PMsys.Systems = new List<SystemModel>();

                    string[] syss = Directory.GetFiles(LoadPathParnet);

                    foreach (string sys in syss)
                    {
                        if (new FileInfo(sys).Extension == FixedString.APP_EXTENSION)
                        {
                            SystemModel SM = General.GetSystemFromJson(sys);
                            PMsys.Systems.Add(SM);
                        }
                    }

                    General.FtaProgram.Projects.Add(PMsys);

                    RecentFiles.AddProject(PMsys.ProjectPath);//最近文件
                }

                RecentFiles.AddFaultTree(LoadPath);//最近文件 
            }

            General.FtaProgram.Initialize();
            //因为这里图标是init完毕才有的
            Project_Load(General.FtaProgram.Projects, LoadPath);

            //重新加载最近文件
            ReloadRecentFiles();
        }

        /// <summary>
        /// 初始化主菜单（左上角菜单，添加事件）
        /// </summary>
        private void Init_ApplicationMenu()
        {
            //新建项目
            barButtonItem_CreateNew_Project.ItemClick += ProjectItemClick;

            //新建系统
            barButtonItem_CreateNew_System.ItemClick += ProjectItemClick;
            barButtonItem_ToolNew.ItemClick += ProjectItemClick;

            //保存程序
            barButtonItem_Save.ItemClick += ItemClick_BarButtonItem_ApplicationMenu;
            barButtonItem_SaveAll.ItemClick += ItemClick_BarButtonItem_ApplicationMenu;
            barButtonItem_ToolSave.ItemClick += ItemClick_BarButtonItem_ApplicationMenu;

            //在新建菜单弹出时，检查是否要禁用新建系统按钮
            barButtonItem_CreateNew.Popup += Popup_BarButtonItem_CreateNew;
        }

        /// <summary>
        /// ribbon的主菜单（左上角下拉大菜单）单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemClick_BarButtonItem_ApplicationMenu(object sender, ItemClickEventArgs e)
        {
            try
            {
                //保存所有
                if (e.Item == barButtonItem_SaveAll)
                {
                    if (General.FtaProgram == null || General.FtaProgram.Projects.Count == 0)
                    {
                        MsgBox.Show(General.FtaProgram.String.NoSavedAll);
                        return;
                    }
                    SplashScreenManager.ShowDefaultWaitForm(General.FtaProgram.String.SavingAll, " ");
                    SaveData();
                    if (SplashScreenManager.Default != null) SplashScreenManager.CloseDefaultWaitForm();
                }
                //保存当前故障树
                else if (e.Item == barButtonItem_Save || e.Item == barButtonItem_ToolSave)
                {
                    if (General.FtaProgram == null || General.FtaProgram.CurrentSystem == null)
                    {
                        MsgBox.Show(General.FtaProgram.String.NoSaved);
                        return;
                    }

                    SplashScreenManager.ShowDefaultWaitForm(General.FtaProgram.String.SavingFaultTree, " ");
                    SaveDataFaultTree(General.FtaProgram.CurrentProject, General.FtaProgram.CurrentSystem);
                    if (SplashScreenManager.Default != null) SplashScreenManager.CloseDefaultWaitForm();
                }
            }
            catch (Exception ex)
            {
                if (SplashScreenManager.Default != null) SplashScreenManager.CloseDefaultWaitForm();
                MsgBox.Show(FixedString.EXCEPTION + ex.Message);
            }
        }

        /// <summary>
        /// 在新建菜单弹出时，检查是否要禁用新建故障树按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Popup_BarButtonItem_CreateNew(object sender, EventArgs e)
        {
            General.TryCatch(() =>
            {
                if (treeList_Project.FocusedNode != null && treeList_Project.FocusedNode.Tag != null && treeList_Project.FocusedNode.Tag.GetType() == typeof(ProjectModel))
                {
                    barButtonItem_CreateNew_System.Enabled = true;
                    barButtonItem_ToolNew.Enabled = true;
                }
                else if (treeList_Project.FocusedNode != null && treeList_Project.FocusedNode.Tag == null)
                {
                    barButtonItem_CreateNew_System.Enabled = true;
                    barButtonItem_ToolNew.Enabled = true;
                }
                else
                {
                    barButtonItem_CreateNew_System.Enabled = false;
                    barButtonItem_ToolNew.Enabled = false;
                }
            });
        }

        /// <summary>
        /// 初始化FTA图形部分事件注册等
        /// </summary>
        private void InitializeDiagramControl()
        {
            this.ftaDiagram.DiagramEvents.RegisterDiagramEvent();

            foreach (BarItemLink link in General.DiagramMenuControl.ItemLinks)
            {
                //转到
                if (link.Item == General.TransferTo)
                {
                    General.TransferTo.GetItemData += GetItemData_BarButtonItem_TransferTo;
                }
                else if (link.Item == this.Blci_Insert)
                {
                    DrawData da = new DrawData();
                    da.Type = DrawType.AndGate;

                    this.Blci_Insert.ItemLinks.AddRange(this.GetCanInsertItems(da.GetAvailableTypeSource(new SystemModel(), General.FtaProgram.String).ToArray()));

                    foreach (BarItemLink item in this.Blci_Insert.ItemLinks)
                    {
                        item.Item.ItemClick += DiagramControlMenuClick;
                    }
                    link.Item.ItemClick += DiagramControlMenuClick;
                }
                //高亮割集
                else if (link.Item == General.HighLightCutSet)
                {
                    //子项单击事件
                    General.HighLightCutSet.GalleryItemClick += this.ftaDiagram.DiagramEvents.GalleryItemClick;

                    //子项移入事件
                    General.HighLightCutSet.GalleryItemHover += this.ftaDiagram.DiagramEvents.GalleryItemHover;

                    //子菜单关闭事件
                    General.HighLightCutSet.GalleryPopupClose += this.ftaDiagram.DiagramEvents.GalleryPopupClose_RibbonGalleryBarItem_HighLightCutSet;
                }
                else if (link.Item == General.Delete)
                {
                    foreach (BarItemLink item in this.Sub_DeleteDiagramNode.ItemLinks)
                    {
                        item.Item.ItemClick += DiagramControlMenuClick;
                    }
                    link.Item.ItemClick += DiagramControlMenuClick;
                }
                else
                {
                    link.Item.ItemClick += DiagramControlMenuClick;
                }
            }
        }

        /// <summary>
        /// Ribbon菜单中和右键菜单公用的功能和一些新功能事件绑定
        /// </summary>
        private void InitializeRibbonDiagramControl()
        {
            barButtonItem_MenuCopy.ItemClick += DiagramControlMenuClick;
            barButtonItem_MenuCopyCurrentView.ItemClick += DiagramControlMenuClick;
            barButtonItem_MenuCopyCurrentSelected.ItemClick += DiagramControlMenuClick;
            barButtonItem_MenuPaste.ItemClick += DiagramControlMenuClick;
            barButtonItem_MenuPasteRepeated.ItemClick += DiagramControlMenuClick;
            barButtonItem_MenuCut.ItemClick += DiagramControlMenuClick;
            barButtonItem_ShapeEdit.ItemClick += DiagramControlMenuClick;

            barButtonItem_DeleteTopRibbon.ItemClick += DiagramControlMenuClick;
            barButtonItem_DeleteLevelRibbon.ItemClick += DiagramControlMenuClick;
            barButtonItem_DeleteNodeRibbon.ItemClick += DiagramControlMenuClick;
            barButtonItem_DeleteNodesRibbon.ItemClick += DiagramControlMenuClick;
            barButtonItem_DeleteTransferRibbon.ItemClick += DiagramControlMenuClick;
            General.SubTransferTo.GetItemData += GetItemData_BarButtonItem_SubTransferTo;
            barButtonItem_MenuBreakIntoTransfer.ItemClick += DiagramControlMenuClick;
            barButtonItem_MenuCollapseTransfer.ItemClick += DiagramControlMenuClick;
        }

        /// <summary>
        /// 创建用来插入节点的菜单
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        private List<BarItem> GetCanInsertItems(string[] ids)
        {
            var result = new List<BarItem>(ids.Length);
            for (int i = 0; i < ids.Length; i++)
            {
                result.Add(new BarButtonItem { Caption = ids[i] });
            }
            return result;
        }

        /// <summary>
        /// 按指定(字节)长度截取字符串
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="len">字节长度</param>
        /// <returns></returns>
        private string CutStringByte(string str, int len)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            if (System.Text.Encoding.Default.GetByteCount(str) < len)
            {
                return str;
            }
            int i = 0;//字节数
            int j = 0;//实际截取长度
            foreach (char newChar in str)
            {
                if ((int)newChar > 127)
                {
                    //汉字
                    i += 2;
                }
                else
                {
                    i++;
                }

                if (i < len)
                    j++;
                else
                    break;
            }
            str = str.Substring(0, j) + "...";
            return str;
        }

        /// <summary>
        /// 自定义右键菜单"转到"的子项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetItemData_BarButtonItem_TransferTo(object sender, EventArgs e)
        {
            General.TryCatch(() =>
            {
                //取消上次注册的事件
                foreach (BarItemLink tmp in barButtonItem_TransferTo.ItemLinks)
                {
                    tmp.Item.ItemClick -= DiagramControlMenuClick;
                }
                barButtonItem_TransferTo.ItemLinks.Clear();

                if (ftaDiagram.SelectedData.Count == 1)
                {
                    DrawData selectedData = ftaDiagram.SelectedData.FirstOrDefault();
                    if (General.FtaProgram.CurrentSystem?.TranferGates?.ContainsKey(selectedData.Identifier) == true)
                    {
                        HashSet<DrawData> transfer = General.FtaProgram.CurrentSystem.TranferGates[selectedData.Identifier];
                        DrawData trans_True = transfer.Where(obj => obj.Type != DrawType.TransferInGate).FirstOrDefault();
                        if (trans_True != null)
                        {
                            List<BarButtonItem> sub_Bars = new List<BarButtonItem>();
                            //先把本体放到第一个
                            BarButtonItem sub_trans_True = new BarButtonItem();

                            // 按指定(字节)长度截取字符串
                            string Desc = selectedData.Identifier;
                            if (selectedData.Comment1 != null && selectedData.Comment1 != "")
                            {
                                Desc += " (" + CutStringByte(selectedData.Comment1, 40) + ")";
                            }

                            sub_trans_True.Caption = Desc;
                            sub_trans_True.Tag = trans_True;
                            sub_Bars.Add(sub_trans_True);

                            if (selectedData == trans_True) sub_trans_True.Visibility = BarItemVisibility.Never;

                            if (selectedData.Type != DrawType.TransferInGate)
                            {
                                foreach (DrawData tmp in transfer)
                                {
                                    if (tmp != trans_True)
                                    {
                                        BarButtonItem sub_trans = new BarButtonItem();
                                        if (tmp.Parent != null)
                                        {
                                            // 按指定(字节)长度截取字符串
                                            string Desc1 = tmp.Parent.Identifier;
                                            if (tmp.Parent.Comment1 != null && tmp.Parent.Comment1 != "")
                                            {
                                                Desc1 += " (" + CutStringByte(tmp.Parent.Comment1, 40) + ")";
                                            }

                                            bool checkName = false;//是否有重名，重名的加个坐标区分
                                            foreach (BarButtonItem item in sub_Bars)
                                            {
                                                DrawData itemData = (DrawData)item.Tag;
                                                if (item.Caption.Replace(" [" + itemData.X.ToString() + "," + itemData.Y.ToString() + "]", "") == Desc1)
                                                {
                                                    if (item.Caption.Contains(" [" + itemData.X.ToString() + "," + itemData.Y.ToString() + "]") == false)
                                                    {
                                                        item.Caption = item.Caption + " [" + itemData.X.ToString() + "," + itemData.Y.ToString() + "]";
                                                    }
                                                    checkName = true;
                                                }
                                            }

                                            if (checkName)
                                            {
                                                sub_trans.Caption = Desc1 + " [" + tmp.X.ToString() + "," + tmp.Y.ToString() + "]";
                                            }
                                            else
                                            {
                                                sub_trans.Caption = Desc1;
                                            }

                                            sub_trans.Tag = tmp;
                                            sub_Bars.Add(sub_trans);
                                        }
                                        if (selectedData == tmp) sub_trans.Visibility = BarItemVisibility.Never;
                                    }
                                }
                            }
                            //添加本次子菜单
                            barButtonItem_TransferTo.ItemLinks.AddRange(sub_Bars);
                            //注册事件
                            foreach (BarItemLink tmp in barButtonItem_TransferTo.ItemLinks)
                            {
                                if (tmp.Item.Tag != selectedData) tmp.Item.ItemClick += DiagramControlMenuClick;
                            }
                        }
                    }
                }
            });
        }


        /// <summary>
        /// Ribbon菜单"转到"的子项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetItemData_BarButtonItem_SubTransferTo(object sender, EventArgs e)
        {
            General.TryCatch(() =>
            {
                //取消上次注册的事件
                foreach (BarItemLink tmp in barSubItem_TransferTo.ItemLinks)
                {
                    tmp.Item.ItemClick -= DiagramControlMenuClick;
                }
                barSubItem_TransferTo.ItemLinks.Clear();

                if (ftaDiagram.SelectedData.Count == 1)
                {
                    DrawData selectedData = ftaDiagram.SelectedData.FirstOrDefault();
                    if (General.FtaProgram.CurrentSystem?.TranferGates?.ContainsKey(selectedData.Identifier) == true)
                    {
                        HashSet<DrawData> transfer = General.FtaProgram.CurrentSystem.TranferGates[selectedData.Identifier];
                        DrawData trans_True = transfer.Where(obj => obj.Type != DrawType.TransferInGate).FirstOrDefault();
                        if (trans_True != null)
                        {
                            List<BarButtonItem> sub_Bars = new List<BarButtonItem>();
                            //先把本体放到第一个
                            BarButtonItem sub_trans_True = new BarButtonItem();

                            // 按指定(字节)长度截取字符串
                            string Desc = selectedData.Identifier;
                            if (selectedData.Comment1 != null && selectedData.Comment1 != "")
                            {
                                Desc += " (" + CutStringByte(selectedData.Comment1, 40) + ")";
                            }

                            sub_trans_True.Caption = Desc;
                            sub_trans_True.Tag = trans_True;
                            sub_Bars.Add(sub_trans_True);

                            if (selectedData == trans_True) sub_trans_True.Visibility = BarItemVisibility.Never;

                            if (selectedData.Type != DrawType.TransferInGate)
                            {
                                foreach (DrawData tmp in transfer)
                                {
                                    if (tmp != trans_True)
                                    {
                                        BarButtonItem sub_trans = new BarButtonItem();
                                        if (tmp.Parent != null)
                                        {
                                            // 按指定(字节)长度截取字符串
                                            string Desc1 = tmp.Parent.Identifier;
                                            if (tmp.Parent.Comment1 != null && tmp.Parent.Comment1 != "")
                                            {
                                                Desc1 += " (" + CutStringByte(tmp.Parent.Comment1, 40) + ")";
                                            }

                                            bool checkName = false;//是否有重名，重名的加个坐标区分
                                            foreach (BarButtonItem item in sub_Bars)
                                            {
                                                DrawData itemData = (DrawData)item.Tag;
                                                if (item.Caption.Replace(" [" + itemData.X.ToString() + "," + itemData.Y.ToString() + "]", "") == Desc1)
                                                {
                                                    if (item.Caption.Contains(" [" + itemData.X.ToString() + "," + itemData.Y.ToString() + "]") == false)
                                                    {
                                                        item.Caption = item.Caption + " [" + itemData.X.ToString() + "," + itemData.Y.ToString() + "]";
                                                    }
                                                    checkName = true;
                                                }
                                            }

                                            if (checkName)
                                            {
                                                sub_trans.Caption = Desc1 + " [" + tmp.X.ToString() + "," + tmp.Y.ToString() + "]";
                                            }
                                            else
                                            {
                                                sub_trans.Caption = Desc1;
                                            }

                                            sub_trans.Tag = tmp;
                                            sub_Bars.Add(sub_trans);
                                        }
                                        if (selectedData == tmp) sub_trans.Visibility = BarItemVisibility.Never;
                                    }
                                }
                            }
                            //添加本次子菜单
                            barSubItem_TransferTo.ItemLinks.AddRange(sub_Bars);
                            //注册事件
                            foreach (BarItemLink tmp in barSubItem_TransferTo.ItemLinks)
                            {
                                if (tmp.Item.Tag != selectedData) tmp.Item.ItemClick += DiagramControlMenuClick;
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 主菜单以及部分插入，复制，粘贴功能是否可见可用
        /// </summary>
        /// <param name="isVisibility"></param>
        /// <param name="current"></param>
        private void ChangeBarButtonItemVisibility(bool isVisibility, DrawData current)
        {
            var visibility = isVisibility ? BarItemVisibility.Always : BarItemVisibility.Never;
            this.barButtonItem_TransferTo.Visibility = visibility;
            this.barSubItem_TransferTo.Enabled = isVisibility;
            this.barButtonItem_Insert.Visibility = visibility;
            this.barButtonItem_InsertRepeatEvent.Visibility = visibility;
            this.barButtonItem_PreviousRepeatEvent.Visibility = current.Repeats < 1 ? BarItemVisibility.Never : visibility;
            this.barButtonItem_NextRepeatEvent.Visibility = current.Repeats < 1 ? BarItemVisibility.Never : visibility;
            this.barButtonItem_BreakIntoTransfer.Visibility = current.IsGateType == false ? BarItemVisibility.Never : visibility;
            this.barButtonItem_CollapseTransfer.Visibility = visibility;
            this.barButtonItem_MenuBreakIntoTransfer.Enabled = current.IsGateType == false ? false : true;
            this.barButtonItem_MenuCollapseTransfer.Enabled = isVisibility;
            General.HighLightCutSet.Visibility = visibility;
            this.barButtonItem_Cut.Visibility = visibility;
            this.barButtonItem_MenuCut.Enabled = isVisibility;
            barButtonItem_ExportToModel.Enabled = isVisibility;
            barButtonItem_LoadFromModel.Enabled = isVisibility;

            barButtonItem_BasicLib.Enabled = true;
            if (current.IsGateType)
            {
                if (current.Type == DrawType.TransferInGate)
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

            barButtonItem_ShapeEdit.Enabled = isVisibility;
            this.barButtonItem_Copy.Visibility = visibility;
            this.barButtonItem_DiaExportToModel.Visibility = visibility;
            barButtonItem_MenuCopy.Enabled = isVisibility;
            this.barButtonItem_Paste.Visibility = visibility;
            this.barButtonItem_PasteRepeatEvent.Visibility = visibility;
            this.barButtonItem_MenuPaste.Enabled = isVisibility;
            this.barButtonItem_MenuPasteRepeated.Enabled = isVisibility;

            Blci_Insert.Visibility = visibility;

            //转移门上不能插入
            if (current.Type == DrawType.TransferInGate)
            {
                Blci_Insert.Enabled = false;
                barButtonItem_InsertRepeatEvent.Enabled = false;
                barButtonItem_FTATable_Insertinput.Enabled = false;

                //转移门不可复制剪切
                this.barButtonItem_Cut.Enabled = false;
                this.barButtonItem_Copy.Enabled = false;
                this.barButtonItem_MenuCopy.Enabled = false;
                this.barButtonItem_MenuCut.Enabled = false;
            }
            else if (current.IsGateType)
            {
                Blci_Insert.Enabled = true;
                barButtonItem_InsertRepeatEvent.Enabled = true;
                barButtonItem_FTATable_Insertinput.Enabled = true;
            }

            Sub_DeleteDiagramNode.Visibility = visibility;
            this.barButtonItem_ShowFTACalEvent.Visibility = BarItemVisibility.Never;
        }


        /// <summary>
        /// 刷新界面上的表格和图表控件
        /// </summary>
        private void UpdateData(bool isRelayout = true)
        {
            this.ftaTable.UpdateData(isRelayout);
            this.ftaDiagram.UpdateData();
        }

        /// <summary>
        /// 刷新布局
        /// </summary>
        private void UpdateLayout()
        {
            General.TableControl.RefreshDataSource();
            this.ftaDiagram.ResetData();
        }

        /// <summary>
        /// 初始化FTA表的treelist
        /// </summary>
        private void InitializeTableControl()
        {
            General.ChangeProbability = this.ftaTable.ChangeProbability;

            this.InitializeEvents();//事件绑定

            this.ftaTable.InitializeImages();//图标绑定

            this.InitializeComboBoxSources();//下拉框绑定

            this.treeList_FTATable.Columns[0].Fixed = FixedStyle.Left;//第一列冻结

            this.ChangeTreetableHeaderLangage();//列标题中英文
        }

        /// <summary>
        /// 事件初始化
        /// </summary>
        private void InitializeEvents()
        {
            //FTATable的右键菜单的显示和菜单是否禁用
            treeList_FTATable.MouseClick += TableControlClick;

            //给每个菜单注册事件
            foreach (BarItemLink link in popupMenu_FTATable.ItemLinks)
            {
                link.Item.ItemClick += TableMenuItemClick;
            }
            foreach (BarItemLink link in Sub_Copy.ItemLinks)
            {
                link.Item.ItemClick += TableMenuItemClick;
            }
            foreach (BarItemLink link in Sub_Delete.ItemLinks)
            {
                link.Item.ItemClick += TableMenuItemClick;
            }
        }

        /// <summary>
        /// 变更DrawData表格列头语言类型
        /// </summary>
        public void ChangeTreetableHeaderLangage() => this.BindLangage(General.FtaProgram.String);

        /// <summary>
        /// 绑定初始化的语言类型
        /// </summary>
        /// <param name="ftaString"></param>
        private void BindLangage(StringModel ftaString)
        {
            this.treeList_FTATable.Columns[nameof(StringModel.Identifier)].Caption = ftaString.Identifier;
            this.treeList_FTATable.Columns[nameof(StringModel.Type)].Caption = ftaString.Type;
            this.treeList_FTATable.Columns[nameof(StringModel.ParentID)].Caption = ftaString.ParentID;
            this.treeList_FTATable.Columns[nameof(StringModel.LogicalCondition)].Caption = ftaString.LogicalCondition;
            this.treeList_FTATable.Columns[nameof(StringModel.Comment1)].Caption = ftaString.Comment1;
            this.treeList_FTATable.Columns[nameof(StringModel.InputType)].Caption = ftaString.InputType;
            this.treeList_FTATable.Columns[nameof(StringModel.FRType)].Caption = ftaString.FRType;
            this.treeList_FTATable.Columns[nameof(StringModel.ExposureTimePercentage)].Caption = ftaString.ExposureTimePercentage;
            this.treeList_FTATable.Columns[nameof(StringModel.DormancyFactor)].Caption = ftaString.DormancyFactor;
            this.treeList_FTATable.Columns[nameof(StringModel.FRPercentage)].Caption = ftaString.FRPercentage;
            this.treeList_FTATable.Columns[nameof(StringModel.InputValue)].Caption = ftaString.InputValue;
            this.treeList_FTATable.Columns[nameof(StringModel.InputValue2)].Caption = ftaString.InputValue2;
            this.treeList_FTATable.Columns[nameof(StringModel.Units)].Caption = ftaString.Units;
            this.treeList_FTATable.Columns[nameof(StringModel.ProblemList)].Caption = ftaString.ProblemList;
            this.treeList_FTATable.Columns[nameof(StringModel.ExtraValue1)].Caption = ftaString.ExtraValue1;
            this.treeList_FTATable.Columns[nameof(StringModel.ExtraValue2)].Caption = ftaString.ExtraValue2;
            this.treeList_FTATable.Columns[nameof(StringModel.ExtraValue3)].Caption = ftaString.ExtraValue3;
            this.treeList_FTATable.Columns[nameof(StringModel.ExtraValue4)].Caption = ftaString.ExtraValue4;
            this.treeList_FTATable.Columns[nameof(StringModel.ExtraValue5)].Caption = ftaString.ExtraValue5;
            this.treeList_FTATable.Columns[nameof(StringModel.ExtraValue6)].Caption = ftaString.ExtraValue6;
            this.treeList_FTATable.Columns[nameof(StringModel.ExtraValue7)].Caption = ftaString.ExtraValue7;
            this.treeList_FTATable.Columns[nameof(StringModel.ExtraValue8)].Caption = ftaString.ExtraValue8;
            this.treeList_FTATable.Columns[nameof(StringModel.ExtraValue9)].Caption = ftaString.ExtraValue9;
            this.treeList_FTATable.Columns[nameof(StringModel.ExtraValue10)].Caption = ftaString.ExtraValue10;

            barSubItem_MenuDelete.Caption = ftaString.Delete;
            this.Sub_DeleteDiagramNode.Caption = ftaString.Delete;
            this.Sub_Delete.Caption = ftaString.Delete;

            barButtonItem_DiaExportToModel.Caption = ftaString.ExportToModel;
            barButtonItem_ExportToModel.Caption = ftaString.ExportToModel;
            barButtonItem_LoadFromModel.Caption = ftaString.LoadFromModel;
            barButtonItem_BasicLib.Caption = ftaString.BasicLib;
            barButtonItem_AddToBasicEventLibrary.Caption = ftaString.AddToBasicEventLibrary;
            barButtonItem_SynchronizeFromBasicEventLibrary.Caption = ftaString.SynchronizeFromBasicEventLibrary;
            barButtonItem_InsertFromBasicEventLibrary.Caption = ftaString.InsertFromBasicEventLibrary;

            this.Sub_Copy.Caption = ftaString.Copy;

            this.barButtonItem_DeleteNodeDiagram.Caption = ftaString.DeleteNode;
            barButtonItem_DeleteNodeRibbon.Caption = ftaString.DeleteNode;
            barButtonItem_DeleteNodesRibbon.Caption = ftaString.DeleteNodes;
            this.barButtonItem_DeleteNodesDiagram.Caption = ftaString.DeleteNodes;

            this.barButtonItem_DeleteTopDiagram.Caption = ftaString.DeleteTop;
            this.barButtonItem_DeleteTopRibbon.Caption = ftaString.DeleteTop;
            this.barButtonItem_DeleteLevelDiagram.Caption = ftaString.DeleteLevel;
            this.barButtonItem_DeleteLevelRibbon.Caption = ftaString.DeleteLevel;
        }

        /// <summary>
        /// FTATable的右键菜单的显示和菜单是否禁用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TableControlClick(object sender, MouseEventArgs e)
        {
            General.TryCatch(() =>
            {
                var hit = treeList_FTATable.CalcHitInfo(e.Location);
                if (!hit.InColumnPanel && !hit.InFilterPanel)
                {
                    if (e.Button == MouseButtons.Right && e.Clicks == 1)
                    {
                        DrawData data_Selected = null;
                        if (treeList_FTATable.FocusedNode != null && hit.Node == treeList_FTATable.FocusedNode)
                        {
                            object obj = treeList_FTATable.FocusedNode.GetValue(FixedString.COLUMNAME_DATA);
                            if (obj != null && obj.GetType() == typeof(DrawData))
                            {
                                data_Selected = obj as DrawData;
                            }
                        }

                        //设置每个菜单是否可用
                        foreach (BarItemLink link in popupMenu_FTATable.ItemLinks)
                        {
                            link.Item.Visibility = GetBarItemIsEnabled(link.Item, data_Selected) ? BarItemVisibility.Always : BarItemVisibility.Never;
                        }

                        popupMenu_FTATable.ShowPopup(MousePosition);
                    }
                }
            });
        }

        /// <summary>
        /// FTATable的右键菜单处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TableMenuItemClick(object sender, ItemClickEventArgs e)
        {
            General.TryCatch(() =>
            {
                DrawData data_Selected = null;
                if (treeList_FTATable.FocusedNode != null)
                {
                    object obj = treeList_FTATable.FocusedNode.GetValue(FixedString.COLUMNAME_DATA);
                    if (obj != null && obj.GetType() == typeof(DrawData)) data_Selected = obj as DrawData;
                }

                //是否满足条件
                if (GetBarItemIsEnabled(e.Item, data_Selected)) this.TableControlMenuClick(General.GetKeyName(e.Item.Caption));
            });
        }

        #region FTA表和图通用的一些方法

        /// <summary>
        /// 移除所有事件并初始化
        /// </summary>
        /// <param name="events"></param>
        private void DeleteMulitEvents(List<DrawData> events)
        {
            var allDrawData = this.GetAllDrawDataFromTree(RenumberedRange.SelectedTree);
            for (int i = 0; i < events.Count; i++)
            {
                var parent = allDrawData.FirstOrDefault(o => o == events[i].Parent);
                parent.Children.Remove(events[i]);
            }
            General.FtaProgram.CurrentSystem.UpdateRepeatedAndTranfer();
            this.UpdateLayout();
        }

        /// <summary>
        /// FTA表和图通用的用于插入一个新顶层节点和图形
        /// </summary>
        /// <returns>创建的新顶层数据对象</returns>
        private DrawData FTATableDiagram_InsertNewTopGate()
        {
            return General.TryCatch(() =>
            {
                DrawData item = null;
                DrawData root = General.FtaProgram.CurrentSystem.CreateOneRootItem();
                VirtualDrawData vData = treeList_FTATable.DataSource as VirtualDrawData;
                vData.data.Children.Add(root);
                this.treeList_FTATable.RefreshDataSource();
                this.ftaDiagram.FocusOn(root);
                this.ftaTable.FocusOn(root);
                this.UpdateLayout();

                //情况: 插入一个新顶层节点
                if ((null != General.FtaProgram)
                    && (null != General.FtaProgram.CurrentSystem))
                {
                    //做历史记录
                    General.FtaProgram.CurrentSystem.TakeBehavor(
                        root
                        , Behavior.Enum.ElementOperate.Creation
                        , Behavior.Enum.ElementOperate.Deletion
                        , null
                        , null);
                }
                return item;
            });
        }

        /// <summary>
        /// FTA表和图通用的用于复制或剪切一个节点或图形
        /// </summary>
        /// <param name="target">操作的节点数据对象</param>
        /// <param name="Is_Copy">是复制还是粘贴</param>
        /// <param name="Is_CopyOrCut_Recurse">是否递归操作</param>
        private void FTATableDiagram_CopyOrCut_WithOrNotRecurse(DrawData target, bool Is_Copy, bool Is_CopyOrCut_Recurse)
        {
            General.TryCatch(() =>
            {
                if (target == null) return;
                General.CopyCutObject = target;
                General.CopyCutSystem = General.FtaProgram.CurrentSystem;
                if (this.ftaTable.TableEvents.IsCopyNode != Is_Copy) this.ftaTable.TableEvents.IsCopyNode = Is_Copy;
                if (General.FTATableDiagram_Is_CopyOrCut_Recurse != Is_CopyOrCut_Recurse) General.FTATableDiagram_Is_CopyOrCut_Recurse = Is_CopyOrCut_Recurse;
                treeList_FTATable.Refresh();
                //使图形选择它的子项
                if (Is_CopyOrCut_Recurse)
                {
                    ftaDiagram.SelectedData.Clear();
                    foreach (var item in target.GetAllData(General.FtaProgram.CurrentSystem))
                    {
                        ftaDiagram.SelectedData.Add(item);
                    }
                    ftaDiagram.Refresh(true);
                }
            });
        }

        /// <summary>
        /// 根据给出的数据对象，返回图形对象列表(已经排好树布局)
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public List<DiagramItem> GenerateDiagramItems(DrawData root)
        {

            List<DiagramItem> shapes = new List<DiagramItem>();
            if (root != null)
            {
                int width = General.DiagramItemPool.Style.ShapeWidth;
                int height = (General.DiagramItemPool.Style.ShapeDescriptionRectHeight + General.DiagramItemPool.Style.ShapeIdRectHeight + General.DiagramItemPool.Style.ShapeSymbolRectHeight) + General.PEN_WIDTH;

                root.ApplyTreeLayout(width, height, width + General.DiagramItemPool.Style.ShapeGap, height + General.DiagramItemPool.Style.ShapeGap, General.DiagramItemPool.Style.ShapeGap, General.DiagramItemPool.Style.ShapeGap);
                FTADiagram_GenerateAllDiagramShape(shapes, root, null);
            }
            return shapes;
        }

        /// <summary>
        /// 根据数据对象递归生成所有的图形对象（包括线条）
        /// </summary>
        /// <param name="shapes">存放结果的集合</param>
        /// <param name="root">父数据对象</param>
        /// <param name="parent_Shape">父图形对象</param>
        private void FTADiagram_GenerateAllDiagramShape(List<DiagramItem> shapes, DrawData root, DiagramShape parent_Shape)
        {
            //第一个根图形
            if (root != null && parent_Shape == null)
            {
                parent_Shape = GenerateDiagramShape(root);
                shapes.Add(parent_Shape);
            }
            if (parent_Shape != null && root.Children != null && root.Children.Count > 0)
            {
                foreach (DrawData child in root.Children)
                {
                    DiagramShape child_Shape = GenerateDiagramShape(child);
                    shapes.Add(child_Shape);
                    //连线
                    DiagramConnector connector = GenerateDiagramConenctor(parent_Shape, child_Shape);
                    shapes.Add(connector);
                    FTADiagram_GenerateAllDiagramShape(shapes, child, child_Shape);
                }
            }
        }

        /// <summary>
        /// 根据给出的数据对象，构造一个图形对象
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public DiagramShape GenerateDiagramShape(DrawData data)
        {
            if (data != null)
            {
                int width = General.DiagramItemPool.Style.ShapeWidth;
                int height = (General.DiagramItemPool.Style.ShapeDescriptionRectHeight + General.DiagramItemPool.Style.ShapeIdRectHeight + General.DiagramItemPool.Style.ShapeSymbolRectHeight) + General.PEN_WIDTH;
                DiagramShape Shape = new DiagramShape(BasicShapes.Rectangle);
                Shape.Tag = data;
                Shape.X = data.X;
                Shape.Y = data.Y;
                Shape.ConnectionPoints = new PointCollection(new List<PointFloat>() { new PointFloat(0.5f, 0), new PointFloat(0.5f, 1) });
                Shape.Size = new Size(width, height);
                Shape.CanEdit = false;
                Shape.CanDelete = false;
                Shape.CanCopy = false;
                Shape.CanMove = false;
                Shape.CanResize = false;
                Shape.CanRotate = false;
                Shape.MinSize = new SizeF(0, 0);
                return Shape;
            }
            return null;
        }

        /// <summary>
        /// 根据给出的数据对象，构造一个图形对象
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public DiagramConnector GenerateDiagramConenctor(DiagramShape parent_Shape, DiagramShape child_Shape)
        {
            if (parent_Shape != null && child_Shape != null)
            {
                //连线
                DiagramConnector connector = new DiagramConnector(parent_Shape, child_Shape);
                connector.BeginItemPointIndex = 1;
                connector.EndItemPointIndex = 0;
                connector.Appearance.BorderColor = System.Drawing.Color.Black;
                connector.EndArrowSize = new SizeF(0, 0);
                connector.CanDelete = false;
                connector.CanMove = false;
                connector.CanChangeRoute = false;
                connector.CanEdit = false;
                connector.CanCopy = false;
                connector.CanResize = false;
                connector.CanRotate = false;
                connector.MinSize = new SizeF(0, 0);
                return connector;
            }
            return null;
        }

        /// <summary>
        /// 复制当前视图到剪贴板
        /// </summary>
        /// <param name="target">操作的根节点数据对象</param>
        /// <param name="visibleData">要显示的对象</param>
        private void FTATableDiagram_CopyCurrentView(DrawData target, List<DrawData> visibleData)
        {
            General.TryCatch(() =>
            {
                if (target == null)
                {
                    return;
                }
                //保存图形 
                List<DiagramItem> shapes = GenerateDiagramItems(target);

                //绘图
                DiagramControl diagramControl_FTA = new DiagramControl();
                diagramControl_FTA.CustomDrawItem += new System.EventHandler<DevExpress.XtraDiagram.CustomDrawItemEventArgs>(this.diagramControl_FTA_CustomDrawItem);

                List<DiagramItem> NoVisibleshapes = shapes.Where(t => (t.Tag != null && visibleData.Contains(t.Tag) == false)).ToList();
                List<DiagramItem> AllConnectorshapes = shapes.Where(t => t.Tag == null).ToList();

                foreach (DiagramItem it in NoVisibleshapes)
                {
                    shapes.Remove(it);
                }
                foreach (DiagramItem it in AllConnectorshapes)
                {
                    if (((DiagramConnector)it).BeginItem != null && visibleData.Contains(((DiagramConnector)it).BeginItem.Tag) == false)
                    {
                        shapes.Remove(it);
                    }
                    else if (((DiagramConnector)it).EndItem != null && visibleData.Contains(((DiagramConnector)it).EndItem.Tag) == false)
                    {
                        shapes.Remove(it);
                    }
                }

                diagramControl_FTA.Items.AddRange(shapes.ToArray());
                diagramControl_FTA.FitToDrawing();
                diagramControl_FTA.Refresh();

                FileStream fs = new FileStream(Environment.CurrentDirectory + "\\tmpCopy.png", FileMode.OpenOrCreate);
                diagramControl_FTA.ExportDiagram(fs, new DevExpress.Diagram.Core.DiagramExportFormat(), 150, 1);
                fs.Close();
                fs.Dispose();

                Clipboard.Clear();
                string file = Environment.CurrentDirectory + "\\tmpCopy.png";
                System.Drawing.Image img = System.Drawing.Image.FromFile(file);
                Clipboard.SetImage(img);
                img.Dispose();
            });
        }

        /// <summary>
        /// 重绘窗口图形(特殊情况)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void diagramControl_FTA_CustomDrawItem(object sender, DevExpress.XtraDiagram.CustomDrawItemEventArgs e)
        {
            try
            {
                DiagramControl diagramControl_FTA = (DiagramControl)sender;
                //绘制到画布上的基本图像，拖动时随鼠标暂时移动的半透明指示图像
                if (e.Context == DiagramDrawingContext.Canvas || e.Context == DiagramDrawingContext.DragPreview)
                {
                    //测试要绘制的图形是否在可视区域内
                    PointF start = diagramControl_FTA.DiagramViewInfo.RulersOffset;
                    float factor = diagramControl_FTA.DiagramViewInfo.ZoomFactor;
                    float width = diagramControl_FTA.DiagramViewInfo.ContentRect.Width;
                    float height = diagramControl_FTA.DiagramViewInfo.ContentRect.Height;
                    RectangleF rect = new RectangleF(-start.X / factor, -start.Y / factor, width / factor, height / factor);
                    RectangleF item_Rect = new RectangleF(e.Item.X, e.Item.Y, e.Item.Width, e.Item.Height);
                    if (!rect.IntersectsWith(item_Rect))
                        return;
                }
                if (e.Item != null && e.Item.GetType() == typeof(DiagramShape) && e.Item.Tag != null && e.Item.Tag.GetType() == typeof(DrawData))
                {
                    //自定义绘制的数据
                    DrawData data = e.Item.Tag as DrawData;
                    if (data.Type == DrawType.NULL) return;

                    if (data.QValue == "")
                    {
                        DrawBase.DrawComponent_New(data, data.Type, e.Graphics, 0, 0, e.Item.Width - General.PEN_WIDTH, data.Comment1, data.Identifier, data.ExtraValue11, data.Type == DrawType.TransferInGate ? true : false);
                    }
                    else
                    {
                        if (data.Type == DrawType.VotingGate)
                        {
                            DrawBase.DrawComponent_New(data, data.Type, e.Graphics, 0, 0, e.Item.Width - General.PEN_WIDTH, data.Comment1, data.Identifier, data.QValue + " M:" + data.ExtraValue1 + ":" + data.Children.Count.ToString(), data.Type == DrawType.TransferInGate ? true : false);
                        }
                        else
                        {
                            DrawBase.DrawComponent_New(data, data.Type, e.Graphics, 0, 0, e.Item.Width - General.PEN_WIDTH, data.Comment1, data.Identifier, data.QValue, data.Type == DrawType.TransferInGate ? true : false);
                        }
                    }

                    //选中的图形加一圈描边效果
                    if (diagramControl_FTA.SelectedItems.Count == 1 && diagramControl_FTA.SelectedItems[0] == e.Item)
                    {
                        using (Pen pen = new Pen(System.Drawing.Color.OrangeRed, 2))
                        {
                            e.Graphics.DrawRectangle(pen, new System.Drawing.Rectangle(-1, -1, (int)e.Size.Width + 1, (int)e.Size.Height + 1));
                        }
                    }
                    e.Handled = true;
                }
            }
            catch (System.Exception ex)
            {
                MsgBox.Show(FixedString.EXCEPTION + ex.Message);
            }
        }


        /// <summary>
        /// FTA表和图通用的用于粘贴节点或图形
        /// </summary>
        /// <param name="dest">目标节点，如果是null表示粘贴为顶层节点</param>
        /// <returns>粘贴后的对象</returns>
        private DrawData FTATableDiagram_Paste(DrawData dest = null)
        {
            return General.TryCatch(() =>
            {
                if (General.CopyCutObject == null)
                    return null;

                if (General.CopyCutSystem.GetHashCode() != General.FtaProgram.CurrentSystem.GetHashCode())
                {
                    List<DrawData> drawDatas = General.CopyCutObject.GetAllData(General.CopyCutSystem, true);
                    if (drawDatas.Where(d => d != null && d.Type == DrawType.TransferInGate).Count() > 0)
                    {
                        MsgBox.Show("跨故障树复制粘贴存在转移门时不支持复制粘贴！");
                        return null;
                    }
                }

                if (dest != null && dest.IsGateType == false) return null;
                DrawData result = null;
                //复制   
                if (this.ftaTable.TableEvents.IsCopyNode)
                {
                    DrawData drawData_Copied = null;
                    HashSet<string> ids = General.FtaProgram.CurrentSystem.GetAllIDs();
                    //递归
                    if (General.FTATableDiagram_Is_CopyOrCut_Recurse)
                    {
                        drawData_Copied = General.CopyCutObject.CopyDrawDataRecurse(ids, false);
                    }
                    else
                    {
                        drawData_Copied = General.CopyCutObject.CopyDrawData(ids, false);
                    }

                    if (drawData_Copied == null) return null;

                    result = drawData_Copied;

                    //情况: 黏贴(已有的)
                    if ((null != result)
                        && (null != General.FtaProgram)
                        && (null != General.FtaProgram.CurrentSystem))
                    {
                        //做历史记录
                        General.FtaProgram.CurrentSystem.TakeBehavor(
                            result
                            , Behavior.Enum.ElementOperate.Creation
                            , Behavior.Enum.ElementOperate.Deletion
                            , dest
                            , null);
                    }

                    //处理复制出的对象里转移门副本
                    List<DrawData> allData = drawData_Copied.GetAllData(General.CopyCutSystem);
                    foreach (DrawData data in allData)
                    {
                        //转移门
                        if (General.FtaProgram.CurrentSystem.TranferGates != null && General.FtaProgram.CurrentSystem.TranferGates.ContainsKey(data.Identifier)
                            && data.Type == DrawType.TransferInGate)
                        {
                            General.FtaProgram.CurrentSystem.AddTranferGate(data);
                        }
                    }

                    //粘贴为顶层节点
                    if (dest == null)
                    {
                        // 绑定父子关系
                        ((VirtualDrawData)treeList_FTATable.DataSource).data.Children.Add(drawData_Copied);
                        General.FtaProgram.CurrentSystem.Roots.Add(drawData_Copied);
                    }
                    else
                    {
                        //绑定父子关系
                        if (dest.Children == null) dest.Children = new List<DrawData>();
                        dest.Children.Add(drawData_Copied);
                        drawData_Copied.Parent = dest;
                    }
                }
                //剪切
                else
                {

                    //情况: 黏贴(剪切单个或整个)
                    if ((null != General.FtaProgram)
                        && (null != General.FtaProgram.CurrentSystem)
                        && (null != General.CopyCutObject))
                    {
                        //做历史记录
                        General.FtaProgram.CurrentSystem.TakeBehavor(
                            General.CopyCutObject,
                            Behavior.Enum.ElementOperate.Move,
                            Behavior.Enum.ElementOperate.Move,
                            dest,
                            General.CopyCutObject.Parent,
                            (General.FTATableDiagram_Is_CopyOrCut_Recurse
                            ? null
                            : (((null != General.CopyCutObject.Children) && (0x00 < General.CopyCutObject.Children.Count))
                                ? new List<Behavior.ObjectBehavior>(General.CopyCutObject.Children)
                                : null)));
                    }

                    //递归
                    if (General.FTATableDiagram_Is_CopyOrCut_Recurse)
                    {
                        //断绝之前关系
                        if (General.CopyCutObject.Parent != null && General.CopyCutObject.Parent.Children != null)
                            General.CopyCutObject.Parent.Children.Remove(General.CopyCutObject);

                        //粘贴为顶层节点
                        if (dest == null)
                        {
                            //重新建立关系
                            General.CopyCutObject.Parent = null;
                            ((VirtualDrawData)treeList_FTATable.DataSource).data.Children.Add(General.CopyCutObject);
                            General.FtaProgram.CurrentSystem.Roots.Add(General.CopyCutObject);
                        }
                        else
                        {
                            //重新建立关系
                            General.CopyCutObject.Parent = dest;
                            dest.Children.Add(General.CopyCutObject);
                        }
                        result = General.CopyCutObject;

                        //粘贴完毕，无法继续粘贴
                        General.CopyCutObject = null;
                    }
                }

                treeList_FTATable.RefreshDataSource();
                if (dest != null) this.ftaDiagram.ResetData();

                return result;
            });
        }

        /// <summary>
        /// 从模型导入（维持原来的内部重复事件）
        /// </summary>
        /// <param name="dest">目标节点，如果是null表示粘贴为顶层节点</param>
        /// <returns>粘贴后的对象</returns>
        private DrawData FTATableDiagram_PasteNew(DrawData dest = null)
        {
            return General.TryCatch(() =>
            {
                if (dest != null && dest.IsGateType == false) return null;
                DrawData result = null;
                //复制   
                if (this.ftaTable.TableEvents.IsCopyNode)
                {
                    DrawData drawData_Copied = null;
                    HashSet<string> ids = General.FtaProgram.CurrentSystem.GetAllIDs();
                    //递归
                    if (General.FTATableDiagram_Is_CopyOrCut_Recurse)
                    {
                        drawData_Copied = General.CopyCutObject.CopyDrawDataRecurse(ids, true);
                    }
                    else
                    {
                        drawData_Copied = General.CopyCutObject.CopyDrawData(ids, true);
                    }

                    if (drawData_Copied == null) return null;

                    result = drawData_Copied;

                    //情况: 黏贴(已有的)
                    if ((null != result)
                        && (null != General.FtaProgram)
                        && (null != General.FtaProgram.CurrentSystem))
                    {
                        //做历史记录
                        General.FtaProgram.CurrentSystem.TakeBehavor(
                            result
                            , Behavior.Enum.ElementOperate.Creation
                            , Behavior.Enum.ElementOperate.Deletion
                            , dest
                            , null);
                    }

                    //处理复制出的对象里转移门副本
                    List<DrawData> allData = drawData_Copied.GetAllData(General.CopyCutSystem);
                    foreach (DrawData data in allData)
                    {
                        //转移门
                        if (General.FtaProgram.CurrentSystem.TranferGates != null && General.FtaProgram.CurrentSystem.TranferGates.ContainsKey(data.Identifier)
                            && data.Type == DrawType.TransferInGate)
                        {
                            General.FtaProgram.CurrentSystem.AddTranferGate(data);
                        }
                    }

                    //粘贴为顶层节点
                    if (dest == null)
                    {
                        // 绑定父子关系
                        ((VirtualDrawData)treeList_FTATable.DataSource).data.Children.Add(drawData_Copied);
                        General.FtaProgram.CurrentSystem.Roots.Add(drawData_Copied);
                    }
                    else
                    {
                        //绑定父子关系
                        if (dest.Children == null) dest.Children = new List<DrawData>();
                        dest.Children.Add(drawData_Copied);
                        drawData_Copied.Parent = dest;
                    }
                }

                treeList_FTATable.RefreshDataSource();
                if (dest != null) this.ftaDiagram.ResetData();

                return result;
            });
        }
        #endregion

        /// <summary>
        /// 初始化FTATable(TreeList控件)模块下单元格的数据（各个列的一些预定值，事件等）
        /// </summary>
        private void InitializeComboBoxSources()
        {
            for (int i = 0; i < 5; i++)
            {
                RepositoryItemComboBox result = new RepositoryItemComboBox();
                result.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                result.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
                treeList_FTATable.RepositoryItems.Add(result);
                switch (i)
                {
                    case 1: { result.Items.AddRange(DrawData.GetAvailableLogicalConditionSource(General.FtaProgram.String)); break; }
                    case 2: { result.Items.AddRange(DrawData.GetAvailableInputTypeSource(General.FtaProgram.String)); break; }
                    case 3: { result.Items.AddRange(DrawData.GetAvailableFailureRateTypeSource(General.FtaProgram.String)); break; }
                    case 4: { result.Items.AddRange(DrawData.GetAvailableUnitSource(General.FtaProgram.String)); break; }
                    default: break;
                }
            }
        }

        #region 缩小放大FTA图的ibbon菜单
        /// <summary>
        /// 初始化Ribbon-Tool-View下的菜单
        /// </summary>
        private void Init_Ribbon_Tool_View()
        {
            //缩小图
            barButtonItem_ZoomOut.ItemClick += ItemClick_BarButtonItem_Ribbon_Tool_View;

            //刷新图        
            barButtonItem_Refresh.ItemClick += ItemClick_BarButtonItem_Ribbon_Tool_View;

            //放大图
            barButtonItem_ZoomIn.ItemClick += ItemClick_BarButtonItem_Ribbon_Tool_View;
        }

        /// <summary>
        /// 缩小，刷新，放大FTA图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemClick_BarButtonItem_Ribbon_Tool_View(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            General.TryCatch(() =>
            {
                if (e.Item == barButtonItem_ZoomOut)
                {
                    General.DiagramControl.ZoomOut();
                }
                else if (e.Item == barButtonItem_Refresh)
                {
                    this.ftaDiagram.Load();
                }
                else if (e.Item == barButtonItem_ZoomIn)
                {
                    General.DiagramControl.ZoomIn();
                }
            });
        }
        #endregion

        #region FTA表的指示器设置ribbon菜单
        /// <summary>
        /// 初始化Ribbon-Setting-FTATableEditor下的菜单
        /// </summary>
        private void Init_Ribbon_Setting_FTATableEditor()
        {
            //FTA表是否显示指示器
            barCheckItem_ShowIndicator.CheckedChanged += CheckedChanged_Ribbon_Setting_FTATableEditor;

            //FTA表指示器顶级门图形颜色设置变化
            barEditItem_IndicatorTopGateColor.EditValueChanged += EditValueChanged_Ribbon_Setting_FTATableEditor;

            //FTA表指示器转移门图形颜色设置变化
            barEditItem_IndicatorTransInGateColor.EditValueChanged += EditValueChanged_Ribbon_Setting_FTATableEditor;
        }

        /// <summary>
        /// FTA表指示器图形颜色设置变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditValueChanged_Ribbon_Setting_FTATableEditor(object sender, EventArgs e)
        {
            General.TryCatch(() =>
            {
                if (sender == barEditItem_IndicatorTopGateColor)
                {
                    General.FtaProgram.Setting.FTATableIndicatorTopGateColor = Ribbon_Start_FTAStyle_ColorChanged(sender, General.FtaProgram.Setting.FTATableIndicatorTopGateColor);
                    treeList_FTATable.Refresh();
                }
                else if (sender == barEditItem_IndicatorTransInGateColor)
                {
                    General.FtaProgram.Setting.FTATableIndicatorTransInGateColor = Ribbon_Start_FTAStyle_ColorChanged(sender, General.FtaProgram.Setting.FTATableIndicatorTransInGateColor);
                    treeList_FTATable.Refresh();
                }
            });
        }

        /// <summary>
        /// FTA表是否显示指示器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckedChanged_Ribbon_Setting_FTATableEditor(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            General.TryCatch(() =>
            {
                //是否显示指示器
                if (sender == barCheckItem_ShowIndicator)
                {
                    treeList_FTATable.OptionsView.ShowIndicator = barCheckItem_ShowIndicator.Checked;
                    if (General.FtaProgram.Setting.Is_ShowIndicator != barCheckItem_ShowIndicator.Checked) General.FtaProgram.Setting.Is_ShowIndicator = barCheckItem_ShowIndicator.Checked;
                }
                if (barCheckItem_ShowIndicator.Checked)
                {
                    barEditItem_IndicatorTopGateColor.Enabled = true;
                    barEditItem_IndicatorTransInGateColor.Enabled = true;
                }
                else
                {
                    barEditItem_IndicatorTopGateColor.Enabled = false;
                    barEditItem_IndicatorTransInGateColor.Enabled = false;
                }
            });
        }


        /// <summary>
        /// 初始化Ribbon-Setting-Language下的菜单
        /// </summary>
        private void Init_Ribbon_Setting_Language()
        {
            //添加可选的语言值
            repositoryItemComboBox_Language.Items.Clear();
            if (barEditItem_Language.EditValue != null && barEditItem_Language.EditValue.ToString() == FixedString.LANGUAGE_EN_EN)
            {
                repositoryItemComboBox_Language.Items.AddRange(new object[] { FixedString.LANGUAGE_EN_CN, FixedString.LANGUAGE_EN_EN });
            }
            else if (barEditItem_Language.EditValue != null && barEditItem_Language.EditValue.ToString() == FixedString.LANGUAGE_CN_CN)
            {
                repositoryItemComboBox_Language.Items.AddRange(new object[] { FixedString.LANGUAGE_CN_CN, FixedString.LANGUAGE_CN_EN });
            }

            //切换语言
            barEditItem_Language.EditValueChanged += LanguageChanged;
        }

        /// <summary>
        /// 语言下拉框选取变化时，切换语言
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LanguageChanged(object sender, EventArgs e)
        {
            General.TryCatch(() =>
            {
                //语言下拉选项绑定（中英文）
                repositoryItemComboBox_Language.Items.Clear();
                if (barEditItem_Language.EditValue != null && (barEditItem_Language.EditValue.ToString() == FixedString.LANGUAGE_EN_EN || barEditItem_Language.EditValue.ToString() == FixedString.LANGUAGE_CN_EN))
                {
                    repositoryItemComboBox_Language.Items.AddRange(new object[] { FixedString.LANGUAGE_EN_CN, FixedString.LANGUAGE_EN_EN });
                }
                else if (barEditItem_Language.EditValue != null && (barEditItem_Language.EditValue.ToString() == FixedString.LANGUAGE_CN_CN || barEditItem_Language.EditValue.ToString() == FixedString.LANGUAGE_EN_CN))
                {
                    repositoryItemComboBox_Language.Items.AddRange(new object[] { FixedString.LANGUAGE_CN_CN, FixedString.LANGUAGE_CN_EN });
                }

                if (barEditItem_Language.EditValue != null && barEditItem_Language.EditValue.ToString() == FixedString.LANGUAGE_CN_EN)
                {
                    barEditItem_Language.EditValue = FixedString.LANGUAGE_EN_EN;
                }
                else if (barEditItem_Language.EditValue != null && barEditItem_Language.EditValue.ToString() == FixedString.LANGUAGE_EN_CN)
                {
                    barEditItem_Language.EditValue = FixedString.LANGUAGE_CN_CN;
                }

                if (barEditItem_Language.EditValue?.GetType() == typeof(string))
                {
                    ChangeLanguage(barEditItem_Language.EditValue as string);
                    General.FtaProgram.Setting.Language = barEditItem_Language.EditValue as string;
                    this.ChangeLogicalConditionLangage();
                    this.ChangeUnitsLangage();
                    this.ChangeTreetableHeaderLangage();
                    this.ChangeInputTypeLangage();
                    General.TableControl.RefreshDataSource();

                    //修复下拉插入选项
                    foreach (BarItemLink link in General.DiagramMenuControl.ItemLinks)
                    {
                        if (link.Item == this.Blci_Insert)
                        {
                            DrawData da = new DrawData();
                            da.Type = DrawType.AndGate;

                            this.Blci_Insert.ItemLinks.Clear();
                            this.Blci_Insert.ItemLinks.AddRange(this.GetCanInsertItems(da.GetAvailableTypeSource(new SystemModel(), General.FtaProgram.String).ToArray()));

                            foreach (BarItemLink item in this.Blci_Insert.ItemLinks)
                            {
                                item.Item.ItemClick += DiagramControlMenuClick;
                            }
                            link.Item.ItemClick += DiagramControlMenuClick;
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 中英文模式Ribbon菜单部分控件大小分开设置
        /// </summary>
        /// <param name="language"></param>
        private void ChangeRibbonControlSize(string language)
        {
            if (language == FixedString.LANGUAGE_EN_EN)
            {
                this.Bsi_SymbolSize.Width = 100;
                this.Bsi_ShapeRectHeight.Width = 100;
                this.Bsi_ShapeRectWidth.Width = 100;

                this.Bsi_ShapeBackColor_Selected.Width = 110;
                this.Bsi_ShapeBackColor_RepeatEvent.Width = 110;
                this.Bsi_CutSetColor.Width = 110;

                this.Bsi_ShapeBackColor_TrueGate.Width = 120;
                this.Bsi_ShapeBackColor_FalseGate.Width = 120;
                this.Bsi_ShapeBackColor.Width = 120;

                this.Bsi_ShowIndicator.Width = 90;
                this.Bsi_IndicatorTopGateColor.Width = 90;
                this.Bsi_IndicatorTransInGateColor.Width = 90;

                this.Bsi_Ruler.Width = 70;
                this.Bsi_Grid.Width = 70;
                this.Bsi_PageBreaks.Width = 70;

                this.Bsi_CanvasFillMode.Width = 160;
                this.Bsi_PageScrollMode.Width = 160;
                this.Bsi_LineColor.Width = 160;

                this.Bsi_LineStyle.Width = 80;
                this.Bsi_ArrowStyle.Width = 80;
                this.Bsi_ArrowSize.Width = 80;

                this.Bsi_MoveAble.Width = 66;
                this.Bci_ScaleAble.Width = 66;
            }
            else if (language == FixedString.LANGUAGE_CN_CN)
            {
                this.Bsi_SymbolSize.Width = 75;
                this.Bsi_ShapeRectHeight.Width = 75;
                this.Bsi_ShapeRectWidth.Width = 75;

                this.Bsi_ShapeBackColor_Selected.Width = 65;
                this.Bsi_ShapeBackColor_RepeatEvent.Width = 65;
                this.Bsi_CutSetColor.Width = 65;

                this.Bsi_ShapeBackColor_TrueGate.Width = 65;
                this.Bsi_ShapeBackColor_FalseGate.Width = 65;
                this.Bsi_ShapeBackColor.Width = 65;

                this.Bsi_ShowIndicator.Width = 50;
                this.Bsi_IndicatorTopGateColor.Width = 50;
                this.Bsi_IndicatorTransInGateColor.Width = 50;

                this.Bsi_Ruler.Width = 50;
                this.Bsi_Grid.Width = 50;
                this.Bsi_PageBreaks.Width = 50;

                this.Bsi_CanvasFillMode.Width = 110;
                this.Bsi_PageScrollMode.Width = 110;
                this.Bsi_LineColor.Width = 110;

                this.Bsi_LineStyle.Width = 60;
                this.Bsi_ArrowStyle.Width = 60;
                this.Bsi_ArrowSize.Width = 60;

                this.Bsi_MoveAble.Width = 40;
                this.Bci_ScaleAble.Width = 40;
            }
        }

        /// <summary>
        /// 真假门语言切换
        /// </summary>
        private void ChangeLogicalConditionLangage()
        {
            General.FtaProgram.CurrentSystem?.GetAllDatas().ForEach(o =>
            {
                switch (General.GetKeyName(o.LogicalCondition))
                {
                    case nameof(StringModel.Normal): { o.LogicalCondition = General.FtaProgram.String.Normal; break; }
                    case nameof(StringModel.False): { o.LogicalCondition = General.FtaProgram.String.False; break; }
                    case nameof(StringModel.True): { o.LogicalCondition = General.FtaProgram.String.True; break; }
                    default: break;
                }
            });
        }

        /// <summary>
        /// 单位语言切换
        /// </summary>
        private void ChangeUnitsLangage()
        {
            General.FtaProgram.CurrentSystem?.GetAllDatas().ForEach(o =>
            {
                if (o.Units != null)
                {
                    switch (General.GetKeyName(o.Units))
                    {
                        case nameof(StringModel.Hour): { o.Units = General.FtaProgram.String.Hour; break; }
                        case nameof(StringModel.Minute): { o.Units = General.FtaProgram.String.Minute; break; }
                        default: break;
                    }
                }
            });
        }

        /// <summary>
        /// 门类型语言切换
        /// </summary>
        private void ChangeInputTypeLangage()
        {
            General.FtaProgram.CurrentSystem?.GetAllDatas().ForEach(o =>
            {
                if (o.InputType != null)
                {
                    switch (General.GetKeyName(o.InputType))
                    {
                        case nameof(FixedString.MODEL_LAMBDA_TAU): { o.InputType = FixedString.MODEL_LAMBDA_TAU; break; }
                        case nameof(StringModel.FailureProbability): { o.InputType = General.FtaProgram.String.FailureProbability; break; }
                        case nameof(StringModel.ConstantProbability): { o.InputType = General.FtaProgram.String.ConstantProbability; break; }
                        default: break;
                    }
                }
            });
        }

        /// <summary>
        /// 初始化Ribbon-Setting-Skin下的菜单
        /// </summary>
        private void Init_Ribbon_Setting_Skin()
        {
            List<string> lst = new List<string>();
            //遍历皮肤，放到列表中  
            //foreach (DevExpress.Skins.SkinContainer skin in DevExpress.Skins.SkinManager.Default.Skins)
            //{
            //    lst.Add(skin.SkinName);
            //}

            lst.Add("Office Dark");
            lst.Add("Office Black");
            lst.Add("Office Blue");
            lst.Add("Dark Side");
            lst.Add("McSkin");
            lst.Add("Summer");

            repositoryItemComboBox1.Items.Clear();
            repositoryItemComboBox1.Items.AddRange(lst);

            //最多显示10个下拉项  
            repositoryItemComboBox1.DropDownRows = lst.Count > 10 ? 10 : lst.Count;

            //切换皮肤时保存皮肤名字
            barEditItem_Skin.EditValueChanged += barEditItem_Skin_EditValueChanged;

            barEditItem_Skin.EditValue = General.FtaProgram.Setting.Skin_Name;
        }

        /// <summary>
        /// 切换皮肤时保存皮肤名字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barEditItem_Skin_EditValueChanged(object sender, EventArgs e)
        {
            General.TryCatch(() =>
            {
                if (barEditItem_Skin.EditValue != null && barEditItem_Skin.EditValue.ToString() != "")
                {
                    General.FtaProgram.Setting.Skin_Name = barEditItem_Skin.EditValue.ToString();
                }
                else
                {
                    General.FtaProgram.Setting.Skin_Name = "Office Dark";
                }

                switch (General.FtaProgram.Setting.Skin_Name)
                {
                    case "Office Dark":
                        UserLookAndFeel.Default.SetSkinStyle("Office 2016 Dark");
                        break;
                    case "Office Black":
                        UserLookAndFeel.Default.SetSkinStyle("Office 2010 Black");
                        break;
                    case "Office Blue":
                        UserLookAndFeel.Default.SetSkinStyle("Office 2010 Blue");
                        break;
                    case "Dark Side":
                        UserLookAndFeel.Default.SetSkinStyle("Dark Side");
                        break;
                    case "McSkin":
                        UserLookAndFeel.Default.SetSkinStyle("McSkin");
                        break;
                    case "Summer":
                        UserLookAndFeel.Default.SetSkinStyle("Summer 2008");
                        break;
                    default:
                        UserLookAndFeel.Default.SetSkinStyle("Office 2016 Dark");
                        break;
                }

                SaveSettings();
            });
        }
        #endregion

        /// <summary>
        /// FTATable的右键菜单处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TableControlMenuClick(string keyName)
        {
            var selectedDrawData = default(DrawData);
            if (treeList_FTATable.FocusedNode != null)
            {
                object obj = treeList_FTATable.FocusedNode.GetValue(FixedString.COLUMNAME_DATA);
                if (obj != null && obj.GetType() == typeof(DrawData))
                {
                    selectedDrawData = obj as DrawData;
                }
            }
            else
            {
                //部分快捷键启用
                bool checkdo = false;
                if (string.IsNullOrEmpty(keyName) == false)
                {
                    switch (keyName)
                    {
                        case nameof(StringModel.Redo):
                            checkdo = true;
                            break;
                        case nameof(StringModel.Undo):
                            checkdo = true;
                            break;
                        case nameof(StringModel.InsertTopNode):
                            checkdo = true;
                            break;
                        case nameof(StringModel.PosParent):
                            checkdo = true;
                            break;
                        case nameof(StringModel.PosChild):
                            checkdo = true;
                            break;
                        case nameof(StringModel.PosLeft):
                            checkdo = true;
                            break;
                        case nameof(StringModel.PosRight):
                            checkdo = true;
                            break;
                    }
                }
                if (checkdo == false)
                {
                    return;
                }
            }

            //转移门无法插入，粘贴
            if (selectedDrawData != null)
            {
                if (string.IsNullOrEmpty(keyName) == false && selectedDrawData.Type == DrawType.TransferInGate && (keyName == nameof(StringModel.InsertNode) || keyName == nameof(StringModel.Paste) || keyName == nameof(StringModel.PasteRepeatedEvent)))
                {
                    return;
                }
            }

            if (string.IsNullOrEmpty(keyName) == false)
            {
                switch (keyName)
                {
                    case nameof(StringModel.PosParent):
                        {
                            barButtonItem_PosParent.PerformClick();
                            break;
                        }
                    case nameof(StringModel.PosChild):
                        {
                            barButtonItem_PosChild.PerformClick();
                            break;
                        }
                    case nameof(StringModel.PosLeft):
                        {
                            PosLeftOrRight(true);
                            break;
                        }
                    case nameof(StringModel.PosRight):
                        {
                            PosLeftOrRight(false);
                            break;
                        }
                    case nameof(StringModel.InsertNode):
                        {
                            DrawData parentData = selectedDrawData;
                            DrawData childData = InsertNode(parentData);
                            if (childData != null)
                            {
                                this.ftaDiagram.FocusOn(childData);
                                this.ftaTable.FocusOn(parentData);
                            }
                            break;
                        }
                    case nameof(StringModel.InsertTopNode): { FTATableDiagram_InsertNewTopGate(); break; }
                    case nameof(StringModel.CutNodes): { FTATableDiagram_CopyOrCut_WithOrNotRecurse(selectedDrawData, false, true); break; }
                    case nameof(StringModel.CopyNode): { FTATableDiagram_CopyOrCut_WithOrNotRecurse(selectedDrawData, true, false); break; }
                    case nameof(StringModel.CopyNodes): { FTATableDiagram_CopyOrCut_WithOrNotRecurse(selectedDrawData, true, true); break; }
                    case nameof(StringModel.Paste):
                        {
                            //情况: 函数内又有TB
                            DrawData data = FTATableDiagram_Paste(selectedDrawData);
                            if (data != null)
                            {
                                this.ftaDiagram.FocusOn(data);
                                this.ftaTable.FocusOn(selectedDrawData);
                                TreeListNode node = this.ftaTable.FTATable_GetTreeListNode(data);
                                if (node != null && !node.Expanded) node.ExpandAll();
                            }
                            break;
                        }
                    case nameof(StringModel.PasteRepeatedEvent):
                        {
                            DrawData result = this.ftaTable.PasteRepeatedEvent(selectedDrawData, this.ftaTable.TableEvents.IsCopyNode, General.CopyCutObject, General.FTATableDiagram_Is_CopyOrCut_Recurse);
                            if (result != null)
                            {
                                this.ftaTable.FocusOn(selectedDrawData);
                                TreeListNode node = this.ftaTable.FTATable_GetTreeListNode(result);
                                if (node != null && !node.Expanded) node.ExpandAll();
                                this.ftaDiagram.FocusOn(result);
                            }

                            //情况: 黏贴(重复事件)
                            if ((null != result)
                                && (null != General.FtaProgram)
                                && (null != General.FtaProgram.CurrentSystem))
                            {
                                //做历史记录
                                General.FtaProgram.CurrentSystem.TakeBehavor(
                                    result
                                    , Behavior.Enum.ElementOperate.Creation
                                    , Behavior.Enum.ElementOperate.Deletion
                                    , selectedDrawData
                                    , null);
                            }
                            break;
                        }
                    case nameof(StringModel.PasteAsTopNode): { FTATableDiagram_Paste(); break; }//情况: 函数内又有TB
                    case nameof(StringModel.CancelCopyOrCut):
                        {
                            General.CopyCutObject = null;
                            treeList_FTATable.Refresh();
                            //重新刷新图形的选取状态
                            if (treeList_FTATable.FocusedNode != null && treeList_FTATable.FocusedNode.GetValue(FixedString.COLUMNAME_DATA) != null
                                && treeList_FTATable.FocusedNode.GetValue(FixedString.COLUMNAME_DATA).GetType() == typeof(DrawData))
                            {
                                ftaDiagram.SelectedData.Clear();
                                ftaDiagram.SelectedData.Add(treeList_FTATable.FocusedNode.GetValue(FixedString.COLUMNAME_DATA) as DrawData);
                                ftaDiagram.Refresh(true);
                            }
                            break;
                        }
                    case nameof(StringModel.DeleteLevel):
                        {
                            if (MsgBox.Show(General.FtaProgram.String.ConfirmDeletionMessage, General.FtaProgram.String.ConfirmTitleDelete, MessageBoxButtons.OKCancel) == DialogResult.OK)
                            {
                                //情况: 移除一个节点
                                DrawData oDeleted = selectedDrawData;
                                if ((null != oDeleted)
                                    && (null != General.FtaProgram)
                                    && (null != General.FtaProgram.CurrentSystem))
                                {
                                    //做历史记录
                                    General.FtaProgram.CurrentSystem.TakeBehavor(
                                        oDeleted
                                        , Behavior.Enum.ElementOperate.Remove
                                        , Behavior.Enum.ElementOperate.Add
                                        , oDeleted.Parent
                                        ,
                                        (((null != oDeleted.Children)
                                        && (0x00 < oDeleted.Children.Count))
                                        ? new List<Behavior.ObjectBehavior>(oDeleted.Children)
                                        : null));
                                }

                                this.ftaTable.TableEvents.DeleteNode(selectedDrawData);
                            }
                            break;
                        }
                    case nameof(StringModel.DeleteNode):
                        {
                            if (MsgBox.Show(General.FtaProgram.String.ConfirmDeletionMessage, General.FtaProgram.String.ConfirmTitleDelete, MessageBoxButtons.OKCancel) == DialogResult.OK)
                            {
                                //情况: 移除一个节点
                                DrawData oDeleted = selectedDrawData;
                                if ((null != oDeleted)
                                    && (null != General.FtaProgram)
                                    && (null != General.FtaProgram.CurrentSystem))
                                {
                                    //做历史记录
                                    General.FtaProgram.CurrentSystem.TakeBehavor(
                                        oDeleted
                                        , Behavior.Enum.ElementOperate.Remove
                                        , Behavior.Enum.ElementOperate.Add
                                        , oDeleted.Parent
                                        ,
                                        (((null != oDeleted.Children)
                                        && (0x00 < oDeleted.Children.Count))
                                        ? new List<Behavior.ObjectBehavior>(oDeleted.Children)
                                        : null));
                                }

                                this.ftaTable.TableEvents.DeleteNode(selectedDrawData);
                            }
                            break;
                        }
                    case nameof(StringModel.DeleteNodes):
                        {
                            //判断如果是唯一顶层非转移门节点，不可删除自身
                            bool checkOnly = false;
                            foreach (DrawData TopDa in General.FtaProgram.CurrentSystem.Roots)
                            {
                                if (General.FtaProgram.CurrentSystem.TranferGates.ContainsKey(TopDa.Identifier) == false && General.FtaProgram.CurrentSystem.Roots[0] == selectedDrawData)
                                {
                                    checkOnly = true;
                                    break;
                                }
                            }

                            string TipM = General.FtaProgram.String.DeleteNodesMessage;
                            if (checkOnly)
                            {
                                TipM = General.FtaProgram.String.DeleteNodesAndTransferMessageOnlyTop;
                            }

                            if (MsgBox.Show(TipM, General.FtaProgram.String.ConfirmTitleDelete, MessageBoxButtons.OKCancel) == DialogResult.OK)
                            {
                                var ids = new List<string>();

                                if (General.FtaProgram.CurrentSystem.Roots.Where(o => o.ThisGuid == selectedDrawData.ThisGuid).Count() > 0)
                                {
                                    //做历史记录
                                    //是转移门
                                    if (General.FtaProgram.CurrentSystem.TranferGates.Where(o => o.Key == selectedDrawData.Identifier).Count() > 0)
                                    {
                                        General.FtaProgram.CurrentSystem.TakeBehavor();
                                    }
                                    else
                                    {
                                        if (checkOnly == false)
                                        {
                                            DrawData oDeleted = selectedDrawData;
                                            if ((null != oDeleted)
                                                && (null != General.FtaProgram)
                                                && (null != General.FtaProgram.CurrentSystem))
                                            {
                                                //做历史记录
                                                General.FtaProgram.CurrentSystem.TakeBehavor(
                                                    oDeleted
                                                    , Behavior.Enum.ElementOperate.Remove
                                                    , Behavior.Enum.ElementOperate.Add
                                                    , oDeleted.Parent
                                                    , null);
                                            }
                                        }
                                        else
                                        {
                                            General.FtaProgram.CurrentSystem.TakeBehavor();
                                        }
                                    }

                                    if (checkOnly == false)
                                    {
                                        General.IsIgnoreTreeListFocusNodeChangeEvent = false;
                                        ids.AddRange(this.ftaTable.TableEvents.DeleteNodeAndChildren(selectedDrawData, false));
                                        if (General.FtaProgram.CurrentSystem != null && General.FtaProgram.CurrentSystem.Roots.Count > 0 && General.FtaProgram.CurrentRoot != null)
                                        {
                                            General.FTATree.FocusedNode = null;

                                            Predicate<TreeListNode> match = new Predicate<TreeListNode>(d => (d.GetValue(FixedString.COLUMNAME_DATA) != null && d.GetValue(FixedString.COLUMNAME_DATA) == General.FtaProgram.CurrentRoot));
                                            TreeListNode nd = General.FTATree.FindNode(match);
                                            General.FTATree.FocusedNode = nd;
                                        }
                                    }
                                    else
                                    {
                                        for (int i = selectedDrawData.Children.Count - 1; i >= 0; i--)
                                        {
                                            ids = this.ftaTable.TableEvents.DeleteNodeAndChildren(selectedDrawData.Children[i], false);
                                        }
                                    }
                                }
                                else
                                {
                                    //情况: 移除一个节点(含子结点)
                                    DrawData oDeleted = selectedDrawData;
                                    if ((null != oDeleted)
                                        && (null != General.FtaProgram)
                                        && (null != General.FtaProgram.CurrentSystem))
                                    {
                                        //做历史记录
                                        General.FtaProgram.CurrentSystem.TakeBehavor(
                                            oDeleted
                                            , Behavior.Enum.ElementOperate.Remove
                                            , Behavior.Enum.ElementOperate.Add
                                            , oDeleted.Parent
                                            , null);
                                    }

                                    this.ftaTable.TableEvents.DeleteNodeAndChildren(selectedDrawData);
                                }
                            }
                            break;
                        }
                    case nameof(StringModel.DeleteNodesAndTransfer):
                        {
                            //判断如果是唯一顶层非转移门节点，不可删除自身
                            bool checkOnly = false;
                            foreach (DrawData TopDa in General.FtaProgram.CurrentSystem.Roots)
                            {
                                if (General.FtaProgram.CurrentSystem.TranferGates.ContainsKey(TopDa.Identifier) == false && General.FtaProgram.CurrentSystem.Roots[0] == selectedDrawData)
                                {
                                    checkOnly = true;
                                    break;
                                }
                            }

                            string TipM = General.FtaProgram.String.DeleteNodesMessage;
                            if (checkOnly)
                            {
                                TipM = General.FtaProgram.String.DeleteNodesAndTransferMessageOnlyTop;
                            }

                            if (MsgBox.Show(TipM, General.FtaProgram.String.ConfirmTitleDelete, MessageBoxButtons.OKCancel) == DialogResult.OK)
                            {
                                var ids = new List<string>();

                                if (General.FtaProgram.CurrentSystem.Roots.Where(o => o.ThisGuid == selectedDrawData.ThisGuid).Count() > 0)
                                {
                                    //做历史记录
                                    //是转移门
                                    if (General.FtaProgram.CurrentSystem.TranferGates.Where(o => o.Key == selectedDrawData.Identifier).Count() > 0)
                                    {
                                        General.FtaProgram.CurrentSystem.TakeBehavor();
                                    }
                                    else
                                    {
                                        if (checkOnly == false)
                                        {
                                            DrawData oDeleted = selectedDrawData;
                                            if ((null != oDeleted)
                                                && (null != General.FtaProgram)
                                                && (null != General.FtaProgram.CurrentSystem))
                                            {
                                                //做历史记录
                                                General.FtaProgram.CurrentSystem.TakeBehavor(
                                                    oDeleted
                                                    , Behavior.Enum.ElementOperate.Remove
                                                    , Behavior.Enum.ElementOperate.Add
                                                    , oDeleted.Parent
                                                    , null);
                                            }
                                        }
                                        else
                                        {
                                            General.FtaProgram.CurrentSystem.TakeBehavor();
                                        }
                                    }

                                    if (checkOnly == false)
                                    {
                                        General.IsIgnoreTreeListFocusNodeChangeEvent = false;
                                        ids.AddRange(this.ftaTable.TableEvents.DeleteNodeAndChildren(selectedDrawData, false));
                                        if (General.FtaProgram.CurrentSystem != null && General.FtaProgram.CurrentSystem.Roots.Count > 0 && General.FtaProgram.CurrentRoot != null)
                                        {
                                            General.FTATree.FocusedNode = null;

                                            Predicate<TreeListNode> match = new Predicate<TreeListNode>(d => (d.GetValue(FixedString.COLUMNAME_DATA) != null && d.GetValue(FixedString.COLUMNAME_DATA) == General.FtaProgram.CurrentRoot));
                                            TreeListNode nd = General.FTATree.FindNode(match);
                                            General.FTATree.FocusedNode = nd;
                                        }
                                    }
                                    else
                                    {
                                        for (int i = selectedDrawData.Children.Count - 1; i >= 0; i--)
                                        {
                                            ids = this.ftaTable.TableEvents.DeleteNodeAndChildren(selectedDrawData.Children[i], false);
                                        }
                                    }
                                }
                                else
                                {
                                    //情况: 移除一个节点(含子结点)
                                    DrawData oDeleted = selectedDrawData;
                                    if ((null != oDeleted)
                                        && (null != General.FtaProgram)
                                        && (null != General.FtaProgram.CurrentSystem))
                                    {
                                        //做历史记录
                                        //做历史记录
                                        General.FtaProgram.CurrentSystem.TakeBehavor(
                                            oDeleted
                                            , Behavior.Enum.ElementOperate.Remove
                                            , Behavior.Enum.ElementOperate.Add
                                            , oDeleted.Parent
                                            , null);
                                    }

                                    ids = this.ftaTable.TableEvents.DeleteNodeAndChildren(selectedDrawData, true);
                                }

                                if (ids.Count > 0)
                                {
                                    MsgBox.Show($"删除的转移门有\r\n{string.Join("\r\n", ids.ToArray())}");
                                }
                            }
                            break;
                        }
                    case nameof(StringModel.DeleteTop):
                        {
                            //判断如果是唯一顶层非转移门节点，不可删除自身
                            bool checkOnly = false;
                            foreach (DrawData TopDa in General.FtaProgram.CurrentSystem.Roots)
                            {
                                if (General.FtaProgram.CurrentSystem.TranferGates.ContainsKey(TopDa.Identifier) == false && General.FtaProgram.CurrentSystem.Roots[0] == selectedDrawData)
                                {
                                    checkOnly = true;
                                    break;
                                }
                            }

                            string TipM = General.FtaProgram.String.DeleteNodesMessage;
                            if (checkOnly)
                            {
                                TipM = General.FtaProgram.String.DeleteNodesAndTransferMessageOnlyTop;
                            }

                            if (MsgBox.Show(TipM, General.FtaProgram.String.ConfirmTitleDelete, MessageBoxButtons.OKCancel) == DialogResult.OK)
                            {
                                var ids = new List<string>();

                                if (General.FtaProgram.CurrentSystem.Roots.Where(o => o.ThisGuid == selectedDrawData.ThisGuid).Count() > 0)
                                {
                                    //做历史记录
                                    //是转移门
                                    if (General.FtaProgram.CurrentSystem.TranferGates.Where(o => o.Key == selectedDrawData.Identifier).Count() > 0)
                                    {
                                        General.FtaProgram.CurrentSystem.TakeBehavor();
                                    }
                                    else
                                    {
                                        if (checkOnly == false)
                                        {
                                            DrawData oDeleted = selectedDrawData;
                                            if ((null != oDeleted)
                                                && (null != General.FtaProgram)
                                                && (null != General.FtaProgram.CurrentSystem))
                                            {
                                                //做历史记录
                                                General.FtaProgram.CurrentSystem.TakeBehavor(
                                                    oDeleted
                                                    , Behavior.Enum.ElementOperate.Remove
                                                    , Behavior.Enum.ElementOperate.Add
                                                    , oDeleted.Parent
                                                    , null);
                                            }
                                        }
                                        else
                                        {
                                            General.FtaProgram.CurrentSystem.TakeBehavor();
                                        }
                                    }

                                    if (checkOnly == false)
                                    {
                                        General.IsIgnoreTreeListFocusNodeChangeEvent = false;
                                        ids.AddRange(this.ftaTable.TableEvents.DeleteNodeAndChildren(selectedDrawData, false));
                                        if (General.FtaProgram.CurrentSystem != null && General.FtaProgram.CurrentSystem.Roots.Count > 0 && General.FtaProgram.CurrentRoot != null)
                                        {
                                            General.FTATree.FocusedNode = null;

                                            Predicate<TreeListNode> match = new Predicate<TreeListNode>(d => (d.GetValue(FixedString.COLUMNAME_DATA) != null && d.GetValue(FixedString.COLUMNAME_DATA) == General.FtaProgram.CurrentRoot));
                                            TreeListNode nd = General.FTATree.FindNode(match);
                                            General.FTATree.FocusedNode = nd;
                                        }
                                    }
                                    else
                                    {
                                        for (int i = selectedDrawData.Children.Count - 1; i >= 0; i--)
                                        {
                                            ids = this.ftaTable.TableEvents.DeleteNodeAndChildren(selectedDrawData.Children[i], false);
                                        }
                                    }
                                }
                                else
                                {
                                    //情况: 移除一个节点
                                    DrawData oDeleted = selectedDrawData;
                                    if ((null != oDeleted)
                                        && (null != General.FtaProgram)
                                        && (null != General.FtaProgram.CurrentSystem))
                                    {
                                        //做历史记录
                                        General.FtaProgram.CurrentSystem.TakeBehavor(
                                            oDeleted
                                            , Behavior.Enum.ElementOperate.Remove
                                            , Behavior.Enum.ElementOperate.Add
                                            , oDeleted.Parent
                                            , null);
                                    }

                                    ids = this.ftaTable.TableEvents.DeleteNodeAndChildren(selectedDrawData, true);
                                }

                                if (ids.Count > 0)
                                {
                                    MsgBox.Show($"删除的转移门有\r\n{string.Join("\r\n", ids.ToArray())}");
                                }
                            }
                            break;
                        }
                    case nameof(StringModel.ExpandNodes):
                        {
                            treeList_FTATable.ExpandAll();
                            //treeList_FTATable.BestFitColumns();
                            break;
                        }
                    case nameof(StringModel.CollapseNodes):
                        {
                            treeList_FTATable.CollapseAll();
                            //treeList_FTATable.BestFitColumns();
                            break;
                        }
                    case nameof(StringModel.FreezeColumn):
                        {
                            //先取消多余的冻结列
                            for (int i = treeList_FTATable.VisibleColumns.Count - 1; i > treeList_FTATable.FocusedColumn.VisibleIndex; i--)
                            {
                                TreeListColumn col = treeList_FTATable.GetColumnByVisibleIndex(i);
                                if (col != null && col.Fixed != FixedStyle.None) col.Fixed = FixedStyle.None;
                            }
                            //冻结需要的列
                            for (int i = 0; i <= treeList_FTATable.FocusedColumn.VisibleIndex; i++)
                            {
                                TreeListColumn col = treeList_FTATable.GetColumnByVisibleIndex(i);
                                if (col != null) col.Fixed = FixedStyle.Left;
                            }
                            break;
                        }
                    case nameof(StringModel.UnfreezeColumns):
                        {
                            for (int i = treeList_FTATable.VisibleColumns.Count - 1; i >= 0; i--)
                            {
                                TreeListColumn col = treeList_FTATable.GetColumnByVisibleIndex(i);
                                if (col != null && col.Fixed != FixedStyle.None) col.Fixed = FixedStyle.None;
                            }
                            break;
                        }
                    case nameof(StringModel.Redo):
                        this.CurrentSystem_Redo_Event();
                        break;
                    case nameof(StringModel.Undo):
                        this.CurrentSystem_Undo_Event();
                        break;
                    default: break;
                }
            }

        }

        /// <summary>
        /// FTA图的右键菜单事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DiagramControlMenuClick(object sender, ItemClickEventArgs e)
        {
            General.TryCatch(() =>
            {
                try
                {
                    //所有菜单基本处理完毕，都是自己定义聚焦对象，所以这里忽略表的选择节点变化
                    General.IsIgnoreTreeListFocusNodeChangeEvent = true;
                    //选中的对象
                    List<DrawData> selectedDrawData = ftaDiagram.GetSelectedDataList();

                    if (selectedDrawData.Count > 0)
                    {
                        //这里检查可能重复，但可以确保万无一失
                        if (!this.GetBarItemIsEnabled(e.Item, selectedDrawData[0], null, null, null)) return;

                        var keyName = General.GetKeyName(e.Item.Caption);

                        if (keyName == nameof(StringModel.AndGate) ||
                keyName == nameof(StringModel.OrGate) ||
                keyName == nameof(StringModel.BasicEvent) ||
                keyName == nameof(StringModel.PriorityAndGate) ||
                keyName == nameof(StringModel.XORGate) ||
                keyName == nameof(StringModel.VotingGate) ||
                keyName == nameof(StringModel.RemarksGate) ||
                keyName == nameof(StringModel.UndevelopedEvent) ||
                keyName == nameof(StringModel.TransferInGate) ||
                keyName == nameof(StringModel.HouseEvent) ||
                keyName == nameof(StringModel.ConditionEvent))
                        {
                            this.InsertNode(this.ftaDiagram.SelectedData.First(), General.GetEnumByName<DrawType>(keyName));
                        }
                        else if (e.Item.Name == nameof(FTAControl.barButtonItem_DeleteNodeDiagram) || e.Item.Name == nameof(FTAControl.barButtonItem_DeleteNodeRibbon) || e.Item.Name == nameof(FTAControl.barButtonItem_DeleteLevelDiagram) || e.Item.Name == nameof(FTAControl.barButtonItem_DeleteLevelRibbon))
                        {
                            if (MsgBox.Show(General.FtaProgram.String.ConfirmDeletionMessage, General.FtaProgram.String.ConfirmTitleDelete, MessageBoxButtons.OKCancel) == DialogResult.OK)
                            {
                                //情况: 移除一个节点
                                DrawData oDeleted = selectedDrawData[0];
                                if ((null != oDeleted)
                                    && (null != General.FtaProgram)
                                    && (null != General.FtaProgram.CurrentSystem))
                                {
                                    //做历史记录
                                    General.FtaProgram.CurrentSystem.TakeBehavor(
                                        oDeleted
                                        , Behavior.Enum.ElementOperate.Remove
                                        , Behavior.Enum.ElementOperate.Add
                                        , oDeleted.Parent
                                        ,
                                        (((null != oDeleted.Children)
                                        && (0x00 < oDeleted.Children.Count))
                                        ? new List<Behavior.ObjectBehavior>(oDeleted.Children)
                                        : null));
                                }

                                this.ftaTable.TableEvents.DeleteNode(selectedDrawData[0]);
                            }
                        }
                        // 删除节点包括子节点
                        else if (e.Item.Name == nameof(FTAControl.barButtonItem_DeleteNodesDiagram) || e.Item.Name == nameof(FTAControl.barButtonItem_DeleteNodesRibbon))
                        {
                            if (MsgBox.Show(General.FtaProgram.String.DeleteNodesMessage, General.FtaProgram.String.ConfirmTitleDelete, MessageBoxButtons.OKCancel) == DialogResult.OK)
                            {
                                if (selectedDrawData.Count == 1)
                                {
                                    if (General.FtaProgram.CurrentSystem.Roots.Where(o => o.ThisGuid == selectedDrawData[0].ThisGuid).Count() > 0)
                                    {
                                        //做历史记录
                                        General.FtaProgram.CurrentSystem.TakeBehavor();

                                        for (int i = selectedDrawData[0].Children.Count - 1; i >= 0; i--)
                                        {
                                            this.ftaTable.TableEvents.DeleteNodeAndChildren(selectedDrawData[0].Children[i]);
                                        }
                                    }
                                    else
                                    {
                                        //情况: 移除一个节点(含子结点)
                                        DrawData oDeleted = selectedDrawData[0];
                                        if ((null != oDeleted)
                                            && (null != General.FtaProgram)
                                            && (null != General.FtaProgram.CurrentSystem))
                                        {
                                            //做历史记录
                                            General.FtaProgram.CurrentSystem.TakeBehavor(
                                                oDeleted
                                                , Behavior.Enum.ElementOperate.Remove
                                                , Behavior.Enum.ElementOperate.Add
                                                , oDeleted.Parent
                                                , null);
                                        }

                                        this.ftaTable.TableEvents.DeleteNodeAndChildren(selectedDrawData[0], false);
                                    }
                                }
                                else
                                {
                                    //做历史记录
                                    General.FtaProgram.CurrentSystem.TakeBehavor();

                                    this.DeleteMulitEvents(selectedDrawData);
                                }
                            }
                        }
                        else if (e.Item.Name == nameof(FTAControl.barButtonItem_DeleteTransferDiagram) || e.Item.Name == nameof(FTAControl.barButtonItem_DeleteTransferRibbon) || e.Item.Name == nameof(FTAControl.barButtonItem_DeleteTopDiagram) || e.Item.Name == nameof(FTAControl.barButtonItem_DeleteTopRibbon))
                        {
                            //判断如果是唯一顶层非转移门节点，不可删除自身
                            bool checkOnly = false;
                            foreach (DrawData TopDa in General.FtaProgram.CurrentSystem.Roots)
                            {
                                if (General.FtaProgram.CurrentSystem.TranferGates.ContainsKey(TopDa.Identifier) == false && General.FtaProgram.CurrentSystem.Roots[0].ThisGuid == selectedDrawData[0].ThisGuid)
                                {
                                    checkOnly = true;
                                    break;
                                }
                            }

                            string TipM = General.FtaProgram.String.DeleteNodesMessage;
                            if (checkOnly)
                            {
                                TipM = General.FtaProgram.String.DeleteNodesAndTransferMessageOnlyTop;
                            }

                            if (MsgBox.Show(TipM, General.FtaProgram.String.ConfirmTitleDelete, MessageBoxButtons.OKCancel) == DialogResult.OK)
                            {
                                var ids = new List<string>();

                                if (General.FtaProgram.CurrentSystem.Roots.Where(o => o.ThisGuid == selectedDrawData[0].ThisGuid).Count() > 0)
                                {
                                    //做历史记录
                                    //是转移门
                                    if (General.FtaProgram.CurrentSystem.TranferGates.Where(o => o.Key == selectedDrawData[0].Identifier).Count() > 0)
                                    {
                                        General.FtaProgram.CurrentSystem.TakeBehavor();
                                    }
                                    else
                                    {
                                        if (checkOnly == false)
                                        {
                                            DrawData oDeleted = selectedDrawData[0];
                                            if ((null != oDeleted)
                                                && (null != General.FtaProgram)
                                                && (null != General.FtaProgram.CurrentSystem))
                                            {
                                                //做历史记录
                                                General.FtaProgram.CurrentSystem.TakeBehavor(
                                                    oDeleted
                                                    , Behavior.Enum.ElementOperate.Remove
                                                    , Behavior.Enum.ElementOperate.Add
                                                    , oDeleted.Parent
                                                    , null);
                                            }
                                        }
                                        else
                                        {
                                            General.FtaProgram.CurrentSystem.TakeBehavor();
                                        }
                                    }

                                    if (checkOnly == false)
                                    {
                                        General.IsIgnoreTreeListFocusNodeChangeEvent = false;
                                        ids.AddRange(this.ftaTable.TableEvents.DeleteNodeAndChildren(selectedDrawData[0], false));
                                        if (General.FtaProgram.CurrentSystem != null && General.FtaProgram.CurrentSystem.Roots.Count > 0 && General.FtaProgram.CurrentRoot != null)
                                        {
                                            General.FTATree.FocusedNode = null;

                                            Predicate<TreeListNode> match = new Predicate<TreeListNode>(d => (d.GetValue(FixedString.COLUMNAME_DATA) != null && d.GetValue(FixedString.COLUMNAME_DATA) == General.FtaProgram.CurrentRoot));
                                            TreeListNode nd = General.FTATree.FindNode(match);
                                            General.FTATree.FocusedNode = nd;
                                        }
                                    }
                                    else
                                    {
                                        for (int i = selectedDrawData[0].Children.Count - 1; i >= 0; i--)
                                        {
                                            ids = this.ftaTable.TableEvents.DeleteNodeAndChildren(selectedDrawData[0].Children[i], false);
                                        }
                                    }
                                }
                                else
                                {
                                    //情况: 移除一个节点
                                    DrawData oDeleted = selectedDrawData[0];
                                    if ((null != oDeleted)
                                        && (null != General.FtaProgram)
                                        && (null != General.FtaProgram.CurrentSystem))
                                    {
                                        //做历史记录
                                        General.FtaProgram.CurrentSystem.TakeBehavor(
                                            oDeleted
                                            , Behavior.Enum.ElementOperate.Remove
                                            , Behavior.Enum.ElementOperate.Add
                                            , oDeleted.Parent
                                            , null);
                                    }

                                    ids = this.ftaTable.TableEvents.DeleteNodeAndChildren(selectedDrawData[0], true);
                                }

                                if (ids.Count > 0)
                                {
                                    MsgBox.Show($"删除的转移门有\r\n{string.Join("\r\n", ids.ToArray())}");
                                }
                            }
                        }
                        //插入重复事件
                        else if (e.Item.Name == nameof(FTAControl.barButtonItem_InsertRepeatEvent))
                        {
                            //找到所有重复和不重复的事件id
                            List<DrawData> datas = General.FtaProgram.CurrentSystem.GetAllDatas();
                            var noRepeatEvents =
                               from n in datas
                               where !General.FtaProgram.CurrentSystem.RepeatedEvents.ContainsKey(n.Identifier) && (n.Type == DrawType.BasicEvent || n.Type == DrawType.HouseEvent || n.Type == DrawType.UndevelopedEvent)
                               group n by n.Identifier;
                            Dictionary<string, DrawData> no_Repeat = new Dictionary<string, DrawData>();
                            foreach (var g in noRepeatEvents)
                            {
                                no_Repeat.Add(g.Key, g.ToArray()[0]);
                            }
                            List<string> ids = new List<string>();
                            List<string> id_No_Repeat = no_Repeat.Keys.ToList();
                            List<string> id_Repeat = General.FtaProgram.CurrentSystem.RepeatedEvents.Keys.ToList();
                            ids.AddRange(id_No_Repeat);
                            ids.AddRange(id_Repeat);
                            //让用户选取id
                            NewRepeatOrTransfer form = new NewRepeatOrTransfer(General.FtaProgram.String, General.FtaProgram.String.InsertRepeatedEvent, ids);
                            if (form.ShowDialog() == DialogResult.Cancel)
                            {
                                form.Dispose();
                                return;
                            }
                            string id = form.GetInfo();
                            form.Dispose();
                            if (!string.IsNullOrEmpty(id))
                            {
                                DrawData source = null;
                                if (no_Repeat.ContainsKey(id)) source = no_Repeat[id];
                                else if (General.FtaProgram.CurrentSystem.RepeatedEvents.ContainsKey(id) && General.FtaProgram.CurrentSystem.RepeatedEvents[id].Count > 0) source = General.FtaProgram.CurrentSystem.RepeatedEvents[id].FirstOrDefault();
                                //复制选择的对象并插入
                                if (source != null)
                                {
                                    DrawData result = this.ftaTable.PasteRepeatedEvent(selectedDrawData[0], true, source, false);
                                    if (result != null)
                                    {
                                        this.ftaDiagram.FocusOn(selectedDrawData[0]);
                                        this.ftaTable.FocusOn(result);
                                    }

                                    //情况: 插入一个子级(重复事件)
                                    if ((null != result)
                                        && (null != General.FtaProgram)
                                        && (null != General.FtaProgram.CurrentSystem))
                                    {
                                        //做历史记录
                                        General.FtaProgram.CurrentSystem.TakeBehavor(
                                            result
                                            , Behavior.Enum.ElementOperate.Creation
                                            , Behavior.Enum.ElementOperate.Deletion
                                            , selectedDrawData[0]
                                            , null);
                                    }

                                    //使得可以不停插入
                                    if (result != null)
                                    {
                                        if (this.ftaDiagram.DiagramEvents.AddingType != result.Type)
                                        {
                                            ReSetCheckState();
                                            this.ftaDiagram.DiagramEvents.AddingType = result.Type;
                                            GalleryItem item_Trans = ribbonGalleryBarItem_GraphicTool.Gallery.GetItemByValue(DrawData.GetDescriptionByEnum(result.Type));
                                            item_Trans.Checked = true;
                                        }
                                        this.ftaDiagram.DiagramEvents.RepeatedEvent = result;
                                    }
                                }
                            }
                        }
                        // 上一个/下一个重复事件
                        //TODO:可能受到影响
                        else if (e.Item.Name == nameof(FTAControl.barButtonItem_PreviousRepeatEvent) || e.Item.Name == nameof(FTAControl.barButtonItem_NextRepeatEvent))
                        {
                            var incrment = e.Item == barButtonItem_PreviousRepeatEvent ? -1 : 1;
                            var current = ftaDiagram.SelectedData.FirstOrDefault();
                            var repeatEvents = General.FtaProgram.CurrentSystem.RepeatedEvents.FirstOrDefault(o => o.Key == current.Identifier).Value.ToList();
                            var index = repeatEvents.IndexOf(current) + incrment;
                            if (index >= repeatEvents.Count) index = 0;
                            else if (index < 0) index = repeatEvents.Count - 1;
                            var repeatedEvents = General.FtaProgram.CurrentSystem.RepeatedEvents;
                            this.ftaDiagram.FocusOn(repeatEvents[index]);
                        }

                        //进入转移门
                        else if (e.Item.Name == nameof(FTAControl.barButtonItem_BreakIntoTransfer) || e.Item.Name == nameof(FTAControl.barButtonItem_MenuBreakIntoTransfer))
                        {

                            //情况: 节点改变属性(改变子项, 添加根项)
                            //if ((null != General.FtaProgram)
                            //    && (null != General.FtaProgram.CurrentSystem))
                            {
                                //做历史记录
                                General.FtaProgram.CurrentSystem.TakeBehavor(
                                   );
                            }

                            //断绝现有关系                   
                            DrawData data_Parent = selectedDrawData[0].Parent;
                            //复制转移门副本
                            DrawData data_Copied = selectedDrawData[0].CopyDrawData();
                            if (data_Copied == null) return;
                            data_Copied.Identifier = selectedDrawData[0].Identifier;
                            data_Copied.Type = DrawType.TransferInGate;

                            //本体关系重置
                            data_Parent.Children.Remove(selectedDrawData[0]);
                            selectedDrawData[0].Parent = null;
                            General.FtaProgram.CurrentSystem.Roots.Add(selectedDrawData[0]);
                            VirtualDrawData vData = treeList_FTATable.DataSource as VirtualDrawData;
                            vData.data.Children.Add(selectedDrawData[0]);

                            //副本关系重置
                            data_Copied.Parent = data_Parent;
                            data_Parent.Children.Add(data_Copied);

                            //转移门集合维护                       
                            General.FtaProgram.CurrentSystem.TranferGates.Add(data_Copied.Identifier, new HashSet<DrawData>() { selectedDrawData[0], data_Copied });

                            //刷新视图
                            treeList_FTATable.RefreshDataSource();

                            this.ftaDiagram.FocusOn(selectedDrawData[0]);
                            this.ftaTable.FocusOn(selectedDrawData[0]);

                            //切换视图必须加，否则会有多余图形
                            this.currentDiagram.ClearSelection();
                            this.ftaDiagram.Refresh(true);
                        }
                        //折叠转移门
                        else if (e.Item.Name == nameof(FTAControl.barButtonItem_CollapseTransfer) || e.Item.Name == nameof(FTAControl.barButtonItem_MenuCollapseTransfer))
                        {

                            //情况: 节点改变属性(改变子项, 添加根项)
                            //if ((null != General.FtaProgram)
                            //    && (null != General.FtaProgram.CurrentSystem))
                            {
                                //做历史记录
                                General.FtaProgram.CurrentSystem.TakeBehavor(
                                   );
                            }

                            HashSet<DrawData> transfer = General.FtaProgram.CurrentSystem.TranferGates[selectedDrawData[0].Identifier];
                            bool isResetDiagramPool = false;//当转移门上折叠时需要重新刷一下图形池
                            DrawData data_Trans = null;
                            if (selectedDrawData[0].Type == DrawType.TransferInGate)
                            {
                                isResetDiagramPool = true;
                                data_Trans = selectedDrawData[0];
                                //转移门集合维护
                                transfer.Remove(selectedDrawData[0]);
                                selectedDrawData[0] = transfer.FirstOrDefault();
                            }
                            else
                            {
                                //转移门集合维护
                                transfer.Remove(selectedDrawData[0]);
                                data_Trans = transfer.FirstOrDefault();
                            }

                            transfer.Clear();
                            General.FtaProgram.CurrentSystem.TranferGates.Remove(selectedDrawData[0].Identifier);

                            //本体关系重置
                            //如果本体是根节点
                            if (General.FtaProgram.CurrentSystem.Roots.Contains(selectedDrawData[0]))
                            {
                                General.FtaProgram.CurrentSystem.Roots.Remove(selectedDrawData[0]);
                                VirtualDrawData vData = treeList_FTATable.DataSource as VirtualDrawData;
                                vData.data.Children.Remove(selectedDrawData[0]);
                            }

                            if (selectedDrawData[0].Parent != null && selectedDrawData[0].Parent.Children != null)
                            {
                                selectedDrawData[0].Parent.Children.Remove(selectedDrawData[0]);
                            }

                            DrawData data_Trans_Parent = data_Trans.Parent;
                            data_Trans.Delete();
                            data_Trans_Parent.Children.Add(selectedDrawData[0]);
                            selectedDrawData[0].Parent = data_Trans_Parent;

                            //刷新视图
                            treeList_FTATable.RefreshDataSource();
                            if (isResetDiagramPool) this.ftaDiagram.DiagramEvents.DiagramItemPool.ResetData();
                            this.ftaDiagram.FocusOn(selectedDrawData[0]);
                            this.ftaTable.FocusOn(selectedDrawData[0]);

                            //切换视图必须加，否则会有多余图形
                            this.currentDiagram.ClearSelection();
                            this.ftaDiagram.Refresh(true);
                        }
                        //转到
                        //else if (barButtonItem_TransferTo.ContainsItem(e.Item))
                        else if (string.IsNullOrEmpty(e.Item.Name))
                        {
                            if (e.Item.Tag != null && e.Item.Tag.GetType() == typeof(DrawData))
                            {
                                this.ftaDiagram.FocusOn(e.Item.Tag as DrawData, true);
                                this.ftaTable.FocusOn(e.Item.Tag as DrawData);

                                //切换视图必须加，否则会有多余图形
                                this.currentDiagram.ClearSelection();
                                this.ftaDiagram.Refresh(true);
                            }
                        }
                        //剪切
                        else if (e.Item.Name == nameof(FTAControl.barButtonItem_Cut) || e.Item.Name == nameof(FTAControl.barButtonItem_MenuCut))
                        {
                            FTATableDiagram_CopyOrCut_WithOrNotRecurse(selectedDrawData[0], false, true);
                        }
                        //编辑
                        else if (e.Item.Name == nameof(FTAControl.barButtonItem_ShapeEdit))
                        {
                            ShapeEdit(selectedDrawData[0]);
                        }
                        //复制
                        else if (e.Item.Name == nameof(FTAControl.barButtonItem_Copy) || e.Item.Name == nameof(FTAControl.barButtonItem_MenuCopy))
                        {
                            FTATableDiagram_CopyOrCut_WithOrNotRecurse(selectedDrawData[0], true, true);
                        }
                        //粘贴
                        else if (e.Item.Name == nameof(FTAControl.barButtonItem_Paste) || e.Item.Name == nameof(FTAControl.barButtonItem_MenuPaste))
                        {
                            //情况: 函数内又有TB
                            DrawData data = FTATableDiagram_Paste(selectedDrawData[0]);
                            if (data != null)
                            {
                                this.ftaDiagram.FocusOn(selectedDrawData[0]);
                                this.ftaTable.FocusOn(data);
                            }
                        }
                        //粘贴重复事件
                        else if (e.Item.Name == nameof(FTAControl.barButtonItem_PasteRepeatEvent) || e.Item.Name == nameof(FTAControl.barButtonItem_MenuPasteRepeated))
                        {
                            DrawData result = this.ftaTable.PasteRepeatedEvent(selectedDrawData[0], this.ftaTable.TableEvents.IsCopyNode, General.CopyCutObject, General.FTATableDiagram_Is_CopyOrCut_Recurse);
                            if (result != null)
                            {
                                this.ftaDiagram.FocusOn(selectedDrawData[0]);
                                this.ftaTable.FocusOn(result);
                            }

                            //情况: 黏贴(重复事件)
                            if ((null != result)
                                && (null != General.FtaProgram)
                                && (null != General.FtaProgram.CurrentSystem))
                            {
                                //做历史记录
                                General.FtaProgram.CurrentSystem.TakeBehavor(
                                    result
                                    , Behavior.Enum.ElementOperate.Creation
                                    , Behavior.Enum.ElementOperate.Deletion
                                    , selectedDrawData[0]
                                    , null);
                            }
                        }
                        //显示割级计算结果
                        else if (e.Item.Name == nameof(FTAControl.barButtonItem_ShowFTACalEvent))
                        {
                            if (this.ftaRibbon.analysisNew.AllGJCalsList.Count == 0)
                            {
                                return;
                            }
                            ShowFTACal f = new View.Diagram.ShowFTACal();
                            f.SelectNode = selectedDrawData[0];
                            f.AllGJCalsList = this.ftaRibbon.analysisNew.AllGJCalsList;
                            f.ShowDialog();
                        }
                        //导出到模型库
                        else if (e.Item.Name == nameof(FTAControl.barButtonItem_DiaExportToModel))
                        {
                            if (Directory.Exists(Environment.CurrentDirectory + "\\Event Library") == false)
                            {
                                Directory.CreateDirectory(Environment.CurrentDirectory + "\\Event Library");
                            }
                            var dialog = new SaveFileDialog { Filter = FixedString.MODEL_FILTER, FileName = $"{selectedDrawData[0].Identifier}_{DateTime.Now.ToString("yyyyMMddhhmm")}" };
                            dialog.InitialDirectory = Environment.CurrentDirectory + "\\Event Library";

                            if (General.FtaProgram.Setting.CommonEventLibraryPath != "" && Directory.Exists(General.FtaProgram.Setting.CommonEventLibraryPath))
                            {
                                dialog.InitialDirectory = General.FtaProgram.Setting.CommonEventLibraryPath;
                            }

                            if (dialog.ShowDialog() == DialogResult.OK)
                            {
                                FileInfo filesys = new FileInfo(dialog.FileName);

                                DrawData da = selectedDrawData[0].CopyDrawDataRecurse();

                                ReplaceTrans(da, General.FtaProgram.CurrentSystem.TranferGates);

                                File.WriteAllText(filesys.FullName, Newtonsoft.Json.JsonConvert.SerializeObject(da), Encoding.UTF8);
                            }
                        }
                    }

                    //复制全图到剪贴板
                    if (e.Item.Name == nameof(FTAControl.barButtonItem_MenuCopyCurrentView))
                    {
                        if (General.FtaProgram.CurrentSystem != null && General.FtaProgram.CurrentRoot != null)
                        {
                            FTATableDiagram_CopyCurrentView(General.FtaProgram.CurrentRoot, General.FtaProgram.CurrentRoot.GetAllData(General.FtaProgram.CurrentSystem));
                        }
                    }

                    //复制选中项截图到剪贴板
                    if (e.Item.Name == nameof(FTAControl.barButtonItem_MenuCopyCurrentSelected))
                    {
                        if (General.FtaProgram.CurrentSystem != null && General.FtaProgram.CurrentRoot != null && General.DiagramControl.SelectedItems.Count > 0)
                        {
                            FTATableDiagram_CopyCurrentView(General.FtaProgram.CurrentRoot, selectedDrawData);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    General.IsIgnoreTreeListFocusNodeChangeEvent = false;
                }
            });
        }

        private void MCL_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.ccaFunction.MinimalCutsetList(General.FtaProgram.CurrentSystem, General.FtaProgram.CurrentRoot, "");
        }

        private void Ic_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.ccaFunction.IndependenceCheck(General.FtaProgram.CurrentSystem, General.FtaProgram.CurrentRoot);
        }

        private void treeList_FTATable_Load(object sender, EventArgs e)
        {
            General.IsTreeListLoaded = true;
            if (General.MessageFromSimfia != "")
            {
                General.InvokeHandler(GlobalEvent.ImprotInfoFromProcess, General.MessageFromSimfia);
            }
        }

        private void barButtonItem_Exit_ItemClick(object sender, ItemClickEventArgs e)
        {
            ((Form1)this.Parent).Close();
        }

        /// <summary>
        /// 显示属性编辑页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem_Properties_ItemClick(object sender, ItemClickEventArgs e)
        {
            ribbonPage_Properties.Visible = true;
            ribbonControl_FTA.SelectedPage = ribbonPage_Properties;
        }

        /// <summary>
        /// 隐藏属性编辑页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem_CloseProperties_ItemClick(object sender, ItemClickEventArgs e)
        {
            ribbonPage_Properties.Visible = false;
            ribbonControl_FTA.SelectedPage = ribbonPage_View;
        }

        /// <summary>
        /// 定位顶点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem_TopGatePos_ItemClick(object sender, ItemClickEventArgs e)
        {
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
        }

        /// <summary>
        /// 插入级别（门类型）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem_InsertLevel_ItemClick(object sender, ItemClickEventArgs e)
        {
            General.TryCatch(() =>
            {
                //获取当前选中的对象
                var selectedDrawData = default(DrawData);
                if (treeList_FTATable.FocusedNode != null)
                {
                    object obj = treeList_FTATable.FocusedNode.GetValue(FixedString.COLUMNAME_DATA);
                    if (obj != null && obj.GetType() == typeof(DrawData))
                    {
                        selectedDrawData = obj as DrawData;
                    }
                }
                else
                {
                    return;
                }

                //当前选中对象是门类型，且有子节点时可插入级别
                if (selectedDrawData.IsGateType && selectedDrawData.Children.Count > 0)
                {
                    DrawData parentData = selectedDrawData;
                    DrawData childData = InsertLevelNode(parentData);
                    if (childData != null)
                    {
                        this.ftaDiagram.FocusOn(childData);
                        this.ftaTable.FocusOn(parentData);
                    }
                }
            });
        }

        /// <summary>
        /// 高亮割级
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem_HighLightCutSets_ItemClick(object sender, ItemClickEventArgs e)
        {
            General.TryCatch(() =>
            {
                if (General.DiagramItemPool.SelectedData.Count == 1 && General.HighLightCutSet?.Gallery?.Groups?.Count > 0)
                {
                    DrawData data_Selected = General.DiagramItemPool.SelectedData.FirstOrDefault();
                    if (General.FtaProgram.CurrentSystem != null && data_Selected.Cutset != null && data_Selected.Cutset.ListCutsets_Real != null
                        && data_Selected.Cutset.ListCutsets_Real.Count > 0)
                    {
                        //检测到已存在割级DockPanel直接显示
                        bool checkexist = false;
                        foreach (DockPanel dp in General.dockManager_FTA.Panels)
                        {
                            if (dp.Name == "DP_ShowCut")
                            {
                                dp.Visibility = DockVisibility.Visible;

                                dp.FloatSize = new System.Drawing.Size(963, 588);
                                int xWidth = SystemInformation.PrimaryMonitorSize.Width;//获取显示器屏幕宽度
                                int yHeight = SystemInformation.PrimaryMonitorSize.Height;//高度 
                                dp.FloatLocation = new System.Drawing.Point(20, yHeight - dp.Height - 70);

                                General.dockManager_FTA.ActivePanel = dp;
                                checkexist = true;
                            }
                        }

                        if (checkexist == false)//没找到割级DockPanel先新建再显示
                        {
                            ShowCut SENew = new ShowCut();
                            DockPanel DP = General.dockManager_FTA.AddPanel(DockingStyle.Float);
                            DP.Name = "DP_ShowCut";
                            DP.Dock = DockingStyle.Float;
                            DP.Options.AllowDockBottom = false;
                            DP.Options.AllowDockFill = false;
                            DP.Options.AllowDockLeft = false;
                            DP.Options.AllowDockRight = false;
                            DP.Options.AllowDockTop = false;
                            DP.Options.AllowDockAsTabbedDocument = false;
                            DP.Text = General.FtaProgram.String.ShowCut;
                            SENew.Dock = DockStyle.Fill;
                            SENew.RefreshDatas(true);
                            General.ShowCut = SENew;
                            SENew.TopLevel = false;
                            DP.Controls.Add(SENew);
                            General.dockManager_FTA.ActivePanel = DP;

                            DP.FloatSize = new System.Drawing.Size(963, 588);
                            int xWidth = SystemInformation.PrimaryMonitorSize.Width;//获取显示器屏幕宽度
                            int yHeight = SystemInformation.PrimaryMonitorSize.Height;//高度
                            DP.FloatLocation = new System.Drawing.Point(20, yHeight - DP.Height - 70);
                        }

                        ShowCut SE = new ShowCut();
                        if (General.ShowCut != null)
                        {
                            SE = General.ShowCut;
                        }
                        else
                        {
                            return;
                        }
                        SE.RefreshText();
                        SE.simpleButton2.PerformClick();
                        SE.Show();
                    }
                    else
                    {
                        MsgBox.Show(General.FtaProgram.String.MastCal);
                    }
                }
                else
                {
                    MsgBox.Show(General.FtaProgram.String.AtLeastSelectOneNode);
                }
            });
        }

        /// <summary>
        /// 显示开始页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem_StartPage_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowHideStartPage(true);
        }

        /// <summary>
        /// 显示隐藏开始画面
        /// </summary>
        /// <param name="isOpen"></param>
        public void ShowHideStartPage(bool isOpen)
        {
            General.TryCatch(() =>
            {
                if (isOpen)
                {
                    dockPanel_Project.Visibility = DockVisibility.Hidden;
                    dockPanel_FTATable.Visibility = DockVisibility.Hidden;
                    dockPanel_FTADiagram.Visibility = DockVisibility.Hidden;
                    if (General.StartPage != null)
                    {
                        General.StartPage.Visible = true;
                        General.StartPage.ReloadRecentFiles();
                    }
                    bar1.Visible = false;
                }
                else
                {
                    if (General.StartPage != null)
                    {
                        General.StartPage.Visible = false;
                    }
                    bar1.Visible = true;
                }
            });
        }

        private void dockPanel_Project_VisibilityChanged(object sender, VisibilityChangedEventArgs e)
        {
            if (e.Visibility == DockVisibility.Visible)
            {
                ShowHideStartPage(false);
            }
        }

        private void dockPanel_FTATable_VisibilityChanged(object sender, VisibilityChangedEventArgs e)
        {
            if (e.Visibility == DockVisibility.Visible)
            {
                ShowHideStartPage(false);
            }
        }

        private void dockPanel_FTADiagram_VisibilityChanged(object sender, VisibilityChangedEventArgs e)
        {
            if (e.Visibility == DockVisibility.Visible)
            {
                ShowHideStartPage(false);
            }
        }

        /// <summary>
        /// 全屏显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem_ShowFullScreen_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (dockPanel_FTADiagram.Visibility == DockVisibility.Visible)
            {
                dockPanel_Project.Visibility = DockVisibility.Hidden;
                dockPanel_FTATable.Visibility = DockVisibility.Hidden;
                dockPanel_FTADiagram.Visibility = DockVisibility.Visible;
                ribbonControl_FTA.Minimized = true;
                bar1.Visible = false;
            }
        }

        /// <summary>
        /// 退出全屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void currentDiagram_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\u001b')//ESC退出
            {
                if (ribbonControl_FTA.Minimized && this.dockPanel_FTATable.Visibility == DockVisibility.Hidden && this.dockPanel_Project.Visibility == DockVisibility.Hidden)
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
                ribbonControl_FTA.Minimized = false;
                bar1.Visible = true;
            }
        }

        /// <summary>
        /// 显示隐藏工具栏菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem_HideShowToolbar_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (StartPage.Visible == false)
            {
                bar1.Visible = !bar1.Visible;
            }
        }

        /// <summary>
        /// 显示\隐藏项目列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem_HideShowProjectNavigator_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (this.dockPanel_Project.Visibility == DockVisibility.Hidden && StartPage.Visible == false)
            {
                //重载布局（否则Dock无法恢复原始布局）
                if (General.FtaProgram.Setting.Language == FixedString.LANGUAGE_CN_CN)
                {
                    this.dockManager_FTA.RestoreLayoutFromXml(Application.StartupPath + "\\FaultTreeStartPage_CN.xml");
                }
                else
                {
                    this.dockManager_FTA.RestoreLayoutFromXml(Application.StartupPath + "\\FaultTreeStartPage_EN.xml");
                }
            }
            else
            {
                this.dockPanel_Project.Visibility = DockVisibility.Hidden;
            }
        }

        /// <summary>
        /// 收缩项目列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem_FoldProject_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.treeList_Project.CollapseAll();
        }

        /// <summary>
        /// 展开项目列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem_ExpandProject_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.treeList_Project.ExpandAll();
        }

        /// <summary>
        /// 自动隐藏属性编辑页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ribbonControl_FTA_SelectedPageChanging(object sender, RibbonPageChangingEventArgs e)
        {
            if (e.Page != ribbonPage_Properties)
            {
                ribbonPage_Properties.Visible = false;
            }
        }

        /// <summary>
        /// 用户手册
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem_UserManual_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frm_UserManual f = new View.Ribbon.Start.Frm_UserManual();
            f.Show();
        }

        /// <summary>
        /// 快捷键目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem_ShowKeymap_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frm_Keymap f = new View.Ribbon.Start.Frm_Keymap();
            f.ShowDialog();
        }

        /// <summary>
        /// 软件更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem_CheckforUpdates_ItemClick(object sender, ItemClickEventArgs e)
        {
            //主版本.此版本.内部版本.修订版本
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            string ver = string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision.ToString("0000"));
            MsgBox.Show(General.FtaProgram.String.UpdateTip + ver);
        }

        /// <summary>
        /// 关于窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem_AboutSmarTree_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frm_About f = new View.Ribbon.Start.Frm_About();
            f.ShowDialog();
            SaveSettings();
        }

        /// <summary>
        /// 打开工程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem_OpenProject_ItemClick(object sender, ItemClickEventArgs e)
        {
            General.TryCatch(() =>
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();

                if (General.FtaProgram.Setting.DefaultFilePath != "" && Directory.Exists(General.FtaProgram.Setting.DefaultFilePath))
                {
                    dialog.SelectedPath = General.FtaProgram.Setting.DefaultFilePath;
                }

                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    LoadData(true, dialog.SelectedPath);

                    //定位
                    foreach (TreeListNode PNode in treeList_Project.Nodes)
                    {
                        if (((ProjectModel)PNode.Tag).ProjectPath == dialog.SelectedPath)
                        {
                            treeList_Project.FocusedNode = PNode;
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 打开故障树
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem_OpenFaultTree_ItemClick(object sender, ItemClickEventArgs e)
        {
            General.TryCatch(() =>
            {
                FileInfo file = new FileInfo(savingDataPath);
                if (!file.Directory.Exists) file.Directory.Create();

                string Datas = File.ReadAllText(this.savingDataPath, Encoding.UTF8);

                OpenFileDialog dialog = new OpenFileDialog();

                if (General.FtaProgram.Setting.DefaultFilePath != "" && Directory.Exists(General.FtaProgram.Setting.DefaultFilePath))
                {
                    dialog.InitialDirectory = General.FtaProgram.Setting.DefaultFilePath;
                }

                dialog.Filter = FixedString.FaultTree_FILTER;
                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    LoadData(false, dialog.FileName);

                    //定位
                    foreach (TreeListNode PNode in treeList_Project.Nodes)
                    {
                        foreach (TreeListNode GNode in PNode.Nodes)
                        {
                            if (GNode.Tag != null && ((ProjectModel)PNode.Tag).ProjectPath + "\\" + ((SystemModel)GNode.Tag).SystemName + FixedString.APP_EXTENSION == dialog.FileName)
                            {
                                treeList_Project.FocusedNode = GNode;
                                break;
                            }
                            foreach (TreeListNode SNode in GNode.Nodes)
                            {
                                if (((ProjectModel)PNode.Tag).ProjectPath + "\\" + ((SystemModel)SNode.Tag).SystemName + FixedString.APP_EXTENSION == dialog.FileName)
                                {
                                    treeList_Project.FocusedNode = SNode;
                                }
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 动态加载 更改门类型下拉选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barSubItem_ChangeGateType_GetItemData(object sender, EventArgs e)
        {
            General.TryCatch(() =>
            {
                //生成选项并绑定切换事件
                General.barSubItem_ChangeGateType.ItemLinks.Clear();
                if (this.currentDiagram.SelectedItems.Count == 0)
                {
                    return;
                }
                DrawData SeData = (DrawData)this.currentDiagram.SelectedItems[0].Tag;

                if (SeData.IsGateType && SeData.Type != DrawType.TransferInGate)
                {
                    //绑定切换类型的下拉选项（根据当前类型生成可切换的选项）
                    List<string> result = new List<string>();
                    List<DrawType> type = new List<DrawType>();

                    if (SeData.HasChild || SeData == General.FtaProgram.CurrentRoot)
                    {
                        type.AddRange(new DrawType[] { DrawType.AndGate, DrawType.OrGate, DrawType.RemarksGate });
                    }
                    else
                    {
                        type.AddRange(new DrawType[] { DrawType.AndGate, DrawType.OrGate, DrawType.RemarksGate, DrawType.BasicEvent, DrawType.HouseEvent, DrawType.UndevelopedEvent });
                    }

                    foreach (DrawType tmp in type)
                    {
                        var value = General.FtaProgram.String.GetType().GetProperties().FirstOrDefault(o => o.Name == tmp.ToString()).GetValue(General.FtaProgram.String).ToString();
                        result.Add(value);
                    }

                    //生成选项并绑定切换事件
                    General.barSubItem_ChangeGateType.ItemLinks.Clear();
                    foreach (string val in result)
                    {
                        BarButtonItem CheckButton = new BarButtonItem() { Caption = val, ButtonStyle = BarButtonStyle.Check };
                        General.barSubItem_ChangeGateType.ItemLinks.Add(CheckButton);
                        CheckButton.DownChanged += ChangeGateType_DownChanged;
                    }

                    //当前类型选中状态
                    string NowVal = General.CNEN_Changes(General.FtaProgram.Setting.Language, DrawData.GetDescriptionByEnum(SeData.Type));
                    foreach (BarItemLink item in General.barSubItem_ChangeGateType.ItemLinks)
                    {
                        if (item.Caption == NowVal)
                        {
                            ((DevExpress.XtraBars.BarButtonItemLink)item).Item.Down = true;
                        }
                        else
                        {
                            ((DevExpress.XtraBars.BarButtonItemLink)item).Item.Down = false;
                        }
                    }
                }
                else if (SeData.IsGateType == false && General.FtaProgram.CurrentSystem.RepeatedEvents != null && General.FtaProgram.CurrentSystem.RepeatedEvents.Count > 0 && General.FtaProgram.CurrentSystem.RepeatedEvents.ContainsKey(SeData.Identifier))//重复事件只能切换成其它事件类型
                {
                    List<string> result = new List<string>();
                    List<DrawType> type = new List<DrawType>();

                    type.AddRange(new DrawType[] { DrawType.BasicEvent, DrawType.HouseEvent, DrawType.UndevelopedEvent });

                    foreach (DrawType tmp in type)
                    {
                        var value = General.FtaProgram.String.GetType().GetProperties().FirstOrDefault(o => o.Name == tmp.ToString()).GetValue(General.FtaProgram.String).ToString();
                        result.Add(value);
                    }

                    General.barSubItem_ChangeGateType.ItemLinks.Clear();
                    foreach (string val in result)
                    {
                        BarButtonItem CheckButton = new BarButtonItem() { Caption = val, ButtonStyle = BarButtonStyle.Check };
                        General.barSubItem_ChangeGateType.ItemLinks.Add(CheckButton);
                        CheckButton.DownChanged += ChangeGateType_DownChanged;
                    }

                    string NowVal = General.CNEN_Changes(General.FtaProgram.Setting.Language, DrawData.GetDescriptionByEnum(SeData.Type));
                    foreach (BarItemLink item in General.barSubItem_ChangeGateType.ItemLinks)
                    {
                        if (item.Caption == NowVal)
                        {
                            ((DevExpress.XtraBars.BarButtonItemLink)item).Item.Down = true;
                        }
                        else
                        {
                            ((DevExpress.XtraBars.BarButtonItemLink)item).Item.Down = false;
                        }
                    }
                }
                else if (SeData.IsGateType == false)//普通事件类型可切换成事件或门类型
                {
                    List<string> result = new List<string>();
                    List<DrawType> type = new List<DrawType>();

                    if (SeData.HasChild || SeData == General.FtaProgram.CurrentRoot)
                    {
                        type.AddRange(new DrawType[] { DrawType.AndGate, DrawType.OrGate, DrawType.RemarksGate });
                    }
                    else
                    {
                        type.AddRange(new DrawType[] { DrawType.AndGate, DrawType.OrGate, DrawType.RemarksGate, DrawType.BasicEvent, DrawType.HouseEvent, DrawType.UndevelopedEvent });
                    }

                    foreach (DrawType tmp in type)
                    {
                        var value = General.FtaProgram.String.GetType().GetProperties().FirstOrDefault(o => o.Name == tmp.ToString()).GetValue(General.FtaProgram.String).ToString();
                        result.Add(value);
                    }

                    General.barSubItem_ChangeGateType.ItemLinks.Clear();
                    foreach (string val in result)
                    {
                        BarButtonItem CheckButton = new BarButtonItem() { Caption = val, ButtonStyle = BarButtonStyle.Check };
                        General.barSubItem_ChangeGateType.ItemLinks.Add(CheckButton);
                        CheckButton.DownChanged += ChangeGateType_DownChanged;
                    }

                    string NowVal = General.CNEN_Changes(General.FtaProgram.Setting.Language, DrawData.GetDescriptionByEnum(SeData.Type));
                    foreach (BarItemLink item in General.barSubItem_ChangeGateType.ItemLinks)
                    {
                        if (item.Caption == NowVal)
                        {
                            ((DevExpress.XtraBars.BarButtonItemLink)item).Item.Down = true;
                        }
                        else
                        {
                            ((DevExpress.XtraBars.BarButtonItemLink)item).Item.Down = false;
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 绑定更改门类型下拉选项选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeGateType_DownChanged(object sender, ItemClickEventArgs e)
        {
            if (((DevExpress.XtraBars.BarBaseButtonItem)e.Item).Down)
            {
                foreach (BarItemLink item in General.barSubItem_ChangeGateType.ItemLinks)
                {
                    if (item.Caption != e.Item.Caption)
                    {
                        ((DevExpress.XtraBars.BarButtonItemLink)item).Item.Down = false;
                    }
                }

                if (this.currentDiagram.SelectedItems.Count == 0 || this.currentDiagram.SelectedItems[0].Tag == null)
                {
                    return;
                }

                DrawData SeData = (DrawData)this.currentDiagram.SelectedItems[0].Tag;

                //第一次生成下拉选项时赋值当前状态会触发此事件，直接跳过属性修改
                if (SeData.Type == (DrawType)Enum.Parse(typeof(DrawType), General.GetKeyName(e.Item.Caption)))
                {
                    return;
                }

                //属性修改：Type类型
                General.IsIgnoreTreeListFocusNodeChangeEvent = true;
                General.InvokeHandler(GlobalEvent.PropertyEdited, new Tuple<string, DrawData, string>("Type", SeData, e.Item.Caption));
                General.IsIgnoreTreeListFocusNodeChangeEvent = false;

                //门或事件类型改变时，自动重命名（按最大序号+1的规则）
                int MaxIDGate = 0;
                int MaxIDEvent = 0;
                DrawData[] AllDatas = General.FtaProgram.CurrentSystem.GetAllDatas().ToArray();

                if (SeData.IsGateType)
                {
                    foreach (DrawData data in AllDatas)
                    {
                        string rel = "";//统计字符串长度，并设定增量。
                        for (int i = data.Identifier.Length - 1; i >= 0; i--)
                        {
                            int r = 0;
                            if (int.TryParse(data.Identifier[i].ToString(), out r))
                            {
                                rel = data.Identifier[i].ToString() + rel;
                            }
                            else
                            {
                                break;
                            }
                        }

                        try
                        {
                            string NewID = "0" + rel;
                            if (data.IsGateType && data != SeData && Convert.ToInt32(NewID) > MaxIDGate)
                            {
                                MaxIDGate = Convert.ToInt32(NewID);
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                else
                {
                    foreach (DrawData data in AllDatas)
                    {
                        string rel = ""; //统计字符串长度，并设定增量。
                        for (int i = data.Identifier.Length - 1; i >= 0; i--)
                        {
                            int r = 0;
                            if (int.TryParse(data.Identifier[i].ToString(), out r))
                            {
                                rel = data.Identifier[i].ToString() + rel;
                            }
                            else
                            {
                                break;
                            }
                        }

                        try
                        {
                            string NewID = "0" + rel;
                            if (data.IsGateType == false && data != SeData && Convert.ToInt32(NewID) > MaxIDEvent)
                            {
                                MaxIDEvent = Convert.ToInt32(NewID);
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                General.IsIgnoreTreeListFocusNodeChangeEvent = true;
                if (SeData.IsGateType)//当前对象如果被修改成门类型 
                {
                    General.InvokeHandler(GlobalEvent.PropertyEdited, new Tuple<string, DrawData, string>("Identifier", SeData, "Gate" + (MaxIDGate + 1).ToString()));
                }
                else//当前对象如果被修改成事件类型 
                {
                    General.InvokeHandler(GlobalEvent.PropertyEdited, new Tuple<string, DrawData, string>("Identifier", SeData, "Event" + (MaxIDEvent + 1).ToString()));
                }
                General.IsIgnoreTreeListFocusNodeChangeEvent = false;
            }

            //改变选择项
            bool ck = false;
            foreach (BarItemLink item in General.barSubItem_ChangeGateType.ItemLinks)
            {
                if (((DevExpress.XtraBars.BarButtonItemLink)item).Item.Down)
                {
                    ck = true;
                }
            }
            if (ck == false)
            {
                ((DevExpress.XtraBars.BarBaseButtonItem)e.Item).Down = true;
            }
        }

        /// <summary>
        /// 另存为FaultTree
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem_SaveAsFaultTree_ItemClick(object sender, ItemClickEventArgs e)
        {
            General.TryCatch(() =>
            {
                if (General.FtaProgram != null && General.FtaProgram.CurrentSystem != null)
                {
                    SaveFileDialog dialog = new SaveFileDialog();

                    if (General.FtaProgram.Setting.DefaultFilePath != "" && Directory.Exists(General.FtaProgram.Setting.DefaultFilePath))
                    {
                        dialog.InitialDirectory = General.FtaProgram.Setting.DefaultFilePath;
                    }

                    dialog.Filter = FixedString.FaultTree_FILTER;
                    DialogResult result = dialog.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        SplashScreenManager.ShowDefaultWaitForm(General.FtaProgram.String.SavingFaultTree, " ");

                        FileInfo filesys = new FileInfo(dialog.FileName);
                        File.WriteAllText(filesys.FullName, Newtonsoft.Json.JsonConvert.SerializeObject(General.FtaProgram.CurrentSystem), Encoding.UTF8);

                        if (SplashScreenManager.Default != null) SplashScreenManager.CloseDefaultWaitForm();
                        MsgBox.Show(General.FtaProgram.String.Successfully_Saved);

                        //系统模型虚拟化应用
                        General.FtaProgram.CurrentSystem.ApplyVirtualization();
                    }
                }
                else
                {
                    MsgBox.Show(General.FtaProgram.String.NoSaved);
                    return;
                }
            });
        }

        /// <summary>
        /// 另存为Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem_SaveAsExcel_ItemClick(object sender, ItemClickEventArgs e)
        {
            General.TryCatch(() =>
            {
                if (General.FtaProgram != null && General.FtaProgram.CurrentSystem != null)
                {
                    string filePath = this.GetFileName(DialogType.SaveFile, $"{General.FtaProgram.CurrentSystem.SystemName}_{DateTime.Now.ToString(FixedString.DATETIME_FORMAT)}");
                    if (string.IsNullOrEmpty(filePath) == false)
                    {
                        SplashScreenManager.ShowDefaultWaitForm();
                        this.WriteDrawDatasToExcelFile(filePath, General.FtaProgram.CurrentSystem.GetAllDatas());
                        if (SplashScreenManager.Default != null) SplashScreenManager.CloseDefaultSplashScreen();
                        MsgBox.Show(General.FtaProgram.String.Successfully_Saved);

                        //系统模型虚拟化应用
                        General.FtaProgram.CurrentSystem.ApplyVirtualization();
                    }
                }
                else
                {
                    MsgBox.Show(General.FtaProgram.String.NoSaved);
                    return;
                }
            });
        }

        /// <summary>
        /// 界面所有DEV控件ToolTip提示绑定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolTipController_Descriptions_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.SelectedControl == null)
                {
                    return;
                }

                if (e.SelectedControl == ribbonControl_FTA)//<<<<<<<<<<ribbon菜单>>>>>>>>>>
                {
                    var item = ribbonControl_FTA.CalcHitInfo(e.ControlMousePosition);
                    if (item != null && item.Item != null && item.Item.Item != null)
                    {
                        string TipContent = "";

                        if (item.Item.Item == barButtonItem_ShowFullScreen)//<<<<<<<<<<全屏>>>>>>>>>>
                        {
                            TipContent = General.FtaProgram.String.TipDescription_FullScreen;
                        }
                        else if (item.Item.Item == ribbonGalleryBarItem_GraphicTool)//工具箱
                        {
                            TipContent = "";
                        }
                        else if (item.Item.Item == barButtonItem_InsertLevel)
                        {
                            TipContent = General.FtaProgram.String.TipDescription_InsertLevel;
                        }
                        else if (item.Item.Item == barSubItem_ChangeGateType)
                        {
                            TipContent = General.FtaProgram.String.TipDescription_ChangeGateType;
                        }
                        else if (item.Item.Item == barButtonItem_TopGatePos)
                        {
                            TipContent = General.FtaProgram.String.TipDescription_TopGatePos;
                        }
                        else if (item.Item.Item == barButtonItem_Cancel)
                        {
                            TipContent = General.FtaProgram.String.TipDescription_Undo;
                        }
                        else if (item.Item.Item == barButtonItem_Redo)
                        {
                            TipContent = General.FtaProgram.String.TipDescription_Redo;
                        }
                        else if (item.Item.Item == barButtonItem_MenuCut)
                        {
                            TipContent = General.FtaProgram.String.TipDescription_Cut;
                        }
                        else if (item.Item.Item == barSubItem_Copy)
                        {
                            TipContent = General.FtaProgram.String.TipDescription_Copy;
                        }
                        else if (item.Item.Item == barSubItem_Paste)
                        {
                            TipContent = General.FtaProgram.String.TipDescription_Paste;
                        }
                        else if (item.Item.Item == barButtonItem_ExportToModel)
                        {
                            TipContent = General.FtaProgram.String.TipDescription_SaveCommonEventLibrary;
                        }
                        else if (item.Item.Item == barButtonItem_LoadFromModel)
                        {
                            TipContent = General.FtaProgram.String.TipDescription_CommonEventLibrary;
                        }
                        else if (item.Item.Item == barButtonItem_Check)
                        {
                            TipContent = General.FtaProgram.String.TipDescription_Check;
                        }
                        else if (item.Item.Item == barButtonItem_FTACalculate)
                        {
                            TipContent = General.FtaProgram.String.TipDescription_Calculate;
                        }
                        else if (item.Item.Item == Bbi_Import)
                        {
                            TipContent = General.FtaProgram.String.TipDescription_Import;
                        }
                        else if (item.Item.Item == Bbi_Export)
                        {
                            TipContent = General.FtaProgram.String.TipDescription_Export;
                        }
                        else if (item.Item.Item == barButtonItem_FTA)
                        {
                            TipContent = General.FtaProgram.String.TipDescription_FTAReport;
                        }
                        else if (item.Item.Item == barButtonItem_Cutset)
                        {
                            TipContent = General.FtaProgram.String.TipDescription_CutsetReport;
                        }
                        else if (item.Item.Item == barButtonItem_StartPage)
                        {
                            TipContent = General.FtaProgram.String.TipDescription_StartPage;
                        }
                        else if (item.Item.Item == barSubItem_Windows)
                        {
                            TipContent = General.FtaProgram.String.TipDescription_Windows;
                        }
                        else if (item.Item.Item == barButtonItem_ZoomIn)
                        {
                            TipContent = General.FtaProgram.String.TipDescription_ZoomIn;
                        }
                        else if (item.Item.Item == barButtonItem_ZoomOut)
                        {
                            TipContent = General.FtaProgram.String.TipDescription_ZoomOut;
                        }
                        else if (item.Item.Item == barButtonItem_FoldProject)
                        {
                            TipContent = General.FtaProgram.String.TipDescription_FoldProject;
                        }
                        else if (item.Item.Item == barButtonItem_ExpandProject)
                        {
                            TipContent = General.FtaProgram.String.TipDescription_ExpandProject;
                        }
                        else if (item.Item.Item == barButtonItem_Properties)
                        {
                            TipContent = General.FtaProgram.String.TipDescription_Properties;
                        }
                        else if (item.Item.Item == barButtonItem_UserManual)
                        {
                            TipContent = General.FtaProgram.String.TipDescription_UserManual;
                        }
                        else if (item.Item.Item == barButtonItem_ShowKeymap)
                        {
                            TipContent = General.FtaProgram.String.TipDescription_Keymap;
                        }
                        else if (item.Item.Item == barButtonItem_CheckforUpdates)
                        {
                            TipContent = General.FtaProgram.String.TipDescription_Updates;
                        }
                        else if (item.Item.Item == barButtonItem_AboutSmarTree)
                        {
                            TipContent = General.FtaProgram.String.TipDescription_AboutSmarTree;
                        }
                        else//<<<<<<<<<<需要扩展其它Ribbon菜单在这里添加>>>>>>>>>>
                        {
                            TipContent = item.Item.Item.Caption;
                        }
                        if (TipContent != "")
                        {
                            DevExpress.Utils.SuperToolTip superToolTip = new DevExpress.Utils.SuperToolTip();
                            DevExpress.Utils.ToolTipItem toolTipItem = new DevExpress.Utils.ToolTipItem();
                            toolTipItem.Text = TipContent;
                            toolTipItem.Font = item.Item.Item.Font;
                            toolTipItem.Appearance.TextOptions.VAlignment = VertAlignment.Center;
                            superToolTip.Items.Add(toolTipItem);
                            e.Info = new ToolTipControlInfo(item, "");
                            e.Info.SuperTip = superToolTip;
                        }
                    }
                }
                else if (e.SelectedControl == currentDiagram)//<<<<<<<<<<FTA图形控件>>>>>>>>>>
                {
                    var item = currentDiagram.CalcHitItem(e.ControlMousePosition);
                    if (item != null && item.Tag != null)
                    {
                        string TipContent = ((FaultTreeAnalysis.DrawData)item.Tag).Comment1;

                        Graphics g = currentDiagram.CreateGraphics();
                        SizeF StrSize = g.MeasureString(TipContent, item.Appearance.Font);
                        //字符面积
                        float Measure = StrSize.Width * StrSize.Height + 200;//200为图形与文字大概间隙
                        //图形内容框面积
                        float MeasureShape = 100 * 50;
                        if (General.DiagramItemPool != null && General.DiagramItemPool.Style != null)
                        {
                            MeasureShape = General.DiagramItemPool.Style.ShapeWidth * General.DiagramItemPool.Style.ShapeDescriptionRectHeight;
                        }

                        if (Measure >= MeasureShape)
                        {
                            DevExpress.Utils.SuperToolTip superToolTip = new DevExpress.Utils.SuperToolTip();
                            DevExpress.Utils.ToolTipItem toolTipItem = new DevExpress.Utils.ToolTipItem();
                            toolTipItem.Text = TipContent;
                            toolTipItem.Font = item.Appearance.Font;
                            toolTipItem.Appearance.TextOptions.VAlignment = VertAlignment.Center;
                            superToolTip.Items.Add(toolTipItem);
                            e.Info = new ToolTipControlInfo(item, "");
                            e.Info.SuperTip = superToolTip;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 定位上一级（包括转移门跳转）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem_PosParent_ItemClick(object sender, ItemClickEventArgs e)
        {
            General.TryCatch(() =>
            {
                if (ftaDiagram.SelectedData.Count == 1)
                {
                    DrawData selectedData = ftaDiagram.SelectedData.FirstOrDefault();
                    General.FTATree.FocusedNode = null;

                    //先追踪当前选中的节点
                    Predicate<TreeListNode> match = new Predicate<TreeListNode>(d => (d.GetValue(FixedString.COLUMNAME_DATA) != null && d.GetValue(FixedString.COLUMNAME_DATA) == selectedData));
                    TreeListNode nd = General.FTATree.FindNode(match);

                    if (nd != null && nd.ParentNode != null)
                    {
                        //有父节点直接追踪
                        General.FTATree.FocusedNode = nd.ParentNode;
                    }
                    else if (nd != null && nd.ParentNode == null)
                    {
                        //没有父节点的如果有转移门对象，直接追踪到第一个转移门对象
                        if (General.FtaProgram.CurrentSystem.TranferGates.ContainsKey(selectedData.Identifier))
                        {
                            Predicate<TreeListNode> match1 = new Predicate<TreeListNode>(d => (d.GetValue(FixedString.COLUMNAME_DATA) != null && d.GetValue("Identifier") != null && d.GetValue("Identifier").ToString() == selectedData.Identifier) && ((DrawData)d.GetValue(FixedString.COLUMNAME_DATA)).Type == DrawType.TransferInGate);
                            TreeListNode nd1 = General.FTATree.FindNode(match1);
                            General.FTATree.FocusedNode = nd1;
                        }
                        else
                        {
                            //没父节点又没转移门的不追踪
                            Predicate<TreeListNode> match1 = new Predicate<TreeListNode>(d => (d.GetValue(FixedString.COLUMNAME_DATA) != null && d.GetValue(FixedString.COLUMNAME_DATA) == selectedData));
                            TreeListNode nd1 = General.FTATree.FindNode(match1);
                            General.FTATree.FocusedNode = nd1;
                        }
                    }
                    else
                    {
                        //未追踪到
                        Predicate<TreeListNode> match1 = new Predicate<TreeListNode>(d => (d.GetValue(FixedString.COLUMNAME_DATA) != null && d.GetValue(FixedString.COLUMNAME_DATA) == selectedData));
                        TreeListNode nd1 = General.FTATree.FindNode(match1);
                        General.FTATree.FocusedNode = nd1;
                    }
                }
            });
        }

        /// <summary>
        /// 定位下一级（包括转移门跳转）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem_PosChild_ItemClick(object sender, ItemClickEventArgs e)
        {
            General.TryCatch(() =>
            {
                if (ftaDiagram.SelectedData.Count == 1)
                {
                    DrawData selectedData = ftaDiagram.SelectedData.FirstOrDefault();
                    General.FTATree.FocusedNode = null;

                    //先追踪当前选中的节点
                    Predicate<TreeListNode> match = new Predicate<TreeListNode>(d => (d.GetValue(FixedString.COLUMNAME_DATA) != null && d.GetValue(FixedString.COLUMNAME_DATA) == selectedData));
                    TreeListNode nd = General.FTATree.FindNode(match);

                    if (nd != null && nd.HasChildren)
                    {
                        nd.ExpandAll();
                        //有子节点直接追踪第一个
                        General.FTATree.FocusedNode = nd.Nodes[0];
                    }
                    else if (nd != null && !nd.HasChildren)
                    {
                        //没有子节点的如果有转移门对象，直接追踪到转移门对象
                        if (General.FtaProgram.CurrentSystem.TranferGates.ContainsKey(selectedData.Identifier))
                        {
                            Predicate<TreeListNode> match1 = new Predicate<TreeListNode>(d => (d.GetValue(FixedString.COLUMNAME_DATA) != null && d.GetValue("Identifier") != null && d.GetValue("Identifier").ToString() == selectedData.Identifier) && ((DrawData)d.GetValue(FixedString.COLUMNAME_DATA)).Type != DrawType.TransferInGate);
                            TreeListNode nd1 = General.FTATree.FindNode(match1);
                            General.FTATree.FocusedNode = nd1;
                        }
                        else
                        {
                            //没子节点又没转移门的不追踪
                            Predicate<TreeListNode> match1 = new Predicate<TreeListNode>(d => (d.GetValue(FixedString.COLUMNAME_DATA) != null && d.GetValue(FixedString.COLUMNAME_DATA) == selectedData));
                            TreeListNode nd1 = General.FTATree.FindNode(match1);
                            General.FTATree.FocusedNode = nd1;
                        }
                    }
                    else
                    {
                        //未追踪到
                        Predicate<TreeListNode> match1 = new Predicate<TreeListNode>(d => (d.GetValue(FixedString.COLUMNAME_DATA) != null && d.GetValue(FixedString.COLUMNAME_DATA) == selectedData));
                        TreeListNode nd1 = General.FTATree.FindNode(match1);
                        General.FTATree.FocusedNode = nd1;
                    }
                }
            });
        }


        /// <summary>
        /// 定位左右侧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PosLeftOrRight(bool isLeft)
        {
            General.TryCatch(() =>
            {
                if (ftaDiagram.SelectedData.Count == 1)
                {
                    DrawData selectedData = ftaDiagram.SelectedData.FirstOrDefault();
                    General.FTATree.FocusedNode = null;

                    //先追踪当前选中的节点
                    Predicate<TreeListNode> match = new Predicate<TreeListNode>(d => (d.GetValue(FixedString.COLUMNAME_DATA) != null && d.GetValue(FixedString.COLUMNAME_DATA) == selectedData));
                    TreeListNode nd = General.FTATree.FindNode(match);

                    nd.Collapse();

                    if (!isLeft)
                    {
                        if (nd != null && nd.NextNode != null)
                        {
                            //追踪下一个节点
                            General.FTATree.FocusedNode = nd.NextNode;
                        }
                        else
                        {
                            General.FTATree.FocusedNode = nd;
                        }
                    }
                    else
                    {
                        if (nd != null && nd.PrevNode != null)
                        {
                            //追踪上一个节点
                            General.FTATree.FocusedNode = nd.PrevNode;
                        }
                        else
                        {
                            General.FTATree.FocusedNode = nd;
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 将当前选中对象（包括其子项）导出到模型库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem_ExportToModel_ItemClick(object sender, ItemClickEventArgs e)
        {
            General.TryCatch(() =>
            {
                if (ftaDiagram.SelectedData.Count > 0)
                {
                    DrawData selectedData = ftaDiagram.SelectedData.FirstOrDefault();

                    if (Directory.Exists(Environment.CurrentDirectory + "\\Event Library") == false)
                    {
                        Directory.CreateDirectory(Environment.CurrentDirectory + "\\Event Library");
                    }
                    var dialog = new SaveFileDialog { Filter = FixedString.MODEL_FILTER, FileName = $"{selectedData.Identifier}_{DateTime.Now.ToString("yyyyMMddhhmm")}" };
                    dialog.InitialDirectory = Environment.CurrentDirectory + "\\Event Library";
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        FileInfo filesys = new FileInfo(dialog.FileName);

                        DrawData da = selectedData.CopyDrawDataRecurse();

                        ReplaceTrans(da, General.FtaProgram.CurrentSystem.TranferGates);

                        File.WriteAllText(filesys.FullName, Newtonsoft.Json.JsonConvert.SerializeObject(da), Encoding.UTF8);
                    }
                }
                else
                {
                    MsgBox.Show(General.FtaProgram.String.AtLeastSelectOneNode);
                }
            });
        }

        /// <summary>
        /// 从模型库导入对象到当前选中项的子节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem_LoadFromModel_ItemClick(object sender, ItemClickEventArgs e)
        {
            General.TryCatch(() =>
            {
                if (ftaDiagram.SelectedData.Count > 0)
                {
                    DrawData selectedData = ftaDiagram.SelectedData.FirstOrDefault();

                    if (selectedData.IsGateType && selectedData.Type != DrawType.TransferInGate)
                    {
                        if (Directory.Exists(Environment.CurrentDirectory + "\\Event Library") == false)
                        {
                            Directory.CreateDirectory(Environment.CurrentDirectory + "\\Event Library");
                        }
                        var dialog = new OpenFileDialog { Filter = FixedString.MODEL_FILTER };
                        dialog.InitialDirectory = Environment.CurrentDirectory + "\\Event Library";

                        if (General.FtaProgram.Setting.CommonEventLibraryPath != "" && Directory.Exists(General.FtaProgram.Setting.CommonEventLibraryPath))
                        {
                            dialog.InitialDirectory = General.FtaProgram.Setting.CommonEventLibraryPath;
                        }

                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            //读取模型库文件（DrawData对象JSON）
                            DrawData NewData = Newtonsoft.Json.JsonConvert.DeserializeObject<DrawData>(File.ReadAllText(dialog.FileName, Encoding.UTF8));

                            if (NewData == null) return;

                            //绑定父子关系（Parent属性为JsonIgnore不会被保存）
                            SetParent(NewData);

                            General.CopyCutObject = NewData;
                            this.ftaTable.TableEvents.IsCopyNode = true;
                            General.FTATableDiagram_Is_CopyOrCut_Recurse = true;

                            //重复ID的重新命名一次（不迭代）
                            List<string> listAll = General.FtaProgram.CurrentSystem.GetAllIDs().ToList();
                            List<DrawData> list = General.CopyCutObject.GetAllData(General.CopyCutSystem);

                            foreach (DrawData st in list)
                            {
                                SetIDs(st, listAll);
                            }

                            //粘贴（不重命名事件）
                            DrawData data = FTATableDiagram_PasteNew(selectedData);

                            //粘贴后标记重复事件
                            List<DrawData> listNew = data.GetAllData(General.FtaProgram.CurrentSystem);
                            var lsm = listNew.GroupBy(x => x.Identifier).Where(x => x.Count() > 1).ToList();

                            foreach (var da in lsm)
                            {
                                foreach (DrawData dd in listNew)
                                {
                                    if (dd.Identifier == da.Key)
                                    {
                                        General.FtaProgram.CurrentSystem.AddRepeatedEvent(dd);
                                    }
                                }
                            }

                            if (data != null)
                            {
                                this.ftaDiagram.FocusOn(selectedData);
                                this.ftaTable.FocusOn(data);
                            }

                            General.CopyCutObject = null;
                        }
                    }
                    else
                    {
                        General.CopyCutObject = null;
                        MsgBox.Show(General.FtaProgram.String.AtLeastSelectOneNode);
                    }
                }
                else
                {
                    General.CopyCutObject = null;
                    MsgBox.Show(General.FtaProgram.String.AtLeastSelectOneNode);
                }
            });
        }

        /// <summary>
        /// 导入前先绑定父子关系（导出时父子关系是JsonIgnore）
        /// </summary>
        /// <param name="ParentData"></param>
        public void SetParent(DrawData ParentData)
        {
            foreach (DrawData da in ParentData.Children)
            {
                da.Parent = ParentData;
                if (da.HasChild)
                {
                    SetParent(da);
                }
            }
        }

        /// <summary>
        /// 导入前先重命名ID
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="listAll"></param>
        public void SetIDs(DrawData Data, List<string> listAll)
        {
            if (listAll.Contains(Data.Identifier))
            {
                Data.Identifier = Data.Identifier + "_1";
                SetIDs(Data, listAll);
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// 菜单栏编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShapeEdit(DrawData selectedData)
        {
            if (this.ftaDiagram.DiagramEvents.IsInsertingNode == false)
            {
                if (ftaDiagram.SelectedData.Count > 0)
                {
                    if (selectedData.Type != DrawType.NULL)
                    {
                        //设置该元素选中
                        General.DiagramItemPool.SelectedData.Clear();
                        General.DiagramItemPool.SelectedData.Add(selectedData);

                        bool isTransfer = false;//转移门禁止编辑类型
                        if (selectedData.Type == DrawType.TransferInGate)
                        {
                            isTransfer = true;
                            selectedData = General.FtaProgram.CurrentSystem.TranferGates[selectedData.Identifier].FirstOrDefault(o => o.Type != DrawType.TransferInGate);
                        }
                        var editPropertyView = new EditPropertyView(General.FtaProgram, selectedData, isTransfer);
                        editPropertyView.ShowDialog();

                        if (editPropertyView.Result.Length > 0)
                        {
                            try
                            {
                                General.IsIgnoreTreeListFocusNodeChangeEvent = true;
                                General.InvokeHandler(GlobalEvent.PropertiesEdited, new Tuple<DrawData, string[]>(selectedData, editPropertyView.Result));
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

        /// <summary>
        /// Dock页右键菜单弹出中英文切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dockManager_FTA_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.Menu.Items.Count == 5)
            {
                e.Menu.Items[0].Caption = General.FtaProgram.String.DockMenuFolat;
                e.Menu.Items[1].Caption = General.FtaProgram.String.DockMenuDock;
                e.Menu.Items[2].Caption = General.FtaProgram.String.DockMenuTabbed;
                e.Menu.Items[3].Caption = General.FtaProgram.String.DockMenuHide;
                e.Menu.Items[4].Caption = General.FtaProgram.String.DockMenuClose;
            }
            else if (e.Menu.Items.Count == 4)
            {
                e.Menu.Items[0].Caption = General.FtaProgram.String.DockMenuFolat;
                e.Menu.Items[1].Caption = General.FtaProgram.String.DockMenuDock;
                e.Menu.Items[2].Caption = General.FtaProgram.String.DockMenuTabbed;
                e.Menu.Items[3].Caption = General.FtaProgram.String.DockMenuHide;
            }
            else//其它情况无右键
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// DockAsDocument时右键菜单弹出中英文切换(右键Window功能禁用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabbedView_FTA_PopupMenuShowing(object sender, DevExpress.XtraBars.Docking2010.Views.PopupMenuShowingEventArgs e)
        {
            if (e.Menu.Items.Count == 2)
            {
                e.Menu.Items[0].Caption = General.FtaProgram.String.DockMenuClose;
                e.Menu.Items[1].Caption = General.FtaProgram.String.DockMenuFolat;
            }
            else//一个菜单右键Window功能禁用
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// 鼠标右键Ribbon自带右键菜单取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ribbonControl_FTA_ShowCustomizationMenu(object sender, RibbonCustomizationMenuEventArgs e)
        {
            e.ShowCustomizationMenu = false;
        }

        /// <summary>
        /// 最大割集深度设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem5_ItemClick(object sender, ItemClickEventArgs e)
        {
            XtraFormCutsets xtraFormCutsets = new XtraFormCutsets();
            xtraFormCutsets.ShowDialog();
        }

        /// <summary>
        /// 基本事件库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem_BasicLib_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frm_NetSettings f = new Frm_NetSettings();
            f.ShowDialog();

            try
            {
                List<DrawData> drawDatas = General.FtaProgram.CurrentSystem.GetAllDatas();
                //3.同步数据到当前事件，同时刷新所有重复事件 
                if (General.EventsLibDB == null || General.EventsLibDB.Rows.Count == 0)
                {
                    return;
                }

                foreach (DrawData da in drawDatas)
                {
                    if (da.GUID != null && da.GUID != "")
                    {
                        DataRow checkDB = General.EventsLibDB.Select("GUID='" + da.GUID + "'").FirstOrDefault();
                        if (checkDB != null)
                        {
                            var typeName = General.GetKeyName(checkDB["Type"].ToString());
                            var type = (DrawType)Enum.Parse(typeof(DrawType), typeName);
                            da.GUID = checkDB["GUID"].ToString();
                            da.Identifier = checkDB["Identifier"].ToString();
                            da.Type = type;
                            da.Comment1 = checkDB["Description"].ToString();
                            da.LogicalCondition = checkDB["LogicalCondition"].ToString();
                            da.InputType = checkDB["InputType"].ToString();
                            da.InputValue = checkDB["InputValue"].ToString();
                            da.InputValue2 = checkDB["InputValue2"].ToString();
                            da.ExtraValue1 = checkDB["ExtraValue1"].ToString();
                            da.Units = checkDB["Units"].ToString();
                        }
                    }
                }
                UpdateLayout();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 添加到基本事件库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem_AddToBasicEventLibrary_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                //将当前选中的基本事件上传至基本事件库 
                //1.如果没有GUID，自动生成GUID，同时如果存在重复事件，要同步重复事件的GUID
                List<DrawData> selectedDrawData = ftaDiagram.GetSelectedDataList();
                DrawData ActData = selectedDrawData[0];
                if (ActData.GUID == "")
                {
                    ActData.GUID = Guid.NewGuid().ToString();
                    if (General.FtaProgram.CurrentSystem.RepeatedEvents.ContainsKey(ActData.Identifier))
                    {
                        foreach (DrawData da in General.FtaProgram.CurrentSystem.RepeatedEvents[ActData.Identifier])
                        {
                            da.GUID = ActData.GUID;
                        }
                    }
                }

                if (ActData.Group == null || ActData.Group == "")
                {
                    XtraFormNewGroup f = new XtraFormNewGroup();

                    if (f.ShowDialog() == DialogResult.OK)
                    {
                        ActData.Group = f.GrouName;
                    }
                    else
                    {
                        return;
                    }
                }

                //2.如果基本事件库里面已经存在这个GUID项，提示是否更新数据到基本事件库 
                if (ConnectSever.CheckExist(ActData.GUID))
                {
                    DialogResult rsC = MsgBox.Show("基本事件库已存在该基本事件，是否更新数据到基本事件库?", "", System.Windows.Forms.MessageBoxButtons.YesNo);
                    if (rsC == DialogResult.No)
                    {
                        return;
                    }
                    ConnectSever.UpdateOne(new object[] { ActData.GUID, ActData.Group, ActData.Identifier, ActData.Type.ToString(), ActData.Comment1, ActData.LogicalCondition, ActData.InputType, ActData.InputValue, ActData.InputValue2, ActData.ExtraValue1, ActData.Units });
                }
                else
                {
                    ConnectSever.InsertOne(new object[] { ActData.GUID, ActData.Group, ActData.Identifier, ActData.Type.ToString(), ActData.Comment1, ActData.LogicalCondition, ActData.InputType, ActData.InputValue, ActData.InputValue2, ActData.ExtraValue1, ActData.Units });
                }

                //基本事件库
                if (File.Exists(System.Environment.CurrentDirectory + "\\BasicEvents.db"))
                {
                    //开启接收线程，读取数据
                    General.EventsLibDB = ConnectSever.ReadInitial("BasicEvents", " ORDER BY ID");
                    General.EventsLibDB.TableName = "BasicEvents";
                }
                SaveData();
                MsgBox.Show("添加成功！");
            }
            catch (Exception ex)
            {
                MsgBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 从基本事件库同步数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem_SynchronizeFromBasicEventLibrary_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                DialogResult rs = MsgBox.Show("此操作可能不可撤销,确定执行操作?", "", System.Windows.Forms.MessageBoxButtons.YesNo);
                if (rs == DialogResult.No)
                {
                    return;
                }
                //从基本事件库同步数据 
                List<DrawData> selectedDrawData = ftaDiagram.GetSelectedDataList();
                DrawData ActData = selectedDrawData[0];
                //1.先查询有没有GUID，没有，提示错误,再确认基本事件库有没有当前GUID，没有，提示错误
                if (ActData.GUID == "" || !ConnectSever.CheckExist(ActData.GUID))
                {
                    MsgBox.Show("当前GUID在基本事件库不存在：" + ActData.GUID);
                    return;
                }

                //3.同步数据到当前事件，同时刷新所有重复事件
                DataTable checkDB = ConnectSever.SelectOne(ActData.GUID);
                if (checkDB == null || checkDB.Rows.Count == 0)
                {
                    return;
                }

                var typeName = General.GetKeyName(checkDB.Rows[0]["Type"].ToString());
                var type = (DrawType)Enum.Parse(typeof(DrawType), typeName);
                ActData.GUID = checkDB.Rows[0]["GUID"].ToString();
                ActData.Type = type;
                ActData.Comment1 = checkDB.Rows[0]["Description"].ToString();
                ActData.LogicalCondition = checkDB.Rows[0]["LogicalCondition"].ToString();
                ActData.InputType = checkDB.Rows[0]["InputType"].ToString();
                ActData.InputValue = checkDB.Rows[0]["InputValue"].ToString();
                ActData.InputValue2 = checkDB.Rows[0]["InputValue2"].ToString();
                ActData.ExtraValue1 = checkDB.Rows[0]["ExtraValue1"].ToString();
                ActData.Units = checkDB.Rows[0]["Units"].ToString();

                //重复事件刷新
                if (ActData.GUID != "")
                {
                    if (General.FtaProgram.CurrentSystem.RepeatedEvents.ContainsKey(ActData.Identifier))
                    {
                        foreach (DrawData da in General.FtaProgram.CurrentSystem.RepeatedEvents[ActData.Identifier])
                        {
                            da.GUID = ActData.GUID;
                            da.Type = ActData.Type;
                            da.Comment1 = ActData.Comment1;
                            da.LogicalCondition = ActData.LogicalCondition;
                            da.InputType = ActData.InputType;
                            da.InputValue = ActData.InputValue;
                            da.InputValue2 = ActData.InputValue2;
                            da.ExtraValue1 = ActData.ExtraValue1;
                            da.Units = ActData.Units;
                        }
                    }
                }
                UpdateLayout();

                SaveData();
                MsgBox.Show("更新成功！");
            }
            catch (Exception ex)
            {
                MsgBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 从基本事件库添加事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem_InsertFromBasicEventLibrary_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                DialogResult rs = MsgBox.Show("此操作可能不可撤销,确定执行操作?", "", System.Windows.Forms.MessageBoxButtons.YesNo);
                if (rs == DialogResult.No)
                {
                    return;
                }

                List<DrawData> selectedDrawData = ftaDiagram.GetSelectedDataList();
                DrawData ActData = selectedDrawData[0];

                Frm_NetSettings_Select f = new Frm_NetSettings_Select();
                f.ShowDialog();

                if (f.InsertDatas != null && f.InsertDatas.Count > 0)
                {
                    foreach (DrawData da in f.InsertDatas)
                    {
                        //从基本事件库添加事件
                        DrawData item = new DrawData();
                        item.Type = da.Type;
                        HashSet<string> ids = General.FtaProgram.CurrentSystem.GetAllIDs();
                        item.Identifier = item.GetNewID(ids, item.Type);
                        item.Parent = ActData;
                        if (!item.IsGateType)
                        {
                            item.GUID = da.GUID;
                            item.Type = da.Type;
                            item.Comment1 = da.Comment1;
                            item.LogicalCondition = da.LogicalCondition;
                            item.InputType = da.InputType;
                            item.InputValue = da.InputValue;
                            item.InputValue2 = da.InputValue2;
                            item.ExtraValue1 = da.ExtraValue1;
                            item.Units = da.Units;

                            //如果存在同GUID的节点，需要刷新当前事件ID和重复事件数量
                            List<DrawData> drawDatas = General.FtaProgram.CurrentSystem.GetAllDatas();
                            IEnumerable<DrawData> Edatas = drawDatas.Where(d => d.GUID == item.GUID);
                            if (Edatas.Count() > 0)
                            {
                                item.Identifier = Edatas.FirstOrDefault().Identifier;
                                General.FtaProgram.CurrentSystem.AddRepeatedEvent(item);

                                foreach (DrawData ds in Edatas)
                                {
                                    if (General.FtaProgram.CurrentSystem.RepeatedEvents[item.Identifier].Contains(ds) == false)
                                    {
                                        General.FtaProgram.CurrentSystem.AddRepeatedEvent(ds);
                                    }
                                    ds.GUID = da.GUID;
                                    ds.Type = da.Type;
                                    ds.Comment1 = da.Comment1;
                                    ds.LogicalCondition = da.LogicalCondition;
                                    ds.InputType = da.InputType;
                                    ds.InputValue = da.InputValue;
                                    ds.InputValue2 = da.InputValue2;
                                    ds.ExtraValue1 = da.ExtraValue1;
                                    ds.Units = da.Units;
                                }
                            }
                        }

                        ActData.Children.Add(item);
                        General.InvokeHandler(GlobalEvent.UpdateLayout);

                        //情况: 插入一个子级
                        if ((null != General.FtaProgram)
                            && (null != General.FtaProgram.CurrentSystem))
                        {
                            //做历史记录
                            General.FtaProgram.CurrentSystem.TakeBehavor(
                                item
                                , Behavior.Enum.ElementOperate.Creation
                                , Behavior.Enum.ElementOperate.Deletion
                                , ActData
                                , null);
                        }
                    }
                    UpdateLayout();

                    SaveData();
                }
            }
            catch (Exception ex)
            {
                MsgBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 插入链接门，该门可以链接到其他故障树节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem_AddLinkGate_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                //这里由于进来的item可能是复制的一个对象不是ribbonGalleryBarItem_GraphicTool里的 
                GalleryItem item_Tmp = ribbonGalleryBarItem_GraphicTool.Gallery.GetItemByValue("LinkGate");
                bool Is_checkedLast = item_Tmp.Checked;
                ReSetCheckState();
                item_Tmp.Checked = !Is_checkedLast;
                this.ftaDiagram.DiagramEvents.IsInsertingNode = item_Tmp.Checked;
                //滚动到选择的元素并显示
                ribbonGalleryBarItem_GraphicTool.Gallery.MakeVisible(item_Tmp);
                General.isLinkGateInsert = false;
                //设置当前选中的图形类型 
                if (item_Tmp.Value != null && item_Tmp.Value.GetType() == typeof(string))
                {
                    if (item_Tmp.Value.ToString() != "LinkGate")
                    {
                        var typeName = General.GetKeyName(item_Tmp.Value as string);
                        var type = (DrawType)Enum.Parse(typeof(DrawType), typeName);

                        if (type == DrawType.NULL) return;
                        if (item_Tmp.Checked) this.ftaDiagram.DiagramEvents.AddingType = type;
                        else this.ftaDiagram.DiagramEvents.AddingType = DrawType.NULL;
                    }
                    else
                    {
                        General.isLinkGateInsert = true;
                        if (item_Tmp.Checked) this.ftaDiagram.DiagramEvents.AddingType = DrawType.BasicEvent;
                        else this.ftaDiagram.DiagramEvents.AddingType = DrawType.NULL;
                    }
                }
            }
            catch (Exception ex)
            {
                MsgBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 更新链接门数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem_UpDateLinkGate_ItemClick(object sender, ItemClickEventArgs e)
        {
            General.TryCatch(() =>
            {
                if (General.ftaDiagram.SelectedData == null || General.ftaDiagram.SelectedData.Count() == 0)
                {
                    return;
                }
                DrawData drawData = General.ftaDiagram.SelectedData.FirstOrDefault();
                Predicate<TreeListNode> match = new Predicate<TreeListNode>(d => d.Tag != null && d.Tag.GetType() == typeof(SystemModel) && ((SystemModel)d.Tag).SystemName == drawData.LinkPath.Split(':')[1] && ((d.ParentNode != null && d.ParentNode.Tag != null && d.ParentNode.Tag.GetType() == typeof(ProjectModel) && ((ProjectModel)d.ParentNode.Tag).ProjectName == drawData.LinkPath.Split(':')[0]) || (d.ParentNode != null && d.ParentNode.ParentNode != null && d.ParentNode.ParentNode.Tag != null && d.ParentNode.ParentNode.Tag.GetType() == typeof(ProjectModel) && ((ProjectModel)d.ParentNode.ParentNode.Tag).ProjectName == drawData.LinkPath.Split(':')[0])));
                TreeListNode nd = General.ProjectControl.FindNode(match);

                if (nd != null && nd.ParentNode != null)
                {
                    if (nd.Tag != null)
                    {
                        SystemModel sys = nd.Tag as SystemModel;

                        drawData.GUID = "";
                        drawData.InputType = FixedString.MODEL_LAMBDA_TAU;
                        drawData.Units = FixedString.UNITS_HOURS;
                        drawData.Comment1 = sys.Roots[0].Comment1;

                        drawData.InputValue = sys.Roots[0].QValue;
                        drawData.InputValue2 = "1";

                        drawData.LogicalCondition = FixedString.LOGICAL_CONDITION_NORMAL;
                        drawData.QValue = sys.Roots[0].QValue;

                        //刷新
                        ftaDiagram.Refresh(true);
                    }
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
                }
            });
        }
    }
}
