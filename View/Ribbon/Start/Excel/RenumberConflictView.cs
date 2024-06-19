using System;
using DevExpress.XtraEditors;

namespace FaultTreeAnalysis.View.Ribbon.Start.Excel
{
    public partial class RenumberConflictView : XtraForm
   {
      public bool IsAuto { get; set; }

      public string KeyWorld { get; set; }

      public bool IsFront { get; set; }

      public string Result { get; set; }

      public RenumberConflictView(string ids)
      {
         InitializeComponent();
         this.Result = ids;

         this.Lc_0.Text = ids.Substring(0,ids.Length-3);
         this.Lc_1.Text = ids;

         this.Te_1.Text = ids.Substring(0, ids.Length - 3);
         this.Te_2.Text = "(2)";


         this.radioGroup1.SelectedIndexChanged += SelectedIndexChanged;

      }

      private void SelectedIndexChanged(object sender, EventArgs e)
      {
         this.ChangeKeyWorldPosition((sender as RadioGroup).SelectedIndex);
      }

      private void simpleButton1_Click(object sender, EventArgs e)
      {
         this.KeyWorld = this.Te_2.Text;
         this.IsAuto = this.checkEdit1.Checked;
         this.IsFront = this.radioGroup1.SelectedIndex == 0 ? false : true;
         this.Close();
      }

      private void ChangeKeyWorldPosition(int selectedIndex)
      {
         if (selectedIndex >= 0) this.Result = selectedIndex == 0 ? $"{this.Te_1.Text}{this.Te_2.Text}" : $"{ this.Te_2.Text}{ this.Te_1.Text}";
         this.Lc_1.Text = Result;
      }

      private void KeyWorldChanged(object sender, EventArgs e)
      {
         this.Te_2.Text = (sender as TextEdit).Text;
         this.ChangeKeyWorldPosition(this.radioGroup1.SelectedIndex);
      }

      private void NewValueChanged(object sender, EventArgs e)
      {
         this.Te_1.Text = (sender as TextEdit).Text;
         this.ChangeKeyWorldPosition(this.radioGroup1.SelectedIndex);
      }
   }
}