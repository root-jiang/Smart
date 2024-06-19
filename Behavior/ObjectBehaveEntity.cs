using FaultTreeAnalysis.Behavior.Enum;
using FaultTreeAnalysis.Behavior.Event;
using System;
using System.Collections.Generic;

namespace FaultTreeAnalysis.Behavior
{
    /// <summary>
    /// 对象行为单体
    ///     用于某种对象具有特有的抽象行为赋予的参数
    /// </summary>
    public class ObjectBehaveEntity
    {
        /// <summary>
        /// 行为时戳
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;

        /// <summary>
        /// 行为原因
        /// </summary>
        public ElementOperate Cause { get; set; }

        /// <summary>
        /// 目标项
        /// </summary>
        public BehaveOperationArgs Objective { get; set; }

        /// <summary>
        /// 目标附加项
        /// </summary>
        //public List<BehaveOperationArgs> ObjectiveAttachments { get; set; }

        /// <summary>
        /// 依赖项
        /// </summary>
        public BehaveOperationArgs Dependency { get; set; }

        /// <summary>
        /// 先前行为实体
        /// </summary>
        public ObjectBehavior PreviousBehavorObject { get; set; }

        /// <summary>
        /// 当前行为实体
        /// </summary>
        public ObjectBehavior PresentBehavorObject { get; set; }

        /// <summary>
        /// 附加项
        /// </summary>
        public List<BehaveOperationArgs> Attachments { get; set; }

        /// <summary>
        /// 行为影响
        /// </summary>
        public ElementOperate Effect { get; set; }


        /// <summary>
        /// 更新时间戳
        /// </summary>
        public void Update()
        {
            this.Timestamp = DateTime.Now;
        }
    }
}
