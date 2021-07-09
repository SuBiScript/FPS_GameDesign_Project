using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Platform_Version2 : MonoBehaviour, IPlayerCollide
{
    public List<Transform> m_PatrolPositions;
    public float m_MaxSpeed;
    public float m_stopBetweenSpots;

    [ReadOnly] private Vector3 movementDirection;
    public Mesh Mesh;

    int m_CurrentPatrolPositionId;
    float m_CurrentTime;
    bool m_PathComplete;
    Rigidbody m_RigidBody;
    private PlayerMover player;

    private MeshCollider _meshCollider;
    private Plane platformPlane;

    private void Awake()
    {
        _meshCollider = GetComponent<MeshCollider>();
        m_RigidBody = GetComponent<Rigidbody>();
        m_RigidBody.Sleep();

        CreatePlane();
    }

    private void CreatePlane()
    {
        var transform1 = transform;
        var up = transform1.up;
        platformPlane = new Plane(up, _meshCollider.ClosestPointOnBounds(transform1.position + up));
    }

    void Start()
    {
        m_CurrentPatrolPositionId = 0;
        m_CurrentTime = m_stopBetweenSpots;
        m_PathComplete = true;
    }

    void Update()
    {
        if (m_PathComplete)
        {
            movementDirection = Vector3.zero;

            if (m_CurrentTime <= 0)
                MoveToNextPatrolPosition();
            else
                m_CurrentTime -= Time.deltaTime;
        }
        else
        {
            m_PathComplete =
                Vector3.Distance(transform.position, m_PatrolPositions[m_CurrentPatrolPositionId].position) < 0.1f;
        }
    }

    void FixedUpdate()
    {
        if (!m_PathComplete)
        {
            m_RigidBody.MovePosition(transform.position + movementDirection * (m_MaxSpeed * Time.fixedDeltaTime));
        }
    }

    void MoveToNextPatrolPosition()
    {
        m_CurrentTime = m_stopBetweenSpots;
        m_PathComplete = false;

        m_CurrentPatrolPositionId++;
        if (m_CurrentPatrolPositionId >= m_PatrolPositions.Count)
            m_CurrentPatrolPositionId = 0;

        movementDirection = (m_PatrolPositions[m_CurrentPatrolPositionId].position - transform.position).normalized;
    }

    #region Gizmos

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        for (int i = 0; i < m_PatrolPositions.Count; i++)
        {
            Gizmos.DrawLine(this.gameObject.transform.position, m_PatrolPositions[i].transform.position);
            Gizmos.DrawWireMesh(Mesh, m_PatrolPositions[i].transform.position, Quaternion.identity,
                transform.localScale);
        }
    }
#endif

    #endregion

    #region Interfaces

    public bool Collide(GameObject self, Vector3 collisionPoint)
    {
        if (!player) player = self.GetComponent<PlayerMover>();
        if (platformPlane.GetSide(collisionPoint))
            player.SetInertia(movementDirection * m_MaxSpeed);
        return true;
    }

    #endregion
}