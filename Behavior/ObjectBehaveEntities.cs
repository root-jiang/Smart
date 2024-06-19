using FaultTreeAnalysis.Behavior.Enum;
using System;
using System.Collections.Generic;

namespace FaultTreeAnalysis.Behavior
{
    /// <summary>
    /// 对象行为复体
    ///     用于某种对象具有特有的抽象行为赋予的参数
    /// </summary>
    public class ObjectBehaveEntities
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
        /// 先前行为实体
        /// </summary>
        public List<ObjectBehaveMeta> PreviousBehavorObjects { get; set; }

        /// <summary>
        /// 当前行为实体
        /// </summary>
        public List<ObjectBehaveMeta> PresentBehavorObjects { get; set; }

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
