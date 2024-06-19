using FaultTreeAnalysis.Behavior.Enum;
using System;

namespace FaultTreeAnalysis.Behavior.Event
{
    /// <summary>
    /// 行为追溯事件参数
    ///     用于对象追溯条件
    /// </summary>
    public class RetrospectiveArgs
        : EventArgs
    {
        /// <summary>
        /// 依赖对象
        /// </summary>
        public OperationObjectArgs DependencyObject { get; set; }

        /// <summary>
        /// 当前对象
        /// </summary>
        public OperationObjectArgs PresentObject { get; set; }

        /// <summary>
        /// 追溯指示
        /// </summary>
        public RetrospectiveInstruction Instruction { get; set; }

        /// <summary>
        /// 行为原因
        /// </summary>
        public ElementOperate Cause { get; set; }

        /// <summary>
        /// 行为影响
        /// </summary>
        public ElementOperate Effect { get; set; }
    }

    /// <summary>
    /// 行为追溯事件
    /// </summary>
    /// <param name="sender">对象</param>
    /// <param name="e">参数</param>
    public delegate void RetrospectiveHandler(object sender, RetrospectiveArgs e);
}
