# Electric_Sheep_Synthesizer Overview:
This is an ongoing digital synthesizer project I decided to do at about 3 oclock in
the morning one day. It will be terrible but I am using it as a learning experience 
for C# and digital signal processing.

#Current implementation:
- Streaming a generated waveform to the speakers (This is a windows specific 
  implementation I believe).
  - Generates a .WAV stored in a memory stream to be streamed directly to the 
    speaker rather than saving it to the harddrive first.
  - Creates a .WAV header file based on the binary waveform data it receives.  
- Sinusoidal Oscillator with programmable frequency,phase offset, and DC offset.
- Two channel audio interleaving.
- Summing and multiplication of waveforms.

#To Do: 
- Implement different types of oscillator (square wave, saw tooth, and triangle).
- Move everything to classes.
- ADSR filter.
- Sample Loader.
- Sequencer.
- UI.
- Whatever pops into my head really. 
