using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XMLContent
{
    public class Ribbon
    {
        public List<Vector2> path;
        public float start;
        public float end;
        public bool loop;
        public int color;

        public Ribbon(List<Vector2> path, float start, float end, bool loop, int color)
        {
            this.path = path;
            this.start = start;
            this.end = end;
            this.loop = loop;
            this.color = color;
        }

        public Ribbon() { }
    }
}
