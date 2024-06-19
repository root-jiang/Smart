namespace FaultTreeAnalysis.Behavior.Enum
{
    /// <summary>
    /// 属性状态
    ///     用于属性状态分析
    /// </summary>
    public enum PropertyStatus
    {
        /// <summary>
        /// 无状态
        /// </summary>
        None = 0x00,

        /// <summary>
        /// 已应用
        /// </summary>
        Applied = 0x01,

        /// <summary>
        /// 未应用
        /// </summary>
        NoApply = 0x02,
    }
}
