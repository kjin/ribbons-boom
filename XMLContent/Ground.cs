using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLContent
{
    public class Ground
    {
        public Vector2 position;
        public Vector2 dimensions;

        public Ground(Vector2 position, Vector2 dimensions)
        {
            this.position = position;
            this.dimensions = dimensions;
        }

        public Ground() { }
    }
}
