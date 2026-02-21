using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricSheepSynth.Synth
{
    //Implements a constant sound source an unchanging DC value
    internal class ConstantNode:SampleNode
    {
        private readonly double value;
        public ConstantNode(double value)
        {
            this.value = value;
        }
        public override double GetNextSample()
        {
            return this.value;
        }
    }
}
