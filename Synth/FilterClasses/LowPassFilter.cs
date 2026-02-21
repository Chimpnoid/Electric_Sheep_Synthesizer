using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricSheepSynth.Synth
{
    // simple first order low pass filter implementation. y[k] = ax[k] + (1-a)y[k-1]
    // mimics simple first order RC filter
    // could probably be generalised to nth order filter but not looked yet. 
    internal class LowPassFilter:Filter
    {

        private double alpha;
        private double RC;

        public LowPassFilter(IAudioSample wave,double cutoff,double sr): base(wave, cutoff, sr)
        {
            double sampleTime = 1 / this.sampleRate;

            this.RC = 1 / (2 * Math.PI * this.cutOffFreq);
            this.alpha = sampleTime / (this.RC + sampleTime);
        }


        public override double GetNextSample()
        {
            this.prevOutput = this.alpha * this.waveform.GetNextSample() + (1 - this.alpha) * this.prevOutput;
            return this.prevOutput;
        }
    }
}
