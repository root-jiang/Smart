using Aspose.Cells;
using Aspose.Pdf.Generator;
using DevExpress.Diagram.Core;
using DevExpress.Utils;
using DevExpress.XtraDiagram;
using DevExpress.XtraSplashScreen;
using FaultTreeAnalysis.Common;
using FaultTreeAnalysis.Model.Enum;
using FaultTreeAnalysis.View.Ribbon;
using FaultTreeAnalysis.View.Ribbon.FTA.CCA;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using Aspose.Words;

namespace FaultTreeAnalysis.Model.Data
{
    public class CCAFunction
    {
        /// <summary>
        /// 画笔宽度
        /// </summary>
        public readonly int PEN_WIDTH = 1;

        /// <summary>
        /// 图形宽度
        /// </summary>
        public readonly int SHAPE_WIDTH = 100;

        /// <summary>
        /// 图形高度
        /// </summary>
        public readonly int SHAPE_HEIGHT = 100;

        /// <summary>
        /// 2图形间左上角X相距
        /// </summary>
        public readonly int DISTANCEX = 140;

        /// <summary>
        /// 2图形间左上角Y相距
        /// </summary>
        public readonly int DISTANCEY = 140;


        /// <summary>
        /// 割级报告
        /// </summary>
        /// <param name="drawData"></param>
        /// <param name="isExcelReport"></param>
        public void MinimalCutsetList(SystemModel sys, DrawData drawData, string Path, bool isExcelReport = true)
        {
            if (Path == "")
            {
                return;
            }
            if (drawData != null && drawData.Children?.Count > 0)
            {
                if (isExcelReport)
                {
                    var path = this.SaveMCLToExcel(sys, drawData, Path);
                }
                else this.PreviewMcl(sys, drawData);
            }
        }

