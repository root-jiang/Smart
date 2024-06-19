using FaultTreeAnalysis.Model.Enum;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace FaultTreeAnalysis.CommonLib.CommonTool
{
    /// <summary>
    /// 这个只是编程期间用于程序，导入导出一些东西或测试用的小工具
    /// </summary>
    class ProgramerTmpTool
    {
        /// <summary>
        /// 导出所有dev内部自带的图片
        /// </summary>
        /// <param name="DestPath">完全路径名称</param>
        public static void ExportDevImages(string DestPath)
        {
            if (!Directory.Exists(DestPath))
            {
                Directory.CreateDirectory(DestPath);
            }
            foreach (string str in DevExpress.Images.ImageResourceCache.Default.GetAllResourceKeys())
            {
                try
                {
                    using (Stream img = DevExpress.Images.ImageResourceCache.Default.GetResource(str))
                    {
                        Image i = Image.FromStream(img);
                        string name = str.Substring(str.LastIndexOf('/'), str.Length - str.LastIndexOf('/'));
                        string s = DestPath + "/" + str.Substring(0, str.LastIndexOf('/'));
                        if (!Directory.Exists(s))
                        {
                            Directory.CreateDirectory(s);
                        }
                        i.Save(s + name);
                    }
                }
                catch (Exception)
                { }
            }
        }

        /// <summary>
        /// 获取测试用的Drawdata数据，总数为x*y+1
        /// </summary>
        /// <param name="Xnum">方向个数</param>
        /// <param name="Ynum">y方向个数</param>
        /// <returns></returns>
        public static DrawData GetTestDrawData(int Xnum, int Ynum)
        {
            int id = 0;
            DrawData root = new DrawData();
            root.Identifier = id++.ToString();
            for (int i = 0; i < Xnum; i++)
            {
                DrawData childX = new DrawData();
                childX.Identifier = id++.ToString();
                childX.Parent = root;
                root.Children.Add(childX);
                for (int j = 1; j < Ynum; j++)
                {
                    DrawData childY = new DrawData();
                    childY.Identifier = id++.ToString();
                    childY.Parent = childX;
                    childX.Children.Add(childY);
                    childX = childY;
                }
            }
            return root;
        }
    }
}
