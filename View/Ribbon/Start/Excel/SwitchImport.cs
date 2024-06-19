using System;
using DevExpress.XtraEditors;
using FaultTreeAnalysis.Common;

namespace FaultTreeAnalysis.View.Ribbon.Start.Excel
{
    public partial class SwitchImport : XtraForm
   {
      public bool? IsImportToSystem { get; set; }

      public SwitchImport()
      {
         InitializeComponent();

         this.simpleButton1.Click += SimpleButtonClick;
         this.simpleButton2.Click += SimpleButtonClick;
      }

      private void SimpleButtonClick(object sender, EventArgs e)
      {
         if (sender == this.simpleButton1) this.IsImportToSystem = this.radioGroup1.SelectedIndex == 0 ? true : false;
         else this.IsImportToSystem = null;
         this.Close();
      }

        private void SwitchImport_Load(object sender, EventArgs e)
        {
            try
            {
                this.simpleButton1.Text = General.FtaProgram.String.OK;
                this.simpleButton2.Text = General.FtaProgram.String.Cancel;
                this.Text = General.FtaProgram.String.SwitchImport_Text; 

                radioGroup1.Properties.Items.Clear();
                radioGroup1.Properties.Items.Add(new DevExpress.XtraEditors.Controls.RadioGroupItem { Description = General.FtaProgram.String.SwitchImport_Item1 });
                radioGroup1.Properties.Items.Add(new DevExpress.XtraEditors.Controls.RadioGroupItem { Description = General.FtaProgram.String.SwitchImport_Item2 });
            }
            catch (Exception)
            { 
            }
        }
    }
}