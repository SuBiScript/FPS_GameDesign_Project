using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class CameraMover : MonoBehaviour
{
    private Vector2 SmoothVector;
    private Vector2 FinalVector;
    public GameObject Player;

    private bool AngleLocked;

    private Camera ControlledCamera;
    [Header("Limits")] public float Sensitivity = 1f;
    public float Smoothing = 3f;
    public float MinPitch = -80f;
    public float MaxPitch = 80f;
    public bool Inverted = false;

    [Header("Keys")] public InputAction MouseMovement;

    public Transform PitchTransform;

    [Header("Debug")] public bool LockOnStart = true;
    public InputAction DebugLockMouse;
    public InputAction DebugLockCamera;

    private void Awake()
    {
        AngleLocked = false;
        ControlledCamera = GetComponent<Camera>();
        SetupActions();
    }
    
    private void SetupActions()
    {
        MouseMovement.performed += MoveCamera;
        DebugLockCamera.performed += _ =>
        {
            AngleLocked = !AngleLocked;
        };
        DebugLockMouse.performed += _ =>
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked
                ? CursorLockMode.None
                : CursorLockMode.Locked;
        };
    }

    private void Start()
    {
        DefaultStart();
    }

    private void DefaultStart()
    {
        if (!LockOnStart)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        FinalVector = new Vector2(this.transform.rotation.eulerAngles.y, PitchTransform.transform.rotation.x);
    }
    
    private void MoveCamera(InputAction.CallbackContext context)
    {
        #if UNITY_EDITOR
        if (AngleLocked) return;
        #endif
        
        var DeltaMouse = context.ReadValue<Vector2>();

        DeltaMouse = Vector2.Scale(DeltaMouse,
            new Vector2(Sensitivity, Sensitivity));

        SmoothVector.x = Mathf.Lerp(SmoothVector.x, DeltaMouse.x, 1f / Smoothing);
        SmoothVector.y = Mathf.Lerp(SmoothVector.y, DeltaMouse.y, 1f / Smoothing);

        FinalVector += SmoothVector;

        FinalVector.y = Mathf.Clamp(FinalVector.y, MinPitch, MaxPitch);

        PitchTransform.localRotation = Quaternion.AngleAxis(FinalVector.y * (Inverted ? 1 : -1) , Vector3.right);
        Player.transform.localRotation = Quaternion.AngleAxis(FinalVector.x, Player.transform.up);
    }

    #region Enablers

    private void OnEnable()
    {
        MouseMovement.Enable();
        DebugLockCamera.Enable();
        DebugLockMouse.Enable();
    }

    private void OnDisable()
    {
        MouseMovement.Disable();
        DebugLockCamera.Disable();
        DebugLockMouse.Disable();
    }

    #endregion
}