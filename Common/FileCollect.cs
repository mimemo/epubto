// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   ファイルの収集
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace epubto.Common
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// ファイルの収集
    /// </summary>
    internal class FileCollect
    {
        /// <summary>
        /// 拡張子 EPUB にマッチするものを探す
        /// </summary>
        private static readonly System.Text.RegularExpressions.Regex IsExtensionIsEpub = new System.Text.RegularExpressions.Regex(@".+\.epub$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        #region Methods

        /// <summary>
        /// ファイルであり、拡張子EPUBのものを収集
        ///     トップフォルダのみ対象とする
        /// </summary>
        /// <param name="path"> 検索対象フォルダ </param>
        /// <param name="opt">検索オプション</param>
        internal static string[] GetEpubFile(string path, SearchOption opt)
        {
            // パスが存在しなければ、何もせず
            if (Directory.Exists(path) == false)
            {
                return null;
            }

            // フォルダの中をチェック
            List<string> directoryList = new List<string>(Directory.GetFiles(path, "*", opt));

            return CollectEpubFiles(directoryList);
        }

        /// <summary>
        /// ファイル名が一致「mimetype」. mimetype の存在するフォルダのリストを取得
        /// </summary>
        public static string[] CollectMimetypeFolders(string[] files)
        {
            // ファイルリスト
            List<string> targetList = new List<string>();
            foreach (string file in files)
            {
                if (System.IO.File.Exists(file))
                {
                    var name = System.IO.Path.GetFileNameWithoutExtension(file);

                    // ファイルの場合
                    if (name.Equals("mimetype", StringComparison.CurrentCultureIgnoreCase))
                    {
                        targetList.Add(file);
                    }
                }

                if (System.IO.Directory.Exists(file))
                {
                    // フォルダの場合
                    List<string> filelist = new List<string>(System.IO.Directory.GetFiles(file, "*", System.IO.SearchOption.AllDirectories));

                    // 名前でフィルタ
                    var filterd = filelist.Where(x => System.IO.Path.GetFileNameWithoutExtension(x).Equals("mimetype", StringComparison.CurrentCultureIgnoreCase));

                    // 名前一致のみ登録
                    targetList.AddRange(filterd);
                }
            }

            if (targetList.Count <= 0)
            {
                return targetList.ToArray();
            }

            // フォルダリスト
            var folders = targetList.Select(x => System.IO.Path.GetDirectoryName(x));

            // 重複削除と、名前ソート
            folders = folders.Distinct().OrderBy(x => x);

            return folders.ToArray();
        }

        /// <summary> 拡張子がEPUBのファイルのみ収集して返す </summary>
        /// <param name="files">ドロップされた全てのファイル／フォルダ</param>
        /// <returns>EPUBファイルのみ返す</returns>
        public static string[] CollectEpubFiles(IEnumerable<string> files)
        {
            // EPUBファイルリスト
            List<string> epublist = new List<string>();
            foreach (string file in files)
            {
                if (System.IO.File.Exists(file))
                {
                    // ファイルの場合
                    if (IsExtensionIsEpub.IsMatch(file))
                    {
                        // 拡張子一致する場合
                        epublist.Add(file);
                    }
                }
                else
                {
                    // フォルダの場合
                    List<string> filelist = new List<string>(System.IO.Directory.GetFiles(file, "*", System.IO.SearchOption.AllDirectories));

                    // 拡張子でフィルタ
                    var filterd = filelist.Where(filepath => IsExtensionIsEpub.IsMatch(filepath));

                    // 拡張子条件を通ったもののみ登録
                    epublist.AddRange(filterd);
                }
            }

            if (epublist.Count <= 0)
            {
                return epublist.ToArray();
            }

            return epublist.ToArray();
        }

        #endregion
    }
}