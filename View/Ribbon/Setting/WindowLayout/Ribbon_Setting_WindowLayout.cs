using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking;
using FaultTreeAnalysis.Common;

namespace FaultTreeAnalysis
{
    partial class FTAControl
    {
        /// <summary>
        /// 初始化Ribbon-Setting-WindowLayout下的菜单
        /// </summary>
        private void Init_Ribbon_Setting_WindowLayout()
        {
            //窗口管理菜单动态产生子菜单
            barSubItem_Windows.GetItemData += GetItemData_BarSubItem_Windows;
        }

        /// <summary>
        /// 窗口管理菜单动态产生子菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetItemData_BarSubItem_Windows(object sender, System.EventArgs e)
        {
            General.TryCatch(() =>
            {
                //解除上次的菜单
                foreach (BarItemLink item in barSubItem_Windows.ItemLinks)
                {
                    item.Item.ItemClick -= ItemClick_Ribbon_Setting_WindowLayout;
                }
                barSubItem_Windows.ItemLinks.Clear();
                //添加新菜单

                foreach (DockPanel panel in dockManager_FTA.Panels)
                {
                    if (panel.Name.Contains("DP_"))
                    {
                        continue;
                    }
                    BarButtonItem bar = new BarButtonItem();
                    bar.Caption = panel.Text;
                    bar.ItemClick += ItemClick_Ribbon_Setting_WindowLayout;
                    bar.Tag = panel;
                    if (panel.Visibility != DockVisibility.Hidden) bar.ImageOptions.Image = Properties.Resources.show_16x16;
                    else bar.ImageOptions.Image = Properties.Resources.hide_16x16;
                    barSubItem_Windows.ItemLinks.Add(bar);
                }
            });
        }

        /// <summary>
        /// Ribbon-Setting-WindowLayout下的菜单的单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemClick_Ribbon_Setting_WindowLayout(object sender, ItemClickEventArgs e)
        {
            General.TryCatch(() =>
            {
                //窗口子菜单按钮
                if (e.Item != null && e.Item.Tag != null && e.Item.Tag.GetType() == typeof(DockPanel))
                {
                    DockPanel panel = e.Item.Tag as DockPanel;
                    if (panel.Visibility == DockVisibility.Hidden)
                    {
                        panel.Visibility = DockVisibility.Visible;
                        e.Item.ImageOptions.Image = Properties.Resources.show_16x16;
                    }
                    else
                    {
                        panel.Visibility = DockVisibility.Hidden;
                        e.Item.ImageOptions.Image = Properties.Resources.hide_16x16;
                    }
                }
            });
        }
    }
}
