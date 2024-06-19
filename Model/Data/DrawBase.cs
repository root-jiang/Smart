using FaultTreeAnalysis.Common;
using FaultTreeAnalysis.Model.Enum;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using System.Text;

namespace FaultTreeAnalysis
{
    /// <summary>
    /// 各种FTA图形元素的绘制类,图形比例决定图形最好看的宽高比，而控件最合适大小是比例宽+线宽，比例高+线宽
    /// </summary>
    class DrawBase
    {
        /// <summary>
        /// 默认的画笔的宽度
        /// </summary>
        private const int WIDTH_PEN = 1;

        /// <summary>
        /// 根据类型在指定画布，指定位置，指定控件宽（默认宽高一致）绘制一个图形（包含文本）
        /// </summary>
        /// <param name="type">图形类型</param>
        /// <param name="grphs">图形对象</param>
        /// <param name="x">绘制的x坐标起点</param>
        /// <param name="y">绘制的y坐标起点</param>
        /// <param name="width">控件宽度(包含了线宽)</param>
        /// <param name="Comment">描述字符串，显示在上矩形区域里</param>
        /// <param name="id">id字符串，显示在中间矩形区域里</param>
        /// <param name="QValue">概率字符串，显示在下矩形区域里</param>
        /// <param name="Is_TransGate">是否是转移门（多绘制一个右侧的三角形）</param>
        /// <param name="rePeatedEventColor">如果是重复事件可以指定颜色，将填充基本图形的区域</param>
        /// <param name="rePeats">重复次数，将显示在左上角的字符串</param>
        /// <param name="width_Pen">画笔宽度</param>
        public static void DrawComponent(DrawData drawData, DrawType type, Graphics grphs, float x, float y, float width, string Comment = null, string id = null, string QValue = null, bool Is_TransGate = false, Color? rePeatedEventColor = null, int? rePeats = null, float width_Pen = WIDTH_PEN)
        {
            //根据组件大小，设置图形的最大宽高                       
            float width_Graphic = width - width_Pen;
            DrawBase.DrawComponent(drawData, type, grphs, x, y, width_Graphic, width_Graphic * 3 / 8, width_Graphic / 8, width_Graphic / 2, Comment, id, QValue, "Arial", width_Graphic / 15, Is_TransGate, rePeatedEventColor, rePeats, null, width_Pen);
        }

        /// <summary>
        /// 根据类型在指定画布，指定位置，指定控件宽（默认宽高一致）绘制一个图形（包含文本）
        /// </summary>
        /// <param name="type">图形类型</param>
        /// <param name="grphs">图形对象</param>
        /// <param name="x">绘制的x坐标起点</param>
        /// <param name="y">绘制的y坐标起点</param>
        /// <param name="width">控件宽度(包含了线宽)</param>
        /// <param name="Comment">描述字符串，显示在上矩形区域里</param>
        /// <param name="id">id字符串，显示在中间矩形区域里</param>
        /// <param name="QValue">概率字符串，显示在下矩形区域里</param>
        /// <param name="Is_TransGate">是否是转移门（多绘制一个右侧的三角形）</param>
        /// <param name="rePeatedEventColor">如果是重复事件可以指定颜色，将填充基本图形的区域</param>
        /// <param name="rePeats">重复次数，将显示在左上角的字符串</param>
        /// <param name="width_Pen">画笔宽度</param>
        public static void DrawComponent_New(DrawData drawData, DrawType type, Graphics grphs, float x, float y, float width, string Comment = null, string id = null, string QValue = null, bool Is_TransGate = false, Color? rePeatedEventColor = null, int? rePeats = null, float width_Pen = WIDTH_PEN)
        {
            //根据组件大小，设置图形的最大宽高                       
            float width_Graphic = width - width_Pen;
            DrawBase.DrawComponent(drawData, type, grphs, x, y, width_Graphic, width_Graphic * 3 / 8, width_Graphic / 8, width_Graphic / 2, Comment, id, QValue, "Arial", 5, Is_TransGate, rePeatedEventColor, rePeats, null, width_Pen);
        }

