using DevExpress.XtraPrinting;
using FaultTreeAnalysis.Common;
using FaultTreeAnalysis.Model.Data;
using FaultTreeAnalysis.Model.Enum;
using IntegratedSystem.Tools;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace FaultTreeAnalysis.View.Ribbon
{
    /// <summary>
    /// 故障树计算算法
    /// </summary>
    public class Analysis
    {
        static int lamdbaNumber = 0;
        int count = 0;
        /// <summary>
        /// 用于保存重复事件，重复事件已经生成配置文件，不用重复生成
        /// </summary>
        HashSet<string> Hash_Id;

        /// <summary>
        /// 用于保存转移门，转移门已经生成配置文件，不用重复生成
        /// </summary>
        HashSet<string> Trans_Id;

        public int num1 = 0;
        public int num2 = 0;
        public Dictionary<Guid, string> dic_Defined_Guid = new Dictionary<Guid, string>();
        public Dictionary<string, string> dic_Defined_Id = new Dictionary<string, string>();

        /// <summary>
        /// 生成xftar支持的文件ID
        /// </summary> 
        /// <param name="rootItem">当前根节点</param>
        public void GenerateDefinedID(SystemModel sys, DrawData rootItem)
        {
            num1 = 0;
            num2 = 0;
            dic_Defined_Guid.Clear();
            List<DrawData> datas = rootItem.GetAllData(sys, true);

            foreach (DrawData data in datas)
            {
                if (dic_Defined_Id.Keys.Contains(data.Identifier) == false)
                {

                    if (data.IsGateType)
                    {
                        num1 += 1;
                        dic_Defined_Id.Add(data.Identifier, "G" + num1.ToString());
                    }
                    else
                    {
                        num2 += 1;
                        dic_Defined_Id.Add(data.Identifier, "E" + num2.ToString());
                    }
                }
            }

            foreach (DrawData data in datas)
            {
                if (dic_Defined_Guid.Keys.Contains(data.ThisGuid) == false)
                {
                    dic_Defined_Guid.Add(data.ThisGuid, dic_Defined_Id[data.Identifier]);
                }
            }
        }

        /// <summary>
        /// 生成标准格式的OPENPSA文件
        /// </summary>
        /// <param name="system">当前系统</param>
        /// <param name="rootItem">当前根节点</param>
        /// <param name="myOPENPSAFilePath">文件路径</param>
        public void GenerateOPENPSAFile(SystemModel system, DrawData rootItem, String myOPENPSAFilePath)
        {
            Hash_Id = new HashSet<string>();
            Trans_Id = new HashSet<string>();
            lamdbaNumber = 0;
            //将保存的文件格式转化为符合OPENPSA规范的xml文件
            XmlDocument myOPENPSAFile = new XmlDocument();
            //创建头部声明
            XmlNode xmlHeader = myOPENPSAFile.CreateNode(XmlNodeType.XmlDeclaration, "", "");
            myOPENPSAFile.AppendChild(xmlHeader);

            //创建根节点
            XmlElement rootElement = myOPENPSAFile.CreateElement("open-psa");
            myOPENPSAFile.AppendChild(rootElement);

            GenerateByDrawData(count, system, rootItem, rootElement, myOPENPSAFile);

            myOPENPSAFile.Save(myOPENPSAFilePath);
        }

        /// <summary>
        /// 生成标准格式的OPENPSA文件
        /// </summary>
        /// <param name="system">当前系统</param>
        /// <param name="rootItem">当前根节点</param>
        /// <param name="myOPENPSAFilePath">文件路径</param>
        public void GenerateOPENPSAFile(DrawData rootItem, String myOPENPSAFilePath)
        {
            Hash_Id = new HashSet<string>();
            Trans_Id = new HashSet<string>();
            lamdbaNumber = 0;
            //将保存的文件格式转化为符合OPENPSA规范的xml文件
            XmlDocument myOPENPSAFile = new XmlDocument();
            //创建头部声明
            XmlNode xmlHeader = myOPENPSAFile.CreateNode(XmlNodeType.XmlDeclaration, "", "");
            myOPENPSAFile.AppendChild(xmlHeader);

            //创建根节点
            XmlElement rootElement = myOPENPSAFile.CreateElement("open-psa");
            myOPENPSAFile.AppendChild(rootElement);

            GenerateByDrawData(rootItem, rootElement, myOPENPSAFile);

            myOPENPSAFile.Save(myOPENPSAFilePath);
        }

        /// <summary>
        /// 递归解析数据，生成标准格式的OPENPSA文件
        /// </summary>
        /// <param name="system">当前系统</param>
        /// <param name="rootItem">树根节点</param>
        /// <param name="rootXmlElement">根节点xml元素</param>
        /// <param name="myOPENPSAFile">xml文档对象</param>
        int GenerateByDrawData(DrawData rootItem, XmlElement rootXmlElement, XmlDocument myOPENPSAFile)
        {
            XmlElement interElement = null;
            XmlElement gateElement = null;
            XmlElement eventElement = null;
            XmlElement parameterElement = null;
            XmlElement parameterValue = null;

            if (rootItem.IsGateType && rootItem.Children?.Count == 1)
            {
                foreach (DrawData grandSon in rootItem.Children)
                {
                    if (grandSon.Type == DrawType.HouseEvent)
                    {
                        return count;
                    }
                }
            }

            switch (rootItem.Type)
            {
                case DrawType.AndGate:
                    interElement = myOPENPSAFile.CreateElement("define-gate");
                    interElement.SetAttribute("name", dic_Defined_Guid[rootItem.ThisGuid]);
                    rootXmlElement.AppendChild(interElement);
                    gateElement = myOPENPSAFile.CreateElement("and");
                    interElement.AppendChild(gateElement);
                    break;
                case DrawType.OrGate:
                case DrawType.RemarksGate:
                    interElement = myOPENPSAFile.CreateElement("define-gate");
                    interElement.SetAttribute("name", dic_Defined_Guid[rootItem.ThisGuid]);
                    rootXmlElement.AppendChild(interElement);
                    gateElement = myOPENPSAFile.CreateElement("or");
                    interElement.AppendChild(gateElement);
                    break;
                case DrawType.VotingGate:
                    interElement = myOPENPSAFile.CreateElement("define-gate");
                    interElement.SetAttribute("name", dic_Defined_Guid[rootItem.ThisGuid]);
                    rootXmlElement.AppendChild(interElement);
                    gateElement = myOPENPSAFile.CreateElement("atleast");
                    gateElement.SetAttribute("min", rootItem.ExtraValue1);
                    interElement.AppendChild(gateElement);
                    break;
                case DrawType.HouseEvent:
                    break;
                case DrawType.BasicEvent:
                case DrawType.UndevelopedEvent:
                case DrawType.ConditionEvent:
                    if (Hash_Id.Contains(rootItem.Identifier))
                    {
                        break;
                    }
                    Hash_Id.Add(rootItem.Identifier);
                    interElement = myOPENPSAFile.CreateElement("define-basic-event");
                    interElement.SetAttribute("name", dic_Defined_Guid[rootItem.ThisGuid]);
                    rootXmlElement.AppendChild(interElement);
                    eventElement = myOPENPSAFile.CreateElement("exponential");
                    interElement.AppendChild(eventElement);
                    parameterElement = myOPENPSAFile.CreateElement("parameter");
                    parameterElement.SetAttribute("name", "lamdba" + (++lamdbaNumber));
                    eventElement.AppendChild(parameterElement);
                    parameterValue = myOPENPSAFile.CreateElement("float");
                    if (rootItem.Units == FixedString.UNITS_HOURS || rootItem.Units == "小时")
                        parameterValue.SetAttribute("value", rootItem.InputValue2);
                    else
                    {
                        decimal d1 = 0;
                        if (decimal.TryParse(rootItem.InputValue2, System.Globalization.NumberStyles.Float, null, out d1))
                        {
                            decimal d = d1 / 60;
                            parameterValue.SetAttribute("value", d.ToString());
                        }
                    }
                    eventElement.AppendChild(parameterValue);

                    //lambda节点
                    interElement = myOPENPSAFile.CreateElement("define-parameter");
                    interElement.SetAttribute("name", "lamdba" + lamdbaNumber);
                    rootXmlElement.AppendChild(interElement);
                    parameterValue = myOPENPSAFile.CreateElement("float");
                    parameterValue.SetAttribute("value", rootItem.InputValue);
                    interElement.AppendChild(parameterValue);

                    try
                    {
                        decimal d1 = 0;
                        decimal d2 = 0;
                        if (decimal.TryParse(rootItem.InputValue2, System.Globalization.NumberStyles.Float, null, out d1))
                        {
                            if (decimal.TryParse(rootItem.InputValue, System.Globalization.NumberStyles.Float, null, out d2))
                            {
                                if (rootItem.Units == FixedString.UNITS_MINUTES || rootItem.Units == "分钟")
                                {
                                    d1 = d1 / 60;
                                }
                                rootItem.QValue = (d1 * d2).ToString("E");
                            }
                        }
                    }
                    catch
                    { }
                    break;
                case DrawType.TransferInGate:
                    if (Trans_Id.Contains(rootItem.Identifier))
                    {
                        break;
                    }
                    Trans_Id.Add(rootItem.Identifier);
                    break;
                default:
                    break;
            }

            if (rootItem.Children != null && gateElement != null)
            {
                foreach (DrawData grandSon in rootItem.Children)
                {
                    XmlElement gs;
                    if (grandSon.Type == DrawType.BasicEvent ||
                        grandSon.Type == DrawType.UndevelopedEvent ||
                        grandSon.Type == DrawType.ConditionEvent)
                    {
                        gs = myOPENPSAFile.CreateElement("basic-event");
                    }
                    else if (grandSon.Type == DrawType.HouseEvent)
                    {
                        continue;
                    }
                    else
                    {
                        gs = myOPENPSAFile.CreateElement("gate");
                    }

                    bool check = false;
                    if (grandSon.IsGateType && grandSon.Children?.Count == 1)
                    {
                        foreach (DrawData grandSonC in grandSon.Children)
                        {
                            if (grandSonC.Type == DrawType.HouseEvent)
                            {
                                check = true;
                            }
                        }
                    }
                    if (check)
                    {
                        continue;
                    }

                    gs.SetAttribute("name", dic_Defined_Guid[grandSon.ThisGuid]);
                    gateElement.AppendChild(gs);
                }

                foreach (DrawData grandSon in rootItem.Children)
                {
                    GenerateByDrawData(grandSon, rootXmlElement, myOPENPSAFile);
                }
            }

            return count;
        }

        /// <summary>
        /// 递归解析数据，生成标准格式的OPENPSA文件
        /// </summary>
        /// <param name="system">当前系统</param>
        /// <param name="rootItem">树根节点</param>
        /// <param name="rootXmlElement">根节点xml元素</param>
        /// <param name="myOPENPSAFile">xml文档对象</param>
        int GenerateByDrawData(int count, SystemModel system, DrawData rootItem, XmlElement rootXmlElement, XmlDocument myOPENPSAFile)
        {
            XmlElement interElement = null;
            XmlElement gateElement = null;
            XmlElement eventElement = null;
            XmlElement parameterElement = null;
            XmlElement parameterValue = null;
            DrawData transGate = null;

            if (rootItem.IsGateType && rootItem.Children?.Count == 1)
            {
                foreach (DrawData grandSon in rootItem.Children)
                {
                    if (grandSon.Type == DrawType.HouseEvent)
                    {
                        return count;
                    }
                }
            }

            count++;

            switch (rootItem.Type)
            {
                case DrawType.AndGate:
                    interElement = myOPENPSAFile.CreateElement("define-gate");
                    interElement.SetAttribute("name", dic_Defined_Guid[rootItem.ThisGuid]);
                    rootXmlElement.AppendChild(interElement);
                    gateElement = myOPENPSAFile.CreateElement("and");
                    interElement.AppendChild(gateElement);
                    break;
                case DrawType.OrGate:
                case DrawType.RemarksGate:
                    interElement = myOPENPSAFile.CreateElement("define-gate");
                    interElement.SetAttribute("name", dic_Defined_Guid[rootItem.ThisGuid]);
                    rootXmlElement.AppendChild(interElement);
                    gateElement = myOPENPSAFile.CreateElement("or");
                    interElement.AppendChild(gateElement);
                    break;
                case DrawType.VotingGate:
                    interElement = myOPENPSAFile.CreateElement("define-gate");
                    interElement.SetAttribute("name", dic_Defined_Guid[rootItem.ThisGuid]);
                    rootXmlElement.AppendChild(interElement);
                    gateElement = myOPENPSAFile.CreateElement("atleast");
                    gateElement.SetAttribute("min", rootItem.ExtraValue1);
                    interElement.AppendChild(gateElement);
                    break;
                case DrawType.HouseEvent:
                    break;
                case DrawType.BasicEvent:
                case DrawType.UndevelopedEvent:
                case DrawType.ConditionEvent:
                    if (Hash_Id.Contains(rootItem.Identifier))
                    {
                        break;
                    }
                    Hash_Id.Add(rootItem.Identifier);
                    interElement = myOPENPSAFile.CreateElement("define-basic-event");
                    interElement.SetAttribute("name", dic_Defined_Guid[rootItem.ThisGuid]);
                    rootXmlElement.AppendChild(interElement);
                    //当为Lambda Tau时
                    if (rootItem.InputType == FixedString.MODEL_LAMBDA_TAU)
                    {
                        string S1;
                        //时间和概率相乘，然后作为恒定概率输入
                        try
                        {
                            decimal d1 = 0;
                            decimal d2 = 0;
                            if (decimal.TryParse(rootItem.InputValue2, System.Globalization.NumberStyles.Float, null, out d1))
                            {
                                if (decimal.TryParse(rootItem.InputValue, System.Globalization.NumberStyles.Float, null, out d2))
                                {
                                    if (rootItem.Units == FixedString.UNITS_MINUTES || rootItem.Units == "分钟")
                                    {
                                        d1 = d1 / 60;
                                    }
                                }
                            }
                            S1 = (d1 * d2).ToString("E");
                        }
                        catch
                        {
                            break;
                        }

                        parameterValue = myOPENPSAFile.CreateElement("float");
                        parameterValue.SetAttribute("value", S1);
                        interElement.AppendChild(parameterValue);
                    }
                    else if (rootItem.InputType == General.FtaProgram.String.ConstantProbability) //当为恒定概率
                    {
                        parameterValue = myOPENPSAFile.CreateElement("float");
                        parameterValue.SetAttribute("value", rootItem.InputValue);
                        interElement.AppendChild(parameterValue);
                    }
                    else
                    {
                        //当为失效概率
                        eventElement = myOPENPSAFile.CreateElement("exponential");
                        interElement.AppendChild(eventElement);
                        parameterElement = myOPENPSAFile.CreateElement("parameter");
                        parameterElement.SetAttribute("name", "lamdba" + (++lamdbaNumber));
                        eventElement.AppendChild(parameterElement);
                        parameterValue = myOPENPSAFile.CreateElement("float");
                        if (rootItem.Units == FixedString.UNITS_HOURS || rootItem.Units == "小时")
                            parameterValue.SetAttribute("value", rootItem.InputValue2);
                        else
                        {
                            decimal d1 = 0;
                            if (decimal.TryParse(rootItem.InputValue2, System.Globalization.NumberStyles.Float, null, out d1))
                            {
                                decimal d = d1 / 60;
                                parameterValue.SetAttribute("value", d.ToString());
                            }
                        }
                        eventElement.AppendChild(parameterValue);

                        //lambda节点
                        interElement = myOPENPSAFile.CreateElement("define-parameter");
                        interElement.SetAttribute("name", "lamdba" + lamdbaNumber);
                        rootXmlElement.AppendChild(interElement);
                        parameterValue = myOPENPSAFile.CreateElement("float");
                        parameterValue.SetAttribute("value", rootItem.InputValue);
                        interElement.AppendChild(parameterValue);

                        try
                        {
                            decimal d1 = 0;
                            decimal d2 = 0;
                            if (decimal.TryParse(rootItem.InputValue2, System.Globalization.NumberStyles.Float, null, out d1))
                            {
                                if (decimal.TryParse(rootItem.InputValue, System.Globalization.NumberStyles.Float, null, out d2))
                                {
                                    if (rootItem.Units == FixedString.UNITS_MINUTES || rootItem.Units == "分钟")
                                    {
                                        d1 = d1 / 60;
                                    }
                                }
                            }
                            rootItem.QValue = (d1 * d2).ToString("E");
                        }
                        catch
                        { }
                    }
                    break;
                case DrawType.TransferInGate:
                    if (Trans_Id.Contains(rootItem.Identifier))
                    {
                        break;
                    }
                    Trans_Id.Add(rootItem.Identifier);
                    transGate = system.GetTranferGateByName(rootItem.Identifier);
                    if (transGate == null)
                    {
                        return -1;
                    }
                    GenerateByDrawData(count, system, transGate, rootXmlElement, myOPENPSAFile);
                    break;
                default:
                    break;


            }

            if (rootItem.Children != null && gateElement != null)
            {
                foreach (DrawData grandSon in rootItem.Children)
                {
                    XmlElement gs;
                    if (grandSon.Type == DrawType.BasicEvent ||
                        grandSon.Type == DrawType.UndevelopedEvent ||
                        grandSon.Type == DrawType.ConditionEvent)
                    {
                        gs = myOPENPSAFile.CreateElement("basic-event");
                    }
                    else if (grandSon.Type == DrawType.HouseEvent)
                    {
                        continue;
                    }
                    else
                    {
                        gs = myOPENPSAFile.CreateElement("gate");
                    }

                    bool check = false;
                    if (grandSon.IsGateType && grandSon.Children?.Count == 1)
                    {
                        foreach (DrawData grandSonC in grandSon.Children)
                        {
                            if (grandSonC.Type == DrawType.HouseEvent)
                            {
                                check = true;
                            }
                        }
                    }
                    if (check)
                    {
                        continue;
                    }

                    gs.SetAttribute("name", dic_Defined_Guid[grandSon.ThisGuid]);
                    gateElement.AppendChild(gs);
                }

                foreach (DrawData grandSon in rootItem.Children)
                {
                    GenerateByDrawData(count, system, grandSon, rootXmlElement, myOPENPSAFile);
                }
            }

            return count;
        }

        /// <summary>
        /// 通过节点递归生成概率值和割集
        /// </summary>
        /// <param name="system">当前系统</param>
        /// <param name="rootItem">树根节点</param>
        /// <param name="myScript">脚本文档对象</param>
        /// <param name="OPENPSAPath">OPENPSA文件路径</param>
        /// <param name="scriptPath">保存路径</param>
        /// <param name="calPath">临时文件路径</param>
        string CalculateByDrawItemNoSys(SystemModel sys, DrawData rootItem, XmlDocument myScript, String OPENPSAPath, string scriptPath, string calPath)
        {

            var errorMessage = string.Empty;
            if (rootItem.Type == DrawType.AndGate || rootItem.Type == DrawType.OrGate || rootItem.Type == DrawType.PriorityAndGate || rootItem.Type == DrawType.RemarksGate || rootItem.Type == DrawType.XORGate || rootItem.Type == DrawType.VotingGate)
            {
                rootItem.QValue = string.Empty;
                //如果没有子节点，则不用计算
                if (rootItem.Children == null || rootItem.Children.Count <= 0)
                    return "没有可计算的子节点";

                string szmcs = string.Empty;
                string szpr = string.Empty;
                //对以该节点作为顶事件的子树进行计算
                XmlNode headerNode = myScript.FirstChild;
                XmlNode rootNode = headerNode.NextSibling;
                XmlNodeList firstLevelChildList = rootNode.ChildNodes;

                foreach (XmlNode xmlNode in firstLevelChildList)
                {
                    //设置OPENPSAFile的路径
                    if (xmlNode.Name.Equals("load"))
                    {
                        XmlNode modelNode = xmlNode.FirstChild;
                        modelNode.Attributes["input"].Value = OPENPSAPath;
                    }
                    //设置顶事件来计算最小割集
                    if (xmlNode.Name.Equals("build"))
                    {
                        XmlNode mcNode = xmlNode.FirstChild;
                        mcNode.Attributes["top-event"].Value = dic_Defined_Guid[rootItem.ThisGuid];
                    }
                    //设置最小割集结果文件的顶事件和输出文件名
                    if (xmlNode.Name.Equals("print"))
                    {
                        XmlNode mcNode = xmlNode.FirstChild;
                        mcNode.Attributes["top-event"].Value = dic_Defined_Guid[rootItem.ThisGuid];
                        szmcs = calPath + "\\" + dic_Defined_Guid[rootItem.ThisGuid] + ".mcs";
                        mcNode.Attributes["output"].Value = szmcs;
                        mcNode.Attributes["mission-time"].Value = "3";
                    }
                    //设置顶事件概率文件的顶事件和输出文件名
                    if (xmlNode.Name.Equals("compute"))
                    {
                        XmlNode prNode = xmlNode.FirstChild;
                        prNode.Attributes["top-event"].Value = dic_Defined_Guid[rootItem.ThisGuid];
                        szpr = calPath + "\\" + dic_Defined_Guid[rootItem.ThisGuid] + ".pr";
                        prNode.Attributes["output"].Value = szpr;
                        prNode.Attributes["mission-time"].Value = "3";
                    }
                }
                //XmlTextWriter writer = new XmlTextWriter(fs, Encoding.UTF8);
                // writer.Formatting = Formatting.Indented;
                myScript.Save(scriptPath);

                //调用xfta.dll对故障树进行计算
                int iRet = Xftar.Execute(1, new String[] { scriptPath });

                //读取生成概率值文件和割集数据文件   
                c_WaitFormProgress.WaitSP.SetWaitFormDescription("概率文件解析中......");
                string[] Probs = File.ReadAllLines(szpr);
                errorMessage = GetProbability(Probs, rootItem);

                c_WaitFormProgress.WaitSP.SetWaitFormDescription("割集文件解析中......");
                string[] mcs = File.ReadAllLines(szmcs);
                errorMessage = GetCutset(sys, mcs, rootItem);
            }

            if (rootItem.Children != null)
            {
                foreach (DrawData grandSon in rootItem.Children)
                {
                    errorMessage = CalculateByDrawItemNoSys(sys, grandSon, myScript, OPENPSAPath, scriptPath, calPath);
                }
            }
            return errorMessage;
        }


        /// <summary>
        /// 生成脚本配置文件
        /// </summary>
        /// <param name="scriptPath">文件路径</param>
        public void GenerateScriptFile(string scriptPath)
        {
            string maximumorder = "";
            try
            {
                string savingDataPath = $"{AppDomain.CurrentDomain.BaseDirectory}" + "//MaximumOrderSetting.txt";
                if (File.Exists(savingDataPath))
                {
                    maximumorder = File.ReadAllText(savingDataPath);
                }
                else
                {
                    File.WriteAllText(savingDataPath, "7");
                    maximumorder = File.ReadAllText(savingDataPath);
                }
            }
            catch (Exception)
            {
                maximumorder = "";
            }

            XmlDocument myScript = new XmlDocument();

            //创建头部声明
            XmlNode xmlHeader = myScript.CreateNode(XmlNodeType.XmlDeclaration, "", "");
            myScript.AppendChild(xmlHeader);

            //创建根节点
            XmlElement rootElement = myScript.CreateElement("xfta");
            myScript.AppendChild(rootElement);

            //创建子节点
            XmlElement loadElement = myScript.CreateElement("load");
            rootElement.AppendChild(loadElement);

            XmlElement modelElement = myScript.CreateElement("model");
            modelElement.SetAttribute("input", "");
            loadElement.AppendChild(modelElement);

            XmlElement buildElement = myScript.CreateElement("build");
            rootElement.AppendChild(buildElement);

            XmlElement mcsElement = myScript.CreateElement("minimal-cutsets");
            mcsElement.SetAttribute("top-event", "");
            mcsElement.SetAttribute("handle", "MCS");
            if (maximumorder != "" && maximumorder != "0")
            {
                mcsElement.SetAttribute("maximum-order", maximumorder); //割集深度
            }
            buildElement.AppendChild(mcsElement);

            XmlElement setElement = myScript.CreateElement("set");
            rootElement.AppendChild(setElement);

            XmlElement opt1Element = myScript.CreateElement("option");
            opt1Element.SetAttribute("name", "print-minimal-cutset-rank");
            opt1Element.SetAttribute("value", "on");
            setElement.AppendChild(opt1Element);

            XmlElement opt2Element = myScript.CreateElement("option");
            opt2Element.SetAttribute("name", "print-minimal-cutset-order");
            opt2Element.SetAttribute("value", "on");
            setElement.AppendChild(opt2Element);

            XmlElement opt3Element = myScript.CreateElement("option");
            opt3Element.SetAttribute("name", "print-minimal-cutset-probability");
            opt3Element.SetAttribute("value", "on");
            setElement.AppendChild(opt3Element);

            XmlElement opt4Element = myScript.CreateElement("option");
            opt4Element.SetAttribute("name", "print-minimal-cutset-contribution");
            opt4Element.SetAttribute("value", "on");
            setElement.AppendChild(opt4Element);

            XmlElement printElement = myScript.CreateElement("print");
            rootElement.AppendChild(printElement);

            XmlElement mcssElement = myScript.CreateElement("minimal-cutsets");
            mcssElement.SetAttribute("top-event", "");
            mcssElement.SetAttribute("handle", "MCS");

            if (maximumorder != "" && maximumorder != "0")
            {
                mcssElement.SetAttribute("maximum-order", maximumorder); //割集深度
            }
            mcssElement.SetAttribute("mission-time", "3");
            mcssElement.SetAttribute("output", "");
            printElement.AppendChild(mcssElement);

            XmlElement computeElement = myScript.CreateElement("compute");
            rootElement.AppendChild(computeElement);

            XmlElement prElement = myScript.CreateElement("probability");
            prElement.SetAttribute("top-event", "");
            prElement.SetAttribute("handle", "MCS");
            prElement.SetAttribute("mission-time", "3");
            prElement.SetAttribute("output", "");
            computeElement.AppendChild(prElement);

            //保存文件
            myScript.Save(scriptPath);
        }

        /// <summary>
        /// 计算割集和概率
        /// </summary>
        /// <param name="system">当前系统</param>
        /// <param name="rootItem">树根节点</param>
        /// <param name="OPENPSAPath">生成的OPENPSA文件路径</param>
        /// <param name="scriptPath">脚本配置文件路径</param>
        public string DFSCalculateProbs(SystemModel system, DrawData rootItem, String OPENPSAPath, string scriptPath, string exePath)
        {
            c_WaitFormProgress.ShowSplashScreen("计算中......", "正在文件初始化（此过程可能持续几分钟，请耐心等待）......");
            count++;
            var errorMessage = string.Empty;
            try
            {
                string claPath = exePath + "\\Temp\\CalData";
                if (!Directory.Exists(claPath))
                    Directory.CreateDirectory(claPath);
                XmlDocument myScript = new XmlDocument();
                myScript.Load(scriptPath);
                errorMessage = CalculateByDrawItem(system, rootItem, myScript, OPENPSAPath, scriptPath, claPath);
                c_WaitFormProgress.CloseSplashScreen();
                return errorMessage;
            }
            catch (Exception ex)
            {
                c_WaitFormProgress.CloseSplashScreen();
                if (ex.HResult != -2146233079 && ex.HResult != -2146232798)
                {
                    return errorMessage + ex.Message;
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// 通过节点递归生成概率值和割集
        /// </summary>
        /// <param name="system">当前系统</param>
        /// <param name="rootItem">树根节点</param>
        /// <param name="myScript">脚本文档对象</param>
        /// <param name="OPENPSAPath">OPENPSA文件路径</param>
        /// <param name="scriptPath">保存路径</param>
        /// <param name="calPath">临时文件路径</param>
        string CalculateByDrawItem(SystemModel sys, DrawData rootItem, XmlDocument myScript, String OPENPSAPath, string scriptPath, string calPath)
        {
            bool check = false;
            if (rootItem.IsGateType && rootItem.Children?.Count == 1)
            {
                foreach (DrawData grandSonC in rootItem.Children)
                {
                    if (grandSonC.Type == DrawType.HouseEvent)
                    {
                        check = true;
                    }
                }
            }
            if (check)
            {
                return "";
            }

            count++;

            var errorMessage = string.Empty;
            if (rootItem.Type == DrawType.AndGate || rootItem.Type == DrawType.OrGate || rootItem.Type == DrawType.PriorityAndGate || rootItem.Type == DrawType.RemarksGate || rootItem.Type == DrawType.XORGate || rootItem.Type == DrawType.VotingGate)
            {
                rootItem.QValue = "";
                //如果没有子节点，则不用计算
                if (rootItem.Children == null || rootItem.Children.Count <= 0)
                    return "没有可计算的子节点";

                string szmcs = string.Empty;
                string szpr = string.Empty;
                //对以该节点作为顶事件的子树进行计算
                XmlNode headerNode = myScript.FirstChild;
                XmlNode rootNode = headerNode.NextSibling;
                XmlNodeList firstLevelChildList = rootNode.ChildNodes;

                foreach (XmlNode xmlNode in firstLevelChildList)
                {
                    //设置OPENPSAFile的路径
                    if (xmlNode.Name.Equals("load"))
                    {
                        XmlNode modelNode = xmlNode.FirstChild;
                        modelNode.Attributes["input"].Value = OPENPSAPath;
                    }
                    //设置顶事件来计算最小割集
                    if (xmlNode.Name.Equals("build"))
                    {
                        XmlNode mcNode = xmlNode.FirstChild;
                        mcNode.Attributes["top-event"].Value = dic_Defined_Guid[rootItem.ThisGuid];
                    }
                    //设置最小割集结果文件的顶事件和输出文件名
                    if (xmlNode.Name.Equals("print"))
                    {
                        XmlNode mcNode = xmlNode.FirstChild;
                        mcNode.Attributes["top-event"].Value = dic_Defined_Guid[rootItem.ThisGuid];
                        szmcs = calPath + "\\" + dic_Defined_Guid[rootItem.ThisGuid] + ".mcs";
                        mcNode.Attributes["output"].Value = szmcs;
                        mcNode.Attributes["mission-time"].Value = "3";
                    }
                    //设置顶事件概率文件的顶事件和输出文件名
                    if (xmlNode.Name.Equals("compute"))
                    {
                        XmlNode prNode = xmlNode.FirstChild;
                        prNode.Attributes["top-event"].Value = dic_Defined_Guid[rootItem.ThisGuid];
                        szpr = calPath + "\\" + dic_Defined_Guid[rootItem.ThisGuid] + ".pr";
                        prNode.Attributes["output"].Value = szpr;
                        prNode.Attributes["mission-time"].Value = "3";
                    }
                }
                //XmlTextWriter writer = new XmlTextWriter(fs, Encoding.UTF8);
                // writer.Formatting = Formatting.Indented;
                myScript.Save(scriptPath);

                //调用xfta.dll对故障树进行计算
                int iRet = Xftar.Execute(1, new String[] { scriptPath });

                //读取生成概率值文件和割集数据文件

                if (File.Exists(szpr) == false)
                {
                    errorMessage = " 计算失败，未能找到文件：" + szpr;
                    return errorMessage;
                }

                if (File.Exists(szmcs) == false)
                {
                    errorMessage = " 计算失败，未能找到文件：" + szmcs;
                    return errorMessage;
                }

                c_WaitFormProgress.WaitSP.SetWaitFormDescription("概率文件解析中......");
                string[] Probs = File.ReadAllLines(szpr);
                errorMessage = GetProbability(Probs, rootItem);

                c_WaitFormProgress.WaitSP.SetWaitFormDescription("割集文件解析中......");
                string[] mcs = File.ReadAllLines(szmcs);
                errorMessage = GetCutset(sys, mcs, rootItem);

                if (errorMessage != "")
                {
                    return errorMessage;
                }
            }
            else if (rootItem.Type == DrawType.TransferInGate)
            {
                DrawData transGate = sys.GetTranferGateByName(rootItem.Identifier);
                if (transGate == null)
                {
                    return string.Empty;
                }
                errorMessage = CalculateByDrawItem(sys, transGate, myScript, OPENPSAPath, scriptPath, calPath);

                if (errorMessage != "")
                {
                    return errorMessage;
                }
            }

            if (rootItem.Children != null)
            {
                foreach (DrawData grandSon in rootItem.Children)
                {
                    errorMessage = CalculateByDrawItem(sys, grandSon, myScript, OPENPSAPath, scriptPath, calPath);

                    if (errorMessage != "")
                    {
                        return errorMessage;
                    }
                }
            }
            return errorMessage;
        }

        /// <summary>
        /// 获取概率值
        /// </summary>
        /// <param name="szPath">文件路径</param>
        /// <param name="item">要赋值的数据对象</param>
        private string GetProbability(string[] Probs, DrawData item)
        {
            var errorMessage = string.Empty;
            try
            {
                string[] lines = Probs.Where(d => d.Trim().StartsWith(dic_Defined_Guid[item.ThisGuid])).ToArray();
                int j = 0;
                long stepTime = 0;
                long AllStep = 0;
                foreach (string linei in lines)
                {
                    j += 1;
                    Stopwatch sw = Stopwatch.StartNew();
                    sw.Start();
                    c_WaitFormProgress.WaitSP.SetWaitFormDescription(item.Identifier + "：概率计算中......");
                    c_WaitFormProgress.RunProgressBar(j, lines.Length);
                    string line = linei.Trim();
                    string[] arr = line.Split('\t');
                    if (arr.Count() < 3)
                        continue;

                    decimal temporary;
                    if (decimal.TryParse(arr[2], System.Globalization.NumberStyles.Float, null, out temporary) && temporary >= 0)
                    {
                        item.QValue = temporary.ToString("E");
                    }
                    sw.Stop();
                    AllStep += sw.ElapsedMilliseconds;
                    stepTime = AllStep / j;
                }
            }
            catch
            {
                errorMessage = $"计算失败，来自于节点：{item.Identifier}";
            }
            return errorMessage;
        }

        /// <summary>
        /// 获取割集值
        /// </summary>
        /// <param name="szPath">文件路径</param>
        /// <param name="item">要赋值的数据对象</param>
        string GetCutset(SystemModel sys, string[] mcs, DrawData item)
        {
            var errorMessage = string.Empty;
            try
            {
                item.Cutset.Clear();
                int j = 0;
                List<DrawData> das = item.GetAllData(sys, true);

                mcs = mcs.Where(d => d.Trim() != "" && d.Trim().Split('\t').Length >= 5).ToArray();

                foreach (string linei in mcs)
                {
                    j += 1;
                    if (j % 1000 == 0 || mcs.Length - j < 20)
                    {
                        c_WaitFormProgress.WaitSP.SetWaitFormDescription(item.Identifier + "：割集计算中......");
                        c_WaitFormProgress.RunProgressBar(j, mcs.Length);
                    }
                    string line = linei.Trim();
                    if (line == "")
                        continue;
                    string[] arr = line.Split('\t');

                    if (arr.Count() < 5)//没有事件或者事件同时发生个数超过5个的排除
                        break;
                    //概率值
                    OneCutsetModel onecutset = new OneCutsetModel();
                    OneCutsetModel onecutset_Real = new OneCutsetModel();
                    decimal temporary = 0;
                    decimal temporary2 = 0;
                    try
                    {
                        if (decimal.TryParse(arr[2], System.Globalization.NumberStyles.Float, null, out temporary) && temporary >= 0)
                        {
                            onecutset.szProb = temporary.ToString("E");
                            onecutset_Real.szProb = temporary.ToString("E");
                        }

                        if (decimal.TryParse(arr[3], System.Globalization.NumberStyles.Float, null, out temporary2) && temporary2 >= 0)
                        {
                            onecutset.szContri = temporary2.ToString("E");
                            onecutset_Real.szContri = temporary2.ToString("E");
                        }
                    }
                    catch (Exception ex)
                    {
                    }

                    if (temporary < 1e-20m)//过滤掉低概率割集
                    {
                        continue;
                    }

                    //从4后都是事件
                    for (int i = 4; i < arr.Count(); i++)
                    {
                        if (arr[i].Trim() == "")
                            continue;

                        try
                        {
                            DrawData da = das.Find(o => dic_Defined_Guid[o.ThisGuid] == arr[i]);
                            if (da != null)
                            {
                                onecutset.Add(dic_Defined_Guid[da.ThisGuid]);
                            }
                            else
                            {
                                onecutset.Add(arr[i]);
                            }

                            if (da != null)
                            {
                                onecutset_Real.Add(da.Identifier);
                            }
                            else
                            {
                                onecutset_Real.Add(arr[i]);
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                    item.Cutset.AddOneCutset(onecutset);
                    item.Cutset.AddOneCutset_Real(onecutset_Real);
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"计算失败，来自于节点：{item.Identifier}{ex.Message}";
            }
            return errorMessage;
        }
    }
}
