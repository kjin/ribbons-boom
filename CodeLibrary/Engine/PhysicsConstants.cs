using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision.Shapes;

namespace CodeLibrary.Engine
{
    public class PhysicsConstants
    {
        public const float OBJECT_EPSILON = 0.02f;
        public const float GROUND_EPSILON_LARGE = 0.3f;
        public const float GROUND_EPSILON_SMALL = 0.01f;
        public const float MIASMA_EPSILON = 0.3f;
        public const float SPIKES_EPSILON = 0.3f;

        //seamstress parameters
        public const float SEAM_EPSILON = 0.02f;
        public const float SEAM_SLANT = 0.06f; //how triangular seamstress is shaped (lesser values = more rectangular, 0.0f = rectangle)
        public const float SEAM_WIDTH = 0.6f;

        //seamstress movement
        public const float SEAM_GROUNDSPEED = 1.0f;
        public const float SEAM_AIRSPEED = 0.1f;
        public const float SEAM_PRIMARY_JUMPFORCE = -10.4f;
        public const float SEAM_SECONDARY_JUMPFORCE = -1.4f;

        // for desired air properties
        public const bool SEAMSTRESS_BIDIRECTIONALDRAG = true; //if false, uses horizontal airdrag
        public const float SEAMSTRESS_HORIZONTAL_AIRDRAG = 0.02f;
        public const float SEAMSTRESS_VERTICAL_AIRDRAG = 0.015f;

        public const float GRAVITY = 16.0f;

        public const float RIBBON_ELEMENT_SENSOR = 0.1f;

        // for "moves to next integer"
        public const bool RIBBON_DISCRETE_ENABLE = false;
        public const float RIBBON_DISCRETE_THRESHOLD = 0.05f;
        public const float RIBBON_STATIC_DRAG = 1.0f;

        // for the desired "moves to closest integer"
        public const bool RIBBON_PULL_ENABLE = true;
        public const float RIBBON_PULL_POWER = 0.005f;

        public const Category GROUND_CATEGORY = Category.Cat2;
        public const Category COLLISIONGROUND_CATEGORY = Category.Cat3;
        public const Category SEAMSTRESS_CATEGORY = Category.Cat4;

    }
}