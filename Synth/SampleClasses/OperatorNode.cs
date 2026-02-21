using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricSheepSynth.Synth
{
    internal class OperatorNode:SampleNode
    {
        private readonly SampleNode a;
        private readonly SampleNode b;
        private readonly Func<double,double,double> op;

        // implements overloading of operators to allow for interaction between sound streams
        // a + b + c creates an operatornode object a+b which then is combined with sample node
        // object c. since operatornode inherits from samplenode this is defined. this is why we do not
        // increment GetNextSample() twice;
        public OperatorNode(SampleNode a,SampleNode b, Func <double,double,double> op)
        {
            this.a = a;
            this.b = b;
            this.op = op;
        }

        //passes current sample values to the lambda function saved in op. and returns the mixed value
        public override double GetNextSample()
        {
            return this.op(this.a.GetNextSample(),this.b.GetNextSample());
        }

    }
}
