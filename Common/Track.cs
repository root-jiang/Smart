using System.Diagnostics;
using System.Linq;

namespace FaultTreeAnalysis.Common
{
    public class Track
   {
      public string[] PrintMessage(string exception)
      {
         StackTrace stackTrace = new StackTrace(new StackFrame(2, true));
         var stackFrame = stackTrace.GetFrames().Last();
         return new string[] 
         {
            stackFrame.GetFileName(),
            stackFrame.GetMethod().Name,
            stackFrame.GetFileLineNumber().ToString(),
            stackFrame.GetFileColumnNumber().ToString(),
            exception
         };
      }
   }
}
