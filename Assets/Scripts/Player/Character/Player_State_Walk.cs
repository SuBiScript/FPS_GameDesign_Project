using System.Collections;
using System.Collections.Generic;
using PlayerFSM;
using UnityEngine;

public class Player_State_Walk : State
{
    private Rigidbody attachedRigidbody;
    private float WalkSpeed;
    private int coyoteFrames;

    protected override void OnStateInitialize(StateMachine machine)
    {
        base.OnStateInitialize(machine);
    }

    public override void OnStateTick(float deltaTime)
    {
        base.OnStateTick(deltaTime);
        if (attachedRigidbody.gameObject.transform.parent != null)
        {
            attachedRigidbody.velocity = Vector3.ClampMagnitude(attachedRigidbody.velocity,
                ((PlayerControllerFSM) Machine.characterController).characterProperties.WalkSpeed);
        }
    }

    public override void OnStateFixedTick(float fixedTime)
    {
        base.OnStateFixedTick(fixedTime);

        if (Machine.characterController.currentBrain.Direction != Vector3.zero ||
            Machine.characterController.transform.parent != null)
        {
            MovementManager.MoveRigidbody(
                attachedRigidbody,
                Machine.characterController.currentBrain.Direction,
                WalkSpeed,
                fixedTime);
        }
        else
        {
            ((PlayerControllerFSM) Machine.characterController).weaponAnimator.SetBool("Walking", false);
        }
    }

    public override void OnStateCheckTransition()
    {
        base.OnStateCheckTransition();
        if (Machine.characterController.currentBrain.Jumping)
        {
            Machine.SwitchState<Player_State_Jumping>();
            return;
        }

        var isGrounded = ((PlayerControllerFSM) Machine.characterController).IsGrounded();
        if (!isGrounded)
        {
            ((PlayerControllerFSM) Machine.characterController).weaponAnimator.SetBool("Walking", false);
            coyoteFrames++;
            if (coyoteFrames >= Machine.characterController.characterProperties.MaxCoyoteFrames)
            {
                Machine.SwitchState<Player_State_OnAir>();
            }
        }
        else if (coyoteFrames > 0)
        {
            coyoteFrames = 0;
        }
    }

    protected override void OnStateEnter()
    {
        base.OnStateEnter();
        WalkSpeed = Machine.characterController.characterProperties.WalkSpeed;
        attachedRigidbody = Machine.characterController.rigidbody;
        ((PlayerControllerFSM) Machine.characterController).ChangeMaterialFriction(true);
        coyoteFrames = 0;
        ((PlayerControllerFSM) Machine.characterController).enableAirControl = true;
        attachedRigidbody.velocity = new Vector3(0f, attachedRigidbody.velocity.y, 0f);
        //((PlayerControllerFSM) Machine.characterController).weaponAnimator.SetBool("OnGround", true);
    }

    protected override void OnStateExit()
    {
        base.OnStateExit();
        coyoteFrames = 0;
        ((PlayerControllerFSM) Machine.characterController).weaponAnimator.SetBool("Walking", false);
        //((PlayerControllerFSM)Machine.characterController).weaponAnimator.SetBool("OnGround", false);
    }
}