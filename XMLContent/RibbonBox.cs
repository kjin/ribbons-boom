using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLContent
{
    public class RibbonBox
    {
        public int ribbonID;
        public float position;
        public Vector2 dimensions;
        public float rotation;
        public bool flipped;

        public RibbonBox(int ribbonID, float position, Vector2 dimensions, float rotation, bool flipped)
        {
            this.ribbonID = ribbonID;
            this.position = position;
            this.dimensions = dimensions;
            this.rotation = rotation;
            this.flipped = flipped;
        }

        public RibbonBox() { }
    }
}
