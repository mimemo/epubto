// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Ace Check
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace epubto.Common
{
    public class AceCheckUtil
    {
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


        public AceCheckUtil()
        {
            this.OutputStandard = new List<string>();
            this.OutputError = new List<string>();
        }

        public bool Do(string iPath)
        {
            try
            {
                var outDir = System.IO.Path.GetDirectoryName(iPath);

                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,

                    WorkingDirectory = AceUtil.Path,
                    FileName = "ace.cmd",

                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                var arg = string.Format(" \"{0}\" --force --subdir --outdir \"{1}\"", iPath, outDir);
                psi.Arguments = arg;
                Console.WriteLine(arg);

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
                        this.OutputError.Add("結果取得に 失敗 @DoAceFor");
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
                this.OutputError.Add("Ace コマンド 失敗 @DoJavaJarFor");

                Console.WriteLine(@"Ace コマンド 失敗");
                Console.WriteLine(ex_psi.ToString());
                return false;
            }

            Console.WriteLine(string.Join("\r\n", this.OutputError));
            Console.WriteLine(string.Join("\r\n", this.OutputStandard));

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