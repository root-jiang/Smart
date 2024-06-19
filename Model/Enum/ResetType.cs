namespace FaultTreeAnalysis.Model.Enum
{
    /// <summary>
    /// FTA图专用的，用于变更设置时，当前显示的图形参数也要重新设置
    /// </summary>
    public enum ResetType
    {
        /// <summary>
        /// 图形宽度变化
        /// </summary>
        ShapeWidth,

        /// <summary>
        /// 图形高度变化
        /// </summary>
        ShapeHeight,

        /// <summary>
        /// 图形线条颜色变化
        /// </summary>
        LineColor,

        /// <summary>
        /// 图形线条样式变化
        /// </summary>
        LineStyle,

        /// <summary>
        /// 线条末端箭头样式变化
        /// </summary>
        ArrowStyle,

        /// <summary>
        /// 线条末端箭头大小变化
        /// </summary>
        ArrowSize,

        /// <summary>
        /// 图形，线能否移动变化
        /// </summary>
        MoveAble,

        /// <summary>
        /// 图形,线可缩放变化
        /// </summary>
        ScaleAble
    }
}
