using ElectricSheepSynth.Synth;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Intrinsics.X86;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace ElectricSheepSynth
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var synth = new SineOscilator(440.1f, 44100.0f, 0.0f);

            List<(double time,double value)> samples = new List<(double, double)>();


            double duration = 1 / synth.GetFrequency();
            int numberSamples = (int)(duration * synth.GetSR());
            double ts = 1/synth.GetSR() ;

            for(int i = 0; i < numberSamples; i++)
            {
                samples.Add((i*ts,synth.GetNextSample()));
            }


        }
    }
}
