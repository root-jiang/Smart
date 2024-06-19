using FaultTreeAnalysis.Common;
using FaultTreeAnalysis.Model.Data;
using FaultTreeAnalysis.Model.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;

namespace FaultTreeAnalysis
{
    /// <summary>
    ///整个FTA的核心数据类,用于每个图形元素以及表的数据显示
    /// </summary>
    public partial class DrawData
    {
        private DrawData parent;

        //致因关系解析需要存储的字段
        public int causPolynomialID;

        [JsonIgnore]
        public int Level { get; set; } = -1;

        [JsonIgnore]
        public bool IsEffective { get; set; }

        public Effect Effect { get; set; } = Effect.Ineffective;

        /// <summary>
        /// 数据的父对象
        /// </summary>
        [JsonIgnore]
        public DrawData Parent
        {
            get { return this.parent; }
            set
            {
                this.parent = value;
                if (value != null)
                {
                    this.ParentID = this.parent.Identifier;
                }
                else this.ParentID = null;
            }
        }

        /// <summary>
        /// 数据的子对象集合
        /// </summary>
        public List<DrawData> Children { get; set; }

        /// <summary>
        /// 用于存放布局后对象的X位置值
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// 用于存放布局后对象的Y位置值
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// 用于树布局的临时变量
        /// </summary>
        [JsonIgnore]
        private int ChildOffsetX { get; set; }

        #region FTA表里的可见字段        
        /// <summary>
        /// 事件对象的分组
        /// </summary>
        [Description(nameof(Group))]
        public string Group = string.Empty;

        /// <summary>
        /// 事件对象的链接故障树名称
        /// </summary>
        [Description(nameof(LinkPath))]
        public string LinkPath = string.Empty;

        /// <summary>
        /// 事件对象的GUID
        /// </summary>
        [Description(nameof(GUID))]
        public string GUID = string.Empty;
        /// <summary>
        /// 对象的ID，除转移门重复事件，默认是不重复的
        /// </summary>
        [Description(nameof(Identifier))]
        public string Identifier { get; set; }

        /// <summary>
        /// 对象的类型，该枚举决定图形绘制时的样子
        /// </summary>
        [Description(nameof(Type))]
        public DrawType Type { get; set; }

        /// <summary>
        ///旧的ASPect项目导入或excel导入等时需要用的
        /// </summary>
        [Description(nameof(ParentID))]
        public string ParentID { get; set; }

        /// <summary>
        /// 描述字段，用于图形绘制时上边框的显示字符串
        /// </summary>
        [Description(nameof(Comment1))]
        public string Comment1 { get; set; }

        /// <summary>
        /// 用于设置真假门/事件颜色时用到
        /// </summary>
        [Description(nameof(LogicalCondition))]
        public string LogicalCondition { get; set; }

        /// <summary>
        /// 和Mode是同一个字段
        /// </summary>
        [Description(nameof(InputType))]
        public string InputType { get; set; }

        /// <summary>
        /// 失效概率类型
        /// </summary>
        [Description(nameof(FRType))]
        public string FRType { get; set; }

        /// <summary>
        /// 曝光时间比例
        /// </summary>
        [Description(nameof(ExposureTimePercentage))]
        public string ExposureTimePercentage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Description(nameof(DormancyFactor))]
        public string DormancyFactor { get; set; }

        [Description(nameof(FRPercentage))]
        public string FRPercentage { get; set; }

        private string inputValue = string.Empty;
        /// <summary>
        /// 概率值，用于FTA计算
        /// </summary>
        [Description(nameof(InputValue))]
        public string InputValue
        {
            get
            {
                return inputValue;
            }
            set
            {
                if (value != null)
                {
                    decimal a;

                    if (value == "")
                    {
                        this.inputValue = "";
                    }
                    else if (decimal.TryParse(value, System.Globalization.NumberStyles.Float, null, out a) && a >= 0)
                    {
                        this.inputValue = a.ToString(FixedString.SCIENTIFIC_NOTATION_FORMAT);
                    }
                }
            }
        }

        private string inputValue2 = string.Empty;
        /// <summary>
        /// 时间值，用于FTA计算
        /// </summary>
        [Description(nameof(InputValue2))]
        public string InputValue2
        {
            get
            {
                return inputValue2;
            }
            set
            {
                if (value != null)
                {
                    decimal temporary;

                    if (value == "")
                    {
                        this.inputValue2 = "";
                    }
                    else if (decimal.TryParse(value, System.Globalization.NumberStyles.Float, null, out temporary) && temporary >= 0)
                    {
                        this.inputValue2 = temporary.ToString();
                    }
                }
            }
        }

        /// <summary>
        /// 时间的单位，小时或秒
        /// </summary>
        [Description(nameof(Units))]
        public string Units { get; set; }

        [Description(nameof(ProblemList))]
        public string ProblemList { get; set; }
        #endregion

        #region FTA表可配置字段(扩展字段)，用户可修改字段显示名字
        [Description(nameof(ExtraValue1))]
        public string ExtraValue1 { get; set; }

        [Description(nameof(ExtraValue2))]
        public string ExtraValue2 { get; set; }

        [Description(nameof(ExtraValue3))]
        public string ExtraValue3 { get; set; }

        [Description(nameof(ExtraValue4))]
        public string ExtraValue4 { get; set; }

        [Description(nameof(ExtraValue5))]
        public string ExtraValue5 { get; set; }

        [Description(nameof(ExtraValue6))]
        public string ExtraValue6 { get; set; }

        [Description(nameof(ExtraValue7))]
        public string ExtraValue7 { get; set; }

