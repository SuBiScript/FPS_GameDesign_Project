using System.Collections;
using System.Collections.Generic;
using PlayerFSM;
using UnityEngine;

public class Player_State_Walk : State
{
    private Rigidbody attachedRigidbody;
    private float WalkSpeed;

    protected override void OnStateInitialize(StateMachine machine)
    {
        base.OnStateInitialize(machine);
    }

    public override void OnStateTick(float deltaTime)
    {
        base.OnStateTick(deltaTime);
    }

    public override void OnStateFixedTick(float fixedTime)
    {
        base.OnStateFixedTick(fixedTime);

        if (Machine.characterController.currentBrain.Direction != Vector3.zero)
        {
            MovementManager.MoveRigidbody(
                attachedRigidbody,
                Machine.characterController.currentBrain.Direction,
                WalkSpeed,
                fixedTime);
        }
    }

    public override void OnStateCheckTransition()
    {
        base.OnStateCheckTransition();
        if (Machine.characterController.currentBrain.Jumping)
        {
            Machine.SwitchState<Player_State_Jumping>();
        }
    }

    protected override void OnStateEnter()
    {
        base.OnStateEnter();
        WalkSpeed = Machine.characterController.characterProperties.WalkSpeed;
        attachedRigidbody = Machine.characterController.rigidbody;
    }

    protected override void OnStateExit()
    {
        base.OnStateExit();
    }
}