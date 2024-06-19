using System;
using System.Collections.Generic;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraEditors.Repository;
using FaultTreeAnalysis.Model.Data;
using FaultTreeAnalysis.Common;

namespace FaultTreeAnalysis.View.Ribbon.FTA
{
    public partial class QuickRenumberView : DevExpress.XtraEditors.XtraForm
   {
      private List<RenumberModel> data;
      private StringModel stringModel;
      public QuickRenumberView(StringModel stringModel, List<RenumberModel> data)
      {
         InitializeComponent();
         this.SubscribeEvents();
         this.data = data;
         this.stringModel = stringModel;
      }

      protected override void OnLoad(EventArgs e)
      {
         base.OnLoad(e);

         string keyFieldName = "Id";
         string parentFieldName = "ParentId";

         //新建list数据源
         //this.data = new List<RenumberModel>();
         //data.Add(new RenumberModel());
         //data.Add(new RenumberModel());
         //data.Add(new RenumberModel());


         //先清除列
         this.Tl_Renumber.Columns.Clear();
         //将列数组添加到集合的结尾。
         this.Tl_Renumber.Columns.AddRange(new TreeListColumn[]
         {
            CreateColumn(nameof(StringModel.SystemName),stringModel.SystemName,nameof(RenumberModel.Name)),

            CreateColumn(nameof(StringModel.GatePrefix),stringModel.GatePrefix,nameof(RenumberModel.GatePrefix)),
            CreateColumn(nameof(StringModel.GateStartNumber),stringModel.GateStartNumber,nameof(RenumberModel.GateStartNumber)),
            CreateColumn(nameof(StringModel.GateMinNumber),stringModel.GateMinNumber,nameof(RenumberModel.GateMinNumber)),
            CreateColumn(nameof(StringModel.GateSuffix), stringModel.GateSuffix,nameof(RenumberModel.GateSuffix)),

            CreateColumn(nameof(StringModel.RenumberedType), stringModel.RenumberedType,nameof(RenumberModel.RenumberedTypeString),false),

            CreateColumn(nameof(StringModel.EventPrefix), stringModel.EventPrefix,nameof(RenumberModel.EventPrefix)),
            CreateColumn(nameof(StringModel.EventStartNumber), stringModel.EventStartNumber,nameof(RenumberModel.EventStartNumber)),
            CreateColumn(nameof(StringModel.EventMinNumber), stringModel.EventMinNumber,nameof(RenumberModel.EventMinNumber)),
            CreateColumn(nameof(StringModel.EventSuffix), stringModel.EventSuffix,nameof(RenumberModel.EventSuffix)),

            CreateColumn(nameof(StringModel.AccordingToGate), stringModel.AccordingToGate,nameof(RenumberModel.AccordingToGate)),

         });


         this.Tl_Renumber.CustomNodeCellEdit += Tl_Renumber_CustomNodeCellEdit;
         //this.Tl_Renumber.ShowingEditor += Tl_Renumber_ShowingEditor;

         #region 绑定数据源
         //设置属性KeyFieldName  ParentFieldName
         //设置一个值，该值指定绑定到XtratreeList控件的数据源的键字段
         this.Tl_Renumber.KeyFieldName = keyFieldName;
         //设置一个值，该值表示标识此数据源中父记录的数据源字段。
         this.Tl_Renumber.ParentFieldName = parentFieldName;
         this.Tl_Renumber.DataSource = data;
         //刷新数据
         this.Tl_Renumber.RefreshDataSource();

         #endregion
      }

      //private void Tl_Renumber_ShowingEditor(object sender, CancelEventArgs e)
      //{
      //   //throw new NotImplementedException();
      //}

      private void Tl_Renumber_CustomNodeCellEdit(object sender, GetCustomNodeCellEditEventArgs e)
      {
         if (e.Node != null && e.Column != null)
         {
            switch (e.Column.Name)
            {
               case nameof(StringModel.AccordingToGate):
               {
                  e.RepositoryItem = new RepositoryItemCheckEdit();
                  break;
               }
               case nameof(StringModel.RenumberedType):
               {
                  var result = new RepositoryItemComboBox();
                  var items = new string[]
                  {
                     this.stringModel.OnlyGate,
                     this.stringModel.OnlyEvent,
                     this.stringModel.BothGateAndEvent
                  };
                  result.Items.AddRange(items);
                  e.RepositoryItem = result;

                  break;
               }
            }

         }
      }

      private RepositoryItemComboBox GetComboBoxItemSource()
      {
         var result = new RepositoryItemComboBox();
         result.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
         result.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
         result.Items.AddRange(new string[] { "a", "b", "c" });
         return result;
      }

      private TreeListColumn CreateColumn(string name, string caption, string fieldName, bool isAllowEdit = true)
      {
         var result = new TreeListColumn();
         //设置名字
         result.Name = name;
         //设置标题
         result.Caption = caption;
         //设置从数据源分配给当前列的字段名。
         result.FieldName = fieldName;
         //设置树列表中显示当前列的位置。
         result.VisibleIndex = 0;
         //是否可见
         result.Visible = true;
         //是否允许编辑
         result.OptionsColumn.AllowEdit = isAllowEdit;
         //是否允许移动    
         //result.OptionsColumn.AllowMove = false;
         //是否允许移动至自定义窗体     
         //result.OptionsColumn.AllowMoveToCustomizationForm = false;
         //是否允许排序
         //result.OptionsColumn.AllowSort = false;
         //是否固定列宽         
         result.OptionsColumn.FixedWidth = false;
         //是否只读         
         //result.OptionsColumn.ReadOnly = true;
         //移除列后是否允许在自定义窗体中显示
         result.OptionsColumn.ShowInCustomizationForm = true;

         return result;
      }

      private void SubscribeEvents()
      {
         this.Sb_Cancel.Click += ButtonClick;
         this.Sb_Ok.Click += ButtonClick;
      }

      private void ButtonClick(object sender, EventArgs e)
      {
         if (sender == this.Sb_Ok)
         {
            foreach (SystemModel item in General.FtaProgram.CurrentProject.Systems)
            {
               item.QuickRenumber();
               item.UpdateRepeatedAndTranfer();
            }
            General.InvokeHandler(Model.Enum.GlobalEvent.UpdateLayout);
         }
         this.Close();
      }
   }
}