        [Description(nameof(ExtraValue8))]
        public string ExtraValue8 { get; set; }

        [Description(nameof(ExtraValue9))]
        public string ExtraValue9 { get; set; }

        [Description(nameof(ExtraValue10))]
        public string ExtraValue10 { get; set; }

        [Description(nameof(ExtraValue11))]//用于导出FTAReport画图显示转页信息
        public string ExtraValue11 { get; set; }
        #endregion

        /// <summary>
        /// 基本事件重复的次数，绘制颜色时要用到（表和图都用到）
        /// </summary>
        public int Repeats { get; set; }

        #region 判断是否是门，能否成为重复事件
        /// <summary>
        /// 该对象类型是否是门中的一种（转移门也是）或Null，Label，否则是事件
        /// </summary>
        [JsonIgnore]
        public bool IsGateType =>
         !(this.Type.Equals(DrawType.BasicEvent)
         || this.Type.Equals(DrawType.ConditionEvent)
         || this.Type.Equals(DrawType.HouseEvent)
         || this.Type.Equals(DrawType.UndevelopedEvent));

        /// <summary>
        /// 该对象的类型是否是重复事件中的一种（能否成为重复事件）
        /// </summary>
        [JsonIgnore]
        public bool CanRepeatedType =>
         (this.Type.Equals(DrawType.BasicEvent)
         || this.Type.Equals(DrawType.HouseEvent)
         || this.Type.Equals(DrawType.UndevelopedEvent));
        #endregion

        [JsonIgnore]
        public bool HasChild => this.Children?.Count > 0 ? true : false;

        private string qValue = string.Empty;
        /// <summary>
        /// FTA计算时要用到，存放每个对象的计算后的概率值，图形上下面矩形框里字符显示
        /// </summary>
        public string QValue
        {
            get
            {
                var result = string.Empty;
                if (this.IsGateType == false)
                {
                    double rate;
                    double time;
                    if (double.TryParse(this.InputValue, out rate) && double.TryParse(this.InputValue2, out time))
                    {
                        time = (this.Units == nameof(StringModel.Minute) || this.Units == "分钟") ? time / 60 : time;

                        if (this.InputType != null && this.InputType == General.FtaProgram.String.FailureProbability)
                        {
                            double d = 1 - Math.Exp(-rate * time);
                            result = d.ToString("E");
                        }
                        else
                        {
                            result = (rate * time).ToString("E");
                        }
                    }
                }
                else
                {
                    result = qValue;
                }
                return result;
            }
            set { qValue = value; }
        }

        [JsonIgnore]
        /// <summary>
        /// 割集对象
        /// </summary>
        public CutsetModel Cutset { get; set; }

        /// <summary>
        /// 初始化方法，默认或门等
        /// </summary>
        public DrawData()
        {
            Parent = null;
            Children = new List<DrawData>();
            Type = DrawType.OrGate;
            ExposureTimePercentage = "100.00";
            FRPercentage = "100.00";
            LogicalCondition = FixedString.LOGICAL_CONDITION_NORMAL;
            Cutset = new CutsetModel();
        }

        /// <summary>
        /// 判断是否是某个数据对象的儿子(根据Parent属性找),传入自己返回false(不包括转移门处理)
        /// </summary>
        /// <param name="parent">要判断的"父"对象</param>
        /// <returns>当前对象是否是某元素的子节点</returns>
        public bool IsChildOfParent(DrawData parent)
        {
            DrawData child = this;
            List<DrawData> parents = new List<DrawData>();
            while (child != null)
            {
                if (parents.Contains(child))
                {
                    //防止无限循环查找
                    return false;
                }
                parents.Add(child);
                child = child.Parent;
                if (child == parent)
                {
                    return true;
                }
            }
            return false;
        }


        private List<DrawData> GetAllData(DrawData current)
        {
            var result = new List<DrawData>();
            if (current != null)
            {
                result.Add(current);
                if (current.Children != null)
                {
                    foreach (DrawData child in current.Children) result.AddRange(GetAllData(child));
                }
            }
            return result;
        }

        /// <summary>
        /// 获取从现有对象开始一直到某个父对象经过的所有对象集合（路径）(根据Parent属性找),传入自己返回自己，路径包括自己和父对象,不存在/错误返回null
        /// </summary>
        /// <param name="parent">路径的结束端"父"对象</param>
        /// <param name="TransInGate">转移门集合</param>
        /// <returns>2个具有父子关系的对象间的数据对象集合</returns>
        public List<DrawData> GetPath(DrawData parent, Dictionary<string, HashSet<DrawData>> TransInGate = null)
        {
            List<DrawData> parents = null;
            DrawData child = this;
            //如果是自己
            if (child == parent) return new List<DrawData>() { parent };
            parents = new List<DrawData>();
            while (child != null)
            {
                if (parents.Contains(child))
                {
                    //防止无限循环查找
                    return null;
                }
                parents.Add(child);
                child = child.Parent;
                if (child == parent)
                {
                    parents.Add(child);
                    return parents;
                }
            }
            //到头了还没找到
            //要处理转移门本体的话
            DrawData top = parents[parents.Count - 1];
            if (TransInGate != null && parents.Count > 0 && TransInGate.ContainsKey(top.Identifier) && top.Type != DrawType.TransferInGate)
            {
                HashSet<DrawData> trans = TransInGate[top.Identifier];
                if (trans != null && trans.Count > 0)
                {
                    foreach (var item in trans)
                    {
                        List<DrawData> path = item.GetPath(parent);
                        if (path != null && path.Count > 0)
                        {
                            parents.AddRange(path);
                            return parents;
                        }
                    }
                }
            }
            return parents;
        }

