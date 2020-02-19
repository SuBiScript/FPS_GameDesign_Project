using System.Collections;
using System.Collections.Generic;
using PlayerFSM;
using UnityEngine;

public class Player_State_Jumping : State
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
        if (attachedRigidbody.velocity.y <= 0f)
        {
            Machine.SwitchState<Player_State_Falling>();
            return;
        }
    }

    protected override void OnStateEnter()
    {
        base.OnStateEnter();
        attachedRigidbody = Machine.characterController.rigidbody;
        WalkSpeed = Machine.characterController.characterProperties.WalkSpeed;
        
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