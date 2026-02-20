using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricSheepSynth.Synth
{
    // SampleNode is currently nothing but will implement mixing and modulation techniques (+ and * overloaded operators).
    internal abstract class SampleNode : IAudioSample
    {
        public  abstract double GetNextSample();

        // overloading summing (mixing),multiplication (modulation),summing with double (y shift), multiplication with double (amplitude change)
        // (x,y) => x op y is a lambda function that defines what should happen to two inputs x and y, these are defined as a function<double,double,double>
        // in OperatorNode

        // signal operations:
        public static SampleNode operator +(SampleNode a, SampleNode b)
        {
            return new OperatorNode(a,b,(x,y) => x+y);
        }


        public static SampleNode operator *(SampleNode a, SampleNode b)
        {
            return new OperatorNode(a, b, (x, y) => x * y); ;
        }

        // operating with constants: ConstantNode(x) casts a double as an ConstantNode which derives from SampleNode.
        public static SampleNode operator *(SampleNode a, double b)
        {
            return new OperatorNode(a, new ConstantNode(b), (x, y) => x * y);
        }
        public static SampleNode operator *(double a, SampleNode b)
        {
            return new OperatorNode(new ConstantNode(a), b, (x, y) => x * y);
        }

        public static SampleNode operator +(SampleNode a, double b)
        {
            return new OperatorNode(a, new ConstantNode(b), (x, y) => x + y);
        }

        public static SampleNode operator +(double a, SampleNode b)
        {
            return new OperatorNode(new ConstantNode(a), b, (x, y) => x + y);
        }
    }
}
