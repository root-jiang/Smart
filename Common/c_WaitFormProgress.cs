using DevExpress.XtraSplashScreen;
using IntegratedSystem.View;
using System;
using System.Threading;

namespace IntegratedSystem.Tools
{
    /// <summary>
    /// 自定义等待框
    /// </summary>
    public class c_WaitFormProgress
    {
        /// <summary>
        /// 等待窗口
        /// </summary>
        public static SplashScreenManager WaitSP = new SplashScreenManager(new Frm_WaitForm(), new Frm_WaitForm().GetType(), false, false);

        /// <summary>
        /// 弹出等待窗口
        /// </summary>
        /// <param name="Tile">标题</param>
        /// <param name="Description">描述</param>
        /// <param name="showTile">是否显示标题</param>
        /// <param name="showDescription">是否显示描述</param>
        /// <param name="showProgressBar">是否显示进度条</param>
        /// <param name="showProgressPercent">进度条是否百分比显示</param>
        public static void ShowSplashScreen(string Tile, string Description, bool showTile = true, bool showDescription = true, bool showProgressBar = true, bool showProgressPercent = false)
        {
            try
            {
                WaitSP.ShowWaitForm();
                WaitSP.SetWaitFormCaption(Tile);
                WaitSP.SetWaitFormDescription(Description);
                WaitSP.SendCommand(WaitFormEnum.WaitFormCommand.SetProgressPanel_ShowHide, showTile);//是否显示标题
                WaitSP.SendCommand(WaitFormEnum.WaitFormCommand.SetProgressPanelDescription_ShowHide, showDescription);//是否显示标题描述
                WaitSP.SendCommand(WaitFormEnum.WaitFormCommand.SetProgressBar_ShowHide, showProgressBar);//是否显示进度条
                WaitSP.SendCommand(WaitFormEnum.WaitFormCommand.SetProgressShowMode_Percent, showProgressPercent);//进度条是否按百分比显示
                Thread.Sleep(100);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 设置进度条进度
        /// </summary>
        /// <param name="CurrentValue">当前值</param>
        /// <param name="Maximum">最大值</param>
        public static void RunProgressBar(Int32 CurrentValue, Int32 Maximum)
        {
            try
            {
                WaitSP.SendCommand(WaitFormEnum.WaitFormCommand.SetProgressValue, new Int32[] { CurrentValue, Maximum });
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 关闭等待窗口
        /// </summary>
        public static void CloseSplashScreen()
        {
            try
            {
                if (WaitSP.IsSplashFormVisible)
                {
                    WaitSP.CloseWaitForm();
                }
            }
            catch (Exception) { }
        }
    }
}
