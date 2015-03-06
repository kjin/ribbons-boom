using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CodeLibrary.Graphics;
using CodeLibrary.Audio;
using CodeLibrary.Input;

namespace CodeLibrary.Context
{
    /// <summary>
    /// An abstract class that represents a visual set of selectable options.
    /// </summary>
    public abstract class Selector
    {
        string themeName;
        float floatValue;
        int intValue;
        bool boolValue;
        protected Vector2 position;

        public Selector(string themeName, Vector2 position, bool initialValue) : this(themeName, position, initialValue ? 1f : 0f) { }

        public Selector(string themeName, Vector2 position, int initialValue) : this(themeName, position, (float)initialValue) { }

        public Selector(string themeName, Vector2 position, float initialValue)
        {
            this.position = position;
            this.themeName = themeName;
            this.floatValue = initialValue;
            this.intValue = (int)initialValue;
            this.boolValue = initialValue != 0f;
        }

        public abstract void Update(InputController inputController);
        public abstract void Draw(Canvas canvas);
        public virtual void PlayAudio(AudioPlayer audioPlayer) { }

        protected string ThemeName { get { return themeName; } }
        public float FloatValue { get { return floatValue; } set { floatValue = value; } }
        public int IntValue { get { return intValue; } set { intValue = value; } }
        public bool BoolValue { get { return boolValue; } set { intValue = value ? 1 : 0; } }
    }
}
