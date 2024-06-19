namespace FaultTreeAnalysis.Model.Data
{
    public class CutsetInfo
    {
        public string EventNames { get; set; }

        public string Probability { get; set; }

        public CutsetInfo(string name, string probability)
        {
            this.EventNames = name;
            this.Probability = probability;
        }
    }
}
