namespace FaultTreeAnalysis.Model.Data
{
   /// <summary>
   /// 重命名的配置信息
   /// </summary>
   public class RenumberConfig
   {
      /// <summary>
      /// 是否保存门配置
      /// </summary>
      public bool IsSaveGateConfig { get; set; }

      /// <summary>
      /// 是否保存事件配置
      /// </summary>
      public bool IsSaveEventConfig { get; set; }

      /// <summary>
      /// 门起始号码
      /// </summary>
      public int GateStartNumber { get; set; } = 1;

      /// <summary>
      /// 事件起始号码
      /// </summary>
      public int EventStartNumber { get; set; } = 1;
   }
}
