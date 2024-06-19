using FaultTreeAnalysis.Common;
using FaultTreeAnalysis.FTAControlEventHandle.Ribbon.FTA.Tool.FTA;
using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using FaultTreeAnalysis.Model.Enum;
using System;
using System.Drawing;
using FaultTreeAnalysis.Model.Data;
using FaultTreeAnalysis.View.Ribbon.FTA;

namespace FaultTreeAnalysis
{
    partial class FTAControl
    {
        /// <summary>
        /// TODO:重命名的配置信息，暂时还未保存（处理）
        /// </summary>
        private RenumberConfig renumberConfig = new RenumberConfig();

        /// <summary>
        /// 初始化Ribbon-Tool-FTA下的菜单
        /// </summary>
        private void Init_Ribbon_Tool_FTA()
        {
            this.Bbi_Renumber.ItemClick += BarButtonItemClick_RibbonToolFta;
            this.barButtonItem_ToolRenumber.ItemClick += BarButtonItemClick_RibbonToolFta;
            this.Bbi_QuickRenumber.ItemClick += BarButtonItemClick_RibbonToolFta;
        }

        /// <summary>
        /// Ribbon按钮项部分点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BarButtonItemClick_RibbonToolFta(object sender, ItemClickEventArgs e)
        {
            if (e.Item == this.Bbi_Renumber || e.Item == barButtonItem_ToolRenumber) this.PopupRenumberFaultTreeView();
            else if (e.Item == this.Bbi_QuickRenumber) MsgBox.Show("To be continue.");
        }

        /// <summary>
        /// 弹出重编号功能窗体
        /// </summary>
        private void PopupRenumberFaultTreeView()
        {
            if (General.FtaProgram.CurrentSystem == null)
            {
                return;
            }

            RenumberedRange SystemMode = RenumberedRange.SelectedTree;
            var selectedDrawData = default(DrawData);
            if (this.treeList_FTATable.FocusedNode != null)
            {
                selectedDrawData = this.treeList_FTATable.FocusedNode?.GetValue(FixedString.COLUMNAME_DATA) as DrawData;
            }
            else if (ftaDiagram.SelectedData.Count != 0)
            {
                selectedDrawData = ftaDiagram.SelectedData.FirstOrDefault();
            }

            if (selectedDrawData == null)
            {
                SystemMode = RenumberedRange.AllSystem;
            }

            var renumberFaultTree = new RenumberView(SystemMode, General.FtaProgram, General.FtaProgram.CurrentSystem, this.GetAllDrawDataFromTree, this.renumberConfig);
            renumberFaultTree.ShowDialog();
            this.treeList_FTATable.RefreshDataSource();
            this.ftaDiagram.Load();

        }

        private void PopupQuickRenumber()
        {
            if (General.FtaProgram.CurrentProject != null) new QuickRenumberView(General.FtaProgram.String, General.FtaProgram.CurrentProject.RenumberItems).Show();
        }

        /// <summary>
        /// 弹出重命名事件窗体
        /// </summary>
        private void PopupEventModifyView()
        {
            var allEventDrawData = General.FtaProgram.CurrentSystem?.GetAllDatas()?.Where(o => o.IsGateType == false);
            if (allEventDrawData != null)
            {
                var selectedEventDrawData = ftaDiagram.SelectedData.Where(o => o?.IsGateType == false)?.ToList();

                if (selectedEventDrawData?.Count > 0)
                {
                    var eventModifyView = new EventModifyView(General.FtaProgram, allEventDrawData, selectedEventDrawData);
                    eventModifyView.ShowDialog();

                    if (eventModifyView.Result)
                    {
                        this.treeList_FTATable.RefreshDataSource();
                        this.ftaDiagram.Load();
                    }
                }
                else MsgBox.Show(General.FtaProgram.String.EventModifyMessage);
            }
            else MsgBox.Show(General.FtaProgram.String.EventModifyMessage);
        }

        /// <summary>
        /// 通过任意节点返回该节点所在树的所有节点集合
        /// </summary>
        /// <returns></returns>
        private List<DrawData> GetAllDrawDataFromTree(RenumberedRange rootRange)
        {
            var selectedDrawData = default(DrawData);
            if (this.treeList_FTATable.FocusedNode != null)
            {
                selectedDrawData = this.treeList_FTATable.FocusedNode?.GetValue(FixedString.COLUMNAME_DATA) as DrawData;
            }
            else if (ftaDiagram.SelectedData.Count != 0)
            {
                selectedDrawData = ftaDiagram.SelectedData.FirstOrDefault();
            }

            var result = default(List<DrawData>);
            if (rootRange.Equals(RenumberedRange.AllSystem))
            {
                result = General.FtaProgram.CurrentSystem.GetAllDatas();
            }
            else
            {
                result = selectedDrawData.GetRoot().GetAllData(General.FtaProgram.CurrentSystem, rootRange == RenumberedRange.SelectedTree ? false : true);
            }

            return result;
        }

        /// <summary>
        /// 初始化Ribbon-Tool-ToolBox下的菜单
        /// </summary>
        private void Init_Ribbon_Tool_ToolBox()
        {
            //插入》图形工具下的菜单切换选中状态
            ribbonGalleryBarItem_GraphicTool.Gallery.ItemClick += ItemClick_ribbonGalleryBarItem_GraphicTool_Gallery;
        }

        /// <summary>
        /// 插入》图形工具下的菜单切换选中状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemClick_ribbonGalleryBarItem_GraphicTool_Gallery(object sender, GalleryItemClickEventArgs e)
        {
            General.TryCatch(() =>
            {
                //这里由于进来的item可能是复制的一个对象不是ribbonGalleryBarItem_GraphicTool里的
                GalleryItem item_Tmp = ribbonGalleryBarItem_GraphicTool.Gallery.GetItemByCaption(e.Item.Caption);
                bool Is_checkedLast = item_Tmp.Checked;
                ReSetCheckState();
                item_Tmp.Checked = !Is_checkedLast;
                this.ftaDiagram.DiagramEvents.IsInsertingNode = item_Tmp.Checked;
                //滚动到选择的元素并显示
                ribbonGalleryBarItem_GraphicTool.Gallery.MakeVisible(item_Tmp);
                General.isLinkGateInsert = false;
                //设置当前选中的图形类型
                if (e.Item.Value != null && e.Item.Value.GetType() == typeof(string))
                {
                    if (e.Item.Value.ToString() != "LinkGate")
                    {
                        var typeName = General.GetKeyName(e.Item.Value as string);
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
            });
        }

        /// <summary>
        /// 重置该菜单页中工具项的选中状态为初始状态
        /// </summary>
        private void ReSetCheckState()
        {
            General.TryCatch(() =>
            {
                //解除基本图形上次选中状态
                foreach (GalleryItem tmp in ribbonGalleryBarItem_GraphicTool.Gallery.GetCheckedItems())
                {
                    tmp.Checked = false;
                }
                //解除label工具选中状态
                this.ftaDiagram.DiagramEvents.AddingType = DrawType.NULL;
                this.transfer = null;
                this.ftaDiagram.DiagramEvents.RepeatedEvent = null;
                General.isLinkGateInsert = false;
            });
        }

        /// <summary>
        /// 获取系统颜色集合
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Color> GetSystemColors()
        {
            foreach (KnownColor item in Enum.GetValues(typeof(KnownColor)))
            {
                Color color = Color.FromKnownColor(item);
                if (color != Color.Transparent) yield return color;
            }
        }

        public void QuickRenumber()
        {

        }
    }
}
