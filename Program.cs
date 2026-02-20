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


            List<SquareOscillator> synthList = new List<SquareOscillator>();

            // creates a list of synth squarewaves for a duty cycle sweep
            for (double duty = 0; duty <= 1.0; duty += 0.2) 
            {
                SquareOscillator Synth = new SquareOscillator(Note.A, 4, sr, phaseOffset,duty);
                synthList.Add(Synth);
            }

            // generates data for 5 whole cycles
            double duration = 5.0 / synthList[0].GetFrequency();
            int numberSamples = (int)(duration * sr);

            //generates a csv file with a single time column and a column for samples from each synth in the list.
            using var writer = new StreamWriter("data.csv");
         
            // Write header
            var header = "Time," + string.Join(",", synthList.Select((s, idx) => $"Synth{idx}"));
            writer.WriteLine(header);

            // sample time 
            double ts = 1.0 / sr;

            for (int i = 0; i < numberSamples; i++)
            {
                double time = ts * i;
                var values = synthList.Select(synth => synth.GetNextSample().ToString());
                writer.WriteLine($"{time},{string.Join(",", values)}");
            }

            writer.Close();
        }
    }
}
