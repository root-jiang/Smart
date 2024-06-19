using FaultTreeAnalysis.Common;
using FaultTreeAnalysis.Model.Enum;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FaultTreeAnalysis.Model.Data
{
    /// <summary>
    /// 表示一个系统对象，系统可以有多个根节点数据对象
    /// </summary>
    public partial class SystemModel
    {
        /// <summary>
        /// 分组
        /// </summary>
        public string GroupLevel { get; set; }

        /// <summary>
        /// 系统名称
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// 当前系统对象中所有树（根节点）的集合
        /// </summary>
        public List<DrawData> Roots { get; set; }

        /// <summary>
        /// 当前系统中的转移门集合（本体和副本），key是他们的id
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, HashSet<DrawData>> TranferGates { get; set; }

        /// <summary>
        /// 当前系统中的重复事件的集合（本体和副本），key是他们的id
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, HashSet<DrawData>> RepeatedEvents { get; set; }

        public RenumberModel RenumberItem { get; set; }

        /// <summary>
        /// 画布里图形里描述框的高度
        /// </summary>
        public int ShapeDescriptionRectHeight { get; set; }

        /// <summary>
        /// 画布里图形里标识框的高度
        /// </summary>
        public int ShapeIdRectHeight { get; set; }

        /// <summary>
        /// 画布里图形里符号框高度
        /// </summary>
        public int ShapeSymbolRectHeight { get; set; }

        /// <summary>
        /// 画布里图形的宽度
        /// </summary>
        public int ShapeWidth { get; set; }

        /// <summary>
        /// 当前系统选中的高亮割集对象
        /// </summary>
        [JsonIgnore]
        public HighLightCutSet CurrentSelectedCutset { get; set; }

        /// <summary>
        /// 初始化Roots
        /// </summary>
        public SystemModel()
        {
            Roots = new List<DrawData>();
            ShapeDescriptionRectHeight = 36;
            ShapeIdRectHeight = 13;
            ShapeSymbolRectHeight = 50;
            ShapeWidth = 100;
        }

        public void UpdateRenumberItem(string systemName)
        {
            this.SystemName = systemName;
            if (this != null && this.RenumberItem != null) this.RenumberItem = new RenumberModel { Name = this.SystemName };
        }

        public SystemModel(List<DrawData> roots)
        {
            this.Roots = roots;
            this.UpdateRepeatedAndTranfer();
            ShapeDescriptionRectHeight = 36;
            ShapeIdRectHeight = 13;
            ShapeSymbolRectHeight = 50;
            ShapeWidth = 100;
        }

        /// <summary>
        /// 创建一个根节点,且添加到根节点Roots列表
        /// </summary>
        public DrawData CreateOneRootItem()
        {
            DrawData item = null;
            HashSet<string> ids = GetAllIDs();
            int gateNumber = 1;
            if (ids != null && ids.Count > 0)
            {
                while (ids.Contains(FixedString.DEFAULT_GateID_PREFIX + gateNumber))
                {
                    gateNumber++;
                }
            }
            item = new DrawData();
            item.Identifier = FixedString.DEFAULT_GateID_PREFIX + gateNumber;
            Roots.Add(item);
            return item;
        }

        /// <summary>
        /// 创建一个根节点,且添加到根节点Roots列表
        /// </summary>
        public DrawData CreateOneRootItem(DrawType tp)
        {
            DrawData item = null;
            HashSet<string> ids = GetAllIDs();
            int gateNumber = 1;
            if (ids != null && ids.Count > 0)
            {
                while (ids.Contains(FixedString.DEFAULT_GateID_PREFIX + gateNumber))
                {
                    gateNumber++;
                }
            }
            item = new DrawData();
            item.Type = tp;
            item.Identifier = FixedString.DEFAULT_GateID_PREFIX + gateNumber;
            Roots.Add(item);
            return item;
        }

        #region 实现json导入时，给每个子对象对象赋值他的父对象
        /// <summary>
        /// 为当前顶层对象的所有节点设置父对象
        /// </summary>
        /// <param name="drawData">父对象</param>
        private void SetParentToChildren(DrawData drawData)
        {
            if (drawData.Children != null || drawData.Children.Count > 0)
            {
                foreach (var item in drawData.Children)
                {
                    item.Parent = drawData;
                    this.SetParentToChildren(item);
                }
            }
        }

        /// <summary>
        /// 为所有树设置父对象
        /// </summary>
        private void SetParentIdToChildren()
        {
            foreach (var item in this.Roots)
            {
                this.SetParentToChildren(item);
            }
        }

        /// <summary>
        /// 暂时只用于设置Drawdata数据的父对象
        /// </summary>
        public void Initialize()
        {
            this.SetParentIdToChildren();
        }
        #endregion


        #region 获取所有对象的列表，获取系统里所有ID集合，有重复的。
        /// <summary>
        /// 获取当前system对象里所有ID集合，不重复的。
        /// </summary>
        /// <returns>系统里所有id的集合（可能重复）</returns>
        public HashSet<string> GetAllIDs()
        {
            HashSet<string> ids = new HashSet<string>();
            foreach (DrawData root in Roots)
            {
                foreach (var item in root.GetAllData(this))
                {
                    if (item.Identifier != null)
                    {
                        ids.Add(item.Identifier);
                    }
                }
            }
            return ids;
        }

        /// <summary>
        /// 获取当前system对象里所有drawdata对象
        /// </summary>
        /// <returns>系统里所有数据对象的集合</returns>
        public List<DrawData> GetAllDatas()
        {
            return this.GetAllDatas(this.Roots);
        }

        private List<DrawData> GetAllDatas(List<DrawData> roots)
        {
            return General.TryCatch(() =>
            {
                List<DrawData> allDatas = new List<DrawData>();
                if (roots != null)
                {
                    foreach (DrawData data in roots) allDatas.AddRange(data.GetAllData(this));
                }
                return allDatas;
            });
        }
        #endregion


        #region 初始化当前系统里的转移门和重复事件集合
        /// <summary>
        /// 初始化转移门和重复事件的集合
        /// </summary>
        public void UpdateRepeatedAndTranfer()
        {
            this.TranferGates = new Dictionary<string, HashSet<DrawData>>();
            this.RepeatedEvents = new Dictionary<string, HashSet<DrawData>>();
            //得到所有数据
            List<DrawData> allDatas = GetAllDatas();
            allDatas.ForEach(o => o.Repeats = 0);
            //根据条件分
            var repeat = from n in allDatas group n by n.Identifier into j where j.Count() > 1 select j;
            foreach (var s in repeat)
            {
                string key = s.Key;
                HashSet<DrawData> tmp = new HashSet<DrawData>();
                foreach (var item in s)
                {
                    tmp.Add(item);
                }
                //重复事件
                if (tmp.First().Type == DrawType.BasicEvent || tmp.First().Type == DrawType.HouseEvent || tmp.First().Type == DrawType.UndevelopedEvent || tmp.First().Type == DrawType.ConditionEvent)
                {
                    DrawData rep = tmp.Where(obj => obj.Type != tmp.First().Type).FirstOrDefault();
                    if (rep == null && !RepeatedEvents.ContainsKey(s.Key))
                    {
                        RepeatedEvents.Add(s.Key, tmp);
                        foreach (DrawData data in tmp) data.Repeats = tmp.Count - 1;
                    }
                }
                //转移门
                else
                {
                    DrawData transfer_copy = tmp.Where(obj => obj.Type == DrawType.TransferInGate).FirstOrDefault();
                    List<DrawData> transfer = tmp.Where(obj => obj.Type != DrawType.TransferInGate).ToList();
                    if (transfer?.Count == 1 && transfer_copy != null && !TranferGates.ContainsKey(s.Key))
                    {
                        TranferGates.Add(s.Key, tmp);
                    }
                }
            }
        }

        #endregion

        #region 转移门和重复事件集合的操作
        /// <summary>
        /// 从转移门的id获取转移门本体对象
        /// </summary>
        /// <param name="id">转移门id</param>
        /// <returns>转移门本体数据对象</returns>
        public DrawData GetTranferGateByName(string id)
        {
            if (TranferGates == null)
            {
                return null;
            }
            DrawData gate = null;
            HashSet<DrawData> ListTranferGate;
            if (TranferGates.ContainsKey(id))
                ListTranferGate = TranferGates[id];
            else
                return null;

            //返回的集合中包括转移门和转移后接入的门，取非转移门，则可取到原来的本体
            foreach (DrawData item in ListTranferGate)
            {
                if (item.Type != DrawType.TransferInGate)
                    return item;
            }
            return gate;
        }

        //public void UpdateRenumberedItem()
        //{
        //   var result = new List<RenumberModel>(this.Roots.Count);
        //   foreach (DrawData item in this.Roots)
        //   {
        //      var a = this.RenumberItems.FirstOrDefault(o => o.Identifier == item.Identifier);
        //      if (a != null) result.Add(a);
        //      else result.Add(new RenumberModel { Identifier = item.Identifier });
        //   }
        //   this.RenumberItems = result;
        //}

        /// <summary>
        /// 同步TranferGate集合里的对象
        /// </summary>
        public void UpdateTranferGate(DrawData oldDrawData, string newId)
        {
            if (this.TranferGates.Keys.Contains(oldDrawData.Identifier))
            {
                foreach (DrawData item in this.TranferGates[oldDrawData.Identifier])
                {
                    item.Identifier = newId;
                    item.RaiseParentID();
                }
            }
            else
            {
                oldDrawData.Identifier = newId;
                oldDrawData.RaiseParentID();
            }
        }

        /// <summary>
        /// 同步指定RepeatedEvents集合里的对象
        /// </summary>
        /// <param name="duplicateEvents"></param>
        public void UpdateRepeatedEvent(DrawData oldIdDrawData, string newId)
        {
            if (this.RepeatedEvents.Keys.Contains(oldIdDrawData.Identifier))
            {
                foreach (DrawData item in this.RepeatedEvents[oldIdDrawData.Identifier])
                {
                    item.Identifier = newId;
                    item.RaiseParentID();
                }
            }
            else
            {
                oldIdDrawData.Identifier = newId;
                oldIdDrawData.RaiseParentID();
            }
        }

        /// <summary>
        /// 从集合里移除一个转移门
        /// </summary>
        /// <param name="data">要移除的转移门数据对象</param>
        public void RemoveTranferGate(DrawData data)
        {
            if (TranferGates == null || data == null) return;
            if (TranferGates.ContainsKey(data.Identifier))
            {
                HashSet<DrawData> trans = TranferGates[data.Identifier];
                if (trans.Contains(data))
                {
                    //副本
                    if (data.Type == DrawType.TransferInGate)
                    {
                        //移除当前转移门副本
                        trans.Remove(data);
                        //不再是转移门了
                        if (trans.Count < 2)
                        {
                            TranferGates.Remove(data.Identifier);
                        }
                    }
                    //本体
                    else if (data.Type != DrawType.TransferInGate)
                    {
                        TranferGates.Remove(data.Identifier);
                    }
                }
            }
        }

        /// <summary>
        /// 从集合里移除一个重复事件
        /// </summary>
        /// <param name="data">要移除的重复事件数据对象</param>
        /// <param name="isRemoveLast">当还剩下一个重复事件时，是否从字典里移除列表</param>
        public void RemoveRepeatedEvent(DrawData data, bool isRemoveLast = true)
        {
            if (RepeatedEvents == null || data == null) return;
            if (RepeatedEvents.ContainsKey(data.Identifier))
            {
                HashSet<DrawData> repeats = RepeatedEvents[data.Identifier];
                if (repeats != null && repeats.Contains(data))
                {
                    //集合维护
                    repeats.Remove(data);
                    foreach (DrawData tmp in repeats)
                    {
                        tmp.Repeats = repeats.Count - 1;
                    }
                    if (repeats.Count < 2)
                    {
                        if (isRemoveLast) RepeatedEvents.Remove(data.Identifier);
                    }
                }
            }
        }

        /// <summary>
        /// 把一条数据加到重复事件集合中
        /// </summary>
        /// <param name="data">要添加的重复事件对象</param>
        public void AddRepeatedEvent(DrawData data)
        {
            if (RepeatedEvents == null || data == null) return;
            HashSet<DrawData> repeats = null;
            //新建重复事件
            if (!RepeatedEvents.ContainsKey(data.Identifier))
            {
                repeats = new HashSet<DrawData>();
                RepeatedEvents.Add(data.Identifier, repeats);
            }
            //添加重复事件
            else if (RepeatedEvents.ContainsKey(data.Identifier))
            {
                repeats = RepeatedEvents[data.Identifier];
            }

            if (repeats != null && !repeats.Contains(data))
            {
                //添加重复事件
                repeats.Add(data);
            }
            //设置重复次数
            if (repeats != null)
            {
                foreach (DrawData tmp in repeats)
                {
                    tmp.Repeats = repeats.Count - 1;
                }
            }
        }

        /// <summary>
        /// 重新初始化RepeatedEvents集合并给Repeats属性重新赋值
        /// </summary>
        public void RaiseRepeatedEvent()
        {
            var allEvents = this.GetAllDatas().Where(o => !o.IsGateType);
            foreach (var item in allEvents) item.Repeats = 0;
            this.RepeatedEvents.Clear();
            var repeatsTmp = allEvents.GroupBy(o => o.Identifier).Where(o => o.Count() > 1);
            foreach (var item in repeatsTmp)
            {
                HashSet<DrawData> datas = new HashSet<DrawData>();
                int value = item.Count();
                foreach (var tmp in item)
                {
                    tmp.Repeats = value - 1;
                    datas.Add(tmp);
                }
                this.RepeatedEvents.Add(item.Key, datas);
            }
        }

        /// <summary>
        /// 把一条数据加到转移门集合中
        /// </summary>
        /// <param name="data">要添加的转移门数据对象</param>
        public void AddTranferGate(DrawData data)
        {
            if (TranferGates == null || data == null) return;
            HashSet<DrawData> trans = null;
            //新建转移门
            if (!TranferGates.ContainsKey(data.Identifier))
            {
                trans = new HashSet<DrawData>();
                TranferGates.Add(data.Identifier, trans);
            }
            //添加重复事件
            else if (TranferGates.ContainsKey(data.Identifier))
            {
                trans = TranferGates[data.Identifier];
            }

            if (trans != null && !trans.Contains(data))
            {
                //添加转移门
                trans.Add(data);
            }
        }

        public void AddRoots(SystemModel newSystemModel)
        {
            foreach (DrawData da in newSystemModel.Roots)
            {
                this.Roots.Add(da.CopyDrawDataRecurse());
            }
        }
        #endregion

        public static SystemModel operator +(SystemModel oldSystem, SystemModel newSystem)
        {
            // to be continue
            return oldSystem;
        }

        public void QuickRenumber()
        {
            var gates = this.GetAllDatas().Where(o => o.IsGateType && o.Type != DrawType.TransferInGate).ToList();
            var renumberedNames = this.RenumberItem.GetSerialNames(gates.Count);
            for (int i = 0; i < gates.Count; i++)
            {
                gates[i].Identifier = renumberedNames[i];
            }
        }

        public HashSet<DrawData> FindChildren(DrawData parent)
        {
            var allChildren = this.Roots.Where(o => o.ParentID == parent.Identifier);
            return new HashSet<DrawData>(allChildren);
        }




    }
}
