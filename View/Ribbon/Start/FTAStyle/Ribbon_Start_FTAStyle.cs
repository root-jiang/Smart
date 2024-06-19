using DevExpress.XtraBars;
using FaultTreeAnalysis.Common;
using FaultTreeAnalysis.Model.Data;
using FaultTreeAnalysis.Model.Enum;
using System;
using System.Drawing;

namespace FaultTreeAnalysis
{
    partial class FTAControl
    {
        /// <summary>
        /// Ribbon 上FTAStyle变化时是否不处理这些值的变化
        /// </summary>
        private bool IsIgnoreFTAStyleCahnged = false;

        /// <summary>
        /// 初始化Ribbon-Start-FTAStyle下的菜单
        /// </summary>
        private void Init_Ribbon_Start_FTAStyle()
        {
            //画布里图形里描述框的高度变化
            Bei_ShapeRectHeight.EditValueChanged += EditValueChanged_Ribbon_Start_FTAStyle;

            //画布里图形里标识框的高度变化
            Bei_UnUse.EditValueChanged += EditValueChanged_Ribbon_Start_FTAStyle;

            //画布里图形里符号框高度变化
            Bei_SymbolSize.EditValueChanged += EditValueChanged_Ribbon_Start_FTAStyle;

            //画布里图形宽度设置变化
            Bei_ShapeRectWidth.EditValueChanged += EditValueChanged_Ribbon_Start_FTAStyle;

            //画布里图形的字体名设置变化
            Bei_ShapeFontName.EditValueChanged += EditValueChanged_Ribbon_Start_FTAStyle;

            //画布里图形的字体大小设置变化
            Bei_ShapeFontSize.EditValueChanged += EditValueChanged_Ribbon_Start_FTAStyle;

            //画布里图形的背景色变化
            barEditItem_ShapeBackColor.EditValueChanged += EditValueChanged_Ribbon_Start_FTAStyle;

            //画布里选中状态下图形的背景色变化
            barEditItem_ShapeBackColor_Selected.EditValueChanged += EditValueChanged_Ribbon_Start_FTAStyle;

            //画布里重复事件图形的背景色变化
            barEditItem_ShapeBackColor_RepeatEvent.EditValueChanged += EditValueChanged_Ribbon_Start_FTAStyle;

            //画布里真门/事件图形的背景色变化
            barEditItem_ShapeBackColor_TrueGate.EditValueChanged += EditValueChanged_Ribbon_Start_FTAStyle;

            //画布里假门/事件图形的背景色变化
            barEditItem_ShapeBackColor_FalseGate.EditValueChanged += EditValueChanged_Ribbon_Start_FTAStyle;

            //画布里割集颜色变化
            barEditItem_CutSetColor.EditValueChanged += EditValueChanged_Ribbon_Start_FTAStyle;

            ReSetFTAStyle();
        }

        /// <summary>
        /// 根据当前项目的设置值,重新设置ribbon》FTAStylePage》下的控件显示值和状态
        /// </summary>
        private void ReSetFTAStyle()
        {
            try
            {
                IsIgnoreFTAStyleCahnged = true;
                StyleModel style = General.FtaProgram?.CurrentProject?.Style;
                ftaTable.FTATable_StateImage_ResetRepeatedEventImage();
                //设置图形的默认宽高,文本框，符号高，字体名和大小
                Bei_ShapeRectWidth.EditValue = null;
                Bei_UnUse.EditValue = null;
                Bei_ShapeRectHeight.EditValue = null;
                Bei_SymbolSize.EditValue = null;
                Bei_ShapeFontName.EditValue = style?.ShapeFontName;
                Bei_ShapeFontSize.EditValue = style?.ShapeFontSize;

                //设置图形的默认背景色
                barEditItem_ShapeBackColor.EditValue = style?.ShapeBackColor;

                //选中状态下图形的背景色
                barEditItem_ShapeBackColor_Selected.EditValue = style?.ShapeBackSelectedColor;

                //重复事件图形的背景色
                barEditItem_ShapeBackColor_RepeatEvent.EditValue = style?.ShapeBackRepeatEventColor;

                //真门/事件图形的背景色
                barEditItem_ShapeBackColor_TrueGate.EditValue = style?.ShapeBackTrueGateColor;

                //假门/事件图形的背景色
                barEditItem_ShapeBackColor_FalseGate.EditValue = style?.ShapeBackFalseGateColor;

                //割集颜色
                barEditItem_CutSetColor.EditValue = style?.CutSetColor;

                //设置菜单是否可用
                if (style != null)
                {
                    ribbonPageGroup_Parameters.Enabled = true;

                }
                else
                {
                    ribbonPageGroup_Parameters.Enabled = false;
                }
                Bei_ShapeRectWidth.Enabled = false;
                Bei_UnUse.Enabled = false;
                Bei_ShapeRectHeight.Enabled = false;
                Bei_SymbolSize.Enabled = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                IsIgnoreFTAStyleCahnged = false;
            }
        }

