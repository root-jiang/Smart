using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using FaultTreeAnalysis.Common;
using FaultTreeAnalysis.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils.Menu;
using DevExpress.XtraTreeList.Menu;
using FaultTreeAnalysis.Model.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using Aspose.Pdf;

namespace FaultTreeAnalysis.View.Table
{
    public class TableEvents
    {
        private TreeList tableControl;

        private ProgramModel programModel;

        public bool IsJoinRepeatEvent { get; set; }

        /// <summary>
        /// FTA表或图是否复制节点（false）剪切节点（true）
        /// </summary>
        public bool IsCopyNode { get; set; }

        /// <summary>
        /// 用于绘制节点图片的图形类型
        /// </summary>
        public List<DrawType> ImageTypes = new List<DrawType>
      {
         DrawType.AndGate, DrawType.OrGate,DrawType.BasicEvent,DrawType.UndevelopedEvent,
         DrawType.TransferInGate,DrawType.HouseEvent,DrawType.PriorityAndGate,
         DrawType.ConditionEvent,DrawType.XORGate,DrawType.VotingGate,DrawType.RemarksGate
      };

        public TableEvents(TreeList tableControl, ProgramModel programModel)
        {
            this.tableControl = tableControl;
            this.programModel = programModel;
            this.RegisterEvents();
        }

        public void RegisterEvents()
        {
            //自定义在默认编辑器编辑时值是否ok
            this.tableControl.ValidatingEditor += ValidatingEditor;

            //自定义单击单元格时弹出的编辑器
            this.tableControl.CustomNodeCellEdit += CustomNodeCellEdit;

            //在显示自定义窗体之前,注册textbox事件等操作
            this.tableControl.ShowCustomizationForm += ShowCustomizationForm;

            //标题列位置变化可视变化记录
            this.tableControl.ColumnPositionChanged += ColumnPositionChanged;

            // 自定义编辑列窗口隐藏
            this.tableControl.HideCustomizationForm += HideCustomizationForm;

            //翻译他的菜单项，和添加自定义的菜单项
            this.tableControl.PopupMenuShowing += PopupMenuShowing;

            //自定义选中的单元格能否编辑值
            this.tableControl.ShowingEditor += ShowingEditor;

            //自定义绘制单元格背景前景样式
            this.tableControl.CustomDrawNodeCell += CustomDrawNodeCell;

            //刷新下数据
            this.tableControl.CellValueChanged += CellValueChanged;

            //当选中节点变化时，使选中的图形也变化
            this.tableControl.FocusedNodeChanged += FocusedNodeChanged;

            //给节点设置图片
            this.tableControl.GetStateImage += this.GetStateImage_treeList_FTATable;

            //自定义绘制FTA表左侧指示器图形
            this.tableControl.CustomDrawNodeIndicator += this.CustomDrawNodeIndicator_TreeList_FTATable;

            this.tableControl.CustomDrawNodeCell += TableControl_CustomDrawNodeCell;

            this.tableControl.KeyDown += KeyDown;
        }

        private void KeyDown(object sender, KeyEventArgs e)
        {
            var eventValue = string.Empty;
            bool isEmpty = false;
            if (e.Control && e.KeyCode == Keys.C) eventValue = nameof(StringModel.CopyNodes);
            else if (e.Control && e.KeyCode == Keys.V) eventValue = nameof(StringModel.Paste);
            else if (e.Control && e.KeyCode == Keys.X) eventValue = nameof(StringModel.CutNodes);
            else if (e.Control && e.KeyCode == Keys.R) eventValue = nameof(StringModel.Redo);
            else if (e.Control && e.KeyCode == Keys.Z) eventValue = nameof(StringModel.Undo);
            else if (e.KeyCode == Keys.Delete)//快捷键删除时，按当前选中项情况区分删除
            {
                if (this.tableControl.FocusedNode != null)
                {
                    object obj = General.TableControl.FocusedNode.GetValue(FixedString.COLUMNAME_DATA);
                    if (obj == null)
                    {
                        return;
                    }

                    DrawData SeData = (DrawData)obj;

                    if (SeData == General.FtaProgram.CurrentRoot)//顶层节点
                    {
                        eventValue = nameof(StringModel.DeleteTop);
                    }
                    else if (SeData?.Parent != null && SeData.Children.Count == 1)//只有一个子节点
                    {
                        eventValue = nameof(StringModel.DeleteNodes);
                    }
                    else if (SeData?.Parent != null && SeData.Children.Count == 0)//没有子节点
                    {
                        eventValue = nameof(StringModel.DeleteNode);
                    }
                    else
                    {
                        eventValue = nameof(StringModel.DeleteNodes);//多个子节点
                    }
                }
                else
                {
                    return;
                }
            }
            else isEmpty = true;

            if (isEmpty == false) General.InvokeHandler(GlobalEvent.TableShortCut, eventValue);
        }

        private void TableControl_CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e)
        {
            TreeList node = sender as TreeList;
            if (e.Node == node.FocusedNode)
            {
                e.Graphics.FillRectangle(Brushes.LightSlateGray, e.Bounds);
                var rectangle = new System.Drawing.Rectangle
                   (e.EditViewInfo.ContentRect.Left,
                   e.EditViewInfo.ContentRect.Top,
                   Convert.ToInt32(e.Graphics.MeasureString(e.CellText, node.Font).Width + 1),
                   Convert.ToInt32(e.Graphics.MeasureString(e.CellText, node.Font).Height));
                //e.Graphics.FillRectangle(SystemBrushes.Highlight, Rectangler);
                e.Graphics.DrawString(e.CellText, node.Font, Brushes.Black, rectangle);
                e.Handled = true;
            }
        }

