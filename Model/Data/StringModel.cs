using System.IO;
using System.Xml.Serialization;

namespace FaultTreeAnalysis.Model.Data
{
    /// <summary>
    /// 程序里用到的需要国际化的所有字符串
    /// </summary>
    public class StringModel
    {
        public string RemovedTransferMessage { get; set; }
        public string CanNotEmpty { get; set; }

        public string CanNotExportPartEvent { get; set; }

        public string PreviousRepeatEvent { get; set; }

        public string NextRepeatEvent { get; set; }

        public string FailureRate { get; set; }

        public string EditPropertyViewTitle { get; set; }

        public string DockMenuFolat { get; set; }

        public string DockMenuDock { get; set; }

        public string DockMenuTabbed { get; set; }

        public string DockMenuHide { get; set; }
        public string DockMenuClose { get; set; }

        public string SearchScopeCaption { get; set; }

        public string SearchScopeTip { get; set; }

        public string MessageBoxCaption { get; set; }

        public string ExportExcelSucceeded { get; set; }

        public string ExportExcelSucceededPath { get; set; }

        public string ExportExcelQD { get; set; }

        public string ExampleNoDo { get; set; }
        public string ConfirmDeletionMessage { get; set; }
        public string ConfirmTitleDelete { get; set; }
        public string ConfirmTitleSave { get; set; }
        public string ConfirmTitleExit { get; set; }

        public string DeleteNodesMessage { get; set; }

        public string DeleteNodesAndTransferMessageOnlyTop { get; set; }

        public string ConfirmExitMessage { get; set; }

        public string ConfirmSavingMessage { get; set; }

        #region Excel导入到处时，用于获取字段本身字符值(nameof(xx))用于标题列显示，放这里感觉并不合适
        public string Identifier { get; set; }
        public string Type { get; set; }
        public string Comment1 { get; set; }
        public string LogicalCondition { get; set; }
        public string Normal { get; set; }
        public string FailureProbability { get; set; }
        public string ConstantProbability { get; set; }
        public string False { get; set; }
        public string True { get; set; }
        public string InputType { get; set; }
        public string FRType { get; set; }
        public string ExposureTimePercentage { get; set; }
        public string DormancyFactor { get; set; }
        public string FRPercentage { get; set; }
        public string InputValue { get; set; }
        public string InputValue_Constant { get; set; }
        public string InputValue_Failed { get; set; }
        public string InputValue_Failed1 { get; set; }
        public string InputValue_Failed2 { get; set; }
        public string InputValue2 { get; set; }
        public string Units { get; set; }
        public string Hour { get; set; }
        public string Minute { get; set; }
        public string ProblemList { get; set; }
        public string SelectProject { get; set; }
        public string IfOnlyEvent { get; set; }

        public string ExtraValue1 { get; set; }
        public string ExtraValue2 { get; set; }
        public string ExtraValue3 { get; set; }
        public string ExtraValue4 { get; set; }
        public string ExtraValue5 { get; set; }
        public string ExtraValue6 { get; set; }
        public string ExtraValue7 { get; set; }
        public string ExtraValue8 { get; set; }
        public string ExtraValue9 { get; set; }
        public string ExtraValue10 { get; set; }

        public string IntegrityCheckString_HasNoRoot { get; set; }
        public string IntegrityCheckString_HasNoChild { get; set; }
        public string IntegrityCheckString_HasNoChildOrOnlyHouseEvent { get; set; }
        public string IntegrityCheckString_HasNoValue { get; set; }
        public string IntegrityCheckWarning_Unreasonable { get; set; }
        public string IntegrityCheckWarning_UnreasonableVote { get; set; }
        public string IntegrityCheckWarning_Nodescription { get; set; }
        public string IntegrityCheckWarning_Abnormalidentifier { get; set; }
        public string IntegrityCheckString_CalculateSuccess { get; set; }
        public string IntegrityCheckString_CalculateFail { get; set; }
        public string IntegrityCheckString_CalculateFailed { get; set; }
        public string IntegrityCheckString_CalculateSuccessRoot { get; set; }
        public string IntegrityCheckString_CalculateSuccessSelect { get; set; }

        public string PrintingSelection_Text { get; set; }
        public string PrintingSelection_Group { get; set; }
        public string PrintingSelection_Item1 { get; set; }
        public string PrintingSelection_Item2 { get; set; }
        public string PrintingSelection_Item3 { get; set; }

        public string SwitchImport_Text { get; set; }
        public string SwitchImport_Item1 { get; set; }
        public string SwitchImport_Item2 { get; set; }
        public string SwitchExport_Text { get; set; }
        public string SwitchExport_Check { get; set; }

        public string FindAndReplace_Text { get; set; }
        public string FindAndReplace_TabFind { get; set; }
        public string FindAndReplace_TabReplace { get; set; }
        public string FindAndReplace_SearchContent { get; set; }
        public string FindAndReplace_ColumnRange { get; set; }
        public string FindAndReplace_Matching { get; set; }
        public string FindAndReplace_CaseSensitive { get; set; }
        public string FindAndReplace_ReplaceContent { get; set; }
        public string FindAndReplace_AboveDown { get; set; }
        public string FindAndReplace_DownUp { get; set; }
        public string FindAndReplace_Next { get; set; }
        public string FindAndReplace_ReplaceAll { get; set; }
        public string FindAndReplace_Inclusion { get; set; }
        public string FindAndReplace_WholeWord { get; set; }


        public string ParentID { get; set; }
        #endregion
        //新建工程窗体
        public string ProjectName { get; set; }
        public string GroupName { get; set; }
        public string Groups { get; set; }

        public string OK { get; set; }
        public string ShowCut { get; set; }
        public string ShowCutCheck { get; set; }
        public string ShowCutName { get; set; }
        public string ShowCutszProb { get; set; }
        public string ShowCutLevel { get; set; }

        public string Cancel { get; set; }
        public string FixedValue { get; set; }

        public string Path { get; set; }

        public string NewProject { get; set; }
        //新建系统窗体
        public string SystemName { get; set; }

        public string NewSystem { get; set; }
        public string NewFolder { get; set; }
        public string RenameFolder { get; set; }
        public string DeleteFolder { get; set; }
        public string FaultTreeCopy { get; set; }
        public string FaultTreePaste { get; set; }
        public string PosParent { get; set; }
        public string PosChild { get; set; }
        public string PosLeft { get; set; }
        public string PosRight { get; set; }
        public string OpenDir { get; set; }
        public string CloseDir { get; set; }
        //FTAControl
        public string FTADiagram { get; set; }

        public string New { get; set; }

        public string Project { get; set; }

        public string System { get; set; }

        public string Open { get; set; }

        public string LocalProject { get; set; }

        public string CurrentAspectProject { get; set; }

        public string AddE { get; set; }
        public string Save { get; set; }
        public string SaveAll { get; set; }

        public string OpenProject { get; set; }
        public string OpenFaultTree { get; set; }

        public string SaveAs { get; set; }

        public string RecentProjects { get; set; }
        public string RecentFiles { get; set; }
        public string OpenRecentFiles { get; set; }

        public string Exit { get; set; }

        public string SetPassword { get; set; }

        public string PrintPreview { get; set; }

        public string PrintSelectedRecords { get; set; }

        public string PrintTable { get; set; }

        public string PrintSettting { get; set; }

        public string GraphicTools { get; set; }

        public string BasicShape { get; set; }

        public string Null { get; set; }

        public string AndGate { get; set; }

        public string SplashScreenCommand_Initial1 { get; set; }
        public string SplashScreenCommand_Initial2 { get; set; }
        public string SplashScreenCommand_Initial3 { get; set; }


        public string OrGate { get; set; }

        public string BasicEvent { get; set; }

        public string LinkGate { get; set; }

        public string UndevelopedEvent { get; set; }

        public string TransferInGate { get; set; }

        public string HouseEvent { get; set; }

        public string PriorityAndGate { get; set; }

        //public string InhibitGate { get; set; }

        public string ConditionEvent { get; set; }

        public string XORGate { get; set; }

        public string VotingGate { get; set; }

        public string RemarksGate { get; set; }

        public string Label { get; set; }

        public string InsertLabel { get; set; }

        public string Bold { get; set; }

        public string Italic { get; set; }

        public string Underline { get; set; }

        public string AlignLeft { get; set; }

        public string AlignCenter { get; set; }

        public string AlignRight { get; set; }

        public string Cut { get; set; }

        public string Copy { get; set; }
        public string CopyCurrentView { get; set; }

        public string CopyCurrentSelected { get; set; }

        public string Paste { get; set; }

        public string Undo { get; set; }

        public string Redo { get; set; }

        public string FindAndReplace { get; set; }

        public string FindAndReplaceCustom { get; set; }

        public string SpellingCheck { get; set; }

        public string Graph { get; set; }

        public string GlobalChange { get; set; }

        public string CreateExcelSpreadSheet { get; set; }

        public string Import { get; set; }

        public string ImportXML { get; set; }

        public string Export { get; set; }

        public string Filter { get; set; }

        public string Ruler { get; set; }

        public string Grid { get; set; }

        public string PageBreaks { get; set; }

        public string CanvasFillMode { get; set; }

