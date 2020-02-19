using System.Collections;
using System.Collections.Generic;
using PlayerFSM;
using UnityEngine;

public class StateOne : PlayerFSM.State
{
    protected override void OnStateInitialize(StateMachine machine)
    {
        base.OnStateInitialize(machine);
    }

    public override void OnStateTick()
    {
        base.OnStateTick();
    }

    public override void OnStateCheckTransition()
    {
        base.OnStateCheckTransition();
        if (!Machine.currentBrain.Interacting)
        {
            Machine.SwitchState<StateTwo>();
        }
    }

    protected override void OnStateEnter()
    {
        base.OnStateEnter();
    }

    protected override void OnStateExit()
    {
        base.OnStateExit();
    }
}