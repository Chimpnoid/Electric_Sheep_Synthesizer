using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ElectricSheepSynth.Synth.EnvelopeClasses
{

    // implements a simple linear Attack Decay Sustain envelope that can be applied to an audio object. 
    // it is a linear ADSR so stages have a simple y =  mx + c formula for modulating amplitude.
    // it currently implements a fixed gating time ( key is pressed for fixed period), this needs to be 
    // extended to include a keyoff function for gating the signal. 
    internal class ADSRLinearEnvelope:Envelopes
    {

        private double Ta;
        private double Td;
        private double Ts;
        private double lSustain;
        private double Tr;
        private double m1;
        private double m2;
        private double m3;
        private double c2;
        private double c3;
        private bool soundOff;
       
        public ADSRLinearEnvelope(IAudioSample wave, double sr,double tAtt,double tDec,double lSus,double tRel) : base(wave, sr)
        {
            this.Ta = tAtt;
            this.Td = this.Ta + tDec;
            this.Ts = this.Td + 0.33;
            this.lSustain = lSus;
            this.Tr = this.Ts + tRel;
            this.soundOff = false;


            // caluclates the attack stage envelope equation.
            // raises from 0.0 to 1.0 in tAtt seconds
            this.m1 = 1.0 / tAtt;

            // calculates the decay stage envelope equation.
            // drops from 1.0 to lSus.
            this.m2 = (this.lSustain - 1.0) / tDec;
            // decay starts at Ta seconds.
            this.c2 = (1.0 - m2 * this.Ta);

            // calculates the release stage envelope equation.
            // starts at level L and decays t zero in tRelease seconds.
            this.m3 = -lSus / (tRel);
            this.c3 = (this.lSustain - m3 * this.Ts);

        }


        // calculate the envelope gain at current increment of time since note plucked. 
        private double envelopeGain()
        {
            if (!soundOff)
            {
                if(this.timeAccumulator <= this.Ta)
                {
                    return this.timeAccumulator / this.Ta;
                }
                else if(this.timeAccumulator > this.Ta && this.timeAccumulator <= this.Td) 
                {

                    return m2*this.timeAccumulator + c2;

                }
                else if(this.timeAccumulator > this.Td && this.timeAccumulator <= this.Ts)
                {
                    return this.lSustain;
                }
                else if(this.timeAccumulator > this.Ts && this.timeAccumulator <= this.Tr)
                {
                    return this.m3 * this.timeAccumulator + c3;
                }
                else
                {
                    this.soundOff = false;
                    return 0.0;

                }

            }

            return 0.0;
        }


        
        public override double GetNextSample()
        {
            // increment time accumulator while sound is playing (from plucked to release)
            this.timeAccumulator = soundOff ? 0.0 : timeAccumulator + 1.0 / this.sampleRate;

            return this.waveform.GetNextSample() * envelopeGain();
        }



    }
}
;