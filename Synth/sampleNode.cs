using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricSheepSynth.Synth
{
    // SampleNode is currently nothing but will implement mixing and modulation techniques (+ and * overloaded operators).
    internal abstract class SampleNode : IAudioSample
    {
        public  abstract double GetNextSample();

    }
}
