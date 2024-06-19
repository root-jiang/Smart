using System;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.XtraPrinting;
using FaultTreeAnalysis.Model.Data;
using System.Linq;
using FaultTreeAnalysis.Common;

namespace FaultTreeAnalysis.View.Ribbon.FTA.CCA
{
    public partial class ExportMclView : DevExpress.XtraEditors.XtraForm
    {
        private string fileName;
        private DrawData drawData;

        public ExportMclView(string fileName, DrawData drawData)
        {
            InitializeComponent();
            this.fileName = fileName;
            this.drawData = drawData;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            new MclPrintingContent(printingSystem1, this.fileName, this.drawData);
        }
    }


    /// <summary>
    /// 包含了打印相关的设置信息，并打印
    /// </summary>
    public class MclPrintingContent : Link
    {
        private List<CutsetInfo> listCutsets;
        protected float topOffset = 10f;
        protected float leftOffset = 10f;
        private List<float> widths = new List<float> { 580f, 180f };
        private string fileName;
        private string cutsetName;
        /// <summary>
        /// 保存要打印的表对象
        /// </summary>
        /// <param name="ps">打印系统对象</param>
        /// <param name="workbook">工作簿（数据对象）</param>
        public MclPrintingContent(PrintingSystem ps, string fileName, DrawData drawData) : base(ps)
        {
            this.fileName = fileName;
            this.cutsetName = drawData.Identifier;
            this.listCutsets = new List<CutsetInfo>(drawData.Cutset.ListCutsets_Real.Count);
            List<DrawData> events = drawData.GetAllData(General.FtaProgram.CurrentSystem, true).Where(o => o.IsGateType == false).ToList();
            for (int i = 0; i < drawData.Cutset.ListCutsets_Real.Count; i++)
            {
                this.listCutsets.Add(drawData.Cutset.ListCutsets_Real[i].GetInfo(events));
            }

            this.Margins = new System.Drawing.Printing.Margins(20, 20, 20, 20);
            CreateDocument(ps);
        }

        //protected override void CreateReportHeader(BrickGraphics graph)
        //{
        //    base.CreateReportHeader(graph);
        //    this.SetGraphFont(graph, 12);

        //    PageInfoBrick brick = graph.DrawPageInfo(PageInfo.Number, "Page#   {0}", Color.Black, new Rectangle(630, 80, 0, graph.Font.Height), BorderSide.None);
        //    brick.Alignment = BrickAlignment.Far;
        //    brick.AutoWidth = true;

        //    graph.DrawString("Date:", Color.Blue, new RectangleF(630, 5, 50, graph.Font.Height + 2), BorderSide.None);
        //    graph.DrawString(DateTime.Now.ToShortDateString(), Color.Black, new RectangleF(680, 5, 200, graph.Font.Height + 2), BorderSide.None);

        //    graph.DrawString("Time:", Color.Blue, new RectangleF(630, 30, 50, graph.Font.Height + 2), BorderSide.None);
        //    graph.DrawString(DateTime.Now.ToShortTimeString(), Color.Black, new RectangleF(680, 30, 200, graph.Font.Height + 2), BorderSide.None);

        //    graph.DrawString("File Name:", Color.Blue, new RectangleF(10, 60, 100, 18), BorderSide.None);
        //    SizeF valueSize = graph.MeasureString(this.fileName, 200);
        //    graph.DrawString(this.fileName, Color.Black, new RectangleF(100, 60, valueSize.Width, valueSize.Height + 2), BorderSide.None);

        //    graph.DrawString("Cut Set List for:", Color.Blue, new RectangleF(10, 70 + valueSize.Height, 130, 18), BorderSide.None);
        //    graph.DrawString(this.cutsetName, Color.Black, new RectangleF(130, 70 + valueSize.Height, valueSize.Width, valueSize.Height + 2), BorderSide.None);

        //    this.SetGraphFont(graph, 15);
        //    graph.DrawString("Fault Tree Cut Set Report", Color.Blue, new RectangleF(300, 15, 300, 18), BorderSide.None);

        //}

        /// <summary>
        /// 创建每一行的信息
        /// </summary>
        /// <param name="graph">打印图形对象</param>
        /// <param name="row">数据行</param>
        /// <param name="rowHeight">行高</param>
        /// <returns></returns>
        private float CreateRow(BrickGraphics graph, CutsetInfo info, float rowHeight)
        {
            var widths = new List<float>(this.widths.ToArray());
            var valueSize = graph.MeasureString(info.EventNames, (int)widths[0]);
            graph.DrawString(info.EventNames, Color.Black, new RectangleF(this.leftOffset, rowHeight, widths[0], valueSize.Height + 16), BorderSide.All);
            graph.DrawString(info.Probability, Color.Black, new RectangleF(this.leftOffset + widths[0], rowHeight, widths[1], valueSize.Height + 16), BorderSide.All);
            return valueSize.Height + 16;
        }
    }

}
