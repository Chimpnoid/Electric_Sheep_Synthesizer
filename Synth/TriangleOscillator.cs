using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ElectricSheepSynth.Synth
{
    internal class TriangleOscillator:Oscillator
    {
        private double duty;

        // Triangle Oscillator constructors - derived from oscillator class. Overloaded constructors to allow for generation with note and octave or 
        // arbitrary frequency.
        // Duty cycle in this class refers to the point where a maximum is achieved. its value is fixed between 0 and 1
        // 0 is a negative sawtooth, 0.5 perfect triangle (equal gradient about duty point), 1.0 positive sawtooth. it wraps any duty given between 0 and 1. 
        // probably should do an error throw but this makes it work no matter what, those who know will know anyway.
        public TriangleOscillator(double frequency, double sampleRate, double phaseOffset, double dutyCycle = 0.5) : base(frequency,sampleRate, phaseOffset) 
        {
            this.duty = dutyCycle % 1.0;
        }
        public TriangleOscillator(Note note, int octave, double sampleRate, double phaseOffset,double dutyCycle = 0.5) : base(note, octave, sampleRate, phaseOffset)
        {
            this.duty = dutyCycle % 1.0;
        }

        // returns instances duty cycle
        public double GetDuty()
        {
            return this.duty;
        }


        // calculates the triangle waveform of an angle, given the phase is provided.
        public override double WaveShape(double Phase)
        {
            // since we normalise between 0 and 1 for both amplitude and phase angle
            // everything is simplified the max point is the 
            // value of the duty (as this is bounded between 0 and 1)
            // if phase < duty y = 2.0/duty * phase - 1.0
            // else y = -2.0/(1.0 - duty) * phase + (cutoff of the negative portion)
            // this is given by c = 1.0 - (-2.0/(1.0 - duty) ) *  phase 

            double m1 = 2.0 / (this.duty);
            double c1 = -1.0;
            double m2 = -2.0 / (1.0 - this.duty);
            double c2 = 1.0 - m2 * this.duty;


            //piece wise to return the current sample based on where the angle is relative to duty
            return Phase < this.duty ? m1*Phase + c1 : m2*Phase+ c2 ;
        }
    }
}
