using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Intrinsics.X86;




namespace ElectricSheepSynth
{
    internal class Program
    {
        // Generate the WAV standard file header. This includes all necessary preamble to 
        // be read as a WAV without the wav data.  
        static byte[] generateWavHeader(short audioFormat,short nmbrChannels,int sampleRate,short bitsPerSample,int sampleNumber)
        {

            
            short bytePerBloc = (short)(nmbrChannels * bitsPerSample / 8);
            int dataSize = sampleNumber*bytePerBloc;
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

        //static byte[] sineWaveData(float freq,int sampleRate,int bitsPerSample,double amplitude,double offset)
        //{
        //    int samplePerWave= (int)(sampleRate/1.0f);

        //    var ms = new MemoryStream();
        //    var bw = new BinaryWriter(ms);

        //    for(int i = 0; i < samplePerWave; i++)
        //    {

        //        double t = (double)i / sampleRate;

        //        double sample = offset + amplitude * Math.Sin(2.0 * Math.PI * freq * t);

        //        switch (bitsPerSample)
        //        {
        //            case 8:
        //                // 8-bit WAV is unsigned, 0-255, silence at 128
        //                bw.Write((byte)(sample * 127 + 128));
        //                break;
        //            case 16:
        //                // 16-bit WAV is signed
        //                bw.Write((short)(sample * 32767));
        //                break;
        //            case 32:
        //                // 32-bit WAV PCM is signed int
        //                bw.Write((int)(sample * 2147483647));
        //                break;
        //        }

        //    }

        //    bw.Flush();
        //    return ms.ToArray();

        //}

        static List<double> sineWaveGen(double freq, int sampleRate, int bitsPerSample, double amplitude, double offset)
        {

            int samplePerWave = (int)(sampleRate / 1.0f);


            List<double> oscillator = new List<double>();

            for (int i = 0; i < samplePerWave; i++)
            {

                double t = (double)i / sampleRate;

                double sample = offset + amplitude * Math.Sin(2.0 * Math.PI * freq * t);

                oscillator.Add(sample);

            }


            return oscillator;

        }

        static List<double> elementWiseMultiplication(List<double> a, List<double> b)
        {
            List<double> c = new List<double>();

            int longestLength = Math.Max(a.Count, b.Count);

            for(int i = 0; i < longestLength; i++)
            {
                c.Add(a[i] * b[i]);
            }


            return c;
        }

        static byte[] convertToByte(List<double> samples, short bitsPerSample)
        {
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);

            foreach(var sample in samples)
            {
                switch (bitsPerSample)
                {
                    case 8:
                        // 8-bit WAV is unsigned, 0-255, silence at 128
                        bw.Write((byte)(sample * 127 + 128));
                        break;
                    case 16:
                        // 16-bit WAV is signed
                        bw.Write((short)(sample * 32767));
                        break;
                    case 32:
                        // 32-bit WAV PCM is signed int
                        bw.Write((int)(sample * 2147483647));
                        break;
                }
            }

            return ms.ToArray();

        } 



        //static byte[] MultiplyAudio16(byte[] a, byte[] b)
        //{
        //    int numSamples = a.Length / 2;
        //    var ms = new MemoryStream();
        //    var bw = new BinaryWriter(ms);

        //    for (int i = 0; i < numSamples; i++)
        //    {
        //        short sA = BitConverter.ToInt16(a, i * 2);
        //        short sB = BitConverter.ToInt16(b, i * 2);
        //        // Normalize to -1..1, multiply, scale back
        //        double result = (sA / 32767.0) * (sB / 32767.0);
        //        bw.Write((short)(result * 32767));
        //    }

        //    bw.Flush();
        //    return ms.ToArray();
        //}

        static byte[] createOscillatorLoop(float oscillatorFreq,int sampleRate,short bitsPerSample, short audioFormat)
        {
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);

            var sineData = sineWaveGen(oscillatorFreq,sampleRate, bitsPerSample,2.0,0.0);
            var envelopeData = sineWaveGen(10, sampleRate, bitsPerSample,0.5,0.5);
            var oscillatorData = convertToByte(elementWiseMultiplication(sineData, envelopeData),bitsPerSample);

            var headerData = generateWavHeader(audioFormat,(short)1,sampleRate,bitsPerSample, (int)(sampleRate / 1.0f));

            bw.Write(headerData);
            bw.Write(oscillatorData);

            bw.Flush(); 

            return ms.ToArray(); 
        }

        static void Main(string[] args)
        {
            
            var ms = new MemoryStream(createOscillatorLoop(880f,44100,(short)32,(short)1));
            var sound = new System.Media.SoundPlayer();
            sound.Stream = ms;

            sound.PlayLooping();

            Console.ReadLine();
        }
    }
}
