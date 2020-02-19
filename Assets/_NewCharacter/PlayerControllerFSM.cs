using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using PlayerFSM;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

[RequireComponent(typeof(Brain))]
[RequireComponent(typeof(State))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerFSM : CharacterController
{
    public CameraFSMController cameraController;
    public void Awake()
    {
        if (defaultBrain == null)
        {
            defaultBrain = GetComponent<Brain>();
        }

        currentBrain = defaultBrain;
        if (defaultState == null)
            defaultState = GetComponent<State>();
        if (stateMachine == null)
            stateMachine = GetComponent<StateMachine>();
        if (rigidbody == null)
            rigidbody = GetComponent<Rigidbody>();
        if(cameraController == null)
            cameraController = GetComponent<CameraFSMController>();
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

        if (currentBrain.isActiveAndEnabled)
            currentBrain.GetActions();
        if (stateMachine.isActiveAndEnabled)
            stateMachine.UpdateTick(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (stateMachine.isActiveAndEnabled)
            stateMachine.FixedUpdateTick(Time.fixedDeltaTime);
        
        //Debug.Log(rigidbody.velocity);
    }
}