using DevExpress.Diagram.Core;
using DevExpress.Utils;
using DevExpress.XtraDiagram;
using DevExpress.XtraEditors;
using FaultTreeAnalysis.Common;
using FaultTreeAnalysis.Model.Data;
using FaultTreeAnalysis.Model.Enum;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace FaultTreeAnalysis.View.Diagram
{
    /// <summary>
    /// 用于实现diagramcontrol动态显示DrawData数据
    /// </summary>
    public class DiagramItemPool : IDisposable
    {
        /// <summary>
        /// 缩放时会同时触发滚动条水平和垂直的变化，但这里只希望刷新一次，这个变量实现该功能
        /// </summary>
        int ScrollCount = 0;

        /// <summary>
        /// 绑定的画布控件对象
        /// </summary>
        private DiagramControl Diagram { get; set; }

        /// <summary>
        /// 全局使用的设置对象
        /// </summary>
        private SettingModel Setting { get; set; }

        /// <summary>
        /// 某个Project使用的设置对象
        /// </summary>
        public StyleModel Style { get; set; }

        /// <summary>
        /// 上一次刷新的视图矩形区域，滚动条的值变化事件会进入2次，某次的视图位置其实没有变化
        /// </summary>
        private RectangleF LastRect { get; set; }

        /// <summary>
        /// 上一次使用的图形集合
        /// </summary>
        private List<DiagramShape> LastUsed_Shape { get; set; }

        /// <summary>
        /// 上一次未使用的图形集合
        /// </summary>
        private List<DiagramShape> LastUnUsed_Shape { get; set; }

        /// <summary>
        /// 上一次使用的连线集合
        /// </summary>
        private List<DiagramConnector> LastUsed_Connector { get; set; }

        /// <summary>
        /// 上一次未使用的连线集合
        /// </summary>
        private List<DiagramConnector> LastUnUsed_Connector { get; set; }

        /// <summary>
        /// 要显示的数据集合,缩短每次查询要显示对象的时间
        /// </summary>
        private List<List<DrawData>> DatasAll { get; set; }

        /// <summary>
        /// 上一次ResetData函数调用时的根节点
        /// </summary>
        private DrawData Root { get; set; }

        /// <summary>
        /// 定义用于最近用户使用个数习惯，解决放大缩小后不会变卡
        /// </summary>
        private RecentUsedData RecentData { get; set; }

        #region 元素选择需要使用的变量
        /// <summary>
        /// 选中的元素集合,他会根据图形控件自动更新数据，当然用户可以自定义数据
        /// </summary>
        public HashSet<DrawData> SelectedData { get; private set; }

        /// <summary>
        /// 当用户按着control选择时(都在可视，一半，都不可视)，至少保证可见一个选择项，才能使用户继续选取。这个数据相当于标志，新选择的包含这个判断出是增加或减少
        /// </summary>
        private DrawData SelectedDataHide { get; set; }

        /// <summary>
        /// 上一次在可视区的选中的数据（刷新时重置，用户主动操作时也会增加减少集合数据）
        /// </summary>
        private HashSet<DrawData> SelectedDataShow { get; set; }

        /// <summary>
        /// 记录用户选择的图形区域贴着4个边的元素数据
        /// </summary>
        private SelectedDataStickToSide SelectedDataSide { get; set; }

        /// <summary>
        /// 防止刷新函数误触发的选择项变化事件，如果用户注册这个事件会受到影响
        /// </summary>
        private bool IsIgnoreSelectionChange { get; set; }
        #endregion

        /// <summary>
        /// 初始化注册图形控件动态刷新数据事件
        /// </summary>
        /// <param name="Diagram"></param>
        /// <param name="m_SimfiaAllData"></param>
        public DiagramItemPool(DiagramControl Diagram, SettingModel Setting)
        {
            if (Diagram != null && Setting != null)
            {
                LastRect = new RectangleF(0, 0, 0, 0);
                LastUsed_Shape = new List<DiagramShape>();
                LastUsed_Connector = new List<DiagramConnector>();
                LastUnUsed_Shape = new List<DiagramShape>();
                LastUnUsed_Connector = new List<DiagramConnector>();
                DatasAll = new List<List<DrawData>>();
                RecentData = new RecentUsedData();
                SelectedData = new HashSet<DrawData>();
                SelectedDataSide = new SelectedDataStickToSide();
                SelectedDataShow = new HashSet<DrawData>();
                this.Diagram = Diagram;
                this.Setting = Setting;
                HScrollBar hbar = Diagram.Controls[0] as HScrollBar;
                VScrollBar vbar = Diagram.Controls[1] as VScrollBar;
                vbar.ValueChanged += Hbar_ValueChanged;
                hbar.ValueChanged += Hbar_ValueChanged;
                Diagram.ZoomFactorChanged += DiagramControl_PreView_ZoomFactorChanged;
                Diagram.SelectionChanged += Diagram_SelectionChanged;
                Diagram.KeyDown += Diagram_KeyDown;
                Diagram.Commands.RegisterHotKeys(o =>
                {
                    o.ClearHotKeys(DiagramCommandsBase.SelectAllCommand);
                });

                ////对原有图形控件命令自定义化,全选图形,具有参考意义
                //Diagram.Commands.RegisterHandlers((ss) => {
                //    ss.RegisterHandlerCore(DiagramCommandsBase.SelectAllCommand,
                //    (a,b,c,d) => {
                //        //c执行后返回鼠标位置参数，这里用不到
                //        //d是原有操作
                //        SelectedData.Clear();
                //        foreach (var item in DatasAll)
                //        {
                //            SelectedData.AddRange(item);
                //        }
                //        RefreshDiagram(true);                       
                //    }, (a,b,c) => {
                //        //何时可用用它自带的
                //        return c();
                //    }
                //    ); });
            }
        }

        #region 注册处理图形控件事件，实现动态刷新，动态选择
        /// <summary>
        /// 全选操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Diagram_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            General.TryCatch(() =>
            {
                //全选
                if (e.Control && e.KeyCode == System.Windows.Forms.Keys.A)
                {
                    SelectedData.Clear();
                    foreach (var item in DatasAll)
                    {
                        foreach (var tmp in item)
                        {
                            if (SelectedDataSide.Left == null) SelectedDataSide.Left = tmp;
                            else if (tmp.X < SelectedDataSide.Left.X) SelectedDataSide.Left = tmp;
                            if (SelectedDataSide.Right == null) SelectedDataSide.Right = tmp;
                            else if (tmp.X > SelectedDataSide.Right.X) SelectedDataSide.Right = tmp;
                            if (SelectedDataSide.Up == null) SelectedDataSide.Up = tmp;
                            else if (tmp.Y < SelectedDataSide.Up.Y) SelectedDataSide.Up = tmp;
                            if (SelectedDataSide.Down == null) SelectedDataSide.Down = tmp;
                            else if (tmp.Y > SelectedDataSide.Down.Y) SelectedDataSide.Down = tmp;
                            SelectedData.Add(tmp);

                        }
                    }
                    RefreshDiagram(true);
                }
            });
        }

        /// <summary>
        /// 记录用户选择项,可能替换，可能增加可能减少
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Diagram_SelectionChanged(object sender, DiagramSelectionChangedEventArgs e)
        {
            General.TryCatch(() =>
            {
                if (!IsIgnoreSelectionChange)
                {
                    //筛选真实选中项
                    List<DrawData> seletedNew = new List<DrawData>();
                    List<DrawData> seleted = Diagram.SelectedItems.Where(obj => obj.Tag != null && obj.Tag is DrawData).Select(obj => obj.Tag as DrawData).ToList();
                    foreach (DrawData da in seleted)
                    {
                        foreach (DiagramItem item in Diagram.Items)
                        {
                            if (item.Tag != null && ((DrawData)item.Tag).ThisGuid == da.ThisGuid && ((DrawData)item.Tag).Children != null)
                            {
                                seletedNew.Add((DrawData)item.Tag);

                                if (seletedNew.Count == seleted.Count)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    //用户按着contorl键增加或减少的数据
                    if (seletedNew != null && SelectedDataHide != null && seletedNew.Contains(SelectedDataHide))
                    {
                        List<DrawData> data_Show = new List<DrawData>(SelectedDataShow);
                        //减少数据
                        foreach (var item in data_Show)
                        {
                            if (!seletedNew.Contains(item))
                            {
                                SelectedDataShow.Remove(item);
                                SelectedData.Remove(item);
                            }
                        }
                        //增加数据
                        foreach (var item in seletedNew)
                        {
                            if (!SelectedData.Contains(item))
                            {
                                SelectedDataShow.Add(item);
                                SelectedData.Add(item);
                            }
                        }
                    }
                    //重新选取
                    else
                    {
                        SelectedData.Clear();
                        SelectedDataShow.Clear();
                        SelectedDataHide = null;
                        SelectedDataSide = new SelectedDataStickToSide();
                        if (seletedNew != null)
                        {
                            foreach (var item in seletedNew)
                            {
                                SelectedDataShow.Add(item);
                                SelectedData.Add(item);
                            }
                        }
                    }

                    InitSelectedSideData();
                }
            });
        }

        /// <summary>
        /// 缩放时,设置一个标记值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DiagramControl_PreView_ZoomFactorChanged(object sender, DiagramZoomFactorChangedEventArgs e)
        {
            General.TryCatch(() =>
            {
                ScrollCount = 1;
            });
        }

        /// <summary>
        /// 滚动条值变化时，垂直或水平
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hbar_ValueChanged(object sender, EventArgs e)
        {
            General.TryCatch(() =>
            {
                if (ScrollCount >= 1) ScrollCount++;
                if (ScrollCount == 0 || ScrollCount == 3)
                {
                    RefreshDiagram();
                    ScrollCount = 0;
                }
            });
        }
        #endregion

        #region 根据设置对象，数据对象返回图形对象
        /// <summary>
        /// 根据给出的数据对象,按内置设置对象，构造一个图形对象
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private DiagramShape GenerateDiagramShape(DrawData data)
        {
            if (data != null && Style != null && Setting != null)
            {
                DiagramShape Shape = new DiagramShape(BasicShapes.Rectangle);
                Shape.Tag = data;
                Shape.X = data.X;
                Shape.Y = data.Y;
                Shape.ConnectionPoints = new PointCollection(new List<PointFloat>() { new PointFloat(0.5f, 0), new PointFloat(0.5f, 1) });
                //图形默认宽高
                int shapeWidth = Style.ShapeWidth;
                int shapeHeight = (Style.ShapeDescriptionRectHeight + Style.ShapeIdRectHeight + Style.ShapeSymbolRectHeight) + General.PEN_WIDTH;
                //int shapeHeight = (General.FtaProgram.CurrentSystem.ShapeHeight + Style.ShapeIdRectHeight + Style.ShapeSymbolRectHeight) + General.PEN_WIDTH;

                Shape.Size = new Size(shapeWidth, shapeHeight);
                Shape.CanEdit = false;
                Shape.CanDelete = false;
                Shape.CanCopy = false;
                Shape.CanMove = Setting.Is_MoveAble;
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
            if (parent_Shape != null && child_Shape != null && Setting != null)
            {
                //连线
                DiagramConnector connector = new DiagramConnector(parent_Shape, child_Shape);
                connector.BeginItemPointIndex = 1;
                connector.EndItemPointIndex = 0;
                connector.Appearance.BorderColor = Setting.LineColor;
                connector.EndArrowSize = new SizeF(0, 0);
                connector.CanDelete = false;
                connector.CanMove = Setting.Is_MoveAble;
                connector.CanChangeRoute = false;
                connector.CanEdit = false;
                connector.CanCopy = false;
                connector.CanResize = false;
                connector.CanRotate = false;
                connector.MinSize = new SizeF(0, 0);
                ConnectorType line = ConnectorType.RegisteredTypes.ToList().Where(obj => obj.TypeName == Setting.LineStyle).FirstOrDefault();
                ArrowDescription arrow = ArrowDescriptions.Arrows.ToList().Where(obj => obj.Name == Setting.ArrowStyle).FirstOrDefault();
                connector.Type = line;
                connector.EndArrow = arrow;
                connector.EndArrowSize = new SizeF(Setting.ArrowSize, Setting.ArrowSize);
                return connector;
            }
            return null;
        }
        #endregion

        /// <summary>
        /// 设置要要刷新的根节点对象，清除所有选择的数据，默认重新布局
        /// </summary>
        /// <param name="root">当前图形展示的根节点数据对象</param>
        /// <param name="style">和project绑定的图形设置对象</param>
        /// <param name="IsApplyTreeLayout">是否重新树布局</param>
        /// <param name="IsRefreshDiagram">是否设置完根节点后就刷新视图</param>
        public void ResetData(DrawData root, StyleModel style = null, bool IsApplyTreeLayout = true, bool IsRefreshDiagram = true)
        {
            if (root != null && root.Children == null)
            {
                return;
            }
            //当传入null时，清空操作
            this.DatasAll?.Clear();
            if (Diagram != null && Diagram.SelectedItems.Count > 0) Diagram.ClearSelection();

            //记录新布局对象
            if (style != null && Style != style) { Style = style; }

            //应用布局和初始化集合
            if (Diagram != null && Style != null && root != null)
            {
                if (IsApplyTreeLayout)
                {
                    int width = Style.ShapeWidth;
                    int height = (Style.ShapeDescriptionRectHeight + Style.ShapeIdRectHeight + Style.ShapeSymbolRectHeight) + General.PEN_WIDTH;
                    //int height = (General.FtaProgram.CurrentSystem.ShapeHeight + Style.ShapeIdRectHeight + Style.ShapeSymbolRectHeight) + General.PEN_WIDTH;

                    SizeF size = root.ApplyTreeLayout(width, height, width + Style.ShapeGap, height + Style.ShapeGap, Style.ShapeGap, Style.ShapeGap);
                    //Diagram.OptionsView.PaperKind = System.Drawing.Printing.PaperKind.Custom;
                    //先得到可视区域
                    float factor = Diagram.DiagramViewInfo.ZoomFactor;
                    float widthTmp = size.Width <= Diagram.DiagramViewInfo.ContentRect.Width / factor ?
                        Diagram.DiagramViewInfo.ContentRect.Width / factor : size.Width;
                    float heightTmp = size.Height <= Diagram.DiagramViewInfo.ContentRect.Height / factor ?
                        Diagram.DiagramViewInfo.ContentRect.Height / factor : size.Height;
                    Diagram.OptionsView.PageSize = new SizeF(widthTmp, heightTmp);
                    // Diagram.OptionsView.CanvasSizeMode = CanvasSizeMode.None;
                }

                List<DrawData> child = new List<DrawData>() { root };
                while (child.Count > 0)
                {
                    DatasAll.Add(child);
                    List<DrawData> child_TMP = new List<DrawData>();
                    foreach (var item in child)
                    {
                        if (item.Children != null)
                        {
                            child_TMP.AddRange(item.Children);
                        }
                    }
                    child = child_TMP;
                }
            }
            Root = root;
            if (IsRefreshDiagram) RefreshDiagram(true);
        }

        /// <summary>
        /// 根据之前的根节点重置数据集合，清除所有选择的数据,默认重新布局
        /// </summary>
        /// <param name="IsApplyTreeLayout">是否重新树布局</param>
        /// <param name="IsRefreshDiagram">是否设置完根节点后就刷新视图</param>
        public void ResetData(bool IsApplyTreeLayout = true, bool IsRefreshDiagram = true)
        {
            ResetData(Root, Style, IsApplyTreeLayout, IsRefreshDiagram);
        }

        /// <summary>
        /// 根据现有选中集合SelectedData，初始化4条边界元素集合
        /// </summary>
        private void InitSelectedSideData()
        {
            //确定边界元素
            SelectedDataSide.Left = null;
            SelectedDataSide.Up = null;
            SelectedDataSide.Right = null;
            SelectedDataSide.Down = null;
            if (SelectedData.Count > 0)
            {
                DrawData data = SelectedData.First();
                SelectedDataSide.Left = data;
                SelectedDataSide.Up = data;
                SelectedDataSide.Right = data;
                SelectedDataSide.Down = data;
                foreach (var item in SelectedData)
                {
                    if (item.X < SelectedDataSide.Left.X) SelectedDataSide.Left = item;
                    if (item.X > SelectedDataSide.Right.X) SelectedDataSide.Right = item;
                    if (item.Y < SelectedDataSide.Up.Y) SelectedDataSide.Up = item;
                    if (item.Y > SelectedDataSide.Down.Y) SelectedDataSide.Down = item;
                }
            }
        }

        /// <summary>
        /// 测试某个数据的位置是否在当前图形可视区内
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool IsInDiagramViewRect(DrawData data)
        {
            //图形默认宽高
            int shapeWidth = Style.ShapeWidth;
            int shapeHeight = (Style.ShapeDescriptionRectHeight + Style.ShapeIdRectHeight + Style.ShapeSymbolRectHeight) + General.PEN_WIDTH;
            //int shapeHeight = (General.FtaProgram.CurrentSystem.ShapeHeight + Style.ShapeIdRectHeight + Style.ShapeSymbolRectHeight) + General.PEN_WIDTH;

            //先得到可视区域
            PointF start = Diagram.DiagramViewInfo.RulersOffset;
            float factor = Diagram.DiagramViewInfo.ZoomFactor;
            float width = Diagram.DiagramViewInfo.ContentRect.Width;
            float height = Diagram.DiagramViewInfo.ContentRect.Height;
            //增加显示范围扩展一个元素，防止出现显示不全
            RectangleF rect = new RectangleF(-start.X / factor, -start.Y / factor, width / factor, height / factor);

            RectangleF rectTmp = new Rectangle(data.X, data.Y, shapeWidth, shapeHeight);
            if (rect.IntersectsWith(rectTmp))
            {
                return true;
            }
            else return false;
        }

        /// <summary>
        /// 根据当前保存的设置对象，重置当前图中所有图形的大小，线条箭头，可移动缩放等设置
        /// </summary>
        /// <param name="type">要设置的类型</param>
        public void ResetSetting(ResetType type)
        {
            if (Diagram != null)
            {
                //没有设置对象返回
                if ((type == ResetType.ShapeWidth || type == ResetType.ShapeHeight))
                {
                    if (Style == null) return;
                }
                else if (Setting == null) return;


                bool Is_NeedReLayout = false;
                switch (type)
                {
                    //宽高变化，会在刷新时，重新判定并设置
                    case ResetType.ShapeWidth:
                        {
                            Is_NeedReLayout = true;
                            break;
                        }
                    case ResetType.ShapeHeight:
                        {
                            Is_NeedReLayout = true;
                            break;
                        }
                    case ResetType.LineColor:
                        {
                            foreach (var item in LastUsed_Connector)
                            {
                                item.Appearance.BorderColor = Setting.LineColor;
                            }
                            foreach (var item in LastUnUsed_Connector)
                            {
                                item.Appearance.BorderColor = Setting.LineColor;
                            }
                            break;
                        }
                    case ResetType.LineStyle:
                        {
                            ConnectorType line = ConnectorType.RegisteredTypes.ToList().Where(obj => obj.TypeName == Setting.LineStyle).FirstOrDefault();
                            if (line != null)
                            {
                                foreach (var item in LastUsed_Connector)
                                {
                                    item.Type = line;
                                }
                                foreach (var item in LastUnUsed_Connector)
                                {
                                    item.Type = line;
                                }
                            }
                            break;
                        }
                    case ResetType.ArrowStyle:
                        {
                            ArrowDescription arrow = ArrowDescriptions.Arrows.ToList().Where(obj => obj.Name == Setting.ArrowStyle).FirstOrDefault();
                            if (arrow != null)
                            {
                                foreach (var item in LastUsed_Connector)
                                {
                                    item.EndArrow = arrow;
                                }
                                foreach (var item in LastUnUsed_Connector)
                                {
                                    item.EndArrow = arrow;
                                }
                            }
                            break;
                        }
                    case ResetType.ArrowSize:
                        {
                            foreach (var item in LastUsed_Connector)
                            {
                                item.EndArrowSize = new SizeF(Setting.ArrowSize, Setting.ArrowSize);
                            }
                            foreach (var item in LastUnUsed_Connector)
                            {
                                item.EndArrowSize = new SizeF(Setting.ArrowSize, Setting.ArrowSize);
                            }
                            break;
                        }
                    case ResetType.MoveAble:
                        {
                            foreach (var item in LastUsed_Shape)
                            {
                                item.CanMove = Setting.Is_MoveAble;
                            }
                            foreach (var item in LastUnUsed_Shape)
                            {
                                item.CanMove = Setting.Is_MoveAble;
                            }
                            foreach (var item in LastUsed_Connector)
                            {
                                item.CanMove = Setting.Is_MoveAble;
                            }
                            foreach (var item in LastUnUsed_Connector)
                            {
                                item.CanMove = Setting.Is_MoveAble;
                            }
                            break;
                        }
                    case ResetType.ScaleAble:
                        {
                            if (Setting.Is_ScaleAble)
                            {
                                General.DiagramControl.OptionsView.MaxZoomFactor = 5F;
                                General.DiagramControl.OptionsView.MinZoomFactor = 0.1F;
                            }
                            else
                            {
                                General.DiagramControl.OptionsView.MaxZoomFactor = General.DiagramControl.OptionsView.ZoomFactor;
                                General.DiagramControl.OptionsView.MinZoomFactor = General.DiagramControl.OptionsView.ZoomFactor;
                            }

                            foreach (var item in LastUsed_Shape)
                            {
                                item.CanResize = false;
                                item.CanRotate = false;
                            }
                            foreach (var item in LastUnUsed_Shape)
                            {
                                item.CanResize = false;
                                item.CanRotate = false;
                            }
                            foreach (var item in LastUsed_Connector)
                            {
                                item.CanResize = false;
                                item.CanRotate = false;
                            }
                            foreach (var item in LastUnUsed_Connector)
                            {
                                item.CanResize = false;
                                item.CanRotate = false;
                            }
                            break;
                        }
                    default: break;
                }
                if (Is_NeedReLayout)
                {
                    ResetData();
                }
                else Diagram.Refresh();
            }

        }

        /// <summary>
        /// 根据用户设置的数据对象，动态刷新图形控件对象
        /// </summary>
        /// <param name="IsRefreshWhenViewRectNochanged">是否忽而略可视矩形区域不变（强制刷新）</param>
        public void RefreshDiagram(bool IsRefreshWhenViewRectNochanged = false)
        {
            try
            {
                if (Diagram == null || Style == null || DatasAll == null || Setting == null) return;

                //锁定视图，禁止重绘等事件发生
                Diagram.BeginUpdate();

                //图形默认宽高
                int shapeWidth = Style.ShapeWidth;
                int shapeHeight = (Style.ShapeDescriptionRectHeight + Style.ShapeIdRectHeight + Style.ShapeSymbolRectHeight) + General.PEN_WIDTH;
                //int shapeHeight = (General.FtaProgram.CurrentSystem.ShapeHeight + Style.ShapeIdRectHeight + Style.ShapeSymbolRectHeight) + General.PEN_WIDTH;

                //先得到可视区域
                PointF start = Diagram.DiagramViewInfo.RulersOffset;
                float factor = Diagram.DiagramViewInfo.ZoomFactor;
                float width = Diagram.DiagramViewInfo.ContentRect.Width;
                float height = Diagram.DiagramViewInfo.ContentRect.Height;
                //增加显示范围扩展一个元素，防止出现显示不全
                RectangleF rect = new RectangleF(-start.X / factor - (shapeWidth + Style.ShapeGap), -start.Y / factor - (shapeHeight + Style.ShapeGap), width / factor + 2 * (shapeWidth + Style.ShapeGap), height / factor + 2 * (shapeHeight + Style.ShapeGap));
                if (!IsRefreshWhenViewRectNochanged && rect.X == LastRect.X && rect.Y == LastRect.Y && rect.Width == LastRect.Width && rect.Height == LastRect.Height)
                    return;
                LastRect = rect;

                //存放本次数据和绑定的图形字典，用于连线查找父子关系
                Dictionary<DrawData, DiagramShape> dic_Shape = new Dictionary<DrawData, DiagramShape>();

                #region 筛选要显示的数据
                //初始化确定哪些drawData要显示，在可视区里的第一批源数据
                //这里特殊化搜索范围，增强性能
                List<DrawData> data_Show = new List<DrawData>();
                for (int i = 0; i < DatasAll.Count; i++)
                {
                    List<DrawData> data_Level = DatasAll[i];

                    if (data_Level.Count > 0 && !((data_Level[0].Y + shapeHeight) < rect.Y || (rect.Y + rect.Height) < data_Level[0].Y))
                    {
                        foreach (DrawData data in data_Level)
                        {
                            RectangleF rectTmp = new Rectangle(data.X, data.Y, shapeWidth, shapeHeight);
                            if (rect.IntersectsWith(rectTmp))
                            {
                                data_Show.Add(data);
                            }
                        }
                    }
                }

                //初始化要展示的图形和连线，源数据基础上增加某些需要的子数据和父数据
                List<DrawData> data_Connector = new List<DrawData>(data_Show);//存放所有子节点的数据（需要连线的数据）
                List<DrawData> data_ParentChild = new List<DrawData>();//临时集合，要增加的数据集合
                foreach (var item in data_Show)
                {
                    //到父节点的线要有
                    if (item.Parent != null && !data_Show.Contains(item.Parent))
                    {
                        data_ParentChild.Add(item.Parent);
                    }
                    //子节点到他的线要有，这里取巧最远和最近的子元素
                    if (item.Children != null && item.Children.Count > 0)
                    {
                        if (!data_Show.Contains(item.Children[0]))
                        {
                            data_ParentChild.Add(item.Children[0]);
                            data_Connector.Add(item.Children[0]);
                        }
                        //最远的2个儿子节点加入就好
                        if (item.Children.Count > 1 && !data_Show.Contains(item.Children[item.Children.Count - 1]))
                        {
                            data_ParentChild.Add(item.Children[item.Children.Count - 1]);
                            data_Connector.Add(item.Children[item.Children.Count - 1]);
                        }
                    }
                }
                data_Show.AddRange(data_ParentChild);
                List<DrawData> data_Shape = data_Show;//所有要显示的图形数据

                //选中图形集合的处理保证至少有一个是选中图形，在源数据基础上，增加选中的数据（这里连线不需要）
                SelectedDataHide = null;
                SelectedDataShow.Clear();
                if (SelectedData != null && SelectedData.Count > 0)
                {
                    //找到所有在可视区的选中的图形
                    foreach (var item in data_Shape)
                    {
                        if (SelectedData.Contains(item))
                        {
                            SelectedDataShow.Add(item);
                        }
                    }
                    //找到第一个不在可视区的选中图形作为增加选取还是替换选取标志
                    foreach (var item in SelectedData)
                    {
                        if (!SelectedDataShow.Contains(item) && item.Type != DrawType.TransferInGate)
                        {
                            SelectedDataHide = item;
                            data_Shape.Add(SelectedDataHide);
                            break;
                        }
                    }
                    //如果4个边的元素存在，就加入，让矩形选择框不至于变形
                    if (SelectedDataSide != null)
                    {
                        if (SelectedDataSide.Left != null && !data_Shape.Contains(SelectedDataSide.Left)) data_Shape.Add(SelectedDataSide.Left);
                        if (SelectedDataSide.Right != null && !data_Shape.Contains(SelectedDataSide.Right)) data_Shape.Add(SelectedDataSide.Right);
                        if (SelectedDataSide.Up != null && !data_Shape.Contains(SelectedDataSide.Up)) data_Shape.Add(SelectedDataSide.Up);
                        if (SelectedDataSide.Down != null && !data_Shape.Contains(SelectedDataSide.Down)) data_Shape.Add(SelectedDataSide.Down);
                    }
                }
                #endregion

                //要选中的图形和连线
                List<DiagramItem> selectedItems = new List<DiagramItem>();

                #region 保留已存在的图形和连线对象,减少要显示的图形连线集合
                //保留已存在图形
                List<DrawData> dataShape_Tmp = new List<DrawData>();
                List<DiagramShape> shape_Keep = new List<DiagramShape>();
                bool IsFindShape = false;
                foreach (var data in data_Shape)
                {
                    IsFindShape = false;
                    foreach (var shape in LastUsed_Shape)
                    {
                        //只有图形位置不变，大小不变才能复用
                        if (shape.Tag == data && shape.X == data.X && shape.Y == data.Y
                            && shape.Width == shapeWidth && shape.Height == shapeHeight)
                        {
                            if (SelectedData != null)
                            {
                                if (SelectedData.Contains(data))
                                {
                                    if (!shape.IsSelected)
                                    {
                                        selectedItems.Add(shape);
                                    }
                                }
                                else if (shape.IsSelected) Diagram.UnselectItem(shape);
                            }
                            else if (shape.IsSelected) Diagram.UnselectItem(shape);

                            IsFindShape = true;
                            shape_Keep.Add(shape);
                            LastUsed_Shape.Remove(shape);
                            break;
                        }
                    }
                    if (!IsFindShape) dataShape_Tmp.Add(data);
                }
                List<DiagramShape> shape_Remove = LastUsed_Shape;
                LastUsed_Shape = shape_Keep;
                data_Shape = dataShape_Tmp;
                //为图形字典赋值
                dic_Shape = LastUsed_Shape.ToDictionary(obj => { return (DrawData)obj.Tag; });

                //保留已存在连线
                List<DrawData> dataConnector_Tmp = new List<DrawData>();
                List<DiagramConnector> connector_Keep = new List<DiagramConnector>();
                bool IsFindConnector = false;
                foreach (var data in data_Connector)
                {
                    IsFindConnector = false;
                    foreach (var con in LastUsed_Connector)
                    {
                        if (con.BeginItem?.Tag == data.Parent && con.EndItem?.Tag == data && dic_Shape.ContainsKey(data.Parent) && dic_Shape.ContainsKey(data))
                        {
                            if (SelectedData != null)
                            {
                                if (SelectedData.Contains(data.Parent) && SelectedData.Contains(data))
                                {
                                    if (!con.IsSelected)
                                    {
                                        selectedItems.Add(con);
                                    }
                                }
                                else if (con.IsSelected) Diagram.UnselectItem(con);
                            }
                            else if (con.IsSelected) Diagram.UnselectItem(con);

                            IsFindConnector = true;
                            connector_Keep.Add(con);
                            LastUsed_Connector.Remove(con);
                            break;
                        }
                    }
                    if (!IsFindConnector) dataConnector_Tmp.Add(data);
                }
                List<DiagramConnector> connector_Remove = LastUsed_Connector;
                LastUsed_Connector = connector_Keep;
                data_Connector = dataConnector_Tmp;
                #endregion

                #region 绑定数据和图形对象以及连线对象
                //新建的图形或连线对象
                List<DiagramItem> itemsAdded = new List<DiagramItem>();

                //绑定图形和数据
                foreach (DrawData data in data_Shape)
                {
                    if (data != null && !dic_Shape.ContainsKey(data))
                    {
                        DiagramShape shape = null;
                        //先从要删除的里面取
                        if (shape_Remove.Count > 0)
                        {
                            shape = shape_Remove[0];
                            shape_Remove.RemoveAt(0);
                        }
                        //取完了到还未使用的里面取
                        else if (LastUnUsed_Shape.Count > 0)
                        {
                            shape = LastUnUsed_Shape[0];
                            LastUnUsed_Shape.RemoveAt(0);
                        }
                        //又用完了就新建
                        else
                        {
                            shape = GenerateDiagramShape(data);
                            itemsAdded.Add(shape);
                        }
                        shape.Tag = data;
                        shape.CanSelect = true;
                        shape.Size = new SizeF(shapeWidth, shapeHeight);
                        shape.Position = new DevExpress.Utils.PointFloat(data.X, data.Y);
                        //选择或取消图形选择
                        if (SelectedData != null)
                        {
                            if (SelectedData.Contains(data))
                            {
                                if (!shape.IsSelected)
                                {
                                    selectedItems.Add(shape);
                                }
                            }
                            else if (shape.IsSelected) Diagram.UnselectItem(shape);
                        }
                        else if (shape.IsSelected) Diagram.UnselectItem(shape);
                        dic_Shape.Add(data, shape);
                        LastUsed_Shape.Add(shape);
                    }
                }

                //连线关系建立
                foreach (DrawData data in data_Connector)
                {
                    if (data.Parent != null)
                    {
                        DiagramConnector con = null;
                        //先从要删除的里面取
                        if (connector_Remove.Count > 0)
                        {
                            con = connector_Remove[0];
                            connector_Remove.RemoveAt(0);
                            con.BeginItem = dic_Shape[data.Parent];
                            con.EndItem = dic_Shape[data];
                        }
                        //取完了到还未使用的里面取
                        else if (LastUnUsed_Connector.Count > 0)
                        {
                            con = LastUnUsed_Connector[0];
                            LastUnUsed_Connector.RemoveAt(0);
                            con.BeginItem = dic_Shape[data.Parent];
                            con.EndItem = dic_Shape[data];
                        }
                        //又用完了就新建
                        else
                        {
                            con = GenerateDiagramConenctor(dic_Shape[data.Parent], dic_Shape[data]);
                            itemsAdded.Add(con);
                        }
                        //设置连线的折点
                        con.Points = new PointCollection(new List<PointFloat>(){
                                         new PointFloat(data.Parent.X + shapeWidth / 2 ,data.Parent.Y  + shapeHeight + (data.Y - shapeHeight - data.Parent.Y)/2),
                                         new PointFloat(data.X + shapeWidth / 2 ,data.Parent.Y  + shapeHeight + (data.Y - shapeHeight - data.Parent.Y)/2)
                                    });
                        //选择或取消连线选择
                        if (SelectedData != null)
                        {
                            if (SelectedData.Contains(data.Parent) && SelectedData.Contains(data))
                            {
                                if (!con.IsSelected)
                                {
                                    selectedItems.Add(con);
                                }
                            }
                            else if (con.IsSelected) Diagram.UnselectItem(con);
                        }
                        else if (con.IsSelected) Diagram.UnselectItem(con);
                        con.CanSelect = true;
                        LastUsed_Connector.Add(con);
                    }
                }

                //增加要增加的图形连线对象
                Diagram.Items.AddRange(itemsAdded.ToArray());
                #endregion

                //设置要增加的选中的图形为选中状态
                Diagram.SelectItems(selectedItems, ModifySelectionMode.AddToSelection);

                #region 处理用剩下的上次图形或连线对象
                //如果还剩下要移除的图形或连线对象
                //清空图形对象属性等
                foreach (var item in shape_Remove)
                {
                    item.Tag = null;
                    item.CanSelect = false;
                    //item.Position = new DevExpress.Utils.PointFloat(0, 0);
                    item.Size = new SizeF(0, 0);
                }
                //清空连线属性等
                foreach (var item in connector_Remove)
                {
                    item.CanSelect = false;
                    item.Position = new DevExpress.Utils.PointFloat(0, 0);
                    item.Size = new SizeF(0, 0);
                    item.BeginItem = null;
                    item.EndItem = null;
                    item.Points = null;
                }
                LastUnUsed_Shape.AddRange(shape_Remove);
                LastUnUsed_Connector.AddRange(connector_Remove);
                #endregion

                #region 根据当前使用个数记录情况，移除池当中多余的图形或连线
                //删除多余的图形或连线对象
                RecentData.Record(LastUsed_Shape.Count);
                int? avg = RecentData.GetRecentUsedCout();
                if (avg != null)
                {
                    if (LastUnUsed_Shape.Count > 0)
                    {
                        if (avg <= LastUsed_Shape.Count)
                        {
                            foreach (var item in LastUnUsed_Shape)
                            {
                                Diagram.Items.Remove(item);
                            }
                            LastUnUsed_Shape.Clear();
                        }
                        else if (avg > LastUsed_Shape.Count)
                        {
                            int abs = (int)avg - LastUsed_Shape.Count;
                            if (abs < LastUnUsed_Shape.Count)
                            {
                                for (int k = 0; k < LastUnUsed_Shape.Count - abs; k++)
                                {
                                    Diagram.Items.Remove(LastUnUsed_Shape[k]);
                                }
                                LastUnUsed_Shape.RemoveRange(0, LastUnUsed_Shape.Count - abs);
                            }
                        }
                    }
                    if (LastUnUsed_Connector.Count > 0)
                    {
                        if (avg <= LastUsed_Connector.Count)
                        {
                            foreach (var item in LastUnUsed_Connector)
                            {
                                Diagram.Items.Remove(item);
                            }
                            LastUnUsed_Connector.Clear();
                        }
                        else if (avg > LastUsed_Connector.Count)
                        {
                            int abs = (int)avg - LastUsed_Connector.Count;
                            if (LastUnUsed_Connector.Count > abs)
                            {
                                for (int k = 0; k < LastUnUsed_Connector.Count - abs; k++)
                                {
                                    Diagram.Items.Remove(LastUnUsed_Connector[k]);
                                }
                                LastUnUsed_Connector.RemoveRange(0, LastUnUsed_Connector.Count - abs);
                            }
                        }
                    }
                }
                #endregion


                //重新刷新父子关系
                if (Root != null)
                {
                    List<DrawData> datalist = Root.GetAllData(General.FtaProgram.CurrentSystem, true);
                    foreach (DiagramItem item in Diagram.Items)
                    {
                        foreach (DrawData da in datalist)
                        {
                            if (da != null && item.Tag != null && ((DrawData)item.Tag).ThisGuid == da.ThisGuid && (((DrawData)item.Tag).Children == null || ((DrawData)item.Tag).Parent == null))
                            {
                                ((DrawData)item.Tag).Children = da.Children;
                                ((DrawData)item.Tag).Parent = da.Parent;
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //这里触发了选择改变事件
                IsIgnoreSelectionChange = true;
                try
                {
                    Diagram.EndUpdate();
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    IsIgnoreSelectionChange = false;
                }
            }
        }

        #region IDisposable Support
        /// <summary>
        /// 是否已经释放了
        /// </summary>
        public bool IsDisposed { get { return isDisposed; } }

        private bool isDisposed = false; // 要检测冗余调用
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    //释放托管状态(托管对象)。
                }

                #region 图形控件清空，以及事件解除注册
                if (Diagram != null && !Diagram.IsDisposed)
                {
                    try
                    {
                        Diagram.BeginUpdate();
                        Diagram.Items.Clear();
                        HScrollBar hbar = Diagram.Controls[0] as HScrollBar;
                        VScrollBar vbar = Diagram.Controls[1] as VScrollBar;
                        vbar.ValueChanged -= Hbar_ValueChanged;
                        hbar.ValueChanged -= Hbar_ValueChanged;
                        Diagram.ZoomFactorChanged -= DiagramControl_PreView_ZoomFactorChanged;
                        Diagram.SelectionChanged -= Diagram_SelectionChanged;
                        Diagram.KeyDown -= Diagram_KeyDown;
                        Diagram.Commands.RegisterHotKeys(o =>
                        {
                            o.RegisterHotKey(System.Windows.Input.Key.A, System.Windows.Input.ModifierKeys.Control, DiagramCommandsBase.SelectAllCommand);
                        });
                    }
                    catch (Exception)
                    {

                    }
                    finally
                    {
                        Diagram.EndUpdate();
                    }
                }
                #endregion

                LastRect = new RectangleF(0, 0, 0, 0);
                LastUsed_Shape?.Clear();
                LastUsed_Connector?.Clear();
                LastUnUsed_Shape?.Clear();
                LastUnUsed_Connector?.Clear();
                DatasAll?.Clear();
                RecentData = null;
                SelectedData?.Clear();
                SelectedDataSide = null;
                SelectedDataShow?.Clear();
                this.Diagram = null;
                this.Style = null;
                this.Setting = null;
                this.IsIgnoreSelectionChange = true;
                this.Root = null;
                this.SelectedDataHide = null;
                this.SelectedDataShow?.Clear();
                isDisposed = true;
            }
        }

        //拥有用于释放未托管资源的代码时才替代终结器。
        ~DiagramItemPool()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(false);
        }

        /// <summary>
        /// 释放该类
        /// </summary>
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region 用于存放选择的图形区域里对角的数据
        private class SelectedDataStickToSide
        {
            public DrawData Left { get; set; }
            public DrawData Up { get; set; }
            public DrawData Right { get; set; }
            public DrawData Down { get; set; }
        }
        #endregion

        #region 统计用户对某项最近使用个数的内部类
        /// <summary>
        /// 一个简单结构，统计用户对某项最近使用个数，用于优化用户缩放操作，清除图形池里的元素
        /// </summary>
        private class RecentUsedData
        {
            /// <summary>
            /// 记录最近使用的个数
            /// </summary>
            private List<int> RecentUsedCout { get; set; }

            /// <summary>
            /// 最大能记录的个数
            /// </summary>
            private UInt16 MaxRecordCount = 10;

            /// <summary>
            /// 连续几次记录结果值相近的情况下符合
            /// </summary>
            private UInt16 ContinueCount = 3;

            /// <summary>
            /// 和平均值相差多少个，才算相近，可正负
            /// </summary>
            private UInt16 DifferenceCountOfAvg = 200;

            /// <summary>
            /// 最少使用多少个
            /// </summary>
            private int MinUseCount = 200;

            /// <summary>
            /// 初始化设置，用于记录和返回平均值
            /// </summary>
            /// <param name="MaxRecordCount">最大能记录的个数</param>
            /// <param name="ContinueCount">连续几次记录结果值相近的情况下符合</param>
            /// <param name="DifferenceCountOfAvg">和平均值相差多少个，才算相近，可正负</param>
            /// <param name="MinUseCount">最少使用多少个</param>
            public RecentUsedData(UInt16 MaxRecordCount = 10, UInt16 ContinueCount = 3, UInt16 DifferenceCountOfAvg = 200, int MinUseCount = 200)
            {
                this.RecentUsedCout = new List<int>();
                this.MaxRecordCount = MaxRecordCount;
                this.ContinueCount = ContinueCount;
                this.DifferenceCountOfAvg = DifferenceCountOfAvg;
                this.MinUseCount = MinUseCount;
            }

            /// <summary>
            /// 记录最近用户的使用个数
            /// </summary>
            public void Record(int num)
            {
                //清除池里的数据，最近使用的图形连续3次都是相近个数，那么xx
                RecentUsedCout.Add(num);
                if (RecentUsedCout.Count > MaxRecordCount) RecentUsedCout = RecentUsedCout.GetRange(RecentUsedCout.Count - MaxRecordCount, MaxRecordCount);
            }

            /// <summary>
            /// 根据记录获取最近使用的数量值,当记录的数据不满足用户设置条件时那么返回null
            /// </summary>
            /// <returns></returns>
            public int? GetRecentUsedCout()
            {
                if (ContinueCount <= MaxRecordCount)
                {
                    //如果每个和他们的平均数接近，那么他们之间应该很接近
                    if (RecentUsedCout.Count >= ContinueCount)
                    {
                        for (int i = RecentUsedCout.Count - ContinueCount; i <= RecentUsedCout.Count - ContinueCount; i++)
                        {
                            int avg = 0;
                            for (int j = 0; j < ContinueCount; j++)
                            {
                                avg += RecentUsedCout[i + j];
                            }
                            avg /= ContinueCount;
                            bool IsOK = true;
                            for (int j = 0; j < ContinueCount; j++)
                            {
                                if (Math.Abs(RecentUsedCout[i + j] - avg) > DifferenceCountOfAvg)
                                {
                                    IsOK = false;
                                    break;
                                }
                            }
                            if (IsOK)
                            {
                                avg = avg < MinUseCount ? MinUseCount : avg;
                                return avg;
                            }
                        }
                    }
                }
                return null;
            }
        }
        #endregion
    }
}