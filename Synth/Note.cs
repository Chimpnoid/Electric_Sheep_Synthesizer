using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricSheepSynth.Synth
{
    // Contains all possible notes in modern western music.
    // defined explicitly to ensure flats and sharps are read as the same.
    public enum Note
    {
        A = 0,
        As = 1, Bb = 1,
        B = 2,
        C = 3,
        Cs = 4, Db = 4,
        D = 5,
        Ds = 6, Eb = 6,
        E = 7,
        F = 8,
        Fs = 9, Gb = 9,
        G = 10,
        Gs = 11, Ab = 11
    }
}
