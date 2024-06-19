using DevExpress.XtraBars;
using DevExpress.XtraDiagram;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraTreeList;
using FaultTreeAnalysis.Model.Data;
using FaultTreeAnalysis.Model.Enum;
using FaultTreeAnalysis.View.Diagram;
using FaultTreeAnalysis.View.Ribbon.Start;
using FaultTreeAnalysis.View.Ribbon.Start.Edit;
using FaultTreeAnalysis.View.Ribbon.Start.Excel;
using FaultTreeAnalysis.View.Table;
using IntegratedSystem.Tools;
using IntegratedSystem.View;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace FaultTreeAnalysis.Common
{
    /// <summary>
    /// 全局事件注册执行类
    /// </summary>
    static class General
    {
        /// <summary>
        /// 是否插入链接门
        /// </summary>
        public static bool isLinkGateInsert = false;
        /// <summary>
        /// 系统数据对象
        /// </summary>
        public static FtaDiagram ftaDiagram;

        /// <summary>
        /// FTA表数据对象
        /// </summary>
        public static FtaTable ftaTable;
        /// <summary>
        /// 编辑时，出现重复编号，是否用现在的值覆盖过去的值，false使用过去的值，true使用当前的值
        /// </summary>
        public static bool isRepeatCopyCurrentValues { get; set; }
        public static SelectCheckErr SelectCheckErr { get; set; }
        public static ShowCut ShowCut { get; set; }
        public static FindAndReplace FindAndReplace { get; set; }

        /// <summary>
        /// 是否忽略FTA树表的focusnodechange事件，不让其触发
        /// </summary>
        public static bool IsIgnoreTreeListFocusNodeChangeEvent { get; set; }
        public static bool isTaskRun { get; set; } = false;
        public static bool IsTreeListLoaded { get; set; } = false;
        public static string MessageFromSimfia { get; set; }
        public static bool IsClosed { get; set; } = false;
        /// <summary>
        /// 画笔宽度，全局定义的设置
        /// </summary>
        public static readonly int PEN_WIDTH = 1;

        /// <summary>
        /// FTA表或图复制/剪切的数据对象
        /// </summary>
        public static DrawData CopyCutObject = null;

        /// <summary>
        /// FTA表或图复制/剪切的数据对象对应的故障树
        /// </summary>
        public static SystemModel CopyCutSystem = null;

        /// <summary>
        /// FTA表或图是否递归复制节点（false）剪切节点（true）
        /// </summary>
        public static bool FTATableDiagram_Is_CopyOrCut_Recurse = true;

        public static ProgramModel FtaProgram { get; set; }

        /// <summary>
        /// 用于动态显示图的类
        /// </summary>
        public static DiagramItemPool DiagramItemPool { get; set; }

        /// <summary>
        /// FTA表和FTA图右键菜单通用的返回某菜单项是否可见或可用的通用函数全局化
        /// </summary>
        public static Func<object, DrawData, DrawData, bool?, bool?, bool> GetBarItemIsEnabled { get; set; }

        public static Func<DrawData, string, string, string, bool> ChangeProbability { get; set; }

        public static PopupMenu DiagramMenuControl { get; set; }

        public static TreeList ProjectControl { get; set; }

        public static TreeList TableControl { get; set; }

        public static DiagramControl DiagramControl { get; set; }
        public static List<DiagramControl> DiagramControlList { get; set; }

        public static Dictionary<string, BarCheckItem> BarCheckItems { get; set; }

        public static DevExpress.XtraBars.Docking.DockManager dockManager_FTA { get; set; }

        public static DevExpress.XtraBars.Docking.ControlContainer ControlContainer { get; set; }

        public static DevExpress.XtraBars.Docking.DockPanel ProjectContainer { get; set; }

        public static DevExpress.XtraBars.Docking.DockPanel PanelContainer { get; set; }

        public static TreeList FTATree { get; set; }

        public static DevExpress.XtraBars.Ribbon.RibbonControl RibbonControl { get; set; }

        public static BarButtonItem barButtonItem_Cut { get; set; }
        public static BarButtonItem barButtonItem_MenuCut { get; set; }
        public static BarButtonItem barButtonItem_ExportToModel { get; set; }
        public static BarButtonItem barButtonItem_LoadFromModel { get; set; }
        public static BarButtonItem barButtonItem_BasicLib { get; set; }
        public static BarButtonItem barButtonItem_AddToBasicEventLibrary { get; set; }
        public static BarButtonItem barButtonItem_SynchronizeFromBasicEventLibrary { get; set; }
        public static BarButtonItem barButtonItem_InsertFromBasicEventLibrary { get; set; }

        public static BarButtonItem barButtonItem_ShapeEdit { get; set; }

        public static BarButtonItem barButtonItem_Copy { get; set; }
        public static BarButtonItem barButtonItem_MenuCopy { get; set; }
        public static BarButtonItem barButtonItem_MenuCopyCurrentView { get; set; }
        public static BarButtonItem barButtonItem_MenuCopyCurrentSelected { get; set; }

        public static BarButtonItem barButtonItem_MenuPaste { get; set; }
        public static BarButtonItem barButtonItem_MenuPasteRepeated { get; set; }
        public static BarButtonItem barButtonItem_DeleteTopRibbon { get; set; }
        public static BarButtonItem barButtonItem_DeleteLevelRibbon { get; set; }
        public static BarButtonItem barButtonItem_DeleteNodeRibbon { get; set; }
        public static BarButtonItem barButtonItem_DeleteNodesRibbon { get; set; }
        public static BarButtonItem barButtonItem_DeleteTransferRibbon { get; set; }


        public static BarButtonItem barButtonItem_DeleteTopTable { get; set; }
        public static BarButtonItem barButtonItem_DeleteLevelTable { get; set; }
        public static BarButtonItem barButtonItem_DeleteNodeTable { get; set; }
        public static BarButtonItem barButtonItem_DeleteNodesTable { get; set; }
        public static BarButtonItem barButtonItem_DeleteTransferTable { get; set; }


        public static BarButtonItem barButtonItem_DeleteTopDiagram { get; set; }
        public static BarButtonItem barButtonItem_DeleteLevelDiagram { get; set; }
        public static BarButtonItem barButtonItem_DeleteNodeDiagram { get; set; }
        public static BarButtonItem barButtonItem_DeleteNodesDiagram { get; set; }
        public static BarButtonItem barButtonItem_DeleteTransferDiagram { get; set; }

        public static BarSubItem barSubItem_ChangeGateType { get; set; }
        public static BarButtonItem barButtonItem_InsertLevel { get; set; }
        public static BarSubItem barSubItem_TransferTo { get; set; }
        public static BarButtonItem barButtonItem_MenuBreakIntoTransfer { get; set; }
        public static BarButtonItem barButtonItem_MenuCollapseTransfer { get; set; }
        public static User_StartPage StartPage { get; set; }

        public static DataTable EventsLibDB { get; set; }

        public static BarStaticItem StaticItem_File { get; set; }

        public static RibbonGalleryBarItem HighLightCutSet { get; set; }

        public static BarSubItem TransferTo { get; set; }
        public static BarSubItem SubTransferTo { get; set; }

        public static BarSubItem Delete { get; set; }
        public static BarSubItem DeleteTable { get; set; }
        public static BarSubItem CopyTable { get; set; }

        /// <summary>
        /// 事件
        /// </summary>
        public static event EventHandler<GlobalEventArgs> GlobalHandler;

        /// <summary>
        /// 事件执行
        /// </summary>
        /// <param name="globalEvent">事件类型</param>
        /// <param name="value">事件的附带数据</param>
        public static void InvokeHandler(GlobalEvent globalEvent, object value = null) => General.GlobalHandler?.Invoke(null, new GlobalEventArgs(globalEvent, value));

        #region 异常捕获，日志记录

        /// <summary>
        /// 日志记录第三方Dll，缺少会报错Genral类型初始化失败
        /// </summary>
        //private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 堆栈跟踪类，实现定位异常产生的方法位置
        /// </summary>
        private static Track Track { get; } = new Track();

        /// <summary>
        /// 包装一段无返回值的代码
        /// </summary>
        /// <param name="action">()=>{包装你的代码}</param>
        public static void TryCatch(Action action)
        {
            try { action(); }
            catch (Exception exception)
            {
                //General.Logger.Error(exception);
                MsgBox.Show(General.Track.PrintMessage(exception.Message)[1]);
            }
        }

        /// <summary>
        /// 包装一段有返回值的代码
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="function">()=>{你的代码}</param>
        /// <returns></returns>
        public static T TryCatch<T>(Func<T> function/*, Func<T> finallyFunc*/)
        {
            var result = default(T);
            try { result = function(); }
            catch (Exception exception) { MsgBox.Show(General.Track.PrintMessage(exception.Message)[1]); }
            //finally { result = finallyFunc(); }
            return result;
        }
        #endregion

        #region win32模拟鼠标操作
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        public static void MouseEvent(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo) => mouse_event(dwFlags, dx, dy, cButtons, dwExtraInfo);
        #endregion

        /// <summary>
        /// 销毁创建的图标句柄，释放资源
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public extern static bool DestroyIcon(IntPtr handle);

        public static string GetKeyName(string valueString)
        {
            var result = string.Empty;
            var en = StringModel.GetENFTAString();
            var ch = StringModel.GetCNFTAString();
            var enProperty = en.GetType().GetProperties().FirstOrDefault(o => String.Equals(o.GetValue(en)?.ToString().Replace(" ", ""), valueString.Replace(" ", ""), StringComparison.CurrentCultureIgnoreCase) == true);
            if (valueString == "InputValue") return en.FailureRate;
            if (valueString == "InputValue2") return en.ExposureTime;
            if (valueString == "Comment1") return "Comment1";
            if (valueString == "Votes") return en.ExtraValue1;
            if (valueString == "表决") return en.ExtraValue1;

            if (enProperty != null) return enProperty.Name;
            else
            {
                var chProperty = ch.GetType().GetProperties().FirstOrDefault(o => o.GetValue(ch)?.ToString() == valueString);
                if (chProperty != null) result = chProperty.Name;
                else result = string.Empty;
            }
            return result;
        }

        public static string GetValueName(string keyString)
        {
            return General.FtaProgram.String.GetType().GetProperties().FirstOrDefault(o => o.Name == keyString).GetValue(General.FtaProgram.String).ToString();
        }

        public static T GetEnumByName<T>(string name)
        {
            return (T)Enum.Parse(typeof(T), name);
        }

        public static DrawData GetTransferMajor(SystemModel system, DrawData transfer)
        {
            try
            {
                if (system != null)
                {
                    var allData = system.GetAllDatas();
                    return allData.FirstOrDefault(o => o.Identifier == transfer.Identifier && o.Type != DrawType.TransferInGate);
                }
                else
                {
                    var allData = General.FtaProgram.CurrentSystem.GetAllDatas();
                    return allData.FirstOrDefault(o => o.Identifier == transfer.Identifier && o.Type != DrawType.TransferInGate);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string CreateName(string name, IEnumerable<string> existNnames)
        {

            while (existNnames.Contains(name))
            {
                name = $"{name}_(2)";
            }
            return name;
        }

        public static string GetExecutingAssemblyPath()
        {
            string path = Application.StartupPath;
            return path;
        }

        public static decimal ConvertDataToDigit(string strData, int place = 6)
        {
            var result = 0.0M;
            if (strData.Contains("E"))
            {
                result = decimal.Round(decimal.Parse(strData, System.Globalization.NumberStyles.Float), 20);
            }
            return result;
        }

        public static string ConvertEStringToDouble(string strData)
        {
            string result;
            if (strData.ToUpper().Contains("E"))
            {
                double b = double.Parse(strData.ToUpper().Split('E')[0].ToString());//整数部分
                double c = double.Parse(strData.ToUpper().Split('E')[1].ToString());//指数部分
                result = (b * Math.Pow(10, c)).ToString("E");
            }
            else
            {
                result = double.Parse(strData).ToString("E");
            }
            return result;
        }

        public static string GetFileName(DialogType dialogType, string name = default(string))
        {
            var result = string.Empty;
            var dialog = default(FileDialog);
            if (dialogType.Equals(DialogType.OpenFile)) dialog = new OpenFileDialog { Filter = FixedString.EXCEL_FILTER };
            else dialog = new SaveFileDialog { Filter = FixedString.EXCEL_FILTER, FileName = name };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                result = dialog.FileName;
                //if (dialogType.Equals(DialogType.OpenFile)) this.fileLastName = (dialog as OpenFileDialog).SafeFileName;
            }
            return result;
        }

        public static string GetFileNamePDF(DialogType dialogType, string name = default(string))
        {
            var result = string.Empty;
            var dialog = default(FileDialog);
            if (dialogType.Equals(DialogType.OpenFile)) dialog = new OpenFileDialog { Filter = "PDF 文件(*.pdf)|*.pdf" };
            else dialog = new SaveFileDialog { Filter = "PDF 文件(*.pdf)|*.pdf", FileName = name };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                result = dialog.FileName;
                //if (dialogType.Equals(DialogType.OpenFile)) this.fileLastName = (dialog as OpenFileDialog).SafeFileName;
            }
            return result;
        }

        /// <summary>
        /// 加载故障树数据
        /// </summary>
        /// <returns></returns>
        public static SystemModel GetSystemFromJson(string sys)
        {
            SystemModel SM = Newtonsoft.Json.JsonConvert.DeserializeObject<SystemModel>(File.ReadAllText(sys, Encoding.UTF8));
            try
            {
                List<DrawData> drawDatas = SM.GetAllDatas();
                //3.同步数据到当前事件，同时刷新所有重复事件 
                if (General.EventsLibDB == null || General.EventsLibDB.Rows.Count == 0)
                {
                    return SM;
                }

                foreach (DrawData da in drawDatas)
                {
                    if (da.GUID != null && da.GUID != "")
                    {
                        DataRow checkDB = General.EventsLibDB.Select("GUID='" + da.GUID + "'").FirstOrDefault();
                        if (checkDB != null)
                        {
                            var typeName = General.GetKeyName(checkDB["Type"].ToString());
                            var type = (DrawType)Enum.Parse(typeof(DrawType), typeName);
                            da.GUID = checkDB["GUID"].ToString();
                            da.Identifier = checkDB["Identifier"].ToString();
                            da.Type = type;
                            da.Comment1 = checkDB["Description"].ToString();
                            da.LogicalCondition = checkDB["LogicalCondition"].ToString();
                            da.InputType = checkDB["InputType"].ToString();
                            da.InputValue = checkDB["InputValue"].ToString();
                            da.InputValue2 = checkDB["InputValue2"].ToString();
                            da.ExtraValue1 = checkDB["ExtraValue1"].ToString();
                            da.Units = checkDB["Units"].ToString();
                        }
                    }
                }
                return SM;
            }
            catch (Exception)
            {
                return SM;
            }
        }

        public static void WriteText(string path, string data)
        {
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fileStream))
                {
                    sw.WriteLine(data);
                    sw.Flush();
                }
            }
        }

        public static string CNEN_Changes(string Language, string Value)
        {
            if (Language == FixedString.LANGUAGE_CN_CN)
            {
                switch (Value)
                {
                    case "And Gate":
                        Value = "与门";
                        break;
                    case "Or Gate":
                        Value = "或门";
                        break;
                    case "Basic Event":
                        Value = "基本事件";
                        break;
                    case "Undeveloped Event":
                        Value = "未发展事件";
                        break;
                    case "Transfer-In Gate":
                        Value = "转移门";
                        break;
                    case "House Event":
                        Value = "屋事件";
                        break;
                    case "Priority AND Gate":
                        Value = "优先与门";
                        break;
                    case "Condition Event":
                        Value = "条件事件";
                        break;
                    case "XOR Gate":
                        Value = "异或门";
                        break;
                    case "Voting Gate":
                        Value = "表决门";
                        break;
                    case "Remarks Gate":
                        Value = "描述框";
                        break;
                }

                switch (Value)
                {
                    case "Normal":
                        Value = "常规";
                        break;
                    case "False":
                        Value = "假";
                        break;
                    case "True":
                        Value = "真";
                        break;
                }

                switch (Value)
                {
                    case "Hours":
                        Value = "小时";
                        break;
                    case "Minutes":
                        Value = "分钟";
                        break;
                }

                switch (Value)
                {
                    case "Constant Probability":
                        Value = "恒定概率";
                        break;
                    case "Lambda Tau":
                        Value = "Lambda Tau";
                        break;
                }
            }
            else
            {
                switch (Value)
                {
                    case "与门":
                        Value = "And Gate";
                        break;
                    case "或门":
                        Value = "Or Gate";
                        break;
                    case "基本事件":
                        Value = "Basic Event";
                        break;
                    case "未发展事件":
                        Value = "Undeveloped Event";
                        break;
                    case "转移门":
                        Value = "Transfer-In Gate";
                        break;
                    case "屋事件":
                        Value = "House Event";
                        break;
                    case "优先与门":
                        Value = "Priority AND Gate";
                        break;
                    case "条件事件":
                        Value = "Condition Event";
                        break;
                    case "异或门":
                        Value = "XOR Gate";
                        break;
                    case "表决门":
                        Value = "Voting Gate";
                        break;
                    case "描述框":
                        Value = "Remarks Gate";
                        break;
                }

                switch (Value)
                {
                    case "常规":
                        Value = "Normal";
                        break;
                    case "假":
                        Value = "False";
                        break;
                    case "真":
                        Value = "True";
                        break;
                }

                switch (Value)
                {
                    case "小时":
                        Value = "Hours";
                        break;
                    case "分钟":
                        Value = "Minutes";
                        break;
                }

                switch (Value)
                {
                    case "恒定概率":
                        Value = "Constant Probability";
                        break;
                    case "Lambda Tau":
                        Value = "Lambda Tau";
                        break;
                }
            }
            return Value;
        }
    }

    /// <summary>
    /// 自定义的事件参数
    /// </summary>
    public class GlobalEventArgs : EventArgs
    {
        public GlobalEvent GlobalEvent { get; private set; }

        public object Value { get; private set; }

        public GlobalEventArgs(GlobalEvent globalEvent, object value)
        {
            this.GlobalEvent = globalEvent;
            this.Value = value;
        }
    }
}
