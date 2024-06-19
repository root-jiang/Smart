using System;
using System.Text.RegularExpressions;

namespace Aspect.AddinFramework
{
    /// <summary>
    /// 客户端版本
    /// </summary>
    public class ClientVersion
    {
        public long Date { get; set; }
        public Version BP { get; set; }
        public Version RC { get; set; }

        public ClientVersion(string version)
        {
            // 20230217(BP1.0-RC1.5.3)
            var vDate = Regex.Match(version, "(.+(?=\\(BP))");
            var vBP = Regex.Match(version, "(?<=\\(BP)(.+?)(?=\\-RC)");
            var vRC = Regex.Match(version, "(?<=\\-RC)(.+?)(?=\\))");

            Date = long.Parse(vDate?.Value);
            BP = Version.Parse(vBP?.Value);
            RC = Version.Parse(vRC?.Value);
        }
    }
}
