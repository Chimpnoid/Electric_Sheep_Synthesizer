using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricSheepSynth.Synth
{
    // this I am less sure about whether it was necessary or not. I do not know the general form
    // of envelopes yet so will be looking at it again when I learn more. 
    internal abstract class Envelopes:IAudioSample
    {
        protected IAudioSample waveform;
        protected double sampleRate;
        protected double timeAccumulator;

        public Envelopes(IAudioSample wave,double sr)
        {
            this.waveform = wave;
            this.sampleRate = sr;

        }

        public abstract double GetNextSample();
    }
}
