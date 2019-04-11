using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using epubto.Common;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Helpers;
using Reactive.Bindings.Interactivity;
using Reactive.Bindings.Notifiers;
using Reactive.Bindings.ObjectExtensions;

namespace epubto.Models
{
    public class KgInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 対象のEPUBファイル
        /// </summary>
        public ReactiveProperty<string> SourcePath { get; set; } = new ReactiveProperty<string>();

        /// <summary>
        /// 保存先 kindlegen単体では保存先指定する方法わからん。未使用。
        /// </summary>
        public ReactiveProperty<string> DestinationPath { get; set; } = new ReactiveProperty<string>();

        public ReactiveProperty<string> SourceName { get; set; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> SourceDirectory { get; set; } = new ReactiveProperty<string>();

        /// <summary> インデックス </summary>
        public int Index { get; private set; }

        public ReactiveProperty<int> StatusCode { get; set; } = new ReactiveProperty<int>(-1);

        /// <summary>
        /// 標準出力
        /// </summary>
        public ReactiveCollection<string> DetailOutputStandard { get; set; } = new ReactiveCollection<string>();

        /// <summary>
        /// エラー出力
        /// </summary>
        public ReactiveCollection<string> DetailOutputError { get; set; } = new ReactiveCollection<string>();

        /// <summary>
        /// 標準出力：テキスト形式
        /// </summary>
        public ReactiveProperty<string> DetailOutputStandardTxt { get; set; } = new ReactiveProperty<string>();

        /// <summary>
        /// エラー出力：テキスト形式
        /// </summary>
        public ReactiveProperty<string> DetailOutputErrorTxt { get; set; } = new ReactiveProperty<string>();

        /// <summary>
        /// 標準出力：最後の１行：テキスト形式
        /// </summary>
        public ReactiveProperty<string> DetailOutputStandardLastLine { get; set; } = new ReactiveProperty<string>();

        /// <summary>
        /// エラー出力：最後の１行：テキスト形式
        /// </summary>
        public ReactiveProperty<string> DetailOutputErrorLastLine { get; set; } = new ReactiveProperty<string>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public KgInfo(string value, int idx)
        {
            this.SourcePath.Value = value;
            this.Index = idx;

            this.DestinationPath.Value = System.IO.Path.ChangeExtension(value, ".mobi");

            this.SourceName.Value = System.IO.Path.GetFileName(value);
            this.SourceDirectory.Value = System.IO.Path.GetDirectoryName(value);

            this.DoCommand = this.CommandNotifire.Select(x => !x).ToReactiveCommand();
            this.DoCommand.Subscribe(myArgs => this.DoProcess(myArgs as string));

        }

        /// <summary>
        /// 実行
        /// </summary>
        public ReactiveCommand DoCommand { get; set; }

        /// <summary>
        /// 実行中か？
        /// </summary>
        public BusyNotifier CommandNotifire { get; set; } = new BusyNotifier();

        /// <summary>
        /// 処理本体
        /// </summary>
        public void DoProcess(string kgexe)
        {
            if (string.IsNullOrWhiteSpace(kgexe))
            {
                return;
            }

            using (var a = this.CommandNotifire.ProcessStart())
            {
                var exeinstance = new KindlegenUtil(kgexe);
                bool isSuccess = exeinstance.Do(this.SourcePath.Value);

                var stdout = new List<string>(exeinstance.OutputStandard);
                var errout = new List<string>(exeinstance.OutputError);

                this.DetailOutputStandard.AddRangeOnScheduler(stdout);
                this.DetailOutputError.AddRangeOnScheduler(errout);
                this.StatusCode.Value = exeinstance.StatusCode;

                this.DetailOutputStandardTxt.Value = string.Join("\r\n", stdout.Where(x => string.IsNullOrWhiteSpace(x) == false));
                this.DetailOutputErrorTxt.Value = string.Join("\r\n", errout.Where(x => string.IsNullOrWhiteSpace(x) == false));

                this.DetailOutputStandardLastLine.Value = stdout.LastOrDefault(x => string.IsNullOrWhiteSpace(x) == false);
                this.DetailOutputErrorLastLine.Value = errout.LastOrDefault(x => string.IsNullOrWhiteSpace(x) == false);
            }
        }
    }
}