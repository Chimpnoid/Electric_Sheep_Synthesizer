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
        private static volatile bool check = true;
        static void Main(string[] args)
        {
            // Start keyboard listener on background thread
            Task.Run(() =>
            {
                while (true)
                {
                    var key = Console.ReadKey(intercept: true);
                    if (key.Key == ConsoleKey.Spacebar)
                    {
                        check = !check;
                        Console.WriteLine(check ? "Key ON" : "Key OFF");
                    }
                }
            });


            double sr = 44100;
            double phaseOffset = 0;


            // create objects for notes in an A major chord
            var A4 = new SineOscillator(Note.A,4, sr, phaseOffset);
            var CS5 = new SineOscillator(Note.Cs,5, sr, phaseOffset);
            var E5 = new SineOscillator(Note.E, 5, sr, phaseOffset);
            var chord0 = (A4 + CS5).ADSRLinearEnvelope(sr,0.4,0.2,0.4,0.2) + E5.ADSRLinearEnvelope(sr,0.1,0.5,0.1,0.7);
        

            // generates data for 5 whole cycles

            double duration = 200000 / A4.GetFrequency();
            int numberSamples = (int)(duration * sr);

            //generates a csv file with a single time column and a column for samples from each synth in the list.
            using var writer = new StreamWriter("data.csv");

            // Write header
            var header = "Time,Sample";
            writer.WriteLine(header);

            // sample time 
            double ts = 1.0 / sr;
            for (int i = 0; i < numberSamples; i++)
            {

                double time = ts * i;
                writer.WriteLine($"{time},{chord0.GetNextSample()}");


                if (check)
                {
                    chord0.KeyOn(); 
                }
                else
                {
                    chord0.KeyOff();
                } 



            }

            writer.Close();
        }
    }
}
