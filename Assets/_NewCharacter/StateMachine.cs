using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace PlayerFSM
{
    public class StateMachine : MonoBehaviour
    {
        protected List<State> statesList = new List<State>();
        protected State currentState;

        public Brain defaultBrain;
        public Brain currentBrain { get; private set; }

        private void Update()
        {
            currentState.Update();
        }

        /// <summary>
        /// Switch the currentstate to a specific new State.
        /// </summary>
        /// <param name="newState">
        /// The state object to set as the currentState</param>
        /// <returns></returns>
        protected virtual bool SwitchState(State newState)
        {
            if (newState && newState != currentState)
            {
                if (currentState) currentState.OnStateExit();
                currentState = newState;
                currentState.OnStateEnter();
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// Switches the currentState to a State of a given type, checking if it already exists.
        /// </summary>
        /// <typeparam name="TState">
        /// The type of the State to use for the currentState</typeparam>
        /// <returns>Whether the State was changed</returns>
        public virtual bool SwitchState<TState>() where TState : State, new()
        {
            //if the state can be found in the list of states 
            //already created, switch to the existing version
            foreach (State state in statesList)
            {
                if (state is TState)
                {
                    return SwitchState(state);
                }
            }

            //if the state is not found in the list, 
            //make a new instance
            State newState = new TState();
            newState.OnStateInitialize(this);
            statesList.Add(newState);
            return SwitchState(newState);
        }
    }
}