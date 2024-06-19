using DevExpress.Diagram.Core;
using DevExpress.XtraBars;
using FaultTreeAnalysis.Common;
using FaultTreeAnalysis.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaultTreeAnalysis
{
    partial class FTAControl
    {
        private BarCheckItem GetCheckItem(string controlName)
        {
            return General.BarCheckItems[controlName];
        }

        /// <summary>
        /// 初始化Ribbon-Setting-Canvas下的菜单
        /// </summary>
        private void Init_Ribbon_Setting_Canvas()
        {
            //设置画布是否显示标尺
            this.GetCheckItem(nameof(FTAControl.Bci_Ruler)).CheckedChanged += CheckedChanged_Ribbon_Setting_Canvas;

            //是否显示画布网格
            Bci_Grid.CheckedChanged += CheckedChanged_Ribbon_Setting_Canvas;

            //是否显示分页虚线
            Bci_PageBreak.CheckedChanged += CheckedChanged_Ribbon_Setting_Canvas;

            //是否总是画布独占模式
            Bci_CanvasFillMode.CheckedChanged += CheckedChanged_Ribbon_Setting_Canvas;

            //是按照页滚动模式，还是内容滚动模式
            Bci_PageScrollMode.CheckedChanged += CheckedChanged_Ribbon_Setting_Canvas;

            //线条颜色变化
            barEditItem_LineColor.EditValueChanged += EditValueChanged_Ribbon_Setting_Canvas;

            //添加可选的连线类型和连线的箭头样式
            List<string> lines = ConnectorType.RegisteredTypes.ToList().Select(obj => obj.TypeName).ToList();
            List<string> arrows = ArrowDescriptions.Arrows.ToList().Select(obj => obj.Name).ToList();
            repositoryItemComboBox_LineStyle.Items.AddRange(lines);
            repositoryItemComboBox_ArrowStyle.Items.AddRange(arrows);

            //切换线条样式
            barEditItem_LineStyle.EditValueChanged += EditValueChanged_Ribbon_Setting_Canvas;

            //切换箭头样式
            barEditItem_ArrowStyle.EditValueChanged += EditValueChanged_Ribbon_Setting_Canvas;

            //设置箭头大小
            barEditItem_ArrowSize.EditValueChanged += EditValueChanged_Ribbon_Setting_Canvas;
              
            //是否可缩放
            Bci_ScaleAble.CheckedChanged += CheckedChanged_Ribbon_Setting_Canvas;
        }

        /// <summary>
        /// 线条颜色变化，线条样式，箭头样式，箭头大小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditValueChanged_Ribbon_Setting_Canvas(object sender, EventArgs e)
        {
            General.TryCatch(() =>
            {
                //线条颜色变化
                if (sender == barEditItem_LineColor)
                {
                    var color_New = Ribbon_Start_FTAStyle_ColorChanged(sender, General.FtaProgram.Setting.LineColor);
                    General.FtaProgram.Setting.LineColor = color_New;
                    this.ftaDiagram.ResetSetting(ResetType.LineColor);
                }
                //线条样式
                else if (sender == barEditItem_LineStyle)
                {
                    if (barEditItem_LineStyle.EditValue != null && barEditItem_LineStyle.EditValue.GetType() == typeof(string))
                    {
                        string style = barEditItem_LineStyle.EditValue as string;
                        if (General.FtaProgram.Setting.LineStyle != style)
                        {
                            General.FtaProgram.Setting.LineStyle = style;
                            this.ftaDiagram.ResetSetting(ResetType.LineStyle);
                        }
                    }
                }
                //箭头样式
                else if (sender == barEditItem_ArrowStyle)
                {
                    if (barEditItem_ArrowStyle.EditValue != null && barEditItem_ArrowStyle.EditValue.GetType() == typeof(string))
                    {
                        string style = barEditItem_ArrowStyle.EditValue as string;
                        if (General.FtaProgram.Setting.ArrowStyle != style)
                        {
                            General.FtaProgram.Setting.ArrowStyle = style;
                            this.ftaDiagram.ResetSetting(ResetType.ArrowStyle);
                        }
                    }
                }
                //箭头大小
                else if (sender == barEditItem_ArrowSize)
                {
                    int newSize = 0;
                    if (barEditItem_ArrowSize.EditValue != null && int.TryParse(barEditItem_ArrowSize.EditValue.ToString(), out newSize))
                    {
                        if (General.FtaProgram.Setting.ArrowSize != newSize)
                        {
                            General.FtaProgram.Setting.ArrowSize = newSize;
                            this.ftaDiagram.ResetSetting(ResetType.ArrowSize);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 设置画布是否显示标尺,是否显示画布网格,是否显示分页虚线,是否总是画布独占模式,是按照页滚动模式/内容滚动模式,是否可以动，缩放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckedChanged_Ribbon_Setting_Canvas(object sender, ItemClickEventArgs e)
        {
            General.TryCatch(() =>
            {
                //是否显示画布标尺
                if (e.Item.Name == nameof(FTAControl.Bci_Ruler))
                {
                    General.DiagramControl.OptionsView.ShowRulers = this.GetCheckItem(nameof(FTAControl.Bci_Ruler)).Checked;
                    if (General.FtaProgram.Setting.Is_Show_Ruler != Bci_Ruler.Checked) General.FtaProgram.Setting.Is_Show_Ruler = this.GetCheckItem(nameof(FTAControl.Bci_Ruler)).Checked;
                }
                //是否显示画布网格
                else if (sender == Bci_Grid)
                {
                    General.DiagramControl.OptionsView.ShowGrid = Bci_Grid.Checked;
                    if (General.FtaProgram.Setting.Is_Show_Grid != Bci_Grid.Checked) General.FtaProgram.Setting.Is_Show_Grid = Bci_Grid.Checked;
                }
                //是否显示分页虚线
                else if (sender == Bci_PageBreak)
                {
                    General.DiagramControl.OptionsView.ShowPageBreaks = Bci_PageBreak.Checked;
                    if (General.FtaProgram.Setting.Is_Show_PageBreak != Bci_PageBreak.Checked) General.FtaProgram.Setting.Is_Show_PageBreak = Bci_PageBreak.Checked;
                }
                //是否总是画布独占模式
                else if (sender == Bci_CanvasFillMode)
                {
                    if (Bci_CanvasFillMode.Checked) General.DiagramControl.OptionsView.CanvasSizeMode = DevExpress.Diagram.Core.CanvasSizeMode.Fill;
                    else General.DiagramControl.OptionsView.CanvasSizeMode = DevExpress.Diagram.Core.CanvasSizeMode.AutoSize;
                    if (General.FtaProgram.Setting.Is_CanvasFillMode != Bci_CanvasFillMode.Checked) General.FtaProgram.Setting.Is_CanvasFillMode = Bci_CanvasFillMode.Checked;
                }
                //是按照页滚动模式/内容滚动模式
                else if (sender == Bci_PageScrollMode)
                {
                    if (Bci_PageScrollMode.Checked) General.DiagramControl.OptionsBehavior.ScrollMode = DevExpress.Diagram.Core.DiagramScrollMode.Page;
                    else General.DiagramControl.OptionsBehavior.ScrollMode = DevExpress.Diagram.Core.DiagramScrollMode.Content;
                    if (General.FtaProgram.Setting.Is_PageScrollMode != Bci_PageScrollMode.Checked) General.FtaProgram.Setting.Is_PageScrollMode = Bci_PageScrollMode.Checked;
                } 
                //是否可缩放
                else if (sender == Bci_ScaleAble)
                {
                    if (General.FtaProgram.Setting.Is_ScaleAble != Bci_ScaleAble.Checked)
                    {
                        General.FtaProgram.Setting.Is_ScaleAble = Bci_ScaleAble.Checked;
                        this.ftaDiagram.ResetSetting(ResetType.ScaleAble);
                    }
                }
            });
        }
    }
}
