using FaultTreeAnalysis.Behavior.Enum;

namespace FaultTreeAnalysis.Behavior.Event
{
    /// <summary>
    /// 属性改变事件扩展参数
    ///     应用于对象属性改变事件
    /// </summary>
    public class ExPropertyChangedEventsArgs
        : System.ComponentModel.PropertyChangedEventArgs
    {

        /// <summary>
        /// 属性状态
        /// </summary>
        public PropertyStatus Status { get; set; }

        /// <summary>
        /// 构造事件扩展参数
        /// </summary>
        /// <param name="strPropertyName">属性名称</param>
        public ExPropertyChangedEventsArgs(string strPropertyName)
            : base(strPropertyName)
        {

        }
    }
}
