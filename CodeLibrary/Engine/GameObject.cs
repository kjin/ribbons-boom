using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeLibrary.Graphics;

namespace CodeLibrary.Engine
{
    public interface GameObject
    {
        void Update(float dt);

        void Draw(Canvas c);

    }
}
