using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CodeLibrary.Storage
{
    public static class StorageHelper
    {
        public static int GetActNumber(string s)
        {
            return Convert.ToInt32(s.Substring(5).Substring(0, s.LastIndexOf('_')-5));
        }

        public static int GetLevelNumber(string s)
        {
            return Convert.ToInt32(s.Substring(s.LastIndexOf('_') + 1));
        }

        public static string Intellisplit(string s, int maxLineLength)
        {
            if (s.Length <= maxLineLength) return s;
            int index = s.IndexOf(' ', s.Length / 2);
            return s.Substring(0, index) + "\n" + s.Substring(index + 1);
        }
    }
}
