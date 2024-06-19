using System.Drawing;

namespace FaultTreeAnalysis.Model.Data
{
    /// <summary>
    /// 存放FTA程序所有设置的类，用于保存读取用户设置
    /// </summary>
    public class SettingModel
    {
        /// <summary>
        /// 默认保存路径
        /// </summary>
        public string DefaultFilePath { get; set; }
        /// <summary>
        /// 事件库路径
        /// </summary>
        public string CommonEventLibraryPath { get; set; }
        /// <summary>
        /// 当前设置的皮肤名称
        /// </summary>
        public string Skin_Name { get; set; }

        /// <summary>
        /// 是否显示画布里的标尺
        /// </summary>
        public bool Is_Show_Ruler { get; set; }

        /// <summary>
        /// 是否显示画布里的网格背景
        /// </summary>
        public bool Is_Show_Grid { get; set; }

        /// <summary>
        /// 是否显示画布里分页的虚线
        /// </summary>
        public bool Is_Show_PageBreak { get; set; }

        /// <summary>
        /// 是否总是画布独占panel模式
        /// </summary>
        public bool Is_CanvasFillMode { get; set; }

        /// <summary>
        /// 是按照页滚动模式，还是内容滚动模式
        /// </summary>
        public bool Is_PageScrollMode { get; set; }

        /// <summary>
        /// 画布里线条的色0,255,255,255格式的字符串表示
        /// </summary>
        public Color LineColor { get; set; }

        /// <summary>
        /// 画布里连线样式
        /// </summary>
        public string LineStyle { get; set; }

        /// <summary>
        /// 画布里连线箭头样式
        /// </summary>
        public string ArrowStyle { get; set; }

        /// <summary>
        /// 画布里连线箭头大小
        /// </summary>
        public int ArrowSize { get; set; }

        /// <summary>
        /// 画布里图形/连线是否可移动，旋转
        /// </summary>
        public bool Is_MoveAble { get; set; }

        /// <summary>
        /// 画布里图形/连线是否可以缩放旋转
        /// </summary>
        public bool Is_ScaleAble { get; set; }

        /// <summary>
        /// FTA表里是否显示指示器
        /// </summary>
        public bool Is_ShowIndicator { get; set; }

        /// <summary>
        /// FTA表里指示器里顶级门图形的颜色0,255,255,255格式的字符串表示
        /// </summary>
        public Color FTATableIndicatorTopGateColor { get; set; }

        /// <summary>
        /// FTA表里指示器里转移门图形的颜色0,255,255,255格式的字符串表示
        /// </summary>
        public Color FTATableIndicatorTransInGateColor { get; set; }

        /// <summary>
        /// 当前程序的语言
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// 初始化个字段的默认值,这里加个无用参数，因为json读取文件时调用默认构造函数，而如果这时初始化里存在list会导致list元素增加了
        /// </summary>
        public SettingModel(bool Is_InitDefaultValue)
        {
            Skin_Name = "Office Dark";
            Is_PageScrollMode = true;
            Language = FixedString.LANGUAGE_EN_EN;
            LineColor = Color.LightSlateGray;
            LineStyle = "OrgChart";
            ArrowStyle = "Filled 90 arrow";
            ArrowSize = 0;
            Is_MoveAble = false;
            Is_ScaleAble = true;
            Is_ShowIndicator = false;
            FTATableIndicatorTopGateColor = Color.Gray;
            FTATableIndicatorTransInGateColor = Color.Gray;
            //默认的初始化             
            Is_Show_Ruler = false;
            Is_Show_Grid = false;
            Is_Show_PageBreak = false;
            Is_CanvasFillMode = true;
        }
    }
}
