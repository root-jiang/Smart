using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace FaultTreeAnalysis.Model.Data
{
    /// <summary>
    /// 该类代表FTA程序，保存了数据，配置等信息，提供了相关操作方法
    /// </summary>
    public class ProgramModel
    {
        /// <summary>
        /// 全局的Project对象集合，保存了软件的所有项目信息
        /// </summary>
        public List<ProjectModel> Projects { get; set; }

        /// <summary>
        /// 当前选中的项目对象
        /// </summary>
        [JsonIgnore]
        public ProjectModel CurrentProject { get; private set; }

        /// <summary>
        /// 当前选中的系统对象
        /// </summary>
        [JsonIgnore]
        public SystemModel CurrentSystem { get; private set; }

        /// <summary>
        /// 当前FTA图所展示的根节点数据对象
        /// </summary>
        [JsonIgnore]
        public DrawData CurrentRoot { get; set; }

        [JsonIgnore]
        /// <summary>
        /// 全局的供大家访问的固定字符串对象，用于实现国际化,这个是内置的固定不变的没必要导出
        /// </summary>
        public StringModel String { get; set; }

        /// <summary>
        /// 全局的设置信息保存对象，用于保存用户设置等
        /// </summary>
        public SettingModel Setting { get; set; }

        /// <summary>
        /// 初始化Projects
        /// </summary>
        public ProgramModel()
        {
            Projects = new List<ProjectModel>();
        }

        /// <summary>
        /// 暂时只用于设置Drawdata数据的父对象
        /// </summary>
        public void Initialize()
        {
            foreach (var item in this.Projects) item.Initialize();
        }

        /// <summary>
        /// 从系统节点获得所在项目节点对象
        /// </summary>
        /// <param name="ftaSystem"></param>
        /// <returns></returns>
        public ProjectModel GetFTAProjectFromFTASystem(SystemModel ftaSystem) => this.Projects.FirstOrDefault(o => o.Systems.Contains(ftaSystem));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="system"></param>
        public void SetCurrentSystem(SystemModel system) => this.CurrentSystem = system;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool SetCurrentSystem()
        {
            if (this.Projects?.Count > 0)
            {
                if (this.Projects[0].Systems?.Count > 0) this.CurrentSystem = this.Projects[0].Systems[0];
            }
            return this.CurrentSystem == null ? false : true;
        }

        /// <summary>
        /// 通过指定项目对象对当前项目对象赋值
        /// </summary>
        /// <param name="ftaProject"></param>
        /// <returns></returns>
        public void SetCurrentProject(ProjectModel ftaProject) => this.CurrentProject = ftaProject;

        /// <summary>
        /// 主动匹配可能的项目对象
        /// </summary>
        public bool SetCurrentProject()
        {
            if (this.CurrentSystem != null) this.CurrentProject = this.Projects.FirstOrDefault(o => o.Systems.Contains(this.CurrentSystem));
            else if (this.Projects?.Count > 0) this.CurrentProject = this.Projects[0];
            return this.CurrentProject == null ? false : true;
        }
    }
}
