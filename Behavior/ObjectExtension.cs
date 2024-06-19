namespace FaultTreeAnalysis
{
    /// <summary>
    /// 对象扩展基类
    ///     用作对特殊对象的扩展通道
    /// </summary>
    public class ObjectExtension
        : FaultTreeAnalysis.Behavior.ObjectBehavior
    //, System.ComponentModel.INotifyPropertyChanged
    //, System.Collections.Specialized.INotifyCollectionChanged
    {

        /// <summary>
        /// 不支持实现重做
        /// </summary>
        //public virtual void Redo()
        //{
        //    throw new NotImplementedException("Not support.");
        //}

        /// <summary>
        /// 不支持实现撤销
        /// </summary>
        //public virtual void Undo()
        //{
        //    throw new NotImplementedException("Not support.");
        //}
        //public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// 不支持实现比较
        /// </summary>
        //public virtual bool Equals(ObjectBehavior other)
        //{
        //    throw new NotImplementedException("Not support.");
        //}
    }
}
