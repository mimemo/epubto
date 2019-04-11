using epubto.Common;
using epubto.Properties;
using epubto.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace epubto.Views
{
    /// <summary>
    /// KindlegenView.xaml の相互作用ロジック
    /// </summary>
    public partial class KindlegenView : UserControl
    {
        public KindlegenViewModel Vm { get; set; } = new KindlegenViewModel();

        public KindlegenView()
        {
            this.InitializeComponent();
            this.DataContext = this.Vm;
        }


        /// <summary>
        /// Kindlegen Drop
        /// </summary>
        private void KindlegenDataGridDrop(object sender, DragEventArgs e)
        {
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
                    MessageBox.Show("EPUB file not found. Drop the file with the extension [.epub] ", "EPUB file check", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (System.IO.File.Exists(Settings.Default.KindlegenExePath) == false)
            {
                var msg = LocalUtil.IsJapan ?
                    "Kindlegen.exe が設定されていません。kindlegen をダウンロード＆解凍し kindlegen.exe を指定してください。" :
                    "Kindlegen.exe is not set. Download and unzip kindlegen and specify kindlegen.exe.";

                MessageBox.Show(msg, "kindlegen check", MessageBoxButton.OK, MessageBoxImage.Question);
                return;
            }

            // パスでソート
            var sortedfiles = (from item in epubfiles
                               orderby item
                               select item).ToList();

            this.Vm.ItemCount = sortedfiles.Count;

            this.Vm.AddData(sortedfiles);

        }


    }
}
