using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace epubto.Services
{
    /// <summary>
    /// 未使用
    /// </summary>
    public static class AppPath
    {
        static AppPath()
        {
            // 一時フォルダをどこかに持つ場合（現状未使用）

            try
            {
                // 候補：ユーザー以下のTEMP
                var rootpath = Path.GetTempPath();
                if (HasWritePermissionOnDir(rootpath))
                {
                    var candidate = System.IO.Path.Combine(rootpath, "epubto");
                    if (System.IO.Directory.Exists(TempPath) == false)
                    {
                        System.IO.Directory.CreateDirectory(candidate);
                    }

                    if (HasWritePermissionOnDir(candidate))
                    {
                        AppPath.TempPath = candidate;
                        CreateReadme(candidate);

                        return;
                    }
                }
            }
            catch (Exception ex) { Debug.WriteLine(ex); }

            try
            {
                // 候補：マイピクチャ以下のTEMP
                var rootpath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                if (HasWritePermissionOnDir(rootpath))
                {
                    var candidate = System.IO.Path.Combine(rootpath, "epubto");
                    if (System.IO.Directory.Exists(TempPath) == false)
                    {
                        System.IO.Directory.CreateDirectory(candidate);
                    }
                    if (HasWritePermissionOnDir(candidate))
                    {
                        AppPath.TempPath = candidate;
                        CreateReadme(candidate);
                        return;
                    }
                }
            }
            catch (Exception ex) { Debug.WriteLine(ex); }

            try
            {
                // 候補：アプリの直近フォルダ以下
                var rootpath = System.AppDomain.CurrentDomain.BaseDirectory;
                if (HasWritePermissionOnDir(rootpath))
                {
                    var candidate = System.IO.Path.Combine(rootpath, "Temp");

                    if (System.IO.Directory.Exists(TempPath) == false)
                    {
                        System.IO.Directory.CreateDirectory(candidate);
                    }

                    if (HasWritePermissionOnDir(candidate))
                    {
                        AppPath.TempPath = candidate;
                        CreateReadme(candidate);

                        return;
                    }
                }
            }
            catch (Exception ex) { Debug.WriteLine(ex); }
        }

        /// <summary>
        /// メモを残す
        /// </summary>
        private static void CreateReadme(string folder)
        {
            var readme = System.IO.Path.Combine(folder, "note.txt");
            var message = "The files in this folder are temporary. It is OK to delete it.\r\nPlease leave the folder";

            System.IO.File.WriteAllText(readme, message);
        }

        public static string TempPath { get; } = string.Empty;

        /// <summary>
        /// 書き込み権限の確認
        /// </summary>
        /// <param name="path">対象ディレクトリ</param>
        /// <returns>権限ありならTRUE</returns>
        public static bool HasWritePermissionOnDir(string path)
        {
            var writeAllow = false;
            var writeDeny = false;

            var accessControlList = Directory.GetAccessControl(path);
            if (accessControlList == null)
            {
                return false;
            }

            var accessRules = accessControlList.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
            if (accessRules == null)
            {
                return false;
            }

            foreach (FileSystemAccessRule rule in accessRules)
            {
                if ((FileSystemRights.Write & rule.FileSystemRights) != FileSystemRights.Write)
                {
                    continue;
                }

                if (rule.AccessControlType == AccessControlType.Allow)
                {
                    writeAllow = true;
                }
                else if (rule.AccessControlType == AccessControlType.Deny)
                {
                    writeDeny = true;
                }
            }

            return writeAllow && !writeDeny;
        }
    }
}
