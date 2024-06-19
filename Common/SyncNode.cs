using DevExpress.XtraTreeList.Nodes;
using System;
using System.Windows.Forms;

namespace FaultTreeAnalysis.Common
{
    public static class SyncNode
    {
        /// <summary>
        ///同步父子节点勾选状态
        ///说明
        ///在AfterCheckNode事件中使用代码
        ///eg:e.Node.SyncNodeCheckState(e.Node.CheckState);
        /// </summary>
        /// <param name="node">需要同步的节点</param>
        /// <param name="check">节点当前勾选状态</param>
        public static void SyncNodeCheckState(this TreeListNode node, CheckState check) { SyncNodeCheckState_Child(node, check); SyncNodeCheckState_Parent(node, check); }

        private static void SyncNodeCheckState_Child(TreeListNode node, CheckState check)
        {
            if (node != null)
            {
                node.DownRecursiveNode(n => n.CheckState = check);
            }
        }
        private static void SyncNodeCheckState_Parent(TreeListNode node, CheckState check)
        {
            if (node.ParentNode != null)
            {
                bool _cked = false;
                CheckState _ckState;
                foreach (TreeListNode cn in node.ParentNode.Nodes)
                {
                    _ckState = cn.CheckState;
                    if (check != _ckState)
                    {
                        _cked = !_cked;
                        break;
                    }
                }
                node.ParentNode.CheckState = _cked ? CheckState.Indeterminate : check;
                SyncNodeCheckState_Parent(node.ParentNode, check);
            }
        }
        /// <summary>
        /// 向下递归TreeListNode节点
        /// </summary>
        /// <param name="node">需要向下递归的节点</param>
        /// <param name="conditionHanlder">委托</param>
        public static void DownRecursiveNode(this TreeListNode node, Action<TreeListNode> conditionHanlder)
        {
            foreach (TreeListNode _childNode in node.Nodes)
            {
                conditionHanlder(_childNode);
                DownRecursiveNode(_childNode, conditionHanlder);
            }
        }
    }
}