        /// <summary>
        /// 获取最大的根节点对象(根据Parent属性找)只有自己时返回自己，错误返回null（不包括转移门处理）
        /// </summary>
        /// <returns>当前对象的根节点数据对象</returns>
        public DrawData GetRoot()
        {
            DrawData child = this;
            List<DrawData> parents = new List<DrawData>();
            while (child != null)
            {
                if (parents.Contains(child))
                {
                    // 防止无限循环查找
                    return null;
                }
                parents.Add(child);
                if (child.parent == null)
                {
                    return child;
                }
                child = child.Parent;
            }
            return null;
        }

        #region 用于FTA的重复事件和转移门等规则的对象复制等操作
        /// <summary>
        /// 从另一个对象中复制数据（用于转移门对象间，重复事件对象间信息同步）
        /// </summary>
        /// <param name="source">提供数据的对象</param>
        public void CopyIntoTransferOrRepeatedEvent(DrawData source)
        {
            if (source != null)
            {
                Type = source.IsGateType ? DrawType.TransferInGate : source.Type; //source.Type;
                Identifier = source.Identifier;
                Comment1 = source.Comment1;
                LogicalCondition = source.LogicalCondition;
                InputType = source.InputType;
                FRType = source.FRType;
                ExposureTimePercentage = source.ExposureTimePercentage;
                DormancyFactor = source.DormancyFactor;
                FRPercentage = source.FRPercentage;
                InputValue = source.InputValue;
                InputValue2 = source.InputValue2;
                Units = source.Units;
                ProblemList = source.ProblemList;
                Repeats = source.Repeats;
                QValue = source.QValue;
                if (source.IsGateType == false) Console.WriteLine($"Name:{source.Identifier}--Type:{source.Type}--Value:{source.QValue}");

                ExtraValue1 = source.ExtraValue1;
                ExtraValue2 = source.ExtraValue2;
                ExtraValue3 = source.ExtraValue3;
                ExtraValue4 = source.ExtraValue4;
                ExtraValue5 = source.ExtraValue5;
                ExtraValue6 = source.ExtraValue6;
                ExtraValue7 = source.ExtraValue7;
                ExtraValue8 = source.ExtraValue8;
                ExtraValue9 = source.ExtraValue9;
                ExtraValue10 = source.ExtraValue10;
            }
        }

        /// <summary>
        /// 递归复制指定对象的副本(根据Chiren属性递归复制)，保留转移门以及能成为重复事件的事件id(参数决定是否保留)，其他依据现有id前缀递增后缀
        /// </summary>
        /// <param name="ids">所有数据对象不同id的列表</param>
        /// <param name="IsReserveEventsID">是否保留能成为重复事件的事件id，用于粘贴为重复事件菜单</param>
        /// <returns>当前对象的一个副本（包含子节点）</returns>
        public DrawData CopyDrawDataRecurse(HashSet<string> ids, bool IsReserveEventsID)
        {
            DrawData data = this;
            if (data == null) return null;
            DrawData target = null;
            target = data.CopyDrawData();
            if (target != null)
            {
                if (target.Repeats != 0) target.Repeats = 0;
                if (!(data.Type == DrawType.TransferInGate || (IsReserveEventsID && CanRepeatedType)))
                {
                    target.Identifier = GetNewID(ids, data.Identifier);
                    ids.Add(target.Identifier);//这里是否需要，为了省事不想太多了。
                }
            }

            if (data.Children != null && data.Children.Count > 0)
            {
                foreach (DrawData child in data.Children)
                {
                    DrawData target_child = child.CopyDrawDataRecurse(ids, IsReserveEventsID);
                    target_child.Parent = target;
                    target.Children.Add(target_child);
                }
            }
            return target;
        }

        /// <summary>
        /// 复制单个DrawData对象，保留转移门,以及能成为重复事件的事件id(参数决定是否保留)，其他根据现有id前缀递增后缀
        /// </summary>
        /// <param name="ids">所有数据对象不同id的列表</param>
        /// <param name="IsReserveEventsID">是否保留能成为重复事件的事件id，用于粘贴为重复事件菜单</param>
        /// <returns>当前对象的副本（不包含子节点）</returns>
        public DrawData CopyDrawData(HashSet<string> ids, bool IsReserveEventsID)
        {
            DrawData data = this;
            if (data != null)
            {
                DrawData newData = data.CopyDrawData();
                if (newData != null)
                {
                    if (newData.Repeats != 0) newData.Repeats = 0;
                    if (!(data.Type == DrawType.TransferInGate || (IsReserveEventsID && CanRepeatedType)))
                    {
                        newData.Identifier = GetNewID(ids, data.Identifier);
                    }
                    return newData;
                }
            }
            return null;
        }

        /// <summary>
        /// 根据给出的id集合，按指定前缀字符（自动多加_），获取下一个不重复的最大id值（后缀）
        /// </summary>
        /// <param name="ids">所有数据对象不同id的列表</param>
        /// <param name="prefix">指定id前缀</param>
        /// <returns>下一个不重复ID</returns>
        public string GetNewID(HashSet<string> ids, string prefix)
        {
            string newID = null;
            int ID_Suffix = 1;
            if (ids != null && ids.Count > 0)
            {
                while (ids.Contains(prefix + "_" + ID_Suffix))
                {
                    ID_Suffix++;
                }
            }
            newID = prefix + "_" + ID_Suffix;
            return newID;
        }

