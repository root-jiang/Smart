using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting;
using FaultTreeAnalysis.Common;
using FaultTreeAnalysis.Model.Data;

namespace FaultTreeAnalysis.View.Ribbon.FTA.CCA
{
    public partial class ExportIcView : XtraForm
    {
        private string fileName;
        private DrawData drawData;

        public ExportIcView(string fileName, DrawData drawData)
        {
            InitializeComponent();
            this.fileName = fileName;
            this.drawData = drawData;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            new IcPrintingContent(this.printingSystem1, this.fileName, this.drawData);
        }
    }

    /// <summary>
    /// 包含了打印相关的设置信息，并打印
    /// </summary>
    public class IcPrintingContent : Link
    {
        private List<CutsetInfo> cutsetInfos;
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
        public IcPrintingContent(PrintingSystem ps, string fileName, DrawData drawData) : base(ps)
        {
            this.fileName = fileName;
            this.cutsetName = drawData.Identifier;
            this.cutsetInfos = new List<CutsetInfo>(drawData.Cutset.ListCutsets_Real.Count);
            List<DrawData> events = drawData.GetAllData(General.FtaProgram.CurrentSystem, true).Where(o => o.IsGateType == false).ToList();
            for (int i = 0; i < drawData.Cutset.ListCutsets_Real.Count; i++)
            {
                this.cutsetInfos.AddRange(drawData.Cutset.ListCutsets_Real[i].GetInfos(events));
            }
            this.Margins = new System.Drawing.Printing.Margins(20, 20, 20, 20);
            CreateDocument(ps);
        }

        /// <summary>
        /// 创建详细信息事件重载函数
        /// </summary>
        /// <param name="graph">打印图形对象</param>
        protected override void CreateDetail(BrickGraphics graph)
        {
            this.SetBrickGraphicsProperties(graph);
            base.CreateDetail(graph);

            var startHeight = 0f;
            for (int i = 1; i < this.cutsetInfos.Count; i++)
            {
                startHeight += this.CreateRow(graph, this.cutsetInfos[i], startHeight);
            }
        }

        /// <summary>
        /// 创建详细信息的表格列标题信息格式
        /// </summary>
        /// <param name="graph">打印图形对象</param>
        protected override void CreateDetailHeader(BrickGraphics graph)
        {
            this.SetBrickGraphicsProperties(graph);
            graph.DrawString("Event List", Color.Green, new RectangleF(this.leftOffset, this.topOffset, this.widths[0], 25), BorderSide.All);
            graph.DrawString("Probability", Color.Green, new RectangleF(this.leftOffset + this.widths[0], this.topOffset, this.widths[1], 25), BorderSide.All);
        }

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

        /// <summary>
        /// 设置打印区域的绘图对象属性
        /// </summary>
        /// <param name="graph">打印图形对象</param>
        private void SetBrickGraphicsProperties(BrickGraphics graph)
        {
            graph.StringFormat = graph.StringFormat.ChangeLineAlignment(StringAlignment.Center);
            graph.Font = graph.DefaultFont;
            graph.BackColor = Color.Transparent;
        }
    }
}