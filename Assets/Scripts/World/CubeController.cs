using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    float m_Timer = 0;
    Vector3 m_LastPosition = Vector3.zero;
    public float m_Speed;

    void Start()
    {
        m_Timer = 0.4f;
    }

    void FixedUpdate()
    {
        m_Speed = (transform.position - m_LastPosition).magnitude;
        m_LastPosition = transform.position;

        if (m_Timer < 0)
        {
            m_Timer = 0.4f;
        }
        else
            m_Timer -= Time.deltaTime;
    }
}
