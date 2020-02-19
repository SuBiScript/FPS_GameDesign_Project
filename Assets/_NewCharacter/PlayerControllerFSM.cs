using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using PlayerFSM;
using UnityEngine;

[RequireComponent(typeof(Brain))]
[RequireComponent(typeof(State))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerFSM : CharacterController
{
    
    public void Awake()
    {
        if (defaultBrain == null)
            defaultBrain = GetComponent<Brain>();
        if (defaultState == null)
            defaultState = GetComponent<State>();
        if (stateMachine == null)
            stateMachine = GetComponent<StateMachine>();
        if (rigidbody == null)
            rigidbody = GetComponent<Rigidbody>();
        characterProperties = characterProperties == null
            ? ScriptableObject.CreateInstance<CharacterProperties>()
            : Instantiate(characterProperties);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            stateMachine.enabled = !stateMachine.enabled;
        }
    }
}