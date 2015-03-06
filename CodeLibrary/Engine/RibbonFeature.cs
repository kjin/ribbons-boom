using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeLibrary.Graphics;

namespace CodeLibrary.Engine
{
    public interface RibbonFeature
    {

        void Move(float dx, float dt);
        void Draw(Canvas c);
    }
}
