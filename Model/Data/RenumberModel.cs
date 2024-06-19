using FaultTreeAnalysis.Common;
using FaultTreeAnalysis.Model.Enum;
using System;
using System.Collections.Generic;

namespace FaultTreeAnalysis.Model.Data
{
    public class RenumberModel
   {
      private readonly int StartNumberMax = 1073741824;

      public string Name { get; set; }

      public string GatePrefix { get; set; } = "Gate";

      public string GateStartNumber { get; set; } = "1";

      public string GateMinNumber { get; set; } = "1";

      public string GateSuffix { get; set; } = string.Empty;

      public RenumberedType RenumberedType { get; private set; }

      public string RenumberedTypeString
      {
         get
         {
            var name = System.Enum.GetName(typeof(RenumberedType), this.RenumberedType);
            return General.GetValueName(name);
         }
         set
         {
            var typeName = General.GetKeyName(value);
            this.RenumberedType = General.GetEnumByName<RenumberedType>(typeName);
         }
      }

      public string EventPrefix { get; set; } = "Event";

      public string EventStartNumber { get; set; } = "0";

      public string EventMinNumber { get; set; } = "1";

      public string EventSuffix { get; set; } = string.Empty;

      public bool AccordingToGate { get; set; }

      /// <summary>
      /// 通过起始数字和长度参数计算输出的固定长度的数字字符串
      /// </summary>
      /// <param name="startNumber"></param>
      /// <param name="bit"></param>
      /// <returns></returns>
      private string GetStartNumberFormatString(int startNumber, decimal bit)
      {
         var result = string.Empty;
         if (startNumber > this.StartNumberMax) startNumber = this.StartNumberMax;
         else if (startNumber < 0) startNumber = 0;
         var _number = startNumber.ToString();
         var length = bit - _number.Length > 0 ? bit : _number.Length;
         var filler = string.Empty.PadLeft((int)length, '0');
         result = startNumber.ToString(filler);
         return result;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="count"></param>
      /// <returns></returns>
      public List<string> GetSerialNames(int count)
      {
         var result = new List<string>(count);
         for (int i = 0; i < count; i++)
         {
            result.Add($"{this.GatePrefix}{this.GetStartNumberFormatString(Convert.ToInt32(this.GateStartNumber) + i, 1)}{this.GateSuffix}");
         }
         return result;
      }
   }
}
