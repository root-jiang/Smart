using DevExpress.XtraSplashScreen;
using FaultTreeAnalysis.Common;
using FaultTreeAnalysis.Model.Data;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace FaultTreeAnalysis
{
    /// <summary>
    /// 自定义的启动画面
    /// </summary>
    public partial class SplashScreen_Start : SplashScreen
    {
        /// <summary>
        /// 屏幕开始画面功能
        /// </summary>
        public SplashScreen_Start()
        {
            InitializeComponent();

            labelControl_SplashTitle.Parent = pictureEdit2;
            labelControl_SplashVer.Parent = pictureEdit2;
            labelControl_SplashCopyRight.Parent = pictureEdit2;
            labelControl_SplashStart.Parent = pictureEdit2;
            labelControl_SplashInfo.Parent = pictureEdit2;
            pictureEdit1.Parent = pictureEdit2;

            string savingDataPath = $"{AppDomain.CurrentDomain.BaseDirectory}{FaultTreeAnalysis.Properties.Resources.SavingDataPath}";

            try
            {
                FileInfo file = new FileInfo(savingDataPath);
                if (!file.Directory.Exists) file.Directory.Create();

                if (File.Exists(savingDataPath) == false)
                {
                    ProgramModel PM = new ProgramModel();
                    File.WriteAllText(savingDataPath, Newtonsoft.Json.JsonConvert.SerializeObject(PM), Encoding.UTF8);
                }
                string Datas = File.ReadAllText(savingDataPath, Encoding.UTF8);

                if (General.FtaProgram == null)
                {
                    General.FtaProgram = Newtonsoft.Json.JsonConvert.DeserializeObject<ProgramModel>(Datas);
                }

                if (General.FtaProgram.Setting.Language == FixedString.LANGUAGE_EN_EN)
                {
                    General.FtaProgram.String = StringModel.GetENFTAString();
                }
                else if (General.FtaProgram.Setting.Language == FixedString.LANGUAGE_CN_CN)
                {
                    General.FtaProgram.String = StringModel.GetCNFTAString();
                }
            }
            catch (Exception)
            {
            }

            //主版本.次版本.内部版本.修订版本
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            string ver = string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision.ToString("0000"));
            labelControl_SplashVer.Text = General.FtaProgram.String.Version + ver;

            Assembly asm = Assembly.GetExecutingAssembly();//如果是当前程序集
            AssemblyCopyrightAttribute asmcpr = (AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(asm, typeof(AssemblyCopyrightAttribute));

            if (General.FtaProgram.Setting.Language == FixedString.LANGUAGE_EN_EN)
            {
                if (asmcpr.Copyright.Contains("|"))
                {
                    labelControl_SplashCopyRight.Text = asmcpr.Copyright.Split(new char[] { '|' })[1];
                }
                else
                {
                    labelControl_SplashCopyRight.Text = asmcpr.Copyright;
                }
            }
            else if (General.FtaProgram.Setting.Language == FixedString.LANGUAGE_CN_CN)
            {
                if (asmcpr.Copyright.Contains("|"))
                {
                    labelControl_SplashCopyRight.Text = asmcpr.Copyright.Split(new char[] { '|' })[0];
                }
                else
                {
                    labelControl_SplashCopyRight.Text = asmcpr.Copyright;
                }
            }

            //labelControl_SplashInfo.Text = General.FtaProgram.String.SplashInfo;
            labelControl_SplashInfo.Text = "The software provides convenient and practical faulttree creation,editing and calculation fuctions,supports fast batch processing of multiple fault tree files.For more infomation about software features,refer to the smartree software user`s manual in help.Finally,we sincerely welcome you to user smartree again.";
        }

        #region Overrides

        /// <summary>
        /// 根据命令类型，处理命令
        /// </summary>
        /// <param name="cmd">命令类型，SplashScreenCommand枚举</param>
        /// <param name="arg">命令绑定的额外数据</param>
        public override void ProcessCommand(Enum cmd, object arg)
        {
            if (cmd.Equals(SplashScreenCommand.CHANGEINFO) && arg != null && arg.GetType() == typeof(string))
            {
                labelControl_SplashStart.Text = arg as string + "... ...";
            }
            base.ProcessCommand(cmd, arg);
        }

        #endregion

        /// <summary>
        /// 自定义命令类型
        /// </summary>
        public enum SplashScreenCommand
        {
            CHANGEINFO
        }
    }
}