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

using CodeLibrary;
using CodeLibrary.Audio;
using CodeLibrary.Content;
using CodeLibrary.Context;
using CodeLibrary.Engine;
using CodeLibrary.Graphics;
using CodeLibrary.Input;

namespace GameShell
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        GameEngine game;

        public Game()
        {
            this.Window.Title = "Ribbons!";
            graphics = new GraphicsDeviceManager(this);
            IsFixedTimeStep = false;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            game = new GameEngine(graphics, Content, GraphicsDevice);
            game.Initialize();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is th      e place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            game.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            game.UnloadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (game.Exit)
                Exit();
            game.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            game.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