        public string PageScrollMode { get; set; }

        public string ShapeColor { get; set; }

        public string ShapeRectWidth { get; set; }

        public string IdRectHeight { get; set; }

        public string ShapeRectHeight { get; set; }

        public string SymbolRectSize { get; set; }

        public string FontName { get; set; }
        public string FontGroup { get; set; }

        public string FontSize { get; set; }

        public string PasteRepeatedEvent { get; set; }

        public string RepeatedEventColor { get; set; }

        public string TrueGateColor { get; set; }

        public string FalseGateColor { get; set; }

        public string SelectedShapeColor { get; set; }

        public string FTAReport { get; set; }

        public string RenumberFaultTree { get; set; }

        public string QuickRenumber { get; set; }

        public string ExposureTime { get; set; }

        public string Votes { get; set; }

        public string EventModify { get; set; }

        public string Check { get; set; }
        public string Check_Type { get; set; }
        public string Check_Name { get; set; }
        public string Check_Info { get; set; }

        public string Refresh { get; set; }

        public string ZoomIn { get; set; }

        public string ZoomOut { get; set; }

        public string FTACalculate { get; set; }
        public string FTACalculateSelected { get; set; }
        public string CusetStr { get; set; }

        public string Insert { get; set; }

        public string Delete { get; set; }
        public string ExportToModel { get; set; }
        public string LoadFromModel { get; set; }
        public string BasicLib { get; set; }
        public string AddToBasicEventLibrary { get; set; }
        public string SynchronizeFromBasicEventLibrary { get; set; }
        public string InsertFromBasicEventLibrary { get; set; }

        public string InsertRepeatedEvent { get; set; }
        public string AddLinkGate { get; set; }

        public string UpDateLinkGate { get; set; }

        public string BreakintoTransfer { get; set; }

        public string CollapseTransfer { get; set; }

        public string TransferTo { get; set; }
        public string GateSettings { get; set; }

        public string ImportExportGtoup { get; set; }

        public string HighlightCutSets { get; set; }
        public string StartPage { get; set; }

        public string DeleteSystem { get; set; }

        public string DeleteProject { get; set; }

        public string RemoveProject { get; set; }

        public string RemoveSystem { get; set; }

        public string RenameProject { get; set; }

        public string RenameSystem { get; set; }

        public string Modeling { get; set; }
        public string RibbonFile { get; set; }

        public string Edit { get; set; }

        public string Parameters { get; set; }

        public string InsertLevel { get; set; }

        public string ChangeGateType { get; set; }
        public string TopGatePos { get; set; }

        public string Color { get; set; }

        public string Report { get; set; }

        public string ToolBox { get; set; }

        public string View { get; set; }

        public string Calculation { get; set; }
        public string col_Pic { get; set; }
        public string col_Name { get; set; }
        public string col_Path { get; set; }

        public string TextAbout { get; set; }
        public string TextAboutForm { get; set; }

        public string Help { get; set; }
        public string Soft { get; set; }
        public string UpdateTip { get; set; }

        public string Version { get; set; }

        public string SplashInfo { get; set; }

        public string UpdateDate { get; set; }
        public string UpdateMessage { get; set; }

        public string UserManual { get; set; }
        public string About_Information { get; set; }
        public string About_Settings { get; set; }
        public string About_DefaultFilePath { get; set; }
        public string About_CommonEventLibraryPath { get; set; }
        public string Examples { get; set; }

        public string UserManualTip { get; set; }
        public string ShowKeymap { get; set; }
        public string ShowKeymapName { get; set; }
        public string ShowKeymapKey { get; set; }
        public string CheckforUpdates { get; set; }
        public string AboutSmarTree { get; set; }

        public string Documentation { get; set; }

        public string ExistProject { get; set; }
        public string ExistFaultTree { get; set; }

        public string Skin { get; set; }

        public string FoldProject { get; set; }

        public string ExpandProject { get; set; }

        public string Properties { get; set; }
        public string CloseProperties { get; set; }

        public string Projects { get; set; }

        public string Canvas { get; set; }

        public string ProjectNavigator { get; set; }

        public string FTATable { get; set; }

        public string Problems { get; set; }

        public string ProblemsForm { get; set; }
        // Cretaenewproject
        public string ProjectNameCannotBeEmpty { get; set; }
        public string ProjectNameCannotOver { get; set; }
        public string SmarTreeNameCannotOver { get; set; }
        public string AllNameCheck { get; set; }
        //Createnewsystem
        public string Systemnamecannotbeempty { get; set; }
        //datahelper
        public string NoProjectsopen { get; set; }

        public string ToplevelnodeID { get; set; }

        public string NoparentPackage { get; set; }

        public string NoStereoType { get; set; }

        public string Exceptionloadingfaulttreedata { get; set; }

        public string Parametercannotbeempty { get; set; }

        public string Nodatafound { get; set; }

        public string IsolatednodesfoundwithID { get; set; }

        public string Whethertocontinue { get; set; }
        public string StartPageWelcome { get; set; }

        public string Erroroccurred { get; set; }
        //applicationmenu
        public string Failedtoquerydata { get; set; }

        public string NoAspectProject { get; set; }

        public string Dataisbeingquerieddonotrepeatthequery { get; set; }
        //ftatable_colummenu
        public string SortAscending { get; set; }

        public string SortDescending { get; set; }

        public string ColumnChooser { get; set; }

        public string BestFitCurrentColumn { get; set; }

        public string BestFitallcolumns { get; set; }

        public string FilterEditor { get; set; }

        public string ShowFindPanel { get; set; }

        public string ShowAutoFilterRow { get; set; }

        public string ClearSorting { get; set; }

        public string ClearAllSorting { get; set; }

        public string HideFindPanel { get; set; }

        public string HideAutoFilterRow { get; set; }

        public string Hidefiltereditor { get; set; }

        public string Filtervisibledatamodedefault { get; set; }

        public string Filteralldatamode { get; set; }
        //ftatable_nodeMenu
        public string InsertNode { get; set; }
        public string InsertLink { get; set; }

        public string InsertTopNode { get; set; }

        public string CutNodes { get; set; }

        public string CopyNode { get; set; }

        public string CopyNodes { get; set; }

        public string PasteAsTopNode { get; set; }

        public string CancelCopyOrCut { get; set; }

        public string DeleteNodes { get; set; }

        public string DeleteNodes_HasTransfer { get; set; }

        public string DeleteNodesAndTransfer { get; set; }

        public string DeleteTop { get; set; }
        public string DeleteLevel { get; set; }
        public string DeleteNode { get; set; }

        public string ExpandNodes { get; set; }

        public string CollapseNodes { get; set; }

        public string FreezeColumn { get; set; }

        public string UnfreezeColumns { get; set; }

        public string Thereisatleastonenodeintheproject { get; set; }
        //projectrename
        public string Namecannotbeempty { get; set; }

        public string Projectnamealreadyexists { get; set; }
        public string Foldernamealreadyexists { get; set; }

        public string Systemnamealreadyexists { get; set; }

        public string Language { get; set; }

        public string ImportExcelOK { get; set; }

        public string ExportExcelOK { get; set; }

        public string ExportFilteredExcelOK { get; set; }

        public string RenumberWizardCancelDialogTitle { get; set; }

        public string RenumberWizardCancelDialogContent { get; set; }

        public string ImportExcelNoProgramError { get; set; }

        public string ExportNoDataError { get; set; }

        public string ImportExcelFailed { get; set; }

        public string ImportExcelIdFailed { get; set; }

        public string ImportExcelTypeFailed { get; set; }

        public string ImportExcelParentIdFailed { get; set; }

        public string RenumberWizardTitle { get; set; }

        public string RenumberWizardPage1Caption { get; set; }

        public string RenumberWizardPage2Caption { get; set; }

        public string RenumberWizardPage3Caption { get; set; }

        public string RenumberWizardPage4Caption { get; set; }

        public string RenumberWizardPage5Caption { get; set; }

        public string RenumberWizardPage6Caption { get; set; }

        public string RenumberWizardPage1Content { get; set; }

        public string RenumberWizardPage2Content { get; set; }

        public string RenumberWizardPage3Content { get; set; }

        public string RenumberWizardPage4Content { get; set; }

        public string RenumberWizardPage5Content { get; set; }

        public string RenumberWizardPage6Content { get; set; }

        public string RenumberWizardPage1Tip { get; set; }

        public string RenumberWizardPage2Tip { get; set; }

        public string RenumberWizardPage3Tip { get; set; }

        public string RenumberWizardPage4Tip { get; set; }

        public string RenumberWizardPage5Tip { get; set; }

        public string RenumberWizardPage6Tip { get; set; }

        public string GatePrefix { get; set; }

        public string EventPrefix { get; set; }

        public string GateStartNumber { get; set; }

        public string EventStartNumber { get; set; }

        public string GateMinNumber { get; set; }

        public string EventMinNumber { get; set; }

        public string GateSuffix { get; set; }

        public string EventSuffix { get; set; }

        public string GateLabel5 { get; set; }

        public string GateLabel6 { get; set; }

        public string AccordingToGate { get; set; }

        public string RenumberedType { get; set; }

        public string RenumberRadiogroup1Raido1Text { get; set; }

