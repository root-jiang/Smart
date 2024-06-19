using DevExpress.Diagram.Core;
using DevExpress.Utils;
using DevExpress.XtraDiagram;
using FaultTreeAnalysis.Common;
using FaultTreeAnalysis.Model.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace FaultTreeAnalysis
{
    /// <summary>
    /// 获取当前路径
    /// </summary>
    public class CommonFunction
    {
        /// <summary>
        /// 获取当前程序的父目录
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentPath()
        {
            string path = Application.StartupPath; 
            return path;
        }

        /// <summary>
        /// 返回一个已经添加好并有位置的图形控件对象
        /// CommonFunction.GetPrintedChildren(root, new SizeF(100, 100), ftaProgram, CustomDrawItem_diagramControl_FTADiagram).ShowPrintPreview();
        /// </summary>
        /// <param name="data">数据对象</param>
        /// <param name="shapeSize">图形大小</param>
        /// <param name="ftaProgram">全局程序对象</param>
        /// <param name="CustomDrawItem">自定义绘制</param>
        /// <returns></returns>
        public static DiagramControl GetPrintedChildren(DrawData data, SizeF shapeSize, ProgramModel ftaProgram, EventHandler<CustomDrawItemEventArgs> CustomDrawItem)
        {
            return GetPrintedChildren_ALL(data, shapeSize, ftaProgram, CustomDrawItem);
        }

        /// <summary>
        /// 返回一个已经添加好并有位置的图形控件对象
        /// </summary>
        /// <param name="data">数据对象集合</param>
        /// <param name="shapeSize">图形大小</param>
        /// <param name="ftaProgram">全局程序对象</param>
        /// <param name="CustomDrawItem">自定义绘制</param>
        /// <returns></returns>
        public static DiagramControl GetPrintedChildren(List<DrawData> data, SizeF shapeSize, ProgramModel ftaProgram, EventHandler<CustomDrawItemEventArgs> CustomDrawItem)
        {
            return GetPrintedChildren_ALL(data, shapeSize, ftaProgram, CustomDrawItem);
        }

        /// <summary>
        /// 返回一个已经添加好并有位置的图形控件对象
        /// </summary>
        /// <param name="data">数据对象</param>
        /// <param name="shapeSize">图形大小</param>
        /// <param name="ftaProgram">全局程序对象</param>
        /// <param name="CustomDrawItem">自定义绘制</param>
        /// <returns></returns>
        private static DiagramControl GetPrintedChildren_ALL(object data, SizeF shapeSize, ProgramModel ftaProgram, EventHandler<CustomDrawItemEventArgs> CustomDrawItem)
        {
            return General.TryCatch(() =>
            {
                DiagramControl result = null;
                result = new DiagramControl();
                ((System.ComponentModel.ISupportInitialize)result).BeginInit();
                result.Location = new Point(0, 0);
                result.Dock = System.Windows.Forms.DockStyle.Fill;
                result.OptionsBehavior.SelectedStencils = new StencilCollection(new string[] { "BasicShapes" });
                result.OptionsView.MaxZoomFactor = 3F;
                result.OptionsView.MinZoomFactor = 0.4F;
                result.OptionsView.PaperKind = System.Drawing.Printing.PaperKind.Letter;
                result.OptionsView.ShowRulers = ftaProgram.Setting.Is_Show_Ruler;
                result.OptionsView.ShowGrid = ftaProgram.Setting.Is_Show_Grid;
                result.OptionsView.ShowPageBreaks = ftaProgram.Setting.Is_Show_PageBreak;
                if (ftaProgram.Setting.Is_CanvasFillMode) result.OptionsView.CanvasSizeMode = CanvasSizeMode.Fill;
                else result.OptionsView.CanvasSizeMode = CanvasSizeMode.AutoSize;
                if (ftaProgram.Setting.Is_PageScrollMode) result.OptionsBehavior.ScrollMode = DiagramScrollMode.Page;
                else result.OptionsBehavior.ScrollMode = DiagramScrollMode.Content;
                result.Size = new Size(1745, 679);
                ((System.ComponentModel.ISupportInitialize)result).EndInit();
                result.CustomDrawItem += CustomDrawItem;
                DrawData data1 = data is DrawData ? data as DrawData : null;
                List<DrawData> data2 = data is List<DrawData> ? data as List<DrawData> : null;
                if (data1 != null)
                {
                    result.Items.AddRange(CommonFunction.GetPrintedChildren(data1, shapeSize, new SizeF(result.OptionsView.PageSize.Width - 50, result.OptionsView.PageSize.Height - 50), ftaProgram, 25, 25).ToArray());
                }
                else if (data2 != null)
                {
                    result.Items.AddRange(CommonFunction.GetPrintedChildren(data2, shapeSize, new SizeF(result.OptionsView.PageSize.Width - 50, result.OptionsView.PageSize.Height - 50), ftaProgram, 25, 25).ToArray());
                }
                return result;
            });
        }

        /// <summary>
        /// 返回打印所需的排好序的图形对象（位置会慢慢错位）
        /// 测试代码this.currentDiagram.Items.AddRange(CommonFunction.GetPrintedChildren(dat, new SizeF(100, 100),new SizeF( currentDiagram.OptionsView.PageSize.Width -50, currentDiagram.OptionsView.PageSize.Height - 50), ftaProgram,25,25).ToArray());
        /// </summary>
        /// <param name="data">数据集合</param>
        /// <param name="shapeSize">图形大小</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="ftaProgram">全局程序对象</param>
        /// <param name="startX">开始X位置</param>
        /// <param name="startY">开始Y位置</param>
        /// <param name="distanceX">图形间水平间隔</param>
        /// <param name="distanceY">图形间上下间隔</param>
        /// <param name="padding">页面内边距</param>
        /// <returns></returns>
        public static List<DiagramItem> GetPrintedChildren(List<DrawData> data, SizeF shapeSize, SizeF pageSize, ProgramModel ftaProgram, float startX = 0, float startY = 0, float distanceX = 20, float distanceY = 30, float padding = 0)
        {
            return General.TryCatch(() =>
            {
                List<DiagramItem> result = new List<DiagramItem>();
                if (data == null) return result;
                data.Reverse();
                //用到的变量
                startX += padding;
                startY += padding;
                int xCount = 0;
                int yCount = 0;
                int pageCount = 0;

                //简单初始化页大小，最大x方向个数，最大y方向个数
                pageSize = new SizeF(pageSize.Width - 2 * padding, pageSize.Height - 2 * padding);
                float tmp = pageSize.Width % (shapeSize.Width + distanceX);
                xCount = tmp > shapeSize.Width ? xCount = (int)(pageSize.Width / (shapeSize.Width + distanceX)) + 1 : (int)(pageSize.Width / (shapeSize.Width + distanceX));
                tmp = pageSize.Height % (shapeSize.Height + distanceY);
                yCount = tmp > shapeSize.Height ? yCount = (int)(pageSize.Height / (shapeSize.Height + distanceY)) + 1 : (int)(pageSize.Height / (shapeSize.Height + distanceY));

                //创建父图形
                DiagramShape shape_Parent = null;

                //添加子图形
                if (xCount > 0 && yCount > 0)
                {
                    xCount = 1;
                    startX += (pageSize.Width - xCount * shapeSize.Width - (xCount - 1) * distanceX) / 2;
                    startY += (pageSize.Height - yCount * shapeSize.Height - (yCount - 1) * distanceY) / 2;
                    while (data.Count > 0)
                    {
                        for (int y = 0; y < yCount; y++)
                        {
                            if (data.Count > 0)
                            {
                                DiagramShape shape = new DiagramShape();
                                shape.Size = shapeSize;
                                shape.Tag = data[data.Count - 1];
                                shape.Position = new DevExpress.Utils.PointFloat(startX,
                                    startY + (pageSize.Height + 2 * padding) * pageCount + y * (shapeSize.Height + distanceY));
                                shape.ConnectionPoints = new PointCollection(new List<PointFloat>() { new PointFloat(0.5f, 0), new PointFloat(0.5f, 1) });
                                result.Add(shape);
                                data.Remove(shape.Tag as DrawData);
                                shape.CanEdit = false;
                                shape.CanDelete = false;
                                shape.CanCopy = false;
                                shape.CanMove = ftaProgram.Setting.Is_MoveAble;
                                shape.CanResize = false;
                                shape.CanRotate = false;

                                if (shape_Parent != null)
                                {
                                    DiagramConnector connector = new DiagramConnector(shape_Parent, shape);

                                    connector.Appearance.BorderColor = ftaProgram.Setting.LineColor;
                                    connector.EndArrowSize = new SizeF(ftaProgram.Setting.ArrowSize, ftaProgram.Setting.ArrowSize);

                                    ArrowDescription arrow = ArrowDescriptions.Arrows.ToList().Where(obj => obj.Name == ftaProgram.Setting.ArrowStyle).FirstOrDefault();
                                    if (arrow != null) connector.EndArrow = arrow;

                                    connector.CanDelete = false;
                                    connector.CanMove = ftaProgram.Setting.Is_MoveAble;
                                    connector.CanChangeRoute = false;
                                    connector.CanEdit = false;
                                    connector.CanCopy = false;
                                    ((DiagramItem)connector).CanResize = false;
                                    ((DiagramItem)connector).CanRotate = false;
                                    result.Add(connector);
                                }
                                shape_Parent = shape;
                            }
                            else break;
                        }
                        pageCount++;
                    }
                }
                return result;
            });
        }

        /// <summary>
        /// 返回打印所需的排好序的图形对象（第一层子节点）（位置会慢慢错位）
        /// </summary>
        /// <param name="data">父节点</param>
        /// <param name="shapeSize">图形大小</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="ftaProgram">全局程序对象</param>
        /// <param name="startX">开始X位置</param>
        /// <param name="startY">开始Y位置</param>
        /// <param name="distanceX">图形间水平间隔</param>
        /// <param name="distanceY">图形间上下间隔</param>
        /// <param name="padding">页面内边距</param>
        /// <returns></returns>
        public static List<DiagramItem> GetPrintedChildren(DrawData data, SizeF shapeSize, SizeF pageSize, ProgramModel ftaProgram, float startX = 0, float startY = 0, float distanceX = 20, float distanceY = 30, float padding = 0)
        {
            return General.TryCatch(() =>
            {
                List<DiagramItem> result = new List<DiagramItem>();
                if (data == null) return result;
                //用到的变量
                startX += padding;
                startY += padding;
                int xCount = 0;
                int yCount = 0;
                int pageCount = 0;

                //简单初始化页大小，最大x方向个数，最大y方向个数
                pageSize = new SizeF(pageSize.Width - 2 * padding, pageSize.Height - 2 * padding);
                float tmp = pageSize.Width % (shapeSize.Width + distanceX);
                xCount = tmp > shapeSize.Width ? xCount = (int)(pageSize.Width / (shapeSize.Width + distanceX)) + 1 : (int)(pageSize.Width / (shapeSize.Width + distanceX));
                tmp = pageSize.Height % (shapeSize.Height + distanceY);
                yCount = tmp > shapeSize.Height ? yCount = (int)(pageSize.Height / (shapeSize.Height + distanceY)) + 1 : (int)(pageSize.Height / (shapeSize.Height + distanceY));

                //创建父图形
                List<DrawData> children = new List<DrawData>(data.Children);
                DiagramShape shape_Parent = new DiagramShape();
                shape_Parent.Size = shapeSize;
                shape_Parent.Tag = data;
                shape_Parent.Position = new DevExpress.Utils.PointFloat((startX - padding + pageSize.Width - shapeSize.Width) / 2, startY);
                shape_Parent.ConnectionPoints = new PointCollection(new List<PointFloat>() { new PointFloat(0.5f, 1) });
                result.Add(shape_Parent);
                shape_Parent.CanEdit = false;
                shape_Parent.CanDelete = false;
                shape_Parent.CanCopy = false;
                shape_Parent.CanMove = ftaProgram.Setting.Is_MoveAble;
                shape_Parent.CanResize = false;
                shape_Parent.CanRotate = false;

                //添加子图形
                if (xCount > 0 && yCount > 0 && data != null && data.Children != null && data.Children.Count > 0)
                {
                    startX += (pageSize.Width - xCount * shapeSize.Width - (xCount - 1) * distanceX) / 2;
                    startY += (pageSize.Height - yCount * shapeSize.Height - (yCount - 1) * distanceY) / 2;
                    shape_Parent.Position = new DevExpress.Utils.PointFloat(startX + (xCount - 1) * (shapeSize.Width + distanceX) / 2, startY);
                    while (children.Count > 0)
                    {
                        for (int y = 0; y < yCount; y++)
                        {
                            //第一页,第一行，不放东西
                            if (pageCount == 0 && y == 0)
                            {
                                continue;
                            }
                            for (int x = 0; x < xCount; x++)
                            {
                                if (children.Count > 0)
                                {
                                    DiagramShape shape = new DiagramShape();
                                    shape.Size = shapeSize;
                                    shape.Tag = children[children.Count - 1];
                                    shape.Position = new DevExpress.Utils.PointFloat(startX + x * (shapeSize.Width + distanceX),
                                        startY + (pageSize.Height + 2 * padding) * pageCount + y * (shapeSize.Height + distanceY));
                                    shape.ConnectionPoints = new PointCollection(new List<PointFloat>() { new PointFloat(0.5f, 0) });
                                    result.Add(shape);
                                    children.Remove(shape.Tag as DrawData);
                                    shape.CanEdit = false;
                                    shape.CanDelete = false;
                                    shape.CanCopy = false;
                                    shape.CanMove = ftaProgram.Setting.Is_MoveAble;
                                    shape.CanResize = false;
                                    shape.CanRotate = false;

                                    #region 测试代码

                                    //ShapeDescription desc = new ShapeDescription("DrawData", () => { return "DrawBase"; },
                                    //    () => { return new System.Windows.Size(100, 100); },
                                    //    new ShapeGetter((w, h, p) => { return  ShapeGeometry.Create(new System.Windows.Point(0,0)); }),
                                    //    new ShapeConnectionPointsGetter((w, h, p) => { return null; }),
                                    //    new System.Func<ParameterCollection>(() => { return null; }),
                                    //    new EditorBoundsGetter((w, h, p) => { return new Rect(0, 0, w, h); }),
                                    //    new System.Func<DiagramItemStyleId>(
                                    //        () =>
                                    //        {
                                    //            return new DiagramItemStyleId("11", () => { return ""; },
                                    //             (th) =>
                                    //             {
                                    //                 return new DiagramItemStyle(new DiagramItemBrush(Color.Transparent, Color.Transparent, Color.Transparent)
                                    //              , new DiagramFontSettings(12, new FontFamilyInfo("Arial"), new DiagramFontEffects(false, false, false, false))
                                    //              , new DiagramItemLineSettings(0, null));
                                    //             });
                                    //        }),
                                    //    new System.Func<bool>(() => { return true; }),
                                    //    new System.Func<bool>(() => { return false; }));
                                    //shape.Shape = desc;
                                    #endregion

                                }
                                //最后一行居中
                                else
                                {
                                    float offsetX = (xCount - x) * (shapeSize.Width + distanceX) / 2;
                                    for (int i = result.Count - x; i < result.Count; i++)
                                    {
                                        result[i].X += offsetX;
                                    }
                                    goto Exit;
                                }
                            }
                        }
                        pageCount++;
                    }
                    Exit:;
                }

                //最后添加连线
                if (result.Count > 0)
                {
                    List<DiagramItem> cons = new List<DiagramItem>();
                    for (int i = 1; i < result.Count; i++)
                    {
                        DiagramConnector connector = new DiagramConnector(shape_Parent, result[i]);

                        connector.Appearance.BorderColor = ftaProgram.Setting.LineColor;
                        connector.EndArrowSize = new SizeF(ftaProgram.Setting.ArrowSize, ftaProgram.Setting.ArrowSize);

                        ArrowDescription arrow = ArrowDescriptions.Arrows.ToList().Where(obj => obj.Name == ftaProgram.Setting.ArrowStyle).FirstOrDefault();
                        if (arrow != null) connector.EndArrow = arrow;

                        connector.CanDelete = false;
                        connector.CanMove = ftaProgram.Setting.Is_MoveAble;
                        connector.CanChangeRoute = false;
                        connector.CanEdit = false;
                        connector.CanCopy = false;
                        connector.CanResize = ftaProgram.Setting.Is_ScaleAble;
                        ((DiagramItem)connector).CanRotate = false;
                        cons.Add(connector);
                    }
                    result.AddRange(cons);
                }
                return result;
            });
        }
    }
}
