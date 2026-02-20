using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricSheepSynth.Synth
{
    internal   class SineOscilator:Oscillator
    {

        public SineOscilator(double frequency, double sampleRate, double PhaseOffset) : base(frequency, sampleRate, PhaseOffset) { }

        public override double WaveShape(double Phase)
        {
            return Math.Sin(2*Math.PI*Phase);
        }
    }
}
