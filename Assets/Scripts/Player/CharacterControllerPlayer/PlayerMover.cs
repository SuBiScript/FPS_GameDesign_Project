using System;
using System.Diagnostics;
using Interfaces;
using Panels;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;
using Math = System.Math;

[RequireComponent(typeof(UnityEngine.CharacterController))]
public class PlayerMover : MonoBehaviour, IBoostable
{
    [Title("Properties")] [Tooltip("Instantiate properties.")]
    public bool InstantiateProperties = true;

    public PlayerProperties Properties;

    private Vector3 Velocity;
    private Vector3 Inertia;
    private bool ResetInertiaOnGround;

    public UnityEngine.CharacterController Controller;

    #region DebugZone

    #endregion

    #region Inputs

    // https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/Interactions.html#multiple-controls-on-an-action

    #endregion

    #region Limiters

    private float LastWalkingSpeed;
    private float LastGravitySpeed;
    private float LastJumpingSpeed;
    private float LastXCameraRotation;

    #endregion

    private void Awake()
    {
        Controller = GetComponent<UnityEngine.CharacterController>();
        Properties = InstantiateProperties ? Instantiate(Properties) : Properties;
    }

    void Start()
    {
        Velocity = Vector3.zero;
    }

    private void Update()
    {
        ApplyVerticalVelocity();
    }

    private void FixedUpdate()
    {
        MoveCharacter();
    }

    void ApplyVerticalVelocity()
    {
        // Check if grounded. If it is, then adjust the position to stick the player to the ground.
        // If it is not grounded, then apply the gravity speed. UNLESS THERE MIGHT BE A DIFFERENT STATUS OR PROPULSED STATUS.
        if (Controller.isGrounded)
        {
            // Do not apply gravity. Stick it to the ground.
            // Velocity.y = 0f;
            if (Inertia.magnitude > 0f && ResetInertiaOnGround)
            {
                Inertia = Vector3.zero; // Resets the inertia from the flying position when touching the ground.
                ResetInertiaOnGround = false;
            }

            if (Velocity.y < -0.5f)
                Velocity.y = 0f;
        }
        else
        {
            //LastGravitySpeed = LimitValue(Properties.GravitySpeed * Time.deltaTime, LastGravitySpeed, 1.5f);
            Velocity.y -= Properties.GravitySpeed * Time.deltaTime;

            //Velocity.x += (1f - floorNormal.y) * floorNormal.x * (1f - slideFriction);
            //Velocity.z += (1f - floorNormal.y) * floorNormal.z * (1f - slideFriction);

            if (!ResetInertiaOnGround) ResetInertiaOnGround = true;
        }
    }

    void LimitVelocity()
    {
        if (Controller.isGrounded)
        {
            Velocity.x = Mathf.Clamp(Velocity.x, -Properties.MaxWalkingSpeed, Properties.MaxWalkingSpeed);
            Velocity.z = Mathf.Clamp(Velocity.z, -Properties.MaxWalkingSpeed, Properties.MaxWalkingSpeed);
        }

        if (Properties.MaxGravity >= 0f)
            Velocity.y = Mathf.Max(Velocity.y, -Properties.MaxGravity);
    }

    void MoveCharacter()
    {
        LimitVelocity();
        Vector3 finalVelocity = (transform.TransformDirection(Velocity) + Inertia) * Time.deltaTime;
        Controller.Move(finalVelocity);
    }

    #region Public Methods

    public void SetVelocity(Vector2 Velocity)
    {
        // Stuff for movement with platforms.
    }

    public void SetInertia(Vector3 Inertia)
    {
        this.Inertia = Inertia;
        ResetInertiaOnGround = false;
    }

    #endregion

    #region Limiters

    private float LimitValue(float OriginalValue, float ComparisonValue, float Threshold = 1.2f)
    {
        // idea here is to divide the original value by the comparison one. 
        // If it is superior than the threshold then return comparison.
        if (Math.Abs(ComparisonValue) < 0.0001f) return OriginalValue;
        return (OriginalValue / ComparisonValue) > Threshold ? ComparisonValue : OriginalValue;
    }

    #endregion

    #region Callbacks

    public void Jump(InputAction.CallbackContext context)
    {
        // Debug.Log("Jumping input detected.");
        // Force remove from ground.
        if (Controller.isGrounded && context.performed)
        {
            //LastJumpingSpeed = LimitValue(Properties.JumpingSpeed * Time.deltaTime, LastJumpingSpeed);
            Velocity.y = Properties.JumpingSpeed;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        var movement = context.ReadValue<Vector2>();
        //Debug.Log(movement);
        float WalkingSpeed = Properties.WalkingSpeed;
        //LastWalkingSpeed = LimitValue(Properties.WalkingSpeed * Time.deltaTime, LastWalkingSpeed, 1.1f);
        Velocity.x = movement.x * WalkingSpeed;
        Velocity.z = movement.y * WalkingSpeed;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // TODO Check here if the object is "Collidable" so we can treat collisions with the CharacterController.
        try
        {
            IPlayerCollide go = hit.gameObject.GetComponent<IPlayerCollide>();
            go.Collide(gameObject, gameObject.transform.position + Vector3.down * (Controller.height * 0.5f));
            //Debug.Log("Player detected collision with " + hit.gameObject.name);
        }
        catch (Exception ex)

        {
            // Ignored
        }
    }

    #endregion

    #region Getters

    #endregion

    #region Enablers

    #endregion

    #region Debug

#if UNITY_EDITOR

    private void OnGUI()
    {
        EditorGUILayout.Vector3Field("Velocity", Velocity);
        EditorGUILayout.Vector3Field("Inertia", Inertia);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(this.gameObject.transform.position + Vector3.down * (Controller.height * 0.5f), 0.2f);
    }
#endif

    #endregion

    #region Interfaces

    public void Boost(Vector3 Force)
    {
        SetInertia(Force);
    }

    #endregion
}