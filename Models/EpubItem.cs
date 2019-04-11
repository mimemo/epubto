// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The epub item.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace epubto.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Windows.Media.Imaging;
    using System.Xml.Linq;
    using System.Reactive.Linq;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using Reactive.Bindings;
    using Reactive.Bindings.Extensions;
    using Reactive.Bindings.ObjectExtensions;
    using Reactive.Bindings.Binding;
    using Reactive.Bindings.Helpers;

    // https://archive.codeplex.com/?p=dynamicjson
    // donwload dynamicjson and place dynamicjson.dll file to [dll]　directory.  
    using Codeplex.Data;

    using epubto.Services;
    using epubto.Common;
    using System.Windows.Media;


    /// <summary>
    /// EPUB情報
    /// </summary>
    public class EpubItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///  処理状態リスト：処理結果ではない
        /// </summary>
        public enum State
        {
            /// <summary>
            /// 未処理
            /// </summary>
            Unexecuted = 0,

            /// <summary>
            /// 実行中
            /// </summary>
            Processing = 1,

            /// <summary>
            /// 完了
            /// </summary>
            Completed = 2,

            /// <summary>
            /// エラー
            /// </summary>
            ProcessError = 3,

        }


        /// <summary> コンストラクタ</summary>
        /// <param name="filePath"> 対象のEPUBファイルフルパス</param>
        /// <param name="index">インデックス</param>
        public EpubItem(string filePath, int index)
        {
            this.Index = index;

            this.FullPath = filePath;

            // カバー画像を取得
            this.ImageAnalyze();

            // below for Ace
            this.OpneReportWithBrowserCommand.Subscribe(_ => this.OpenBrowser());
        }

        /// <summary>
        /// ローカルファイルを開く
        /// </summary>
        private void OpenBrowser()
        {
            if (this.AceResultHtml == null || string.IsNullOrWhiteSpace(this.AceResultHtml.Value))
            {
                return;
            }

            if (System.IO.File.Exists(this.AceResultHtml.Value))
            {
                System.Diagnostics.Process.Start(this.AceResultHtml.Value);
            }
            else
            {
                var folder = System.IO.Path.GetDirectoryName(this.AceResultHtml.Value);
                if (System.IO.Directory.Exists(folder))
                {
                    System.Diagnostics.Process.Start(folder);
                }
            }
        }

        /// <summary>
        /// ファイルパスからAceの結果ファイルの場所に変換
        /// </summary>
        /// <param name="filePath">EPUBファイルパス</param>
        /// <param name="isHtml">HtmlならTRUE,それ以外はFalse</param>
        /// <returns>ファイルパス</returns>
        private string MakePath(string filePath, bool isHtml)
        {
            var folder = System.IO.Path.GetDirectoryName(filePath);
            var name = System.IO.Path.GetFileNameWithoutExtension(filePath);

            if (string.IsNullOrWhiteSpace(folder))
            {
                return string.Empty;
            }

            return System.IO.Path.Combine(folder, name, isHtml ? "report.html" : "report.json");
        }

        private readonly static string[] BitmapImageSUPPORTS = new[] { ".bmp", ".gif", ".ico", ".jpg", ".png", ".tiff" };


        /// <summary>
        /// 画像を取得
        /// </summary>
        private void ImageAnalyze()
        {
            string zipPath = this.FullPath;

            if (System.IO.File.Exists(zipPath) == false)
            {
                const string fileUri = "/Assets/imagebroken.png";
                var bitmap = new BitmapImage(new Uri(fileUri, UriKind.Relative));
                this.CoverImg.Value = bitmap;
                return;
            }

            using (ZipArchive archive = ZipFile.OpenRead(zipPath))
            {
                // opfファイルを探す
                var opfTxt = string.Empty;

                foreach (ZipArchiveEntry entry in archive.Entries.Where(x => x.FullName.EndsWith(".opf", StringComparison.OrdinalIgnoreCase)))
                {
                    Console.WriteLine(entry.FullName);

                    using (StreamReader reader = new StreamReader(entry.Open()))
                    {
                        opfTxt = reader.ReadToEnd();
                    }
                }

                // カバーファイルを探す
                var coverfilename = string.Empty;

                if (string.IsNullOrWhiteSpace(opfTxt) == false)
                {
                    XDocument xdoc = XDocument.Parse(opfTxt);

                    var coverfilenames = (from item in xdoc.Descendants()
                                          where item.Name.LocalName == "item"
                                          let attr = item.Attribute("properties")
                                          let href = item.Attribute("href")
                                          where attr != null && href != null
                                          where item.Attribute("properties").Value.Contains("cover-image")
                                          select item.Attribute("href").Value).ToList();

                    if (coverfilenames.Any(x => BitmapImageSUPPORTS.Contains(System.IO.Path.GetExtension(x)?.ToLower())))
                    {
                        coverfilename = System.IO.Path.GetFileName(coverfilenames[0]);
                    }
                }

                // カバーファイルを読み込む
                var entryArray = archive.Entries.Where(x => x.Name.Equals(coverfilename, StringComparison.OrdinalIgnoreCase)).ToList();

                if (string.IsNullOrWhiteSpace(coverfilename) == false && entryArray.Any())
                {
                    foreach (ZipArchiveEntry entry in entryArray)
                    {
                        Byte[] buffer = new Byte[entry.Length];

                        using (Stream stream = entry.Open())
                        {
                            stream.Read(buffer, 0, buffer.Length);

                            BitmapImage bmpImage = new BitmapImage();
                            bmpImage.BeginInit();
                            bmpImage.CacheOption = BitmapCacheOption.OnLoad;

                            bmpImage.StreamSource = new System.IO.MemoryStream(buffer);
                            bmpImage.EndInit();
                            bmpImage.Freeze();

                            this.CoverImg.Value = bmpImage;

                        }

                        break;
                    }
                }
                else
                {
                    const string fileUri = "/Assets/imagebroken.png";
                    var bitmap = new BitmapImage(new Uri(fileUri, UriKind.Relative));
                    this.CoverImg.Value = bitmap;
                }
            }
        }

        /// <summary> インデックス </summary>
        public int Index { get; private set; }

        /// <summary> 画像 </summary>
        public ReactiveProperty<BitmapImage> CoverImg { get; set; } = new ReactiveProperty<BitmapImage>();

        /// <summary>
        /// 処理状態
        /// </summary>
        public ReactiveProperty<State> CurState { get; set; } = new ReactiveProperty<State>(State.Unexecuted);

        /// <summary>
        /// EPUBファイルのフルパス
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// ファイル名
        /// </summary>
        public string FileName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.FullPath))
                {
                    return string.Empty;
                }

                return Path.GetFileName(this.FullPath);
            }
        }

        /// <summary>
        /// 標準出力を記憶
        /// </summary>
        private List<string> DetailOutputStandard { get; set; } = new List<string>();

        /// <summary>
        /// 標準エラー出力を記憶
        /// </summary>
        private List<string> DetailOutputError { get; set; } = new List<string>();




        #region Ace Property

        /// <summary>
        /// Ace result, html file path
        /// </summary>
        public ReactiveProperty<string> AceResultHtml { get; set; } = new ReactiveProperty<string>(string.Empty);
        public ReactiveProperty<string> AceResultJson { get; set; } = new ReactiveProperty<string>(string.Empty);

        /// <summary>
        /// 標準出力を記憶 ace
        /// </summary>
        public ReactiveCollection<AceLog> DetailOutputStandardAce { get; set; } = new ReactiveCollection<AceLog>();

        /// <summary>
        /// 標準エラー出力を記憶 ace
        /// </summary>
        public ReactiveCollection<AceLog> DetailOutputErrorAce { get; set; } = new ReactiveCollection<AceLog>();

        /// <summary> 終了ステータスコード </summary>
        public ReactiveProperty<int> ExitCodeAce { get; set; } = new ReactiveProperty<int>(-1);
        public ReactiveProperty<SolidColorBrush> ExitCodeAceColor { get; set; } = new ReactiveProperty<SolidColorBrush>();

        public AceSummaryOfViolations Violations { get; set; } = new AceSummaryOfViolations();

        public ReadOnlyReactivePropertySlim<int> ViolationsTotalCritical { get; set; }

        public ReactiveCollection<AceViolationCount> SummaryOfViolationsList { get; set; } = new ReactiveCollection<AceViolationCount>();

        /// <summary>
        /// ブラウザで開く
        /// </summary>
        public ReactiveCommand OpneReportWithBrowserCommand { get; private set; } = new ReactiveCommand();

        #endregion



        /// <summary>
        /// 対象のJSONファイルを削除
        /// </summary>
        private void RemoveJsonFile(string target)
        {
            try
            {
                System.IO.File.Delete(target);
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.WriteLine(ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// JSON出力ファイルパスを生成
        /// </summary>
        /// <param name="fullPath">元のEPUBファイルパス</param>
        /// <returns>JSONファイルパス</returns>
        private string MakeJsonPath(string fullPath)
        {
            //8文字のランダムな文字列を作成する
            Guid g = System.Guid.NewGuid();
            string rdm = g.ToString("N").Substring(0, 8);

            var orgname = System.IO.Path.GetFileNameWithoutExtension(fullPath);

            // 出力ファイル名
            var jsonName = string.Format("{0}_{1}{2}", orgname, rdm, ".json");

            return System.IO.Path.Combine(AppPath.TempPath, jsonName);
        }

        /// <summary>
        /// チェック本体
        /// </summary>
        public void DoProcessAce()
        {
            this.CurState.Value = State.Processing;

            // Ace by Daisy
            var aceCheckUtil = new AceCheckUtil();

            aceCheckUtil.Do(this.FullPath);

            this.DetailOutputStandardAce.AddRangeOnScheduler(aceCheckUtil.OutputStandard.Select(x => new AceLog(x)));
            this.DetailOutputErrorAce.AddRangeOnScheduler(aceCheckUtil.OutputError.Select(x => new AceLog(x)));

            this.AceResultHtml.Value = this.MakePath(this.FullPath, true);
            this.AceResultJson.Value = this.MakePath(this.FullPath, false);

            this.ExitCodeAce.Subscribe(x =>
            {
                switch (x)
                {
                    case -1:
                        {
                            // non
                            this.ExitCodeAceColor.Value = new SolidColorBrush(Colors.WhiteSmoke);
                            break;
                        }
                    case 0:
                        {
                            // valid
                            this.ExitCodeAceColor.Value = new SolidColorBrush(Colors.LightGreen);
                            break;
                        }
                    case 1:
                        {
                            // invalid
                            this.ExitCodeAceColor.Value = new SolidColorBrush(Colors.LightPink);
                            break;
                        }

                    default:
                        this.ExitCodeAceColor.Value = new SolidColorBrush(Colors.WhiteSmoke);
                        break;
                }
            });


            this.ExitCodeAce.Value = aceCheckUtil.StatusCode;
            this.LoadJson(this.AceResultJson.Value);

            // 各値を更新
            this.SummaryOfViolationsList.ClearOnScheduler();
            this.SummaryOfViolationsList.AddRangeOnScheduler(this.MakeAceSummary());

            this.CurState.Value = State.Completed;
        }

        /// <summary>
        /// JSONの読み込み
        /// </summary>
        /// <param name="aceResultJson">Ace結果パス</param>
        private void LoadJson(string filepath)
        {
            if (System.IO.File.Exists(filepath) == false)
            {
                return;
            }

            // 読み込み
            var rawtxt = System.IO.File.ReadAllText(filepath);
            rawtxt = rawtxt.Replace("\"earl:", "\"");

            // パース
            var json = DynamicJson.Parse(@rawtxt);

            try
            {
                foreach (dynamic item in json.assertions)
                {
                    Console.WriteLine("-- assertions at -- ");
                    Console.WriteLine(item);

                    foreach (dynamic item2 in item.assertions)
                    {
                        Console.WriteLine("\t" + item2);

                        if (item2.IsDefined("test"))
                        {
                            dynamic testItem = item2.test;

                            var tags = new List<string>();
                            if (testItem.IsDefined("rulesetTags"))
                            {
                                foreach (dynamic tagitem in testItem.rulesetTags)
                                {
                                    tags.Add(tagitem);
                                }
                            }

                            string impactVal = testItem.IsDefined("impact") ? testItem.impact : string.Empty;

                            this.Violations.Add(impactVal, tags);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// AceのOutputをテキストに
        /// </summary>
        private AceViolationCount[] MakeAceSummary()
        {
            var result = new List<AceViolationCount>
            {
                this.Violations.WCAG20A.Value,
                this.Violations.WCAG20AA.Value,
                this.Violations.EPUB.Value,
                this.Violations.BestPractice.Value,
                this.Violations.Other.Value,
            };

            this.Violations.Total.Value.Critical.Value = result.Sum(x => x.Critical.Value);
            this.Violations.Total.Value.Serious.Value = result.Sum(x => x.Serious.Value);
            this.Violations.Total.Value.Moderate.Value = result.Sum(x => x.Moderate.Value);
            this.Violations.Total.Value.Minor.Value = result.Sum(x => x.Minor.Value);
            this.Violations.Total.Value.Total.Value = result.Sum(x => x.Total.Value);

            result.Add(this.Violations.Total.Value);

            return result.ToArray();
        }




        /// <summary>
        /// 自身をリセット。フルパスのみ保持
        /// </summary>
        /// <returns>自身 </returns>
        public EpubItem Clear()
        {
            // フルパス以外は初期値に戻す
            this.CurState.Value = State.Unexecuted;

            this.DetailOutputStandard = new List<string>();
            this.DetailOutputError = new List<string>();

            return this;
        }
    }
}