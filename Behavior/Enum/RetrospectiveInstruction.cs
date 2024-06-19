namespace FaultTreeAnalysis.Behavior.Enum
{
    /// <summary>
    /// 追溯指示
    ///     用于追溯行为方向
    /// </summary>
    public enum RetrospectiveInstruction
    {
        /// <summary>
        /// 无指示
        /// </summary>
        None = 0x00,

        /// <summary>
        /// 重做指示
        /// </summary>
        Redo = 0x01,

        /// <summary>
        /// 撤销指示
        /// </summary>
        Undo = 0x02,
    }
}
