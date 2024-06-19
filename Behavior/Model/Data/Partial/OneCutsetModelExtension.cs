using FaultTreeAnalysis.Behavior.Enum;
using System;
using System.Collections.Generic;

namespace FaultTreeAnalysis.Model.Data
{
    /// <summary>
    /// OneCutsetModel对象扩展
    ///     用于数据克隆和数据比较
    /// </summary>
    public partial class OneCutsetModel : ObjectExtension
    {

        /// <summary>
        /// 重写当前对象的相等比较
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            //比较对象
            OneCutsetModel oOTher = obj as OneCutsetModel;
            if (null == oOTher)
            {
                throw new ArgumentNullException("Parameter oOTher is null.");
            }

            //浅表对象
            if ((this.szProb != oOTher.szProb)
                || (this.szContri != oOTher.szContri))
            {
                return false;
            }

            //深层对象
            return new Behavior.Common.MultiSetComparer<string>(/*StringComparer.OrdinalIgnoreCase*/)
                .Equals(oOTher.Events, this.Events);
        }

        /// <summary>
        /// 根据克隆动作克隆对象
        /// </summary>
        /// <param name="enCloneAction">克隆动作</param>
        /// <returns>返回对象的克隆体</returns>
        public override object Clone(CloneAction enCloneAction = CloneAction.Shallow)
        {
            //当前对象只适用于浅表克隆

            switch (enCloneAction)
            {
                //浅表克隆
                case CloneAction.Shallow:
                    return ThisShallowCloneAction();
                    //break;
            }

            //
            return base.Clone(enCloneAction);
        }


        /// <summary>
        /// 克隆对象的浅表数据对象体
        /// </summary>
        /// <returns>返回对象的克隆体<</returns>
        private OneCutsetModel ThisShallowCloneAction()
        {
            //克隆对象
            OneCutsetModel oClone = new OneCutsetModel();

            //浅表对象
            oClone.szProb = this.szProb?.Substring(0x00);
            oClone.szContri = this.szContri?.Substring(0x00);

            //深层对象
            if (null != this.Events)
            {
                //创建并复制克隆属性
                oClone.Events = new List<string>(this.Events.Capacity);
                List<string> lstCloneSource = new List<string>(this.Events);

                //遍历被克隆对象数据
                foreach (string item in lstCloneSource)
                {
                    if (null != item)
                    {
                        oClone.Events.Add(item.Substring(0x00));
                    }
                }
            }
            else
            {
                oClone.Events = null;
            }

            //
            return oClone;
        }
    }
}
