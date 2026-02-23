# Electric_Sheep_Synthesizer Overview:
This is an ongoing digital synthesizer project I decided to do at about 3 oclock in
the morning one day. It will be terrible but I am using it as a learning experience 
for C# and digital signal processing.

# Current implementation:
- The Current implementation has no way of streaming audio to the speaker as I haven't figured out how to do this
  yet with the new method of generating samples. Currently I have been saving waveform data to a csv and plotting
  in matlab to see what they do. 
- Several oscillators have been added (Sine,Triangle (includes sawtooth) and Square (PWM)
- Filter Classes have been added and simple filters implemented.
- Extension class added to allow streams to be defined in a fluent manner akin to a signal chain. 
- overloaded operators to allow for DC shifting, gain, mixing, modulating.
- Linear ADSR using a statemachine.

# To do:
- Implement AudioPlayer class which will use com api implementation of WASAPI for streaming samples to speaker
- Loading samples from stored files, I assume this will be a parser of some description that rips data and stores
  in a buffer.
- More complex filters
- more complicated envelopes
- Effects (reverb,chorus, delay etc); 
- FFT Functions
- Console Plotting function for waveforms and FFT data.
- Sequencer.
- GUI
