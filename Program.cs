using Microsoft.Win32.SafeHandles;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Runtime;
using System.Runtime.Intrinsics.X86;




namespace ElectricSheepSynth
{
    internal class Program
    {
        // Generate the WAV standard file header. This includes all necessary preamble to 
        // be read as a WAV without the wav data.  
        static byte[] GenerateWavHeader(short audioFormat,short nmbrChannels,int sampleRate,short bitsPerSample,int sampleNumber)
        {

            //this is how many bytes per sample need to be read. multiplied by number of channels
            //as wav interleaves sample data. 
            short bytePerBloc = (short)(nmbrChannels * bitsPerSample / 8);
            
            //calculates how big the sample data in the wave file is
            int dataSize = sampleNumber*bytePerBloc;

            //how many bytes are streamed to the speakers per second.
            int bytePerSec = bytePerBloc * sampleRate;

            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);

            //=====================================================================
            // RIFF Header Chunk
            //=====================================================================

            //Identifier: 4 bytes
            bw.Write(new byte[] { 0x52, 0x49, 0x46, 0x46 }); // identifier = RIFF

            //overall file size - 8 bytes. I am assuming it ignores the 8 bytes of tag and size?: 4 bytes
            bw.Write(44 + dataSize - 8); // 44 bytes is total header size dataSize is the size of the sampled data

            //file format id: 4 bytes
            bw.Write(new byte[] { 0x57, 0x41, 0x56, 0x45 }); // format = WAVE 

            //=====================================================================
            // data format chunk
            //=====================================================================

            //format block ID: 4 bytes 
            bw.Write(new byte[] { 0x66, 0x6D, 0x74, 0x20 }); // Identifier = fmt

            //block Size: 2 bytes
            bw.Write((int) 16); // size of this block is always 16 bits

            //audio Format (1: PCM Integer or 3: IEEE 754 float): 2 bytes
            bw.Write(audioFormat); // defaulting to PCM Integer format

            //number of channels: 2 bytes
            bw.Write(nmbrChannels);

            //sample rate in Hz: 4 bytes
            bw.Write(sampleRate);

            //number of bytes read per second (frequency * bytes per block) : 4 bytes
            bw.Write(bytePerSec);

            //number of bytes per block (number of channels * bitsPerSample/8): 2 bytes
            bw.Write(bytePerBloc);

            //bits per sample: 2 bytes
            bw.Write(bitsPerSample); 

            //=====================================================================
            // sampled data chunk
            //=====================================================================

            //data block id - 4 bytes
            bw.Write(new byte[] { 0x64, 0x61, 0x74, 0x61 }); // Identifier = data

            //data size  - 4 bytes
            bw.Write(dataSize);

            return ms.ToArray();
        }

        //generate sine waveform of programmable frequency and amplitude
        static List<double> SineWaveGen(double freq, int sampleRate, int bitsPerSample, double amplitude, double dcOffset, double phaseOffset)
        {

            //creates a fixed length sample. This needs to be updated to create variable length waveforms.
            int samplePeriod = (int)(sampleRate / freq);


            List<double> oscillator = new List<double>();



            //generate sample by sample sine wave. essentially computes y[k] = sin(k*2*pi*dt)
            //where dt is the sampleTime and k is the currentSample 
            for (int i = 0; i < samplePeriod; i++)
            {

                double t = (double)i / sampleRate; // k*dt
                double sample = dcOffset + amplitude * Math.Sin(2.0 * Math.PI * freq * t + phaseOffset);

                oscillator.Add(sample);

            }


            return oscillator;

        }

        //calculates greatest common divisor using euclids algorithm
        static int GCD(int a, int b)
        {
            int big = Math.Abs(Math.Max(a, b));
            int small = Math.Abs(Math.Min(a, b));

            while (small != 0)
            {
                int rem = big % small; ;

                if (rem == 0) return small;

                big = small;
                small = rem;
            }
            return -1;
        }

        //Helper function: calculate lowest common multiple of 2 integers
        static int LCM(int a, int b)
        {
            return (int)((long)Math.Abs(a) * Math.Abs(b) / GCD(a, b));

        }


