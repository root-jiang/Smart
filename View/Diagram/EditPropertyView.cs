using System;
using DevExpress.XtraEditors;
using System.Collections.Generic;
using FaultTreeAnalysis.Model.Data;
using FaultTreeAnalysis.Common;
using FaultTreeAnalysis.Model.Enum;
using System.Windows.Forms;
using System.Linq;

namespace FaultTreeAnalysis.FTAControlEventHandle.FTADiagram
{
    public partial class EditPropertyView : XtraForm
    {
        private DrawData drawData;
        private StringModel FString;

        /// <summary>
        /// 单个DrawData属性编辑窗体的结果字符串
        /// </summary>
        public string[] Result { get; private set; } = new string[0];

        /// <summary>
        /// 单个DrawData属性编辑窗体的构造函数
        /// </summary>
        /// <param name="ftaProgram"></param>
        /// <param name="drawData"></param>
        public EditPropertyView(ProgramModel ftaProgram, DrawData drawData, bool isTransfer = false)
        {
            InitializeComponent();
            this.drawData = drawData;
            this.Initialize(ftaProgram.CurrentSystem, ftaProgram.String, drawData);
            if (isTransfer)
            {
                this.textEdit1.Enabled = false;
            }
        }

        /// <summary>
        /// 单个DrawData属性编辑窗体的初始化函数
        /// </summary>
        /// <param name="ftaSystem"></param>
        /// <param name="ftaString"></param>
        /// <param name="drawData"></param>
        private void Initialize(SystemModel ftaSystem, StringModel ftaString, DrawData drawData)
        {
            this.textEdit7.Enabled = false;
            FString = ftaString;
            if (drawData.IsGateType)
            {
                this.textEdit4.Enabled = false;
                this.textEdit5.Enabled = false;
                this.textEdit6.Enabled = false;
                if (drawData.Type == DrawType.VotingGate)
                {
                    this.textEdit7.Enabled = true;
                }
            }

            //先移除事件，防止初始化值时触发自动编号事件
            this.textEdit1.SelectedIndexChanged -= textEdit1_SelectedIndexChanged;

            this.textEdit0.Text = drawData.Identifier;
            this.textEdit1.Text = DrawData.GetDescriptionByEnum(drawData.Type);
            this.textEdit1.Properties.Items.AddRange(drawData.GetAvailableTypeSource(ftaSystem, ftaString));

            this.textEdit2.Text = drawData.Comment1;

            this.textEdit3.Text = drawData.LogicalCondition;
            this.textEdit3.Properties.Items.AddRange(DrawData.GetAvailableLogicalConditionSource(ftaString));

            this.comboBoxEdit1.Properties.Items.AddRange(DrawData.GetAvailableInputTypeSource(ftaString));
            comboBoxEdit1.Text = drawData.InputType;

            this.textEdit4.Text = drawData.InputValue;
            this.textEdit5.Text = drawData.InputValue2;
            this.textEdit7.Text = drawData.ExtraValue1;

            this.textEdit6.Text = drawData.Units;
            this.textEdit6.Properties.Items.AddRange(DrawData.GetAvailableUnitSource(ftaString));

            textEdit1.Text = General.CNEN_Changes(General.FtaProgram.Setting.Language, textEdit1.Text);
            textEdit3.Text = General.CNEN_Changes(General.FtaProgram.Setting.Language, textEdit3.Text);
            textEdit6.Text = General.CNEN_Changes(General.FtaProgram.Setting.Language, textEdit6.Text);
            comboBoxEdit1.Text = General.CNEN_Changes(General.FtaProgram.Setting.Language, comboBoxEdit1.Text);

            //重新绑定事件
            this.textEdit1.SelectedIndexChanged += textEdit1_SelectedIndexChanged;

            if (this.drawData.IsGateType)
            {
                comboBoxEdit1.Enabled = false;
            }

            this.BindLangage(ftaString);
            this.RegisterEvent();
        }

        /// <summary>
        /// 单个DrawData属性编辑窗体的语言绑定函数
        /// </summary>
        /// <param name="ftaString"></param>
        private void BindLangage(StringModel ftaString)
        {
            this.labelControl1.Text = ftaString.Identifier;
            this.labelControl2.Text = ftaString.Type;
            this.labelControl3.Text = ftaString.Comment1;
            this.labelControl4.Text = ftaString.LogicalCondition;
            if (comboBoxEdit1.SelectedIndex == 2)
            {
                this.labelControl5.Text = FString.InputValue_Constant;
            }
            else if (!this.drawData.IsGateType)
            {
                this.labelControl5.Text = FString.InputValue;
            }
            this.labelControl9.Text = ftaString.ExtraValue1;
            this.labelControl6.Text = ftaString.InputValue2;
            this.labelControl7.Text = ftaString.Units;
            this.Text = ftaString.EditPropertyViewTitle;
            this.simpleButton1.Text = ftaString.OK;
            this.simpleButton2.Text = ftaString.Cancel;
            this.labelControl8.Text = ftaString.InputType;
        }

