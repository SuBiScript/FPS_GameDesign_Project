using ColorPanels;
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
    public bool enableAirControl { get; set; }
    public WeaponScript equippedWeapon;
    public Collider attachedCollider;
    public Transform m_PitchControllerTransform;

    [Header("Sound settings")]
    public AudioClip[] m_FootstepSounds;
    public AudioClip m_JumpSound;
    public AudioClip m_LandSound;
    [Range(0.1f, 2.0f)] public float m_StepTimeWaking;
    private float m_StepTimeRange;
    private float m_StepTime = 0.0f;
    private AudioSource m_AudioSource;

    [Header("Ground Detection")] public Transform groundPosition;
    [Range(0.1f, 10f)] public float castRadius = 1f;
    public LayerMask walkableLayers;
    public bool isPlayerGrounded { get; private set; }

    public PhysicMaterial onAirMaterial;
    public PhysicMaterial onGroundMaterial;

    public Animator weaponAnimator;

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
        weaponAnimator = equippedWeapon.weaponMeshRenderer.gameObject.GetComponent<Animator>();

        m_AudioSource = GetComponent<AudioSource>();
        m_StepTime = Time.time + m_StepTimeRange;
        m_StepTimeRange = m_StepTimeWaking;
    }

    private void Start()
    {
        enableAirControl = true;
        equippedWeapon.Init(this);
    }

    void Update()
    {
        isPlayerGrounded = CheckOnGround();

        if (currentBrain.isActiveAndEnabled)
            currentBrain.GetActions();

        if (currentBrain.Shooting && !GameController.Instance.m_GamePaused)
        {
            equippedWeapon.MainFire();
        }

        if (currentBrain.Aiming && !GameController.Instance.m_GamePaused)
        {
            if (equippedWeapon.AltFire())
                weaponAnimator.SetTrigger("ChangeColor");
        }

        if (stateMachine.isActiveAndEnabled && !GameController.Instance.m_GamePaused &&
            !GameController.Instance.m_PlayerDied && GameController.Instance.m_IntroFinished)
        {
            stateMachine.UpdateTick(Time.deltaTime);
        }

        if (currentBrain.Direction.magnitude > .1f && isPlayerGrounded && m_StepTime <= Time.time && !GameController.Instance.m_PlayerDied && GameController.Instance.m_IntroFinished)
        {
            PlayFootStepAudio();
            m_StepTime = Time.time + m_StepTimeRange;
        }
    }

    private void FixedUpdate()
    {
        if (stateMachine.isActiveAndEnabled && !GameController.Instance.m_GamePaused &&
            GameController.Instance.m_IntroFinished) //TODO Reenable stop functionality with GameController
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

    private void PlayFootStepAudio()
    {
        if (!IsGrounded())
            return;

        int i = Random.Range(1, m_FootstepSounds.Length);
        m_AudioSource.clip = m_FootstepSounds[i];
        m_AudioSource.PlayOneShot(m_AudioSource.clip);
        m_FootstepSounds[i] = m_FootstepSounds[0];
        m_FootstepSounds[0] = m_AudioSource.clip;
    }

    public void LandSound()
    {
        m_AudioSource.PlayOneShot(m_LandSound);
    }
}