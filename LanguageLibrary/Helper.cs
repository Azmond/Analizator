using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LanguageLibrary
{
    static class Helper
    {
        public static bool IsAnyOf(this char ch, char[] mas)
        {
            return mas.Any(p => p == ch);
        }

        public static bool IsAnyOf(this LexemKind k, LexemKind[] mas)
        {
            return mas.Any(p => p == k);
        }


    }
}
