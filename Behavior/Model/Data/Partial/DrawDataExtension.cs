using FaultTreeAnalysis.Model.Data;
using FaultTreeAnalysis.Behavior.Enum;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using DevExpress.Data.Extensions;

namespace FaultTreeAnalysis
{
    /// <summary>
    /// DrawData对象扩展
    ///     用于数据克隆, 数据比较, 数据行为记录和数据条件查找定位
    /// </summary>
    public partial class DrawData
        : ObjectExtension
    {

        /// <summary>
        /// UID
        /// </summary>
        [JsonIgnore]
        public Guid ThisGuid { get; internal set; } = Guid.NewGuid();


        /// <summary>
        /// 通过条件查找
        /// </summary>
        /// <param name="preFunc">查找条件</param>
        /// <returns>null:找不到, 否则找到</returns>
        public DrawData FindDrawDataBy(Predicate<DrawData> preFunc)
        {
            //检查
            if (null == preFunc)
            {
                throw new ArgumentNullException("Parameter preFunc is null.");
            }

            //无搜索条件
            if (null == this.Children)
            {
                return null;
            }

            //查找对象
            DrawData oResult = this.Children.Find(preFunc);
            if (null != oResult)
            {
                return oResult;
            }

            //唯一对象搜索条件
            List<DrawData> lstSources = new List<DrawData>(this.Children);
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
        /// 通过条件浅表查找
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
            if (null == this.Children)
            {
                throw new ArgumentNullException("Parameter Children is null.");
            }

            //当前级搜索
            lstResult.AddRange(this.Children.FindAll(preFunc));
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
            if (null == this.Children)
            {
                throw new ArgumentNullException("Parameter Children is null.");
            }

            //当前级搜索
            lstResult.AddRange(this.Children.FindAll(preFunc));

            //子级搜索
            List<DrawData> lstSources = new List<DrawData>(this.Children);
            foreach (DrawData item in lstSources)
            {
                FindDrawDataBy(lstResult, item, preFunc);
            }
        }

        /// <summary>
        /// 隶入从属关系
        /// </summary>
        /// <param name="oDrawData">从属对象</param>
        public void ThisAffiliation(DrawData oDrawData)
        {
            //检查
            if (null == oDrawData)
            {
                //throw new ArgumentNullException("Parameter oDrawData is null.");
            }

            //将当前对象归属于oDrawData
            this.Parent = oDrawData;
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
            if (null == this.Children)
            {
                this.Children = new List<DrawData>();
            }

            //元素追加
            this.Children.Add(oDrawData);
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
            if (null == this.Children)
            {
                this.Children = new List<DrawData>();
            }

            //元素增添
            if (nIndex >= this.Children.Count)
            {
                //追加
                this.Children.Add(oDrawData);
            }
            else
            {
                //插入
                this.Children.Insert(nIndex, oDrawData);
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
            if (null == this.Children)
            {
                return;
            }

            //移除操作
            ThisRemoveAt(this.Children.FindIndex<DrawData>((o) => { return oDrawData.ThisGuid == o.ThisGuid; }));
        }

        /// <summary>
        /// 元素移除
        /// </summary>
        /// <param name="nIndex">索引</param>
        public void ThisRemoveAt(int nIndex)
        {
            //检查
            if (null == this.Children)
            {
                return;
            }
            if ((0x00 > nIndex)
                || (nIndex >= this.Children.Count))
            {
                return;
            }

            //移除操作
            this.Children.RemoveAt(nIndex);
        }

        /// <summary>
        /// 元素清理
        /// </summary>
        public void ThisClear()
        {
            //检查
            if (null == this.Children)
            {
                return;
            }

            //清除操作
            this.Children.Clear();
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
                lstDrawDataSet = this.Children;
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
        /// 对象相等判断
        /// </summary>
        /// <param name="oEntity">行为体</param>
        /// <returns></returns>
        public override bool Equals(FaultTreeAnalysis.Behavior.ObjectBehavior oEntity)
        {
            //对象非空
            DrawData oOther = oEntity as DrawData;
            if (null == oOther)
            {
                throw new NullReferenceException("Parameter oOther is null.");
            }

            //属性比较

            //浅表对象
            if (this.causPolynomialID != oOther.causPolynomialID)
            {
                return false;
            }
            if (this.Level != oOther.Level)
            {
                return false;
            }
            if (this.IsEffective != oOther.IsEffective)
            {
                return false;
            }
            if (this.Effect != oOther.Effect)
            {
                return false;
            }
            if (this.X != oOther.X)
            {
                return false;
            }
            if (this.Y != oOther.Y)
            {
                return false;
            }
            if (this.ChildOffsetX != oOther.ChildOffsetX)
            {
                return false;
            }
            if (this.Identifier != oOther.Identifier)
            {
                return false;
            }
            if (this.Type != oOther.Type)
            {
                return false;
            }
            if (this.ParentID != oOther.ParentID)
            {
                return false;
            }
            if (this.Comment1 != oOther.Comment1)
            {
                return false;
            }
            if (this.LogicalCondition != oOther.LogicalCondition)
            {
                return false;
            }
            if (this.InputType != oOther.InputType)
            {
                return false;
            }
            if (this.FRType != oOther.FRType)
            {
                return false;
            }
            if (this.ExposureTimePercentage != oOther.ExposureTimePercentage)
            {
                return false;
            }
            if (this.DormancyFactor != oOther.DormancyFactor)
            {
                return false;
            }
            if (this.FRPercentage != oOther.FRPercentage)
            {
                return false;
            }
            if (this.inputValue != oOther.inputValue)
            {
                return false;
            }
            if (this.inputValue2 != oOther.inputValue2)
            {
                return false;
            }
            if (this.Units != oOther.Units)
            {
                return false;
            }
            if (this.ProblemList != oOther.ProblemList)
            {
                return false;
            }
            if (this.ExtraValue1 != oOther.ExtraValue1)
            {
                return false;
            }
            if (this.ExtraValue2 != oOther.ExtraValue2)
            {
                return false;
            }
            if (this.ExtraValue3 != oOther.ExtraValue3)
            {
                return false;
            }
            if (this.ExtraValue4 != oOther.ExtraValue4)
            {
                return false;
            }
            if (this.ExtraValue5 != oOther.ExtraValue5)
            {
                return false;
            }
            if (this.ExtraValue6 != oOther.ExtraValue6)
            {
                return false;
            }
            if (this.ExtraValue7 != oOther.ExtraValue7)
            {
                return false;
            }
            if (this.ExtraValue8 != oOther.ExtraValue8)
            {
                return false;
            }
            if (this.ExtraValue9 != oOther.ExtraValue9)
            {
                return false;
            }
            if (this.ExtraValue10 != oOther.ExtraValue10)
            {
                return false;
            }
            if (this.ExtraValue11 != oOther.ExtraValue11)
            {
                return false;
            }
            if (this.Repeats != oOther.Repeats)
            {
                return false;
            }
            if (this.QValue != oOther.QValue)
            {
                return false;
            }
            if (this.Units != oOther.Units)
            {
                return false;
            }

            //较深层对象
            if (!(this.Cutset.Equals(oOther.Cutset)))
            {
                return false;
            }

            //深层对象 Parent
            {
                //基本比较
                if (null == this.Parent)
                {
                    return (null == oOther.Parent);
                }
                if (null == oOther.Parent)
                {
                    return false;
                }
                if (ReferenceEquals(this.Parent, oOther.Parent))
                {
                    return true;
                }
            }

            //深层对象 Children
            {
                return new Behavior.Common.MultiSetComparer<DrawData>()
                    .Equals(oOther.Children, this.Children);
            }

            //相等
            return true;
        }


        /// <summary>
        /// 标注
        /// </summary>
        /// <param name="oEntity">标注对象</param>
        /// <returns>受影响数</returns>
        public override int Remark(FaultTreeAnalysis.Behavior.ObjectBehavior oEntity)
        {
            //对象非空
            if (null == oEntity)
            {
                throw new NullReferenceException("Parameter oEntity is null.");
            }

            //行为对象体
            DrawData oOther = oEntity as DrawData;
            if (null == oOther)
            {
                throw new NullReferenceException("Parameter oOther is null.");
            }

            //浅表对象
            if (this.causPolynomialID != oOther.causPolynomialID)
            {
                Remarks.Add(nameof(causPolynomialID));
            }
            if (this.Level != oOther.Level)
            {
                Remarks.Add(nameof(Level));
            }
            if (this.IsEffective != oOther.IsEffective)
            {
                Remarks.Add(nameof(IsEffective));
            }
            if (this.Effect != oOther.Effect)
            {
                Remarks.Add(nameof(Effect));
            }
            if (this.X != oOther.X)
            {
                Remarks.Add(nameof(X));
            }
            if (this.Y != oOther.Y)
            {
                Remarks.Add(nameof(Y));
            }
            if (this.ChildOffsetX != oOther.ChildOffsetX)
            {
                Remarks.Add(nameof(ChildOffsetX));
            }
            if (this.Identifier != oOther.Identifier)
            {
                Remarks.Add(nameof(Identifier));
            }
            if (this.Type != oOther.Type)
            {
                Remarks.Add(nameof(Type));
            }
            if (this.ParentID != oOther.ParentID)
            {
                Remarks.Add(nameof(ParentID));
            }
            if (this.Comment1 != oOther.Comment1)
            {
                Remarks.Add(nameof(Comment1));
            }
            if (this.LogicalCondition != oOther.LogicalCondition)
            {
                Remarks.Add(nameof(LogicalCondition));
            }
            if (this.InputType != oOther.InputType)
            {
                Remarks.Add(nameof(InputType));
            }
            if (this.FRType != oOther.FRType)
            {
                Remarks.Add(nameof(FRType));
            }
            if (this.ExposureTimePercentage != oOther.ExposureTimePercentage)
            {
                Remarks.Add(nameof(ExposureTimePercentage));
            }
            if (this.DormancyFactor != oOther.DormancyFactor)
            {
                Remarks.Add(nameof(DormancyFactor));
            }
            if (this.FRPercentage != oOther.FRPercentage)
            {
                Remarks.Add(nameof(FRPercentage));
            }
            if (this.inputValue != oOther.inputValue)
            {
                Remarks.Add(nameof(inputValue));
            }
            if (this.inputValue2 != oOther.inputValue2)
            {
                Remarks.Add(nameof(inputValue2));
            }
            if (this.Units != oOther.Units)
            {
                Remarks.Add(nameof(Units));
            }
            if (this.ProblemList != oOther.ProblemList)
            {
                Remarks.Add(nameof(ProblemList));
            }
            if (this.ExtraValue1 != oOther.ExtraValue1)
            {
                Remarks.Add(nameof(ExtraValue1));
            }
            if (this.ExtraValue2 != oOther.ExtraValue2)
            {
                Remarks.Add(nameof(ExtraValue2));
            }
            if (this.ExtraValue3 != oOther.ExtraValue3)
            {
                Remarks.Add(nameof(ExtraValue3));
            }
            if (this.ExtraValue4 != oOther.ExtraValue4)
            {
                Remarks.Add(nameof(ExtraValue4));
            }
            if (this.ExtraValue5 != oOther.ExtraValue5)
            {
                Remarks.Add(nameof(ExtraValue5));
            }
            if (this.ExtraValue6 != oOther.ExtraValue6)
            {
                Remarks.Add(nameof(ExtraValue6));
            }
            if (this.ExtraValue7 != oOther.ExtraValue7)
            {
                Remarks.Add(nameof(ExtraValue7));
            }
            if (this.ExtraValue8 != oOther.ExtraValue8)
            {
                Remarks.Add(nameof(ExtraValue8));
            }
            if (this.ExtraValue9 != oOther.ExtraValue9)
            {
                Remarks.Add(nameof(ExtraValue9));
            }
            if (this.ExtraValue10 != oOther.ExtraValue10)
            {
                Remarks.Add(nameof(ExtraValue10));
            }
            if (this.ExtraValue11 != oOther.ExtraValue11)
            {
                Remarks.Add(nameof(ExtraValue11));
            }
            if (this.Repeats != oOther.Repeats)
            {
                Remarks.Add(nameof(Repeats));
            }
            if (this.QValue != oOther.QValue)
            {
                Remarks.Add(nameof(QValue));
            }
            if (this.Units != oOther.Units)
            {
                Remarks.Add(nameof(Units));
            }

            //较深层对象
            if (!(this.Cutset.Equals(oOther.Cutset)))
            {
                Remarks.Add(nameof(Cutset));
            }

            //深层对象 Parent
            {
                //基本比较
                //if (null == this.Parent)
                //{
                //    return (null == oOther.Parent);
                //}
                //if (null == oOther.Parent)
                //{
                //    return false;
                //}
                //if (ReferenceEquals(this.Parent, oOther.Parent))
                //{
                //    return true;
                //}
            }

            //深层对象 Children
            {
                //return new Behavior.Common.MultiSetComparer<DrawData>()
                //    .Equals(oOther.Children, this.Children);
            }

            //
            return Remarks.Count;
        }


        /// <summary>
        /// 通过一个对象更新对象
        /// </summary>
        /// <param name="oOther">参照更新对象</param>
        /// <param name="hsUpdateRemark">更新标记</param>
        /// <returns></returns>
        public virtual bool ThisUpdateBy(DrawData oOther, HashSet<string> hsUpdateRemark)
        {
            //对象非空
            if (null == oOther)
            {
                throw new NullReferenceException("Parameter oOther is null.");
            }
            if (null == hsUpdateRemark)
            {
                throw new NullReferenceException("Parameter hsUpdateRemark is null.");
            }

            //更新属性
            if (hsUpdateRemark.Contains(nameof(Identifier)))
            {
                this.Identifier = (oOther.Identifier?.Substring(0x00));
            }
            if (hsUpdateRemark.Contains(nameof(ParentID)))
            {
                //此处更新忽略, 已由Parent字段代更
                //this.ParentID = (oOther.ParentID?.Substring(0x00));
            }
            if (hsUpdateRemark.Contains(nameof(Repeats)))
            {
                //此处更新忽略, 已由其事件代更
                //this.Repeats = oOther.Repeats;
            }
            if (hsUpdateRemark.Contains(nameof(Type)))
            {
                //转移们入口类型不变
                if (Model.Enum.DrawType.TransferInGate != this.Type)
                {
                    this.Type = oOther.Type;
                }
            }

            //浅表对象
            if (hsUpdateRemark.Contains(nameof(X)))
            {
                //此处更新忽略, 已由其事件代更
                //this.X = oOther.X;
            }
            if (hsUpdateRemark.Contains(nameof(Y)))
            {
                //此处更新忽略, 已由其事件代更
                //this.Y = oOther.Y;
            }
            if (hsUpdateRemark.Contains(nameof(causPolynomialID)))
            {
                this.causPolynomialID = oOther.causPolynomialID;
            }
            if (hsUpdateRemark.Contains(nameof(Level)))
            {
                this.Level = oOther.Level;
            }
            if (hsUpdateRemark.Contains(nameof(IsEffective)))
            {
                this.IsEffective = oOther.IsEffective;
            }
            if (hsUpdateRemark.Contains(nameof(Effect)))
            {
                this.Effect = oOther.Effect;
            }
            if (hsUpdateRemark.Contains(nameof(ChildOffsetX)))
            {
                this.ChildOffsetX = oOther.ChildOffsetX;
            }
            if (hsUpdateRemark.Contains(nameof(Comment1)))
            {
                this.Comment1 = oOther.Comment1?.Substring(0x00);
            }
            if (hsUpdateRemark.Contains(nameof(LogicalCondition)))
            {
                this.LogicalCondition = oOther.LogicalCondition?.Substring(0x00);
            }
            if (hsUpdateRemark.Contains(nameof(InputType)))
            {
                this.InputType = oOther.InputType?.Substring(0x00);
            }
            if (hsUpdateRemark.Contains(nameof(FRType)))
            {
                this.FRType = oOther.FRType?.Substring(0x00);
            }
            if (hsUpdateRemark.Contains(nameof(ExposureTimePercentage)))
            {
                this.ExposureTimePercentage = oOther.ExposureTimePercentage?.Substring(0x00);
            }
            if (hsUpdateRemark.Contains(nameof(DormancyFactor)))
            {
                this.DormancyFactor = oOther.DormancyFactor?.Substring(0x00);
            }
            if (hsUpdateRemark.Contains(nameof(FRPercentage)))
            {
                this.FRPercentage = oOther.FRPercentage?.Substring(0x00);
            }
            if (hsUpdateRemark.Contains(nameof(inputValue)))
            {
                this.inputValue = oOther.inputValue?.Substring(0x00);
            }
            if (hsUpdateRemark.Contains(nameof(inputValue2)))
            {
                this.inputValue2 = oOther.inputValue2?.Substring(0x00);
            }
            if (hsUpdateRemark.Contains(nameof(Units)))
            {
                this.Units = oOther.Units?.Substring(0x00);
            }
            if (hsUpdateRemark.Contains(nameof(ProblemList)))
            {
                this.ProblemList = oOther.ProblemList?.Substring(0x00);
            }
            if (hsUpdateRemark.Contains(nameof(ExtraValue1)))
            {
                this.ExtraValue1 = oOther.ExtraValue1?.Substring(0x00);
            }
            if (hsUpdateRemark.Contains(nameof(ExtraValue2)))
            {
                this.ExtraValue2 = oOther.ExtraValue2?.Substring(0x00);
            }
            if (hsUpdateRemark.Contains(nameof(ExtraValue3)))
            {
                this.ExtraValue3 = oOther.ExtraValue3?.Substring(0x00);
            }
            if (hsUpdateRemark.Contains(nameof(ExtraValue4)))
            {
                this.ExtraValue4 = oOther.ExtraValue4?.Substring(0x00);
            }
            if (hsUpdateRemark.Contains(nameof(ExtraValue5)))
            {
                this.ExtraValue5 = oOther.ExtraValue5?.Substring(0x00);
            }
            if (hsUpdateRemark.Contains(nameof(ExtraValue6)))
            {
                this.ExtraValue6 = oOther.ExtraValue6?.Substring(0x00);
            }
            if (hsUpdateRemark.Contains(nameof(ExtraValue7)))
            {
                this.ExtraValue7 = oOther.ExtraValue7?.Substring(0x00);
            }
            if (hsUpdateRemark.Contains(nameof(ExtraValue8)))
            {
                this.ExtraValue8 = oOther.ExtraValue8?.Substring(0x00);
            }
            if (hsUpdateRemark.Contains(nameof(ExtraValue9)))
            {
                this.ExtraValue9 = oOther.ExtraValue9?.Substring(0x00);
            }
            if (hsUpdateRemark.Contains(nameof(ExtraValue10)))
            {
                this.ExtraValue10 = oOther.ExtraValue10?.Substring(0x00);
            }
            if (hsUpdateRemark.Contains(nameof(ExtraValue11)))
            {
                this.ExtraValue11 = oOther.ExtraValue11?.Substring(0x00);
            }
            if (hsUpdateRemark.Contains(nameof(QValue)))
            {
                this.QValue = oOther.QValue?.Substring(0x00);
            }
            if (hsUpdateRemark.Contains(nameof(Units)))
            {
                this.Units = oOther.Units?.Substring(0x00);
            }

            //较深层对象
            //if (!(this.Cutset.Equals(oOther.Cutset)))
            if (hsUpdateRemark.Contains(nameof(Cutset)))
            {
                //创建并复制克隆属性
                this.Cutset = (CutsetModel)(oOther.Cutset.Clone(CloneAction.Extreme));
            }

            //深层对象
            {

            }

            //
            return true;
        }


        /// <summary>
        /// 根据克隆动作克隆对象
        /// </summary>
        /// <param name="enCloneAction">克隆动作</param>
        /// <returns>返回对象的克隆体</returns>
        public override object Clone(
            CloneAction enCloneAction
            //= (
            //  CloneAction.Shallow)
            )
        {
            //影子克隆(过程中没有实体产生)
            if (CloneAction.Shadow == (CloneAction.Shadow & enCloneAction))
            {
                return ThisShadowCloneAction();
            }

            //对象克隆体
            DrawData oClone = null;

            //克隆行为关系
            if (CloneAction.Shallow == (CloneAction.Shallow & enCloneAction))
            {
                //浅表克隆(产出克隆体)
                oClone = ThisShallowCloneAction();

                //级联克隆
                if (CloneAction.Beyond == (CloneAction.Beyond & enCloneAction))
                {
                    ThisBeyondCloneAction(oClone);
                }

                //深层克隆
                if (CloneAction.Extreme == (CloneAction.Extreme & enCloneAction))
                {
                    ThisExtremeCloneAction(oClone);
                }
            }

            //
            return oClone;
        }


        /// <summary>
        /// 克隆对象的引用对象体
        /// </summary>
        /// <returns>返回对象的克隆体</returns>
        private DrawData ThisShadowCloneAction()
        {
            //对象非空
            //if (null == oClone)
            {
                //throw new NullReferenceException("Parameter oClone is null.");
            }

            //影子克隆
            //oClone = this;

            //
            return this;
        }

        /// <summary>
        /// 克隆对象的深度数据对象体
        /// </summary>
        /// <param name="oClone">对象克隆体</param>
        /// <returns>返回克隆体</returns>
        private DrawData ThisExtremeCloneAction(DrawData oClone)
        {
            //对象非空
            if (null == oClone)
            {
                throw new NullReferenceException("Parameter oClone is null.");
            }

            //深层对象
            if (null != this.Children)
            {
                //创建并复制克隆属性
                oClone.Children = new List<DrawData>(this.Children.Capacity);
                List<DrawData> lstCloneSource = new List<DrawData>(this.Children);

                //遍历被克隆对象数据
                foreach (DrawData item in lstCloneSource)
                {
                    oClone.Children.Add((DrawData)item.Clone(CloneAction.Major));
                }
            }
            else
            {
                oClone.Children = null; // new List<DrawData>();
            }

            //
            return oClone;
        }

        /// <summary>
        /// 克隆对象的级联对象体
        /// </summary>
        /// <param name="oClone">克隆体对象</param>
        /// <returns>返回克隆体</returns>
        private DrawData ThisBeyondCloneAction(DrawData oClone)
        {
            //对象非空
            if (null == oClone)
            {
                throw new NullReferenceException("Parameter oClone is null.");
            }

            //深层对象
            if (null != this.Parent)
            {
                //1.当前对象的级联 -> 影子克隆
                {
                    oClone.Parent = (DrawData)(this.Parent.Clone(CloneAction.Shadow));
                }
                //2.当前对象的级联对象 -> 非影子克隆
                {
                    //太复杂, 暂略
                }
            }
            else
            {
                oClone.Parent = null;
            }

            //
            return oClone;
        }

        /// <summary>
        /// 产生包含对象浅表数据的克隆体
        /// </summary>
        /// <returns>返回克隆体<</returns>
        private DrawData ThisShallowCloneAction()
        {
            //浅表数据的克隆对象
            DrawData oClone = new DrawData();
            //克隆体继承被克隆对象的Guid
            oClone.ThisGuid = this.ThisGuid;

            //ParentID字段值由Parent字段决定
            //oClone.ParentID = (this.ParentID?.Substring(0x00));

            oClone.causPolynomialID = this.causPolynomialID;
            oClone.Level = this.Level;
            oClone.IsEffective = this.IsEffective;
            oClone.Effect = this.Effect;
            oClone.X = this.X;
            oClone.Y = this.Y;
            oClone.ChildOffsetX = this.ChildOffsetX;
            oClone.Identifier = (this.Identifier?.Substring(0x00));
            oClone.Type = this.Type;
            oClone.Comment1 = this.Comment1?.Substring(0x00);
            oClone.LogicalCondition = this.LogicalCondition?.Substring(0x00);
            oClone.InputType = this.InputType?.Substring(0x00);
            oClone.FRType = this.FRType?.Substring(0x00);
            oClone.ExposureTimePercentage = this.ExposureTimePercentage?.Substring(0x00);
            oClone.DormancyFactor = this.DormancyFactor?.Substring(0x00);
            oClone.FRPercentage = this.FRPercentage?.Substring(0x00);
            oClone.inputValue = this.inputValue?.Substring(0x00);
            oClone.inputValue2 = this.inputValue2?.Substring(0x00);
            oClone.Units = this.Units?.Substring(0x00);
            oClone.ProblemList = this.ProblemList?.Substring(0x00);
            oClone.ExtraValue1 = this.ExtraValue1?.Substring(0x00);
            oClone.ExtraValue2 = this.ExtraValue2?.Substring(0x00);
            oClone.ExtraValue3 = this.ExtraValue3?.Substring(0x00);
            oClone.ExtraValue4 = this.ExtraValue4?.Substring(0x00);
            oClone.ExtraValue5 = this.ExtraValue5?.Substring(0x00);
            oClone.ExtraValue6 = this.ExtraValue6?.Substring(0x00);
            oClone.ExtraValue7 = this.ExtraValue7?.Substring(0x00);
            oClone.ExtraValue8 = this.ExtraValue8?.Substring(0x00);
            oClone.ExtraValue9 = this.ExtraValue9?.Substring(0x00);
            oClone.ExtraValue10 = this.ExtraValue10?.Substring(0x00);
            oClone.ExtraValue11 = this.ExtraValue11?.Substring(0x00);
            oClone.Repeats = this.Repeats;
            oClone.QValue = this.QValue?.Substring(0x00);
            oClone.Units = this.Units?.Substring(0x00);

            //较深层对象
            if (null != this.Cutset)
            {
                //创建并复制克隆属性
                oClone.Cutset = (CutsetModel)(this.Cutset.Clone(CloneAction.Extreme));
            }
            else
            {
                oClone.Cutset = null;
            }

            //
            return oClone;
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


        [Obsolete("")]
        private void BehaveBeforePropertyChanged<T>(string strPropertyName, T tNewValue, T tOldValue)
        {
            //检查
            if ((null == tNewValue) ||
                (null == tOldValue))
            {
                return;
            }

            //限制条件：属性值有变化
            if (tNewValue.Equals(tOldValue))
            {
                return;
            }

            //项目初始化后


            //此路不同
            //...
            return;
        }
    }
}
