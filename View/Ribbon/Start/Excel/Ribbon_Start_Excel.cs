using Aspose.Cells;
using DevExpress.XtraBars;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraTreeList.Nodes;
using FaultTreeAnalysis.Common;
using FaultTreeAnalysis.Model.Data;
using FaultTreeAnalysis.Model.Enum;
using FaultTreeAnalysis.View.Ribbon.Start.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace FaultTreeAnalysis
{
    partial class FTAControl
    {

        /// <summary>
        /// excel导入时，除了根节点的所有其他子节点暂存的集合，用于得到根节点对象
        /// </summary>
        private List<DrawData> drawDataChildrenCache = new List<DrawData>();

        /// <summary>
        /// 用于判断文件对话框是打开还是保存
        /// </summary>
        private enum DialogType
        {
            OpenFile,
            SaveFile
        }

        /// <summary>
        /// 打开的本地Excel文件名
        /// </summary>
        private string fileLastName;

        private string[] fileLastNames;

        /// <summary>
        /// 初始化Ribbon-Start-Excel下的菜单
        /// </summary>
        private void Init_Ribbon_Start_Excel()
        {
            this.RegisterButtonClick();
            var str = General.FtaProgram.String;
        }

        /// <summary>
        /// 注册按钮事件
        /// </summary>
        private void RegisterButtonClick()
        {
            this.Bbi_Import.ItemClick += BarButtonItemClick;
            this.Bbi_Export.ItemClick += BarButtonItemClick;
        }

        /// <summary>
        /// 点击“导入Excel”按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BarButtonItemClick(object sender, ItemClickEventArgs e)
        {
            var message = string.Empty;
            if (e.Item == this.Bbi_Import)
            {
                if (General.FtaProgram?.CurrentProject?.ColumnFieldInfos == null)
                {
                    MsgBox.Show(General.FtaProgram.String.SelectProject);
                    return;
                }
                var switchImport = new SwitchImport();
                switchImport.ShowDialog();
                var result = switchImport.IsImportToSystem;
                if (result.HasValue)
                {
                    if (result.Value == true)
                    {
                        message = this.ImportFiles();
                    }
                    else
                    {
                        if (General.FtaProgram.CurrentSystem != null)
                        {
                            var newRoots = new List<DrawData>();
                            this.ImportPartExcelFile(out newRoots);
                            var newSystem = new SystemModel(newRoots);

                            //newSystem.UpdateRenumberItem();
                            if (newRoots != null)
                            {
                                this.Renumber(General.FtaProgram.CurrentSystem.GetAllDatas(), newSystem.GetAllDatas());
                                General.FtaProgram.CurrentSystem.Roots.AddRange(newRoots);
                                General.FtaProgram.CurrentSystem.UpdateRepeatedAndTranfer();
                                this.ftaTable.TableEvents.LoadDataToTableControl(General.FtaProgram.CurrentSystem);
                            }
                        }
                    }
                    if (General.FtaProgram.CurrentSystem != null && General.FtaProgram.CurrentSystem.Roots.Count > 0 && General.FtaProgram.CurrentRoot != null)
                    {
                        General.FTATree.FocusedNode = null;

                        Predicate<TreeListNode> match = new Predicate<TreeListNode>(d => (d.GetValue(FixedString.COLUMNAME_DATA) != null && d.GetValue(FixedString.COLUMNAME_DATA) == General.FtaProgram.CurrentRoot));
                        TreeListNode nd = General.FTATree.FindNode(match);
                        General.FTATree.FocusedNode = nd;
                    }
                }
            }
            else if (e.Item == this.Bbi_Export)
                message = this.ExportFilteredDrawDataToExcelFile();

            if (string.IsNullOrEmpty(message) == false) MsgBox.Show(message);
        }

        private void Renumber(List<DrawData> oldDataList, List<DrawData> newDataList)
        {
            var isAuto = false;
            var keyWorld = "(2)";
            var isFront = false;
            for (int i = 0; i < oldDataList.Count; i++)
            {
                var old = oldDataList[i];

                for (int j = 0; j < newDataList.Count; j++)
                {
                    var _new = newDataList[j];
                    // 检测是否重名
                    if (_new.Identifier == old.Identifier)
                    {
                        // 判断是否事件类型
                        if ((_new.Type == old.Type) && (_new.IsGateType == false) && (_new.Type != DrawType.TransferInGate))
                        {
                            // Id重名且是事件类型 则作为重复事件处理
                            var events = newDataList.Where(o => o.Identifier == _new.Identifier);

                            // 带上已有的所有重复事件
                            foreach (var item in events) item.ConvertToRepeatEvent(old);

                        }
                        // 门和转入门
                        else
                        {
                            var result = isFront ? $"{keyWorld}{_new.Identifier}" : $"{_new.Identifier}{keyWorld}";
                            var result3 = this.Renumber(oldDataList.Select(o => o.Identifier).ToList(),
                               newDataList.Select(o => o.Identifier).ToList(), result, keyWorld, isFront);

                            // 有设置非批量处理 则弹出窗体设置命名规则
                            if (isAuto == false)
                            {
                                var renumberConfilict = new RenumberConflictView(result3);
                                renumberConfilict.ShowDialog();
                                isAuto = renumberConfilict.IsAuto;
                                keyWorld = renumberConfilict.KeyWorld;
                                isFront = renumberConfilict.IsFront;
                                result = renumberConfilict.Result;
                            }
                            //else SplashScreenManager.ShowDefaultWaitForm();


                            if (_new.Type == DrawType.TransferInGate)
                            {
                                var transfers = newDataList.Where(o => o.Identifier == _new.Identifier).ToArray();
                                for (int k = 0; k < transfers.Length; k++)
                                {
                                    transfers[k].Identifier = result;
                                }
                            }
                            else _new.Identifier = result;
                        }
                    }
                }

            }
            //SplashScreenManager.CloseDefaultWaitForm();
        }

        private string Renumber(List<string> names, string result, string keyValue, bool isFront)
        {

            while (names.Contains(result))
            {

                result = isFront ? $"{keyValue}{result}" : $"{result}{keyValue}";
            }
            return result;
        }

        private string Renumber(List<string> oldNames, List<string> newNames, string result, string keyWorld, bool isFront)
        {
            var result2 = this.Renumber(oldNames, result, keyWorld, isFront);
            return this.Renumber(newNames, result2, keyWorld, isFront);
        }

        /// <summary>
        /// 获取本地Excel文件的路径
        /// </summary>
        /// <param name="dialogType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private string GetFileName(DialogType dialogType, string name = default(string))
        {
            var result = string.Empty;
            var dialog = default(FileDialog);
            if (dialogType.Equals(DialogType.OpenFile)) dialog = new OpenFileDialog { Filter = FixedString.EXCELXML_FILTER };
            else dialog = new SaveFileDialog { Filter = FixedString.EXCEL_FILTER, FileName = name };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                result = dialog.FileName;
                if (dialogType.Equals(DialogType.OpenFile)) this.fileLastName = (dialog as OpenFileDialog).SafeFileName;
            }
            return result;
        }

        /// <summary>
        /// 导入弹框
        /// </summary>
        /// <param name="dialogType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private List<string> GetFileNames(DialogType dialogType, string name = default(string))
        {
            var result = new List<string>();
            var dialog = default(FileDialog);
            if (dialogType.Equals(DialogType.OpenFile)) dialog = new OpenFileDialog { Multiselect = true, Filter = FixedString.EXCELXML_FILTER };
            else dialog = new SaveFileDialog { Filter = FixedString.EXCELXML_FILTER, FileName = name };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                result.AddRange(dialog.FileNames);
                if (dialogType.Equals(DialogType.OpenFile)) this.fileLastNames = (dialog as OpenFileDialog).SafeFileNames;
            }
            return result;
        }

        /// <summary>
        /// 读取Excel文档到List<DrawData>对象
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="columnFieldInfo"></param>
        /// <returns></returns>
        private List<DrawData> ReadExcelFileToDrawDatas(string filePath)
        {
            List<DrawData> result = new List<DrawData>();
            try
            {
                var book = new Workbook(filePath);
                var cells = default(Cells);
                var dictionary = new Dictionary<string, int>();
                bool CheckP = false;

                foreach (var item in book.Worksheets)
                {
                    var worksheet = item as Worksheet;
                    if (worksheet.Cells.Count >= this.treeList_FTATable.VisibleColumns.Count)
                    {
                        cells = worksheet.Cells;

                        dictionary.Clear();
                        var index1 = 0;

                        foreach (var item1 in cells.Rows[0])
                        {
                            var name = General.GetKeyName((item1 as Cell).StringValue);
                            if (string.IsNullOrEmpty(name) == false)
                                dictionary.Add(name, index1);
                            index1++;
                        }

                        if (dictionary.ContainsKey("Identifier") && dictionary.ContainsKey("Type") && dictionary.ContainsKey("ParentID"))
                        {
                            CheckP = true;
                            break;
                        }
                    }
                }

                if (!CheckP)
                {
                    return null;
                }

                for (int i = 1; i <= cells.MaxRow; i++)
                {
                    var drawData = new DrawData();
                    var row = cells.Rows[i];
                    foreach (var item in dictionary)
                    {
                        var aa = drawData.GetType().GetProperties();
                        var property = drawData.GetType().GetProperties().FirstOrDefault(o => o.Name == item.Key.Replace(" ", ""));
                        property = this.ChangeInputValueToFailure(property, drawData, item);

                        if (property.PropertyType.Name == nameof(DrawType))
                        {
                            var typeName = General.GetKeyName(row[item.Value].StringValue);
                            property.SetValue(drawData, (DrawType)Enum.Parse(typeof(DrawType), typeName));
                        }
                        else if (property.Name == nameof(StringModel.Units))
                        {
                            var unitsName = General.GetKeyName(row[item.Value].StringValue);


                            var aaaa = General.FtaProgram.String.GetType().GetProperties().FirstOrDefault(o => o.Name == unitsName);
                            var unitsValue = aaaa.GetValue(General.FtaProgram.String);
                            property.SetValue(drawData, unitsValue);
                        }
                        else
                        {
                            if (row[item.Value].Value != null)
                            {
                                if (property.Name == nameof(StringModel.LogicalCondition))
                                {
                                    switch (General.GetKeyName(row[item.Value].StringValue))
                                    {
                                        case nameof(StringModel.Normal): { property.SetValue(drawData, General.FtaProgram.String.Normal); break; }
                                        case nameof(StringModel.False): { property.SetValue(drawData, General.FtaProgram.String.False); break; }
                                        case nameof(StringModel.True): { property.SetValue(drawData, General.FtaProgram.String.True); break; }
                                        default: break;
                                    }
                                }
                                else property.SetValue(drawData, row[item.Value].StringValue);
                            }
                        }
                    }

                    //恒定概率空值重置为1
                    if (!drawData.IsGateType && drawData.InputType == General.FtaProgram.String.ConstantProbability)
                    {
                        if (drawData.InputValue2 == "")
                        {
                            drawData.InputValue2 = "1";
                        }
                    }

                    result.Add(drawData);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 导出FTA Excel表
        /// </summary>
        /// <param name="filePath">导出的路径</param>
        /// <param name="drawData">要导出的数据集合</param>
        /// <returns></returns>
        public string WriteDrawDatasToExcelFile(string filePath, List<DrawData> drawData)
        {
            var result = string.Empty;
            try
            {
                var workbook = this.GetWorkbook(drawData, General.FtaProgram.Projects[0].ColumnFieldInfos);
                if (workbook != null)
                {
                    workbook.Worksheets[0].AutoFitColumns();
                    workbook.Worksheets[0].AutoFitRows();
                    workbook.Save(filePath);
                }
            }
            catch (Exception ex) { result = ex.Message; }
            return result;
        }

        /// <summary>
        /// 添加List<DrawData>对象的关联关系
        /// </summary>
        /// <param name="drawData"></param>
        private void SetDrawDatasRelation(List<DrawData> drawData)
        {
            foreach (var item in drawData)
            {
                if (string.IsNullOrEmpty(item.ParentID) == false)
                {
                    if (item.Parent == null)
                    {
                        var parent = drawData.FirstOrDefault(o => o.Type != DrawType.TransferInGate && o.Identifier == item.ParentID);
                        var children = drawData.Where(o => parent != null && o.ParentID == parent.Identifier);
                        if (children != null) this.drawDataChildrenCache.AddRange(children);
                        foreach (var item2 in children) item2.Parent = parent;
                        if (parent != null) parent.Children = children.ToList();
                    }
                }
            }
        }

        /// <summary>
        /// 导入本地Excel文件
        /// </summary>
        private string ImportInfoFromProcess(string info)
        {
            var result = string.Empty;
            var subString = info.Split(';');
            var projectName = subString[0];
            var sytemName = subString[1];
            var path = subString[2];

            if (General.FtaProgram.Projects.FirstOrDefault(o => o.ProjectName == projectName) == null) this.AddProject(projectName, "");

            try
            {
                if (string.IsNullOrEmpty(path) == false)
                {
                    ProjectModel project = null;
                    SplashScreenManager.ShowDefaultWaitForm();
                    var drawData = this.ReadExcelFileToDrawDatas(path);
                    this.SetDrawDatasRelation(drawData);
                    drawData = drawData.Except(this.drawDataChildrenCache).ToList();
                    TreeListNode node = null;
                    foreach (TreeListNode item in this.treeList_Project.Nodes)
                    {
                        if (item.Level == 0)
                        {
                            project = item.Tag as ProjectModel;
                            if (project.ProjectName == projectName)
                            {
                                node = item;
                                break;
                            }
                        }
                    }

                    //未找到工程
                    //不存在工程节点的先创建工程，并加载出其下所有故障树
                    if (node == null)
                    {
                        ProjectModel PMsys = new ProjectModel();
                        PMsys.ProjectPath = General.FtaProgram.Setting.DefaultFilePath + "\\" + projectName;
                        PMsys.ProjectName = projectName;
                        PMsys.Systems = new List<SystemModel>();

                        string[] syss = Directory.GetFiles(PMsys.ProjectPath);

                        foreach (string sys in syss)
                        {
                            if (new FileInfo(sys).Extension == FixedString.APP_EXTENSION)
                            {
                                SystemModel SM = General.GetSystemFromJson(sys);
                                PMsys.Systems.Add(SM);
                            }
                        }

                        General.FtaProgram.Projects.Add(PMsys);

                        RecentFiles.AddProject(PMsys.ProjectPath);//最近文件

                        //定位
                        foreach (TreeListNode PNode in treeList_Project.Nodes)
                        {
                            if (((ProjectModel)PNode.Tag).ProjectPath == PMsys.ProjectPath)
                            {
                                node = PNode;
                            }
                        }
                    }

                    var existNames = project.Systems.Select(o => o.SystemName);
                    TreeListNode Fnode = this.AddSystem(node, General.CreateName(sytemName, existNames), drawData);

                    if (Fnode != null)
                    {
                        this.ActivateLastSystem(Fnode);
                    }

                    General.FtaProgram.SetCurrentProject(project);

                    if (Fnode != null)
                    {
                        this.ActivateLastSystem(Fnode);
                    }
                    if (SplashScreenManager.Default != null) SplashScreenManager.CloseDefaultWaitForm();
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;//General.ftaProgram.String.ImportExcelFailed;
                SplashScreenManager.CloseDefaultWaitForm();
            }
            General.MessageFromSimfia = "";
            return result;
        }

        /// <summary>
        /// 导入本地文件
        /// </summary>
        private string ImportFiles()
        {
            var result = string.Empty;
            try
            {
                var excelFilePaths = this.GetFileNames(DialogType.OpenFile);

                for (int i = 0; i < excelFilePaths.Count; i++)
                {
                    //导入Excel
                    if (string.IsNullOrEmpty(excelFilePaths[i]) == false && (new FileInfo(excelFilePaths[i]).Extension == FixedString.XLS_EXTENSION || new FileInfo(excelFilePaths[i]).Extension == FixedString.XLSX_EXTENSION))
                    {
                        SplashScreenManager.ShowDefaultWaitForm();
                        var drawData = this.ReadExcelFileToDrawDatas(excelFilePaths[i]);

                        if (drawData == null)
                        {
                            if (SplashScreenManager.Default != null)
                            {
                                SplashScreenManager.CloseDefaultWaitForm();
                            }
                            return General.FtaProgram.String.ImportExcelFailed;
                        }

                        this.SetDrawDatasRelation(drawData);
                        drawData = drawData.Except(this.drawDataChildrenCache).ToList();
                        TreeListNode node = null;
                        if (this.treeList_Project.FocusedNode == null) result = General.FtaProgram.String.ImportExcelNoProgramError;
                        else
                        {
                            if (this.treeList_Project.FocusedNode.Tag != null && this.treeList_Project.FocusedNode.Tag.GetType().Equals(typeof(SystemModel)))
                            {
                                node = this.treeList_Project.FocusedNode.ParentNode;
                            }
                            else
                            {
                                node = this.treeList_Project.FocusedNode;
                            }

                            TreeListNode Fnode = this.AddSystem(node, this.fileLastNames[i].Split('.').First(), drawData);

                            if (Fnode != null)
                            {
                                this.ActivateLastSystem(Fnode);
                            }
                        }
                        if (SplashScreenManager.Default != null) SplashScreenManager.CloseDefaultWaitForm();
                    }
                    //导入XML
                    else if (string.IsNullOrEmpty(excelFilePaths[i]) == false && new FileInfo(excelFilePaths[i]).Extension == FixedString.XML_EXTENSION)
                    {
                        SplashScreenManager.ShowDefaultWaitForm();

                        string err = "";
                        var drawData = ImportMediniDatas.ImportXMLFiles(excelFilePaths, ref err);

                        if (err != "")
                        {
                            if (SplashScreenManager.Default != null) SplashScreenManager.CloseDefaultWaitForm();
                            return err;
                        }

                        if (drawData == null)
                        {
                            if (SplashScreenManager.Default != null)
                            {
                                SplashScreenManager.CloseDefaultWaitForm();
                            }
                            return General.FtaProgram.String.ImportExcelFailed;
                        }

                        this.SetDrawDatasRelation(drawData);
                        drawData = drawData.Except(this.drawDataChildrenCache).ToList();
                        TreeListNode node = null;
                        if (this.treeList_Project.FocusedNode == null) result = General.FtaProgram.String.ImportExcelNoProgramError;
                        else
                        {
                            if (this.treeList_Project.FocusedNode.Tag != null && this.treeList_Project.FocusedNode.Tag.GetType().Equals(typeof(SystemModel)))
                            {
                                node = this.treeList_Project.FocusedNode.ParentNode;
                            }
                            else
                            {
                                node = this.treeList_Project.FocusedNode;
                            }

                            TreeListNode Fnode = this.AddSystem(node, this.fileLastNames[i].Split('.').First(), drawData);

                            if (Fnode != null)
                            {
                                this.ActivateLastSystem(Fnode);
                            }
                        }
                        if (SplashScreenManager.Default != null) SplashScreenManager.CloseDefaultWaitForm();
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;//General.ftaProgram.String.ImportExcelFailed; 
                if (SplashScreenManager.Default != null) SplashScreenManager.CloseDefaultWaitForm();
            }
            return result;
        }

        private string ImportPartExcelFile(out List<DrawData> outDrawData)
        {
            var result = string.Empty;
            List<DrawData> drawData = null;
            try
            {
                var excelFilePath = this.GetFileName(DialogType.OpenFile);
                //导入Excel
                if (string.IsNullOrEmpty(excelFilePath) == false && (new FileInfo(excelFilePath).Extension == FixedString.XLS_EXTENSION || new FileInfo(excelFilePath).Extension == FixedString.XLSX_EXTENSION))
                {
                    SplashScreenManager.ShowDefaultWaitForm();
                    drawData = this.ReadExcelFileToDrawDatas(excelFilePath);
                    this.SetDrawDatasRelation(drawData);
                    drawData = drawData.Except(this.drawDataChildrenCache).ToList();
                    TreeListNode node = null;
                    if (this.treeList_Project.FocusedNode == null) result = General.FtaProgram.String.ImportExcelNoProgramError;
                    else
                    {
                        if (this.treeList_Project.FocusedNode.Tag.GetType().Equals(typeof(SystemModel))) node = this.treeList_Project.FocusedNode.ParentNode;
                        else node = this.treeList_Project.FocusedNode;

                    }
                    if (SplashScreenManager.Default != null) SplashScreenManager.CloseDefaultWaitForm();
                }
                //导入XML
                else if (string.IsNullOrEmpty(excelFilePath) == false && new FileInfo(excelFilePath).Extension == FixedString.XML_EXTENSION)
                {
                    SplashScreenManager.ShowDefaultWaitForm();

                    string err = "";
                    drawData = ImportMediniDatas.ImportXMLFiles(new List<string>() { excelFilePath }, ref err);

                    if (err != "")
                    {
                        if (SplashScreenManager.Default != null) SplashScreenManager.CloseDefaultWaitForm();
                        outDrawData = drawData;
                        return err;
                    }

                    this.SetDrawDatasRelation(drawData);
                    drawData = drawData.Except(this.drawDataChildrenCache).ToList();
                    TreeListNode node = null;
                    if (this.treeList_Project.FocusedNode == null) result = General.FtaProgram.String.ImportExcelNoProgramError;
                    else
                    {
                        if (this.treeList_Project.FocusedNode.Tag.GetType().Equals(typeof(SystemModel))) node = this.treeList_Project.FocusedNode.ParentNode;
                        else node = this.treeList_Project.FocusedNode;

                    }
                    if (SplashScreenManager.Default != null) SplashScreenManager.CloseDefaultWaitForm();
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;//General.ftaProgram.String.ImportExcelFailed;
                SplashScreenManager.CloseDefaultWaitForm();
            }
            outDrawData = drawData;
            return result;
        }

        /// <summary>
        /// 导出界面上已筛选的数据到本地Excel
        /// </summary>
        private string ExportFilteredDrawDataToExcelFile()
        {
            var result = string.Empty;
            var filePath = string.Empty;
            var filePathDir = string.Empty;
            try
            {
                //选择要导出的部分 
                if (General.FtaProgram.CurrentProject == null)//未选择，提供全部选择项
                {
                    SwitchExport SE = new View.Ribbon.Start.Excel.SwitchExport();
                    SE.treeList_Project.BringToFront();

                    foreach (ProjectModel PM in General.FtaProgram.Projects)
                    {
                        TreeListNode node_Project = SE.treeList_Project.Nodes.Add(new object[] { PM.ProjectName, "Project" });
                        node_Project.StateImageIndex = 0;
                        node_Project.Tag = PM;

                        foreach (SystemModel sys in PM.Systems)
                        {
                            //添加分组
                            TreeListNode node_GroupLevel = null;
                            if (sys.GroupLevel != null && sys.GroupLevel != "")
                            {
                                foreach (TreeListNode nd in node_Project.Nodes)
                                {
                                    if (nd.GetDisplayText("name") == sys.GroupLevel)
                                    {
                                        node_GroupLevel = nd;
                                    }
                                }
                                if (node_GroupLevel == null)
                                {
                                    node_GroupLevel = node_Project.Nodes.Add(new object[] { sys.GroupLevel, "Group" });
                                }

                                node_GroupLevel.StateImageIndex = 3;
                                if (!node_GroupLevel.Expanded) node_GroupLevel.Expand();
                            }

                            //添加系统节点
                            if (node_GroupLevel == null)
                            {
                                TreeListNode node_sys = node_Project.Nodes.Add(new object[] { sys.SystemName, "System" });
                                node_sys.StateImageIndex = 1;
                                node_sys.Tag = sys;
                            }
                            else
                            {
                                TreeListNode node_sys = node_GroupLevel.Nodes.Add(new object[] { sys.SystemName, "System" });
                                node_sys.StateImageIndex = 1;
                                node_sys.Tag = sys;
                            }
                        }
                    }

                    if (SE.treeList_Project.Nodes.Count == 0)
                    {
                        return "";
                    }

                    SE.treeList_Project.ExpandAll();
                    SE.ShowDialog();

                    if (SE.sysNamesAll.Count > 0 && SE.parentPath != "")
                    {
                        SplashScreenManager.ShowDefaultWaitForm();

                        foreach (KeyValuePair<SystemModel, ProjectModel> sm in SE.sysNamesAll)
                        {
                            if (Directory.Exists(SE.parentPath + "\\" + sm.Value.ProjectName) == false)
                            {
                                Directory.CreateDirectory(SE.parentPath + "\\" + sm.Value.ProjectName);
                            }
                            this.WriteDrawDatasToExcelFile(SE.parentPath + "\\" + sm.Value.ProjectName + "\\" + sm.Key.SystemName + "_" + DateTime.Now.ToString(FixedString.DATETIME_FORMAT) + ".xlsx", sm.Key.GetAllDatas());
                        }

                        if (SplashScreenManager.Default != null) SplashScreenManager.CloseDefaultSplashScreen();

                        filePathDir = SE.parentPath;
                    }
                }
                else if (General.FtaProgram.CurrentProject != null && General.FtaProgram.CurrentSystem == null)//选择的是工程节点
                {
                    SwitchExport SE = new View.Ribbon.Start.Excel.SwitchExport();
                    SE.treeList_Project.BringToFront();

                    TreeListNode node_Project = SE.treeList_Project.Nodes.Add(new object[] { General.FtaProgram.CurrentProject.ProjectName, "Project" });
                    node_Project.StateImageIndex = 0;
                    node_Project.Tag = General.FtaProgram.CurrentProject;

                    foreach (SystemModel sys in General.FtaProgram.CurrentProject.Systems)
                    {
                        //添加分组
                        TreeListNode node_GroupLevel = null;
                        if (sys.GroupLevel != null && sys.GroupLevel != "")
                        {
                            foreach (TreeListNode nd in node_Project.Nodes)
                            {
                                if (nd.GetDisplayText("name") == sys.GroupLevel)
                                {
                                    node_GroupLevel = nd;
                                }
                            }
                            if (node_GroupLevel == null)
                            {
                                node_GroupLevel = node_Project.Nodes.Add(new object[] { sys.GroupLevel, "Group" });
                            }

                            node_GroupLevel.StateImageIndex = 3;
                            if (!node_GroupLevel.Expanded) node_GroupLevel.Expand();
                        }

                        //添加系统节点
                        if (node_GroupLevel == null)
                        {
                            TreeListNode node_sys = node_Project.Nodes.Add(new object[] { sys.SystemName, "System" });
                            node_sys.StateImageIndex = 1;
                            node_sys.Tag = sys;
                        }
                        else
                        {
                            TreeListNode node_sys = node_GroupLevel.Nodes.Add(new object[] { sys.SystemName, "System" });
                            node_sys.StateImageIndex = 1;
                            node_sys.Tag = sys;
                        }
                    }

                    if (SE.treeList_Project.Nodes.Count == 0)
                    {
                        return "";
                    }

                    SE.treeList_Project.ExpandAll();
                    SE.ShowDialog();

                    if (SE.sysNamesAll.Count > 0 && SE.parentPath != "")
                    {
                        SplashScreenManager.ShowDefaultWaitForm();

                        foreach (KeyValuePair<SystemModel, ProjectModel> sm in SE.sysNamesAll)
                        {
                            if (Directory.Exists(SE.parentPath + "\\" + sm.Value.ProjectName) == false)
                            {
                                Directory.CreateDirectory(SE.parentPath + "\\" + sm.Value.ProjectName);
                            }
                            this.WriteDrawDatasToExcelFile(SE.parentPath + "\\" + sm.Value.ProjectName + "\\" + sm.Key.SystemName + "_" + DateTime.Now.ToString(FixedString.DATETIME_FORMAT) + ".xlsx", sm.Key.GetAllDatas());
                        }

                        if (SplashScreenManager.Default != null) SplashScreenManager.CloseDefaultSplashScreen();

                        filePathDir = SE.parentPath;
                    }
                }
                else if (General.FtaProgram.CurrentProject != null && General.FtaProgram.CurrentSystem != null && this.ftaDiagram.SelectedData.Count != 1)//选择的是系统节点
                {
                    if (MsgBox.Show(General.FtaProgram.String.ExportExcelQD, General.FtaProgram.String.MessageBoxCaption, MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        filePath = this.GetFileName(DialogType.SaveFile, $"{General.FtaProgram.CurrentSystem.SystemName}_{DateTime.Now.ToString(FixedString.DATETIME_FORMAT)}");
                        if (string.IsNullOrEmpty(filePath) == false)
                        {
                            SplashScreenManager.ShowDefaultWaitForm();
                            this.WriteDrawDatasToExcelFile(filePath, General.FtaProgram.CurrentSystem.GetAllDatas());
                            if (SplashScreenManager.Default != null) SplashScreenManager.CloseDefaultSplashScreen();
                        }
                    }
                }
                else if (General.FtaProgram.CurrentProject != null && General.FtaProgram.CurrentSystem != null && this.ftaDiagram.SelectedData.Count == 1)//选择的是具体的Gate
                {
                    var selectedData = this.ftaDiagram.SelectedData.First();
                    if (selectedData.IsGateType)
                    {
                        filePath = this.GetFileName(DialogType.SaveFile, $"{General.FtaProgram.CurrentSystem.SystemName}_{DateTime.Now.ToString(FixedString.DATETIME_FORMAT)}");
                        if (string.IsNullOrEmpty(filePath) == false)
                        {
                            SplashScreenManager.ShowDefaultWaitForm();
                            this.WriteDrawDatasToExcelFile(filePath, General.FtaProgram.CurrentSystem.GetAllDatas());
                            if (SplashScreenManager.Default != null) SplashScreenManager.CloseDefaultSplashScreen();
                        }
                    }
                    else
                    {
                        MsgBox.Show(General.FtaProgram.String.CanNotExportPartEvent);
                    }
                }

                //定位目录或打开文件
                if (string.IsNullOrEmpty(filePath) == false)
                {
                    if (MsgBox.Show(General.FtaProgram.String.ExportExcelSucceeded, General.FtaProgram.String.MessageBoxCaption, MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        System.Diagnostics.Process.Start(filePath);
                    }
                }
                else if (string.IsNullOrEmpty(filePathDir) == false)
                {
                    if (MsgBox.Show(General.FtaProgram.String.ExportExcelSucceededPath, General.FtaProgram.String.MessageBoxCaption, MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        System.Diagnostics.Process.Start(filePathDir);
                    }
                }
            }
            catch (Exception exception)
            {
                result = exception.Message;
                if (SplashScreenManager.Default != null) SplashScreenManager.CloseDefaultSplashScreen();
            }

            return result;
        }

        /// <summary>
        /// 通过节点集合生成顶层故障树集合数据的Excel格式的workbook对象
        /// </summary>
        /// <param name="drawData"></param>
        /// <param name="infos"></param>
        /// <returns></returns>
        private Workbook GetWorkbook(List<DrawData> drawData, List<HeaderInfoModel> infos)
        {
            var result = new Workbook();
            try
            {
                var fieldNames = infos.Where(o => o.Visible == true).Select(o => o.Name).ToList();
                var captions = this.treeList_FTATable.Columns.Where(o => o.Visible == true).OrderBy(o => o.VisibleIndex).Select(o => o.Caption).ToList();
                //var fieldNames = infos.Select(o => o.Name).ToList();
                //var captions = this.treeList_FTATable.Columns.Select(o => o.Caption).ToList();
                //this.ChangeFailureToInputValue(captions);
                var sheet = result.Worksheets[0];
                for (int i = 0; i < fieldNames.Count; i++)
                {
                    if (i < captions.Count)
                        sheet.Cells.Rows[0][i].Value = captions[i];
                }

                for (int i = 0; i < drawData.Count; i++)
                {
                    for (int j = 0; j < fieldNames.Count; j++)
                    {
                        var a = fieldNames[j];
                        var b = drawData[i];
                        var value = b.GetType().GetProperties().FirstOrDefault(o => o.Name == a)?.GetValue(b)?.ToString();
                        if (fieldNames[j] == nameof(Type)) value = General.FtaProgram.String.GetType().GetProperties().FirstOrDefault(o => o.Name == value).GetValue(General.FtaProgram.String).ToString();  //DrawData.GetDescriptionByName<DrawType>(value);
                        sheet.Cells[i + 1, j].Value = value;
                    }
                }
            }
            catch (Exception ex) { throw ex; }
            return result;
        }

        private PropertyInfo ChangeInputValueToFailure(PropertyInfo property, DrawData drawData, KeyValuePair<string, int> item)
        {
            if (property == null)
            {
                if (item.Key.Replace(" ", "") == nameof(StringModel.FailureRate)) property = drawData.GetType().GetProperties().FirstOrDefault(o => o.Name == nameof(StringModel.InputValue));
                else if (item.Key.Replace(" ", "") == nameof(StringModel.ExposureTime)) property = drawData.GetType().GetProperties().FirstOrDefault(o => o.Name == nameof(StringModel.InputValue2));
                else if (item.Key.Replace(" ", "") == nameof(StringModel.Votes)) property = drawData.GetType().GetProperties().FirstOrDefault(o => o.Name == nameof(StringModel.ExtraValue1));
            }
            return property;
        }
    }
}
