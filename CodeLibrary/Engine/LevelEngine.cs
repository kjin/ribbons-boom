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
using CodeLibrary.Content;
using CodeLibrary.Graphics;
using CodeLibrary.Audio;
using CodeLibrary.Input;
using CodeLibrary.Context;
using CodeLibrary.Storage;

using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using XMLContent;

namespace CodeLibrary.Engine
{
    public class LevelEngine
    {

        #region Constants

        const float GRAVITY = PhysicsConstants.GRAVITY;

        #endregion

        #region Fields
        ParallaxBackgroundSet backgrounds;

        //SeamstressSprites seamstress;

        int actNumber;
        //Important objects
        SeamstressObject seamstress;
        //RibbonObject ribbon;
        List<GameObject> objects;
        //Direct references to miasma/ground for 3D
        MiasmaObject miasma;
        GroundObject ground;
        List<RibbonObject> ribbons;
        List<RibbonFeature> dynamics;

        List<DecorationObject> decorations;

        //Force controllers
        SeamstressForceController seamstressController;
        RibbonForceController ribbonController;

        //Farseer physics world
        World world;

        //Collision controller
        CollisionController collisionController;

        //Camera Controller
        CameraController cameraController;

        InputController inputController;
        Canvas canvas;
        AudioPlayer audioPlayer;
        bool dispose;

        LevelInfo levelInfo;

        LevelGridStore[,] gridModel;

        #endregion

        #region Properties
        public bool Win { get { return seamstress.win; } }
        public bool Lose { get { return !seamstress.alive; } }
        public DeathMethods DeathMethod { get { return seamstress.DeathMethod; } }
        public int SpawnID { get { return seamstress.spawnID; } }
        #endregion

        public LevelEngine(LevelInfo lI, InputController inputController, Canvas canvas, AudioPlayer audioPlayer)
        {
            this.inputController = inputController;
            this.canvas = canvas;
            this.audioPlayer = audioPlayer;

            //canvas.Camera.EnableBounds(new Vector2(0, -20), new Vector2(50, 40), -5)
            world = new World(new Vector2(0, GRAVITY));
            
            levelInfo = lI;

            // load level
            bool gc = inputController.ControllerConnected;
            LevelGenerator levelGenerator = new LevelGenerator(levelInfo, canvas, audioPlayer, world, gc);
            gridModel = levelGenerator.GenerateGridModel();
            Console.WriteLine(gridModel.GetLength(0) + " " + gridModel.GetLength(1));
            backgrounds = levelGenerator.Background;
            //levelGenerator.printLevelData();

            // put level info in physics engine
            seamstress = levelGenerator.Seamstress;
            //ribbon = levelGenerator.Ribbon;
            objects = levelGenerator.Objects;
            objects.Add(new CameraBoundsObject(canvas.Camera, world));
            miasma = levelGenerator.Miasma;
            ground = levelGenerator.Ground;
            ribbons = levelGenerator.Ribbons;
            decorations = levelGenerator.Decorations;
            dynamics = levelGenerator.Dynamics;
            actNumber = levelGenerator.ActNumber;

            //hack to get audio into ribbonelements
            foreach (RibbonObject r in ribbons)
                r.SetRibbonSounds(audioPlayer);

            backgrounds = levelGenerator.Background;

            collisionController = new CollisionController(world, seamstress, objects);

            canvas.Camera.Position = seamstress.body.Position;
            canvas.Camera.JumpToTarget();
            // create controllers
            cameraController = new CameraController(canvas.Camera, levelGenerator.CameraHotspots, inputController, audioPlayer);
            ribbonController = new RibbonForceController(seamstress, inputController);
            world.AddController(ribbonController);
            seamstressController = new SeamstressForceController(seamstress, inputController);
            world.AddController(seamstressController);
            dispose = false;
            miasma.GridModel = gridModel;
            levelInfo.CurrentTime = levelInfo.SaveTime;
        }

        public LevelEngine(Level level, LevelInfo levelInfo, InputController inputController, Canvas canvas, AudioPlayer audioPlayer)
        {
            this.inputController = inputController;
            this.canvas = canvas;
            this.audioPlayer = audioPlayer;

            //canvas.Camera.EnableBounds(new Vector2(0, -20), new Vector2(50, 40), -5);

            world = new World(new Vector2(0, GRAVITY));

            // load level
            bool gc = inputController.ControllerConnected;
            LevelGenerator levelGenerator = new LevelGenerator(level, levelInfo, canvas, audioPlayer, world, gc);
            gridModel = levelGenerator.GenerateGridModel();
            backgrounds = levelGenerator.Background;
            //levelGenerator.printLevelData();

            // put level info in physics engine
            seamstress = levelGenerator.Seamstress;
            //ribbon = levelGenerator.Ribbon;
            objects = levelGenerator.Objects;
            miasma = levelGenerator.Miasma;
            ground = levelGenerator.Ground;
            ribbons = levelGenerator.Ribbons;
            decorations = levelGenerator.Decorations;
            actNumber = levelGenerator.ActNumber;

            //hack to get audio into ribbonelements
            foreach (RibbonObject r in ribbons)
                r.SetRibbonSounds(audioPlayer);

            collisionController = new CollisionController(world, seamstress, objects);

            canvas.Camera.Position = seamstress.body.Position;
            canvas.Camera.JumpToTarget();
            // create controllers
            cameraController = new CameraController(canvas.Camera, levelGenerator.CameraHotspots, inputController, audioPlayer);
            ribbonController = new RibbonForceController(seamstress, inputController);
            world.AddController(ribbonController);
            seamstressController = new SeamstressForceController(seamstress, inputController);
            world.AddController(seamstressController);
            miasma.GridModel = gridModel;
            levelInfo.CurrentTime = levelInfo.SaveTime;
        }

