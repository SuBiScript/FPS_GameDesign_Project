﻿using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Localization.Editor;
using Weapon;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    Vector3 m_Movement;
    Rigidbody m_RigidBody;
    CapsuleCollider m_Collider;
    float m_HAxis;
    float m_VAxis;
    private bool fire;
    public WeaponScript equippedWeapon;
    public Camera m_AttachedCamera;

    [Range(0.1f, 10.0f)] public float m_Speed;
    [Range(0.1f, 10.0f)] public float m_JumpForce;
    public PhysicMaterial m_ZeroFriction;
    public PhysicMaterial m_Friction;
    public LayerMask m_GroundLayers;
    private bool m_PanelJump;
    private int airFrames;

    void Start()
    {
        m_RigidBody = GetComponent<Rigidbody>();
        m_Collider = GetComponent<CapsuleCollider>();
        //m_AttachedCamera = GetComponent<Camera>(); //Debug.LogWarning("No camera found in PlayerController");
        if (equippedWeapon == null)
            equippedWeapon = GetComponent<WeaponScript>() ?? gameObject.AddComponent<WeaponScript>();
        equippedWeapon.Init(this);
        airFrames = 0;
    }

    void Update()
    {
        m_HAxis = Input.GetAxisRaw("Horizontal");
        m_VAxis = Input.GetAxisRaw("Vertical");
        m_Movement = new Vector3(m_HAxis, 0, m_VAxis);
        if (Input.GetMouseButtonDown(0))
        {
            equippedWeapon.MainFire();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            equippedWeapon.AltFire();
        }

        if (Input.GetButtonDown("Jump"))
            Jump();

        if (IsGrounded() && airFrames <= 0)
        {
            m_PanelJump = false;
        }
        else if(airFrames > 0)
        {
            airFrames -= 1;
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    public void PlatformJump()
    {
        m_PanelJump = true;
        airFrames = 5;
    }

    void Move()
    {
        if (!m_PanelJump)
            m_RigidBody.MovePosition(transform.position +
                                     Time.fixedDeltaTime * m_Speed * transform.TransformDirection(m_Movement));
    }

    void Jump()
    {
        if (IsGrounded() && !m_PanelJump)
        {
            m_RigidBody.AddForce(Vector3.up * m_JumpForce, ForceMode.Impulse);
        }
    }

    public bool IsGrounded()
    {
        return Physics.CheckCapsule(m_Collider.bounds.center,
            new Vector3(m_Collider.bounds.center.x, m_Collider.bounds.min.y, m_Collider.center.z),
            m_Collider.radius * 0.1f, m_GroundLayers);
    }
}