        /// <summary>
        /// 根据类型在指定画布，指定位置，指定图形的宽高绘制一个图形（包含文本）
        /// </summary>
        /// <param name="type">图形类型</param>
        /// <param name="grphs">图形对象</param>
        /// <param name="x">绘制的x坐标起点</param>
        /// <param name="y">绘制的y坐标起点</param>
        /// <param name="width_Graphic">图形的宽度（控件宽度减去画笔宽度）</param>
        /// <param name="height_Graphic_TopRect">上部显示文本的矩形高度</param>
        /// <param name="height_Graphic_CenterRect">中部显示文本的矩形高度</param>
        /// <param name="height_Graphic_BottomRect">下部显示文本的矩形高度</param>
        /// <param name="Comment">描述字符串，显示在上矩形区域里</param>
        /// <param name="id">id字符串，显示在中间矩形区域里</param>
        /// <param name="QValue">概率字符串，显示在下矩形区域里</param>
        /// <param name="font_Name">绘制字符串时用的字体名字</param>
        /// <param name="font_Size">绘制字符串时用的字体大小</param>
        /// <param name="Is_TransGate">是否是转移门（多绘制一个右侧的三角形）</param>
        /// <param name="rePeatedEventColor">如果是重复事件可以指定颜色，将填充基本图形的区域</param>
        /// <param name="rePeats">重复次数，将显示在左上角的字符串</param>
        /// <param name="color_Pen">画笔颜色，就是线条颜色</param>
        /// <param name="width_Pen">画笔宽度</param>
        public static void DrawComponent(DrawData drawData, DrawType type, Graphics grphs, float x, float y, float width_Graphic, float height_Graphic_TopRect, float height_Graphic_CenterRect, float height_Graphic_BottomRect, string Comment = null, string id = null, string QValue = null, string font_Name = "Arial", float font_Size = 8, bool Is_TransGate = false, Color? rePeatedEventColor = null, int? rePeats = null, Color? color_Pen = null, float width_Pen = WIDTH_PEN)
        {
            if (width_Graphic <= 0 || height_Graphic_TopRect <= 0 || height_Graphic_CenterRect <= 0 || height_Graphic_BottomRect <= 0) return;
            if (color_Pen == null) color_Pen = Color.Black;
            using (Matrix matrix_Before = grphs.Transform.Clone())
            {
                //图形设置
                using (Pen pen = new Pen((Color)color_Pen, width_Pen))
                {
                    grphs.SmoothingMode = SmoothingMode.HighQuality;//抗锯齿
                    grphs.PixelOffsetMode = PixelOffsetMode.HighSpeed;

                    //平移矩阵
                    grphs.TranslateTransform(x + (width_Pen) / 2, y + (width_Pen) / 2);

                    if (rePeats != null && rePeatedEventColor != null)
                    {
                        //重复事件字符串
                        string text_Repeats = rePeats + "Repeats";
                        using (Font f_tmp = new Font(font_Name, font_Size))
                        {
                            SizeF size_Repeats = grphs.MeasureString(text_Repeats, f_tmp);
                            if (size_Repeats.Width <= width_Graphic)//字体太大，这里就不画了
                            {
                                using (var brush = new SolidBrush(pen.Color))
                                {
                                    grphs.DrawString(text_Repeats, f_tmp, brush, 0, -size_Repeats.Height);
                                }
                            }
                        }
                    }

                    //绘制链接门（也是基本事件模型）
                    if (drawData != null && drawData.LinkPath != "")
                    {
                        //重复事件字符串
                        string text_Link = "Link";
                        using (Font f_tmp = new Font(font_Name, font_Size, FontStyle.Underline))
                        {
                            SizeF size_Repeats = grphs.MeasureString(text_Link, f_tmp);
                            if (size_Repeats.Width <= width_Graphic)//字体太大，这里就不画了
                            {
                                using (var brush = new SolidBrush(Color.Blue))
                                {
                                    grphs.DrawString(text_Link, f_tmp, brush, width_Graphic - size_Repeats.Width, -size_Repeats.Height - 1);
                                }
                            }
                        }
                    }

                    //画矩形说明框
                    grphs.DrawRectangle(pen, 0, 0, width_Graphic, height_Graphic_TopRect + height_Graphic_CenterRect);

                    //画分隔线
                    grphs.DrawLine(pen, 0, height_Graphic_TopRect, width_Graphic, height_Graphic_TopRect);

                    //画Comment文字
                    if (!string.IsNullOrEmpty(Comment))
                    {
                        if (drawData != null && drawData.LinkPath != "")
                        {
                            DrawText(grphs, new RectangleF(0, pen.Width, width_Graphic, height_Graphic_TopRect), font_Name, font_Size, "[" + drawData.LinkPath + "]:" + Comment);
                        }
                        else
                        {
                            DrawText(grphs, new RectangleF(0, pen.Width, width_Graphic, height_Graphic_TopRect), font_Name, font_Size, Comment);
                        }
                    }

                    //画标识符文字
                    if (!string.IsNullOrEmpty(id))
                    {
                        DrawText(grphs, new RectangleF(pen.Width, height_Graphic_TopRect + pen.Width, width_Graphic, height_Graphic_CenterRect), font_Name, font_Size, id);
                    }

                    //上连接线
                    grphs.DrawLine(pen, width_Graphic / 2, height_Graphic_TopRect + height_Graphic_CenterRect, width_Graphic / 2, height_Graphic_TopRect + height_Graphic_CenterRect + height_Graphic_BottomRect / 8);

                    //得到基本xx门的大小
                    float height_Graphic_Part = height_Graphic_BottomRect * 13 / 16;
                    if (type == DrawType.RemarksGate) height_Graphic_Part = height_Graphic_BottomRect * 7 / 16;
                    Size size_Graphic_Part = GetGraphicFitSize(type);
                    float width_Graphic_Part = height_Graphic_Part * size_Graphic_Part.Width / size_Graphic_Part.Height;

                    if (width_Graphic_Part > width_Graphic)
                    {
                        width_Graphic_Part = width_Graphic;
                        height_Graphic_Part = width_Graphic_Part * size_Graphic_Part.Height / size_Graphic_Part.Width;
                    }

                    //转移门多绘制一个三角形
                    //比例58:15三角形
                    if (Is_TransGate)
                    {
                        grphs.DrawLine(pen, width_Graphic / 2, height_Graphic_TopRect + height_Graphic_CenterRect + height_Graphic_BottomRect / 16, 3 * width_Graphic / 4, height_Graphic_TopRect + height_Graphic_CenterRect + height_Graphic_BottomRect / 16);
                        //矩形上中心点保持不变
                        PointF startTopCenterPolygon = new PointF(3 * width_Graphic / 4, height_Graphic_TopRect + height_Graphic_CenterRect + height_Graphic_BottomRect / 16);
                        float widthPolygon = 2 * width_Graphic / 8;
                        float heightPolygon = 2 * height_Graphic_BottomRect / 16;
                        //以高为基准
                        if (widthPolygon * 15 / 58 > heightPolygon)
                        {
                            widthPolygon = heightPolygon * 58 / 15;
                        }
                        //以宽为基准
                        else
                        {
                            heightPolygon = widthPolygon * 15 / 58;
                        }
                        grphs.DrawPolygon(pen, FormatPoints(startTopCenterPolygon.X, startTopCenterPolygon.Y, startTopCenterPolygon.X - widthPolygon / 2, startTopCenterPolygon.Y + heightPolygon, startTopCenterPolygon.X + widthPolygon / 2, startTopCenterPolygon.Y + heightPolygon));
                    }

                    //平移矩阵
                    grphs.TranslateTransform((width_Graphic - width_Graphic_Part) / 2, height_Graphic_TopRect + height_Graphic_CenterRect + height_Graphic_BottomRect / 8);

                    if (rePeats != null && rePeatedEventColor != null)
                    {
                        using (Brush br = new SolidBrush((Color)rePeatedEventColor))
                        {    //重复事件的填充图形
                            DrawFilledBaseComponent(drawData, type, grphs, pen, br, 0, 0, width_Graphic_Part, height_Graphic_Part, false);
                        }
                    }
                    else
                    {
                        //根据不同组件类型绘制图形
                        DrawBaseComponent(drawData, type, grphs, pen, 0, 0, width_Graphic_Part, height_Graphic_Part, false);
                    }

                    //画有Q的文字，这里默认了文字大小等
                    if (!string.IsNullOrEmpty(QValue))
                    {
                        using (Font font = new Font(font_Name, font_Size))
                        {
                            SizeF qvalue_Size = grphs.MeasureString(QValue, font);
                            float width_Rect = 0;
                            float height_Rect = qvalue_Size.Height + 1;
                            float pos_y = height_Graphic_Part / 2 - qvalue_Size.Height / 2 - 1;
                            width_Rect = qvalue_Size.Width > width_Graphic - 1 ? width_Graphic : qvalue_Size.Width + 1;
                            float pos_x = (width_Graphic_Part - width_Rect) / 2;
                            grphs.DrawRectangle(pen, pos_x, pos_y, width_Rect, height_Rect);
                            grphs.FillRectangle(Brushes.White, pos_x, pos_y, width_Rect, height_Rect);
                            DrawText(grphs, new RectangleF(pos_x, pos_y + pen.Width, width_Rect, height_Rect), font_Name, font_Size, QValue, 0, 0);
                        }
                    }

                    //平移矩阵
                    grphs.TranslateTransform(0, height_Graphic_Part);

                    //画下边线
                    switch (type)
                    {
                        case DrawType.AndGate:
                        case DrawType.PriorityAndGate:
                        case DrawType.XORGate:
                            grphs.DrawLine(pen, width_Graphic_Part / 2, 0, width_Graphic_Part / 2, height_Graphic_BottomRect / 16);
                            break;
                        case DrawType.OrGate:
                            grphs.DrawLine(pen, width_Graphic_Part / 2, -height_Graphic_Part * 10 / 74, width_Graphic_Part / 2, height_Graphic_BottomRect / 16);
                            break;
                        case DrawType.VotingGate:
                            grphs.DrawLine(pen, width_Graphic_Part / 2, -height_Graphic_Part * 1 / 10, width_Graphic_Part / 2, height_Graphic_BottomRect / 16);
                            break;
                        case DrawType.RemarksGate:
                            grphs.DrawLine(pen, width_Graphic_Part / 2, 0, width_Graphic_Part / 2, 7 * height_Graphic_BottomRect / 8 - height_Graphic_Part);
                            break;
                        default:
                            break;
                    }
                    //设置回原来的矩阵
                    grphs.Transform.Dispose();
                    grphs.Transform = matrix_Before;
                }
            }
        }

