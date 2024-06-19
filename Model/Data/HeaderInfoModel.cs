namespace FaultTreeAnalysis.Model.Data
{
   public class HeaderInfoModel
   {
      public string Name { get; set; }

      public string Caption { get; set; }

      public bool Visible { get; set; }

      public int Index { get; set; }

      public HeaderInfoModel(string name, string caption, int index, bool visible)
      {
         this.ChangeProperty(name, caption, index, visible);
      }

      public void ChangeProperty(string name, string caption, int index, bool visible)
      {
         this.Name = name;
         this.Index = index;
         this.Caption = caption;
         this.Visible = visible;
      }
   }
}
