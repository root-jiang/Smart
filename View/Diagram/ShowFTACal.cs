using System;
using System.Collections.Generic;
using System.Data;
using FaultTreeAnalysis.Common;

namespace FaultTreeAnalysis.View.Diagram
{
    public partial class ShowFTACal : DevExpress.XtraEditors.XtraForm
    {
        public ShowFTACal()
        {
            InitializeComponent();
        }

        public DrawData SelectNode = null;
        public Dictionary<DrawData, List<string>> AllGJCalsList = new Dictionary<DrawData, List<string>>();//所有最小割级

        private void ShowFTACal_Load(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Name");
                dt.Columns.Add("Aggregate");
                dt.Columns.Add("Probability");

                string GName = SelectNode.Identifier;
                string Aggregate = AllGJCalsList[SelectNode][0];
                string Probability = AllGJCalsList[SelectNode][1];

                dt.Rows.Add(new string[] { GName, Aggregate, Probability });

                gridControl1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MsgBox.Show(ex.Message);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkEdit1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkEdit1.Checked)
            {
                try
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("ID");
                    dt.Columns.Add("Name");
                    dt.Columns.Add("Aggregate");
                    dt.Columns.Add("Probability");

                    int i = 0;
                    foreach (KeyValuePair<DrawData, List<string>> item1 in AllGJCalsList)
                    {
                        i += 1;
                        string GName = item1.Key.Identifier;
                        string Aggregate = item1.Value[0];
                        string Probability = item1.Value[1];

                        dt.Rows.Add(new string[] { i.ToString("00000"), GName, Aggregate, Probability });
                    }

                    dt.DefaultView.Sort = "ID desc";
                    gridControl1.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MsgBox.Show(ex.Message);
                }
            }
            else
            {
                try
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("ID");
                    dt.Columns.Add("Name");
                    dt.Columns.Add("Aggregate");
                    dt.Columns.Add("Probability");

                    string GName = SelectNode.Identifier;
                    string Aggregate = AllGJCalsList[SelectNode][0];
                    string Probability = AllGJCalsList[SelectNode][1];

                    dt.Rows.Add(new string[] { "1", GName, Aggregate, Probability });

                    gridControl1.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MsgBox.Show(ex.Message);
                }
            }
        }
    }
}