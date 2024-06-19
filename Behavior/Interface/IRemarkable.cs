using System.Collections.Generic;

namespace FaultTreeAnalysis.Behavior.Interface
{
    /// <summary>
    /// 可记录接口
    ///     用于记录某种特殊对象具有的行为
    /// </summary>
    interface IRemarkable<T>
    {
        /// <summary>
        /// 标注记录
        /// </summary>
        HashSet<string> Remarks { get; set; }

        /// <summary>
        /// 标注
        /// </summary>
        /// <param name="oRemark">标注对象</param>
        /// <returns>受影响数</returns>
        int Remark(T oRemark);
    }
}
