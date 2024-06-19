namespace FaultTreeAnalysis
{
    /// <summary>
    /// 整个FTA程序用到的固定字符串，就算多语言下也是固定的
    /// </summary>
    public class FixedString
    {
        public const string APP_EXTENSION = ".fta";
        /// <summary>
        /// 旧的，估计废弃的从aspect项目获取故障树的用到的字符串
        /// </summary>
        public const string TAG_VALUE_NEED_GET_NOTES = "<memo>";

        #region 通用的报异常时前缀字符串
        public const string EXCEPTION = "Exception Occured: ";
        #endregion

        #region 可切换的语言
        public const string LANGUAGE_EN_CN = "Chinese";
        public const string LANGUAGE_EN_EN = "English";
        public const string LANGUAGE_CN_CN = "中文";
        public const string LANGUAGE_CN_EN = "英文";
        #endregion

        #region FTA表里标题对象的Column_Name属性的值
        public const string COLUMNAME_DATA = "Data";
        //public const string COLUMNAME_ID = "Identifier";
        //public const string COLUMNAME_TYPE = "Type";
        //public const string COLUMNAME_COMMENT = "Comment1";
        //public const string COLUMNAME_LOGICALCONDITION = "LogicalCondition";
        //public const string COLUMNAME_MODEL = "InputType";
        //public const string COLUMNAME_FAILURERATE_TYPE = "FRType";
        //public const string COLUMNAME_EXPOSURETIME_PERCENTAGE = "ExposureTimePercentage";
        //public const string COLUMNAME_DORMANCYFACTOR = "DormancyFactor";
        //public const string COLUMNAME_FAILURERATE_PERCENTAGE = "FRPercentage";
        //public const string COLUMNAME_INPUTVALUE = "InputValue";
        //public const string COLUMNAME_INPUTVALUE2 = "InputValue2";
        //public const string COLUMNAME_UNITS = "Units";
        //public const string COLUMNAME_ASSOCIATEDPROBLEMS = "ProblemList";
        //public const string COLUMNAME_EXTRAVALUE = "ExtraValue";
        #region 额外可配置字段
        public const string COLUMNAME_EXTRAVALUE1 = "ExtraValue1";
        public const string COLUMNAME_EXTRAVALUE2 = "ExtraValue2";
        public const string COLUMNAME_EXTRAVALUE3 = "ExtraValue3";
        public const string COLUMNAME_EXTRAVALUE4 = "ExtraValue4";
        public const string COLUMNAME_EXTRAVALUE5 = "ExtraValue5";
        public const string COLUMNAME_EXTRAVALUE6 = "ExtraValue6";
        public const string COLUMNAME_EXTRAVALUE7 = "ExtraValue7";
        public const string COLUMNAME_EXTRAVALUE8 = "ExtraValue8";
        public const string COLUMNAME_EXTRAVALUE9 = "ExtraValue9";
        public const string COLUMNAME_EXTRAVALUE10 = "ExtraValue10";
        #endregion
        #endregion

        #region logical condition字段可选的三种字符串
        public const string LOGICAL_CONDITION_TRUE = "True";
        public const string LOGICAL_CONDITION_NORMAL = "Normal";
        public const string LOGICAL_CONDITION_FALSE = "False";
        #endregion

        #region model字段可选字符串
        public const string MODEL_CONSTANT_PROBABILITY = "Constant Probability";
        public const string MODEL_FR_MTBF = "FR/MTBF";
        public const string MODEL_FREQUENCY = "Frequency";
        public const string MODEL_FAILURE_WITH_REPAIR = "Failure With Repair";
        public const string MODEL_FAILURE_WITH_PERIODIC_INSPECTION = "Failure With Periodic Inspection";
        public const string MODEL_LAMBDA_TAU = "Lambda Tau";
        #endregion

        #region failure rate type字段可选字符串
        public const string FAILURE_RATE_TYPE_FAILURE_RATE = "Failure Rate";
        public const string FAILURE_RATE_TYPE_MTBF = "MTBF";
        #endregion

        #region units字段可选字符串
        public const string UNITS_HOURS = "Hours";
        public const string UNITS_MINUTES = "Minutes";
        public const string UNITS_HOURS_CN = "小时";
        public const string UNITS_MINUTES_CN = "分钟";
        #endregion

        #region 通用报错窗口的标题
        public const string FAULT_TREE_ANALYSIS = "Fault Tree Analysis";
        #endregion

        //public const string EXCEL_FILTER = "Excel 文件(*.xlsx)|*.xlsx|Excel 文件(*.xls)|*.xls|所有文件(*.*)|*.*";
        public const string XLS_EXTENSION = ".xls";
        public const string XLSX_EXTENSION = ".xlsx";
        public const string XML_EXTENSION = ".xml";
        public const string FaultTree_FILTER = "fta文件(*.fta)|*.fta";
        public const string EXCEL_FILTER = "Excel 文件(*.xlsx)|*.xlsx|Excel 文件(*.xls)|*.xls";
        public const string EXCELXML_FILTER = "Excel 文件(*.xlsx)|*.xlsx|Excel 文件(*.xls)|*.xls|xml文件(*.xml)|*.xml";
        public const string MODEL_FILTER = "Model 文件(*.mdl)|*.mdl";
        public const string IMAGE_FILTER = "PNG 图片(*.png)|*.png";
        public const string DATETIME_FORMAT = "yyyy_MM_dd hh_mm";
        public const string SCIENTIFIC_NOTATION_FORMAT = "E";//"0.00000e+000";
        public const string DEFAULT_GateID_PREFIX = "Gate";
        public const string DEFAULT_EventID_PREFIX = "Event";
        public const string SEMICOLON = ";";
        public const string DEFAULT_SKIN = "Office Dark";
        public const string DLL_PATH = "Resources\\Dlls\\xfta.dll";
        //public const string DLL_PATH = "xfta.dll";
    }
}
