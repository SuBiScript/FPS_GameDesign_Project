using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{

    public enum PlatformType
    {
        Standard,
        Trigger
    }

    public PlatformType m_PlatformType;
    public List<Transform> m_PatrolPositions;
    public float m_MaxSpeed;
    public float m_stopBetweenSpots;

    int m_CurrentPatrolPositionId;
    float m_CurrentTime;
    bool m_PathComplete;
    bool m_MoveToNextPatrolPosition;
    bool m_PlatformTriggered;
    bool m_AvoidPathFinding;
    Rigidbody m_RigidBody;

    void Start()
    {
        m_CurrentTime = m_stopBetweenSpots;
        m_PathComplete = true;
        m_MoveToNextPatrolPosition = false;
        m_CurrentPatrolPositionId = 0;
        m_PlatformTriggered = false;
        m_AvoidPathFinding = false;
        m_RigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        switch (m_PlatformType)
        {
            case PlatformType.Standard:
                if (m_PathComplete)
                {
                    if (m_CurrentTime <= 0)
                        MoveToNextPatrolPosition();
                    else
                        m_CurrentTime -= Time.deltaTime;
                }
                else
                {
                    if (PathComplete())
                        m_PathComplete = true;
                }
                break;
            default:
                if (m_PathComplete && m_PlatformTriggered)
                {
                    if (m_CurrentTime <= 0)
                        MoveToNextPatrolPosition();
                    else
                        m_CurrentTime -= Time.deltaTime;
                }
                else
                {
                    if (m_AvoidPathFinding)
                        if (PathComplete())
                            m_PathComplete = true;
                }
                break;
        }
    }

    void FixedUpdate()
    {
        if (m_MoveToNextPatrolPosition)
        {
            Vector3 l_Direction = (m_PatrolPositions[m_CurrentPatrolPositionId].position - transform.position).normalized;
            m_RigidBody.MovePosition(transform.position + l_Direction * m_MaxSpeed * Time.fixedDeltaTime);

            //transform.position += ((l_Direction * m_MaxSpeed) * Time.fixedDeltaTime);
            //transform.position = Vector3.MoveTowards(transform.position, m_PatrolPositions[m_CurrentPatrolPositionId].position, m_MaxSpeed * Time.deltaTime);
        }
    }

    void MoveToNextPatrolPosition()
    {
        m_CurrentTime = m_stopBetweenSpots;
        m_PathComplete = false;
        ++m_CurrentPatrolPositionId;
        if (m_CurrentPatrolPositionId >= m_PatrolPositions.Count)
            m_CurrentPatrolPositionId = 0;

        m_MoveToNextPatrolPosition = true;
    }

    bool PathComplete()
    {
        if (Vector3.Distance(transform.position, m_PatrolPositions[m_CurrentPatrolPositionId].position) < 0.1f)
        {
            m_MoveToNextPatrolPosition = false;
            return true;
        }
        else
            return false;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.transform.position.y > this.gameObject.transform.position.y &&
            other.gameObject.transform.parent != this.gameObject.transform)
        {
            other.gameObject.transform.parent = this.gameObject.transform;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.transform.parent == this.gameObject.transform)
        {
            other.gameObject.transform.parent = null;
        }
    }


    /*void OnTriggerEnter(Collider col)
    {
        if (col.gameObject == this.gameObject) return;

        if (col.tag == "Player")
        {
            GameController.Instance.playerComponents.PlayerController.transform.parent = transform;
        }

        if (m_PlatformType == PlatformType.Trigger)
        {
            m_PlatformTriggered = true;
            m_AvoidPathFinding = true;
        }

    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject == this.gameObject) return;
        if (col.tag == "Player")
            GameController.Instance.playerComponents.PlayerController.transform.parent = null;

        if (m_PlatformType == PlatformType.Trigger)
        {
            m_MoveToNextPatrolPosition = false;
            m_PathComplete = true;
            m_AvoidPathFinding = false;
            m_CurrentPatrolPositionId = -1;
        }
    }*/

}
