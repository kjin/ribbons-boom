using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLContent
{
    public class TelescopingBlock
    {
        public int ribbonID;
        public int ribbonPosition;
        public int height;
        public float rotation;
        public bool flipped;

        public TelescopingBlock(int ribbonID, int ribbonPosition, int height, float rotation, bool flipped)
        {
            this.ribbonID = ribbonID;
            this.ribbonPosition = ribbonPosition;
            this.height = height;
            this.rotation = rotation;
            this.flipped = flipped;
        }

        public TelescopingBlock() { }
    }
}
