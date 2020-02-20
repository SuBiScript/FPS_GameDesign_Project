using System.Collections;
using System.Collections.Generic;
using PlayerFSM;
using UnityEngine;

public class Player_State_OnAir : State
{
    private Rigidbody attachedRigidbody;
    private float onAirSpeed;
    private float timer;
    protected override void OnStateInitialize(StateMachine machine)
    {
        base.OnStateInitialize(machine);
    }

    public override void OnStateTick(float deltaTime)
    {
        base.OnStateTick(deltaTime);
        if (timer > 0f) timer -= deltaTime;
    }

    public override void OnStateFixedTick(float fixedDeltaTime)
    {
        base.OnStateFixedTick(fixedDeltaTime);
        if (Machine.characterController.currentBrain.Direction != Vector3.zero)
        {
            MovementManager.MoveRigidbody(
                attachedRigidbody,
                Machine.characterController.currentBrain.Direction,
                onAirSpeed,
                fixedDeltaTime);
        }

    }

    public override void OnStateCheckTransition()
    {
        base.OnStateCheckTransition();
        if (((PlayerControllerFSM)Machine.characterController).IsGrounded() && timer <= 0f)
        {
            Machine.SwitchState<Player_State_Walk>();
        }
    }

    protected override void OnStateEnter()
    {
        base.OnStateEnter();
        timer = 0.2f; //Safety Timer
        attachedRigidbody = Machine.characterController.rigidbody;
        onAirSpeed = ((PlayerControllerFSM) Machine.characterController).enableAirControl
            ? Machine.characterController.characterProperties.WalkSpeed
            : Machine.characterController.characterProperties.OnAirSpeed;
    }

    protected override void OnStateExit()
    {
        base.OnStateExit();
        ((PlayerControllerFSM) Machine.characterController).enableAirControl = true;
    }
}