using FaultTreeAnalysis.Model.Data;

namespace FaultTreeAnalysis.View.Ribbon
{
    public class FtaRibbon
    {
        private ProgramModel programModel;
        public RibbionEvents RibbionEvents { get; set; }
        public AnalysisNew analysisNew = null; 

        public FtaRibbon(ProgramModel programModel)
        {
            this.programModel = programModel;
            this.RibbionEvents = new RibbionEvents(this.programModel);
            analysisNew = RibbionEvents.analysisNew; 
        }
    }
}
