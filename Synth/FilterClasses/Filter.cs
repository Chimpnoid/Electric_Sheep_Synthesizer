using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricSheepSynth.Synth
{
    internal abstract class Filter:IAudioSample
    {
        protected IAudioSample waveform;
        protected double cutOffFreq;
        protected double prevOutput;
        protected double sampleRate;

        public Filter(IAudioSample waveform,double cutOffFrequency,double sr)
        {
            this.waveform = waveform;
            this.cutOffFreq = cutOffFrequency;
            this.prevOutput = 0;
            this.sampleRate = sr;
        }


        public abstract double GetNextSample();

    }
}
