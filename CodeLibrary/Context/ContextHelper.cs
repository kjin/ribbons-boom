using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CodeLibrary.Graphics;

namespace CodeLibrary.Context
{
    public static class ContextHelper
    {
        public static MultiOptionSelector BuildMultiOptionSelector(Canvas canvas, string theme, string selectorName, List<Option> options, Cursor cursor, int initialValue = 0)
        {
            return BuildMultiOptionSelector(canvas, theme, selectorName, options, MultiOptionArrangement.ListY, cursor, initialValue);
        }

        public static MultiOptionSelector BuildMultiOptionSelector(Canvas canvas, string theme, string selectorName, List<Option> options, MultiOptionArrangement arrangement, Cursor cursor, int initialValue = 0)
        {
            string selectorTheme = canvas.AssetDictionary.LookupString(theme, selectorName + "SelectorTheme");
            Vector2 selectorPosition = canvas.AssetDictionary.LookupVector2(theme, selectorName + "SelectorPosition");
            selectorPosition *= GraphicsConstants.VIEWPORT_DIMENSIONS / GraphicsConstants.DEFAULT_DIMENSIONS;
            Anchor selectorAnchor = Anchor.Center;
            if (canvas.AssetDictionary.CheckPropertyExists(theme, selectorName + "SelectorAnchor"))
                Enum.TryParse<Anchor>(canvas.AssetDictionary.LookupString(theme, selectorName + "SelectorAnchor"), out selectorAnchor);
            return new MultiOptionSelector(canvas, selectorTheme, selectorPosition, selectorAnchor, options, arrangement, cursor, initialValue);
        }
    }
}
