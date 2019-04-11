// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The drop file util.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace epubto.Common
{
    using System.IO;
    using System.Linq;
    using System.Windows;

    /// <summary>
    /// ドロップされたものを整理
    /// </summary>
    internal class DropFileUtil
    {
        #region Methods

        /// <summary>
        /// 引数から１つのファイルを取り出して返す。
        /// </summary>
        /// <param name="args">
        /// ドロップされたアイテム（個数不明）
        /// </param>
        /// <returns>
        /// １つのフォルダであればそれを返す
        /// </returns>
        internal static string IsSingleDirectory(DragEventArgs args)
        {
            // ドロップされたオブジェクトが空でないか
            if (args.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                var fileNames = args.Data.GetData(DataFormats.FileDrop, true) as string[];

                // ドロップされたファイルが１つだけかどうか。
                if (fileNames.Length == 1)
                {
                    // ドロップされたファイルがフォルダかどうか。
                    if (Directory.Exists(fileNames[0]))
                    {
                        return fileNames[0];
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 引数から１つのファイルを取り出して返す。
        ///     拡張子を判定しない
        /// </summary>
        /// <param name="args">
        /// ドロップされたアイテム（個数不明）
        /// </param>
        /// <returns>
        /// １つのファイルであればそれを返す
        /// </returns>
        internal static string IsSingleFile(DragEventArgs args)
        {
            return IsSingleFile(args, string.Empty);
        }

        /// <summary>
        /// 引数から１つのファイルを取り出して返す。
        /// </summary>
        /// <param name="args">
        /// ドロップされたアイテム（個数不明）
        /// </param>
        /// <param name="extensionText">
        /// 拡張子文字列（ドットを含む）例：[.txt]
        /// </param>
        /// <returns>
        /// １つのファイルであればそれを返す
        /// </returns>
        internal static string IsSingleFile(DragEventArgs args, string extensionText)
        {
            // ドロップされたオブジェクトが空でないか
            if (args.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                var fileNames = args.Data.GetData(DataFormats.FileDrop, true) as string[];

                // ドロップされたファイルが１つだけかどうか。
                if (fileNames.Length == 1)
                {
                    if (string.IsNullOrWhiteSpace(extensionText) == false)
                    {
                        // 拡張子を取得(比較用の小文字にする)
                        string fileExte = Path.GetExtension(fileNames[0]).ToLower();

                        // 拡張子判定あり（複数指定可能）
                        string[] extes = extensionText.Split('|');

                        // 拡張子があっているかどうか
                        if (extes.Contains(fileExte))
                        {
                            // ドロップされたファイルが存在するかどうか
                            if (File.Exists(fileNames[0]))
                            {
                                return fileNames[0];
                            }
                        }
                    }
                    else
                    {
                        // 拡張子判定無し
                        // ドロップされたファイルが存在するかどうか
                        if (File.Exists(fileNames[0]))
                        {
                            return fileNames[0];
                        }
                    }
                }
            }

            return string.Empty;
        }

        #endregion
    }
}