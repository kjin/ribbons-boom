using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLContent
{
    public class SaveRock
    {
        public Vector2 position;
        public bool endFlag;

        public SaveRock(Vector2 position, bool endFlag)
        {
            this.position = position;
            this.endFlag = endFlag;
        }

        public SaveRock() { }
    }
}
