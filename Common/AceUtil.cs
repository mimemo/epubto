// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The Ace util.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace epubto.Common
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// The Ace util.
    /// </summary>
    public static class AceUtil
    {
        #region Static Fields

        /// <summary>
        /// Ace version.
        /// </summary>
        public static string Version { get; private set; } = string.Empty;

        /// <summary>
        /// Nodejp version.
        /// </summary>
        public static string NodeVersion { get; private set; } = string.Empty;

        /// <summary>
        /// Aceのワークディレクトリ
        /// </summary>
        public static string Path { get; private set; } = string.Empty;

        /// <summary>
        /// Aceのバージョンがあるか
        /// </summary>
        public static bool CanRun
        {
            get
            {
                return !string.IsNullOrWhiteSpace(AceUtil.Version);
            }
        }

        #endregion

        static AceUtil()
        {
            string output = string.Empty;
            string errput = string.Empty;

            string programFiles = Environment.ExpandEnvironmentVariables("%ProgramW6432%");
            string programFilesX86 = Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%");

            var nodeexe = System.IO.Path.Combine(programFiles, "nodejs", "node.exe");
            var nodeexe86 = System.IO.Path.Combine(programFilesX86, "nodejs", "node.exe");

            string fpath = string.Empty;

            if (System.IO.File.Exists(@nodeexe))
            {
                fpath = nodeexe;
            }
            else if (System.IO.File.Exists(@nodeexe86))
            {
                fpath = @nodeexe86;
            }
            else
            {
                // node 見つからない
                return;
            }

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = fpath,

                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                psi.Arguments = @" -v";
                using (Process p = Process.Start(psi))
                {
                    var outputArray = new List<string>();

                    output = p.StandardOutput.ReadToEnd(); // 標準出力の結果
                    errput = p.StandardError.ReadToEnd(); // エラー出力

                    // WaitForExitはReadToEndの後である必要がある (親プロセス、子プロセスでブロック防止のため)
                    p.WaitForExit();

                    outputArray.AddRange(output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
                    NodeVersion = outputArray.FirstOrDefault()?.Trim();

                    Console.WriteLine("NodeVersion = {0}", NodeVersion);
                }

                psi.Arguments = string.Format(" \"{0}\" -g list @daisy/ace",
                    System.IO.Path.Combine(System.IO.Path.GetDirectoryName(fpath), "node_modules", "npm", "bin", "npm-cli.js"));
                Console.WriteLine("psi.Arguments = {0}", psi.Arguments);

                // TODO:この辺整理
                using (Process p = Process.Start(psi))
                {
                    var outputArray = new List<string>();

                    output = p.StandardOutput.ReadToEnd(); // 標準出力の結果
                    errput = p.StandardError.ReadToEnd(); // エラー出力

                    // WaitForExitはReadToEndの後である必要がある (親プロセス、子プロセスでブロック防止のため)
                    p.WaitForExit();

                    outputArray.AddRange(output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));

                    Path = outputArray.FirstOrDefault(x => x.Contains(System.IO.Path.AltDirectorySeparatorChar) || x.Contains(System.IO.Path.DirectorySeparatorChar))?.Trim();
                    Version = outputArray.FirstOrDefault(x => x.Contains("@daisy/ace"))?.Trim(new[] { '`', '-', ' ' });

                    Console.WriteLine("Path = {0}", Path);
                    Console.WriteLine("Version = {0}", Version);
                }
            }
            catch (Exception ex_psi)
            {
                Version = string.Empty;
                Console.WriteLine("No Ace コマンド");
                Console.WriteLine(ex_psi.ToString());
                return;
            }
        }
    }
}