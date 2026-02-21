using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricSheepSynth.Synth
{
    internal abstract class Envelopes:IAudioSample
    {
        public abstract double GetNextSample();
    }
}
