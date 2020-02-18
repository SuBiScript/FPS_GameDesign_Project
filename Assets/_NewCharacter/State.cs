using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerFSM
{
    public abstract class State
    {
        public StateMachine Machine;

        public State()
        {
            
        }

        public virtual void Update()
        {
            
        }


        public virtual void OnStateInitialize(StateMachine machine)
        {
            Machine = machine;
        }

        public virtual void OnStateEnter()
        {
        }

        public virtual void OnStateExit()
        {
        }

        public static implicit operator bool(State state)
        {
            return state != null;
        }
    }
}