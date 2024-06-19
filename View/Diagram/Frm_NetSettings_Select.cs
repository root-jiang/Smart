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

namespace IntegratedSystem.View.TestFunction
{
    public partial class Frm_NetSettings_Select : DevExpress.XtraEditors.XtraForm
    {
        public Frm_NetSettings_Select()
        {
            InitializeComponent();
        }

        public bool CG = false;
        public DataTable DTServers = new DataTable();
        public List<DrawData> InsertDatas = new List<DrawData>();

        private void Frm_NetSettings_Select_Load(object sender, EventArgs e)
        {
            try
            {
                InsertDatas = new List<DrawData>();
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
                this.simpleButton1.Text = General.FtaProgram.String.OK;
                this.simpleButton2.Text = General.FtaProgram.String.Cancel;

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
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("读取基本事件库失败：" + ex.Message, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {

        }

        private void Frm_NetSettings_Select_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void gridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (int handle in gridView1.GetSelectedRows())
                {
                    string GUID = gridView1.GetRowCellDisplayText(handle, "GUID");
                    string Identifier = gridView1.GetRowCellDisplayText(handle, "Identifier");
                    string Type = gridView1.GetRowCellDisplayText(handle, "Type");
                    string Description = gridView1.GetRowCellDisplayText(handle, "Description");
                    string LogicalCondition = gridView1.GetRowCellDisplayText(handle, "LogicalCondition");
                    string InputType = gridView1.GetRowCellDisplayText(handle, "InputType");
                    string InputValue = gridView1.GetRowCellDisplayText(handle, "InputValue");
                    string InputValue2 = gridView1.GetRowCellDisplayText(handle, "InputValue2");
                    string ExtraValue1 = gridView1.GetRowCellDisplayText(handle, "ExtraValue1");
                    string Units = gridView1.GetRowCellDisplayText(handle, "Units");

                    var typeName = General.GetKeyName(Type);
                    var typeE = (DrawType)Enum.Parse(typeof(DrawType), typeName);

                    DrawData item = new DrawData();
                    item.Identifier = Identifier;
                    item.GUID = GUID;
                    item.Type = typeE;
                    item.Comment1 = Description;
                    item.LogicalCondition = LogicalCondition;
                    item.InputType = InputType;
                    item.InputValue = InputValue;
                    item.InputValue2 = InputValue2;
                    item.ExtraValue1 = ExtraValue1;
                    item.Units = Units;
                    InsertDatas.Add(item);
                }
            }
            catch (Exception)
            {
                InsertDatas = new List<DrawData>();
            }

            this.Close();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}