        /// <summary>
        /// 根据给出的id集合,按固定门和事件的前缀，获取下一个不重复的最大id值（后缀）
        /// </summary>
        /// <param name="ids">所有数据对象不同id的列表</param>
        /// <param name="type">类型</param>
        /// <returns>下一个不重复的最大id值</returns>
        public string GetNewID(HashSet<string> ids, DrawType type)
        {
            int gateID = 1;
            int eventID = 1;
            return GetNewID(ids, type, ref gateID, ref eventID);
        }

        /// <summary>
        /// 根据给出的id集合,固定门和事件的前缀，获取下一个不重复的最大id值（后缀）
        /// </summary>
        /// <param name="ids">所有数据对象不同id的列表</param>
        /// <param name="type">类型</param>
        /// <param name="gateID_Suffix">门的起始后缀数字</param>
        /// <param name="eventID_Suffix">事件的起始后缀数字</param>
        /// <returns>下一个不重复的最大id值</returns>
        /// 
        //江启帆 重命名后继续按照重命名规则功能编号1
        private (bool typeVar, string head, int startNumber, int minNo, string tail) ReadVariablesFromFile(string filePath)
        {
            string lastLine = File.ReadLines(filePath).Last();
            var parts = lastLine.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            bool typeVar = false;

            if (filePath.Contains("Gate"))
            {

            }
            string head = "Event";
            int startNumber = 1;
            int minNo = 1;
            string tail = "";
            if (parts.Length > 1)
            {
                typeVar = bool.Parse(parts[1]);
            }
            if (parts.Length > 3)
            {
                head = parts[3];
            }
            if (parts.Length > 5)
            {
                startNumber = int.Parse(parts[5]);
            }
            if (parts.Length > 7)
            {
                minNo = int.Parse(parts[7]);
            }
            if (parts.Length > 9)
            {
                tail = parts[9];
            }

            return (typeVar, head, startNumber, minNo, tail);

        }
        private string GetStartNumberString(int number, int minNo)
        {
            return number.ToString().PadLeft(minNo, '0');
        }


        private string GetNewID(HashSet<string> ids, DrawType type, ref int gateID_Suffix, ref int eventID_Suffix)
        {
            string newID = null;

            if (type == DrawType.BasicEvent || type == DrawType.ConditionEvent || type == DrawType.HouseEvent || type == DrawType.UndevelopedEvent)
            {
                if (File.Exists("variables_outputEvent.txt"))
                {
                    (bool typeVar, string prefix, int startNumber, int minNo, string tail) = ReadVariablesFromFile("variables_outputEvent.txt");

                    if (ids != null && ids.Count > 0)
                    {
                        while (ids.Contains($"{prefix}{GetStartNumberString(eventID_Suffix, minNo)}{tail}"))
                        {
                            eventID_Suffix++;
                        }
                    }
                    newID = $"{prefix}{GetStartNumberString(eventID_Suffix, minNo)}{tail}";
                    eventID_Suffix++;
                }
                else
                {
                    if (ids != null && ids.Count > 0)
                    {
                        while (ids.Contains(FixedString.DEFAULT_EventID_PREFIX + eventID_Suffix))
                        {
                            eventID_Suffix++;
                        }
                    }
                    newID = FixedString.DEFAULT_EventID_PREFIX + eventID_Suffix++;
                }
            }
            else
            {
                if (File.Exists("variables_outputGate.txt"))
                {
                    (bool typeVar, string prefix, int startNumber, int minNo, string tail) = ReadVariablesFromFile("variables_outputGate.txt");

                    if (ids != null && ids.Count > 0)
                    {
                        while (ids.Contains($"{prefix}{GetStartNumberString(eventID_Suffix, minNo)}{tail}"))
                        {
                            eventID_Suffix++;
                        }
                    }
                    newID = $"{prefix}{GetStartNumberString(eventID_Suffix, minNo)}{tail}";
                    eventID_Suffix++;
                }
                else
                {
                    if (ids != null && ids.Count > 0)
                    {
                        while (ids.Contains(FixedString.DEFAULT_GateID_PREFIX + gateID_Suffix))
                        {
                            gateID_Suffix++;
                        }
                    }
                    newID = FixedString.DEFAULT_GateID_PREFIX + gateID_Suffix++;
                }
            }
            return newID;
        }
        //江启帆 重命名后继续按照重命名规则功能编号1
        #endregion

        /// <summary>
        /// 完全递归复制指定对象的副本(根据Chiren属性递归复制)（除父子关系,割集，tag数据）
        /// </summary>
        /// <returns>当前对象的副本（包含子节点）</returns>
        public DrawData CopyDrawDataRecurse()
        {
            DrawData data = this;
            if (data == null) return null;
            DrawData target = data.CopyDrawData();
            if (data.Children != null && data.Children.Count > 0)
            {
                foreach (DrawData child in data.Children)
                {
                    DrawData target_child = child.CopyDrawDataRecurse();
                    target_child.Parent = target;
                    target.Children.Add(target_child);
                }
            }
            return target;
        }

