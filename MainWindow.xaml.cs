
namespace epubto
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;
    using epubto.Common;
    using epubto.Properties;
    using epubto.ViewModels;

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel Vm = new MainViewModel();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.DataContext = this.Vm;


            // タイトル
            this.Title = VersionTxt.MakeTextNmaeAndVersion();
        }

        #region Settings

        /// <summary>
        /// The main window on closing.
        /// </summary>
        private void MainWindowOnClosing(object sender, CancelEventArgs e)
        {
            this.SettingSave();
        }

        /// <summary>
        /// The main window on loaded.
        /// </summary>
        private void MainWindowOnLoaded(object sender, RoutedEventArgs e)
        {
            this.SettingLoad();
        }

        /// <summary>
        /// load setting.
        /// </summary>
        private void SettingLoad()
        {
            // 前バージョンからのUpgradeを実行していないときは、Upgradeを実施する
            if (Settings.Default.IsUpgrade == false)
            {
                // Upgradeを実行する
                Settings.Default.Upgrade();

                // 「Upgradeを実行した」という情報を設定する
                Settings.Default.IsUpgrade = true;

                // 現行バージョンの設定を保存する
                Settings.Default.Save();
            }

        }

        /// <summary>
        /// save setting.
        /// </summary>
        private void SettingSave()
        {
            // アプリケーションの設定を保存する
            Settings.Default.Save();
        }

        #endregion

    }
}
