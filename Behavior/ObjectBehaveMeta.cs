using FaultTreeAnalysis.Behavior.Event;
using System.Collections.Generic;

namespace FaultTreeAnalysis.Behavior
{
    /// <summary>
    /// 对象行为元
    ///     用于某种行为对象相关联元表
    /// </summary>
    public class ObjectBehaveMeta
    {
        /// <summary>
        /// 依赖项
        /// </summary>
        public BehaveOperationArgs Dependency { get; set; }

        /// <summary>
        /// 行为对象
        /// </summary>
        public ObjectBehavior BehavorObject { get; set; }

        /// <summary>
        /// 附加项
        /// </summary>
        public List<BehaveOperationArgs> Attachments { get; set; }
    }
}
