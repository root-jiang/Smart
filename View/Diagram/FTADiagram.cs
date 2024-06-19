using DevExpress.Utils;
using DevExpress.XtraDiagram;
using FaultTreeAnalysis.Common;
using FaultTreeAnalysis.Model.Data;
using FaultTreeAnalysis.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace FaultTreeAnalysis.View.Diagram
{
    public class FtaDiagram
    {
        private ProgramModel programModel;

        /// <summary>
        /// 要处理的图形控件对象
        /// </summary>
        private DiagramControl diagramControl;

        /// <summary>
        /// 
        /// </summary>
        public DiagramEvents DiagramEvents { get; set; }

        public HashSet<DrawData> SelectedData => this.DiagramEvents.DiagramItemPool.SelectedData;

        public FtaDiagram(DiagramControl diagramControl, ProgramModel programModel)
        {
            this.diagramControl = diagramControl;
            this.programModel = programModel;
            this.DiagramEvents = new DiagramEvents(this.diagramControl, this.programModel);
            General.DiagramItemPool = this.DiagramEvents.DiagramItemPool;
        }

        /// <summary>
        /// 使指定drawdata图形对象可见,如果该图形不在该根节点会自动切换根节点
        /// </summary>
        /// <param name="data">要可见的数据对象</param>
        /// <param name="IsScrollToCenter">是否要把该元素滚动到中心位置</param>
        public void FocusOn(DrawData data, bool IsScrollToCenter = false)
        {
            try
            {
                if (data != null)
                {
                    //如果当前展示的图形不是同个根节点
                    DrawData root = data.GetRoot();
                    if (root != this.programModel.CurrentRoot)
                    {
                        if (root != null)
                        {
                            this.programModel.CurrentRoot = root;
                            this.DiagramEvents.DiagramItemPool.ResetData(root, null, true, false);
                        }
                        else if (this.programModel.CurrentRoot != null) root = this.programModel.CurrentRoot;
                    }

                    //设置该元素选中
                    this.DiagramEvents.DiagramItemPool.SelectedData.Clear();
                    this.DiagramEvents.DiagramItemPool.SelectedData.Add(data);

                    if (IsScrollToCenter || !this.DiagramEvents.DiagramItemPool.IsInDiagramViewRect(data))
                    {
                        //滚动滚动条使该元素可见
                        if (data == root) diagramControl.ScrollToPoint(new PointFloat(data.X, data.Y), HorzAlignment.Center, VertAlignment.Top);
                        else diagramControl.ScrollToPoint(new PointFloat(data.X, data.Y));
                    }

                    //这里强制刷新下，而且如果滚动条位置不变那么这里就必须要了   
                    this.DiagramEvents.DiagramItemPool.RefreshDiagram(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 初始化FTA图为初始状态
        /// </summary>
        public void ReSetDiagram()
        {
            General.TryCatch(() =>
            {
                this.DiagramEvents.DiagramItemPool.ResetData(null);
                this.programModel.CurrentRoot = null;
                this.DiagramEvents.IsCreateNew = false;
            });
        }

        /// <summary>
        /// 根据给出的节点对象，把内容显示到画布里,并把节点保存下来
        /// </summary>
        /// <param name="root">节点对象</param>
        public void Load(DrawData root = null, StyleModel style = null)
        {
            General.TryCatch(() =>
            {
                if (root != null && this.programModel.CurrentRoot != root)
                {
                    this.programModel.CurrentRoot = root;
                }
                else root = this.programModel.CurrentRoot;

                if (root != null)
                {
                    this.DiagramEvents.DiagramItemPool.ResetData(root, style, true, false);
                    diagramControl.ScrollToPoint(new PointFloat(root.X, root.Y), HorzAlignment.Center, VertAlignment.Top);
                    this.DiagramEvents.DiagramItemPool.RefreshDiagram(true);
                }
            });
        }

        public List<DrawData> GetSelectedDataList()
        {
            var result = new List<DrawData>();
            if (this.DiagramEvents.DiagramItemPool.SelectedData != null) result = this.DiagramEvents.DiagramItemPool.SelectedData.ToList();
            return result;
        }

        public void ResetData()
        {
            this.DiagramEvents.DiagramItemPool.ResetData();
        }

        public void Refresh(bool IsRefreshWhenViewRectNochanged = false)
        {
            this.DiagramEvents.DiagramItemPool.RefreshDiagram(IsRefreshWhenViewRectNochanged);
        }

        public void ResetSetting(ResetType type)
        {
            this.DiagramEvents.DiagramItemPool.ResetSetting(type);
        }


        /// <summary>
        /// 刷新界面上的表格和图表控件
        /// </summary>
        public void UpdateData()
        {
            this.diagramControl.Refresh();
        }
    }
}