        /// <summary>
        /// 完全复制单个DrawData对象（除父子关系,割集，tag数据）
        /// </summary>
        /// <returns>当前对象的副本（不包含子节点），如果是null表示出错</returns>
        public DrawData CopyDrawData()
        {
            DrawData data = this;
            if (data != null)
                return new DrawData
                {
                    Children = new List<DrawData>(),
                    Parent = null,
                    Cutset = new CutsetModel(),
                    Type = data.Type,
                    X = data.X,
                    Y = data.Y,
                    ChildOffsetX = data.ChildOffsetX,
                    Identifier = data.Identifier,
                    ParentID = data.ParentID,
                    Comment1 = data.Comment1,
                    LogicalCondition = data.LogicalCondition,
                    InputType = data.InputType,
                    FRType = data.FRType,
                    ExposureTimePercentage = data.ExposureTimePercentage,
                    DormancyFactor = data.DormancyFactor,
                    FRPercentage = data.FRPercentage,
                    InputValue = data.InputValue,
                    InputValue2 = data.InputValue2,
                    Units = data.Units,
                    ProblemList = data.ProblemList,
                    Repeats = data.Repeats,
                    QValue = data.QValue,
                    ExtraValue1 = data.ExtraValue1,
                    ExtraValue2 = data.ExtraValue2,
                    ExtraValue3 = data.ExtraValue3,
                    ExtraValue4 = data.ExtraValue4,
                    ExtraValue5 = data.ExtraValue5,
                    ExtraValue6 = data.ExtraValue6,
                    ExtraValue7 = data.ExtraValue7,
                    ExtraValue8 = data.ExtraValue8,
                    ExtraValue9 = data.ExtraValue9,
                    ExtraValue10 = data.ExtraValue10,
                    LinkPath = data.LinkPath,
                    GUID = data.GUID,
                    causPolynomialID = data.causPolynomialID,
                    Level = data.Level,
                    Group = data.Group,
                    Remarks = data.Remarks,
                };
            return null;
        }

        public void ConvertToRepeatEvent(DrawData targetEvent)
        {
            this.Identifier = targetEvent.Identifier;
            this.Comment1 = targetEvent.Comment1;
            this.Type = targetEvent.Type;
            this.LogicalCondition = targetEvent.LogicalCondition;
            this.InputType = targetEvent.InputType;
            this.FRType = targetEvent.FRType;
            this.ExposureTimePercentage = targetEvent.ExposureTimePercentage;
            this.DormancyFactor = targetEvent.DormancyFactor;
            this.FRPercentage = targetEvent.FRPercentage;
            this.InputValue = targetEvent.InputValue;
            this.InputValue2 = targetEvent.InputValue2;
            this.Units = targetEvent.Units;
            this.ProblemList = targetEvent.ProblemList;
            this.QValue = targetEvent.QValue;
            this.ExtraValue1 = targetEvent.ExtraValue1;
            this.ExtraValue2 = targetEvent.ExtraValue2;
            this.ExtraValue3 = targetEvent.ExtraValue3;
            this.ExtraValue4 = targetEvent.ExtraValue4;
            this.ExtraValue5 = targetEvent.ExtraValue5;
            this.ExtraValue6 = targetEvent.ExtraValue6;
            this.ExtraValue7 = targetEvent.ExtraValue7;
            this.ExtraValue8 = targetEvent.ExtraValue8;
            this.ExtraValue9 = targetEvent.ExtraValue9;
            this.ExtraValue10 = targetEvent.ExtraValue10;
        }


        /// <summary>
        /// 彻底释放(删除)drawData对象,递归解除父子关系,把他和他的子节点从他的父节点里移除，防止内存泄漏
        /// </summary>
        /// <returns>是否成功执行</returns>
        public bool Delete()
        {
            DrawData data = this;
            if (data != null)
            {
                if (data.Children != null)
                {
                    for (int i = data.Children.Count - 1; i >= 0; i--)
                    {
                        if (!data.Children[i].Delete())
                        {
                            return false;
                        }
                    }
                }
                data.Children = null;

                if (data.parent != null && data.parent.Children != null)
                {
                    data.parent.Children.Remove(data);
                }
                data.Parent = null;
                data = null;
            }
            return true;
        }

        public void AddChildren(List<DrawData> drawDatas)
        {
            for (int i = 0; i < drawDatas.Count; i++)
            {
                drawDatas[i].Parent = this;
                drawDatas[i].ParentID = this.Identifier;
            }
            this.Children.AddRange(drawDatas);
        }

        public void AddChild(DrawData drawData)
        {
            drawData.Parent = this;
            drawData.ParentID = this.Identifier;
            this.Children.Add(drawData);
        }

        ///// <summary>
        ///// 通过枚举对象的描述获得其自身对象
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="description"></param>
        ///// <returns>枚举值</returns>
        //public static T GetEnumByDescription<T>(string description)
        //{
        //   description = GetDescriptionByChinese(description);
        //   var result = default(T);
        //   var fields = typeof(T).GetFields();
        //   foreach (var field in fields)
        //   {
        //      object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
        //      if (objs.Length > 0 && (objs[0] as DescriptionAttribute).Description.ToUpper().Trim() == description.ToUpper().Trim()) result = (T)field.GetValue(null);
        //   }
        //   return result;
        //}

        //public static T GetEnumByName<T>(string name)
        //{
        //   name = GetDescriptionByChinese(name);
        //   return (T)Enum.Parse(typeof(T), name);
        //}

        /// <summary>
        /// 通过枚举对象反射其对象的描述信息
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns>枚举的描述特性字符串</returns>
        public static string GetDescriptionByEnum(Enum enumValue)
        {
            var result = enumValue.ToString();
            var value = result;
            var field = enumValue.GetType().GetField(value);
            object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (objs.Length != 0) result = ((DescriptionAttribute)objs[0]).Description;
            return result;
        }

        ///// <summary>
        ///// 通过枚举对象名称取得其描述信息
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="name"></param>
        ///// <returns></returns>
        //public static string GetDescriptionByName<T>(string name)
        //{
        //   var result = string.Empty;
        //   name = GetDescriptionByChinese(name);
        //   var target = (T)Enum.Parse(typeof(T), name);
        //   var field = target.GetType().GetField(name);
        //   object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
        //   if (objs.Length != 0) result = ((DescriptionAttribute)objs[0]).Description;
        //   return result;
        //}

