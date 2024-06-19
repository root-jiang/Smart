using FaultTreeAnalysis.Model.Data;
using System;

namespace FaultTreeAnalysis.FTAControlEventHandle.Ribbon.Start.Edit
{
   /// <summary>
   /// 给用户定义搜索的标题列的小窗体（查找替换功能）
   /// </summary>
   public partial class SearchView : DevExpress.XtraEditors.XtraForm
   {
      /// <summary>
      /// 用户选择的标题列
      /// </summary>
      public string[] Result { get; private set; }

      /// <summary>
      /// 初始化可选标题值，设置控件语言显示
      /// </summary>
      /// <param name="ftaProgram">当前程序对象</param>
      /// <param name="columns">用户可选的FTA表的标题</param>
      public SearchView(ProgramModel ftaProgram, string[] columns)
      {
         InitializeComponent();
         this.BindLangage(ftaProgram.String);
         this.checkedComboBoxEdit1.Properties.Items.AddRange(columns);
         this.RegisterEvent();
      }

      /// <summary>
      /// 查找信息功能的相关事件注册功能
      /// </summary>
      private void RegisterEvent()
      {
         this.simpleButton1.Click += Button_Click;
         this.simpleButton2.Click += Button_Click;
      }

      /// <summary>
      /// 查找信息功能按钮事件函数
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void Button_Click(object sender, EventArgs e)
      {
         if (sender.Equals(this.simpleButton1))
         {
            this.Result = this.checkedComboBoxEdit1.EditValue.ToString().Split(FixedString.SEMICOLON.ToCharArray()[0]);
         }
         this.Close();
         this.UnregisterEvent();
      }

      /// <summary>
      /// 查找信息功能的相关事件注销功能
      /// </summary>
      private void UnregisterEvent()
      {
         this.simpleButton1.Click -= Button_Click;
         this.simpleButton2.Click -= Button_Click;
      }

      /// <summary>
      /// 查找信息窗体语言绑定功能
      /// </summary>
      /// <param name="ftaString">国际化字符串</param>
      private void BindLangage(StringModel ftaString)
      {
         this.Text = ftaString.SearchScopeCaption;
         this.labelControl1.Text = ftaString.SearchScopeTip;
         this.simpleButton1.Text = ftaString.OK;
         this.simpleButton2.Text = ftaString.Cancel;
      }

      /// <summary>
      /// 装载窗体事件重载
      /// </summary>
      /// <param name="e"></param>
      protected override void OnLoad(EventArgs e)
      {
         base.OnLoad(e);
         checkedComboBoxEdit1.Properties.SeparatorChar = FixedString.SEMICOLON.ToCharArray()[0];
         this.checkedComboBoxEdit1.CheckAll();
      }
   }
}