using Microsoft.Win32.SafeHandles;
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
            int fixedLengthPeriod = (int)(sampleRate / 1.0f);


            List<double> oscillator = new List<double>();



            //generate sample by sample sine wave. essentially computes y[k] = sin(k*2*pi*dt)
            //where dt is the sampleTime and k is the currentSample 
            for (int i = 0; i < fixedLengthPeriod; i++)
            {

                double t = (double)i / sampleRate; // k*dt
                double sample = dcOffset + amplitude * Math.Sin(2.0 * Math.PI * freq * t + phaseOffset);

                oscillator.Add(sample);

            }


            return oscillator;

        }

        //Helper Function: multiplies 2 arrays element by element.
        static List<double> MultiplicationEW(List<double> a, List<double> b)
        {

            //this is placeholder functionality and assumes the lists have the same number of samples
            //since these are cyclical waveforms I need to figure out a way to make it so that each can have variable length
            //and we loop back to the beginning if we reach the end before another. ( maybe find lowest common multiple).

            List<double> c = new List<double>();

            int longestLength = Math.Max(a.Count, b.Count);

            for(int i = 0; i < longestLength; i++)
            {
                c.Add(a[i] * b[i]);
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

            int longestLength = Math.Max(a.Count, b.Count);

            for (int i = 0; i < longestLength; i++)
            {
                c.Add(a[i] + b[i]);
            }


            return c;
        }

        //helper function: converts a list of samples to a byte array for use in PCM 
        //encoded wav files
        static byte[] ConvertToByte(List<double> samples, short bitsPerSample)
        {
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);

            //convert from the sample 
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

        //interleaves two byte arrays per sample to allow for 2 channel audio - possibly a way to make this generic to n channels,
        //haven't thought of a solution yet
        static byte[] TwoChInteleaving(byte[] leftChannel, byte[] rightChannel, short bitsPerSample)
        {
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);

            //calculates how many bytes of data are held in a sample.
            int bytesPerSample = (int)bitsPerSample / 8;

            //loops through each sample - assumes length of channel data are identical currently. 
            // samples are made up of bytesPerSample elements of the channel arrays
            for(int i = 0; i < leftChannel.Length; i+=bytesPerSample)
            {

                //left channel - writes however many bytes make up a sample to the stream
                for (int j = 0 ;j < bytesPerSample; j++)
                {
                    ms.WriteByte(leftChannel[i+j]); // j is indexing from the beginning of the current ample we are viewing
                }

                //right channel - follows by writing the second sample of bytes to the stream
                for (int j = 0; j < bytesPerSample; j++)
                {
                    
                    ms.WriteByte(rightChannel[i+j]);
                }
            }

            return ms.ToArray();
        }

        // generates sinewave oscillator. currently hardcoded to play an A power chord with tremelo panning from left to write. 
        static byte[] CreateOscillatorLoop(float oscillatorFreq,int sampleRate,short bitsPerSample, short audioFormat)
        {
            //memory stream allows for soundloop to be 
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);

            //create message and envelope for ring mod
            var sineDataA4 = SineWaveGen(440,sampleRate, bitsPerSample,0.1,0.0,0.0);
            var sineDataE5 = SineWaveGen(660,sampleRate, bitsPerSample,0.1,0.0,0.0);

            var sineDataAPowerChord = AdditionEW(sineDataA4, sineDataE5);

            var tremeloEnvelope = SineWaveGen(25, sampleRate, bitsPerSample, 0.5, 0.5, 0.0);

            //ringmod
            var messageData = MultiplicationEW(sineDataAPowerChord, tremeloEnvelope);

            //generates panning envelopes to ensure constant power panning.
            var envelopeLeft = SineWaveGen(1,sampleRate,bitsPerSample,0.5,0.5,0.0);
            var envelopeRight = SineWaveGen(1, sampleRate, bitsPerSample, 0.5, 0.5, Math.PI/2);

            var leftChannel  = MultiplicationEW(messageData , envelopeLeft);
            var rightChannel = MultiplicationEW(messageData , envelopeRight);

            var oscillatorData = TwoChInteleaving(ConvertToByte(leftChannel,bitsPerSample),ConvertToByte(rightChannel,bitsPerSample),bitsPerSample);

            //generate wav data based on the generated waveform. currently has fixed length.
            var headerData = GenerateWavHeader(audioFormat,(short)2,sampleRate,bitsPerSample, (int)(sampleRate / 1.0f));

            bw.Write(headerData);
            bw.Write(oscillatorData);

            bw.Flush(); 

            return ms.ToArray(); 
        }

        static void Main(string[] args)
        {
            
            //memory stream stores WAV file in ram.
            var ms = new MemoryStream(CreateOscillatorLoop(150f,44100,(short)32,(short)1));
            var sound = new System.Media.SoundPlayer();
            sound.Stream = ms;

            //loops wav file in memory stream until user input 
            sound.PlayLooping();
            Console.ReadLine();
        }
    }
}
