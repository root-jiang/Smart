using FaultTreeAnalysis.Model.Data;
using System.Collections.Generic;

namespace FaultTreeAnalysis
{
    /// <summary>
    /// 高亮割集功能，用到的数据对象
    /// </summary>
    public class HighLightCutSet
    {
        /// <summary>
        /// 当前的割集对象
        /// </summary>
        public OneCutsetModel Cutset { get; set; }

        /// <summary>
        /// 要高亮的DrawData数据对象的集合
        /// </summary>
        public HashSet<DrawData> HighLightData { get; set; }

    }
}
