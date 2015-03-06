using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CodeLibrary.Graphics;
using CodeLibrary.Input;
using CodeLibrary.Audio;

namespace CodeLibrary.Context
{
    /// <summary>
    /// A selector that allows users to select between several text options.
    /// </summary>
    public class SingularSelector : Selector
    {
        Option option;
        int blinkRate;

        bool selected;
        SoundObject sound;

        int timer;

        public SingularSelector(Canvas canvas, string themeName, Vector2 position, Anchor anchor, Option option)
            : base(themeName, position, 0)
        {
            string optionTheme = canvas.AssetDictionary.LookupString(themeName, "optionTheme");
            blinkRate = canvas.AssetDictionary.LookupInt32(themeName, "blinkRate");
            if (canvas.AssetDictionary.CheckPropertyExists(themeName, "sound"))
                sound = new SoundObject(canvas.Assets.GetSFX(canvas.AssetDictionary.LookupString(themeName, "sound")));
            option.Initialize(canvas, optionTheme);

            this.position *= GraphicsConstants.VIEWPORT_DIMENSIONS / GraphicsConstants.DEFAULT_DIMENSIONS;
            this.position -= GraphicsHelper.ComputeAnchorOrigin(anchor, option.Dimensions);
            this.option = option;
            IntValue = 0;
            timer = 0;
            selected = false;
        }

        public override void Update(InputController inputController)
        {
            timer++;
            if (inputController.Pause.JustPressed)
                selected = true;
            option.Update(true);
        }

        public override void Draw(Canvas canvas)
        {
            if (timer % blinkRate < blinkRate / 2)
                option.Draw(canvas, position);
        }

        public override void PlayAudio(AudioPlayer audioPlayer)
        {
            if (sound != null)
                audioPlayer.PlayOnSetTrue(sound, selected);
        }
    }
}
