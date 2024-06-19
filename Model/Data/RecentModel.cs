using System.Collections.Generic;

namespace FaultTreeAnalysis.Model.Data
{
    public class RecentModel
    {
        /// <summary>
        /// 最近工程
        /// </summary>
        public Dictionary<string, string> RecentProject { get; set; }

        /// <summary>
        /// 最近故障树
        /// </summary>
        public Dictionary<string, string> RecentFaultTree { get; set; }
    }
}
