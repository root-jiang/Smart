using System;

namespace FaultTreeAnalysis.Behavior.Event
{
    /// <summary>
    /// 对象参数
    ///     用于定位对象
    /// </summary>
    public class OperationObjectArgs
        : EventArgs
    {
        /// <summary>
        /// Guid
        /// </summary>
        public Guid? ThisGuid { get; internal set; }

        /// <summary>
        /// 标识
        /// </summary>
        public string Identifier { get; internal set; }
    }

    /// <summary>
    /// 操作对象事件
    /// </summary>
    /// <param name="sender">对象</param>
    /// <param name="e">参数</param>
    public delegate void OperationObjectHandler(object sender, OperationObjectArgs e);
}
