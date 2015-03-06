using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Xml.Serialization;

namespace XMLContent
{
    [Serializable()]
    public class Level
    {
        public int levelNum;
        public Seamstress seamstress;
        public List<Ground> ground;
        public List<Miasma> miasma;
        public Vector2 gameGrid;
        public List<Ribbon> ribbons;
        public List<Hotspot> cameras;
        public List<RibbonBox> ribbonBoxes;
        public List<Shooter> shooters;
        public List<Needle> needles;
        public List<Hook> hooks;
        public List<TelescopingBlock> telescopingBlocks;
        public List<Flipbar> flipbars;
        public List<RotatingPlatform> rotatingPlatforms;
        public List<SaveRock> saveRocks;
        public List<Decoration> decorations;
        public List<Collectable> collectables;
        public List<Gem> gems;

        public Level(int levelNum, Vector2 gameGrid, List<Hotspot> cameras, Seamstress seamstress, List<Ground> ground, List<Miasma> miasma,
             List<Ribbon> ribbons, List<RibbonBox> ribbonBoxes, List<Shooter>shooters, List<Needle> needles,
            List<Hook> hooks, List<TelescopingBlock> telescopingBlocks, List<Flipbar> flipbars, List<RotatingPlatform> rotatingPlatforms,
            List<SaveRock> saveRocks, List<Decoration> decorations, List<Collectable> collectables, List<Gem> gems)
        {
            this.levelNum = levelNum;
            this.seamstress = seamstress;
            this.ground = ground;
            this.miasma = miasma;
            this.gameGrid = gameGrid;
            this.ribbons = ribbons;
            this.cameras = cameras;
            this.ribbonBoxes = ribbonBoxes;
            this.shooters = shooters;
            this.needles = needles;
            this.hooks = hooks;
            this.telescopingBlocks = telescopingBlocks;
            this.flipbars = flipbars;
            this.rotatingPlatforms = rotatingPlatforms;
            this.saveRocks = saveRocks;
            this.decorations = decorations;
            this.collectables = collectables;
            this.gems = gems;
        }

        public Level() { }
    }
}
