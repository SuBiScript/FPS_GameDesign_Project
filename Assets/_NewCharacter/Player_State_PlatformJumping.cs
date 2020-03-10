using System.Collections;
using System.Collections.Generic;
using ColorPanels;
using PlayerFSM;
using UnityEngine;

public class Player_State_PlatformJumping : State
{
    private Rigidbody attachedRigidbody;
    private byte timer;
    private float movementSpeed;

    protected override void OnStateInitialize(StateMachine machine)
    {
        base.OnStateInitialize(machine);
    }

    public override void OnStateTick(float deltaTime)
    {
        base.OnStateTick(deltaTime);
        if (timer > 0) timer--;
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
        if (timer <= 0)
        {
            Machine.SwitchState<Player_State_OnAir>();
        }
    }

    protected override void OnStateEnter()
    {
        base.OnStateEnter();
        attachedRigidbody = Machine.characterController.rigidbody;

        attachedRigidbody.velocity = Vector3.zero;

        ColorPanelEffects.PlayerJump(((PlayerControllerFSM) Machine.characterController), attachedRigidbody);

        ((PlayerControllerFSM) Machine.characterController).ChangeMaterialFriction(false);

        ((PlayerControllerFSM) Machine.characterController).weaponAnimator.SetTrigger("Jumping");
        
        movementSpeed =  movementSpeed = ((PlayerControllerFSM) Machine.characterController).enableAirControl
            ? Machine.characterController.characterProperties.AirControlSpeed
            : Machine.characterController.characterProperties.OnAirSpeed;

        AudioManager.instance.Play("JumpPlatform");

        timer = 5;
    }

    protected override void OnStateExit()
    {
        base.OnStateExit();
    }
}