        /// <summary>
        /// 单个DrawData属性编辑窗体的事件的注册
        /// </summary>
        private void RegisterEvent()
        {
            this.simpleButton1.Click += Button_Click;
            this.simpleButton2.Click += Button_Click;
        }

        /// <summary>
        /// 单个DrawData属性编辑窗体的事件注销
        /// </summary>
        private void UnregisterEvent()
        {
            this.simpleButton1.Click -= Button_Click;
            this.simpleButton2.Click -= Button_Click;
        }

        /// <summary>
        /// 单个DrawData属性编辑窗体的按钮事件的处理函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, EventArgs e)
        {
            if (sender.Equals(this.simpleButton1))
            {
                if (this.drawData.IsGateType == false)
                {
                    try//检查选择固定概率时，概率值的范围是否满足0<X<1
                    {
                        if (this.textEdit4.Text != "")
                        {
                            double dData = 0.0;
                            dData = double.Parse(textEdit4.Text, System.Globalization.NumberStyles.Float);

                            if (comboBoxEdit1.SelectedIndex == 2)
                            {
                                if (dData < 0 || dData > 1)
                                {
                                    MsgBox.Show(FString.InputValue_Failed1);
                                    return;
                                }
                            }
                            else
                            {
                                if (dData < 0 || dData > 1)
                                {
                                    MsgBox.Show(FString.InputValue_Failed);
                                    return;
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        MsgBox.Show(this.labelControl5.Text + "_" + FString.InputValue_Failed2 + ":" + textEdit4.Text);
                        return;
                    }

                    if (comboBoxEdit1.SelectedIndex != 2) //检查选择非固定概率时，概率值时间值的范围是否满足
                    {
                        if (this.textEdit4.Text != "")
                        {
                            decimal a;
                            if (decimal.TryParse(this.textEdit4.Text, System.Globalization.NumberStyles.Float, null, out a) == false || a < 0)
                            {
                                MsgBox.Show(this.labelControl5.Text + "_" + FString.InputValue_Failed2 + ":" + textEdit4.Text);
                                return;
                            }
                        }
                        if (this.textEdit5.Text != "")
                        {
                            decimal b;
                            if (decimal.TryParse(this.textEdit5.Text, System.Globalization.NumberStyles.Float, null, out b) == false || b <= 0)
                            {
                                MsgBox.Show(this.labelControl6.Text + "_" + FString.InputValue_Failed2 + ":" + textEdit5.Text);
                                return;
                            }
                        }
                    }
                }

                if (drawData.Type == DrawType.VotingGate)
                {
                    if (this.textEdit7.Text == "")
                    {
                        XtraMessageBox.Show(General.FtaProgram.String.IntegrityCheckWarning_UnreasonableVote, "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (Convert.ToInt32(this.textEdit7.Text) > drawData.Children.Count)
                    {
                        XtraMessageBox.Show(General.FtaProgram.String.IntegrityCheckWarning_UnreasonableVote, "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                var result = new List<string> { this.textEdit0.Text, this.textEdit1.Text, this.textEdit2.Text, this.textEdit3.Text };
                if (this.drawData.IsGateType == false)
                {
                    result.Add(this.textEdit4.Text);
                    result.Add(this.textEdit5.Text);
                    result.Add(this.textEdit6.Text);
                    result.Add(this.comboBoxEdit1.Text);
                }
                result.Add(this.textEdit7.Text);
                this.Result = result.ToArray();

                //检查是否存在同ID
                if (General.FtaProgram.CurrentSystem.GetAllDatas().Where(d => d.Identifier == textEdit0.Text).Count() > 1)
                {
                    DialogResult rs = MsgBox.Show("已存在相同ID的项，是否使用当前属性参数覆盖重复项（选否则使用原属性,选取消则不保存继续修改）?", "", System.Windows.Forms.MessageBoxButtons.YesNoCancel);
                    if (rs == DialogResult.Yes)
                    {
                        General.isRepeatCopyCurrentValues = true;
                    }
                    else if (rs == DialogResult.No)
                    {
                        General.isRepeatCopyCurrentValues = false;
                    }
                    else
                    {
                        return;
                    }
                }
                else if (General.FtaProgram.CurrentSystem.GetAllDatas().Where(d => d.Identifier == textEdit0.Text).Count() == 1 && this.drawData.Identifier != textEdit0.Text)
                {
                    DialogResult rs = MsgBox.Show("已存在相同ID的项，是否使用当前属性参数覆盖重复项（选否则使用原属性,选取消则不保存继续修改）?", "", System.Windows.Forms.MessageBoxButtons.YesNoCancel);
                    if (rs == DialogResult.Yes)
                    {
                        General.isRepeatCopyCurrentValues = true;
                    }
                    else if (rs == DialogResult.No)
                    {
                        General.isRepeatCopyCurrentValues = false;
                    }
                    else
                    {
                        return;
                    }
                }
            }

            this.Close();
        }

        private void EditPropertyView_Load(object sender, EventArgs e)
        {
        }

        private void comboBoxEdit1_TextChanged(object sender, EventArgs e)
        {
            if (comboBoxEdit1.SelectedIndex == 2)
            {
                textEdit5.Text = "1";
                textEdit5.Enabled = false;
                textEdit4.Text = this.drawData.QValue;
                this.labelControl5.Text = FString.InputValue_Constant;
            }
            else if (!this.drawData.IsGateType)
            {
                textEdit5.Enabled = true;
                this.labelControl5.Text = FString.InputValue;
            }
        }

        /// <summary>
        /// 门或事件类型改变自动重命名（按序号递增）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //门或事件类型改变时，自动重命名
            int MaxIDGate = 0;
            int MaxIDEvent = 0;
            DrawData[] AllDatas = General.FtaProgram.CurrentSystem.GetAllDatas().ToArray();

            var typeName = General.GetKeyName(textEdit1.Text);
            var type = (DrawType)Enum.Parse(typeof(DrawType), typeName);

            if (type == DrawType.AndGate || type == DrawType.OrGate || type == DrawType.PriorityAndGate || type == DrawType.RemarksGate || type == DrawType.VotingGate || type == DrawType.XORGate)//当前对象如果被修改成门类型
            {
                //如果原始数据是门，不重命名
                if (this.drawData.IsGateType)
                {
                    textEdit0.Text = this.drawData.Identifier;
                    return;
                }

                foreach (DrawData data in AllDatas)
                {
                    string rel = "";
                    for (int i = data.Identifier.Length - 1; i >= 0; i--) //统计字符串长度，并设定增量。
                    {
                        int r = 0;
                        if (int.TryParse(data.Identifier[i].ToString(), out r))
                        {
                            rel = data.Identifier[i].ToString() + rel;
                        }
                        else
                        {
                            break;
                        }
                    }

                    try
                    {
                        string NewID = "0" + rel;
                        if (data.IsGateType && data != drawData && Convert.ToInt32(NewID) > MaxIDGate)
                        {
                            MaxIDGate = Convert.ToInt32(NewID);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            else if (type == DrawType.BasicEvent || type == DrawType.HouseEvent || type == DrawType.UndevelopedEvent)//当前对象如果被修改成事件类型
            {
                //如果原始数据是事件，不重命名
                if (!this.drawData.IsGateType)
                {
                    textEdit0.Text = this.drawData.Identifier;
                    return;
                }

                foreach (DrawData data in AllDatas)
                {
                    string rel = "";
                    for (int i = data.Identifier.Length - 1; i >= 0; i--) //统计字符串长度，并设定增量。
                    {
                        int r = 0;
                        if (int.TryParse(data.Identifier[i].ToString(), out r))
                        {
                            rel = data.Identifier[i].ToString() + rel;
                        }
                        else
                        {
                            break;
                        }
                    }

                    try
                    {
                        string NewID = "0" + rel;
                        if (data.IsGateType == false && data != drawData && Convert.ToInt32(NewID) > MaxIDEvent)
                        {
                            MaxIDEvent = Convert.ToInt32(NewID);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            if (type == DrawType.AndGate || type == DrawType.OrGate || type == DrawType.PriorityAndGate || type == DrawType.RemarksGate || type == DrawType.VotingGate || type == DrawType.XORGate)//当前对象如果被修改成门类型
            {
                textEdit0.Text = "Gate" + (MaxIDGate + 1).ToString();
            }
            else if (type == DrawType.BasicEvent || type == DrawType.HouseEvent || type == DrawType.UndevelopedEvent)//当前对象如果被修改成事件类型
            {
                textEdit0.Text = "Event" + (MaxIDEvent + 1).ToString();
            }
        }

        private void textEdit7_Properties_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }
    }
}