        #region 绘制文本
        /// <summary>
        /// 在一个指定大小矩形内绘制文本，如果文本过多会把最后一行的结尾3个字符换为...
        /// </summary>
        /// <param name="grphs">图形对象</param>
        /// <param name="rect_X">矩形x位置</param>
        /// <param name="rect_Y">矩形y位置</param>
        /// <param name="width_Rect">一行文字的最大宽度值（矩形宽）</param>
        /// <param name="height_Rect">矩形最大的高</param>
        /// <param name="font_Name">字体名字</param>
        /// <param name="font_Size">字体大小</param>
        /// <param name="text">绘制的文字</param>
        /// <param name="line_distance">行高</param>
        /// <param name="padding">矩形内边距</param>
        private static void DrawText(Graphics grphs, RectangleF rect, string font_Name, float font_Size, string text, float line_distance = 0, float padding = 0)
        {
            using (Font font_Text = new Font(font_Name, font_Size))
            {
                List<string> texts = GetMeasuredString(grphs, rect, font_Text, text, line_distance, padding);
                DrawText(grphs, rect, line_distance, font_Text, texts);
            }
        }

        /// <summary>
        /// 把长字符串根据设置，切割为按空白符分割的字符串列表
        /// </summary>
        /// <param name="grphs">图形对象</param>
        /// <param name="rect">绘制区域</param>
        /// <param name="font_Text">字体对象</param>
        /// <param name="text">要处理的文本</param>
        /// <param name="line_distance">行距离</param>
        /// <param name="padding">矩形内边距</param>
        /// <returns></returns>
        private static List<string> GetMeasuredString(Graphics grphs, RectangleF rect, Font font_Text, string text, float line_distance, float padding)
        {
            List<string> texts = new List<string>();
            //表示当前文本总高度
            float height_Text = 2 * padding;
            float height_Tmp = 0;
            //表示是否需要省略号
            bool Is_TooMuchText = false;
            char[] chars = text.ToCharArray();
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < chars.Length; i++)
            {
                //如果一行已满，那么继续下一行
                if (grphs.MeasureString(builder.ToString() + chars[i], font_Text).Width > (rect.Width - 2 * padding))
                {
                    //如果已超出最大高度，退出循环
                    height_Tmp = grphs.MeasureString(builder.ToString(), font_Text).Height;
                    if ((height_Text + height_Tmp) > rect.Height && texts.Count > 0)//保证必须要有一行数据
                    {
                        Is_TooMuchText = true;
                        builder.Clear();
                        if (texts.Count > 0) height_Text -= line_distance;//最后一行不要行距
                        break;
                    }
                    //还没到最大高度,发生了字符切割,2个字母是连续的
                    if (!char.IsWhiteSpace(builder[builder.Length - 1]) && !char.IsWhiteSpace(chars[i]))
                    {
                        int index = -1;
                        for (int j = builder.Length - 1; j >= 0; j--)
                        {
                            if (char.IsWhiteSpace(builder[j]))
                            {
                                index = j;
                                break;
                            }
                        }
                        //找到最后的空白符
                        if (index > 0)
                        {
                            i = i - (builder.Length - index);
                            builder.Remove(index, builder.Length - index);
                        }
                    }
                    texts.Add(builder.ToString());
                    height_Text += height_Tmp + line_distance;
                    builder.Clear();
                }
                builder.Append(chars[i]);
            }

