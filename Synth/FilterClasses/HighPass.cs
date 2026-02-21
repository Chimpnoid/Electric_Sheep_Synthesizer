using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricSheepSynth.Synth
{
    internal class HighPass:Filter
    {

        private double alpha;
        private double prevInput;
        private double RC;

        public HighPass(IAudioSample wave,double cutoff, double sr) : base(wave, cutoff, sr)
        {
            double sampleTime = 1 / this.sampleRate;

            this.RC = 1 / (2 * Math.PI * this.cutOffFreq);

            this.alpha = this.RC / (this.RC + sampleTime);
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