        ///// <summary>
        ///// 简单的按中文DrawType获取英文的描述
        ///// </summary>
        ///// <param name="langage">中文字符串</param>
        ///// <returns>对应英文版本字符串</returns>
        //public static string GetDescriptionByChinese(string langage)
        //{
        //   if (langage == null) return null;
        //   langage = langage.Trim();
        //   switch (langage)
        //   {
        //      case "与门": { return DrawData.GetDescriptionByEnum(DrawType.AndGate); }
        //      case "或门": { return DrawData.GetDescriptionByEnum(DrawType.OrGate); }
        //      case "基本事件": { return DrawData.GetDescriptionByEnum(DrawType.BasicEvent); }
        //      case "转入门": return DrawData.GetDescriptionByEnum(DrawType.TransferInGate);
        //      case "未发展事件": return DrawData.GetDescriptionByEnum(DrawType.UndevelopedEvent);
        //      case "常规": return FixedString.LOGICAL_CONDITION_NORMAL;
        //      case "真": return FixedString.LOGICAL_CONDITION_TRUE;
        //      case "假": return FixedString.LOGICAL_CONDITION_FALSE;
        //      case "小时": return FixedString.UNITS_HOURS;
        //      case "分钟": return FixedString.UNITS_MINUTES;
        //      default: return langage;
        //   }
        //}

        /// <summary>
        /// 获取当前DrawData对象的可转换类型的集合数据源
        /// </summary>
        /// <param name="systemModel"></param>
        /// <returns></returns>
        public List<string> GetAvailableTypeSource(SystemModel systemModel, StringModel stringModel, bool OnlyEvent = false)
        {
            var result = new List<string>();
            if (systemModel != null)
            {
                List<DrawType> type = new List<DrawType>();
                //可有子节点的
                if (this.IsGateType && this.Type != DrawType.TransferInGate)
                {
                    //type.AddRange(new DrawType[] { DrawType.AndGate, DrawType.OrGate, DrawType.PriorityAndGate, DrawType.XORGate, DrawType.VotingGate, DrawType.RemarksGate });

                    type.AddRange(new DrawType[] { DrawType.AndGate, DrawType.OrGate, DrawType.VotingGate, DrawType.RemarksGate });

                    //没有儿子的,不是顶层节点的
                    if (!(this.Children != null && this.Children.Count > 0) && !systemModel.Roots.Contains(this))
                    {
                        type.AddRange(new DrawType[] { DrawType.BasicEvent, DrawType.HouseEvent, DrawType.UndevelopedEvent });
                    }
                }
                //重复事件只能换另一种事件
                else if (systemModel.RepeatedEvents != null && systemModel.RepeatedEvents.Count > 0 && systemModel.RepeatedEvents.ContainsKey(this.Identifier))
                {
                    type.AddRange(new DrawType[] { DrawType.BasicEvent, DrawType.HouseEvent, DrawType.UndevelopedEvent });
                }
                //不重复事件
                else
                {
                    type.AddRange(new DrawType[] { DrawType.AndGate, DrawType.OrGate, DrawType.VotingGate, DrawType.RemarksGate });
                    type.AddRange(new DrawType[] { DrawType.BasicEvent, DrawType.HouseEvent, DrawType.UndevelopedEvent });
                }

                if (OnlyEvent)
                {
                    type.Clear();
                    type.AddRange(new DrawType[] { DrawType.BasicEvent, DrawType.HouseEvent, DrawType.UndevelopedEvent });
                }

                foreach (DrawType tmp in type)
                {
                    var value = stringModel.GetType().GetProperties().FirstOrDefault(o => o.Name == tmp.ToString()).GetValue(stringModel).ToString();
                    result.Add(value);
                }
            }
            return result;
        }

        /// <summary>
        /// 获得当前DrawData对象的可用的逻辑条件集合数据源
        /// </summary>
        /// <returns></returns>
        public static string[] GetAvailableLogicalConditionSource(StringModel ftaString) => new string[] { ftaString.Normal, ftaString.False, ftaString.True };


        /// <summary>
        /// 获得当前DrawData对象的可用的输入类型集合数据源
        /// </summary>
        /// <returns></returns>
        public static string[] GetAvailableInputTypeSource(StringModel ftaString) => new string[] { FixedString.MODEL_LAMBDA_TAU, ftaString.FailureProbability, ftaString.ConstantProbability };

        /// <summary>
        /// 获得当前DrawData对象的Mode字段可修改的集合数据源
        /// </summary>
        /// <returns></returns>
        public static string[] GetAvailableModeSource(StringModel ftaString) => new string[] { FixedString.MODEL_CONSTANT_PROBABILITY, FixedString.MODEL_FR_MTBF, FixedString.MODEL_FREQUENCY, FixedString.MODEL_FAILURE_WITH_REPAIR, FixedString.MODEL_FAILURE_WITH_PERIODIC_INSPECTION };

        /// <summary>
        /// 获得当前DrawData对象FailureRateType字段可修改的集合数据
        /// </summary>
        /// <returns></returns>
        public static string[] GetAvailableFailureRateTypeSource(StringModel ftaString) => new string[] { FixedString.FAILURE_RATE_TYPE_FAILURE_RATE, FixedString.FAILURE_RATE_TYPE_MTBF };

        /// <summary>
        /// 获得当前DrawData对象Unit字段可修改的集合数据
        /// </summary>
        /// <returns></returns>
        public static string[] GetAvailableUnitSource(StringModel ftaString) => new string[] { ftaString.Hour, ftaString.Minute };