        //Helper Function: multiplies 2 arrays element by element.
        static List<double> MultiplicationEW(List<double> a, List<double> b)
        {

            //this is placeholder functionality and assumes the lists have the same number of samples
            //since these are cyclical waveforms I need to figure out a way to make it so that each can have variable length
            //and we loop back to the beginning if we reach the end before another. ( maybe find lowest common multiple).

            List<double> c = new List<double>();

            int longestLength = LCM(a.Count, b.Count);

            for (int i = 0; i < longestLength; i++)
            {
                c.Add(a[i % a.Count] * b[i % b.Count]);
            }


            return c;
        }

        //Helper Function: adds 2 arrays element by element.
        static List<double> AdditionEW(List<double> a, List<double> b)
        {

            //this is placeholder functionality and assumes the lists have the same number of samples
            //since these are cyclical waveforms I need to figure out a way to make it so that each can have variable length
            //and we loop back to the beginning if we reach the end before another. ( maybe find lowest common multiple).

            List<double> c = new List<double>();

            int longestLength = LCM(a.Count, b.Count);

            for (int i = 0; i < longestLength; i++)
            {
                c.Add(a[i % a.Count] + b[i % b.Count]);
            }


            return c;
        }

        static void WriteSample(BinaryWriter writer, double sample, int bitDepth)
        {
            switch (bitDepth)
            {
                case 8: writer.Write((byte)(sample * 127 + 128)); break;
                case 16: writer.Write((short)(sample * 32767)); break;
                case 32: writer.Write((int)(sample * 2147483647)); break;
            }
        }

        //helper function: converts a list of samples to a byte array for use in PCM 
        //encoded wav files
        static byte[] ConvertToByte(List<double> samples, short bitsPerSample)
        {
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);

            //convert from the sample 
            foreach (var sample in samples)
            {
                WriteSample(bw, sample, bitsPerSample);
            }

            return ms.ToArray();

        }

        //interleaves two byte arrays per sample to allow for 2 channel audio - possibly a way to make this generic to n channels,
        //haven't thought of a solution yet
        static byte[] TwoChInterleaving(List<double> leftChannel, List<double> rightChannel, short bitsPerSample)
        {
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);

            int bytesPerSample = bitsPerSample / 8;
   
            for (int i = 0; i < LCM(leftChannel.Count, rightChannel.Count); i++)
            {
                WriteSample(bw, leftChannel[i % leftChannel.Count], bitsPerSample);
                WriteSample(bw, rightChannel[i % rightChannel.Count], bitsPerSample);
            }

            return ms.ToArray();
        }

       
        // generates sinewave oscillator. currently hardcoded to play an A power chord with tremelo panning from left to write. 
        static byte[] GenerateOscillatorWav(float oscillatorFreq, int sampleRate, short bitsPerSample, short audioFormat,short chNumber)
        {
            //memory stream allows for soundloop to be 
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);

            //create message 
            var messageData = SineWaveGen(440, sampleRate, bitsPerSample, 0.5, 0.0, 0.0);

            var tremeloEnvelope = SineWaveGen(6.5, sampleRate, bitsPerSample, 0.5, 0.5, 0.0);



            var leftEnvelope = SineWaveGen(0.1, sampleRate, bitsPerSample, 1.0, 0.0, 0);
            var rightEnvelope = SineWaveGen(0.1, sampleRate, bitsPerSample, 1.0, 0.0, Math.PI / 2);


            var left = MultiplicationEW(messageData, leftEnvelope);
            var right = MultiplicationEW(messageData, rightEnvelope);

            //var oscillatorData = ConvertToByte(sineDataA4, bitsPerSample);
            var oscillatorData = TwoChInterleaving(left,right, bitsPerSample);

            int bytesPerSample = (int)bitsPerSample / 8;
            //generate wav data based on the generated waveform. currently has fixed length.
            var headerData = GenerateWavHeader(audioFormat, chNumber, sampleRate, bitsPerSample,oscillatorData.Length/bytesPerSample/chNumber );

            bw.Write(headerData);
            bw.Write(oscillatorData);

            bw.Flush();
            return ms.ToArray();
        }

        static void Main(string[] args)
        {

            //memory stream stores WAV file in ram.
            var ms = new MemoryStream(GenerateOscillatorWav(150f, 44100, (short)32, (short)1,(short)2));
            var sound = new System.Media.SoundPlayer();
            sound.Stream = ms;

            //loops wav file in memory stream until user input 
            sound.PlayLooping();
            Console.ReadLine();


        }
    }
}
