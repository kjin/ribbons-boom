using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using CodeLibrary.Content;
using CodeLibrary.Engine;
using CodeLibrary.Context;
using CodeLibrary.Graphics;
using CodeLibrary.Audio;
using CodeLibrary.Input;
using CodeLibrary.Storage;

using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using XMLContent;

namespace CodeLibrary.Engine
{
    /// <summary>
    /// The game engine. Provides top-level context control over the game.
    /// </summary>
    public class GameEngine
    {
        #region Fields

        //Graphics and Audio
        GraphicsDeviceManager graphics;
        ContentManager content;
        GraphicsDevice graphicsDevice;
        AssetManager assets;
        Canvas canvas;
        AudioPlayer audioPlayer;
        //Input Controller
        InputController inputController;
        //File Manager
        FileManager fileManager;
        //Current running context
        public GameContext currentContext;
        float currentOverlayAlpha;
        float targetOverlayAlpha;
        bool exitGame;
        Texture2D loading;
        Texture2D princeBeau;
        //async stuff
        bool asyncStarted;
        bool asyncFinished;
        #endregion

        #region Properties
        public bool Exit { get { return exitGame; } }

        //public Point MousePosition { get { return inputController.MousePosition; } }
        #endregion

        public GameEngine(GraphicsDeviceManager graphics, ContentManager content, GraphicsDevice graphicsDevice)
        {
            this.graphics = graphics;
            this.content = content;
            this.graphicsDevice = graphicsDevice;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        public void Initialize()
        {
            canvas = new Canvas(graphics, graphicsDevice);
#if DEBUG
            canvas.DebugMode = true;
#else
            canvas.DebugMode = false;
#endif
            // Initialize audio player.
            audioPlayer = new AudioPlayer();
            assets = new AssetManager();
            exitGame = false;

            //launch initialize asynchronously
            //ThreadPool.QueueUserWorkItem(new WaitCallback(InitializeNextContext));
            asyncFinished = true;
            /*Thread t = new Thread(new ThreadStart(InitializeNextContext));
            t.IsBackground = true;
            t.Start();*/
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        public void LoadContent()
        {
            // load all content
            assets.LoadContent(content, graphicsDevice);
            canvas.LoadContent(assets);
            audioPlayer.LoadContent(assets);
            inputController = new InputController(assets);
            fileManager = new FileManager(assets);

            inputController.Update();
            GameContext dummy = new DummyContext(fileManager, inputController, canvas, audioPlayer);
            currentContext = new IntroContext(dummy);
            currentContext.Initialize();

            currentOverlayAlpha = 0;

            loading = assets.GetTexture("text_loading");
            princeBeau = assets.GetTexture("loading_trappedbeau");
        }

        public Level GetLevel(string path)
        {
            return canvas.Assets.GetLevel(path);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        public void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            currentContext.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            // Get input from the player
            inputController.Update();
            // screencaps
            if (inputController.ScreenCap.JustPressed)
            {
                Draw(gameTime);
                Texture2D texture = new Texture2D(graphicsDevice, GraphicsConstants.VIEWPORT_WIDTH, GraphicsConstants.VIEWPORT_HEIGHT);
                Color[] data = new Color[texture.Width * texture.Height];
                graphicsDevice.GetBackBufferData<Color>(data);
                texture.SetData<Color>(data);
                Stream stream = File.OpenWrite("output.png");
                texture.SaveAsPng(stream, texture.Width, texture.Height);
                stream.Dispose();
                texture.Dispose();
            }

            if (currentContext.Exit || currentContext.NextContext != null)
            {
                targetOverlayAlpha = 1;
                if (currentOverlayAlpha == 1)
                {
                    //audioPlayer.StopSong();
                    if (currentContext.Exit)
                        exitGame = true;
                    else if (!asyncStarted)
                    {
                        asyncStarted = true;
                        asyncFinished = false;
                        currentContext.NextContext.Initialize();
                        asyncFinished = true;
                    }
                    if (asyncStarted && asyncFinished)
                    {
                        //Thread.Sleep(1000);
                        asyncStarted = false;
                        targetOverlayAlpha = 0;
                        currentContext.Dispose();
                        currentContext = currentContext.NextContext;
                    }
                }
            }
            else
            {
                currentContext.Update(gameTime);
            }

            currentOverlayAlpha = MathHelper.Lerp(currentOverlayAlpha, targetOverlayAlpha, currentContext.FadeMultiplier);
            //Console.WriteLine(Math.Abs(currentOverlayAlpha - targetOverlayAlpha));
            if (Math.Abs(currentOverlayAlpha - targetOverlayAlpha) < 0.001f || inputController.Zoom.Pressed)
                currentOverlayAlpha = targetOverlayAlpha;
            PlayAudio(gameTime);
        }

        void InitializeNextContext()
        {
            while (true)
            {
                Thread.Sleep(100);
                if (asyncFinished) continue;
                Console.WriteLine("Context loading started...");
                currentContext.NextContext.Initialize();
                asyncFinished = true;
                Console.WriteLine("Context loading finished.");
            }
        }

        public void ChangeEditorContext(int context, Level level)
        {
            if (context == 1)
            {
                if (level != null)
                {
                    currentContext.NextContext = new EditorBuildContext(level, currentContext);
                }
                else
                {
                    currentContext.NextContext = new EditorBuildContext(currentContext);
                }
            }
            if (context == 4)
                currentContext.NextContext = new LevelContext(level, fileManager.ActiveFile.GetLevelInfo(0, 1), currentContext);
        }

        /// <summary>
        /// This is called when the game should play sounds.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected void PlayAudio(GameTime gameTime)
        {
            audioPlayer.Update(gameTime);
            currentContext.PlayAudio(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Draw(GameTime gameTime)
        {
            graphicsDevice.Clear(ClearOptions.Stencil | ClearOptions.Target, currentContext.BackgroundColor, 0, 0);
            canvas.BeginDraw();
            currentContext.Draw(gameTime);
            canvas.CoordinateMode = CoordinateMode.ScreenCoordinates;
            canvas.DrawRectangle(Color.Transparent, new Color(0f, 0f, 0f, currentOverlayAlpha), 0, Vector2.Zero, canvas.Camera.Dimensions, 0, false);
            if (currentOverlayAlpha == 1)
            {
                //canvas.DrawString("LOADING", Color.White, canvas.Camera.Dimensions / 2, Anchor.Center);
                canvas.DrawTexture(loading, Color.White, canvas.Camera.Dimensions / 2, Anchor.Center, 0, 0.5f);
                canvas.DrawTexture(princeBeau, Color.White, canvas.Camera.Dimensions - 40 * Vector2.UnitX, Anchor.BottomRight, 0, 0.5f);
            }
            canvas.EndDraw();
        }
    }
}
