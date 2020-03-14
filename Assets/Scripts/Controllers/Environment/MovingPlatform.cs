using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour, InterfaceCanParent
{

    public enum PlatformType
    {
        Standard,
        Trigger
    }
    
    public List<ObjectInformation> ParentInfos { get; set; } //Interface imported

    public PlatformType m_PlatformType;
    public List<Transform> m_PatrolPositions;
    public float m_MaxSpeed;
    public float m_stopBetweenSpots;
    public Vector3 movementDirection;

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
        ParentInfos = new List<ObjectInformation>();
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
            movementDirection = (m_PatrolPositions[m_CurrentPatrolPositionId].position - transform.position).normalized;
            m_RigidBody.MovePosition(transform.position + movementDirection * m_MaxSpeed * Time.fixedDeltaTime);

            //transform.position += ((l_Direction * m_MaxSpeed) * Time.fixedDeltaTime);
            //transform.position = Vector3.MoveTowards(transform.position, m_PatrolPositions[m_CurrentPatrolPositionId].position, m_MaxSpeed * Time.deltaTime);
        }
        else
        {
            movementDirection = Vector3.zero;
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
        try
        {
            IParentable parentableObject = other.gameObject.GetComponent<IParentable>();
            if (parentableObject == null) return;

            Transform collidedObject = parentableObject.ReturnSelf();

            if (collidedObject.position.y > this.gameObject.transform.position.y &&
                collidedObject.parent != this.gameObject.transform)
            {
                EmparentObject(parentableObject);
            }
        }
        catch (Exception e)
        {
            //None
            Debug.LogError(e.Message);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        try
        {
            IParentable parentableObject = other.gameObject.GetComponent<IParentable>();
            if (parentableObject == null) return;

            Transform collidedObject = parentableObject.ReturnSelf();
            DeparentObject(parentableObject);
        }
        catch (Exception e)

        {
            Debug.LogError(e.Message);
            //None
        }
    }

    public void EmparentObject(IParentable parentableObject)
    {
        ObjectInformation newObjectInfo;
        newObjectInfo.associatedGameObject = parentableObject.ReturnSelf().gameObject;
        newObjectInfo.oldParent = newObjectInfo.associatedGameObject.transform.parent;
        if (parentableObject.Emparent(this.gameObject))
        {
            ParentInfos.Add(newObjectInfo);
        }
        else return;

        //newObjectInfo.associatedGameObject.transform.parent = this.gameObject.transform;
    }

    public void DeparentObject(IParentable parentableObject)
    {
        foreach (ObjectInformation objectInformation in ParentInfos)
        {
            if (parentableObject.ReturnSelf().GetHashCode() ==
                objectInformation.associatedGameObject.transform.GetHashCode())
            {
                parentableObject.Deparent(objectInformation.oldParent);
                ParentInfos.Remove(objectInformation);
                return;
            }
        }
    }
}