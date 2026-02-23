using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricSheepSynth.Synth
{
    // implements a simple first order RC high pass filter.
    // y[k] = a(y[k-1]+x[k]+x[k-1])
    // this could probably be generalised to nth order but haven't thought about it yet.
    internal class HighPassFIlter:Filter
    {

        private double alpha;
        private double prevInput;
        private double RC;

        public HighPassFIlter(IAudioSample wave,double cutoff, double sr) : base(wave, cutoff, sr)
        {
            double sampleTime = 1 / this.sampleRate;

            this.RC = 1 / (2 * Math.PI * this.cutOffFreq);
            this.alpha = RC / (this.RC + sampleTime);
        }


        public override double GetNextSample()
        {
            double curInput = this.waveform.GetNextSample();
            this.prevOutput = this.alpha * (this.prevOutput + curInput - this.prevInput);
            this.prevInput = curInput;

            return this.prevOutput;
        }

    }
}
