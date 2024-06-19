using Aspose.Cells;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraTreeList.Nodes;
using FaultTreeAnalysis.Model.Data;
using FaultTreeAnalysis.Model.Enum;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Xml;

namespace FaultTreeAnalysis.Common
{
    /// <summary>
    /// Medini XML文件导入处理
    /// </summary>
    public class ImportMediniDatas
    {
        /// <summary>
        /// 导入时，除了根节点的所有其他子节点暂存的集合，用于得到根节点对象
        /// </summary>
        public static List<DrawData> drawDataChildrenCache = new List<DrawData>();

        #region XML文件节点处理
        public static DataTable CreateDataTableXml()
        {
            DataTable dataTableXml = new DataTable();
            dataTableXml.Columns.Add("Identifier", typeof(string));
            dataTableXml.Columns.Add("Type", typeof(string));
            dataTableXml.Columns.Add("ParentID", typeof(string));
            dataTableXml.Columns.Add("Description", typeof(string));
            dataTableXml.Columns.Add("Logical Condition", typeof(string));
            dataTableXml.Columns.Add("Input Type", typeof(string));
            dataTableXml.Columns.Add("Units", typeof(string));
            dataTableXml.Columns.Add("Exposure Time", typeof(string));
            dataTableXml.Columns.Add("Failure Rate", typeof(string));
            dataTableXml.Columns.Add("Votes", typeof(string));
            return dataTableXml;
        }

        public static void AddAllNodesToDataTableXml(XmlDocument xmlDocument, DataTable dataTableXml)
        {
            foreach (XmlNode gateNode in xmlDocument.SelectNodes("//Gates"))
            {
                AddGateToDataTableXml(gateNode, dataTableXml);
            }

            foreach (XmlNode primaryEventNode in xmlDocument.SelectNodes("//PrimaryEvents"))
            {
                AddPrimaryEventToDataTableXml(primaryEventNode, dataTableXml, xmlDocument);
            }

            foreach (XmlNode gateInputNode in xmlDocument.SelectNodes("//GateInputs"))
            {
                string gateId = gateInputNode.SelectSingleNode("Gate").InnerText;
                string objectType = gateInputNode.SelectSingleNode("ObjectType").InnerText;
                int objectIndex = int.Parse(gateInputNode.SelectSingleNode("ObjectIndex").InnerText);

                if (objectType == "Primary event")
                {
                    XmlNode primaryEventNode = xmlDocument.SelectNodes("//PrimaryEvents")[objectIndex];
                    UpdateParentId(primaryEventNode, dataTableXml, gateId);
                }
                else if (objectType == "Gate")
                {
                    XmlNode childGateNode = xmlDocument.SelectNodes("//Gates")[objectIndex];
                    UpdateParentId(childGateNode, dataTableXml, gateId);
                }
            }
        }

        public static void AddGateToDataTableXml(XmlNode gateNode, DataTable dataTableXml)
        {
            DataRow rowXml = dataTableXml.NewRow();
            rowXml["Identifier"] = gateNode.SelectSingleNode("Id").InnerText;
            rowXml["Type"] = gateNode.SelectSingleNode("Type").InnerText + " Gate";
            rowXml["ParentID"] = DBNull.Value.ToString();
            rowXml["Description"] = gateNode.SelectSingleNode("Description").InnerText;
            rowXml["Votes"] = gateNode.SelectSingleNode("Vote") != null ? gateNode.SelectSingleNode("Vote").InnerText : string.Empty;
            dataTableXml.Rows.Add(rowXml);
        }

