using System.Collections;
using System.Collections.Generic;
using PlayerFSM;
using UnityEngine;

public class Player_State_Jumping : State
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
    }

    protected override void OnStateEnter()
    {
        base.OnStateEnter();
        MovementManager.RigidbodyAddForce(
            Machine.characterController.rigidbody, 
            Machine.transform.up,
            Machine.characterController.characterProperties.JumpForce, 
            ForceMode.Impulse);
    }

    protected override void OnStateExit()
    {
        base.OnStateExit();
    }
}