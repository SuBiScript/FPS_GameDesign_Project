using System;
using PlayerFSM;
using UnityEngine;
using Weapon;

[RequireComponent(typeof(Brain))]
[RequireComponent(typeof(State))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerFSM : CharacterController, IPlatformJump
{
    public CameraFSMController cameraController;
    public bool enableAirControl;
    public WeaponScript equippedWeapon;
    public LayerMask walkableLayers;
    public Collider attachedCollider;
    [Range(0f, 1f)] public float groundDetectionRange = 0.2f;

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
        if (cameraController == null)
            cameraController = GetComponent<CameraFSMController>();
        if (equippedWeapon == null)
            equippedWeapon = GetComponentInChildren<WeaponScript>();
        characterProperties = characterProperties == null
            ? ScriptableObject.CreateInstance<CharacterProperties>()
            : Instantiate(characterProperties);

        attachedCollider = GetComponent<Collider>();
    }

    private void Start()
    {
        enableAirControl = true;
        equippedWeapon.Init(this);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            stateMachine.enabled = !stateMachine.enabled;
        }

        if (currentBrain.isActiveAndEnabled)
            currentBrain.GetActions();

        if (currentBrain.Shooting)
            equippedWeapon.MainFire();
        if (currentBrain.Aiming)
            equippedWeapon.AltFire();

        if (stateMachine.isActiveAndEnabled)
            stateMachine.UpdateTick(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (stateMachine.isActiveAndEnabled)
            stateMachine.FixedUpdateTick(Time.fixedDeltaTime);
    }

    public void MakeItJump(bool airControl)
    {
        enableAirControl = airControl;
        stateMachine.SwitchState<Player_State_OnAir>();
    }

    public bool IsGrounded()
    {
        Bounds bounds = attachedCollider.bounds;
        Vector3 boundExtents = bounds.extents;
        if (Physics.BoxCast(bounds.center, boundExtents, Vector3.down,
            Quaternion.identity, groundDetectionRange, walkableLayers))
            return true;
        return false;
    }

}