﻿using System;
using System.Collections;
using System.Collections.Generic;
using PlayerFSM;
using UnityEngine;

public class Player_State_Jumping : State
{
    private Rigidbody attachedRigidbody;

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
    }

    public override void OnStateCheckTransition()
    {
        base.OnStateCheckTransition();
        Machine.SwitchState<Player_State_OnAir>();
    }

    protected override void OnStateEnter()
    {
        base.OnStateEnter();
        attachedRigidbody = Machine.characterController.rigidbody;

        if (((PlayerControllerFSM) Machine.characterController).AirPropulsed)
        {
            var MovingPlatformAsParent = attachedRigidbody.transform.parent.GetComponentInChildren<MovingPlatform>();
            Machine.characterController.characterProperties.TemporalPropulsionSpeed =
                MovingPlatformAsParent.movementDirection.normalized != Vector3.zero
                    ? MovingPlatformAsParent.m_MaxSpeed
                    : Machine.characterController.characterProperties.AirControlSpeed;
        }
        else
        {
            Machine.characterController.characterProperties.TemporalPropulsionSpeed = 1f;
        }

        attachedRigidbody.gameObject.transform.parent = null;

        RemoveVerticalSpeed();
        ((PlayerControllerFSM) Machine.characterController).enableAirControl = true;
        ((PlayerControllerFSM) Machine.characterController).ChangeMaterialFriction(false);
        MovementManager.RigidbodyAddForce(
            Machine.characterController.rigidbody,
            Machine.transform.up,
            Machine.characterController.characterProperties.JumpForce,
            ForceMode.Impulse);
        ((PlayerControllerFSM) Machine.characterController).weaponAnimator.SetTrigger("Jumping");
        AudioManager.instance.Play("Jump");
    }

    private void RemoveVerticalSpeed()
    {
        Vector3 rbSpeed = attachedRigidbody.velocity;
        rbSpeed.y = 0f;
        attachedRigidbody.velocity = rbSpeed;
    }

    protected override void OnStateExit()
    {
        base.OnStateExit();
    }
}