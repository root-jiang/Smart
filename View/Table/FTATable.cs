using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using FaultTreeAnalysis.Common;
using FaultTreeAnalysis.Model.Data;
using FaultTreeAnalysis.Model.Enum;
using FaultTreeAnalysis.Properties;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace FaultTreeAnalysis.View.Table
{
    public class FtaTable
    {
        private ProgramModel programModel;

        public TableEvents TableEvents { get; set; }

        private TreeList tableControl;

        public FtaTable(TreeList tableControl, ProgramModel programModel)
        {
            this.tableControl = tableControl;
            this.programModel = programModel;
            this.TableEvents = new TableEvents(this.tableControl, this.programModel);
        }

        /// <summary>
        /// 初始化FTATable(TreeList控件)模块下节点的图片
        /// </summary>
        public void InitializeImages()
        {
            //初始化图片列表
            var state_Images = new ImageList();
            using (var p = new Pen(Color.Black, General.PEN_WIDTH))
            {
                for (int i = 0; i < this.TableEvents.ImageTypes.Count; i++)
                {
                    Bitmap bit = new Bitmap(16, 16);
                    Graphics grphs = Graphics.FromImage(bit);
                    //设置图像绘制
                    grphs.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                    DrawBase.DrawBaseComponent(null, this.TableEvents.ImageTypes[i], grphs, p, 0, 0, bit.Width - p.Width, bit.Height - p.Width, true);
                    grphs.Dispose();
                    state_Images.Images.Add(bit);
                }
            }

            //剪切复制图片
            state_Images.Images.Add(Resources.cut_16x16);
            state_Images.Images.Add(Resources.copy_16x16);
            state_Images.Images.Add(new Bitmap(16, 16));
            state_Images.Images.Add(new Bitmap(16, 16));
            state_Images.Images.Add(new Bitmap(16, 16));
            this.tableControl.StateImageList = state_Images;
        }


        /// <summary>
        /// 用于重复事件颜色变化时重置fTA表左侧图形的显示图形
        /// </summary>
        public void FTATable_StateImage_ResetRepeatedEventImage()
        {
            StyleModel style = this.programModel?.CurrentProject?.Style;
            if (style == null) style = new StyleModel();
            int id = this.TableEvents.ImageTypes.Count + 4;
            ((ImageList)this.tableControl.StateImageList).Images.RemoveAt(id);
            ((ImageList)this.tableControl.StateImageList).Images.RemoveAt(id - 1);
            ((ImageList)this.tableControl.StateImageList).Images.RemoveAt(id - 2);
            Pen p = new Pen(Color.Black, General.PEN_WIDTH);

            Brush br = new SolidBrush(style.ShapeBackRepeatEventColor);

            Bitmap bit = new Bitmap(16, 16);
            Graphics grphs = Graphics.FromImage(bit);
            grphs.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            DrawBase.DrawFilledBaseComponent(null, DrawType.BasicEvent, grphs, p, br, 0, 0, bit.Width - p.Width, bit.Height - p.Width, true);
            ((ImageList)this.tableControl.StateImageList).Images.Add(bit);
            grphs.Dispose();

            bit = new Bitmap(16, 16);
            grphs = Graphics.FromImage(bit);
            grphs.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            DrawBase.DrawFilledBaseComponent(null, DrawType.HouseEvent, grphs, p, br, 0, 0, bit.Width - p.Width, bit.Height - p.Width, true);
            ((ImageList)this.tableControl.StateImageList).Images.Add(bit);
            grphs.Dispose();

            bit = new Bitmap(16, 16);
            grphs = Graphics.FromImage(bit);
            grphs.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            DrawBase.DrawFilledBaseComponent(null, DrawType.UndevelopedEvent, grphs, p, br, 0, 0, bit.Width - p.Width, bit.Height - p.Width, true);
            ((ImageList)this.tableControl.StateImageList).Images.Add(bit);
            grphs.Dispose();
            br.Dispose();
            p.Dispose();
        }

        /// <summary>
        /// 刷新界面上的表格和图表控件
        /// </summary>
        public void UpdateData(bool isRelayout = true)
        {
            if (isRelayout) this.tableControl.RefreshDataSource();
            //this.tableControl.BestFitColumns();
        }

        /// <summary>
        /// 编辑DrawData集合中的Mode字段的属性
        /// </summary>
        /// <param name="data"></param>
        /// <param name="value"></param>
        public void EditPropertyOfMode(DrawData data, string value)
        {
            //编辑属性后重复事件的处理
            if (this.programModel.CurrentSystem.RepeatedEvents.ContainsKey(data.Identifier))
            {
                HashSet<DrawData> repeats = this.programModel.CurrentSystem.RepeatedEvents[data.Identifier];
                if (!General.isRepeatCopyCurrentValues)
                {
                    data.InputType = repeats.FirstOrDefault().InputType;
                }
                else
                {
                    if (repeats != null)
                    {
                        foreach (DrawData tmp in repeats) tmp.InputType = value;
                    }
                }
            }
            else data.InputType = value;
            switch (value)
            {
                case FixedString.MODEL_CONSTANT_PROBABILITY:
                case FixedString.MODEL_FREQUENCY:
                    {//编辑属性后重复事件的处理
                        if (this.programModel.CurrentSystem.RepeatedEvents.ContainsKey(data.Identifier))
                        {
                            HashSet<DrawData> repeats = this.programModel.CurrentSystem.RepeatedEvents[data.Identifier];
                            if (!General.isRepeatCopyCurrentValues)
                            {
                                data.FRType = repeats.FirstOrDefault().FRType;
                            }
                            else
                            {
                                if (repeats != null)
                                {
                                    foreach (DrawData tmp in repeats) tmp.FRType = string.Empty;
                                }
                            }
                        }
                        else data.FRType = string.Empty;
                        break;
                    }
                case FixedString.MODEL_FR_MTBF:
                case FixedString.MODEL_FAILURE_WITH_REPAIR:
                case FixedString.MODEL_FAILURE_WITH_PERIODIC_INSPECTION:
                    {
                        if (string.IsNullOrEmpty(data.FRType))
                        {
                            //编辑属性后重复事件的处理
                            if (this.programModel.CurrentSystem.RepeatedEvents.ContainsKey(data.Identifier))
                            {
                                HashSet<DrawData> repeats = this.programModel.CurrentSystem.RepeatedEvents[data.Identifier];
                                if (!General.isRepeatCopyCurrentValues)
                                {
                                    data.FRType = repeats.FirstOrDefault().FRType;
                                }
                                else
                                {
                                    if (repeats != null)
                                    {
                                        foreach (DrawData tmp in repeats) tmp.FRType = FixedString.FAILURE_RATE_TYPE_MTBF;
                                    }
                                }
                            }
                            else data.FRType = FixedString.FAILURE_RATE_TYPE_MTBF;
                        }
                        break;
                    }
                default: break;
            }
            switch (value)
            {
                case FixedString.MODEL_CONSTANT_PROBABILITY:
                case FixedString.MODEL_FREQUENCY:
                case FixedString.MODEL_FR_MTBF:
                    {
                        //编辑属性后重复事件的处理
                        if (this.programModel.CurrentSystem.RepeatedEvents.ContainsKey(data.Identifier))
                        {
                            HashSet<DrawData> repeats = this.programModel.CurrentSystem.RepeatedEvents[data.Identifier];
                            if (!General.isRepeatCopyCurrentValues)
                            {
                                data.Units = repeats.FirstOrDefault().Units;
                            }
                            else
                            {
                                if (repeats != null)
                                {
                                    foreach (DrawData tmp in repeats) tmp.Units = string.Empty;
                                }
                            }
                        }
                        else data.Units = string.Empty;
                        break;
                    }
                case FixedString.MODEL_FAILURE_WITH_REPAIR:
                case FixedString.MODEL_FAILURE_WITH_PERIODIC_INSPECTION:
                    {
                        if (string.IsNullOrEmpty(data.Units))
                        {
                            //编辑属性后重复事件的处理
                            if (this.programModel.CurrentSystem.RepeatedEvents.ContainsKey(data.Identifier))
                            {
                                HashSet<DrawData> repeats = this.programModel.CurrentSystem.RepeatedEvents[data.Identifier];
                                if (!General.isRepeatCopyCurrentValues)
                                {
                                    data.Units = repeats.FirstOrDefault().Units;
                                }
                                else
                                {
                                    if (repeats != null)
                                    {
                                        foreach (DrawData tmp in repeats) tmp.Units = FixedString.UNITS_HOURS;
                                    }
                                }
                            }
                            else data.Units = FixedString.UNITS_HOURS;
                        }
                        break;
                    }
                default: break;
            }
            General.InvokeHandler(GlobalEvent.UpdateData, true);
        }

        /// <summary>
        /// FTA表修改时，修改有关概率的三个字段的值
        /// </summary>
        /// <param name="data">数据对象</param>
        /// <param name="newBaseProbability">对应inputValue字段值的修改</param>
        /// <param name="newTime">对应inputValue2字段值的修改</param>
        /// <param name="newUints">对应Units字段值的修改</param>
        /// <returns>是否修改成功</returns>
        public bool ChangeProbability(DrawData data, string newBaseProbability, string newTime, string newUints)
        {
            try
            {
                if (data == null) return false;
                if (newBaseProbability == data.InputValue) newBaseProbability = null;
                if (newTime == data.InputValue2) newTime = null;
                if (newUints == data.Units) newUints = null;
                else if (newUints != FixedString.UNITS_HOURS && newUints != FixedString.UNITS_MINUTES && newUints != FixedString.UNITS_HOURS_CN && newUints != FixedString.UNITS_MINUTES_CN) newUints = null;
                decimal dl__InputValue = 0;
                decimal dl__InputValue2 = 0;
                decimal qvalue = 0;
                bool IS_SetQvalue = false;

                //如果想修改基本概率失败
                if (!string.IsNullOrEmpty(newBaseProbability))
                {
                    if (!(decimal.TryParse(newBaseProbability, System.Globalization.NumberStyles.Float, null, out dl__InputValue) && dl__InputValue >= 0))
                    {
                        return false;
                    }
                }
                //如果想修改时间失败
                if (!string.IsNullOrEmpty(newTime))
                {
                    if (!(decimal.TryParse(newTime, System.Globalization.NumberStyles.Float, null, out dl__InputValue2) && dl__InputValue2 >= 0))
                    {
                        return false;
                    }
                }

                if (decimal.TryParse(string.IsNullOrEmpty(newBaseProbability) ? data.InputValue : newBaseProbability, System.Globalization.NumberStyles.Float, null, out dl__InputValue) && dl__InputValue >= 0)
                {
                    if (decimal.TryParse(string.IsNullOrEmpty(newTime) ? data.InputValue2 : newTime, System.Globalization.NumberStyles.Float, null, out dl__InputValue2) && dl__InputValue2 >= 0)
                    {
                        qvalue = (string.IsNullOrEmpty(newUints) ? data.Units : newUints) != FixedString.UNITS_MINUTES ?
                                   (dl__InputValue * dl__InputValue2)
                                   : (dl__InputValue * dl__InputValue2 / 60);
                        IS_SetQvalue = true;
                    }
                }

                //编辑属性后重复事件的处理
                if (this.programModel.CurrentSystem.RepeatedEvents.ContainsKey(data.Identifier))
                {
                    HashSet<DrawData> repeats = this.programModel.CurrentSystem.RepeatedEvents[data.Identifier];
                    if (!General.isRepeatCopyCurrentValues)
                    {
                        data.InputValue = repeats.FirstOrDefault().InputValue;
                        data.InputValue2 = repeats.FirstOrDefault().InputValue2;
                        data.Units = repeats.FirstOrDefault().Units;
                        data.QValue = repeats.FirstOrDefault().QValue;
                    }
                    else
                    {
                        if (repeats != null)
                        {
                            foreach (DrawData tmp in repeats)
                            {
                                if (!string.IsNullOrEmpty(newBaseProbability)) tmp.InputValue = newBaseProbability;
                                if (!string.IsNullOrEmpty(newTime)) tmp.InputValue2 = newTime;
                                if (!string.IsNullOrEmpty(newUints)) tmp.Units = newUints;
                                if (IS_SetQvalue) tmp.QValue = qvalue.ToString(FixedString.SCIENTIFIC_NOTATION_FORMAT);

                            }
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(newBaseProbability)) data.InputValue = newBaseProbability;
                    if (!string.IsNullOrEmpty(newTime)) data.InputValue2 = newTime;
                    if (!string.IsNullOrEmpty(newUints)) data.Units = newUints;
                    if (IS_SetQvalue) data.QValue = qvalue.ToString(FixedString.SCIENTIFIC_NOTATION_FORMAT);
                    if (data.IsGateType == false) Console.WriteLine($"Name:{data.Identifier}--Type:{data.Type}--Value:{data.QValue}");

                }

                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        /// <summary>
        /// 编辑DrawData集合中的Units字段的属性
        /// </summary>
        /// <param name="data"></param>
        /// <param name="value"></param>
        private void EditPropertyOfID(DrawData data, string value)
        {
            int result = this.TableEvents.ModifyIdentfiy(data, value);
            //result == 0 || 1 表示成功
            if ((0x00 == result)
                || (0x01 == result))
            {
                data.Identifier = value;
            }

            General.InvokeHandler(GlobalEvent.UpdateData, true);
        }

        /// <summary>
        /// 编辑DrawData集合中的Units字段的属性
        /// </summary>
        /// <param name="data"></param>
        /// <param name="value"></param>
        private void EditPropertyOfUnits(DrawData data, string value)
        {
            this.ChangeProbability(data, null, null, value);
            General.InvokeHandler(GlobalEvent.UpdateData, true);
        }

        /// <summary>
        /// 编辑DrawData集合中的VotingValue字段的属性
        /// </summary>
        /// <param name="data"></param>
        /// <param name="value"></param>
        private void EditPropertyOfVotingValue(DrawData data, string value)
        {
            var tranferGates = this.programModel.CurrentSystem.TranferGates.FirstOrDefault(o => o.Key == data.Identifier);
            if (tranferGates.Value != null) foreach (var item in tranferGates.Value) item.ExtraValue1 = value;

            var repeatedEvents = this.programModel.CurrentSystem.RepeatedEvents.FirstOrDefault(o => o.Key == data.Identifier);
            if (repeatedEvents.Value != null) foreach (var item in repeatedEvents.Value) item.ExtraValue1 = value;

            data.ExtraValue1 = value;
            General.InvokeHandler(GlobalEvent.UpdateData, true);
        }

        /// <summary>
        /// 编辑DrawData集合中的Type字段的属性
        /// </summary>
        /// <param name="data"></param>
        /// <param name="value"></param>
        private void EditPropertyOfType(DrawData data, string value)
        {
            var typeName = General.GetKeyName(value);


            var type = (DrawType)Enum.Parse(typeof(DrawType), typeName);// DrawData.GetEnumByDescription<DrawType>(value);
            if (type != DrawType.NULL && this.programModel.CurrentSystem?.TranferGates != null && this.programModel.CurrentSystem.RepeatedEvents != null)
            {
                //可有子节点的
                if (data.IsGateType && data.Type != DrawType.TransferInGate)
                {
                    //没有儿子的,转移门本体，换成了重复事件
                    if (!(data.Children != null && data.Children.Count > 0)
                    && this.programModel.CurrentSystem.TranferGates.ContainsKey(data.Identifier)
                    && (type == DrawType.BasicEvent || type == DrawType.HouseEvent || type == DrawType.UndevelopedEvent))
                    {
                        if (MsgBox.Show(this.programModel.String.ConvertToRepeatedEvents,
                            FixedString.FAULT_TREE_ANALYSIS, MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            HashSet<DrawData> trans = this.programModel.CurrentSystem.TranferGates[data.Identifier];
                            if (trans != null || trans.Count > 1)
                            {
                                foreach (DrawData tran in trans)
                                {
                                    tran.Type = type;
                                    tran.Repeats = trans.Count - 1;

                                    tran.InputType = FixedString.MODEL_LAMBDA_TAU;
                                    tran.Units = FixedString.UNITS_HOURS;
                                }
                                //维护集合
                                this.programModel.CurrentSystem.TranferGates.Remove(data.Identifier);
                                this.programModel.CurrentSystem.RepeatedEvents.Add(data.Identifier, trans);
                            }
                        }
                    }
                    else
                    {
                        data.Type = type;
                        //门转为事件
                        if (!data.IsGateType)
                        {
                            data.InputType = FixedString.MODEL_LAMBDA_TAU;
                            data.Units = FixedString.UNITS_HOURS;
                        }
                    }
                }
                //编辑属性后重复事件的处理只能换另一种事件
                else if (this.programModel.CurrentSystem.RepeatedEvents.ContainsKey(data.Identifier))
                {
                    HashSet<DrawData> repeats = this.programModel.CurrentSystem.RepeatedEvents[data.Identifier];
                    if (!General.isRepeatCopyCurrentValues)
                    {
                        data.Type = repeats.FirstOrDefault().Type;
                    }
                    else
                    {
                        if (repeats != null || repeats.Count > 1)
                        {
                            foreach (DrawData repeat in repeats)
                            {
                                repeat.Type = type;
                            }
                        }
                    }
                }
                //不重复事件
                else
                {
                    data.Type = type;
                    //事件变成门
                    if (data.IsGateType)
                    {
                        data.InputType = string.Empty;
                        data.Units = string.Empty;
                        data.InputValue = string.Empty;
                        data.InputValue2 = string.Empty;
                    }
                }
            }
            General.InvokeHandler(GlobalEvent.UpdateData, true);
        }

        /// <summary>
        /// 编辑DrawData集合中的Description字段的属性
        /// </summary>
        /// <param name="data"></param>
        /// <param name="value"></param>
        private void EditPropertyOfDescription(DrawData data, string value)
        {
            var tranferGates = this.programModel.CurrentSystem.TranferGates?.FirstOrDefault(o => o.Key == data.Identifier);
            if (tranferGates?.Value != null) foreach (var item in tranferGates?.Value) item.Comment1 = value;

            //编辑属性后重复事件的处理
            if (this.programModel.CurrentSystem.RepeatedEvents.ContainsKey(data.Identifier))
            {
                HashSet<DrawData> repeats = this.programModel.CurrentSystem.RepeatedEvents[data.Identifier];
                if (!General.isRepeatCopyCurrentValues)
                {
                    data.Comment1 = repeats.FirstOrDefault().Comment1;
                }
                else
                {
                    if (repeats != null)
                    {
                        foreach (DrawData tmp in repeats)
                        {
                            tmp.Comment1 = value;
                        }
                    }
                }
            }
            else
            {
                data.Comment1 = value;
            }
            General.InvokeHandler(GlobalEvent.UpdateData, true);
        }

        /// <summary>
        /// 编辑DrawData集合中的InputType字段的属性
        /// </summary>
        /// <param name="data"></param>
        /// <param name="value"></param>
        private void EditPropertyOfInputType(DrawData data, string value)
        {
            var tranferGates = this.programModel.CurrentSystem.TranferGates.FirstOrDefault(o => o.Key == data.Identifier);
            if (tranferGates.Value != null) foreach (var item in tranferGates.Value) item.InputType = value;

            //编辑属性后重复事件的处理
            if (this.programModel.CurrentSystem.RepeatedEvents.ContainsKey(data.Identifier))
            {
                HashSet<DrawData> repeats = this.programModel.CurrentSystem.RepeatedEvents[data.Identifier];
                if (!General.isRepeatCopyCurrentValues)
                {
                    data.InputType = repeats.FirstOrDefault().InputType;
                }
                else
                {
                    if (repeats != null)
                    {
                        foreach (DrawData tmp in repeats)
                        {
                            tmp.InputType = value;
                        }
                    }
                }
            }
            else
            {
                data.InputType = value;
            }
            General.InvokeHandler(GlobalEvent.UpdateData, true);
        }

        /// <summary>
        /// 编辑DrawData集合中的InputValue字段的属性
        /// </summary>
        /// <param name="data"></param>
        /// <param name="value"></param>
        private void EditPropertyOfInputValue(DrawData data, string value)
        {
            try
            {
                if (data.InputType == this.programModel.String.ConstantProbability)
                {
                    double dData = 0.0;
                    dData = double.Parse(value, System.Globalization.NumberStyles.Float);
                    if (dData < 0 || dData > 1)
                    {
                        value = data.InputValue;
                    }
                }
            }
            catch (Exception ex)
            {
                return;
            }

            var tranferGates = this.programModel.CurrentSystem.TranferGates.FirstOrDefault(o => o.Key == data.Identifier);
            if (tranferGates.Value != null) foreach (var item in tranferGates.Value) item.InputValue = value;

            //编辑属性后重复事件的处理
            if (this.programModel.CurrentSystem.RepeatedEvents.ContainsKey(data.Identifier))
            {
                HashSet<DrawData> repeats = this.programModel.CurrentSystem.RepeatedEvents[data.Identifier];
                if (!General.isRepeatCopyCurrentValues)
                {
                    data.InputValue = repeats.FirstOrDefault().InputValue;
                }
                else
                {
                    if (repeats != null)
                    {
                        foreach (DrawData tmp in repeats)
                        {
                            tmp.InputValue = value;
                        }
                    }
                }
            }
            else
            {
                data.InputValue = value;
            }
            General.InvokeHandler(GlobalEvent.UpdateData, true);
        }

        /// <summary>
        /// 编辑DrawData集合中的InputValue2字段的属性
        /// </summary>
        /// <param name="data"></param>
        /// <param name="value"></param>
        private void EditPropertyOfInputValue2(DrawData data, string value)
        {
            var tranferGates = this.programModel.CurrentSystem.TranferGates.FirstOrDefault(o => o.Key == data.Identifier);
            if (tranferGates.Value != null) foreach (var item in tranferGates.Value) item.InputValue2 = value;

            //编辑属性后重复事件的处理
            if (this.programModel.CurrentSystem.RepeatedEvents.ContainsKey(data.Identifier))
            {
                HashSet<DrawData> repeats = this.programModel.CurrentSystem.RepeatedEvents[data.Identifier];
                if (!General.isRepeatCopyCurrentValues)
                {
                    data.InputValue2 = repeats.FirstOrDefault().InputValue2;
                }
                else
                {
                    if (repeats != null)
                    {
                        foreach (DrawData tmp in repeats)
                        {
                            tmp.InputValue2 = value;
                        }
                    }
                }
            }
            else
            {
                data.InputValue2 = value;
            }
            General.InvokeHandler(GlobalEvent.UpdateData, true);
        }

        /// <summary>
        /// 编辑DrawData集合中的LogicalCondition字段的属性
        /// </summary>
        /// <param name="data"></param>
        /// <param name="value"></param>
        private void EditPropertyOfLogicalCondition(DrawData data, string value)
        {
            //转移门本体
            if (this.programModel.CurrentSystem.TranferGates.ContainsKey(data.Identifier))
            {
                HashSet<DrawData> trans = this.programModel.CurrentSystem.TranferGates[data.Identifier];
                if (trans != null)
                {
                    foreach (DrawData tmp in trans)
                    {
                        tmp.LogicalCondition = value;
                    }
                }
            }
            //编辑属性后重复事件的处理
            else if (this.programModel.CurrentSystem.RepeatedEvents.ContainsKey(data.Identifier))
            {
                HashSet<DrawData> repeats = this.programModel.CurrentSystem.RepeatedEvents[data.Identifier];
                if (!General.isRepeatCopyCurrentValues)
                {
                    data.LogicalCondition = repeats.FirstOrDefault().LogicalCondition;
                }
                else
                {
                    if (repeats != null)
                    {
                        foreach (DrawData tmp in repeats)
                        {
                            tmp.LogicalCondition = value;
                        }
                    }
                }
            }
            else data.LogicalCondition = value;
            General.InvokeHandler(GlobalEvent.UpdateData, true);
        }

        /// <summary>
        /// 编辑DrawData集合中的FailureRateType字段的属性
        /// </summary>
        /// <param name="data"></param>
        /// <param name="value"></param>
        private void EditPropertyOfFailureRateType(DrawData data, string value)
        {
            //编辑属性后重复事件的处理
            if (this.programModel.CurrentSystem.RepeatedEvents.ContainsKey(data.Identifier))
            {
                HashSet<DrawData> repeats = this.programModel.CurrentSystem.RepeatedEvents[data.Identifier];
                if (!General.isRepeatCopyCurrentValues)
                {
                    data.FRType = repeats.FirstOrDefault().FRType;
                }
                else
                {
                    if (repeats != null)
                    {
                        foreach (DrawData tmp in repeats) tmp.FRType = value;
                    }
                }
            }
            else data.FRType = value;
            General.InvokeHandler(GlobalEvent.UpdateData, true);
        }

        /// <summary>
        /// 通过传入的列名来编辑不同字段的属性
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="data"></param>
        /// <param name="value"></param>
        private void EditProperty(string columnName, DrawData data, string value)
        {
            //(下拉框)对象属性改变前
            FaultTreeAnalysis.Behavior.ObjectBehaveEntity oBehaveEntity = null;
            if ((columnName == nameof(StringModel.Type))
                || (columnName == nameof(StringModel.LogicalCondition))
                || (columnName == nameof(StringModel.InputType))
                || (columnName == nameof(StringModel.FRType))
                || (columnName == nameof(StringModel.Units)) || (columnName == nameof(StringModel.InputValue))
                || (columnName == nameof(StringModel.InputValue2)) || (columnName == nameof(StringModel.Identifier)))
            {
                if ((null != this.programModel)
                    && (null != this.programModel.CurrentSystem))
                {
                    oBehaveEntity = this.programModel.CurrentSystem.TakeBehavor(null, data);
                }
            }

            switch (columnName)
            {
                //修改类型
                case nameof(StringModel.Type): { this.EditPropertyOfType(data, value); break; }
                case nameof(StringModel.LogicalCondition): { this.EditPropertyOfLogicalCondition(data, value); break; }
                case nameof(StringModel.InputType): { this.EditPropertyOfMode(data, value); break; }
                case nameof(StringModel.FRType): { this.EditPropertyOfFailureRateType(data, value); break; }
                case nameof(StringModel.Units): { this.EditPropertyOfUnits(data, value); break; }
                case nameof(StringModel.InputValue): { this.EditPropertyOfInputValue(data, value); break; }
                case nameof(StringModel.InputValue2): { this.EditPropertyOfInputValue2(data, value); break; }
                case nameof(StringModel.Identifier): { this.EditPropertyOfID(data, value); break; }
                default: break;
            }

            //(下拉框)对象属性改变后
            if ((columnName == nameof(StringModel.Type))
                || (columnName == nameof(StringModel.LogicalCondition))
                || (columnName == nameof(StringModel.InputType))
                || (columnName == nameof(StringModel.FRType))
                || (columnName == nameof(StringModel.Units)) || (columnName == nameof(StringModel.InputValue))
                || (columnName == nameof(StringModel.InputValue2)) || (columnName == nameof(StringModel.Identifier)))
            {
                if ((null != this.programModel)
                && (null != this.programModel.CurrentSystem)
                && (null != oBehaveEntity))
                {
                    //
                    oBehaveEntity.Cause = Behavior.Enum.ElementOperate.AlterProperty;
                    oBehaveEntity.Effect = Behavior.Enum.ElementOperate.AlterProperty;
                    this.programModel.CurrentSystem.TakeBehavor(oBehaveEntity, data);
                }
            }
        }

        /// <summary>
        /// 修改多个字段
        /// </summary>
        /// <param name="data"></param>
        /// <param name="values"></param>
        private void EditProperties(DrawData data, string[] values)
        {
            //(弹窗)对象属性改变前
            FaultTreeAnalysis.Behavior.ObjectBehaveEntity oBehaveEntity = null;
            if ((null != this.programModel)
                && (null != this.programModel.CurrentSystem))
            {
                oBehaveEntity = this.programModel.CurrentSystem.TakeBehavor(null, data);
            }

            this.TableEvents.ModifyIdentfiy(data, values[0]);
            if (this.TableEvents.IsJoinRepeatEvent == false)
            {
                this.EditPropertyOfType(data, values[1]);
                this.EditPropertyOfDescription(data, values[2]);
                this.EditPropertyOfLogicalCondition(data, values[3]);

                if (values.Length == 9)
                {
                    this.EditPropertyOfInputValue(data, values[4]);
                    this.EditPropertyOfInputValue2(data, values[5]);
                    this.EditPropertyOfUnits(data, values[6]);
                    this.EditPropertyOfInputType(data, values[7]);
                    this.EditPropertyOfVotingValue(data, values[8]);
                }
                else
                {
                    this.EditPropertyOfVotingValue(data, values[4]);
                }

                //(弹窗)对象属性改变后
                if ((null != this.programModel)
                    && (null != this.programModel.CurrentSystem)
                    && (null != oBehaveEntity))
                {
                    //
                    oBehaveEntity.Cause = Behavior.Enum.ElementOperate.AlterProperty;
                    oBehaveEntity.Effect = Behavior.Enum.ElementOperate.AlterProperty;
                    this.programModel.CurrentSystem.TakeBehavor(oBehaveEntity, data);
                }
            }
            else General.InvokeHandler(GlobalEvent.UpdateData, true);
            this.TableEvents.IsJoinRepeatEvent = false;
        }


        public void EditProperty(object param)
        {
            var value = param as Tuple<string, DrawData, string>;
            this.EditProperty(value.Item1, value.Item2, value.Item3);
            //自动更新基本事件库
            try
            {
                if (value.Item2.GUID != null && value.Item2.GUID != "")
                {
                    if (ConnectSever.CheckExist(value.Item2.GUID))
                    {
                        ConnectSever.UpdateOne(new object[] { value.Item2.GUID, value.Item2.Group, value.Item2.Identifier, value.Item2.Type.ToString(), value.Item2.Comment1, value.Item2.LogicalCondition, value.Item2.InputType, value.Item2.InputValue, value.Item2.InputValue2, value.Item2.ExtraValue1, value.Item2.Units });
                    }
                    else
                    {
                        ConnectSever.InsertOne(new object[] { value.Item2.GUID, value.Item2.Group, value.Item2.Identifier, value.Item2.Type.ToString(), value.Item2.Comment1, value.Item2.LogicalCondition, value.Item2.InputType, value.Item2.InputValue, value.Item2.InputValue2, value.Item2.ExtraValue1, value.Item2.Units });
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public void EditProperties(object param)
        {
            var value = param as Tuple<DrawData, string[]>;
            this.EditProperties(value.Item1, value.Item2);
            //自动更新基本事件库
            try
            {
                if (value.Item1.GUID != null && value.Item1.GUID != "")
                {
                    if (ConnectSever.CheckExist(value.Item1.GUID))
                    {
                        ConnectSever.UpdateOne(new object[] { value.Item1.GUID, value.Item1.Group, value.Item1.Identifier, value.Item1.Type.ToString(), value.Item1.Comment1, value.Item1.LogicalCondition, value.Item1.InputType, value.Item1.InputValue, value.Item1.InputValue2, value.Item1.ExtraValue1, value.Item1.Units });
                    }
                    else
                    {
                        ConnectSever.InsertOne(new object[] { value.Item1.GUID, value.Item1.Group, value.Item1.Identifier, value.Item1.Type.ToString(), value.Item1.Comment1, value.Item1.LogicalCondition, value.Item1.InputType, value.Item1.InputValue, value.Item1.InputValue2, value.Item1.ExtraValue1, value.Item1.Units });
                    }
                }
            }
            catch (Exception)
            {
            }
        }


        /// <summary>
        /// FTA表修改时，对于普通文本值字段修改通用的操作
        /// </summary>
        /// <param name="data">数据对象</param>
        /// <param name="propertyName">数据对象里文本类型的属性名字</param>
        /// <param name="e">事件参数</param>
        private void ChangeCommonCellValue(DrawData data, string propertyName, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            General.TryCatch(() =>
            {//先对值得合法性检查
                if (e.Value == null || e.Value.GetType() != typeof(string))
                {
                    e.Valid = false;
                    e.ErrorText = this.programModel.String.EmptyError;
                    return;
                }
                string Value = e.Value as string;

                //如果是之前的值不变就好
                if (Value.Equals(data.GetType().GetProperty(propertyName).GetValue(data))) return;
                Value = Value.Trim();
                //转移门本体
                if (this.programModel.CurrentSystem.TranferGates.ContainsKey(data.Identifier))
                {
                    HashSet<DrawData> trans = this.programModel.CurrentSystem.TranferGates[data.Identifier];
                    if (trans != null)
                    {
                        foreach (DrawData tmp in trans)
                        {
                            tmp.GetType().GetProperty(propertyName).SetValue(tmp, Value);
                        }
                    }
                }
                //编辑属性后重复事件的处理
                else if (this.programModel.CurrentSystem.RepeatedEvents.ContainsKey(data.Identifier))
                {
                    HashSet<DrawData> repeats = this.programModel.CurrentSystem.RepeatedEvents[data.Identifier];
                    if (!General.isRepeatCopyCurrentValues)
                    {
                        data.GetType().GetProperty(propertyName).SetValue(data, repeats.FirstOrDefault().GetType().GetProperty(propertyName).GetValue(data));
                    }
                    else
                    {
                        if (repeats != null)
                        {
                            foreach (DrawData tmp in repeats)
                            {
                                tmp.GetType().GetProperty(propertyName).SetValue(tmp, Value);
                            }
                        }
                    }
                }
                else data.GetType().GetProperty(propertyName).SetValue(data, Value);
            });
        }

        public void ChangeCommonCellValue(object param)
        {
            var value = param as Tuple<DrawData, string, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs>;
            this.ChangeCommonCellValue(value.Item1, value.Item2, value.Item3);
        }

        /// <summary>
        /// 获取当前树视图里指定drawdata对象的节点
        /// </summary>
        /// <param name="data">要获取节点对象的数据对象</param>
        public TreeListNode FTATable_GetTreeListNode(DrawData data)
        {
            TreeListNode node_Result = null;
            try
            {
                if (data == null) return null;

                //刚好是选中的，不操作
                if (this.tableControl.FocusedNode != null && this.tableControl.FocusedNode.GetValue(FixedString.COLUMNAME_DATA) == data)
                {
                    return this.tableControl.FocusedNode;
                }

                //找到该对象由最顶层父它的一串对象
                DrawData tmp = data;
                Stack<DrawData> stack = new Stack<DrawData>();
                while (tmp != null)
                {
                    stack.Push(tmp);
                    tmp = tmp.Parent;
                }

                TreeListNodes nodes_Current = this.tableControl.Nodes;

                //找到需要的节点对象
                TreeListNode node = null;
                while (stack.Count > 0)
                {
                    if (nodes_Current == null) break;

                    DrawData parent = stack.Pop();

                    node = nodes_Current.Where(obj => obj.GetValue(FixedString.COLUMNAME_DATA) != null && ((DrawData)obj.GetValue(FixedString.COLUMNAME_DATA)).ThisGuid == parent.ThisGuid).LastOrDefault();
                    if (node == null)
                    {
                        break;
                    }
                    else
                    {
                        if (stack.Count == 0)
                        {
                            node_Result = node;
                        }
                        else
                        {
                            //还没找到，就下层扩展，再搜寻
                            if (!node.Expanded) node.Expand();
                            nodes_Current = node.Nodes;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //XtraMessageBox.Show(FixedString.EXCEPTION + ex.Message);
            }
            return node_Result;
        }

        /// <summary>
        /// 使指定drawdata对象的节点可见
        /// </summary>
        /// <param name="data">要可见的数据对象</param>
        public void FocusOn(DrawData data)
        {

            if (data == null) return;
            TreeListNode node = FTATable_GetTreeListNode(data);
            if (node == null) return;
            if (!node.Expanded) node.ExpandAll();
            if (node == this.tableControl.FocusedNode) this.tableControl.MakeNodeVisible(this.tableControl.FocusedNode);
            else
            {
                //防止触发节点改变事件
                General.IsIgnoreTreeListFocusNodeChangeEvent = true;
                //找到了我们要的节点对象，设置焦点，显示到该处
                this.tableControl.MakeNodeVisible(node);
                node.Selected = true;
                General.IsIgnoreTreeListFocusNodeChangeEvent = false;
            }
            //宽度自适应
            //this.tableControl.BestFitColumns();
        }

        /// <summary>
        /// FTA表和图通用的用于粘贴重复事件，之前没有就是新建，有的话就是增加,由于在其他地方也用了所以参数搞多点
        /// </summary>
        /// <param name="dest">目标数据对象</param>
        /// <param name="FTATable_Is_CopyNode">是复制还是剪切</param>
        /// <param name="FTATableDiagram_DrawData_CopyOrCut">要复制或剪切的源对象</param>
        /// <param name="FTATableDiagram_Is_CopyOrCut_Recurse">是否递归</param>
        /// <returns>粘贴后产生的（复制出的）新的数据对象</returns>
        public DrawData PasteRepeatedEvent(DrawData dest, bool FTATable_Is_CopyNode, DrawData FTATableDiagram_DrawData_CopyOrCut, bool FTATableDiagram_Is_CopyOrCut_Recurse)
        {
            DrawData result = null;

            if (dest == null || FTATableDiagram_DrawData_CopyOrCut == null) return null;

            //复制   
            if (FTATable_Is_CopyNode)
            {
                //同一个故障树下的粘贴
                if (General.CopyCutSystem == null || this.programModel.CurrentSystem == General.CopyCutSystem)
                {
                    DrawData drawData_Copied = null;
                    HashSet<string> ids = this.programModel.CurrentSystem.GetAllIDs();
                    //递归
                    if (FTATableDiagram_Is_CopyOrCut_Recurse)
                    {
                        drawData_Copied = FTATableDiagram_DrawData_CopyOrCut.CopyDrawDataRecurse(ids, true);
                    }
                    else
                    {
                        drawData_Copied = FTATableDiagram_DrawData_CopyOrCut.CopyDrawData(ids, true);
                    }

                    if (drawData_Copied == null) return null;

                    //分别处理复制出的对象里重复事件和转移门副本
                    List<DrawData> no_Repeat_Source = FTATableDiagram_DrawData_CopyOrCut.GetAllData(General.CopyCutSystem).
                        Where(obj => obj.CanRepeatedType && !this.programModel.CurrentSystem.RepeatedEvents.ContainsKey(obj.Identifier)).ToList();
                    List<DrawData> allData_Copied = drawData_Copied.GetAllData(General.CopyCutSystem);
                    foreach (DrawData data in no_Repeat_Source)
                    {
                        this.programModel.CurrentSystem.AddRepeatedEvent(data);
                    }
                    foreach (DrawData data in allData_Copied)
                    {
                        if (data.CanRepeatedType)
                        {
                            this.programModel.CurrentSystem.AddRepeatedEvent(data);
                        }
                        //转移门
                        else if (this.programModel.CurrentSystem.TranferGates != null && this.programModel.CurrentSystem.TranferGates.ContainsKey(data.Identifier)
                            && data.Type == DrawType.TransferInGate)
                        {
                            this.programModel.CurrentSystem.AddTranferGate(data);
                        }
                    }

                    //绑定父子关系
                    if (dest.Children == null) dest.Children = new List<DrawData>();
                    dest.Children.Add(drawData_Copied);
                    drawData_Copied.Parent = dest;
                    result = drawData_Copied;
                }
                else
                {
                    //跨故障树
                    DrawData drawData_Copied = null;
                    //if (General.CopyCutSystem == null)
                    //    return null;
                    HashSet<string> ids = General.CopyCutSystem.GetAllIDs();
                    //递归
                    if (FTATableDiagram_Is_CopyOrCut_Recurse)
                    {
                        drawData_Copied = FTATableDiagram_DrawData_CopyOrCut.CopyDrawDataRecurse(ids, true);
                    }
                    else
                    {
                        drawData_Copied = FTATableDiagram_DrawData_CopyOrCut.CopyDrawData(ids, true);
                    }

                    if (drawData_Copied == null) return null;

                    //分别处理复制出的对象里重复事件和转移门副本
                    List<DrawData> no_Repeat_Source = drawData_Copied.GetAllData(General.CopyCutSystem).Where(obj => obj.CanRepeatedType && (this.programModel.CurrentSystem.GetAllIDs().Contains(obj.Identifier))).ToList();

                    List<string> duplicates = drawData_Copied.GetAllData(General.CopyCutSystem).GroupBy(n => n.Identifier).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
                    //分别处理复制出的对象里重复事件和转移门副本
                    List<DrawData> no_Repeat_SourceNew = drawData_Copied.GetAllData(General.CopyCutSystem).Where(obj => obj.CanRepeatedType && duplicates.Contains(obj.Identifier)).ToList();
                    List<DrawData> no_Repeat_SourceOld = this.programModel.CurrentSystem.GetAllDatas().Where(obj => obj.CanRepeatedType && (drawData_Copied.GetAllData(General.CopyCutSystem).Where(d => d.Identifier == obj.Identifier).Count() > 0)).ToList();

                    List<DrawData> allData_Copied = drawData_Copied.GetAllData(General.CopyCutSystem);
                    foreach (DrawData data in no_Repeat_Source)
                    {
                        this.programModel.CurrentSystem.AddRepeatedEvent(data);
                    }
                    foreach (DrawData data in no_Repeat_SourceNew)
                    {
                        this.programModel.CurrentSystem.AddRepeatedEvent(data);
                    }
                    foreach (DrawData data in no_Repeat_SourceOld)
                    {
                        this.programModel.CurrentSystem.AddRepeatedEvent(data);
                    }
                    foreach (DrawData data in allData_Copied)
                    {
                        //转移门
                        if (this.programModel.CurrentSystem.TranferGates != null && this.programModel.CurrentSystem.TranferGates.ContainsKey(data.Identifier)
                           && data.Type == DrawType.TransferInGate)
                        {
                            this.programModel.CurrentSystem.AddTranferGate(data);
                        }
                    }

                    //绑定父子关系
                    if (dest.Children == null) dest.Children = new List<DrawData>();
                    dest.Children.Add(drawData_Copied);
                    drawData_Copied.Parent = dest;
                    result = drawData_Copied;
                }
            }
            General.InvokeHandler(GlobalEvent.UpdateLayout);

            return result;
        }
    }
}
