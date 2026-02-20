using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricSheepSynth.Synth
{

    // base interface for all audio streams. Declares the requirement for all IAudioSample to implement a way of retrieving the current sample
    // the synth is at

    internal interface IAudioSample
    {
        public double GetNextSample();
    }
}
