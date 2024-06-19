using DevExpress.XtraTreeList.Columns;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FaultTreeAnalysis.Model.Data
{
    /// <summary>
    /// 代表一个项目（工程），项目可以有多个系统，还提供从其他路径导入项目的方法
    /// </summary>
    public class ProjectModel
    {
        /// <summary>
        /// 项目名
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 项目名
        /// </summary>
        public string ProjectPath { get; set; }

        /// <summary>
        /// 和project绑定的表格的名字，显示与否等设置信息
        /// </summary>
        public List<HeaderInfoModel> ColumnFieldInfos { get; set; }

        /// <summary>
        /// 和project绑定的样式设置信息，比如图形宽高，图形颜色等
        /// </summary>
        public StyleModel Style { get; set; }

        /// <summary>
        /// 额外绑定的数据对象
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// 工程里的系统集合
        /// </summary>
        public List<SystemModel> Systems { get; set; }

        public List<RenumberModel> RenumberItems => this.Systems.Select(o => o.RenumberItem).ToList();

        public ProjectModel()
        {
            Style = new StyleModel();
        }

        /// <summary>
        /// 暂时只用于设置Drawdata数据的父对象
        /// </summary>
        public void Initialize()
        {
            foreach (var item in this.Systems) item.Initialize();
            this.InitializeColumnFieldInfos();
        }

        private List<HeaderInfoModel> GetDefaultColumnFieldInfos()
        {
            var result = new List<HeaderInfoModel>();
            var properties = typeof(DrawData).GetProperties().Where(o => o.CustomAttributes.Where(o2 => o2.AttributeType.Name.Contains(nameof(System.ComponentModel.DescriptionAttribute))).Count() > 0);
            foreach (var item in properties)
            { 
                if (item.Name == "Identifier" || item.Name == "Type" || item.Name == "Comment1" || item.Name == "LogicalCondition" || item.Name == "InputType" || item.Name == "InputValue" || item.Name == "InputValue2" || item.Name == "Units" || item.Name == "ParentID" || item.Name == "ExtraValue1")
                {
                    result.Add(new HeaderInfoModel(item.Name, item.Name, result.Count, true));
                }
                else
                {
                    result.Add(new HeaderInfoModel(item.Name, item.Name, result.Count, false));
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public void InitializeColumnFieldInfos() => this.ColumnFieldInfos = this.GetDefaultColumnFieldInfos();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="treeListColumn"></param>
        private void EditColumnFieldInfos(TreeListColumn treeListColumn) => this.ColumnFieldInfos.FirstOrDefault(o => o.Name == treeListColumn.Name)?.ChangeProperty(treeListColumn.Name, treeListColumn.Caption, treeListColumn.AbsoluteIndex, treeListColumn.Visible);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="treeListColumns"></param>
        public void EditColumnFieldInfos(IEnumerable<TreeListColumn> treeListColumns)
        {
            foreach (var item in treeListColumns)
            {
                this.EditColumnFieldInfos(item);
            }
        }
    }
}
