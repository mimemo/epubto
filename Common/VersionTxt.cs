using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace epubto.Common
{
    public static class VersionTxt
    {
        /// <summary>
        /// （主にタイトルバー用）にアプリ名とバージョン を文字列として返す
        /// </summary>
        public static string MakeTextNmaeAndVersion()
        {
            return GetAssemblyTitle() + "  ver " + GetVersionText();
        }


        /// <summary>
        /// タイトル取得
        /// </summary>
        private static string GetAssemblyTitle()
        {
            // AssemblyTitleの取得
            System.Reflection.AssemblyTitleAttribute asmttl = (System.Reflection.AssemblyTitleAttribute)Attribute.GetCustomAttribute(System.Reflection.Assembly.GetExecutingAssembly(), typeof(System.Reflection.AssemblyTitleAttribute));

            return asmttl.Title;
        }

        /// <summary>
        /// バージョン情報取得
        /// </summary>
        public static string GetVersionText(int iLevel = 0)
        {
            //自分自身のAssemblyを取得
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();

            //バージョンの取得
            System.Version ver = asm.GetName().Version;

            // 引数によって表記を切り替える。
            // 例：バージョン ３．２．１の場合
            // 引数１の時：「３」
            // 引数２の時：「３．２」
            // 引数３の時：「３．２．１」
            // 引数上記以外か省略時「３．２．１」

            if (iLevel == 1)
            {
                return ver.Major.ToString();
            }

            if (iLevel == 2)
            {
                return ver.Major + "." + ver.Minor;
            }

            if (iLevel == 3)
            {
                return ver.Major + "." + ver.Minor + "." + ver.Build;
            }

            if (iLevel == 4)
            {
                return ver.Major + "." + ver.Minor + "." + ver.Build + "." + ver.Revision; ;
            }

            return ver.Major + "." + ver.Minor + "." + ver.Build;
        }


    }
}
