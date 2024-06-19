using System;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using FaultTreeAnalysis.Common;
using FaultTreeAnalysis;
using DevExpress.Internal.WinApi;
using System.Linq;
using FaultTreeAnalysis.Model.Enum;
using System.Collections.Generic;
using DevExpress.Internal.WinApi.Windows.UI.Notifications;
using FaultTreeAnalysis.Model.Data;
using System.IO;
using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace IntegratedSystem.View.TestFunction
{
    public partial class Frm_NetSettings : DevExpress.XtraEditors.XtraForm
    {
        public Frm_NetSettings()
        {
            InitializeComponent();
        }

        public bool CG = false;
        public DataTable DTServers = new DataTable();

        public List<string> GetGroups()
        {
            try
            {
                List<string> Groups = new List<string>();
                foreach (DataRow da in General.EventsLibDB.Rows)
                {
                    string Group = da["Group"]?.ToString();
                    if (!Groups.Contains(Group) && Group != "")
                    {
                        Groups.Add(Group);
                    }
                }
                return Groups;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        private void Frm_NetSettings_Load(object sender, EventArgs e)
        {
            try
            {
                //基本事件库
                if (File.Exists(System.Environment.CurrentDirectory + "\\BasicEvents.db"))
                {
                    //开启接收线程，读取数据
                    General.EventsLibDB = ConnectSever.ReadInitial("BasicEvents", " ORDER BY ID");
                    General.EventsLibDB.TableName = "BasicEvents";
                }
                if (General.EventsLibDB != null)
                {
                    gridControl1.DataSource = General.EventsLibDB;
                }
                textEdit9.Properties.Items.Clear();
                textEdit9.Properties.Items.AddRange(GetGroups());

                this.layoutControlItem10.Text = General.FtaProgram.String.Groups;
                this.layoutControlItem9.Text = General.FtaProgram.String.Identifier;
                this.layoutControlItem6.Text = General.FtaProgram.String.Type;
                this.layoutControlItem3.Text = General.FtaProgram.String.Comment1;
                this.layoutControlItem5.Text = General.FtaProgram.String.LogicalCondition;
                this.layoutControlItem18.Text = General.FtaProgram.String.InputType;
                this.layoutControlItem13.Text = General.FtaProgram.String.InputValue;
                this.layoutControlItem14.Text = General.FtaProgram.String.InputValue2;
                this.layoutControlItem21.Text = General.FtaProgram.String.ExtraValue1;
                this.layoutControlItem4.Text = General.FtaProgram.String.Units;
                this.simpleButton2.Text = General.FtaProgram.String.AddE;
                this.simpleButton3.Text = General.FtaProgram.String.Delete;
                this.simpleButton1.Text = General.FtaProgram.String.Exit;
                this.simpleButton4.Text = General.FtaProgram.String.Edit;

                gridColumn8.Caption = "GUID";
                gridColumn11.Caption = General.FtaProgram.String.Groups;
                gridColumn10.Caption = General.FtaProgram.String.Identifier;
                gridColumn1.Caption = General.FtaProgram.String.Type;
                gridColumn9.Caption = General.FtaProgram.String.Comment1;
                gridColumn4.Caption = General.FtaProgram.String.LogicalCondition;
                gridColumn5.Caption = General.FtaProgram.String.InputType;
                gridColumn2.Caption = General.FtaProgram.String.InputValue;
                gridColumn7.Caption = General.FtaProgram.String.InputValue2;
                gridColumn3.Caption = General.FtaProgram.String.ExtraValue1;
                gridColumn6.Caption = General.FtaProgram.String.Units;

                this.textEdit0.Text = "";
                this.textEdit1.Text = "";
                this.textEdit2.Text = "";
                this.textEdit3.Text = "";
                this.comboBoxEdit1.Text = "";
                this.textEdit4.Text = "";
                this.textEdit5.Text = "";
                this.textEdit7.Text = "";
                this.textEdit6.Text = "";
                this.textEdit8.Text = "";
                this.textEdit9.Text = "";

                List<string> result = new List<string>();
                DrawType[] type = new DrawType[] { DrawType.BasicEvent, DrawType.HouseEvent, DrawType.UndevelopedEvent };
                foreach (DrawType tmp in type)
                {
                    result.Add(tmp.ToString());
                }
                this.textEdit1.Properties.Items.AddRange(result);
                this.textEdit3.Properties.Items.AddRange(DrawData.GetAvailableLogicalConditionSource(General.FtaProgram.String));
                this.comboBoxEdit1.Properties.Items.AddRange(DrawData.GetAvailableInputTypeSource(General.FtaProgram.String));
                this.textEdit6.Properties.Items.AddRange(DrawData.GetAvailableUnitSource(General.FtaProgram.String));

                textEdit1.Text = General.CNEN_Changes(General.FtaProgram.Setting.Language, textEdit1.Text);
                textEdit3.Text = General.CNEN_Changes(General.FtaProgram.Setting.Language, textEdit3.Text);
                textEdit6.Text = General.CNEN_Changes(General.FtaProgram.Setting.Language, textEdit6.Text);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("读取基本事件库失败：" + ex.Message, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                this.textEdit0.Text = gridView1.GetFocusedRowCellDisplayText("GUID");
                this.textEdit9.Text = gridView1.GetFocusedRowCellDisplayText("Group");
                this.textEdit8.Text = gridView1.GetFocusedRowCellDisplayText("Identifier");
                this.textEdit1.Text = gridView1.GetFocusedRowCellDisplayText("Type");
                this.textEdit2.Text = gridView1.GetFocusedRowCellDisplayText("Description");
                this.textEdit3.Text = gridView1.GetFocusedRowCellDisplayText("LogicalCondition");
                this.comboBoxEdit1.Text = gridView1.GetFocusedRowCellDisplayText("InputType");
                this.textEdit4.Text = gridView1.GetFocusedRowCellDisplayText("InputValue");
                this.textEdit5.Text = gridView1.GetFocusedRowCellDisplayText("InputValue2");
                this.textEdit7.Text = gridView1.GetFocusedRowCellDisplayText("ExtraValue1");
                this.textEdit6.Text = gridView1.GetFocusedRowCellDisplayText("Units");

                textEdit1.Text = General.CNEN_Changes(General.FtaProgram.Setting.Language, textEdit1.Text);
                textEdit3.Text = General.CNEN_Changes(General.FtaProgram.Setting.Language, textEdit3.Text);
                textEdit6.Text = General.CNEN_Changes(General.FtaProgram.Setting.Language, textEdit6.Text);
            }
            catch (Exception)
            {
            }
        }

        private void Frm_NetSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DialogResult rs = MsgBox.Show("确定执行操作?", "", System.Windows.Forms.MessageBoxButtons.YesNo);
            if (rs == DialogResult.No)
            {
                return;
            }
            try
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
                                MsgBox.Show(General.FtaProgram.String.InputValue_Failed1);
                                return;
                            }
                        }
                        else
                        {
                            if (dData < 0 || dData > 1)
                            {
                                MsgBox.Show(General.FtaProgram.String.InputValue_Failed);
                                return;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    MsgBox.Show(General.FtaProgram.String.InputValue_Failed2 + ":" + textEdit4.Text);
                    return;
                }

                if (comboBoxEdit1.SelectedIndex != 2) //检查选择非固定概率时，概率值时间值的范围是否满足
                {
                    if (this.textEdit4.Text != "")
                    {
                        decimal a;
                        if (decimal.TryParse(this.textEdit4.Text, System.Globalization.NumberStyles.Float, null, out a) == false || a < 0)
                        {
                            MsgBox.Show(General.FtaProgram.String.InputValue_Failed2 + ":" + textEdit4.Text);
                            return;
                        }
                    }
                    if (this.textEdit5.Text != "")
                    {
                        decimal b;
                        if (decimal.TryParse(this.textEdit5.Text, System.Globalization.NumberStyles.Float, null, out b) == false || b <= 0)
                        {
                            MsgBox.Show(General.FtaProgram.String.InputValue_Failed2 + ":" + textEdit5.Text);
                            return;
                        }
                    }
                }
                this.textEdit0.Text = Guid.NewGuid().ToString();

                ConnectSever.InsertOne(new object[] { this.textEdit0.Text, this.textEdit9.Text, this.textEdit8.Text, this.textEdit1.Text, this.textEdit2.Text, this.textEdit3.Text, this.comboBoxEdit1.Text, this.textEdit4.Text, this.textEdit5.Text, this.textEdit7.Text, this.textEdit6.Text });
                //基本事件库
                if (File.Exists(System.Environment.CurrentDirectory + "\\BasicEvents.db"))
                {
                    //开启接收线程，读取数据
                    General.EventsLibDB = ConnectSever.ReadInitial("BasicEvents", " ORDER BY ID");
                    General.EventsLibDB.TableName = "BasicEvents";
                }
                if (General.EventsLibDB != null)
                {
                    gridControl1.DataSource = General.EventsLibDB;
                }
                textEdit9.Properties.Items.Clear();
                textEdit9.Properties.Items.AddRange(GetGroups());
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton3_Click(object sender, EventArgs e)
        {
            DialogResult rs = MsgBox.Show("确定执行操作?", "", System.Windows.Forms.MessageBoxButtons.YesNo);
            if (rs == DialogResult.No)
            {
                return;
            }
            try
            {
                ConnectSever.DeleteOne(this.textEdit0.Text);
                //基本事件库
                if (File.Exists(System.Environment.CurrentDirectory + "\\BasicEvents.db"))
                {
                    //开启接收线程，读取数据
                    General.EventsLibDB = ConnectSever.ReadInitial("BasicEvents", " ORDER BY ID");
                    General.EventsLibDB.TableName = "BasicEvents";
                }
                if (General.EventsLibDB != null)
                {
                    gridControl1.DataSource = General.EventsLibDB;
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton4_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult rs = MsgBox.Show("确定执行操作?", "", System.Windows.Forms.MessageBoxButtons.YesNo);
                if (rs == DialogResult.No)
                {
                    return;
                }
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
                                MsgBox.Show(General.FtaProgram.String.InputValue_Failed1);
                                return;
                            }
                        }
                        else
                        {
                            if (dData < 0 || dData > 1)
                            {
                                MsgBox.Show(General.FtaProgram.String.InputValue_Failed);
                                return;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    MsgBox.Show(General.FtaProgram.String.InputValue_Failed2 + ":" + textEdit4.Text);
                    return;
                }

                if (comboBoxEdit1.SelectedIndex != 2) //检查选择非固定概率时，概率值时间值的范围是否满足
                {
                    if (this.textEdit4.Text != "")
                    {
                        decimal a;
                        if (decimal.TryParse(this.textEdit4.Text, System.Globalization.NumberStyles.Float, null, out a) == false || a < 0)
                        {
                            MsgBox.Show(General.FtaProgram.String.InputValue_Failed2 + ":" + textEdit4.Text);
                            return;
                        }
                    }
                    if (this.textEdit5.Text != "")
                    {
                        decimal b;
                        if (decimal.TryParse(this.textEdit5.Text, System.Globalization.NumberStyles.Float, null, out b) == false || b <= 0)
                        {
                            MsgBox.Show(General.FtaProgram.String.InputValue_Failed2 + ":" + textEdit5.Text);
                            return;
                        }
                    }
                }

                ConnectSever.UpdateOne(new object[] { this.textEdit0.Text, this.textEdit9.Text, this.textEdit8.Text, this.textEdit1.Text, this.textEdit2.Text, this.textEdit3.Text, this.comboBoxEdit1.Text, this.textEdit4.Text, this.textEdit5.Text, this.textEdit7.Text, this.textEdit6.Text });

                //基本事件库
                if (File.Exists(System.Environment.CurrentDirectory + "\\BasicEvents.db"))
                {
                    //开启接收线程，读取数据
                    General.EventsLibDB = ConnectSever.ReadInitial("BasicEvents", " ORDER BY ID");
                    General.EventsLibDB.TableName = "BasicEvents";
                }
                if (General.EventsLibDB != null)
                {
                    gridControl1.DataSource = General.EventsLibDB;
                }
                textEdit9.Properties.Items.Clear();
                textEdit9.Properties.Items.AddRange(GetGroups());
            }
            catch (Exception)
            {
            }
        }

        private void gridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                this.textEdit0.Text = gridView1.GetRowCellDisplayText(e.RowHandle, "GUID");
                this.textEdit9.Text = gridView1.GetRowCellDisplayText(e.RowHandle, "Group");
                this.textEdit8.Text = gridView1.GetRowCellDisplayText(e.RowHandle, "Identifier");
                this.textEdit1.Text = gridView1.GetRowCellDisplayText(e.RowHandle, "Type");
                this.textEdit2.Text = gridView1.GetRowCellDisplayText(e.RowHandle, "Description");
                this.textEdit3.Text = gridView1.GetRowCellDisplayText(e.RowHandle, "LogicalCondition");
                this.comboBoxEdit1.Text = gridView1.GetRowCellDisplayText(e.RowHandle, "InputType");
                this.textEdit4.Text = gridView1.GetRowCellDisplayText(e.RowHandle, "InputValue");
                this.textEdit5.Text = gridView1.GetRowCellDisplayText(e.RowHandle, "InputValue2");
                this.textEdit7.Text = gridView1.GetRowCellDisplayText(e.RowHandle, "ExtraValue1");
                this.textEdit6.Text = gridView1.GetRowCellDisplayText(e.RowHandle, "Units");

                textEdit1.Text = General.CNEN_Changes(General.FtaProgram.Setting.Language, textEdit1.Text);
                textEdit3.Text = General.CNEN_Changes(General.FtaProgram.Setting.Language, textEdit3.Text);
                textEdit6.Text = General.CNEN_Changes(General.FtaProgram.Setting.Language, textEdit6.Text);
            }
            catch (Exception)
            {
            }
        }
    }
}