using PlayerFSM;
using UnityEngine;
using Weapon;

[RequireComponent(typeof(Brain))]
[RequireComponent(typeof(State))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerFSM : CharacterController
{
    public CameraFSMController cameraController;
    public bool enableAirControl;
    public WeaponScript equippedWeapon;
    public Collider attachedCollider;
    public Transform m_PitchControllerTransform;

    [Header("Ground Detection")] public Transform groundPosition;
    [Range(0.1f, 10f)] public float castRadius = 1f;
    public LayerMask walkableLayers;
    public bool isPlayerGrounded { get; private set; }

    public PhysicMaterial onAirMaterial;
    public PhysicMaterial onGroundMaterial;

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
        /*if (Input.GetKeyDown(KeyCode.Escape))
        {
            stateMachine.enabled = !stateMachine.enabled;
        }*/

        isPlayerGrounded = CheckOnGround();

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
        if (stateMachine.isActiveAndEnabled) //TODO Reenable stop functionality with GameController
            stateMachine.FixedUpdateTick(Time.fixedDeltaTime);
    }

    private bool CheckOnGround()
    {
        bool foundGround;
        var list = Physics.OverlapSphere(groundPosition.position, castRadius, walkableLayers);
        return list.Length > 0;
    }

    public bool IsGrounded() => isPlayerGrounded;

    public void ChangeMaterialFriction(bool grounded)
    {
        if (grounded)
        {
            attachedCollider.material = onGroundMaterial;
        }
        else
        {
            attachedCollider.material = onAirMaterial;
        }
    }
}