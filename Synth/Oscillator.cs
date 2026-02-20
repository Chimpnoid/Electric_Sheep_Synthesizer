using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ElectricSheepSynth.Synth
{
    internal abstract class Oscillator:SampleNode
    {
        private double freq;
        private double sampleRate;
        private double phaseIncrement;
        
        public Oscillator(double frequency,double sampleRate,double PhaseOffset)
        {
            this.freq = frequency;
            this.sampleRate = sampleRate;   
            incrementPhase(PhaseOffset);
            
        }
        public double GetSR()
        {
            return this.sampleRate;
        }

        public double GetFrequency()
        {
            return this.freq;
        }
        public double GetPhaseIncrement()
        {
            return this.phaseIncrement;
        }

        private void incrementPhase(double increment)
        {
            this.phaseIncrement += increment;
            this.phaseIncrement %= 1.0;
        }   
        public override double GetNextSample()
        {

            double sample = WaveShape(phaseIncrement);
            incrementPhase(this.freq/sampleRate);


            return sample;
        }

        public abstract double WaveShape(double Phase);

    }
}
