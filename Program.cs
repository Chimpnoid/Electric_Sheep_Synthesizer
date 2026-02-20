using ElectricSheepSynth.Synth;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Intrinsics.X86;
using System.Timers;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace ElectricSheepSynth
{
    internal class Program
    {
        static void Main(string[] args)
        {


            double sr = 1000000;
            double phaseOffset = 0;



            var A4 = new SineOscillator(440.0, sr, phaseOffset);
            var CS5 = new SineOscillator(550.0, sr, phaseOffset);
            var E5 = new SineOscillator(660.0, sr, phaseOffset);

            var tremeloSinusoid = new SineOscillator(5.0, sr, phaseOffset);
            var tremeloEnvelope = tremeloSinusoid * 0.5 + 0.5;

            SampleNode chord = A4 * tremeloEnvelope;
            // generates data for 5 whole cycles
            double duration = 20.0 / tremeloSinusoid.GetFrequency();
            int numberSamples = (int)(duration * sr);

            //generates a csv file with a single time column and a column for samples from each synth in the list.
            using var writer = new StreamWriter("data.csv");

            // Write header
            var header = "Time,sample";
            writer.WriteLine(header);

            // sample time 
            double ts = 1.0 / sr;

            for (int i = 0; i < numberSamples; i++)
            {
                double time = ts * i;
                var value = chord.GetNextSample();
                writer.WriteLine($"{time},{value}");
            }

            writer.Close();
        }
    }
}