            if (builder.Length > 0)
            {
                //如果已超出最大高度，舍弃最后一行数据
                height_Tmp = grphs.MeasureString(builder.ToString(), font_Text).Height;
                if ((height_Text + height_Tmp) > rect.Height && texts.Count > 0)//保证至少有一行数据
                {
                    Is_TooMuchText = true;
                    if (texts.Count > 0) height_Text -= line_distance;//最后一行不要行距
                }
                else
                {
                    //还没到最大高度
                    texts.Add(builder.ToString());
                    height_Text += height_Tmp;         //最后一行不要行距        
                }
                builder.Clear();
            }

            //替换可绘制的最后一行，最后3个字符为...
            if (Is_TooMuchText && texts.Count > 0)
            {
                if (texts[texts.Count - 1].Length > 3)//如果字符数大于3个
                    texts[texts.Count - 1] = texts[texts.Count - 1].Remove(texts[texts.Count - 1].Length - 3, 3) + "...";
            }
            return texts;
        }

        /// <summary>
        /// 根据给出的文本list绘制文本，始终居中显示
        /// </summary>
        /// <param name="grphs">图形对象</param>
        /// <param name="rect">绘制区域</param>
        /// <param name="line_distance">行间距</param>
        /// <param name="font_Text">字体对象</param>
        /// <param name="texts">文本列表</param>
        private static void DrawText(Graphics grphs, RectangleF rect, float line_distance, Font font_Text, List<string> texts)
        {
            if (grphs != null && font_Text != null && texts?.Count > 0)
            {
                //确定文本的起始高度位置
                float start_height = 0;
                foreach (var text in texts)
                {
                    //如果已超出最大高度，舍弃最后一行数据
                    start_height += (grphs.MeasureString(text, font_Text).Height + line_distance);
                }
                start_height -= line_distance;
                start_height = rect.Y + (rect.Height - start_height) / 2;

                //绘制每行的文字
                for (int i = 0; i < texts.Count; i++)
                {
                    SizeF size_Text = grphs.MeasureString(texts[i], font_Text);
                    grphs.DrawString(texts[i], font_Text, Brushes.Black, rect.X + (rect.Width - size_Text.Width) / 2, start_height);
                    start_height += size_Text.Height + line_distance;
                }
            }
        }
        #endregion

