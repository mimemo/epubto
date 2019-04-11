using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace epubto.Common
{
    public class LocalUtil
    {
        static LocalUtil()
        {

            var currentCulTxt = CultureInfo.CurrentCulture.ToString();
            IsJapan = currentCulTxt.Contains("ja-JP");

        }

        public static bool IsJapan { get; private set; }
    }

}