        public string RenumberRadiogroup1Raido2Text { get; set; }

        public string RenumberRadiogroup1Raido3Text { get; set; }

        public string OnlyGate { get; set; }

        public string OnlyEvent { get; set; }

        public string BothGateAndEvent { get; set; }

        public string Yes { get; set; }

        public string No { get; set; }

        public string Previous { get; set; }

        public string Next { get; set; }

        public string Finish { get; set; }

        public string WizardCancel { get; set; }

        public string RenumberingSucceeded { get; set; }

        public string AtLeastSelectOneNode { get; set; }
        public string MastCal { get; set; }

        public string EventModifyDescription { get; set; }

        public string EventName { get; set; }

        public string EventModifyMessage { get; set; }

        public string LineColor { get; set; }

        public string LineStyle { get; set; }

        public string ArrowStyle { get; set; }

        public string ArrowSize { get; set; }

        public string Move { get; set; }

        public string Scale { get; set; }

        public string FTATableEditor { get; set; }
        public string FTAGroup { get; set; }

        public string Indicator { get; set; }

        public string IndicatorTopGate { get; set; }

        public string IndicatorTransInGate { get; set; }

        public string EmptyError { get; set; }

        public string ConvertToCopy { get; set; }

        public string ConvertToAnotherCopy { get; set; }

        public string ConvertToRepeatedEvents { get; set; }

        public string ConvertToAnotherRepeatedEvents { get; set; }

        public string IdAlreadyExist { get; set; }

        public string PositiveNumber { get; set; }

        public string CutSetColor { get; set; }

        public string Successfully_Saved { get; set; }
        public string NoSaved { get; set; }
        public string NoSavedAll { get; set; }
        public string SavingFaultTree { get; set; }
        public string SavingAll { get; set; }

        public string Property { get; set; }

        public string WindowLayout { get; set; }
        public string ShowFullScreen { get; set; }
        public string TipDescription_FullScreen { get; set; }
        public string TipDescription_InsertLevel { get; set; }
        public string TipDescription_TopGatePos { get; set; }
        public string TipDescription_ChangeGateType { get; set; }
        public string TipDescription_Undo { get; set; }
        public string TipDescription_Redo { get; set; }
        public string TipDescription_Cut { get; set; }
        public string TipDescription_Copy { get; set; }
        public string TipDescription_Paste { get; set; }
        public string TipDescription_Check { get; set; }
        public string TipDescription_Calculate { get; set; }
        public string TipDescription_Import { get; set; }
        public string TipDescription_Export { get; set; }
        public string TipDescription_FTAReport { get; set; }
        public string TipDescription_CutsetReport { get; set; }
        public string TipDescription_SaveCommonEventLibrary { get; set; }
        public string TipDescription_CommonEventLibrary { get; set; }
        public string TipDescription_StartPage { get; set; }
        public string TipDescription_ZoomIn { get; set; }
        public string TipDescription_ZoomOut { get; set; }
        public string TipDescription_Windows { get; set; }
        public string TipDescription_FoldProject { get; set; }
        public string TipDescription_ExpandProject { get; set; }
        public string TipDescription_Properties { get; set; }
        public string TipDescription_UserManual { get; set; }
        public string TipDescription_Keymap { get; set; }
        public string TipDescription_Updates { get; set; }
        public string TipDescription_AboutSmarTree { get; set; }

        public string ExitFullScreen { get; set; }
        public string HideShowToolbar { get; set; }
        public string HideShowProjectNavigator { get; set; }

        public string Window { get; set; }

        public string NewFTAItem_Text { get; set; }

        public string NewFTAItem_Label { get; set; }

        /// <summary>
        /// 把之前导出的FTAStringXML文件导入为一个对象
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        public static StringModel ImportFTAString(string fullFileName)
        {
            StringModel ftaString = null;
            if (!File.Exists(fullFileName))
            {
                return null;
            }
            //反序列化xml
            XmlSerializer Serializer = new XmlSerializer(typeof(StringModel));
            using (FileStream stream = new FileStream(fullFileName, FileMode.Open))
            {
                ftaString = (StringModel)Serializer.Deserialize(stream);
            }
            return ftaString;
        }

