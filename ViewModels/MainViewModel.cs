// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   メイン処理クラス
// </summary>
// --------------------------------------------------------------------------------------------------------------------
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
    using System.Diagnostics;
    using epubto.Services;
    using System.Windows.Media;
    using System.Windows.Data;
    using System.ComponentModel;

    /// <summary>メイン処理クラス </summary>
    public class MainViewModel
    {
        /// <summary>コンストラクタ Initializes a new instance of the <see cref="MainViewModel"/> class. </summary>
        public MainViewModel()
        {
            this.KindlegenMenuCommand.Subscribe(_ => this.ChangeMode(0));
            this.AceByDaisyMenuCommand.Subscribe(_ => this.ChangeMode(1));
            this.AboutMenuCommand.Subscribe(_ => this.ChangeMode(2));
            this.CurrentViewTitle = this.CurrentViewIndex.Select(x => this.IndexToTitle(x)).ToReactiveProperty();

            // wpf-Webbrowserが最前面にきちゃう問題回避
            this.CurrentViewIndex.Subscribe(x =>
            {

                if (x == 1)
                {
                    AceViewModel.IsShowBrowser.TurnOn();
                }
                else
                {
                    AceViewModel.IsShowBrowser.TurnOff();
                }
            });
        }

        public ReactiveProperty<string> CurrentViewTitle { get; set; } = new ReactiveProperty<string>(string.Empty);

        private string IndexToTitle(int x)
        {
            switch (x)
            {
                case 0:
                    {
                        return "kindlegen";
                    }
                case 1:
                    {
                        return "Ace by DAISY";
                    }
                case 2:
                    {
                        return "About";
                    }
                default:
                    {
                        return "Unknown";
                    }
            }

        }



        /// <summary>
        /// 選択中の画面
        /// </summary>
        public ReactiveProperty<int> CurrentViewIndex { get; set; } = new ReactiveProperty<int>(0);

        /// <summary>
        /// 表示更新
        /// </summary>
        /// <param name="v">モード</param>
        private void ChangeMode(int mode)
        {
            this.CurrentViewIndex.Value = mode;
            this.SideMenuIsOpen.TurnOff();
        }


        /// <summary>
        /// 選択中のViewインデックス
        /// </summary>
        public ReactiveProperty<int> SelectedIndex { get; set; } = new ReactiveProperty<int>(0);



        public BooleanNotifier SideMenuIsOpen { get; set; } = new BooleanNotifier(false);



        /// <summary> The change message command.</summary>
        public ReactiveCommand KindlegenMenuCommand { get; set; } = new ReactiveCommand();
        public ReactiveCommand AceByDaisyMenuCommand { get; set; } = new ReactiveCommand();
        public ReactiveCommand AboutMenuCommand { get; set; } = new ReactiveCommand();




    }
}