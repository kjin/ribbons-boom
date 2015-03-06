using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLContent
{
    public class Shooter
    {
        // Need to add the case where it's not on the ribbon
        public int ribbonID;
        public Vector2 position;
        public float ribbonPosition;
        public float frequency;
        public float rotation;
        public bool flipped;

        public Shooter(int ribbonID, Vector2 position, float ribbonPosition, float frequency, float rotation, bool flipped)
        {
            this.ribbonID = ribbonID;
            this.position = position;
            this.ribbonPosition = ribbonPosition;
            this.frequency = frequency;
            this.rotation = rotation;
            this.flipped = flipped;
        }

        public Shooter() { }
    }
}
