using System.Collections.Generic;

namespace FaultTreeAnalysis.Behavior.Interface
{
    /// <summary>
    /// 可溯原性接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// 1. 操作Redo
    ///     1.1 从RedoHistory中取出RedoTop
    ///     1.2 将RedoTop.NewValue赋值给对象当前值
    ///     1.3 将RedoTop入Undo栈
    /// 2. 操作Undo
    ///     2.1 从UndoHistory中取出UndoTop
    ///     2.2 将UndoTop.OldValue赋值给对象当前值
    ///     2.3 将UndoTop入Redo栈
    /// 3. 对象值改变
    ///     3.1 将对象新值, 当前值保存入Undo栈
    /// </remarks>
    public interface IRetrospective<T>
    {
        /// <summary>
        /// 重做历史
        /// </summary>
        Stack<T> RedoHistory { get; set; }

        /// <summary>
        /// 撤销历史
        /// </summary>
        Stack<T> UndoHistory { get; set; }

        /// <summary>
        /// 重做
        /// </summary>
        /// <returns>true: 成功, false: 失败</returns>
        bool Redo();

        /// <summary>
        /// 撤销
        /// </summary>
        /// <returns>true: 成功, false: 失败</returns>
        bool Undo();
    }
}