        public static void AddPrimaryEventToDataTableXml(XmlNode primaryEventNode, DataTable dataTableXml, XmlDocument xmlDocument)
        {
            DataRow rowXml = dataTableXml.NewRow();
            rowXml["Identifier"] = primaryEventNode.SelectSingleNode("Id").InnerText;
            string eventType = primaryEventNode.SelectSingleNode("EventType").InnerText;

            // Transform Conditional events to Basic events
            if (eventType == "Conditional")
            {
                eventType = "Basic";
            }

            rowXml["Type"] = eventType + " Event";
            rowXml["Input Type"] = "Lambda Tau";
            rowXml["Description"] = primaryEventNode.SelectSingleNode("Description").InnerText;
            rowXml["Units"] = "Hours";
            rowXml["ParentID"] = DBNull.Value.ToString();

            string failureModelId = primaryEventNode.SelectSingleNode("FailureModel")?.InnerText;
            XmlNode failureModelNode = xmlDocument.SelectSingleNode($"//FailureModels[Id='{failureModelId}']");

            if (failureModelNode != null)
            {
                string modelType = failureModelNode.SelectSingleNode("ModelType").InnerText;
                if (modelType == "Fixed")
                {
                    rowXml["Input Type"] = "Constant Probability";
                    rowXml["Exposure Time"] = "1";
                    rowXml["Failure Rate"] = failureModelNode.SelectSingleNode("Unavailability").InnerText;
                }
                else if (modelType == "Rate")
                {
                    rowXml["Input Type"] = "Failure Rate";  // 设置 Input Type 为 Failure Rate
                    rowXml["Failure Rate"] = failureModelNode.SelectSingleNode("FailureRate").InnerText;  // 从 FailureRate 字段获取概率值
                    rowXml["Exposure Time"] = "1";
                }
                else if (modelType == "Weibull")
                {
                    rowXml["Input Type"] = "Failure Probability";
                    rowXml["Exposure Time"] = "1";
                    rowXml["Failure Rate"] = "1E-9";
                }
            }

            dataTableXml.Rows.Add(rowXml);
        }

        public static void UpdateParentId(XmlNode node, DataTable dataTableXml, string parentId)
        {
            DataRow row = dataTableXml.AsEnumerable().FirstOrDefault(r => r.Field<string>("Identifier") == node.SelectSingleNode("Id").InnerText);
            if (row != null)
            {
                row["ParentID"] = parentId;
            }
        }

        public static void AddNodeAndDescendantsToDataTable(DataRow node, DataTable sourceTable, DataTable destinationTable)
        {
            DataRow newRow = destinationTable.NewRow();
            newRow.ItemArray = node.ItemArray;
            destinationTable.Rows.Add(newRow);

            var childNodes = sourceTable.AsEnumerable().Where(row => row.Field<string>("ParentID") == node.Field<string>("Identifier")).ToList();
            foreach (var childNode in childNodes)
            {
                AddNodeAndDescendantsToDataTable(childNode, sourceTable, destinationTable);
            }
        }