        public void SetHeaderFooterToFTA(Aspose.Pdf.Generator.Pdf pdf1, Aspose.Pdf.Generator.Section sec1, int Mode, string txt, string FileName = "", string Topevent = "", string EventNum = "", string GateNum = "")
        {
            try
            {
                if (Mode == 0)
                {
                    //the header or footer is to be added
                    Aspose.Pdf.Generator.HeaderFooter hf1 = new Aspose.Pdf.Generator.HeaderFooter(sec1);
                    hf1.TextInfo.FontName = "SimSun";
                    //Set the header of odd pages of the PDF document
                    sec1.OddHeader = hf1;
                    //Set the header of even pages of the PDF document
                    sec1.EvenHeader = hf1;
                    //Enable this header for first page only
                    hf1.IsFirstPageOnly = false;
                    //Add Distance From Edge Property to 0 unit Points
                    hf1.DistanceFromEdge = 40;
                    //Set the First HeaderFooter, top and bottom property respectively
                    hf1.Margin.Top = 5;
                    hf1.Margin.Bottom = 10;
                    hf1.Margin.Left = 1;
                    hf1.Margin.Right = 1;

                    Aspose.Pdf.Generator.Text textTile = new Aspose.Pdf.Generator.Text(hf1, "                                                                  " + "Fault Tree Cut Set Report" + "            " + "Date:" + DateTime.Now.ToShortDateString().ToString());
                    hf1.Paragraphs.Add(textTile);

                    Aspose.Pdf.Generator.Text text = new Aspose.Pdf.Generator.Text(hf1, "                                                                                                                                                                      Time:" + DateTime.Now.ToLongTimeString().ToString());
                    hf1.Paragraphs.Add(text);

                    Aspose.Pdf.Generator.Text textFile = new Aspose.Pdf.Generator.Text(hf1, "File Name:" + FileName);
                    hf1.Paragraphs.Add(textFile);
                    Aspose.Pdf.Generator.Text textTopevent = new Aspose.Pdf.Generator.Text(hf1, "Cut Set List For:" + Topevent + "                                                                                                                                  " + "Page: $p / $P ");
                    hf1.Paragraphs.Add(textTopevent);
                }
                else
                {
                    //the header or footer is to be added
                    Aspose.Pdf.Generator.HeaderFooter hf1 = new Aspose.Pdf.Generator.HeaderFooter(sec1);
                    hf1.TextInfo.FontName = "SimSun";
                    //Set the header of odd pages of the PDF document
                    sec1.OddFooter = hf1;
                    //Set the header of even pages of the PDF document
                    sec1.EvenFooter = hf1;
                    //Enable this header for first page only
                    hf1.IsFirstPageOnly = false;
                    //Add Distance From Edge Property to 0 unit Points
                    hf1.DistanceFromEdge = 0;
                    //Set the First HeaderFooter, top and bottom property respectively
                    hf1.Margin.Top = 10;
                    hf1.Margin.Bottom = 10;
                    hf1.Margin.Left = 10;
                    hf1.Margin.Right = 10;

                    Aspose.Pdf.Generator.Text text = new Aspose.Pdf.Generator.Text(hf1, txt);
                    hf1.Paragraphs.Add(text);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void SetHeaderFooter(bool isOnlyDiagram, Aspose.Pdf.Generator.Pdf pdf1, Aspose.Pdf.Generator.Section sec1, int Mode, string txt, string FileName = "", string Topevent = "", string EventNum = "", string GateNum = "")
        {
            try
            {
                if (Mode == 0)
                {
                    //the header or footer is to be added
                    Aspose.Pdf.Generator.HeaderFooter hf1 = new Aspose.Pdf.Generator.HeaderFooter(sec1);
                    hf1.TextInfo.FontName = "SimSun";
                    //Set the header of odd pages of the PDF document
                    sec1.OddHeader = hf1;
                    //Set the header of even pages of the PDF document
                    sec1.EvenHeader = hf1;
                    //Enable this header for first page only
                    hf1.IsFirstPageOnly = false;
                    //Add Distance From Edge Property to 0 unit Points
                    hf1.DistanceFromEdge = 20;
                    //Set the First HeaderFooter, top and bottom property respectively
                    hf1.Margin.Top = 5;
                    hf1.Margin.Bottom = 10;
                    hf1.Margin.Left = 3;
                    hf1.Margin.Right = 3;
                    //hf1.TextInfo.Color = new Aspose.Pdf.Generator.Color(0, 0, 255);

                    if (isOnlyDiagram)
                    {
                        Aspose.Pdf.Generator.Text textTile = new Aspose.Pdf.Generator.Text(hf1, "Date:" + DateTime.Now.ToShortDateString().ToString());
                        hf1.Paragraphs.Add(textTile);
                    }
                    else
                    {
                        Aspose.Pdf.Generator.Text textTile = new Aspose.Pdf.Generator.Text(hf1, "                                                                            " + "Fault Tree Report" + "                                                        " + "Date:" + DateTime.Now.ToShortDateString().ToString());
                        hf1.Paragraphs.Add(textTile);
                    }

                    Aspose.Pdf.Generator.Text text = new Aspose.Pdf.Generator.Text(hf1, txt + "Time:" + DateTime.Now.ToLongTimeString().ToString());
                    hf1.Paragraphs.Add(text);

                    Aspose.Pdf.Generator.Text textFile = new Aspose.Pdf.Generator.Text(hf1, "File Name:" + FileName);
                    hf1.Paragraphs.Add(textFile);
                    Aspose.Pdf.Generator.Text textTopevent = new Aspose.Pdf.Generator.Text(hf1, "Topevent Num:" + Topevent + "                                                                                                                     " + "Page: $p / $P ");
                    hf1.Paragraphs.Add(textTopevent);
                    Aspose.Pdf.Generator.Text textEvent = new Aspose.Pdf.Generator.Text(hf1, "Event Num:" + EventNum + "     " + "Gate Num:" + GateNum);
                    hf1.Paragraphs.Add(textEvent);
                }
                else
                {
                    //the header or footer is to be added
                    Aspose.Pdf.Generator.HeaderFooter hf1 = new Aspose.Pdf.Generator.HeaderFooter(sec1);
                    hf1.TextInfo.FontName = "SimSun";
                    //Set the header of odd pages of the PDF document
                    sec1.OddFooter = hf1;
                    //Set the header of even pages of the PDF document
                    sec1.EvenFooter = hf1;
                    //Enable this header for first page only
                    hf1.IsFirstPageOnly = false;
                    //Add Distance From Edge Property to 0 unit Points
                    hf1.DistanceFromEdge = 5;
                    //Set the First HeaderFooter, top and bottom property respectively
                    hf1.Margin.Top = 10;
                    hf1.Margin.Bottom = 10;
                    hf1.Margin.Left = 10;
                    hf1.Margin.Right = 10;
                    //hf1.TextInfo.Color = new Aspose.Pdf.Generator.Color(0, 0, 0);

                    Aspose.Pdf.Generator.Text text = new Aspose.Pdf.Generator.Text(hf1, txt);
                    hf1.Paragraphs.Add(text);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public double ConvertE(string a)
        {
            double result = -1;
            try
            {
                if (a.ToUpper().Contains("E"))
                {
                    double b = double.Parse(a.ToUpper().Split('E')[0].ToString());//整数部分
                    double c = double.Parse(a.ToUpper().Split('E')[1].ToString());//指数部分
                    result = b * Math.Pow(10, c);
                }
                else
                {
                    result = -1;
                }
            }
            catch (Exception)
            {
                result = -1;
            }
            return result;
        }

        public void SetSection(Aspose.Pdf.Generator.Section sec1, float PageWidth, float PageHeight)
        {
            sec1.PageInfo.PageWidth = PageWidth;
            sec1.PageInfo.PageHeight = PageHeight;

            Aspose.Pdf.Generator.MarginInfo marginInfo = new Aspose.Pdf.Generator.MarginInfo();

            marginInfo.Top = 5;
            marginInfo.Bottom = 5;
            marginInfo.Left = 10;
            marginInfo.Right = 10;

            sec1.PageInfo.Margin = marginInfo;
        }

        //记录所有分页
        public int PageNumID = 0;
        public Dictionary<int, DrawData> AllTransData = new Dictionary<int, DrawData>();
        public string TopName = "";
        public int EventN = 0;
        public int GateN = 0;
        public Dictionary<DrawData, System.Drawing.Point> AllTransDataPoint = new Dictionary<DrawData, System.Drawing.Point>();
        public bool FTARpt(SystemModel sys, DrawData drawData, string result)
        {
            try
            {
                string RootTag = "";
                //RootTag = "Shanghai Avionics Corporation Proprietary Information" + "\r\n";
                if (result == "")
                {
                    if (SplashScreenManager.Default != null && SplashScreenManager.Default.IsSplashFormVisible)
                    {
                        SplashScreenManager.CloseDefaultWaitForm();
                    }
                    return false;
                }

                SplashScreenManager.ShowDefaultWaitForm(General.FtaProgram.String.Export);
                Thread.Sleep(200);

                if (Directory.Exists(Environment.CurrentDirectory + "\\img_temp"))
                {
                    Directory.Delete(Environment.CurrentDirectory + "\\img_temp", true);
                    Directory.CreateDirectory(Environment.CurrentDirectory + "\\img_temp");
                }
                else
                {
                    Directory.CreateDirectory(Environment.CurrentDirectory + "\\img_temp");
                }

                if (drawData == null)
                {
                    if (SplashScreenManager.Default.IsSplashFormVisible)
                    {
                        SplashScreenManager.CloseDefaultWaitForm();
                    }
                    return false;
                }
                //先复制一份数据
                EventN = 0;
                GateN = 1;
                TopName = drawData.Identifier;
                DrawData ReportData = drawData.CopyDrawDataRecurse();
                SolAllDataNum(ReportData);

                //页眉页脚
                //Create a Section object by calling Add method of Sections collection of Pdf class
                Aspose.Pdf.Generator.Pdf pdf1 = new Aspose.Pdf.Generator.Pdf();
                Aspose.Pdf.Generator.Section sec1 = pdf1.Sections.Add();
                SetSection(sec1, Aspose.Pdf.Generator.PageSize.A4Width, Aspose.Pdf.Generator.PageSize.A4Height);

                //假设画布大小为MaxSize 
                Size MaxSize = new Size(Convert.ToInt32(Math.Floor(sec1.PageInfo.PageWidth)), Convert.ToInt32(Math.Floor(sec1.PageInfo.PageHeight)));
                int NewWidth = MaxSize.Width - 30;
                int NewHeight = MaxSize.Height - 50;
                MaxSize = new Size(NewWidth, NewHeight);

                int TablePageCount = 0;
                for (int index = 0; index < 2; index++)
                {
                    pdf1.Sections.Clear();
                    //表格导出 
                    //表1（Fault Tree Gates/Events Table）
                    Aspose.Pdf.Generator.Section sec2 = pdf1.Sections.Add();
                    SetSection(sec2, Aspose.Pdf.Generator.PageSize.A4Width, Aspose.Pdf.Generator.PageSize.A4Height);

                    SetHeaderFooter(false, pdf1, sec2, 0, "                                                                    " + "Fault Tree Gates/Events Table" + "                                            ", sys.SystemName, TopName, EventN.ToString(), GateN.ToString());
                    SetHeaderFooter(false, pdf1, sec2, 1, RootTag + "Use or disclosure of data contained on this sheet is subject to the restrictions on the cover of the first page.");

                    //Create a table
                    Aspose.Pdf.Generator.Table tab1 = new Aspose.Pdf.Generator.Table();
                    //Add the table into the paragraphs collection of section
                    sec2.Paragraphs.Add(tab1);
                    //Set the column widths of the table
                    tab1.ColumnWidths = "100 200 80 100 100";
                    //Set the default cell border using BorderInfo instance
                    tab1.DefaultCellBorder = new Aspose.Pdf.Generator.BorderInfo((int)Aspose.Pdf.Generator.BorderSide.All);

                    Aspose.Pdf.Generator.Row row1 = tab1.Rows.Add();
                    row1.Cells.Add("Gate Lable");
                    row1.Cells.Add("Description");
                    row1.Cells.Add("Type");
                    row1.Cells.Add("Probability");
                    row1.Cells.Add("Exposure Time");
                    List<DrawData> AllData = sys.GetAllDatas();
                    foreach (DrawData ChildData in AllData)
                    {
                        TextInfo TxInfo = new TextInfo();
                        TxInfo.FontName = "SimSun";
                        Aspose.Pdf.Generator.Row row2 = tab1.Rows.Add();
                        row2.Cells.Add(ChildData.Identifier, TxInfo);
                        row2.Cells.Add(ChildData.Comment1, TxInfo);
                        row2.Cells.Add(ChildData.Type.ToString(), TxInfo);
                        row2.Cells.Add(ChildData.InputValue, TxInfo);
                        row2.Cells.Add(ChildData.InputValue2, TxInfo);
                    }

                    //表2（Fault Tree Calculation Results）
                    Aspose.Pdf.Generator.Section sec3 = pdf1.Sections.Add();
                    SetSection(sec3, Aspose.Pdf.Generator.PageSize.A4Width, Aspose.Pdf.Generator.PageSize.A4Height);

                    SetHeaderFooter(false, pdf1, sec3, 0, "                                                                    " + "Fault Tree Calculation Results" + "                                            ", sys.SystemName, TopName, EventN.ToString(), GateN.ToString());
                    SetHeaderFooter(false, pdf1, sec3, 1, RootTag + "Use or disclosure of data contained on this sheet is subject to the restrictions on the cover of the first page.");

                    //Create a table
                    Aspose.Pdf.Generator.Table tab2 = new Aspose.Pdf.Generator.Table();
                    //Add the table into the paragraphs collection of section
                    sec3.Paragraphs.Add(tab2);
                    //Set the column widths of the table
                    tab2.ColumnWidths = "110 100 100 260";
                    //Set the default cell border using BorderInfo instance
                    tab2.DefaultCellBorder = new Aspose.Pdf.Generator.BorderInfo((int)Aspose.Pdf.Generator.BorderSide.All);

                    Aspose.Pdf.Generator.Row row3 = tab2.Rows.Add();
                    row3.Cells.Add("Gate Lable");
                    row3.Cells.Add("Probability");
                    row3.Cells.Add("Exposure Time");
                    row3.Cells.Add("Description");

                    AllData.Sort((x, y) => -ConvertE(x.InputValue).CompareTo(ConvertE(y.InputValue)));//降序

                    foreach (DrawData ChildData in AllData)
                    {
                        TextInfo TxInfo = new TextInfo();
                        TxInfo.FontName = "SimSun";
                        Aspose.Pdf.Generator.Row row4 = tab2.Rows.Add();
                        row4.Cells.Add(ChildData.Identifier, TxInfo);
                        row4.Cells.Add(ChildData.InputValue, TxInfo);
                        row4.Cells.Add(ChildData.InputValue2, TxInfo);
                        row4.Cells.Add(ChildData.Comment1, TxInfo);
                    }

                    //图形导出
                    AllTransData.Clear();
                    AllTransDataPoint.Clear();
                    //先记录这个顶以及页码编号
                    PageNumID = 0;
                    AllTransData.Add(PageNumID, ReportData);
                    AllTransDataPoint.Add(ReportData, GetNowPageLocation(ReportData));

                    if (index == 0)
                    {
                        pdf1.Save(result);
                        TablePageCount = pdf1.PageCount;
                    }
                }

                //图形分页拆分成图片导出
                ReportData.ApplyTreeLayout(150, 150, 200, 200, 20, 20);
                SolAllData(ReportData, new Size(NewWidth, NewHeight), TablePageCount + 1);

                List<FileStream> Ims = new List<FileStream>();
                //生成图并导出成图片
                foreach (int Key in AllTransData.Keys)
                {
                    DrawData data = AllTransData[Key];
                    //重新排布 
                    data.ApplyTreeLayout(150, 150, 200, 200, 20, 20);

                    List<DiagramItem> shapes = GenerateDiagramItems(data);

                    DiagramControl diagramControl_FTA = new DiagramControl();

                    diagramControl_FTA.OptionsView.PageSize = new SizeF(NewWidth, NewHeight);

                    diagramControl_FTA.CustomDrawItem += new System.EventHandler<DevExpress.XtraDiagram.CustomDrawItemEventArgs>(this.diagramControl_FTA_CustomDrawItem);

                    diagramControl_FTA.Items.AddRange(shapes.ToArray());

                    diagramControl_FTA.FitToDrawing();
                    diagramControl_FTA.Refresh();

                    float MinPG = 10000;
                    float MaxPG = -1;
                    foreach (DiagramItem di in shapes)
                    {
                        if (di.X > MaxPG)
                        {
                            MaxPG = di.X;
                        }
                        if (di.X < MinPG)
                        {
                            MinPG = di.X;
                        }
                    }

                    float BL = (float)1;
                    if (MaxPG + 150 - MinPG - sec1.PageInfo.PageWidth - 130 > 0)
                    {
                        BL = (float)sec1.PageInfo.PageWidth / (MaxPG + 150 - MinPG - 130);
                    }

                    FileStream fs = new FileStream(Environment.CurrentDirectory + "\\img_temp\\tmp" + Key.ToString() + ".png", FileMode.OpenOrCreate);
                    diagramControl_FTA.ExportDiagram(fs, new DevExpress.Diagram.Core.DiagramExportFormat(), 150, BL);
                    Ims.Add(fs);
                }

                //open document
                foreach (FileStream imageStream in Ims)
                {
                    Aspose.Pdf.Generator.Section sec = pdf1.Sections.Add();

                    SetSection(sec, Aspose.Pdf.Generator.PageSize.A4Width, Aspose.Pdf.Generator.PageSize.A4Height);

                    SetHeaderFooter(false, pdf1, sec, 0, "                                                                           " + "Fault Tree Diagram" + "                                                       ", sys.SystemName, TopName, EventN.ToString(), GateN.ToString());
                    SetHeaderFooter(false, pdf1, sec, 1, RootTag + "Use or disclosure of data contained on this sheet is subject to the restrictions on the cover of the first page.");

                    //Create an image object
                    Aspose.Pdf.Generator.Image image1 = new Aspose.Pdf.Generator.Image(sec);
                    //Add the image into paragraphs collection of the section
                    sec.Paragraphs.Add(image1);
                    image1.ImageInfo.ImageFileType = Aspose.Pdf.Generator.ImageFileType.Bmp;
                    //Set the ImageStream to a MemoryStream object
                    image1.ImageInfo.ImageStream = imageStream;
                    //Set desired the image scale 
                    image1.ImageInfo.Alignment = AlignmentType.Center;
                    image1.ImageInfo.Title = "";

                    image1.ImageInfo.ImageBorder = new Aspose.Pdf.Generator.BorderInfo((int)Aspose.Pdf.BorderSide.None, 0.5F, new Aspose.Pdf.Generator.Color("Black"));
                }

                //save updated document 
                pdf1.SetUnicode();
                pdf1.Save(result);

                foreach (FileStream imageStream in Ims)
                {
                    imageStream.Close();
                    imageStream.Dispose();
                }

                if (Directory.Exists(Environment.CurrentDirectory + "\\img_temp"))
                {
                    Directory.Delete(Environment.CurrentDirectory + "\\img_temp", true);
                }

                //导出word
                Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(result);
                string NewFile = new FileInfo(result).FullName.Replace(new FileInfo(result).Extension, ".docx");
                pdfDocument.Save(NewFile, Aspose.Pdf.SaveFormat.DocX);

                if (SplashScreenManager.Default != null)
                {
                    if (SplashScreenManager.Default.IsSplashFormVisible)
                    {
                        SplashScreenManager.CloseDefaultWaitForm();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                if (SplashScreenManager.Default != null)
                {
                    if (SplashScreenManager.Default.IsSplashFormVisible)
                    {
                        SplashScreenManager.CloseDefaultWaitForm();
                    }
                }
                MsgBox.Show(ex.Message);
                return false;
            }
        }

        public bool FTARpt_OnlyDiagram(SystemModel sys, DrawData drawData, string result)
        {
            try
            {
                if (result == "")
                {
                    if (SplashScreenManager.Default != null && SplashScreenManager.Default.IsSplashFormVisible)
                    {
                        SplashScreenManager.CloseDefaultWaitForm();
                    }
                    return false;
                }

                SplashScreenManager.ShowDefaultWaitForm(General.FtaProgram.String.Export);
                Thread.Sleep(200);

                if (Directory.Exists(Environment.CurrentDirectory + "\\img_temp"))
                {
                    Directory.Delete(Environment.CurrentDirectory + "\\img_temp", true);
                    Directory.CreateDirectory(Environment.CurrentDirectory + "\\img_temp");
                }
                else
                {
                    Directory.CreateDirectory(Environment.CurrentDirectory + "\\img_temp");
                }

                if (drawData == null)
                {
                    if (SplashScreenManager.Default.IsSplashFormVisible)
                    {
                        SplashScreenManager.CloseDefaultWaitForm();
                    }
                    return false;
                }
                //先复制一份数据
                EventN = 0;
                GateN = 1;
                TopName = drawData.Identifier;
                DrawData ReportData = drawData.CopyDrawDataRecurse();
                SolAllDataNum(ReportData);

                //页眉页脚
                //Create a Section object by calling Add method of Sections collection of Pdf class
                Aspose.Pdf.Generator.Pdf pdf1 = new Aspose.Pdf.Generator.Pdf();
                Aspose.Pdf.Generator.Section sec1 = pdf1.Sections.Add();
                SetSection(sec1, Aspose.Pdf.Generator.PageSize.A4Width, Aspose.Pdf.Generator.PageSize.A4Height);

                //假设画布大小为MaxSize 
                Size MaxSize = new Size(Convert.ToInt32(Math.Floor(sec1.PageInfo.PageWidth)), Convert.ToInt32(Math.Floor(sec1.PageInfo.PageHeight)));
                int NewWidth = MaxSize.Width - 30;
                int NewHeight = MaxSize.Height - 50;
                MaxSize = new Size(NewWidth, NewHeight);

                int TablePageCount = 0;
                for (int index = 0; index < 2; index++)
                {
                    pdf1.Sections.Clear();

                    //图形导出
                    AllTransData.Clear();
                    AllTransDataPoint.Clear();
                    //先记录这个顶以及页码编号
                    PageNumID = 0;
                    AllTransData.Add(PageNumID, ReportData);
                    AllTransDataPoint.Add(ReportData, GetNowPageLocation(ReportData));

                    if (index == 0)
                    {
                        pdf1.Save(result);
                        TablePageCount = pdf1.PageCount;
                    }
                }

                //图形分页拆分成图片导出
                ReportData.ApplyTreeLayout(150, 150, 200, 200, 20, 20);
                SolAllData(ReportData, new Size(NewWidth, NewHeight), TablePageCount + 1);

                List<FileStream> Ims = new List<FileStream>();
                //生成图并导出成图片
                foreach (int Key in AllTransData.Keys)
                {
                    DrawData data = AllTransData[Key];
                    //重新排布 
                    data.ApplyTreeLayout(150, 150, 200, 200, 20, 20);

                    List<DiagramItem> shapes = GenerateDiagramItems(data);

                    DiagramControl diagramControl_FTA = new DiagramControl();

                    diagramControl_FTA.OptionsView.PageSize = new SizeF(NewWidth, NewHeight);

                    diagramControl_FTA.CustomDrawItem += new System.EventHandler<DevExpress.XtraDiagram.CustomDrawItemEventArgs>(this.diagramControl_FTA_CustomDrawItem);

                    diagramControl_FTA.Items.AddRange(shapes.ToArray());

                    diagramControl_FTA.FitToDrawing();
                    diagramControl_FTA.Refresh();

                    float MinPG = 10000;
                    float MaxPG = -1;
                    foreach (DiagramItem di in shapes)
                    {
                        if (di.X > MaxPG)
                        {
                            MaxPG = di.X;
                        }
                        if (di.X < MinPG)
                        {
                            MinPG = di.X;
                        }
                    }

                    float BL = (float)1;
                    if (MaxPG + 150 - MinPG - sec1.PageInfo.PageWidth - 130 > 0)
                    {
                        BL = (float)sec1.PageInfo.PageWidth / (MaxPG + 150 - MinPG - 130);
                    }

                    FileStream fs = new FileStream(Environment.CurrentDirectory + "\\img_temp\\tmp" + Key.ToString() + ".png", FileMode.OpenOrCreate);
                    diagramControl_FTA.ExportDiagram(fs, new DevExpress.Diagram.Core.DiagramExportFormat(), 150, BL);
                    Ims.Add(fs);
                }

                //open document
                foreach (FileStream imageStream in Ims)
                {
                    Aspose.Pdf.Generator.Section sec = pdf1.Sections.Add();

                    SetSection(sec, Aspose.Pdf.Generator.PageSize.A4Width, Aspose.Pdf.Generator.PageSize.A4Height);

                    SetHeaderFooter(true, pdf1, sec, 0, "                                                                           " + "Fault Tree Diagram" + "                                                       ", sys.SystemName, TopName, EventN.ToString(), GateN.ToString());

                    //Create an image object
                    Aspose.Pdf.Generator.Image image1 = new Aspose.Pdf.Generator.Image(sec);
                    //Add the image into paragraphs collection of the section
                    sec.Paragraphs.Add(image1);
                    image1.ImageInfo.ImageFileType = Aspose.Pdf.Generator.ImageFileType.Bmp;
                    //Set the ImageStream to a MemoryStream object
                    image1.ImageInfo.ImageStream = imageStream;
                    //Set desired the image scale 
                    image1.ImageInfo.Alignment = AlignmentType.Center;
                    image1.ImageInfo.Title = "";

                    image1.ImageInfo.ImageBorder = new Aspose.Pdf.Generator.BorderInfo((int)Aspose.Pdf.BorderSide.None, 0.5F, new Aspose.Pdf.Generator.Color("Black"));
                }

                //save updated document 
                pdf1.SetUnicode();
                pdf1.Save(result);

                foreach (FileStream imageStream in Ims)
                {
                    imageStream.Close();
                    imageStream.Dispose();
                }

                if (Directory.Exists(Environment.CurrentDirectory + "\\img_temp"))
                {
                    Directory.Delete(Environment.CurrentDirectory + "\\img_temp", true);
                }

                //导出word
                Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(result);
                string NewFile = new FileInfo(result).FullName.Replace(new FileInfo(result).Extension, ".docx");
                pdfDocument.Save(NewFile, Aspose.Pdf.SaveFormat.DocX);

                if (SplashScreenManager.Default != null)
                {
                    if (SplashScreenManager.Default.IsSplashFormVisible)
                    {
                        SplashScreenManager.CloseDefaultWaitForm();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                if (SplashScreenManager.Default != null)
                {
                    if (SplashScreenManager.Default.IsSplashFormVisible)
                    {
                        SplashScreenManager.CloseDefaultWaitForm();
                    }
                }
                MsgBox.Show(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 重绘窗口图形
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void diagramControl_FTA_CustomDrawItem(object sender, DevExpress.XtraDiagram.CustomDrawItemEventArgs e)
        {
            try
            {
                DiagramControl diagramControl_FTA = (DiagramControl)sender;
                //绘制到画布上的基本图像，拖动时随鼠标暂时移动的半透明指示图像
                if (e.Context == DiagramDrawingContext.Canvas || e.Context == DiagramDrawingContext.DragPreview)
                {
                    //测试要绘制的图形是否在可视区域内
                    PointF start = diagramControl_FTA.DiagramViewInfo.RulersOffset;
                    float factor = diagramControl_FTA.DiagramViewInfo.ZoomFactor;
                    float width = diagramControl_FTA.DiagramViewInfo.ContentRect.Width;
                    float height = diagramControl_FTA.DiagramViewInfo.ContentRect.Height;
                    RectangleF rect = new RectangleF(-start.X / factor, -start.Y / factor, width / factor, height / factor);
                    RectangleF item_Rect = new RectangleF(e.Item.X, e.Item.Y, e.Item.Width, e.Item.Height);
                    if (!rect.IntersectsWith(item_Rect))
                        return;
                }
                if (e.Item != null && e.Item.GetType() == typeof(DiagramShape) && e.Item.Tag != null && e.Item.Tag.GetType() == typeof(DrawData))
                {
                    //自定义绘制的数据
                    DrawData data = e.Item.Tag as DrawData;
                    if (data.Type == DrawType.NULL) return;

                    if (data.QValue == "")
                    {
                        DrawBase.DrawComponent_New(data, data.Type, e.Graphics, 0, 0, e.Item.Width - General.PEN_WIDTH, data.Comment1, data.Identifier, data.ExtraValue11, data.Type == DrawType.TransferInGate ? true : false);
                    }
                    else
                    {
                        if (data.Type == DrawType.VotingGate)
                        {
                            DrawBase.DrawComponent_New(data, data.Type, e.Graphics, 0, 0, e.Item.Width - General.PEN_WIDTH, data.Comment1, data.Identifier, data.QValue + " M:" + data.ExtraValue1 + ":" + data.Children.Count.ToString(), data.Type == DrawType.TransferInGate ? true : false);
                        }
                        else
                        {
                            DrawBase.DrawComponent_New(data, data.Type, e.Graphics, 0, 0, e.Item.Width - General.PEN_WIDTH, data.Comment1, data.Identifier, data.QValue, data.Type == DrawType.TransferInGate ? true : false);
                        }
                    }

                    //选中的图形加一圈描边效果
                    if (diagramControl_FTA.SelectedItems.Count == 1 && diagramControl_FTA.SelectedItems[0] == e.Item)
                    {
                        using (Pen pen = new Pen(System.Drawing.Color.OrangeRed, 2))
                        {
                            e.Graphics.DrawRectangle(pen, new System.Drawing.Rectangle(-1, -1, (int)e.Size.Width + 1, (int)e.Size.Height + 1));
                        }
                    }
                    e.Handled = true;
                }
            }
            catch (System.Exception ex)
            {
                MsgBox.Show(FixedString.EXCEPTION + ex.Message);
            }
        }

        /// <summary>
        /// 获取当前页坐标
        /// </summary>
        /// <param name="TopData">当前页顶层节点</param>
        public System.Drawing.Point GetNowPageLocation(DrawData TopData)
        {
            try
            {
                //计算当前页坐标范围，X起点为第一个子节点,Y为当前顶节点
                int MinX = TopData.X;
                int MinY = TopData.Y;
                if (TopData.Children.Count > 0)
                {
                    MinX = TopData.Children.Select(t => t.X).ToList().Min();
                }
                return new System.Drawing.Point(MinX, MinY);
            }
            catch (Exception)
            {
                return new System.Drawing.Point(-1, -1);
            }
        }

        public DrawData NowTop = null;
        public void GetNowTop(DrawData NowChild)
        {
            if (NowChild.Parent != null)
            {
                GetNowTop(NowChild.Parent);
            }
            else
            {
                NowTop = NowChild;
            }
        }

        public void SolAllDataNum(DrawData TopData)
        {
            foreach (DrawData da in TopData.Children)
            {
                if (da.IsGateType)
                {
                    GateN += 1;
                }
                else if (da.Type.ToString().Contains("Event"))
                {
                    EventN += 1;
                }

                if (da.HasChild)
                {
                    SolAllDataNum(da);
                }
            }
        }

        /// <summary>
        /// 递归查询分页数据
        /// </summary>
        /// <param name="TopData"></param>
        /// <param name="MaxSize"></param>
        public void SolAllData(DrawData TopData, Size MaxSize, int PageCount)
        {
            try
            {
                //图形大小
                int ShapWidth = 100;
                int ShapHeight = 100;

                for (int i = 0; i < TopData.Children.Count; i++)
                {
                    try
                    {
                        if (TopData.Children[i].Identifier != "")
                        {
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    if (TopData.Children[i].Type == DrawType.TransferInGate)
                    {
                        continue;
                    }
                    System.Drawing.Point NowPageLocation = new System.Drawing.Point(-1, -1);

                    //求当前页最顶层节点
                    NowTop = null;
                    GetNowTop(TopData.Children[i]);
                    AllTransDataPoint.TryGetValue(NowTop, out NowPageLocation);

                    if (NowPageLocation == new System.Drawing.Point(-1, -1))
                    {
                        continue;
                    }

                    bool CheckX = TopData.Children[i].X >= NowPageLocation.X && TopData.Children[i].X + ShapWidth <= NowPageLocation.X + MaxSize.Width;
                    bool CheckY = TopData.Children[i].Y >= NowPageLocation.Y && TopData.Children[i].Y + ShapHeight <= NowPageLocation.Y + MaxSize.Height;

                    //如果不在区域内,转换成转移门，再作为下一页顶层节点来重新检查。
                    if (CheckX == false || CheckY == false)
                    {
                        //如果不在区域内且不是门，要把它的父节点切割成转移门
                        if (TopData.Children[i].IsGateType == false)
                        {
                            if (AllTransData.Values.Contains(TopData) == false)
                            {
                                PageNumID += 1;
                                DrawData OldData = TopData;

                                //复制转移门副本
                                DrawData data_Copied1 = OldData.CopyDrawData();
                                data_Copied1.Identifier = OldData.Identifier;
                                data_Copied1.Type = DrawType.TransferInGate;
                                data_Copied1.ExtraValue11 = "To Page " + (PageCount + PageNumID).ToString();

                                //断绝现有关系                   
                                DrawData data_Parent1 = OldData.Parent;
                                int indexs = data_Parent1.Children.IndexOf(OldData);

                                //本体关系重置
                                data_Parent1.Children.Remove(OldData);
                                OldData.Parent = null;

                                //副本关系重置
                                data_Copied1.Parent = data_Parent1;
                                data_Parent1.Children.Insert(indexs, data_Copied1);

                                AllTransData.Add(PageNumID, OldData);
                                if (AllTransDataPoint.Keys.Contains(OldData) == false)
                                {
                                    AllTransDataPoint.Add(OldData, GetNowPageLocation(OldData));
                                }
                            }
                        }
                        else
                        {
                            //如果不在区域内，且它是门，直接转转移门
                            PageNumID += 1;
                            DrawData OldData = TopData.Children[i];

                            //复制转移门副本
                            DrawData data_Copied = OldData.CopyDrawData();
                            data_Copied.Identifier = OldData.Identifier;
                            data_Copied.Type = DrawType.TransferInGate;
                            data_Copied.ExtraValue11 = "To Page " + (PageCount + PageNumID).ToString();

                            //断绝现有关系                   
                            DrawData data_Parent = OldData.Parent;
                            int indexs = data_Parent.Children.IndexOf(OldData);

                            //本体关系重置
                            data_Parent.Children.Remove(OldData);
                            OldData.Parent = null;

                            //副本关系重置
                            data_Copied.Parent = data_Parent;
                            data_Parent.Children.Insert(indexs, data_Copied);

                            AllTransData.Add(PageNumID, OldData);
                            AllTransDataPoint.Add(OldData, GetNowPageLocation(OldData));

                            SolAllData(OldData, MaxSize, PageCount);
                        }
                    }//如果在区域内,进行下一轮检查
                    else
                    {
                        SolAllData(TopData.Children[i], MaxSize, PageCount);
                    }
                }
            }
            catch (Exception ex)
            {
                MsgBox.Show(ex.Message);
            }
        }

        public void IndependenceCheck(SystemModel sys, DrawData drawData)
        {
            if (drawData?.Children?.Count > 0)
            {
                var errorMessage = string.Empty;
                try
                {  //（不能包含中文路径）
                    string ApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    if (Directory.Exists(ApplicationData + "\\ASPECT\\Xfta") == false)
                    {
                        Directory.CreateDirectory(ApplicationData + "\\ASPECT\\Xfta");
                    }
                    if (Directory.Exists(ApplicationData + "\\ASPECT\\Xfta\\Temp") == false)
                    {
                        Directory.CreateDirectory(ApplicationData + "\\ASPECT\\Xfta\\Temp");
                    }

                    string exePath = ApplicationData + "\\ASPECT\\Xfta";

                    Analysis analysis = new Analysis();
                    String myOPENPSAFilePath = exePath + "\\Temp\\OPENPSA.xml";
                    //生成标准格式的OPENPSA文件
                    analysis.GenerateDefinedID(sys, drawData);
                    analysis.GenerateOPENPSAFile(drawData, myOPENPSAFilePath);
                    //生成脚本文件
                    string szScriptPath = exePath + "\\Temp\\FNScript.xml";
                    analysis.GenerateScriptFile(szScriptPath);
                    //调用函数生成割集和概率
                    analysis.DFSCalculateProbs(sys, drawData, myOPENPSAFilePath, szScriptPath, exePath);
                }
                catch (Exception ex)
                {
                    if (ex.HResult != -2146233079 && ex.HResult != -2146232798) MsgBox.Show(General.FtaProgram.String.IntegrityCheckString_CalculateFailed + ex.Message);
                    return;
                }

                this.PreviewIc(drawData);
            }
        }

        private Workbook GetWorkbook(DrawData drawData)
        {
            var result = new Workbook();
            try
            {
                var fieldNames = new string[] { "Cutset No.", "Order", "Cutsets", "LRU", "Probability", "Contribution" };

                var sheet = result.Worksheets[0];
                for (int i = 0; i < fieldNames.Length; i++) sheet.Cells.Rows[0][i].Value = fieldNames[i];

                for (int i = 0; i < drawData.Cutset.ListCutsets_Real.Count; i++)
                {
                    var row = i + 1;
                    var events = drawData.Cutset.ListCutsets_Real[i].Events;
                    sheet.Cells.Rows[row][0].Value = $"{drawData.Identifier}-{drawData.Comment1}-Cutset-{row}";
                    sheet.Cells.Rows[row][1].Value = $"{events.Count}";
                    sheet.Cells.Rows[row][2].Value = $"{string.Join(",", events.ToArray())}";
                    sheet.Cells.Rows[row][3].Value = "IDU";
                    sheet.Cells.Rows[row][4].Value = General.ConvertEStringToDouble(drawData.Cutset.ListCutsets_Real[i].szProb);
                    sheet.Cells.Rows[row][5].Value = $"{General.ConvertEStringToDouble(drawData.Cutset.ListCutsets_Real[i].szContri)}%";

                }
            }
            catch (Exception ex) { throw ex; }
            return result;
        }

        private DataTable GetPDFData(SystemModel sys, DrawData drawData)
        {
            try
            {
                List<DrawData> das = sys.GetAllDatas();
                DataTable dt = new DataTable();
                dt.Columns.Add("Cutsets");
                dt.Columns.Add("Description");
                dt.Columns.Add("Probability");

                var fieldNames = new string[] { "Cutset No.", "Order", "Cutsets", "LRU", "Probability", "Contribution" };

                for (int i = 0; i < drawData.Cutset.ListCutsets_Real.Count; i++)
                {
                    var row = i + 1;
                    var events = drawData.Cutset.ListCutsets_Real[i].Events;
                    string Cutsets = $"{string.Join(",", events.ToArray())}";
                    string Probability = General.ConvertEStringToDouble(drawData.Cutset.ListCutsets_Real[i].szProb);

                    string Description = "";
                    foreach (string ev in events)
                    {
                        Description += das.Where(d => d.Identifier == ev).FirstOrDefault().Comment1 + "\r\n";
                    }

                    dt.Rows.Add(new string[] { Cutsets, Description, Probability });
                }

                dt.AcceptChanges();

                return dt;
            }
            catch (Exception) { return null; }
        }

        private string WriteMCLToPDFFile(SystemModel sys, string filePath, DrawData drawData)
        {
            var result = string.Empty;
            try
            {
                var PDFData = this.GetPDFData(sys, drawData);

                if (PDFData != null)
                {
                    //页眉页脚
                    //Create a Section object by calling Add method of Sections collection of Pdf class
                    Aspose.Pdf.Generator.Pdf pdf1 = new Aspose.Pdf.Generator.Pdf();
                    pdf1.Sections.Clear();
                    //表格导出 
                    Aspose.Pdf.Generator.Section sec2 = pdf1.Sections.Add();
                    SetSection(sec2, Aspose.Pdf.Generator.PageSize.A4Width, Aspose.Pdf.Generator.PageSize.A4Height);

                    SetHeaderFooterToFTA(pdf1, sec2, 0, "", sys.SystemName, drawData.Identifier, EventN.ToString(), GateN.ToString());

                    //Create a table
                    Aspose.Pdf.Generator.Table tab1 = new Aspose.Pdf.Generator.Table();
                    //Add the table into the paragraphs collection of section
                    sec2.Paragraphs.Add(tab1);
                    //Set the column widths of the table
                    tab1.ColumnWidths = "250 200 100";
                    //Set the default cell border using BorderInfo instance
                    tab1.DefaultCellBorder = new Aspose.Pdf.Generator.BorderInfo((int)Aspose.Pdf.Generator.BorderSide.All);

                    Aspose.Pdf.Generator.Row row1 = tab1.Rows.Add();
                    row1.Cells.Add("Event List");
                    row1.Cells.Add("Description");
                    row1.Cells.Add("Probability");
                    foreach (DataRow row in PDFData.Rows)
                    {
                        TextInfo TxInfo = new TextInfo();
                        TxInfo.FontName = "SimSun";
                        Aspose.Pdf.Generator.Row row2 = tab1.Rows.Add();
                        row2.Cells.Add(row["Cutsets"].ToString(), TxInfo);
                        row2.Cells.Add(row["Description"].ToString(), TxInfo);
                        row2.Cells.Add(row["Probability"].ToString(), TxInfo);
                    }

                    pdf1.SetUnicode();
                    pdf1.Save(filePath);

                    //导出word
                    Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(filePath);
                    string NewFile = new FileInfo(filePath).FullName.Replace(new FileInfo(filePath).Extension, ".docx");
                    pdfDocument.Save(NewFile, Aspose.Pdf.SaveFormat.DocX);
                }
            }
            catch (Exception ex) { result = ex.Message; }
            return result;
        }

        private string WriteMCLToExcelFile(string filePath, DrawData drawData)
        {
            var result = string.Empty;
            try
            {
                var workbook = this.GetWorkbook(drawData);
                if (workbook != null)
                {
                    workbook.Worksheets[0].AutoFitColumns();
                    workbook.Worksheets[0].AutoFitRows();
                    workbook.Save(filePath);
                }
            }
            catch (Exception ex) { result = ex.Message; }
            return result;
        }

        private string SaveMCLToExcel(SystemModel sys, DrawData drawData, string Path)
        {
            var result = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(Path) == false)
                {
                    SplashScreenManager.ShowDefaultWaitForm();
                    //this.WriteMCLToExcelFile(path, drawData);

                    this.WriteMCLToPDFFile(sys, Path, drawData);

                    if (SplashScreenManager.Default != null) SplashScreenManager.CloseDefaultWaitForm();
                }
                result = Path;
            }
            catch (Exception ex)
            {
                if (SplashScreenManager.Default != null) SplashScreenManager.CloseDefaultWaitForm();
            }
            return result;
        }

        private void PreviewMcl(SystemModel sys, DrawData drawdata)
        {
            new ExportMclView(sys.SystemName, drawdata).ShowDialog();
        }

        private void PreviewIc(DrawData drawdata)
        {
            new ExportIcView(General.FtaProgram.CurrentSystem.SystemName, drawdata).ShowDialog();
        }

        public void IndependeceCheck(DiagramControl diagramControl, List<string> eventNames)
        {
            var root = diagramControl.Items.First().Tag as DrawData;
            root.CalcEffect(eventNames);
            diagramControl.Refresh();
        }

        public void IndependeceCheck(DiagramControl diagramControl, List<int> eventIDs)
        {
            var root = diagramControl.Items.First().Tag as DrawData;
            root.CalcEffect3(eventIDs);
            diagramControl.Refresh();
        }


        /// <summary>
        /// 根据给出的数据对象，返回图形对象列表(已经排好树布局)
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public List<DiagramItem> GenerateDiagramItems(DrawData root)
        {

            List<DiagramItem> shapes = new List<DiagramItem>();
            if (root != null)
            {
                root.ApplyTreeLayout(150, 150, 200, 200, 20, 20);
                FTADiagram_GenerateAllDiagramShape(shapes, root, null);
            }
            return shapes;
        }

        /// <summary>
        /// 根据数据对象递归生成所有的图形对象（包括线条）
        /// </summary>
        /// <param name="shapes">存放结果的集合</param>
        /// <param name="root">父数据对象</param>
        /// <param name="parent_Shape">父图形对象</param>
        private void FTADiagram_GenerateAllDiagramShape(List<DiagramItem> shapes, DrawData root, DiagramShape parent_Shape)
        {
            //第一个根图形
            if (root != null && parent_Shape == null)
            {
                parent_Shape = GenerateDiagramShape(root);
                shapes.Add(parent_Shape);
            }
            if (parent_Shape != null && root.Children != null && root.Children.Count > 0)
            {
                foreach (DrawData child in root.Children)
                {
                    DiagramShape child_Shape = GenerateDiagramShape(child);
                    shapes.Add(child_Shape);
                    //连线
                    DiagramConnector connector = GenerateDiagramConenctor(parent_Shape, child_Shape);
                    shapes.Add(connector);
                    FTADiagram_GenerateAllDiagramShape(shapes, child, child_Shape);
                }
            }
        }


        /// <summary>
        /// 根据给出的数据对象，构造一个图形对象
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public DiagramShape GenerateDiagramShape(DrawData data)
        {
            if (data != null)
            {
                DiagramShape Shape = new DiagramShape(BasicShapes.Rectangle);
                Shape.Tag = data;
                Shape.X = data.X;
                Shape.Y = data.Y;
                Shape.ConnectionPoints = new PointCollection(new List<PointFloat>() { new PointFloat(0.5f, 0), new PointFloat(0.5f, 1) });
                Shape.Size = new Size(150, 150);
                Shape.CanEdit = false;
                Shape.CanDelete = false;
                Shape.CanCopy = false;
                Shape.CanMove = false;
                Shape.CanResize = false;
                Shape.CanRotate = false;
                Shape.MinSize = new SizeF(0, 0);
                return Shape;
            }
            return null;
        }



        /// <summary>
        /// 根据给出的数据对象，构造一个图形对象
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public DiagramConnector GenerateDiagramConenctor(DiagramShape parent_Shape, DiagramShape child_Shape)
        {
            if (parent_Shape != null && child_Shape != null)
            {
                //连线
                DiagramConnector connector = new DiagramConnector(parent_Shape, child_Shape);
                connector.BeginItemPointIndex = 1;
                connector.EndItemPointIndex = 0;
                connector.Appearance.BorderColor = System.Drawing.Color.Black;
                connector.EndArrowSize = new SizeF(0, 0);
                connector.CanDelete = false;
                connector.CanMove = false;
                connector.CanChangeRoute = false;
                connector.CanEdit = false;
                connector.CanCopy = false;
                connector.CanResize = false;
                connector.CanRotate = false;
                connector.MinSize = new SizeF(0, 0);
                return connector;
            }
            return null;
        }
    }
}
