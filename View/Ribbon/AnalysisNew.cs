using DevExpress.XtraSplashScreen;
using FaultTreeAnalysis.Common;
using FaultTreeAnalysis.Model.Data;
using FaultTreeAnalysis.Model.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace FaultTreeAnalysis.View.Ribbon
{
    /// <summary>
    /// 故障树计算算法
    /// </summary>
    public class AnalysisNew
    {
        public int EventNum = 0;
        public Dictionary<DrawData, string> AllRootsCalsList = new Dictionary<DrawData, string>();//所有Gate
        public Dictionary<DrawData, List<string>> AllGJCalsList = new Dictionary<DrawData, List<string>>();//所有最小割级
        public Dictionary<string, string> AllEventCalsList = new Dictionary<string, string>();//所有Event
        public Dictionary<DrawData, string> AllEventList = new Dictionary<DrawData, string>();
        public Dictionary<DrawData, double> AllGLList = new Dictionary<DrawData, double>();
        public AnalysisNewCal NewCal = new AnalysisNewCal();
        public List<string> bz = new List<string>();

        /// <summary>
        /// 计算割集和概率
        /// </summary>
        /// <param name="system">当前系统</param>
        /// <param name="rootItem">树根节点</param>
        public void DFSCalculateProbs(SystemModel system, DrawData rootItem)
        {
            NewCal = new AnalysisNewCal();
            var errorMessage = string.Empty;
            try
            { 
                //先递归出所有Gate点对应的公式（等式）
                AllRootsCalsList.Clear();
                AllEventCalsList.Clear();
                AllGJCalsList.Clear();
                AllEventList.Clear();
                AllGLList.Clear();
                rootItem.Level = 1;
                CalculateByDrawItem(system, rootItem);

                //根据Level倒序排列
                AllRootsCalsList = AllRootsCalsList.OrderByDescending(d => d.Key.Level).ToDictionary(p => p.Key, o => o.Value);

                //迭代替换所有公式中的Gate直到只剩基本事件(用代号T替换)
                TranCal(AllRootsCalsList.ToDictionary(p => p.Key, o => o.Value)); 
            }
            catch (Exception ex)
            {
                if (ex.HResult != -2146233079 && ex.HResult != -2146232798)
                {
                    MsgBox.Show(errorMessage + ex.Message);
                }
            }
        }

        /// <summary>
        /// 去括号算法
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public string GetList(string root)
        {
            List<string> newList_a = new List<string>();//加法
            List<string> newList_c = new List<string>();//乘法
            try
            {
                string str1 = "";
                if (root.Contains("*"))
                {
                    if (root.Contains("M") == false)
                    {
                        return root;
                    }

                    string[] ss = root.Split(new char[] { '*' });
                    for (int i = ss.Length - 1; i >= 0; i--)
                    {
                        if (ss[i].Contains("M"))
                        {
                            ss[i] = NewCal.calList[ss[i]];
                        }
                    }

                    while (true)
                    {
                        List<string> NewSS1 = new List<string>();//加集合
                        List<string> NewSS2 = new List<string>();//乘集合

                        bool ck = false;//乘法
                        for (int i = 0; i <= ss.Length - 1; i += 2)
                        {
                            for (int j = 0; j <= ss.Length - 1; j++)
                            {
                                if (j - i == 1)
                                {
                                    if (ss[i].Contains("+") && ss[j].Contains("+"))
                                    {
                                        foreach (string s1 in ss[i].Split(new char[] { '+' }))
                                        {
                                            foreach (string s2 in ss[j].Split(new char[] { '+' }))
                                            {
                                                NewSS1.Add(s1 + "*" + s2);
                                            }
                                        }
                                        ck = true;
                                    }
                                    else if (ss[i].Contains("*") && ss[j].Contains("*"))
                                    {
                                        NewSS1.Add(ss[i] + "*" + ss[j]);
                                        ck = false;
                                    }
                                    else if (ss[i].Contains("+") && ss[j].Contains("*"))
                                    {
                                        foreach (string s1 in ss[i].Split(new char[] { '+' }))
                                        {
                                            NewSS1.Add(s1 + "*" + ss[j]);
                                        }
                                        ck = true;
                                    }
                                    else if (ss[i].Contains("*") && ss[j].Contains("+"))
                                    {
                                        foreach (string s2 in ss[j].Split(new char[] { '+' }))
                                        {
                                            NewSS1.Add(ss[i] + "*" + s2);
                                        }
                                        ck = true;
                                    }
                                    else if (ss[i].Contains("*") == false && ss[i].Contains("+") == false && ss[j].Contains("+"))
                                    {
                                        foreach (string s2 in ss[j].Split(new char[] { '+' }))
                                        {
                                            NewSS1.Add(ss[i] + "*" + s2);
                                        }
                                        ck = true;
                                    }
                                    else if (ss[i].Contains("*") == false && ss[i].Contains("+") == false && ss[j].Contains("*"))
                                    {
                                        NewSS1.Add(ss[i] + "*" + ss[j]);
                                        ck = false;
                                    }
                                    else if (ss[j].Contains("*") == false && ss[j].Contains("+") == false && ss[i].Contains("+"))
                                    {
                                        foreach (string s1 in ss[i].Split(new char[] { '+' }))
                                        {
                                            NewSS1.Add(s1 + "*" + ss[j]);
                                        }
                                        ck = true;
                                    }
                                    else if (ss[j].Contains("*") == false && ss[j].Contains("+") == false && ss[i].Contains("*"))
                                    {
                                        NewSS1.Add(ss[i] + "*" + ss[j]);
                                        ck = false;
                                    }
                                    else if (ss[i].Contains("*") == false && ss[i].Contains("+") == false && ss[j].Contains("*") == false && ss[j].Contains("+") == false)
                                    {
                                        NewSS1.Add(ss[i] + "*" + ss[j]);
                                        ck = false;
                                    }
                                }
                            }

                            //重组乘集合
                            string AllS = "";
                            foreach (string S1 in NewSS1)
                            {
                                if (AllS == "")
                                {
                                    AllS = S1;
                                }
                                else
                                {
                                    if (ck)
                                    {
                                        AllS += "+" + S1;
                                    }
                                    else
                                    {
                                        AllS += "*" + S1;
                                    }
                                }
                            }
                            NewSS2.Add(AllS);

                            if (ss.Length % 2 != 0)
                            {
                                NewSS2.Add(ss[ss.Length - 1]);
                            }
                        }

                        ss = NewSS2.ToArray();

                        if (ss.Length == 1)//乘法两两计算后直到剩一个式子（a+b）*(b+c)*(d+e)=>abd+......+bce
                        {
                            if (ss[0].Contains("M"))
                            {
                                string AllS = "";
                                foreach (string sr in ss[0].Split(new char[] { '+' }))
                                {
                                    if (AllS == "")
                                    {
                                        AllS = GetList(sr);
                                    }
                                    else
                                    {
                                        AllS += "+" + GetList(sr);
                                    }
                                }
                                ss[0] = AllS;
                            }
                            break;
                        }
                    }

                    return ss[0];
                }
                else if (root.Contains("+"))
                {
                    foreach (string str in root.Split(new char[] { '+' }))
                    {
                        if (str1 == "")
                        {
                            str1 = GetList(str);
                        }
                        else
                        {
                            str1 += "+" + GetList(str);
                        }
                    }
                }
                else
                {
                    if (root.Contains("M"))
                    {
                        str1 = GetList(NewCal.calList[root]);
                    }
                    else
                    {
                        str1 = root;
                    }
                }

                return str1;
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// 迭代替换所有公式中的Gate直到只剩基本事件代号T，同时去括号
        /// </summary>
        /// <param name="lst"></param>
        public void TranCal(Dictionary<DrawData, string> lst)
        {
            int index = 0;
            foreach (KeyValuePair<DrawData, string> item in lst)
            {
                //一直替换到只剩基本事件的计算公式 
                if (item.Value.Contains("_Gate"))
                {
                    foreach (string id in item.Value.Split(new char[] { '+', '*', '(', ')' }))
                    {
                        lst = AllRootsCalsList.ToDictionary(p => p.Key, o => o.Value);
                        foreach (KeyValuePair<DrawData, string> item1 in lst)
                        {
                            if (item1.Key.Identifier == id.Replace("_Gate", ""))
                            {
                                if (Regex.Matches(item1.Value, "_Gate").Count == 1 || item1.Value.ToCharArray().Where(x => x == 'T').Count() == 1)
                                {
                                    AllRootsCalsList[item.Key] = AllRootsCalsList[item.Key].Replace(id, item1.Value);
                                }
                                else
                                {
                                    AllRootsCalsList[item.Key] = AllRootsCalsList[item.Key].Replace(id, "(" + item1.Value + ")");
                                }
                            }
                        }
                    }
                }

                foreach (string id in item.Value.Split(new char[] { '+', '*', '(', ')' }))
                {
                    if (id.Contains("_Event"))
                    {
                        if (AllEventCalsList[id].ToCharArray().Where(x => x == 'T').Count() == 1)
                        {
                            AllRootsCalsList[item.Key] = AllRootsCalsList[item.Key].Replace(id, AllEventCalsList[id]);
                        }
                        else
                        {
                            AllRootsCalsList[item.Key] = AllRootsCalsList[item.Key].Replace(id, "(" + AllEventCalsList[id] + ")");
                        }
                    }
                }

                string val = AllRootsCalsList[item.Key];
                index += 1;

                SplashScreenManager.ShowDefaultWaitForm("", index.ToString() + "/" + lst.Count);
                NewCal.Calucate(val);//找出所有最小子项calList和替换后的顶点公式(例如M1*M2)

                string rel = GetList(NewCal.rel);//正向迭代去掉括号得到(例如T1*T2*T1+T3*T4...)

                //去掉各个子项中的重复项
                List<string> res = new List<string>();
                foreach (string rs in rel.Split(new char[] { '+' }))
                {
                    List<string> rss = rs.Split(new char[] { '*' }).ToList();
                    rss = rss.Where((x, i) => rss.FindIndex(n => n == x) == i).ToList();
                    string AllS = "";
                    foreach (string S1 in rss)
                    {
                        if (AllS == "")
                        {
                            AllS = S1;
                        }
                        else
                        {
                            AllS += "*" + S1;
                        }
                    }
                    res.Add(AllS);
                }
                res = res.Where((x, i) => res.FindIndex(n => n == x) == i).ToList();

                //找出最小集合
                List<string> newL = new List<string>();
                for (int i = 0; i <= res.Count - 1; i++)
                {
                    bool isContainFlag = false;
                    for (int j = i + 1; j <= res.Count - 1; j++)
                    {
                        List<string> list1 = res[i].Split(new char[] { '*' }).ToList();
                        List<string> list2 = res[j].Split(new char[] { '*' }).ToList();
                        isContainFlag = !list2.Except(list1).Any();
                        if (isContainFlag)
                        {
                            break;
                        }
                    }

                    if (isContainFlag == false)
                    {
                        newL.Add(res[i]);
                    }
                }

                string rr = string.Join("+", newL.ToArray());
                AllRootsCalsList[item.Key] = rr;

                //记录割级
                string GJ_Event = "";
                string GJ_Event_gv = "";
                foreach (string gj in newL)
                {
                    string GJ_Event1 = "{";
                    string GJ_Event_gv1 = "{";
                    foreach (string evv in gj.Split(new char[] { '*' }))
                    {
                        List<DrawData> keys = AllEventList.Where(q => q.Value == evv).Select(q => q.Key).ToList();
                        DrawData Ev = keys[0];

                        if (GJ_Event1 == "{")
                        {
                            GJ_Event1 += Ev.Identifier;
                        }
                        else
                        {
                            GJ_Event1 += "," + Ev.Identifier;
                        }
                        if (GJ_Event_gv1 == "{")
                        {
                            GJ_Event_gv1 += Ev.QValue;
                        }
                        else
                        {
                            GJ_Event_gv1 += "," + Ev.QValue;
                        }
                    }
                    GJ_Event1 += "}";
                    GJ_Event_gv1 += "}";

                    GJ_Event += GJ_Event1;
                    GJ_Event_gv += GJ_Event_gv1;
                }

                List<string> ls = new List<string>();
                ls.Add(GJ_Event);
                ls.Add(GJ_Event_gv);
                AllGJCalsList.Add(item.Key, ls);

                //概率计算(一阶近似值)
                double k = 0;
                foreach (string sc in newL)
                {
                    double k1 = 1;
                    foreach (string sc1 in sc.Split(new char[] { '*' }))
                    {
                        List<DrawData> keys = AllEventList.Where(q => q.Value == sc1).Select(q => q.Key).ToList();
                        DrawData Ev = keys[0];
                        double gl = double.Parse(Ev.QValue);
                        k1 = k1 * gl;
                    }
                    k = k + k1;
                }

                //概率计算(二阶近似值)
                double k_2 = 0;
                for (int i = 0; i <= newL.Count - 1; i++)
                {
                    for (int j = 0; j <= newL.Count - 1; j++)
                    {
                        if (j > i)
                        {
                            double k1 = 1;
                            double k2 = 1;
                            foreach (string sc1 in newL[i].Split(new char[] { '*' }))
                            {
                                List<DrawData> keys = AllEventList.Where(q => q.Value == sc1).Select(q => q.Key).ToList();
                                DrawData Ev = keys[0];
                                double gl = double.Parse(Ev.QValue);
                                k1 = k1 * gl;
                            }
                            foreach (string sc1 in newL[j].Split(new char[] { '*' }))
                            {
                                List<DrawData> keys = AllEventList.Where(q => q.Value == sc1).Select(q => q.Key).ToList();
                                DrawData Ev = keys[0];
                                double gl = double.Parse(Ev.QValue);
                                k2 = k2 * gl;
                            }
                            k_2 = k_2 + k1 * k2;
                        }
                    }
                }

                double k3 = k - k_2 / 2;
                item.Key.QValue = double.Parse(k3.ToString()).ToString("E");
                AllGLList.Add(item.Key, k3);
            }
        }

        /// <summary>
        /// 通过节点递归生成所有割集计算等式
        /// </summary>
        /// <param name="system">当前系统</param>
        /// <param name="rootItem">当前指定的根节点</param>
        public void CalculateByDrawItem(SystemModel system, DrawData rootItem)
        {
            var errorMessage = string.Empty;
            if (rootItem.Type.ToString().Contains("Gate"))
            {
                if (rootItem.Parent != null)
                {
                    rootItem.Level = rootItem.Parent.Level + 1;
                }

                rootItem.QValue = "";
                //如果没有子节点，则不用计算
                if (rootItem.Children == null || rootItem.Children.Count <= 0)
                {
                    return;
                }

                //记录以当前门为顶的计算公式
                string f = "";
                if (rootItem.Type == DrawType.AndGate)
                {
                    foreach (DrawData grandSon in rootItem.Children)
                    {
                        if (grandSon.Type.ToString().Contains("Gate"))
                        {
                            f += "*" + grandSon.Identifier + "_Gate";
                        }
                        else
                        {
                            f += "*" + grandSon.Identifier + "_Event";
                        }
                    }
                }
                else if (rootItem.Type == DrawType.OrGate)
                {
                    foreach (DrawData grandSon in rootItem.Children)
                    {
                        if (grandSon.Type.ToString().Contains("Gate"))
                        {
                            f += "+" + grandSon.Identifier + "_Gate";
                        }
                        else
                        {
                            f += "+" + grandSon.Identifier + "_Event";
                        }
                    }
                }
                f = f.Trim(new char[] { '*' }).Trim(new char[] { '+' });
                AllRootsCalsList.Add(rootItem, f);
            }
            else if (rootItem.Type == DrawType.TransferInGate)
            {
                if (rootItem.Parent != null)
                {
                    rootItem.Level = rootItem.Parent.Level + 1;
                }

                DrawData transGate = system.GetTranferGateByName(rootItem.Identifier);
                if (transGate == null)
                {
                    return;
                }
                CalculateByDrawItem(system, transGate);
            }
            else if (rootItem.Type.ToString().Contains("Event"))
            {
                if (AllEventCalsList.ContainsKey(rootItem.Identifier + "_Event") == false)
                {
                    EventNum += 1;
                    AllEventCalsList.Add(rootItem.Identifier + "_Event", "T" + EventNum.ToString());
                    AllEventList.Add(rootItem, "T" + EventNum.ToString());
                }
            }

            if (rootItem.Children != null)
            {
                foreach (DrawData grandSon in rootItem.Children)
                {
                    CalculateByDrawItem(system, grandSon);
                }
            }
            return;
        }
    }
}
