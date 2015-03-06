using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FarseerPhysics.Dynamics;

namespace CodeLibrary.Engine
{
    class TrampolineBox : RibbonElement
    {
        public TrampolineBox(World w, RibbonObject r, PhysicalObject b, float pos) : base(w, r, b, pos) { }
    }
}
