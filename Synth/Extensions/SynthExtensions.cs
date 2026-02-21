using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ElectricSheepSynth.Synth
{
    internal static class SynthExtensions
    {

        public static LowPass LowPass(this IAudioSample wave,double cutoff,double sr)
        {
            return new LowPass(wave,cutoff,sr);
        }

        public static HighPass HighPass(this IAudioSample wave,double cutoff,double sr)
        {
            return new HighPass(wave,cutoff,sr);
        }
    }
}