        /// <summary>
        /// 导出FTAString为一个XML文件
        /// </summary>
        /// <param name="ftaString"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        public static void ExportFTAString(StringModel ftaString, string path, string fileName)
        {
            path = path.TrimEnd(new char[] { '/', '\\' }) + "/";
            fileName = fileName.Trim();
            if (Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            XmlSerializer formatter = new XmlSerializer(typeof(StringModel));
            using (FileStream stream = new FileStream(path + fileName, FileMode.Create))
            {
                formatter.Serialize(stream, ftaString);
            }
        }

        /// <summary>
        /// 获取英文版的字符串对象
        /// </summary>
        /// <returns></returns>
        public static StringModel GetENFTAString()
        {
            return new StringModel
            {
                AlignLeft = "Align Left",
                AlignCenter = "Align Center",
                AlignRight = "Align Right",
                AndGate = "And Gate",
                ArrowStyle = "Arrow Style",
                ArrowSize = "Arrow Size  ",

                SplashScreenCommand_Initial1 = "Initializing Configuration",
                SplashScreenCommand_Initial2 = "Initializing Control",
                SplashScreenCommand_Initial3 = "Initializing StartPage",

                BreakintoTransfer = "Break Into Transfer",
                Bold = "Bold",
                BasicShape = "Basic Shape",
                BasicEvent = "Basic Event",
                LinkGate = "Link Gate",
                BestFitCurrentColumn = "BestFit(CurrentColumn)",
                BestFitallcolumns = "BestFit(AllColumns)",

                CurrentAspectProject = "Current Aspect Project",
                ColumnChooser = "Column Chooser",
                CutNodes = "Cut",
                CopyNode = "Copy",
                CopyNodes = "Copy",
                ClearSorting = "Clear Sorting",
                ClearAllSorting = "Clear All Sorting",
                CancelCopyOrCut = "Cancel Copy Or Cut",
                CollapseNodes = "Collapse All Nodes",
                AtLeastSelectOneNode = "Please select one node from a Fault Tree.",
                MastCal = "Please select one node from a Fault Tree and Calculate",
                ConfirmDeletionMessage = "Are you sure you want to delete this node?",
                ExampleNoDo = "The example project cannot be operated.",
                ConfirmExitMessage = "Exit the SmarTree?",

                ConfirmTitleDelete = "Confirm Delete",
                ConfirmTitleSave = "Confirm Save",
                ConfirmTitleExit = "Confirm Exit",

                ConfirmSavingMessage = "Do you want to save all changes?",
                Cut = "Cut",
                Copy = "Copy",
                CopyCurrentView = "Copy Current View Screenshot To Clipboard",
                CopyCurrentSelected = "Copy Selected Screenshot To Clipboard",
                Calculation = "Calculation",
                col_Pic = "Pic",
                col_Name = "Name",
                col_Path = "Details",

                TextAbout = "   Welcome to Smartree, this is an engineering software for system fault tree analysis, suitable for scientific research, project development and other purposes.The software provides convenient and practical fault tree creation, editing and calculation functions, supports rapid batch processing of multiple fault tree files, can carry out a variety of formats of fault tree data import, export and fault tree analysis report generation, has a wealth of fault tree editing options, to meet the needs of users in the use of software.To learn more about the software features, refer to the Smartree software user manual in the Help.Finally, we sincerely welcome you to use Smartree again. If you have any questions, please feel free to contact us.",


                TextAboutForm = "   Welcome to Smartree, this is an engineering software for system fault tree analysis, suitable for scientific research, project development and other purposes.The software provides convenient and practical fault tree creation, editing and calculation functions, supports rapid batch processing of multiple fault tree files, can carry out a variety of formats of fault tree data import, export and fault tree analysis report generation, has a wealth of fault tree editing options, to meet the needs of users in the use of software.To learn more about the software features, refer to the Smartree software user manual in the Help.Finally, we sincerely welcome you to use Smartree again. If you have any questions, please feel free to contact us.",

                CollapseTransfer = "Collapse Transfer",
                Canvas = "Property",
                CreateExcelSpreadSheet = "Export part",
                CanvasFillMode = "Canvas Fill Mode",
                Cancel = "Cancel",
                FixedValue = "Fixed Value",
                CutSetColor = "Cutsets Marker",
                ConvertToCopy = "Are you sure convert all the trans-in gates to another copy of trans-in gates?",
                ConvertToAnotherCopy = "Are you sure convert to another copy of trans-in gates?",
                ConvertToRepeatedEvents = "Are you sure convert all the trans-in gates to repeated events?",
                ConvertToAnotherRepeatedEvents = "Are you sure convert to another repeated events?",
                Comment1 = "Description",
                ConditionEvent = "Condition Event",
                CanNotExportPartEvent = "Single basic event cannot be exported",

                DeleteSystem = "Delete Fault Tree",
                DeleteProject = "Delete Project",
                Delete = "Delete",
                ExportToModel = "Save to Common Library",
                LoadFromModel = "Common Library",

                BasicLib = "Basic Event Library",
                AddToBasicEventLibrary = "Add To Basic Event Library",
                SynchronizeFromBasicEventLibrary = "Synchronize From Basic Event Library",
                InsertFromBasicEventLibrary = "Insert From Basic Event Library",

                DeleteNodesAndTransfer = "Delete（All chlid nodes)",
                DormancyFactor = nameof(DormancyFactor),
                DockMenuFolat = "Folat",
                DockMenuDock = "Dock",
                DockMenuTabbed = "Dock as tabbed document",
                DockMenuHide = "Auto hide",
                DockMenuClose = "Close",
                DeleteTop = "Delete（All chlid nodes)",
                DeleteLevel = "Delete level",
                DeleteNodes = "Delete（All chlid nodes)",
                DeleteNodes_HasTransfer = "Delete（Include transfer gate and other child nodes）",
                DeleteNodesMessage = "Are you sure you want to delete this node and its child node(s)?",
                DeleteNodesAndTransferMessageOnlyTop = "Are you sure you want to delete its child node(s)? ",
                DeleteNode = "Delete (Only current node)",
                Dataisbeingquerieddonotrepeatthequery = "Data is being queried, do not repeat the query",

                Edit = "Edit",
                Export = "Export",
                EmptyError = "Can not be empty",
                ExportExcelSucceeded = "Do you want to open the exported file?",
                ExportExcelSucceededPath = "Do you want to open the exported file path？",
                ExportExcelQD = "Do you want to export the FaultTree？",
                ExposureTime = "Exposure Time",
                Votes = "Votes",
                ExposureTimePercentage = nameof(ExposureTimePercentage),
                EditPropertyViewTitle = "Property Editor",
                EventModify = "Common Event Library",
                Check = "Check",
                Check_Type = "Type",
                Check_Name = "Name",
                Check_Info = "Infomation",
                Erroroccurred = "Error occurred",
                Exceptionloadingfaulttreedata = "Failed to import FaultTree data",
                ExportNoDataError = "No data can be exported",
                EventModifyDescription = "Please input event name to modify",
                EventModifyMessage = "Please select at least one Event object",
                EventName = "Event Name",
                ExpandNodes = "Expand All Nodes",
                ExportExcelOK = "Export Excel file succeeded",
                ExportFilteredExcelOK = "Export filtered Excel file succeeded",
                EventPrefix = nameof(EventPrefix),
                EventStartNumber = nameof(EventStartNumber),
                EventMinNumber = nameof(EventMinNumber),
                EventSuffix = nameof(EventSuffix),

                FailureRate = "Failure Rate",
                FTADiagram = "FTADiagram",
                OpenDir = "Open Directory",
                CloseDir = "Close Project",
                FTATableEditor = "FTA Table Editor",
                FTAGroup = "Check And Renumber",
                Finish = "Finish",
                FindAndReplace = "Find",
                FindAndReplaceCustom = "Find And Replace",
                Filter = "Filter",
                FontName = "Font Name",
                FontGroup = "Font And Zoom Settings",
                FontSize = "Font Size   ",
                FalseGateColor = "False Gate/Event",
                FTAReport = "FTA Report",
                Parameters = "Parameters",
                InsertLevel = " Insert Level",
                ChangeGateType = "Change GateType",
                TopGatePos = "TopGate Pos",
                GateSettings = "GateSettings",
                Color = "Color",
                FTATable = "FTATable",
                FilterEditor = "FilterEditor",
                FRType = nameof(FRType),
                Filtervisibledatamodedefault = "Filter visible Data Mode (Default)",
                Filteralldatamode = "Filter All Data Mode",
                FRPercentage = nameof(FRPercentage),
                FreezeColumn = "Freeze Column",
                FTACalculate = "Calculator",
                FTACalculateSelected = "Calculator Selected",
                CusetStr = "MaxCutset",
                Failedtoquerydata = "Failed to query data",
                Graph = "Graph",
                GlobalChange = "Global Change",
                Grid = "Grid",
                GraphicTools = "Graphic Tools",
                GatePrefix = "GatePrefix",
                GateStartNumber = nameof(GateStartNumber),
                GateMinNumber = nameof(GateMinNumber),
                GateSuffix = nameof(GateSuffix),
                GateLabel5 = "Example:",
                GateLabel6 = "Save symbol when finished",
                AccordingToGate = "Use the same number for Gate and Event",

                HouseEvent = "House Event",
                HideFindPanel = "Hide FindPanel",
                HideAutoFilterRow = "Hide AutoFilterRow",
                Hidefiltereditor = "Hide FilterEditor",
                HighlightCutSets = "HighLight CutSets",
                StartPage = "Start Page",
                Hour = "Hours",

                //InhibitGate = "Inhibit Gate",
                Italic = "Italic",
                InsertNode = "Insert Input",
                InsertLink = "Insert Link",
                InsertTopNode = "Insert New Top Event",
                IsolatednodesfoundwithID = "Isolated nodes found with ID：",
                Identifier = nameof(Identifier),
                InputValue = "Failure Rate",
                InputValue_Constant = "Probability",
                InputValue_Failed = "The probability must be greater than or equal to 0 and less than or equal to 1",
                InputValue_Failed1 = "The probability must be greater than 0 and less than 1",
                InputValue_Failed2 = "invalid data",
                InputValue2 = "Exposure Time",
                InputType = "Input Type",
                Import = "Import",
                ImportXML = "Import XML",
                InsertLabel = "InsertLabel",
                Insert = "Insert",
                IdRectHeight = "IdRect Height",
                InsertRepeatedEvent = "Insert Repeated Event",
                AddLinkGate = "Add Link Gate",
                UpDateLinkGate = " UpDate Link Gate Data",
                ImportExcelOK = "Import Excel file succeeded",
                ImportExcelNoProgramError = "Please make sure that at least one project exisit.",
                ImportExcelFailed = "Import Excel failed,please check the file.",
                ImportExcelIdFailed = "Field \"Id\" not found in imported file.",
                ImportExcelTypeFailed = "Field \"Type\" not found in imported file.",
                ImportExcelParentIdFailed = "Field \"ParentId\" not found in imported file.",
                Indicator = "Indicator    ",
                IndicatorTopGate = "Top Gate     ",
                IndicatorTransInGate = "Trans-In Gate",
                IdAlreadyExist = "the same ID already exists and cannot be converted.",

                Label = "Label",
                LocalProject = "Local Project",
                LogicalCondition = "Logical Condition",
                Language = "Language",
                LineColor = "Line Color",
                LineStyle = "Line Style    ",
                IfOnlyEvent = "Do you want to modify this event only",
                Move = "Movable",
                MessageBoxCaption = "SmarTree",
                Minute = "Minutes",
                SelectProject = "Please select the project to import",
                Normal = nameof(Normal),
                FailureProbability = "Failure Probability",
                ConstantProbability = "Constant Probability",
                NewProject = "New Project",
                NewSystem = "New Fault Tree",
                NewFolder = "New Group",
                RenameFolder = "Rename Group",
                DeleteFolder = "Delete Group",
                FaultTreeCopy = "Copy Fault Tree",
                FaultTreePaste = "Paste Fault Tree",
                PosParent = "ParentGate Pos",
                PosChild = "ChildGate Pos",
                PosLeft = "LeftGate Pos",
                PosRight = "RightGate Pos",
                New = "New",
                NextRepeatEvent = nameof(NextRepeatEvent),
                Null = "",
                NoProjectsopen = "No project opened.",
                NoparentPackage = "No parent package or no data for parent package.",
                NoStereoType = "No stereotype = topevent element found. please make sure the correct fault tree package is selected.",
                Nodatafound = "No data found",
                Next = "Next",
                Namecannotbeempty = "Name cannot be empty",
                NoAspectProject = "There is no currently Aspect program be opened",
                No = "No",

                Open = "Open",
                OK = "OK",
                OrGate = "Or Gate",
                ShowCut = "HighLight CutSets",
                ShowCutCheck = "HighLight",
                ShowCutName = "CutSets",
                ShowCutszProb = "Probability",
                ShowCutLevel = "MaxCutOrder",

                PasteAsTopNode = "Paste As Top Gate",
                Projectnamealreadyexists = "Project name already exists",
                Foldernamealreadyexists = "Group name already exists",
                Path = "Path:",
                Project = "Project",
                Exit = "Exit",
                PrintPreview = "Print Preview",
                PrintSelectedRecords = "Print Selected Records",
                PrintTable = "Print Table",
                PrintSettting = "Print Settting",
                PreviousRepeatEvent = nameof(PreviousRepeatEvent),
                PriorityAndGate = "Priority AND Gate",
                PageBreaks = "PageBreaks",
                PageScrollMode = "Page/Content Scroll Mode",
                Paste = "Paste",
                ProblemList = nameof(ProblemList),
                ExtraValue1 = "Votes",
                ExtraValue2 = nameof(ExtraValue2),
                ExtraValue3 = nameof(ExtraValue3),
                ExtraValue4 = nameof(ExtraValue4),
                ExtraValue5 = nameof(ExtraValue5),
                ExtraValue6 = nameof(ExtraValue6),
                ExtraValue7 = nameof(ExtraValue7),
                ExtraValue8 = nameof(ExtraValue8),
                ExtraValue9 = nameof(ExtraValue9),
                ExtraValue10 = nameof(ExtraValue10),

                IntegrityCheckString_HasNoRoot = "No root is created under the fault tree.",
                IntegrityCheckString_HasNoChild = "No child node is created under logic gate.",
                IntegrityCheckString_HasNoChildOrOnlyHouseEvent = "No child node is created under logic gate or only HouseEvent under logic gate",
                IntegrityCheckString_HasNoValue = " in the event datas is missing.",
                IntegrityCheckWarning_Unreasonable = "The data in the event is unreasonable.",
                IntegrityCheckWarning_UnreasonableVote = "The data in the Voting Gate is unreasonable.(0<X<=child node count)",
                IntegrityCheckWarning_Nodescription = "No description text.",
                IntegrityCheckWarning_Abnormalidentifier = "Abnormal identifier.",

                IntegrityCheckString_CalculateSuccess = "Check Passed",
                IntegrityCheckString_CalculateFail = "Calculate fail: Incomplete fault tree!",
                IntegrityCheckString_CalculateFailed = "FTA Calculate Failed,Reason:",
                IntegrityCheckString_CalculateSuccessSelect = "Calculate successfully! The Probability of the selected node is:",
                IntegrityCheckString_CalculateSuccessRoot = "Calculate successfully! The Probability of the root node is:",
                PrintingSelection_Text = "Printing Selection",
                PrintingSelection_Group = "Printing Items",
                PrintingSelection_Item1 = "FTA Report",
                PrintingSelection_Item3 = "FTA Diagram",
                PrintingSelection_Item2 = "Cutset Report",
                SwitchImport_Text = "Import",
                SwitchImport_Item1 = "Generate new fault tree",
                SwitchImport_Item2 = "Insert data under the selected node",
                SwitchExport_Text = "Export",
                SwitchExport_Check = "SelectAll",
                FindAndReplace_Text = "FindAndReplace",
                FindAndReplace_TabFind = "Find",
                FindAndReplace_TabReplace = "Replace",
                FindAndReplace_Next = "Next",
                FindAndReplace_ReplaceAll = "ReplaceAll",
                FindAndReplace_SearchContent = "SearchContent:",
                FindAndReplace_ReplaceContent = "ReplaceContent:",
                FindAndReplace_ColumnRange = "ColumnRange:",
                FindAndReplace_Matching = "Matching:",
                FindAndReplace_Inclusion = "Inclusion",
                FindAndReplace_WholeWord = "WholeWord",
                FindAndReplace_AboveDown = "AboveDown",
                FindAndReplace_DownUp = "DownUp",
                FindAndReplace_CaseSensitive = "CaseSensitive",
                ParentID = nameof(ParentID),
                False = nameof(False),
                ProjectName = "Project Name：",
                GroupName = "Group Name：",
                Groups = "Groups",
                PasteRepeatedEvent = "Paste Repeated Event",
                ProjectNavigator = "Project Navigator",
                Problems = "Problems",
                ProblemsForm = "Simfia Form",
                ProjectNameCannotBeEmpty = "Project name cannot be empty.",
                ProjectNameCannotOver = "Project name cannot exceed 100 characters.",
                SmarTreeNameCannotOver = "SmarTree name cannot exceed 100 characters.",
                AllNameCheck = "The name cannot contain : (，,。.？?！!：:;；*[\'\"@#$%/^&~\\])`=+-{}/|<>",
                Parametercannotbeempty = "Parameter cannot be empty",
                PositiveNumber = "Please enter a positive number.",
                Previous = "Back",
                Property = "Property",

                RemarksGate = "Remarks Gate",
                Ruler = "Ruler",

                Report = "Report",
                RenumberWizardCancelDialogTitle = "SmarTree",
                RenumberWizardCancelDialogContent = "Do you want to cancel the wizard or discard the changes？",
                RenumberWizardTitle = "Faulttree renumber wizard",
                RenumberWizardPage1Caption = "Select fault tree",
                RenumberWizardPage2Caption = "Select graph",
                RenumberWizardPage3Caption = "Select format",
                RenumberWizardPage4Caption = "Select format",
                RenumberWizardPage5Caption = "Confirm changes",
                RenumberWizardPage6Caption = "Complete wizard",
                RenumberWizardPage1Content = "Select root in the faulttree to update",
                RenumberWizardPage2Content = "Select only gate ,only event or both of them",
                RenumberWizardPage3Content = "Format the renumbered number",
                RenumberWizardPage4Content = "Format to renumbered objects",
                RenumberWizardPage5Content = "The changes cannot be undo,please confirm if continue.",
                RenumberWizardPage6Content = "Press 'Finish' to end the wizard ",
                RenumberWizardPage1Tip = "Which root do you want to renumber in the faulttree",
                RenumberWizardPage2Tip = "Which graph should be renumbered?",
                RenumberWizardPage3Tip = "How to renumber these Gates?",
                RenumberWizardPage4Tip = "How to renumber these Events?",
                RenumberWizardPage5Tip = "Whether it is still implemented",
                RenumberWizardPage6Tip = "Press 'Done' to apply changes，the changes cannot be undo.press 'Cancel' to return faulttree and giveup changes",
                RenumberingSucceeded = "Allready renumbered graph：{0} objects.",
                RenumberRadiogroup1Raido1Text = "All of roots in the faulttree",
                RenumberRadiogroup1Raido2Text = "Only selected root(transfers not included) in the faulttree",
                RenumberRadiogroup1Raido3Text = "Only selected root(transfers included) in the faulttree",
                OnlyGate = "Only renumber Gate",
                OnlyEvent = "Only renumber Event",
                BothGateAndEvent = "Renumber Gate and Event",
                RenumberFaultTree = "Renumber Fault Tree",
                QuickRenumber = "Quick Renumber",
                RemoveProject = "Remove Project",
                RemoveSystem = "Remove Fault Tree",
                RenameProject = "Rename Project",
                RenameSystem = "Rename Fault Tree",
                RepeatedEventColor = "Repeated Events",
                Refresh = "Refresh",
                Redo = "Redo",
                RenumberedType = "RenumberedType",

                SpellingCheck = "Spelling Check",
                SortAscending = "Sort Ascending",
                SortDescending = "Sort Descending",
                ShowFindPanel = "Show FindPanel",
                ShowAutoFilterRow = "Show AutoFilterRow",
                Systemnamecannotbeempty = "Fault Tree name cannot be empty.",
                Systemnamealreadyexists = "Fault Tree name already exists",
                SearchScopeCaption = "Search",
                SearchScopeTip = "Select columns for search",
                ShapeColor = "Graphic Background",
                ShapeRectWidth = "TextBox Width",
                ShapeRectHeight = "TextBox Height",
                SymbolRectSize = "Symbol Size",
                System = "Fault Tree",
                SystemName = "FaultTree Name：",
                Save = "Save",
                SaveAs = "SaveAs",
                AddE = "Add",
                SaveAll = "SaveAll",
                OpenProject = "Open Project",
                OpenFaultTree = "Open Fault Tree",
                RecentProjects = "Recent Projects",
                RecentFiles = "Recent Files",
                OpenRecentFiles = "Open Recent Files",
                SetPassword = "Set Password",
                Help = "Help",
                Soft = "Soft",
                UpdateTip = "This is the latest version：",
                Version = "Version：",
                SplashInfo = "The software provides convenient and practical faulttree creation,editing and calculation fuctions,supports fast batch processing of multiple fault tree files.For more infomation about software features,refer to the smartree software user`s manual in help.Finally,we sincerely welcome you to user smartree again.",

                UpdateDate = "Release Date：",
                UpdateMessage = "For more information, visit:",
                UserManual = "User Manual",
                About_Information = "Information",
                About_Settings = "Data Catalog",
                About_DefaultFilePath = "Default File Path：",
                About_CommonEventLibraryPath = "Common Library Path：",
                Examples = "Example",
                UserManualTip = "User Manual No Exist",
                ShowKeymap = "Show Keymap",
                ShowKeymapName = "KeyName",
                ShowKeymapKey = "KeyValue",
                CheckforUpdates = "Check for Updates",
                AboutSmarTree = "About SmarTree",
                Documentation = "Documentation",
                ExistProject = "The same Project name already exists",
                ExistFaultTree = "The same FaultTree name already exists",
                Skin = "Interface Skins",
                FoldProject = "Fold Project",
                ExpandProject = "Expand Project",
                Properties = "Properties",
                CloseProperties = "Hide Properties",
                Projects = "Projects",
                Modeling = "Modeling",
                RibbonFile = "File",
                RemovedTransferMessage = "Here are transfers removed",
                SelectedShapeColor = "Selected Graph",

                Scale = "Scaleable",
                Successfully_Saved = "Successfully Saved!",
                NoSaved = "No FaultTree Opened!",
                NoSavedAll = "No Project Opened!",
                SavingFaultTree = "Saving All",
                SavingAll = "Saving Current FaultTree",

                Thereisatleastonenodeintheproject = "There is at least one node in the project",
                Type = nameof(Type),
                True = nameof(True),
                TransferInGate = "Transfer-In Gate",
                TrueGateColor = "True Gate/Event",

                ImportExportGtoup = "Import/Export",
                TransferTo = "Transfer To",
                ToolBox = "ToolBox",
                ToplevelnodeID = "Top level node ID",

                UnfreezeColumns = "Unfreeze Columns",
                Underline = "Underline",
                Undo = "Undo",
                UndevelopedEvent = "Undeveloped Event",
                Units = nameof(Units),

                VotingGate = "Voting Gate",
                View = "View",

                Whethertocontinue = "Whether to continue？",
                StartPageWelcome = "Welcome to SmarTree",
                WizardCancel = "Cancel",
                Window = "Window",
                WindowLayout = "Appearance",

                ShowFullScreen = "Show Full Screen",
                ExitFullScreen = "Exit Full Screen",
                TipDescription_FullScreen = "Show full screen and press ESC to exit",
                TipDescription_InsertLevel = "Insert a new level",
                TipDescription_TopGatePos = "Quickly locate the top event",
                TipDescription_ChangeGateType = "Change the type of the selected gate",
                TipDescription_Undo = "Undo(Ctrl+Z)",
                TipDescription_Redo = "Redo(Ctrl+R)",
                TipDescription_Cut = "Cut(Ctrl+X)",
                TipDescription_Copy = "Copy(Ctrl+C)",
                TipDescription_Paste = "Paste(Ctrl+V)",
                TipDescription_Check = "Check the current fault tree",
                TipDescription_Calculate = "Calculate the probability of an event",
                TipDescription_Import = "Import fault tree data",
                TipDescription_Export = "Export fault tree data",
                TipDescription_FTAReport = "Generate FTA report",
                TipDescription_CutsetReport = "Generate  cutset report",
                TipDescription_SaveCommonEventLibrary = "Save the selected branch to the common library",
                TipDescription_CommonEventLibrary = "Open the built common Library",
                TipDescription_StartPage = "Back to the start page",
                TipDescription_ZoomIn = "Zoom In(Ctrl+MouseWheelUP)",
                TipDescription_ZoomOut = "Zoom Out(Ctrl+MouseWheelDown)",
                TipDescription_Windows = "Set the window view",
                TipDescription_FoldProject = "Fold all project",
                TipDescription_ExpandProject = "Expand all project",
                TipDescription_Properties = "Enter project property settings",
                TipDescription_UserManual = "Open user manual",
                TipDescription_Keymap = "Show keymap",
                TipDescription_Updates = "Check for Updates",
                TipDescription_AboutSmarTree = "Display SmarTree information",

                HideShowToolbar = "Hide/Show Toolbar",
                HideShowProjectNavigator = "Hide/Show Project Navigator",

                NewFTAItem_Text = "InsertFTAItem",
                NewFTAItem_Label = "ItemType：",

                XORGate = "XOR Gate",

                Yes = "Yes",

                ZoomIn = "Zoom In",
                ZoomOut = "Zoom Out",

            };
        }

        /// <summary>
        /// 获取中文版的字符串对象
        /// </summary>
        /// <returns></returns>
        public static StringModel GetCNFTAString()
        {
            return new StringModel
            {
                AlignLeft = "居左",
                AlignCenter = "居中",
                AlignRight = "居右",
                ArrowStyle = "箭头样式",
                ArrowSize = "箭头大小",
                AndGate = "与门",

                SplashScreenCommand_Initial1 = "正在初始化配置",
                SplashScreenCommand_Initial2 = "正在初始化控件",
                SplashScreenCommand_Initial3 = "正在初始化开始页",

                BasicShape = "基本图形",
                BasicEvent = "基本事件",
                LinkGate = "链接门",
                Bold = "加粗",
                BreakintoTransfer = "进入转移门",
                BestFitCurrentColumn = "自动适应大小(当前列)",
                BestFitallcolumns = "自动适应大小(所有列)",

                CancelCopyOrCut = "取消复制或剪切",
                Comment1 = "描述",
                Cancel = "取消",
                FixedValue = "固定值",
                CurrentAspectProject = "当前的ASPECT工程",
                ConditionEvent = "条件事件",
                Cut = "剪切",
                Copy = "复制",
                CopyCurrentView = "复制当前视图截图到剪贴板",
                CopyCurrentSelected = "复制当前选中项截图到剪贴板",
                CreateExcelSpreadSheet = "导出部分",
                CanvasFillMode = "全画布模式      ",
                CollapseTransfer = "收起转入门",
                Calculation = "计算",
                col_Pic = "图标",
                col_Name = "名称",
                col_Path = "详情",

                TextAbout = "    欢迎使用SmarTree，这是一个用于系统故障树分析的工程软件，适用于科学研究、项目研制等用途。软件提供了便捷实用的故障树创建、编辑、计算功能，支持快速批量处理多个故障树文件，能够进行多种格式的故障树数据导入、导出以及故障树分析报告生成，具有丰富的故障树编辑选项，以满足用户在软件使用中的需求。如需了解更多软件功能，可参阅帮助中的SmarTree软件用户手册。最后再次诚挚的欢迎您使用SmarTree，如有任何疑问，请及时与我们联系。",

                TextAboutForm = "    SmarTree是一款用于故障树绘制和分析的工程软件，其提供了故障树绘制常用的逻辑符号库，功能涵盖了故障树分析中的常用分析活动，具备故障树创建、编辑、计算、割集分析等基本功能，建模支持与门、或门、表决门、转移门、基本事件、未发展事件等多种逻辑门及逻辑事件，同时提供故障树图形的自定义功能、故障树结构及数据检查、整体故障树或子树的复制粘贴、中间事件的独立子树分析、多个故障树文件数据的导入/出、批量故障树分析报告一键生成、故障树重编号、顶事件的快速定位以及公共数据库的创建和使用等快捷操作功能，能够帮助工程人员快速开展故障树建模和分析，并生成相应的故障树分析报告。SmarTree软件版权属上海航空电子有限责任公司所有，其产权受国家法律绝对保护，未经本公司授权，其他公司、单位、代理商及个人不得非法使用和拷贝。",

                Canvas = "属性",
                ColumnChooser = "标题列选入选出",
                ClearSorting = "清除排序",
                ClearAllSorting = "清除所有排序",
                CutNodes = "剪切(包括子节点)",
                CopyNode = "复制(单节点)",
                CopyNodes = "复制(包括子节点)",
                CollapseNodes = "收缩所有节点",
                AtLeastSelectOneNode = "请选择一个节点",
                MastCal = "请先选择一个节点并进行计算",
                ConfirmDeletionMessage = "是否确定删除当前节点？ ",
                ExampleNoDo = "例子工程不能操作",
                ConfirmExitMessage = "是否退出SmraTree？",
                ConfirmSavingMessage = "是否保存所有更改？",
                ConvertToCopy = "确定要把所有该转移门转为另一个转移门的副本?",
                ConvertToAnotherCopy = "确定要转为另一个转移门的副本?",
                ConvertToRepeatedEvents = "确定要将该转移门全部转换为重复事件?",
                ConvertToAnotherRepeatedEvents = "确定要转为另一个重复事件?",
                CutSetColor = "割集标记",
                CanNotExportPartEvent = "不支持单个基本事件的导出功能",

                Delete = "删除",
                ExportToModel = "保存到公共库",
                LoadFromModel = "公共库",

                BasicLib = "基本事件库",
                AddToBasicEventLibrary = "添加到基本事件库",
                SynchronizeFromBasicEventLibrary = "从基本事件库同步数据",
                InsertFromBasicEventLibrary = "从基本事件库插入",

                DeleteTop = "删除所有子节点",
                DeleteLevel = "删除分级",
                DeleteNode = "删除(仅当前节点)",
                DeleteNodes = "删除(包括所有子节点)",
                DeleteNodes_HasTransfer = "删除(包括转移门和其他子节点)",
                DeleteNodesMessage = "是否删除选中门事件以及门事件以下的分支节点?",
                DeleteNodesAndTransferMessageOnlyTop = "是否删除选中门事件的分支节点? ",
                DeleteNodesAndTransfer = "删除(包括所有子节点)",

                Dataisbeingquerieddonotrepeatthequery = "数据正在查询中，不要重复查询",
                DormancyFactor = "休眠因子",
                DeleteSystem = "删除故障树",
                DeleteProject = "删除工程",
                DockMenuFolat = "浮动",
                DockMenuDock = "停靠",
                DockMenuTabbed = "作为标签页停靠",
                DockMenuHide = "自动隐藏",
                DockMenuClose = "关闭",

                Check = "检查",
                Check_Type = "类型",
                Check_Name = "名称",
                Check_Info = "信息",
                EventModify = "基本事件库",
                ExposureTime = "暴露时间",
                Votes = "表决",
                ExposureTimePercentage = "曝光比率",
                EditPropertyViewTitle = "编辑属性",
                Export = "导出",
                Edit = "编辑",
                Exceptionloadingfaulttreedata = "加载故障树数据时异常：",
                Erroroccurred = "发生错误",
                ExpandNodes = "扩展开所有节点",
                ExportExcelOK = "成功导出Excel文件",
                ExportFilteredExcelOK = "成功导出已筛选的Excel",
                ExportNoDataError = "没有可导出的数据",
                ExportExcelSucceeded = "是否浏览导出的信息",
                ExportExcelSucceededPath = "是否浏览导出的目录",
                ExportExcelQD = "是否导出当前故障树下全部内容?",
                EventModifyDescription = "输入修改后的事件名",
                EventModifyMessage = "请输入至少一个事件",
                EventName = "事件名",
                EmptyError = "不能为空",
                EventPrefix = "事件前缀",
                EventStartNumber = "事件起始号",
                EventMinNumber = "事件最小位",
                EventSuffix = "事件后缀",

                FailureRate = "失效率",
                FalseGateColor = "假门/事件",
                FilterEditor = "过滤编辑器",
                Failedtoquerydata = "查询数据失败",
                FRType = "失效类型",
                FRPercentage = "失效百分比",
                False = "假",
                FTADiagram = "FTA图",
                OpenDir = "打开目录",
                CloseDir = "关闭工程",
                FindAndReplace = "查找",
                FindAndReplaceCustom = "查找和替换",
                Filter = "过滤器",
                FontGroup = "字体和缩放设置",
                FontName = "字体名称",
                FontSize = "字体大小",
                FTAReport = "FTA报告",
                FTACalculate = "FTA计算",
                FTACalculateSelected = "计算当前选中分支",
                CusetStr = "最大割集设置",
                Parameters = "参数",
                InsertLevel = "插入分级",
                ChangeGateType = "更改门类型",
                TopGatePos = "顶事件位置",
                GateSettings = "门设置",
                Color = "颜色",
                FTATable = "FTA表",
                Filtervisibledatamodedefault = "过滤可见数据模式(默认)",
                Filteralldatamode = "过滤所有数据模式",
                FreezeColumn = "冻结列",
                Finish = "完成",
                FTATableEditor = "表指示器",
                FTAGroup = "检查和编号",

                Grid = "网格  ",
                GatePrefix = "门前缀",
                GateStartNumber = "门起始号",
                GateMinNumber = "门最小位",
                GateSuffix = "门后缀",
                GateLabel5 = "示例:",
                GateLabel6 = "完成时保存标识符数据",
                AccordingToGate = "汇入门编号",
                GraphicTools = "图形工具集合",
                Graph = "图表",
                GlobalChange = "全局更改",

                HouseEvent = "屋事件",
                Hour = "小时",
                HideFindPanel = "隐藏查找栏",
                HideAutoFilterRow = "隐藏过滤栏",
                Hidefiltereditor = "清除过滤编辑器",
                HighlightCutSets = "亮化最小割集",
                StartPage = "开始页",

                Identifier = "编号",
                InputType = "输入类型",
                InputValue = "失效率",
                InputValue_Constant = "概率",
                InputValue_Failed = "概率必须大于等于0小于等于1",
                InputValue_Failed1 = "概率必须大于0小于1",
                InputValue_Failed2 = "非法数据",
                InputValue2 = "暴露时间",
                //InhibitGate = "禁止门",
                Italic = "斜体",
                Import = "导入",
                ImportXML = "导入XML",
                IdRectHeight = "标识框高度",
                Insert = "插入",
                InsertRepeatedEvent = "插入重复事件",
                AddLinkGate = "插入链接门",
                UpDateLinkGate = " 更新链接门数据",
                IsolatednodesfoundwithID = "发现有孤立的节点，他们的ID是：",
                InsertNode = "插入输入",
                InsertLink = "插入链接门",
                InsertTopNode = "插入新的顶级门",
                ImportExcelOK = "成功导入Excel文件",
                ImportExcelNoProgramError = "请确保至少存在一个工程",
                ImportExcelFailed = "导入Excel文件失败，请检查数据源是否正确。",
                ImportExcelIdFailed = "导入的文件缺少 \"Id\" 字段。",
                ImportExcelTypeFailed = "导入的文件缺少 \"Type\" 字段。",
                ImportExcelParentIdFailed = "导入的文件缺少 \"ParentId\" 字段。",
                Indicator = "指示器",
                IndicatorTopGate = "顶级门",
                IndicatorTransInGate = "转移门",
                IdAlreadyExist = "已存在相同ID的对象，且无法转换。",

                Language = "语言",
                Label = "标签",
                LocalProject = "本地工程",
                LogicalCondition = "逻辑条件",
                LineColor = "线条颜色",
                LineStyle = "连线样式",

                MessageBoxCaption = "提示",
                Move = "移动",
                Minute = "分钟",
                SelectProject = "请选择要导入的项目节点",
                IfOnlyEvent = "是否仅修改本事件",

                NoAspectProject = "当前没有打开的ASPECT程序",
                NoProjectsopen = "未打开任何工程",
                Normal = "常规",
                FailureProbability = "失效概率",
                ConstantProbability = "恒定概率",
                No = "否",
                Next = "下一步",
                NewProject = "新建工程",
                NewSystem = "新建故障树",
                NewFolder = "新建分组",
                RenameFolder = "重命名分组",
                DeleteFolder = "删除分组",
                FaultTreeCopy = "复制故障树",
                FaultTreePaste = "粘贴故障树",
                PosParent = "父节点位置",
                PosChild = "子节点位置",
                PosLeft = "左侧定位",
                PosRight = "右侧定位",
                New = "新建",
                NextRepeatEvent = "下一个重复事件",
                Null = "",
                NoparentPackage = "没有父包或父包没有数据",
                NoStereoType = "未找到任何Stereotype = TopEvent元素请确保选择的是正确的故障树包",
                Nodatafound = "查询不到任何数据",
                Namecannotbeempty = "名称不能为空",

                OK = "确定",
                Open = "打开",
                OrGate = "或门",
                ShowCut = "亮化最小割集",
                ShowCutCheck = "高亮",
                ShowCutName = "割级",
                ShowCutszProb = "概率",
                ShowCutLevel = "最大割级阶数",

                Projectnamealreadyexists = "工程名已存在。",
                Foldernamealreadyexists = "分组名已存在",
                ProblemList = "问题集合",
                ExtraValue1 = "表决",
                ExtraValue2 = "扩展值2",
                ExtraValue3 = "扩展值3",
                ExtraValue4 = "扩展值4",
                ExtraValue5 = "扩展值5",
                ExtraValue6 = "扩展值6",
                ExtraValue7 = "扩展值7",
                ExtraValue8 = "扩展值8",
                ExtraValue9 = "扩展值9",
                ExtraValue10 = "扩展值10",
                IntegrityCheckString_HasNoRoot = "没有根节点。",
                IntegrityCheckString_HasNoChild = "未创建任何子节点。",
                IntegrityCheckString_HasNoChildOrOnlyHouseEvent = "未创建任何子节点或者子节点只有屋事件。",
                IntegrityCheckString_HasNoValue = "在事件数据中缺失。",
                IntegrityCheckWarning_Unreasonable = "事件中的数据不合理(λt>1)。",
                IntegrityCheckWarning_UnreasonableVote = "表决门的表决值不合理(必须大于0小于等于子节点数量)。",
                IntegrityCheckWarning_Nodescription = "无描述文本。",
                IntegrityCheckWarning_Abnormalidentifier = "异常标识符。",

                IntegrityCheckString_CalculateSuccess = "检查完成,未发现问题!",
                IntegrityCheckString_CalculateFail = "计算失败: 故障树不完整!",
                IntegrityCheckString_CalculateFailed = "FTA计算失败,错误原因:",
                IntegrityCheckString_CalculateSuccessSelect = "计算成功!\r\n当前选中的项故障概率是: ",
                IntegrityCheckString_CalculateSuccessRoot = "计算成功!\r\n当前故障树的故障概率是: ",
                PrintingSelection_Text = "报告类型选择",
                PrintingSelection_Group = "类型",
                PrintingSelection_Item1 = "FTA报告",
                PrintingSelection_Item3 = "FTA故障树",
                PrintingSelection_Item2 = "割级报告",
                SwitchImport_Text = "导入选项",
                SwitchImport_Item1 = "导入数据到一个新故障树",
                SwitchImport_Item2 = "导入数据到当前选中的节点",
                SwitchExport_Text = "导出选项",
                SwitchExport_Check = "全选",
                FindAndReplace_Text = "查找和替换",
                FindAndReplace_TabFind = "查找",
                FindAndReplace_TabReplace = "替换",
                FindAndReplace_Next = "下一个",
                FindAndReplace_ReplaceAll = "全部替换",
                FindAndReplace_SearchContent = "查找内容：",
                FindAndReplace_ReplaceContent = "替换内容：",
                FindAndReplace_ColumnRange = "匹配的列：",
                FindAndReplace_Matching = "匹配条件：",
                FindAndReplace_Inclusion = "包含",
                FindAndReplace_WholeWord = "全字匹配",
                FindAndReplace_AboveDown = "向下",
                FindAndReplace_DownUp = "向上",
                FindAndReplace_CaseSensitive = "区分大小写",
                ParentID = "父编号",
                ProjectName = "工程名称：",
                GroupName = "文件夹名称：",
                Groups = "分组",
                Path = "路径:",
                Project = "工程",
                Exit = "退出",
                PrintPreview = "打印预览",
                PrintSelectedRecords = "打印所选记录",
                PrintTable = "打印表",
                PrintSettting = "打印设置",
                PreviousRepeatEvent = "上一个重复事件",
                PriorityAndGate = "优先与门",
                Paste = "粘贴",
                PageBreaks = "分页线",
                PageScrollMode = "页/内容滚动模式",
                PasteRepeatedEvent = "粘贴重复事件",
                ProjectNavigator = "工程导航器",
                Problems = "问题",
                ProblemsForm = "Simfia",
                ProjectNameCannotBeEmpty = "工程名不能为空",
                ProjectNameCannotOver = "工程名长度不能超过100",
                SmarTreeNameCannotOver = "故障树名称长度不能超过100",
                AllNameCheck = "名称不能包含 : (，,。.？?！!：:;；*['\"@#$%/^&~\\])`=+-{}/|<>",
                Parametercannotbeempty = "参数不能为空",
                PasteAsTopNode = "粘贴为顶级门",
                Previous = "上一步",
                PositiveNumber = "请输入正数",
                Property = "属性",

                RenumberWizardTitle = "故障树重新编号向导",
                RenumberWizardPage1Caption = "选择故障树",
                RenumberWizardPage2Caption = "选择图",
                RenumberWizardPage3Caption = "选择格式",
                RenumberWizardPage4Caption = "选择格式",
                RenumberWizardPage5Caption = "确认更改",
                RenumberWizardPage6Caption = "向导完成",
                RenumberWizardPage1Content = "选择要更新故障树中的哪些根节点",
                RenumberWizardPage2Content = "选取仅对门、仅对事件或两者重新编号",
                RenumberWizardPage3Content = "设置要对重新编号的图使用的格式",
                RenumberWizardPage4Content = "设置要对重新编号的图使用的格式",
                RenumberWizardPage5Content = "将要进行的更改不能被撤销。请确认是否要永久更改文件中的数据。",
                RenumberWizardPage6Content = "单击“完成”以结束向导",
                RenumberWizardPage1Tip = "你想要对故障树中的哪些根节点重新编号？",
                RenumberWizardPage2Tip = "要对哪些图重新编号？",
                RenumberWizardPage3Tip = "要如何对这些门重编号？",
                RenumberWizardPage4Tip = "要如何对这些事件重编号？",
                RenumberWizardPage5Tip = "无法撤销这些更改，确实要继续？",
                RenumberWizardPage6Tip = "按“完成”应用更改，这些更改不能被撤销，按“取消”返回故障树而不做任何更改。",
                RenumberRadiogroup1Raido1Text = "对故障树中所有顶节点编号",
                RenumberRadiogroup1Raido2Text = "仅对所选节点的顶节点（不包含转移门）编号",
                RenumberRadiogroup1Raido3Text = "仅对所选节点的顶节点（包含转移门）编号",
                OnlyGate = "仅对门重编号",
                OnlyEvent = "仅对事件重编号",
                BothGateAndEvent = "对门和事件重编号",
                RenumberingSucceeded = "已成功重编号：{0}个图形。",
                RemarksGate = "描述框",
                Redo = "重做",
                Ruler = "标尺  ",
                RenumberFaultTree = "重新编号",
                QuickRenumber = "快速编号",
                ShapeRectHeight = "描述框高度",
                RepeatedEventColor = "重复事件",
                RemoveProject = "移除工程",
                RemoveSystem = "移除故障树",
                RenameProject = "重命名工程",
                RenameSystem = "重命名故障树",
                Report = "报告",
                Refresh = "刷新",
                RenumberWizardCancelDialogTitle = "提示",
                RenumberWizardCancelDialogContent = "确实要取消向导并放弃更改？",
                RenumberedType = "重编号类型",

                ShowFindPanel = "显示查找栏",
                ShowAutoFilterRow = "显示过滤栏",
                SystemName = "故障树名称：",
                System = "故障树",
                Save = "保存",
                SaveAll = "保存所有",
                OpenProject = "打开本地工程",
                OpenFaultTree = "打开本地故障树",
                SaveAs = "另存为",
                AddE = "新增",
                RecentProjects = "最近打开的工程",
                RecentFiles = "最近打开的故障树",
                OpenRecentFiles = "打开最近文件",
                SetPassword = "设置密码",
                SpellingCheck = "拼写检查",
                ShapeColor = "图形背景",
                ShapeRectWidth = "描述框宽度",
                SymbolRectSize = "符号尺寸",
                SelectedShapeColor = "选中图形",
                Modeling = "绘制",
                RibbonFile = "文件",
                Help = "帮助",
                Soft = "软件",
                UpdateTip = "已是最新版本：",
                Version = "版本：",
                SplashInfo = "该软件提供了方便实用的故障树创建、编辑和计算功能，支持多个故障树文件的快速批处理。有关软件功能的更多信息，请参阅帮助中的SmarTree软件用户手册。最后，我们真诚欢迎您再次使用SmarTree。",
                UpdateDate = "发布日期：",
                UpdateMessage = "要了解更多信息，请访问：",
                UserManual = "用户手册",
                UserManualTip = "用户手册不存在",
                About_Information = "关于信息",
                About_Settings = "数据目录",
                About_DefaultFilePath = "默认保存路径：",
                About_CommonEventLibraryPath = "公共库路径：",
                Examples = "示例",
                ShowKeymap = "工具快捷键 ",
                ShowKeymapName = "快捷键名称",
                ShowKeymapKey = "快捷键值",
                CheckforUpdates = "检查更新",
                AboutSmarTree = "关于SmarTree",
                Documentation = "文档",
                ExistProject = "已存在同名的工程",
                ExistFaultTree = "已存在同名的故障树",
                Skin = "背景",
                Projects = "工程",
                FoldProject = "折叠项目",
                ExpandProject = "展开项目",
                Properties = "工程属性",
                CloseProperties = "隐藏属性编辑",
                Systemnamecannotbeempty = "故障树名不能为空",
                SortAscending = "升序",
                SortDescending = "降序",
                Systemnamealreadyexists = "故障树名已存在。",
                SearchScopeCaption = "查询",
                SearchScopeTip = "选择要查询的列",
                Scale = "缩放",
                Successfully_Saved = "保存成功!",
                NoSaved = "当前未打开任何故障树!",
                NoSavedAll = "当前未打开任何工程!",
                SavingFaultTree = "保存所有",
                SavingAll = "保存当前故障树",
                RemovedTransferMessage = "删除的转移门有",

                Thereisatleastonenodeintheproject = "工程里至少有一个节点",
                ToplevelnodeID = "顶层节点ID",
                Type = "类型",
                True = "真",
                TransferInGate = "转入门",
                TrueGateColor = "真门/事件",
                TransferTo = "转到",
                ImportExportGtoup = "导入/导出",
                ToolBox = "工具箱",

                Units = "单位",
                UndevelopedEvent = "未发展事件",
                Underline = "下划线",
                Undo = "撤销",
                UnfreezeColumns = "取消冻结列",

                View = "视图",
                VotingGate = "表决门",

                Whethertocontinue = "是否继续？",
                StartPageWelcome = "欢迎使用SmarTree",
                WizardCancel = "取消",
                Window = "窗口",
                WindowLayout = "显示",

                ShowFullScreen = "显示全屏",
                ExitFullScreen = "退出全屏显示",
                TipDescription_FullScreen = "进入全屏显示，请按ESC退出",
                TipDescription_InsertLevel = "插入一个新的分级",
                TipDescription_TopGatePos = "快速定位到顶事件",
                TipDescription_ChangeGateType = "更改逻辑门类型",
                TipDescription_Undo = "撤销(Ctrl+Z)",
                TipDescription_Redo = "恢复(Ctrl+R)",
                TipDescription_Cut = "剪切(Ctrl+X)",
                TipDescription_Copy = "复制(Ctrl+C)",
                TipDescription_Paste = "粘贴(Ctrl+V)",
                TipDescription_Check = "检查当前故障树",
                TipDescription_Calculate = "计算事件发生的概率",
                TipDescription_Import = "导入故障树数据",
                TipDescription_Export = "导出故障树数据",
                TipDescription_FTAReport = "生成FTA报告",
                TipDescription_CutsetReport = "生成割集报告",
                TipDescription_SaveCommonEventLibrary = "将选中分支添加到公共库",
                TipDescription_CommonEventLibrary = "打开构建的公共库",
                TipDescription_StartPage = "启动起始页界面",
                TipDescription_ZoomIn = "放大(Ctrl+MouseWheelUP)",
                TipDescription_ZoomOut = "缩小(Ctrl+MouseWheelDown)",
                TipDescription_Windows = "设置窗口视图",
                TipDescription_FoldProject = "折叠所有项目",
                TipDescription_ExpandProject = "展开所有项目",
                TipDescription_Properties = "进入工程属性设置",
                TipDescription_UserManual = "打开用户手册",
                TipDescription_Keymap = "显示工具快捷键",
                TipDescription_Updates = "检查更新",
                TipDescription_AboutSmarTree = "显示SmarTree信息",


                HideShowToolbar = "隐藏/显示工具栏",
                HideShowProjectNavigator = "隐藏/显示工程导航窗口",

                NewFTAItem_Text = "插入输入",
                NewFTAItem_Label = "插入类型：",

                XORGate = "异或门",
                Yes = "是",

                ZoomIn = "放大",
                ZoomOut = "缩小",
            };
        }
    }
}
