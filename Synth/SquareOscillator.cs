using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ElectricSheepSynth.Synth
{
    internal class SquareOscillator:Oscillator
    {

        private double duty;

        // Implements square Oscillator as a derived oscillator. Overloaded constructors to allow for definition with a note and a harmonic or an arbitrary 
        // frequency. 
        // duty cycle refers to the portion of the waveform the signal is at 1.0 for. D = 0 negative DC, D = 0.5 perfect square, D = 1.0 positive DC;
        // Duty wraps from 0.0 to 1.0 probably should throw an error but those who know should know it isn't and those who don't it will just map between them
        // anyway.
        public SquareOscillator(double frequency, double sampleRate, double phaseOffset, double dutyCycle = 0.5) : base(frequency, sampleRate, phaseOffset) 
        {
            this.duty = dutyCycle % 1.0;
        
        }
        public SquareOscillator(Note note, int octave, double sampleRate, double phaseOffset, double dutyCycle = 0.5) : base(note, octave,sampleRate, phaseOffset) 
        {
            this.duty = dutyCycle % 1.0;
        }

        //returns instances duty cycle
        public double GetDuty()
        {
            return this.duty;
        }

        // calculates the square waveform of an angle.
        public override double WaveShape(double Phase)
        {
            // since our phase angle is normalised to one, we just need to check if we exceed the duty value.
            return Phase < this.duty ? 1.0:-1.0;
        }




    }
}