        public static List<DrawData> ImportXMLFiles(List<string> ImportFiles, ref string err)
        {
            var result = string.Empty;
            try
            {
                for (int i = 0; i < ImportFiles.Count; i++)
                {
                    if (!string.IsNullOrEmpty(ImportFiles[i]))
                    {
                        if (Path.GetExtension(ImportFiles[i]).Equals(".xml", StringComparison.OrdinalIgnoreCase))
                        {
                            List<DrawData> convertedDatas = XmlToDrawDatas(ImportFiles[i], ref err);
                            return convertedDatas;
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                result = ex.Message;
                err = $"Exception occurred: {ex.Message}";
            }
            err = result;
            return null;
        }
        #endregion

        /// <summary>
        /// 读取XML文档到List<DrawData>对象
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="columnFieldInfo"></param>
        /// <returns></returns>
        public static List<DrawData> XmlToDrawDatas(string xmlFilePath, ref string err)
        {
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            XmlDocument xmlDocument = new XmlDocument();
            string xmlContent = File.ReadAllText(xmlFilePath);
            xmlDocument.LoadXml(xmlContent);

            List<DrawData> generatedDatas = new List<DrawData>();
            DataTable dataTableXml = CreateDataTableXml();
            AddAllNodesToDataTableXml(xmlDocument, dataTableXml);
            var rootNodes = dataTableXml.AsEnumerable().Where(row => row.Field<string>("ParentID") == DBNull.Value.ToString()).ToList();

            foreach (var rootNode in rootNodes)
            {
                DataTable faultTreeDataTable = dataTableXml.Clone();
                AddNodeAndDescendantsToDataTable(rootNode, dataTableXml, faultTreeDataTable);

                List<DrawData> datas = DataTableToDrawDatas(faultTreeDataTable, ref err);

                SetDrawDatasRelation(datas);
                datas = datas.Except(drawDataChildrenCache).ToList();

                generatedDatas.AddRange(datas);
            }
            return generatedDatas;
        }

        /// <summary>
        /// 添加List<DrawData>对象的关联关系
        /// </summary>
        /// <param name="drawData"></param>
        public static void SetDrawDatasRelation(List<DrawData> drawData)
        {
            foreach (var item in drawData)
            {
                if (string.IsNullOrEmpty(item.ParentID) == false)
                {
                    if (item.Parent == null)
                    {
                        var parent = drawData.FirstOrDefault(o => o.Type != DrawType.TransferInGate && o.Identifier == item.ParentID);
                        var children = drawData.Where(o => parent != null && o.ParentID == parent.Identifier);
                        if (children != null) drawDataChildrenCache.AddRange(children);
                        foreach (var item2 in children) item2.Parent = parent;
                        if (parent != null) parent.Children = children.ToList();
                    }
                }
            }
        }

        /// <summary>
        /// DataTable到List<DrawData>对象
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="columnFieldInfo"></param>
        /// <returns></returns>
        /// 
        public static List<DrawData> DataTableToDrawDatas(DataTable tb, ref string err)
        {
            List<DrawData> result = new List<DrawData>();
            try
            {
                for (int i = 0; i <= tb.Rows.Count - 1; i++)
                {
                    var drawData = new DrawData();
                    var row = tb.Rows[i];
                    foreach (DataColumn col in tb.Columns)
                    {
                        try
                        {
                            if (col.ColumnName == "Type")
                            {
                                string tp = row[col.ColumnName].ToString().Replace(" ", "");
                                if (tp == "ORGate")
                                {
                                    tp = "OrGate";
                                }
                                if (tp == "ANDGate")
                                {
                                    tp = "AndGate";
                                }
                                if (tp == "VOTEGate")
                                {
                                    tp = "VotingGate";
                                }

                                drawData.Type = (DrawType)Enum.Parse(typeof(DrawType), tp);
                            }
                            else if (col.ColumnName == "Units")
                            {
                                var unitsName = General.GetKeyName(row[col.ColumnName].ToString());
                                drawData.Units = unitsName;
                            }
                            else
                            {
                                if (row[col.ColumnName] != null)
                                {
                                    if (col.ColumnName == "Logical Condition")
                                    {
                                        var logicalCondition = General.GetKeyName(row[col.ColumnName].ToString());
                                        switch (logicalCondition)
                                        {
                                            case nameof(StringModel.Normal): drawData.LogicalCondition = General.FtaProgram.String.Normal; break;
                                            case nameof(StringModel.False): drawData.LogicalCondition = General.FtaProgram.String.False; break;
                                            case nameof(StringModel.True): drawData.LogicalCondition = General.FtaProgram.String.True; break;
                                            default: break;
                                        }
                                    }
                                    else if (col.ColumnName == "Description")
                                    {
                                        drawData.Comment1 = row[col.ColumnName].ToString();
                                    }
                                    else if (col.ColumnName == "Failure Rate")
                                    {
                                        drawData.InputValue = row[col.ColumnName].ToString();
                                    }
                                    else if (col.ColumnName == "Exposure Time")
                                    {
                                        drawData.InputValue2 = row[col.ColumnName].ToString();
                                    }
                                    else if (col.ColumnName == "Votes")
                                    {
                                        drawData.ExtraValue1 = row[col.ColumnName].ToString();
                                    }
                                    else if (col.ColumnName == "Input Type")
                                    {
                                        drawData.InputType = row[col.ColumnName].ToString();
                                    }
                                    else if (col.ColumnName == "Input Type")
                                    {
                                        drawData.InputType = row[col.ColumnName].ToString();
                                    }
                                    else if (col.ColumnName == "Identifier")
                                    {
                                        drawData.Identifier = row[col.ColumnName].ToString();
                                    }
                                    else if (col.ColumnName == "ParentID")
                                    {
                                        drawData.ParentID = row[col.ColumnName].ToString();
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            err = $"Error setting property {col.ColumnName}: {ex.Message}";
                        }
                    }

                    // 恒定概率空值重置为1
                    if (!drawData.IsGateType && drawData.InputType == General.FtaProgram.String.ConstantProbability)
                    {
                        if (drawData.InputValue2 == "")
                        {
                            drawData.InputValue2 = "1";
                        }
                    }

                    result.Add(drawData);
                }
            }
            catch (Exception ex)
            {
                err = $"Exception occurred while reading Excel file: {ex.Message}";
                throw ex;
            }
            return result;
        }
    }
}