        /// <summary>
        /// 应用树布局，并把排好的位置信息存放在Drawdata的X和Y里,调用示例：
        /// <para>int width = ftaProgram.Setting.ShapeWidth;
        ///</para><para>int height = (ftaProgram.Setting.ShapeDescriptionRectHeight + ftaProgram.Setting.ShapeIdRectHeight + ftaProgram.Setting.ShapeSymbolRectHeight) + PEN_WIDTH;
        ///</para><para>root.ApplyTreeLayout(width, height, width + 40, height + 40);
        ///</para>
        /// </summary>
        /// <param name="Width">图形宽度</param>
        /// <param name="Height">图形高度</param>
        /// <param name="distanceX">图形间水平间隔</param>
        /// <param name="distanceY">图形间垂直间隔</param>
        /// <param name="startX">左侧开始左边</param>
        /// <param name="startY">顶部开始坐标</param>
        /// <returns>布局后需要的页面大小</returns>
        public Size ApplyTreeLayout(int Width, int Height, int distanceX, int distanceY, int startX = 0, int startY = 0)
        {
            DrawData root = this;

            Size result = new Size(0, 0);
            if (distanceY < Height) distanceY = Height;
            if (distanceX < Width) distanceX = Width;
            Stack<List<DrawData>> levels = new Stack<List<DrawData>>();
            List<DrawData> child = new List<DrawData>() { root };
            while (child.Count > 0)
            {
                levels.Push(child);
                List<DrawData> child_TMP = new List<DrawData>();
                foreach (var item in child)
                {
                    if (item.Children != null)
                    {
                        child_TMP.AddRange(item.Children);
                    }
                }
                child = child_TMP;
            }
            //第一次位置
            int levelNum = levels.Count;
            while (levels.Count > 0)
            {
                List<DrawData> datas = levels.Pop();

                List<DrawData> data_HasChild = datas.Where(obj => obj.Children != null && obj.Children.Count > 0).ToList();
                //先确定有子节点位置
                foreach (var item in data_HasChild)
                {
                    item.X = item.Children.Count == 1 ? item.Children[0].X :
                            item.Children[0].X + (item.Children[item.Children.Count - 1].X - item.Children[0].X) / 2;
                }
                //该层排序
                for (int i = 0; i < datas.Count; i++)
                {
                    datas[i].Y = (levelNum - 1) * distanceY;
                    //没有子节点
                    if (datas[i].Children == null || datas[i].Children.Count == 0)
                    {
                        //开始位置0
                        if (i == 0)
                        {
                            datas[i].X = 0;
                        }
                        //往后推
                        else
                        {
                            datas[i].X = datas[i - 1].X + distanceX;
                        }
                    }
                    //有子节点
                    else
                    {
                        //覆盖情况
                        if (i > 0 && datas[i].X < (datas[i - 1].X + distanceX))
                        {
                            int offset = datas[i - 1].X + distanceX - datas[i].X;
                            //把偏移量反映给右侧的每个父节点（移动整个右侧数据）
                            int startIndex = data_HasChild.IndexOf(datas[i]);
                            for (int j = startIndex; j < data_HasChild.Count; j++)
                            {
                                data_HasChild[j].ChildOffsetX += offset;
                                data_HasChild[j].X += offset;
                            }
                        }
                    }
                }
                levelNum--;
            }
            //第二次位置
            child = new List<DrawData>() { root };
            while (child.Count > 0)
            {
                List<DrawData> child_TMP = new List<DrawData>();
                foreach (var item in child)
                {
                    if (item.Children != null && item.ChildOffsetX != 0)
                    {
                        foreach (var tmp in item.Children)
                        {
                            tmp.X += item.ChildOffsetX;
                            tmp.ChildOffsetX += item.ChildOffsetX;
                        }
                        item.ChildOffsetX = 0;
                    }

                    if (item.Children != null)
                    {
                        child_TMP.AddRange(item.Children);
                    }
                    item.X += startX;
                    item.Y += startY;
                    if (result.Width < item.X) result.Width = item.X;
                    if (result.Height < item.Y) result.Height = item.Y;
                }
                child = child_TMP;
            }
            result.Width += Width;
            result.Height += Height;
            return result;
        }

        private List<DrawData> GetAllData(SystemModel sys, DrawData current, bool isIncludeTransfer)
        {
            var result = new List<DrawData>();
            if (current != null)
            {
                var _current = current;
                if (isIncludeTransfer && current.Type == DrawType.TransferInGate)
                {
                    var aa = General.GetTransferMajor(sys, current);
                    _current = aa;
                    result.Add(current);//原转入门
                }
                result.Add(_current);
                if (_current != null && _current.Children != null)
                {
                    foreach (DrawData child in _current.Children) result.AddRange(GetAllData(sys, child, isIncludeTransfer));
                }
            }
            return result;
        }

        public List<DrawData> GetAllData(SystemModel sys, bool isIncludeTransfer = false) => this.GetAllData(sys, this, isIncludeTransfer);

        public string ToPolymial(DrawData drawData = null)
        {
            var result = string.Empty;
            if (drawData == null) drawData = this;
            var type = drawData.Type == DrawType.OrGate ? "Or" : "And";
            var symbol = "=";
            List<string> abc = new List<string>();
            foreach (var item in drawData.Children)
            {
                if (item.IsGateType == false) result = item.Identifier;
                else result = "\"" + this.Identifier + "\"" + type + ToPolymial(item);
                abc.Add(result);
            }
            result = string.Join(",", abc);
            if (string.IsNullOrEmpty(drawData.ParentID)) result = type + "{" + this.Identifier + "}" + symbol + "{" + result + "}";
            else result = "{" + result + "}";
            return result;
        }