        /// <summary>
        /// 在指定位置在画布上画出指定类型的图形
        /// </summary>
        /// <param name="type">图形类型</param>
        /// <param name="grphs">图形对象</param>
        /// <param name="pen">绘制的画笔对象</param>
        /// <param name="x">绘制的起始x坐标</param>
        /// <param name="y">绘制的起始y坐标</param>
        /// <param name="width_Graphic">图形的宽度（不包含画笔宽度）</param>
        /// <param name="height_Graphic">图形的宽度（不包含画笔高度）</param>
        /// <param name="is_FitSizeAndCenter">是否按最合适宽高比例绘制，且绘制后在矩形里居中</param>
        public static void DrawBaseComponent(DrawData data, DrawType type, Graphics grphs, Pen pen, float x, float y, float width_Graphic, float height_Graphic, bool is_FitSizeAndCenter = false)
        {
            if (width_Graphic <= 0 || height_Graphic <= 0) return;

            float width_Pen = pen.Width;
            //如果不是原点，就移动
            using (Matrix matix_Before = grphs.Transform.Clone())
            {
                grphs.TranslateTransform(x, y);
                if (is_FitSizeAndCenter)
                {
                    //判断以宽或高为基准绘制
                    SizeF fit_Size = GetGraphicFitSize(type);
                    float width_Graphic_Tmp = width_Graphic;
                    float height_Graphic_Tmp = width_Graphic_Tmp * fit_Size.Height / fit_Size.Width;
                    if (height_Graphic_Tmp > height_Graphic)
                    {//以高为基准，宽居中
                        width_Graphic_Tmp = height_Graphic * fit_Size.Width / fit_Size.Height;
                        grphs.TranslateTransform((width_Graphic - width_Graphic_Tmp) / 2, 0);
                        width_Graphic = width_Graphic_Tmp;
                    }
                    else
                    {//以宽为基准，高居中
                        grphs.TranslateTransform(0, (height_Graphic - height_Graphic_Tmp) / 2);
                        height_Graphic = height_Graphic_Tmp;
                    }
                }
                switch (type)
                {
                    case DrawType.AndGate:
                        grphs.DrawLines(pen, FormatPoints(0, height_Graphic * 29 / 74, 0, height_Graphic, width_Graphic, height_Graphic, width_Graphic, height_Graphic * 29 / 74));
                        grphs.DrawArc(pen, 0, 0, width_Graphic, width_Graphic, 0, -180);
                        break;
                    case DrawType.OrGate:
                        grphs.DrawLine(pen, 0, height_Graphic * 37 / 74, 0, height_Graphic + width_Pen * 2 / 5);
                        grphs.DrawBezier(pen, new PointF(0, height_Graphic), new PointF(width_Graphic * 26 / 58, height_Graphic * 52 / 74), new PointF(width_Graphic, height_Graphic), new PointF(width_Graphic, height_Graphic));
                        grphs.DrawLine(pen, width_Graphic, height_Graphic + width_Pen * 2 / 5, width_Graphic, height_Graphic * 37 / 74);
                        grphs.DrawBezier(pen, new PointF(width_Graphic / 2 - width_Pen / 4, 0), new PointF(width_Graphic * 48 / 58, height_Graphic * 9 / 74), new PointF(width_Graphic, height_Graphic * 37 / 74 + width_Pen / 4), new PointF(width_Graphic, height_Graphic * 37 / 74 + width_Pen / 4));
                        grphs.DrawBezier(pen, new PointF(width_Graphic / 2 + width_Pen / 4, 0), new PointF(width_Graphic * 10 / 58, height_Graphic * 9 / 74), new PointF(0, height_Graphic * 37 / 74 + width_Pen / 4), new PointF(0, height_Graphic * 37 / 74 + width_Pen / 4));
                        break;
                    case DrawType.BasicEvent:
                        if (data != null && data.LinkPath != "")
                        {
                            // 设置虚线样式
                            Pen dashedPen = new Pen(Color.Blue, 2);
                            dashedPen.DashStyle = DashStyle.Dot;

                            // 圆的中心点和半径
                            float xE = width_Graphic / 2;
                            float yE = height_Graphic / 2;
                            float radius = Math.Min(width_Graphic, height_Graphic);

                            // 绘制虚线圆
                            grphs.DrawEllipse(dashedPen, xE - radius * 0.5f, yE - radius * 0.5f, radius, radius);
                        }
                        else
                        {
                            grphs.DrawArc(pen, 0, 0, width_Graphic, height_Graphic, 0, 360);
                        }
                        break;
                    case DrawType.UndevelopedEvent:
                        grphs.DrawPolygon(pen, FormatPoints(0, height_Graphic / 2, width_Graphic / 2, height_Graphic, width_Graphic, height_Graphic / 2, width_Graphic / 2, 0));
                        break;
                    case DrawType.TransferInGate:
                        grphs.DrawPolygon(pen, FormatPoints(0, height_Graphic, width_Graphic, height_Graphic, width_Graphic / 2, 0));
                        break;
                    case DrawType.LinkGate:
                        grphs.DrawPolygon(pen, FormatPoints(0, height_Graphic, width_Graphic, height_Graphic, width_Graphic / 2, 0));
                        break;
                    case DrawType.HouseEvent:
                        grphs.DrawPolygon(pen, FormatPoints(0, height_Graphic * 33 / 81, 0, height_Graphic, width_Graphic, height_Graphic, width_Graphic, height_Graphic * 33 / 81, width_Graphic / 2, 0));
                        break;
                    case DrawType.PriorityAndGate:
                        grphs.DrawLines(pen, FormatPoints(0, height_Graphic * 29 / 74, 0, height_Graphic, width_Graphic, height_Graphic, width_Graphic, height_Graphic * 29 / 74));
                        grphs.DrawArc(pen, 0, 0, width_Graphic, width_Graphic, 0, -180);
                        grphs.DrawLine(pen, 0, height_Graphic * 64 / 74, width_Graphic, height_Graphic * 64 / 74);
                        break;
                    //case DrawType.InhibitGate:
                    //    grphs.DrawPolygon(pen, FormatPoints(0, height_Graphic * 18 / 72, 0, height_Graphic * 54 / 72, width_Graphic * 29 / 58, height_Graphic, width_Graphic, height_Graphic * 54 / 72, width_Graphic, height_Graphic * 18 / 72, width_Graphic * 29 / 58, 0));
                    //    break;
                    case DrawType.ConditionEvent:
                        grphs.DrawArc(pen, 0, 0, height_Graphic, height_Graphic, 90, 180);
                        grphs.DrawArc(pen, width_Graphic * 10 / 18, 0, height_Graphic, height_Graphic, -90, 180);
                        grphs.DrawLine(pen, width_Graphic * 4 / 18 - width_Pen / 4, 0, width_Graphic * 14 / 18 + width_Pen / 4, 0);
                        grphs.DrawLine(pen, width_Graphic * 4 / 18 - width_Pen / 4, height_Graphic, width_Graphic * 14 / 18 + width_Pen / 4, height_Graphic);
                        break;
                    case DrawType.XORGate:
                        grphs.DrawBezier(pen, new PointF(width_Graphic / 2 + width_Pen / 8, 0), new PointF(width_Graphic * 20 / 100, 0), new PointF(0, height_Graphic + width_Pen / 4), new PointF(0, height_Graphic + width_Pen / 4));
                        grphs.DrawBezier(pen, new PointF(width_Graphic / 2 - width_Pen / 8, 0), new PointF(width_Graphic * 80 / 100, 0), new PointF(width_Graphic, height_Graphic + width_Pen / 4), new PointF(width_Graphic, height_Graphic + width_Pen / 4));
                        grphs.DrawBezier(pen, new PointF(-width_Pen / 3, height_Graphic + width_Pen / 3), new PointF(width_Graphic * 50 / 100, height_Graphic * 81 / 100), new PointF(width_Graphic, height_Graphic + width_Pen / 4), new PointF(width_Graphic, height_Graphic - width_Pen / 8));
                        grphs.DrawLine(pen, 0, height_Graphic, width_Graphic, height_Graphic);
                        break;
                    case DrawType.VotingGate:
                        grphs.DrawBezier(pen, new PointF(width_Graphic / 2 + width_Pen / 8, 0), new PointF(width_Graphic * 20 / 100, 0), new PointF(0, height_Graphic + width_Pen / 4), new PointF(0, height_Graphic + width_Pen / 4));
                        grphs.DrawBezier(pen, new PointF(width_Graphic / 2 - width_Pen / 8, 0), new PointF(width_Graphic * 80 / 100, 0), new PointF(width_Graphic, height_Graphic + width_Pen / 4), new PointF(width_Graphic, height_Graphic + width_Pen / 4));
                        grphs.DrawBezier(pen, new PointF(-width_Pen / 3, height_Graphic + width_Pen / 3), new PointF(width_Graphic * 50 / 100, height_Graphic * 81 / 100), new PointF(width_Graphic, height_Graphic + width_Pen / 4), new PointF(width_Graphic, height_Graphic - width_Pen / 8));
                        break;
                    case DrawType.RemarksGate:
                        grphs.DrawRectangle(pen, 0, 0, width_Graphic, height_Graphic);
                        break;
                    default:
                        break;
                }
                grphs.Transform.Dispose();
                grphs.Transform = matix_Before;
            }
        }

