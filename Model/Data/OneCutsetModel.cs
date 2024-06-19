using System.Collections.Generic;
using System.Linq;

namespace FaultTreeAnalysis.Model.Data
{
    /// <summary>
    /// 单个割集对象
    /// </summary>
    public partial class OneCutsetModel
    {
        /// <summary>
        /// 割集概率
        /// </summary>
        public string szProb;

        /// <summary>
        /// Contri(不知道什么，算法自动生成的，先预留)
        /// </summary>
        public string szContri;

        /// <summary>
        /// 初始化Events
        /// </summary>
        public OneCutsetModel()
        {
            szProb = "";
            szContri = "";
            Events = new List<string>();
        }

        /// <summary>
        /// 一个割集由一个或多个事件组成，事件的id集合
        /// </summary>
        public List<string> Events;
        //List<DrawData> 

        /// <summary>
        /// 把事件id加到集合里
        /// </summary>
        /// <param name="EventName">事件id</param>
        public void Add(string EventName)
        {
            Events.Add(EventName);
        }

        public CutsetInfo GetInfo(List<DrawData> drawData)
        {
            List<string> descriptions = new List<string>();
            foreach (string item in this.Events)
            {
                DrawData current = drawData.FirstOrDefault(o => o.Identifier == item);
                if (current != null) descriptions.Add(current.Comment1);
            }
            string eventNames = string.Join(",", descriptions);
            return new CutsetInfo(eventNames, this.szProb);
        }

        public List<CutsetInfo> GetInfos(List<DrawData> drawData)
        {
            List<CutsetInfo> result = new List<CutsetInfo>(drawData.Count);
            foreach (string item in this.Events)
            {
                DrawData current = drawData.FirstOrDefault(o => o.Identifier == item);
                if (current != null) result.Add(new CutsetInfo(current.Comment1, this.szProb));
            }
            return result;
        }
    }
}
