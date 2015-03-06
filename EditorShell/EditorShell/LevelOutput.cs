using CodeLibrary.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLContent;

namespace EditorShell
{
    public static class LevelOutput
    {
        public static Level CreateLevel(Vector2 gameGrid, SeamstressObject seamstress, List<RibbonPointStoreObject> ribbonStores, List<RibbonObject> ribbons, GameObject[,] gridModel, GroundObject ground, MiasmaObject miasma){
            List<RibbonElementStore> ribbonBoxes = new List<RibbonElementStore>();
            List<RibbonElementStore> hooks = new List<RibbonElementStore>();
            List<RibbonElementStore> ribbonShooters = new List<RibbonElementStore>();
            List<RibbonElementStore> telescoping = new List<RibbonElementStore>();
            List<SaveRockObject> saveRocks = new List<SaveRockObject>();
            
            foreach (GameObject o in gridModel)
            {
                if (o != null)
                {
                    if (o is RibbonElementStore)
                    {
                        RibbonElementStore curr = o as RibbonElementStore;
                        if (curr.Type == "box")
                        {
                            ribbonBoxes.Add(curr);
                        }
                        if (curr.Type == "hook")
                        {
                            hooks.Add(curr);
                        }
                        if(curr.Type == "shooter")
                        {
                            ribbonShooters.Add(curr);
                        }
                        if (curr.Type == "telescoping")
                        {
                            telescoping.Add(curr);
                        }
                    }
                    if (o is SaveRockObject)
                    {
                        SaveRockObject curr = o as SaveRockObject;
                        saveRocks.Add(curr);
                    }
                }
            }
            Seamstress s = CreateSeamstress(seamstress);
            List<Ground> g = CreateGround(ground);
            List<Miasma> m = CreateMiasma(miasma);
            List<Ribbon> r = CreateRibbons(ribbonStores, ribbons);
            List<RibbonBox> rb = CreateRibbonBoxes(ribbonBoxes);
            List<Shooter> sh = CreateShooters(ribbonShooters);
            List<Needle> n = CreateNeedles();
            List<Hook> h = CreateHooks(hooks);
            List<TelescopingBlock> tb = CreateTelescopingBlocks(telescoping);
            List<Flipbar> f = CreateFlipBars();
            List<RotatingPlatform> rp = CreateRotatingPlatforms();
            List<SaveRock> sr = CreateSaveRocks(saveRocks);
            List<Decoration> d = CreateDecorations();
            List<Collectable> c = CreateCollectables();
            List<Gem> gems = CreateGems();
            List<Hotspot> cam = CreateHotspots();

            Level l = new Level(1, gameGrid, cam, s, g, m, r, rb, sh, n, h, tb, f, rp, sr, d, c, gems);
            return l;
        }

        private static List<Hotspot> CreateHotspots()
        {
            List<Hotspot> output = new List<Hotspot>();
            return output;
        }

        private static List<Gem> CreateGems()
        {
            List<Gem> output = new List<Gem>();
            return output;
        }

        private static List<Decoration> CreateDecorations()
        {
            List<Decoration> output = new List<Decoration>();
            return output;
        }

        private static List<Collectable> CreateCollectables()
        {
            List<Collectable> output = new List<Collectable>();
            return output;
        }

        private static List<SaveRock> CreateSaveRocks(List<SaveRockObject> saveRocks)
        {
            List<SaveRock> output = new List<SaveRock>();
            foreach (SaveRockObject save in saveRocks)
            {
                output.Add(new SaveRock(save.body.Position, save.endFlag));
            }
            return output;
        }

        private static List<RotatingPlatform> CreateRotatingPlatforms()
        {
            List<RotatingPlatform> output = new List<RotatingPlatform>();
            return output;
        }

        private static List<Flipbar> CreateFlipBars()
        {
            List<Flipbar> output = new List<Flipbar>();
            return output;
        }

