
using System.Collections.Generic;

namespace FaultTreeAnalysis.Model.Data
{
    /// <summary>
    /// 割集类
    /// </summary>
    public partial class CutsetModel
    {
        /// <summary>
        /// 初始化ListCutsets
        /// </summary>
        public CutsetModel()
        {
            ListCutsets = new List<OneCutsetModel>();
            ListCutsets_Real = new List<OneCutsetModel>();
        }

        /// <summary>
        /// 割集,事件名称的集合
        /// </summary>
        public List<OneCutsetModel> ListCutsets;

        /// <summary>
        /// 割集,事件名称的集合-真
        /// </summary>
        public List<OneCutsetModel> ListCutsets_Real;

        /// <summary>
        /// 向割集列表里添加新的割集对象
        /// </summary>
        /// <param name="cutset">要添加的割集对象</param>
        public void AddOneCutset(OneCutsetModel cutset)
        {
            if (cutset == null)
                return;
            ListCutsets.Add(cutset);
        }

        /// <summary>
        /// 向割集列表里添加新的割集对象-真
        /// </summary>
        /// <param name="cutset">要添加的割集对象</param>
        public void AddOneCutset_Real(OneCutsetModel cutset)
        {
            if (cutset == null)
                return;
            ListCutsets_Real.Add(cutset);
        }

        /// <summary>
        /// 清空割集列表
        /// </summary>
        public void Clear()
        {
            ListCutsets = new List<OneCutsetModel>();
            ListCutsets_Real = new List<OneCutsetModel>();
        }
    }
}
