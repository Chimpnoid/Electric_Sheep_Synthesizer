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


            double sr = 44100;
            double phaseOffset = 0;


            List<SineOscillator> synthList = new List<SineOscillator>();

            // creates a list of synth sinewaves for each chromatic note. .Cast<Note>.Distinct() filters flats and sharps.
            foreach (Note note in Enum.GetValues(typeof(Note)).Cast<Note>().Distinct())
            {
                SineOscillator Synth = new SineOscillator(sr,phaseOffset,note,4);
                synthList.Add(Synth);
            }


            // determine the lowest frequency using a LINQ query. so the buffer contains at least one cycle of
            // each frequency. 
            double lowestFreq = synthList.Min(s => s.GetFrequency());
            double highestFreq = synthList.Max(s => s.GetFrequency());

            double duration = 1.0 / lowestFreq;
            int numberSamples = (int)(duration * sr);

            double ts = 1.0 / sr;



            //generates a csv file with a single time column and a column for samples from each synth in the list. 
            using var writer = new StreamWriter("data.csv");

            // Write header
            var header = "Time," + string.Join(",", synthList.Select((s, idx) => $"Synth{idx}"));
            writer.WriteLine(header);

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
