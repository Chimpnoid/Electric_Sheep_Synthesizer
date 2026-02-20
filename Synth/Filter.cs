using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricSheepSynth.Synth
{
    internal abstract class Filter:IAudioSample
    {
        private double cutOffFreq;

        public Filter(double cutOffFrequency)
        {
            this.cutOffFreq = cutOffFrequency;
        }


        public abstract double GetNextSample();

    }
}
