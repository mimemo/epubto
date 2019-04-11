using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
using epubto.ViewModels;

namespace epubto.Views
{
    /// <summary>
    /// AceByDaisyUserControl.xaml の相互作用ロジック
    /// </summary>
    public partial class AceByDaisyUserControl : UserControl
    {
        public AceViewModel Vm = new AceViewModel();

        public AceByDaisyUserControl()
        {

            this.InitializeComponent();


            // TODO:Windows 10 Edgeベースにすべきか？
            // javascript 抑制
            var axIWebBrowser2 = typeof(WebBrowser).GetProperty("AxIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            var comObj = axIWebBrowser2.GetValue(this.MyBrowserView, null);
            comObj.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, comObj, new object[] { true });

            this.DataContext = this.Vm;
        }

        /// <summary>
        /// ファイルをドロップ
        /// </summary>
        private void ListBoxDrop(object sender, DragEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AceUtil.Version))
            {
                var a = LocalUtil.IsJapan ?
                   MessageBox.Show("Ace, by Daisy があなたのPCにインストールされていません。Ace, by Daisy環境を整えてください.", "Ace", MessageBoxButton.OK, MessageBoxImage.Warning) :
                   MessageBox.Show("Ace, check is not available. Ace(by Daisy) not found on your PC.  ", "Ace by Daisy", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // リストをクリア
            this.Vm.ClearData();

            // ドロップファイル一覧の取得
            var dropfiles = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            // 拡張子EPUBファイル一覧の取得
            string[] epubfiles = FileCollect.CollectEpubFiles(dropfiles);

            if (epubfiles == null || epubfiles.Length <= 0)
            {
                var a = LocalUtil.IsJapan ?
                    MessageBox.Show("EPUBファイルが見つかりませんでした", "EPUBファイル数チェック", MessageBoxButton.OK, MessageBoxImage.Warning) :
                    MessageBox.Show("EPUB file not found. Drop [xxx.epub] file. ", "EPUB file check", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            this.Vm.ItemCount = epubfiles.Length;

            // パスでソートしてから登録
            var sortedfiles = (from item in epubfiles
                               orderby item
                               select item).ToList();
            this.Vm.AddData(sortedfiles);
        }

        /// <summary>
        /// マウスイベントで選択を切替
        /// </summary>
        private void ItemOnPreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ((ListBoxItem)sender).IsSelected = true;
        }

        /// <summary>
        /// ブラウザで開く
        /// </summary>
        private void OpneReportWithBrowserButtonClick(object sender, RoutedEventArgs e)
        {
            this.Vm.SelectedItem.Value?.OpneReportWithBrowserCommand.Execute();
        }

    }
}
