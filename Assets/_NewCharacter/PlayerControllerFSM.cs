using PlayerFSM;
using UnityEngine;
using Weapon;

[RequireComponent(typeof(Brain))]
[RequireComponent(typeof(State))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerFSM : CharacterController , IPlatformJump
{
    public CameraFSMController cameraController;
    public bool enableAirControl;
    public WeaponScript equippedWeapon;
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
        if (equippedWeapon == null)
            equippedWeapon = GetComponentInChildren<WeaponScript>();
        characterProperties = characterProperties == null
            ? ScriptableObject.CreateInstance<CharacterProperties>()
            : Instantiate(characterProperties);
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
        
        if(currentBrain.Shooting)
            equippedWeapon.MainFire();
        if(currentBrain.Aiming)
            equippedWeapon.AltFire();
        
        if (stateMachine.isActiveAndEnabled)
            stateMachine.UpdateTick(Time.deltaTime);
        
                
    }

    private void FixedUpdate()
    {
        if (stateMachine.isActiveAndEnabled)
            stateMachine.FixedUpdateTick(Time.fixedDeltaTime);
        
        //Debug.Log(rigidbody.velocity);
    }

    public void MakeItJump()
    {
        enableAirControl = false;
        stateMachine.SwitchState<Player_State_OnAir>();
    }

    public bool IsGrounded() //TODO Improve method to compute ground detection
    {
        if (stateMachine.GetCurrentState is Player_State_OnAir || stateMachine.GetCurrentState is Player_State_Jumping)
            return false;
        return true;
    }
}