        public void Dispose()
        {
            world.RemoveController(ribbonController);
            world.RemoveController(seamstressController);
            world.Clear();
        }


        public void Update(GameTime gameTime)
        {
            if (dispose)
            {
                foreach (RibbonObject ribbon in ribbons)
                {
                    ribbon.RemoveBodies();
                }
            }
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (inputController.Debug.Pressed) world.Gravity = new Vector2(0, 0);
            else world.Gravity = new Vector2(0, GRAVITY);

            // Step physics Engine
            world.Step(dt);

            seamstress.Update(dt);
            //ribbon.Update(dt);
            if (objects != null)
            {
                foreach (GameObject o in objects)
                {
                    o.Update(dt);
                }
            }
            if (dynamics != null)
            {
                foreach (RibbonFeature o in dynamics)
                {
                    o.Move(1.0f, dt);
                }
            }

            collisionController.Update();
            if (seamstress.alive)
                cameraController.Update(seamstress.body.Position);

            backgrounds.Update(gameTime);

            if (!seamstress.win)
                levelInfo.CurrentTime += gameTime.ElapsedGameTime.Milliseconds;
        }

        public void PlayAudio(GameTime gameTime)
        {
            //ribbon.PlaySound(audioPlayer);
            seamstress.PlaySound(audioPlayer);
            cameraController.PlayAudio(audioPlayer);
            foreach(RibbonObject r in ribbons)
            {
                r.PlaySound(audioPlayer);
            }
            foreach(GameObject p in objects)
            {
                if(p is CollectableObject)
                {
                    ((CollectableObject)p).PlaySound(audioPlayer);
                }
                if(p is SaveRockObject)
                {
                    //((SaveRockObject)p).PlaySound(audioPlayer);
                }
                if (p is RibbonGemObject)
                {
                    ((RibbonGemObject)p).PlaySound(audioPlayer);
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            canvas.CoordinateMode = CoordinateMode.PhysicalCoordinates;
            backgrounds.Draw(gameTime, canvas);

            foreach (DecorationObject d in decorations)
            {
                d.Draw(canvas);
            }

            //ribbon.Draw(canvas);
            Draw3D(Projections.Behind);
            if (objects != null)
            {
                foreach (GameObject o in objects)
                {
                    if (!(o is MiasmaObject || o is GroundObject))
                        o.Draw(canvas);
                }
            }
            if (dynamics != null)
            {
                foreach (RibbonFeature f in dynamics)
                {
                    f.Draw(canvas);
                }
            }
            ground.Draw(canvas);
            miasma.Draw(canvas);
            seamstress.Draw(canvas);
            Draw3D(Projections.Front);

            #if DEBUG
            /*foreach (LevelGridStore lgs in gridModel)
            {
                if (lgs != null)
                    lgs.DrawDebug(canvas);
            }*/
            #endif
            canvas.CoordinateMode = CoordinateMode.ScreenCoordinates;
            foreach (RibbonObject r in ribbons)
                r.DrawCollisionNotifications(canvas);

            canvas.CoordinateMode = CoordinateMode.ScreenCoordinates;
            int mins = (int)((levelInfo.CurrentTime / 1000f) / 60f);
            int secs = (int)(levelInfo.CurrentTime / 1000f) - mins*60;
            int millisecs = levelInfo.CurrentTime - mins * 1000 * 60 - secs * 1000;
            canvas.DrawString(String.Format("{0:D2}:{1:D2}:{2:D3}", mins, secs, millisecs), Color.Black, new Vector2(canvas.Camera.Dimensions.X/2, 5), Anchor.TopCenter);
            //if (seamstress.Ribbon != null)
            //    canvas.DrawString(seamstress.Ribbon.ribbonStart + " <-> " + seamstress.Ribbon.ribbonEnd, Color.Black, Vector2.Zero);
        }

        private void Draw3D(Projections pass)
        {
            canvas.EndDraw();
            canvas.Begin3D();
            ground.Draw3D(canvas, pass);
            miasma.Draw3D(canvas, pass);
            foreach (RibbonObject r in ribbons)
                r.Draw3D(canvas, pass);
            canvas.End3D();
            canvas.BeginDraw(0, BlendState.NonPremultiplied);
        }

        public Color BackgroundColor { get { return backgrounds.Color; } }

        public int ActNumber { get { return actNumber; } }
    }
}
