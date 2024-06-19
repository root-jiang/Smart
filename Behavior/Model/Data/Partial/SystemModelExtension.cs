#define _FTA_DEGUB_
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Extensions;
using FaultTreeAnalysis.Behavior;
using FaultTreeAnalysis.Behavior.Enum;
using FaultTreeAnalysis.Behavior.Event;
using Newtonsoft.Json;

namespace FaultTreeAnalysis.Model.Data
{
    /// <summary>
    /// SystemModel对象扩展
    ///     用于系统模型数据的行为和状态追溯包含
    ///     1.数据的重做和恢复
    ///     2.数据变化通知
    /// </summary>
    public partial class SystemModel
        : Behavior.Interface.IRetrospective<ObjectBehaveEntity>
        , System.ComponentModel.INotifyPropertyChanged
    {

        /// <summary>
        /// 系统的虚拟化系统名字
        /// </summary>
        [JsonIgnore]
        public string VirtualizeSystemName { get { return string.Concat("*", SystemName); } }

        /// <summary>
        /// 系统的虚拟化状态
        /// </summary>
        [JsonIgnore]
        public bool Virtualized { get; internal protected set; } = false;

        /// <summary>
        /// 重做历史
        /// </summary>
        [JsonIgnore]
        public Stack<ObjectBehaveEntity> RedoHistory { get; set; } = new Stack<ObjectBehaveEntity>();
        //ObservableCollection
        /// <summary>
        /// 撤销历史
        /// </summary>
        [JsonIgnore]
        public Stack<ObjectBehaveEntity> UndoHistory { get; set; } = new Stack<ObjectBehaveEntity>();

        /// <summary>
        /// 行为追溯完成事件
        /// </summary>
        public event RetrospectiveHandler RetrospectCompleted;

        /// <summary>
        /// 对象属性改变事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;



        /// <summary>
        /// 项目模型虚拟化应用
        /// </summary>
        public void ApplyVirtualization()
        {
            //外因触发属性变化事件
            OnObjectPropertyChanged(false);
        }

        /// <summary>
        /// 手动触发属性改变事件
        /// </summary>
        /// <param name="bVirtualized">虚拟化状态</param>
        public void ManualFiredPropertyChangedEvent(bool bVirtualized)
        {
            //外因触发属性变化事件
            OnObjectPropertyChanged(bVirtualized);
        }


        /// <summary>
        /// 当前系统实现重做
        /// </summary>
        /// <returns>true: 成功, false: 失败</returns>
        public virtual bool Redo()
        {
            //检查
            if (null == RedoHistory)
            {
                throw new ArgumentNullException("Parameter RedoHistory is null.");
            }
            //无历史记录
            if (0x00 >= RedoHistory.Count)
            {
                return false;
            }

            //应该收缩撤销历史
            if (0x01 == RedoHistory.Count)
            {
                //收缩条件为: 最后一个重做对象之前的撤销对象
            }

            //取出栈顶对象
            ObjectBehaveEntity oBehaveEntity = RedoHistory.Pop();

            //检查
            if (null == oBehaveEntity)
            {
                return false;
            }

            //回收对象
            if (null != UndoHistory)
            {
                UndoHistory.Push(oBehaveEntity);
            }

            //处理对象
            bool bResult = RedoHandle(oBehaveEntity);

            //
            if (bResult)
            {
                //触发行为追溯完成事件
                OnObjectRetrospectCompleted(RetrospectiveInstruction.Redo, oBehaveEntity);

                //内因触发属性变化事件
                OnObjectPropertyChanged();
            }

            //
            return bResult;
        }
        //重做处理
        private bool RedoHandle(ObjectBehaveEntity oEntity)
        {
            //检查
            if (null == oEntity)
            {
                throw new ArgumentNullException("Parameter oEntity is null.");
            }

            //根据行为影响做还原

            //属性修改
            if (Behavior.Enum.ElementOperate.AlterProperty == (oEntity.Cause & Behavior.Enum.ElementOperate.AlterProperty))
            {
                //
                return SingleObjectBehavorHandleAlter(
                    oEntity.PresentBehavorObject,
                    oEntity.PreviousBehavorObject,
                    oEntity.PreviousBehavorObject?.Remarks);
            }

            //元素创建
            if (Behavior.Enum.ElementOperate.Creation == (oEntity.Cause & Behavior.Enum.ElementOperate.Creation))
            {
                //
                return SingleObjectBehavorHandleInclude(
                    oEntity.PresentBehavorObject,
                    oEntity.Dependency,
                    oEntity.Attachments);
            }

            //元素移除
            if (Behavior.Enum.ElementOperate.Remove == (oEntity.Cause & Behavior.Enum.ElementOperate.Remove))
            {
                //
                return SingleObjectBehavorHandleExclude(
                    oEntity.PresentBehavorObject,
                    oEntity.Dependency,
                    oEntity.Attachments);
            }

            //元素移动
            if (Behavior.Enum.ElementOperate.Move == (oEntity.Effect & Behavior.Enum.ElementOperate.Move))
            {
                //
                return SingleObjectBehavorHandleMove(
                    oEntity.PresentBehavorObject,
                    oEntity.Dependency,
                    oEntity.Attachments,
                    oEntity.Objective,
                    null);//目标附加项
            }

            //除非过程出现有误,否则返回操作无误
            return true;
        }

        /// <summary>
        /// 当前系统实现撤销
        /// </summary>
        /// <returns>true: 成功, false: 失败</returns>
        public virtual bool Undo()
        {
            //检查
            if (null == UndoHistory)
            {
                throw new ArgumentNullException("Parameter UndoHistory is null.");
            }
            //无历史记录
            if (0x00 >= UndoHistory.Count)
            {
                return false;
            }

            //取出栈顶对象
            ObjectBehaveEntity oBehaveEntity = UndoHistory.Pop();

            //检查
            if (null == oBehaveEntity)
            {
                return false;
            }

            //回收对象
            if (null != RedoHistory)
            {
                RedoHistory.Push(oBehaveEntity);
            }

            //处理对象
            bool bResult = UndoHandle(oBehaveEntity);

            //
            if (bResult)
            {
                //触发行为追溯完成事件
                OnObjectRetrospectCompleted(RetrospectiveInstruction.Undo, oBehaveEntity);

                //内因触发属性变化事件
                OnObjectPropertyChanged();
            }

            //
            return bResult;
        }
        //撤销处理
        private bool UndoHandle(ObjectBehaveEntity oEntity)
        {
            //检查
            if (null == oEntity)
            {
                throw new ArgumentNullException("Parameter oEntity is null.");
            }

            //根据行为影响做还原

            //属性修改
            if (Behavior.Enum.ElementOperate.AlterProperty == (oEntity.Effect & Behavior.Enum.ElementOperate.AlterProperty))
            {
                //
                return SingleObjectBehavorHandleAlter(
                    oEntity.PreviousBehavorObject,
                    oEntity.PresentBehavorObject,
                    oEntity.PreviousBehavorObject?.Remarks);
            }

            //元素删除
            if (Behavior.Enum.ElementOperate.Deletion == (oEntity.Effect & Behavior.Enum.ElementOperate.Deletion))
            {
                //
                return SingleObjectBehavorHandleExclude(
                    oEntity.PresentBehavorObject,
                    oEntity.Dependency,
                    oEntity.Attachments);
            }

            //元素添加
            if (Behavior.Enum.ElementOperate.Add == (oEntity.Effect & Behavior.Enum.ElementOperate.Add))
            {
                //
                return SingleObjectBehavorHandleInclude(
                    oEntity.PresentBehavorObject,
                    oEntity.Dependency,
                    oEntity.Attachments);
            }

            //元素移动
            if (Behavior.Enum.ElementOperate.Move == (oEntity.Effect & Behavior.Enum.ElementOperate.Move))
            {
                //
                return SingleObjectBehavorHandleMove(
                    oEntity.PresentBehavorObject,
                    oEntity.Objective,
                    null,//依赖附加项
                    oEntity.Dependency,
                    oEntity.Attachments);
            }

            //除非过程出现有误,否则返回操作无误
            return true;
        }



        /// <summary>
        /// 单行为对象变化处理(移动)
        /// </summary>
        /// <param name="oPrototypeBehavior">原型行为对象</param>
        /// <param name="oBehaviorDependency">行为依赖参数</param>
        /// <param name="lstBehaviorDependencyAttachments">行为依赖附加参数</param>
        /// <param name="oBehaviorObjective">行为目标参数</param>
        /// <param name="lstBehaviorObjectiveAttachments">行为目标附加参数</param>
        /// <returns>处理结果true处理成功, 否则失败</returns>
        protected virtual bool SingleObjectBehavorHandleMove(
            ObjectBehavior oPrototypeBehavior,
            BehaveOperationArgs oBehaviorDependency,
            List<BehaveOperationArgs> lstBehaviorDependencyAttachments,
            BehaveOperationArgs oBehaviorObjective,
            List<BehaveOperationArgs> lstBehaviorObjectiveAttachments)
        {
            //检查原型行为对象
            DrawData oPrototypeBehaviorObject = oPrototypeBehavior as DrawData;
            if (null == oPrototypeBehaviorObject)
            {
                throw new ArgumentNullException("Parameter oPrototypeBehaviorObject is null.");
            }
            if (null == oBehaviorDependency)
            {
                throw new ArgumentNullException("Parameter oBehaviorDependency is null.");
            }
            if (null == oBehaviorObjective)
            {
                throw new ArgumentNullException("Parameter oBehaviorObjective is null.");
            }

            //查找原型行为对象对应的原型对象(从当前系统)
            DrawData oPrototypeObject = FindDrawDataBy((o) => { return o.ThisGuid == oPrototypeBehaviorObject.ThisGuid; });
            //检查原型对象
            if (null == oPrototypeObject)
            {
                throw new ArgumentNullException("Parameter oPrototypeObject is null.");
            }

            //1.依赖

            //依赖对象
            DrawData oDependObject = null;
            if (null != oBehaviorDependency)
            {
                //无依赖
                if ((null == oBehaviorDependency.ThisGuid)
                    && (string.IsNullOrEmpty(oBehaviorDependency.Identifier)))
                {
                    oDependObject = null;
                }
                else
                {
                    //尝试查找依赖(从当前系统)
                    oDependObject = FindDrawDataBy(
                        new DrawData() { ThisGuid = (Guid)oBehaviorDependency.ThisGuid },
                        (first, second) => { return first?.ThisGuid == second?.ThisGuid; }
                        );
                }
            }
            //依赖附加对象集合
            List<DrawData> lstAttachDependencyObjects = null;
            if ((null != lstBehaviorDependencyAttachments)
                && (0x00 < lstBehaviorDependencyAttachments.Count))
            {
                //有附加
                lstAttachDependencyObjects = new List<DrawData>(lstBehaviorDependencyAttachments.Count);

                //附加项GUID集合
                List<Guid> lstGuidMap = lstBehaviorDependencyAttachments.Select(o => (Guid)o?.ThisGuid).Where(o => (null != o)).ToList();

                //获取附加对象集合
                if (null != oDependObject)
                {
                    //从依赖项
                    oDependObject.FindDrawDataBy(lstAttachDependencyObjects, (o) => { return lstGuidMap.Contains<Guid>((Guid)o.ThisGuid); });
                }
                else
                {
                    //从当前系统
                    ThisFindDrawDataBy(lstAttachDependencyObjects, (o) => { return lstGuidMap.Contains<Guid>((Guid)o.ThisGuid); });
                }
            }

            //2.目标

            //目标对象
            DrawData oObjectiveObject = null;
            if (null != oBehaviorObjective)
            {
                //无目标
                if ((null == oBehaviorObjective.ThisGuid)
                    && (string.IsNullOrEmpty(oBehaviorObjective.Identifier)))
                {
                    oObjectiveObject = null;
                }
                else
                {
                    //尝试查找目标(从当前系统)
                    oObjectiveObject = FindDrawDataBy(
                        new DrawData() { ThisGuid = (Guid)oBehaviorObjective.ThisGuid },
                        (first, second) => { return first?.ThisGuid == second?.ThisGuid; }
                        );
                }
            }
            //目标附加对象集合
            List<DrawData> lstAttachObjectiveObjects = null;
            if ((null != lstBehaviorObjectiveAttachments)
                && (0x00 < lstBehaviorObjectiveAttachments.Count))
            {
                //有附加
                lstAttachObjectiveObjects = new List<DrawData>(lstBehaviorObjectiveAttachments.Count);

                //附加项GUID集合
                List<Guid> lstGuidMap = lstBehaviorObjectiveAttachments.Select(o => (Guid)o?.ThisGuid).Where(o => (null != o)).ToList();

                //获取附加对象集合
                if (null != oObjectiveObject)
                {
                    //从目标项
                    oObjectiveObject.FindDrawDataBy(lstAttachObjectiveObjects, (o) => { return lstGuidMap.Contains<Guid>((Guid)o.ThisGuid); });
                }
                else
                {
                    //从当前系统
                    ThisFindDrawDataBy(lstAttachObjectiveObjects, (o) => { return lstGuidMap.Contains<Guid>((Guid)o.ThisGuid); });
                }
            }

            //3.从依赖向目标的元素移动操作

            //无依赖对象
            if (null == oDependObject)
            {
                //有依赖附加
                if ((null != lstAttachDependencyObjects)
                    && (0x00 < lstAttachDependencyObjects.Count))
                {
                    //对每个附加项
                    foreach (DrawData item in lstAttachDependencyObjects)
                    {
                        //将附加项追加到当前系统
                        ThisAppend(item);

                        //将附加项从原型对象中移除
                        oPrototypeObject.ThisRemove(item);

                        //修改隶属关系
                        item.ThisAffiliation(null);
                    }
                }

                //无目标对象
                if (null == oObjectiveObject)
                {
                    //有目标附加
                    if ((null != lstAttachObjectiveObjects)
                        && (0x00 < lstAttachObjectiveObjects.Count))
                    {
                        //对每个附加项
                        foreach (DrawData item in lstAttachObjectiveObjects)
                        {
                            //将附加项追加到原型对象
                            oPrototypeObject.ThisAppend(item);

                            //将附加项从当前系统中移除
                            ThisRemove(item);

                            //修改隶属关系
                            item.ThisAffiliation(oPrototypeObject);
                        }
                    }

                    //将原型对象从当前系统中移除(依赖)
                    ThisRemove(oPrototypeObject);
                    //将原型对象插入到当前系统中(目标)
                    ThisInsertAt(oPrototypeObject, oBehaviorObjective.IndexPresent);

                    //修改隶属关系(原型对象)
                    oPrototypeObject.ThisAffiliation(null);
                }
                //有目标对象
                else
                {
                    //有目标附加
                    if ((null != lstAttachObjectiveObjects)
                        && (0x00 < lstAttachObjectiveObjects.Count))
                    {
                        //对每个附加项
                        foreach (DrawData item in lstAttachObjectiveObjects)
                        {
                            //将附加项追加到原型对象
                            oPrototypeObject.ThisAppend(item);

                            //将附加项从目标对象中移除
                            oObjectiveObject.ThisRemove(item);

                            //修改隶属关系
                            item.ThisAffiliation(oPrototypeObject);
                        }
                    }

                    //将原型对象从当前系统中移除(依赖)
                    ThisRemove(oPrototypeObject);

                    //将原型对象插入到目标对象中
                    oObjectiveObject.ThisInsertAt(oPrototypeObject, oBehaviorObjective.IndexPresent);

                    //修改隶属关系(原型对象)
                    oPrototypeObject.ThisAffiliation(oObjectiveObject);
                }
            }
            //有依赖对象
            else
            {
                //有依赖附加
                if ((null != lstAttachDependencyObjects)
                    && (0x00 < lstAttachDependencyObjects.Count))
                {
                    //对每个附加项
                    foreach (DrawData item in lstAttachDependencyObjects)
                    {
                        //将附加项追加到依赖对象
                        oDependObject.ThisAppend(item);

                        //将附加项从原型对象中移除
                        oPrototypeObject.ThisRemove(item);

                        //修改隶属关系
                        item.ThisAffiliation(oDependObject);
                    }
                }

                //无目标对象
                if (null == oObjectiveObject)
                {
                    //有目标附加
                    if ((null != lstAttachObjectiveObjects)
                        && (0x00 < lstAttachObjectiveObjects.Count))
                    {
                        //对每个附加项
                        foreach (DrawData item in lstAttachObjectiveObjects)
                        {
                            //将附加项追加到原型对象
                            oPrototypeObject.ThisAppend(item);

                            //将附加项从当前系统中移除
                            ThisRemove(item);

                            //修改隶属关系
                            item.ThisAffiliation(oPrototypeObject);
                        }
                    }

                    //将原型对象从依赖对象中移除
                    oDependObject.ThisRemove(oPrototypeObject);

                    //将原型对象插入到当前系统中(目标)
                    ThisInsertAt(oPrototypeObject, oBehaviorObjective.IndexPresent);

                    //修改隶属关系(原型对象)
                    oPrototypeObject.ThisAffiliation(null);
                }
                //有目标对象
                else
                {
                    //有目标附加
                    if ((null != lstAttachObjectiveObjects)
                        && (0x00 < lstAttachObjectiveObjects.Count))
                    {
                        //对每个附加项
                        foreach (DrawData item in lstAttachObjectiveObjects)
                        {
                            //将附加项追加到原型对象
                            oPrototypeObject.ThisAppend(item);

                            //将附加项从目标对象中移除
                            oObjectiveObject.ThisRemove(item);

                            //修改隶属关系
                            item.ThisAffiliation(oPrototypeObject);
                        }
                    }

                    //将原型对象从依赖对象中移除
                    oDependObject.ThisRemove(oPrototypeObject);

                    //将原型对象插入到目标对象中
                    oObjectiveObject.ThisInsertAt(oPrototypeObject, oBehaviorObjective.IndexPresent);

                    //修改隶属关系(原型对象)
                    oPrototypeObject.ThisAffiliation(oObjectiveObject);
                }
            }

            //
            return true;
        }

        /// <summary>
        /// 单行为对象变化处理(包含)
        /// </summary>
        /// <param name="oPrototypeBehavior">原型行为对象</param>
        /// <param name="oBehaviorDependency">行为依赖参数</param>
        /// <param name="lstBehaviorAttachments">行为附加参数</param>
        /// <returns>处理结果true处理成功, 否则失败</returns>
        protected virtual bool SingleObjectBehavorHandleInclude(
            ObjectBehavior oPrototypeBehavior,
            BehaveOperationArgs oBehaviorDependency,
            List<BehaveOperationArgs> lstBehaviorAttachments)
        {
            //检查原型行为对象
            DrawData oPrototypeBehaviorObject = oPrototypeBehavior as DrawData;
            if (null == oPrototypeBehaviorObject)
            {
                throw new ArgumentNullException("Parameter oPrototypeBehaviorObject is null.");
            }
            if (null == oBehaviorDependency)
            {
                throw new ArgumentNullException("Parameter oBehaviorDependency is null.");
            }

            //依赖对象
            DrawData oDependObject = null;
            if (null != oBehaviorDependency)
            {
                //无依赖
                if ((null == oBehaviorDependency.ThisGuid)
                    && (string.IsNullOrEmpty(oBehaviorDependency.Identifier)))
                {
                    oDependObject = null;
                }
                else
                {
                    //尝试查找依赖(从当前系统)
                    oDependObject = FindDrawDataBy(
                        new DrawData() { ThisGuid = (Guid)oBehaviorDependency.ThisGuid },
                        (first, second) => { return first?.ThisGuid == second?.ThisGuid; }
                        );
                }
            }

            //克隆原型行为对象对应的原型对象
            DrawData oPrototypeObject = oPrototypeBehaviorObject.Clone(oPrototypeBehavior.CloneAction) as DrawData;
            //检查原型对象
            if (null == oPrototypeObject)
            {
                throw new ArgumentNullException("Parameter oPrototypeObject is null.");
            }

            //附加对象集合
            List<DrawData> lstAttachObjects = null;
            if ((null != lstBehaviorAttachments)
                && (0x00 < lstBehaviorAttachments.Count))
            {
                //有附加
                lstAttachObjects = new List<DrawData>(lstBehaviorAttachments.Count);

                //附加项GUID集合
                List<Guid> lstGuidMap = lstBehaviorAttachments.Select(o => (Guid)o?.ThisGuid).Where(o => (null != o)).ToList();

                //获取附加对象集合
                if (null != oDependObject)
                {
                    //从依赖项
                    oDependObject.FindDrawDataBy(lstAttachObjects, (o) => { return lstGuidMap.Contains<Guid>((Guid)o.ThisGuid); });
                }
                else
                {
                    //从当前系统
                    ThisFindDrawDataBy(lstAttachObjects, (o) => { return lstGuidMap.Contains<Guid>((Guid)o.ThisGuid); });
                }
            }

            //根据原型,附加,依赖对象的结果操作

            //无依赖对象
            if (null == oDependObject)
            {
                //有附加
                if ((null != lstAttachObjects)
                    && (0x00 < lstAttachObjects.Count))
                {
                    //对每个附加项
                    foreach (DrawData item in lstAttachObjects)
                    {
                        //将附加项追加到原型对象
                        oPrototypeObject.ThisAppend(item);

                        //将附加项追加到原型行为对象
                        oPrototypeBehaviorObject.ThisAppend(item);

                        //将附加项从当前系统中移除
                        ThisRemove(item);

                        //修改隶属关系
                        item.ThisAffiliation(oPrototypeObject);
                    }
                }

                //将原型对象插入到当前系统中
                ThisInsertAt(oPrototypeObject, oBehaviorDependency.IndexPresent);

                //修改隶属关系(原型对象)
                oPrototypeObject.ThisAffiliation(null);

                //修改隶属关系(原型行为对象)
                oPrototypeBehaviorObject.ThisAffiliation(null);
            }
            //有依赖对象
            else
            {
                //有附加
                if ((null != lstAttachObjects)
                    && (0x00 < lstAttachObjects.Count))
                {
                    //对每个附加项
                    foreach (DrawData item in lstAttachObjects)
                    {
                        //将附加项追加到原型对象
                        oPrototypeObject.ThisAppend(item);

                        //将附加项追加到原型行为对象
                        oPrototypeBehaviorObject.ThisAppend(item);

                        //将附加项从依赖对象中移除
                        oDependObject.ThisRemove(item);

                        //修改隶属关系
                        item.ThisAffiliation(oPrototypeObject);
                    }
                }

                //将原型对象插入到当前系统中
                oDependObject.ThisInsertAt(oPrototypeObject, oBehaviorDependency.IndexPresent);

                //修改隶属关系(原型对象)
                oPrototypeObject.ThisAffiliation(oDependObject);

                //修改隶属关系(原型行为对象)
                oPrototypeBehaviorObject.ThisAffiliation(oDependObject);
            }

            //
            return true;
        }

        /// <summary>
        /// 单行为对象变化处理(排除)
        /// </summary>
        /// <param name="oPrototypeBehavior">原型行为对象</param>
        /// <param name="oBehaviorDependency">行为依赖参数</param>
        /// <param name="lstBehaviorAttachments">行为附加参数</param>
        /// <returns>处理结果true处理成功, 否则失败</returns>
        protected virtual bool SingleObjectBehavorHandleExclude(
            ObjectBehavior oPrototypeBehavior,
            BehaveOperationArgs oBehaviorDependency,
            List<BehaveOperationArgs> lstBehaviorAttachments)
        {
            //检查原型行为对象
            DrawData oPrototypeBehaviorObject = oPrototypeBehavior as DrawData;
            if (null == oPrototypeBehaviorObject)
            {
                throw new ArgumentNullException("Parameter oPrototypeBehaviorObject is null.");
            }
            if (null == oBehaviorDependency)
            {
                throw new ArgumentNullException("Parameter oBehaviorDependency is null.");
            }

            //依赖对象
            DrawData oDependObject = null;
            if (null != oBehaviorDependency)
            {
                //无依赖
                if ((null == oBehaviorDependency.ThisGuid)
                    && (string.IsNullOrEmpty(oBehaviorDependency.Identifier)))
                {
                    oDependObject = null;
                }
                else
                {
                    //尝试查找依赖(从当前系统)
                    oDependObject = FindDrawDataBy(
                        new DrawData() { ThisGuid = (Guid)oBehaviorDependency.ThisGuid },
                        (first, second) => { return first?.ThisGuid == second?.ThisGuid; }
                        );
                }
            }

            //查找原型行为对象对应的原型对象
            DrawData oPrototypeObject = null;
            if (null != oDependObject)
            {
                //从依赖项
                oPrototypeObject = oDependObject.FindDrawDataBy((o) => { return o?.ThisGuid == oPrototypeBehaviorObject.ThisGuid; });
            }
            else
            {
                //从当前系统
                oPrototypeObject = FindDrawDataBy((o) => { return o?.ThisGuid == oPrototypeBehaviorObject.ThisGuid; });
            }
            //检查原型对象
            if (null == oPrototypeObject)
            {
                throw new ArgumentNullException("Parameter oPrototypeObject is null.");
            }

            //附加对象集合
            List<DrawData> lstAttachObjects = null;
            if ((null != lstBehaviorAttachments)
                && (0x00 < lstBehaviorAttachments.Count))
            {
                //有附加
                lstAttachObjects = new List<DrawData>(lstBehaviorAttachments.Count);

                //附加项GUID集合
                List<Guid> lstGuidMap = lstBehaviorAttachments.Select(o => (Guid)o?.ThisGuid).Where(o => (null != o)).ToList();

                //获取附加对象集合
                if (null != oDependObject)
                {
                    //从依赖项
                    oDependObject.FindDrawDataBy(lstAttachObjects, (o) => { return lstGuidMap.Contains<Guid>((Guid)o.ThisGuid); });
                }
                else
                {
                    //从当前系统
                    ThisFindDrawDataBy(lstAttachObjects, (o) => { return lstGuidMap.Contains<Guid>((Guid)o.ThisGuid); });
                }
            }

            //根据原型,附加,依赖对象的结果操作

            //无依赖对象
            if (null == oDependObject)
            {
                //有附加
                if ((null != lstAttachObjects)
                    && (0x00 < lstAttachObjects.Count))
                {
                    //对每个附加项
                    foreach (DrawData item in lstAttachObjects)
                    {
                        //将附加项追加到当前系统
                        ThisAppend(item);

                        //将附加项从原型对象中移除
                        oPrototypeObject.ThisRemove(item);

                        //将附加项从原型行为对象中移除
                        oPrototypeBehaviorObject.ThisRemove(item);

                        //修改隶属关系
                        item.ThisAffiliation(null);
                    }
                }

                //将原型对象从当前系统中移除(元素条件)
                ThisRemove(oPrototypeObject);

                //修改隶属关系(原型对象)
                oPrototypeObject.ThisAffiliation(null);

                //修改隶属关系(原型行为对象)
                oPrototypeBehaviorObject.ThisAffiliation(null);
            }
            //有依赖对象
            else
            {
                //有附加
                if ((null != lstAttachObjects)
                    && (0x00 < lstAttachObjects.Count))
                {
                    //对每个附加项
                    foreach (DrawData item in lstAttachObjects)
                    {
                        //将附加项追加到依赖对象
                        oDependObject.ThisAppend(item);

                        //将附加项从原型对象中移除
                        oPrototypeObject.ThisRemove(item);

                        //将附加项从原型行为对象中移除
                        oPrototypeBehaviorObject.ThisRemove(item);

                        //修改隶属关系
                        item.ThisAffiliation(oDependObject);
                    }
                }

                //将原型对象从依赖对象中移除(元素条件)
                oDependObject.ThisRemove(oPrototypeObject);

                //修改隶属关系(原型对象)
                oPrototypeObject.ThisAffiliation(null);

                //修改隶属关系(原型行为对象)
                oPrototypeBehaviorObject.ThisAffiliation(null);
            }

            //
            return true;
        }

        /// <summary>
        /// 单行为对象变化处理(属性)
        /// </summary>
        /// <param name="oPrototypeBehavior">行为原型</param>
        /// <param name="oReferenceBehavior">行为引用</param>
        /// <param name="hsRemarks">标记</param>
        /// <returns>处理结果true处理成功, 否则失败</returns>
        protected virtual bool SingleObjectBehavorHandleAlter(
            ObjectBehavior oPrototypeBehavior,
            ObjectBehavior oReferenceBehavior,
            HashSet<string> hsRemarks)
        {
            //检查
            if (null == hsRemarks)
            {
                throw new ArgumentNullException("Parameter hsRemarks is null.");
            }

            //对象原型体
            DrawData oPrototypeObject = oPrototypeBehavior as DrawData;
            //对象引用体
            DrawData oReferenceObject = oReferenceBehavior as DrawData;

            //检查
            if (null == oPrototypeObject)
            {
                throw new ArgumentNullException("Parameter oPrototypeObject is null.");
            }
            if (null == oReferenceObject)
            {
                throw new ArgumentNullException("Parameter oReferenceObject is null.");
            }

            //查找协变对象条件为ID
            Func<DrawData, DrawData, bool> deFunc = (first, second) => { return first?.Identifier == second?.Identifier; };

            //质性变更
            if (hsRemarks.Contains(nameof(oPrototypeObject.Identifier)))
            {
                //包含ID且类型或重复性不同
                if ((oPrototypeObject.Type != oReferenceObject.Type)
                    || (oPrototypeObject.Repeats != oReferenceObject.Repeats))
                {
                    //修改查找条件为GUID
                    deFunc = (first, second) => { return first?.ThisGuid == second?.ThisGuid; };
                }
            }

            //通过引用体查找协变体
            List<DrawData> lstResult = new List<DrawData>();
            int iResult = FindDrawDataBy(lstResult, oReferenceObject, deFunc);
            foreach (DrawData item in lstResult)
            {
                //通过原型体更新协变体
                if (null != item)
                {
                    item.ThisUpdateBy(oPrototypeObject, hsRemarks);
                }
            }

            //
            if (0x00 >= iResult)
            {
                return false;
            }

            //
            return true;
        }



        public virtual void TakeBehavor()
        {
            //为一下准备
            //1.同时删除多个同级对象
            //2.拷贝,剪切,复制操作
            //3.监控系统数据变化
            //4.添加个中间节点
            //5.转移们的折叠和扩展
            //6.


            //外因触发属性变化事件
            OnObjectPropertyChanged();

        }

        /// <summary>
        /// 行为捕捉(属性变化)
        /// </summary>
        /// <param name="oBehavor">行为体</param>
        /// <param name="oEntity">行为对象体</param>
        /// <returns>行为体</returns>
        public virtual ObjectBehaveEntity TakeBehavor(ObjectBehaveEntity oBehavor, ObjectBehavior oEntity)
        {
            //检查
            if (null == oEntity)
            {
                throw new ArgumentNullException("Parameter oEntity is null.");
            }

            //原始行为
            if (null == oBehavor)
            {
                //构建行为体
                oBehavor = new ObjectBehaveEntity();

                //保存先前
                oBehavor.PreviousBehavorObject = (ObjectBehavior)oEntity.Clone(FaultTreeAnalysis.Behavior.Enum.CloneAction.Shallow);
            }
            //变动行为
            else
            {
                //外因触发属性变化事件
                OnObjectPropertyChanged();


                //原始行为检查
                if (null == oBehavor.PreviousBehavorObject)
                {
                    throw new ArgumentNullException("Parameter PreviousBehavorObject is null.");
                }

                //对象不存在差异
                if (oBehavor.PreviousBehavorObject.Equals(oEntity))
                {
                    return oBehavor;
                }

                //差异记录

                //属性比较并记录
                if (null == oBehavor.PreviousBehavorObject.Remarks)
                {
                    oBehavor.PreviousBehavorObject.Remarks = new HashSet<string>();
                }
                else
                {
                    oBehavor.PreviousBehavorObject.Remarks.Clear();
                }
                oBehavor.PreviousBehavorObject.Remark(oEntity);

                //保存当前行为对象体
                oBehavor.PresentBehavorObject = (ObjectBehavior)oEntity.Clone(FaultTreeAnalysis.Behavior.Enum.CloneAction.Shallow);

                //成功获取
                if (null != UndoHistory)
                {
                    UndoHistory.Push(oBehavor);

                    //新的动作成立后， 应当消除旧动作的重做
                    if (null != RedoHistory)
                    {
                        RedoHistory.Clear();
                    }
                }
            }

            //返回当前操作的行为体
            return oBehavor;
        }

        /// <summary>
        /// 行为捕捉(元素增减变化)
        /// </summary>
        /// <param name="oEntity">行为对象体</param>
        /// <param name="oOperateCause">操作原因</param>
        /// <param name="oOperateEffect">操作影响</param>
        /// <param name="oDependEntity">行为对象依赖体</param>
        /// <param name="lstAttachEntities">行为对象附属体集合</param>
        /// <returns>行为体</returns>
        public virtual ObjectBehaveEntity TakeBehavor(
            ObjectBehavior oEntity,
            ElementOperate oOperateCause,
            ElementOperate oOperateEffect = ElementOperate.None,
            ObjectBehavior oDependEntity = null,
            List<ObjectBehavior> lstAttachEntities = null)
        {

            //外因触发属性变化事件
            OnObjectPropertyChanged();


            //检查
            if (null == oEntity)
            {
                throw new ArgumentNullException("Parameter oEntity is null.");
            }
            if (null == oDependEntity)
            {
                //throw new ArgumentNullException("Parameter oDependEntity is null.");
            }
            if (null == lstAttachEntities)
            {
                //throw new ArgumentNullException("Parameter lstAttachEntities is null.");
            }

            //构建行为体
            ObjectBehaveEntity oBehavor = new ObjectBehaveEntity()
            {
                Cause = oOperateCause,
                Effect = oOperateEffect
            };

            //创建性裁决
            if (ElementOperate.Creation == (ElementOperate.Creation & oOperateCause))
            {
                //行为对象动作裁决
                CloneAction enEntityAction = TakeBehavorCreation(oBehavor, oEntity, oDependEntity, lstAttachEntities);

                //保存当前行为对象体
                oBehavor.PresentBehavorObject = (ObjectBehavior)oEntity.Clone(enEntityAction);
                oBehavor.PresentBehavorObject.CloneAction = enEntityAction;
            }

            //移除性裁决
            if (ElementOperate.Remove == (ElementOperate.Remove & oOperateCause))
            {
                //行为对象动作裁决
                CloneAction enEntityAction = TakeBehavorRemove(oBehavor, oEntity, oDependEntity, lstAttachEntities);

                //保存当前行为对象体
                oBehavor.PresentBehavorObject = (ObjectBehavior)oEntity.Clone(enEntityAction);
                oBehavor.PresentBehavorObject.CloneAction = enEntityAction;
            }

            //成功获取
            if (null != UndoHistory)
            {
                UndoHistory.Push(oBehavor);

                //新的动作成立后, 应当消除旧动作的重做
                if (null != RedoHistory)
                {
                    RedoHistory.Clear();
                }
            }

            //返回当前操作的行为体
            return oBehavor;
        }

        /// <summary>
        /// 行为捕捉(元素移动变化)
        /// </summary>
        /// <param name="oEntity">行为对象体</param>
        /// <param name="oOperateCause">操作原因</param>
        /// <param name="oOperateEffect">操作影响</param>
        /// <param name="oDestine">行为对象体目的</param>
        /// <param name="oSource">行为对象体来源</param>
        /// <param name="lstAttachEntities">行为对象附属体集合</param>
        /// <returns>行为体</returns>
        public virtual ObjectBehaveEntity TakeBehavor(
            ObjectBehavior oEntity,
            ElementOperate oOperateCause,
            ElementOperate oOperateEffect = ElementOperate.None,
            ObjectBehavior oDestine = null,
            ObjectBehavior oSource = null,
            List<ObjectBehavior> lstAttachEntities = null
            )
        {
            //外因触发属性变化事件
            OnObjectPropertyChanged();


            //检查
            if (null == oEntity)
            {
                throw new ArgumentNullException("Parameter oEntity is null.");
            }
            if (null == oDestine)
            {
                //throw new ArgumentNullException("Parameter oDestine is null.");
            }
            if (null == oSource)
            {
                //throw new ArgumentNullException("Parameter oSource is null.");
            }
            if (null == lstAttachEntities)
            {
                //throw new ArgumentNullException("Parameter lstAttachEntities is null.");
            }

            //构建行为体
            ObjectBehaveEntity oBehavor = new ObjectBehaveEntity()
            {
                Cause = oOperateCause,
                Effect = oOperateEffect
            };

            //移动性裁决
            if (ElementOperate.Move == (ElementOperate.Move & oOperateCause))
            {
                //行为对象动作裁决
                CloneAction enEntityAction = TakeBehavorMove(oBehavor, oEntity, oDestine, oSource, lstAttachEntities);

                //保存当前行为对象体(影子)
                oBehavor.PresentBehavorObject = (ObjectBehavior)oEntity.Clone(enEntityAction);
                oBehavor.PresentBehavorObject.CloneAction = enEntityAction;
            }

            //成功获取
            if (null != UndoHistory)
            {
                UndoHistory.Push(oBehavor);

                //新的动作成立后, 应当消除旧动作的重做
                if (null != RedoHistory)
                {
                    RedoHistory.Clear();
                }
            }

            //返回当前操作的行为体
            return oBehavor;
        }


        /// <summary>
        /// 行为捕捉(元素移动变化)
        /// </summary>
        /// <param name="oBehavor">行为体</param>
        /// <param name="oEntity">行为对象体</param>
        /// <param name="oDestine">行为对象体目的</param>
        /// <param name="oSource">行为对象体来源</param>
        /// <param name="lstAttachEntities">行为对象附属体集合</param>
        /// <returns>行为动作</returns>
        private CloneAction TakeBehavorMove(
            ObjectBehaveEntity oBehavor,
            ObjectBehavior oEntity,
            ObjectBehavior oDestine,
            ObjectBehavior oSource,
            List<ObjectBehavior> lstAttachEntities)
        {
            //检查
            if (null == oBehavor)
            {
                throw new ArgumentNullException("Parameter oBehavor is null.");
            }
            if (null == oEntity)
            {
                throw new ArgumentNullException("Parameter oEntity is null.");
            }

            //动作标识(影子)
            CloneAction enEntityAction = CloneAction.None;
            //元素移动不产生实体
            enEntityAction |= CloneAction.Shadow;

            //依赖性
            oBehavor.Dependency = new BehaveOperationArgs();
            if (null != oSource)
            {
                //依赖某元素
                DrawData oDepend = (oSource as DrawData);
                if (null == oDepend)
                {
                    throw new ArgumentNullException("Parameter oDepend is null.");
                }

                //依赖属性
                oBehavor.Dependency.ThisGuid = oDepend.ThisGuid;
                oBehavor.Dependency.Identifier = oDepend.Identifier;
                oBehavor.Dependency.IndexPrevious = -1;
                oBehavor.Dependency.IndexPresent = ThisIndexWith((oEntity as DrawData), oDepend.Children);

                //依赖性裁决
                //enEntityAction |= CloneAction.Beyond;
            }
            else
            {
                //无依赖性
                oBehavor.Dependency.ThisGuid = null;
                oBehavor.Dependency.Identifier = string.Empty;
                oBehavor.Dependency.IndexPrevious = -1;
                oBehavor.Dependency.IndexPresent = ThisIndexWith((oEntity as DrawData), null);
            }

            //附加性
            oBehavor.Attachments = new List<Behavior.Event.BehaveOperationArgs>();
            if ((null != lstAttachEntities)
                && (0x00 < lstAttachEntities.Count))
            {
                //依赖元素集合
                foreach (ObjectBehavior item in lstAttachEntities)
                {
                    DrawData oAttach = (item as DrawData);
                    if (null != oAttach)
                    {
                        //附加属性
                        oBehavor.Attachments.Add(new BehaveOperationArgs()
                        {
                            ThisGuid = oAttach.ThisGuid,
                            Identifier = oAttach.Identifier,
                            IndexPrevious = -1,
                            IndexPresent = ThisIndexWith(oAttach, ((DrawData)oEntity).Children)
                        });
                        //
                    }
                }
            }
            else
            {
                //无附加性
                //oBehavor.Attachments.Add(new BehaveOperationArgs()
                //{
                //    ThisGuid = null,
                //    Identifier = string.Empty,
                //    IndexPrevious = -1,
                //    IndexPresent = 0x00// ThisIndexWith((oEntity as DrawData), oAttach.Children)
                //});

                //附加性裁决
                //enEntityAction |= CloneAction.Extreme;
            }

            //目标项
            oBehavor.Objective = new BehaveOperationArgs();
            if (null != oDestine)
            {
                //目标某元素
                DrawData oObjective = (oDestine as DrawData);
                if (null == oObjective)
                {
                    throw new ArgumentNullException("Parameter oObjective is null.");
                }

                //目标属性
                oBehavor.Objective.ThisGuid = oObjective.ThisGuid;
                oBehavor.Objective.Identifier = oObjective.Identifier;
                oBehavor.Objective.IndexPrevious = -1;
                oBehavor.Objective.IndexPresent = 0x00;// ThisIndexWith((oEntity as DrawData), oObjective.Children);

                //目标性裁决
                //enEntityAction |= CloneAction.Beyond;
            }
            else
            {
                //无目标性
                oBehavor.Objective.ThisGuid = null;
                oBehavor.Objective.Identifier = string.Empty;
                oBehavor.Objective.IndexPrevious = -1;
                oBehavor.Objective.IndexPresent = 0x00;// ThisIndexWith((oEntity as DrawData), null);
            }

            //操作行为
            return enEntityAction;
        }
        /// <summary>
        /// 行为捕捉(元素减变化)
        /// </summary>
        /// <param name="oBehavor">行为体</param>
        /// <param name="oEntity">行为对象体</param>
        /// <param name="oDependEntity">行为对象依赖体</param>
        /// <param name="lstAttachEntities">行为对象附属体集合</param>
        /// <returns>行为动作</returns>
        private CloneAction TakeBehavorRemove(
            ObjectBehaveEntity oBehavor,
            ObjectBehavior oEntity,
            ObjectBehavior oDependEntity,
            List<ObjectBehavior> lstAttachEntities)
        {
            //检查
            if (null == oBehavor)
            {
                throw new ArgumentNullException("Parameter oBehavor is null.");
            }
            if (null == oEntity)
            {
                throw new ArgumentNullException("Parameter oEntity is null.");
            }

            //浅表标识
            CloneAction enEntityAction = CloneAction.None;
            enEntityAction |= CloneAction.Shallow;

            //依赖性
            oBehavor.Dependency = new Behavior.Event.BehaveOperationArgs();
            if (null != oDependEntity)
            {
                //依赖某元素
                DrawData oDepend = (oDependEntity as DrawData);
                if (null == oDepend)
                {
                    throw new ArgumentNullException("Parameter oDepend is null.");
                }

                //依赖属性
                oBehavor.Dependency.ThisGuid = oDepend.ThisGuid;
                oBehavor.Dependency.Identifier = oDepend.Identifier;
                oBehavor.Dependency.IndexPrevious = -1;
                oBehavor.Dependency.IndexPresent = ThisIndexWith((oEntity as DrawData), oDepend.Children);

                //依赖性裁决
                //enEntityAction |= CloneAction.Beyond;
            }
            else
            {
                //无依赖性
                oBehavor.Dependency.ThisGuid = null;
                oBehavor.Dependency.Identifier = string.Empty;
                oBehavor.Dependency.IndexPrevious = -1;
                oBehavor.Dependency.IndexPresent = ThisIndexWith((oEntity as DrawData), null);
            }

            //附加性
            oBehavor.Attachments = new List<Behavior.Event.BehaveOperationArgs>();
            if ((null != lstAttachEntities)
                && (0x00 < lstAttachEntities.Count))
            {
                //依赖元素集合
                foreach (ObjectBehavior item in lstAttachEntities)
                {
                    DrawData oAttach = (item as DrawData);
                    if (null != oAttach)
                    {
                        //附加属性
                        oBehavor.Attachments.Add(new BehaveOperationArgs()
                        {
                            ThisGuid = oAttach.ThisGuid,
                            Identifier = oAttach.Identifier,
                            IndexPrevious = -1,
                            IndexPresent = ThisIndexWith(oAttach, ((DrawData)oEntity).Children)
                        });
                        //
                    }
                }
            }
            else
            {
                //无附加性
                //oBehavor.Attachments.Add(new BehaveOperationArgs()
                //{
                //    ThisGuid = null,
                //    Identifier = string.Empty,
                //    IndexPrevious = -1,
                //    IndexPresent = 0x00// ThisIndexWith((oEntity as DrawData), oAttach.Children)
                //});

                //附加性裁决
                enEntityAction |= CloneAction.Extreme;
            }

            //操作行为
            return enEntityAction;
        }
        /// <summary>
        /// 行为捕捉(元素增变化)
        /// </summary>
        /// <param name="oBehavor">行为体</param>
        /// <param name="oEntity">行为对象体</param>
        /// <param name="oDependEntity">行为对象依赖体</param>
        /// <param name="lstAttachEntities">行为对象附属体集合</param>
        /// <returns>行为动作</returns>
        private CloneAction TakeBehavorCreation(
            ObjectBehaveEntity oBehavor,
            ObjectBehavior oEntity,
            ObjectBehavior oDependEntity,
            List<ObjectBehavior> lstAttachEntities)
        {
            //检查
            if (null == oBehavor)
            {
                throw new ArgumentNullException("Parameter oBehavor is null.");
            }
            if (null == oEntity)
            {
                throw new ArgumentNullException("Parameter oEntity is null.");
            }

            //浅表标识
            CloneAction enEntityAction = CloneAction.None;
            enEntityAction |= CloneAction.Shallow;

            //依赖性
            oBehavor.Dependency = new Behavior.Event.BehaveOperationArgs();
            if (null != oDependEntity)
            {
                //依赖某元素
                DrawData oDepend = (oDependEntity as DrawData);
                if (null == oDepend)
                {
                    throw new ArgumentNullException("Parameter oDepend is null.");
                }

                //依赖属性
                oBehavor.Dependency.ThisGuid = oDepend.ThisGuid;
                oBehavor.Dependency.Identifier = oDepend.Identifier;
                oBehavor.Dependency.IndexPrevious = -1;
                oBehavor.Dependency.IndexPresent = ThisIndexWith((oEntity as DrawData), oDepend.Children);

                //依赖性裁决
                //enEntityAction |= CloneAction.Beyond;
            }
            else
            {
                //无依赖性
                oBehavor.Dependency.ThisGuid = null;
                oBehavor.Dependency.Identifier = string.Empty;
                oBehavor.Dependency.IndexPrevious = -1;
                oBehavor.Dependency.IndexPresent = ThisIndexWith((oEntity as DrawData), null);
            }

            //附加性
            oBehavor.Attachments = new List<Behavior.Event.BehaveOperationArgs>();
            if ((null != lstAttachEntities)
                && (0x00 < lstAttachEntities.Count))
            {
                //依赖元素集合
                foreach (ObjectBehavior item in lstAttachEntities)
                {
                    DrawData oAttach = (item as DrawData);
                    if (null != oAttach)
                    {
                        //附加属性
                        oBehavor.Attachments.Add(new BehaveOperationArgs()
                        {
                            ThisGuid = oAttach.ThisGuid,
                            Identifier = oAttach.Identifier,
                            IndexPrevious = -1,
                            IndexPresent = ThisIndexWith(oAttach, ((DrawData)oEntity).Children)
                        });
                        //
                    }
                }

                //附加性裁决(测试代码) 
#if _FTA_DEGUB_
                //enEntityAction |= CloneAction.Extreme;
#endif
            }
            else
            {
                //无附加性
                //oBehavor.Attachments.Add(new BehaveOperationArgs()
                //{
                //    ThisGuid = null,
                //    Identifier = string.Empty,
                //    IndexPrevious = -1,
                //    IndexPresent = 0x00// ThisIndexWith((oEntity as DrawData), null)
                //});
            }

            //
            return enEntityAction;
        }




        /// <summary>
        /// 附加元素
        /// </summary>
        /// <param name="oDrawData">被附加元素</param>
        public void ThisAppend(DrawData oDrawData)
        {
            //检查
            if (null == oDrawData)
            {
                throw new ArgumentNullException("Parameter oDrawData is null.");
            }

            //
            if (null == this.Roots)
            {
                this.Roots = new List<DrawData>();
            }

            //元素追加
            this.Roots.Add(oDrawData);
        }

        /// <summary>
        /// 元素插入
        /// </summary>
        /// <param name="oDrawData">元素</param>
        /// <param name="nIndex">索引</param>
        public void ThisInsertAt(DrawData oDrawData, int nIndex)
        {
            //检查
            if (null == oDrawData)
            {
                throw new ArgumentNullException("Parameter oDrawData is null.");
            }
            //越界
            if (0x00 > nIndex)
            {
                return;
            }

            //
            if (null == this.Roots)
            {
                this.Roots = new List<DrawData>();
            }

            //元素增添
            if (nIndex >= this.Roots.Count)
            {
                //追加
                this.Roots.Add(oDrawData);
            }
            else
            {
                //插入
                this.Roots.Insert(nIndex, oDrawData);
            }
        }

        /// <summary>
        /// 元素移除
        /// </summary>
        /// <param name="oDrawData">被移除元素</param>
        public void ThisRemove(DrawData oDrawData)
        {
            if (null == oDrawData)
            {
                return;
            }
            if (null == this.Roots)
            {
                return;
            }

            //移除操作
            ThisRemoveAt(this.Roots.FindIndex<DrawData>((o) => { return oDrawData.ThisGuid == o.ThisGuid; }));
        }

        /// <summary>
        /// 元素移除
        /// </summary>
        /// <param name="nIndex">索引</param>
        public void ThisRemoveAt(int nIndex)
        {
            //检查
            if (null == this.Roots)
            {
                return;
            }
            if ((0x00 > nIndex)
                || (nIndex >= this.Roots.Count))
            {
                return;
            }

            //移除操作
            this.Roots.RemoveAt(nIndex);
        }

        /// <summary>
        /// 搜索元素所在集合的索引
        /// </summary>
        /// <param name="oDrawData">元素</param>
        /// <param name="lstDrawDataSet">元素集合</param>
        /// <returns>元素所在集合的索引</returns>
        public int ThisIndexWith(DrawData oDrawData, List<DrawData> lstDrawDataSet)
        {
            //检查
            if (null == oDrawData)
            {
                throw new ArgumentNullException("Parameter oDrawData is null.");
            }

            //诺集合为空则默认当前集合
            if (null == lstDrawDataSet)
            {
                lstDrawDataSet = this.Roots;
            }
            if (null == lstDrawDataSet)
            {
                //异常
                return -1;
                //throw new ArgumentNullException("Parameter oDrawDataSet is null.");
            }

            //根据GUID条件返回元素位置
            return lstDrawDataSet.FindIndex<DrawData>((item) => { return item.ThisGuid == oDrawData.ThisGuid; });
        }


        /// <summary>
        /// 通过协变体查找原体
        /// </summary>
        /// <param name="oCovariant">协变体对象</param>
        /// <param name="deFunc">查找条件</param>
        /// <returns>null:找不到, 否则找到原体</returns>
        public DrawData FindDrawDataBy(DrawData oCovariant, Func<DrawData, DrawData, bool> deFunc)
        {
            //检查
            if (null == oCovariant)
            {
                throw new ArgumentNullException("Parameter oCovariant is null.");
            }
            if (null == deFunc)
            {
                throw new ArgumentNullException("Parameter deFunc is null.");
            }
            if (null == this.Roots)
            {
                throw new ArgumentNullException("Parameter Roots is null.");
            }

            //查找对象
            DrawData oResult = null;

            //唯一对象搜索条件
            List<DrawData> lstSources = new List<DrawData>(this.Roots);
            foreach (DrawData item in lstSources)
            {
                oResult = FindDrawDataBy(item, oCovariant, deFunc);
                //已经找到
                if (null != oResult)
                {
                    return oResult;
                }
            }

            //未找到
            return oResult;
        }

        /// <summary>
        /// 通过条件查找
        /// </summary>
        /// <param name="preFunc">查找条件</param>
        /// <returns>null:找不到, 否则找到原体</returns>
        public DrawData FindDrawDataBy(Predicate<DrawData> preFunc)
        {
            //检查
            if (null == preFunc)
            {
                throw new ArgumentNullException("Parameter preFunc is null.");
            }
            if (null == Roots)
            {
                throw new ArgumentNullException("Parameter Roots is null.");
            }

            //查找对象
            DrawData oResult = this.Roots.Find(preFunc);
            if (null != oResult)
            {
                return oResult;
            }

            //唯一对象搜索条件
            List<DrawData> lstSources = new List<DrawData>(this.Roots);
            foreach (DrawData item in lstSources)
            {
                oResult = FindDrawDataBy(item, preFunc);
                //已经找到
                if (null != oResult)
                {
                    return oResult;
                }
            }

            //未找到
            return oResult;
        }

        /// <summary>
        /// 当前浅表查询
        /// </summary>
        /// <param name="lstResult">结果集</param>
        /// <param name="preFunc">条件</param>
        public void ThisFindDrawDataBy(List<DrawData> lstResult, Predicate<DrawData> preFunc)
        {
            //检查
            if (null == lstResult)
            {
                throw new ArgumentNullException("Parameter lstResult is null.");
            }
            if (null == preFunc)
            {
                throw new ArgumentNullException("Parameter preFunc is null.");
            }
            if (null == this.Roots)
            {
                throw new ArgumentNullException("Parameter Roots is null.");
            }

            //当前级搜索
            lstResult.AddRange(this.Roots.FindAll(preFunc));
        }

        /// <summary>
        /// 集合对象查找
        /// </summary>
        /// <param name="lstResult">结果集</param>
        /// <param name="preFunc">条件</param>
        public void FindDrawDataBy(List<DrawData> lstResult, Predicate<DrawData> preFunc)
        {
            //检查
            if (null == lstResult)
            {
                throw new ArgumentNullException("Parameter lstResult is null.");
            }
            if (null == preFunc)
            {
                throw new ArgumentNullException("Parameter preFunc is null.");
            }
            if (null == this.Roots)
            {
                throw new ArgumentNullException("Parameter Roots is null.");
            }

            //当前级搜索
            lstResult.AddRange(this.Roots.FindAll(preFunc));

            //子级搜索
            List<DrawData> lstSources = new List<DrawData>(this.Roots);
            foreach (DrawData item in lstSources)
            {
                FindDrawDataBy(lstResult, item, preFunc);
            }
        }

        /// <summary>
        /// 通过协变体查找原体
        /// </summary>
        /// <param name="lstResult">结果集合</param>
        /// <param name="oCovariant">协变体对象</param>
        /// <param name="deFunc">查找条件</param>
        /// <returns>结果集合数量</returns>
        public int FindDrawDataBy(List<DrawData> lstResult, DrawData oCovariant, Func<DrawData, DrawData, bool> deFunc)
        {
            //检查
            if (null == lstResult)
            {
                throw new ArgumentNullException("Parameter lstResult is null.");
            }
            if (null == oCovariant)
            {
                throw new ArgumentNullException("Parameter oCovariant is null.");
            }
            if (null == this.Roots)
            {
                throw new ArgumentNullException("Parameter Roots is null.");
            }

            //对象搜索
            List<DrawData> lstSources = new List<DrawData>(this.Roots);
            foreach (DrawData item in lstSources)
            {
                FindDrawDataBy(lstResult, item, oCovariant, deFunc);
            }

            //未找到
            return lstResult.Count;
        }


        /// <summary>
        /// 对象查找
        /// </summary>
        /// <param name="oContrast">对照体</param>
        /// <param name="preFunc">条件</param>
        /// <returns>查找到的对象</returns>
        private DrawData FindDrawDataBy(DrawData oContrast, Predicate<DrawData> preFunc)
        {
            //检查
            if (null == preFunc)
            {
                throw new ArgumentNullException("Parameter preFunc is null.");
            }
            if (null == oContrast)
            {
                throw new ArgumentNullException("Parameter oContrast is null.");
            }

            //查找对象
            DrawData oResult = null;

            //无搜索条件
            if (null == oContrast.Children)
            {
                return oResult;
            }

            //查找对象
            oResult = oContrast.Children.Find(preFunc);
            if (null != oResult)
            {
                return oResult;
            }

            //唯一对象搜索条件
            List<DrawData> lstSources = new List<DrawData>(oContrast.Children);
            foreach (DrawData item in lstSources)
            {
                oResult = FindDrawDataBy(item, preFunc);
                //已经找到
                if (null != oResult)
                {
                    return oResult;
                }
            }

            //未找到
            return oResult;
        }
        /// <summary>
        /// 对象查找
        /// </summary>
        /// <param name="oPrototype">原体</param>
        /// <param name="oContrast">对照体</param>
        /// <param name="deFunc">条件</param>
        /// <returns>查找到的对象</returns>
        private DrawData FindDrawDataBy(DrawData oPrototype, DrawData oContrast, Func<DrawData, DrawData, bool> deFunc)
        {
            //检查
            if (null == oPrototype)
            {
                throw new ArgumentNullException("Parameter oPrototype is null.");
            }
            if (null == oContrast)
            {
                throw new ArgumentNullException("Parameter oContrast is null.");
            }
            if (null == deFunc)
            {
                throw new ArgumentNullException("Parameter deFunc is null.");
            }

            //命中当前对象
            if (deFunc(oPrototype, oContrast))
            {
                return oPrototype;
            }

            //深度查找
            if (null != oPrototype.Children)
            {
                List<DrawData> lstSources = new List<DrawData>(oPrototype.Children);
                foreach (DrawData item in lstSources)
                {
                    DrawData oResult = FindDrawDataBy(item, oContrast, deFunc);
                    //已经找到
                    if (null != oResult)
                    {
                        return oResult;
                    }
                }
            }

            //未找到
            return null;
        }
        /// <summary>
        /// 集合对象查找
        /// </summary>
        /// <param name="lstResult">结果集</param>
        /// <param name="oPrototype">原型体</param>
        /// <param name="preFunc">条件</param>
        private void FindDrawDataBy(List<DrawData> lstResult, DrawData oPrototype, Predicate<DrawData> preFunc)
        {
            //检查
            if (null == lstResult)
            {
                throw new ArgumentNullException("Parameter lstResult is null.");
            }
            if (null == oPrototype)
            {
                throw new ArgumentNullException("Parameter oPrototype is null.");
            }
            if (null == preFunc)
            {
                throw new ArgumentNullException("Parameter preFunc is null.");
            }

            //无层级条件
            if (null == oPrototype.Children)
            {
                return;
            }

            //当前级搜索
            lstResult.AddRange(oPrototype.Children.FindAll(preFunc));

            //子级搜索
            List<DrawData> lstSources = new List<DrawData>(oPrototype.Children);
            foreach (DrawData item in lstSources)
            {
                FindDrawDataBy(lstResult, item, preFunc);
            }
        }
        /// <summary>
        /// 对象查找
        /// </summary>
        /// <param name="lstResult">结果集合</param>
        /// <param name="oPrototype">原体</param>
        /// <param name="oContrast">对照体</param>
        /// <param name="deFunc">条件</param>
        private void FindDrawDataBy(List<DrawData> lstResult, DrawData oPrototype, DrawData oContrast, Func<DrawData, DrawData, bool> deFunc)
        {
            //检查
            if (null == lstResult)
            {
                throw new ArgumentNullException("Parameter lstResult is null.");
            }
            if (null == oPrototype)
            {
                throw new ArgumentNullException("Parameter oPrototype is null.");
            }
            if (null == oContrast)
            {
                throw new ArgumentNullException("Parameter oContrast is null.");
            }
            if (null == deFunc)
            {
                throw new ArgumentNullException("Parameter deFunc is null.");
            }

            //命中当前对象
            if (deFunc(oPrototype, oContrast))
            {
                lstResult.Add(oPrototype);
            }

            //深度查找
            if (null != oPrototype.Children)
            {
                List<DrawData> lstSources = new List<DrawData>(oPrototype.Children);
                foreach (DrawData item in lstSources)
                {
                    FindDrawDataBy(lstResult, item, oContrast, deFunc);
                }
            }
        }


        /// <summary>
        /// 当对象追溯完成时触发
        /// </summary>
        /// <param name="oBehaveEntity"></param>
        private void OnObjectRetrospectCompleted(RetrospectiveInstruction enInstruction, ObjectBehaveEntity oBehaveEntity)
        {
            if (null == oBehaveEntity)
            {
                throw new ArgumentNullException("Parameter oBehaveEntity is null.");
            }

            //事件触发
            if (null != RetrospectCompleted)
            {
                RetrospectCompleted(this, new RetrospectiveArgs()
                {
                    //追溯参数
                    Instruction = enInstruction,
                    Cause = oBehaveEntity.Cause,
                    Effect = oBehaveEntity.Effect,
                    //追溯对象
                    DependencyObject = new OperationObjectArgs()
                    {
                        ThisGuid = oBehaveEntity.Dependency?.ThisGuid,
                        Identifier = oBehaveEntity.Dependency?.Identifier
                    },
                    PresentObject = new OperationObjectArgs()
                    {
                        ThisGuid = ((DrawData)oBehaveEntity.PresentBehavorObject)?.ThisGuid,
                        Identifier = ((DrawData)oBehaveEntity.PresentBehavorObject)?.Identifier
                    }
                });
            }
        }

        /// <summary>
        /// 当对象数据改变时触发
        /// </summary>
        /// <param name="bVirtualized">虚拟化状态</param>
        private void OnObjectPropertyChanged(bool bVirtualized = true)
        {
            //触发虚拟化标识
            this.Virtualized = bVirtualized;

            //事件触发
            if (null != PropertyChanged)
            {
                //最根本属性改变
                PropertyChanged(this, new ExPropertyChangedEventsArgs(nameof(this.Roots))
                {
                    //
                    Status = (this.Virtualized) ? PropertyStatus.NoApply : PropertyStatus.Applied
                });
            }
        }


        /// <summary>
        /// 设置当前系统快照
        /// </summary>
        [Obsolete("数据量巨大不适用")]
        public virtual void TakeSnapshoot()
        {
            //元素检查
            if (null == this.Roots)
            {
                throw new ArgumentNullException("Roots is null.");
            }

            //取元数据快照
            List<DrawData> lstClone = new List<DrawData>(this.Roots.Capacity);
            List<DrawData> lstRoot = new List<DrawData>(this.Roots);
            foreach (DrawData item in lstRoot)
            {
                lstClone.Add((DrawData)
                    item.Clone(
                        Behavior.Enum.CloneAction.Extreme
                        | Behavior.Enum.CloneAction.Beyond
                        | Behavior.Enum.CloneAction.Shallow
                    ));
            }

            //元素检查
            if (null == UndoHistory)
            {
                throw new ArgumentNullException("UndoElements is null.");
            }

            //保存数据快照
            //UndoHistory.Push(lstClone);
        }
    }
}