        /// <summary>
        /// 根据系统的宽高设置，重置宽高菜单
        /// </summary>
        private void ReSetFTAStyleWidthHeight()
        {
            try
            {
                IsIgnoreFTAStyleCahnged = true;
                if (General.FtaProgram.CurrentSystem != null)
                {
                    //设置图形的默认宽高,文本框，符号高，字体名和大小
                    Bei_ShapeRectWidth.EditValue = General.FtaProgram.CurrentSystem.ShapeWidth;
                    Bei_UnUse.EditValue = General.FtaProgram.CurrentSystem.ShapeIdRectHeight;
                    Bei_ShapeRectHeight.EditValue = General.FtaProgram.CurrentSystem.ShapeDescriptionRectHeight;
                    Bei_SymbolSize.EditValue = General.FtaProgram.CurrentSystem.ShapeSymbolRectHeight;
                    Bei_ShapeRectWidth.Enabled = true;
                    Bei_UnUse.Enabled = true;
                    Bei_ShapeRectHeight.Enabled = true;
                    Bei_SymbolSize.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                IsIgnoreFTAStyleCahnged = false;
            }
        }

        /// <summary>
        /// 画布里图形的描述框的高度，标识框的高度，符号框高度,宽度，字体名，字体大小变化,背景色，选中背景色，重复事件背景色，真假门背景色，割集颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditValueChanged_Ribbon_Start_FTAStyle(object sender, EventArgs e)
        {
            General.TryCatch(() =>
            {
                StyleModel style = General.FtaProgram?.CurrentProject?.Style;
                if (style != null && !IsIgnoreFTAStyleCahnged)
                {
                 //画布里图形里描述框的高度变化
                 if (sender == Bei_ShapeRectHeight)
                    {
                        int? num_New = Ribbon_Start_FTAStyle_ShapeSizeChanged(sender, style.ShapeDescriptionRectHeight);
                        if (num_New != null)
                        {
                            style.ShapeDescriptionRectHeight = (int)num_New;
                            General.FtaProgram.CurrentSystem.ShapeDescriptionRectHeight = (int)num_New;
                            this.ftaDiagram.ResetSetting(ResetType.ShapeHeight);
                        }
                    }
                 //画布里图形里标识框的高度变化
                 else if (sender == Bei_UnUse)
                    {
                        int? num_New = Ribbon_Start_FTAStyle_ShapeSizeChanged(sender, style.ShapeIdRectHeight);
                        if (num_New != null)
                        {
                            style.ShapeIdRectHeight = (int)num_New;
                            General.FtaProgram.CurrentSystem.ShapeIdRectHeight = (int)num_New;
                            this.ftaDiagram.ResetSetting(ResetType.ShapeHeight);
                        }
                    }
                 //画布里图形里符号框高度变化
                 else if (sender == Bei_SymbolSize)
                    {
                        int? num_New = Ribbon_Start_FTAStyle_ShapeSizeChanged(sender, style.ShapeSymbolRectHeight);
                        if (num_New != null)
                        {
                            style.ShapeSymbolRectHeight = (int)num_New;
                            General.FtaProgram.CurrentSystem.ShapeSymbolRectHeight = (int)num_New;
                            this.ftaDiagram.ResetSetting(ResetType.ShapeHeight);
                        }
                    }
                 //画布里图形宽度设置变化
                 else if (sender == Bei_ShapeRectWidth)
                    {
                        int? num_New = Ribbon_Start_FTAStyle_ShapeSizeChanged(sender, style.ShapeWidth);
                        if (num_New != null)
                        {
                            style.ShapeWidth = (int)num_New;
                            General.FtaProgram.CurrentSystem.ShapeWidth = (int)num_New;
                            this.ftaDiagram.ResetSetting(ResetType.ShapeWidth);
                        }
                    }
                 //画布里图形的字体名设置变化
                 else if (sender == Bei_ShapeFontName)
                    {
                        object obj = Bei_ShapeFontName.EditValue;
                        if (obj != null && obj.GetType() == typeof(string))
                        {
                            string font_Name = obj as string;
                         //值不变，不操作
                         if (font_Name == style.ShapeFontName) return;
                            style.ShapeFontName = font_Name;
                            General.DiagramControl.Refresh();
                            return;
                        }
                     //还原之前的值
                     Bei_ShapeFontName.EditValue = style.ShapeFontName;
                    }
                 //画布里图形的字体大小设置变化
                 else if (sender == Bei_ShapeFontSize)
                    {
                        object obj = Bei_ShapeFontSize.EditValue;
                        float value = 0;
                        if (obj != null && float.TryParse(obj.ToString(), out value))
                        {
                         //值不变，不操作
                         if (value == style.ShapeFontSize) return;
                            if (value > 0)
                            {
                                style.ShapeFontSize = value;
                                General.DiagramControl.Refresh();
                                return;
                            }
                        }
                     //还原之前的值
                     Bei_ShapeFontSize.EditValue = style.ShapeFontSize;
                    }
                 //画布里图形的背景色变化
                 else if (sender == barEditItem_ShapeBackColor)
                    {
                        var color_New = Ribbon_Start_FTAStyle_ColorChanged(sender, style.ShapeBackColor);
                        style.ShapeBackColor = color_New;
                        General.DiagramControl.Refresh();
                    }
                 //画布里选中状态下图形的背景色变化
                 else if (sender == barEditItem_ShapeBackColor_Selected)
                    {
                        var color_New = Ribbon_Start_FTAStyle_ColorChanged(sender, style.ShapeBackSelectedColor);
                        style.ShapeBackSelectedColor = color_New;
                        General.DiagramControl.Refresh();
                    }
                 //画布里重复事件图形的背景色变化
                 else if (sender == barEditItem_ShapeBackColor_RepeatEvent)
                    {
                        var color_New = Ribbon_Start_FTAStyle_ColorChanged(sender, style.ShapeBackRepeatEventColor);
                        style.ShapeBackRepeatEventColor = color_New;
                        this.ftaTable.FTATable_StateImage_ResetRepeatedEventImage();
                        General.DiagramControl.Refresh();
                        treeList_FTATable.Refresh();
                    }
                 //画布里真门/事件图形的背景色变化
                 else if (sender == barEditItem_ShapeBackColor_TrueGate)
                    {
                        var color_New = Ribbon_Start_FTAStyle_ColorChanged(sender, style.ShapeBackTrueGateColor);
                        style.ShapeBackTrueGateColor = color_New;
                        General.DiagramControl.Refresh();
                    }
                 //画布里假门/事件图形的背景色变化
                 else if (sender == barEditItem_ShapeBackColor_FalseGate)
                    {
                        var color_New = Ribbon_Start_FTAStyle_ColorChanged(sender, style.ShapeBackFalseGateColor);
                        style.ShapeBackFalseGateColor = color_New;
                        General.DiagramControl.Refresh();
                    }
                 //画布里割集颜色变化
                 else if (sender == barEditItem_CutSetColor)
                    {
                        style.CutSetColor = Ribbon_Start_FTAStyle_ColorChanged(sender, style.CutSetColor);
                        General.DiagramControl.Refresh();
                    }
                }
            });
        }

        private Color Ribbon_Start_FTAStyle_ColorChanged(object sender, Color before)
        {
            return General.TryCatch<Color>(() =>
            {
                var result = Color.Transparent;
                if (sender?.GetType() == typeof(BarEditItem))
                {
                    object obj = ((BarEditItem)sender).EditValue;

                    if (obj?.GetType() == typeof(Color)) result = (Color)obj;
                }
                return result;
            });
        }

        /// <summary>this.General.FtaProgram.Setting
        /// Ribbon-Start-FTAStyle菜单下，图形大小参数变化时的处理函数(正值返回，其他就还原旧值返回null)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="num_Before">旧的大小</param>
        /// <returns>新的大小，如果是null则表示不需要赋值</returns>
        private int? Ribbon_Start_FTAStyle_ShapeSizeChanged(object sender, int num_Before)
        {
            return General.TryCatch<int?>(() =>
            {
                if (sender != null && sender.GetType() == typeof(BarEditItem))
                {
                    object obj = ((BarEditItem)sender).EditValue;
                    int value = 0;
                    if (obj != null && int.TryParse(obj.ToString(), out value))
                    {
                     //值不变，不操作
                     if (value == num_Before) return null;
                        if (value > 0)
                        {
                            return value;
                        }
                    }
                //还原之前的值
                ((BarEditItem)sender).EditValue = num_Before;
                }
                return null;
            });
        }
    }
}
