using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLContent
{
    public class Hook
    {
        public int ribbonID;
        public int ribbonPosition;
        public Vector2 dimensions;
        public bool flipped;

        public Hook(int ribbonID, int ribbonPosition, Vector2 dimensions, bool flipped)
        {
            this.ribbonID = ribbonID;
            this.ribbonPosition = ribbonPosition;
            this.dimensions = dimensions;
            this.flipped = flipped;
        }

        public Hook() { }
    }
}