        public override string ToString()
        {
            return this.Identifier;

        }

        public static DrawData ToEntity(string expression, DrawData parent = null)
        {
            var result = parent;

            var selfExpression = string.Empty;
            var childExpression = expression;

            DrawType type = DrawType.OrGate;
            if (expression.Contains("="))
            {
                var a = expression.Split('=');
                selfExpression = a[0];
                childExpression = a[1];


                var b = selfExpression.Split('{');
                type = b[0] == "Or" ? DrawType.OrGate : DrawType.AndGate;
                var topName = b[1].Remove(b[1].Length - 1);

                if (result == null) result = new DrawData();
                result.Type = type;
                result.Identifier = topName;
                if (childExpression.Length > 2) ToEntity(childExpression, result);
            }
            else
            {
                type = DrawType.BasicEvent;
                var charCcache = new List<char>();
                var currentChildDrawData = new DrawData();
                //var isCreatNode = false;
                var isGate = false;
                var isType = false;
                var bracesCount = 0;
                var charArray = new Queue<char>(childExpression.ToCharArray());
                do
                {
                    var first = charArray.Dequeue();
                    if (first == '{')
                    {
                        bracesCount++;
                        if (isType)
                        {
                            type = new string(charCcache.ToArray()) == "Or" ? DrawType.OrGate : DrawType.AndGate;
                        }
                    }


                    else if (first == '}') bracesCount--;
                    else if (first == ',')
                    {


                        var drawData = new DrawData
                        {
                            Identifier = new string(charCcache.ToArray()),
                            Type = type
                        };
                        result.AddChild(drawData);

                        charCcache.Clear();
                    }
                    else if (first == '"')
                    {
                        if (isGate == false) isGate = true;
                        else
                        {
                            isType = true;
                            currentChildDrawData = new DrawData { Identifier = new string(charCcache.ToArray()) };
                        }
                    }
                    else
                    {
                        charCcache.Add(first);
                    }
                }
                while (bracesCount > 0);
            }

            return result;
        }

        public void RaiseParentID()
        {
            if (this.ParentID != null && this.Parent != null) this.ParentID = this.Parent.Identifier;
        }

        public void CalcEffect(List<string> eventNames)
        {
            if (this.parent == null && this.Children?.Count > 0)
            {
                var root = this;
                var allData = root.GetAllData(General.FtaProgram.CurrentSystem);
                var allEvents = allData.Where(o => o.IsGateType == false).ToList();
                allEvents.ForEach(o => o.Effect = Effect.Ineffective);
                var maxLevel = this.CalcLevel(0, root);
                foreach (string item in eventNames)
                {
                    var _event = allData.Where(o => o.Identifier == item).ToList();
                    if (_event?.Count > 0) _event.ForEach(o => o.Effect = Effect.Effective);
                }
                root.CalcEffect(maxLevel);
            }
        }

        private void CalcEffect(int maxLevel)
        {
            var allData = this.GetAllData(General.FtaProgram.CurrentSystem, true);
            //var children = allData.Where(o => o.Level == maxLevel);
            var parentLevel = maxLevel - 1;
            var parents = allData.Where(o => o.Level == parentLevel && o.IsGateType && o.Type != DrawType.TransferInGate).ToArray();
            for (int i = 0; i < parents.Length; i++)
            {
                var children = parents[i].Children;
                var childrenCount = children.Count;
                var effectCount = children.Where(o => o.Effect == Effect.Effective).Count();
                if (children?.Count > 0)
                {
                    if (parents[i].Type == DrawType.AndGate)
                    {
                        if (childrenCount == effectCount) parents[i].Effect = Effect.Effective;
                        else parents[i].Effect = Effect.Ineffective;
                    }
                    else if (parents[i].Type == DrawType.OrGate)
                    {
                        if (effectCount > 0) parents[i].Effect = Effect.Effective;
                        else parents[i].Effect = Effect.Ineffective;
                    }


                }
            }
            if (parentLevel > 0) this.CalcEffect(parentLevel);
        }

        private int CalcLevel(int level, DrawData drawData)
        {
            var maxLevel = 0;
            if (level == 0) drawData = this.GetRoot();
            //if (drawData.Type == DrawType.TransferInGate) drawData = General.GetTransferMajor(drawData);
            drawData.Level = level;
            //drawData.Comment1 = level.ToString();
            if (drawData.Children?.Count > 0)
            {
                level += 1;
                for (int i = 0; i < drawData.Children.Count; i++)
                {
                    var childResult = this.CalcLevel(level, drawData.Children[i]);
                    if (childResult > maxLevel) maxLevel = childResult;
                }
            }
            else maxLevel = level;
            return maxLevel;
        }


        public void CalcEffect3(List<int> eventIDs)
        {
            if (this.Parent == null && this.Children?.Count > 0)
            {
                DrawData root = this;
                List<DrawData> allData = root.GetAllData(this);
                List<DrawData> allEvents = allData.Where(o => o.IsGateType == false).ToList();
                allData.ForEach(o => o.IsEffective = false);
                int maxLevel = this.CalcLevel(0, root);
                foreach (int item in eventIDs)
                {
                    List<DrawData> _event = allData.Where(o => o.causPolynomialID == item && o.Type == DrawType.BasicEvent).ToList();
                    if (_event?.Count > 0) _event.ForEach(o => o.IsEffective = true);
                }
                root.CalcEffect(maxLevel);
            }
        }
    }
}
