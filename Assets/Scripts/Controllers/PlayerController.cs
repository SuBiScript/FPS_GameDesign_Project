using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Vector3 m_Movement;
    Rigidbody m_RigidBody;
    CapsuleCollider m_Collider;
    float m_HAxis;
    float m_VAxis;


    [Range(0.1f, 10.0f)] public float m_Speed;
    [Range(0.1f, 10.0f)] public float m_JumpForce;
    public PhysicMaterial m_ZeroFriction;
    public PhysicMaterial m_Friction;
    public LayerMask m_GroundLayers;
    [HideInInspector] public bool m_PanelJump;


    void Start()
    {
        m_RigidBody = GetComponent<Rigidbody>();
        m_Collider = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        m_HAxis = Input.GetAxisRaw("Horizontal");
        m_VAxis = Input.GetAxisRaw("Vertical");
        m_Movement = new Vector3(m_HAxis, 0, m_VAxis);

        Jump();

        if (IsGrounded())
        {
            m_Collider.material = m_Friction;
            m_PanelJump = false;
        }
        else
            m_Collider.material = m_ZeroFriction;
    }

    void FixedUpdate()
    {
        if (!m_PanelJump)
            Move();
    }

    void Move()
    {
        m_RigidBody.MovePosition(transform.position + Time.fixedDeltaTime * m_Speed * transform.TransformDirection(m_Movement));
    }

    void Jump()
    {
        if (IsGrounded() && !m_PanelJump && Input.GetButtonDown("Jump"))
        {
            m_RigidBody.AddForce(Vector3.up * m_JumpForce, ForceMode.Impulse);
        }
    }

    public bool IsGrounded()
    {
        return Physics.CheckCapsule(m_Collider.bounds.center, new Vector3(m_Collider.bounds.center.x, m_Collider.bounds.min.y, m_Collider.center.z), m_Collider.radius * .9f, m_GroundLayers);
    }
}
