using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricSheepSynth.MiscClasses
{
    internal class FiniteStateMachine<TState,TTrigger>
        where TState : notnull
        where TTrigger : notnull
    {

        private TState currentState;

        // stores a list of current state and a trigger and what transition occurs with that trigger from that state.
        private readonly Dictionary<(TState, TTrigger),TState> transitions = new();
        
        // stores what action(void lambda) is done when a state is entered from outside the state
        private readonly Dictionary<TState, Action> onEnter = new();

        // stores what acttion(void lambda( is done when reentering a state from inside the state
        private readonly Dictionary<TState, Action> onReEnter = new();
        
    
        public FiniteStateMachine(TState initialState)
        {
            this.currentState = initialState;
        }

        
        // adds transition trigger pairs to dictionary
        public void addTransition(TState from, TTrigger trigger,TState to)
        {
            this.transitions[(from, trigger)] = to;
        }

        // adds what is done when a state is entered from outside to dictionary
        public void OnEnter(TState state,Action action)
        {
            this.onEnter[(state)] = action;
        }

        // adds what is done when a state is reentered from inside state to dictionary 
        public void OnReEnter(TState state,Action action)
        {
            this.onReEnter[(state)] = action;
        }

        // runs state transition
        public void Fire(TTrigger trigger)
        {
            TState nextState = this.transitions[(this.currentState, trigger)];

            if (this.currentState.Equals(nextState))
            {
                this.onReEnter[this.currentState]();
            }
            else
            {
                this.currentState = nextState;
                this.onEnter[this.currentState]();
            }
        }

        // initialises statemachine using the currently stored state. this should only be ran once when starting the sm.
        // runs the stored onEnter operator if one is present.
        public void Start()
        {
            if (this.onEnter.ContainsKey(this.currentState))
            {
                this.onEnter[this.currentState]();
            }
        }

    }
}
