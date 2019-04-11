using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using epubto.Models;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;
using Reactive.Bindings.Notifiers;
using epubto.Properties;

namespace epubto.ViewModels
{
    public class KindlegenViewModel
    {
        public ReactiveProperty<string> Status { get; set; } = new ReactiveProperty<string>();

        /// <summary>
        /// アイテム一覧
        /// </summary>
        public ReactiveCollection<KgInfo> Items { get; set; } = new ReactiveCollection<KgInfo>();

        /// <summary>
        /// 選択しているアイテム
        /// </summary>
        public ReactiveProperty<KgInfo> SelectItem { get; set; } = new ReactiveProperty<KgInfo>();

        /// <summary>
        /// ドロップ想定数（bad know how 
        /// </summary>
        public int ItemCount { get; set; } = -1;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public KindlegenViewModel()
        {
            // 追加時に処理
            this.Items.ObserveAddChanged().Delay(TimeSpan.FromSeconds(0.1)).Subscribe(x => this.UpData());
        }

        /// <summary>
        ///  追加される毎に処理が走る
        /// </summary>
        private void UpData()
        {
            // exe path
            var kgpath = Settings.Default.KindlegenExePath;

            // 現在のItemsの数が、想定数と一致したら更新開始
            if (this.Items.Count == this.ItemCount)
            {
                foreach (var item in this.Items)
                {
                    // 念のため１つづつ。並列実行しても大丈夫かな？
                    item.DoCommand.Execute(kgpath);
                }

                this.ItemCount = -1;

            }
        }

        /// <summary>
        /// クリア
        /// </summary>
        public void ClearData()
        {
            this.Items.Clear();
        }

        /// <summary>
        /// 追加
        /// </summary>
        public void AddData(List<string> files)
        {
            foreach (var item in files.Select((value, index) => new { value, index }))
            {
                this.Items.AddOnScheduler(new KgInfo(item.value, item.index + 1));
            }
        }
    }
}
