using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricSheepSynth.Synth
{
    internal   class SineOscillator:Oscillator
    {
        // Implements Sine Oscillator as a derived oscillator.
        public SineOscillator(double sampleRate, double phaseOffset, double frequency) : base(sampleRate, phaseOffset, frequency) { }
        public SineOscillator(double sampleRate, double phaseOffset, Note note, int octave = 4) : base(sampleRate, phaseOffset, note, octave) { }
        // calculates the sine of an angle, given the phase is provided.
        public override double WaveShape(double Phase)
        {
            return Math.Sin(2*Math.PI*Phase);
        }
    }
}
