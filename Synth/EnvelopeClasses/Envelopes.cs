using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectricSheepSynth.MiscClasses;

namespace ElectricSheepSynth.Synth
{
    // this I am less sure about whether it was necessary or not. I do not know the general form
    // of envelopes yet so will be looking at it again when I learn more. 
    internal abstract class Envelopes:SampleNode
    {
        protected IAudioSample waveform;
        protected double sampleRate;
        protected double timeAccumulator;
        

        public Envelopes(IAudioSample wave,double sr)
        {
            this.waveform = wave;
            this.sampleRate = sr;
            
        }

        protected void ResetTime()
        {
            this.timeAccumulator = 0.0;
        }
        protected void IncrementTime()
        {
            this.timeAccumulator += 1.0 / this.sampleRate;
        }
        public abstract override double GetNextSample();
    }
}
