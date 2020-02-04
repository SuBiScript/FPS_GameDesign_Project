using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPanelController : MonoBehaviour
{
    float m_JumpForce = 10.0f;

    void Start()
    {

    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider _Collider)
    {
        if (_Collider.tag == "Player")
        {
            GameController.Instance.m_PlayerController.m_PanelJump = true;

            if (!GameController.Instance.m_PlayerController.IsGrounded())
                m_JumpForce = 12.5f;
            else
                m_JumpForce = 10;
        }
        else if (_Collider.tag == "Cube")
            m_JumpForce = 25;

        _Collider.GetComponent<Rigidbody>().AddForce(transform.up * m_JumpForce, ForceMode.Impulse);
    }
}
