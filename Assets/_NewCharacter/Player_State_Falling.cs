using System.Collections;
using System.Collections.Generic;
using PlayerFSM;
using UnityEngine;

public class Player_State_Falling : State
{
    private Rigidbody attachedRigidbody;
    private float onAirSpeed;

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
                onAirSpeed,
                fixedTime);
        }
    }

    public override void OnStateCheckTransition()
    {
        base.OnStateCheckTransition();
        if (attachedRigidbody.velocity.y >= 0f)
        {
            Machine.SwitchState<Player_State_Walk>();
        }
    }

    protected override void OnStateEnter()
    {
        base.OnStateEnter();
        attachedRigidbody = Machine.characterController.rigidbody;
        onAirSpeed = Machine.characterController.characterProperties.OnAirSpeed;
    }

    protected override void OnStateExit()
    {
        base.OnStateExit();
    }
}