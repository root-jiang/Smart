using Microsoft.Win32;
using System;

namespace Aspect.AddinFramework
{
    /// <summary>
    /// 有效期限
    /// </summary>
    public class Validity
    {
        /// <summary>
        /// 检查有效期限
        /// </summary>
        /// <returns>true有效，false已失效</returns>
        public static bool CheckClientValidity(out string message)
        {
            message = null;
            // 默认有效期限90天
            var days = 90;
            RegistryKey key = null;
            try
            {
                // 软件版本
                var version = Registry.CurrentUser.OpenSubKey("Software\\ASPECT", true)?.GetValue("Version")?.ToString();
                // 读注册表
                key = Registry.CurrentUser.OpenSubKey("Software\\956B023E-2E63-D9D5-6CD6-27CE6F7FD42B", true);
                // 首次登录，写注册表
                if (key == null)
                {
                    key = Registry.CurrentUser.CreateSubKey("Software\\956B023E-2E63-D9D5-6CD6-27CE6F7FD42B");
                }
                // 安装时间
                var keyIDate = key?.GetValue("Install Date")?.ToString();
                // 安装版本(用于检查版本更新)
                var keyV = key?.GetValue("Install Version")?.ToString();
                if (string.IsNullOrEmpty(keyIDate) || string.IsNullOrEmpty(keyV))
                {
                    key.SetValue("Install Date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    key.SetValue("Install Version", version);
                    return true;
                }
                var newV = new ClientVersion(version);
                var oldV = new ClientVersion(keyV);

                // 当前版本更新，修改安装时间
                if (newV.Date > oldV.Date || newV.BP > oldV.BP || newV.RC > oldV.RC)
                {
                    key.SetValue("Install Date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    key.SetValue("Install Version", version);
                    return true;
                }
                
                if(DateTime.Parse(keyIDate) > DateTime.Now)
                {
                    message = "Invalid client license!";
                    return false;
                }

                // 检查有效期限
                return DateTime.Parse(keyIDate).AddDays(days) > DateTime.Now;
            }
            finally
            {
                key?.Close();
            }
        }
    }
}