        /// <summary>
        /// 在指定位置在画布上按指定画刷填充出指定类型的图形
        /// </summary>
        /// <param name="type">图形类型</param>
        /// <param name="grphs">图形对象</param>
        /// <param name="pen">画笔对象</param>
        /// <param name="br">画刷对象</param>
        /// <param name="x">绘制起始x坐标</param>
        /// <param name="y">绘制起始y坐标</param>
        /// <param name="width_Graphic">图形宽度（不包含画笔宽度）</param>
        /// <param name="height_Graphic">图形高度（不包含画笔宽度）</param>
        /// <param name="is_FitSizeAndCenter">是否按最合适宽高比例绘制，且绘制后在矩形里居中</param>
        public static void DrawFilledBaseComponent(DrawData drawData, DrawType type, Graphics grphs, Pen pen, Brush br, float x, float y, float width_Graphic, float height_Graphic, bool is_FitSizeAndCenter = false)
        {
            if (width_Graphic <= 0 || height_Graphic <= 0) return;
            float width_Pen = pen.Width;
            //如果不是原点，就移动
            using (Matrix matix_Before = grphs.Transform.Clone())
            {
                grphs.TranslateTransform(x, y);

                if (is_FitSizeAndCenter)
                {
                    //判断以宽或高为基准绘制
                    SizeF fit_Size = GetGraphicFitSize(type);
                    float width_Graphic_Tmp = width_Graphic;
                    float height_Graphic_Tmp = width_Graphic_Tmp * fit_Size.Height / fit_Size.Width;
                    if (height_Graphic_Tmp > height_Graphic)
                    {//以高为基准，宽居中
                        width_Graphic_Tmp = height_Graphic * fit_Size.Width / fit_Size.Height;
                        grphs.TranslateTransform((width_Graphic - width_Graphic_Tmp) / 2, 0);
                        width_Graphic = width_Graphic_Tmp;
                    }
                    else
                    {//以宽为基准，高居中
                        grphs.TranslateTransform(0, (height_Graphic - height_Graphic_Tmp) / 2);
                        height_Graphic = height_Graphic_Tmp;
                    }
                }

                //填充路径
                using (GraphicsPath path = new GraphicsPath())
                {
                    switch (type)
                    {
                        case DrawType.AndGate:
                            path.AddLines(FormatPoints(0, height_Graphic * 29 / 74, 0, height_Graphic, width_Graphic, height_Graphic, width_Graphic, height_Graphic * 29 / 74));
                            path.AddArc(0, 0, width_Graphic, width_Graphic, 0, -180);
                            break;
                        case DrawType.OrGate:
                            path.AddLine(0, height_Graphic * 37 / 74, 0, height_Graphic + width_Pen * 2 / 5);
                            path.AddBezier(new PointF(0, height_Graphic), new PointF(width_Graphic * 26 / 58, height_Graphic * 52 / 74), new PointF(width_Graphic, height_Graphic), new PointF(width_Graphic, height_Graphic));
                            path.AddLine(width_Graphic, height_Graphic + width_Pen * 2 / 5, width_Graphic, height_Graphic * 37 / 74);
                            path.AddBezier(new PointF(width_Graphic / 2 - width_Pen / 4, 0), new PointF(width_Graphic * 48 / 58, height_Graphic * 9 / 74), new PointF(width_Graphic, height_Graphic * 37 / 74 + width_Pen / 4), new PointF(width_Graphic, height_Graphic * 37 / 74 + width_Pen / 4));
                            path.AddBezier(new PointF(width_Graphic / 2 + width_Pen / 4, 0), new PointF(width_Graphic * 10 / 58, height_Graphic * 9 / 74), new PointF(0, height_Graphic * 37 / 74 + width_Pen / 4), new PointF(0, height_Graphic * 37 / 74 + width_Pen / 4));
                            break;
                        case DrawType.BasicEvent:
                            path.AddArc(0, 0, width_Graphic, height_Graphic, 0, 360);
                            break;
                        case DrawType.UndevelopedEvent:
                            path.AddPolygon(FormatPoints(0, height_Graphic / 2, width_Graphic / 2, height_Graphic, width_Graphic, height_Graphic / 2, width_Graphic / 2, 0));
                            break;
                        case DrawType.TransferInGate:
                            path.AddPolygon(FormatPoints(0, height_Graphic, width_Graphic, height_Graphic, width_Graphic / 2, 0));
                            break;
                        case DrawType.HouseEvent:
                            path.AddPolygon(FormatPoints(0, height_Graphic * 33 / 81, 0, height_Graphic, width_Graphic, height_Graphic, width_Graphic, height_Graphic * 33 / 81, width_Graphic / 2, 0));
                            break;
                        case DrawType.PriorityAndGate:
                            path.AddLines(FormatPoints(0, height_Graphic * 29 / 74, 0, height_Graphic, width_Graphic, height_Graphic, width_Graphic, height_Graphic * 29 / 74));
                            path.AddArc(0, 0, width_Graphic, width_Graphic, 0, -180);
                            break;
                        //case DrawType.InhibitGate:
                        //    path.AddPolygon(FormatPoints(0, height_Graphic * 18 / 72, 0, height_Graphic * 54 / 72, width_Graphic * 29 / 58, height_Graphic, width_Graphic, height_Graphic * 54 / 72, width_Graphic, height_Graphic * 18 / 72, width_Graphic * 29 / 58, 0));
                        //    break;
                        case DrawType.ConditionEvent:
                            path.AddArc(0, 0, height_Graphic, height_Graphic, 90, 180);
                            path.AddLine(width_Graphic * 4 / 18 - width_Pen / 4, height_Graphic, width_Graphic * 14 / 18 + width_Pen / 4, height_Graphic);
                            path.AddArc(width_Graphic * 10 / 18, 0, height_Graphic, height_Graphic, -90, 180);
                            path.AddLine(width_Graphic * 14 / 18 + width_Pen / 4, 0, width_Graphic * 4 / 18 - width_Pen / 4, 0);
                            break;
                        case DrawType.XORGate:
                            path.AddBezier(new PointF(width_Graphic / 2 + width_Pen / 8, 0), new PointF(width_Graphic * 20 / 100, 0), new PointF(0, height_Graphic + width_Pen / 4), new PointF(0, height_Graphic + width_Pen / 4));
                            path.AddLine(0, height_Graphic, width_Graphic, height_Graphic);
                            path.AddBezier(new PointF(width_Graphic, height_Graphic + width_Pen / 4), new PointF(width_Graphic * 80 / 100, 0), new PointF(width_Graphic / 2 - width_Pen / 8, 0), new PointF(width_Graphic / 2 - width_Pen / 8, 0));
                            break;
                        case DrawType.VotingGate:
                            path.AddBezier(new PointF(width_Graphic / 2 + width_Pen / 8, 0), new PointF(width_Graphic * 20 / 100, 0), new PointF(0, height_Graphic + width_Pen / 4), new PointF(0, height_Graphic + width_Pen / 4));
                            path.AddLine(0, height_Graphic, width_Graphic, height_Graphic);
                            path.AddBezier(new PointF(width_Graphic, height_Graphic + width_Pen / 4), new PointF(width_Graphic * 80 / 100, 0), new PointF(width_Graphic / 2 - width_Pen / 8, 0), new PointF(width_Graphic / 2 - width_Pen / 8, 0));
                            break;
                        case DrawType.RemarksGate:
                            path.AddRectangle(new RectangleF(0, 0, width_Graphic, height_Graphic));
                            break;
                        default:
                            break;
                    }

                    grphs.FillPath(br, path);
                }
                //绘制线条
                DrawBaseComponent(drawData, type, grphs, pen, 0, 0, width_Graphic, height_Graphic);
                grphs.Transform.Dispose();
                grphs.Transform = matix_Before;
            }
        }

