namespace FaultTreeAnalysis.Behavior.Enum
{
    /// <summary>
    /// 克隆动作
    ///     用于自定义克隆对象数据枚举
    /// </summary>
    public enum CloneAction : ushort
    {
        /// <summary>
        /// 无动作
        /// </summary>
        None = 0x00,

        /// <summary>
        /// 影子动作
        /// </summary>
        Shadow = 0x01,

        /// <summary>
        /// 级联动作
        /// </summary>
        Beyond = 0x02,

        /// <summary>
        /// 浅表动作
        /// </summary>
        Shallow = 0x04,

        /// <summary>
        /// 深层动作
        /// </summary>
        Extreme = 0x08,

        /// <summary>
        /// 主要动作
        /// </summary>
        Major = 0x0E, //(Beyond | Shallow | Extreme)

        /// <summary>
        /// 终极动作
        /// </summary>
        Ultimate = 0x0F //(Shadow | Beyond | Shallow | Extreme)
    }
}
