using DevExpress.Utils.About;
using DevExpress.XtraEditors;
using FaultTreeAnalysis.Common;
using FaultTreeAnalysis.Model.Data;
using FaultTreeAnalysis.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace FaultTreeAnalysis.FTAControlEventHandle.Ribbon.FTA.Tool.FTA
{
    public partial class RenumberView : XtraForm
    {
        /// <summary>
        /// 提供国际化的字符串对象
        /// </summary>
        private StringModel ftaString;

        /// <summary>
        /// 当前系统对象
        /// </summary>
        private SystemModel ftaSystem;

        /// <summary>
        /// 保存重命名设置信息
        /// </summary>
        private RenumberConfig renumberConfig;

        /// <summary>
        /// 要处理的数据对象集合
        /// </summary>
        private Func<RenumberedRange, List<DrawData>> getAllDrawDataFromSelectedNode;

        /// <summary>
        /// 命名的对象类型（门，事件，都要）
        /// </summary>
        internal RenumberedType type => (RenumberedType)this.Rp_Type.SelectedIndex;

        private RenumberedRange range => (RenumberedRange)this.Rp_Range.SelectedIndex;

        /// <summary>
        /// 是否取消重命名
        /// </summary>
        public bool IsCancel { get; set; } = false;

        /// <summary>
        /// 无参数构造函数 
        /// </summary>
        public RenumberView(RenumberedRange Range, ProgramModel ftaProgram, SystemModel ftaSystem, Func<RenumberedRange, List<DrawData>> getAllDrawDataFromSelectedNode, RenumberConfig renumberConfig)
        {
            InitializeComponent();
            this.Initialize(Range, ftaProgram, ftaSystem, getAllDrawDataFromSelectedNode, renumberConfig);
        }

        /// <summary>
        /// 初始化界面的默认状态
        /// </summary>
        private void Initialize(RenumberedRange Range, ProgramModel ftaProgram, SystemModel ftaSystem, Func<RenumberedRange, List<DrawData>> getAllDrawDataFromSelectedNode, RenumberConfig renumberConfig)
        {
            if (Range == RenumberedRange.AllSystem)
            {
                this.Rp_Range.ReadOnly = true;
                this.Rp_Range.SelectedIndex = 0;
            }
            else
            {
                this.Rp_Range.ReadOnly = false;
                this.Rp_Range.SelectedIndex = 1;
            }
            this.Rp_Type.SelectedIndex = 0;
            this.wizardPage5.Visible = false;
            this.SetTypeMode(0);
            if (Range == RenumberedRange.AllSystem)
            {
                this.Rp_Range.SelectedIndex = 0;
            }
            else
            {
                this.Rp_Range.SelectedIndex = 1;
            }
            //this.SetConfirmMode(0);
            this.SubscribeEvents();
            this.ftaString = ftaProgram.String;
            this.ftaSystem = ftaSystem;
            this.LoadConfig(renumberConfig);
            this.getAllDrawDataFromSelectedNode = getAllDrawDataFromSelectedNode;
            this.BindLanguage();
        }

        /// <summary>
        /// 装载配置
        /// </summary>
        /// <param name="config">配置对象</param>
        private void LoadConfig(RenumberConfig config)
        {
            this.Gate_CheckEdit.Checked = config.IsSaveGateConfig;
            this.Event_CheckEdit1.Checked = config.IsSaveEventConfig;
            this.GateStartNumber.Text = config.GateStartNumber.ToString();
            this.EventStartNumber.Text = config.EventStartNumber.ToString();
            this.renumberConfig = config;
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="increments">门和事件各自重名后的最大值</param>
        private void SaveConfig(List<int> increments)
        {
            this.renumberConfig.IsSaveGateConfig = this.Gate_CheckEdit.Checked;
            this.renumberConfig.IsSaveEventConfig = this.Event_CheckEdit1.Checked;
            var gateOrigin = Convert.ToInt32(this.GateStartNumber.Text);
            var eventOrigin = Convert.ToInt32(this.EventStartNumber.Text);
            switch (this.type)
            {
                case RenumberedType.BothGateAndEvent:
                    {
                        if (this.Event_CheckEdit2.Checked)
                        {
                            this.renumberConfig.GateStartNumber = this.Gate_CheckEdit.Checked ? increments[0] + gateOrigin : gateOrigin;
                            this.renumberConfig.EventStartNumber = this.renumberConfig.GateStartNumber;
                        }
                        else
                        {
                            this.renumberConfig.GateStartNumber = this.Gate_CheckEdit.Checked ? increments[0] + gateOrigin : gateOrigin;
                            this.renumberConfig.EventStartNumber = this.Event_CheckEdit1.Checked ? increments[1] + eventOrigin : eventOrigin;
                        }
                        break;
                    }
                case RenumberedType.OnlyGate:
                    {
                        this.renumberConfig.GateStartNumber = this.Gate_CheckEdit.Checked ? increments[0] + gateOrigin : gateOrigin;
                        break;
                    }
                case RenumberedType.OnlyEvent:
                    {
                        this.renumberConfig.EventStartNumber = this.Event_CheckEdit1.Checked ? increments[0] + eventOrigin : eventOrigin;
                        break;
                    }
            }
        }

        /// <summary>
        /// 绑定语言
        /// </summary>
        private void BindLanguage()
        {
            this.Text = this.ftaString.RenumberWizardTitle;
            this.wizardPage1.Text = this.ftaString.RenumberWizardPage1Caption;
            this.wizardPage2.Text = this.ftaString.RenumberWizardPage2Caption;
            this.wizardPage3.Text = this.ftaString.RenumberWizardPage3Caption;
            this.wizardPage4.Text = this.ftaString.RenumberWizardPage4Caption;
            this.wizardPage5.Text = this.ftaString.RenumberWizardPage5Caption;
            this.wizardPage6.Text = this.ftaString.RenumberWizardPage6Caption;

            this.wizardPage1.DescriptionText = this.ftaString.RenumberWizardPage1Content;
            this.wizardPage2.DescriptionText = this.ftaString.RenumberWizardPage2Content;
            this.wizardPage3.DescriptionText = this.ftaString.RenumberWizardPage3Content;
            this.wizardPage4.DescriptionText = this.ftaString.RenumberWizardPage4Content;
            this.wizardPage5.DescriptionText = this.ftaString.RenumberWizardPage5Content;
            this.wizardPage6.DescriptionText = this.ftaString.RenumberWizardPage6Content;

            this.Page1Tip_LlabelControl.Text = this.ftaString.RenumberWizardPage1Tip;
            this.Page2Tip_LlabelControl.Text = this.ftaString.RenumberWizardPage2Tip;
            this.Page3Tip_LlabelControl.Text = this.ftaString.RenumberWizardPage3Tip;
            this.Page4Tip_LlabelControl.Text = this.ftaString.RenumberWizardPage4Tip;
            this.Page5Tip_LlabelControl.Text = this.ftaString.RenumberWizardPage5Tip;
            this.Page6Tip_LlabelControl.Text = this.ftaString.RenumberWizardPage6Tip;

            this.GateLabel1.Text = this.ftaString.GatePrefix;
            this.GateLabel2.Text = this.ftaString.GateStartNumber;
            this.GateLabel3.Text = this.ftaString.GateMinNumber;
            this.GateLabel4.Text = this.ftaString.GateSuffix;
            this.GateLabel5.Text = this.ftaString.GateLabel5;

            this.EventLabel1.Text = this.ftaString.EventPrefix;
            this.EventLabel2.Text = this.ftaString.EventStartNumber;
            this.EventLabel3.Text = this.ftaString.EventMinNumber;
            this.EventLabel4.Text = this.ftaString.EventSuffix;
            this.EventLabel5.Text = this.ftaString.GateLabel5;

            this.Gate_CheckEdit.Text = this.Event_CheckEdit1.Text = this.ftaString.GateLabel6;
            this.Event_CheckEdit2.Text = this.ftaString.AccordingToGate;

            this.Rp_Range.Properties.Items[0].Description = this.ftaString.RenumberRadiogroup1Raido1Text;
            this.Rp_Range.Properties.Items[1].Description = this.ftaString.RenumberRadiogroup1Raido2Text;
            this.Rp_Range.Properties.Items[2].Description = this.ftaString.RenumberRadiogroup1Raido3Text;

            this.Rp_Type.Properties.Items[0].Description = this.ftaString.OnlyGate;
            this.Rp_Type.Properties.Items[1].Description = this.ftaString.OnlyEvent;
            this.Rp_Type.Properties.Items[2].Description = this.ftaString.BothGateAndEvent;

            this.Confirm_RadioGroup.Properties.Items[0].Description = this.ftaString.Yes;
            this.Confirm_RadioGroup.Properties.Items[1].Description = this.ftaString.No;

            this.wizardControl1.PreviousText = this.ftaString.Previous;
            this.wizardControl1.NextText = this.ftaString.Next;
            this.wizardControl1.FinishText = this.ftaString.Finish;
            this.wizardControl1.CancelText = this.ftaString.Cancel;
        }

        /// <summary>
        /// 订阅事件的集中处理函数
        /// </summary>
        private void SubscribeEvents()
        {
            this.Rp_Range.SelectedIndexChanged += RadioGroup_SelectedIndexChanged;
            this.Rp_Type.SelectedIndexChanged += RadioGroup_SelectedIndexChanged;
            this.Confirm_RadioGroup.SelectedIndexChanged += RadioGroup_SelectedIndexChanged;

            this.GateHead.TextChanged += TextEdit_TextChanged;
            this.GateStartNumber.TextChanged += TextEdit_TextChanged;
            this.GateMinNo.TextChanged += TextEdit_TextChanged;
            this.GateTail.TextChanged += TextEdit_TextChanged;

            this.EventHead.TextChanged += TextEdit_TextChanged;
            this.EventStartNumber.TextChanged += TextEdit_TextChanged;
            this.EventMinNo.TextChanged += TextEdit_TextChanged;
            this.EventTail.TextChanged += TextEdit_TextChanged;

            this.Event_CheckEdit2.CheckedChanged += CheckEdit_CheckedChanged;
        }

        /// <summary>
        /// 取消事件订阅的集中处理函数
        /// </summary>
        private void UnsubscribeEvents()
        {
            this.Rp_Range.SelectedIndexChanged -= RadioGroup_SelectedIndexChanged;
            this.Rp_Type.SelectedIndexChanged -= RadioGroup_SelectedIndexChanged;
            this.Confirm_RadioGroup.SelectedIndexChanged -= RadioGroup_SelectedIndexChanged;

            this.GateHead.TextChanged -= TextEdit_TextChanged;
            this.GateStartNumber.TextChanged -= TextEdit_TextChanged;
            this.GateMinNo.TextChanged -= TextEdit_TextChanged;
            this.GateTail.TextChanged -= TextEdit_TextChanged;

            this.EventHead.TextChanged -= TextEdit_TextChanged;
            this.EventStartNumber.TextChanged -= TextEdit_TextChanged;
            this.EventMinNo.TextChanged -= TextEdit_TextChanged;
            this.EventTail.TextChanged -= TextEdit_TextChanged;

            this.Event_CheckEdit2.CheckedChanged -= CheckEdit_CheckedChanged;
        }

        /// <summary>
        /// 切换选择“对哪些数字重编号”时的界面变更操作函数
        /// </summary>
        /// <param name="index">RadioGroup控件当前选中的索引</param>
        private void SetTypeMode(int index)
        {
            this.wizardControl1.Pages[2].Visible = true;
            this.wizardControl1.Pages[3].Visible = true;
            this.Event_CheckEdit2.Visible = true;
            switch (index)
            {
                case 0: { this.wizardControl1.Pages[3].Visible = false; break; }
                case 1: { this.wizardControl1.Pages[2].Visible = false; this.Event_CheckEdit2.Visible = false; break; }
                default: break;
            }
            this.Rp_Type.SelectedIndex = index;
        }

        ///// <summary>
        ///// 设置配置模式
        ///// </summary>
        ///// <param name="index"></param>
        //private void SetConfirmMode(int index)
        //{
        //    this.wizardControl1.Pages[4].AllowNext = index == 0 ? true : false;
        //    this.Confirm_RadioGroup.SelectedIndex = index;
        //}

        /// <summary>
        /// 通过起始数字字符串和长度参数计算输出的固定长度的数字字符串
        /// </summary>
        /// <param name="startNumber">输入的起始数字字符串</param>
        /// <returns></returns>
        private string GetStartNumberString(string startNumber, decimal bit)
        {
            var result = string.Empty;
            int number = 0;
            if (int.TryParse(startNumber, out number))
            {
                result = this.GetStartNumberString(number, Convert.ToInt32(bit));
            }
            return result;
        }

        /// <summary>
        /// 通过起始数字和长度参数计算输出的固定长度的数字字符串
        /// </summary>
        /// <param name="startNumber"></param>
        /// <param name="bit"></param>
        /// <returns></returns>
        private string GetStartNumberString(int startNumber, decimal bit)
        {
            var result = string.Empty;
            if (startNumber > 1073741824) startNumber = 1073741824;
            else if (startNumber < 0) startNumber = 0;
            var _number = startNumber.ToString();
            var length = bit - _number.Length > 0 ? bit : _number.Length;
            var filler = string.Empty.PadLeft((int)length, '0');
            result = startNumber.ToString(filler);
            return result;
        }

        /// <summary>
        /// 重编号配置的预览字符串方法
        /// </summary>
        /// <param name="renumberType">类型选择（门、事件）</param>
        private string GetExampleString(RenumberedType type)
        {
            var result = string.Empty;
            switch (type)
            {
                case RenumberedType.OnlyGate:
                    {
                        result = $"{this.GateHead.Text}{this.GetStartNumberString(this.GateStartNumber.Text, this.GateMinNo.Value)}{this.GateTail.Text}";
                        break;
                    }
                case RenumberedType.OnlyEvent:
                    {
                        result = $"{this.EventHead.Text}{this.GetStartNumberString(this.EventStartNumber.Text, this.EventMinNo.Value)}{this.EventTail.Text}";
                        break;
                    }
                default: break;
            }
            return result;
        }

        /// <summary>
        /// 计算重编号的字符串结果集
        /// </summary>
        /// <param name="drawData">要重命名的DrawData对象</param>
        /// <param name="isDependOnGate">是否沿用“门”类型相同的顺序</param>
        /// <returns></returns>

        public string Head { get; private set; }
        public int StartNumber { get; private set; }
        public int MinNo { get; private set; }
        public string Tail { get; private set; }
        /// <summary>
        /// 计算重编号的字符串结果集
        /// </summary>
        /// <param name="drawData">要重命名的DrawData对象</param>
        /// <param name="isDependOnGate">是否沿用“门”类型相同的顺序</param>
        /// <returns></returns>

        public List<string> GetRenumberedNames(List<DrawData> drawData, bool isDependOnGate = false)
        {
            //江启帆 按照重命名后规则继续命名函数2

            var result = new List<string>(drawData.Count);

            if (drawData.Count > 0)
            {
                if (drawData[0].IsGateType)
                {
                    File.Delete("variables_outputGate.txt");
                    using (StreamWriter writer = new StreamWriter("variables_outputGate.txt"))
                    {
                        for (int i = 0; i < drawData.Count; i++)
                        {
                            var type = drawData[i].IsGateType;
                            Head = type ? this.GateHead.Text : this.EventHead.Text;
                            StartNumber = (!type && !isDependOnGate) ? Convert.ToInt32(this.EventStartNumber.Text) : Convert.ToInt32(this.GateStartNumber.Text);
                            MinNo = (!type && !isDependOnGate) ? (int)this.EventMinNo.Value : (int)this.GateMinNo.Value;
                            Tail = type ? this.GateTail.Text : this.EventTail.Text;
                            result.Add($"{Head}{this.GetStartNumberString(StartNumber + i, MinNo)}{Tail}");
                            writer.WriteLine($"Type: {type}, Head: {Head}, StartNumber: {StartNumber}, MinNo: {MinNo}, Tail: {Tail}");
                        }
                    }
                }
                else
                {
                    File.Delete("variables_outputEvent.txt");
                    using (StreamWriter writer = new StreamWriter("variables_outputEvent.txt"))
                    {
                        for (int i = 0; i < drawData.Count; i++)
                        {
                            var type = drawData[i].IsGateType;
                            Head = type ? this.GateHead.Text : this.EventHead.Text;
                            StartNumber = (!type && !isDependOnGate) ? Convert.ToInt32(this.EventStartNumber.Text) : Convert.ToInt32(this.GateStartNumber.Text);
                            MinNo = (!type && !isDependOnGate) ? (int)this.EventMinNo.Value : (int)this.GateMinNo.Value;
                            Tail = type ? this.GateTail.Text : this.EventTail.Text;
                            result.Add($"{Head}{this.GetStartNumberString(StartNumber + i, MinNo)}{Tail}");
                            writer.WriteLine($"Type: {type}, Head: {Head}, StartNumber: {StartNumber}, MinNo: {MinNo}, Tail: {Tail}");
                        }
                    }
                }
            }
            return result;
            //江启帆 按照重命名后规则继续命名函数2
        }


        /// <summary>
        /// 根据类型、起始数字以及长度，计算重编号结果集
        /// </summary>
        /// <param name="type">类型（门、事件）</param>
        /// <param name="startNumber">起始数字</param>
        /// <param name="count">长度</param>
        /// <returns></returns>
        private List<string> GetRenumberedNames(RenumberedType type, int startNumber, int count)
        {
            var result = new List<string>(count);

            var head = this.GateHead.Text;
            var minNo = this.GateMinNo.Value;
            var tail = this.GateTail.Text;
            if (type.Equals(RenumberedType.OnlyEvent))
            {
                head = this.EventHead.Text;
                minNo = this.EventMinNo.Value;
                tail = this.EventTail.Text;
            }
            for (int i = 0; i < count; i++) result.Add($"{head}{this.GetStartNumberString(startNumber + i, minNo)}{tail}");
            return result;
        }

        /// <summary>
        /// 计算两个string集合重名的字符串集合
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        private IEnumerable<string> GatDuplicateIds(List<string> first, List<string> second) => first.Intersect(second);

        /// <summary>
        /// 获取重复的门的名字集合
        /// </summary>
        /// <param name="type"></param>
        /// <param name="origin"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private IEnumerable<string> GetDeDuplicateNames(RenumberedType type, IEnumerable<string> origin, IEnumerable<string> target)
        {
            var newOrigin = new List<string>(origin);
            var duplicateNames = origin.Intersect(target);
            var result = origin.Except(duplicateNames).ToList();
            var duplicateCount = duplicateNames.Count();
            var startNumber = type == RenumberedType.OnlyGate ? newOrigin.Count + Convert.ToInt32(this.GateStartNumber.Text) : newOrigin.Count + Convert.ToInt32(this.EventStartNumber.Text);
            while (duplicateCount > 0)
            {
                newOrigin = this.GetRenumberedNames(type, startNumber, duplicateCount);
                duplicateNames = newOrigin.Intersect(target);
                var aa = newOrigin.Except(duplicateNames);
                result.AddRange(aa);
                duplicateCount = duplicateNames.Count();
                startNumber += newOrigin.Count;
            }
            return result;
        }

        /// <summary>
        /// 执行重编号方法
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isDependOnGate"></param>
        /// <returns></returns>
        private List<int> Renumber(RenumberedType type, bool isDependOnGate)
        {
            // 要给出的统计结果（门重编号数、事件重编号数）
            var result = new List<int>(2);

            // 收集要重编号的DrawData对象集合（不去查找转移门里的节点）
            var targetAll = this.getAllDrawDataFromSelectedNode((RenumberedRange)this.Rp_Range.SelectedIndex).ToList();

            // 如果需要需要包括转移门，则继续搜索转移门里所有子节点
            if (this.range == RenumberedRange.SelectedTreeAndTransfer) targetAll = this.getAllDrawDataFromSelectedNode(this.range).ToList();

            // 过滤转移门节点
            targetAll = targetAll.Where(o => o.Type != DrawType.TransferInGate).ToList();

            // 事件编号和门类型编号混合情况下
            if (type.Equals(RenumberedType.BothGateAndEvent) && isDependOnGate)
            {
                // 去掉待重编号对象集合中的重复事件
                var targetGatesEvents = targetAll.Where((o, o2) => targetAll.FindIndex(o3 => o3.Identifier == o.Identifier) == o2).ToList();

                // 计算出重编号后的新编号集合
                var renumberedNames = this.GetRenumberedNames(targetGatesEvents, true);

                // 循环改编号
                for (int i = 0; i < targetGatesEvents.Count; i++)
                {
                    // 门类型编号（需要考虑转移门连带修改）
                    if (targetGatesEvents[i].IsGateType) this.ftaSystem.UpdateTranferGate(targetGatesEvents[i], renumberedNames[i]);

                    // 事件类型编号（需要考虑重复事件）
                    else this.ftaSystem.UpdateRepeatedEvent(targetGatesEvents[i], renumberedNames[i]);
                }
                // 统计编号数量
                result.Add(targetGatesEvents.Count);
            }
            else
            {
                // 收集需要重命名的门集合（从待重命名集合里找出门类型的一部分）
                var targetGates = targetAll.Where(o => o.IsGateType).ToList();

                // 收集需要重命名的事件集合 （从待重命名集合里找出事件类型的一部分） 
                var allEvents = targetAll.Where(o => !o.IsGateType).ToList();

                // 事件的待重命名集合需要去掉重复事件
                var targetEvents = allEvents.Where((o, o2) => allEvents.FindIndex(o3 => o3.Identifier == o.Identifier) == o2).ToList();

                // 计算出重命名后的门集合的名字
                var renumberedGateNames = ((type == RenumberedType.BothGateAndEvent && !isDependOnGate) || type == RenumberedType.OnlyGate) ? this.GetRenumberedNames(targetGates.ToList()) : targetGates.Select(o => o.Identifier).ToList();

                // 计算出重命名后的事件集合的名字
                var renumberedEventNames = ((type == RenumberedType.BothGateAndEvent && !isDependOnGate) || type == RenumberedType.OnlyEvent) ? this.GetRenumberedNames(targetEvents.ToList()) : targetEvents.Select(o => o.Identifier).ToList();

                // 取得系统下所有节点的编号
                var allNames = this.ftaSystem.GetAllIDs();

                // 只要是有对门重编号的
                if (type.Equals(RenumberedType.OnlyGate) || type.Equals(RenumberedType.BothGateAndEvent))
                {
                    // 要重编号的名子必须去重
                    var outPutNames = this.GetDeDuplicateNames(type, renumberedGateNames, allNames).ToList();

                    // 循环对门类型编号
                    for (int i = 0; i < targetGates.Count; i++) this.ftaSystem.UpdateTranferGate(targetGates[i], outPutNames[i]);

                    // 加入统计结果
                    result.Add(targetGates.Count);

                    // 保存门重编后后新的编号集合和事件的新编号集合对比去重
                    renumberedGateNames = outPutNames;
                }
                //  只要是对事件重编号的
                if (type.Equals(RenumberedType.OnlyEvent) || type.Equals(RenumberedType.BothGateAndEvent))
                {
                    // 先和新的重编号后门的编号集合去重
                    var outPutNames = this.GetDeDuplicateNames(type, renumberedEventNames, renumberedGateNames).ToList();

                    // 给事件对象循环编号
                    for (int i = 0; i < targetEvents.Count; i++) this.ftaSystem.UpdateRepeatedEvent(targetEvents[i], outPutNames[i]);

                    // 加入统计结果
                    result.Add(targetEvents.Count);
                }
            }
            // 刷新显示
            this.ftaSystem.UpdateRepeatedAndTranfer();

            // 返回统计结果
            return result;
        }

        ///// <summary>
        ///// 执行重编号方法
        ///// </summary>
        ///// <param name="type"></param>
        ///// <param name="isDependOnGate"></param>
        ///// <returns></returns>
        //private List<int> Renumber(RenumberedType type, bool isDependOnGate)
        //{
        //   var result = new List<int>(2);
        //   var targetAll = this.getAllDrawDataFromSelectedNode(this.RenumberRange_RadioGroup.SelectedIndex).ToList();
        //   var duplicateEvents = new List<DrawData>();
        //   if (type.Equals(RenumberedType.BothGateAndEvent) && isDependOnGate)
        //   {
        //      var deDuplicateTargets = targetAll.Where((o, o2) => targetAll.FindIndex(o3 => o3.Identifier == o.Identifier) == o2).ToList();
        //      if(this.renumberRange== RenumberedRange.SelectedTree) deDuplicateTargets = targetAll.Where((o, o2) => targetAll.FindIndex(o3 => o3.Identifier == o.Identifier && o.Type!= DrawType.TransferInGate) == o2).ToList();
        //      duplicateEvents = targetAll.Except(deDuplicateTargets).ToList();
        //      var renumberedNames = this.GetRenumberedNames(deDuplicateTargets, true);
        //      for (int i = 0; i < deDuplicateTargets.Count; i++)
        //      {
        //         if (deDuplicateTargets[i].IsGateType) this.ftaSystem.UpdateTranferGate(deDuplicateTargets[i], renumberedNames[i]);
        //         else this.ftaSystem.UpdateRepeatedEvent(deDuplicateTargets[i], renumberedNames[i]);
        //      }
        //      result.Add(deDuplicateTargets.Count);
        //   }
        //   else
        //   {
        //      var targetGates = targetAll.Where(o => o.IsGateType && o.Type != DrawType.TransferInGate).ToList();
        //      if(this.renumberRange== RenumberedRange.SelectedTreeAndTransfer) targetGates = targetAll.Where(o => o.IsGateType).ToList();
        //      var allEvents = targetAll.Where(o => !o.IsGateType && o.Type != DrawType.TransferInGate).ToList();
        //      var targetEvents = allEvents.Where((o, o2) => allEvents.FindIndex(o3 => o3.Identifier == o.Identifier) == o2).ToList();
        //      duplicateEvents = allEvents.Except(targetEvents).ToList();
        //      var renumberedGateNames = ((type == RenumberedType.BothGateAndEvent && !isDependOnGate) || type == RenumberedType.OnlyGate) ? this.GetRenumberedNames(targetGates.ToList()) : targetGates.Select(o => o.Identifier).ToList();
        //      var renumberedEventNames = ((type == RenumberedType.BothGateAndEvent && !isDependOnGate) || type == RenumberedType.OnlyEvent) ? this.GetRenumberedNames(targetEvents.ToList()) : targetEvents.Select(o => o.Identifier).ToList();
        //      var allNames = targetAll.Select(o => o.Identifier);
        //      if (type.Equals(RenumberedType.OnlyGate)|| type.Equals(RenumberedType.BothGateAndEvent))
        //      {
        //         var outPutNames =this.GetDeDuplicateNames(type, renumberedGateNames, allNames.Except(renumberedGateNames)).ToList();
        //         for (int i = 0; i < targetGates.Count; i++) this.ftaSystem.UpdateTranferGate(targetGates[i], outPutNames[i]);
        //         result.Add(targetGates.Count);
        //      }
        //      if (type.Equals(RenumberedType.OnlyEvent) || type.Equals(RenumberedType.BothGateAndEvent))
        //      {
        //         var outPutNames = this.GetDeDuplicateNames(type,renumberedEventNames, allNames.Except(renumberedEventNames)).ToList();
        //         for (int i = 0; i < targetEvents.Count; i++) this.ftaSystem.UpdateRepeatedEvent(targetEvents[i], outPutNames[i]);
        //         result.Add(targetEvents.Count);
        //      }
        //   }
        //   this.ftaSystem.UpdateRepeatedAndTranfer();
        //   return result;
        //}
        #region 事件

        /// <summary>
        /// 所有窗体内文本的TextChanged事件处理函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextEdit_TextChanged(object sender, EventArgs e)
        {
            var baseEdit = sender as BaseEdit;
            if (baseEdit.Equals(this.GateHead) || baseEdit.Equals(this.GateStartNumber) || baseEdit.Equals(this.GateMinNo) || baseEdit.Equals(this.GateTail))
            {
                this.GateExample_LabelControl.Text = this.GetExampleString(RenumberedType.OnlyGate);
            }
            else if (baseEdit.Equals(this.EventHead) || baseEdit.Equals(this.EventStartNumber) || baseEdit.Equals(this.EventMinNo) || baseEdit.Equals(this.EventTail))
            {
                this.EventExample_LabelControl.Text = this.GetExampleString(RenumberedType.OnlyEvent);
            }
        }

        /// <summary>
        /// 重编号窗体内所有RadioGroup事件的集合处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            var radioGroup = sender as RadioGroup;
            switch (radioGroup.Name)
            {
                case nameof(this.Rp_Range): break;
                case nameof(this.Rp_Type): { this.SetTypeMode(radioGroup.SelectedIndex); break; }
                default: break;//{ this.SetConfirmMode(radioGroup.SelectedIndex); break; }
            }
        }

        /// <summary>
        /// 重编号窗体内所有ChecEdit事件的集中处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckEdit_CheckedChanged(object sender, EventArgs e)
        {
            var checkEdit = sender as CheckEdit;
            switch (checkEdit.Name)
            {
                case nameof(this.Event_CheckEdit2):
                    {
                        this.EventStartNumber.Enabled = !checkEdit.Checked;
                        this.EventMinNo.Enabled = !checkEdit.Checked;
                        break;
                    }
            }
        }

        /// <summary>
        /// 向导窗体“Closing”事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RenumberFaultTrre_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.IsCancel)
            {
                if (MsgBox.Show(
                   this.ftaString.RenumberWizardCancelDialogContent,
                   this.ftaString.RenumberWizardCancelDialogTitle,
                   MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    e.Cancel = false;
                    this.IsCancel = true;
                }
                else
                {
                    e.Cancel = true;
                    this.IsCancel = false;
                }
            }
            else
            {
                var resultCount = this.Renumber(this.type, this.Event_CheckEdit2.Checked);
                this.SaveConfig(resultCount);
                this.UnsubscribeEvents();

                General.FtaProgram.CurrentSystem.ManualFiredPropertyChangedEvent(true);

                var count = 0;
                for (int i = 0; i < resultCount.Count; i++) count += resultCount[i];
                MsgBox.Show(string.Format(this.ftaString.RenumberingSucceeded, count));
            }
        }

        /// <summary>
        /// 点击向导控件的“取消”按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wizardControl1_CancelClick(object sender, CancelEventArgs e)
        {
            this.IsCancel = true;
        }


        ///// <summary>
        ///// 获取重编号后的信息
        ///// </summary>
        ///// <param name="type"></param>
        ///// <param name="isDependOnGate"></param>
        ///// <returns></returns>
        //private List<string>[] RenumberedResults(RenumberedType type, bool isDependOnGate)
        //{
        //   var result = default(List<string>[]);
        //   var targetAll = this.getAllDrawDataFromSelectedNode(this.RenumberRange_RadioGroup.SelectedIndex);
        //   var targetGates = targetAll.Where(o => o.IsGateType);
        //   var targetEvents = targetAll.Where(o => !o.IsGateType);

        //   var renumberedGateNames = default(List<string>);
        //   var renumberedEventNames = default(List<string>);

        //   if (type.Equals(RenumberedType.Both))
        //   {
        //      if (isDependOnGate == false)
        //      {
        //         result = new List<string>[2];
        //         renumberedGateNames = this.GetRenumberedNames(targetGates.ToList());
        //         renumberedEventNames = this.GetRenumberedNames(targetEvents.ToList());

        //         var duplicateIds = this.GatDuplicateIds(renumberedGateNames, renumberedEventNames).ToList();
        //         for (int i = 0; i < duplicateIds.Count; i++)
        //         {
        //            for (int j = 0; j < renumberedEventNames.Count; j++)
        //            {
        //               if (renumberedEventNames[j] == duplicateIds[i]) { renumberedEventNames[j] += "_2"; break; }
        //            }
        //         }

        //         result[0] = renumberedGateNames;
        //         result[1] = renumberedEventNames;
        //      }
        //      else
        //      {
        //         result = new List<string>[] { this.GetRenumberedNames(targetAll, true) };
        //      }
        //   }
        //   else if (type.Equals(RenumberedType.Gate))
        //   {
        //      result = new List<string>[1];
        //      renumberedGateNames = this.GetRenumberedNames(targetGates.ToList());
        //      renumberedEventNames = targetEvents.Select(o => o.Identifier).ToList();

        //      var duplicateIds = this.GatDuplicateIds(renumberedGateNames, renumberedEventNames).ToList();
        //      for (int i = 0; i < duplicateIds.Count; i++)
        //      {
        //         for (int j = 0; j < renumberedGateNames.Count; j++)
        //         {
        //            if (renumberedGateNames[j] == duplicateIds[i]) { renumberedGateNames[j] += "_2"; break; }
        //         }
        //      }
        //      result[0] = renumberedGateNames;
        //   }
        //   else if (type.Equals(RenumberedType.Event))
        //   {
        //      result = new List<string>[1];
        //      renumberedGateNames = targetGates.Select(o => o.Identifier).ToList();
        //      renumberedEventNames = this.GetRenumberedNames(targetEvents.ToList());

        //      var duplicateIds = this.GatDuplicateIds(renumberedGateNames, renumberedEventNames).ToList();
        //      for (int i = 0; i < duplicateIds.Count; i++)
        //      {
        //         for (int j = 0; j < renumberedEventNames.Count; j++)
        //         {
        //            if (renumberedEventNames[j] == duplicateIds[i]) { renumberedEventNames[j] += "_2"; break; }
        //         }
        //      }
        //      result[0] = renumberedEventNames;
        //   }


        //   return result;
        //}
        #endregion

        private void GateStartNumber_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            if (e.NewValue.ToString().Contains("-") || e.NewValue.ToString() == "0")
            {
                e.Cancel = true;
            }
        }

        private void EventStartNumber_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            if (e.NewValue.ToString().Contains("-") || e.NewValue.ToString() == "0")
            {
                e.Cancel = true;
            }
        }
    }
}