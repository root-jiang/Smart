using System;

namespace FaultTreeAnalysis.Behavior.Interface
{
    /// <summary>
    /// 自定义克隆接口
    ///     用于条件性克隆某对象的接口
    /// </summary>
    interface ICustomClone : ICloneable
    {
        /// <summary>
        /// 克隆行为标识
        /// </summary>
        Enum.CloneAction CloneAction { get; set; }

        /// <summary>
        /// 根据克隆动作克隆对象
        /// </summary>
        /// <param name="enCloneAction">克隆动作</param>
        /// <returns>返回对象的克隆体</returns>
        object Clone(FaultTreeAnalysis.Behavior.Enum.CloneAction enCloneAction);
    }
}
