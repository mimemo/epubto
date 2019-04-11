using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Reactive.Bindings;

namespace epubto.Models
{
    public class AceLog : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ReactiveProperty<string> State { get; set; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> Body { get; set; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> Rawtxt { get; set; } = new ReactiveProperty<string>();

        public AceLog(string raw)
        {
            this.Rawtxt.Value = raw;

            this.State.Value = this.DivideLog(raw).Item1;
            this.Body.Value = this.DivideLog(raw).Item2;
        }

        private readonly static Regex rgxColor = new Regex("\\x1b\\[[0-9]{1,2}m");


        private Tuple<string, string> DivideLog(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return new Tuple<string, string>(string.Empty, string.Empty);
            }

            string[] arry = raw.Split(new[] { ": " }, StringSplitOptions.None);

            // 区切り文字ない場合はそのまま
            if (arry.Length == 1)
            {
                return new Tuple<string, string>(string.Empty, raw.Trim());
            }

            var inf = arry[0].Trim();
            var msg = arry[1].Trim();

            inf = rgxColor.Replace(inf, string.Empty);
            inf = inf.Trim();

            return new Tuple<string, string>(inf, msg);
        }


    }
}