        private static List<TelescopingBlock> CreateTelescopingBlocks(List<RibbonElementStore> telescoping)
        {
            List<TelescopingBlock> output = new List<TelescopingBlock>();
            foreach (RibbonElementStore res in telescoping)
            {
                if (res.RibbonFlipped)
                    output.Add(new TelescopingBlock(res.RibbonID, (int)Math.Round(res.RibbonPos) - 1, 1, 0, res.RibbonFlipped));
                else
                    output.Add(new TelescopingBlock(res.RibbonID, (int)Math.Round(res.RibbonPos), 1, 0, res.RibbonFlipped));
            }
            return output;
        }

        private static List<Hook> CreateHooks(List<RibbonElementStore> hooks)
        {
            List<Hook> output = new List<Hook>();
            foreach (RibbonElementStore res in hooks)
            {
                if (res.RibbonFlipped)
                    output.Add(new Hook(res.RibbonID, (int)Math.Round(res.RibbonPos) - 1, new Vector2(1, 1), res.RibbonFlipped));
                else
                    output.Add(new Hook(res.RibbonID, (int)Math.Round(res.RibbonPos), new Vector2(1, 1), res.RibbonFlipped));
            }
            return output;
        }

        private static List<Needle> CreateNeedles()
        {
            List<Needle> output = new List<Needle>();
            return output;
        }

        private static List<Shooter> CreateShooters(List<RibbonElementStore> ribbonShooters)
        {
            List<Shooter> output = new List<Shooter>();
            foreach (RibbonElementStore res in ribbonShooters)
            {
                if (res.RibbonFlipped)
                    output.Add(new Shooter(res.RibbonID, res.Position, (int)Math.Round(res.RibbonPos) - 1, 5,0, res.RibbonFlipped));
                else
                    output.Add(new Shooter(res.RibbonID, res.Position, (int)Math.Round(res.RibbonPos), 5, 0, res.RibbonFlipped));
            }
            return output;
        }

        private static List<RibbonBox> CreateRibbonBoxes(List<RibbonElementStore> ribbonBoxes)
        {
            List<RibbonBox> output = new List<RibbonBox>();
            foreach (RibbonElementStore res in ribbonBoxes)
            {
                if (res.RibbonFlipped)
                    output.Add(new RibbonBox(res.RibbonID, res.RibbonPos - 1, new Vector2(1, 1), 0f, res.RibbonFlipped));
                else
                    output.Add(new RibbonBox(res.RibbonID, res.RibbonPos, new Vector2(1, 1), 0f, res.RibbonFlipped));
            }
            return output;
        }
        private static Seamstress CreateSeamstress(SeamstressObject seamstress)
        {
            Vector2 position = seamstress.body.Position;
            Seamstress s = new Seamstress(position);
            return s;
        }

        private static List<Ground> CreateGround(GroundObject ground)
        {
            List<Ground> output = new List<Ground>();
            if (ground != null)
            {
                List<Rectangle> gl = ground.rectangles;
                foreach (Rectangle r in gl)
                {
                    output.Add(new Ground(new Vector2(r.X, r.Y), new Vector2(r.Width, r.Height)));
                }
            }
            return output;
        }

        private static List<Miasma> CreateMiasma(MiasmaObject miasma)
        {
            List<Miasma> output = new List<Miasma>();
            if (miasma != null)
            {
                List<Rectangle> ml = miasma.rectangles;
                foreach (Rectangle r in ml)
                {
                    output.Add(new Miasma(new Vector2(r.X, r.Y), new Vector2(r.Width, r.Height)));
                }
            }
            return output;
        }

        //implement colors later
        private static List<Ribbon> CreateRibbons(List<RibbonPointStoreObject> ribbonStores, List<RibbonObject> ribbons)
        {
            List<Ribbon> output = new List<Ribbon>();
            for (int i = 0; i < ribbonStores.Count; i++)
            {
                RibbonPointStoreObject rps = ribbonStores[i];
                RibbonObject r = ribbons[i];
                output.Add(new Ribbon(rps.createPath(), r.PointToPosition(rps.RibbonStart.Point), r.PointToPosition(rps.RibbonEnd.Point), r.loop, 0));
            }
            return output;
        }
    }
}
