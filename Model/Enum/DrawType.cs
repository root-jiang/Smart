using System.ComponentModel;

namespace FaultTreeAnalysis.Model.Enum
{
    /// <summary>
    /// 自定义的图形类型，用于绘制和数据显示
    /// </summary>
    public enum DrawType
    {
        /// <summary>
        /// 空的类型
        /// </summary>
        [Description("")]
        NULL,

        /// <summary>
        /// 与门
        /// </summary>
        [Description("And Gate")]
        AndGate,

        /// <summary>
        /// 或门
        /// </summary>
        [Description("Or Gate")]
        OrGate,

        /// <summary>
        /// 基本事件
        /// </summary>
        [Description("Basic Event")]
        BasicEvent,

        /// <summary>
        /// 转移符号（转移门）
        /// </summary>
        [Description("Transfer-In Gate")]
        TransferInGate,

        /// <summary>
        /// 屋事件
        /// </summary>
        [Description("House Event")]
        HouseEvent,

        /// <summary>
        /// 未发展事件
        /// </summary>
        [Description("Undeveloped Event")]
        UndevelopedEvent,

        /// <summary>
        /// 条件事件
        /// </summary>
        [Description("Condition Event")]
        ConditionEvent,

        /// <summary>
        /// 描述框
        /// </summary>
        [Description("Remarks Gate")]
        RemarksGate,

        /// <summary>
        /// 异或门
        /// </summary>
        [Description("XOR Gate")]
        XORGate,

        /// <summary>
        /// 表决门
        /// </summary>
        [Description("Voting Gate")]
        VotingGate,

        /// <summary>
        /// 优先与门
        /// </summary>
        [Description("Priority AND Gate")]
        PriorityAndGate,

        ///// <summary>
        ///// 禁止门
        ///// </summary>
        //[Description("Inhibit Gate")]
        //InhibitGate

        ///// <summary>
        ///// 禁止门
        ///// </summary>
        [Description("Link Gate")]
        LinkGate
    }
}
