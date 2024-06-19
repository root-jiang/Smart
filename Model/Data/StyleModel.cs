using System.Drawing;

namespace FaultTreeAnalysis.Model.Data
{
    /// <summary>
    /// FTA图形样式的设置项目，比如图形宽高，颜色等
    /// </summary>
    public class StyleModel
    {
        /// <summary>
        /// 画布里图形里描述框的高度
        /// </summary>
        public int ShapeDescriptionRectHeight { get; set; }

        /// <summary>
        /// 画布里图形里标识框的高度
        /// </summary>
        public int ShapeIdRectHeight { get; set; }

        /// <summary>
        /// 画布里图形里符号框高度
        /// </summary>
        public int ShapeSymbolRectHeight { get; set; }

        /// <summary>
        /// 画布里图形的宽度
        /// </summary>
        public int ShapeWidth { get; set; }

        /// <summary>
        /// 布局时图形间的空隙
        /// </summary>
        public int ShapeGap { get; set; }

        /// <summary>
        /// 画布里图形里文字字体名
        /// </summary>
        public string ShapeFontName { get; set; }

        /// <summary>
        /// 画布里图形里文字大小
        /// </summary>
        public float ShapeFontSize { get; set; }

        /// <summary>
        /// 画布里图形的背景色0,255,255,255格式的字符串表示
        /// </summary>
        public Color ShapeBackColor { get; set; }

        /// <summary>
        /// 画布里选中状态下图形的背景色0,255,255,255格式的字符串表示
        /// </summary>
        public Color ShapeBackSelectedColor { get; set; }

        /// <summary>
        /// 画布里重复事件图形的背景色0,255,255,255格式的字符串表示
        /// </summary>
        public Color ShapeBackRepeatEventColor { get; set; }

        /// <summary>
        /// 画布里真门/事件图形的背景色0,255,255,255格式的字符串表示
        /// </summary>
        public Color ShapeBackTrueGateColor { get; set; }

        /// <summary>
        /// 画布里假门/事件图形的背景色0,255,255,255格式的字符串表示
        /// </summary>
        public Color ShapeBackFalseGateColor { get; set; }

        /// <summary>
        /// 画布里割集图形的线条色0,255,255,255格式的字符串表示
        /// </summary>
        public Color CutSetColor { get; set; }

        /// <summary>
        /// 初始化设置项的值为默认数值
        /// </summary>
        public StyleModel()
        {
            ShapeDescriptionRectHeight = 36;
            ShapeIdRectHeight = 13;
            ShapeSymbolRectHeight = 50;
            ShapeWidth = 100;
            ShapeGap = 40;
            ShapeFontName = "Arial";
            ShapeFontSize = 8;
            ShapeBackColor = Color.Transparent;
            ShapeBackSelectedColor = Color.LightSlateGray;
            ShapeBackRepeatEventColor = Color.LightSlateGray;
            ShapeBackTrueGateColor = Color.LightSlateGray;
            ShapeBackFalseGateColor = Color.LightSlateGray;
            CutSetColor = Color.Salmon;
        }
    }
}
