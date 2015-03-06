using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeLibrary.Audio
{
    /// <summary>
    /// A collection of Ribon sounds.
    /// </summary>
    public class RibbonSounds : SoundObjectCollection
    {
        /// <summary>
        /// Constructs a new RibbonSprites instance.
        /// </summary>
        /// <param name="audioPlayer">The audio player associated with the game.</param>
        public RibbonSounds(AudioPlayer audioPlayer)
            : base(audioPlayer)
        {
            Add("ribbon_flip"); //     0
            Add("ribbon_flip");
            Add("ribbon_collision"); //     2
            Add("ribbongem_powerup"); //     3
            this[0].Playable = false;
            this[1].Playable = false;
        }

        public SoundObject Flip1 { get { return this[0]; } }
        public SoundObject Flip2 { get { return this[1]; } }
        public SoundObject Collision { get { return this[2]; } }
        public SoundObject RibbonGemPowerUp { get { return this[3]; } }
    }
}
