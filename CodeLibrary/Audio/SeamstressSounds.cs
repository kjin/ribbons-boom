using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeLibrary.Audio
{
    /// <summary>
    /// A collection of Seamstress sounds.
    /// </summary>
    public class SeamstressSounds : SoundObjectCollection
    {
        /// <summary>
        /// Constructs a new SeamstressSprites instance.
        /// </summary>
        /// <param name="audioPlayer">The audio player associated with the game.</param>
        public SeamstressSounds(AudioPlayer audioPlayer)
            : base(audioPlayer)
        {
            Add("seamstress_land"); //     0
            Add("seamstress_step"); //     1
            Add("seamstress_jump"); //     2
            Add("ribbon_swap");     //     3

        }

        public SoundObject Land { get { return this[0]; } }
        public SoundObject Step { get { return this[1]; } }
        public SoundObject Jump { get { return this[2]; } }
        public SoundObject SwapRibbons { get { return this[3]; } }
    }
}
