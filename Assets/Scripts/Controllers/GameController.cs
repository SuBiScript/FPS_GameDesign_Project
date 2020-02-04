using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameController : Singleton<GameController>
{
    [Header("Required attributes")]
    public PlayerController m_PlayerController;


    KeyCode m_DebugLockAngleKeyCode = KeyCode.I;
    KeyCode m_DebugLockKeyCode = KeyCode.O;
    [HideInInspector] public bool m_AngleLocked;
    [HideInInspector] public bool m_GamePaused;
    [HideInInspector] public bool m_PlayerDied;

    void Start()
    {
        m_GamePaused = false;
        m_PlayerDied = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {


#if UNITY_EDITOR
        if (Input.GetKeyDown(m_DebugLockAngleKeyCode))
            m_AngleLocked = !m_AngleLocked;

        if (Input.GetKeyDown(m_DebugLockKeyCode))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
        }
#endif
    }
}