using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CodeLibrary.Graphics;
using CodeLibrary.Audio;
using CodeLibrary.Input;
using CodeLibrary.Storage;

namespace CodeLibrary.Context
{
    public abstract class MenuContext : GameContext
    {
        public MenuContext(GameContext other)
            : base(other) { }

        //public MenuContext(FileManager fileManager, InputController inputController, Canvas canvas, AudioPlayer audioPlayer)
        //    : base(fileManager, inputController, canvas, audioPlayer) { }

        public override void Dispose() { }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime) { }

        public override void PlayAudio(GameTime gameTime) { }
    }
}