        /// <summary>
        /// 自定义在默认编辑器编辑时值是否ok,并且赋值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
        {
            General.TryCatch(() =>
            {
                //设置焦点
                General.TableControl.Focus();
                if (General.TableControl.FocusedNode != null && General.TableControl.FocusedColumn != null)
                {
                    object obj = General.TableControl.FocusedNode.GetValue(FixedString.COLUMNAME_DATA);
                    if (obj?.GetType() == typeof(DrawData) && this.programModel.CurrentSystem?.TranferGates != null && this.programModel.CurrentSystem?.RepeatedEvents != null)
                    {
                        DrawData data = obj as DrawData;
                        string strColumnName = General.TableControl.FocusedColumn.Name;

                        //(输入框)对象属性改变前
                        bool bBehaveApply = false;
                        FaultTreeAnalysis.Behavior.ObjectBehaveEntity oBehaveEntity = null;
                        if ((strColumnName == nameof(StringModel.Identifier))
                            || (strColumnName == nameof(StringModel.Comment1))
                            || (strColumnName == nameof(StringModel.InputValue))
                            || (strColumnName == nameof(StringModel.InputValue2))
                            || (strColumnName == (FixedString.COLUMNAME_EXTRAVALUE1))
                            || (strColumnName == (FixedString.COLUMNAME_EXTRAVALUE2))
                            || (strColumnName == (FixedString.COLUMNAME_EXTRAVALUE3))
                            || (strColumnName == (FixedString.COLUMNAME_EXTRAVALUE4))
                            || (strColumnName == (FixedString.COLUMNAME_EXTRAVALUE5))
                            || (strColumnName == (FixedString.COLUMNAME_EXTRAVALUE6))
                            || (strColumnName == (FixedString.COLUMNAME_EXTRAVALUE7))
                            || (strColumnName == (FixedString.COLUMNAME_EXTRAVALUE8))
                            || (strColumnName == (FixedString.COLUMNAME_EXTRAVALUE9))
                            || (strColumnName == (FixedString.COLUMNAME_EXTRAVALUE10)))
                        {
                            if ((null != this.programModel)
                                && (null != this.programModel.CurrentSystem))
                            {
                                oBehaveEntity = this.programModel.CurrentSystem.TakeBehavor(null, data);
                            }
                        }

                        //
                        switch (strColumnName)
                        {
                            // 编辑ID
                            case nameof(StringModel.Identifier):
                                {
                                    // ID列的修改检查和更新
                                    //先对值得合法性检查
                                    if (e.Value?.GetType() != typeof(string))
                                    {
                                        e.Valid = false;
                                        e.ErrorText = this.programModel.String.CanNotEmpty;
                                    }
                                    else
                                    {
                                        var result = this.ModifyIdentfiy(data, e.Value.ToString());
                                        switch (result)
                                        {
                                            case 2: { e.Valid = false; e.ErrorText = this.programModel.String.CanNotEmpty; break; }
                                            case 1: { e.Value = data.Identifier; break; }
                                            default: break;
                                        }
                                        //Bug ???
                                        //result == 0 || 1 表示成功
                                        if ((0x00 == result)
                                            || (0x01 == result))
                                        {
                                            //属性成功修改
                                            bBehaveApply = true;
                                        }
                                    }
                                    break;
                                }

                            //case FixedString.COLUMNAME_COMMENT:
                            case nameof(StringModel.Comment1):
                                {
                                    General.InvokeHandler(GlobalEvent.CommonCellValueChanged, new Tuple<DrawData, string, BaseContainerValidateEditorEventArgs>(data, nameof(StringModel.Comment1), e));

                                    //属性成功修改
                                    bBehaveApply = true;

                                    break;
                                }
                            //case FixedString.COLUMNAME_INPUTVALUE:
                            case nameof(StringModel.InputValue):
                                {
                                    #region inputvalue字段修改
                                    //先对值得合法性检查
                                    if (e.Value == null || e.Value.GetType() != typeof(string))
                                    {
                                        e.Valid = false;
                                        e.ErrorText = this.programModel.String.CanNotEmpty;
                                        return;
                                    }

                                    try//检查选择固定概率时，概率值的范围是否满足0=<X<=1
                                    {
                                        if (e.Value != null && e.Value.ToString() != "")
                                        {
                                            string TP = tableControl.FocusedNode.GetValue("InputType").ToString();

                                            double dData = 0.0;
                                            dData = double.Parse(e.Value.ToString(), System.Globalization.NumberStyles.Float);

                                            if (TP == this.programModel.String.ConstantProbability)
                                            {
                                                if (dData < 0 || dData > 1)
                                                {
                                                    e.Valid = false;
                                                    e.ErrorText = this.programModel.String.InputValue_Failed;
                                                    return;
                                                }
                                            }
                                            else
                                            {
                                                if (dData < 0 || dData > 1)
                                                {
                                                    e.Valid = false;
                                                    e.ErrorText = this.programModel.String.InputValue_Failed;
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        e.Valid = false;
                                        e.ErrorText = this.programModel.String.InputValue_Failed2 + ":" + ex.Message;
                                        return;
                                    }

                                    string inputValue = e.Value as string;
                                    //如果是之前的值不变就好
                                    if (inputValue == data.InputValue) return;
                                    inputValue = inputValue.Trim();
                                    if (string.IsNullOrEmpty(inputValue))
                                    {
                                        e.Valid = false;
                                        e.ErrorText = this.programModel.String.CanNotEmpty;
                                        return;
                                    }

                                    if (!General.ChangeProbability(data, inputValue, null, null))
                                    {
                                        e.Valid = false;
                                        e.ErrorText = this.programModel.String.PositiveNumber;
                                        return;
                                    }

                                    //属性成功修改
                                    //bBehaveApply = true;

                                    #endregion
                                    break;
                                }
                            //case FixedString.COLUMNAME_INPUTVALUE2:
                            case nameof(StringModel.InputValue2):
                                {
                                    #region inputValue2字段修改
                                    //先对值得合法性检查
                                    if (e.Value == null || e.Value.GetType() != typeof(string))
                                    {
                                        e.Valid = false;
                                        e.ErrorText = this.programModel.String.CanNotEmpty;
                                        return;
                                    }
                                    string inputValue2 = e.Value as string;
                                    //如果是之前的值不变就好
                                    if (inputValue2 == data.InputValue2) return;
                                    inputValue2 = inputValue2.Trim();
                                    if (string.IsNullOrEmpty(inputValue2))
                                    {
                                        e.Valid = false;
                                        e.ErrorText = this.programModel.String.CanNotEmpty;
                                        return;
                                    }

                                    if (!General.ChangeProbability(data, null, inputValue2, null))
                                    {
                                        e.Valid = false;
                                        e.ErrorText = this.programModel.String.PositiveNumber;
                                        return;
                                    }

                                    //属性成功修改
                                    //bBehaveApply = true;

                                    #endregion
                                    break;
                                }
                            case nameof(StringModel.InputType):
                                {
                                    #region inputValue2字段修改
                                    //先对值得合法性检查
                                    if (e.Value == null || e.Value.GetType() != typeof(string))
                                    {
                                        e.Valid = false;
                                        e.ErrorText = this.programModel.String.CanNotEmpty;
                                        return;
                                    }
                                    string inputType = e.Value as string;
                                    //如果是之前的值不变就好
                                    if (inputType == data.InputType) return;
                                    inputType = inputType.Trim();
                                    if (string.IsNullOrEmpty(inputType))
                                    {
                                        e.Valid = false;
                                        e.ErrorText = this.programModel.String.CanNotEmpty;
                                        return;
                                    }

                                    //属性成功修改
                                    bBehaveApply = true;

                                    #endregion
                                    break;
                                }
                            case FixedString.COLUMNAME_EXTRAVALUE1:
                            case FixedString.COLUMNAME_EXTRAVALUE2:
                            case FixedString.COLUMNAME_EXTRAVALUE3:
                            case FixedString.COLUMNAME_EXTRAVALUE4:
                            case FixedString.COLUMNAME_EXTRAVALUE5:
                            case FixedString.COLUMNAME_EXTRAVALUE6:
                            case FixedString.COLUMNAME_EXTRAVALUE7:
                            case FixedString.COLUMNAME_EXTRAVALUE8:
                            case FixedString.COLUMNAME_EXTRAVALUE9:
                            case FixedString.COLUMNAME_EXTRAVALUE10:
                                {
                                    General.InvokeHandler
                                    (
                                       GlobalEvent.CommonCellValueChanged,
                                       new Tuple<DrawData, string, BaseContainerValidateEditorEventArgs>(data, General.TableControl.FocusedColumn.Name, e)
                                    );

                                    //属性成功修改
                                    bBehaveApply = true;

                                    break;
                                }
                            default: break;
                        }

                        //(输入框)对象属性改变后
                        if ((strColumnName == nameof(StringModel.Identifier))
                            || (strColumnName == nameof(StringModel.Comment1))
                            || (strColumnName == nameof(StringModel.InputValue))
                            || (strColumnName == nameof(StringModel.InputValue2))
                            || (strColumnName == nameof(StringModel.InputType))
                            || (strColumnName == (FixedString.COLUMNAME_EXTRAVALUE1))
                            || (strColumnName == (FixedString.COLUMNAME_EXTRAVALUE2))
                            || (strColumnName == (FixedString.COLUMNAME_EXTRAVALUE3))
                            || (strColumnName == (FixedString.COLUMNAME_EXTRAVALUE4))
                            || (strColumnName == (FixedString.COLUMNAME_EXTRAVALUE5))
                            || (strColumnName == (FixedString.COLUMNAME_EXTRAVALUE6))
                            || (strColumnName == (FixedString.COLUMNAME_EXTRAVALUE7))
                            || (strColumnName == (FixedString.COLUMNAME_EXTRAVALUE8))
                            || (strColumnName == (FixedString.COLUMNAME_EXTRAVALUE9))
                            || (strColumnName == (FixedString.COLUMNAME_EXTRAVALUE10)))
                        {
                            if (bBehaveApply
                            && (null != this.programModel)
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
                }
            });
        }

        /// <summary>
        /// 获得下拉框内的候选资源
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="drawData"></param>
        /// <returns></returns>
        private RepositoryItemComboBox GetComboBoxItemSource(string columnName, DrawData drawData)
        {
            var result = new RepositoryItemComboBox();
            result.TextEditStyle = TextEditStyles.DisableTextEditor;
            result.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            switch (columnName)
            {
                case nameof(StringModel.Type): { result.Items.AddRange(drawData.GetAvailableTypeSource(this.programModel.CurrentSystem, this.programModel.String)); break; }
                case nameof(StringModel.LogicalCondition): { result.Items.AddRange(DrawData.GetAvailableLogicalConditionSource(this.programModel.String)); break; }
                case nameof(StringModel.InputType): { result.Items.AddRange(DrawData.GetAvailableInputTypeSource(this.programModel.String)); break; }
                case nameof(StringModel.FRType): { result.Items.AddRange(DrawData.GetAvailableFailureRateTypeSource(this.programModel.String)); break; }
                case nameof(StringModel.Units): { result.Items.AddRange(DrawData.GetAvailableUnitSource(this.programModel.String)); break; }
                default: break;
            }
            return result;
        }

        /// <summary>
        /// 自定义单击单元格时弹出的编辑器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomNodeCellEdit(object sender, GetCustomNodeCellEditEventArgs e)
        {
            try
            {
                if (e.Node != null && e.Column != null)
                {
                    object obj = e.Node.GetValue(FixedString.COLUMNAME_DATA);
                    if (obj != null && obj.GetType() == typeof(DrawData))
                    {
                        DrawData data = obj as DrawData;
                        if (FTATable_CellEvent_IsEnableCellEdit(data, e.Column.Name))
                        {
                            var names = new List<string> { nameof(StringModel.Type), nameof(StringModel.LogicalCondition), nameof(StringModel.InputType), nameof(StringModel.FRType), nameof(StringModel.Units) };
                            var index = names.IndexOf(e.Column.Name);
                            if (index >= 0 && e.RepositoryItem != General.TableControl.RepositoryItems[index])
                            {
                                var aaa = this.GetComboBoxItemSource(e.Column.Name, data);
                                e.RepositoryItem = aaa;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var aa = ex.Message;
            }
        }

        /// <summary>
        /// FTA表用于判断某个单元格是否可编辑
        /// </summary>
        /// <param name="data">当前数据对象</param>
        /// <param name="ColumName">当前标题名（列名）</param>
        /// <returns>是否可编辑该单元格</returns>
        private bool FTATable_CellEvent_IsEnableCellEdit(DrawData data, string ColumName)
        {
            return General.TryCatch(() =>
            {
                bool Is_Enable = false;
                //转移门副本都不能改
                if (data != null && !string.IsNullOrEmpty(ColumName) && data.Type != DrawType.TransferInGate)
                {
                    switch (ColumName)
                    {
                        case nameof(StringModel.Identifier):
                        case nameof(StringModel.Comment1):
                        case nameof(StringModel.LogicalCondition)://FixedString.COLUMNAME_LOGICALCONDITION:
                        case FixedString.COLUMNAME_EXTRAVALUE1:
                        case FixedString.COLUMNAME_EXTRAVALUE2:
                        case FixedString.COLUMNAME_EXTRAVALUE3:
                        case FixedString.COLUMNAME_EXTRAVALUE4:
                        case FixedString.COLUMNAME_EXTRAVALUE5:
                        case FixedString.COLUMNAME_EXTRAVALUE6:
                        case FixedString.COLUMNAME_EXTRAVALUE7:
                        case FixedString.COLUMNAME_EXTRAVALUE8:
                        case FixedString.COLUMNAME_EXTRAVALUE9:
                        case FixedString.COLUMNAME_EXTRAVALUE10:
                            {
                                Is_Enable = true;
                                break;
                            }

                        //条件事件不能修改类型
                        case nameof(StringModel.Type):
                            {
                                if (data.Type != DrawType.ConditionEvent) Is_Enable = true;
                                break;
                            }
                        case nameof(StringModel.InputType):
                            if (!data.IsGateType)
                            {
                                Is_Enable = true;
                            }
                            break;
                        case nameof(StringModel.InputValue):
                            if (!data.IsGateType)
                            {
                                Is_Enable = true;
                            }
                            break;
                        ////事件并且normal
                        //if (!data.IsGateType && data.LogicalCondition == FixedString.LOGICAL_CONDITION_NORMAL)
                        //{
                        //    Is_Enable = true;
                        //}
                        //break;
                        case nameof(StringModel.FRType):
                            //if (data.Model == FixedString.MODEL_FR_MTBF || data.Model == FixedString.MODEL_FAILURE_WITH_REPAIR
                            //    || data.Model == FixedString.MODEL_FAILURE_WITH_PERIODIC_INSPECTION)
                            //{
                            //    Is_Enable = true;
                            //}
                            break;
                        case nameof(StringModel.InputValue2):
                            #region 暂时
                            if (!data.IsGateType && data.InputType != this.programModel.String.ConstantProbability)
                            {
                                Is_Enable = true;
                            }
                            break;
                        #endregion
                        case nameof(StringModel.Units):
                            //if (data.LogicalCondition == FixedString.LOGICAL_CONDITION_NORMAL &&
                            //    (data.Model == FixedString.MODEL_FAILURE_WITH_REPAIR ||
                            //    data.Model == FixedString.MODEL_FAILURE_WITH_PERIODIC_INSPECTION))
                            //{
                            //    Is_Enable = true;
                            //}
                            #region 暂时
                            if (!data.IsGateType)
                            {
                                Is_Enable = true;
                            }
                            #endregion
                            break;
                        default:
                            break;
                    }
                }
                return Is_Enable;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isEnabled"></param>
        private void SetEnable(bool isEnabled)
        {
            General.RibbonControl.Enabled = isEnabled;
            General.ProjectControl.Enabled = isEnabled;
        }

        /// <summary>
        /// 在显示自定义窗体之前,注册textbox事件等操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowCustomizationForm(object sender, EventArgs e)
        {
            General.TryCatch(() =>
            {
                this.SetEnable(false);

                General.TableControl.CustomizationForm.Height = 260;
                General.TableControl.CustomizationForm.ActiveListBox.MouseUp -= ActiveListBox_MouseUp;
                General.TableControl.CustomizationForm.ActiveListBox.MouseUp += ActiveListBox_MouseUp;
            });
        }

        /// <summary>
        /// 单击标题列项时，显示文本编辑框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ActiveListBox_MouseUp(object sender, MouseEventArgs e)
        {
            General.TryCatch(() =>
            {
                if (e.Button == MouseButtons.Left && e.Clicks == 1)
                {
                    for (int i = 0; i < General.TableControl.CustomizationForm.ActiveListBox.ItemCount; i++)
                    {
                        var rect = General.TableControl.CustomizationForm.ActiveListBox.GetItemRectangle(i);
                        if (rect.Contains(e.Location))
                        {
                            var a = (TreeListColumn)General.TableControl.CustomizationForm.ActiveListBox.GetItem(i);
                            switch (a.Name)
                            {
                                case FixedString.COLUMNAME_EXTRAVALUE1:
                                case FixedString.COLUMNAME_EXTRAVALUE2:
                                case FixedString.COLUMNAME_EXTRAVALUE3:
                                case FixedString.COLUMNAME_EXTRAVALUE4:
                                case FixedString.COLUMNAME_EXTRAVALUE5:
                                case FixedString.COLUMNAME_EXTRAVALUE6:
                                case FixedString.COLUMNAME_EXTRAVALUE7:
                                case FixedString.COLUMNAME_EXTRAVALUE8:
                                case FixedString.COLUMNAME_EXTRAVALUE9:
                                case FixedString.COLUMNAME_EXTRAVALUE10:
                                    TextEdit textEdit = new TextEdit();
                                    textEdit.Location = rect.Location;
                                    textEdit.Text = a.Caption;
                                    textEdit.Width = rect.Width;
                                    textEdit.Tag = a;
                                    textEdit.EnterMoveNextControl = true;
                                    textEdit.Validating += TextEditValidating;
                                    General.TableControl.CustomizationForm.ActiveListBox.Controls.Add(textEdit);
                                    textEdit.Focus();
                                    textEdit.SelectionLength = textEdit.Text.Length;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 编辑标题列名时验证字符串
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextEditValidating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            General.TryCatch(() =>
            {
                if (sender != null && sender.GetType() == typeof(TextEdit))
                {
                    TextEdit textEdit = sender as TextEdit;

                    string text = textEdit.Text;
                    if (text != null && textEdit.Tag != null && textEdit.Tag.GetType() == typeof(TreeListColumn))
                    {
                        text = text.Trim();
                        TreeListColumn column = textEdit.Tag as TreeListColumn;
                        if (!string.IsNullOrEmpty(text)) column.Caption = text;
                    }
                    textEdit.Validating -= TextEditValidating;
                    textEdit.Parent.Controls.Remove(textEdit);
                    textEdit.Dispose();
                }
            });
        }

        /// <summary>
        /// 标题列可视位置变化时，记录该位置设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColumnPositionChanged(object sender, EventArgs e)
        {
            var tableColumn = sender as TreeListColumn;
            this.programModel.CurrentProject.ColumnFieldInfos.FirstOrDefault(o => o.Name == tableColumn.Name).Index = tableColumn.VisibleIndex;
        }

        /// <summary>
        /// 自定义列小窗体隐藏事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HideCustomizationForm(object sender, EventArgs e)
        {
            this.programModel.CurrentProject?.EditColumnFieldInfos(General.TableControl.Columns);
            this.SetEnable(true);
        }

        /// <summary>
        /// 右键treeList时显示的过滤菜单，这里翻译他的菜单项，和添加自定义的菜单项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            General.TryCatch(() =>
            {
                if (e.Menu != null && e.Menu.MenuType == TreeListMenuType.Column)
                {
                    //手动翻译菜单项
                    foreach (DXMenuItem item in e.Menu.Items)
                    {
                        switch (item.Caption)
                        {
                            case "Sort Ascending":
                                item.Caption = this.programModel.String.SortAscending;
                                break;
                            case "Sort Descending":
                                item.Caption = this.programModel.String.SortDescending;
                                break;
                            case "Column Chooser":
                                item.Caption = this.programModel.String.ColumnChooser;
                                break;
                            case "Best Fit":
                                item.Caption = this.programModel.String.BestFitCurrentColumn;
                                break;
                            case "Best Fit (all columns)":
                                item.Caption = this.programModel.String.BestFitallcolumns;
                                break;
                            case "Filter Editor...":
                                item.Caption = this.programModel.String.FilterEditor;
                                break;
                            case "Show Find Panel":
                                item.Caption = this.programModel.String.ShowFindPanel;
                                break;
                            case "Show Auto Filter Row":
                                item.Caption = this.programModel.String.ShowAutoFilterRow;
                                break;
                            case "Clear Sorting":
                                item.Caption = this.programModel.String.ClearSorting;
                                break;
                            case "Clear All Sorting":
                                item.Caption = this.programModel.String.ClearAllSorting;
                                break;
                            case "Hide Find Panel":
                                item.Caption = this.programModel.String.HideFindPanel;
                                break;
                            case "Hide Auto Filter Row":
                                item.Caption = this.programModel.String.HideAutoFilterRow;
                                break;
                            case "Clear Filter":
                                item.Caption = this.programModel.String.Hidefiltereditor;
                                break;
                            default:
                                break;
                        }
                    }

                    //添加菜单项
                    DXMenuItem item_FilterSeeing = new DXMenuItem();
                    item_FilterSeeing.Caption = this.programModel.String.Filtervisibledatamodedefault;
                    if (VirtualDrawData.is_FilterAllMode) item_FilterSeeing.Enabled = true;
                    else item_FilterSeeing.Enabled = false;
                    e.Menu.Items.Add(item_FilterSeeing);

                    //添加菜单项
                    DXMenuItem item_FilterAll = new DXMenuItem();
                    item_FilterAll.Caption = this.programModel.String.Filteralldatamode;
                    if (!VirtualDrawData.is_FilterAllMode) item_FilterAll.Enabled = true;
                    else item_FilterAll.Enabled = false;
                    e.Menu.Items.Add(item_FilterAll);

                    //设置菜单项单击事件
                    e.Menu.ItemClick += ItemClick;
                }
            });
        }

        /// <summary>
        /// 切换过滤模式，全部过滤或可见节点过滤
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemClick(object sender, DXMenuItemEventArgs e)
        {
            General.TryCatch(() =>
            {
                if (e.Item != null && !string.IsNullOrEmpty(e.Item.Caption))
                {
                    if (e.Item.Caption == this.programModel.String.Filtervisibledatamodedefault && VirtualDrawData.is_FilterAllMode)
                    {
                        VirtualDrawData.is_FilterAllMode = false;
                        General.TableControl.RefreshDataSource();
                    }
                    else if (e.Item.Caption == this.programModel.String.Filteralldatamode && !VirtualDrawData.is_FilterAllMode)
                    {
                        VirtualDrawData.is_FilterAllMode = true;
                        General.TableControl.RefreshDataSource();
                    }
                }
            });
        }

        /// <summary>
        /// 自定义选中的单元格能否编辑值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowingEditor(object sender, System.ComponentModel.CancelEventArgs e)
        {
            General.TryCatch(() =>
            {
                if (this.tableControl.FocusedNode != null && this.tableControl.FocusedColumn != null)
                {
                    object obj = this.tableControl.FocusedNode.GetValue(FixedString.COLUMNAME_DATA);
                    if (obj != null && obj.GetType() == typeof(DrawData))
                    {
                        DrawData data = obj as DrawData;
                        e.Cancel = !FTATable_CellEvent_IsEnableCellEdit(data, this.tableControl.FocusedColumn.Name);
                        //type列显示时动态添加数据
                        if (!e.Cancel && this.tableControl.FocusedColumn.Name == nameof(StringModel.Type))
                        {
                            RepositoryItemComboBox comb_Type = this.tableControl.RepositoryItems[0] as RepositoryItemComboBox;
                            comb_Type.Items.Clear();
                            //初始化type列的combo可选值
                            List<string> comb_Data = GetAvailableTypeSource(data);
                            if (comb_Data != null && comb_Data.Count > 0) comb_Type.Items.AddRange(comb_Data);
                        }
                        return;
                    }
                }
                e.Cancel = true;
            });
        }

        /// <summary>
        /// 返回FTA表某条数据的的Type列comboBox里的可选数据
        /// </summary>
        /// <param name="data">当前数据对象</param>
        /// <returns>可选图形类型的列表</returns>
        private List<string> GetAvailableTypeSource(DrawData data)
        {
            var result = new List<string>();
            try
            {
                if (this.programModel.CurrentSystem?.TranferGates != null && this.programModel.CurrentSystem?.RepeatedEvents != null)
                {
                    result = data?.GetAvailableTypeSource(this.programModel.CurrentSystem, this.programModel.String);
                }
            }
            catch (Exception ex) { throw new Exception($"{FixedString.EXCEPTION}{ex.Message}"); }
            return result;
        }

        /// <summary>
        /// 刷新下表里的数据显示（在文本型编辑框修改并保存完毕后）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <summary>
        /// 自定义绘制单元格背景前景样式（颜色）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e)
        {
            General.TryCatch(() =>
            {
                bool Is_Disable = false;
                if (e.Node != null)
                {
                    object obj = e.Node.GetValue(FixedString.COLUMNAME_DATA);
                    if (obj != null && obj.GetType() == typeof(DrawData))
                    {
                        DrawData data = obj as DrawData;
                        Is_Disable = !FTATable_CellEvent_IsEnableCellEdit(data, e.Column.Name);
                    }
                }
                if (Is_Disable)
                {
                    e.Appearance.BackColor = System.Drawing.Color.DarkGray;
                    e.Appearance.ForeColor = System.Drawing.Color.Black;
                }
            });
        }

        /// <summary>
        /// 表格单元格变更事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            General.TryCatch(() =>
            {
                if (e.Column != null && e.Node != null)
                {
                    object obj = e.Node.GetValue(FixedString.COLUMNAME_DATA);
                    if (obj.GetType() == typeof(DrawData) && e.Value != null && e.Value.GetType() == typeof(string) && e.Column.Name != "Identifier")
                    {
                        General.InvokeHandler(GlobalEvent.PropertyEdited, new Tuple<string, DrawData, string>(e.Column.Name, obj as DrawData, e.Value.ToString()));

                        //联动刷新恒定概率
                        if (e.Column.Caption == this.programModel.String.InputType && e.Value.ToString() == this.programModel.String.ConstantProbability)
                        {
                            General.InvokeHandler(GlobalEvent.PropertyEdited, new Tuple<string, DrawData, string>("InputValue", obj as DrawData, ((DrawData)obj).QValue));
                            General.InvokeHandler(GlobalEvent.PropertyEdited, new Tuple<string, DrawData, string>("InputValue2", obj as DrawData, "1"));
                        }
                    }
                }

                switch (e.Column.Name)
                {
                    //因为值在验证时已经修改了，所以修改完后这里刷新
                    case nameof(StringModel.Identifier)://FixedString.COLUMNAME_ID:
                    case nameof(StringModel.Comment1):
                    case nameof(StringModel.InputValue):
                    case nameof(StringModel.InputValue2):
                    case nameof(StringModel.InputType):
                    case FixedString.COLUMNAME_EXTRAVALUE1:
                    case FixedString.COLUMNAME_EXTRAVALUE2:
                    case FixedString.COLUMNAME_EXTRAVALUE3:
                    case FixedString.COLUMNAME_EXTRAVALUE4:
                    case FixedString.COLUMNAME_EXTRAVALUE5:
                    case FixedString.COLUMNAME_EXTRAVALUE6:
                    case FixedString.COLUMNAME_EXTRAVALUE7:
                    case FixedString.COLUMNAME_EXTRAVALUE8:
                    case FixedString.COLUMNAME_EXTRAVALUE9:
                    case FixedString.COLUMNAME_EXTRAVALUE10: { General.InvokeHandler(GlobalEvent.UpdateData, true); break; }
                    default: break;
                }

                //var bb = (sender as TreeList).Selection[0].GetValue(FixedString.COLUMNAME_DATA);
                //General.InvokeHandler(GlobalEvent.TableFocused, bb);
            });
        }

        /// <summary>
        /// 当选中节点变化时，使选中的图形也变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {
            General.TryCatch(() =>
            {
                if (!General.IsIgnoreTreeListFocusNodeChangeEvent && e.Node != null)
                {
                    var newNode = e.Node.GetValue(FixedString.COLUMNAME_DATA);
                    if (newNode?.GetType() == typeof(DrawData))
                    {
                        General.InvokeHandler(GlobalEvent.FTADiagram_MakeVisable, newNode);
                    }
                }
            });
        }

        /// <summary>
        /// 变更新的ID后需要校验多种不同情况
        /// 比如加入和退出重复事件以及转入门的情况
        /// </summary>
        /// <param name="data"></param>
        /// <param name="newID"></param>
        /// <returns></returns>
        public int ModifyIdentfiy(DrawData data, string newID)
        {
            var result = 0;

            // 新值和原值有差异
            if (newID != data.Identifier)
            {
                newID = newID.Trim();
                // 新值为空
                if (string.IsNullOrEmpty(newID)) result = 2;
                else
                {
                    //初始化准备
                    bool Is_ChangeId = true;
                    List<DrawData> allDatas = this.programModel.CurrentSystem.GetAllDatas();
                    if (allDatas != null)
                    {
                        List<string> ids = allDatas.Select(ss => ss.Identifier).ToList();
                        if (ids != null)
                        {
                            // 变更后的ID和现有的ID有重复
                            if (ids.Contains(newID))
                            {
                                Is_ChangeId = false;
                                // 重复的是转移门id
                                if (this.programModel.CurrentSystem.TranferGates.ContainsKey(newID))
                                {
                                    HashSet<DrawData> newTrans = this.programModel.CurrentSystem.TranferGates[newID];
                                    DrawData newTran_True = newTrans.Where(ss => ss.Type != DrawType.TransferInGate).FirstOrDefault();
                                    DrawData newTran_Copied = newTrans.Where(ss => ss.Type == DrawType.TransferInGate).FirstOrDefault();
                                    //要做转移门副本，没有儿子，要有父亲
                                    if
                                    (
                                       newTrans != null
                                       && newTran_True != null
                                       && newTran_Copied != null
                                       && data.HasChild == false
                                       && data.Type != DrawType.ConditionEvent
                                       && !this.programModel.CurrentSystem.Roots.Contains(data)
                                       && data.Type != DrawType.TransferInGate
                                    )
                                    {
                                        // 转移门本体
                                        if (this.programModel.CurrentSystem.TranferGates.ContainsKey(data.Identifier))
                                        {
                                            HashSet<DrawData> trans = this.programModel.CurrentSystem.TranferGates[data.Identifier];
                                            //检查是否是目标本体的儿子，检查如果把目标副本复制给给父节点，各父节点能否接受
                                            bool Is_Ok = true;
                                            if (trans != null)
                                            {
                                                foreach (DrawData tmp in trans)
                                                {
                                                    if (tmp.IsChildOfParent(newTran_True) || !this.CanPasteTransGate(newTran_Copied, tmp.Parent))
                                                    {
                                                        Is_Ok = false;
                                                        break;
                                                    }
                                                }
                                            }

                                            if (trans != null && Is_Ok)
                                            {
                                                if (MsgBox.Show(this.programModel.String.ConvertToCopy, FixedString.FAULT_TREE_ANALYSIS, MessageBoxButtons.OKCancel) == DialogResult.OK)
                                                {
                                                    //集合维护
                                                    this.programModel.CurrentSystem.RemoveTranferGate(data);
                                                    foreach (DrawData tmp in trans)
                                                    {
                                                        tmp.CopyIntoTransferOrRepeatedEvent(newTran_Copied);
                                                    }
                                                    foreach (DrawData item in trans)
                                                    {
                                                        newTrans.Add(item);
                                                    }
                                                    Is_ChangeId = true;
                                                    result = 0;
                                                }
                                                else
                                                {
                                                    Is_ChangeId = true;
                                                    result = 1;
                                                }
                                            }
                                        }

                                        // 重复事件，正常事件，门
                                        else
                                        {
                                            if (!data.IsChildOfParent(newTran_True) && this.CanPasteTransGate(newTran_Copied, data.Parent))
                                            {
                                                if (MsgBox.Show(this.programModel.String.ConvertToAnotherCopy,
                                                FixedString.FAULT_TREE_ANALYSIS, MessageBoxButtons.OKCancel) == DialogResult.OK)
                                                {
                                                    //重复事件
                                                    if (this.programModel.CurrentSystem.RepeatedEvents.ContainsKey(data.Identifier))
                                                    {
                                                        this.programModel.CurrentSystem.RemoveRepeatedEvent(data);
                                                    }
                                                    data.CopyIntoTransferOrRepeatedEvent(newTran_Copied);
                                                    newTrans.Add(data);
                                                    Is_ChangeId = true;
                                                    result = 0;
                                                }
                                                else
                                                {
                                                    Is_ChangeId = true;
                                                    result = 1;
                                                }
                                            }
                                        }
                                    }
                                }
                                // 重复的是重复事件id
                                else if (this.programModel.CurrentSystem.RepeatedEvents.ContainsKey(newID))
                                {
                                    HashSet<DrawData> newRepeats = this.programModel.CurrentSystem.RepeatedEvents[newID];
                                    //要做重复事件，没有儿子，要有父亲
                                    if (newRepeats != null
                                       && data.HasChild == false
                                       && data.Type != DrawType.ConditionEvent
                                       && !this.programModel.CurrentSystem.Roots.Contains(data)
                                       && data.Type != DrawType.TransferInGate)
                                    {
                                        // 转移门本体
                                        if (this.programModel.CurrentSystem.TranferGates.ContainsKey(data.Identifier))
                                        {
                                            HashSet<DrawData> trans = this.programModel.CurrentSystem.TranferGates[data.Identifier];
                                            if (trans != null)
                                            {
                                                if (MsgBox.Show(this.programModel.String.ConvertToRepeatedEvents, FixedString.FAULT_TREE_ANALYSIS, MessageBoxButtons.OKCancel) == DialogResult.OK)
                                                {
                                                    //集合维护
                                                    this.programModel.CurrentSystem.RemoveTranferGate(data);
                                                    foreach (DrawData tmp in trans) tmp.CopyIntoTransferOrRepeatedEvent(newRepeats.First());
                                                    foreach (var item in trans)
                                                    {
                                                        newRepeats.Add(item);
                                                    }
                                                    foreach (DrawData tmp in newRepeats) tmp.Repeats = newRepeats.Count - 1;
                                                    result = 0;
                                                }
                                                else result = 1;

                                            }
                                        }

                                        // 重复事件，正常事件，门
                                        else
                                        {
                                            if (MsgBox.Show(this.programModel.String.ConvertToAnotherRepeatedEvents,
                                                 FixedString.FAULT_TREE_ANALYSIS, MessageBoxButtons.OKCancel) == DialogResult.OK)
                                            {
                                                this.IsJoinRepeatEvent = true;
                                                //重复事件
                                                if (this.programModel.CurrentSystem.RepeatedEvents.ContainsKey(data.Identifier))
                                                {
                                                    this.programModel.CurrentSystem.RemoveRepeatedEvent(data);
                                                }
                                                data.CopyIntoTransferOrRepeatedEvent(newRepeats.First());
                                                this.programModel.CurrentSystem.AddRepeatedEvent(data);
                                                Is_ChangeId = true;
                                                result = 0;
                                            }
                                            else
                                            {
                                                Is_ChangeId = true;
                                                result = 1;
                                            }
                                        }
                                    }
                                }
                                // 其他
                                else
                                {
                                    // 重复的是非重复事件的ID
                                    DrawData repeat = allDatas.Where(ss => ss.Identifier == newID && (ss.Type == DrawType.BasicEvent || ss.Type == DrawType.HouseEvent || ss.Type == DrawType.UndevelopedEvent)).FirstOrDefault();
                                    if (repeat != null && !this.programModel.CurrentSystem.RepeatedEvents.ContainsKey(repeat.Identifier))
                                    {

                                        //要做重复事件，没有儿子，要有父亲
                                        if
                                        (
                                           data.HasChild == false
                                           && data.Type != DrawType.ConditionEvent
                                           && !this.programModel.CurrentSystem.Roots.Contains(data)
                                           && data.Type != DrawType.TransferInGate
                                        )
                                        {
                                            // 转移门本体
                                            if (this.programModel.CurrentSystem.TranferGates.ContainsKey(data.Identifier))
                                            {
                                                HashSet<DrawData> trans = this.programModel.CurrentSystem.TranferGates[data.Identifier];
                                                if (trans != null)
                                                {
                                                    if (MsgBox.Show(this.programModel.String.ConvertToRepeatedEvents, FixedString.FAULT_TREE_ANALYSIS, MessageBoxButtons.OKCancel) == DialogResult.OK)
                                                    {
                                                        //集合维护
                                                        this.programModel.CurrentSystem.RemoveTranferGate(data);
                                                        foreach (DrawData tmp in trans)
                                                        {
                                                            tmp.CopyIntoTransferOrRepeatedEvent(repeat);
                                                        }
                                                        HashSet<DrawData> newRepeats = new HashSet<DrawData>() { repeat };
                                                        this.programModel.CurrentSystem.RepeatedEvents.Add(repeat.Identifier, newRepeats);
                                                        foreach (var item in trans)
                                                        {
                                                            newRepeats.Add(item);
                                                        }

                                                        foreach (DrawData tmp in newRepeats)
                                                        {
                                                            tmp.Repeats = newRepeats.Count - 1;
                                                        }
                                                        result = 0;
                                                    }
                                                    else result = 1;

                                                }
                                            }

                                            // 重复事件，正常事件，门
                                            else
                                            {
                                                if (MsgBox.Show(this.programModel.String.ConvertToAnotherRepeatedEvents, FixedString.FAULT_TREE_ANALYSIS, MessageBoxButtons.OKCancel) == DialogResult.OK)
                                                {

                                                    //重复事件
                                                    if (this.programModel.CurrentSystem.RepeatedEvents.ContainsKey(data.Identifier))
                                                    {
                                                        this.programModel.CurrentSystem.RemoveRepeatedEvent(data);
                                                    }
                                                    data.CopyIntoTransferOrRepeatedEvent(repeat);
                                                    HashSet<DrawData> newRepeats = new HashSet<DrawData>() { repeat, data };
                                                    this.programModel.CurrentSystem.RepeatedEvents.Add(repeat.Identifier, newRepeats);
                                                    foreach (DrawData tmp in newRepeats) tmp.Repeats = newRepeats.Count - 1;
                                                    Is_ChangeId = true;
                                                    result = 0;
                                                }
                                                else
                                                {
                                                    Is_ChangeId = true;
                                                    result = 1;
                                                }
                                            }
                                        }
                                    }
                                }
                                if (!Is_ChangeId) result = 2;
                            }
                            else
                            {
                                //正常不重复id时
                                //转移门本体
                                if (this.programModel.CurrentSystem.TranferGates.ContainsKey(data.Identifier))
                                {
                                    string id_Before = data.Identifier;
                                    HashSet<DrawData> trans = this.programModel.CurrentSystem.TranferGates[data.Identifier];
                                    foreach (DrawData tmp in trans)
                                    {
                                        tmp.Identifier = newID;
                                    }
                                    this.programModel.CurrentSystem.TranferGates.Remove(id_Before);
                                    this.programModel.CurrentSystem.TranferGates.Add(newID, trans);
                                }
                                //重复事件
                                else if (this.programModel.CurrentSystem.RepeatedEvents.ContainsKey(data.Identifier))
                                {
                                    var messageResult = MsgBox.Show(General.FtaProgram.String.IfOnlyEvent, FixedString.FAULT_TREE_ANALYSIS, MessageBoxButtons.YesNoCancel);
                                    if (messageResult == DialogResult.Yes)
                                    {
                                        data.Identifier = newID;
                                        this.programModel.CurrentSystem.UpdateRepeatedAndTranfer();
                                    }
                                    else if (messageResult == DialogResult.No)
                                    {
                                        string id_Before = data.Identifier;
                                        HashSet<DrawData> repeats = this.programModel.CurrentSystem.RepeatedEvents[data.Identifier];
                                        foreach (DrawData tmp in repeats)
                                        {
                                            tmp.Identifier = newID;
                                        }
                                        this.programModel.CurrentSystem.RepeatedEvents.Remove(id_Before);
                                        this.programModel.CurrentSystem.RepeatedEvents.Add(newID, repeats);
                                    }
                                    else
                                    {
                                        result = 1;
                                    }
                                }
                                else data.Identifier = newID;
                            }
                        }
                    }
                    this.programModel.CurrentSystem.UpdateRepeatedAndTranfer();
                }
            }
            return result;
        }

        /// <summary>
        /// 每个节点获取静态图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetStateImage_treeList_FTATable(object sender, GetStateImageEventArgs e)
        {
            General.TryCatch(() =>
            {
                object obj = e.Node.GetValue(FixedString.COLUMNAME_DATA);
                if (obj != null && obj.GetType() == typeof(DrawData))
                {
                    DrawData data = obj as DrawData;
                    //如果该节点要复制或剪切/如果递归操作那么是这个的子节点
                    if (General.CopyCutObject != null && (General.CopyCutObject == obj || (General.FTATableDiagram_Is_CopyOrCut_Recurse && data.IsChildOfParent(General.CopyCutObject))))
                    {
                        if (this.IsCopyNode) e.NodeImageIndex = ImageTypes.Count + 1;
                        else e.NodeImageIndex = ImageTypes.Count;
                    }
                    //如果该节点是重复事件
                    else if (data.Repeats >= 1)
                    {
                        if (data.Type == DrawType.BasicEvent) e.NodeImageIndex = ImageTypes.Count + 2;
                        else if (data.Type == DrawType.HouseEvent) e.NodeImageIndex = ImageTypes.Count + 3;
                        else if (data.Type == DrawType.UndevelopedEvent) e.NodeImageIndex = ImageTypes.Count + 4;
                    }
                    else
                    {
                        e.NodeImageIndex = ImageTypes.IndexOf(data.Type);
                    }
                }
            });
        }

        /// <summary>
        /// 自定义绘制FTA表左侧指示器图形
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomDrawNodeIndicator_TreeList_FTATable(object sender, CustomDrawNodeIndicatorEventArgs e)
        {

            if (this.programModel?.CurrentSystem?.Roots != null && e.Node != null && this.programModel?.Setting != null)
            {
                object obj = e.Node.GetValue(FixedString.COLUMNAME_DATA);
                if (obj?.GetType() == typeof(DrawData))
                {
                    float offset = 1.5f;
                    DrawData data = obj as DrawData;
                    //转移门本体
                    if (this.programModel?.CurrentSystem?.TranferGates?.ContainsKey(data.Identifier) == true && data.Type != DrawType.TransferInGate)
                    {
                        //防止默认绘制编辑选中等小图标
                        e.IndicatorArgs.ImageIndex = -1;
                        e.DefaultDraw();
                        e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                        //画出自定义图形
                        SolidBrush b = new SolidBrush(this.programModel.Setting.FTATableIndicatorTransInGateColor);
                        Pen p = new Pen(System.Drawing.Color.Black, 1);
                        DrawBase.DrawFilledBaseComponent(data, DrawType.TransferInGate, e.Graphics, p, b, e.Bounds.X + offset, e.Bounds.Y + offset, e.Bounds.Width - 2 * offset - p.Width, e.Bounds.Height - 2 * offset - p.Width, true);
                        b.Dispose();
                        p.Dispose();
                        e.Handled = true;
                    }
                    //根节点图形
                    else if (this.programModel.CurrentSystem?.Roots.Contains(data) == true)
                    {
                        //防止默认绘制编辑选中等小图标
                        e.IndicatorArgs.ImageIndex = -1;
                        e.DefaultDraw();
                        e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                        //画出自定义图形
                        SolidBrush b = new SolidBrush(this.programModel.Setting.FTATableIndicatorTopGateColor);
                        Pen p = new Pen(System.Drawing.Color.Black, 1);
                        //背景
                        DrawBase.DrawFilledBaseComponent(data, data.Type, e.Graphics, p, b, e.Bounds.X + offset, e.Bounds.Y + offset, e.Bounds.Width - 2 * offset - p.Width, e.Bounds.Height - 2 * offset - p.Width, true);
                        b.Dispose();
                        p.Dispose();
                        e.Handled = true;
                    }
                }
            }

        }

        //TODO:这个函数耗时，需优化
        /// <summary>
        /// 特殊的用于转移门（副本或本体）判断是否能够粘贴的函数
        /// </summary>
        /// <param name="transGate">复制的或剪切的转移门对象（本体或副本）</param>
        /// <param name="target">粘贴给目标</param>
        /// <returns>该转移门对象能否粘贴</returns>
        public bool CanPasteTransGate(DrawData transGate, DrawData target)
        {
            return General.TryCatch(() =>
            {
                //如果是转移门
                if (this.programModel.CurrentSystem.TranferGates != null && this.programModel.CurrentSystem.TranferGates.ContainsKey(transGate.Identifier))
                {
                    HashSet<DrawData> transfer = this.programModel.CurrentSystem.TranferGates[transGate.Identifier];
                    if (transfer != null && transfer.Contains(transGate))
                    {
                        DrawData trans_True = transfer.Where(obj => obj.Type != DrawType.TransferInGate).FirstOrDefault();
                        if (trans_True != null)
                        {
                            //转移门副本不允许粘贴给自己本体
                            if (transGate.Type == DrawType.TransferInGate && (trans_True == target || target.IsChildOfParent(trans_True)))
                            {
                                return false;
                            }
                            //A本体包含b副本，那么b本体不能包含A副本或本体
                            else
                            {
                                //找到b本体,这里集合时因为，data可能是多个不同转移门（转移门间本体包含）儿子
                                List<DrawData> trans_TrueB = new List<DrawData>();
                                foreach (var pair in this.programModel.CurrentSystem.TranferGates)
                                {
                                    if (pair.Key != transGate.Identifier)
                                    {
                                        foreach (DrawData tmp in pair.Value)
                                        {
                                            //找到一个本体
                                            if (tmp.Type != DrawType.TransferInGate)
                                            {
                                                if (tmp == target || target.IsChildOfParent(tmp))
                                                {
                                                    trans_TrueB.Add(tmp);
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                                //检查a本体是否包含b副本
                                if (trans_TrueB.Count > 0)
                                {
                                    foreach (DrawData tmp in trans_TrueB)
                                    {
                                        foreach (DrawData transB in this.programModel.CurrentSystem.TranferGates[tmp.Identifier])
                                        {
                                            if (trans_True == transB || transB.IsChildOfParent(trans_True))
                                            {
                                                return false;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            });
        }

        /// <summary>
        /// 删除节点(包括所有子节点)
        /// </summary>
        /// <param name="selectedNode">操作的节点数据对象</param>
        public List<string> DeleteNodeAndChildren(DrawData selectedNode, bool isIncludeTransfer = false)
        {
            var result = new List<string>();
            try
            {
                if (selectedNode != null)
                {
                    DrawData parent = selectedNode.Parent;
                    List<DrawData> allData = selectedNode.GetAllData(General.FtaProgram.CurrentSystem);
                    List<DrawData> repeat = allData.Where(obj => this.programModel.CurrentSystem.RepeatedEvents.ContainsKey(obj.Identifier)).ToList();
                    List<DrawData> transfer = allData.Where(obj => this.programModel.CurrentSystem.TranferGates.ContainsKey(obj.Identifier)).ToList();

                    if (repeat != null)
                    {
                        foreach (DrawData item in repeat)
                        {
                            General.FtaProgram.CurrentSystem.RemoveRepeatedEvent(item);
                        }
                    }

                    if (transfer != null)
                    {
                        List<DrawData> rootTransfers = new List<DrawData>();
                        foreach (DrawData item in transfer)
                        {
                            HashSet<DrawData> trans = General.FtaProgram.CurrentSystem.TranferGates[item.Identifier];
                            if (item.Type != DrawType.TransferInGate)
                            {
                                //删除其他所有的副本
                                foreach (DrawData tmp in trans)
                                {
                                    if (tmp != item) tmp.Delete();
                                }
                            }
                            var master = trans.FirstOrDefault(o => o.Type != DrawType.TransferInGate);
                            var transCopies = trans.Where(o => o.Type == DrawType.TransferInGate);
                            if (transCopies?.Count() == 1) rootTransfers.Add(master);
                            General.FtaProgram.CurrentSystem.RemoveTranferGate(item);
                        }

                        // 如果转移们没有引用了就删除 
                        if (isIncludeTransfer && rootTransfers.Count > 0)
                        {
                            for (int i = 0; i < rootTransfers.Count; i++)
                            {
                                DeleteNodeAndChildren(rootTransfers[i]);
                                result.Add(rootTransfers[i].Identifier);
                            }
                            General.InvokeHandler(GlobalEvent.UpdateData, true);
                            General.InvokeHandler(GlobalEvent.UpdateLayout);
                        }
                    }

                    //节点的删除
                    selectedNode.Delete();

                    //如果是顶层节点
                    if (General.FtaProgram.CurrentSystem.Roots.Contains(selectedNode))
                    {
                        //从父集合里移除
                        if (this.tableControl.DataSource?.GetType() == typeof(VirtualDrawData))
                        {
                            VirtualDrawData vData = this.tableControl.DataSource as VirtualDrawData;
                            vData.data?.Children?.Remove(selectedNode);
                        }
                        //从全局的数据里删除
                        General.FtaProgram.CurrentSystem.Roots.Remove(selectedNode);
                        if (General.FtaProgram.CurrentRoot == selectedNode) General.FtaProgram.CurrentRoot = null;
                    }

                    //节点图形结构变化
                    General.InvokeHandler(GlobalEvent.UpdateLayout);
                    if (parent != null)
                    {
                        General.InvokeHandler(GlobalEvent.FTADiagram_MakeVisable, parent);
                        General.InvokeHandler(GlobalEvent.TableFocused, parent);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 清空无应用的转移门
        /// </summary>
        public void DeleteAllTransNull()
        {
            try
            {
                List<DrawData> dt = this.programModel.CurrentSystem.Roots.ToList();

                foreach (DrawData tran in dt)
                {
                    if (tran.Parent == null && tran != this.programModel.CurrentRoot)
                    {
                        DeleteNodeAndChildren(tran);
                        General.InvokeHandler(GlobalEvent.UpdateData, true);
                        General.InvokeHandler(GlobalEvent.UpdateLayout);
                    }
                }
                //节点图形结构变化
                General.InvokeHandler(GlobalEvent.UpdateLayout);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public List<string> CalculateDeletedTransfers(DrawData selectedNode)
        {
            var result = new List<string>();

            return result;
        }

        /// <summary>
        /// 删除选中的节点（不递归子节点）
        /// </summary>
        /// <param name="selectedNode"></param>
        public void DeleteNode(DrawData selectedNode)
        {
            General.TryCatch(() =>
            {
                if (selectedNode != null)
                {
                    var parent = selectedNode.Parent;
                    var children = selectedNode.Children;
                    if (parent != null)
                    {
                        if (children != null)
                        {
                            parent.Children.AddRange(children);
                            parent.Children.Remove(selectedNode);
                            children.ForEach(o => o.Parent = parent);
                        }
                        else parent.Children = null;
                    }
                    else
                    {
                        if (children != null)
                        {
                            children.ForEach(o => o.Parent = null);
                            General.FtaProgram.CurrentSystem.Roots.Remove(selectedNode);
                            General.FtaProgram.CurrentSystem.Roots.AddRange(children);
                        }
                    }
                    General.FtaProgram.CurrentSystem.UpdateRepeatedAndTranfer();
                    this.LoadDataToTableControl(General.FtaProgram.CurrentSystem);
                    General.InvokeHandler(GlobalEvent.UpdateLayout);

                    General.InvokeHandler(GlobalEvent.TableFocused, parent);

                    if (parent != null) General.InvokeHandler(GlobalEvent.FTADiagram_MakeVisable, parent);//this.ftaDiagram.FocusOn(parent, true);
                }
            });
        }

        /// <summary>
        /// 根据给出的系统对象，把内容显示到treelist里
        /// </summary>
        /// <param name="system">要显示的系统对象</param>
        public void LoadDataToTableControl(SystemModel system)
        {
            try
            {
                ClearTableControl();
                if (system?.Roots?.Count > 0)
                {
                    //这里由于不显示第一个节点，我们只能先构造一个父亲。
                    DrawData parent = new DrawData();
                    parent.Children = new List<DrawData>();
                    parent.Children.AddRange(system.Roots);
                    this.tableControl.DataSource = new VirtualDrawData(parent, parent, General.FtaProgram);
                    General.FtaProgram.SetCurrentSystem(system);
                    //宽度自适应
                    //this.tableControl.BestFitColumns(false);
                }
            }
            catch
            {
                //SplashScreenManager.CloseDefaultWaitForm();
            }
        }

        /// <summary>
        /// 重置FTA表为初始状态
        /// </summary>
        public void ClearTableControl()
        {
            General.TryCatch(() =>
            {
                this.tableControl.DataSource = null;
                this.tableControl.Nodes.Clear();
                General.FtaProgram.SetCurrentSystem(null);
                //General.FTATableDiagram_DrawData_CopyOrCut = null;
                //this.IsCopyNode = false;
                //General.FTATableDiagram_Is_CopyOrCut_Recurse = false;
                General.IsIgnoreTreeListFocusNodeChangeEvent = false;
            });
        }

        ///// <summary>
        ///// 项目列表拖放事件，设置此时操作的显示样式
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void TreeList_FTATable_DragOver(object sender, DragEventArgs e)
        //{
        //   TreeListNode dragNode = e.Data.GetData(typeof(TreeListNode)) as TreeListNode;
        //   TreeListNode targetNode = this.tableControl.CalcHitInfo(this.tableControl.PointToClient(Control.MousePosition)).Node;
        //   if (dragNode != null && targetNode != null)
        //   {
        //      object obj_Drag = dragNode.GetValue(FixedString.COLUMNAME_DATA);
        //      object obj_Target = targetNode.GetValue(FixedString.COLUMNAME_DATA);
        //      if (obj_Drag != null && obj_Target != null && obj_Drag.GetType() == typeof(DrawData) && obj_Target.GetType() == typeof(DrawData))
        //      {
        //         DrawData data_Drag = obj_Drag as DrawData;
        //         DrawData data_Target = obj_Target as DrawData;
        //         //复制模式，且是要复制的节点
        //         if (General.FTATableDiagram_DrawData_CopyOrCut != null && General.FTATableDiagram_Is_CopyNode
        //             && General.FTATableDiagram_DrawData_CopyOrCut == data_Drag)
        //         {
        //            if (GetBarItemIsEnabled(barButtonItem_Paste, data_Target))
        //               e.Effect = DragDropEffects.Copy;
        //            else e.Effect = DragDropEffects.None;
        //            return;
        //         }
        //         if (GetBarItemIsEnabled(barButtonItem_Paste, data_Target))
        //            e.Effect = DragDropEffects.Move;
        //         return;
        //      }
        //   }
        //   e.Effect = DragDropEffects.None;
        //}

        ///// <summary>
        ///// 拖放节点改变父子关系时处理实际数据的父子关系
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void BeforeDropNode_treeList_FTATable(object sender, BeforeDropNodeEventArgs e)
        //{
        //   try
        //   {
        //      e.Cancel = true;
        //      if (e.DestinationNode != null && e.SourceNode != null)
        //      {
        //         object obj = e.DestinationNode.GetValue(FixedString.COLUMNAME_DATA);
        //         DrawData DestinationNode = null;
        //         if (obj != null && obj.GetType() == typeof(DrawData))
        //         {
        //            DestinationNode = obj as DrawData;
        //         }
        //         obj = e.SourceNode.GetValue(FixedString.COLUMNAME_DATA);
        //         DrawData SourceNode = null;
        //         if (obj != null && obj.GetType() == typeof(DrawData))
        //         {
        //            SourceNode = obj as DrawData;
        //         }
        //         if (SourceNode != null && DestinationNode != null && SourceNode != DestinationNode && DestinationNode != SourceNode.Parent)
        //         {
        //            //转换节点实际的父子关系
        //            if (SourceNode.Parent != null && SourceNode.Parent.Children != null) SourceNode.Parent.Children.Remove(SourceNode);
        //            SourceNode.Parent = DestinationNode;
        //            DestinationNode.Children.Add(SourceNode);

        //            //节点图形结构变化
        //            this.ftaDiagram.FTADiagram_Load();
        //         }
        //      }
        //   }
        //   catch (System.Exception ex)
        //   {
        //      MsgBox.Show($"{ FixedString.EXCEPTION} { ex.Message}", this.programModel.String.MessageBoxCaption);
        //   }
        //}
    }
}
