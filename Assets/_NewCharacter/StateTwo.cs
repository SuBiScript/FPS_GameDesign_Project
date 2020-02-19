using System.Collections;
using System.Collections.Generic;
using PlayerFSM;
using UnityEngine;

public class StateTwo : PlayerFSM.State
{
    protected override void OnStateInitialize(StateMachine machine)
    {
        base.OnStateInitialize(machine);
    }

    public override void OnStateTick(float deltaTime)
    {
        base.OnStateTick(deltaTime);
    }

    public override void OnStateCheckTransition()
    {
        base.OnStateCheckTransition();
        if (Machine.characterController.currentBrain.Interacting)
        {
            Machine.SwitchState<StateOne>();
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
