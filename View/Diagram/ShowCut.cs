using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FaultTreeAnalysis.Common;
using FaultTreeAnalysis.Model.Data;

namespace FaultTreeAnalysis.View.Diagram
{
    public partial class ShowCut : DevExpress.XtraEditors.XtraForm
    {
        public ShowCut()
        {
            InitializeComponent();
        }

        private void ShowCut_Load(object sender, EventArgs e)
        {
            RefreshText();
        }

        public void RefreshText()
        {
            try
            {
                simpleButton2.Text = General.FtaProgram.String.Refresh;
                this.Text = General.FtaProgram.String.ShowCut;

                gridColumn3.Caption = General.FtaProgram.String.ShowCutCheck;//高亮
                gridColumn1.Caption = General.FtaProgram.String.ShowCutName;//割级
                gridColumn4.Caption = General.FtaProgram.String.ShowCutszProb;//概率
                labelControl1.Text = General.FtaProgram.String.ShowCutLevel;//显示的割级个数上限
            }
            catch (Exception ex)
            {
                MsgBox.Show(ex.Message);
            }
        }

        private void gridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            if (gridControl1.DataSource == null)
            {
                return;
            }

            bool AC = !(bool)gridView1.GetRowCellValue(e.RowHandle, "Check");

            foreach (DataRow row in ((DataTable)gridControl1.DataSource).Rows)
            {
                row["Check"] = false;
            }

            gridView1.SetRowCellValue(e.RowHandle, "Check", AC);

            OneCutsetModel item = (OneCutsetModel)gridView1.GetRowCellValue(e.RowHandle, "Tag");

            //选中的对象
            DrawData data_Selected = General.DiagramItemPool.SelectedData.FirstOrDefault();
            if (item != null)
            {
                OneCutsetModel cutSet = item;
                if (General.FtaProgram.CurrentSystem.CurrentSelectedCutset != null && General.FtaProgram.CurrentSystem.CurrentSelectedCutset.Cutset == cutSet)
                {
                    General.FtaProgram.CurrentSystem.CurrentSelectedCutset = null;
                }
                else
                {
                    General.FtaProgram.CurrentSystem.CurrentSelectedCutset = new HighLightCutSet();
                    General.FtaProgram.CurrentSystem.CurrentSelectedCutset.Cutset = cutSet;
                    General.FtaProgram.CurrentSystem.CurrentSelectedCutset.HighLightData = new HashSet<DrawData>();
                    //重新计算路径
                    List<DrawData> allDatas = General.FtaProgram.CurrentSystem.GetAllDatas().ToList();
                    var ids = cutSet.Events.Distinct();
                    foreach (string id in ids)
                    {
                        var evts = allDatas.Where(obj => obj.CanRepeatedType && obj.Identifier == id);
                        foreach (DrawData evt in evts)
                        {
                            List<DrawData> path = evt.GetPath(data_Selected, General.FtaProgram.CurrentSystem.TranferGates);
                            if (path != null)
                            {
                                foreach (DrawData tmp in path)
                                {
                                    General.FtaProgram.CurrentSystem.CurrentSelectedCutset.HighLightData.Add(tmp);
                                }
                            }
                        }
                    }
                }
                General.DiagramControl.Refresh();
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="initial">是否初始化默认值</param>
        public void RefreshDatas(bool initial)
        {
            DataTable sub_Items = new DataTable();
            sub_Items.Columns.Add("Name");
            sub_Items.Columns.Add("szProb");
            sub_Items.Columns.Add("Tag", new OneCutsetModel().GetType());
            sub_Items.Columns.Add("Check", true.GetType());

            if (General.DiagramItemPool.SelectedData == null)
            {
                gridControl1.DataSource = sub_Items;
                General.DiagramControl.Refresh();
                return;
            }

            //获取当前割级最大级数作为默认值
            DrawData data_Selected = General.DiagramItemPool.SelectedData.FirstOrDefault();
            List<OneCutsetModel> ordered_CutSet1 = data_Selected.Cutset.ListCutsets_Real.OrderByDescending(obj => obj.Events.Count).ToList();
            if (initial)
            {
                if (ordered_CutSet1.Count > 0)
                {
                    spinEdit1.EditValue = ordered_CutSet1[0].Events.Count;
                }
            }

            List<OneCutsetModel> ordered_CutSet = data_Selected.Cutset.ListCutsets_Real.OrderBy(obj => obj.szProb).ToList();
            for (int i = 0; i < ordered_CutSet.Count; i++)
            {
                if (ordered_CutSet[i] != null && !string.IsNullOrEmpty(ordered_CutSet[i].szProb) && ordered_CutSet[i].Events != null && ordered_CutSet[i].Events.Count > 0)
                {
                    string CName = "";

                    if (ordered_CutSet[i].Events.Count > Convert.ToInt32(spinEdit1.EditValue))
                    {
                        CName = "";
                    }
                    else
                    {
                        CName = string.Join(",", ordered_CutSet[i].Events);
                    }

                    if (CName != "")
                    {
                        string Caption = i + ":Q = " + ordered_CutSet[i].szProb;
                        OneCutsetModel Tag = ordered_CutSet[i];
                        if (General.FtaProgram.CurrentSystem.CurrentSelectedCutset != null && General.FtaProgram.CurrentSystem.CurrentSelectedCutset.Cutset != null && General.FtaProgram.CurrentSystem.CurrentSelectedCutset.Cutset == ordered_CutSet[i])
                        {
                            //如果是已经选择的割集 ，勾选
                            sub_Items.Rows.Add(new object[] { "{" + CName + "}", Caption, Tag, true });
                        }
                        else
                        {
                            //否则 ，不勾选
                            sub_Items.Rows.Add(new object[] { "{" + CName + "}", Caption, Tag, false });
                        }
                    }
                    else if (General.FtaProgram.CurrentSystem.CurrentSelectedCutset != null && General.FtaProgram.CurrentSystem.CurrentSelectedCutset.Cutset != null && General.FtaProgram.CurrentSystem.CurrentSelectedCutset.Cutset == ordered_CutSet[i])
                    {
                        General.FtaProgram.CurrentSystem.CurrentSelectedCutset.Cutset = null;
                    }
                }
            }
            gridControl1.DataSource = sub_Items;
            General.DiagramControl.Refresh();
        }

        private void spinEdit1_EditValueChanged(object sender, EventArgs e)
        {
            RefreshDatas(false);
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            RefreshDatas(true);
        }
    }
}
