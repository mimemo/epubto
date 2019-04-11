using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using epubto.Common;
using epubto.Properties;
using epubto.ViewModels;
using Reactive.Bindings;
using Reactive.Bindings.Notifiers;

namespace epubto.Views
{
    /// <summary>
    /// KindlegenUserControl.xaml の相互作用ロジック
    /// </summary>
    public partial class KindlegenUserControl : UserControl
    {
        /// <summary>
        /// View内のタブ的なやつ
        /// </summary>
        public ReactiveProperty<int> InViewSelectedIndex { get; set; } = new ReactiveProperty<int>(0);
        public BooleanNotifier InViewIsSelected0 { get; set; } = new BooleanNotifier(true);
        public BooleanNotifier InViewIsSelected1 { get; set; } = new BooleanNotifier(false);
        public BooleanNotifier InViewIsSelected2 { get; set; } = new BooleanNotifier(false);


        /// <summary>
        /// パス表示
        /// </summary>
        public ReactiveProperty<string> KgExePath { get; set; } = new ReactiveProperty<string>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public KindlegenUserControl()
        {
            this.InitializeComponent();
            this.DataContext = this;

            // 上部タブ的メニュー
            this.InViewIsSelected0.Where(x => x).Subscribe(_ => this.InViewSelectedIndex.Value = 0);
            this.InViewIsSelected1.Where(x => x).Subscribe(_ => this.InViewSelectedIndex.Value = 1);
            this.InViewIsSelected2.Where(x => x).Subscribe(_ => this.InViewSelectedIndex.Value = 2);

            this.KindleGenLink.NavigateUri = new Uri(@"https://www.amazon.com/gp/feature.html?ie=UTF8&docId=1000765211");
        }

        #region Settings

        /// <summary>
        /// The main window on loaded.
        /// </summary>
        private void KindlegenUserControlOnLoaded(object sender, RoutedEventArgs e)
        {
            this.SettingLoad();
        }

        /// <summary>
        /// upgrade はMainViewで実施済。ここでは行わない
        /// </summary>
        private void SettingLoad()
        {
            // ファイルが存在すれば設定
            var kepath = Settings.Default.KindlegenExePath;
            this.KgExePath.Value = System.IO.File.Exists(kepath) ? kepath : string.Empty;
        }

        /// <summary>
        /// save setting.
        /// </summary>
        private void SettingSave()
        {
            Settings.Default.KindlegenExePath = this.KgExePath.Value;

            // アプリケーションの設定を保存する
            Settings.Default.Save();
        }

        #endregion

        /// <summary>
        /// kindlegen.exe をドロップ
        /// </summary>
        private void KgPathTextBoxDrop(object sender, DragEventArgs e)
        {
            // ドロップファイル一覧の取得
            var dropfiles = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            // 拡張子exeファイル一覧の取得
            var exefiles = dropfiles.Where(x => string.Equals(System.IO.Path.GetFileName(x), "kindlegen.exe", StringComparison.CurrentCultureIgnoreCase)).ToArray();

            if (exefiles.Any() == false)
            {
                var a = LocalUtil.IsJapan ?
                    MessageBox.Show("kindlegen.exe をドロップしてください", "EPUBファイル数チェック", MessageBoxButton.OK, MessageBoxImage.Warning) :
                    MessageBox.Show("Drop [kindlegen.exe]  file ", "EPUB file check", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            this.KgExePath.Value = exefiles.First();
            this.SettingSave();
        }

        /// <summary>
        /// ドロップ許可
        /// </summary>
        private void KgPathTextBoxPreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Link;
            e.Handled = true;
        }


        /// <summary>
        /// 外部ページへ
        /// </summary>
        private void KindleGenLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }
    }
}
