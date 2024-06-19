using FaultTreeAnalysis.Model.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FaultTreeAnalysis.Common
{
    public static class RecentFiles
    {
        public static void AddProject(string Path)
        {
            RecentModel RM = new Model.Data.RecentModel();
            string savingDataPath = $"{AppDomain.CurrentDomain.BaseDirectory}{Properties.Resources.SavingDataPath}";//\Repository\Configuration\Data.json
            FileInfo info = new FileInfo(savingDataPath);

            if (File.Exists(info.DirectoryName + "\\RecentFile.json"))
            {
                string Datas = File.ReadAllText(info.DirectoryName + "\\RecentFile.json", Encoding.UTF8);
                try
                {
                    RM = Newtonsoft.Json.JsonConvert.DeserializeObject<RecentModel>(Datas);
                }
                catch (Exception)
                {
                    RM = new Model.Data.RecentModel();
                }
            }

            if (RM.RecentProject == null)
            {
                RM.RecentProject = new Dictionary<string, string>();
            }
            if (RM.RecentFaultTree == null)
            {
                RM.RecentFaultTree = new Dictionary<string, string>();
            }

            //超过10个删除第一个
            if (RM.RecentProject.Count > 10)
            {
                RM.RecentProject.Remove(RM.RecentProject.Keys.First());
            }

            //已存在要移动到最后
            if (RM.RecentProject.Keys.Contains(Path))
            {
                Dictionary<string, string> NewDic = new Dictionary<string, string>();
                foreach (KeyValuePair<string, string> keyV in RM.RecentProject)
                {
                    if (keyV.Key != Path)
                    {
                        NewDic.Add(keyV.Key, keyV.Value);
                    }
                }
                NewDic.Add(Path, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                RM.RecentProject = NewDic;
            }
            else
            {
                RM.RecentProject.Add(Path, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }

            //删除不存在的文件
            KeyValuePair<string, string>[] KVs1 = RM.RecentProject.Where(it => Directory.Exists(it.Key) == false).ToArray();
            KeyValuePair<string, string>[] KVs2 = RM.RecentFaultTree.Where(it => (File.Exists(it.Key) == false || new FileInfo(it.Key).Extension != FixedString.APP_EXTENSION)).ToArray();
            foreach (KeyValuePair<string, string> keyV in KVs1)
            {
                RM.RecentProject.Remove(keyV.Key);
            }
            foreach (KeyValuePair<string, string> keyV in KVs2)
            {
                RM.RecentFaultTree.Remove(keyV.Key);
            }

            File.WriteAllText(info.DirectoryName + "\\RecentFile.json", Newtonsoft.Json.JsonConvert.SerializeObject(RM), Encoding.UTF8);
        }

        public static void AddFaultTree(string Path)
        {
            RecentModel RM = new Model.Data.RecentModel();
            string savingDataPath = $"{AppDomain.CurrentDomain.BaseDirectory}{Properties.Resources.SavingDataPath}";//\Repository\Configuration\Data.json
            FileInfo info = new FileInfo(savingDataPath);

            if (File.Exists(info.DirectoryName + "\\RecentFile.json"))
            {
                string Datas = File.ReadAllText(info.DirectoryName + "\\RecentFile.json", Encoding.UTF8);
                try
                {
                    RM = Newtonsoft.Json.JsonConvert.DeserializeObject<RecentModel>(Datas);
                }
                catch (Exception)
                {
                    RM = new Model.Data.RecentModel();
                }
            }

            if (RM.RecentProject == null)
            {
                RM.RecentProject = new Dictionary<string, string>();
            }
            if (RM.RecentFaultTree == null)
            {
                RM.RecentFaultTree = new Dictionary<string, string>();
            }

            if (RM.RecentFaultTree.Count > 10)
            {
                RM.RecentFaultTree.Remove(RM.RecentFaultTree.Keys.First());
            }


            //已存在要移动到最后
            if (RM.RecentFaultTree.Keys.Contains(Path))
            {
                Dictionary<string, string> NewDic = new Dictionary<string, string>();
                foreach (KeyValuePair<string, string> keyV in RM.RecentFaultTree)
                {
                    if (keyV.Key != Path)
                    {
                        NewDic.Add(keyV.Key, keyV.Value);
                    }
                }
                NewDic.Add(Path, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                RM.RecentFaultTree = NewDic;
            }
            else
            {
                RM.RecentFaultTree.Add(Path, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }

            //删除不存在的文件
            KeyValuePair<string, string>[] KVs1 = RM.RecentProject.Where(it => Directory.Exists(it.Key) == false).ToArray();
            KeyValuePair<string, string>[] KVs2 = RM.RecentFaultTree.Where(it => (File.Exists(it.Key) == false || new FileInfo(it.Key).Extension != FixedString.APP_EXTENSION)).ToArray();
            foreach (KeyValuePair<string, string> keyV in KVs1)
            {
                RM.RecentProject.Remove(keyV.Key);
            }
            foreach (KeyValuePair<string, string> keyV in KVs2)
            {
                RM.RecentFaultTree.Remove(keyV.Key);
            }

            File.WriteAllText(info.DirectoryName + "\\RecentFile.json", Newtonsoft.Json.JsonConvert.SerializeObject(RM), Encoding.UTF8);
        }

        public static RecentModel GetRecentModel()
        {
            RecentModel RM = new Model.Data.RecentModel();
            string savingDataPath = $"{AppDomain.CurrentDomain.BaseDirectory}{Properties.Resources.SavingDataPath}";//\Repository\Configuration\Data.json
            FileInfo info = new FileInfo(savingDataPath);

            if (File.Exists(info.DirectoryName + "\\RecentFile.json"))
            {
                string Datas = File.ReadAllText(info.DirectoryName + "\\RecentFile.json", Encoding.UTF8);

                try
                {
                    RM = Newtonsoft.Json.JsonConvert.DeserializeObject<RecentModel>(Datas);
                }
                catch (Exception)
                {
                    RM = new Model.Data.RecentModel();
                }
            }

            if (RM.RecentProject == null)
            {
                RM.RecentProject = new Dictionary<string, string>();
            }
            if (RM.RecentFaultTree == null)
            {
                RM.RecentFaultTree = new Dictionary<string, string>();
            }

            //删除不存在的
            KeyValuePair<string, string>[] KVs1 = RM.RecentProject.Where(it => Directory.Exists(it.Key) == false).ToArray();
            KeyValuePair<string, string>[] KVs2 = RM.RecentFaultTree.Where(it => (File.Exists(it.Key) == false || new FileInfo(it.Key).Extension != FixedString.APP_EXTENSION)).ToArray();
            foreach (KeyValuePair<string, string> keyV in KVs1)
            {
                RM.RecentProject.Remove(keyV.Key);
            }
            foreach (KeyValuePair<string, string> keyV in KVs2)
            {
                RM.RecentFaultTree.Remove(keyV.Key);
            }

            return RM;
        }
    }
}
