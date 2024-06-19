namespace FaultTreeAnalysis.Behavior.Enum
{
    /// <summary>
    /// 元素操作
    ///     用于对象数据元素操作枚举
    /// </summary>
    public enum ElementOperate
    {
        /// <summary>
        /// 无操作
        /// </summary>
        None = 0x00,

        /// <summary>
        /// 创建元素
        /// </summary>
        Creation = 0x01,

        /// <summary>
        /// 添加元素
        /// </summary>
        Add = 0x02,

        /// <summary>
        /// 元素移动
        /// </summary>
        Move = 0x04,

        /// <summary>
        /// 元素属性变更
        /// </summary>
        AlterProperty = 0x08,

        /// <summary>
        /// 移除元素
        /// </summary>
        Remove = 0x10,

        /// <summary>
        /// 删除元素
        /// </summary>
        Deletion = 0x20,
    }
}
