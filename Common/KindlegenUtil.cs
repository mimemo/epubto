// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The epub check util.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace epubto.Common
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;

    /// <summary>
    /// The epub check util.
    /// </summary>
    public class KindlegenUtil
    {
        /// <summary>
        /// Gets or sets the jar path.
        /// </summary>
        public static string ExePath { get; private set; }

        /// <summary>
        /// Gets or sets the output standard.
        /// </summary>
        public List<string> OutputStandard { get; set; }

        /// <summary>
        /// Gets or sets the output error.
        /// </summary>
        public List<string> OutputError { get; set; }

        /// <summary>
        /// 終了コード
        /// </summary>
        public int StatusCode { get; set; } = -1;

        public KindlegenUtil(string exepath)
        {
            ExePath = exepath;

            this.OutputStandard = new List<string>();
            this.OutputError = new List<string>();
        }

        /// <summary>
        /// Java コマンド実行
        /// </summary>
        /// <param name="iPath">対象ファイルパス</param>
        /// <returns>エラーまたは警告があればFALSE、それ以外はTRUE</returns>
        public bool Do(string iPath)
        {
            this.OutputStandard.Clear();
            this.OutputError.Clear();
            try
            {
                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo
                {
                    StandardOutputEncoding = new UTF8Encoding(false),
                    StandardErrorEncoding = new UTF8Encoding(false),

                    UseShellExecute = false,
                    CreateNoWindow = true,

                    FileName = ExePath,

                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                psi.Arguments = string.Format(" \"{0}\" -verbose", iPath);

                using (System.Diagnostics.Process p = System.Diagnostics.Process.Start(psi))
                {
                    // OutputDataReceivedとErrorDataReceivedイベントハンドラを追加
                    p.OutputDataReceived += this.P_OutputDataReceived;
                    p.ErrorDataReceived += this.P_ErrorDataReceived;

                    try
                    {
                        // 非同期で出力とエラーの読み取りを開始
                        p.BeginOutputReadLine();
                        p.BeginErrorReadLine();
                    }
                    catch
                    {
                        this.OutputError.Add("結果取得に 失敗 @DoKindlegenFor");
                        Console.WriteLine(@"結果取得に 失敗");
                    }

                    // WaitForExitはReadToEndの後である必要がある (親プロセス、子プロセスでブロック防止のため)
                    p.WaitForExit();
                    this.StatusCode = p.ExitCode;
                }

            }
            catch (Exception ex_psi)
            {
                this.OutputError.Clear();
                this.OutputError.Add("java コマンド 失敗 @DoKindlegenFor");

                Console.WriteLine(@"java コマンド 失敗");
                Console.WriteLine(ex_psi.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// OutputDataReceivedイベントハンドラ
        /// 行が出力されるたびに呼び出される
        /// </summary>
        private void P_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                this.OutputStandard.Add(e.Data);
            }
        }

        /// <summary>
        /// ErrorDataReceivedイベントハンドラ
        /// </summary>
        private void P_ErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                this.OutputError.Add(e.Data);
            }
        }

    }
}