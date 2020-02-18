﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerBrain : Brain
{
    public InputNames inputNames;

    private void Start()
    {
        if(inputNames == null) Debug.LogWarning($"Controls in {gameObject.name} could not work.", gameObject);
    }

    public override void GetActions()
    {
        //All available buttons
        Shooting = Input.GetButtonDown(inputNames.Shoot ?? "Shoot");
        Aiming = Input.GetButtonDown(inputNames.ChangeColor ?? "Change Color");
        Jumping = Input.GetButtonDown(inputNames.Jump ?? "Jump");
        Running = Input.GetButtonDown(inputNames.Run ?? "Run");
        Crouching = Input.GetButtonDown(inputNames.Crouch ?? "Crouch");
        Interacting = Input.GetButton(inputNames.Interact ?? "Interact");

        //Directional input control.
        Direction = new Vector2(
            Input.GetAxis(inputNames.HorizontalMovement ?? "Horizontal"),
            Input.GetAxis(inputNames.ForwardMovement ?? "Vertical")
            );
        Forward = Direction.y > 0f;
        Backwards = Direction.y < 0f;
        Right = Direction.x > 0f;
        Left = Direction.x < 0f;
    }
}