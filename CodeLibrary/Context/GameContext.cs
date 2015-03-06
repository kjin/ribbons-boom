using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using CodeLibrary.Graphics;
using CodeLibrary.Audio;
using CodeLibrary.Input;
using CodeLibrary.Content;
using CodeLibrary.Storage;

namespace CodeLibrary.Context
{
    /// <summary>
    /// Represents one of several contexts of the game. Possible contexts include menus and the actual gameplay itself.
    /// </summary>
    public abstract class GameContext
    {
        InputController inputController;
        Canvas canvas;
        AudioPlayer audioPlayer;
        FileManager fileManager;

        float gridWidth;
        float gridHeight;
        bool gridChanged;

        GameContext nextState;
        bool exit;
        Color backgroundColor;
        float fadeMultiplier;

        TimeSpan timeLoaded;
        bool timeSet;

        static float DEFAULT_FADE_MULTIPLIER = 0.2f;

        /// <summary>
        /// Gets the input controller associated with this game.
        /// </summary>
        protected InputController InputController { get { return inputController; } }
        /// <summary>
        /// Gets the canvas associated with this game.
        /// </summary>
        protected Canvas Canvas { get { return canvas; } }
        /// <summary>
        /// Gets the audio player associated with this game.
        /// </summary>
        protected AudioPlayer AudioPlayer { get { return audioPlayer; } }
        /// <summary>
        /// Gets the file manager associated with this game.
        /// </summary>
        protected FileManager FileManager { get { return fileManager; } }
        /// <summary>
        /// Gets or sets the next context to switch to. When set to a non-null target, the game engine will automatically switch to
        /// that target during its update loop.
        /// </summary>
        public GameContext NextContext { get { return nextState; } set { nextState = value; } }
        /// <summary>
        /// Gets or sets whether the game should exit.
        /// </summary>
        public bool Exit { get { return exit; } protected set { exit = value; } }
        /// <summary>
        /// Gets or sets the background color of this game context.
        /// </summary>
        public Color BackgroundColor { get { return backgroundColor; } protected set { backgroundColor = value; } }
        /// <summary>
        /// Gets or sets the fade in/out multiplier. The higher this number is, the faster the context will fade in or out.
        /// </summary>
        public float FadeMultiplier { get { return fadeMultiplier; } protected set { fadeMultiplier = value; } }

        public float GridWidth { get { return gridWidth; } set { gridWidth = value; } }
        public float GridHeight { get { return gridHeight; } set { gridHeight = value; } }
        public bool GridChanged { get { return gridChanged; } set { gridChanged = value; } }

        public TimeSpan TimeLoaded
        {
            get
            {
                if (timeSet)
                    return timeLoaded;
                else
                    return TimeSpan.MaxValue; //just a really long timespan.
            }
        }

        /// <summary>
        /// Constucts a new GameContext object.
        /// </summary>
        /// <param name="other">The previous game context.</param>
        public GameContext(GameContext other) : this(other.fileManager, other.inputController, other.canvas, other.audioPlayer) { timeSet = false; }

        /// <summary>
        /// Constucts a new GameContext object.
        /// </summary>
        /// <param name="fileManager">The file manager associated with this game.</param>
        /// <param name="inputController">The input controller associated with this game.</param>
        /// <param name="canvas">The canvas associated with this game.</param>
        /// <param name="audioPlayer">The audio player associated with this game.</param>
        public GameContext(FileManager fileManager, InputController inputController, Canvas canvas, AudioPlayer audioPlayer)
        {
            this.fileManager = fileManager;
            this.inputController = inputController;
            this.canvas = canvas;
            this.audioPlayer = audioPlayer;
            this.fadeMultiplier = DEFAULT_FADE_MULTIPLIER;
            timeSet = false;
        }

        /// <summary>
        /// Initialize the current context.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Frees memory allocated during the creation of this instance.
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Updates the context state.
        /// </summary>
        /// <param name="gameTime">A snapshot of timing values.</param>
        public virtual void Update(GameTime gameTime)
        {
            if (!timeSet)
            {
                timeLoaded = gameTime.TotalGameTime;
                timeSet = true;
            }
        }
        /// <summary>
        /// Draws objects in this context.
        /// </summary>
        /// <param name="gameTime">A snapshot of timing values.</param>
        public abstract void Draw(GameTime gameTime);
        /// <summary>
        /// Plays sounds in this context.
        /// </summary>
        /// <param name="gameTime">A snapshot of timing values.</param>
        public abstract void PlayAudio(GameTime gameTime);
    }
}
