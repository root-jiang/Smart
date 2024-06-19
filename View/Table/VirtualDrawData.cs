using DevExpress.XtraTreeList;
using FaultTreeAnalysis.Common;
using FaultTreeAnalysis.Model.Data;
using FaultTreeAnalysis.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaultTreeAnalysis
{
    /// <summary>
    /// 用于显示FTA表的数据类
    /// </summary>
    class VirtualDrawData : TreeList.IVirtualTreeListData
   {
      ProgramModel ftaProgram;

      /// <summary>
      /// 实际的数据对象
      /// </summary>
      public DrawData data = null;

      /// <summary>
      /// 用于FTATable显示和过滤数据时是否过滤所有数据
      /// </summary>
      public static bool is_FilterAllMode = false;

      /// <summary>
      /// 用于全过滤模式返回所有子节点的开始对象
      /// </summary>
      private static DrawData root = null;

      private static ProgramModel _ftrProgram;

      /// <summary>
      /// 构造方法，设置数据对象和根节点对象
      /// </summary>
      /// <param name="data">数据对象</param>
      /// <param name="root">用于全过滤模式返回所有子节点的开始对象，默认应该等于data，或data的子节点</param>
      public VirtualDrawData(DrawData data, DrawData root,ProgramModel ftaProgram)
      {
         this.data = data;
         VirtualDrawData.root = root;
         //this.bb= Newtonsoft.Json.JsonConvert.SerializeObject(ftaProgram);
         this.ftaProgram = ftaProgram;
         VirtualDrawData._ftrProgram = ftaProgram;
      }

      /// <summary>
      /// 用于内部构造子对象时使用
      /// </summary>
      /// <param name="data">内部要保存的数据对象</param>
      private VirtualDrawData(DrawData data, ProgramModel ftaProgram)
      {
         this.data = data;
         this.ftaProgram = ftaProgram;
      }

      /// <summary>
      /// 初始化treeList时单元格获取显示的值
      /// </summary>
      /// <param name="info"></param>
      void TreeList.IVirtualTreeListData.VirtualTreeGetCellValue(VirtualTreeGetCellValueInfo info)
      {
         if (info != null && info.Column != null && info.Node != null)
         {
            var virtualData = ((VirtualDrawData)info.Node);
            var property = virtualData.data.GetType().GetProperties().FirstOrDefault(o => o.Name == info.Column.Name);
            if (property?.PropertyType?.Name == nameof(DrawType))
            {
               var drawTypeName = Enum.GetName(typeof(DrawType), virtualData.data.Type);
               info.CellData = ftaProgram.String.GetType().GetProperties().FirstOrDefault(o => o.Name == drawTypeName).GetValue(ftaProgram.String);
            }
            else if (info.Column.Name == FixedString.COLUMNAME_DATA) info.CellData = virtualData.data;
            else info.CellData = property?.GetValue(virtualData.data) ?? string.Empty;
         }
      }

      /// <summary>
      /// 当treelist单元格的值被修改时，自定义处理
      /// </summary>
      /// <param name="info"></param>
      void TreeList.IVirtualTreeListData.VirtualTreeSetCellValue(VirtualTreeSetCellValueInfo info)
      {

      }

      /// <summary>
      /// 用于返回treelist子节点的方法
      /// </summary>
      /// <param name="info"></param>
      void TreeList.IVirtualTreeListData.VirtualTreeGetChildNodes(VirtualTreeGetChildNodesInfo info)
      {
         if (info.Node != null)
         {
            var virtualData = ((VirtualDrawData)info.Node);
            if (!is_FilterAllMode)
            {
               //返回DrawData对象的下一层子节点
               if (virtualData.data != null && virtualData.data.Children != null)
               {
                  List<VirtualDrawData> children = new List<VirtualDrawData>();
                  foreach (DrawData tmp in virtualData.data.Children)
                     children.Add(new VirtualDrawData(tmp,General.FtaProgram));
                  info.Children = children;
               }
            }
            else
            {
               //全数据过滤模式下应该返回所有子节点
               if (VirtualDrawData.root != null && virtualData != null && virtualData.data != null && virtualData.data == VirtualDrawData.root)
               {
                  List<VirtualDrawData> children = new List<VirtualDrawData>();
                  foreach (DrawData data in virtualData.data.Children)
                  {
                     GetAllDrawDatas(children, virtualData.data.Children[0]);
                  }
                  info.Children = children;
               }
            }
         }
      }

      /// <summary>
      /// 递归遍历所有Drawdata对象，并放到list里返回
      /// </summary>
      /// <param name="children">存放数据的集合</param>
      /// <param name="root">节点起始数据对象</param>
      private static void GetAllDrawDatas(List<VirtualDrawData> children, DrawData root)
      {
         //转移门不处理，因为根节点的转移门已经展示了
         if (root.Type == DrawType.TransferInGate) return;
         children.Add(new VirtualDrawData(root,_ftrProgram));
         if (root.Children != null && root.Children.Count > 0)
         {
            foreach (DrawData child in root.Children)
            {
               GetAllDrawDatas(children, child);
            }
         }
      }
   }
}
