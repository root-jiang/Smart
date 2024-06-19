using System;
using System.Windows.Forms;
using FaultTreeAnalysis.Common;
using System.Reflection;

namespace FaultTreeAnalysis.View.Ribbon.Start
{
    public partial class Frm_About : DevExpress.XtraEditors.XtraForm
    {
        public Frm_About()
        {
            InitializeComponent();
        }

        private void Frm_About_Load(object sender, EventArgs e)
        {
            xtraTabControl1.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False;
            xtraTabControl1.SelectedTabPageIndex = 0;
            //navBarGroup1.Caption = General.FtaProgram.String.AboutSmarTree;
            this.Text = General.FtaProgram.String.AboutSmarTree;
            memoEdit__About.Text = General.FtaProgram.String.TextAboutForm;
            labelControl3.Text = General.FtaProgram.String.UpdateDate;
            labelControl4.Text = General.FtaProgram.String.UpdateMessage;
            hyperlink_UserManual.Text = General.FtaProgram.String.UserManual;

            simpleButton3.Text = General.FtaProgram.String.About_Information;
            simpleButton4.Text = General.FtaProgram.String.About_Settings;
            labelControl1.Text = General.FtaProgram.String.About_DefaultFilePath;
            labelControl2.Text = General.FtaProgram.String.About_CommonEventLibraryPath;

            //主版本.次版本.内部版本.修订版本
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            string ver = string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision.ToString("0000"));
            labelControl_Ver.Text = General.FtaProgram.String.Version + ver;

            Assembly asm = Assembly.GetExecutingAssembly();//如果是当前程序集
            AssemblyCopyrightAttribute asmcpr = (AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(asm, typeof(AssemblyCopyrightAttribute));

            if (General.FtaProgram.Setting.Language == FixedString.LANGUAGE_EN_EN)
            {
                if (asmcpr.Copyright.Contains("|"))
                {
                    labelControl6.Text = asmcpr.Copyright.Split(new char[] { '|' })[1];
                }
                else
                {
                    labelControl6.Text = asmcpr.Copyright;
                }
            }
            else if (General.FtaProgram.Setting.Language == FixedString.LANGUAGE_CN_CN)
            {
                if (asmcpr.Copyright.Contains("|"))
                {
                    labelControl6.Text = asmcpr.Copyright.Split(new char[] { '|' })[0];
                }
                else
                {
                    labelControl6.Text = asmcpr.Copyright;
                }
            } 

            textEdit1.Text = General.FtaProgram.Setting.DefaultFilePath;
            textEdit2.Text = General.FtaProgram.Setting.CommonEventLibraryPath; 
        } 

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult rel = dialog.ShowDialog();
            if (rel == DialogResult.OK)
            {
                textEdit1.Text = dialog.SelectedPath;
                General.FtaProgram.Setting.DefaultFilePath = textEdit1.Text;
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult rel = dialog.ShowDialog();
            if (rel == DialogResult.OK)
            {
                textEdit2.Text = dialog.SelectedPath;
                General.FtaProgram.Setting.CommonEventLibraryPath = textEdit2.Text;
            }
        }

        private void hyperlink_UserManual_Click(object sender, EventArgs e)
        {
            Frm_UserManual f = new View.Ribbon.Start.Frm_UserManual();
            f.Show();
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            xtraTabControl1.SelectedTabPageIndex = 0;
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            xtraTabControl1.SelectedTabPageIndex = 1;
        }
    }
}