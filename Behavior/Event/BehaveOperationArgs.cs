namespace FaultTreeAnalysis.Behavior.Event
{
    /// <summary>
    /// 行为操作参数
    ///     用于对象特殊集合行为参数
    /// </summary>
    public class BehaveOperationArgs
        : OperationObjectArgs
    {
        /// <summary>
        /// 先前索引
        /// </summary>
        public int IndexPrevious { get; internal set; }

        /// <summary>
        /// 当前索引
        /// </summary>
        public int IndexPresent { get; internal set; }
    }

    /// <summary>
    /// 行为操作事件
    /// </summary>
    /// <param name="sender">对象</param>
    /// <param name="e">参数</param>
    public delegate void BehaveOperationHandler(object sender, BehaveOperationArgs e);
}
