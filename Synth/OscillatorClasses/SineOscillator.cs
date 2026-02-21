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
        public SineOscillator(double frequency, double sampleRate, double phaseOffset) : base(frequency,sampleRate, phaseOffset) { }
        public SineOscillator(Note note, int octave,double sampleRate, double phaseOffset) : base(note, octave,sampleRate, phaseOffset) { }
        // calculates the sine of an angle, given the phase is provided.
        public override double WaveShape(double Phase)
        {
            return Math.Sin(2*Math.PI*Phase);
        }
    }
}
