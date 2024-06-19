using FaultTreeAnalysis.Behavior.Enum;
using FaultTreeAnalysis.Behavior.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FaultTreeAnalysis.Behavior
{
    /// <summary>
    /// 对象行为
    ///     用于某种对象具有特有的抽象行为
    /// </summary>
    public abstract class ObjectBehavior
        : ICustomClone
        , IRemarkable<ObjectBehavior>
        , IEquatable<ObjectBehavior>
    //, IEqualityComparer<T>
    {
        /// <summary>
        /// 行为记录
        /// </summary>
        [JsonIgnore]
        public HashSet<string> Remarks { get; set; } = new HashSet<string>();

        /// <summary>
        /// 克隆行为标识
        /// </summary>
        public CloneAction CloneAction { get; set; } =  CloneAction.None;



        /// <summary>
        /// 根据克隆动作克隆对象
        /// </summary>
        /// <param name="enCloneAction">克隆动作</param>
        /// <returns>返回对象的克隆体</returns>
        public virtual object Clone(CloneAction enCloneAction)
        {
            return new object();
        }

        /// <summary>
        /// 原始克隆(已被忽略)
        /// </summary>
        /// <returns>返回不支持操作异常</returns>
        public virtual object Clone()
        {
            throw new NotSupportedException("Be Ignored.");
        }

        /// <summary>
        /// 对象比较
        /// </summary>
        /// <param name="other">另一个对象</param>
        /// <returns>true:相等, false:不相等</returns>
        public virtual bool Equals(ObjectBehavior other)
        {
            throw new NotSupportedException("Be Ignored.");
        }


        /// <summary>
        /// 标注
        /// </summary>
        /// <param name="oRemark">标注对象</param>
        /// <returns>受影响数</returns>
        public virtual int Remark(ObjectBehavior oRemark)
        {
            throw new NotImplementedException("Be Ignored.");
        }
    }
}
