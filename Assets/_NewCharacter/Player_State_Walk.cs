using System.Collections;
using System.Collections.Generic;
using PlayerFSM;
using UnityEngine;

public class Player_State_Walk : State
{
    protected override void OnStateInitialize(StateMachine machine)
    {
        base.OnStateInitialize(machine);
    }

    public override void OnStateTick()
    {
        base.OnStateTick();
    }

    public override void OnStateFixedTick()
    {
        base.OnStateFixedTick();
        if (Machine.currentBrain.Direction != Vector3.zero)
        {
            MovementManager.MoveRigidbody(Machine.characterController.rigidbody, Machine.currentBrain.Direction,
                Machine.characterController.characterProperties.WalkSpeed);
        }
    }

    public override void OnStateCheckTransition()
    {
        base.OnStateCheckTransition();
        if (Machine.currentBrain.Jumping)
        {
            Machine.SwitchState<Player_State_Jumping>();
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