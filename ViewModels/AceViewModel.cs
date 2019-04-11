namespace epubto.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using epubto.Common;
    using epubto.Models;
    using System.Windows;
    using Reactive.Bindings;
    using Reactive.Bindings.Extensions;
    using Reactive.Bindings.Notifiers;
    using Reactive.Bindings.Helpers;
    using Reactive.Bindings.ObjectExtensions;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using System.Collections.ObjectModel;

    /// <summary>メイン処理クラス </summary>
    public class AceViewModel
    {
        /// <summary>コンストラクタ Initializes a new instance of the <see cref="AceViewModel"/> class. </summary>
        public AceViewModel()
        {
            this.CurrentItemViewIndex.Value = 0;

            this.Items.ObserveAddChanged().Delay(TimeSpan.FromSeconds(0.1)).Subscribe(_ => this.UpData());

            // アイテムViewモードのリスト選択。未選択状態(-1)であれば強制的に0とする
            this.CurrentItemViewIndex.Subscribe(x => this.CurrentItemViewIndex.Value = x == -1 ? 0 : x);

            // 初期１個を追加
            var sampleepub = new EpubItem("sample.epub", 1);
            sampleepub.CurState.Value = EpubItem.State.Completed;

            this.Items.AddOnScheduler(sampleepub);

            this.EnableAceCheck.Value = AceUtil.CanRun;

            this.AceVersionTxt.Value = string.IsNullOrEmpty(AceUtil.Version) ? string.Empty : string.Format("{0}\r\n{1}", AceUtil.Path, AceUtil.Version);
            this.NodeVersionTxt.Value = string.Format("{0}", AceUtil.NodeVersion);

            // 上部タブ的メニュー
            this.InViewIsSelected0.Where(x => x).Subscribe(_ =>
            {
                this.InViewSelectedIndex.Value = 0;
                AceViewModel.IsShowBrowser.TurnOn();
            });
            this.InViewIsSelected1.Where(x => x).Subscribe(_ =>
            {
                this.InViewSelectedIndex.Value = 1;
                AceViewModel.IsShowBrowser.TurnOff();
            });
            this.InViewIsSelected2.Where(x => x).Subscribe(_ =>
            {
                this.InViewSelectedIndex.Value = 2;
                AceViewModel.IsShowBrowser.TurnOff();
            });
        }


        #region Public Properties

        /// <summary> The change message command.</summary>

        public ReactiveProperty<int> CurrentViewIndex { get; set; } = new ReactiveProperty<int>(0);
        public ReactiveProperty<int> CurrentItemViewIndex { get; set; } = new ReactiveProperty<int>(0);

        /// <summary>
        /// 全体的な情報をテキストで表示
        /// </summary>
        /// <value> 表示用のテキスト </value>
        public string StatusText { get; set; }

        /// <summary>
        /// 表示モード
        /// </summary>
        public ReactiveProperty<int> Mode { get; set; } = new ReactiveProperty<int>(0);

        /// <summary>
        /// AirSpace 問題対応
        /// </summary>
        public static BooleanNotifier IsShowBrowser { get; set; } = new BooleanNotifier(false);

        /// <summary>
        /// 処理対象データ
        /// </summary>
        public ReactiveCollection<EpubItem> Items { get; set; } = new ReactiveCollection<EpubItem>();

        /// <summary>
        ///  選択中のアイテム
        /// </summary>
        public ReactiveProperty<EpubItem> SelectedItem { get; set; } = new ReactiveProperty<EpubItem>();

        public int ItemCount { get; set; } = -1;

        /// <summary>
        /// Ace by DAISY
        /// </summary>
        public BooleanNotifier EnableAceCheck { get; set; } = new BooleanNotifier(false);

        public ReactiveProperty<string> AceVersionTxt { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> NodeVersionTxt { get; } = new ReactiveProperty<string>();

        /// <summary>
        /// View内のタブ的なやつ
        /// </summary>
        public ReactiveProperty<int> InViewSelectedIndex { get; set; } = new ReactiveProperty<int>(0);
        public BooleanNotifier InViewIsSelected0 { get; set; } = new BooleanNotifier(true);
        public BooleanNotifier InViewIsSelected1 { get; set; } = new BooleanNotifier(false);
        public BooleanNotifier InViewIsSelected2 { get; set; } = new BooleanNotifier(false);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// 更新
        /// </summary>
        public void UpData()
        {
            // 現在のItemsの数が、想定数と一致したらいっきに更新開始
            if (this.ItemCount == this.Items.Count)
            {
                // 順次実行
                foreach (var item in this.Items)
                {
                    item.DoProcessAce();
                }
            }

            this.ItemCount = -1;
        }

        /// <summary>データを追加</summary>
        /// <param name="files">対象ファイルパスのリスト</param>
        public void AddData(IEnumerable<string> files)
        {
            foreach (var item in files.Select((value, index) => new { value, index }))
            {
                this.Items.AddOnScheduler(new EpubItem(item.value, item.index + 1));
            }
        }

        /// <summary>データをクリア</summary>
        public void ClearData()
        {
            this.Items.Clear();
        }

        /// <summary>
        /// データをリセットする
        /// </summary>
        public void ResetData()
        {
            // 各行をリセット。最後に更新（確定）の為にリストにしている
            var resets = (from item in this.Items select item.Clear()).ToList();
        }

        #endregion

    }
}