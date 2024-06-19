using System;
using System.Data;
using FaultTreeAnalysis.Common;

namespace FaultTreeAnalysis.View.Ribbon.Start
{
    public partial class Frm_Keymap : DevExpress.XtraEditors.XtraForm
    {
        public Frm_Keymap()
        {
            InitializeComponent();
        }

        private void Frm_Keymap_Load(object sender, EventArgs e)
        {
            this.Text = General.FtaProgram.String.ShowKeymap;
            gridColumn1.Caption = General.FtaProgram.String.ShowKeymapName;
            gridColumn2.Caption = General.FtaProgram.String.ShowKeymapKey;

            DataTable dt = new DataTable();
            dt.Columns.Add("KeyName");
            dt.Columns.Add("KeyValue");
            dt.Rows.Add(new string[] { General.FtaProgram.String.Copy, "Ctrl+C" });
            dt.Rows.Add(new string[] { General.FtaProgram.String.Paste, "Ctrl+V" });
            dt.Rows.Add(new string[] { General.FtaProgram.String.Cut, "Ctrl+X" });
            dt.Rows.Add(new string[] { General.FtaProgram.String.SaveAll, "Ctrl+S" });
            dt.Rows.Add(new string[] { General.FtaProgram.String.Undo, "Ctrl+Z" });
            dt.Rows.Add(new string[] { General.FtaProgram.String.Redo, "Ctrl+R" });
            dt.Rows.Add(new string[] { General.FtaProgram.String.PosParent, "UP" });
            dt.Rows.Add(new string[] { General.FtaProgram.String.PosChild, "Down" });
            dt.Rows.Add(new string[] { General.FtaProgram.String.PosLeft, "Left" });
            dt.Rows.Add(new string[] { General.FtaProgram.String.PosRight, "Right" });
            dt.Rows.Add(new string[] { General.FtaProgram.String.ZoomOut, "Ctrl+MouseWheelUP" });
            dt.Rows.Add(new string[] { General.FtaProgram.String.ZoomIn, "Ctrl+MouseWheelDown" });
            dt.Rows.Add(new string[] { General.FtaProgram.String.ExitFullScreen, "Esc" });
            gridControl1.DataSource = dt;
        }
    }
}