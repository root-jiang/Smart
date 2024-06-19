using System;
using System.Windows.Forms;
using FaultTreeAnalysis.Common;
using System.IO;

namespace FaultTreeAnalysis.View.Ribbon.Start
{
    public partial class Frm_UserManual : DevExpress.XtraEditors.XtraForm
    {
        public Frm_UserManual()
        {
            InitializeComponent();
        }

        private void Frm_UserManual_Load(object sender, EventArgs e)
        {
            this.Text = General.FtaProgram.String.UserManual;
            General.TryCatch(() =>
            {
                if (File.Exists(Application.StartupPath + "\\SmarTree_UserManual.pdf"))
                {
                    pdfViewer1.DocumentFilePath = Application.StartupPath + "\\SmarTree_UserManual.pdf";
                }
                else
                {
                    MsgBox.Show(General.FtaProgram.String.UserManualTip);
                }
            });
        }
    }
}