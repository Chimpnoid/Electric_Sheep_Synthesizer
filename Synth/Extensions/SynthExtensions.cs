using ElectricSheepSynth.Synth.EnvelopeClasses;
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
        // extensions allow for fluent syntax of application of modifiers to audio objects. Akin to a signal chain. Key idea is using the this keyword.
        public static LowPassFilter LowPassFilter(this IAudioSample wave,double cutoff,double sr)
        {
            return new LowPassFilter(wave,cutoff,sr);
        }

        public static HighPassFIlter HighPassFilter(this IAudioSample wave,double cutoff,double sr)
        {
            return new HighPassFIlter(wave,cutoff,sr);
        }

        public static ADSRLinearEnvelope ADSRLinearEnvelope(this IAudioSample wave, double sr, double tAtt, double tDec, double lSus, double tRel)
        {
            return new ADSRLinearEnvelope(wave,sr,tAtt,tDec,lSus,tRel);
        }
    }
}
