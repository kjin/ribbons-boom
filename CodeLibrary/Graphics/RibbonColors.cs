using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CodeLibrary.Graphics
{
    public static class RibbonColors
    {
        static Color[] colors = new Color[] {
            Color.Crimson,
            Color.Chocolate,
            Color.DarkGreen,
            Color.DarkSlateBlue
        };

        static Color[] spoolColors = new Color[]{
            Color.Crimson,
            Color.Chocolate,
            Color.DarkGreen,
            Color.DarkSlateBlue
        };

        public static Color GetColor(int i) { return colors[i % colors.Length]; }

        public static Color GetSpoolColor(int i) { return spoolColors[i % spoolColors.Length];  }
    }
}
