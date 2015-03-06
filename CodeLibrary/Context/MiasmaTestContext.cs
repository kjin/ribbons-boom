using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CodeLibrary.Graphics;
using CodeLibrary.Audio;
using CodeLibrary.Input;

namespace CodeLibrary.Context
{
    class MiasmaTestContext : GameContext
    {
        MiasmaDrip miasma;
        //MiasmaParticleSystem miasma;
        Effect miasmaEffect;

        public MiasmaTestContext(GameContext other)
            : base(other)
        {
        }

        public override void Initialize()
        {
            List<Rectangle> rectangles = new List<Rectangle>();
            rectangles.Add(new Rectangle(-10, -5, 21, 11));
            miasma = new MiasmaDrip(rectangles, 10);
            //miasma = new MiasmaParticleSystem(10);
            Canvas.Camera.Position = Vector2.Zero;
            Canvas.Camera.JumpToTarget();
            miasmaEffect = Canvas.Assets.GetEffect("MiasmaDripEffect");
            //miasmaEffect = canvas.Assets.GetEffect("MiasmaParticleEffect");
            //miasmaEffect.Parameters["Color0"].SetValue(Color.Red.ToVector4());
            //miasmaEffect.Parameters["Color1"].SetValue(Color.Blue.ToVector4());
        }

        public override void Dispose() { }

        public override void Update(GameTime gameTime)
        {
            miasma.Update();
            if (InputController.Pause.JustPressed)
                NextContext = new SplashScreenMenuContext(this);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Canvas.CoordinateMode = CoordinateMode.PhysicalCoordinates;
            Canvas.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(miasmaEffect, miasma);
        }

        public override void PlayAudio(GameTime gameTime) { }
    }
}
