using System.Collections;
using System.Collections.Generic;
using ColorPanels;
using PlayerFSM;
using UnityEngine;

public class Player_State_PlatformJumping : State
{
    private Rigidbody attachedRigidbody;
    private float timer;

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
        ColorPanelEffects.PlayerJump(((PlayerControllerFSM)Machine.characterController), attachedRigidbody);
        ((PlayerControllerFSM)Machine.characterController).ChangeMaterialFriction(false);
        ((PlayerControllerFSM) Machine.characterController).weaponAnimator.SetTrigger("Jumping");

    }

    protected override void OnStateExit()
    {
        base.OnStateExit();
    }
}