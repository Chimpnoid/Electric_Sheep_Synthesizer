using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ElectricSheepSynth.MiscClasses;

namespace ElectricSheepSynth.Synth.EnvelopeClasses
{

    // implements a simple linear Attack Decay Sustain envelope that can be applied to an audio object. 
    // it is a linear ADSR so stages have a simple y =  mx + c formula for modulating amplitude.
    // it currently implements a fixed gating time ( key is pressed for fixed period), this needs to be 
    // extended to include a keyoff function for gating the signal. 
    internal class ADSRLinearEnvelope:Envelopes
    {
        enum States
        {
            Attack,
            Decay,
            Sustain,
            Release,
            Off
        }

        enum Triggers
        {
            Normal,
            Reenter,
            Interrupted
        }


        FiniteStateMachine<States,Triggers> sm = new FiniteStateMachine<States,Triggers>(States.Attack);

        private States currentState;
        private bool keyOn;
        private double curEnvelopeGain;
        private double attackPeriod;
        private double decayPeriod;
        private double releasePeriod;
        private double lSustain;

        private double m;
        private double c;

        // ADSR is implemented using a simple finite statemachine. this allows for transitions from state to state of the filter in a normal fashion but also an interrupt. 
        public ADSRLinearEnvelope(IAudioSample wave, double sr, double TAtt, double TDec,double TRel,double Lsus) : base(wave, sr)
        {
            this.curEnvelopeGain = 0.0;
            this.currentState = States.Attack;
            this.keyOn = true;

            this.attackPeriod = TAtt;
            this.decayPeriod = TDec;
            this.releasePeriod = TRel;
            this.lSustain = Lsus;

            // add normal transitions between states attack -> decay -> Sustain -> Release -> off -> attack
            this.sm.addTransition(States.Attack, Triggers.Normal, States.Decay);
            this.sm.addTransition(States.Decay, Triggers.Normal, States.Sustain);
            this.sm.addTransition(States.Sustain, Triggers.Normal, States.Release);
            this.sm.addTransition(States.Release, Triggers.Normal, States.Off);
            this.sm.addTransition(States.Off, Triggers.Normal, States.Attack);

            // add interrupted state transitions (key off before sustain is reached or key on when in release)
            this.sm.addTransition(States.Attack, Triggers.Interrupted, States.Release);
            this.sm.addTransition(States.Decay, Triggers.Interrupted, States.Release);
            this.sm.addTransition(States.Release, Triggers.Interrupted, States.Attack);

            // re-entering states. 
            this.sm.addTransition(States.Attack, Triggers.Reenter, States.Attack);
            this.sm.addTransition(States.Decay, Triggers.Reenter, States.Decay);
            this.sm.addTransition(States.Sustain, Triggers.Reenter, States.Sustain);
            this.sm.addTransition(States.Release, Triggers.Reenter, States.Release);
            this.sm.addTransition(States.Off, Triggers.Reenter, States.Off);


            // define what happens when a state is entered from outside the state for each state
            foreach(States state in Enum.GetValues(typeof(States)))
            {
                this.sm.OnEnter(state,() => 
                { 
                    // time is reset to zero on each transition. shifts y axis so intercept is easier to determine
                    ResetTime();
                    switch (state)
                    {
                        // using curEnvelopegain means the gradient will change if we go back into attack halfway through
                        // release state
                        case States.Attack:
                            this.currentState = States.Attack;
                            this.m = (1.0 - this.curEnvelopeGain) / this.attackPeriod;
                            this.c = this.curEnvelopeGain;
                            break;
                        // ideally cut-off will be one but this prevents a slight discontinuity if L is slightly bigger than 1.0
                        case States.Decay:
                            this.currentState = States.Decay;
                            this.m = (this.lSustain - 1.0) / this.decayPeriod;
                            this.c = this.curEnvelopeGain;
                            break;
                        // sustain has no gradient and its intercept is defined
                        case States.Sustain:
                            this.currentState = States.Sustain;
                            this.m = 0.0;
                            this.c = this.lSustain;
                            break;
                        // release gradient and cut-off depend on where it is entered from
                        case States.Release:
                            this.currentState = States.Release;
                            this.m = -this.curEnvelopeGain / this.releasePeriod;
                            this.c = this.curEnvelopeGain;
                            break;
                        // gain and intercept is zero when off
                        case States.Off:
                            this.currentState = States.Off;
                            this.m = 0.0;
                            this.c = 0.0;
                            break;
                    }

                    this.curEnvelopeGain = this.m * this.timeAccumulator + this.c;

                    IncrementTime();
                });

                //define rentry from the same state. timer doesn't need resetting and the grad and intercept 
                // don't change
                this.sm.OnReEnter(state, () => 
                {
                    this.curEnvelopeGain = this.m * this.timeAccumulator + this.c;
                    IncrementTime();
                });

                // ensure sm begins so that m0 and c0 are defined as not zero. triggers onEnter for Attack state initially
                this.sm.Start();
            }
        }

        // implements checks for transitions and output calculators
        private void SwitchingLogic()
        {
            switch (this.currentState)
            {
                case States.Attack:

                    // if keyOff occurs mid way through attack we need to jump straight to release
                    if (keyOn)
                    {
                        if(this.curEnvelopeGain  < 1.0)
                        {
                            // attack before envelope reaches a peak
                            this.sm.Fire(Triggers.Reenter);
                        }
                        else
                        {
                            // otherwise transition to decay
                            this.sm.Fire(Triggers.Normal);
                        }
                    }
                    else
                    {
                        // jump to release
                        this.sm.Fire(Triggers.Interrupted);
                    }
                    break;
                case States.Decay:
                    // if keyOff occurs mid way through decay we need to jump straight to release
                    if (keyOn)
                    {
                        if(this.curEnvelopeGain > this.lSustain)
                        {
                            // decay before sustain level is reached
                            this.sm.Fire(Triggers.Reenter);
                        }
                        else
                        {
                            //transition to sustain
                            this.sm.Fire(Triggers.Normal);
                        }
                    }
                    else
                    {
                        // jump to release
                        this.sm.Fire(Triggers.Interrupted);
                    }
                    break;
                case States.Sustain:
                    if (keyOn)
                    {
                        //sustain while key pressed
                        this.sm.Fire(Triggers.Reenter);
                    }
                    else
                    {
                        // jump to release
                        this.sm.Fire(Triggers.Normal);
                    }
                    break;
                case States.Release:

                    // if keyon during release jump back to attack
                    if (!keyOn)
                    {
                        if(this.curEnvelopeGain > 0.0)
                        {
                            // release until envelope reaches zero
                            this.sm.Fire(Triggers.Reenter);
                        }
                        else
                        {
                            // jump to off
                            this.sm.Fire(Triggers.Normal);
                        }
                    }
                    else
                    {
                        // jump to attack
                        this.sm.Fire(Triggers.Interrupted);
                    }
                    break;
                case States.Off:

                    // stay off until keyon
                    if (!keyOn)
                    {
                        // stay off
                        this.sm.Fire(Triggers.Reenter);
                    }
                    else
                    {
                        // jump to attack
                        this.sm.Fire(Triggers.Normal);
                    }

                    break;
            }
        }
      
        // keyon and keyoff override samplenode definitions to turn envelop on and off. 

        public override void KeyOn()
        {
            this.keyOn = true; 
        }
        public override void KeyOff()
        {
            this.keyOn = false;
        }

        public override double GetNextSample()
        {
            SwitchingLogic();
            return this.waveform.GetNextSample() * this.curEnvelopeGain;
        }



    }
}
