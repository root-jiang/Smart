using System;

namespace FaultTreeAnalysis.Model.Enum
{
   /// <summary>
   /// 重命名故障树时，命名的对象类型（门，事件，都要）
   /// </summary>
   [Flags]
   public enum RenumberedType
   {
      OnlyGate = 0,
      OnlyEvent = 1,
      BothGateAndEvent = 2
   }

   public enum RenumberedRange
   {
      AllSystem = 0,
      SelectedTree = 1,
      SelectedTreeAndTransfer = 2
   }
}
