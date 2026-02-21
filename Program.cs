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


            // create objects for notes in an A major chord
            var A4 = new SineOscillator(Note.A,4, sr, phaseOffset);
            var CS5 = new SineOscillator(Note.Cs,5, sr, phaseOffset);
            var E5 = new SineOscillator(Note.E, 5, sr, phaseOffset);

            // create LFO oscillator for tremelo
            var tremeloSinusoid = new SineOscillator(5.0, sr, phaseOffset);
            var tremeloEnvelope = tremeloSinusoid * 0.5 + 0.5;

            var chord = ((A4 + CS5 + E5)*tremeloEnvelope).ADSRLinearEnvelope(sr,0.33,0.33,0.4,0.5);

            // generates data for 5 whole cycles
            double duration = 50.0 / tremeloSinusoid.GetFrequency();
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
