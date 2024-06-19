using FaultTreeAnalysis.Behavior.Enum;
using System;
using System.Collections.Generic;

namespace FaultTreeAnalysis.Model.Data
{
    /// <summary>
    /// CutsetModel对象扩展
    ///     用于数据克隆和数据比较
    /// </summary>
    public partial class CutsetModel
        : ObjectExtension
        , IEquatable<CutsetModel>
    {

        /// <summary>
        /// 重写当前对象的相等比较
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool Equals(CutsetModel obj)
        {
            //比较对象
            CutsetModel oOther = obj as CutsetModel;
            if (null == oOther)
            {
                throw new ArgumentNullException("Parameter oOther is null.");
            }

            //深层对象
            return new Behavior.Common.MultiSetComparer<OneCutsetModel>()
                .Equals(oOther.ListCutsets_Real, this.ListCutsets_Real);
        }


        /// <summary>
        /// 根据克隆动作克隆对象
        /// </summary>
        /// <param name="enCloneAction">克隆动作</param>
        /// <returns>返回对象的克隆体</returns>
        public override object Clone(CloneAction enCloneAction = CloneAction.Extreme)
        {
            //当前对象只适合于深层克隆

            switch (enCloneAction)
            {
                //深层克隆
                case CloneAction.Extreme:
                    return ThisExtremeCloneAction();
                    //break;
            }

            //
            return base.Clone(enCloneAction);
        }


        /// <summary>
        /// 克隆对象的深层数据对象体
        /// </summary>
        /// <returns>返回对象的克隆体<</returns>
        private CutsetModel ThisExtremeCloneAction()
        {
            //克隆对象
            CutsetModel oClone = new CutsetModel();

            //深层对象
            if (null != this.ListCutsets_Real)
            {
                //创建并复制克隆属性
                oClone.ListCutsets_Real = new List<OneCutsetModel>(this.ListCutsets_Real.Capacity);
                List<OneCutsetModel> lstCloneSource = new List<OneCutsetModel>(this.ListCutsets_Real);

                //遍历被克隆对象数据
                foreach (OneCutsetModel item in lstCloneSource)
                {
                    oClone.ListCutsets_Real.Add((OneCutsetModel)(item.Clone(CloneAction.Shallow)));
                }
            }
            else
            {
                oClone.ListCutsets_Real = null;
            }

            //
            return oClone;
        }

    }
}
