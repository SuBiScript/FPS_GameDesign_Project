using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector2 m_MouseLook;
    Vector2 m_SmoothVector;
    GameObject m_Character;

    [Range(0.1f, 10.0f)] public float m_Sensitivity;
    [Range(0.1f, 10.0f)] public float m_Smoothing;
    [Range(-100.0f, 100.0f)] public float m_MinPitch;
    [Range(-100.0f, 100.0f)] public float m_MaxPitch;
    public Transform m_PitchControllerTransform;

    void Start()
    {
        m_Character = GameController.Instance.playerComponents.PlayerController.gameObject;
    }

    void Update()
    {
        if (!GameController.Instance.m_AngleLocked && !GameController.Instance.m_GamePaused && !GameController.Instance.m_PlayerDied)
            Aiming();
    }

    void Aiming()
    {
        Vector2 l_DeltaMouse = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        l_DeltaMouse = Vector2.Scale(l_DeltaMouse,
            new Vector2(m_Sensitivity * m_Smoothing, m_Sensitivity * m_Smoothing));

        m_SmoothVector.x = Mathf.Lerp(m_SmoothVector.x, l_DeltaMouse.x, 1f / m_Smoothing);
        m_SmoothVector.y = Mathf.Lerp(m_SmoothVector.y, l_DeltaMouse.y, 1f / m_Smoothing);

        m_MouseLook += m_SmoothVector;

        m_MouseLook.y = Mathf.Clamp(m_MouseLook.y, m_MinPitch, m_MaxPitch);

        m_PitchControllerTransform.localRotation = Quaternion.AngleAxis(-m_MouseLook.y, Vector3.right);
        m_Character.transform.localRotation = Quaternion.AngleAxis(m_MouseLook.x, m_Character.transform.up);
    }
}