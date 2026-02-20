using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ElectricSheepSynth.Synth
{

    // oscillator class is abstract as it requires a modifier of oscillator type which is a derived class. It implements base functionality
    // required for generating the next sample in the waveform. Uses lazy sample by sample generation. 
    internal abstract class Oscillator:SampleNode
    {
        private double freq; // frequency of oscillator in hertz
        private double sampleRate; // sample rate of signal in samples per second
        private double phaseIncrement; // current phase increment, maps between 0 and 1, essentially how of the way through a cycle are we
        

        public Oscillator(Note note,int octave,double sampleRate,double phaseOffset)
        {
            this.freq = Note2Freq(note,octave);
            this.sampleRate = sampleRate;
            incrementPhase(phaseOffset);
        }

        public Oscillator(double frequency, double sampleRate,double PhaseOffset)
        {
            this.freq = frequency;
            this.sampleRate = sampleRate;   

            // increments an initial phase offset which becomes the starting positon.
            incrementPhase(PhaseOffset);
            
        }

        // returns sample rate.
        public double GetSR()
        {
            return this.sampleRate;
        }

        // returns oscillator frequency
        public double GetFrequency()
        {
            return this.freq;
        }

        // returns current phase increment (how far through the cycle we are)
        public double GetPhaseIncrement()
        {
            return this.phaseIncrement;
        }


        // increments to next sample and wraps between 0 and 1 ( equivalent to 0 and 2pi)
        private void incrementPhase(double increment)
        {
            this.phaseIncrement += increment;
            this.phaseIncrement %= 1.0;
        }   

        // implementation of the GetNextSample() method required by the interface. Uses a generic abstract WaveShape Function which is defined by
        // the derived oscillator classes. increments phase to the next sample. I haven't decided whether this needs an amplitude variable.  
        public override double GetNextSample()
        {

            double sample = WaveShape(phaseIncrement);
            incrementPhase(this.freq/sampleRate);


            return sample;
        }

        // convert the note from Enum to an actual frequency corresponding to the western music notes. 
        double Note2Freq(Note note,int octave) 
        {
            double baseFreq;

            switch (note)
            {
                case Note.As or Note.Bb:
                    baseFreq = 466.16;
                    break;
                case Note.B:
                    baseFreq = 493.88;
                    break;
                case Note.C:
                    baseFreq = 261.63;
                    break;
                case Note.Cs or Note.Db:
                    baseFreq = 277.18;
                    break;
                case Note.D:
                    baseFreq = 293.66;
                    break;
                case Note.Ds or Note.Eb:
                    baseFreq = 311.13;
                    break;
                case Note.E:
                    baseFreq = 329.63;
                    break;
                case Note.F:
                    baseFreq = 349.23;
                    break;
                case Note.Fs or Note.Gb:
                    baseFreq = 369.99;
                    break;
                case Note.G:
                    baseFreq = 392.00;
                    break;
                case Note.Gs or Note.Ab:
                    baseFreq = 415.30;
                    break;
                default:
                    baseFreq = 440.0;
                    break;
                    
            }

            return baseFreq * Math.Pow(2, octave - 4);
        }


        // definition of WaveShape to be implemented by derived classes.
        public abstract double WaveShape(double Phase);

    }
}