        /// <summary>
        /// 获取每种基本图形的黄金比例（宽高）
        /// </summary>
        /// <param name="type">图形类型</param>
        /// <returns>大小比例</returns>
        private static Size GetGraphicFitSize(DrawType type)
        {
            Size size = new Size(0, 0);
            switch (type)
            {
                case DrawType.AndGate:
                    size.Width = 58;
                    size.Height = 74;
                    break;
                case DrawType.OrGate:
                    size.Width = 58;
                    size.Height = 74;
                    break;
                case DrawType.BasicEvent:
                    size.Width = 58;
                    size.Height = 58;
                    break;
                case DrawType.UndevelopedEvent:
                    size.Width = 58;
                    size.Height = 38;
                    break;
                case DrawType.TransferInGate:
                    size.Width = 58;
                    size.Height = 58;
                    break;
                case DrawType.HouseEvent:
                    size.Width = 58;
                    size.Height = 81;
                    break;
                case DrawType.PriorityAndGate:
                    size.Width = 58;
                    size.Height = 74;
                    break;
                //case DrawType.InhibitGate:
                //    size.Width = 58;
                //    size.Height = 72;
                //    break;
                case DrawType.ConditionEvent:
                    size.Width = 18;
                    size.Height = 8;
                    break;
                case DrawType.XORGate:
                    size.Width = 100;
                    size.Height = 100;
                    break;
                case DrawType.VotingGate:
                    size.Width = 100;
                    size.Height = 100;
                    break;
                case DrawType.RemarksGate:
                    size.Width = 58;
                    size.Height = 38;
                    break;
                default:
                    break;
            }
            return size;
        }

        /// <summary>
        /// 根据多组x,y的坐标点，返回他们point结构的数组
        /// </summary>
        /// <param name="X_Y">x或y坐标</param>
        /// <returns>点的数组</returns>
        private static PointF[] FormatPoints(params float[] X_Y)
        {
            if (X_Y.Length < 4 || X_Y.Length % 2 != 0) return null;
            PointF[] points = new PointF[X_Y.Length / 2];
            for (int i = 0; i < X_Y.Length / 2; i++)
            {
                points[i] = new PointF(X_Y[2 * i], X_Y[2 * i + 1]);
            }
            return points;
        }
    }
}
