﻿using System.Collections;
using System.Collections.Generic;
using PlayerFSM;
using UnityEngine;

public class Player_State_OnAir : State
{
    private Rigidbody attachedRigidbody;
    private float movementSpeed;
    
    protected override void OnStateInitialize(StateMachine machine)
    {
        base.OnStateInitialize(machine);
    }

    public override void OnStateTick(float deltaTime)
    {
        base.OnStateTick(deltaTime);
    }

    public override void OnStateFixedTick(float fixedDeltaTime)
    {
        base.OnStateFixedTick(fixedDeltaTime);
        if (Machine.characterController.currentBrain.Direction != Vector3.zero)
        {
            MovementManager.MoveRigidbody(
                attachedRigidbody,
                Machine.characterController.currentBrain.Direction,
                movementSpeed,
                fixedDeltaTime);
        }

    }

    public override void OnStateCheckTransition()
    {
        base.OnStateCheckTransition();
        if (attachedRigidbody.velocity.y <= 0f)
        {
            Machine.SwitchState<Player_State_Falling>();
        }
    }

    protected override void OnStateEnter()
    {
        base.OnStateEnter();
        attachedRigidbody = Machine.characterController.rigidbody;
        movementSpeed = ((PlayerControllerFSM) Machine.characterController).enableAirControl
            ? Machine.characterController.characterProperties.WalkSpeed
            : Machine.characterController.characterProperties.OnAirSpeed;
    }

    protected override void OnStateExit()
    {
        base.OnStateExit();
    }
}