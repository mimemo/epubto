
namespace epubto.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;
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
    using System.Xml;

    /// <summary>
    /// AboutUserControl.xaml の相互作用ロジック
    /// </summary>
    public partial class AboutUserControl : UserControl
    {
        public AboutUserControl()
        {
            this.InitializeComponent();

            this.Initialize();
        }

        private void Initialize()
        {
            Assembly asm = Assembly.GetExecutingAssembly(); // 実行中のアセンブリを取得する。

            // AssemblyNameから取得
            AssemblyName asmName = asm.GetName();
            string version = "Version : " + asmName.Version + "\r\n";

            // CustomAttributeから取得
            object[] productarray = asm.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            string product = "AppName : " + ((AssemblyProductAttribute)productarray[0]).Product + "\r\n";

            this.Appinfo.Text = string.Join("\r\n", new[] { product, version });

            var c = Thread.CurrentThread.CurrentCulture;
            var cui = Thread.CurrentThread.CurrentUICulture;
            var isJp = c.Name == "ja-JP" && cui.Name == "ja-JP";

            string tx = string.Empty;
            if (isJp)
            {
                tx += "\r\n ## 2019/04/12 versino 1.0.3";
                tx += "\r\n  初期版";
                tx += "\r\n";
            }
            else
            {
                tx += "\r\n ## 2019/04/12 versino 1.0.3";
                tx += "\r\n  Initial release";
                tx += "\r\n";
            }

            this.historyinfo.Text = tx;

        }

        /// <summary>
        /// ブラウザで開く
        /// </summary>
        private void HyperlinkRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }
    }
}
