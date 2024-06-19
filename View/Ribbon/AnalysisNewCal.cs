using System;
using System.Collections.Generic;

namespace FaultTreeAnalysis.View.Ribbon
{
    public class AnalysisNewCal
    {
        public Dictionary<string, string> calList = new Dictionary<string, string>();
        public int ct = 0;
        public string rel = "";

        /// <summary>
        /// 找出所有最小子项
        /// </summary>
        /// <param name="expression"></param>
        public void Calucate(string expression)
        { 
            try
            {
                for (int i = 0; i <= expression.Length - 1; i++)
                {
                    if (expression[i] == '(')//找到左括号时寻找下一个右括号，如果这之间未出现任何左括号，就是最简表达式
                    {
                        int i2 = 0;//下一个)
                        for (int j = i + 1; j <= expression.Length - 1; j++)
                        {
                            if (expression[j] == ')')
                            {
                                i2 = j;
                                break;
                            }
                            else if (expression[j] == '(')
                            {
                                break;
                            }
                        }
                        if (i2 != 0)//确认存在最简表达式
                        {
                            string e_ss = expression.Substring(i + 1, i2 - i - 1);
                            ct += 1;
                            calList.Add("M" + ct.ToString(), e_ss);

                            string M1 = expression.Substring(0, i);
                            string M2 = expression.Substring(i2 + 1, expression.Length - i2 - 1);
                            expression = M1 + "M" + ct.ToString() + M2;
                        }
                    }
                }

                if (expression.Contains("("))//还存在括号就继续替换
                {
                    Calucate(expression);
                }
                else
                {
                    rel = expression;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
