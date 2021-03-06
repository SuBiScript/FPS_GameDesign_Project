﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraFSMController : MonoBehaviour
{
    Vector2 m_MouseLook;
    Vector2 m_SmoothVector;
    GameObject m_Character;

    public KeyCode m_DebugLockAngleKeyCode = KeyCode.I;
    public KeyCode m_DebugLockKeyCode = KeyCode.O;
    private bool m_AngleLocked;
    public Camera attachedCamera { get; private set; }

    [Range(0.1f, 10.0f)] public float m_Sensitivity = 1f;
    [Range(0.1f, 10.0f)] public float m_Smoothing = 3f;
    [Range(-100.0f, 100.0f)] public float m_MinPitch = -80f;
    [Range(-100.0f, 100.0f)] public float m_MaxPitch = 70f;
    public Transform m_PitchControllerTransform;

    void Awake()
    {
        m_AngleLocked = false;
        m_Character = GetComponentInParent<PlayerControllerFSM>().gameObject;
        attachedCamera = GetComponent<Camera>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        //if (!m_AngleLocked)
        //    Aiming();

        if (!m_AngleLocked && !GameController.Instance.m_GamePaused && !GameController.Instance.m_PlayerDied && GameController.Instance.m_IntroFinished)
            Aiming();

#if UNITY_EDITOR
        LockCameraAndMouse();
#endif
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

    /// <summary>
    /// Unity editor stuff
    /// </summary>
#if UNITY_EDITOR
    void LockCameraAndMouse()
    {
        if (Input.GetKeyDown(m_DebugLockAngleKeyCode))
            m_AngleLocked = !m_AngleLocked;

        if (Input.GetKeyDown(m_DebugLockKeyCode))
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
    }
#endif
}