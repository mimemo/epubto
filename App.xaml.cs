using epubto.Common;
using epubto.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace epubto
{
    public partial class App : Application
    {

        public App()
        {
            var ss = new SplashScreenView();
            this.MainWindow = ss;
            ss.Show();

            Task.Factory.StartNew(() =>
            {
                // static 呼び出し用(Ace実体を探し中・・・)
                var a = AceUtil.NodeVersion;
                Debug.WriteLine(a);

                // UI thread
                this.Dispatcher.Invoke(() =>
                {
                    // メイン起動しスイッチ
                    this.MainWindow = new MainWindow();
                    this.MainWindow.Show();
                    ss.Close();
                });
            });
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }
    }

}
