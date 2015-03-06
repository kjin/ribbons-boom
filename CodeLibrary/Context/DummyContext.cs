using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeLibrary.Graphics;
using CodeLibrary.Audio;
using CodeLibrary.Input;
using CodeLibrary.Engine;
using CodeLibrary.Storage;
using Microsoft.Xna.Framework;

namespace CodeLibrary.Context
{
    public class DummyContext : GameContext
    {
        public DummyContext(FileManager fileManager, InputController inputController, Canvas canvas, AudioPlayer audioPlayer)
            : base(fileManager, inputController, canvas, audioPlayer)
        {
            FadeMultiplier = 1;
        }

        public override void Initialize() { }

        public override void Dispose() { }

        public override void Update(GameTime gameTime) { base.Update(gameTime); }

        public override void Draw(GameTime gameTime) { }

        public override void PlayAudio(GameTime gameTime) { }
    }
}
